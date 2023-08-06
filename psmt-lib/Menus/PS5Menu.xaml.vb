﻿Imports System.Windows
Imports System.Windows.Controls

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

    Private Sub OpenFTPBDRipMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles OpenFTPBDRipMenuItem.Click
        Dim NewFTPBDRip As New FTPBDRip() With {.ShowActivated = True}

        'Set values if SharedConsoleAddress is set
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            NewFTPBDRip.ConsoleIP = SharedConsoleAddress.Split(":"c)(0)
            NewFTPBDRip.IPTextBox.Text = SharedConsoleAddress.Split(":"c)(0)
            NewFTPBDRip.PortTextBox.Text = SharedConsoleAddress.Split(":"c)(1)
        End If

        NewFTPBDRip.Show()
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

#End Region

End Class
