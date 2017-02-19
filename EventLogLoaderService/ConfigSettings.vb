Imports Newtonsoft.Json

Public Module ConfigSettingsModule

    Class InfobaseSetting
        Public ESServerName As String = ""
        Public DatabaseID As String = ""
        Public DatabaseName As String = ""
        Public DatabaseCatalog As String = ""
        Public Found As Boolean = False
    End Class

    Class ElasticSearchFieldSynonymsClass
        Public ServerName As String
        Public DatabaseName As String
        Public RowID As String
        Public Severity As String
        Public DateTime As String
        Public ConnectID As String
        Public DataType As String
        Public SessionNumber As String
        Public DataStructure As String
        Public DataString As String
        Public Comment As String
        Public SessionDataSplitCode As String
        Public EventType As String
        Public Metadata As String
        Public Computer As String
        Public PrimaryPort As String
        Public Server As String
        Public SecondaryPort As String
        Public Application As String
        Public UserName As String
    End Class

    Class ConfigSetting
        Public ConnectionString As String = ""
        Public DBType As String = ""
        Public RepeatTime As Integer = 0
        Public ESIndexName As String = ""
        Public ESUseSynonymsForFieldsNames As Boolean = False
        Public ESFieldSynonyms As ElasticSearchFieldSynonymsClass
        Public Infobases As List(Of InfobaseSetting)
        Sub New()
            Infobases = New List(Of InfobaseSetting)
            ESFieldSynonyms = New ElasticSearchFieldSynonymsClass
        End Sub
    End Class

    Public Function LoadConfigSettingFromFile(ConfigFilePath As String) As ConfigSetting

        If My.Computer.FileSystem.FileExists(ConfigFilePath) Then

            Dim JsonText = My.Computer.FileSystem.ReadAllText(ConfigFilePath)

            Dim ConfigSettingObj = JsonConvert.DeserializeObject(Of ConfigSetting)(JsonText)

            Return ConfigSettingObj

        End If

        Return New ConfigSetting

    End Function

    Public Sub SaveConfigSettingToFile(ConfigSettingObj As ConfigSetting, ConfigFilePath As String)

        Dim JsonText As String = JsonConvert.SerializeObject(ConfigSettingObj, Formatting.Indented)

        My.Computer.FileSystem.WriteAllText(ConfigFilePath, JsonText, False)

    End Sub


End Module
