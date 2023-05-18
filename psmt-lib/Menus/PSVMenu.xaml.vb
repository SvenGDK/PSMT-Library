Imports System.IO
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media.Imaging

Public Class PSVMenu

    Public LView As Controls.ListView
    Public GamesLView As Controls.ListView

    Dim DownloadsList As New List(Of Structures.Package)
    Dim WithEvents DownloadMenuItem As New Controls.MenuItem() With {.Header = "Download", .Icon = New Image() With {.Source = New BitmapImage(New Uri("/Images/download.png", UriKind.Relative))}}
    Dim WithEvents InfosMenuItem As New Controls.MenuItem() With {.Header = "Show package infos", .Icon = New Image() With {.Source = New BitmapImage(New Uri("/Images/information-button.png", UriKind.Relative))}}

#Region "NPS Downloads"

    Public Sub LoadDownloaderContextMenu()
        Dim DownloaderContextMenu As New Controls.ContextMenu()
        DownloaderContextMenu.Items.Add(DownloadMenuItem)
        DownloaderContextMenu.Items.Add(InfosMenuItem)
        LView.ContextMenu = DownloaderContextMenu
    End Sub

    Private Sub ClearAndShowDLListView()
        'Hide other ListViews
        GamesLView.Visibility = Visibility.Hidden

        If LView.Visibility = Visibility.Hidden Then
            LView.Visibility = Visibility.Visible
        End If

        LView.Items.Clear()
        DownloadsList.Clear()
    End Sub

    Private Sub LanguageCheckBoxChanged(sender As Object, e As RoutedEventArgs)
        LView.Items.Clear()

        If EUCheckbox.IsChecked Then
            For Each item As Structures.Package In DownloadsList.Where(Function(lvi) lvi.PackageRegion.ToLower().Contains("eu"))
                LView.Items.Add(item)
            Next
        End If
        If USCheckbox.IsChecked Then
            For Each item As Structures.Package In DownloadsList.Where(Function(lvi) lvi.PackageRegion.ToLower().Contains("us"))
                LView.Items.Add(item)
            Next
        End If
        If JPCheckbox.IsChecked Then
            For Each item As Structures.Package In DownloadsList.Where(Function(lvi) lvi.PackageRegion.ToLower().Contains("jp"))
                LView.Items.Add(item)
            Next
        End If
        If INTCheckbox.IsChecked Then
            For Each item As Structures.Package In DownloadsList.Where(Function(lvi) lvi.PackageRegion.ToLower().Contains("int"))
                LView.Items.Add(item)
            Next
        End If
    End Sub

    Private Async Sub LoadDLList(RequestedList As String, LoadLatest As Boolean)
        'Get the latest database from NPS
        If LoadLatest = True Then
            Using NewWebClient As New WebClient
                Dim GamesList As String = Await NewWebClient.DownloadStringTaskAsync(New Uri("https://nopaystation.com/tsv/" + RequestedList))
                Dim GamesListLines As String() = GamesList.Split(CChar(vbCrLf))
                For Each GameLine As String In GamesListLines.Skip(1)
                    Dim SplittedValues As String() = GameLine.Split(CChar(vbTab))
                    Dim AdditionalInfo As Structures.PackageInfo = Utils.GetFileSizeAndDate(SplittedValues(8).Trim(), SplittedValues(6).Trim())
                    Dim NewPackage As New Structures.Package() With {.PackageName = SplittedValues(2).Trim(),
                        .PackageURL = SplittedValues(3).Trim(),
                        .PackageTitleID = SplittedValues(0).Trim(),
                        .PackageContentID = SplittedValues(5).Trim(),
                        .PackageRAP = SplittedValues(4).Trim(),
                        .PackageDate = AdditionalInfo.FileDate,
                        .PackageSize = AdditionalInfo.FileSize,
                        .PackageRegion = SplittedValues(1).Trim()}
                    If Not SplittedValues(3).Trim() = "MISSING" Then 'Only add available PKGs
                        DownloadsList.Add(NewPackage)
                    End If
                Next
            End Using
        Else 'Use local .tsv file
            If File.Exists(My.Computer.FileSystem.CurrentDirectory + "\Databases\" + RequestedList) Then
                Dim FileReader As String() = File.ReadAllLines(My.Computer.FileSystem.CurrentDirectory + "\Databases\" + RequestedList, Text.Encoding.UTF8)
                For Each GameLine As String In FileReader.Skip(1) 'Skip 1st line in TSV
                    Dim SplittedValues As String() = GameLine.Split(CChar(vbTab))
                    Dim AdditionalInfo As Structures.PackageInfo = Utils.GetFileSizeAndDate(SplittedValues(8), SplittedValues(6))
                    Dim NewPackage As New Structures.Package() With {.PackageName = SplittedValues(2),
                        .PackageURL = SplittedValues(3),
                        .PackageTitleID = SplittedValues(0),
                        .PackageContentID = SplittedValues(5),
                        .PackageRAP = SplittedValues(4),
                        .PackageDate = AdditionalInfo.FileDate,
                        .PackageSize = AdditionalInfo.FileSize,
                        .PackageRegion = SplittedValues(1)}
                    If Not SplittedValues(3) = "MISSING" Then 'Only add available PKGs
                        DownloadsList.Add(NewPackage)
                    End If
                Next
            Else
                MsgBox("Nothing available. Please add TSV files to the 'Databases' directory.", MsgBoxStyle.Exclamation, "Could not load list")
            End If
        End If

        'Add to the list
        For Each AvailablePKG In DownloadsList
            LView.Items.Add(AvailablePKG)
        Next
    End Sub

    Private Sub LoadGamesListMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles LoadGamesListMenuItem.Click
        ClearAndShowDLListView()

        Dim Result As MsgBoxResult = MsgBox("Do you want to load the latest database ?", MsgBoxStyle.YesNoCancel, "Loading game database")
        If Result = MsgBoxResult.Yes Then
            LoadDLList("PSV_GAMES.tsv", True)
        ElseIf Result = MsgBoxResult.No Then
            LoadDLList("PSV_GAMES.tsv", False)
        End If
    End Sub

    Private Sub LoadDemosListMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles LoadDemosListMenuItem.Click
        ClearAndShowDLListView()

        Dim Result As MsgBoxResult = MsgBox("Do you want to load the latest database ?", MsgBoxStyle.YesNoCancel, "Loading demos database")
        If Result = MsgBoxResult.Yes Then
            LoadDLList("PSV_DEMOS.tsv", True)
        ElseIf Result = MsgBoxResult.No Then
            LoadDLList("PSV_DEMOS.tsv", False)
        End If
    End Sub

    Private Sub LoadDLCsListMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles LoadDLCsListMenuItem.Click
        ClearAndShowDLListView()

        Dim Result As MsgBoxResult = MsgBox("Do you want to load the latest database ?", MsgBoxStyle.YesNoCancel, "Loading DLCs database")
        If Result = MsgBoxResult.Yes Then
            LoadDLList("PSV_DLCS.tsv", True)
        ElseIf Result = MsgBoxResult.No Then
            LoadDLList("PSV_DLCS.tsv", False)
        End If
    End Sub

    Private Sub LoadThemesListMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles LoadThemesListMenuItem.Click
        ClearAndShowDLListView()

        Dim Result As MsgBoxResult = MsgBox("Do you want to load the latest database ?", MsgBoxStyle.YesNoCancel, "Loading themes database")
        If Result = MsgBoxResult.Yes Then
            LoadDLList("PSV_THEMES.tsv", True)
        ElseIf Result = MsgBoxResult.No Then
            LoadDLList("PSV_THEMES.tsv", False)
        End If
    End Sub

    Private Sub LoadUpdatesListMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles LoadUpdatesListMenuItem.Click
        ClearAndShowDLListView()

        Dim Result As MsgBoxResult = MsgBox("Do you want to load the latest database ?", MsgBoxStyle.YesNoCancel, "Loading updates database")
        If Result = MsgBoxResult.Yes Then
            LoadDLList("PSV_UPDATES.tsv", True)
        ElseIf Result = MsgBoxResult.No Then
            LoadDLList("PSV_UPDATES.tsv", False)
        End If
    End Sub

    Private Sub TextSearch(TxtBox As Controls.TextBox)
        LView.Items.Clear()

        If TxtBox.Name = NameSearchTextBox.Name Then
            For Each item As Structures.Package In DownloadsList.Where(Function(lvi) lvi.PackageName.ToLower().Contains(TxtBox.Text.ToLower().Trim()))
                LView.Items.Add(item)
            Next
        ElseIf TxtBox.Name = TIDSearchTextBox.Name Then
            For Each item As Structures.Package In DownloadsList.Where(Function(lvi) lvi.PackageTitleID.ToLower().Contains(TxtBox.Text.ToLower().Trim()))
                LView.Items.Add(item)
            Next
        ElseIf TxtBox.Name = CIDSearchTextBox.Name Then
            For Each item As Structures.Package In DownloadsList.Where(Function(lvi) lvi.PackageContentID.ToLower().Contains(TxtBox.Text.ToLower().Trim()))
                LView.Items.Add(item)
            Next
        End If
    End Sub

    Private Sub NameSearchTextBox_TextChanged(sender As Object, e As TextChangedEventArgs) Handles NameSearchTextBox.TextChanged
        TextSearch(NameSearchTextBox)
    End Sub

    Private Sub TIDSearchTextBox_TextChanged(sender As Object, e As TextChangedEventArgs) Handles TIDSearchTextBox.TextChanged
        TextSearch(TIDSearchTextBox)
    End Sub

    Private Sub CIDSearchTextBox_TextChanged(sender As Object, e As TextChangedEventArgs) Handles CIDSearchTextBox.TextChanged
        TextSearch(CIDSearchTextBox)
    End Sub

#End Region

#Region "Downloads Contextmenu"

    Private Sub DownloadMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles DownloadMenuItem.Click
        If LView.SelectedItem IsNot Nothing Then
            Dim SelectedPackage As Structures.Package = CType(LView.SelectedItem, Structures.Package)

            Dim NewDownloader As New Downloader() With {.ShowActivated = True, .Title = "Downloading " + SelectedPackage.PackageName,
                .PackageConsole = "PSV",
                .PackageContentID = SelectedPackage.PackageContentID,
                .PackageTitleID = SelectedPackage.PackageTitleID}

            NewDownloader.Show()
            If NewDownloader.CreateNewDownload(SelectedPackage.PackageURL, True, SelectedPackage.PackageName + ".pkg") = False Then
                MsgBox("Could not download " + SelectedPackage.PackageName, MsgBoxStyle.Critical)
                NewDownloader.Close()
            End If
        End If
    End Sub

    Private Sub InfosMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles InfosMenuItem.Click
        If LView.SelectedItem IsNot Nothing Then
            Dim SelectedPackage As Structures.Package = CType(LView.SelectedItem, Structures.Package)
            Dim NewPackageInfoWindow As New DownloadPackageInfoWindow() With {.ShowActivated = True, .Title = SelectedPackage.PackageName, .CurrentPackage = SelectedPackage, .PackageConsole = "PSV"}
            NewPackageInfoWindow.Show()
        End If
    End Sub

#End Region

#Region "Menu Downloads"

#Region "Homebrew"

    Private Sub DownloadAdrenaline_Click(sender As Object, e As RoutedEventArgs) Handles DownloadAdrenaline.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/Adrenaline-7.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadAdrenalineBubbleManager_Click(sender As Object, e As RoutedEventArgs) Handles DownloadAdrenalineBubbleManager.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/AdrenalineBubbleManager-6.19.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadAdrenalineBubbleManagerGitHub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadAdrenalineBubbleManagerGitHub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/ONElua/AdrenalineBubbleManager/releases/latest/download/AdrenalineBubbleManager.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadAdrenalineGitHub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadAdrenalineGitHub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/TheOfficialFloW/Adrenaline/releases/latest/download/Adrenaline.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadAdrenalineStates_Click(sender As Object, e As RoutedEventArgs) Handles DownloadAdrenalineStates.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/AdrenalineStates.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadApollo_Click(sender As Object, e As RoutedEventArgs) Handles DownloadApollo.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/apollo-vita-1.2.4.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadApolloGitHub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadApolloGitHub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/bucanero/apollo-vita/releases/latest/download/apollo-vita.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadAppDBTool_Click(sender As Object, e As RoutedEventArgs) Handles DownloadAppDBTool.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/appdbtool.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadDownloadEnabler_Click(sender As Object, e As RoutedEventArgs) Handles DownloadDownloadEnabler.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/download_enabler.suprx") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadDownloadEnablerGithub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadDownloadEnablerGithub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/TheOfficialFloW/VitaTweaks/releases/latest/download/download_enabler.suprx") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadElevenMPV_Click(sender As Object, e As RoutedEventArgs) Handles DownloadElevenMPV.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/ElevenMPV-A-7.10.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadElevenMPVGitHub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadElevenMPVGitHub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/GrapheneCt/ElevenMPV-A/releases/latest/download/ElevenMPV-A.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadFontInstaller_Click(sender As Object, e As RoutedEventArgs) Handles DownloadFontInstaller.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/fontInstaller-1.0.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadFontInstallerGitHub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadFontInstallerGitHub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/cxziaho/fontInstaller/releases/download/latest/fontInstaller.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadFontRedirect_Click(sender As Object, e As RoutedEventArgs) Handles DownloadFontRedirect.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/fontRedirect.suprx") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadFTPClient_Click(sender As Object, e As RoutedEventArgs) Handles DownloadFTPClient.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/ftpclient-1.54.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadFTPClientGitHub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadFTPClientGitHub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/cy33hc/vita-ftp-client/releases/download/latest/ftpclient.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadiTLSEnso_Click(sender As Object, e As RoutedEventArgs) Handles DownloadiTLSEnso.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/iTLS-Enso-3.2.1.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadiTLSEnsoGitHub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadiTLSEnsoGitHub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/SKGleba/iTLS-Enso/releases/latest/download/iTLS-Enso.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadMediaImporter_Click(sender As Object, e As RoutedEventArgs) Handles DownloadMediaImporter.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/MediaImporter-0.91.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadMediaImporterGitHub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadMediaImporterGitHub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/cnsldv/MediaImporter/releases/latest/download/MediaImporter.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadMoonlight_Click(sender As Object, e As RoutedEventArgs) Handles DownloadMoonlight.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/moonlight-0.9.2.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadMoonlightGitHub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadMoonlightGitHub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/xyzz/vita-moonlight/releases/latest/download/moonlight.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadMultidownloadVita_Click(sender As Object, e As RoutedEventArgs) Handles DownloadMultidownloadVita.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/Multidownload-Vita-1.0.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadMultidownloadVitaGitHub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadMultidownloadVitaGitHub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/DavisDev/Multidownload-Vita/releases/latest/download/Multidownload-Vita.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadMVPLAYER0_Click(sender As Object, e As RoutedEventArgs) Handles DownloadMVPLAYER0.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/MVPLAYER0-1.3.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadMVPLAYER0GitHub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadMVPLAYER0GitHub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/AntHJ/MVPlayer/releases/latest/download/MVPLAYER0.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadNetcheckBypass_Click(sender As Object, e As RoutedEventArgs) Handles DownloadNetcheckBypass.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/netcheck_bypass.suprx") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadNetcheckBypassGithub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadNetcheckBypassGithub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/yifanlu/netcheck_bypass/releases/latest/download/netcheck_bypass.suprx") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadNetStream_Click(sender As Object, e As RoutedEventArgs) Handles DownloadNetStream.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/NetStream-2.04.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadNetStreamGitHub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadNetStreamGitHub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/GrapheneCt/NetStream/releases/latest/download/NetStream.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadParentalControlBypass_Click(sender As Object, e As RoutedEventArgs) Handles DownloadParentalControlBypass.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/Parental_Control_Bypass.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub Downloadpkgj_Click(sender As Object, e As RoutedEventArgs) Handles Downloadpkgj.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/pkgj-0.55.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadpkgjGitHub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadpkgjGitHub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/blastrock/pkgj/releases/latest/download/pkgj.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadPNGShot_Click(sender As Object, e As RoutedEventArgs) Handles DownloadPNGShot.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/pngshot.suprx") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadPNGShotGithub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadPNGShotGithub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/xyzz/pngshot/releases/latest/download/pngshot.suprx") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadRegistryEditor_Click(sender As Object, e As RoutedEventArgs) Handles DownloadRegistryEditor.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/RegistryEditor-1.0.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadRetroFlow_Click(sender As Object, e As RoutedEventArgs) Handles DownloadRetroFlow.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/RetroFlow_v6.0.0.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadRetroFlowAdrenalineLauncher_Click(sender As Object, e As RoutedEventArgs) Handles DownloadRetroFlowAdrenalineLauncher.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/RetroFlow_Adrenaline_Launcher_v3.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadRetroFlowAdrenalineLauncherGitHub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadRetroFlowAdrenalineLauncherGitHub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/jimbob4000/RetroFlow-Launcher/releases/latest/download/RetroFlow_Adrenaline_Launcher_v3.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadRetroFlowGitHub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadRetroFlowGitHub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/jimbob4000/RetroFlow-Launcher/releases/latest/download/RetroFlow_v6.0.0.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadSMBClient_Click(sender As Object, e As RoutedEventArgs) Handles DownloadSMBClient.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/smbclient-1.04.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadSMBClientGitHub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadSMBClientGitHub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/cy33hc/vita-smb-client/releases/download/latest/smbclient.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadUSBDisable_Click(sender As Object, e As RoutedEventArgs) Handles DownloadUSBDisable.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/USBDisable.suprx") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadUSBDisableGithub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadUSBDisableGithub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/Ibrahim778/USBDisable/releases/latest/download/USBDisable.suprx") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadUserAgentSpoofer_Click(sender As Object, e As RoutedEventArgs) Handles DownloadUserAgentSpoofer.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/UserAgentSpoofer.suprx") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadUserAgentSpooferGithub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadUserAgentSpooferGithub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/GrapheneCt/UserAgentSpoofer/releases/latest/download/UserAgentSpoofer.suprx") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadVITAlbum_Click(sender As Object, e As RoutedEventArgs) Handles DownloadVITAlbum.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/VITAlbum-1.40.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadVITAlbumGitHub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadVITAlbumGitHub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/joel16/VITAlbum/releases/latest/download/VITAlbum.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadVitaMediaPlayer_Click(sender As Object, e As RoutedEventArgs) Handles DownloadVitaMediaPlayer.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/VitaMediaPlayer-1.01.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadVitaMediaPlayerGitHub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadVitaMediaPlayerGitHub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/SonicMastr/Vita-Media-Player/releases/latest/download/VitaMediaPlayer.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadVitaShell_Click(sender As Object, e As RoutedEventArgs) Handles DownloadVitaShell.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/VitaShell-2.02.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadVitaShellGitHub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadVitaShellGitHub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/TheOfficialFloW/VitaShell/releases/latest/download/VitaShell.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadWebDAVClient_Click(sender As Object, e As RoutedEventArgs) Handles DownloadWebDAVClient.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/webdavclient-1.02.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadWebDAVClientGitHub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadWebDAVClientGitHub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/cy33hc/vita-webdav-client/releases/download/latest/webdavclient.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloaTrophiesFixer_Click(sender As Object, e As RoutedEventArgs) Handles DownloaTrophiesFixer.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/hb/trophies_fixer-1.1.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloaTrophiesFixerGitHub_Click(sender As Object, e As RoutedEventArgs) Handles DownloaTrophiesFixerGitHub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/Yoti/psv_trophfix/releases/download/latest/trophies_fixer.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

