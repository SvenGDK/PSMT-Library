﻿Imports System.IO
Imports System.Security.Authentication
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports FluentFTP

Public Class PS5Menu

    Public SharedConsoleAddress As String = ""
    Public Shared ReadOnly IPChangedEvent As RoutedEvent = EventManager.RegisterRoutedEvent(name:="ConsoleAddressChanged", routingStrategy:=RoutingStrategy.Bubble, handlerType:=GetType(RoutedEventHandler), ownerType:=GetType(PS5Menu))

    Public Custom Event IPTextChanged As RoutedEventHandler
        AddHandler(value As RoutedEventHandler)
            [AddHandler](IPChangedEvent, value)
        End AddHandler

        RemoveHandler(value As RoutedEventHandler)
            [RemoveHandler](IPChangedEvent, value)
        End RemoveHandler

        RaiseEvent(sender As Object, e As RoutedEventArgs)
            [RaiseEvent](e)
        End RaiseEvent
    End Event

    Private Sub RaiseIPTextChangedRoutedEvent()
        Dim routedEventArgs As New RoutedEventArgs(routedEvent:=IPChangedEvent)
        [RaiseEvent](routedEventArgs)
    End Sub

    Private Sub FTPIPTextBox_TextChanged(sender As Object, e As TextChangedEventArgs) Handles FTPIPTextBox.TextChanged
        If Not String.IsNullOrEmpty(FTPIPTextBox.Text) And FTPIPTextBox.Text.Contains(":"c) Then
            SharedConsoleAddress = FTPIPTextBox.Text
            RaiseIPTextChangedRoutedEvent()
        End If
    End Sub

