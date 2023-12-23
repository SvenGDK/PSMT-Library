Imports System.Net
Imports System.Windows
Imports Microsoft.Web.WebView2.Core

Public Class PS5GamePatchSelector

    Public CurrentGameID As String = ""

    Private Async Sub GamePatchesWebView_NavigationCompleted(sender As Object, e As CoreWebView2NavigationCompletedEventArgs) Handles GamePatchesWebView.NavigationCompleted
        'Remove parts of the site
        Dim JS As String = "document.getElementsByClassName('navbar navbar-expand-lg bd-navbar sticky-top')[0].style.display='none';document.getElementsByClassName('py-2')[0].style.display='none';document.getElementsByClassName('py-4')[0].style.display='none';document.getElementsByClassName('ms-2 fw-normal')[0].style.display='none';document.getElementsByClassName('nav-link flex-fill share-icon')[0-x].style.display='none';"
        Dim AdditionalJS As String = "var sharebuttons = document.getElementsByClassName('nav-link flex-fill share-icon');for (let sharebutton of sharebuttons) { sharebutton.style.display='none'; };"

        If e.IsSuccess And GamePatchesWebView.Source.ToString.StartsWith("https://prosperopatches.com/") Then
            Dim ReturnValue As String = Await GamePatchesWebView.ExecuteScriptAsync(JS) 'Null
            Dim ReturnValue2 As String = Await GamePatchesWebView.ExecuteScriptAsync(AdditionalJS) 'Null

            LoadingTextBlock.Visibility = Visibility.Hidden
            BrowserGrid.Visibility = Visibility.Visible
        End If
    End Sub

    Private Sub DownloadStarting(sender As Object, args As CoreWebView2DownloadStartingEventArgs)
        args.Handled = True

        Dim MsgBoxResult = MsgBox("Do you want to add this download to the queue ?" + vbNewLine + "Selecting 'No' will download the file instantly.", MsgBoxStyle.YesNoCancel, "Select Download Option")
        If MsgBoxResult = MsgBoxResult.Yes Then
            AddToQueue(args.DownloadOperation.Uri, Utils.GetFilenameFromUrl(New Uri(args.DownloadOperation.Uri)), args.DownloadOperation.TotalBytesToReceive)
        ElseIf MsgBoxResult = MsgBoxResult.No Then
            Dim NewDownloader As New Downloader() With {.ShowActivated = True}
            NewDownloader.Show()

            If NewDownloader.CreateNewDownload(args.DownloadOperation.Uri) = False Then
                MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
                NewDownloader.Close()
            End If
        Else
            args.DownloadOperation.Cancel()
        End If

    End Sub

    Private Sub PS5GamePatchSelector_ContentRendered(sender As Object, e As EventArgs) Handles Me.ContentRendered
        'Set loading text
        LoadingTextBlock.Text = "Loading game patches for" + vbCrLf + vbCrLf + CurrentGameID + vbCrLf + vbCrLf + "Please wait ..."
    End Sub

    Private Sub AddToQueue(URL As String, FileName As String, TotalSize As ULong?)

        Dim NewQueueItem As New Structures.DownloadQueueItem() With {.FileName = FileName, .GameID = CurrentGameID, .DownloadURL = URL}

        'Set the download file size
        If TotalSize Is Nothing Then
            Dim FileSize As String = FormatNumber(TotalSize / 1073741824, 2) + " GB"
            NewQueueItem.PKGSize = FileSize
        Else
            Dim myRequest As HttpWebRequest = CType(WebRequest.Create(URL), HttpWebRequest)
            Dim myResponse As HttpWebResponse = CType(myRequest.GetResponse(), HttpWebResponse)
            myResponse.Close()
            Dim NewRetrievedSize As String = FormatNumber(myResponse.ContentLength / 1073741824, 2) + " GB"
            NewQueueItem.PKGSize = NewRetrievedSize
        End If

        'Add to queue
        Dim OpenGamePatchesWindow As PS5GamePatches
        For Each OpenWin In Application.Current.Windows()
            If OpenWin.ToString = "psmt_lib.PS5GamePatches" Then
                OpenGamePatchesWindow = CType(OpenWin, PS5GamePatches)
                OpenGamePatchesWindow.DownloadQueueListView.Items.Add(NewQueueItem)
                Exit For
            End If
        Next

    End Sub

    Private Sub GamePatchesWebView_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles GamePatchesWebView.CoreWebView2InitializationCompleted
        'Add custom download handler
        AddHandler GamePatchesWebView.CoreWebView2.DownloadStarting, AddressOf DownloadStarting
    End Sub
End Class
