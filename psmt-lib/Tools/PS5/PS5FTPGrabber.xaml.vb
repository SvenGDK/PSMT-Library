Imports System.ComponentModel
Imports System.Windows.Input
Imports WinSCP

Public Class PS5FTPGrabber

    Public ConsoleIP As String = String.Empty

    Dim SelectedPath As String = String.Empty
    Dim App0RemoteFolder As String = String.Empty
    Dim DiscRemoteFolder As String = String.Empty
    Dim CustomRemoteFolder As String = String.Empty

    Dim TotalFiles As Integer = 0
    Dim CopiedFiles As Integer = 0

    Dim WithEvents CopyWorker As New BackgroundWorker With {.WorkerReportsProgress = True}
    Dim DumpResult As SynchronizationResult

    Private Sub DownloadButton_Click(sender As Object, e As Windows.RoutedEventArgs) Handles DownloadButton.Click

        App0RemoteFolder = String.Empty
        DiscRemoteFolder = String.Empty
        CustomRemoteFolder = String.Empty

        If SelectedFolderComboBox.Text = "/mnt/sandbox/pfsmnt/" Then

            Cursor = Cursors.Wait

            If GetApp0(ConsoleIP) = True Then

                If Dispatcher.CheckAccess() = False Then
                    Dispatcher.BeginInvoke(Sub()
                                               ReceiveStatusTextBlock.Text = "Starting, please wait ... 0/" + TotalFiles.ToString()
                                               ReceiveProgressBar.Maximum = TotalFiles
                                           End Sub)
                Else
                    ReceiveStatusTextBlock.Text = "Starting, please wait ... 0/" + TotalFiles.ToString()
                    ReceiveProgressBar.Maximum = TotalFiles
                End If

                CopyWorker.RunWorkerAsync()
            Else
                Cursor = Cursors.Arrow
                MsgBox("Could not find any mounted game.", MsgBoxStyle.Exclamation, "Error reading data")
            End If

        ElseIf SelectedFolderComboBox.Text = "/mnt/disc/" Then

            Cursor = Cursors.Wait

            If FilesAvailable("/mnt/disc") = True Then
                DiscRemoteFolder = "/mnt/disc/"

                If Dispatcher.CheckAccess() = False Then
                    Dispatcher.BeginInvoke(Sub()
                                               ReceiveStatusTextBlock.Text = "Starting, please wait ... 0/" + TotalFiles.ToString()
                                               ReceiveProgressBar.Maximum = TotalFiles
                                           End Sub)
                Else
                    ReceiveStatusTextBlock.Text = "Starting, please wait ... 0/" + TotalFiles.ToString()
                    ReceiveProgressBar.Maximum = TotalFiles
                End If

                CopyWorker.RunWorkerAsync()
            Else
                Cursor = Cursors.Arrow
                MsgBox("No disc inserted !", MsgBoxStyle.Critical, "Error reading data")
            End If

        Else

            CustomRemoteFolder = SelectedFolderComboBox.Text
            Cursor = Cursors.Wait

            If FilesAvailable(CustomRemoteFolder) = True Then
                If Dispatcher.CheckAccess() = False Then
                    Dispatcher.BeginInvoke(Sub()
                                               ReceiveStatusTextBlock.Text = "Starting, please wait ... 0/" + TotalFiles.ToString()
                                               ReceiveProgressBar.Maximum = TotalFiles
                                           End Sub)
                Else
                    ReceiveStatusTextBlock.Text = "Starting, please wait ... 0/" + TotalFiles.ToString()
                    ReceiveProgressBar.Maximum = TotalFiles
                End If

                CopyWorker.RunWorkerAsync()
            Else
                Cursor = Cursors.Arrow
                MsgBox("No files available !", MsgBoxStyle.Critical, "Error reading data")
            End If

        End If

    End Sub

    Public Function GetApp0(ConsoleIP As String) As Boolean
        Dim sessionOptions As New SessionOptions
        With sessionOptions
            .Protocol = Protocol.Ftp
            .HostName = ConsoleIP
            .UserName = "anonymous"
            .Password = "anonymous"
            .PortNumber = 1337
        End With

        Dim NewSession As New Session()

        Try
            ' Connect
            NewSession.Open(sessionOptions)

            ' Get the app0 folder
            For Each DirectoryInFTP As RemoteFileInfo In NewSession.EnumerateRemoteFiles(SelectedFolderComboBox.Text, "", EnumerationOptions.MatchDirectories)
                If DirectoryInFTP.Name.EndsWith("app0") Then
                    App0RemoteFolder = DirectoryInFTP.FullName
                    Exit For
                End If
            Next

        Catch ex As Exception
            MsgBox(ex.Message)
            MsgBox(ex.InnerException.Message)
        End Try

        If String.IsNullOrEmpty(App0RemoteFolder) Then
            Return False
        Else
            Try
                ' Reset and get total files count
                TotalFiles = 0
                CopiedFiles = 0

                For Each FileInFTP In NewSession.EnumerateRemoteFiles(App0RemoteFolder, "", EnumerationOptions.AllDirectories)
                    TotalFiles += 1
                Next
            Catch ex As Exception
                MsgBox(ex.Message)
                MsgBox(ex.InnerException.Message)
            End Try

            Return True
        End If
    End Function

    Public Function FilesAvailable(RemotePath As String) As Boolean
        Dim sessionOptions As New SessionOptions
        With sessionOptions
            .Protocol = Protocol.Ftp
            .HostName = ConsoleIP
            .UserName = "anonymous"
            .Password = "anonymous"
            .PortNumber = 1337
        End With

        Dim NewSession As New Session()

        ' Connect
        NewSession.Open(sessionOptions)

        ' Reset and get total files count
        TotalFiles = 0
        CopiedFiles = 0

        For Each FileInFTP In NewSession.EnumerateRemoteFiles(RemotePath, "", EnumerationOptions.AllDirectories)
            TotalFiles += 1
        Next

        If TotalFiles > 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Sub CopyWorker_DoWork(sender As Object, e As DoWorkEventArgs) Handles CopyWorker.DoWork
        Try
            'Setup session options
            Dim NewSessionOptions As New SessionOptions
            With NewSessionOptions
                .Protocol = Protocol.Ftp
                .HostName = ConsoleIP
                .UserName = "anonymous"
                .Password = "anonymous"
                .PortNumber = 1337
            End With

            Dim NewSession As New Session()

            'Report progress of synchronization
            AddHandler NewSession.FileTransferred, AddressOf FileTransferred

            'Connect
            NewSession.Open(NewSessionOptions)

            'Synchronize files and folders
            If Not String.IsNullOrEmpty(App0RemoteFolder) Then
                DumpResult = NewSession.SynchronizeDirectories(SynchronizationMode.Local, SelectedPath, App0RemoteFolder, False)
            End If
            If Not String.IsNullOrEmpty(DiscRemoteFolder) Then
                DumpResult = NewSession.SynchronizeDirectories(SynchronizationMode.Local, SelectedPath, DiscRemoteFolder, False)
            End If
            If Not String.IsNullOrEmpty(CustomRemoteFolder) Then
                DumpResult = NewSession.SynchronizeDirectories(SynchronizationMode.Local, SelectedPath, CustomRemoteFolder, False)
            End If

        Catch ex As Exception
            MsgBox("Error reading data.", MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub FileTransferred(sender As Object, e As TransferEventArgs)
        CopiedFiles += 1

        If Dispatcher.CheckAccess() = False Then
            Dispatcher.BeginInvoke(Sub()
                                       ReceiveStatusTextBlock.Text = e.FileName + " copied. " + CopiedFiles.ToString() + "/" + TotalFiles.ToString()
                                       ReceiveProgressBar.Value += 1
                                   End Sub)
        Else
            ReceiveStatusTextBlock.Text = e.FileName + " copied. " + CopiedFiles.ToString() + "/" + TotalFiles.ToString()
            ReceiveProgressBar.Value += 1
        End If
    End Sub

    Private Sub CopyWorker_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles CopyWorker.RunWorkerCompleted

        Cursor = Cursors.Arrow

        If Dispatcher.CheckAccess() = False Then
            Dispatcher.BeginInvoke(Sub() ReceiveProgressBar.Value = 0)
        Else
            ReceiveProgressBar.Value = 0
        End If

        ' Throw on any error
        DumpResult.Check()

        ' Check result
        If DumpResult.IsSuccess Then
            MsgBox("Dump completed.", MsgBoxStyle.Information, "Success")
        Else
            MsgBox("Could not dump all files.", MsgBoxStyle.Exclamation)
        End If

    End Sub

    Private Sub BrowseFolderButton_Click(sender As Object, e As Windows.RoutedEventArgs) Handles BrowseFolderButton.Click
        Dim FBD As New Windows.Forms.FolderBrowserDialog()

        If FBD.ShowDialog() = Windows.Forms.DialogResult.OK Then
            SelectedDirectoryTextBox.Text = FBD.SelectedPath
            SelectedPath = FBD.SelectedPath
        End If
    End Sub

End Class
