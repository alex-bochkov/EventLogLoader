Public Module ParserServices

    Function ParseEventLogString(Text As String)

        Dim ArrayLines(0)

        Dim Text2 = Text.Substring(1, IIf(Text.EndsWith(","), Text.Length - 3, Text.Length - 2)) + ","

        Dim Str = ""

        Dim Delim = Text2.IndexOf(",")
        Dim i = 0

        While Delim > 0
            Str = Str + Text2.Substring(0, Delim).Trim
            Text2 = Text2.Substring(Delim + 1)

            If CountSubstringInString(Str, "{") = CountSubstringInString(Str, "}") _
                And Math.IEEERemainder(CountSubstringInString(Str, """"), 2) = 0 Then

                ReDim Preserve ArrayLines(i)

                If Str.StartsWith("""") And Str.EndsWith("""") Then
                    Str = Str.Substring(1, Str.Length - 2)
                End If

                ArrayLines(i) = Str
                i = i + 1
                Str = ""
            Else
                Str = Str + ","
            End If

            Delim = Text2.IndexOf(",")

        End While

        Return ArrayLines

    End Function

    Function CountSubstringInString(Str As String, SubStr As String)

        CountSubstringInString = (Str.Length - Str.Replace(SubStr, "").Length) / SubStr.Length

    End Function


End Module