#Region "Tools"

    Private Sub SenderMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles SenderMenuItem.Click
        Dim NewPS5Sender As New PS5Sender() With {.ShowActivated = True}

        'Set values if SharedConsoleAddress is set
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            NewPS5Sender.IPTextBox.Text = SharedConsoleAddress.Split(":"c)(0)
            NewPS5Sender.PortTextBox.Text = SharedConsoleAddress.Split(":"c)(1)
        End If

        NewPS5Sender.Show()
    End Sub

    Private Sub OpenFTPBrowserMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles OpenFTPBrowserMenuItem.Click
        Dim NewFTPBrowser As New FTPBrowser() With {.ShowActivated = True, .FTPS5Mode = True}

        'Set values if SharedConsoleAddress is set
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            NewFTPBrowser.ConsoleIPTextBox.Text = SharedConsoleAddress.Split(":"c)(0)
            NewFTPBrowser.PortTextBox.Text = SharedConsoleAddress.Split(":"c)(1)
        End If

        NewFTPBrowser.Show()
    End Sub

    Private Sub OpenBDBurnerMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles OpenBDBurnerMenuItem.Click
        Dim NewBDBurner As New BDBurner() With {.ShowActivated = True}
        NewBDBurner.Show()
    End Sub

    Private Sub OpenWebBrowserInstallerMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles OpenWebBrowserInstallerMenuItem.Click
        Dim NewPS5WebBrowserAdder As New PS5WebBrowserAdder() With {.ShowActivated = True}

        'Set values if SharedConsoleAddress is set
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            NewPS5WebBrowserAdder.ConsoleIP = SharedConsoleAddress.Split(":"c)(0)
        End If

        NewPS5WebBrowserAdder.Show()
    End Sub

    Private Sub OpenNotificationManagerMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles OpenNotificationManagerMenuItem.Click
        Dim NewPS5NotificationsManager As New PS5Notifications() With {.ShowActivated = True}

        'Set values if SharedConsoleAddress is set
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            NewPS5NotificationsManager.IPTextBox.Text = SharedConsoleAddress.Split(":"c)(0)
            NewPS5NotificationsManager.PortTextBox.Text = SharedConsoleAddress.Split(":"c)(1)
        End If

        NewPS5NotificationsManager.Show()
    End Sub

    Private Sub ClearErrorHistoryMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles ClearErrorHistoryMenuItem.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then

            Cursor = Cursors.Wait

            Try
                Using conn As New FtpClient(SharedConsoleAddress.Split(":"c)(0), "anonymous", "anonymous", 1337)

                    'Configurate the FTP connection
                    conn.Config.EncryptionMode = FtpEncryptionMode.None
                    conn.Config.SslProtocols = SslProtocols.None
                    conn.Config.DataConnectionEncryption = False

                    'Connect
                    conn.Connect()

                    Dim ErrorFiles As New List(Of FtpListItem)()

                    'List disc directory
                    For Each item In conn.GetListing("/system_data/priv/error/history")
                        Select Case item.Type
                            Case FtpObjectType.File
                                ErrorFiles.Add(item)
                        End Select
                    Next

                    For Each FTPFile In ErrorFiles
                        'Delete the error file
                        conn.DeleteFile(FTPFile.FullName)
                    Next

                    'Disonnect
                    conn.Disconnect()
                End Using

                MsgBox("The error history on the console has been deleted." + vbCrLf + "Check your Settings->System Software->Error History on the PS5.", MsgBoxStyle.Information)
            Catch ex As Exception
                MsgBox("Could delete the error history, please verify your connection.", MsgBoxStyle.Exclamation)
            End Try

            Cursor = Cursors.Arrow
        Else
            MsgBox("Please set your IP:Port in the settings first.", MsgBoxStyle.Information, "Cannot connect to the PS5")
        End If
    End Sub

    Private Sub OpenGP5ManagerMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles OpenGP5ManagerMenuItem.Click
        Dim NewGP5Creator As New GP5Creator() With {.ShowActivated = True}
        NewGP5Creator.Show()
    End Sub

    Private Sub OpenRCODumperMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles OpenRCODumperMenuItem.Click
        Dim NewPS5RcoDumper As New PS5RcoDumper() With {.ShowActivated = True}

        'Set values if SharedConsoleAddress is set
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            NewPS5RcoDumper.ConsoleIP = SharedConsoleAddress.Split(":"c)(0)
        End If

        NewPS5RcoDumper.Show()
    End Sub

    Private Sub OpenRCOExtractorMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles OpenRCOExtractorMenuItem.Click
        Dim NewPS5RcoExtractor As New PS5RcoExtractor() With {.ShowActivated = True}
        NewPS5RcoExtractor.Show()
    End Sub

    Private Sub OpenParamEditorMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles OpenParamEditorMenuItem.Click
        Dim NewParamEditor As New PS5ParamEditor() With {.ShowActivated = True}
        NewParamEditor.Show()
    End Sub

    Private Sub OpenPKGBuilderMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles OpenPKGBuilderMenuItem.Click

        Dim NewPKGBuilder As New PS5PKGBuilder()

        If File.Exists(My.Computer.FileSystem.SpecialDirectories.ProgramFiles + "\XXX\Pros\Tools\Publishing Tools\bin\pros-pub-cmd.exe") Then
            NewPKGBuilder.PubToolsPath = My.Computer.FileSystem.SpecialDirectories.ProgramFiles + "\XXX\Pros\Tools\Publishing Tools\bin\pros-pub-cmd.exe"
        ElseIf File.Exists(My.Computer.FileSystem.CurrentDirectory + "\Tools\PS5\pros-pub-cmd.exe") Then
            NewPKGBuilder.PubToolsPath = My.Computer.FileSystem.CurrentDirectory + "\Tools\PS5\pros-pub-cmd.exe"
        Else
            NewPKGBuilder.IsEnabled = False
            MsgBox("Could not find any publishing tools." + vbCrLf + "Please add them inside the 'Tools\PS5' folder inside PS Multi Tools.", MsgBoxStyle.Information, "Pub Tools not available")
        End If

        NewPKGBuilder.Show()

    End Sub

    Private Sub OpenAudioConverterMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles OpenAudioConverterMenuItem.Click
        Dim NewAudioConverter As New PS5AT9Converter() With {.ShowActivated = True}

        If File.Exists(My.Computer.FileSystem.CurrentDirectory + "\Tools\PS5\at9tool.exe") Then
            NewAudioConverter.AT9Tool = My.Computer.FileSystem.CurrentDirectory + "\Tools\PS5\at9tool.exe"
        Else
            NewAudioConverter.IsEnabled = False
            MsgBox("Could not find the at9tool." + vbCrLf + "Please add it inside the 'Tools\PS5' folder inside PS Multi Tools.", MsgBoxStyle.Information, "at9tool not available")
        End If

        NewAudioConverter.Show()
    End Sub

    Private Sub OpenGamePatchesMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles OpenGamePatchesMenuItem.Click
        Dim NewPS5GamePatches As New PS5GamePatches() With {.ShowActivated = True}
        NewPS5GamePatches.Show()
    End Sub

    Private Sub OpenModPatchesMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles OpenModPatchesMenuItem.Click
        Dim NewPS5ModPatches As New PS5ModPatches() With {.ShowActivated = True}

        'Set values if SharedConsoleAddress is set
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            NewPS5ModPatches.ConsoleIP = SharedConsoleAddress.Split(":"c)(0)
        End If

        NewPS5ModPatches.Show()
    End Sub

    Private Sub OpenPKGMergerMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles OpenPKGMergerMenuItem.Click
        Dim NewPS5PKGMerger As New PS5PKGMerger() With {.ShowActivated = True}
        NewPS5PKGMerger.Show()
    End Sub

    Private Sub OpenFTPGrabberMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles OpenFTPGrabberMenuItem.Click
        Dim NewFTPGrabber As New PS5FTPGrabber() With {.ShowActivated = True}

        'Set values if SharedConsoleAddress is set
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            NewFTPGrabber.ConsoleIP = SharedConsoleAddress.Split(":"c)(0)
        End If

        NewFTPGrabber.Show()
    End Sub

    Private Sub OpenManifestEditorMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles OpenManifestEditorMenuItem.Click
        Dim NewManifestEditor As New PS5ManifestEditor() With {.ShowActivated = True}
        NewManifestEditor.Show()
    End Sub

