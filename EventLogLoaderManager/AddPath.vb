Public Class AddPath

    Public Success As Boolean = False
    Public ExistedRow As Boolean = False


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Success = True
        Close()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Close()
    End Sub

    Private Sub AddPath_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If String.IsNullOrEmpty(IBGUID.Text) Then
            IBGUID.Text = Guid.NewGuid.ToString
        End If
    End Sub

    Private Sub ButtonChoosePath_Click(sender As Object, e As EventArgs) Handles ButtonChoosePath.Click
        FolderBrowserDialog.ShowDialog()

        If Not String.IsNullOrEmpty(FolderBrowserDialog.SelectedPath) Then
            Path.Text = FolderBrowserDialog.SelectedPath
        End If
    End Sub
End Class