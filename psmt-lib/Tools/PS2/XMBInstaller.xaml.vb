﻿Imports System.ComponentModel
Imports System.IO
Imports System.Net
Imports System.Windows
Imports System.Windows.Forms
Imports System.Windows.Media

Public Class XMBInstaller

    Public MountedDrive As MountedPSXDrive

    Public WithEvents HDL_DumpWorker As New BackgroundWorker() With {.WorkerReportsProgress = True}
    Dim WithEvents ContentDownloader As New WebClient()

    Private Sub XMBInstaller_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

        'Set up a projects directory to save all created projects
        If Not Directory.Exists(My.Computer.FileSystem.CurrentDirectory + "\Projects") Then
            Directory.CreateDirectory(My.Computer.FileSystem.CurrentDirectory + "\Projects")
        Else
            For Each SavedProject In Directory.GetFiles(My.Computer.FileSystem.CurrentDirectory + "\Projects", "*.CFG")

                Dim ProjectState As String = File.ReadAllLines(SavedProject)(5).Split("="c)(1)

                If ProjectState = "FALSE" Then
                    ProjectListComboBox.Items.Add(SavedProject)
                Else
                    ProjectListComboBox.Items.Add(SavedProject)
                    PreparedProjectsComboBox.Items.Add(SavedProject)
                End If

            Next
        End If

        'Check if NBD driver is installed
        Using WNBDClient As New Process()
            WNBDClient.StartInfo.FileName = My.Computer.FileSystem.CurrentDirectory + "\Tools\wnbd-client.exe"
            WNBDClient.StartInfo.Arguments = "-v"
            WNBDClient.StartInfo.RedirectStandardOutput = True
            WNBDClient.StartInfo.UseShellExecute = False
            WNBDClient.Start()

            Dim OutputReader As StreamReader = WNBDClient.StandardOutput
            Dim ProcessOutput As String = OutputReader.ReadToEnd()
            Dim SplittedOutput As String() = ProcessOutput.Split({vbCrLf}, StringSplitOptions.None)

            Dim NBDDriverVersion As String

            If Not SplittedOutput(2).Trim() = "" Then
                NBDDriverVersion = SplittedOutput(2).Trim().Split(":"c)(1).Trim()
                NBDDriverVersionLabel.Content = NBDDriverVersion
                NBDDriverVersionLabel.Foreground = Brushes.Red

                'Check if NBD is connected and if the drive is already mounted
                If IsNBDConnected() Then
                    InstallButton.IsEnabled = True
                    NBDConnectionLabel.Content = "Connected"
                    NBDConnectionLabel.Foreground = Brushes.Green
                    ConnectButton.Content = "Disconnect"
                ElseIf IsLocalHDDConnected() Then
                    MountedDrive.DriveID = GetHDDID()

                    InstallButton.IsEnabled = True
                    NBDConnectionStatusLabel.Content = "Local Connection:"
                    NBDConnectionLabel.Content = "Connected"

                    EnterIPTextBlock.Text = "Local PS2 HDD is connected."
                    PSXIPTextBox.IsEnabled = False
                    ConnectButton.Visibility = Visibility.Hidden

                    NBDConnectionLabel.Foreground = Brushes.Green
                    ConnectButton.Content = "Disconnect"
                End If
            Else
                NBDDriverVersionLabel.Content = "Not installed"
                NBDDriverVersionLabel.Foreground = Brushes.Red
            End If
        End Using

    End Sub

    Public Sub ReloadProjects()
        ProjectListComboBox.Items.Clear()
        PreparedProjectsComboBox.Items.Clear()

        For Each Projects In Directory.GetFiles(My.Computer.FileSystem.CurrentDirectory + "\Projects", "*.CFG")
            Dim ProjectState As String = File.ReadAllLines(Projects)(5).Split("="c)(1)
            If ProjectState = "FALSE" Then
                ProjectListComboBox.Items.Add(Projects)
            Else
                ProjectListComboBox.Items.Add(Projects)
                PreparedProjectsComboBox.Items.Add(Projects)
            End If
        Next
    End Sub

    Public Sub LockUI()
        If ProjectListComboBox.IsEnabled = False Then
            MainMenu.IsEnabled = True
            ProjectListComboBox.IsEnabled = True
            EditButton.IsEnabled = True
            PrepareButton.IsEnabled = True
            PSXIPTextBox.IsEnabled = True
            ConnectButton.IsEnabled = True
            PreparedProjectsComboBox.IsEnabled = True
            InstallButton.IsEnabled = True
        Else
            MainMenu.IsEnabled = False
            ProjectListComboBox.IsEnabled = False
            EditButton.IsEnabled = False
            PrepareButton.IsEnabled = False
            PSXIPTextBox.IsEnabled = False
            ConnectButton.IsEnabled = False
            PreparedProjectsComboBox.IsEnabled = False
            InstallButton.IsEnabled = False
        End If
    End Sub

