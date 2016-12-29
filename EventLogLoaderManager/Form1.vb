Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Management
Imports System.Data.SqlClient
Imports MySql.Data.MySqlClient
Imports System.Net
Imports EventLogLoaderService

Public Class Form1

    Dim ArrayServices() As ServiceDescriptionClass
    Dim PathConfigFile = Path.Combine(My.Application.Info.DirectoryPath, "config.json")
    Dim GroupExtraPath As ListViewGroup

    Dim ConfigSetting As ConfigSetting = New ConfigSetting

    Sub LoadConfigSetting()

        ConfigSetting = ConfigSettingsModule.LoadConfigSettingFromFile(PathConfigFile)

        If ConfigSetting.RepeatTime = 0 Then
            ConfigSetting.RepeatTime = 60
        End If

        If String.IsNullOrEmpty(ConfigSetting.ESIndexName) Then
            ConfigSetting.ESIndexName = "event-log"
        End If

        If String.IsNullOrEmpty(ConfigSetting.DBType) Then
            ConfigSetting.DBType = "ElasticSearch"
            If String.IsNullOrEmpty(ConfigSetting.ConnectionString) Then
                ConfigSetting.ConnectionString = "http://localhost:9200/"
            End If
        End If

        ConnectionStringBox.Text = ConfigSetting.ConnectionString
        DBType.Text = ConfigSetting.DBType
        RepeatTime.Text = ConfigSetting.RepeatTime.ToString
        ESIndexNameTextBox.Text = ConfigSetting.ESIndexName

    End Sub


    Function FindInfobase(Guid As String) As ServiceDescriptionClass.Infobases

        FindInfobase = New ServiceDescriptionClass.Infobases

        For Each Srv In ArrayServices
            If Not Srv.ArrayInfobases Is Nothing Then
                For Each IB In Srv.ArrayInfobases
                    If IB.GUID = Guid Then

                        Return IB

                    End If
                Next
            End If
        Next

    End Function

    Function FindInfobaseInSavedParams(Guid As String) As InfobaseSetting

        For Each IB In ConfigSetting.Infobases
            If IB.DatabaseID = Guid Then
                IB.Found = True
                Return IB
            End If
        Next

        Return New InfobaseSetting

    End Function

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click

        Dim ConfigSetting = New ConfigSetting
        ConfigSetting.ConnectionString = ConnectionStringBox.Text.Trim
        ConfigSetting.DBType = DBType.Text.Trim

        For Each Item As ListViewItem In ListView.Items
            If Item.Checked Then

                Dim IBSetting = New ConfigSettingsModule.InfobaseSetting
                IBSetting.DatabaseID = Item.SubItems(1).Text
                IBSetting.DatabaseName = Item.SubItems(0).Text
                IBSetting.DatabaseCatalog = Item.SubItems(4).Text
                IBSetting.ESServerName = Item.SubItems(5).Text

                ConfigSetting.Infobases.Add(IBSetting)

            End If
        Next


        Dim Rep = Convert.ToInt32(RepeatTime.Text)

        ConfigSetting.RepeatTime = IIf(Rep = 0, 60, Rep)
        ConfigSettingsModule.SaveConfigSettingToFile(ConfigSetting, PathConfigFile)


        Dim sc = New System.ServiceProcess.ServiceController("EventLog loader service")
        Try
            If sc.Status = ServiceProcess.ServiceControllerStatus.Running Then
                If MsgBox("Параметры успешно изменены, но служба в настоящий момент работает." +
                          vbNewLine + "Перезапустить службу для применения изменений?", MsgBoxStyle.YesNo, Text) = MsgBoxResult.Yes Then
                    sc.Stop()
                    sc.WaitForStatus(ServiceProcess.ServiceControllerStatus.Stopped)
                    sc.Start()
                    sc.WaitForStatus(ServiceProcess.ServiceControllerStatus.Running)
                End If
            Else
                MsgBox("Параметры успешно изменены.", MsgBoxStyle.OkOnly, Text)
            End If
        Catch ex As Exception
            MsgBox("Параметры успешно изменены.", MsgBoxStyle.OkOnly, Text)
        End Try


    End Sub


    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click

        Dim PathName = Path.Combine(My.Application.Info.DirectoryPath, "EventLogLoaderService.exe")
        Dim ServName = "EventLog loader service"
        Dim DisplayName = "EventLog loader service"
        Dim lpDependencies = "Tcpip"
        Dim User = "LocalSystem"
        Dim Pwd = ""

        If Not ObjTec.Services.ServiceInstaller.InstallService(PathName, ServName, DisplayName, lpDependencies, User, Pwd) Then

            Dim ErrorCode = Marshal.GetLastWin32Error()
            MsgBox("Ошибка установки службы Windows: " + ObjTec.Services.ServiceInstaller.GetErrorDescription(ErrorCode), , Text)

        Else

            MsgBox("Служба Windows успешно установлена!")

        End If


    End Sub

    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click

        Dim ServName = "EventLog loader service"

        If Not ObjTec.Services.ServiceInstaller.UninstallService(ServName) Then

            Dim ErrorCode = Marshal.GetLastWin32Error()
            MsgBox("Ошибка удаления службы Windows: " + ObjTec.Services.ServiceInstaller.GetErrorDescription(ErrorCode), , Text)

        Else

            MsgBox("Служба Windows успешно удалена")

        End If

    End Sub

    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Text = My.Application.Info.ProductName + " / " + My.Application.Info.Copyright

        LoadConfigSetting()

        RefreshInfobaseList()

    End Sub


    Sub RefreshInfobaseList()

        ListView.Items.Clear()

        Dim search As New ManagementObjectSearcher("SELECT * FROM Win32_Service WHERE PathName like '%ragent.exe%'")

        'Dim info

        Dim i = 0
        For Each info As ManagementObject In search.Get()

            Dim PathName As String = info("PathName")
            Dim DisplayName As String = info("DisplayName")

            Dim Serv = New ServiceDescriptionClass
            Serv.Name = info("Name")
            Serv.DisplayName = info("DisplayName")
            Serv.Description = info("Description")
            Serv.PathName = info("PathName")
            Serv.ParsePath()
            Serv.GetInfobases()

            ReDim Preserve ArrayServices(i)
            ArrayServices(i) = Serv
            i = i + 1

            Dim Group = New ListViewGroup(Serv.DisplayName + " (порт " + Serv.PortAgent.ToString + ")")
            ListView.Groups.Add(Group)

            If Not Serv.ArrayInfobases Is Nothing Then
                For Each a In Serv.ArrayInfobases

                    Dim SavedIB As InfobaseSetting = FindInfobaseInSavedParams(a.GUID)

                    Dim item1 = New ListViewItem(a.Name, Group)
                    item1.Checked = (Not String.IsNullOrEmpty(SavedIB.DatabaseID))
                    item1.SubItems.Add(a.GUID)
                    item1.SubItems.Add(a.Description)
                    item1.SubItems.Add(a.SizeEventLog.ToString)
                    item1.SubItems.Add(a.CatalogEventLog)
                    item1.SubItems.Add(SavedIB.ESServerName) 'ESServerName

                    ListView.Items.Add(item1)

                Next
            End If
        Next

        'Загрузим файловые базы
        Dim FileIbases = LoadFileInfobasesList()
        If Not FileIbases Is Nothing Then
            Dim Group = New ListViewGroup("Файловые базы данных текущего пользователя")
            ListView.Groups.Add(Group)

            For Each a In FileIbases

                Dim SavedIB As InfobaseSetting = FindInfobaseInSavedParams(a.GUID)

                Dim item1 = New ListViewItem(a.Name, Group)
                item1.Checked = (Not String.IsNullOrEmpty(SavedIB.DatabaseID))
                item1.SubItems.Add(a.GUID)
                item1.SubItems.Add(a.Description)
                item1.SubItems.Add(a.SizeEventLog.ToString)
                item1.SubItems.Add(a.CatalogEventLog)
                item1.SubItems.Add(SavedIB.ESServerName) 'ESServerName

                ListView.Items.Add(item1)

            Next

        End If

        GroupExtraPath = New ListViewGroup("Дополнительные пути для загрузки событий из ЖР")
        ListView.Groups.Add(GroupExtraPath)

        For Each IB In ConfigSetting.Infobases
            If Not IB.Found Then
                Dim item1 = New ListViewItem(IB.DatabaseName, GroupExtraPath)
                item1.Checked = True
                item1.SubItems.Add(IB.DatabaseID)
                item1.SubItems.Add("")
                item1.SubItems.Add(CalcullateFolderSize(IB.DatabaseCatalog))
                item1.SubItems.Add(IB.DatabaseCatalog)
                item1.SubItems.Add(IB.ESServerName)

                ListView.Items.Add(item1)
            End If
        Next

    End Sub

    Private Function LoadFileInfobasesList() As List(Of ServiceDescriptionClass.Infobases)

        Dim Result As List(Of ServiceDescriptionClass.Infobases) = New List(Of ServiceDescriptionClass.Infobases)

        'Try

        Dim IbasesListPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "1C\1CEStart\ibases.v8i")

        If My.Computer.FileSystem.FileExists(IbasesListPath) Then

            Dim reader As StreamReader = My.Computer.FileSystem.OpenTextFileReader(IbasesListPath)
            Dim aa As String = ""
            Dim Infobase As ServiceDescriptionClass.Infobases = Nothing

            Try
                Do


                    Dim a = aa.Trim

                    Try
                        If a.StartsWith("[") And a.EndsWith("]") Then
                            If Not Infobase.GUID = Nothing And
                                Not Infobase.CatalogEventLog Is Nothing Then
                                Result.Add(Infobase)
                            End If

                            Infobase = New ServiceDescriptionClass.Infobases
                            Infobase.Name = a.Substring(1, a.Length - 2)
                        ElseIf a.StartsWith("Connect=File=") Then
                            'Connect=File="C:\Users\Alex\Documents\AS";

                            Dim BaseDir = a.Substring(14, a.Length - 16)
                            If My.Computer.FileSystem.DirectoryExists(BaseDir) Then
                                Infobase.CatalogEventLog = Path.Combine(BaseDir, "1Cv8Log")
                                Try
                                    Dim SizeLog As UInt64 = 0
                                    If My.Computer.FileSystem.DirectoryExists(Infobase.CatalogEventLog) Then
                                        For Each File In My.Computer.FileSystem.GetFiles(Infobase.CatalogEventLog)
                                            Dim FI = My.Computer.FileSystem.GetFileInfo(File)
                                            SizeLog = SizeLog + FI.Length
                                        Next
                                        Infobase.SizeEventLog = Math.Round(SizeLog / 1024 / 1024, 2)
                                    End If
                                Catch ex As Exception
                                End Try
                            End If


                        ElseIf a.StartsWith("ID=") Then
                            'ID=f3e16c18-e4ff-4ac9-9ee7-2f0a37d196d9
                            Infobase.GUID = a.Substring(3)
                        End If
                    Catch ex As Exception
                        Infobase = New ServiceDescriptionClass.Infobases
                    End Try

                    aa = reader.ReadLine

                Loop Until aa Is Nothing
            Catch ex As Exception
                Dim aaa = ex.Message
            End Try


            If Not Infobase.GUID = Nothing And Not Infobase.CatalogEventLog Is Nothing Then
                Result.Add(Infobase)
            End If

            reader.Close()


        End If

        Return Result

    End Function

    Private Sub Button4_Click(sender As System.Object, e As System.EventArgs) Handles Button4.Click

        Try
            If DBType.Text = "MySQL" Then
                Dim objConn As New MySqlConnection(ConnectionStringBox.Text.Trim)
                objConn.Open()
                Dim command As New MySqlCommand("SELECT 1", objConn)
                command.ExecuteReader()
            ElseIf DBType.Text = "MSSQL" Then
                Dim objConn As New SqlConnection(ConnectionStringBox.Text.Trim)
                objConn.Open()
                Dim command As New SqlCommand("SELECT 1", objConn)
                command.ExecuteReader()
            ElseIf DBType.Text = "ElasticSearch" Then

                Dim _WebRequest As System.Net.WebRequest = System.Net.WebRequest.Create(ConnectionStringBox.Text.Trim)
                Dim Resp As HttpWebResponse = _WebRequest.GetResponse()

                If Not Resp.StatusCode = HttpStatusCode.OK Then
                    Throw New System.Exception("Expected responce code 200. Status code returned: " + Resp.StatusCode.ToString)
                End If
            End If

            MsgBox("Подключение выполнено успешно!", , Text)

        Catch ex As Exception
            MsgBox("Ошибка при подключении: " + ex.Message, , Text)
        End Try

    End Sub

    Private Sub Button6_Click(sender As System.Object, e As System.EventArgs) Handles Button6.Click

        Dim About = New AboutBox
        About.ShowDialog()

    End Sub

    Private Sub Label2_Click(sender As System.Object, e As System.EventArgs) Handles Label2.Click

    End Sub

    Private Sub RepeatTime_Leave(sender As System.Object, e As System.EventArgs) Handles RepeatTime.Leave
        Try
            Dim a = Convert.ToInt32(RepeatTime.Text)
        Catch ex As Exception
            RepeatTime.Text = "60"
            MsgBox("Было указано неправильное значение паузы." + vbNewLine + "Установлено значение по умолчанию - 60 секунд.", , Text)
        End Try
    End Sub

    Function CalcullateFolderSize(CatalogEventLog) As String

        Try
            Dim SizeLog As UInt64 = 0
            If My.Computer.FileSystem.DirectoryExists(CatalogEventLog) Then
                For Each File In My.Computer.FileSystem.GetFiles(CatalogEventLog)
                    Dim FI = My.Computer.FileSystem.GetFileInfo(File)
                    SizeLog = SizeLog + FI.Length
                Next
                Return Math.Round(SizeLog / 1024 / 1024, 2).ToString
            End If
        Catch ex As Exception
        End Try

        Return ""

    End Function

    Private Sub ButtonAddPath_Click(sender As Object, e As EventArgs) Handles ButtonAddPath.Click

        Dim AddPath = New AddPath
        AddPath.ShowDialog()

        If AddPath.Success Then

            Dim item1 = New ListViewItem(AddPath.IBName.Text, GroupExtraPath)
            item1.Checked = True
            item1.SubItems.Add(AddPath.IBGUID.Text)
            item1.SubItems.Add(AddPath.Description.Text)
            item1.SubItems.Add(CalcullateFolderSize(AddPath.Path.Text))
            item1.SubItems.Add(AddPath.Path.Text)
            item1.SubItems.Add(AddPath.ESServerNameTextBox.Text)

            ListView.Items.Add(item1)


        End If


    End Sub



    Private Sub ListView_DoubleClick(sender As Object, e As EventArgs) Handles ListView.DoubleClick

        Dim item As ListViewItem = sender.SelectedItems(0)

        'If item.Group Is GroupExtraPath Then
        Dim AddPath = New AddPath
        AddPath.IBName.Text = item.Text
        AddPath.IBGUID.Text = item.SubItems(1).Text
        AddPath.ESServerNameTextBox.Text = item.SubItems(5).Text
        AddPath.Description.Text = item.SubItems(2).Text
        AddPath.Path.Text = item.SubItems(4).Text
        AddPath.ExistedRow = True
        AddPath.ShowDialog()

        If AddPath.Success Then

            item.SubItems.Clear()

            item.Text = AddPath.IBName.Text
            item.SubItems.Add(AddPath.IBGUID.Text)
            item.SubItems.Add(AddPath.Description.Text)
            item.SubItems.Add(CalcullateFolderSize(AddPath.Path.Text))
            item.SubItems.Add(AddPath.Path.Text)
            item.SubItems.Add(AddPath.ESServerNameTextBox.Text)

        End If



        'End If



    End Sub



    Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        Process.Start("https://github.com/alekseybochkov/")
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Process.Start("http://infostart.ru/public/182820/")
    End Sub

    Private Sub LinkLabel3_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel3.LinkClicked
        Process.Start("https://github.com/alekseybochkov/EventLogLoader/")
    End Sub
End Class
