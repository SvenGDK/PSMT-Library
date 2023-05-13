Imports System.ComponentModel
Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Windows
Imports System.Windows.Forms
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class PKGSender

    Dim NewHttpListener As New HttpListener() With {.IgnoreWriteExceptions = True}
    Dim WithEvents RequestWorker As New BackgroundWorker()
    Dim WithEvents SenderWorker As New BackgroundWorker() With {.WorkerReportsProgress = True}

    Private Sub PKGSender_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If Not HttpListener.IsSupported Then
            MsgBox("Unable to start a web server - Required to send .pkg files.", MsgBoxStyle.Critical)
            LogTextBox.AppendText("Unable to start a web server - Required to send .pkg files." & vbCrLf)
        Else
            'StartWebServer()
        End If
    End Sub

    Private Sub PKGSender_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        'Stop the web server
        NewHttpListener.Stop()
    End Sub

#Region "Server & PKG Sender"

    Private Sub SendPKGButton_Click(sender As Object, e As RoutedEventArgs) Handles SendPKGButton.Click
        If PKGsListView.SelectedItem IsNot Nothing Then
            Dim SelectedPKG As PKGLVItem = CType(PKGsListView.SelectedItem, PKGLVItem)
            Dim InstallResponse = InstallPKG($"http://{PS4IPTextBox.Text}:12800/api/install", SelectedPKG.InstallData)

            If InstallResponse.Contains("task_id") Then
                Dim TaskID = Long.Parse(JToken.Parse(InstallResponse)("task_id").ToString())
                'MsgBox("Installation Task ID = " + TaskID.ToString())
            End If
        End If
    End Sub

    Private Sub BrowsePKGButton_Click(sender As Object, e As RoutedEventArgs) Handles BrowsePKGButton.Click
        Dim OFD As New OpenFileDialog() With {.Filter = "pkg files (*.pkg)|*.pkg", .CheckFileExists = True, .Multiselect = True}
        If OFD.ShowDialog() = Forms.DialogResult.OK Then

            If OFD.FileNames.Count > 1 Then
                For Each SelectedPKG As String In OFD.FileNames
                    Dim PKGFileName As String = Path.GetFileName(SelectedPKG)
                    Dim NewPKGListViewItem As New PKGLVItem() With {.PackagePath = SelectedPKG, .PackageName = Path.GetFileNameWithoutExtension(SelectedPKG)}
                    Dim NewInstallData As String = GetJSONForPKG(PKGFileName)
                    NewPKGListViewItem.InstallData = NewInstallData
                    PKGsListView.Items.Add(NewPKGListViewItem)
                Next
            Else
                Dim PKGFileName As String = Path.GetFileName(OFD.FileName)
                Dim NewPKGListViewItem As New PKGLVItem() With {.PackagePath = OFD.FileName, .PackageName = Path.GetFileNameWithoutExtension(OFD.FileName)}
                Dim NewInstallData As String = GetJSONForPKG(PKGFileName)
                NewPKGListViewItem.InstallData = NewInstallData
                PKGsListView.Items.Add(NewPKGListViewItem)
            End If

        End If
    End Sub

    Public Structure PS4PKG
        Public Property type As String
        Public Property packages As String()
    End Structure

    Public Structure PKGLVItem
        Private _InstallData As String
        Private _PackagePath As String
        Private _PackageName As String

        Public Property PackageName As String
            Get
                Return _PackageName
            End Get
            Set
                _PackageName = Value
            End Set
        End Property

        Public Property PackagePath As String
            Get
                Return _PackagePath
            End Get
            Set
                _PackagePath = Value
            End Set
        End Property

        Public Property InstallData As String
            Get
                Return _InstallData
            End Get
            Set
                _InstallData = Value
            End Set
        End Property
    End Structure

    Private Sub StartWebServer()
        NewHttpListener.Prefixes.Add($"http://*:1336/")

        'Start listening
        Try
            NewHttpListener.Start()
            LogTextBox.AppendText("Web server started listening on port 1336" & vbCrLf)

            RequestWorker.RunWorkerAsync()
        Catch ex As Exception
            MsgBox($"Error trying to start server : {ex.Message}")
            Return
        End Try
    End Sub

    Private Sub RequestWorker_DoWork(sender As Object, e As DoWorkEventArgs) Handles RequestWorker.DoWork
        Do
            Dim context = NewHttpListener.GetContext
            Dim response As HttpListenerResponse = context.Response
            Dim request = context.Request

            Select Case request.RawUrl.ToLower
                Case "/"
                    WriteResponse(DefaultPage(), response)
                Case Else
                    'OfferPKG(context, "Cuphead.pkg")
                    WriteResponse(Get404(), response)
            End Select

            response.OutputStream.Close()
        Loop
    End Sub

    Private Sub WriteResponse(Text As String, Resp As HttpListenerResponse)
        Dim TextBytes As Byte() = Encoding.UTF8.GetBytes(Text)
        Resp.ContentLength64 = TextBytes.Length
        Resp.OutputStream.Write(TextBytes, 0, TextBytes.Length)
    End Sub

    Private Function DefaultPage() As String
        Dim StrBuilder As New StringBuilder
        StrBuilder.AppendLine("<HTML>")
        StrBuilder.AppendLine($"<b style=""font-size:32px"">PS Multi Tools v13</b>")
        StrBuilder.AppendLine("</br>")
        StrBuilder.AppendLine($"<b style=""font-size:24px"">PKG Web Server v0.1</b>")
        StrBuilder.AppendLine("</HTML>")
        Return StrBuilder.ToString()
    End Function

    Private Function Get404() As String
        Dim StrBuilder As New StringBuilder
        StrBuilder.AppendLine("<HTML>")
        StrBuilder.AppendLine($"<b style=""font-size:32px"">PS Multi Tools v13</b>")
        StrBuilder.AppendLine("</br>")
        StrBuilder.AppendLine($"<b style=""font-size:24px"">Error 404 - There is nothing here.</b>")
        StrBuilder.AppendLine("</HTML>")
        Return StrBuilder.ToString()
    End Function

    Private Sub OfferPKG(HttpLisContxt As HttpListenerContext, FilePath As String)
        Dim response = HttpLisContxt.Response

        Using NewFS As FileStream = File.OpenRead(FilePath)
            Dim PKGToSend As String = Path.GetFileName(FilePath)
            response.ContentLength64 = NewFS.Length
            response.KeepAlive = True
            response.ContentType = Mime.MediaTypeNames.Application.Octet
            response.AddHeader("Content-disposition", "attachment; filename=" & PKGToSend)
            Dim Buffer As Byte() = New Byte(65535) {}
            Dim Read As Integer

            Using bw As New BinaryWriter(response.OutputStream)
                Do
                    Read = NewFS.Read(Buffer, 0, Buffer.Length)
                    bw.Write(Buffer, 0, Read)
                    bw.Flush()
                Loop While Read > 0
            End Using

            response.StatusCode = HttpStatusCode.OK
            response.StatusDescription = "OK"
            response.OutputStream.Close()
        End Using
    End Sub

    Public Function GetJSONForPKG(PKGFileName As String) As String
        Dim JSerializer As New JsonSerializer With {.NullValueHandling = NullValueHandling.Ignore}
        Dim PKGforJ As New PS4PKG() With {.type = "direct", .packages = {"http://192.168.178.23:1336/" + PKGFileName}}
        Return JsonConvert.SerializeObject(PKGforJ)
    End Function

    Private Function InstallPKG(url As String, data As String) As String
        Using PostClient As New WebClient() With {.Encoding = Encoding.UTF8}
            Return PostClient.UploadString(url, data)
        End Using
    End Function