#Region "Functions"

    Private Function IsNBDConnected() As Boolean
        Using WNBDClient As New Process()
            WNBDClient.StartInfo.FileName = My.Computer.FileSystem.CurrentDirectory + "\Tools\wnbd-client.exe"
            WNBDClient.StartInfo.Arguments = "list"
            WNBDClient.StartInfo.RedirectStandardOutput = True
            WNBDClient.StartInfo.UseShellExecute = False
            WNBDClient.StartInfo.CreateNoWindow = True
            WNBDClient.Start()

            Dim OutputReader As StreamReader = WNBDClient.StandardOutput
            Dim ProcessOutput As String = OutputReader.ReadToEnd()

            If ProcessOutput.Contains("wnbd-client") Then 'This is only shown when a drive is actually mounted

                If ProcessOutput.Contains("PS2HDD") Then 'Used by PFS BatchKit Manager
                    MountedDrive.NBDDriveName = "PS2HDD"
                    PSXIPTextBox.Text = GetConnectedNBDIP("PS2HDD")
                ElseIf ProcessOutput.Contains("PSXHDD") Then 'Used by PSX XMB Manager
                    MountedDrive.NBDDriveName = "PSXHDD"
                    PSXIPTextBox.Text = GetConnectedNBDIP("PSXHDD")
                End If

                MountedDrive.HDLDriveName = GetHDLDriveName()
                MountedDrive.DriveID = GetHDDID()

                Return True
            Else
                Return False
            End If
        End Using
    End Function

    Private Function IsLocalHDDConnected() As Boolean
        'Query the drives
        Using HDLDump As New Process()
            HDLDump.StartInfo.FileName = My.Computer.FileSystem.CurrentDirectory + "\Tools\hdl_dump.exe"
            HDLDump.StartInfo.Arguments = "query"
            HDLDump.StartInfo.RedirectStandardOutput = True
            HDLDump.StartInfo.UseShellExecute = False
            HDLDump.StartInfo.CreateNoWindow = True
            HDLDump.Start()

            'Read the output
            Dim OutputReader As StreamReader = HDLDump.StandardOutput
            Dim ProcessOutput As String() = OutputReader.ReadToEnd().Split({vbCrLf}, StringSplitOptions.None)

            'Find the local drive
            For Each Line As String In ProcessOutput
                If Not String.IsNullOrWhiteSpace(Line) Then
                    If Line.Contains("formatted Playstation 2 HDD") Then
                        'Set the found drive as mounted PSX drive
                        Dim DriveInfos As String() = Line.Split(New String() {" "}, StringSplitOptions.RemoveEmptyEntries)
                        If DriveInfos(0) IsNot Nothing Then
                            MountStatusLabel.Content = "on " + DriveInfos(0).Trim()
                            MountStatusLabel.Foreground = Brushes.Green
                            MountedDrive.HDLDriveName = DriveInfos(0).Trim()
                            Return True
                        End If
                    End If
                End If
            Next
        End Using
    End Function

    Private Function GetConnectedNBDIP(NBDDriveName As String) As String
        Using WNBDClient As New Process()
            WNBDClient.StartInfo.FileName = My.Computer.FileSystem.CurrentDirectory + "\Tools\wnbd-client.exe"
            WNBDClient.StartInfo.Arguments = "show " + NBDDriveName
            WNBDClient.StartInfo.RedirectStandardOutput = True
            WNBDClient.StartInfo.UseShellExecute = False
            WNBDClient.StartInfo.CreateNoWindow = True
            WNBDClient.Start()

            Dim OutputReader As StreamReader = WNBDClient.StandardOutput
            Dim ProcessOutput As String() = OutputReader.ReadToEnd().Split({vbCrLf}, StringSplitOptions.None)

            For Each ReturnedLine As String In ProcessOutput
                If ReturnedLine.Contains("Hostname") Then
                    Return ReturnedLine.Split(":"c)(1).Trim()
                    Exit Function
                End If
            Next
        End Using
    End Function

    Private Function GetHDLDriveName() As String
        'Query the drives
        Using HDLDump As New Process()
            HDLDump.StartInfo.FileName = My.Computer.FileSystem.CurrentDirectory + "\Tools\hdl_dump.exe"
            HDLDump.StartInfo.Arguments = "query"
            HDLDump.StartInfo.RedirectStandardOutput = True
            HDLDump.StartInfo.UseShellExecute = False
            HDLDump.StartInfo.CreateNoWindow = True
            HDLDump.Start()

            'Read the output
            Dim OutputReader As StreamReader = HDLDump.StandardOutput
            Dim ProcessOutput As String() = OutputReader.ReadToEnd().Split({vbCrLf}, StringSplitOptions.None)

            'Find the drive
            For Each Line As String In ProcessOutput
                If Not String.IsNullOrWhiteSpace(Line) Then
                    If Line.Contains("formatted Playstation 2 HDD") Then
                        'Set the found drive as mounted PSX drive
                        Dim DriveInfos As String() = Line.Split(New String() {" "}, StringSplitOptions.RemoveEmptyEntries)
                        If DriveInfos(0) IsNot Nothing Then
                            MountStatusLabel.Content = "on " + DriveInfos(0).Trim()
                            MountStatusLabel.Foreground = Brushes.Green
                            Return DriveInfos(0).Trim()
                        End If
                    End If
                End If
            Next
        End Using
    End Function

    Private Function GetHDDID() As String
        'Query the drives
        Using WMIC As New Process()
            WMIC.StartInfo.FileName = "wmic"
            WMIC.StartInfo.Arguments = "diskdrive get Caption,DeviceID"
            WMIC.StartInfo.RedirectStandardOutput = True
            WMIC.StartInfo.UseShellExecute = False
            WMIC.StartInfo.CreateNoWindow = True
            WMIC.Start()

            'Read the output
            Dim OutputReader As StreamReader = WMIC.StandardOutput
            Dim ProcessOutput As String() = OutputReader.ReadToEnd().Split({vbCrLf}, StringSplitOptions.None)

            'Find the drive
            For Each Line As String In ProcessOutput
                If Not String.IsNullOrWhiteSpace(Line) Then
                    If Line.Contains("WNBD WNBD_DISK SCSI Disk Device") Then
                        Return Line.Split(New String() {" "}, StringSplitOptions.RemoveEmptyEntries)(5).Trim()
                    ElseIf Line.Contains("Microsoft Virtual Disk") Then 'For testing with local VHD
                        Return Line.Split(New String() {" "}, StringSplitOptions.RemoveEmptyEntries)(3).Trim()
                    End If
                End If
            Next
        End Using
    End Function

