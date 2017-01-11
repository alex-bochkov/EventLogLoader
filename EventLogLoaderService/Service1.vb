Imports System.Threading
Imports System.Globalization
Imports System.Data.SqlClient
Imports System.IO
Imports MySql.Data.MySqlClient
Imports Nest
Imports Newtonsoft.Json

Public Class EventLogLoaderService

    Dim ListOfProcessors As List(Of EventLogProcessor) = New List(Of EventLogProcessor)
    Dim ArrayThread() As Threading.Thread
    Dim ConfigSettingObj As ConfigSetting = New ConfigSetting

    Dim ConnectionString As String
    Dim DBType As String
    Dim ItIsMSSQL As Boolean = False
    Dim ItIsMySQL As Boolean = False
    Dim ItIsES As Boolean = False

    Public SleepTime As Integer = 60 * 1000 '1 минута

    Function LoadConfigSetting()

        LoadConfigSetting = False

        Try

            Dim PathConfigFile = Path.Combine(My.Application.Info.DirectoryPath, "config.json")
            If My.Computer.FileSystem.FileExists(PathConfigFile) Then

                ConfigSettingObj = ConfigSettingsModule.LoadConfigSettingFromFile(PathConfigFile)

                ConnectionString = ConfigSettingObj.ConnectionString

                DBType = ConfigSettingObj.DBType
                If DBType = "MySQL" Then
                    ItIsMySQL = True
                ElseIf DBType = "MS SQL Server" Then
                    ItIsMSSQL = True
                ElseIf DBType = "ElasticSearch" Then
                    ItIsES = True
                End If

                Dim s = ConfigSettingObj.RepeatTime
                SleepTime = s * 1000

                For Each IBConfig In ConfigSettingObj.Infobases

                    Dim IB_ID = IBConfig.DatabaseID
                    Dim IB_Name = IBConfig.DatabaseName
                    Dim IB_Catalog = IBConfig.DatabaseCatalog

                    Dim EventLogProcessorObj = New EventLogProcessor
                    EventLogProcessorObj.Log = NLog.LogManager.GetLogger("CurrentThread")
                    EventLogProcessorObj.InfobaseGuid = IB_ID
                    EventLogProcessorObj.InfobaseName = IB_Name
                    EventLogProcessorObj.Catalog = IB_Catalog
                    EventLogProcessorObj.ConnectionString = ConnectionString
                    EventLogProcessorObj.SleepTime = SleepTime
                    EventLogProcessorObj.ItIsMSSQL = ItIsMSSQL
                    EventLogProcessorObj.ItIsMySQL = ItIsMySQL
                    EventLogProcessorObj.ItIsES = ItIsES
                    EventLogProcessorObj.ESIndexName = ConfigSettingObj.ESIndexName
                    EventLogProcessorObj.ESServerName = IBConfig.ESServerName

                    ListOfProcessors.Add(EventLogProcessorObj)

                Next

                LoadConfigSetting = True
            Else
                Log.Error("File config.json was not found!")
            End If
        Catch ex As Exception
            Log.Error(ex, "Parameters cannot be load from config.json file (it may be corrupted)")
        End Try


    End Function

    Private WorkerThread As Thread
    Private Log As NLog.Logger

    Protected Overrides Sub OnStart(ByVal args() As String)
        ' Добавьте здесь код запуска службы. Этот метод должен настроить все необходимое
        ' для успешной работы службы.
        SubStart()
    End Sub

    Protected Overrides Sub OnStop()
        ' Добавьте здесь код для завершающих операций перед остановкой службы.
        SubStop()
    End Sub

    Public Sub SubStart()
        WorkerThread.Start()
    End Sub

    Public Sub SubStop()
        WorkerThread.Abort()
    End Sub

    Public Sub New()

        ' Этот вызов является обязательным для конструктора.
        InitializeComponent()

        ' Добавьте все инициализирующие действия после вызова InitializeComponent().

        'Нужно для того, что служба создавала логи в своем каталоге
        Directory.SetCurrentDirectory(My.Application.Info.DirectoryPath)

        Log = NLog.LogManager.GetLogger("CurrentThread")
        'Log.Info("Создание объекта сервиса")

        WorkerThread = New Threading.Thread(AddressOf DoWork)
        WorkerThread.SetApartmentState(Threading.ApartmentState.STA)


    End Sub

    Private Sub DoWork()

        'Log.Info("Запуск основного функционала")

        If Not LoadConfigSetting() Then
            Log.Error("Error while working with config.json file in application folder")
            Environment.Exit(-1)
        End If

        CreateTables()

        Dim i = 0

        For Each IB In ListOfProcessors

            Try
                IB.GetInfobaseIDFromDatabase()
            Catch ex As Exception
                Log.Error(ex, "Error occurred while getting infobase ID from target database (" + IB.InfobaseName + ")")
                Continue For
            End Try

            Try
                Dim Thead = New Threading.Thread(New ParameterizedThreadStart(AddressOf IB.DoWork))
                Thead.IsBackground = True
                Thead.Start()

                ReDim Preserve ArrayThread(i)

                ArrayThread(i) = Thead

                i = i + 1
            Catch ex As Exception
                Log.Error(ex, "Error occurred while starting new thread (" + IB.InfobaseName + ")")
            End Try

        Next

    End Sub


    Sub CreateTables()

        Try
            If ItIsMSSQL Then

                Dim objConn As New SqlConnection(ConnectionString)
                objConn.Open()

                Dim command As New SqlCommand("IF NOT EXISTS (select * from sysobjects where id = object_id(N'Events'))
                                                BEGIN
                                                  CREATE TABLE [dbo].[Events]([InfobaseCode] int Not NULL, [DateTime] [datetime] Not NULL, 
                                                        [TransactionStatus] [varchar](1) NULL,	
                                                        [TransactionStartTime] [datetime] NULL, 
                                              		    [TransactionMark] bigint NULL,	
                                                        [Transaction] [varchar](100) NULL,	
                                              		    [UserName] int NULL,	
                                              	        [ComputerName] int NULL,	
                                              		    [AppName] Int NULL, 
                                                        [Field2] [varchar](100) NULL,	
                                                        [EventID] int NULL, 
                                              		    [EventType] [varchar](1) NULL,	
                                                        [Comment] [nvarchar](max) NULL,	
                                              		    [MetadataID] int NULL,	
                                              		    [DataStructure] [nvarchar](max) NULL, 
                                              		    [DataString] [nvarchar](max) NULL,	
                                                        [ServerID] int NULL, 
                                              		    [MainPortID] int NULL,	
                                                        [SecondPortID] int NULL, 
                                              		    [Seance] int NULL,	
                                                        [Field7] [varchar](100) NULL,	
                                              		    [Field8] [varchar](100) NULL);
                                                    CREATE CLUSTERED INDEX [CIX_Events] ON [dbo].[Events]([InfobaseCode], [DateTime])
                                                END", objConn)
                command.ExecuteNonQuery()

                '**********************************************************************************
                'command.CommandText = "IF NOT EXISTS (select * from sysobjects where id = object_id(N'Params'))" + vbNewLine + _
                '                    "	CREATE TABLE [dbo].[Params] ([InfobaseCode] int NOT NULL, [Position] bigint, [Filename] [char](100), [LastEventID] int);" + vbNewLine + _
                '                    " IF NOT EXISTS (select * from sys.indexes where object_id = object_id(N'Params') AND Name = 'ClusteredIndex')" + vbNewLine + _
                '                    " CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex] ON [dbo].[Params] ([InfobaseCode] ASC);"
                'command.ExecuteNonQuery()
                '**********************************************************************************
                command.CommandText = "IF NOT EXISTS (select * from sysobjects where id = object_id(N'Infobases'))" + vbNewLine +
                                    "	CREATE TABLE [dbo].[Infobases] ([Guid] [char](40) NOT NULL, [Code] int NOT NULL, [Name] [char](100))" + vbNewLine +
                                    " IF NOT EXISTS (select * from sys.indexes where object_id = object_id(N'Infobases') AND Name = 'ClusteredIndex')" + vbNewLine +
                                    " CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex] ON [dbo].[Infobases] ([Guid] ASC);"
                command.ExecuteNonQuery()
                '**********************************************************************************
                command.CommandText =
                    "IF NOT EXISTS (select * from sysobjects where id = object_id(N'Users'))" + vbNewLine +
                    "CREATE TABLE [dbo].[Users]([InfobaseCode] int NOT NULL, [Code] int NOT NULL, [Name] [nvarchar](100), [Guid] [varchar](40));" + vbNewLine +
                    "IF NOT EXISTS (select * from sys.indexes where object_id = object_id(N'Users') AND Name = 'ClusteredIndex')" + vbNewLine +
                    "CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex] ON [dbo].[Users] ([InfobaseCode] ASC, [Code] ASC);" + vbNewLine +
                    "" +
                    "IF NOT EXISTS (select * from sysobjects where id = object_id(N'Metadata'))" + vbNewLine +
                    "CREATE TABLE [dbo].[Metadata]([InfobaseCode] int NOT NULL, [Code] int NOT NULL, [Name] [nvarchar](100), [Guid] [varchar](40));" + vbNewLine +
                    "IF NOT EXISTS (select * from sys.indexes where object_id = object_id(N'Metadata') AND Name = 'ClusteredIndex')" + vbNewLine +
                    "CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex] ON [dbo].[Metadata] ([InfobaseCode] ASC, [Code] ASC);" + vbNewLine +
                    "" +
                    "IF NOT EXISTS (select * from sysobjects where id = object_id(N'Computers'))" + vbNewLine +
                    "CREATE TABLE [dbo].[Computers]([InfobaseCode] int NOT NULL, [Code] int NOT NULL, [Name] [nvarchar](100));" + vbNewLine +
                    "IF NOT EXISTS (select * from sys.indexes where object_id = object_id(N'Computers') AND Name = 'ClusteredIndex')" + vbNewLine +
                    "CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex] ON [dbo].[Computers] ([InfobaseCode] ASC, [Code] ASC);" + vbNewLine +
                    "" +
                    "IF NOT EXISTS (select * from sysobjects where id = object_id(N'Applications'))" + vbNewLine +
                    "CREATE TABLE [dbo].[Applications]([InfobaseCode] int NOT NULL, [Code] int NOT NULL,[Name] [nvarchar](100));" + vbNewLine +
                    "IF NOT EXISTS (select * from sys.indexes where object_id = object_id(N'Applications') AND Name = 'ClusteredIndex')" + vbNewLine +
                    "CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex] ON [dbo].[Applications] ([InfobaseCode] ASC, [Code] ASC);" + vbNewLine +
                    "" +
                    "IF NOT EXISTS (select * from sysobjects where id = object_id(N'EventsType'))" + vbNewLine +
                    "CREATE TABLE [dbo].[EventsType]([InfobaseCode] int NOT NULL, [Code] int NOT NULL, [Name] [nvarchar](max));" + vbNewLine +
                    "IF NOT EXISTS (select * from sys.indexes where object_id = object_id(N'EventsType') AND Name = 'ClusteredIndex')" + vbNewLine +
                    "CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex] ON [dbo].[EventsType] ([InfobaseCode] ASC, [Code] ASC);" + vbNewLine +
                    "" +
                    "IF NOT EXISTS (select * from sysobjects where id = object_id(N'Servers'))" + vbNewLine +
                    "CREATE TABLE [dbo].[Servers]([InfobaseCode] int NOT NULL, [Code] int NOT NULL, [Name] [nvarchar](100));" + vbNewLine +
                    "IF NOT EXISTS (select * from sys.indexes where object_id = object_id(N'Servers') AND Name = 'ClusteredIndex')" + vbNewLine +
                    "CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex] ON [dbo].[Servers] ([InfobaseCode] ASC, [Code] ASC);" + vbNewLine +
                    "" +
                    "IF NOT EXISTS (select * from sysobjects where id = object_id(N'MainPorts'))" + vbNewLine +
                    "CREATE TABLE [dbo].[MainPorts]([InfobaseCode] int NOT NULL, [Code] int NOT NULL, [Name] [nvarchar](100));" + vbNewLine +
                    "IF NOT EXISTS (select * from sys.indexes where object_id = object_id(N'MainPorts') AND Name = 'ClusteredIndex')" + vbNewLine +
                    "CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex] ON [dbo].[MainPorts] ([InfobaseCode] ASC, [Code] ASC);" + vbNewLine +
                    "" +
                    "IF NOT EXISTS (select * from sysobjects where id = object_id(N'SecondPorts'))" + vbNewLine +
                    "CREATE TABLE [dbo].[SecondPorts]([InfobaseCode] int NOT NULL, [Code] int NOT NULL, [Name] [nvarchar](100));" + vbNewLine +
                    "IF NOT EXISTS (select * from sys.indexes where object_id = object_id(N'SecondPorts') AND Name = 'ClusteredIndex')" + vbNewLine +
                    "CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex] ON [dbo].[SecondPorts] ([InfobaseCode] ASC, [Code] ASC);"

                command.ExecuteNonQuery()
                '**********************************************************************************

                command.CommandText = "SELECT TOP 1 * FROM Events"
                command.ExecuteReader()

                command.Dispose()
                objConn.Close()
                objConn.Dispose()

            ElseIf ItIsMySQL Then

                Dim objConn = New MySql.Data.MySqlClient.MySqlConnection(ConnectionString)
                objConn.Open()

                Dim DBName = objConn.Database

                Dim command = New MySql.Data.MySqlClient.MySqlCommand
                command.Connection = objConn

                command.CommandText = "CREATE TABLE IF NOT EXISTS `Events` (`InfobaseCode` int(11) NOT NULL, `DateTime` int(11) NOT NULL," +
                    "`TransactionStatus` varchar(1) NULL, `TransactionStartTime` datetime NULL,	" +
                    "`TransactionMark` bigint NULL, `Transaction` varchar(100) NULL,	`UserName` int(11) NULL, `ComputerName` int(11) NULL,	" +
                    "`AppName` int(11) NULL, `Field2` varchar(100) NULL,	`EventID` int(11) NULL, `EventType` varchar(1) NULL,	" +
                    "`Comment` text NULL, `MetadataID` int(11) NULL,	`DataStructure` text NULL, `DataString` text NULL,	" +
                    "`ServerID` int(11) NULL, `MainPortID` int(11) NULL,	`SecondPortID` int(11) NULL, `Seance` int(11) NULL,	" +
                    "`Field7` varchar(100) NULL, `Field8` varchar(100) NULL" +
                    ") ENGINE=InnoDB DEFAULT CHARSET=utf8;"
                command.ExecuteNonQuery()
                '**********************************************************************************
                'command.CommandText = "CREATE TABLE IF NOT EXISTS `Params` (`InfobaseCode` int(11) NOT NULL, `Position` bigint, `Filename` varchar(100)," + _
                '                        "`LastEventID` int(11), PRIMARY KEY `InfobaseCode` (`InfobaseCode`));"
                'command.ExecuteNonQuery()
                '**********************************************************************************
                command.CommandText = "CREATE TABLE IF NOT EXISTS `Infobases` (`Guid` varchar(40) NOT NULL, `Code` int(11) NOT NULL, `Name` varchar(100)," +
                                        "PRIMARY KEY `Guid` (`Guid`));"
                command.ExecuteNonQuery()
                '**********************************************************************************
                command.CommandText =
                    "CREATE TABLE IF NOT EXISTS `Users`(`InfobaseCode` int(11) NOT NULL, `Code` int(11) NOT NULL, `Name` varchar(100), `Guid` varchar(40), PRIMARY KEY (`InfobaseCode`, `Code`));" + vbNewLine +
                    "" +
                    "CREATE TABLE IF NOT EXISTS `Metadata`(`InfobaseCode` int(11) NOT NULL, `Code` int(11) NOT NULL, `Name` varchar(100), `Guid` varchar(40), PRIMARY KEY (`InfobaseCode`, `Code`));" + vbNewLine +
                    "" +
                    "CREATE TABLE IF NOT EXISTS `Computers`(`InfobaseCode` int(11) NOT NULL, `Code` int(11) NOT NULL, `Name` varchar(100), PRIMARY KEY (`InfobaseCode`, `Code`));" + vbNewLine +
                    "" +
                    "CREATE TABLE IF NOT EXISTS `Applications`(`InfobaseCode` int(11) NOT NULL, `Code` int(11) NOT NULL, `Name` varchar(100), PRIMARY KEY (`InfobaseCode`, `Code`));" + vbNewLine +
                    "" +
                    "CREATE TABLE IF NOT EXISTS `EventsType`(`InfobaseCode` int(11) NOT NULL, `Code` int(11) NOT NULL, `Name` text, PRIMARY KEY (`InfobaseCode`, `Code`));" + vbNewLine +
                    "" +
                    "CREATE TABLE IF NOT EXISTS `Servers`(`InfobaseCode` int(11) NOT NULL, `Code` int(11) NOT NULL, `Name` varchar(100), PRIMARY KEY (`InfobaseCode`, `Code`));" + vbNewLine +
                    "" +
                    "CREATE TABLE IF NOT EXISTS `MainPorts`(`InfobaseCode` int(11) NOT NULL, `Code` int(11) NOT NULL, `Name` varchar(100), PRIMARY KEY (`InfobaseCode`, `Code`));" + vbNewLine +
                    "" +
                    "CREATE TABLE IF NOT EXISTS `SecondPorts`(`InfobaseCode` int(11) NOT NULL, `Code` int(11) NOT NULL, `Name` varchar(100), PRIMARY KEY (`InfobaseCode`, `Code`));"

                command.ExecuteNonQuery()
                '**********************************************************************************



                command.Dispose()
                objConn.Close()
                objConn.Dispose()

            End If

            Log.Info("Target database tables have been verified!")

        Catch ex As Exception

            Log.Error(ex, "Error occurred while during target database tables verification")

        End Try

    End Sub


End Class
