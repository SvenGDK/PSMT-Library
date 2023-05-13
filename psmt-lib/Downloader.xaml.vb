Imports Microsoft.Win32
Imports System.ComponentModel
Imports System.IO
Imports System.Net
Imports System.Windows
Imports System.Windows.Forms
Imports System.Windows.Media
Imports System.Windows.Media.Imaging

Public Class Downloader

    Dim TimerID As IntPtr = Nothing
    Dim DownloadSpeed As Integer = 0
    Dim MaximumSpeed As Integer = 0
    Dim AverageSpeed As Integer = 0
    Dim LoopCount As Integer = 0
    Dim ByteCount As Integer = 0
    Dim CurrentBytes As Long = 0
    Dim PreviousBytes As Long = 0
    Dim DownloadSize As Long = 0
    Dim StartTime As Long = 0
    Dim ElapsedTime As TimeSpan = Nothing
    Dim TimeLeft As TimeSpan = Nothing
    Dim TimeLeftAverage As Double = 0
    Dim DownloadIcon As ImageSource = Nothing

    Dim WithEvents NPSBrowser As New Forms.WebBrowser() With {.ScriptErrorsSuppressed = True}
    Dim WithEvents DownloadClient As New WebClient()
    Public PackageConsole As String
    Public PackageTitleID As String
    Public PackageContentID As String

    Public Sub New()
        InitializeComponent()

        TimerID = Nothing
        CurrentBytes = 0
        PreviousBytes = 0
        DownloadSpeed = 0
        MaximumSpeed = 0
        AverageSpeed = 0
        LoopCount = 0
        ByteCount = 0
        DownloadProgressBar.Value = 0

        AddHandler SystemEvents.TimerElapsed, AddressOf DownloadUpdating
    End Sub

    Public Shared Function GetFilenameFromUrl(FileURL As Uri) As String
        Return FileURL.Segments(FileURL.Segments.Length - 1)
    End Function

    Public Function WebFileSize(sURL As String) As Double

        If sURL.StartsWith("ftp://") Then
            Dim myRequest As FtpWebRequest

            myRequest = CType(WebRequest.Create(sURL), FtpWebRequest)
            myRequest.Method = WebRequestMethods.Ftp.GetFileSize
            myRequest.Credentials = New NetworkCredential("anonymous", "")

            Dim myResponse As FtpWebResponse = CType(myRequest.GetResponse(), FtpWebResponse)
            Dim ResponseLenght As Long = myResponse.ContentLength
            myResponse.Close()

            Return Math.Round(ResponseLenght / 1024 / 1024, 2)
        Else
            Dim myRequest As HttpWebRequest = CType(WebRequest.Create(sURL), HttpWebRequest)
            Dim myResponse As HttpWebResponse = CType(myRequest.GetResponse(), HttpWebResponse)
            myResponse.Close()

            Return Math.Round(myResponse.ContentLength / 1024 / 1024, 2)
        End If

    End Function

    Public Function CreateNewDownload(Source As String, Optional ModifyName As Boolean = False, Optional NewName As String = "") As Boolean
        'Create Downloads directory if not exists
        If Not Directory.Exists(My.Computer.FileSystem.CurrentDirectory + "\Downloads") Then Directory.CreateDirectory(My.Computer.FileSystem.CurrentDirectory + "\Downloads")

        If DownloadIcon IsNot Nothing Then DownloadImage.Source = DownloadIcon

        Dim FileName As String = GetFilenameFromUrl(New Uri(Source))
        If Not String.IsNullOrEmpty(FileName) Then
            TimerID = SystemEvents.CreateTimer(1000)
            StartTime = Now.Ticks

            'Change the file name for .pkgs
            If ModifyName = True And Not String.IsNullOrEmpty(NewName) Then
                FileName = NewName
            End If

            DownloadFileSizeTB.Text = "File Size: " + WebFileSize(Source).ToString + " MB"
            FileToDownloadTB.Text = "Downloading " + FileName + " ..."
            DownloadClient.DownloadFileAsync(New Uri(Source), My.Computer.FileSystem.CurrentDirectory + "\Downloads\" + FileName)
            Return True
        Else
            Return False
        End If
    End Function

    Public Sub DownloadUpdating(sender As Object, e As TimerElapsedEventArgs)
        DownloadSpeed = CInt(CurrentBytes - PreviousBytes)
        ElapsedTime = TimeSpan.FromTicks(Now.Ticks - StartTime)
        DownloadETATB.Text = "Time elapsed: " + String.Format("{0:00}h {1:00}m {2:00}s", ElapsedTime.TotalHours, ElapsedTime.Minutes, ElapsedTime.Seconds)

        If DownloadSpeed < 1 Then
            DownloadSpeedTB.Text = "< 1 KB/s"
        Else
            DownloadSpeedTB.Text = FormatNumber(DownloadSpeed / 1024 / 1024, 2).ToString & " MB/s"
        End If

        If Not DownloadSpeed < 1 Then
            LoopCount += 1
            ByteCount += DownloadSpeed

            TimeLeftAverage = ElapsedTime.TotalSeconds / CurrentBytes
            TimeLeft = TimeSpan.FromSeconds(TimeLeftAverage * (DownloadSize - CurrentBytes))

            DownloadETALeftTB.Text = "Time left: " + String.Format("{0:00}h {1:00}m {2:00}s", TimeLeft.TotalHours, TimeLeft.Minutes, TimeLeft.Seconds)
        End If

        PreviousBytes = CurrentBytes
    End Sub

    Private Sub DownloadClient_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs) Handles DownloadClient.DownloadProgressChanged
        DownloadProgressBar.Value = e.ProgressPercentage
        DownloadSize = e.TotalBytesToReceive
        CurrentBytes = e.BytesReceived
    End Sub

    Private Sub DownloadClient_DownloadFileCompleted(sender As Object, e As AsyncCompletedEventArgs) Handles DownloadClient.DownloadFileCompleted
        If e.Cancelled Then
            Try
                SystemEvents.KillTimer(TimerID)
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
        Else
            SystemEvents.KillTimer(TimerID)

            If FileToDownloadTB.Dispatcher.CheckAccess() = False Then
                FileToDownloadTB.Dispatcher.BeginInvoke(Sub() FileToDownloadTB.Text = "Download finished")
            Else
                FileToDownloadTB.Text = "Download finished"
            End If

            If MsgBox("Download completed. Open the Downloads folder ?", MsgBoxStyle.YesNo, "Completed") = MsgBoxResult.Yes Then
                Process.Start("explorer", My.Computer.FileSystem.CurrentDirectory + "\Downloads")
            End If
        End If
    End Sub

    Private Sub Downloader_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        'Show art for downloads from NPS
        If PackageConsole = "PS3" Then
            If Utils.IsURLValid("https://nopaystation.com/view/" + PackageConsole + "/" + PackageTitleID + "/" + PackageContentID + "/1") Then
                NPSBrowser.Navigate("https://nopaystation.com/view/" + PackageConsole + "/" + PackageTitleID + "/" + PackageContentID + "/1")
            End If
        ElseIf PackageConsole = "PSV" Then
            If Utils.IsURLValid("https://nopaystation.com/view/" + PackageConsole + "/" + PackageTitleID + "/" + PackageContentID + "/1") Then
                NPSBrowser.Navigate("https://nopaystation.com/view/" + PackageConsole + "/" + PackageTitleID + "/" + PackageContentID + "/1")
            End If
        ElseIf PackageConsole = "PSX" Then
            If Utils.IsURLValid("https://nopaystation.com/view/" + PackageConsole + "/" + PackageTitleID + "/" + PackageContentID + "/0") Then
                NPSBrowser.Navigate("https://nopaystation.com/view/" + PackageConsole + "/" + PackageTitleID + "/" + PackageContentID + "/0")
            End If
        End If
    End Sub

    Private Sub NPSBrowser_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles NPSBrowser.DocumentCompleted
        'Art
        If NPSBrowser.Document.GetElementById("itemArtwork") IsNot Nothing Then
            DownloadImage.Source = New BitmapImage(New Uri(NPSBrowser.Document.GetElementById("itemArtwork").GetAttribute("src"), UriKind.RelativeOrAbsolute))
        End If
    End Sub

End Class