#End Region

#Region "Structures & Enums"

    Public Structure MountedPSXDrive
        Private _HDLDriveName As String
        Private _NBDDriveName As String
        Private _DriveID As String

        Public Property DriveID As String
            Get
                Return _DriveID
            End Get
            Set
                _DriveID = Value
            End Set
        End Property

        Public Property HDLDriveName As String
            Get
                Return _HDLDriveName
            End Get
            Set
                _HDLDriveName = Value
            End Set
        End Property

        Public Property NBDDriveName As String
            Get
                Return _NBDDriveName
            End Get
            Set
                _NBDDriveName = Value
            End Set
        End Property
    End Structure

    Public Structure HDL_Dump_Args
        Private _Args As String()
        Private _Command As String

        Public Property Command As String
            Get
                Return _Command
            End Get
            Set
                _Command = Value
            End Set
        End Property

        Public Property Args As String()
            Get
                Return _Args
            End Get
            Set
                _Args = Value
            End Set
        End Property
    End Structure

    Public Enum DiscType
        CD
        DVD
    End Enum

    Private Function GetDiscType(ISOFile As String) As DiscType
        Dim ISOFileSize As Double = CDbl(New FileInfo(ISOFile).Length / 1048576)

        If ISOFileSize > 700 Then
            Return DiscType.DVD
        Else
            Return DiscType.CD
        End If
    End Function

#End Region