#End Region

#Region "Menu Downloads"

#Region "Homebrew"

    Private Sub DownloadFTPS5_Click(sender As Object, e As RoutedEventArgs) Handles DownloadFTPS5.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/hb/ftps5.elf") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadPS5NetworkELFLoader650_Click(sender As Object, e As RoutedEventArgs) Handles DownloadPS5NetworkELFLoader650.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/hb/PS2NetworkELFLoader/VMC0-PS5-6-50.card") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadPS5NetworkGameLoader650_Click(sender As Object, e As RoutedEventArgs) Handles DownloadPS5NetworkGameLoader650.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/hb/PS2NetworkGameLoader/mast1c0re-ps2-network-game-loader-PS5-6-50.elf") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadInternetBrowserPKG_Click(sender As Object, e As RoutedEventArgs) Handles DownloadInternetBrowserPKG.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True, .PackageConsole = "PS5"}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/pkg/InternetBrowserPS5.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadStorePreviewPKG_Click(sender As Object, e As RoutedEventArgs) Handles DownloadStorePreviewPKG.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True, .PackageConsole = "PS5"}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/pkg/StorePreviewPS5.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadGameHubPreviewPKG_Click(sender As Object, e As RoutedEventArgs) Handles DownloadGameHubPreviewPKG.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True, .PackageConsole = "PS5"}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/pkg/GameHubPreviewPS5.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadDebugPKG_Click(sender As Object, e As RoutedEventArgs) Handles DownloadDebugPKG.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True, .PackageConsole = "PS5"}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/pkg/DebugSettingsPS5.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadPSMTPKG_Click(sender As Object, e As RoutedEventArgs) Handles DownloadPSMTPKG.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True, .PackageConsole = "PS5"}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/pkg/PSMultiToolsHost.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadKStuff_Click(sender As Object, e As RoutedEventArgs) Handles DownloadKStuff.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True, .PackageConsole = "PS5"}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/hb/ps5-kstuff.bin") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadetaHEN_Click(sender As Object, e As RoutedEventArgs) Handles DownloadetaHEN.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True, .PackageConsole = "PS5"}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/LightningMods/etaHEN/releases/download/1.1b/etaHEN.bin") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadetaHENwithCheats_Click(sender As Object, e As RoutedEventArgs) Handles DownloadetaHENwithCheats.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True, .PackageConsole = "PS5"}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/LightningMods/etaHEN/releases/download/1.1b/etaHENwithcheats.bin") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadHomebrewStore_Click(sender As Object, e As RoutedEventArgs) Handles DownloadHomebrewStore.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True, .PackageConsole = "PS5"}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/hb/Store-R2-PS5.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

