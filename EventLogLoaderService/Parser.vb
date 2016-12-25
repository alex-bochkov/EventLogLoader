Public Module ParserServices

    Function ParseString(Text As String)

        Dim Array(0)

        If Not Text = "" Then

            Dim RowArr(2)

            Dim j = 0
            Dim OpenBlock = False
            Dim Level = 0

            For i = 0 To Text.Length - 1

                Dim Simb = Text.Substring(i, 1)
                Dim SubStr = ""

                If OpenBlock Then
                    If Simb = """" Then
                        OpenBlock = False
                    End If
                Else
                    If Simb = "{" Then
                        Level = Level + 1
                        SubStr = "НачалоУровня"
                    ElseIf Simb = "}" Then
                        Level = Level - 1
                        SubStr = "ОкончаниеУровня"
                    ElseIf Simb = """" Then
                        OpenBlock = True
                    ElseIf Simb = "," Then
                        SubStr = "Разделитель"
                    End If
                End If

                If Not SubStr = "" Then
                    ReDim Preserve Array(j)
                    ReDim RowArr(2)
                    RowArr(0) = i
                    RowArr(1) = SubStr
                    RowArr(2) = Level

                    Array(j) = RowArr
                    j = j + 1
                End If


            Next
        End If

        Dim ArrayBases(0)
        Dim ArrayRow(2)
        Dim ArrayLines(0)

        If Array.Length > 1 Then

            Dim ArrayLevel(10) As Integer
            Dim ArrayValue(0)

            Dim Level = 0
            ' Dim CountLines = 0
            Dim LastVal = 0
            Dim StrLevel = ""

            For Each a In Array

                Select Case a(1)
                    Case "НачалоУровня"

                        If Not StrLevel = "" Then
                            ArrayValue(0) = StrLevel
                            ArrayLines(ArrayLines.Length - 1) = ArrayValue
                            ReDim Preserve ArrayLines(ArrayLines.Length)
                        End If


                        ' CountLines = CountLines + 1
                        ArrayLevel(Level) = ArrayLevel(Level) + 1
                        Level = Level + 1

                        StrLevel = ""
                        For j = 0 To Level - 1
                            StrLevel = IIf(StrLevel = "", "", StrLevel + ".") + ArrayLevel(j).ToString
                        Next

                        ReDim ArrayValue(0)

                    Case "ОкончаниеУровня"

                        Dim TextStr = Text.Substring(LastVal + 1, a(0) - LastVal - 1)
                        TextStr = TextStr.Replace("""""", """")
                        If TextStr = """" Then TextStr = ""

                        If TextStr.StartsWith("""") And TextStr.EndsWith("""") Then
                            TextStr = TextStr.Substring(1, TextStr.Length - 2)
                        End If
                        'If Not TextStr = "" Then
                        ReDim Preserve ArrayValue(ArrayValue.Length)
                        ArrayValue(ArrayValue.Length - 1) = TextStr
                        'End If


                        ArrayValue(0) = StrLevel
                        ArrayLines(ArrayLines.Length - 1) = ArrayValue
                        ReDim Preserve ArrayLines(ArrayLines.Length)

                        'ArrayLevel(Level) = ArrayLevel(Level) - 1
                        ArrayLevel(Level) = 0
                        Level = Level - 1

                        ReDim ArrayValue(0)

                        StrLevel = ""
                        For j = 0 To Level - 1
                            StrLevel = IIf(StrLevel = "", "", StrLevel + ".") + ArrayLevel(j).ToString
                        Next


                    Case "Разделитель"

                        Dim TextStr = Text.Substring(LastVal + 1, a(0) - LastVal - 1)
                        TextStr = TextStr.Replace("""""", """")
                        If TextStr = """" Then TextStr = ""

                        If TextStr.StartsWith("""") And TextStr.EndsWith("""") Then
                            TextStr = TextStr.Substring(1, TextStr.Length - 2)
                        End If

                        'If Not TextStr = "" Then
                        ReDim Preserve ArrayValue(ArrayValue.Length)
                        ArrayValue(ArrayValue.Length - 1) = TextStr
                        'End If

                End Select

                LastVal = a(0)

            Next
        End If

        Return ArrayLines

    End Function

    Function ParseEventlogString(Text As String)

        Dim ArrayLines(0)

        Dim Text2 = Text.Substring(1, IIf(Text.EndsWith(","), Text.Length - 3, Text.Length - 2)) + ","

        Dim Str = ""

        Dim Delim = Text2.IndexOf(",")
        Dim i = 0

        While Delim > 0
            Str = Str + Text2.Substring(0, Delim)
            Text2 = Text2.Substring(Delim + 1)

            If CountSubstringInString(Str, "{") = CountSubstringInString(Str, "}") _
                And Math.IEEERemainder(CountSubstringInString(Str, """"), 2) = 0 Then

                ReDim Preserve ArrayLines(i)
                ArrayLines(i) = Str.Trim
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