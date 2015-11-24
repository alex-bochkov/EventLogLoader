Imports System.Threading
Imports System.Globalization
Imports System.Data.SqlClient
Imports System.IO
Imports MySql.Data.MySqlClient

Public Class EventLogLoaderService

    Class ReferenceClass

        Structure User
            Dim Code As Int32
            Dim Name As String
            Dim Guid As String
        End Structure

        Structure Metadata
            Dim Code As Int32
            Dim Name As String
            Dim Guid As String
        End Structure

        Structure Computer
            Dim Code As Int32
            Dim Name As String
        End Structure

        Structure Application
            Dim Code As Int32
            Dim Name As String
        End Structure

        Structure EventType
            Dim Code As Int32
            Dim Name As String
        End Structure

        Structure Server
            Dim Code As Int32
            Dim Name As String
        End Structure

        Structure MainPort
            Dim Code As Int32
            Dim Name As String
        End Structure

        Structure SecondPort
            Dim Code As Int32
            Dim Name As String
        End Structure

        Dim ArrayUsers As List(Of User) = New List(Of User)
        Dim ArrayComputers As List(Of Computer) = New List(Of Computer)
        Dim ArrayApplications As List(Of Application) = New List(Of Application)
        Dim ArrayEvents As List(Of EventType) = New List(Of EventType)
        Dim ArrayMetadata As List(Of Metadata) = New List(Of Metadata)
        Dim ArrayServers As List(Of Server) = New List(Of Server)
        Dim ArrayMainPorts As List(Of MainPort) = New List(Of MainPort)
        Dim ArraySecondPorts As List(Of SecondPort) = New List(Of SecondPort)

        Public InfobaseName As String
        Public InfobaseGuid As String
        Public InfobaseID As Integer
        Public ConnectionString As String
        Public ItIsMSSQL As Boolean = False
        Public ItIsMySQL As Boolean = False

        Public LastUpdate As DateTime

        Public Sub AddUser(Code As Integer, Guid As String, Name As String)

            Dim Usr = New User
            Usr.Code = Code
            Usr.Name = Name
            Usr.Guid = Guid

            ArrayUsers.Add(Usr)

        End Sub

        Public Sub AddComputer(Code As Integer, Name As String)

            Dim Item = New Computer
            Item.Code = Code
            Item.Name = Name

            ArrayComputers.Add(Item)

        End Sub

        Public Sub AddApplication(Code As Integer, Name As String)

            Dim Item = New Application
            Item.Code = Code
            Item.Name = Name

            ArrayApplications.Add(Item)

        End Sub

        Public Sub AddEvent(Code As Integer, Name As String)

            Dim Item = New EventType
            Item.Code = Code
            Item.Name = Name

            ArrayEvents.Add(Item)

        End Sub

        Public Sub AddMetadata(Code As Integer, Guid As String, Name As String)

            Dim MD = New Metadata
            MD.Code = Code
            MD.Name = Name
            MD.Guid = Guid

            ArrayMetadata.Add(MD)

        End Sub

        Public Sub AddServer(Code As Integer, Name As String)

            Dim Item = New Server
            Item.Code = Code
            Item.Name = Name

            ArrayServers.Add(Item)

        End Sub

        Public Sub AddMainPort(Code As Integer, Name As String)

            Dim Item = New MainPort
            Item.Code = Code
            If String.IsNullOrEmpty(Name) Then
                Item.Name = ""
            Else
                Item.Name = Name
            End If


            ArrayMainPorts.Add(Item)

        End Sub

        Public Sub AddSecondPort(Code As Integer, Name As String)

            Dim Item = New SecondPort
            Item.Code = Code
            If String.IsNullOrEmpty(Name) Then
                Item.Name = ""
            Else
                Item.Name = Name
            End If

            ArraySecondPorts.Add(Item)

        End Sub

        Public Sub SaveReferenceToSQL()

            If ItIsMSSQL Then
                Dim objConn As New SqlConnection(ConnectionString)
                objConn.Open()

                '---------------------------------------------------------------------------------------------------------
                Dim command As New SqlCommand("IF NOT EXISTS (select * from [dbo].[Applications] where [Code] = @v1 AND [InfobaseCode] = @v3) " + _
                                              "INSERT INTO [dbo].[Applications] ([InfobaseCode],[Code],[Name]) VALUES(@v3, @v1, @v2)", objConn)

                For Each Item In ArrayApplications
                    Try
                        command.Parameters.Clear()
                        command.Parameters.Add(New SqlParameter("@v1", SqlDbType.Int)).Value = Item.Code
                        command.Parameters.Add(New SqlParameter("@v2", SqlDbType.Char)).Value = Item.Name
                        command.Parameters.Add(New SqlParameter("@v3", SqlDbType.Int)).Value = InfobaseID
                        command.ExecuteNonQuery()
                    Catch ex As Exception
                        Console.WriteLine(ex.Message)
                    End Try
                Next

                '---------------------------------------------------------------------------------------------------------
                command.CommandText = "IF NOT EXISTS (select * from [dbo].[Computers] where [Code] = @v1 AND [InfobaseCode] = @v3) " + _
                                        "INSERT INTO [dbo].[Computers] ([InfobaseCode],[Code],[Name]) VALUES(@v3, @v1,@v2)"

                For Each Item In ArrayComputers
                    Try
                        command.Parameters.Clear()
                        command.Parameters.Add(New SqlParameter("@v1", SqlDbType.Int)).Value = Item.Code
                        command.Parameters.Add(New SqlParameter("@v2", SqlDbType.Char)).Value = Item.Name
                        command.Parameters.Add(New SqlParameter("@v3", SqlDbType.Int)).Value = InfobaseID
                        command.ExecuteNonQuery()
                    Catch ex As Exception
                        Console.WriteLine(ex.Message)
                    End Try
                Next

                '---------------------------------------------------------------------------------------------------------
                command.CommandText = "IF NOT EXISTS (select * from [dbo].[EventsType] where [Code] = @v1 AND [InfobaseCode] = @v3) " + _
                                        "INSERT INTO [dbo].[EventsType] ([InfobaseCode],[Code],[Name]) VALUES(@v3, @v1,@v2)"

                For Each Item In ArrayEvents
                    Try
                        command.Parameters.Clear()
                        command.Parameters.Add(New SqlParameter("@v1", SqlDbType.Int)).Value = Item.Code
                        command.Parameters.Add(New SqlParameter("@v2", SqlDbType.Char)).Value = Item.Name
                        command.Parameters.Add(New SqlParameter("@v3", SqlDbType.Int)).Value = InfobaseID
                        command.ExecuteNonQuery()
                    Catch ex As Exception
                        Console.WriteLine(ex.Message)
                    End Try
                Next

                '---------------------------------------------------------------------------------------------------------
                command.CommandText = "IF NOT EXISTS (select * from [dbo].[MainPorts] where [Code] = @v1 AND [InfobaseCode] = @v3) " + _
                                        "INSERT INTO [dbo].[MainPorts] ([InfobaseCode],[Code],[Name]) VALUES(@v3, @v1,@v2)"

                For Each Item In ArrayMainPorts
                    Try
                        command.Parameters.Clear()
                        command.Parameters.Add(New SqlParameter("@v1", SqlDbType.Int)).Value = Item.Code
                        command.Parameters.Add(New SqlParameter("@v2", SqlDbType.Char)).Value = Item.Name
                        command.Parameters.Add(New SqlParameter("@v3", SqlDbType.Int)).Value = InfobaseID
                        command.ExecuteNonQuery()
                    Catch ex As Exception
                        Console.WriteLine(ex.Message)
                    End Try
                Next

                '---------------------------------------------------------------------------------------------------------
                command.CommandText = "IF NOT EXISTS (select * from [dbo].[Metadata] where [Code] = @v1 AND [InfobaseCode] = @v4) " + _
                                        "INSERT INTO [dbo].[Metadata] ([InfobaseCode],[Code],[Name],[Guid]) VALUES(@v4, @v1,@v2,@v3)"

                For Each Item In ArrayMetadata
                    Try
                        command.Parameters.Clear()
                        command.Parameters.Add(New SqlParameter("@v1", SqlDbType.Int)).Value = Item.Code
                        command.Parameters.Add(New SqlParameter("@v2", SqlDbType.Char)).Value = Item.Name
                        command.Parameters.Add(New SqlParameter("@v3", SqlDbType.Char)).Value = Item.Guid
                        command.Parameters.Add(New SqlParameter("@v4", SqlDbType.Int)).Value = InfobaseID
                        command.ExecuteNonQuery()
                    Catch ex As Exception
                        Console.WriteLine(ex.Message)
                    End Try
                Next

                '---------------------------------------------------------------------------------------------------------
                command.CommandText = "IF NOT EXISTS (select * from [dbo].[Users] where [Code] = @v1 AND [InfobaseCode] = @v4) " + _
                                        "INSERT INTO [dbo].[Users] ([InfobaseCode],[Code],[Name],[Guid]) VALUES(@v4, @v1,@v2,@v3)"

                For Each Item In ArrayUsers
                    Try
                        command.Parameters.Clear()
                        command.Parameters.Add(New SqlParameter("@v1", SqlDbType.Int)).Value = Item.Code
                        command.Parameters.Add(New SqlParameter("@v2", SqlDbType.Char)).Value = Item.Name
                        command.Parameters.Add(New SqlParameter("@v3", SqlDbType.Char)).Value = Item.Guid
                        command.Parameters.Add(New SqlParameter("@v4", SqlDbType.Int)).Value = InfobaseID
                        command.ExecuteNonQuery()
                    Catch ex As Exception
                        Console.WriteLine(ex.Message)
                    End Try
                Next

                '---------------------------------------------------------------------------------------------------------
                command.CommandText = "IF NOT EXISTS (select * from [dbo].[SecondPorts] where [Code] = @v1 AND [InfobaseCode] = @v3) " + _
                                        "INSERT INTO [dbo].[SecondPorts] ([InfobaseCode],[Code],[Name]) VALUES(@v3, @v1,@v2)"

                For Each Item In ArraySecondPorts
                    Try
                        command.Parameters.Clear()
                        command.Parameters.Add(New SqlParameter("@v1", SqlDbType.Int)).Value = Item.Code
                        command.Parameters.Add(New SqlParameter("@v2", SqlDbType.Char)).Value = Item.Name
                        command.Parameters.Add(New SqlParameter("@v3", SqlDbType.Int)).Value = InfobaseID
                        command.ExecuteNonQuery()
                    Catch ex As Exception
                        Console.WriteLine(ex.Message)
                    End Try
                Next

                '---------------------------------------------------------------------------------------------------------
                command.CommandText = "IF NOT EXISTS (select * from [dbo].[Servers] where [Code] = @v1 AND [InfobaseCode] = @v3) " + _
                                        "INSERT INTO [dbo].[Servers] ([InfobaseCode],[Code],[Name]) VALUES(@v3, @v1,@v2)"

                For Each Item In ArrayServers
                    Try
                        command.Parameters.Clear()
                        command.Parameters.Add(New SqlParameter("@v1", SqlDbType.Int)).Value = Item.Code
                        command.Parameters.Add(New SqlParameter("@v2", SqlDbType.Char)).Value = Item.Name
                        command.Parameters.Add(New SqlParameter("@v3", SqlDbType.Int)).Value = InfobaseID
                        command.ExecuteNonQuery()
                    Catch ex As Exception
                        Console.WriteLine(ex.Message)
                    End Try
                Next

                '---------------------------------------------------------------------------------------------------------
                command.CommandText = "IF NOT EXISTS (select * from [dbo].[Infobases] where [Guid] = @v0) " + _
                                        "INSERT INTO [dbo].[Infobases] ([Guid],[Code],[Name]) VALUES(@v0,@v1,@v2)"

                For Each Item In ArrayServers
                    Try
                        command.Parameters.Clear()
                        command.Parameters.Add(New SqlParameter("@v0", SqlDbType.Char)).Value = InfobaseGuid
                        command.Parameters.Add(New SqlParameter("@v1", SqlDbType.Int)).Value = InfobaseID
                        command.Parameters.Add(New SqlParameter("@v2", SqlDbType.Char)).Value = InfobaseName
                        command.ExecuteNonQuery()
                    Catch ex As Exception
                        Console.WriteLine(ex.Message)
                    End Try
                Next

                command.Dispose()
                objConn.Close()
                objConn.Dispose()

            ElseIf ItIsMySQL Then

                Dim objConn = New MySql.Data.MySqlClient.MySqlConnection(ConnectionString)
                objConn.Open()

                Dim command As New MySqlCommand("REPLACE INTO `Applications`(`InfobaseCode`, `Code`, `Name`) VALUES(@v3, @v1, @v2)", objConn)

                For Each Item In ArrayApplications
                    Try
                        command.Parameters.Clear()
                        command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.Int32)).Value = Item.Code
                        command.Parameters.Add(New MySqlParameter("@v2", MySqlDbType.VarChar)).Value = Item.Name
                        command.Parameters.Add(New MySqlParameter("@v3", MySqlDbType.Int32)).Value = InfobaseID
                        command.ExecuteNonQuery()
                    Catch ex As Exception
                        Console.WriteLine(ex.Message)
                    End Try
                Next

                '---------------------------------------------------------------------------------------------------------
                command.CommandText = "REPLACE INTO `Computers` (`InfobaseCode`,`Code`,`Name`) VALUES(@v3, @v1, @v2)"

                For Each Item In ArrayComputers
                    Try
                        command.Parameters.Clear()
                        command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.Int32)).Value = Item.Code
                        command.Parameters.Add(New MySqlParameter("@v2", MySqlDbType.VarChar)).Value = Item.Name
                        command.Parameters.Add(New MySqlParameter("@v3", MySqlDbType.Int32)).Value = InfobaseID
                        command.ExecuteNonQuery()
                    Catch ex As Exception
                        Console.WriteLine(ex.Message)
                    End Try
                Next

                '---------------------------------------------------------------------------------------------------------
                command.CommandText = "REPLACE INTO `EventsType` (`InfobaseCode`,`Code`,`Name`) VALUES(@v3, @v1,@v2)"

                For Each Item In ArrayEvents
                    Try
                        command.Parameters.Clear()
                        command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.Int32)).Value = Item.Code
                        command.Parameters.Add(New MySqlParameter("@v2", MySqlDbType.VarChar)).Value = Item.Name
                        command.Parameters.Add(New MySqlParameter("@v3", MySqlDbType.Int32)).Value = InfobaseID
                        command.ExecuteNonQuery()
                    Catch ex As Exception
                        Console.WriteLine(ex.Message)
                    End Try
                Next

                '---------------------------------------------------------------------------------------------------------
                command.CommandText = "REPLACE INTO `MainPorts` (`InfobaseCode`,`Code`,`Name`) VALUES(@v3, @v1,@v2)"

                For Each Item In ArrayMainPorts
                    Try
                        command.Parameters.Clear()
                        command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.Int32)).Value = Item.Code
                        command.Parameters.Add(New MySqlParameter("@v2", MySqlDbType.VarChar)).Value = Item.Name
                        command.Parameters.Add(New MySqlParameter("@v3", MySqlDbType.Int32)).Value = InfobaseID
                        command.ExecuteNonQuery()
                    Catch ex As Exception
                        Console.WriteLine(ex.Message)
                    End Try
                Next

                '---------------------------------------------------------------------------------------------------------
                command.CommandText = "REPLACE INTO `Metadata` (`InfobaseCode`,`Code`,`Name`,`Guid`) VALUES(@v4, @v1,@v2,@v3)"

                For Each Item In ArrayMetadata
                    Try
                        command.Parameters.Clear()
                        command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.Int32)).Value = Item.Code
                        command.Parameters.Add(New MySqlParameter("@v2", MySqlDbType.VarChar)).Value = Item.Name
                        command.Parameters.Add(New MySqlParameter("@v3", MySqlDbType.VarChar)).Value = Item.Guid
                        command.Parameters.Add(New MySqlParameter("@v4", MySqlDbType.Int32)).Value = InfobaseID
                        command.ExecuteNonQuery()
                    Catch ex As Exception
                        Console.WriteLine(ex.Message)
                    End Try
                Next

                '---------------------------------------------------------------------------------------------------------
                command.CommandText = "REPLACE INTO `Users` (`InfobaseCode`,`Code`,`Name`,`Guid`) VALUES(@v4, @v1,@v2,@v3)"

                For Each Item In ArrayUsers
                    Try
                        command.Parameters.Clear()
                        command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.Int32)).Value = Item.Code
                        command.Parameters.Add(New MySqlParameter("@v2", MySqlDbType.VarChar)).Value = Item.Name
                        command.Parameters.Add(New MySqlParameter("@v3", MySqlDbType.VarChar)).Value = Item.Guid
                        command.Parameters.Add(New MySqlParameter("@v4", MySqlDbType.Int32)).Value = InfobaseID
                        command.ExecuteNonQuery()
                    Catch ex As Exception
                        Console.WriteLine(ex.Message)
                    End Try
                Next

                '---------------------------------------------------------------------------------------------------------
                command.CommandText = "REPLACE INTO `SecondPorts` (`InfobaseCode`,`Code`,`Name`) VALUES(@v3, @v1,@v2)"

                For Each Item In ArraySecondPorts
                    Try
                        command.Parameters.Clear()
                        command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.Int32)).Value = Item.Code
                        command.Parameters.Add(New MySqlParameter("@v2", MySqlDbType.VarChar)).Value = Item.Name
                        command.Parameters.Add(New MySqlParameter("@v3", MySqlDbType.Int32)).Value = InfobaseID
                        command.ExecuteNonQuery()
                    Catch ex As Exception
                        Console.WriteLine(ex.Message)
                    End Try
                Next

                '---------------------------------------------------------------------------------------------------------
                command.CommandText = "REPLACE INTO `Servers` (`InfobaseCode`,`Code`,`Name`) VALUES(@v3, @v1,@v2)"

                For Each Item In ArrayServers
                    Try
                        command.Parameters.Clear()
                        command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.Int32)).Value = Item.Code
                        command.Parameters.Add(New MySqlParameter("@v2", MySqlDbType.VarChar)).Value = Item.Name
                        command.Parameters.Add(New MySqlParameter("@v3", MySqlDbType.Int32)).Value = InfobaseID
                        command.ExecuteNonQuery()
                    Catch ex As Exception
                        Console.WriteLine(ex.Message)
                    End Try
                Next

                '---------------------------------------------------------------------------------------------------------
                command.CommandText = "REPLACE INTO `Infobases` (`Guid`,`Code`,`Name`) VALUES(@v0,@v1,@v2)"

                For Each Item In ArrayServers
                    Try
                        command.Parameters.Clear()
                        command.Parameters.Add(New MySqlParameter("@v0", MySqlDbType.VarChar)).Value = InfobaseGuid
                        command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.Int32)).Value = InfobaseID
                        command.Parameters.Add(New MySqlParameter("@v2", MySqlDbType.VarChar)).Value = InfobaseName
                        command.ExecuteNonQuery()
                    Catch ex As Exception
                        Console.WriteLine(ex.Message)
                    End Try
                Next

                command.Dispose()
                objConn.Close()
                objConn.Dispose()


                End If



                ArrayUsers.Clear()
                ArrayComputers.Clear()
                ArrayApplications.Clear()
                ArrayEvents.Clear()
                ArrayMetadata.Clear()
                ArrayServers.Clear()
                ArrayMainPorts.Clear()
                ArraySecondPorts.Clear()

                LastUpdate = Now

        End Sub


    End Class

    Class EventLogLoader

        Structure OneEvent
            Dim RowID As Integer
            Dim DateTime As Date
            'Dim RecordType As String
            Dim TransactionStatus As String
            Dim Transaction As String
            Dim TransactionStartTime As Date
            Dim TransactionMark As Int64
            Dim UserName As Integer
            Dim ComputerName As Integer
            Dim AppName As Integer
            Dim Field2 As String
            Dim EventID As Integer
            Dim EventType As String
            Dim Comment As String
            Dim MetadataID As Integer
            Dim DataStructure As String
            Dim DataString As String
            Dim ServerID As Integer
            Dim MainPortID As Integer
            Dim SecondPortID As Integer
            Dim Seance As Integer
            Dim Field7 As String
            Dim Field8 As String
        End Structure

        Public Events As List(Of OneEvent) = New List(Of OneEvent)
        Public CurrentPosition As Int64 = 0
        Public CurrentFilename As String = ""
        Public Log As NLog.Logger

        Public InfobaseName As String
        Public InfobaseID As Integer
        Public LastEventNumber83 As Integer = 0

        Public ConnectionString As String
        Public ItIsMSSQL As String
        Public ItIsMySQL As String

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

        Function DeleteQuote(Str As String)

            Dim SubStr = Str.Substring(1, Str.Length - 2)

            Return SubStr

        End Function

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
            Dim OneEvent = New OneEvent

            'If Not Events(0).DateTime = Nothing Then
            '    ReDim Preserve Events(Events.Length)
            'End If

            Dim Array = Parser.ParserServices.ParsesClass.ParseEventlogString(Str)
            OneEvent.DateTime = Date.ParseExact(Array(0), "yyyyMMddHHmmss", provider)
            OneEvent.TransactionStatus = Array(1)

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
            OneEvent.Field2 = Array(6)
            OneEvent.EventID = Convert.ToInt32(Array(7))
            OneEvent.EventType = Array(8)
            OneEvent.Comment = DeleteQuote(Array(9))
            OneEvent.MetadataID = Convert.ToInt32(Array(10))
            OneEvent.DataStructure = Array(11)
            OneEvent.DataString = DeleteQuote(Array(12))
            OneEvent.ServerID = Convert.ToInt32(Array(13))
            OneEvent.MainPortID = Convert.ToInt32(Array(14))
            OneEvent.SecondPortID = Convert.ToInt32(Array(15))
            OneEvent.Seance = Convert.ToInt32(Array(16))
            OneEvent.Field7 = Array(17)
            OneEvent.Field8 = Array(18)

            '*************************************************************************


            Events.Add(OneEvent)
            'Events(Events.Length - 1) = OneEvent

            If Events.Count >= 1000 Then
                'Console.WriteLine("Выгрузка 1000 событий: " + Now.ToString)
                SaveEventsToSQL()
            End If

        End Sub

        Public Sub SaveEventsToSQL()

            If Events.Count = 0 Then
                Return
            End If

            If ItIsMSSQL Then
                Dim objConn As New SqlConnection(ConnectionString)
                objConn.Open()

                Dim command As New SqlCommand("BEGIN TRANSACTION", objConn)
                command.ExecuteNonQuery()

                command.CommandText = "INSERT INTO [dbo].[Events] ([InfobaseCode],[DateTime],[TransactionStatus],[Transaction],[UserName],[ComputerName]" + _
                                          ",[AppName],[Field2],[EventID],[EventType],[Comment],[MetadataID],[DataStructure],[DataString]" + _
                                          ",[ServerID],[MainPortID],[SecondPortID],[Seance],[Field7],[Field8],[TransactionStartTime],[TransactionMark])" + _
                                          " VALUES(@v0,@v1,@v2,@v3,@v4,@v5,@v6,@v7,@v8,@v9,@v10,@v11,@v12,@v13,@v14,@v15,@v16,@v17,@v18,@v19,@v20,@v21)"

                Dim i = 0
                For Each Ev In Events

                    If Ev.AppName = Nothing Then Continue For

                    Try
                        command.Parameters.Clear()
                        command.Parameters.Add(New SqlParameter("@v0", SqlDbType.Int)).Value = InfobaseID
                        command.Parameters.Add(New SqlParameter("@v1", SqlDbType.DateTime)).Value = Ev.DateTime
                        command.Parameters.Add(New SqlParameter("@v2", SqlDbType.Char)).Value = Ev.TransactionStatus
                        command.Parameters.Add(New SqlParameter("@v3", SqlDbType.Char)).Value = Ev.Transaction
                        command.Parameters.Add(New SqlParameter("@v4", SqlDbType.Int)).Value = Ev.UserName
                        command.Parameters.Add(New SqlParameter("@v5", SqlDbType.Int)).Value = Ev.ComputerName
                        command.Parameters.Add(New SqlParameter("@v6", SqlDbType.Int)).Value = Ev.AppName
                        command.Parameters.Add(New SqlParameter("@v7", SqlDbType.Char)).Value = Ev.Field2
                        command.Parameters.Add(New SqlParameter("@v8", SqlDbType.Int)).Value = Ev.EventID
                        command.Parameters.Add(New SqlParameter("@v9", SqlDbType.Char)).Value = Ev.EventType
                        command.Parameters.Add(New SqlParameter("@v10", SqlDbType.VarChar)).Value = Ev.Comment
                        command.Parameters.Add(New SqlParameter("@v11", SqlDbType.Int)).Value = Ev.MetadataID

                        If Ev.DataStructure.Length > 100 Then
                            Ev.DataStructure = Ev.DataStructure.Substring(0, 99)
                        End If
                        command.Parameters.Add(New SqlParameter("@v12", SqlDbType.Char)).Value = Ev.DataStructure

                        command.Parameters.Add(New SqlParameter("@v13", SqlDbType.VarChar)).Value = Ev.DataString
                        command.Parameters.Add(New SqlParameter("@v14", SqlDbType.Int)).Value = Ev.ServerID
                        command.Parameters.Add(New SqlParameter("@v15", SqlDbType.Int)).Value = Ev.MainPortID
                        command.Parameters.Add(New SqlParameter("@v16", SqlDbType.Int)).Value = Ev.SecondPortID
                        command.Parameters.Add(New SqlParameter("@v17", SqlDbType.Int)).Value = Ev.Seance
                        command.Parameters.Add(New SqlParameter("@v18", SqlDbType.Char)).Value = Ev.Field7
                        command.Parameters.Add(New SqlParameter("@v19", SqlDbType.Char)).Value = Ev.Field8
                        command.Parameters.Add(New SqlParameter("@v20", SqlDbType.DateTime)).Value = Ev.TransactionStartTime
                        command.Parameters.Add(New SqlParameter("@v21", SqlDbType.BigInt)).Value = Ev.TransactionMark

                        command.ExecuteNonQuery()
                        i += 1
                    Catch ex As Exception
                        Log.Error("Ошибка сохранения в БД записи от " + Ev.DateTime.ToString + _
                                           " по ИБ " + InfobaseName + " : " + ex.Message)
                    End Try


                Next

                Console.WriteLine(Now.ToShortTimeString + " Записано новых событий в базу " + i.ToString)

                command.CommandText = "IF NOT EXISTS (select * from [dbo].[Params] where [InfobaseCode] = @v3) " + _
                                    "INSERT INTO [dbo].[Params] ([InfobaseCode], [Position], [Filename], [LastEventID]) VALUES(@v3,@v1,@v2,@v4) " + _
                                    "ELSE UPDATE [dbo].[Params] SET [Position] = @v1, [Filename] = @v2, [LastEventID] = @v4 WHERE [InfobaseCode] = @v3"

                command.Parameters.Clear()
                command.Parameters.Add(New SqlParameter("@v1", SqlDbType.BigInt)).Value = CurrentPosition
                command.Parameters.Add(New SqlParameter("@v2", SqlDbType.Char)).Value = IIf(CurrentFilename = "", " ", CurrentFilename)
                command.Parameters.Add(New SqlParameter("@v3", SqlDbType.Int)).Value = InfobaseID
                command.Parameters.Add(New SqlParameter("@v4", SqlDbType.Int)).Value = IIf(LastEventNumber83 = 0, -1, LastEventNumber83)
                command.ExecuteNonQuery()

                command.CommandText = "COMMIT TRANSACTION"
                command.Parameters.Clear()
                command.ExecuteNonQuery()

                command.Dispose()
                objConn.Close()
                objConn.Dispose()

            ElseIf ItIsMySQL Then

                Dim objConn As New MySqlConnection(ConnectionString)
                objConn.Open()

                Dim command As New MySqlCommand("START TRANSACTION", objConn)
                command.ExecuteNonQuery()

                command.CommandText = "INSERT INTO `Events` (`InfobaseCode`,`DateTime`,`TransactionStatus`,`Transaction`,`UserName`,`ComputerName`" + _
                                          ",`AppName`,`Field2`,`EventID`,`EventType`,`Comment`,`MetadataID`,`DataStructure`,`DataString`" + _
                                          ",`ServerID`,`MainPortID`,`SecondPortID`,`Seance`,`Field7`,`Field8`,`TransactionStartTime`,`TransactionMark`)" + _
                                          " VALUES(@v0,@v1,@v2,@v3,@v4,@v5,@v6,@v7,@v8,@v9,@v10,@v11,@v12,@v13,@v14,@v15,@v16,@v17,@v18,@v19,@v20,@v21)"

                Dim i = 0
                For Each Ev In Events

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
                        command.Parameters.Add(New MySqlParameter("@v7", MySqlDbType.VarChar)).Value = Ev.Field2
                        command.Parameters.Add(New MySqlParameter("@v8", MySqlDbType.Int32)).Value = Ev.EventID
                        command.Parameters.Add(New MySqlParameter("@v9", MySqlDbType.VarChar)).Value = Ev.EventType
                        command.Parameters.Add(New MySqlParameter("@v10", MySqlDbType.VarChar)).Value = Ev.Comment
                        command.Parameters.Add(New MySqlParameter("@v11", MySqlDbType.Int32)).Value = Ev.MetadataID
                        command.Parameters.Add(New MySqlParameter("@v12", MySqlDbType.VarChar)).Value = Ev.DataStructure
                        command.Parameters.Add(New MySqlParameter("@v13", MySqlDbType.VarChar)).Value = Ev.DataString
                        command.Parameters.Add(New MySqlParameter("@v14", MySqlDbType.Int32)).Value = Ev.ServerID
                        command.Parameters.Add(New MySqlParameter("@v15", MySqlDbType.Int32)).Value = Ev.MainPortID
                        command.Parameters.Add(New MySqlParameter("@v16", MySqlDbType.Int32)).Value = Ev.SecondPortID
                        command.Parameters.Add(New MySqlParameter("@v17", MySqlDbType.Int32)).Value = Ev.Seance
                        command.Parameters.Add(New MySqlParameter("@v18", MySqlDbType.VarChar)).Value = Ev.Field7
                        command.Parameters.Add(New MySqlParameter("@v19", MySqlDbType.VarChar)).Value = Ev.Field8
                        command.Parameters.Add(New MySqlParameter("@v20", MySqlDbType.DateTime)).Value = Ev.TransactionStartTime
                        command.Parameters.Add(New MySqlParameter("@v21", MySqlDbType.Int64)).Value = Ev.TransactionMark

                        command.ExecuteNonQuery()
                        i += 1
                    Catch ex As Exception
                        Log.Error("Ошибка сохранения в БД записи от " + Ev.DateTime.ToString + _
                                           " по ИБ " + InfobaseName + " : " + ex.Message)
                    End Try


                Next

                Console.WriteLine(Now.ToShortTimeString + " Записано новых событий в базу " + i.ToString)

                command.CommandText = "REPLACE INTO `Params` (`InfobaseCode`, `Position`, `Filename`, `LastEventID`) VALUES(@v3,@v1,@v2,@v4)"

                command.Parameters.Clear()
                command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.Int64)).Value = CurrentPosition
                command.Parameters.Add(New MySqlParameter("@v2", MySqlDbType.VarChar)).Value = IIf(CurrentFilename = "", " ", CurrentFilename)
                command.Parameters.Add(New MySqlParameter("@v3", MySqlDbType.Int32)).Value = InfobaseID
                command.Parameters.Add(New MySqlParameter("@v4", MySqlDbType.Int32)).Value = IIf(LastEventNumber83 = 0, -1, LastEventNumber83)
                command.ExecuteNonQuery()

                command.CommandText = "COMMIT"
                command.Parameters.Clear()
                command.ExecuteNonQuery()

                command.Dispose()
                objConn.Close()
                objConn.Dispose()

            End If



            Events.Clear()

        End Sub

        Public Sub GetParamFromSQL()

            If ItIsMSSQL Then

                Dim objConn As New SqlConnection(ConnectionString)
                objConn.Open()

                Dim command As New SqlCommand("SELECT [Position],[Filename],[LastEventID] FROM [dbo].[Params] WHERE [InfobaseCode] = @v1", objConn)
                command.Parameters.Add(New SqlParameter("@v1", SqlDbType.Int)).Value = InfobaseID

                Dim rs = command.ExecuteReader

                While rs.Read
                    CurrentPosition = Convert.ToInt64(rs(0))
                    CurrentFilename = rs(1).ToString.Trim
                    LastEventNumber83 = rs(2)
                End While

                rs.Close()
                command.Dispose()
                objConn.Close()
                objConn.Dispose()

            ElseIf ItIsMySQL Then

                Dim objConn = New MySql.Data.MySqlClient.MySqlConnection(ConnectionString)
                objConn.Open()


                Dim command As New MySqlCommand("SELECT `Position`,`Filename`,`LastEventID` FROM `Params` WHERE `InfobaseCode` = @v1", objConn)
                command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.Int32)).Value = InfobaseID

                Dim rs = command.ExecuteReader

                While rs.Read
                    CurrentPosition = Convert.ToInt64(rs(0))
                    CurrentFilename = rs(1).ToString.Trim
                    LastEventNumber83 = rs(2)
                End While

                rs.Close()
                command.Dispose()
                objConn.Close()
                objConn.Dispose()

            End If



        End Sub

    End Class

    Class InfoBaseEventlog

        Public Catalog As String
        Public Log As NLog.Logger
        Public Name As String
        Public ConnectionString As String
        Public ItIsMSSQL As Boolean = False
        Public ItIsMySQL As Boolean = False
        Public Guid As String
        Public ID As Integer = 0
        Public Reference As ReferenceClass
        Public Events As EventLogLoader
        'Dim Parser As New Parser
        Public SleepTime As Integer = 60 * 1000 '1 минут

        Public Sub PrepareInfobaseID()

            'У нас есть ГУИД базы, но нет сквозного кода для информации.
            'Сделано для оптимизации, чтобы в каждой строке события не записывать ГУИД, а хранить целое число.

            If ItIsMSSQL Then

                Dim objConn As New SqlConnection(ConnectionString)
                objConn.Open()

                Dim command As New SqlCommand("SELECT [Code] FROM [dbo].[Infobases] WHERE [Guid] = @v1", objConn)
                command.Parameters.Add(New SqlParameter("@v1", SqlDbType.Char)).Value = Guid

                Dim rs = command.ExecuteReader

                If rs.Read Then
                    ID = Convert.ToInt32(rs(0))
                End If
                rs.Close()

                If ID = 0 Then

                    command.CommandText = "INSERT INTO Infobases ([Code],[Name],[guid])" + _
                                         " SELECT MAX(f) AS [Code], @v0 as [Name], @v1 as [guid] FROM " + _
                                         " (SELECT MAX(Code) + 1 AS f FROM Infobases UNION ALL" + _
                                         " SELECT 1 AS Expr1) AS T"
                    command.Parameters.Clear()
                    command.Parameters.Add(New SqlParameter("@v0", SqlDbType.Char)).Value = Name
                    command.Parameters.Add(New SqlParameter("@v1", SqlDbType.Char)).Value = Guid
                    command.ExecuteNonQuery()

                    command.CommandText = "SELECT [Code] FROM [dbo].[Infobases] WHERE [Guid] = @v1"
                    rs = command.ExecuteReader()

                    If rs.Read Then
                        ID = Convert.ToInt32(rs(0))
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
                command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.VarChar)).Value = Guid

                Dim rs = command.ExecuteReader

                If rs.Read Then
                    ID = Convert.ToInt32(rs(0))
                End If
                rs.Close()

                If ID = 0 Then

                    command.CommandText = "INSERT INTO Infobases (`Code`,`Name`,`guid`)" + _
                                         " SELECT MAX(f) AS `Code`, @v0 as `Name`, @v1 as `guid` FROM " + _
                                         " (SELECT MAX(Code) + 1 AS f FROM `Infobases` UNION ALL" + _
                                         " SELECT 1 AS `Expr1`) AS T"
                    command.Parameters.Clear()
                    command.Parameters.Add(New MySqlParameter("@v0", MySqlDbType.VarChar)).Value = Name
                    command.Parameters.Add(New MySqlParameter("@v1", MySqlDbType.VarChar)).Value = Guid
                    command.ExecuteNonQuery()

                    command.CommandText = "SELECT `Code` FROM `Infobases` WHERE `Guid` = @v1"
                    rs = command.ExecuteReader()

                    If rs.Read Then
                        ID = Convert.ToInt32(rs(0))
                    End If

                    rs.Close()

                End If

                command.Dispose()
                objConn.Close()
                objConn.Dispose()

            End If

            '--------------------------------------------------------------
        End Sub

        Public Sub Initialize()

            Events = New EventLogLoader
            Events.InfobaseID = ID
            Events.InfobaseName = Name
            Events.ConnectionString = ConnectionString
            Events.Log = Log
            Events.ItIsMSSQL = ItIsMSSQL
            Events.ItIsMySQL = ItIsMySQL
            Events.GetParamFromSQL()

        End Sub

        Public Sub LoadReference()

            Try
                Dim FileName = Path.Combine(Catalog, "1Cv8.lgf")

                If My.Computer.FileSystem.FileExists(FileName) Then

                    Dim FI = My.Computer.FileSystem.GetFileInfo(FileName)

                    If FI.LastWriteTime >= Reference.LastUpdate Then

                        Dim FS As FileStream = New FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                        Dim SR As StreamReader = New StreamReader(FS)

                        'Dim TextFile = My.Computer.FileSystem.OpenTextFileReader(FileName)
                        'Dim Text = TextFile.ReadToEnd()
                        'TextFile.Close()
                        Dim Text = SR.ReadToEnd()
                        SR.Close()
                        FS.Close()


                        Dim ArrayLines = Parser.ParserServices.ParsesClass.ParseString(Text)

                        Dim i = 0
                        For Each a In ArrayLines
                            If Not a Is Nothing Then
                                Select Case a(1)
                                    Case "1"
                                        Reference.AddUser(Convert.ToInt32(a(4)), a(2), a(3))
                                    Case "2"
                                        Reference.AddComputer(Convert.ToInt32(a(3)), a(2))
                                    Case "3"
                                        Reference.AddApplication(Convert.ToInt32(a(3)), a(2))
                                    Case "4"
                                        Reference.AddEvent(Convert.ToInt32(a(3)), a(2))
                                    Case "5"
                                        Reference.AddMetadata(Convert.ToInt32(a(4)), a(2), a(3))
                                    Case "6"
                                        Reference.AddServer(Convert.ToInt32(a(3)), a(2))
                                    Case "7"
                                        Reference.AddMainPort(Convert.ToInt32(a(3)), a(2))
                                    Case "8"
                                        Reference.AddSecondPort(Convert.ToInt32(a(3)), a(2))
                                        'Case "9" - не видел этих в файле
                                        'Case "10"
                                    Case "11"
                                    Case "12"
                                    Case "13"
                                        'в числе последних трех должны быть статус транзакци и важность
                                    Case Else

                                End Select

                            End If
                        Next

                        Reference.SaveReferenceToSQL()

                    End If

                End If
            Catch ex As Exception
                Log.ErrorException("Ошибка при работе со справочником", ex)
            End Try


            Try

                Dim FileName = Path.Combine(Catalog, "1Cv8.lgd")

                If My.Computer.FileSystem.FileExists(FileName) Then

                    Dim Conn = New System.Data.SQLite.SQLiteConnection("Data Source=" + FileName)
                    Conn.Open()
                    Dim Command = New System.Data.SQLite.SQLiteCommand
                    Command.Connection = Conn

                    Command.CommandText = "SELECT [code], [name] FROM [AppCodes]"
                    Dim rs = Command.ExecuteReader
                    While rs.Read
                        Reference.AddApplication(rs(0), rs(1))
                    End While
                    rs.Close()

                    Command.CommandText = "SELECT [code], [name] FROM [ComputerCodes]"
                    rs = Command.ExecuteReader
                    While rs.Read
                        Reference.AddComputer(rs(0), rs(1))
                    End While
                    rs.Close()

                    Command.CommandText = "SELECT [code], [name] FROM [EventCodes]"
                    rs = Command.ExecuteReader
                    While rs.Read
                        Reference.AddEvent(rs(0), rs(1))
                    End While
                    rs.Close()

                    Command.CommandText = "SELECT [code], [uuid], [name] FROM [UserCodes]"
                    rs = Command.ExecuteReader
                    While rs.Read
                        Reference.AddUser(rs(0), rs(1), rs(2))
                    End While
                    rs.Close()

                    Command.CommandText = "SELECT [code], [name] FROM [WorkServerCodes]"
                    rs = Command.ExecuteReader
                    While rs.Read
                        Reference.AddServer(rs(0), rs(1))
                    End While
                    rs.Close()

                    Command.CommandText = "SELECT [code], [uuid], [name] FROM [MetadataCodes]"
                    rs = Command.ExecuteReader
                    While rs.Read
                        Reference.AddMetadata(rs(0), rs(1), rs(2))
                    End While
                    rs.Close()

                    Command.CommandText = "SELECT [code], [name] FROM [PrimaryPortCodes]"
                    rs = Command.ExecuteReader
                    While rs.Read
                        Reference.AddMainPort(rs(0), rs(1))
                    End While
                    rs.Close()

                    Command.CommandText = "SELECT [code], [name] FROM [SecondaryPortCodes]"
                    rs = Command.ExecuteReader
                    While rs.Read
                        Reference.AddSecondPort(rs(0), rs(1))
                    End While
                    rs.Close()

                    Command.Dispose()
                    Conn.Close()
                    Conn.Dispose()

                    Reference.SaveReferenceToSQL()

                End If

            Catch ex As Exception
                Log.ErrorException("Ошибка при работе со справочником", ex)
            End Try

        End Sub

        Sub FindAndStartParseFiles()

            Dim v83File = Path.Combine(Catalog, "1Cv8.lgd")
            If My.Computer.FileSystem.FileExists(v83File) Then

                LoadEvents83(v83File)

                Events.SaveEventsToSQL()

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
                                If FI.Name >= Events.CurrentFilename Then
                                    If Not FI.Name = Events.CurrentFilename Then
                                        Events.CurrentPosition = 61 ' start position for log-file 1C
                                    End If
                                    Events.CurrentFilename = FI.Name
                                    LoadEvents(File)
                                End If
                            Catch ex As Exception
                                Log.ErrorException("Ошибка в FindAndStartParseFiles", ex)
                            End Try
                        End If

                    Next

                    Events.SaveEventsToSQL()

                End If
            End If




        End Sub

        Sub LoadEvents83(FileName As String)
            Try

                Dim Conn = New System.Data.SQLite.SQLiteConnection("Data Source=" + FileName)
                Conn.Open()
                Dim Command = New System.Data.SQLite.SQLiteCommand
                Command.Connection = Conn

                Dim ANSI = Text.Encoding.GetEncoding(1251)
                Dim UTF8 = Text.Encoding.UTF8

                While True

                    Command.CommandText = "SELECT [rowID],[severity],[date],[connectID],[session]" + _
                                        ",[transactionStatus],[transactionDate],[transactionID],[userCode],[computerCode],[appCode]" + _
                                        ",[eventCode],[comment],[metadataCodes],[sessionDataSplitCode],[dataType],[data]" + _
                                        ",[dataPresentation],[workServerCode],[primaryPortCode],[secondaryPortCode]" + _
                                        " FROM [EventLog] WHERE [rowID] > " + Events.LastEventNumber83.ToString + " LIMIT 1000"
                    Dim rs = Command.ExecuteReader

                    While rs.Read

                        Dim OneEvent = New EventLogLoader.OneEvent
                        OneEvent.RowID = rs("rowID")
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

                        OneEvent.DataString = rs("dataPresentation")
                        OneEvent.ServerID = rs("workServerCode")
                        OneEvent.MainPortID = rs("primaryPortCode")
                        OneEvent.SecondPortID = rs("secondaryPortCode")
                        OneEvent.Seance = rs("session")

                        OneEvent.Transaction = ""
                        OneEvent.Field2 = ""
                        OneEvent.EventType = ""
                        OneEvent.Field7 = ""
                        OneEvent.Field8 = ""



                        Events.Events.Add(OneEvent)

                        Events.LastEventNumber83 = OneEvent.RowID

                    End While
                    rs.Close()

                    If Events.Events.Count = 0 Then
                        Exit While
                    End If

                    Events.SaveEventsToSQL()

                End While




                Command.Dispose()
                Conn.Close()
                Conn.Dispose()

            Catch ex As Exception
                Log.ErrorException("Ошибка при работе с таблицей событий", ex)
            End Try


        End Sub

        Sub LoadEvents(FileName As String)

            Dim FS As FileStream = New FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
            FS.Position = Events.CurrentPosition

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
            Dim Position = Events.CurrentPosition
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

                Events.CurrentPosition = Position

                If NewLine Then
                    StrEvent = TextLine
                Else
                    StrEvent = StrEvent + vbNewLine + TextLine
                End If

                If ItsEndOfEvent(TextLine, CountBracket) Then
                    NewLine = True
                    If Not StrEvent Is Nothing Then
                        Try
                            Events.AddEvent(StrEvent)
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

            Events.SaveEventsToSQL()

            SR.Close()
            fs.Close()

        End Sub

        Function ItsEndOfEvent(Str As String, ByRef Count As Integer)

            ItsEndOfEvent = False

            Dim TempStr = Str
            Dim TextBlockOpen = False

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

        Public Sub DoWork()

            Try
                Reference = New ReferenceClass
                Reference.InfobaseGuid = Guid
                Reference.InfobaseID = ID
                Reference.InfobaseName = Name
                Reference.ConnectionString = ConnectionString
                Reference.ItIsMSSQL = ItIsMSSQL
                Reference.ItIsMySQL = ItIsMySQL

            Catch ex As Exception
                Log.ErrorException("Ошибка создания объекта справочника для ИБ (" + Name + ")", ex)
            End Try


            While True

                Console.WriteLine(Now.ToShortTimeString + " Запуск итерации проверки новых событий")
                Try

                    LoadReference()

                    Initialize()

                    FindAndStartParseFiles()

                Catch ex As Exception
                    Log.ErrorException("Ошибка обработки файлов событий ИБ (" + Name + ")", ex)
                End Try

                Threading.Thread.Sleep(SleepTime)

            End While


        End Sub

    End Class

    Dim ArrayIB() As InfoBaseEventlog
    Dim ArrayThread() As Threading.Thread
    Dim Param As IniFile.IniFileClass
    Dim ConnectionString As String
    Dim DBType As String
    Dim ItIsMSSQL As Boolean = False
    Dim ItIsMySQL As Boolean = False
    Public SleepTime As Integer = 60 * 1000 '1 минута

    Function LoadIniFileParams()

        LoadIniFileParams = False

        Try
            Param = New IniFile.IniFileClass

            Dim PathIniFile = Path.Combine(My.Application.Info.DirectoryPath, "setting.ini")
            If My.Computer.FileSystem.FileExists(PathIniFile) Then

                Param.Load(PathIniFile)

                Dim s As String

                s = Param.RestoreIniValue(Param, "GlobalValues", "ConnectionString")
                If Not s = "" Then
                    ConnectionString = s
                End If

                s = Param.RestoreIniValue(Param, "GlobalValues", "DBType")
                If Not s = "" Then
                    DBType = s

                    If DBType = "MySQL" Then
                        ItIsMySQL = True
                    ElseIf DBType = "MS SQL Server" Then
                        ItIsMSSQL = True
                    End If
                End If

                Try
                    s = Param.RestoreIniValue(Param, "GlobalValues", "RepeatTime")
                    If Not s = "" Then
                        SleepTime = Convert.ToInt32(s) * 1000
                    End If
                Catch ex As Exception
                End Try

                Dim CountStr = Param.RestoreIniValue(Param, "GlobalValues", "DatabaseCount")
                Dim Count As Integer = Convert.ToInt32(IIf(CountStr = "", "0", CountStr))

                For i = 1 To Count

                    Dim IB_ID = Param.RestoreIniValue(Param, "Databases", "DatabaseID" + i.ToString)
                    Dim IB_Name = Param.RestoreIniValue(Param, "Databases", "DatabaseName" + i.ToString)
                    Dim IB_Catalog = Param.RestoreIniValue(Param, "Databases", "DatabaseCatalog" + i.ToString)

                    Dim IB = New InfoBaseEventlog
                    IB.Log = NLog.LogManager.GetLogger("CurrentThread")
                    IB.Guid = IB_ID
                    IB.Name = IB_Name
                    IB.Catalog = IB_Catalog
                    IB.ConnectionString = ConnectionString
                    IB.SleepTime = SleepTime
                    IB.ItIsMSSQL = ItIsMSSQL
                    IB.ItIsMySQL = ItIsMySQL

                    ReDim Preserve ArrayIB(i - 1)
                    ArrayIB(i - 1) = IB

                Next


                LoadIniFileParams = True
            Else
                Log.Error("Файл setting.ini не найден")
            End If
        Catch ex As Exception
            Log.ErrorException("Ошибка загрузки параметров из ini-файла", ex)
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

        If Not LoadIniFileParams() Then
            Log.Error("Ошибка работы с файлом параметров setting.ini в каталоге приложения")
            Environment.Exit(-1)
        End If

        CreateTables()

        Dim i = 0

        For Each IB In ArrayIB

            Try
                IB.PrepareInfobaseID()
            Catch ex As Exception
                Log.ErrorException("Ошибка получения ID ИБ из БД (" + IB.Name + ")", ex)
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
                Log.ErrorException("Ошибка запуска потока для обслуживания ИБ (" + IB.Name + ")", ex)
            End Try

        Next

    End Sub


    Sub CreateTables()

        Try
            If ItIsMSSQL Then

                Dim objConn As New SqlConnection(ConnectionString)
                objConn.Open()

                Dim command As New SqlCommand("IF NOT EXISTS (select * from sysobjects where id = object_id(N'Events'))" + vbNewLine + _
                                              "	CREATE TABLE [dbo].[Events]([InfobaseCode] int NOT NULL, [DateTime] [datetime] NOT NULL,	" + _
                                              "		[TransactionStatus] [char](1) NULL,	" + _
                                              "		[TransactionStartTime] [datetime] NULL,	" + _
                                              "		[TransactionMark] bigint NULL,	" + _
                                              "		[Transaction] [char](100) NULL,	" + _
                                              "		[UserName] int NULL,	" + _
                                              "		[ComputerName] int NULL,	" + _
                                              "		[AppName] int NULL,	" + _
                                              "     [Field2] [char](100) NULL,	" + _
                                              "		[EventID] int NULL,	" + _
                                              "		[EventType] [char](1) NULL,	" + _
                                              "		[Comment] [varchar](max) NULL,	" + _
                                              "		[MetadataID] int NULL,	" + _
                                              "		[DataStructure] [char](100) NULL,	" + _
                                              "		[DataString] [varchar](max) NULL,	" + _
                                              "		[ServerID] int NULL,	" + _
                                              "		[MainPortID] int NULL,	" + _
                                              "		[SecondPortID] int NULL,	" + _
                                              "		[Seance] int NULL,	" + _
                                              "		[Field7] [char](100) NULL,	" + _
                                              "		[Field8] [char](100) NULL)", objConn)
                command.ExecuteNonQuery()

                '**********************************************************************************
                command.CommandText = "IF NOT EXISTS (select * from sysobjects where id = object_id(N'Params'))" + vbNewLine + _
                                    "	CREATE TABLE [dbo].[Params] ([InfobaseCode] int NOT NULL, [Position] bigint, [Filename] [char](100), [LastEventID] int);" + vbNewLine + _
                                    " IF NOT EXISTS (select * from sys.indexes where object_id = object_id(N'Params') AND Name = 'ClusteredIndex')" + vbNewLine + _
                                    " CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex] ON [dbo].[Params] ([InfobaseCode] ASC);"
                command.ExecuteNonQuery()
                '**********************************************************************************
                command.CommandText = "IF NOT EXISTS (select * from sysobjects where id = object_id(N'Infobases'))" + vbNewLine + _
                                    "	CREATE TABLE [dbo].[Infobases] ([Guid] [char](40) NOT NULL, [Code] int NOT NULL, [Name] [char](100))" + vbNewLine + _
                                    " IF NOT EXISTS (select * from sys.indexes where object_id = object_id(N'Infobases') AND Name = 'ClusteredIndex')" + vbNewLine + _
                                    " CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex] ON [dbo].[Infobases] ([Guid] ASC);"
                command.ExecuteNonQuery()
                '**********************************************************************************
                command.CommandText =
                    "IF NOT EXISTS (select * from sysobjects where id = object_id(N'Users'))" + vbNewLine + _
                    "CREATE TABLE [dbo].[Users]([InfobaseCode] int NOT NULL, [Code] int NOT NULL, [Name] [char](100), [Guid] [char](40));" + vbNewLine + _
                    "IF NOT EXISTS (select * from sys.indexes where object_id = object_id(N'Users') AND Name = 'ClusteredIndex')" + vbNewLine + _
                    "CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex] ON [dbo].[Users] ([InfobaseCode] ASC, [Code] ASC);" + vbNewLine + _
                    "" + _
                    "IF NOT EXISTS (select * from sysobjects where id = object_id(N'Metadata'))" + vbNewLine + _
                    "CREATE TABLE [dbo].[Metadata]([InfobaseCode] int NOT NULL, [Code] int NOT NULL, [Name] [char](100), [Guid] [char](40));" + vbNewLine + _
                    "IF NOT EXISTS (select * from sys.indexes where object_id = object_id(N'Metadata') AND Name = 'ClusteredIndex')" + vbNewLine + _
                    "CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex] ON [dbo].[Metadata] ([InfobaseCode] ASC, [Code] ASC);" + vbNewLine + _
                    "" + _
                    "IF NOT EXISTS (select * from sysobjects where id = object_id(N'Computers'))" + vbNewLine + _
                    "CREATE TABLE [dbo].[Computers]([InfobaseCode] int NOT NULL, [Code] int NOT NULL, [Name] [char](100));" + vbNewLine + _
                    "IF NOT EXISTS (select * from sys.indexes where object_id = object_id(N'Computers') AND Name = 'ClusteredIndex')" + vbNewLine + _
                    "CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex] ON [dbo].[Computers] ([InfobaseCode] ASC, [Code] ASC);" + vbNewLine + _
                    "" + _
                    "IF NOT EXISTS (select * from sysobjects where id = object_id(N'Applications'))" + vbNewLine + _
                    "CREATE TABLE [dbo].[Applications]([InfobaseCode] int NOT NULL, [Code] int NOT NULL,[Name] [char](100));" + vbNewLine + _
                    "IF NOT EXISTS (select * from sys.indexes where object_id = object_id(N'Applications') AND Name = 'ClusteredIndex')" + vbNewLine + _
                    "CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex] ON [dbo].[Applications] ([InfobaseCode] ASC, [Code] ASC);" + vbNewLine + _
                    "" + _
                    "IF NOT EXISTS (select * from sysobjects where id = object_id(N'EventsType'))" + vbNewLine + _
                    "CREATE TABLE [dbo].[EventsType]([InfobaseCode] int NOT NULL, [Code] int NOT NULL, [Name] [char](100));" + vbNewLine + _
                    "IF NOT EXISTS (select * from sys.indexes where object_id = object_id(N'EventsType') AND Name = 'ClusteredIndex')" + vbNewLine + _
                    "CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex] ON [dbo].[EventsType] ([InfobaseCode] ASC, [Code] ASC);" + vbNewLine + _
                    "" + _
                    "IF NOT EXISTS (select * from sysobjects where id = object_id(N'Servers'))" + vbNewLine + _
                    "CREATE TABLE [dbo].[Servers]([InfobaseCode] int NOT NULL, [Code] int NOT NULL, [Name] [char](100));" + vbNewLine + _
                    "IF NOT EXISTS (select * from sys.indexes where object_id = object_id(N'Servers') AND Name = 'ClusteredIndex')" + vbNewLine + _
                    "CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex] ON [dbo].[Servers] ([InfobaseCode] ASC, [Code] ASC);" + vbNewLine + _
                    "" + _
                    "IF NOT EXISTS (select * from sysobjects where id = object_id(N'MainPorts'))" + vbNewLine + _
                    "CREATE TABLE [dbo].[MainPorts]([InfobaseCode] int NOT NULL, [Code] int NOT NULL, [Name] [char](100));" + vbNewLine + _
                    "IF NOT EXISTS (select * from sys.indexes where object_id = object_id(N'MainPorts') AND Name = 'ClusteredIndex')" + vbNewLine + _
                    "CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex] ON [dbo].[MainPorts] ([InfobaseCode] ASC, [Code] ASC);" + vbNewLine + _
                    "" + _
                    "IF NOT EXISTS (select * from sysobjects where id = object_id(N'SecondPorts'))" + vbNewLine + _
                    "CREATE TABLE [dbo].[SecondPorts]([InfobaseCode] int NOT NULL, [Code] int NOT NULL, [Name] [char](100));" + vbNewLine + _
                    "IF NOT EXISTS (select * from sys.indexes where object_id = object_id(N'SecondPorts') AND Name = 'ClusteredIndex')" + vbNewLine + _
                    "CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex] ON [dbo].[SecondPorts] ([InfobaseCode] ASC, [Code] ASC);"

                command.ExecuteNonQuery()
                '**********************************************************************************

                command.CommandText = "SELECT TOP 1 * FROM Events"
                command.ExecuteReader()

                command.Dispose()
                objConn.Close()

            ElseIf ItIsMySQL Then

                Dim objConn = New MySql.Data.MySqlClient.MySqlConnection(ConnectionString)
                objConn.Open()

                Dim DBName = objConn.Database

                Dim command = New MySql.Data.MySqlClient.MySqlCommand
                command.Connection = objConn

                command.CommandText = "CREATE TABLE IF NOT EXISTS `Events` (`InfobaseCode` int(11) NOT NULL, `DateTime` int(11) NOT NULL," + _
                    "`TransactionStatus` varchar(1) NULL, `TransactionStartTime` datetime NULL,	" + _
                    "`TransactionMark` bigint NULL, `Transaction` varchar(100) NULL,	`UserName` int(11) NULL, `ComputerName` int(11) NULL,	" + _
                    "`AppName` int(11) NULL, `Field2` varchar(100) NULL,	`EventID` int(11) NULL, `EventType` varchar(1) NULL,	" + _
                    "`Comment` text NULL, `MetadataID` int(11) NULL,	`DataStructure` varchar(100) NULL, `DataString` text NULL,	" + _
                    "`ServerID` int(11) NULL, `MainPortID` int(11) NULL,	`SecondPortID` int(11) NULL, `Seance` int(11) NULL,	" + _
                    "`Field7` varchar(100) NULL, `Field8` varchar(100) NULL" + _
                    ") ENGINE=InnoDB DEFAULT CHARSET=utf8;"
                command.ExecuteNonQuery()
                '**********************************************************************************
                command.CommandText = "CREATE TABLE IF NOT EXISTS `Params` (`InfobaseCode` int(11) NOT NULL, `Position` bigint, `Filename` varchar(100)," + _
                                        "`LastEventID` int(11), PRIMARY KEY `InfobaseCode` (`InfobaseCode`));"
                command.ExecuteNonQuery()
                '**********************************************************************************
                command.CommandText = "CREATE TABLE IF NOT EXISTS `Infobases` (`Guid` varchar(40) NOT NULL, `Code` int(11) NOT NULL, `Name` varchar(100)," + _
                                        "PRIMARY KEY `Guid` (`Guid`));"
                command.ExecuteNonQuery()
                '**********************************************************************************
                command.CommandText =
                    "CREATE TABLE IF NOT EXISTS `Users`(`InfobaseCode` int(11) NOT NULL, `Code` int(11) NOT NULL, `Name` varchar(100), `Guid` varchar(40), PRIMARY KEY (`InfobaseCode`, `Code`));" + vbNewLine + _
                    "" + _
                    "CREATE TABLE IF NOT EXISTS `Metadata`(`InfobaseCode` int(11) NOT NULL, `Code` int(11) NOT NULL, `Name` varchar(100), `Guid` varchar(40), PRIMARY KEY (`InfobaseCode`, `Code`));" + vbNewLine + _
                    "" + _
                    "CREATE TABLE IF NOT EXISTS `Computers`(`InfobaseCode` int(11) NOT NULL, `Code` int(11) NOT NULL, `Name` varchar(100), PRIMARY KEY (`InfobaseCode`, `Code`));" + vbNewLine + _
                    "" + _
                    "CREATE TABLE IF NOT EXISTS `Applications`(`InfobaseCode` int(11) NOT NULL, `Code` int(11) NOT NULL, `Name` varchar(100), PRIMARY KEY (`InfobaseCode`, `Code`));" + vbNewLine + _
                    "" + _
                    "CREATE TABLE IF NOT EXISTS `EventsType`(`InfobaseCode` int(11) NOT NULL, `Code` int(11) NOT NULL, `Name` varchar(100), PRIMARY KEY (`InfobaseCode`, `Code`));" + vbNewLine + _
                    "" + _
                    "CREATE TABLE IF NOT EXISTS `Servers`(`InfobaseCode` int(11) NOT NULL, `Code` int(11) NOT NULL, `Name` varchar(100), PRIMARY KEY (`InfobaseCode`, `Code`));" + vbNewLine + _
                    "" + _
                    "CREATE TABLE IF NOT EXISTS `MainPorts`(`InfobaseCode` int(11) NOT NULL, `Code` int(11) NOT NULL, `Name` varchar(100), PRIMARY KEY (`InfobaseCode`, `Code`));" + vbNewLine + _
                    "" + _
                    "CREATE TABLE IF NOT EXISTS `SecondPorts`(`InfobaseCode` int(11) NOT NULL, `Code` int(11) NOT NULL, `Name` varchar(100), PRIMARY KEY (`InfobaseCode`, `Code`));"

                command.ExecuteNonQuery()
                '**********************************************************************************



                command.Dispose()
                objConn.Close()


            End If

            Log.Info("Проверка таблиц в БД выполнена успешно!")

        Catch ex As Exception

            Log.ErrorException("Ошибка при проверке таблиц в БД", ex)

        End Try

    End Sub


End Class
