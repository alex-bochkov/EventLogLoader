Imports Newtonsoft.Json

Public Module ConfigSettingsModule

    Class InfobaseSetting
        Public DatabaseID As String
        Public DatabaseName As String
        Public DatabaseCatalog As String
        Public Found As Boolean
    End Class

    Class ConfigSetting
        Public ConnectionString As String
        Public DBType As String
        Public RepeatTime As Integer
        Public Infobases As List(Of InfobaseSetting)
        Sub New()
            Infobases = New List(Of InfobaseSetting)
        End Sub
    End Class

    Public Function LoadConfigSettingFromFile(ConfigFilePath As String) As ConfigSetting

        If My.Computer.FileSystem.FileExists(ConfigFilePath) Then

            Dim JsonText = My.Computer.FileSystem.ReadAllText(ConfigFilePath)

            Dim ConfigSettingObj = JsonConvert.DeserializeObject(Of ConfigSetting)(JsonText)

            Return ConfigSettingObj

        End If

        Return Nothing

    End Function

    Public Sub SaveConfigSettingToFile(ConfigSettingObj As ConfigSetting, ConfigFilePath As String)

        Dim JsonText As String = JsonConvert.SerializeObject(ConfigSettingObj, Formatting.Indented)

        My.Computer.FileSystem.WriteAllText(ConfigFilePath, JsonText, False)


    End Sub


End Module
