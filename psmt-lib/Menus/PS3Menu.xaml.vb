﻿Imports System.IO
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Forms
Imports Microsoft.Web.WebView2.Core
Imports psmt_lib.INI

Public Class PS3Menu

    Private WithEvents PS3NetSrvProcess As Process = Nothing
    Private IswebMANMODWebViewReady As Boolean = False
    Private IswebMANMODCommandExecuted As Boolean = False

    Public SharedConsoleAddress As String = ""
    Public Shared ReadOnly IPChangedEvent As RoutedEvent = EventManager.RegisterRoutedEvent(name:="ConsoleAddressChanged", routingStrategy:=RoutingStrategy.Bubble, handlerType:=GetType(RoutedEventHandler), ownerType:=GetType(PS3Menu))

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

    Private Sub PS3Menu_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        'Load config if exists
        If File.Exists(My.Computer.FileSystem.CurrentDirectory + "\psmt-config.ini") Then
            Try
                Dim MainConfig As New IniFile(My.Computer.FileSystem.CurrentDirectory + "\psmt-config.ini")
                SharedConsoleAddress = MainConfig.IniReadValue("PS3 Tools", "IP") + ":" + MainConfig.IniReadValue("PS3 Tools", "Port")
                FTPIPTextBox.Text = MainConfig.IniReadValue("PS3 Tools", "IP") + ":" + MainConfig.IniReadValue("PS3 Tools", "Port")
            Catch ex As FileNotFoundException
                MsgBox("Could not find a valid config file.", MsgBoxStyle.Exclamation)
            End Try
        End If
    End Sub

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
        Dim NewCoreOSTools As New PS3CoreOSTools() With {.ShowActivated = True}
        NewCoreOSTools.Show()
    End Sub

    Private Sub OpenFixTar_Click(sender As Object, e As RoutedEventArgs) Handles OpenFixTar.Click
        Dim NewFixTar As New PS3FixTar() With {.ShowActivated = True}
        NewFixTar.Show()
    End Sub

    Private Sub OpenPUPUnpacker_Click(sender As Object, e As RoutedEventArgs) Handles OpenPUPUnpacker.Click
        Dim NewPUPUnpacker As New PS3PUPUnpacker() With {.ShowActivated = True}
        NewPUPUnpacker.Show()
    End Sub

    Private Sub OpenRCODumper_Click(sender As Object, e As RoutedEventArgs) Handles OpenRCODumper.Click
        Dim NewRCODumper As New PS3RCODumper() With {.ShowActivated = True}
        NewRCODumper.Show()
    End Sub

    Private Sub OpenSELFReader_Click(sender As Object, e As RoutedEventArgs) Handles OpenSELFReader.Click
        Dim NewSELFReader As New PS3ReadSELF() With {.ShowActivated = True}
        NewSELFReader.Show()
    End Sub

    Private Sub OpenFTPBrowser_Click(sender As Object, e As RoutedEventArgs) Handles OpenFTPBrowser.Click
        Dim NewFTPBrowser As New FTPBrowser() With {.ShowActivated = True}
        NewFTPBrowser.Show()
    End Sub

    Private Sub OpenPKGExtractor_Click(sender As Object, e As RoutedEventArgs) Handles OpenPKGExtractor.Click
        Dim NewPKGExtractor As New PS3PKGExtractor() With {.ShowActivated = True}
        NewPKGExtractor.Show()
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

