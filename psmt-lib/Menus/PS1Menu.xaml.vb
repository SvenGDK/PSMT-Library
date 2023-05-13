Imports System.Windows

Public Class PS1Menu

    Public GamesLView As Controls.ListView

#Region "Menu Downloads"

    Private Sub DownloadFreePSXBoot_Click(sender As Object, e As RoutedEventArgs) Handles DownloadFreePSXBoot.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/folderforps1/freepsxboot-2.1.zip") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadTonyHax_Click(sender As Object, e As RoutedEventArgs) Handles DownloadTonyHax.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/folderforps1/tonyhax-v1.4.5.zip") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

#End Region

#Region "Tools"

    Private Sub OpenMergeBinTool_Click(sender As Object, e As RoutedEventArgs) Handles OpenMergeBinTool.Click
        Dim NewMergeBINTool As New MergeBinTool() With {.ShowActivated = True}
        NewMergeBINTool.Show()
    End Sub

    Private Sub OpenBINCUEConverter_Click(sender As Object, e As RoutedEventArgs) Handles OpenBINCUEConverter.Click
        Dim NewBINCUEConverter As New BINCUEConverter() With {.ShowActivated = True}
        NewBINCUEConverter.Show()
    End Sub

#End Region

End Class
