Imports System.Windows

Public Class PS5Menu

#Region "Tools"

    Private Sub SenderMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles SenderMenuItem.Click
        Dim NewPS5Sender As New PS5Sender() With {.ShowActivated = True}
        NewPS5Sender.Show()
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

    Private Sub OpenMast1c0reGitHub_Click(sender As Object, e As RoutedEventArgs) Handles OpenMast1c0reGitHub.Click
        Process.Start("https://github.com/McCaulay/mast1c0re")
    End Sub

    Private Sub DownloadPS5BDJBElfLoader_Click(sender As Object, e As RoutedEventArgs) Handles DownloadPS5BDJBElfLoader.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps5/hb/PS5_BD-JB_ELF_Loader_v1.6.1.iso") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
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
