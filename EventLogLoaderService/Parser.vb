Public Module ParserServices

    Function ParseEventLogString(Text As String)

        Dim ArrayLines(0)

        If Text Is Nothing Or Text.Length = 0 Then
            Return ArrayLines
        End If

        ' Сим отсекаем скобочки
        Dim Text2 = Text.Substring(1, IIf(Text.EndsWith(","), Text.Length - 3, Text.Length - 2)) + ","

        Dim i = 0
        Dim Str = ""
        Dim TextBlockOpen = False
        Dim count = 0
        Dim BracketCount = 0

        For i = 0 To Text2.Length - 1

            Dim simb = Text2.Substring(i, 1)

            If (simb = ",") And Not TextBlockOpen And (BracketCount = 0) Then

                ReDim Preserve ArrayLines(count)
                ArrayLines(count) = Str
                count = count + 1

                Str = ""

            Else If simb = "{" And Not TextBlockOpen Then

                BracketCount = BracketCount + 1
                Str = Str + simb

            Else If simb = "}" And Not TextBlockOpen Then

                BracketCount = BracketCount - 1
                Str = Str + simb

            Else If simb = """" Then

                TextBlockOpen = Not TextBlockOpen
                Str = Str + """"

            Else

                Str = Str + simb

            End If

        Next

        Return ArrayLines

    End Function

    Function CountSubstringInString(Str As String, SubStr As String)

        CountSubstringInString = (Str.Length - Str.Replace(SubStr, "").Length) / SubStr.Length

    End Function


End Module