#End Region

#Region "Firmwares"

    Private Sub DownloadOFW365_Click(sender As Object, e As RoutedEventArgs) Handles DownloadOFW365.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/fw/OFW3.65.7z") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

#End Region

#Region "Emulators"

    Private Sub DownloadDaedalusX64_Click(sender As Object, e As RoutedEventArgs) Handles DownloadDaedalusX64.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/emu/DaedalusX64.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadDaedalusX64Github_Click(sender As Object, e As RoutedEventArgs) Handles DownloadDaedalusX64Github.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/Rinnegatamante/DaedalusX64-vitaGL/releases/latest/download/DaedalusX64.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadFlycast_Click(sender As Object, e As RoutedEventArgs) Handles DownloadFlycast.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/emu/Flycast-1.1.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadFlycastGithub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadFlycastGithub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/Rinnegatamante/flycast-vita/releases/latest/download/Flycast.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadmGBA_Click(sender As Object, e As RoutedEventArgs) Handles DownloadmGBA.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/emu/mGBA-0.10.1-vita.7z") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadRetroArch_Click(sender As Object, e As RoutedEventArgs) Handles DownloadRetroArch.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/emu/RetroArch.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadRetroArchData_Click(sender As Object, e As RoutedEventArgs) Handles DownloadRetroArchData.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/emu/RetroArch_data.7z") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadSnes9xVITA_Click(sender As Object, e As RoutedEventArgs) Handles DownloadSnes9xVITA.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/vita/emu/Snes9xVITA.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadSnes9xVITAGithub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadSnes9xVITAGithub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/theheroGAC/Snes9xVITA/releases/download/latest/Snes9xVITA.vpk") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

#End Region

#End Region

#Region "Tools"

    Private Sub OpenPKGExtractor_Click(sender As Object, e As RoutedEventArgs) Handles OpenPKGExtractor.Click
        Dim NewPKGExtractor As New PKGExtractor() With {.ShowActivated = True}
        NewPKGExtractor.Show()
    End Sub

    Private Sub OpenRCOExtractor_Click(sender As Object, e As RoutedEventArgs) Handles OpenRCOExtractor.Click
        Dim NewVitaRCOMage As New VitaRCOMage() With {.ShowActivated = True}
        NewVitaRCOMage.Show()
    End Sub

    Private Sub OpenIMGTools_Click(sender As Object, e As RoutedEventArgs) Handles OpenIMGTools.Click
        'Dim NewPSVIMGTools As New PSVIMGTools() With {.ShowActivated = True}
        'NewPSVIMGTools.Show()
        MsgBox("Not ready yet", MsgBoxStyle.Information)
    End Sub

    Private Sub OpenParamSFOEditorMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles OpenParamSFOEditorMenuItem.Click
        Dim NewSFOEditor As New SFOEditor() With {.ShowActivated = True}
        NewSFOEditor.Show()
    End Sub

#End Region

End Class