#End Region

#Region "Firmwares"

    Private Sub DownloadRecoveryFW403_Click(sender As Object, e As RoutedEventArgs) Handles DownloadRecoveryFW403.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/fw/recovery/04.03/PS5UPDATE.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadRecoveryFW450_Click(sender As Object, e As RoutedEventArgs) Handles DownloadRecoveryFW450.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/fw/recovery/04.50/PS5UPDATE.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadRecoveryFW451_Click(sender As Object, e As RoutedEventArgs) Handles DownloadRecoveryFW451.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/fw/recovery/04.51/PS5UPDATE.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadRecoveryFW550_Click(sender As Object, e As RoutedEventArgs) Handles DownloadRecoveryFW550.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/fw/recovery/05.50/PS5UPDATE.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadRecoveryFW600_Click(sender As Object, e As RoutedEventArgs) Handles DownloadRecoveryFW600.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/fw/recovery/06.00/PS5UPDATE.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadRecoveryFW650_Click(sender As Object, e As RoutedEventArgs) Handles DownloadRecoveryFW650.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/fw/recovery/06.50/PS5UPDATE.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadSystemFW403_Click(sender As Object, e As RoutedEventArgs) Handles DownloadSystemFW403.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/fw/system/04.03/PS5UPDATE.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadSystemFW450_Click(sender As Object, e As RoutedEventArgs) Handles DownloadSystemFW450.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/fw/system/04.50/PS5UPDATE.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadSystemFW451_Click(sender As Object, e As RoutedEventArgs) Handles DownloadSystemFW451.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/fw/system/04.51/PS5UPDATE.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadSystemFW550_Click(sender As Object, e As RoutedEventArgs) Handles DownloadSystemFW550.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/fw/system/05.50/PS5UPDATE.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadSystemFW600_Click(sender As Object, e As RoutedEventArgs) Handles DownloadSystemFW600.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/fw/system/06.00/PS5UPDATE.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadSystemFW650_Click(sender As Object, e As RoutedEventArgs) Handles DownloadSystemFW650.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/fw/system/06.50/PS5UPDATE.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

#End Region

    Private Sub DownloadPS5BDJBElfLoader_Click(sender As Object, e As RoutedEventArgs) Handles DownloadPS5BDJBElfLoader.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/hb/PS5_BD-JB_ELF_Loader_v1.6.1.iso") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub


#End Region

#Region "Exploits"

    Private Sub OpenMast1c0reGitHub_Click(sender As Object, e As RoutedEventArgs) Handles OpenMast1c0reGitHub.Click
        Process.Start("https://github.com/McCaulay/mast1c0re")
    End Sub

    Private Sub DownloadPS5IPV6Expl_Click(sender As Object, e As RoutedEventArgs) Handles DownloadPS5IPV6Expl.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/ex/PS5-IPV6-Kernel-Exploit-1.03.zip") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub OpenKexGitHub_Click(sender As Object, e As RoutedEventArgs) Handles OpenKexGitHub.Click
        Process.Start("https://github.com/Cryptogenic/PS5-IPV6-Kernel-Exploit")
    End Sub

    Private Sub OpenJARLoaderGitHub_Click(sender As Object, e As RoutedEventArgs) Handles OpenJARLoaderGitHub.Click
        Process.Start("https://github.com/hammer-83/ps5-jar-loader")
    End Sub

    Private Sub DownloadJARLoader_Click(sender As Object, e As RoutedEventArgs) Handles DownloadJARLoader.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/hammer-83/ps5-jar-loader/releases/download/v20231027/ps5-jar-loader.iso") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

#End Region

End Class