#Region "webMAN MOD Downloads"

    Private Sub DownloadCoversPackPS3_Click(sender As Object, e As RoutedEventArgs) Handles DownloadCoversPackPS3.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/aldostools/Resources/releases/download/1.0/EP0001-BLES80608_00-COVERS0000000000.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadCoversPackPSXPS2_Click(sender As Object, e As RoutedEventArgs) Handles DownloadCoversPackPSXPS2.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/aldostools/Resources/releases/download/1.0/EP0001-BLES80608_00-COVERS00000RETRO.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadNetsrv_Click(sender As Object, e As RoutedEventArgs) Handles DownloadNetsrv.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/aldostools/webMAN-MOD/releases/download/1.47.46/ps3netsrv_20240210.zip") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadPrepIso_Click(sender As Object, e As RoutedEventArgs) Handles DownloadPrepIso.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/aldostools/webMAN-MOD/releases/download/1.47.46/prepISO_1.33.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadPS2ClassicsLauncher_Click(sender As Object, e As RoutedEventArgs) Handles DownloadPS2ClassicsLauncher.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/aldostools/webMAN-MOD/releases/download/1.47.46/PS2_Classics_Launcher.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadPS2Config_Click(sender As Object, e As RoutedEventArgs) Handles DownloadPS2Config.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/aldostools/webMAN-MOD/releases/download/1.47.46/PS2CONFIG.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadPSPMinisLauncher_Click(sender As Object, e As RoutedEventArgs) Handles DownloadPSPMinisLauncher.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/aldostools/webMAN-MOD/releases/download/1.47.46/PSP_Minis_Launcher.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadPSPRemastersLauncher_Click(sender As Object, e As RoutedEventArgs) Handles DownloadPSPRemastersLauncher.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/aldostools/webMAN-MOD/releases/download/1.47.46/PSP_Remasters_Launcher.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadWebManMod_Click(sender As Object, e As RoutedEventArgs) Handles DownloadWebManMod.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/aldostools/webMAN-MOD/releases/download/1.47.46/webMAN_MOD_1.47.46_Installer.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadColorfulWMTheme_Click(sender As Object, e As RoutedEventArgs) Handles DownloadColorfulWMTheme.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/aldostools/Resources/releases/download/Themes/wm_theme_colorful.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadFlowerificationWMTheme_Click(sender As Object, e As RoutedEventArgs) Handles DownloadFlowerificationWMTheme.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/aldostools/Resources/releases/download/Themes/wm_theme_flowerification.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadMetalificationWMTheme_Click(sender As Object, e As RoutedEventArgs) Handles DownloadMetalificationWMTheme.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/aldostools/Resources/releases/download/Themes/wm_theme_metalification.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadRebugificationWMTheme_Click(sender As Object, e As RoutedEventArgs) Handles DownloadRebugificationWMTheme.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/aldostools/Resources/releases/download/Themes/wm_theme_rebugification.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

    Private Sub DownloadStandardWMTheme_Click(sender As Object, e As RoutedEventArgs) Handles DownloadStandardWMTheme.Click
        Dim NewDownloader As New Downloader() With {.ShowActivated = True}
        NewDownloader.Show()
        If NewDownloader.CreateNewDownload("https://github.com/aldostools/Resources/releases/download/Themes/wm_theme_standard.pkg") = False Then
            MsgBox("Could not download the selected file.", MsgBoxStyle.Critical)
            NewDownloader.Close()
        End If
    End Sub

#End Region

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

