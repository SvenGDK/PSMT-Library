Imports System.IO
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media.Imaging

Public Class PS3Menu

    Public LView As Controls.ListView
    Public PS3Img As Controls.Image
    Public PS3Title As Controls.TextBlock
    Public GamesLView As Controls.ListView

    Dim DownloadsList As New List(Of Structures.Package)
    Dim WithEvents DownloadMenuItem As New Controls.MenuItem() With {.Header = "Download", .Icon = New Image() With {.Source = New BitmapImage(New Uri("/Images/download.png", UriKind.Relative))}}
    Dim WithEvents InfosMenuItem As New Controls.MenuItem() With {.Header = "Show package infos", .Icon = New Image() With {.Source = New BitmapImage(New Uri("/Images/information-button.png", UriKind.Relative))}}
    Dim WithEvents CreateRAPMenuItem As New Controls.MenuItem() With {.Header = "Create .rap file", .Icon = New Image() With {.Source = New BitmapImage(New Uri("/Images/create.png", UriKind.Relative))}}

#Region "NPS Downloads"

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
            LoadDLList("PS3_GAMES.tsv", True)
        ElseIf Result = MsgBoxResult.No Then
            LoadDLList("PS3_GAMES.tsv", False)
        End If
    End Sub

    Private Sub LoadDemosListMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles LoadDemosListMenuItem.Click
        ClearAndShowDLListView()

        Dim Result As MsgBoxResult = MsgBox("Do you want to load the latest database ?", MsgBoxStyle.YesNoCancel, "Loading demos database")
        If Result = MsgBoxResult.Yes Then
            LoadDLList("PS3_DEMOS.tsv", True)
        ElseIf Result = MsgBoxResult.No Then
            LoadDLList("PS3_DEMOS.tsv", False)
        End If
    End Sub

    Private Sub LoadDLCsListMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles LoadDLCsListMenuItem.Click
        ClearAndShowDLListView()

        Dim Result As MsgBoxResult = MsgBox("Do you want to load the latest database ?", MsgBoxStyle.YesNoCancel, "Loading DLCs database")
        If Result = MsgBoxResult.Yes Then
            LoadDLList("PS3_DLCS.tsv", True)
        ElseIf Result = MsgBoxResult.No Then
            LoadDLList("PS3_DLCS.tsv", False)
        End If
    End Sub

    Private Sub LoadThemesListMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles LoadThemesListMenuItem.Click
        ClearAndShowDLListView()

        Dim Result As MsgBoxResult = MsgBox("Do you want to load the latest database ?", MsgBoxStyle.YesNoCancel, "Loading themes database")
        If Result = MsgBoxResult.Yes Then
            LoadDLList("PS3_THEMES.tsv", True)
        ElseIf Result = MsgBoxResult.No Then
            LoadDLList("PS3_THEMES.tsv", False)
        End If
    End Sub

    Private Sub LoadAvatarsListMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles LoadAvatarsListMenuItem.Click
        ClearAndShowDLListView()

        Dim Result As MsgBoxResult = MsgBox("Do you want to load the latest database ?", MsgBoxStyle.YesNoCancel, "Loading avatars database")
        If Result = MsgBoxResult.Yes Then
            LoadDLList("PS3_AVATARS.tsv", True)
        ElseIf Result = MsgBoxResult.No Then
            LoadDLList("PS3_AVATARS.tsv", False)
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

    Private Sub CreateRAPMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles CreateRAPMenuItem.Click
        If LView.SelectedItem IsNot Nothing Then
            Dim SelectedPackage As Structures.Package = CType(LView.SelectedItem, Structures.Package)
            CreateRAP(SelectedPackage.PackageContentID, SelectedPackage.PackageRAP)
        End If
    End Sub

    Public Sub LoadDownloaderContextMenu()
        Dim DownloaderContextMenu As New Controls.ContextMenu()
        DownloaderContextMenu.Items.Add(DownloadMenuItem)
        DownloaderContextMenu.Items.Add(InfosMenuItem)
        DownloaderContextMenu.Items.Add(CreateRAPMenuItem)
        LView.ContextMenu = DownloaderContextMenu
    End Sub

    Private Sub ClearAndShowDLListView()
        If GamesLView.Visibility = Visibility.Visible Then
            PS3Img.Visibility = Visibility.Hidden
            PS3Title.Visibility = Visibility.Hidden
            GamesLView.Visibility = Visibility.Hidden
        End If

        If LView.Visibility = Visibility.Hidden Then
            LView.Visibility = Visibility.Visible
        End If

        LView.Items.Clear()
        DownloadsList.Clear()
    End Sub

    Private Sub CreateRAP(ContentID As String, RAP As String)
        Try
            If Not String.IsNullOrEmpty(ContentID) AndAlso RAP.Length Mod 2 = 0 Then

                'Create exdata folder if not exists
                If Not Directory.Exists(My.Computer.FileSystem.CurrentDirectory + "\Downloads\exdata") Then Directory.CreateDirectory(My.Computer.FileSystem.CurrentDirectory + "\Downloads\exdata")

                Dim bytes As Byte() = New Byte(CInt((RAP.Length / 2) - 1)) {}
                For index As Integer = 0 To CInt((RAP.Length / 2) - 1)
                    bytes(index) = Convert.ToByte(RAP.Substring(index * 2, 2), 16)
                Next
                File.WriteAllBytes(My.Computer.FileSystem.CurrentDirectory + "\Downloads\exdata\" + ContentID & ".rap", bytes)

                MsgBox(ContentID & ".rap created!" & vbCrLf & "You can find it in the 'Downloads\exdata' folder.", MsgBoxStyle.Information)
            Else
                MsgBox("This package requires no .rap file. Simply activate it with ReactPSN.", MsgBoxStyle.Information)
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

#End Region

#Region "Downloads Contextmenu"

    Private Sub InfosMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles InfosMenuItem.Click
        If LView.SelectedItem IsNot Nothing Then
            Dim SelectedPackage As Structures.Package = CType(LView.SelectedItem, Structures.Package)
            Dim NewPackageInfoWindow As New DownloadPackageInfoWindow() With {.ShowActivated = True, .Title = SelectedPackage.PackageName, .CurrentPackage = SelectedPackage, .PackageConsole = "PS3"}
            NewPackageInfoWindow.Show()
        End If
    End Sub

    Private Sub DownloadMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles DownloadMenuItem.Click
        If LView.SelectedItem IsNot Nothing Then
            Dim SelectedPackage As Structures.Package = CType(LView.SelectedItem, Structures.Package)

            Dim NewDownloader As New Downloader() With {.ShowActivated = True, .Title = "Downloading " + SelectedPackage.PackageName,
                .PackageConsole = "PS3",
                .PackageContentID = SelectedPackage.PackageContentID,
                .PackageTitleID = SelectedPackage.PackageTitleID}

            NewDownloader.Show()
            If NewDownloader.CreateNewDownload(SelectedPackage.PackageURL, True, SelectedPackage.PackageName + ".pkg") = False Then
                MsgBox("Could not download " + SelectedPackage.PackageName, MsgBoxStyle.Critical)
                NewDownloader.Close()
            End If
        End If
    End Sub

#End Region

#Region "Tools"

    Private Sub OpenSFOEditor_Click(sender As Object, e As RoutedEventArgs) Handles OpenSFOEditor.Click
        Dim NewSFOEditor As New SFOEditor() With {.ShowActivated = True}
        NewSFOEditor.Show()
    End Sub

    Private Sub OpenISOTools_Click(sender As Object, e As RoutedEventArgs) Handles OpenISOTools.Click
        Dim NewISOTools As New PS3ISOTools() With {.ShowActivated = True}
        NewISOTools.Show()
    End Sub

    Private Sub OpenCoreOSTools_Click(sender As Object, e As RoutedEventArgs) Handles OpenCoreOSTools.Click
        Dim NewCoreOSTools As New SFOEditor() With {.ShowActivated = True}
        NewCoreOSTools.Show()
    End Sub

    Private Sub OpenFixTar_Click(sender As Object, e As RoutedEventArgs) Handles OpenFixTar.Click
        Dim NewFixTar As New SFOEditor() With {.ShowActivated = True}
        NewFixTar.Show()
    End Sub

    Private Sub OpenPUPUnpacker_Click(sender As Object, e As RoutedEventArgs) Handles OpenPUPUnpacker.Click
        Dim NewPUPUnpacker As New SFOEditor() With {.ShowActivated = True}
        NewPUPUnpacker.Show()
    End Sub

    Private Sub OpenRCODumper_Click(sender As Object, e As RoutedEventArgs) Handles OpenRCODumper.Click
        Dim NewRCODumper As New SFOEditor() With {.ShowActivated = True}
        NewRCODumper.Show()
    End Sub

    Private Sub OpenSELFReader_Click(sender As Object, e As RoutedEventArgs) Handles OpenSELFReader.Click
        Dim NewSELFReader As New SFOEditor() With {.ShowActivated = True}
        NewSELFReader.Show()
    End Sub

    Private Sub OpenFTPBrowser_Click(sender As Object, e As RoutedEventArgs) Handles OpenFTPBrowser.Click
        Dim NewFTPBrowser As New FTPBrowser() With {.ShowActivated = True}
        NewFTPBrowser.Show()
    End Sub

#End Region

#Region "Menu Downloads"

#Region "Homebrew"

    Private Sub DownloadAdvancedPowerOptions_Click(sender As Object, e As RoutedEventArgs) Handles DownloadAdvancedPowerOptions.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/hb/Advanced_Power_Options_v1.11.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadAdvancedTools_Click(sender As Object, e As RoutedEventArgs) Handles DownloadAdvancedTools.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/hb/PS3AdvancedTools_v1.0.1.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadApollo_Click(sender As Object, e As RoutedEventArgs) Handles DownloadApollo.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/hb/apollo-ps3-v1.8.4.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadApolloGithub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadApolloGithub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/bucanero/apollo-ps3/releases/latest/download/apollo-ps3.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadArtemis_Click(sender As Object, e As RoutedEventArgs) Handles DownloadArtemis.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/hb/ArtemisPS3-GUI-r6.3..pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadArtemisGithub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadArtemisGithub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/bucanero/ArtemisPS3/releases/latest/download/ArtemisPS3-GUI.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadAwesomeMPManager_Click(sender As Object, e As RoutedEventArgs) Handles DownloadAwesomeMPManager.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/hb/Awesome_MountPoint_Manager_1.1a.AllCFW.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadCCAPI_Click(sender As Object, e As RoutedEventArgs) Handles DownloadCCAPI.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/hb/CCAPI_v2.80_Rev10.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadComgenieGeohot_Click(sender As Object, e As RoutedEventArgs) Handles DownloadComgenieGeohot.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/hb/ComgenieAwesomeFilemanager355.zip") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadComgenieNew_Click(sender As Object, e As RoutedEventArgs) Handles DownloadComgenieNew.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/hb/ComgenieAwesomeFilemanager421.zip") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadComgenieOld_Click(sender As Object, e As RoutedEventArgs) Handles DownloadComgenieOld.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/hb/ComgenieAwesomeFilemanager.zip") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadIrisman_Click(sender As Object, e As RoutedEventArgs) Handles DownloadIrisman.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/hb/IRISMAN_4.90.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadIrismanGithub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadIrismanGithub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/aldostools/IRISMAN/releases/download/4.90/IRISMAN_4.90.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadManagunzBM_Click(sender As Object, e As RoutedEventArgs) Handles DownloadManagunzBM.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/hb/ManaGunZ_v1.41.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadManagunzFM_Click(sender As Object, e As RoutedEventArgs) Handles DownloadManagunzFM.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/hb/ManaGunZ_FileManager_v1.41.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadMovian_Click(sender As Object, e As RoutedEventArgs) Handles DownloadMovian.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/hb/movian-5.0.730-deank-playstation3.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadMultiMAN_Click(sender As Object, e As RoutedEventArgs) Handles DownloadMultiMAN.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/hb/multiMAN_04.85.01_BASE_(20191010).pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadNetsrv_Click(sender As Object, e As RoutedEventArgs) Handles DownloadNetsrv.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/aldostools/webMAN-MOD/releases/download/1.47.45/ps3netsrv_20220813.zip") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadPKGi_Click(sender As Object, e As RoutedEventArgs) Handles DownloadPKGi.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/hb/pkgi-ps3.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadPKGiGithub_Click(sender As Object, e As RoutedEventArgs) Handles DownloadPKGiGithub.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/bucanero/pkgi-ps3/releases/latest/download/pkgi-ps3.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadPrepIso_Click(sender As Object, e As RoutedEventArgs) Handles DownloadPrepIso.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/aldostools/webMAN-MOD/releases/download/1.47.45/prepISO_1.30.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadPS2ClassicsLauncher_Click(sender As Object, e As RoutedEventArgs) Handles DownloadPS2ClassicsLauncher.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/aldostools/webMAN-MOD/releases/download/1.47.45/PS2_Classics_Launcher.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadPS2Config_Click(sender As Object, e As RoutedEventArgs) Handles DownloadPS2Config.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/aldostools/webMAN-MOD/releases/download/1.47.45/PS2CONFIG.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadPSPMinisLauncher_Click(sender As Object, e As RoutedEventArgs) Handles DownloadPSPMinisLauncher.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/aldostools/webMAN-MOD/releases/download/1.47.45/PSP_Minis_Launcher.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadPSPRemastersLauncher_Click(sender As Object, e As RoutedEventArgs) Handles DownloadPSPRemastersLauncher.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/aldostools/webMAN-MOD/releases/download/1.47.45/PSP_Remasters_Launcher.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadReact_Click(sender As Object, e As RoutedEventArgs) Handles DownloadReact.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/hb/reActPSN_v3.20+.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadRebugToolbox_Click(sender As Object, e As RoutedEventArgs) Handles DownloadRebugToolbox.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/hb/REBUG_TOOLBOX_02.03.06.MULTI.16.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadSENEnabler_Click(sender As Object, e As RoutedEventArgs) Handles DownloadSENEnabler.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/hb/SEN_Enabler_v6.2.7_[CEX-DEX]_[4.87].pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadUltimateToolbox_Click(sender As Object, e As RoutedEventArgs) Handles DownloadUltimateToolbox.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/hb/Ultimate_Toolbox_v2.03_FULL_version.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadUnlockHDDSpace_Click(sender As Object, e As RoutedEventArgs) Handles DownloadUnlockHDDSpace.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/hb/Unlock_HDD_Space.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadWebManMod_Click(sender As Object, e As RoutedEventArgs) Handles DownloadWebManMod.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/aldostools/webMAN-MOD/releases/download/1.47.45/webMAN_MOD_1.47.45_Installer.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

