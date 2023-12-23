Imports System.Windows

Public Class PS5GamePatches

    Dim TotalPatches As Integer = 0
    Public SearchForGamePatchWithID As String = String.Empty

    Private Sub SearchButton_Click(sender As Object, e As RoutedEventArgs) Handles SearchButton.Click
        If Not String.IsNullOrEmpty(SearchGameIDTextBox.Text) Then
            If Utils.IsURLValid("https://prosperopatches.com/" + SearchGameIDTextBox.Text) Then
                Dim NewWin As New PS5GamePatchSelector() With {.CurrentGameID = SearchGameIDTextBox.Text}
                NewWin.GamePatchesWebView.Source = New Uri("https://prosperopatches.com/" + SearchGameIDTextBox.Text)
                NewWin.Show()
            End If
        End If
    End Sub

    Private Sub VisitButton_Click(sender As Object, e As RoutedEventArgs) Handles VisitButton.Click
        Process.Start("https://prosperopatches.com/" + SearchGameIDTextBox.Text)
    End Sub

    Private Sub PS5GamePatches_ContentRendered(sender As Object, e As EventArgs) Handles Me.ContentRendered
        If Not String.IsNullOrEmpty(SearchForGamePatchWithID) Then
            SearchGameIDTextBox.Text = SearchForGamePatchWithID

            If Utils.IsURLValid("https://prosperopatches.com/" + SearchForGamePatchWithID) Then
                Dim NewWin As New PS5GamePatchSelector() With {.CurrentGameID = SearchForGamePatchWithID}
                NewWin.GamePatchesWebView.Source = New Uri("https://prosperopatches.com/" + SearchForGamePatchWithID)
                NewWin.Show()
            End If
        End If
    End Sub

    Private Sub DownloadQueueListView_SelectionChanged(sender As Object, e As Controls.SelectionChangedEventArgs) Handles DownloadQueueListView.SelectionChanged
        If DownloadQueueListView.SelectedItem Is Nothing Then
            DownloadButton.IsEnabled = False
        Else
            DownloadButton.IsEnabled = True
        End If
    End Sub

    Private Sub DownloadButton_Click(sender As Object, e As RoutedEventArgs) Handles DownloadButton.Click
        If DownloadQueueListView.SelectedItem IsNot Nothing Then
            If DownloadQueueListView.SelectedItems.Count > 1 Then

                For Each SelectedItem In DownloadQueueListView.SelectedItems
                    Dim SelectedItemAsQueueItem As Structures.DownloadQueueItem = CType(SelectedItem, Structures.DownloadQueueItem)

                    'Create a new download window for each selected item
                    Dim NewDownloader As New Downloader() With {.ShowActivated = True}
                    NewDownloader.Show()
                    If NewDownloader.CreateNewDownload(SelectedItemAsQueueItem.DownloadURL) = False Then
                        MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
                        NewDownloader.Close()
                    End If
                Next

            Else
                Dim SelectedItemAsQueueItem As Structures.DownloadQueueItem = CType(DownloadQueueListView.SelectedItem, Structures.DownloadQueueItem)

                Dim NewDownloader As New Downloader() With {.ShowActivated = True}
                NewDownloader.Show()
                If NewDownloader.CreateNewDownload(SelectedItemAsQueueItem.DownloadURL) = False Then
                    MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
                    NewDownloader.Close()
                End If
            End If
        End If

    End Sub

End Class