#Region "Install Subs"

    Private Sub InstallApp()
        'Check if drive is already identified, if not get the drive name
        If String.IsNullOrEmpty(MountedDrive.HDLDriveName) Then
            MountedDrive.HDLDriveName = GetHDLDriveName()
            'Retry
            InstallApp()
        Else
            'Proceed to installation on HDD
            Dim HomebrewTitle As String = File.ReadAllLines(PreparedProjectsComboBox.Text)(0).Split("="c)(1)
            Dim HomebrewELF As String = File.ReadAllLines(PreparedProjectsComboBox.Text)(3).Split("="c)(1)
            Dim HomebrewPartition As String

            If HomebrewTitle.Contains("Open PS2 Loader") Or HomebrewTitle.Contains("OPL") Then
                HomebrewPartition = "PP.APPS-00001..OPL"
            ElseIf HomebrewTitle.Contains("LaunchELF") Or HomebrewTitle.Contains("uLE") Or HomebrewTitle.Contains("wLE") Then
                HomebrewPartition = "PP.APPS-00002..WLE"
            ElseIf HomebrewTitle.Contains("hdl_srv") Or HomebrewTitle.Contains("hdl_server") Or HomebrewTitle.Contains("hdl server") Then
                HomebrewPartition = "PP.APPS-00003..HDL"
            ElseIf HomebrewTitle.Contains("SMS") Or HomebrewTitle.Contains("Simple Media System") Then
                HomebrewPartition = "PP.APPS-00004..SMS"
            Else
                HomebrewPartition = InputBox("Please enter a valid partition name:", "Could not determine partition for this homebrew.", "PP.APP-000.01..APP")
            End If

            If StatusTextBlock.Dispatcher.CheckAccess() = False Then
                StatusTextBlock.Dispatcher.BeginInvoke(Sub() StatusTextBlock.Text = "Creating partition, please wait...")
            Else
                StatusTextBlock.Text = "Creating partition, please wait..."
            End If

            If Not String.IsNullOrEmpty(HomebrewPartition) Then
                LockUI()
                CreateHomebrewPartition(HomebrewPartition)
            Else
                MsgBox("Partition name cannot be empty! Please try again.", MsgBoxStyle.Exclamation, "Error")
                Exit Sub
            End If

        End If
    End Sub

    Private Sub InstallGame()
        'Check if drive is already identified, if not get the drive name
        If MountedDrive.HDLDriveName = "" Then
            MountedDrive.HDLDriveName = GetHDLDriveName()
            'Retry
            InstallGame()
        Else
            'Proceed to installation on HDD
            Dim GameTitle As String = File.ReadAllLines(PreparedProjectsComboBox.Text)(0).Split("="c)(1)
            Dim GameID As String = File.ReadAllLines(PreparedProjectsComboBox.Text)(1).Split("="c)(1)
            Dim GameISO As String = File.ReadAllLines(PreparedProjectsComboBox.Text)(3).Split("="c)(1)

            'Check if CD or DVD
            If GetDiscType(GameISO) = DiscType.DVD Then
                LockUI() 'Disable UI controls
                HDL_DumpWorker.RunWorkerAsync(New HDL_Dump_Args() With {.Command = "inject_dvd", .Args = {GameTitle, GameISO, GameID}})
            Else
                MsgBox("Not supported yet.", MsgBoxStyle.Exclamation)
                Exit Sub
            End If
        End If
    End Sub

    Private Sub HDL_DumpWorker_DoWork(sender As Object, e As DoWorkEventArgs) Handles HDL_DumpWorker.DoWork
        Dim HDLWorker As BackgroundWorker = TryCast(sender, BackgroundWorker)
        Dim HDLArgs As HDL_Dump_Args = CType(e.Argument, HDL_Dump_Args)

        If HDLArgs.Command = "inject_dvd" Then
            Using HDLDump As New Process()
                HDLDump.StartInfo.FileName = My.Computer.FileSystem.CurrentDirectory + "\Tools\hdl_dump.exe"
                HDLDump.StartInfo.Arguments = "inject_dvd " + MountedDrive.HDLDriveName + " """ + HDLArgs.Args(0) + """ """ + HDLArgs.Args(1) + """ """ + HDLArgs.Args(2) + """ *u4 -hide"
                HDLDump.StartInfo.RedirectStandardOutput = True
                HDLDump.StartInfo.UseShellExecute = False
                HDLDump.StartInfo.CreateNoWindow = True
                HDLDump.Start()

                While Not HDLDump.HasExited
                    Dim output = HDLDump.StandardOutput.ReadLine()
                    HDLWorker.ReportProgress(0, output)
                End While
            End Using
        End If
    End Sub

    Private Sub HDL_DumpWorker_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles HDL_DumpWorker.RunWorkerCompleted
        'Proceed to game partition
        If StatusTextBlock.Dispatcher.CheckAccess() = False Then
            StatusTextBlock.Dispatcher.BeginInvoke(Sub() StatusTextBlock.Text = "Creating game PP partition ...")
        Else
            StatusTextBlock.Text = "Creating game PP partition ..."
        End If

        CreateGamePartition()
    End Sub

    Private Sub HDL_DumpWorker_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles HDL_DumpWorker.ProgressChanged
        If StatusTextBlock.Dispatcher.CheckAccess() = False Then
            StatusTextBlock.Dispatcher.BeginInvoke(Sub() StatusTextBlock.Text = CStr(e.UserState))
        Else
            StatusTextBlock.Text = CStr(e.UserState)
        End If
    End Sub

    Private Sub CreateGamePartition()

        Dim GameID As String = File.ReadAllLines(PreparedProjectsComboBox.Text)(1).Split("="c)(1).Replace("_", "-").Replace(".", "").Trim()
        Dim ProjectDirectory As String = File.ReadAllLines(PreparedProjectsComboBox.Text)(2).Split("="c)(1)
        Dim CreatedGamePartition As String = ""

        'Get the created partition
        Dim QueryOutput As String()
        Using HDLDump As New Process()
            HDLDump.StartInfo.FileName = My.Computer.FileSystem.CurrentDirectory + "\Tools\hdl_dump.exe"
            HDLDump.StartInfo.Arguments = "toc " + MountedDrive.HDLDriveName
            HDLDump.StartInfo.RedirectStandardOutput = True
            HDLDump.StartInfo.UseShellExecute = False
            HDLDump.StartInfo.CreateNoWindow = True
            HDLDump.Start()

            Dim OutputReader As StreamReader = HDLDump.StandardOutput
            QueryOutput = OutputReader.ReadToEnd().Split({vbCrLf}, StringSplitOptions.None)
        End Using

        For Each HDDPartition As String In QueryOutput
            If HDDPartition.Split(New String() {" "}, StringSplitOptions.RemoveEmptyEntries).Count > 3 Then
                HDDPartition = HDDPartition.Split(New String() {" "}, StringSplitOptions.RemoveEmptyEntries)(4)
            End If

            If HDDPartition.Trim().StartsWith("__." + GameID) Then 'The created hidden partition
                CreatedGamePartition = HDDPartition.Trim()
                Exit For
            End If
        Next

        'Set mkpart command
        Dim CommandFile As String = My.Computer.FileSystem.CurrentDirectory + "\Tools\cmdlist\mkpart.txt"
        Dim CommandFileLines() As String = File.ReadAllLines(CommandFile)

        CommandFileLines(0) = "device " + MountedDrive.DriveID
        CommandFileLines(1) = "mkpart " + CreatedGamePartition.Replace("__", "PP") + " 128M PFS"
        File.WriteAllLines(CommandFile, CommandFileLines)

        'Proceed to partition creation
        Dim PFSShellOutput As String
        Using PFSShellProcess As New Process()
            PFSShellProcess.StartInfo.FileName = "cmd"
            PFSShellProcess.StartInfo.Arguments = """/c type """ + My.Computer.FileSystem.CurrentDirectory + "\Tools\cmdlist\mkpart.txt"" | """ + My.Computer.FileSystem.CurrentDirectory + "\Tools\pfsshell.exe"" 2>&1"

            PFSShellProcess.StartInfo.RedirectStandardOutput = True
            PFSShellProcess.StartInfo.UseShellExecute = False

            PFSShellProcess.Start()

            Dim ShellReader As StreamReader = PFSShellProcess.StandardOutput
            Dim ProcessOutput As String = ShellReader.ReadToEnd()

            ShellReader.Close()
            PFSShellOutput = ProcessOutput
        End Using

        If PFSShellOutput.Contains("Main partition of 128M created.") Then

            If StatusTextBlock.Dispatcher.CheckAccess() = False Then
                StatusTextBlock.Dispatcher.BeginInvoke(Sub() StatusTextBlock.Text = "Partition created, modifying header...")
            Else
                StatusTextBlock.Text = "Game partition created, modifying header..."
            End If

            'Modify the created partition
            ModifyPartitionHeader(CreatedGamePartition.Replace("__", "PP"))
        Else
            MsgBox("There was an error in creating the game's PP partition, please check if the name doesn't already exists of if you have enough space.", MsgBoxStyle.Exclamation, "Error installing game")
            Exit Sub
        End If

    End Sub

    Private Sub CreateHomebrewPartition(PartitionName As String)

        Dim ProjectDirectory As String = File.ReadAllLines(PreparedProjectsComboBox.Text)(2).Split("="c)(1)

        'Set mkpart command
        Dim CommandFile As String = My.Computer.FileSystem.CurrentDirectory + "\Tools\cmdlist\mkpart.txt"
        Dim CommandFileLines() As String = File.ReadAllLines(CommandFile)

        CommandFileLines(0) = "device " + MountedDrive.DriveID
        CommandFileLines(1) = "mkpart " + PartitionName + " 128M PFS"
        File.WriteAllLines(CommandFile, CommandFileLines)

        'Proceed to partition creation
        Dim PFSShellOutput As String
        Using PFSShellProcess As New Process()
            PFSShellProcess.StartInfo.FileName = "cmd"
            PFSShellProcess.StartInfo.Arguments = """/c type """ + My.Computer.FileSystem.CurrentDirectory + "\Tools\cmdlist\mkpart.txt"" | """ + My.Computer.FileSystem.CurrentDirectory + "\Tools\pfsshell.exe"" 2>&1"

            PFSShellProcess.StartInfo.RedirectStandardOutput = True
            PFSShellProcess.StartInfo.UseShellExecute = False

            PFSShellProcess.Start()

            Dim ShellReader As StreamReader = PFSShellProcess.StandardOutput
            Dim ProcessOutput As String = ShellReader.ReadToEnd()

            ShellReader.Close()
            PFSShellOutput = ProcessOutput
        End Using

        If PFSShellOutput.Contains("Main partition of 128M created.") Then

            If StatusTextBlock.Dispatcher.CheckAccess() = False Then
                StatusTextBlock.Dispatcher.BeginInvoke(Sub() StatusTextBlock.Text = "Partition created, modifying header...")
            Else
                StatusTextBlock.Text = "Partition created, modifying header..."
            End If

            'Modify the created partition
            ModifyPartitionHeader(PartitionName)
        Else
            MsgBox("There was an error in creating the homebrew's PP partition, please check if the name doesn't already exists of if you have enough space.", MsgBoxStyle.Exclamation, "Error installing homebrew")
            Exit Sub
        End If

    End Sub

    Private Sub ModifyPartitionHeader(PartitionName As String)

        Dim ProjectDirectory As String = File.ReadAllLines(PreparedProjectsComboBox.Text)(2).Split("="c)(1)

        'Create a copy of hdl_dump in the project directory
        File.Copy(My.Computer.FileSystem.CurrentDirectory + "\Tools\hdl_dump.exe", ProjectDirectory + "\hdl_dump.exe", True)

        'Switch to project directory and inject the files
        Directory.SetCurrentDirectory(ProjectDirectory)

        Using HDLDump As New Process()
            HDLDump.StartInfo.FileName = "hdl_dump.exe"
            HDLDump.StartInfo.Arguments = "modify_header " + MountedDrive.HDLDriveName + " " + PartitionName
            HDLDump.StartInfo.RedirectStandardOutput = True
            HDLDump.StartInfo.UseShellExecute = False
            HDLDump.StartInfo.CreateNoWindow = False
            HDLDump.Start()

            Dim OutputReader As StreamReader = HDLDump.StandardOutput
            Dim output = HDLDump.StandardOutput.ReadToEnd()

            If Not output.Contains("partition not found:") Then

                If StatusTextBlock.Dispatcher.CheckAccess() = False Then
                    StatusTextBlock.Dispatcher.BeginInvoke(Sub() StatusTextBlock.Text = "Partition header modified, adding files...")
                Else
                    StatusTextBlock.Text = "Partition header modified, adding files..."
                End If

                AddFilesToPartition(PartitionName)
            Else
                MsgBox("There was an error while modifying the partition, please check if you have enough space and report the next error.", MsgBoxStyle.Exclamation, "Error installing game")
                MsgBox(output)
                'Set the current directory back
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory)
                Exit Sub
            End If
        End Using
    End Sub

    Private Sub AddFilesToPartition(PartitionName As String)
        'Now put the "res" folder and EXECUTE.KELF file into the partition
        Dim PFSShellOutput As String

        'Set the mkdir & put commands
        Using CommandFileWriter As New StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Tools\cmdlist\push.txt", False)
            CommandFileWriter.WriteLine("device " + MountedDrive.DriveID)
            CommandFileWriter.WriteLine("mount " + PartitionName)
            CommandFileWriter.WriteLine("put EXECUTE.KELF")
            CommandFileWriter.WriteLine("mkdir res")
            CommandFileWriter.WriteLine("cd res")

            If File.Exists("res\info.sys") Then
                CommandFileWriter.WriteLine("put res\info.sys")
                CommandFileWriter.WriteLine("rename res\info.sys info.sys")
            End If
            If File.Exists("res\jkt_001.png") Then
                CommandFileWriter.WriteLine("put res\jkt_001.png")
                CommandFileWriter.WriteLine("rename res\jkt_001.png jkt_001.png")
            End If
            If File.Exists("res\jkt_002.png") Then
                CommandFileWriter.WriteLine("put res\jkt_002.png")
                CommandFileWriter.WriteLine("rename res\jkt_002.png jkt_002.png")
            End If
            If File.Exists("res\jkt_cp.png") Then
                CommandFileWriter.WriteLine("put res\jkt_cp.png")
                CommandFileWriter.WriteLine("rename res\jkt_cp.png jkt_cp.png")
            End If
            If File.Exists("res\man.xml") Then
                CommandFileWriter.WriteLine("put res\man.xml")
                CommandFileWriter.WriteLine("rename res\man.xml man.xml")
            End If
            If File.Exists("res\notice.jpg") Then
                CommandFileWriter.WriteLine("put res\notice.jpg")
                CommandFileWriter.WriteLine("rename res\notice.jpg notice.jpg")
            End If

            If Directory.Exists("res\image") Then
                CommandFileWriter.WriteLine("mkdir image")
                CommandFileWriter.WriteLine("cd image")

                If File.Exists("res\image\0.png") Then
                    CommandFileWriter.WriteLine("put res\image\0.png")
                    CommandFileWriter.WriteLine("rename res\image\0.png 0.png")
                End If
                If File.Exists("res\image\1.png") Then
                    CommandFileWriter.WriteLine("put res\image\1.png")
                    CommandFileWriter.WriteLine("rename res\image\1.png 1.png")
                End If
                If File.Exists("res\image\2.png") Then
                    CommandFileWriter.WriteLine("put res\image\2.png")
                    CommandFileWriter.WriteLine("rename res\image\2.png 2.png")
                End If
            End If

            CommandFileWriter.WriteLine("umount")
            CommandFileWriter.WriteLine("exit")
        End Using

        'Put all detected files to the partition
        Using PFSShellProcess As New Process()
            PFSShellProcess.StartInfo.FileName = "cmd"
            PFSShellProcess.StartInfo.Arguments = """/c type """ + AppDomain.CurrentDomain.BaseDirectory + "Tools\cmdlist\push.txt"" | """ + AppDomain.CurrentDomain.BaseDirectory + "Tools\pfsshell.exe"" 2>&1"
            PFSShellProcess.StartInfo.RedirectStandardOutput = True
            PFSShellProcess.StartInfo.UseShellExecute = False

            PFSShellProcess.Start()

            Dim ShellReader As StreamReader = PFSShellProcess.StandardOutput
            Dim ProcessOutput As String = ShellReader.ReadToEnd()

            ShellReader.Close()
            PFSShellOutput = ProcessOutput
        End Using

        LockUI()

        If StatusTextBlock.Dispatcher.CheckAccess() = False Then
            StatusTextBlock.Dispatcher.BeginInvoke(Sub() StatusTextBlock.Text = "Installed !")
        Else
            StatusTextBlock.Text = "Installed !"
        End If

        'Set the current directory back
        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory)

    End Sub