#End Region

#Region "Payload Sender"

    Public Structure PayloadLVItem
        Private _PayloadPath As String
        Private _PayloadName As String
        Private _IPAddress As IPAddress

        Public Property PayloadName As String
            Get
                Return _PayloadName
            End Get
            Set
                _PayloadName = Value
            End Set
        End Property

        Public Property PayloadPath As String
            Get
                Return _PayloadPath
            End Get
            Set
                _PayloadPath = Value
            End Set
        End Property

        Public Property IPAddress As IPAddress
            Get
                Return _IPAddress
            End Get
            Set
                _IPAddress = Value
            End Set
        End Property
    End Structure

    Private Sub BrowsePayloadButton_Click(sender As Object, e As RoutedEventArgs) Handles BrowsePayloadButton.Click
        Dim OFD As New OpenFileDialog() With {.Title = "Select a PS4 payload", .Multiselect = True, .Filter = "bin files (*.bin)|*.bin"}

        If OFD.ShowDialog() = Forms.DialogResult.OK Then

            If OFD.FileNames.Count > 1 Then
                For Each SelectedPayload As String In OFD.FileNames
                    Dim PayloadFileName As String = Path.GetFileName(SelectedPayload)
                    Dim NewPayloadListViewItem As New PayloadLVItem() With {.PayloadPath = SelectedPayload, .PayloadName = Path.GetFileNameWithoutExtension(SelectedPayload)}

                    PayloadListView.Items.Add(NewPayloadListViewItem)
                Next
            Else
                Dim PayloadFileName As String = Path.GetFileName(OFD.FileName)
                Dim ConsoleIP As IPAddress = IPAddress.Parse(PS4IPTextBox.Text)
                Dim NewPayloadListViewItem As New PayloadLVItem() With {.PayloadPath = OFD.FileName, .PayloadName = Path.GetFileNameWithoutExtension(OFD.FileName), .IPAddress = ConsoleIP}

                PayloadListView.Items.Add(NewPayloadListViewItem)
            End If

        End If

    End Sub

    Private Sub SendPayloadButton_Click(sender As Object, e As RoutedEventArgs) Handles SendPayloadButton.Click

        If PayloadListView.SelectedItem IsNot Nothing Then
            Dim SelectedPayload As PayloadLVItem = CType(PayloadListView.SelectedItem, PayloadLVItem)
            SenderWorker.RunWorkerAsync(SelectedPayload)
        End If

    End Sub

    Private Sub SenderWorker_DoWork(sender As Object, e As DoWorkEventArgs) Handles SenderWorker.DoWork

        Dim SelectedPayload As PayloadLVItem = CType(e.Argument, PayloadLVItem)

        'Send the selected payload
        Using SenderSocket As New Socket(SocketType.Stream, ProtocolType.Tcp) With {.ReceiveTimeout = 3000}
            SenderSocket.Connect(SelectedPayload.IPAddress, 9020)
            SenderSocket.SendFile(SelectedPayload.PayloadPath)
        End Using

    End Sub

    Private Sub SenderWorker_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles SenderWorker.RunWorkerCompleted
        MsgBox("Payload sent.")
    End Sub


#End Region

End Class
