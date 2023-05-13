Public Class InputDialog

    Private Sub ConfirmButton_Click(sender As Object, e As Windows.RoutedEventArgs) Handles ConfirmButton.Click
        DialogResult = True
        Close()
    End Sub

End Class