#End Region

#Region "Menu Items"

    Private Sub NewGameProjectMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles NewGameProjectMenuItem.Click
        Dim NewXMBGameProject As New XMBGameProject() With {.ShowActivated = True}
        NewXMBGameProject.Show()
    End Sub

    Private Sub NewHomebrewProjectMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles NewHomebrewProjectMenuItem.Click
        Dim NewXMBAppProject As New XMBAppProject() With {.ShowActivated = True}
        NewXMBAppProject.Show()
    End Sub

    Private Sub OpenNBDDriverPage_Click(sender As Object, e As RoutedEventArgs) Handles OpenNBDDriverPage.Click
        Process.Start("https://cloudbase.it/ceph-for-windows/")
    End Sub

#End Region

#Region "Button Actions"

    Private Sub EditButton_Click(sender As Object, e As RoutedEventArgs) Handles EditButton.Click
        If ProjectListComboBox.SelectedItem IsNot Nothing Then
            'Get project infos
            Dim ProjectInfos As String() = File.ReadAllLines(ProjectListComboBox.Text)
            Dim ProjectName As String = ProjectInfos(0).Split("="c)(1)
            Dim ProjectSubtitle As String = ProjectInfos(1).Split("="c)(1)
            Dim ProjectDirectory As String = ProjectInfos(2).Split("="c)(1)
            Dim ProjectFile As String = ProjectInfos(3).Split("="c)(1)
            Dim ProjectType As String = ProjectInfos(4).Split("="c)(1)

            If ProjectType = "APP" Then
                Dim HomebrewInfos As String() = File.ReadAllLines(ProjectDirectory + "\icon.sys")
                Dim HomebrewProjectEditor As New XMBAppProject() With {.Title = "Editing project " + ProjectName + " - " + ProjectDirectory}

                HomebrewProjectEditor.ProjectNameTextBox.Text = ProjectName
                HomebrewProjectEditor.ProjectDirectoryTextBox.Text = ProjectDirectory
                HomebrewProjectEditor.ProjectTitleTextBox.Text = HomebrewInfos(1).Split("="c)(1)
                HomebrewProjectEditor.ProjectSubTitleTextBox.Text = ProjectSubtitle
                HomebrewProjectEditor.ProjectSubTitleTextBox.Text = HomebrewInfos(2).Split("="c)(1)
                HomebrewProjectEditor.ProjectUninstallMsgTextBox.Text = HomebrewInfos(15).Split("="c)(1)
                HomebrewProjectEditor.ProjectELFFileTextBox.Text = ProjectFile

                If File.Exists(ProjectDirectory + "\list.ico") Then
                    HomebrewProjectEditor.ProjectIconPathTextBox.Text = ProjectDirectory + "\list.ico"
                End If

                HomebrewProjectEditor.Show()
            ElseIf ProjectType = "GAME" Then
                Dim GameInfos As String() = File.ReadAllLines(ProjectDirectory + "\icon.sys")
                Dim GameProjectEditor As New XMBGameProject() With {.Title = "Editing project " + ProjectName + " - " + ProjectDirectory}

                GameProjectEditor.ProjectNameTextBox.Text = ProjectName
                GameProjectEditor.ProjectDirectoryTextBox.Text = ProjectDirectory
                GameProjectEditor.ProjectTitleTextBox.Text = GameInfos(1).Split("="c)(1)
                GameProjectEditor.ProjectIDTextBox.Text = ProjectSubtitle
                GameProjectEditor.ProjectSubTitleTextBox.Text = GameInfos(2).Split("="c)(1)
                GameProjectEditor.ProjectUninstallMsgTextBox.Text = GameInfos(15).Split("="c)(1)
                GameProjectEditor.ProjectISOFileTextBox.Text = ProjectFile

                If File.Exists(ProjectDirectory + "\list.ico") Then
                    GameProjectEditor.ProjectIconPathTextBox.Text = ProjectDirectory + "\list.ico"
                End If

                GameProjectEditor.Show()
            End If
        End If
    End Sub

    Private Sub PrepareButton_Click(sender As Object, e As RoutedEventArgs) Handles PrepareButton.Click
        If ProjectListComboBox.SelectedItem IsNot Nothing Then
            Dim ProjectDIR As String = File.ReadAllLines(ProjectListComboBox.Text)(2).Split("="c)(1)

            'Check if KELF already exists
            If File.Exists(ProjectDIR + "\EXECUTE.KELF") Or File.Exists(ProjectDIR + "\boot.elf") Or File.Exists(ProjectDIR + "\boot.kelf") Then
                MsgBox("Your Project doesn't need to be prepared again.", MsgBoxStyle.Information)
            Else
                Dim ProjectELForISO As String = File.ReadAllLines(ProjectListComboBox.Text)(3).Split("="c)(1)
                Dim ProjectTYPE As String = File.ReadAllLines(ProjectListComboBox.Text)(4).Split("="c)(1)

                If ProjectTYPE = "APP" Then
                    'Wrap the application ELF as EXECUTE.KELF
                    Dim WrapProcess As New Process()
                    WrapProcess.StartInfo.FileName = My.Computer.FileSystem.CurrentDirectory + "\Tools\SCEDoormat_NoME.exe"
                    WrapProcess.StartInfo.Arguments = """" + ProjectELForISO + """ " + ProjectDIR + "\EXECUTE.KELF"
                    WrapProcess.Start()
                    WrapProcess.WaitForExit()

                    Dim ProjectConfigFileLines() As String = File.ReadAllLines(ProjectListComboBox.Text)
                    ProjectConfigFileLines(5) = "SIGNED=TRUE"
                    File.WriteAllLines(ProjectListComboBox.Text, ProjectConfigFileLines)

                    MsgBox("Homebrew Project prepared with success !" + vbCrLf + "You can now proceed with the installation on the PSX.", MsgBoxStyle.Information, "Success")
                Else
                    If File.Exists(My.Computer.FileSystem.CurrentDirectory + "\Tools\EXECUTE.KELF") Then
                        'Copy included OPL-Launcher to project folder
                        File.Copy(My.Computer.FileSystem.CurrentDirectory + "\Tools\EXECUTE.KELF", ProjectDIR + "\EXECUTE.KELF", True)
                    Else
                        'OPL-Launcher not found...
                        Dim HomebrewELF As String = ""

                        HomebrewELF = InputBox("OPL-Launcher has been deleted from the Tools folder." + vbCrLf + "Please enter the full path to the .elf file or leave the URL to download OPL-Launcher.",
                                               "Missing file",
                                               "https://github.com/ps2homebrew/OPL-Launcher/releases/download/latest/OPL-Launcher.elf")

                        If Not String.IsNullOrEmpty(HomebrewELF) Then
                            If HomebrewELF = "https://github.com/ps2homebrew/OPL-Launcher/releases/download/latest/OPL-Launcher.elf" Then
                                'Download latest OPL-Launcher
                                ContentDownloader.DownloadFile("https://github.com/ps2homebrew/OPL-Launcher/releases/download/latest/OPL-Launcher.elf", My.Computer.FileSystem.CurrentDirectory + "\Tools\OPL-Launcher.elf")
                            End If
                        Else
                            MsgBox("Not valid file provided, aborting ...", MsgBoxStyle.Exclamation, "Aborting")
                            Exit Sub
                        End If

                        'Wrap OPL-Launcher as EXECUTE.KELF
                        Dim SignProcess As New Process()
                        SignProcess.StartInfo.FileName = My.Computer.FileSystem.CurrentDirectory + "\Tools\SCEDoormat_NoME.exe"
                        SignProcess.StartInfo.Arguments = """" + My.Computer.FileSystem.CurrentDirectory + "\Tools\OPL-Launcher.elf"" """ + ProjectDIR + "\EXECUTE.KELF"""
                        SignProcess.Start()
                        SignProcess.WaitForExit()
                    End If

                    Dim ProjectConfigFileLines() As String = File.ReadAllLines(ProjectListComboBox.Text)
                    ProjectConfigFileLines(5) = "SIGNED=TRUE"
                    File.WriteAllLines(ProjectListComboBox.Text, ProjectConfigFileLines)

                    MsgBox("Game Project is now prepared !" + vbCrLf + "You can now proceed with the installation on the PSX.", MsgBoxStyle.Information, "Success")
                End If

            End If

            ReloadProjects()
        End If
    End Sub

    Private Sub ConnectButton_Click(sender As Object, e As RoutedEventArgs) Handles ConnectButton.Click
        If ConnectButton.Content.ToString = "Connect" Then
            Try
                Using WNBDClient As New Process()
                    If File.Exists(My.Computer.FileSystem.SpecialDirectories.ProgramFiles + "\Ceph\bin\wnbd-client.exe") Then
                        WNBDClient.StartInfo.FileName = My.Computer.FileSystem.SpecialDirectories.ProgramFiles + "\Ceph\bin\wnbd-client.exe"
                    Else
                        WNBDClient.StartInfo.FileName = My.Computer.FileSystem.CurrentDirectory + "\Tools\wnbd-client.exe"
                    End If
                    WNBDClient.StartInfo.Arguments = "map PSXHDD " + PSXIPTextBox.Text
                    WNBDClient.StartInfo.RedirectStandardOutput = True
                    WNBDClient.StartInfo.UseShellExecute = False
                    WNBDClient.StartInfo.CreateNoWindow = True
                    WNBDClient.Start()

                    Dim OutputReader As StreamReader = WNBDClient.StandardOutput
                    Dim ProcessOutput As String = OutputReader.ReadToEnd()

                    'libwnbd.dll!WnbdIoctlCreate ERROR Could not create WNBD disk. - Probably NBD driver not correctly installed or just entered the wrong IP
                    If ProcessOutput.Trim().Contains("libwnbd.dll!WnbdIoctlCreate") Then
                        MsgBox("Could not map your PSX HDD, please check if your NBD server is running and set up correctly." + vbCrLf + "Also check if you entered the correct IP address.")
                        WNBDClient.Close()
                        Exit Sub
                    End If

                    WNBDClient.WaitForExit()

                    MsgBox("Your PSX HDD is now connected." + vbCrLf + "You can now install your project on the PSX.")

                    MountedDrive.DriveID = GetHDDID()
                    InstallButton.IsEnabled = True
                    NBDConnectionLabel.Content = "Connected"
                    NBDConnectionLabel.Foreground = Brushes.Green
                    ConnectButton.Content = "Disconnect"
                End Using
            Catch AnyException As Exception
                MsgBox(AnyException.Message)
            End Try
        Else
            Try
                Dim WNBDProcess As New Process()
                If File.Exists(My.Computer.FileSystem.SpecialDirectories.ProgramFiles + "\Ceph\bin\wnbd-client.exe") Then
                    WNBDProcess.StartInfo.FileName = My.Computer.FileSystem.SpecialDirectories.ProgramFiles + "\Ceph\bin\wnbd-client.exe"
                Else
                    WNBDProcess.StartInfo.FileName = My.Computer.FileSystem.CurrentDirectory + "\Tools\wnbd-client.exe"
                End If
                WNBDProcess.StartInfo.Arguments = "unmap PSXHDD"
                WNBDProcess.Start()
                WNBDProcess.WaitForExit()

                MsgBox("Your PSX HDD is now disconnected." + vbCrLf + "You can now safely close the NBD server.", MsgBoxStyle.Information)

                InstallButton.IsEnabled = True
                NBDConnectionLabel.Content = "Disconnected"
                NBDConnectionLabel.Foreground = Brushes.OrangeRed

                ConnectButton.Content = "Connect"

            Catch AnyException As Exception
                MsgBox(AnyException.Message)
            End Try
        End If
    End Sub

    Private Sub InstallButton_Click(sender As Object, e As RoutedEventArgs) Handles InstallButton.Click
        If PreparedProjectsComboBox.SelectedItem IsNot Nothing Then
            Dim ProjectTitle As String = File.ReadAllLines(PreparedProjectsComboBox.Text)(0).Split("="c)(1)
            If MsgBox("Do you really want to install " + ProjectTitle + " on your PSX ?", MsgBoxStyle.YesNo, "Please confirm") = MsgBoxResult.Yes Then
                'Identify project type
                Dim ProjectType As String = File.ReadAllLines(PreparedProjectsComboBox.Text)(4).Split("="c)(1)

                If ProjectType = "APP" Then
                    StatusTextBlock.Text = "Installing Homebrew, please wait..."
                    InstallApp()
                ElseIf ProjectType = "GAME" Then
                    StatusTextBlock.Text = "Installing Game, please wait..."
                    InstallGame()
                End If
            Else
                MsgBox("Installation aborted.", MsgBoxStyle.OkOnly, "Aborted")
            End If
        Else
            MsgBox("Please select a project first.", MsgBoxStyle.Exclamation, "No project selected")
        End If
    End Sub

#End Region

End Class
