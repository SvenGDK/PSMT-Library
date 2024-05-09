Imports System.ComponentModel
Imports System.IO
Imports System.Net.NetworkInformation
Imports System.Windows
Imports System.Windows.Forms
Imports Microsoft.Win32

Public Class PPPwner

    Dim WithEvents PPPwnWoker As New BackgroundWorker()
    Dim WithEvents PPPwn As New Process()

    Private Sub PPPwner_ContentRendered(sender As Object, e As EventArgs) Handles Me.ContentRendered
        'List Ethernet Adapters
        For Each AvailableNetworkInterface As NetworkInterface In NetworkInterface.GetAllNetworkInterfaces()
            Select Case AvailableNetworkInterface.NetworkInterfaceType
                'Show only Ethernet Interfaces
                Case NetworkInterfaceType.Ethernet, NetworkInterfaceType.FastEthernetT, NetworkInterfaceType.GigabitEthernet, NetworkInterfaceType.Ppp
                    'Dim AvailableNetworkInterfaceProperties As IPInterfaceProperties = AvailableNetworkInterface.GetIPProperties()
                    EthernetInterfacesComboBox.Items.Add(AvailableNetworkInterface.Description)
            End Select
        Next

        'Check if Npcap is installed
        If Registry.LocalMachine.OpenSubKey("SOFTWARE\Npcap", False) Is Nothing AndAlso Registry.LocalMachine.OpenSubKey("SOFTWARE\WOW6432Node\Npcap", False) Is Nothing Then
            If MsgBox("Npcap is not installed." + vbCrLf + "Do you want to install it now ? (Required)", MsgBoxStyle.YesNo, "Npcap Required") = MsgBoxResult.Yes Then
                Process.Start(My.Computer.FileSystem.CurrentDirectory + "\Tools\npcap-1.79.exe")
            End If
        End If
    End Sub

    Private Sub StartPPPwnButton_Click(sender As Object, e As RoutedEventArgs) Handles StartPPPwnButton.Click
        If StartPPPwnButton.Content.ToString() = "Stop PPPwn" Then
            If PPPwn.HasExited = False Then
                'Stop PPPwn
                PPPwn.Kill()

                'Update Button
                If Dispatcher.CheckAccess() = False Then
                    Dispatcher.BeginInvoke(Sub()
                                               StartPPPwnButton.IsEnabled = False
                                               StartPPPwnButton.Content = "Start PPPWn"
                                           End Sub)
                Else
                    StartPPPwnButton.IsEnabled = False
                    StartPPPwnButton.Content = "Start PPPWn"
                End If
            End If
        Else
            If EthernetInterfacesComboBox.SelectedItem IsNot Nothing AndAlso FirmwaresComboBox.SelectedItem IsNot Nothing Then

                'Get selected Ethernet interface
                Dim SelectedEthernetInterface As String = EthernetInterfacesComboBox.Text
                'Set firmware
                Dim SelectedFirmware As String = ""
                Select Case FirmwaresComboBox.Text
                    Case "8.50"
                        SelectedFirmware = "850"
                    Case "9.00"
                        SelectedFirmware = "900"
                    Case "9.03"
                        SelectedFirmware = "903"
                    Case "9.04"
                        SelectedFirmware = "904"
                    Case "9.50"
                        SelectedFirmware = "950"
                    Case "9.60"
                        SelectedFirmware = "960"
                    Case "10.00"
                        SelectedFirmware = "1000"
                    Case "10.01"
                        SelectedFirmware = "1001"
                    Case "10.50"
                        SelectedFirmware = "1050"
                    Case "10.70"
                        SelectedFirmware = "1070"
                    Case "10.71"
                        SelectedFirmware = "1071"
                    Case "11.00"
                        SelectedFirmware = "1100"
                End Select

                'Set the files for stage1 & stage2
                Dim Stage1File As String = ""
                Dim Stage2File As String = ""
                If UseSiSStageFilesCheckBox.IsChecked Then
                    Select Case SelectedFirmware
                        Case "900"
                            Stage1File = My.Computer.FileSystem.CurrentDirectory + "\Tools\PS4\stage1\SiS-stage1-900.bin"
                            Stage2File = My.Computer.FileSystem.CurrentDirectory + "\Tools\PS4\stage2\SiS-stage2-900.bin"
                        Case "1100"
                            Stage1File = My.Computer.FileSystem.CurrentDirectory + "\Tools\PS4\stage1\SiS-stage1-1100.bin"
                            Stage2File = My.Computer.FileSystem.CurrentDirectory + "\Tools\PS4\stage2\SiS-stage2-1100.bin"
                    End Select
                ElseIf UseCustomStageFilesCheckBox.IsChecked Then
                    Stage1File = CustomStage1PayloadTextBox.Text
                    Stage2File = CustomStage2PayloadTextBox.Text
                Else
                    Select Case SelectedFirmware
                        Case "850"
                            Stage1File = My.Computer.FileSystem.CurrentDirectory + "\Tools\PS4\stage1\ToF-stage1-850.bin"
                            Stage2File = My.Computer.FileSystem.CurrentDirectory + "\Tools\PS4\stage2\ToF-stage2-850.bin"
                        Case "900"
                            Stage1File = My.Computer.FileSystem.CurrentDirectory + "\Tools\PS4\stage1\ToF-stage1-900.bin"
                            Stage2File = My.Computer.FileSystem.CurrentDirectory + "\Tools\PS4\stage2\ToF-stage2-900.bin"
                        Case "903", "904"
                            Stage1File = My.Computer.FileSystem.CurrentDirectory + "\Tools\PS4\stage1\ToF-stage1-903.bin"
                            Stage2File = My.Computer.FileSystem.CurrentDirectory + "\Tools\PS4\stage2\ToF-stage2-903.bin"
                        Case "950", "960"
                            Stage1File = My.Computer.FileSystem.CurrentDirectory + "\Tools\PS4\stage1\ToF-stage1-950.bin"
                            Stage2File = My.Computer.FileSystem.CurrentDirectory + "\Tools\PS4\stage2\ToF-stage2-950.bin"
                        Case "1000", "1001"
                            Stage1File = My.Computer.FileSystem.CurrentDirectory + "\Tools\PS4\stage1\ToF-stage1-1000.bin"
                            Stage2File = My.Computer.FileSystem.CurrentDirectory + "\Tools\PS4\stage2\ToF-stage2-1000.bin"
                        Case "1050", "1070", "1071"
                            Stage1File = My.Computer.FileSystem.CurrentDirectory + "\Tools\PS4\stage1\ToF-stage1-1050.bin"
                            Stage2File = My.Computer.FileSystem.CurrentDirectory + "\Tools\PS4\stage2\ToF-stage2-1050.bin"
                        Case "1100"
                            Stage1File = My.Computer.FileSystem.CurrentDirectory + "\Tools\PS4\stage1\ToF-stage1-1100.bin"
                            Stage2File = My.Computer.FileSystem.CurrentDirectory + "\Tools\PS4\stage2\ToF-stage2-1100.bin"
                    End Select
                End If

                'Update button
                If Dispatcher.CheckAccess() = False Then
                    Dispatcher.BeginInvoke(Sub()
                                               StartPPPwnButton.Content = "Stop PPPWn"
                                           End Sub)
                Else
                    StartPPPwnButton.Content = "Stop PPPWn"
                End If

                PPPwnWoker.RunWorkerAsync("--interface """ + SelectedEthernetInterface + """ --fw " + SelectedFirmware + " --stage1 """ + Stage1File + """ --stage2 """ + Stage2File + """")
            Else
                MsgBox("Please select your Ethernet interface, PS4 firmware and Payload.", MsgBoxStyle.Exclamation, "Error")
            End If
        End If
    End Sub

    Private Sub PPPwn_Exited(sender As Object, e As EventArgs) Handles PPPwn.Exited
        PPPwn.Dispose()

        'Update button on exit
        If Dispatcher.CheckAccess() = False Then
            Dispatcher.BeginInvoke(Sub()
                                       StartPPPwnButton.IsEnabled = True
                                       StartPPPwnButton.Content = "Start PPPWn"
                                   End Sub)
        Else
            StartPPPwnButton.IsEnabled = True
            StartPPPwnButton.Content = "Start PPPWn"
        End If
    End Sub

    Private Sub UseSiSStageFilesCheckBox_Checked(sender As Object, e As RoutedEventArgs) Handles UseSiSStageFilesCheckBox.Checked
        Select Case FirmwaresComboBox.Text
            Case "9.00", "11.00"
                UseCustomStageFilesCheckBox.IsEnabled = False
            Case Else
                MsgBox("Not compatible with selected Firmware.", MsgBoxStyle.Exclamation, "Only for 9.00 & 11.00")
                UseSiSStageFilesCheckBox.IsChecked = False
                e.Handled = True
        End Select
    End Sub

    Private Sub UseCustomStageFilesCheckBox_Checked(sender As Object, e As RoutedEventArgs) Handles UseCustomStageFilesCheckBox.Checked
        UseSiSStageFilesCheckBox.IsEnabled = False
    End Sub

    Private Sub UseSiSStageFilesCheckBox_Unchecked(sender As Object, e As RoutedEventArgs) Handles UseSiSStageFilesCheckBox.Unchecked
        UseCustomStageFilesCheckBox.IsEnabled = True
    End Sub

    Private Sub UseCustomStageFilesCheckBox_Unchecked(sender As Object, e As RoutedEventArgs) Handles UseCustomStageFilesCheckBox.Unchecked
        UseSiSStageFilesCheckBox.IsEnabled = True
    End Sub

    Private Sub BrowseStage1PayloadButton_Click(sender As Object, e As RoutedEventArgs) Handles BrowseStage1PayloadButton.Click
        Dim OFD As New Windows.Forms.OpenFileDialog() With {.Title = "Select a stage1 payload", .Filter = "BIN files (*.bin)|*.bin"}
        If OFD.ShowDialog() = Windows.Forms.DialogResult.OK Then
            CustomStage1PayloadTextBox.Text = OFD.FileName
        Else
            MsgBox("No file selected.", MsgBoxStyle.Exclamation)
        End If
    End Sub

    Private Sub BrowseStage2PayloadButton_Click(sender As Object, e As RoutedEventArgs) Handles BrowseStage2PayloadButton.Click
        Dim OFD As New Windows.Forms.OpenFileDialog() With {.Title = "Select a stage2 payload", .Filter = "BIN files (*.bin)|*.bin"}
        If OFD.ShowDialog() = Windows.Forms.DialogResult.OK Then
            CustomStage2PayloadTextBox.Text = OFD.FileName
        Else
            MsgBox("No file selected.", MsgBoxStyle.Exclamation)
        End If
    End Sub

    Private Sub PPPwnWoker_DoWork(sender As Object, e As DoWorkEventArgs) Handles PPPwnWoker.DoWork
        'Set PPPwn process properties
        PPPwn = New Process()
        PPPwn.StartInfo.FileName = My.Computer.FileSystem.CurrentDirectory + "\Tools\pppwn.exe"
        PPPwn.StartInfo.Arguments = e.Argument.ToString()
        PPPwn.StartInfo.RedirectStandardOutput = False
        PPPwn.StartInfo.UseShellExecute = False
        PPPwn.StartInfo.CreateNoWindow = False
        PPPwn.EnableRaisingEvents = True

        AddHandler PPPwn.OutputDataReceived, Sub(SenderProcess As Object, DataArgs As DataReceivedEventArgs)
                                                 If Not String.IsNullOrEmpty(DataArgs.Data) Then
                                                     Console.WriteLine(DataArgs.Data)
                                                     'Append log from PPPWn
                                                     If Dispatcher.CheckAccess() = False Then
                                                         Dispatcher.BeginInvoke(Sub()
                                                                                    '... did not work
                                                                                End Sub)
                                                     Else
                                                         '... did not work
                                                     End If
                                                 End If
                                             End Sub

        'Start PPPwn & read process output data
        PPPwn.Start()
        'PPPwn.BeginOutputReadLine()
    End Sub

    Private Sub CopyGoldHENButton_Click(sender As Object, e As RoutedEventArgs) Handles CopyGoldHENButton.Click
        Dim FBD As New FolderBrowserDialog() With {.Description = "Select an USB drive"}
        If FBD.ShowDialog() = Forms.DialogResult.OK Then
            If File.Exists(My.Computer.FileSystem.CurrentDirectory + "\Tools\PS4\goldhen.bin") Then
                Try
                    File.Copy(My.Computer.FileSystem.CurrentDirectory + "\Tools\PS4\goldhen.bin", FBD.SelectedPath + "goldhen.bin", True)
                    MsgBox("Copy done!", MsgBoxStyle.Information)
                Catch ex As Exception
                    MsgBox("Could not copy GoldHEN to selected USB drive.", MsgBoxStyle.Exclamation, "Error")
                End Try
            Else
                MsgBox("Could not find goldhen.bin at " + My.Computer.FileSystem.CurrentDirectory + "\Tools\PS4\goldhen.bin", MsgBoxStyle.Exclamation, "Error")
            End If
        Else
            MsgBox("No USB drive selected.", MsgBoxStyle.Exclamation, "Error")
        End If
    End Sub

End Class
