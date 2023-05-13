Imports System.ComponentModel
Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Windows
Imports System.Windows.Forms

Public Class PS5Sender

    Dim WithEvents SenderWorker As New BackgroundWorker() With {.WorkerReportsProgress = True}
    ReadOnly Magic As UInteger = &HEA6E
    Dim TotalBytes As Integer = 0
    Dim CurrentType As SendType
    Public SelectedISO As String

    Public Structure WorkerArgs
        Private _DeviceIP As IPAddress
        Private _FileToSend As String
        Private _ChunkSize As Integer

        Public Property DeviceIP As IPAddress
            Get
                Return _DeviceIP
            End Get
            Set
                _DeviceIP = Value
            End Set
        End Property

        Public Property FileToSend As String
            Get
                Return _FileToSend
            End Get
            Set
                _FileToSend = Value
            End Set
        End Property

        Public Property ChunkSize As Integer
            Get
                Return _ChunkSize
            End Get
            Set
                _ChunkSize = Value
            End Set
        End Property
    End Structure

    Enum SendType
        ELF
        ISO
        CONF
    End Enum

    Private Sub SendELFButton_Click(sender As Object, e As RoutedEventArgs) Handles SendELFButton.Click
        'Check if a ELF is selected
        If Not String.IsNullOrEmpty(SelectedELFTextBox.Text) Then
            'Check if an IP address was entered
            If Not String.IsNullOrWhiteSpace(IPTextBox.Text) Then
                Dim DeviceIP As IPAddress

                Try
                    DeviceIP = IPAddress.Parse(IPTextBox.Text)
                Catch ex As FormatException
                    MsgBox("Could not send selected ELF. Please check your IP.", MsgBoxStyle.Exclamation, "Error sending file")
                    Exit Sub
                End Try

                Dim SelectedELF As String = SelectedELFTextBox.Text
                Dim ELFFileInfo As New FileInfo(SelectedELF)

                SendELFButton.IsEnabled = False
                SendISOButton.IsEnabled = False
                BrowseELFButton.IsEnabled = False
                BrowseISOButton.IsEnabled = False

                'Set the progress bar maximum and TotalBytes to send
                SendProgressBar.Value = 0
                SendProgressBar.Maximum = CDbl(ELFFileInfo.Length)
                TotalBytes = CInt(ELFFileInfo.Length)

                'Start sending
                CurrentType = SendType.ELF
                SenderWorker.RunWorkerAsync(New WorkerArgs() With {.DeviceIP = DeviceIP, .FileToSend = SelectedELF, .ChunkSize = 4096})
            Else
                MsgBox("No IP address was entered." + vbCrLf + "Please enter an IP address.", MsgBoxStyle.Exclamation, "No IP address")
            End If
        Else
            MsgBox("No ELF selected." + vbCrLf + "Please select an ELF first.", MsgBoxStyle.Exclamation, "No ELF selected")
        End If
    End Sub

    Private Sub SendISOButton_Click(sender As Object, e As RoutedEventArgs) Handles SendISOButton.Click
        'Check if a game is selected
        If Not String.IsNullOrEmpty(SelectedISOTextBox.Text) Then
            'Check if an IP address was entered before
            If Not String.IsNullOrWhiteSpace(IPTextBox.Text) Then

                If MsgBox("Send " + SelectedISOTextBox.Text + " to the console ?", MsgBoxStyle.YesNo, "Confirm") = MsgBoxResult.Yes Then

                    Dim DeviceIP As IPAddress = IPAddress.Parse(IPTextBox.Text)
                    Dim GameFileInfo As New FileInfo(SelectedISOTextBox.Text)

                    SendELFButton.IsEnabled = False
                    SendISOButton.IsEnabled = False
                    BrowseELFButton.IsEnabled = False
                    BrowseISOButton.IsEnabled = False

                    'Set the progress bar maximum and TotalBytes to send
                    SendProgressBar.Maximum = CDbl(GameFileInfo.Length)
                    TotalBytes = CInt(GameFileInfo.Length)

                    'Start sending
                    Dim WorkArgs As New WorkerArgs() With {.DeviceIP = DeviceIP, .FileToSend = SelectedISOTextBox.Text, .ChunkSize = 63488}
                    CurrentType = SendType.ISO
                    SenderWorker.RunWorkerAsync(WorkArgs)
                End If

            Else
                MsgBox("No IP address was entered." + vbCrLf + "Please enter an IP address on the main window and re-open the backup manager.", MsgBoxStyle.Exclamation, "No IP address")
            End If
        Else
            MsgBox("No game selected." + vbCrLf + "Please select a game first.", MsgBoxStyle.Exclamation, "No game selected")
        End If
    End Sub

    Private Sub SendConfigButton_Click(sender As Object, e As RoutedEventArgs) Handles SendConfigButton.Click
        'Open choose config dialog
        Dim OFD As New OpenFileDialog() With {.Title = "Select a .conf file", .Filter = "Config files (*.conf)|*.conf"}
        Dim DeviceIP As IPAddress = IPAddress.Parse(IPTextBox.Text)

        If OFD.ShowDialog() = Forms.DialogResult.OK Then

            SendELFButton.IsEnabled = False
            SendISOButton.IsEnabled = False
            BrowseELFButton.IsEnabled = False
            BrowseISOButton.IsEnabled = False

            Dim ConfigFileInfo As New FileInfo(OFD.FileName)
            Dim FilePath As String = Path.GetFullPath(OFD.FileName)

            'Set the progress bar maximum and TotalBytes to send
            SendProgressBar.Value = 0
            SendProgressBar.Maximum = CDbl(ConfigFileInfo.Length)
            TotalBytes = CInt(ConfigFileInfo.Length)

            'Start sending
            Dim WorkArgs As New WorkerArgs() With {.DeviceIP = DeviceIP, .FileToSend = FilePath, .ChunkSize = 10}
            CurrentType = SendType.CONF
            SenderWorker.RunWorkerAsync(WorkArgs)
        End If
    End Sub

    Private Sub SenderWorker_DoWork(sender As Object, e As DoWorkEventArgs) Handles SenderWorker.DoWork

        Dim CurrentWorkerArgs As WorkerArgs = CType(e.Argument, WorkerArgs)

        Dim FileInfos As New FileInfo(CurrentWorkerArgs.FileToSend)
        Dim FileSizeAsLong As Long = FileInfos.Length
        Dim FileSizeAsULong As ULong = CULng(FileInfos.Length)

        Dim MagicBytes = BytesConverter.ToLittleEndian(Magic)
        Dim NewFileSizeBytes = BytesConverter.ToLittleEndian(FileSizeAsULong)

        Using SenderSocket As New Socket(SocketType.Stream, ProtocolType.Tcp) With {.ReceiveTimeout = 3000}

            SenderSocket.Connect(CurrentWorkerArgs.DeviceIP, 9045)

            SenderSocket.Send(MagicBytes)
            SenderSocket.Send(NewFileSizeBytes)

            Dim BytesRead As Integer = 0
            Dim SendBytes As Integer = 0
            Dim Buffer(CurrentWorkerArgs.ChunkSize - 1) As Byte

            'Open the file and send
            Using SenderFileStream As New FileStream(CurrentWorkerArgs.FileToSend, FileMode.Open, FileAccess.Read)

                Do
                    BytesRead = SenderFileStream.Read(Buffer, 0, Buffer.Length)

                    If BytesRead > 0 Then
                        SendBytes += SenderSocket.Send(Buffer, 0, BytesRead, SocketFlags.None)

                        'Update the status text
                        If SendStatusTextBlock.Dispatcher.CheckAccess() = False Then
                            SendStatusTextBlock.Dispatcher.BeginInvoke(Sub() SendStatusTextBlock.Text = "Sending file: " + SendBytes.ToString + " bytes of " + TotalBytes.ToString + " bytes sent.")
                        Else
                            SendStatusTextBlock.Text = "Sending file: " + SendBytes.ToString + " of " + TotalBytes.ToString + " sent."
                        End If

                        'Update the status progress bar
                        If SendProgressBar.Dispatcher.CheckAccess() = False Then
                            SendProgressBar.Dispatcher.BeginInvoke(Sub() SendProgressBar.Value = SendBytes)
                        Else
                            SendProgressBar.Value = SendBytes
                        End If

                    End If
                Loop While BytesRead > 0

            End Using

            'Close the connection
            SenderSocket.Close()
        End Using

    End Sub

    Private Sub SenderWorker_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles SenderWorker.RunWorkerCompleted

        SendStatusTextBlock.Text = "Status:"
        SendProgressBar.Value = 0

        If Not e.Cancelled Then
            Select Case CurrentType
                Case SendType.ELF
                    SendConfigButton.IsEnabled = True
                    SendELFButton.IsEnabled = True
                    SendISOButton.IsEnabled = True
                    BrowseELFButton.IsEnabled = True
                    BrowseISOButton.IsEnabled = True
                    MsgBox("ELF successfully sent!")
                Case SendType.ISO
                    SendConfigButton.IsEnabled = True
                    SendELFButton.IsEnabled = True
                    SendISOButton.IsEnabled = True
                    BrowseELFButton.IsEnabled = True
                    BrowseISOButton.IsEnabled = True
                    MsgBox("Game successfully sent!" + vbCrLf + "You can now send a config file if you want to.", MsgBoxStyle.Information, "Success")
                Case SendType.CONF
                    SendConfigButton.IsEnabled = False
                    SendELFButton.IsEnabled = True
                    SendISOButton.IsEnabled = True
                    BrowseELFButton.IsEnabled = True
                    BrowseISOButton.IsEnabled = True
                    MsgBox("Config successfully sent!", MsgBoxStyle.Information, "Success")
            End Select
        End If

    End Sub

    Private Sub BrowseELFButton_Click(sender As Object, e As RoutedEventArgs) Handles BrowseELFButton.Click
        Dim OFD As New OpenFileDialog() With {.Title = "Select an .elf file", .Filter = "ELF files (*.elf)|*.elf"}
        If OFD.ShowDialog() = Forms.DialogResult.OK Then
            SelectedELFTextBox.Text = OFD.FileName
        End If
    End Sub

    Private Sub BrowseISOButton_Click(sender As Object, e As RoutedEventArgs) Handles BrowseISOButton.Click
        Dim OFD As New OpenFileDialog() With {.Title = "Select an .iso file", .Filter = "ELF files (*.iso)|*.iso"}
        If OFD.ShowDialog() = Forms.DialogResult.OK Then
            SelectedISOTextBox.Text = OFD.FileName
        End If
    End Sub

    Private Sub PS5Sender_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If Not String.IsNullOrEmpty(SelectedISO) Then
            SelectedELFTextBox.Text = SelectedISO
        End If
    End Sub
End Class
