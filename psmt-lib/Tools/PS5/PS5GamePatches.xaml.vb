Imports System.Windows
Imports System.Windows.Media.Imaging
Imports Microsoft.Web.WebView2.Core
Imports Newtonsoft.Json.Linq

Public Class PS5GamePatches

    Dim TotalPatches As Integer = 0

    Private Sub SearchButton_Click(sender As Object, e As RoutedEventArgs) Handles SearchButton.Click
        If Not String.IsNullOrEmpty(SearchGameIDTextBox.Text) Then
            PatchesListView.Items.Clear()
            ContentWebView.Source = New Uri("https://prosperopatches.com/" + SearchGameIDTextBox.Text)
        End If
    End Sub

    Private Async Sub ContentWebView_NavigationCompleted(sender As Object, e As CoreWebView2NavigationCompletedEventArgs) Handles ContentWebView.NavigationCompleted
        Try
            If e.IsSuccess And ContentWebView.Source.ToString.StartsWith("https://prosperopatches.com/") Then
                Dim PatchesList As String = Await ContentWebView.ExecuteScriptAsync("document.getElementsByClassName('mb-4 patch-wrapper');")
                Dim GameTitle As String = Await ContentWebView.ExecuteScriptAsync("document.getElementsByClassName('bd-title mb-0 text-white')[0].innerText;")

                'Game Title
                If Not String.IsNullOrEmpty(GameTitle) Then
                    AvailableGamePatchesTextBlock.Text = "Available Game Patches for " + GameTitle.Replace("""", "")
                End If

                'Game Image
                Dim GameImageURL As String = Await ContentWebView.ExecuteScriptAsync("document.getElementsByClassName('game-icon secondary')[0].outerHTML;")
                Dim SplittedGameImageURL As String() = GameImageURL.Split(New String() {"(", ")"}, StringSplitOptions.None)
                If SplittedGameImageURL.Count > 0 Then
                    GameImage.Source = New BitmapImage(New Uri(SplittedGameImageURL(1)))
                End If

                'Patches
                Dim JSONParser As JObject = JObject.Parse(PatchesList)
                TotalPatches = JSONParser.Count

                For index As Integer = 0 To TotalPatches - 1
                    Dim PatchInfo As String = Await ContentWebView.ExecuteScriptAsync("document.getElementsByClassName('mb-4 patch-wrapper')[" + index.ToString + "].innerText;")
                    Dim SplittedPatchInfo As String() = PatchInfo.Split(New String() {"\n"}, StringSplitOptions.RemoveEmptyEntries)

                    Dim NewGamePatch As New Structures.ProsperoPatch() With {.Version = SplittedPatchInfo(0).Replace("""", ""), .RequiredFirmware = SplittedPatchInfo(4), .DateAdded = SplittedPatchInfo(6), .PKGSize = SplittedPatchInfo(2)}
                    PatchesListView.Items.Add(NewGamePatch)
                Next
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub VisitButton_Click(sender As Object, e As RoutedEventArgs) Handles VisitButton.Click
        Process.Start("https://prosperopatches.com/" + SearchGameIDTextBox.Text)
    End Sub

End Class