#Region "webMAN MOD Tools"

    Public Sub NavigateTowebMANMODUrl(InputURL As String)
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            If IswebMANMODWebViewReady AndAlso IswebMANMODCommandExecuted Then
                IswebMANMODCommandExecuted = False
                WebMANWebView.CoreWebView2.Navigate(InputURL)
            End If
        End If
    End Sub

    Private Sub EjectDisc_Click(sender As Object, e As RoutedEventArgs) Handles EjectDisc.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            If IswebMANMODWebViewReady AndAlso IswebMANMODCommandExecuted Then
                IswebMANMODCommandExecuted = False
                WebMANWebView.CoreWebView2.Navigate("http://" & SharedConsoleAddress.Split(":"c)(0) & "/eject.ps3")
            Else
                MsgBox("Please wait a couple seconds until WebView2 is ready.", MsgBoxStyle.Information, "webMAN MOD not ready")
            End If
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub ExitToXMB_Click(sender As Object, e As RoutedEventArgs) Handles ExitToXMB.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            If IswebMANMODWebViewReady AndAlso IswebMANMODCommandExecuted Then
                IswebMANMODCommandExecuted = False
                WebMANWebView.CoreWebView2.Navigate("http://" & SharedConsoleAddress.Split(":"c)(0) & "/xmb.ps3$exit")
            Else
                MsgBox("Please wait a couple seconds until WebView2 is ready.", MsgBoxStyle.Information, "webMAN MOD not ready")
            End If
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub DownloadPKGFromURLToPS3_Click(sender As Object, e As RoutedEventArgs) Handles DownloadPKGFromURLToPS3.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            If IswebMANMODWebViewReady AndAlso IswebMANMODCommandExecuted Then
                Dim NewInputDialog As New InputDialog With {
                    .Title = "Download a PKG to the PS3",
                    .NewValueTextBox_Text = "",
                    .InputDialogTitleTextBlock_Text = "Enter the PKG URL :",
                    .ConfirmButton_Text = "Download"
                }

                If NewInputDialog.ShowDialog() = True Then
                    Dim InputDialogResult As String = NewInputDialog.NewValueTextBox_Text
                    IswebMANMODCommandExecuted = False
                    WebMANWebView.CoreWebView2.Navigate("http://" & SharedConsoleAddress.Split(":"c)(0) & "/xmb.ps3/download.ps3?url=" + InputDialogResult)
                End If
            Else
                MsgBox("Please wait a couple seconds until WebView2 is ready.", MsgBoxStyle.Information, "webMAN MOD not ready")
            End If
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub DownloadFileFromURLToPS3_Click(sender As Object, e As RoutedEventArgs) Handles DownloadFileFromURLToPS3.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            If IswebMANMODWebViewReady AndAlso IswebMANMODCommandExecuted Then
                Dim NewURLInputDialog As New InputDialog With {
                    .Title = "Download a file to the PS3",
                    .NewValueTextBox_Text = "",
                    .InputDialogTitleTextBlock_Text = "Enter the file URL :",
                    .ConfirmButton_Text = "Download"
                }

                Dim NewDestinationInputDialog As New InputDialog With {
                    .Title = "Download Destination",
                    .NewValueTextBox_Text = "/dev_hdd0/FOLDER/FILENAME.EXTENSION",
                    .InputDialogTitleTextBlock_Text = "Enter the destination path :",
                    .ConfirmButton_Text = "Confirm"
                }

                If NewURLInputDialog.ShowDialog() = True AndAlso NewDestinationInputDialog.ShowDialog() = True Then
                    Dim NewURLInputDialogResult As String = NewURLInputDialog.NewValueTextBox_Text
                    Dim NewDestinationInputDialogResult As String = NewDestinationInputDialog.NewValueTextBox_Text

                    IswebMANMODCommandExecuted = False
                    WebMANWebView.CoreWebView2.Navigate("http://" & SharedConsoleAddress.Split(":"c)(0) & "/xmb.ps3/download.ps3?to=" + NewDestinationInputDialogResult + "&url=" + NewURLInputDialogResult)
                End If
            Else
                MsgBox("Please wait a couple seconds until WebView2 is ready.", MsgBoxStyle.Information, "webMAN MOD not ready")
            End If
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub DownloadAndInstallPKGFromURL_Click(sender As Object, e As RoutedEventArgs) Handles DownloadAndInstallPKGFromURL.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            If IswebMANMODWebViewReady AndAlso IswebMANMODCommandExecuted Then
                Dim NewInputDialog As New InputDialog With {
                    .Title = "Download & Install a PKG on the PS3",
                    .NewValueTextBox_Text = "",
                    .InputDialogTitleTextBlock_Text = "Enter the PKG URL :",
                    .ConfirmButton_Text = "Install"
                }

                If NewInputDialog.ShowDialog() = True Then
                    Dim InputDialogResult As String = NewInputDialog.NewValueTextBox_Text
                    IswebMANMODCommandExecuted = False
                    WebMANWebView.CoreWebView2.Navigate("http://" & SharedConsoleAddress.Split(":"c)(0) & "/xmb.ps3/install.ps3?url=" + InputDialogResult)
                End If
            Else
                MsgBox("Please wait a couple seconds until WebView2 for webMAN MOD is ready.", MsgBoxStyle.Information, "webMAN MOD not ready")
            End If
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub HardRebootPS3_Click(sender As Object, e As RoutedEventArgs) Handles HardRebootPS3.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            If IswebMANMODWebViewReady AndAlso IswebMANMODCommandExecuted Then
                IswebMANMODCommandExecuted = False
                WebMANWebView.CoreWebView2.Navigate("http://" & SharedConsoleAddress.Split(":"c)(0) & "/reboot.ps3")
            Else
                MsgBox("Please wait a couple seconds until WebView2 is ready.", MsgBoxStyle.Information, "webMAN MOD not ready")
            End If
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub InsertDisc_Click(sender As Object, e As RoutedEventArgs) Handles InsertDisc.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            If IswebMANMODWebViewReady AndAlso IswebMANMODCommandExecuted Then
                IswebMANMODCommandExecuted = False
                WebMANWebView.CoreWebView2.Navigate("http://" & SharedConsoleAddress.Split(":"c)(0) & "/insert.ps3")
            Else
                MsgBox("Please wait a couple seconds until WebView2 is ready.", MsgBoxStyle.Information, "webMAN MOD not ready")
            End If
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub InstallPKGFromPS3HDD_Click(sender As Object, e As RoutedEventArgs) Handles InstallPKGFromPS3HDD.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            If IswebMANMODWebViewReady AndAlso IswebMANMODCommandExecuted Then
                Dim NewInputDialog As New InputDialog With {
                    .Title = "Install a PKG",
                    .NewValueTextBox_Text = "/dev_hdd0/packages/Homebrew.pkg",
                    .InputDialogTitleTextBlock_Text = "Enter the full path to the .pkg file:",
                    .ConfirmButton_Text = "Install"
                }

                If NewInputDialog.ShowDialog() = True Then
                    Dim InputDialogResult As String = NewInputDialog.NewValueTextBox_Text
                    IswebMANMODCommandExecuted = False
                    WebMANWebView.CoreWebView2.Navigate("http://" & SharedConsoleAddress.Split(":"c)(0) & "/install_ps3" + InputDialogResult)
                End If
            Else
                MsgBox("Please wait a couple seconds until WebView2 is ready.", MsgBoxStyle.Information, "webMAN MOD not ready")
            End If
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub InstallThemeFromPS3HDD_Click(sender As Object, e As RoutedEventArgs) Handles InstallThemeFromPS3HDD.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            If IswebMANMODWebViewReady AndAlso IswebMANMODCommandExecuted Then
                Dim NewInputDialog As New InputDialog With {
                    .Title = "Install a Theme",
                    .NewValueTextBox_Text = "/dev_hdd0/Themes/THEME.p3t",
                    .InputDialogTitleTextBlock_Text = "Enter the full path to the .p3t file:",
                    .ConfirmButton_Text = "Install"
                }

                If NewInputDialog.ShowDialog() = True Then
                    Dim InputDialogResult As String = NewInputDialog.NewValueTextBox_Text
                    IswebMANMODCommandExecuted = False
                    WebMANWebView.CoreWebView2.Navigate("http://" & SharedConsoleAddress.Split(":"c)(0) & "/install.ps3" + InputDialogResult)
                End If
            Else
                MsgBox("Please wait a couple seconds until WebView2 is ready.", MsgBoxStyle.Information, "webMAN MOD not ready")
            End If
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub OpenPS3WebBrowserURL_Click(sender As Object, e As RoutedEventArgs) Handles OpenPS3WebBrowserURL.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            If IswebMANMODWebViewReady AndAlso IswebMANMODCommandExecuted Then

                Dim NewInputDialog As New InputDialog With {
                    .Title = "Open PS3 Web Browser with URL",
                    .NewValueTextBox_Text = "",
                    .InputDialogTitleTextBlock_Text = "Enter an URL to browse :",
                    .ConfirmButton_Text = "Open"
                }

                If NewInputDialog.ShowDialog() = True Then
                    IswebMANMODCommandExecuted = False
                    Dim InputDialogResult As String = NewInputDialog.NewValueTextBox_Text
                    WebMANWebView.CoreWebView2.Navigate("http://" & SharedConsoleAddress.Split(":"c)(0) & "/browser.ps3?" + InputDialogResult)
                End If

            Else
                MsgBox("Please wait a couple seconds until WebView2 is ready.", MsgBoxStyle.Information, "webMAN MOD not ready")
            End If
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub OpenWebGUICelcius_Click(sender As Object, e As RoutedEventArgs) Handles OpenWebGUICelcius.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            Dim NewwebMANMODWebGUI As New PS3webMANBrowser() With {.ShowActivated = True, .WebMANAddress = "http://" & SharedConsoleAddress.Split(":"c)(0) & "/tempc.html"}
            NewwebMANMODWebGUI.Show()
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub OpenWebGUIFahrenheit_Click(sender As Object, e As RoutedEventArgs) Handles OpenWebGUIFahrenheit.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            Dim NewwebMANMODWebGUI As New PS3webMANBrowser() With {.ShowActivated = True, .WebMANAddress = "http://" & SharedConsoleAddress.Split(":"c)(0) & "/tempf.html"}
            NewwebMANMODWebGUI.Show()
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub OpenwebMANMODWebGUI_Click(sender As Object, e As RoutedEventArgs) Handles OpenwebMANMODWebGUI.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            Dim NewwebMANMODWebGUI As New PS3webMANBrowser() With {.ShowActivated = True, .WebMANAddress = "http://" & SharedConsoleAddress.Split(":"c)(0)}
            NewwebMANMODWebGUI.Show()
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub PlayDiscMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles PlayDiscMenuItem.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            If IswebMANMODWebViewReady AndAlso IswebMANMODCommandExecuted Then
                IswebMANMODCommandExecuted = False
                WebMANWebView.CoreWebView2.Navigate("http://" & SharedConsoleAddress.Split(":"c)(0) & "/play.ps3")
            Else
                MsgBox("Please wait a couple seconds until WebView2 is ready.", MsgBoxStyle.Information, "webMAN MOD not ready")
            End If
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub QuickRebootPS3_Click(sender As Object, e As RoutedEventArgs) Handles QuickRebootPS3.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            If IswebMANMODWebViewReady AndAlso IswebMANMODCommandExecuted Then
                IswebMANMODCommandExecuted = False
                WebMANWebView.CoreWebView2.Navigate("http://" & SharedConsoleAddress.Split(":"c)(0) & "/reboot.ps3?quick")
            Else
                MsgBox("Please wait a couple seconds until WebView2 is ready.", MsgBoxStyle.Information, "webMAN MOD not ready")
            End If
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub RebootPS3UsingVSH_Click(sender As Object, e As RoutedEventArgs) Handles RebootPS3UsingVSH.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            If IswebMANMODWebViewReady AndAlso IswebMANMODCommandExecuted Then
                IswebMANMODCommandExecuted = False
                WebMANWebView.CoreWebView2.Navigate("http://" & SharedConsoleAddress.Split(":"c)(0) & "/reboot.ps3?vsh")
            Else
                MsgBox("Please wait a couple seconds until WebView2 is ready.", MsgBoxStyle.Information, "webMAN MOD not ready")
            End If
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub ReloadGame_Click(sender As Object, e As RoutedEventArgs) Handles ReloadGame.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            If IswebMANMODWebViewReady AndAlso IswebMANMODCommandExecuted Then
                IswebMANMODCommandExecuted = False
                WebMANWebView.CoreWebView2.Navigate("http://" & SharedConsoleAddress.Split(":"c)(0) & "/xmb.ps3$reloadgame")
            Else
                MsgBox("Please wait a couple seconds until WebView2 is ready.", MsgBoxStyle.Information, "webMAN MOD not ready")
            End If
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub RescanGamesRefreshXML_Click(sender As Object, e As RoutedEventArgs) Handles RescanGamesRefreshXML.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            If IswebMANMODWebViewReady AndAlso IswebMANMODCommandExecuted Then
                IswebMANMODCommandExecuted = False
                WebMANWebView.CoreWebView2.Navigate("http://" & SharedConsoleAddress.Split(":"c)(0) & "/refresh.ps3?xmb")
            Else
                MsgBox("Please wait a couple seconds until WebView2 is ready.", MsgBoxStyle.Information, "webMAN MOD not ready")
            End If
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub RestartPS3_Click(sender As Object, e As RoutedEventArgs) Handles RestartPS3.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            If IswebMANMODWebViewReady AndAlso IswebMANMODCommandExecuted Then
                IswebMANMODCommandExecuted = False
                WebMANWebView.CoreWebView2.Navigate("http://" & SharedConsoleAddress.Split(":"c)(0) & "/restart.ps3")
            Else
                MsgBox("Please wait a couple seconds until WebView2 is ready.", MsgBoxStyle.Information, "webMAN MOD not ready")
            End If
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub RestartPS3ShowMinVersion_Click(sender As Object, e As RoutedEventArgs) Handles RestartPS3ShowMinVersion.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            If IswebMANMODWebViewReady AndAlso IswebMANMODCommandExecuted Then
                IswebMANMODCommandExecuted = False
                WebMANWebView.CoreWebView2.Navigate("http://" & SharedConsoleAddress.Split(":"c)(0) & "/restart.ps3?min")
            Else
                MsgBox("Please wait a couple seconds until WebView2 is ready.", MsgBoxStyle.Information, "webMAN MOD not ready")
            End If
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub RestartPS3WithContentScan_Click(sender As Object, e As RoutedEventArgs) Handles RestartPS3WithContentScan.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            If IswebMANMODWebViewReady AndAlso IswebMANMODCommandExecuted Then
                IswebMANMODCommandExecuted = False
                WebMANWebView.CoreWebView2.Navigate("http://" & SharedConsoleAddress.Split(":"c)(0) & "/restart.ps3?0")
            Else
                MsgBox("Please wait a couple seconds until WebView2 is ready.", MsgBoxStyle.Information, "webMAN MOD not ready")
            End If
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub ShowSystemInfoOnPS3_Click(sender As Object, e As RoutedEventArgs) Handles ShowSystemInfoOnPS3.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            If IswebMANMODWebViewReady AndAlso IswebMANMODCommandExecuted Then
                IswebMANMODCommandExecuted = False
                WebMANWebView.CoreWebView2.Navigate("http://" & SharedConsoleAddress.Split(":"c)(0) & "/popup.ps3")
            Else
                MsgBox("Please wait a couple seconds until WebView2 is ready.", MsgBoxStyle.Information, "webMAN MOD not ready")
            End If
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub ShutdownPS3_Click(sender As Object, e As RoutedEventArgs) Handles ShutdownPS3.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            If IswebMANMODWebViewReady AndAlso IswebMANMODCommandExecuted Then
                IswebMANMODCommandExecuted = False
                WebMANWebView.CoreWebView2.Navigate("http://" & SharedConsoleAddress.Split(":"c)(0) & "/shutdown.ps3")
            Else
                MsgBox("Please wait a couple seconds until WebView2 is ready.", MsgBoxStyle.Information, "webMAN MOD not ready")
            End If
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub SoftRebootPS3_Click(sender As Object, e As RoutedEventArgs) Handles SoftRebootPS3.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            If IswebMANMODWebViewReady AndAlso IswebMANMODCommandExecuted Then
                IswebMANMODCommandExecuted = False
                WebMANWebView.CoreWebView2.Navigate("http://" & SharedConsoleAddress.Split(":"c)(0) & "/reboot.ps3?soft")
            Else
                MsgBox("Please wait a couple seconds until WebView2 is ready.", MsgBoxStyle.Information, "webMAN MOD not ready")
            End If
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub ToggleInGameBGMusicPlayback_Click(sender As Object, e As RoutedEventArgs) Handles ToggleInGameBGMusicPlayback.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            If IswebMANMODWebViewReady AndAlso IswebMANMODCommandExecuted Then
                IswebMANMODCommandExecuted = False
                WebMANWebView.CoreWebView2.Navigate("http://" & SharedConsoleAddress.Split(":"c)(0) & "/sysbgm.ps3")
            Else
                MsgBox("Please wait a couple seconds until WebView2 is ready.", MsgBoxStyle.Information, "webMAN MOD not ready")
            End If
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub ToggleVideoRecording_Click(sender As Object, e As RoutedEventArgs) Handles ToggleVideoRecording.Click
        If Not String.IsNullOrEmpty(SharedConsoleAddress) Then
            If IswebMANMODWebViewReady AndAlso IswebMANMODCommandExecuted Then
                IswebMANMODCommandExecuted = False
                WebMANWebView.CoreWebView2.Navigate("http://" & SharedConsoleAddress.Split(":"c)(0) & "/videorec.ps3")
            Else
                MsgBox("Please wait a couple seconds until WebView2 is ready.", MsgBoxStyle.Information, "webMAN MOD not ready")
            End If
        Else
            MsgBox("Please set your PS3 IP address in the Settings.", MsgBoxStyle.Information, "No IP Address")
        End If
    End Sub

    Private Sub WebMANWebView_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles webMANWebView.CoreWebView2InitializationCompleted
        If e.IsSuccess Then
            IswebMANMODWebViewReady = True
        End If
    End Sub

    Private Sub WebMANWebView_NavigationCompleted(sender As Object, e As CoreWebView2NavigationCompletedEventArgs) Handles WebMANWebView.NavigationCompleted
        If e.IsSuccess Then
            IswebMANMODCommandExecuted = True
        End If
    End Sub

    Private Sub CreateFolderStructure_Click(sender As Object, e As RoutedEventArgs) Handles CreateFolderStructure.Click
        Dim FBD As New FolderBrowserDialog() With {.RootFolder = Environment.SpecialFolder.Desktop, .Description = "Select a destination path"}
        If FBD.ShowDialog() = Forms.DialogResult.OK Then
            Directory.CreateDirectory(FBD.SelectedPath + "\GAMES")
            Directory.CreateDirectory(FBD.SelectedPath + "\PS3ISO")
            Directory.CreateDirectory(FBD.SelectedPath + "\PSXISO")
            Directory.CreateDirectory(FBD.SelectedPath + "\PS2ISO")
            Directory.CreateDirectory(FBD.SelectedPath + "\PSPISO")
            Directory.CreateDirectory(FBD.SelectedPath + "\BDISO")
            Directory.CreateDirectory(FBD.SelectedPath + "\DVDISO")
            Directory.CreateDirectory(FBD.SelectedPath + "\ROMS")
            Directory.CreateDirectory(FBD.SelectedPath + "\GAMEI")
            Directory.CreateDirectory(FBD.SelectedPath + "\PKG")
            Directory.CreateDirectory(FBD.SelectedPath + "\MOVIES")
            Directory.CreateDirectory(FBD.SelectedPath + "\MUSIC")
            Directory.CreateDirectory(FBD.SelectedPath + "\PICTURE")
            Directory.CreateDirectory(FBD.SelectedPath + "\REDKEY")

            MsgBox("Directories created!", MsgBoxStyle.Information)
        End If
    End Sub

    Private Sub ManageVirtualFolders_Click(sender As Object, e As RoutedEventArgs) Handles ManageVirtualFolders.Click
        Dim NewVirtualFolderManager As New PS3VirtualFolderManager() With {.ShowActivated = True}
        NewVirtualFolderManager.Show()
    End Sub

    Private Sub ShareASingleFolder_Click(sender As Object, e As RoutedEventArgs) Handles ShareASingleFolder.Click
        Select Case ShareASingleFolder.Header.ToString()
            Case "Share a single folder"
                If File.Exists(My.Computer.FileSystem.CurrentDirectory + "\Tools\ps3netsrv\ps3netsrv.exe") Then
                    Dim FBD As New FolderBrowserDialog() With {.RootFolder = Environment.SpecialFolder.Desktop, .Description = "Select the folder you want to share"}
                    If FBD.ShowDialog() = Forms.DialogResult.OK Then
                        If MsgBox(FBD.SelectedPath + " will be shared using ps3netsrv. Continue ?", MsgBoxStyle.YesNo, "Please confirm sharing the selected folder") = MsgBoxResult.Yes Then

                            Dim NewArgs As String = ""
                            If FBD.SelectedPath.Length <= 3 Then
                                NewArgs = FBD.SelectedPath + "\"
                            Else
                                NewArgs = """" + FBD.SelectedPath + """"
                            End If

                            PS3NetSrvProcess = New Process() With {.EnableRaisingEvents = True, .StartInfo = New ProcessStartInfo With {
                                .Arguments = NewArgs,
                                .FileName = My.Computer.FileSystem.CurrentDirectory + "\Tools\ps3netsrv\ps3netsrv.exe"}}

                            PS3NetSrvProcess.Start()

                            If Dispatcher.CheckAccess() = False Then
                                Dispatcher.BeginInvoke(Sub() ShareASingleFolder.Header = "Stop sharing")
                            Else
                                ShareASingleFolder.Header = "Stop sharing"
                            End If

                        Else
                            Exit Sub
                        End If
                    End If
                Else
                    MsgBox("Could not find " + My.Computer.FileSystem.CurrentDirectory + "\Tools\ps3netsrv\ps3netsrv.exe", MsgBoxStyle.Critical, "Cannot share without ps3netsrv")
                End If
            Case "Stop sharing"
                If PS3NetSrvProcess IsNot Nothing Then
                    If PS3NetSrvProcess.HasExited = False Then

                        PS3NetSrvProcess.Kill()

                        If Dispatcher.CheckAccess() = False Then
                            Dispatcher.BeginInvoke(Sub() ShareASingleFolder.Header = "Share a single folder")
                        Else
                            ShareASingleFolder.Header = "Share a single folder"
                        End If

                    End If
                End If
        End Select
    End Sub

    Private Sub ShareManagedFolders_Click(sender As Object, e As RoutedEventArgs) Handles ShareManagedFolders.Click
        Select Case ShareManagedFolders.Header.ToString()
            Case "Share configured managed virtual folders"
                If File.Exists(My.Computer.FileSystem.CurrentDirectory + "\Tools\ps3netsrv\ps3netsrv.exe") Then

                    Directory.SetCurrentDirectory(My.Computer.FileSystem.CurrentDirectory + "\Tools\ps3netsrv")

                    PS3NetSrvProcess = New Process() With {.EnableRaisingEvents = True, .StartInfo = New ProcessStartInfo With {.Arguments = ".", .FileName = "ps3netsrv.exe"}}
                    PS3NetSrvProcess.Start()

                    Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory)

                    If Dispatcher.CheckAccess() = False Then
                        Dispatcher.BeginInvoke(Sub() ShareManagedFolders.Header = "Stop sharing")
                    Else
                        ShareManagedFolders.Header = "Stop sharing"
                    End If
                Else
                    MsgBox("Could not find " + My.Computer.FileSystem.CurrentDirectory + "\Tools\ps3netsrv\ps3netsrv.exe", MsgBoxStyle.Critical, "Cannot share without ps3netsrv")
                End If
            Case "Stop sharing"
                If PS3NetSrvProcess IsNot Nothing Then
                    If PS3NetSrvProcess.HasExited = False Then

                        PS3NetSrvProcess.Kill()

                        If Dispatcher.CheckAccess() = False Then
                            Dispatcher.BeginInvoke(Sub() ShareASingleFolder.Header = "Share a single folder")
                        Else
                            ShareASingleFolder.Header = "Share a single folder"
                        End If

                    End If
                End If
        End Select
    End Sub

    Private Sub PS3NetSrvProcess_Exited(sender As Object, e As EventArgs) Handles PS3NetSrvProcess.Exited
        If Dispatcher.CheckAccess() = False Then
            Dispatcher.BeginInvoke(Sub()
                                       ShareASingleFolder.Header = "Share a single folder"
                                       ShareManagedFolders.Header = "Share configured managed virtual folders"
                                   End Sub)
        Else
            ShareASingleFolder.Header = "Share a single folder"
            ShareManagedFolders.Header = "Share configured managed virtual folders"
        End If

        PS3NetSrvProcess.Dispose()
        PS3NetSrvProcess = Nothing
    End Sub

#End Region

End Class
