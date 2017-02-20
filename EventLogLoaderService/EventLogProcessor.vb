Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports MySql.Data.MySqlClient
Imports Nest
Imports Newtonsoft.Json

Public Class EventLogProcessor
#Region "Reference classes"
    Structure User
        Dim Code As Integer
        Dim Name As String
        Dim Guid As String
    End Structure

    Structure Metadata
        Dim Code As Integer
        Dim Name As String
        Dim Guid As String
    End Structure

    Structure Computer
        Dim Code As Integer
        Dim Name As String
    End Structure

    Structure Application
        Dim Code As Integer
        Dim Name As String
    End Structure

    Structure EventType
        Dim Code As Integer
        Dim Name As String
    End Structure

    Structure Server
        Dim Code As Integer
        Dim Name As String
    End Structure

    Structure MainPort
        Dim Code As Integer
        Dim Name As String
    End Structure

    Structure SecondPort
        Dim Code As Integer
        Dim Name As String
    End Structure

    Sub AddUser(Code As Integer, Guid As String, Name As String)

        Dim Usr = New User
        Usr.Code = Code
        Usr.Name = Name
        Usr.Guid = Guid

        'reference file in old evenlog format may contain duplicated values when object description has been changed (for example, user renamed)
        If DictUsers.ContainsKey(Code) Then
            DictUsers.Remove(Code)
        End If

        DictUsers.Add(Code, Usr)

    End Sub

    Sub AddComputer(Code As Integer, Name As String)

        Dim Item = New Computer
        Item.Code = Code
        Item.Name = Name

        DictComputers.Add(Code, Item)

    End Sub

    Sub AddApplication(Code As Integer, Name As String)

        Dim Item = New Application
        Item.Code = Code
        Item.Name = Name

        DictApplications.Add(Code, Item)

    End Sub

    Sub AddEvent(Code As Integer, Name As String)

        Dim Item = New EventType
        Item.Code = Code
        Item.Name = Name

        DictEvents.Add(Code, Item)

    End Sub

    Sub AddMetadata(Code As Integer, Guid As String, Name As String)

        Dim MD = New Metadata
        MD.Code = Code
        MD.Name = Name
        MD.Guid = Guid

        'reference file in old evenlog format may contain duplicated values when object description has been changed (for example, user renamed)
        If DictMetadata.ContainsKey(Code) Then
            DictMetadata.Remove(Code)
        End If

        DictMetadata.Add(Code, MD)

    End Sub

    Sub AddServer(Code As Integer, Name As String)

        Dim Item = New Server
        Item.Code = Code
        Item.Name = Name

        DictServers.Add(Code, Item)

    End Sub

    Sub AddMainPort(Code As Integer, Name As String)

        Dim Item = New MainPort
        Item.Code = Code
        If String.IsNullOrEmpty(Name) Then
            Item.Name = ""
        Else
            Item.Name = Name
        End If


        DictMainPorts.Add(Code, Item)

    End Sub

    Sub AddSecondPort(Code As Integer, Name As String)

        Dim Item = New SecondPort
        Item.Code = Code
        If String.IsNullOrEmpty(Name) Then
            Item.Name = ""
        Else
            Item.Name = Name
        End If

        DictSecondPorts.Add(Code, Item)

    End Sub

