Imports System.IO
Imports EventLogLoaderService

Public Class ServiceDescriptionClass
    Public Name As String
    Public DisplayName As String
    Public Description As String
    Public PathName As String
    Public User As String

    Public Debug As Boolean = False
    Public ExeFile As String = ""
    Public ClusterFiles As String = ""
    Public PortAgent As Integer = 1540
    Public PortMngr As Integer = 1541
    Public PortProcessBegin As Integer = 1560
    Public PortProcessEnd As Integer = 1591

    Public Structure Infobases
        Dim Name As String
        Dim Description As String
        Dim GUID As String
        Dim SizeEventLog As Integer
        Dim CatalogEventLog As String
    End Structure

    Public ArrayInfobases() As Infobases

    Sub ParsePath()

        Dim PathNameTemp = PathName.ToLower

        If PathNameTemp.Contains("-debug") _
            Or PathNameTemp.Contains("/debug") Then

            Debug = True

            PathNameTemp = PathNameTemp.Replace("-debug", "")
            PathNameTemp = PathNameTemp.Replace("/debug", "")

        End If

        Dim Ind = PathNameTemp.IndexOf("ragent.exe")
        If Ind > 0 Then
            ExeFile = PathNameTemp.Substring(0, Ind + 10)
            ExeFile = ExeFile.Replace("""", "")

            PathNameTemp = PathNameTemp.Substring(Ind + 11)
        End If

        Ind = PathNameTemp.IndexOf("-regport")
        If Ind > 0 Then

            Dim PortStr = ""
            Dim i = 0
            Dim Simb = PathNameTemp.Substring(Ind + 8 + i, 1)

            While "0123456789 ".Contains(Simb)

                PortStr = PortStr + Simb
                i = i + 1
                Simb = PathNameTemp.Substring(Ind + 8 + i, 1)

            End While

            Try
                PortMngr = Convert.ToInt32(PortStr)
            Catch ex As Exception

            End Try

        End If

        Ind = PathNameTemp.IndexOf("-port")
        If Ind > 0 Then

            Dim PortStr = ""
            Dim i = 0
            Dim Simb = PathNameTemp.Substring(Ind + 5 + i, 1)

            While "0123456789 ".Contains(Simb)

                PortStr = PortStr + Simb
                i = i + 1
                Simb = PathNameTemp.Substring(Ind + 5 + i, 1)

            End While

            Try
                PortAgent = Convert.ToInt32(PortStr)
            Catch ex As Exception

            End Try

        End If

        Ind = PathNameTemp.IndexOf("-range")
        If Ind > 0 Then

            Dim PortStr = ""
            Dim i = 0
            Dim Simb = PathNameTemp.Substring(Ind + 6 + i, 1)

            While "0123456789 :".Contains(Simb)

                PortStr = PortStr + Simb
                i = i + 1
                Simb = PathNameTemp.Substring(Ind + 6 + i, 1)

            End While

            Try

                Ind = PortStr.IndexOf(":")
                PortProcessBegin = Convert.ToInt32(PortStr.Substring(0, Ind))
                PortProcessEnd = Convert.ToInt32(PortStr.Substring(Ind + 1))

            Catch ex As Exception

            End Try

        End If

        Ind = PathNameTemp.IndexOf("-d")
        If Ind > 0 Then

            Dim PathStr = PathNameTemp.Substring(Ind + 2)
            Dim Simb = PathStr.Substring(0, 1)

            While " """.Contains(Simb)

                PathStr = PathStr.Substring(2)
                Simb = PathStr.Substring(1, 1)

            End While

            Ind = PathStr.IndexOf("""")
            If Ind > 0 Then
                ClusterFiles = PathStr.Substring(0, Ind)

            End If

        End If


        'А теперь финт ушами - ищем полученные строки в иходной строке и вырезаем оттуда с корректным регистром
        Ind = PathName.ToLower.IndexOf(ExeFile)
        If Ind > 0 Then
            ExeFile = PathName.Substring(Ind, ExeFile.Length)
        End If

        Ind = PathName.ToLower.IndexOf(ClusterFiles)
        If Ind > 0 Then
            ClusterFiles = PathName.Substring(Ind, ClusterFiles.Length)
        End If

        '"C:\Program Files\1cv82\8.2.17.153\bin\ragent.exe" -debug -srvc -agent 
        '-regport 2541 -port 2540 -range 2560:2591 -d "C:\Program Files\1cv82\srvinfo"

    End Sub

    Sub GetInfobases()

        Try
            Dim Catalog = Path.Combine(ClusterFiles, "reg_" + PortMngr.ToString)

            Dim TMP = My.Computer.FileSystem.GetTempFileName()

            '8.2
            Dim ConfFilePath As String = Path.Combine(Catalog, "1CV8Reg.lst")
            If My.Computer.FileSystem.FileExists(ConfFilePath) Then
                My.Computer.FileSystem.CopyFile(ConfFilePath, TMP, True)

            End If

            '8.3
            ConfFilePath = Path.Combine(Catalog, "1CV8Clst.lst")
            If My.Computer.FileSystem.FileExists(ConfFilePath) Then
                My.Computer.FileSystem.CopyFile(ConfFilePath, TMP, True)
            End If

            Dim Text = My.Computer.FileSystem.ReadAllText(TMP)
            My.Computer.FileSystem.DeleteFile(TMP)

            Dim Array = ParserServices.ParseString(Text)

            Dim i = 0

            For Each a In Array
                If Not a Is Nothing Then
                    If a.Length = 11 And a(0).StartsWith("1.2.") Then

                        Dim IB = New Infobases
                        IB.Name = a(2).ToString
                        IB.GUID = a(1).ToString
                        IB.Description = a(3).ToString
                        IB.SizeEventLog = 0

                        Try

                            Dim SizeLog As UInt64 = 0
                            Dim CatalogEventLog = Path.Combine(Path.Combine(Catalog, IB.GUID), "1Cv8Log\")

                            IB.CatalogEventLog = CatalogEventLog

                            If My.Computer.FileSystem.DirectoryExists(CatalogEventLog) Then



                                For Each File In My.Computer.FileSystem.GetFiles(CatalogEventLog)
                                    Dim FI = My.Computer.FileSystem.GetFileInfo(File)
                                    SizeLog = SizeLog + FI.Length
                                Next

                                IB.SizeEventLog = Math.Round(SizeLog / 1024 / 1024, 2)

                            End If

                        Catch ex As Exception

                        End Try

                        ReDim Preserve ArrayInfobases(i)
                        ArrayInfobases(i) = IB

                        i = i + 1


                    End If
                End If
            Next


            System.Array.Sort(ArrayInfobases)

        Catch ex As Exception

        End Try







    End Sub

End Class