#End Region

#Region "Firmwares"

    Private Sub Download355DexDowngrader_Click(sender As Object, e As RoutedEventArgs) Handles Download355DexDowngrader.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/fw/cfw/3.55/PS3-CFW-3.55-DEX-DOWNGRADER_PS3UPDAT.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadCobra355_Click(sender As Object, e As RoutedEventArgs) Handles DownloadCobra355.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/fw/cfw/3.55/Cobra%203.55%20CFW/PS3UPDAT.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadGeoHot_Click(sender As Object, e As RoutedEventArgs) Handles DownloadGeoHot.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/fw/cfw/3.55/GeoHot%203.55%20CFW/PS3UPDAT.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadKmeaw_Click(sender As Object, e As RoutedEventArgs) Handles DownloadKmeaw.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/fw/cfw/3.55/Kmeaw%203.55%20CFW/PS3UPDAT.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadMiralaTijera_Click(sender As Object, e As RoutedEventArgs) Handles DownloadMiralaTijera.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/fw/cfw/3.55/MiralaTijera%203.55%20CFW/PS3UPDAT.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadOFW102_Click(sender As Object, e As RoutedEventArgs) Handles DownloadOFW102.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/fw/ofw/1.02/PS3UPDAT.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadOFW315_Click(sender As Object, e As RoutedEventArgs) Handles DownloadOFW315.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/fw/ofw/3.15/PS3UPDAT.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadOFW355_Click(sender As Object, e As RoutedEventArgs) Handles DownloadOFW355.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/fw/ofw/3.55/PS3UPDAT.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadOTHEROSColdBoot_Click(sender As Object, e As RoutedEventArgs) Handles DownloadOTHEROSColdBoot.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/fw/cfw/3.55/OTHEROS++%20COLD-BOOT%203.55%20CFW/PS3UPDAT.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadOTHEROSSpecial_Click(sender As Object, e As RoutedEventArgs) Handles DownloadOTHEROSSpecial.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/fw/cfw/3.55/OTHEROS++%20SPECIAL%203.55%20CFW/PS3UPDAT.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadPS3ITA_Click(sender As Object, e As RoutedEventArgs) Handles DownloadPS3ITA.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/fw/cfw/3.55/PS3ITA%203.55%20CFW%20v1.1/PS3UPDAT.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadPS3ULTIMATE_Click(sender As Object, e As RoutedEventArgs) Handles DownloadPS3ULTIMATE.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/fw/cfw/3.55/PS3ULTIMATE%203.55%20CFW/PS3UPDAT.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadRebugRex_Click(sender As Object, e As RoutedEventArgs) Handles DownloadRebugRex.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/fw/cfw/3.55/REBUG%20REX%20EDITION%203.55.4%20CFW/PS3UPDAT.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadRogero_Click(sender As Object, e As RoutedEventArgs) Handles DownloadRogero.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/fw/cfw/3.55/Rogero%20v3.7%203.55%20CFW/PS3UPDAT.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadWaninkoko_Click(sender As Object, e As RoutedEventArgs) Handles DownloadWaninkoko.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/fw/cfw/3.55/Waninkoko%203.55%20CFW%20v2/PS3UPDAT.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadWutangrza_Click(sender As Object, e As RoutedEventArgs) Handles DownloadWutangrza.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/fw/cfw/3.55/Wutangrza%203.55%20CFW/PS3UPDAT.PUP") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadREBUGDRex484_Click(sender As Object, e As RoutedEventArgs) Handles DownloadREBUGDRex484.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/fw/cfw/4.84/REBUG%20D-REX%20EDITION%204.84.2%20CFW.7z") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadREBUGRex484_Click(sender As Object, e As RoutedEventArgs) Handles DownloadREBUGRex484.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/fw/cfw/4.84/REBUG%20REX%20EDITION%204.84.2%20CFW.7z") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadEvilnatCobra490Cex_Click(sender As Object, e As RoutedEventArgs) Handles DownloadEvilnatCobra490Cex.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/fw/cfw/4.90/CFW%204.90%20Evilnat%20Cobra%208.4%20[CEX].rar") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadEvilnatCobra490Dex_Click(sender As Object, e As RoutedEventArgs) Handles DownloadEvilnatCobra490Dex.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/fw/cfw/4.90/CFW%204.90%20Evilnat%20Cobra%208.4%20[DEX].rar") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

#End Region

#Region "Emulators"

    Private Sub DownloadRetroArchCommunity_Click(sender As Object, e As RoutedEventArgs) Handles DownloadRetroArchCommunity.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("http://X.X.X.X/ps3/emu/RetroArch_Psx-Place_Community_Edition_unofficial_beta-20220315.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

#End Region

#End Region

End Class