#End Region

    Class ReadParameters
        Public InfobaseId As Integer
        Public CurrentPosition As Long
        Public CurrentFilename As String
        Public LastEventNumber83 As Long
    End Class

    Class OneEventRecord
        Public RowID As Long
        Public DateTime As Date
        Public ConnectID As Integer
        Public Severity As Integer
        Public TransactionStatus As String
        Public Transaction As String
        Public TransactionStartTime As Date
        Public TransactionMark As Int64
        Public UserName As Integer
        Public ComputerName As Integer
        Public AppName As Integer
        Public EventID As Integer
        Public EventType As String
        Public Comment As String
        Public MetadataID As Integer
        Public SessionDataSplitCode As Integer
        Public DataStructure As String
        Public DataString As String
        Public DataType As Integer
        Public ServerID As Integer
        Public MainPortID As Integer
        Public SecondPortID As Integer
        Public SessionNumber As Integer
    End Class

    Class ESRecord
        Public RowID As Long
        Public ServerName As String
        Public DatabaseName As String
        Public DateTime As Date
        Public Severity As String
        Public EventType As EventType
        Public Computer As String
        Public Application As String
        Public Metadata As Metadata
        Public UserName As User
        Public SessionNumber As Integer
        Public ConnectID As Integer
        Public DataType As Integer
        Public DataStructure As String
        Public DataString As String
        Public Comment As String
        Public PrimaryPort As Integer
        Public SecondaryPort As Integer
        Public Server As String
        Public SessionDataSplitCode As Integer
    End Class

    Public EventsList As List(Of OneEventRecord) = New List(Of OneEventRecord)

    Public ESIndexName As String
    Public ESServerName As String

    Public InfobaseName As String
    Public InfobaseGuid As String
    Public InfobaseID As Integer = 0
    Public ConnectionString As String = ""
    Public ItIsMSSQL As Boolean = False
    Public ItIsMySQL As Boolean = False
    Public ItIsES As Boolean = False
    Public ESUseSynonymsForFieldsNames As Boolean = False
    Public LoadEventsStartingAt As Date
    Public ESFieldSynonyms As ElasticSearchFieldSynonymsClass = New ElasticSearchFieldSynonymsClass
    Public SleepTime As Integer = 60 * 1000 '1 минута

    Public Log As NLog.Logger

    Public Catalog As String
    Dim CurrentPosition As Int64 = 0
    Dim CurrentFilename As String = ""
    Dim LastEventNumber83 As Integer = 0

    Dim DictUsers As Dictionary(Of Integer, User) = New Dictionary(Of Integer, User)
    Dim DictComputers As Dictionary(Of Integer, Computer) = New Dictionary(Of Integer, Computer)
    Dim DictApplications As Dictionary(Of Integer, Application) = New Dictionary(Of Integer, Application)
    Dim DictEvents As Dictionary(Of Integer, EventType) = New Dictionary(Of Integer, EventType)
    Dim DictMetadata As Dictionary(Of Integer, Metadata) = New Dictionary(Of Integer, Metadata)
    Dim DictServers As Dictionary(Of Integer, Server) = New Dictionary(Of Integer, Server)
    Dim DictMainPorts As Dictionary(Of Integer, MainPort) = New Dictionary(Of Integer, MainPort)
    Dim DictSecondPorts As Dictionary(Of Integer, SecondPort) = New Dictionary(Of Integer, SecondPort)

    Public LastReferenceUpdate As DateTime

    Public Sub SaveReferenceValuesToDatabase()

        If ItIsMSSQL Then
            Dim objConn As New SqlConnection(ConnectionString)
            objConn.Open()

            '---------------------------------------------------------------------------------------------------------
            Dim command As New SqlCommand("IF NOT EXISTS (select * from [dbo].[Applications] where [Code] = @v1 AND [InfobaseCode] = @v3) " +
                                          "INSERT INTO [dbo].[Applications] ([InfobaseCode],[Code],[Name]) VALUES(@v3, @v1, @v2)", objConn)

            For Each Item In DictApplications
                Try
                    command.Parameters.Clear()
                    command.Parameters.Add(New SqlParameter("@v1", SqlDbType.Int)).Value = Item.Value.Code
                    command.Parameters.Add(New SqlParameter("@v2", SqlDbType.Char)).Value = Item.Value.Name
                    command.Parameters.Add(New SqlParameter("@v3", SqlDbType.Int)).Value = InfobaseID
                    command.ExecuteNonQuery()
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            Next

            '---------------------------------------------------------------------------------------------------------
            command.CommandText = "IF NOT EXISTS (select * from [dbo].[Computers] where [Code] = @v1 AND [InfobaseCode] = @v3) " +
                                    "INSERT INTO [dbo].[Computers] ([InfobaseCode],[Code],[Name]) VALUES(@v3, @v1,@v2)"

            For Each Item In DictComputers
                Try
                    command.Parameters.Clear()
                    command.Parameters.Add(New SqlParameter("@v1", SqlDbType.Int)).Value = Item.Value.Code
                    command.Parameters.Add(New SqlParameter("@v2", SqlDbType.Char)).Value = Item.Value.Name
                    command.Parameters.Add(New SqlParameter("@v3", SqlDbType.Int)).Value = InfobaseID
                    command.ExecuteNonQuery()
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            Next

            '---------------------------------------------------------------------------------------------------------
            command.CommandText = "IF NOT EXISTS (select * from [dbo].[EventsType] where [Code] = @v1 AND [InfobaseCode] = @v3) " +
                                    "INSERT INTO [dbo].[EventsType] ([InfobaseCode],[Code],[Name]) VALUES(@v3, @v1,@v2)"

            For Each Item In DictEvents
                Try
                    command.Parameters.Clear()
                    command.Parameters.Add(New SqlParameter("@v1", SqlDbType.Int)).Value = Item.Value.Code
                    command.Parameters.Add(New SqlParameter("@v2", SqlDbType.Char)).Value = Item.Value.Name
                    command.Parameters.Add(New SqlParameter("@v3", SqlDbType.Int)).Value = InfobaseID
                    command.ExecuteNonQuery()
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            Next

            '---------------------------------------------------------------------------------------------------------
            command.CommandText = "IF NOT EXISTS (select * from [dbo].[MainPorts] where [Code] = @v1 AND [InfobaseCode] = @v3) " +
                                    "INSERT INTO [dbo].[MainPorts] ([InfobaseCode],[Code],[Name]) VALUES(@v3, @v1,@v2)"

            For Each Item In DictMainPorts
                Try
                    command.Parameters.Clear()
                    command.Parameters.Add(New SqlParameter("@v1", SqlDbType.Int)).Value = Item.Value.Code
                    command.Parameters.Add(New SqlParameter("@v2", SqlDbType.Char)).Value = Item.Value.Name
                    command.Parameters.Add(New SqlParameter("@v3", SqlDbType.Int)).Value = InfobaseID
                    command.ExecuteNonQuery()
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            Next

            '---------------------------------------------------------------------------------------------------------
            command.CommandText = "MERGE INTO [dbo].[Metadata] AS Target
                                    USING (SELECT @v1 AS [Code],
                                                    @v4 AS [InfobaseCode],
                                                    @v3 AS [Guid]) AS Source 
                                    ON (Target.[Code] = Source.[Code]
	                                    AND Target.[InfobaseCode] = Source.[InfobaseCode]
	                                    AND Target.[Guid] = Source.[Guid])
                                    WHEN MATCHED AND NOT ([Name] = @v2) THEN UPDATE 
                                    SET [Name] = @v2
                                    WHEN NOT MATCHED THEN INSERT ([InfobaseCode], [Code], [Name], [Guid]) VALUES (@v4, @v1, @v2, @v3);"

            For Each Item In DictMetadata
                Try
                    command.Parameters.Clear()
                    command.Parameters.Add(New SqlParameter("@v1", SqlDbType.Int)).Value = Item.Value.Code
                    command.Parameters.Add(New SqlParameter("@v2", SqlDbType.Char)).Value = Item.Value.Name
                    command.Parameters.Add(New SqlParameter("@v3", SqlDbType.Char)).Value = Item.Value.Guid
                    command.Parameters.Add(New SqlParameter("@v4", SqlDbType.Int)).Value = InfobaseID
                    command.ExecuteNonQuery()
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            Next

            '---------------------------------------------------------------------------------------------------------
            command.CommandText = "MERGE INTO [dbo].[Users] AS Target
                                    USING (SELECT @v1 AS [Code],
                                                  @v4 AS [InfobaseCode],
                                                  @v3 AS [Guid]) AS Source 
                                    ON (Target.[Code] = Source.[Code]
	                                    AND Target.[InfobaseCode] = Source.[InfobaseCode]
	                                    AND Target.[Guid] = Source.[Guid])
                                    WHEN MATCHED AND NOT ([Name] = @v2) THEN UPDATE 
                                    SET [Name] = @v2
                                    WHEN NOT MATCHED THEN INSERT ([InfobaseCode], [Code], [Name], [Guid]) VALUES (@v4, @v1, @v2, @v3);"

            For Each Item In DictUsers
                Try
                    command.Parameters.Clear()
                    command.Parameters.Add(New SqlParameter("@v1", SqlDbType.Int)).Value = Item.Value.Code
                    command.Parameters.Add(New SqlParameter("@v2", SqlDbType.Char)).Value = Item.Value.Name
                    command.Parameters.Add(New SqlParameter("@v3", SqlDbType.Char)).Value = Item.Value.Guid
                    command.Parameters.Add(New SqlParameter("@v4", SqlDbType.Int)).Value = InfobaseID
                    command.ExecuteNonQuery()
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            Next

            '---------------------------------------------------------------------------------------------------------
            command.CommandText = "IF NOT EXISTS (select * from [dbo].[SecondPorts] where [Code] = @v1 AND [InfobaseCode] = @v3) " +
                                    "INSERT INTO [dbo].[SecondPorts] ([InfobaseCode],[Code],[Name]) VALUES(@v3, @v1,@v2)"

            For Each Item In DictSecondPorts
                Try
                    command.Parameters.Clear()
                    command.Parameters.Add(New SqlParameter("@v1", SqlDbType.Int)).Value = Item.Value.Code
                    command.Parameters.Add(New SqlParameter("@v2", SqlDbType.Char)).Value = Item.Value.Name
                    command.Parameters.Add(New SqlParameter("@v3", SqlDbType.Int)).Value = InfobaseID
                    command.ExecuteNonQuery()
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            Next

            '---------------------------------------------------------------------------------------------------------
            command.CommandText = "IF NOT EXISTS (select * from [dbo].[Servers] where [Code] = @v1 AND [InfobaseCode] = @v3) " +
                                    "INSERT INTO [dbo].[Servers] ([InfobaseCode],[Code],[Name]) VALUES(@v3, @v1,@v2)"

            For Each Item In DictServers
                Try
                    command.Parameters.Clear()
                    command.Parameters.Add(New SqlParameter("@v1", SqlDbType.Int)).Value = Item.Value.Code
                    command.Parameters.Add(New SqlParameter("@v2", SqlDbType.Char)).Value = Item.Value.Name
                    command.Parameters.Add(New SqlParameter("@v3", SqlDbType.Int)).Value = InfobaseID
                    command.ExecuteNonQuery()
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            Next

            '---------------------------------------------------------------------------------------------------------
            command.CommandText = "IF NOT EXISTS (select * from [dbo].[Infobases] where [Guid] = @v0) " +
                                    "INSERT INTO [dbo].[Infobases] ([Guid],[Code],[Name]) VALUES(@v0,@v1,@v2)"


            Try
                command.Parameters.Clear()
                command.Parameters.Add(New SqlParameter("@v0", SqlDbType.Char)).Value = InfobaseGuid
                command.Parameters.Add(New SqlParameter("@v1", SqlDbType.Int)).Value = InfobaseID
                command.Parameters.Add(New SqlParameter("@v2", SqlDbType.Char)).Value = InfobaseName
                command.ExecuteNonQuery()
            Catch ex As Exception
                Console.WriteLine(ex.Message)
            End Try

            command.Dispose()
            objConn.Close()
            objConn.Dispose()

        ElseIf ItIsMySQL Then

            Dim objConn = New MySql.Data.MySqlClient.MySqlConnection(ConnectionString)
            objConn.Open()

            Dim command As New MySqlCommand("REPLACE INTO `Applications`(`InfobaseCode`, `Code`, `Name`) VALUES(@v3, @v1, @v2)", objConn)

            For Each Item In DictApplications
                Try
                    command.Parameters.Clear()
                    command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.Int32)).Value = Item.Value.Code
                    command.Parameters.Add(New MySqlParameter("@v2", MySqlDbType.VarChar)).Value = Item.Value.Name
                    command.Parameters.Add(New MySqlParameter("@v3", MySqlDbType.Int32)).Value = InfobaseID
                    command.ExecuteNonQuery()
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            Next

            '---------------------------------------------------------------------------------------------------------
            command.CommandText = "REPLACE INTO `Computers` (`InfobaseCode`,`Code`,`Name`) VALUES(@v3, @v1, @v2)"

            For Each Item In DictComputers
                Try
                    command.Parameters.Clear()
                    command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.Int32)).Value = Item.Value.Code
                    command.Parameters.Add(New MySqlParameter("@v2", MySqlDbType.VarChar)).Value = Item.Value.Name
                    command.Parameters.Add(New MySqlParameter("@v3", MySqlDbType.Int32)).Value = InfobaseID
                    command.ExecuteNonQuery()
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            Next

            '---------------------------------------------------------------------------------------------------------
            command.CommandText = "REPLACE INTO `EventsType` (`InfobaseCode`,`Code`,`Name`) VALUES(@v3, @v1,@v2)"

            For Each Item In DictEvents
                Try
                    command.Parameters.Clear()
                    command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.Int32)).Value = Item.Value.Code
                    command.Parameters.Add(New MySqlParameter("@v2", MySqlDbType.VarChar)).Value = Item.Value.Name
                    command.Parameters.Add(New MySqlParameter("@v3", MySqlDbType.Int32)).Value = InfobaseID
                    command.ExecuteNonQuery()
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            Next

            '---------------------------------------------------------------------------------------------------------
            command.CommandText = "REPLACE INTO `MainPorts` (`InfobaseCode`,`Code`,`Name`) VALUES(@v3, @v1,@v2)"

            For Each Item In DictMainPorts
                Try
                    command.Parameters.Clear()
                    command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.Int32)).Value = Item.Value.Code
                    command.Parameters.Add(New MySqlParameter("@v2", MySqlDbType.VarChar)).Value = Item.Value.Name
                    command.Parameters.Add(New MySqlParameter("@v3", MySqlDbType.Int32)).Value = InfobaseID
                    command.ExecuteNonQuery()
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            Next

            '---------------------------------------------------------------------------------------------------------
            command.CommandText = "REPLACE INTO `Metadata` (`InfobaseCode`,`Code`,`Name`,`Guid`) VALUES(@v4, @v1,@v2,@v3)"

            For Each Item In DictMetadata
                Try
                    command.Parameters.Clear()
                    command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.Int32)).Value = Item.Value.Code
                    command.Parameters.Add(New MySqlParameter("@v2", MySqlDbType.VarChar)).Value = Item.Value.Name
                    command.Parameters.Add(New MySqlParameter("@v3", MySqlDbType.VarChar)).Value = Item.Value.Guid
                    command.Parameters.Add(New MySqlParameter("@v4", MySqlDbType.Int32)).Value = InfobaseID
                    command.ExecuteNonQuery()
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            Next

            '---------------------------------------------------------------------------------------------------------
            command.CommandText = "REPLACE INTO `Users` (`InfobaseCode`,`Code`,`Name`,`Guid`) VALUES(@v4, @v1,@v2,@v3)"

            For Each Item In DictUsers
                Try
                    command.Parameters.Clear()
                    command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.Int32)).Value = Item.Value.Code
                    command.Parameters.Add(New MySqlParameter("@v2", MySqlDbType.VarChar)).Value = Item.Value.Name
                    command.Parameters.Add(New MySqlParameter("@v3", MySqlDbType.VarChar)).Value = Item.Value.Guid
                    command.Parameters.Add(New MySqlParameter("@v4", MySqlDbType.Int32)).Value = InfobaseID
                    command.ExecuteNonQuery()
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            Next

            '---------------------------------------------------------------------------------------------------------
            command.CommandText = "REPLACE INTO `SecondPorts` (`InfobaseCode`,`Code`,`Name`) VALUES(@v3, @v1,@v2)"

            For Each Item In DictSecondPorts
                Try
                    command.Parameters.Clear()
                    command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.Int32)).Value = Item.Value.Code
                    command.Parameters.Add(New MySqlParameter("@v2", MySqlDbType.VarChar)).Value = Item.Value.Name
                    command.Parameters.Add(New MySqlParameter("@v3", MySqlDbType.Int32)).Value = InfobaseID
                    command.ExecuteNonQuery()
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            Next

            '---------------------------------------------------------------------------------------------------------
            command.CommandText = "REPLACE INTO `Servers` (`InfobaseCode`,`Code`,`Name`) VALUES(@v3, @v1,@v2)"

            For Each Item In DictServers
                Try
                    command.Parameters.Clear()
                    command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.Int32)).Value = Item.Value.Code
                    command.Parameters.Add(New MySqlParameter("@v2", MySqlDbType.VarChar)).Value = Item.Value.Name
                    command.Parameters.Add(New MySqlParameter("@v3", MySqlDbType.Int32)).Value = InfobaseID
                    command.ExecuteNonQuery()
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            Next

            '---------------------------------------------------------------------------------------------------------
            command.CommandText = "REPLACE INTO `Infobases` (`Guid`,`Code`,`Name`) VALUES(@v0,@v1,@v2)"

            Try
                command.Parameters.Clear()
                command.Parameters.Add(New MySqlParameter("@v0", MySqlDbType.VarChar)).Value = InfobaseGuid
                command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.Int32)).Value = InfobaseID
                command.Parameters.Add(New MySqlParameter("@v2", MySqlDbType.VarChar)).Value = InfobaseName
                command.ExecuteNonQuery()
            Catch ex As Exception
                Console.WriteLine(ex.Message)
            End Try


            command.Dispose()
            objConn.Close()
            objConn.Dispose()


        End If

        LastReferenceUpdate = Now

    End Sub

    Public Sub GetInfobaseIDFromDatabase()

        'У нас есть ГУИД базы, но нет сквозного кода для информации.
        'Сделано для оптимизации, чтобы в каждой строке события не записывать ГУИД, а хранить целое число.

        If ItIsMSSQL Then

            Dim objConn As New SqlConnection(ConnectionString)
            objConn.Open()

            Dim command As New SqlCommand("SELECT [Code] FROM [dbo].[Infobases] WHERE [Guid] = @v1", objConn)
            command.Parameters.Add(New SqlParameter("@v1", SqlDbType.Char)).Value = InfobaseGuid

            Dim rs = command.ExecuteReader

            If rs.Read Then
                InfobaseID = Convert.ToInt32(rs(0))
            End If
            rs.Close()

            If InfobaseID = 0 Then

                command.CommandText = "INSERT INTO Infobases ([Code], [Name], [guid])
                                        SELECT MAX(f) AS [Code],
                                               @v0 AS [Name],
                                               @v1 AS [guid]
                                        FROM (SELECT MAX(Code) + 1 AS f
                                              FROM Infobases
                                              UNION ALL
                                              SELECT 1 AS Expr1) AS T;"
                command.Parameters.Clear()
                command.Parameters.Add(New SqlParameter("@v0", SqlDbType.Char)).Value = InfobaseName
                command.Parameters.Add(New SqlParameter("@v1", SqlDbType.Char)).Value = InfobaseGuid
                command.ExecuteNonQuery()

                command.CommandText = "SELECT [Code] FROM [dbo].[Infobases] WHERE [Guid] = @v1"
                rs = command.ExecuteReader()

                If rs.Read Then
                    InfobaseID = Convert.ToInt32(rs(0))
                End If

                rs.Close()

            End If

            command.Dispose()
            objConn.Close()
            objConn.Dispose()

        ElseIf ItIsMySQL Then

            Dim objConn As New MySqlConnection(ConnectionString)
            objConn.Open()

            Dim command As New MySqlCommand("SELECT `Code` FROM `Infobases` WHERE `Guid` = @v1", objConn)
            command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.VarChar)).Value = InfobaseGuid

            Dim rs = command.ExecuteReader

            If rs.Read Then
                InfobaseID = Convert.ToInt32(rs(0))
            End If
            rs.Close()

            If InfobaseID = 0 Then

                command.CommandText = "INSERT INTO Infobases (`Code`,`Name`,`guid`)" +
                                     " SELECT MAX(f) AS `Code`, @v0 as `Name`, @v1 as `guid` FROM " +
                                     " (SELECT MAX(Code) + 1 AS f FROM `Infobases` UNION ALL" +
                                     " SELECT 1 AS `Expr1`) AS T"
                command.Parameters.Clear()
                command.Parameters.Add(New MySqlParameter("@v0", MySqlDbType.VarChar)).Value = InfobaseName
                command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.VarChar)).Value = InfobaseGuid
                command.ExecuteNonQuery()

                command.CommandText = "SELECT `Code` FROM `Infobases` WHERE `Guid` = @v1"
                rs = command.ExecuteReader()

                If rs.Read Then
                    InfobaseID = Convert.ToInt32(rs(0))
                End If

                rs.Close()

            End If

            command.Dispose()
            objConn.Close()
            objConn.Dispose()

        End If

        '--------------------------------------------------------------
    End Sub

    Sub SaveReadParametersToFile()

        Dim ReadParametersFile = Path.Combine(My.Application.Info.DirectoryPath, "read-setting-" + InfobaseGuid + ".json")

        Dim ConfigSettingObj = New ReadParameters
        ConfigSettingObj.CurrentPosition = CurrentPosition
        ConfigSettingObj.CurrentFilename = CurrentFilename
        ConfigSettingObj.LastEventNumber83 = LastEventNumber83

        Dim JsonText As String = JsonConvert.SerializeObject(ConfigSettingObj, Formatting.Indented)

        My.Computer.FileSystem.WriteAllText(ReadParametersFile, JsonText, False)

    End Sub

    Sub GetReadParametersFromFile()

        Dim ReadParametersFile = Path.Combine(My.Application.Info.DirectoryPath, "read-setting-" + InfobaseGuid + ".json")

        If My.Computer.FileSystem.FileExists(ReadParametersFile) Then

            Dim JsonText = My.Computer.FileSystem.ReadAllText(ReadParametersFile)

            Dim ConfigSettingObj = JsonConvert.DeserializeObject(Of ReadParameters)(JsonText)

            CurrentPosition = Convert.ToInt64(ConfigSettingObj.CurrentPosition)
            CurrentFilename = ConfigSettingObj.CurrentFilename
            LastEventNumber83 = ConfigSettingObj.LastEventNumber83

        End If

    End Sub

    Sub SaveEventsToSQL()

        If EventsList.Count = 0 Then
            Return
        End If

        If ItIsMSSQL Then

            Dim objConn As New SqlConnection(ConnectionString)
            objConn.Open()

            Dim dt = New DataTable
            For jj = 1 To 19
                dt.Columns.Add(New DataColumn())
            Next

            For Each Ev In EventsList

                If Ev.AppName = Nothing Then Continue For

                Dim Data(18)

                'Select [InfobaseCode]
                '    ,[DateTime]
                '    ,[TransactionStatus]
                '    ,[TransactionStartTime]
                '    ,[TransactionMark]
                '    ,[Transaction]
                '    ,[UserName]
                '    ,[ComputerName]
                '    ,[AppName]
                '    ,[EventID]
                '    ,[EventType]
                '    ,[Comment]
                '    ,[MetadataID]
                '    ,[DataStructure]
                '    ,[DataString]
                '    ,[ServerID]
                '    ,[MainPortID]
                '    ,[SecondPortID]
                '    ,[Seance]
                'FROM [EventLog].[dbo].[Events]

                Data(0) = InfobaseID
                Data(1) = Ev.DateTime
                Data(2) = Ev.TransactionStatus
                Data(3) = Ev.TransactionStartTime
                Data(4) = Ev.TransactionMark
                Data(5) = Ev.Transaction
                Data(6) = Ev.UserName
                Data(7) = Ev.ComputerName
                Data(8) = Ev.AppName
                Data(9) = Ev.EventID
                Data(10) = Ev.EventType
                Data(11) = Ev.Comment
                Data(12) = Ev.MetadataID
                Data(13) = Ev.DataStructure
                Data(14) = Ev.DataString
                Data(15) = Ev.ServerID
                Data(16) = Ev.MainPortID
                Data(17) = Ev.SecondPortID
                Data(18) = Ev.SessionNumber

                Dim row As DataRow = dt.NewRow()
                row.ItemArray = Data
                dt.Rows.Add(row)

            Next

            Using copy As New SqlBulkCopy(objConn)
                For jj = 0 To 18
                    copy.ColumnMappings.Add(jj, jj)
                Next
                copy.DestinationTableName = "Events"
                copy.WriteToServer(dt)
            End Using

            SaveReadParametersToFile()

            Console.WriteLine(Now.ToShortTimeString + " New records have been processed " + EventsList.Count.ToString)

            objConn.Close()
            objConn.Dispose()

        ElseIf ItIsMySQL Then

            Dim objConn As New MySqlConnection(ConnectionString)
            objConn.Open()

            Dim command As New MySqlCommand("START TRANSACTION", objConn)
            command.ExecuteNonQuery()

            command.CommandText = "INSERT INTO `Events` (`InfobaseCode`,`DateTime`,`TransactionStatus`,`Transaction`,`UserName`,`ComputerName`" +
                                      ",`AppName`,`EventID`,`EventType`,`Comment`,`MetadataID`,`DataStructure`,`DataString`" +
                                      ",`ServerID`,`MainPortID`,`SecondPortID`,`Seance`,`TransactionStartTime`,`TransactionMark`)" +
                                      " VALUES(@v0,@v1,@v2,@v3,@v4,@v5,@v6,@v7,@v8,@v9,@v10,@v11,@v12,@v13,@v14,@v15,@v16,@v17,@v18)"

            Dim i = 0
            For Each Ev In EventsList

                If Ev.AppName = Nothing Then Continue For

                Try
                    command.Parameters.Clear()
                    command.Parameters.Add(New MySqlParameter("@v0", MySqlDbType.Int32)).Value = InfobaseID
                    command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.DateTime)).Value = Ev.DateTime
                    command.Parameters.Add(New MySqlParameter("@v2", MySqlDbType.VarChar)).Value = Ev.TransactionStatus
                    command.Parameters.Add(New MySqlParameter("@v3", MySqlDbType.VarChar)).Value = Ev.Transaction
                    command.Parameters.Add(New MySqlParameter("@v4", MySqlDbType.Int32)).Value = Ev.UserName
                    command.Parameters.Add(New MySqlParameter("@v5", MySqlDbType.Int32)).Value = Ev.ComputerName
                    command.Parameters.Add(New MySqlParameter("@v6", MySqlDbType.Int32)).Value = Ev.AppName
                    command.Parameters.Add(New MySqlParameter("@v7", MySqlDbType.Int32)).Value = Ev.EventID
                    command.Parameters.Add(New MySqlParameter("@v8", MySqlDbType.VarChar)).Value = Ev.EventType
                    command.Parameters.Add(New MySqlParameter("@v9", MySqlDbType.VarChar)).Value = Ev.Comment
                    command.Parameters.Add(New MySqlParameter("@v10", MySqlDbType.Int32)).Value = Ev.MetadataID
                    command.Parameters.Add(New MySqlParameter("@v11", MySqlDbType.VarChar)).Value = Ev.DataStructure
                    command.Parameters.Add(New MySqlParameter("@v12", MySqlDbType.VarChar)).Value = Ev.DataString
                    command.Parameters.Add(New MySqlParameter("@v13", MySqlDbType.Int32)).Value = Ev.ServerID
                    command.Parameters.Add(New MySqlParameter("@v14", MySqlDbType.Int32)).Value = Ev.MainPortID
                    command.Parameters.Add(New MySqlParameter("@v15", MySqlDbType.Int32)).Value = Ev.SecondPortID
                    command.Parameters.Add(New MySqlParameter("@v16", MySqlDbType.Int32)).Value = Ev.SessionNumber
                    command.Parameters.Add(New MySqlParameter("@v17", MySqlDbType.DateTime)).Value = Ev.TransactionStartTime
                    command.Parameters.Add(New MySqlParameter("@v18", MySqlDbType.Int64)).Value = Ev.TransactionMark

                    command.ExecuteNonQuery()
                    i += 1
                Catch ex As Exception
                    Log.Error("Ошибка сохранения в БД записи от " + Ev.DateTime.ToString +
                                       " по ИБ " + InfobaseName + " : " + ex.Message)
                End Try


            Next

            Console.WriteLine(Now.ToShortTimeString + " New records have been processed " + i.ToString)

            SaveReadParametersToFile()

            command.CommandText = "COMMIT"
            command.Parameters.Clear()
            command.ExecuteNonQuery()

            command.Dispose()
            objConn.Close()
            objConn.Dispose()

        ElseIf ItIsES Then

            Dim node = New Uri(ConnectionString)

            Dim _settings = New ConnectionSettings(node).DefaultIndex(ESIndexName).MaximumRetries(2).MaxRetryTimeout(TimeSpan.FromSeconds(150))
            Dim _current = New ElasticClient(_settings)

            'Let's create proper array for ES
            Dim NewRecords As List(Of Object) = New List(Of Object)

            For Each EventRecord In EventsList
                Dim ESRecord = New ESRecord With {.ServerName = ESServerName, .DatabaseName = InfobaseName}
                ESRecord.RowID = EventRecord.RowID

                Select Case EventRecord.Severity
                    Case 1
                        ESRecord.Severity = "Information"
                    Case 2
                        ESRecord.Severity = "Warning"
                    Case 3
                        ESRecord.Severity = "Error"
                    Case 4
                        ESRecord.Severity = "Note"
                End Select

                ESRecord.DateTime = EventRecord.DateTime
                ESRecord.ConnectID = EventRecord.ConnectID
                ESRecord.DataType = EventRecord.DataType
                ESRecord.SessionNumber = EventRecord.SessionNumber
                ESRecord.DataStructure = EventRecord.DataStructure
                ESRecord.DataString = EventRecord.DataString
                ESRecord.Comment = EventRecord.Comment
                ESRecord.SessionDataSplitCode = EventRecord.SessionDataSplitCode


                Dim EventObj = New EventType
                If DictEvents.TryGetValue(EventRecord.EventID, EventObj) Then
                    ESRecord.EventType = EventObj
                End If

                Dim MetadataObj = New Metadata
                If DictMetadata.TryGetValue(EventRecord.MetadataID, MetadataObj) Then
                    ESRecord.Metadata = MetadataObj
                End If

                Dim ComputerObj = New Computer
                If DictComputers.TryGetValue(EventRecord.ComputerName, ComputerObj) Then
                    ESRecord.Computer = ComputerObj.Name
                End If

                Dim MainPortObj = New MainPort
                If DictMainPorts.TryGetValue(EventRecord.MainPortID, MainPortObj) Then
                    ESRecord.PrimaryPort = MainPortObj.Name
                End If

                Dim ServerObj = New Server
                If DictServers.TryGetValue(EventRecord.ServerID, ServerObj) Then
                    ESRecord.Server = ServerObj.Name
                End If

                Dim SecondPortObj = New SecondPort
                If DictSecondPorts.TryGetValue(EventRecord.SecondPortID, SecondPortObj) Then
                    ESRecord.SecondaryPort = SecondPortObj.Name
                End If

                Dim ApplicationObj = New Application
                If DictApplications.TryGetValue(EventRecord.AppName, ApplicationObj) Then
                    ESRecord.Application = ApplicationObj.Name
                End If

                Dim UserNameObj = New User
                If DictUsers.TryGetValue(EventRecord.UserName, UserNameObj) Then
                    ESRecord.UserName = UserNameObj
                End If

                If ESUseSynonymsForFieldsNames Then

                    Dim ESRecordUserFields = New Dictionary(Of String, Object)
                    ESRecordUserFields.Add(ESFieldSynonyms.ServerName, ESRecord.ServerName)
                    ESRecordUserFields.Add(ESFieldSynonyms.DatabaseName, ESRecord.DatabaseName)
                    ESRecordUserFields.Add(ESFieldSynonyms.RowID, ESRecord.RowID)
                    ESRecordUserFields.Add(ESFieldSynonyms.Severity, ESRecord.Severity)
                    ESRecordUserFields.Add(ESFieldSynonyms.DateTime, ESRecord.DateTime)
                    ESRecordUserFields.Add(ESFieldSynonyms.ConnectID, ESRecord.ConnectID)
                    ESRecordUserFields.Add(ESFieldSynonyms.DataType, ESRecord.DataType)
                    ESRecordUserFields.Add(ESFieldSynonyms.SessionNumber, ESRecord.SessionNumber)
                    ESRecordUserFields.Add(ESFieldSynonyms.DataStructure, ESRecord.DataStructure)
                    ESRecordUserFields.Add(ESFieldSynonyms.DataString, ESRecord.DataString)
                    ESRecordUserFields.Add(ESFieldSynonyms.Comment, ESRecord.Comment)
                    ESRecordUserFields.Add(ESFieldSynonyms.SessionDataSplitCode, ESRecord.SessionDataSplitCode)
                    ESRecordUserFields.Add(ESFieldSynonyms.EventType, ESRecord.EventType)
                    ESRecordUserFields.Add(ESFieldSynonyms.Metadata, ESRecord.Metadata)
                    ESRecordUserFields.Add(ESFieldSynonyms.Computer, ESRecord.Computer)
                    ESRecordUserFields.Add(ESFieldSynonyms.PrimaryPort, ESRecord.PrimaryPort)
                    ESRecordUserFields.Add(ESFieldSynonyms.Server, ESRecord.Server)
                    ESRecordUserFields.Add(ESFieldSynonyms.SecondaryPort, ESRecord.SecondaryPort)
                    ESRecordUserFields.Add(ESFieldSynonyms.Application, ESRecord.Application)
                    ESRecordUserFields.Add(ESFieldSynonyms.UserName, ESRecord.UserName)

                    NewRecords.Add(ESRecordUserFields)

                Else

                    NewRecords.Add(ESRecord)

                End If

            Next

            Dim Result = _current.IndexMany(NewRecords, ESIndexName, "event-log-record")

            Console.WriteLine(Now.ToShortTimeString + " New records have been processed " + NewRecords.Count.ToString)

            SaveReadParametersToFile()

        End If

        EventsList.Clear()

    End Sub

    Sub LoadReferenceFromTheTextFile(FileName As String, ByRef LastProcessedObjectForDebug As String)

        Dim FS As FileStream = New FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
        Dim SR As StreamReader = New StreamReader(FS)

        'Dim TextFile = My.Computer.FileSystem.OpenTextFileReader(FileName)
        'Dim Text = TextFile.ReadToEnd()
        'TextFile.Close()
        Dim Text = SR.ReadToEnd()
        SR.Close()
        FS.Close()

        Text = Text.Substring(Text.IndexOf("{"))

        Dim ObjectTexts = ParserServices.ParseEventLogString("{" + Text + "}")

        For Each TextObject In ObjectTexts

            LastProcessedObjectForDebug = TextObject

            Dim a = ParserServices.ParseEventLogString(TextObject)

            If Not a Is Nothing Then
                Select Case a(0)
                    Case "1"
                        AddUser(Convert.ToInt32(a(3)), a(1), a(2))
                    Case "2"
                        AddComputer(Convert.ToInt32(a(2)), a(1))
                    Case "3"
                        AddApplication(Convert.ToInt32(a(2)), a(1))
                    Case "4"
                        AddEvent(Convert.ToInt32(a(2)), a(1))
                    Case "5"
                        AddMetadata(Convert.ToInt32(a(3)), a(1), a(2))
                    Case "6"
                        AddServer(Convert.ToInt32(a(2)), a(1))
                    Case "7"
                        AddMainPort(Convert.ToInt32(a(2)), a(1))
                    Case "8"
                        AddSecondPort(Convert.ToInt32(a(2)), a(1))
                                        'Case "9" - не видел этих в файле
                                        'Case "10"
                    Case "11"
                    Case "12"
                    Case "13"
                        'в числе последних трех должны быть статус транзакции и важность
                    Case Else

                End Select

            End If



        Next

        SaveReferenceValuesToDatabase()

    End Sub

    Sub LoadReference()

        'Clear all reference dictionaries
        DictUsers.Clear()
        DictComputers.Clear()
        DictApplications.Clear()
        DictEvents.Clear()
        DictMetadata.Clear()
        DictServers.Clear()
        DictMainPorts.Clear()
        DictSecondPorts.Clear()

        Dim FileName = Path.Combine(Catalog, "1Cv8.lgd")

        If My.Computer.FileSystem.FileExists(FileName) Then

            Try
                Dim Conn = New SQLite.SQLiteConnection("Data Source=" + FileName)
                Conn.Open()
                Dim Command = New SQLite.SQLiteCommand
                Command.Connection = Conn

                Command.CommandText = "SELECT [code], [name] FROM [AppCodes]"
                Dim rs = Command.ExecuteReader
                While rs.Read
                    AddApplication(rs(0), RemoveQuotes(rs(1)))
                End While
                rs.Close()

                Command.CommandText = "SELECT [code], [name] FROM [ComputerCodes]"
                rs = Command.ExecuteReader
                While rs.Read
                    AddComputer(rs(0), RemoveQuotes(rs(1)))
                End While
                rs.Close()

                Command.CommandText = "SELECT [code], [name] FROM [EventCodes]"
                rs = Command.ExecuteReader
                While rs.Read
                    AddEvent(rs(0), RemoveQuotes(rs(1)))
                End While
                rs.Close()

                Command.CommandText = "SELECT [code], [uuid], [name] FROM [UserCodes]"
                rs = Command.ExecuteReader
                While rs.Read
                    AddUser(rs(0), rs(1), rs(2))
                End While
                rs.Close()

                Command.CommandText = "SELECT [code], [name] FROM [WorkServerCodes]"
                rs = Command.ExecuteReader
                While rs.Read
                    AddServer(rs(0), RemoveQuotes(rs(1)))
                End While
                rs.Close()

                Command.CommandText = "SELECT [code], [uuid], [name] FROM [MetadataCodes]"
                rs = Command.ExecuteReader
                While rs.Read
                    AddMetadata(rs(0), rs(1), rs(2))
                End While
                rs.Close()

                Command.CommandText = "SELECT [code], [name] FROM [PrimaryPortCodes]"
                rs = Command.ExecuteReader
                While rs.Read
                    AddMainPort(rs(0), rs(1))
                End While
                rs.Close()

                Command.CommandText = "SELECT [code], [name] FROM [SecondaryPortCodes]"
                rs = Command.ExecuteReader
                While rs.Read
                    AddSecondPort(rs(0), rs(1))
                End While
                rs.Close()

                Command.Dispose()
                Conn.Close()
                Conn.Dispose()

                SaveReferenceValuesToDatabase()
            Catch ex As Exception
                Log.Error(ex, "Error occurred while working with reference tables")
            End Try

        Else

            Dim LastProcessedObjectForDebug As String = ""
            FileName = Path.Combine(Catalog, "1Cv8.lgf")

            If My.Computer.FileSystem.FileExists(FileName) Then

                Try

                    Dim FI = My.Computer.FileSystem.GetFileInfo(FileName)

                    If FI.LastWriteTime >= LastReferenceUpdate Then

                        LoadReferenceFromTheTextFile(FileName, LastProcessedObjectForDebug)

                    End If

                Catch ex As Exception

                    Dim AdditionalString = ""
                    If Not String.IsNullOrEmpty(LastProcessedObjectForDebug) Then
                        AdditionalString = "Attempted to process this object: " + LastProcessedObjectForDebug
                    End If

                    Log.Error(ex, "Error occurred while working with reference file. " + AdditionalString)

                End Try

            End If

        End If

    End Sub

    Function RemoveQuotes(Str As String) As String

        RemoveQuotes = Str

        If RemoveQuotes.StartsWith("""") Then
            RemoveQuotes = RemoveQuotes.Substring(1)
        End If

        If RemoveQuotes.EndsWith("""") Then
            RemoveQuotes = RemoveQuotes.Substring(0, RemoveQuotes.Length - 1)
        End If

        Return RemoveQuotes

    End Function

    Sub FindAndStartParseFiles()

        Dim v83File = Path.Combine(Catalog, "1Cv8.lgd")
        If My.Computer.FileSystem.FileExists(v83File) Then

            LoadEvents83(v83File)

            SaveEventsToSQL()

        Else

            Dim ArrayFiles(0) As String
            Dim i = 0

            If My.Computer.FileSystem.DirectoryExists(Catalog) Then

                Dim Files = My.Computer.FileSystem.GetFiles(Catalog)
                For Each File In Files
                    If File.EndsWith(".lgp") Then
                        ReDim Preserve ArrayFiles(i)
                        ArrayFiles(i) = File
                        i = i + 1
                    End If
                Next

                System.Array.Sort(ArrayFiles)

                For Each File In ArrayFiles
                    If Not File Is Nothing Then
                        Try
                            Dim FI = My.Computer.FileSystem.GetFileInfo(File)
                            If FI.Name >= CurrentFilename Then
                                If Not FI.Name = CurrentFilename Then
                                    CurrentPosition = 61 ' start position for log-file 1C
                                End If
                                CurrentFilename = FI.Name
                                LoadEvents(File)
                            End If
                        Catch ex As Exception
                            Log.Error(ex, "Error in FindAndStartParseFiles")
                        End Try
                    End If

                Next

                SaveEventsToSQL()

            End If
        End If


    End Sub

    Sub LoadEvents83(FileName As String)
        Try

            Dim Conn = New System.Data.SQLite.SQLiteConnection("Data Source=" + FileName)
            Conn.Open()
            Dim Command = New System.Data.SQLite.SQLiteCommand
            Command.Connection = Conn

            Dim ANSI = Text.Encoding.GetEncoding(1252)
            Dim UTF8 = Text.Encoding.UTF8

            While True

                Command.CommandText = "SELECT 
                                            [rowID],
                                            [severity],
                                            [date],
                                            [connectID],
                                            [session],
                                            [transactionStatus],
                                            [transactionDate],
                                            [transactionID],
                                            [userCode],
                                            [computerCode],
                                            [appCode],
                                            [eventCode],
                                            [comment],
                                            [metadataCodes],
                                            [sessionDataSplitCode],
                                            [dataType],
                                            [data],
                                            [dataPresentation],
                                            [workServerCode],
                                            [primaryPortCode],
                                            [secondaryPortCode]
                                        FROM [EventLog] 
                                        WHERE [rowID] > @LastEventNumber83 AND [date] >= @MinimumDate
                                        ORDER BY 1
                                        LIMIT 1000"

                Command.Parameters.AddWithValue("LastEventNumber83", LastEventNumber83)

                Dim noOfSeconds As Int64 = (LoadEventsStartingAt - New Date).TotalSeconds
                Command.Parameters.AddWithValue("MinimumDate", noOfSeconds * 10000)

                Dim rs = Command.ExecuteReader

                Dim HasData = rs.HasRows
                While rs.Read

                    Dim OneEvent = New OneEventRecord
                    OneEvent.RowID = rs("rowID")
                    OneEvent.Severity = rs("severity")

                    OneEvent.ConnectID = rs("connectID")
                    OneEvent.DateTime = New Date().AddSeconds(Convert.ToInt64(rs("date") / 10000))
                    OneEvent.TransactionStatus = rs("transactionStatus")
                    OneEvent.TransactionMark = rs("transactionID")

                    OneEvent.TransactionStartTime = New Date().AddYears(2000)

                    Try
                        If Not rs("transactionDate") = 0 Then
                            OneEvent.TransactionStartTime = New Date().AddSeconds(Convert.ToInt64(rs("transactionDate") / 10000))
                        End If
                    Catch ex As Exception
                    End Try

                    OneEvent.UserName = rs("userCode")
                    OneEvent.ComputerName = rs("computerCode")
                    OneEvent.AppName = rs("appCode")
                    OneEvent.EventID = rs("eventCode")
                    OneEvent.Comment = rs("comment")
                    'OneEvent.MetadataID = rs("metadataCodes")
                    Dim MDCodes As String = rs("metadataCodes")
                    If String.IsNullOrEmpty(MDCodes) Then
                        OneEvent.MetadataID = 0
                    ElseIf MDCodes.Contains(",") Then
                        Dim MDCode As String = MDCodes.Split(New Char() {","c}).GetValue(0)
                        Integer.TryParse(MDCode, OneEvent.MetadataID)
                    Else
                        Integer.TryParse(MDCodes, OneEvent.MetadataID)
                    End If

                    Dim s = ""
                    If Not String.IsNullOrEmpty(rs("data")) Then
                        s = UTF8.GetString(ANSI.GetBytes(rs("data")))
                    End If
                    OneEvent.DataStructure = s

                    OneEvent.DataType = rs("dataType")
                    OneEvent.DataString = rs("dataPresentation")
                    OneEvent.ServerID = rs("workServerCode")
                    OneEvent.MainPortID = rs("primaryPortCode")
                    OneEvent.SecondPortID = rs("secondaryPortCode")
                    OneEvent.SessionNumber = rs("session")
                    OneEvent.SessionDataSplitCode = rs("sessionDataSplitCode")

                    OneEvent.Transaction = ""
                    OneEvent.EventType = ""

                    EventsList.Add(OneEvent)

                    LastEventNumber83 = OneEvent.RowID

                End While

                rs.Close()

                If EventsList.Count > 0 Then
                    SaveEventsToSQL()
                End If

                If Not HasData Then
                    Exit While
                End If

            End While


            Command.Dispose()
            Conn.Close()
            Conn.Dispose()

        Catch ex As Exception
            Log.Error(ex, "Error while working with EventLog table (SQLite)")
        End Try


    End Sub

    Sub LoadEvents(FileName As String)

        Dim FS As FileStream = New FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
        FS.Position = CurrentPosition

        Dim SR As StreamReader = New StreamReader(FS)



        'Dim TextFile = My.Computer.FileSystem.OpenTextFileReader(FileName)


        'TextFile.BaseStream.Position = Events.CurrentPosition  ' учесть, что первые 2 символа служебные, т.е. первый - №3
        '' + 2 символа перевода каретки в конце каждой строки

        '' '' TEMP
        ''Dim TextFile2 = My.Computer.FileSystem.OpenTextFileReader(FileName)
        ''TextFile2.BaseStream.Position = Events.CurrentPosition + 1
        '' '' TEMP

        Dim TextLine = ""

        Dim CountLines = 0
        Dim NewLine = True
        Dim StrEvent = ""
        Dim CountBracket = 0
        Dim TextBlockOpen = False
        Dim Position = CurrentPosition
        'Dim WasReadSomeString = False

        While Not TextLine Is Nothing

            'CountLines = CountLines + 1
            TextLine = SR.ReadLine()

            If TextLine Is Nothing Then
                'если чтение прервано на середине, то ничего прибавлять не нужно, если же чтение 
                'было завершено до конца, а потом читаются новые события, то появляется запятая, которую нужно учеть с ПЛЮС ОДИН позиции
                'If WasReadSomeString Then

                '    Position = Position + 1

                '    Events.CurrentPosition = Position

                'End If

                Exit While

            End If

            ' WasReadSomeString = True

            Position = Position + 2 + Text.Encoding.UTF8.GetBytes(TextLine).Length

            CurrentPosition = Position

            If NewLine Then
                StrEvent = TextLine
            Else
                StrEvent = StrEvent + vbNewLine + TextLine
            End If

            If ItsEndOfEvent(TextLine, CountBracket, TextBlockOpen) Then
                NewLine = True
                If Not StrEvent Is Nothing Then
                    Try
                        AddEvent(StrEvent)
                    Catch ex As Exception

                    End Try

                    '***
                    'Exit While
                    '**
                End If
            Else
                NewLine = False
            End If

        End While

        SaveEventsToSQL()

        SR.Close()
        FS.Close()

    End Sub

    Function ItsEndOfEvent(Str As String, ByRef Count As Integer, ByRef TextBlockOpen As Boolean)

        ItsEndOfEvent = False

        Dim TempStr = Str

        For i = 0 To TempStr.Length - 1
            Dim Simb = TempStr.Substring(i, 1)
            If Simb = """" Then
                TextBlockOpen = Not TextBlockOpen
            ElseIf Simb = "}" And Not TextBlockOpen Then
                Count = Count - 1
            ElseIf Simb = "{" And Not TextBlockOpen Then
                Count = Count + 1
            End If
        Next

        ItsEndOfEvent = (Count = 0)

    End Function

    Sub CreateStr(ByRef Str As String, LastLvl As Integer, Arr() As Object)

        Dim StrLvl = Arr(0).GetEnumerator
        Dim Lvl = 0
        Dim StrTab = ""

        While StrLvl.MoveNext
            If StrLvl.Current = "." Then
                Lvl = Lvl + 1

            End If
        End While

        For j = 1 To Lvl - 1
            StrTab = StrTab + " "
        Next


        Dim StrVal = ""

        Dim Count = 0
        For Each Ar In Arr
            Count = Count + 1
            If Count > 1 Then
                StrVal = StrVal + IIf(StrVal = "", "", "; ") + Ar.ToString.Trim
            End If
        Next


        If Not StrVal = "" Then
            If Lvl <> LastLvl Then
                Str = Str + IIf(Str = "", "", vbNewLine)
            End If
            Str = Str + StrTab + StrVal
        End If

        LastLvl = Lvl

    End Sub

    Function From16To10(Str As String)

        From16To10 = 0

        Dim Simb = "0123456789ABCDEF"

        Dim L = Str.Length

        For i = 0 To L - 1
            Dim S = Str.Substring(i, 1)

            Dim Ind = Simb.IndexOf(S.ToUpper)

            If Ind > -1 Then
                From16To10 = From16To10 + Ind * Math.Pow(16, L - (i + 1))
            End If
        Next

    End Function

    Public Sub AddEvent(Str As String)

        Dim provider As CultureInfo = CultureInfo.InvariantCulture
        Dim OneEvent As OneEventRecord = New OneEventRecord

        Dim Array = ParserServices.ParseEventLogString(Str)
        OneEvent.DateTime = Date.ParseExact(Array(0), "yyyyMMddHHmmss", provider)
        OneEvent.TransactionStatus = Array(1)

        If OneEvent.DateTime < LoadEventsStartingAt Then
            Exit Sub
        End If

        Dim TransStr = Array(2).ToString.Replace("}", "").Replace("{", "")
        Dim TransDate = From16To10(TransStr.Substring(0, TransStr.IndexOf(",")))

        OneEvent.TransactionStartTime = New Date().AddYears(2000)

        Try
            If Not TransDate = 0 Then
                OneEvent.TransactionStartTime = New Date().AddSeconds(Convert.ToInt64(TransDate / 10000))
            End If
        Catch ex As Exception
        End Try

        OneEvent.TransactionMark = From16To10(TransStr.Substring(TransStr.IndexOf(",") + 1))

        OneEvent.Transaction = Array(2)
        OneEvent.UserName = Convert.ToInt32(Array(3))
        OneEvent.ComputerName = Convert.ToInt32(Array(4))
        OneEvent.AppName = Convert.ToInt32(Array(5))
        OneEvent.EventID = Convert.ToInt32(Array(7))
        OneEvent.EventType = Array(8)
        OneEvent.Comment = RemoveQuotes(Array(9))
        OneEvent.MetadataID = Convert.ToInt32(Array(10))
        OneEvent.DataStructure = Array(11)
        OneEvent.DataString = RemoveQuotes(Array(12))
        OneEvent.ServerID = Convert.ToInt32(Array(13))
        OneEvent.MainPortID = Convert.ToInt32(Array(14))
        OneEvent.SecondPortID = Convert.ToInt32(Array(15))
        OneEvent.SessionNumber = Convert.ToInt32(Array(16))

        '*************************************************************************
        'Additional parsing to make sure that data looks the same between old and new EventLog formats
        If OneEvent.DataStructure = "{""U""}" Then 'empty reference
            OneEvent.DataStructure = ""
        ElseIf OneEvent.DataStructure.StartsWith("{") Then
            'internal representation for different objects.
            Dim ParsedObject = ParserServices.ParseEventlogString(OneEvent.DataStructure)
            If ParsedObject.Length = 2 Then
                If ParsedObject(0) = """S""" _
                    Or ParsedObject(0) = """R""" Then 'this is string or reference 

                    OneEvent.DataStructure = RemoveQuotes(ParsedObject(1)) 'string value

                End If

            End If

        End If

        Select Case OneEvent.EventType
            Case "I"
                OneEvent.Severity = 1 '"Information"
            Case "W"
                OneEvent.Severity = 2 '"Warning"
            Case "E"
                OneEvent.Severity = 3'"Error"
            Case "N"
                OneEvent.Severity = 4 '"Note"
        End Select

        '*************************************************************************


        EventsList.Add(OneEvent)
        'Events(Events.Length - 1) = OneEvent

        If EventsList.Count >= 1000 Then
            'Console.WriteLine("Выгрузка 1000 событий: " + Now.ToString)
            SaveEventsToSQL()
        End If

    End Sub

    Public Sub DoWork()

        While True

            Console.WriteLine(Now.ToShortTimeString + " Start new iteration...")

            Try

                LoadReference()

                GetReadParametersFromFile()

                FindAndStartParseFiles()

            Catch ex As Exception
                Log.Error(ex, "Error occurred during log file processing (" + InfobaseName + ")")
            End Try

            Threading.Thread.Sleep(SleepTime)

        End While


    End Sub
End Class
