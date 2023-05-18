Imports System.ComponentModel
Imports System.Windows
Imports System.Windows.Media
Imports PS4_Tools

Public Class PKGInfo

    Public SelectedPKG As String = String.Empty

    Dim WithEvents PKGWorker As New BackgroundWorker()
    Dim PKGSoundBytes As Byte() = Nothing
    Dim IsSoundPlaying As Boolean = False

    Private Sub PKGInfo_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If Not String.IsNullOrEmpty(SelectedPKG) Then
            PKGWorker.RunWorkerAsync()
        End If
    End Sub

    Private Sub PlayStopButton_Click(sender As Object, e As RoutedEventArgs) Handles PlayStopButton.Click
        If PlayStopButton.Content.ToString = "Play Soundtrack" AndAlso PKGSoundBytes IsNot Nothing Then
            PlayStopButton.Content = "Stop Soundtrack"
            IsSoundPlaying = True
            Utils.PlaySND(PKGSoundBytes)
        Else
            PlayStopButton.Content = "Play Soundtrack"
            IsSoundPlaying = False
            Utils.StopSND()
        End If
    End Sub

    Private Sub PKGWorker_DoWork(sender As Object, e As DoWorkEventArgs) Handles PKGWorker.DoWork

        Dim GamePKG As PKG.SceneRelated.Unprotected_PKG = PKG.SceneRelated.Read_PKG(SelectedPKG)

        If Not String.IsNullOrEmpty(GamePKG.BuildDate) Then
            PKGBuildDateTextBlock.Dispatcher.BeginInvoke(Sub() PKGBuildDateTextBlock.Text = GamePKG.BuildDate)
        End If

        If Not String.IsNullOrEmpty(GamePKG.Firmware_Version) Then
            PKGFirmwareVersionTextBlock.Dispatcher.BeginInvoke(Sub() PKGFirmwareVersionTextBlock.Text = GamePKG.Firmware_Version)
        End If

        If Not String.IsNullOrEmpty(GamePKG.Size) Then
            PKGSizeTextBlock.Dispatcher.BeginInvoke(Sub() PKGSizeTextBlock.Text = GamePKG.Size)
        End If

        If Not String.IsNullOrEmpty(GamePKG.Content_ID) Then
            PKGContentIDTextBlock.Dispatcher.BeginInvoke(Sub() PKGContentIDTextBlock.Text = GamePKG.Content_ID)
        Else
            If Not String.IsNullOrEmpty(GamePKG.Param.ContentID) Then
                PKGContentIDTextBlock.Dispatcher.BeginInvoke(Sub() PKGContentIDTextBlock.Text = GamePKG.Param.ContentID)
            End If
        End If

        If Not String.IsNullOrEmpty(GamePKG.Region) Then
            PKGRegionTextBlock.Dispatcher.BeginInvoke(Sub() PKGRegionTextBlock.Text = GamePKG.Region)
        End If

        If GamePKG.Sound IsNot Nothing AndAlso GamePKG.Sound.Length > 0 Then
            Dispatcher.BeginInvoke(Sub() PKGSoundBytes = Media.Atrac9.LoadAt9(GamePKG.Sound))
            PlayStopButton.Dispatcher.BeginInvoke(Sub() PlayStopButton.IsEnabled = True)
        End If
        If GamePKG.Background IsNot Nothing AndAlso GamePKG.Background.Length > 0 Then
            Dispatcher.BeginInvoke(Sub() Background = New ImageBrush(Utils.BitmapSourceFromByteArray(GamePKG.Background)))
        End If
        If GamePKG.Icon IsNot Nothing AndAlso GamePKG.Icon.Length > 0 Then
            GameIcon.Dispatcher.BeginInvoke(Sub() GameIcon.Source = Utils.BitmapSourceFromByteArray(GamePKG.Icon))
        End If
        If GamePKG.Image IsNot Nothing AndAlso GamePKG.Image.Length > 0 Then
            GameImage.Dispatcher.BeginInvoke(Sub() GameImage.Source = Utils.BitmapSourceFromByteArray(GamePKG.Image))
        End If

        Select Case GamePKG.PKGState
            Case PKG.SceneRelated.PKG_State.Official
                PKGStateTextBlock.Dispatcher.BeginInvoke(Sub() PKGStateTextBlock.Text = "Official")
            Case PKG.SceneRelated.PKG_State.Officail_DP
                PKGStateTextBlock.Dispatcher.BeginInvoke(Sub() PKGStateTextBlock.Text = "Official DP")
            Case PKG.SceneRelated.PKG_State.Fake
                PKGStateTextBlock.Dispatcher.BeginInvoke(Sub() PKGStateTextBlock.Text = "Fake")
            Case PKG.SceneRelated.PKG_State.Unkown
                PKGStateTextBlock.Dispatcher.BeginInvoke(Sub() PKGStateTextBlock.Text = "Unknown")
            Case Else
                PKGStateTextBlock.Dispatcher.BeginInvoke(Sub() PKGStateTextBlock.Text = "Unknown")
        End Select

        Select Case GamePKG.PKG_Type
            Case PKG.SceneRelated.PKGType.Addon_Theme
                PKGTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGTypeTextBlock.Text = "Theme")
            Case PKG.SceneRelated.PKGType.App
                PKGTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGTypeTextBlock.Text = "Application")
            Case PKG.SceneRelated.PKGType.Game
                PKGTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGTypeTextBlock.Text = "Game")
            Case PKG.SceneRelated.PKGType.Patch
                PKGTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGTypeTextBlock.Text = "Patch")
            Case PKG.SceneRelated.PKGType.Unknown
                PKGTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGTypeTextBlock.Text = "Unknown")
            Case Else
                PKGTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGTypeTextBlock.Text = "Unknown")
        End Select

        Select Case GamePKG.Param.PlaystationVersion
            Case Param_SFO.PARAM_SFO.Playstation.ps3
                PKGConsoleTextBlock.Dispatcher.BeginInvoke(Sub() PKGConsoleTextBlock.Text = "PS3")
            Case Param_SFO.PARAM_SFO.Playstation.ps4
                PKGConsoleTextBlock.Dispatcher.BeginInvoke(Sub() PKGConsoleTextBlock.Text = "PS4")
            Case Param_SFO.PARAM_SFO.Playstation.psp
                PKGConsoleTextBlock.Dispatcher.BeginInvoke(Sub() PKGConsoleTextBlock.Text = "PSP")
            Case Param_SFO.PARAM_SFO.Playstation.psvita
                PKGConsoleTextBlock.Dispatcher.BeginInvoke(Sub() PKGConsoleTextBlock.Text = "PS Vita")
            Case Param_SFO.PARAM_SFO.Playstation.unknown
                PKGConsoleTextBlock.Dispatcher.BeginInvoke(Sub() PKGConsoleTextBlock.Text = "Unknown")
            Case Else
                PKGConsoleTextBlock.Dispatcher.BeginInvoke(Sub() PKGConsoleTextBlock.Text = "Unknown")
        End Select

        If GamePKG.Param IsNot Nothing Then
            LoadParamSFO(GamePKG.Param)
        End If

    End Sub

    Private Sub LoadParamSFO(Param As Param_SFO.PARAM_SFO)

        PKGAttributesTextBlock.Dispatcher.BeginInvoke(Sub() PKGAttributesTextBlock.Text = Param.Attribute)
        PKGAppVerTextBlock.Dispatcher.BeginInvoke(Sub() PKGAppVerTextBlock.Text = Param.APP_VER)
        PKGCategoryTextBlock.Dispatcher.BeginInvoke(Sub() PKGCategoryTextBlock.Text = GetCategory(Param.Category))
        PKGTitleTextBlock.Dispatcher.BeginInvoke(Sub() PKGTitleTextBlock.Text = Param.Title)

        Select Case Param.DataType
            Case Param_SFO.PARAM_SFO.DataTypes.DiscGame
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "Disc Game")
            Case Param_SFO.PARAM_SFO.DataTypes.Additional_Content
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "Additional Content")
            Case Param_SFO.PARAM_SFO.DataTypes.AppleTV
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "AppleTV")
            Case Param_SFO.PARAM_SFO.DataTypes.AppMusic
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "Music App")
            Case Param_SFO.PARAM_SFO.DataTypes.AppPhoto
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "Photo App")
            Case Param_SFO.PARAM_SFO.DataTypes.AppVideo
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "Video App")
            Case Param_SFO.PARAM_SFO.DataTypes.AutoInstallRoot
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "AutoInstall Root")
            Case Param_SFO.PARAM_SFO.DataTypes.Blu_Ray_Disc
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "Blu Ray Disc")
            Case Param_SFO.PARAM_SFO.DataTypes.BroadCastVideo
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "Broadcast Video")
            Case Param_SFO.PARAM_SFO.DataTypes.CellBE
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "CellBE")
            Case Param_SFO.PARAM_SFO.DataTypes.DiscGame
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "Disc Game")
            Case Param_SFO.PARAM_SFO.DataTypes.DiscMovie
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "Disc Movie")
            Case Param_SFO.PARAM_SFO.DataTypes.DiscPackage
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "Disc Package")
            Case Param_SFO.PARAM_SFO.DataTypes.ExtraRoot
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "Extra Root")
            Case Param_SFO.PARAM_SFO.DataTypes.GameContent
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "Game Content")
            Case Param_SFO.PARAM_SFO.DataTypes.GameData
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "Game Data")
            Case Param_SFO.PARAM_SFO.DataTypes.Game_Digital_Application
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "Game Digital Application")
            Case Param_SFO.PARAM_SFO.DataTypes.GDE
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "GDE")
            Case Param_SFO.PARAM_SFO.DataTypes.HDDGame
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "HDD Game")
            Case Param_SFO.PARAM_SFO.DataTypes.Home
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "Home")
            Case Param_SFO.PARAM_SFO.DataTypes.None
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "None")
            Case Param_SFO.PARAM_SFO.DataTypes.PS4_Game_Application_Patch
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "Game Application Patch")
            Case Param_SFO.PARAM_SFO.DataTypes.PSN_Game
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "PSN Game")
            Case Param_SFO.PARAM_SFO.DataTypes.SaveData
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "Save Data")
            Case Param_SFO.PARAM_SFO.DataTypes.StoreFronted
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "Store Fronted")
            Case Param_SFO.PARAM_SFO.DataTypes.ThemeRoot
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "Theme Root")
            Case Param_SFO.PARAM_SFO.DataTypes.VideoRoot
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "Video Root")
            Case Param_SFO.PARAM_SFO.DataTypes.WebTV
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "WebTV")
            Case Else
                PKGDataTypeTextBlock.Dispatcher.BeginInvoke(Sub() PKGDataTypeTextBlock.Text = "Unknown")
        End Select

        For Each TableEntry As Param_SFO.PARAM_SFO.Table In Param.Tables.ToList()
            If TableEntry.Name = "TITLE_ID" Then
                PKGTitleDTextBlock.Dispatcher.BeginInvoke(Sub() PKGTitleDTextBlock.Text = TableEntry.Value.Trim())
            End If
            If TableEntry.Name = "VERSION" Then
                PKGVersionTextBlock.Dispatcher.BeginInvoke(Sub() PKGVersionTextBlock.Text = TableEntry.Value.Trim())
            End If
        Next

    End Sub

    Public Shared Function GetCategory(SFOCategory As String) As String
        Select Case SFOCategory
            Case "ac"
                Return "Additional Content"
            Case "bd"
                Return "Blu-ray Disc"
            Case "gc"
                Return "Game Content"
            Case "gd"
                Return "Game Digital Application"
            Case "gda"
                Return "System Application"
            Case "gdb"
                Return "Unknown"
            Case "gdc"
                Return "Non-Game Big Application"
            Case "gdd"
                Return "BG Application"
            Case "gde"
                Return "Non-Game Mini App / Video Service Native App"
            Case "gdk"
                Return "Video Service Web App"
            Case "gdl"
                Return "PS Cloud Beta App"
            Case "gdO"
                Return "PS2 Classic"
            Case "gp"
                Return "Game Application Patch"
            Case "gpc"
                Return "Non-Game Big App Patch"
            Case "gpd"
                Return "BG Application patch"
            Case "gpe"
                Return "Non-Game Mini App Patch / Video Service Native App Patch"
            Case "gpk"
                Return "Video Service Web App Patch"
            Case "gpl"
                Return "PS Cloud Beta App Patch"
            Case "sd"
                Return "Save Data"
            Case "la"
                Return "Live Area"
            Case "wda"
                Return "Unknown"
            Case Else
                Return "Unknown"
        End Select
    End Function

End Class
