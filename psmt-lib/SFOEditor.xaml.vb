Imports System.Windows
Imports System.Text
Imports System.Windows.Forms

Public Class SFOEditor

    Dim WithEvents LPCM71 As New Controls.CheckBox With {.Content = "7.1 Linear pulse-code modulation (LPCM)", .IsChecked = False}
    Dim WithEvents LPCM51 As New Controls.CheckBox With {.Content = "5.1 Linear pulse-code modulation (LPCM)", .IsChecked = False}
    Dim WithEvents LPCM20 As New Controls.CheckBox With {.Content = "2.0 Linear pulse-code modulation (LPCM)", .IsChecked = False}
    Dim WithEvents DTS As New Controls.CheckBox With {.Content = "DTS", .IsChecked = False}
    Dim WithEvents DolbyDigital As New Controls.CheckBox With {.Content = "Dolby Digital", .IsChecked = False}

    Dim WithEvents FULLHD As New Controls.CheckBox With {.Content = "1080p", .IsChecked = False}
    Dim WithEvents HD As New Controls.CheckBox With {.Content = "720p", .IsChecked = False}
    Dim WithEvents SD169 As New Controls.CheckBox With {.Content = "576p (16:9)", .IsChecked = False}
    Dim WithEvents SD As New Controls.CheckBox With {.Content = "576p", .IsChecked = False}
    Dim WithEvents LOWRES169 As New Controls.CheckBox With {.Content = "480p (16:9)", .IsChecked = False}
    Dim WithEvents LOWRES As New Controls.CheckBox With {.Content = "480p", .IsChecked = False}

    Public CurrentSFO As Param_SFO.PARAM_SFO
    Public CurrentSFOFilePath As String

    Public Structure ParamListViewItem
        Private _ParamName As String
        Private _ParamValue As String

        Public Property ParamName As String
            Get
                Return _ParamName
            End Get
            Set
                _ParamName = Value
            End Set
        End Property

        Public Property ParamValue As String
            Get
                Return _ParamValue
            End Get
            Set
                _ParamValue = Value
            End Set
        End Property
    End Structure

    Private Sub AddAudioFormats()
        AudioFormatsListBox.Items.Add(LPCM71)
        AudioFormatsListBox.Items.Add(LPCM51)
        AudioFormatsListBox.Items.Add(LPCM20)
        AudioFormatsListBox.Items.Add(DTS)
        AudioFormatsListBox.Items.Add(DolbyDigital)
    End Sub

    Private Sub AddVideoFormats()
        VideoFormatsListBox.Items.Add(FULLHD)
        VideoFormatsListBox.Items.Add(HD)
        VideoFormatsListBox.Items.Add(SD169)
        VideoFormatsListBox.Items.Add(SD)
        VideoFormatsListBox.Items.Add(LOWRES169)
        VideoFormatsListBox.Items.Add(LOWRES)
    End Sub

    Private Sub SFOEditor_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        AddAudioFormats()
        AddVideoFormats()
    End Sub

    Public Sub OpenPARAMSFO(NewParamSFO As Param_SFO.PARAM_SFO, Optional SFOFile As String = "")
        'Clear tabs
        ConsolesTabControl.Items.Clear()

        'Load selected PARAM.SFO
        CurrentSFOFilePath = SFOFile
        Dim CurrentSFOFile As Param_SFO.PARAM_SFO
        If NewParamSFO IsNot Nothing And SFOFile = "" Then
            CurrentSFOFile = NewParamSFO
        ElseIf NewParamSFO Is Nothing And Not SFOFile = "" Then
            CurrentSFOFile = New Param_SFO.PARAM_SFO(SFOFile)
        Else
            MsgBox("Could not open the selected PARAM.SFO file.", MsgBoxStyle.Exclamation)
            Exit Sub
        End If

        Dim SFOVer = CurrentSFOFile.PlaystationVersion
        'Display the correct tab
        If SFOVer = Param_SFO.PARAM_SFO.Playstation.ps3 Then
            ConsolesTabControl.Items.Add(PS3TabItem)
            ConsolesTabControl.SelectedItem = PS3TabItem
        ElseIf SFOVer = Param_SFO.PARAM_SFO.Playstation.ps4 Then
            ConsolesTabControl.Items.Add(PS4TabItem)
            ConsolesTabControl.SelectedItem = PS4TabItem
        ElseIf SFOVer = Param_SFO.PARAM_SFO.Playstation.psp Then
            ConsolesTabControl.Items.Add(PSPTabItem)
            ConsolesTabControl.SelectedItem = PSPTabItem
        ElseIf SFOVer = Param_SFO.PARAM_SFO.Playstation.psvita Then
            ConsolesTabControl.Items.Add(PSVTabItem)
            ConsolesTabControl.SelectedItem = PSVTabItem
        End If

        Dim AddedParameters As New List(Of String)()

        For Each TableEntry As Param_SFO.PARAM_SFO.Table In CurrentSFOFile.Tables.ToList()

            If TableEntry.Name = "PUBTOOLINFO" Then
                PS4PubToolInfoTextBox.Text = TableEntry.Value.Trim()
                PS4PubToolInfoTextBox.Tag = TableEntry.Name
                PS4PubToolInfoTextBox.MaxLength = Convert.ToInt32(TableEntry.Indextable.param_data_max_len)

                AddedParameters.Add(TableEntry.Name)
            End If
            If TableEntry.Name = "PUBTOOLVER" Then
                Dim value As Integer = Convert.ToInt32(TableEntry.Value)
                Dim hexOutput As String = String.Format("{0:X}", value)

                PS4PubToolVersionComboBox.Items.Add(hexOutput)
                PS4PubToolVersionComboBox.Tag = TableEntry.Name
                PS4PubToolVersionComboBox.SelectedIndex = 0

                AddedParameters.Add(TableEntry.Name)
            End If

#Region "PSV"
            If TableEntry.Name = "PSP2_SYSTEM_VER" Then
                Dim Value As Integer = Convert.ToInt32(TableEntry.Value)
                Dim hexOutput As String = String.Format("{0:X}", Value)

                SystemVersionTextBox.Text = hexOutput
                SystemVersionTextBox.Tag = TableEntry.Name
                SystemVersionTextBox.MaxLength = Convert.ToInt32(TableEntry.Indextable.param_data_max_len)

                AddedParameters.Add(TableEntry.Name)
            End If
            If TableEntry.Name = "NP_COMMUNICATION_ID" Then
                VitaSupportGameBootMsgCheckBox.IsChecked = True
                VitaNPComIDTextBox.Text = TableEntry.Value
                VitaNPComIDTextBox.MaxLength = Convert.ToInt32(TableEntry.Indextable.param_data_max_len)

                AddedParameters.Add(TableEntry.Name)
            End If
            If TableEntry.Name = "INSTALL_DIR_ADDCONT" Then
                VitaAddtionalContentCheckBox.IsChecked = True
                ShareAppTitleTextBox.Text = TableEntry.Value
                ShareAppTitleTextBox.MaxLength = Convert.ToInt32(TableEntry.Indextable.param_data_max_len)

                AddedParameters.Add(TableEntry.Name)
            End If
            If TableEntry.Name = "INSTALL_DIR_SAVEDATA" Then
                VitaEnableShareSaveCheckBox.IsChecked = True
                VitaShareSaveDataTextBox.Text = TableEntry.Value
                VitaShareSaveDataTextBox.MaxLength = Convert.ToInt32(TableEntry.Indextable.param_data_max_len)

                AddedParameters.Add(TableEntry.Name)
            End If
            If TableEntry.Name = "SAVEDATA_MAX_SIZE" Then
                VitaSaveDataQuotaTextBox.Text = TableEntry.Value
                VitaSaveDataQuotaTextBox.MaxLength = Convert.ToInt32(TableEntry.Indextable.param_data_max_len)

                AddedParameters.Add(TableEntry.Name)
            End If
            If TableEntry.Name = "STITLE" Then
                AppShotTitleTextBox.Text = TableEntry.Value
                AppShotTitleTextBox.Tag = TableEntry.Name
                AppShotTitleTextBox.MaxLength = Convert.ToInt32(TableEntry.Indextable.param_data_max_len)

                AddedParameters.Add(TableEntry.Name)
            End If
#End Region

#Region "PS3"
            If TableEntry.Name = "SOUND_FORMAT" Then
                Dim Value As Integer = Convert.ToInt32(TableEntry.Value)

                Select Case Value
                    Case 1
                        LPCM20.IsChecked = True
                    Case 4
                        LPCM51.IsChecked = True
                    Case 5
                        LPCM20.IsChecked = True
                        LPCM51.IsChecked = True
                    Case 16
                        LPCM71.IsChecked = True
                    Case 21
                        LPCM20.IsChecked = True
                        LPCM51.IsChecked = True
                        LPCM71.IsChecked = True
                    Case 258
                        DolbyDigital.IsChecked = True
                    Case 279
                        LPCM20.IsChecked = True
                        LPCM51.IsChecked = True
                        LPCM71.IsChecked = True
                        DolbyDigital.IsChecked = True
                    Case 514
                        DTS.IsChecked = True
                    Case 772
                        DolbyDigital.IsChecked = True
                        DTS.IsChecked = True
                    Case 791
                        LPCM20.IsChecked = True
                        LPCM51.IsChecked = True
                        LPCM71.IsChecked = True
                        DolbyDigital.IsChecked = True
                        DTS.IsChecked = True
                    Case 793
                        LPCM20.IsChecked = True
                        LPCM51.IsChecked = True
                        LPCM71.IsChecked = True
                        DolbyDigital.IsChecked = True
                        DTS.IsChecked = True
                End Select

                AddedParameters.Add(TableEntry.Name)
            End If
            If TableEntry.Name = "RESOLUTION" Then
                Dim Value As Integer = Convert.ToInt32(TableEntry.Value)

                Select Case Value
                    Case 1
                        LOWRES.IsChecked = True
                    Case 2
                        SD.IsChecked = True
                    Case 3
                        LOWRES.IsChecked = True
                        SD.IsChecked = True
                    Case 4
                        HD.IsChecked = True
                    Case 5
                        HD.IsChecked = True
                        LOWRES.IsChecked = True
                    Case 6
                        HD.IsChecked = True
                        SD.IsChecked = True
                    Case 7
                        HD.IsChecked = True
                        SD.IsChecked = True
                        LOWRES.IsChecked = True
                    Case 8
                        FULLHD.IsChecked = True
                    Case 9
                        FULLHD.IsChecked = True
                        LOWRES.IsChecked = True
                    Case 10
                        FULLHD.IsChecked = True
                        SD.IsChecked = True
                    Case 11
                        FULLHD.IsChecked = True
                        SD.IsChecked = True
                        LOWRES.IsChecked = True
                    Case 12
                        FULLHD.IsChecked = True
                        HD.IsChecked = True
                    Case 13
                        FULLHD.IsChecked = True
                        HD.IsChecked = True
                        LOWRES.IsChecked = True
                    Case 14
                        FULLHD.IsChecked = True
                        HD.IsChecked = True
                        SD.IsChecked = True
                    Case 15
                        FULLHD.IsChecked = True
                        HD.IsChecked = True
                        SD.IsChecked = True
                        LOWRES.IsChecked = True
                    Case 16
                        LOWRES169.IsChecked = True
                    Case 17
                        LOWRES169.IsChecked = True
                        LOWRES.IsChecked = True
                    Case 18
                        LOWRES169.IsChecked = True
                        SD.IsChecked = True
                    Case 19
                        LOWRES.IsChecked = True
                        LOWRES169.IsChecked = True
                        SD.IsChecked = True
                    Case 20
                        LOWRES169.IsChecked = True
                        HD.IsChecked = True
                    Case 21
                        LOWRES.IsChecked = True
                        LOWRES169.IsChecked = True
                        HD.IsChecked = True
                    Case 22
                        LOWRES169.IsChecked = True
                        SD.IsChecked = True
                        HD.IsChecked = True
                    Case 23
                        LOWRES169.IsChecked = True
                        LOWRES.IsChecked = True
                        SD.IsChecked = True
                        HD.IsChecked = True
                    Case 24
                        FULLHD.IsChecked = True
                        LOWRES169.IsChecked = True
                    Case 25
                        FULLHD.IsChecked = True
                        LOWRES.IsChecked = True
                        LOWRES169.IsChecked = True
                    Case 26
                        FULLHD.IsChecked = True
                        SD.IsChecked = True
                        LOWRES169.IsChecked = True
                    Case 27
                        FULLHD.IsChecked = True
                        SD.IsChecked = True
                        LOWRES.IsChecked = True
                        LOWRES169.IsChecked = True
                    Case 28
                        FULLHD.IsChecked = True
                        HD.IsChecked = True
                        LOWRES169.IsChecked = True
                    Case 29
                        FULLHD.IsChecked = True
                        HD.IsChecked = True
                        LOWRES.IsChecked = True
                        LOWRES169.IsChecked = True
                    Case 30
                        FULLHD.IsChecked = True
                        HD.IsChecked = True
                        SD.IsChecked = True
                        LOWRES169.IsChecked = True
                    Case 31
                        FULLHD.IsChecked = True
                        HD.IsChecked = True
                        LOWRES.IsChecked = True
                        LOWRES169.IsChecked = True
                        SD.IsChecked = True
                    Case 32
                        SD169.IsChecked = True
                    Case 33
                        SD169.IsChecked = True
                        LOWRES.IsChecked = True
                    Case 34
                        SD169.IsChecked = True
                        SD.IsChecked = True
                    Case 35
                        SD169.IsChecked = True
                        SD.IsChecked = True
                        LOWRES.IsChecked = True
                    Case 36
                        SD169.IsChecked = True
                        HD.IsChecked = True
                    Case 37
                        HD.IsChecked = True
                        SD169.IsChecked = True
                        LOWRES.IsChecked = True
                    Case 38
                        HD.IsChecked = True
                        SD.IsChecked = True
                        SD169.IsChecked = True
                    Case 39
                        HD.IsChecked = True
                        SD169.IsChecked = True
                        SD.IsChecked = True
                        LOWRES.IsChecked = True
                    Case 40
                        SD169.IsChecked = True
                        FULLHD.IsChecked = True
                    Case 41
                        FULLHD.IsChecked = True
                        SD169.IsChecked = True
                        LOWRES.IsChecked = True
                    Case 42
                        FULLHD.IsChecked = True
                        SD169.IsChecked = True
                        SD.IsChecked = True
                    Case 43
                        FULLHD.IsChecked = True
                        SD169.IsChecked = True
                        SD.IsChecked = True
                        LOWRES.IsChecked = True
                    Case 44
                        FULLHD.IsChecked = True
                        SD169.IsChecked = True
                        HD.IsChecked = True
                    Case 45
                        FULLHD.IsChecked = True
                        SD169.IsChecked = True
                        HD.IsChecked = True
                        LOWRES.IsChecked = True
                    Case 46
                        FULLHD.IsChecked = True
                        SD169.IsChecked = True
                        HD.IsChecked = True
                        SD.IsChecked = True
                    Case 47
                        FULLHD.IsChecked = True
                        SD169.IsChecked = True
                        HD.IsChecked = True
                        SD.IsChecked = True
                        LOWRES.IsChecked = True
                    Case 48
                        LOWRES169.IsChecked = True
                        SD169.IsChecked = True
                    Case 49
                        LOWRES169.IsChecked = True
                        SD169.IsChecked = True
                        LOWRES.IsChecked = True
                    Case 50
                        LOWRES169.IsChecked = True
                        SD169.IsChecked = True
                        SD.IsChecked = True
                    Case 51
                        LOWRES169.IsChecked = True
                        SD169.IsChecked = True
                        SD.IsChecked = True
                        LOWRES.IsChecked = True
                    Case 52
                        LOWRES169.IsChecked = True
                        SD169.IsChecked = True
                        HD.IsChecked = True
                    Case 53
                        LOWRES169.IsChecked = True
                        SD169.IsChecked = True
                        HD.IsChecked = True
                        LOWRES.IsChecked = True
                    Case 54
                        LOWRES169.IsChecked = True
                        SD169.IsChecked = True
                        HD.IsChecked = True
                        SD.IsChecked = True
                    Case 55
                        LOWRES169.IsChecked = True
                        SD169.IsChecked = True
                        HD.IsChecked = True
                        SD.IsChecked = True
                        LOWRES.IsChecked = True
                    Case 56
                        LOWRES169.IsChecked = True
                        SD169.IsChecked = True
                        FULLHD.IsChecked = True
                    Case 57
                        LOWRES169.IsChecked = True
                        SD169.IsChecked = True
                        FULLHD.IsChecked = True
                        LOWRES.IsChecked = True
                    Case 58
                        LOWRES169.IsChecked = True
                        SD169.IsChecked = True
                        FULLHD.IsChecked = True
                        SD.IsChecked = True
                    Case 59
                        LOWRES169.IsChecked = True
                        SD169.IsChecked = True
                        FULLHD.IsChecked = True
                        SD.IsChecked = True
                        LOWRES.IsChecked = True
                    Case 60
                        LOWRES169.IsChecked = True
                        SD169.IsChecked = True
                        FULLHD.IsChecked = True
                        HD.IsChecked = True
                    Case 61
                        LOWRES169.IsChecked = True
                        SD169.IsChecked = True
                        FULLHD.IsChecked = True
                        HD.IsChecked = True
                        LOWRES.IsChecked = True
                    Case 62
                        LOWRES169.IsChecked = True
                        SD169.IsChecked = True
                        FULLHD.IsChecked = True
                        HD.IsChecked = True
                        SD.IsChecked = True
                    Case 63
                        LOWRES169.IsChecked = True
                        SD169.IsChecked = True
                        FULLHD.IsChecked = True
                        HD.IsChecked = True
                        SD.IsChecked = True
                        LOWRES.IsChecked = True
                    Case Else
                End Select

                AddedParameters.Add(TableEntry.Name)
            End If
            If TableEntry.Name = "PS3_SYSTEM_VER" Then
                SystemVersionTextBox.Tag = TableEntry.Name
                SystemVersionTextBox.Text = TableEntry.Value
                SystemVersionTextBox.MaxLength = Convert.ToInt32(TableEntry.Indextable.param_data_max_len)

                AddedParameters.Add(TableEntry.Name)
            End If
#End Region

#Region "General parameters"
            If TableEntry.Name = "TITLE_ID" Then
                IDTextBox.Text = TableEntry.Value.Trim()
                IDTextBox.Tag = TableEntry.Name
                IDTextBox.MaxLength = Convert.ToInt32(TableEntry.Indextable.param_data_max_len)

                AddedParameters.Add(TableEntry.Name)
            End If
            If TableEntry.Name = "ATTRIBUTE" Then
                Dim Value As Integer = 0
                Integer.TryParse(TableEntry.Value.Trim(), Value)

                If SFOVer = Param_SFO.PARAM_SFO.Playstation.ps4 Then
                    Select Case Value
                        Case 0
                            PS4InitLogoutCheckBox.IsChecked = True
                        Case 1
                            PS4InitLogoutCheckBox.IsChecked = False
                    End Select
                ElseIf SFOVer = Param_SFO.PARAM_SFO.Playstation.psvita Then
                    Select Case Value
                        Case 2
                            VitaUseLibLocationCheckBox.IsChecked = True
                        Case 128
                            VitaDisplayInfoBarCheckBox.IsChecked = True
                        Case 130
                            VitaUseLibLocationCheckBox.IsChecked = True
                            VitaDisplayInfoBarCheckBox.IsChecked = True
                        Case 256
                            VitaColorInfoBarCheckBox.IsChecked = True
                        Case 258
                            VitaUseLibLocationCheckBox.IsChecked = True
                            VitaColorInfoBarCheckBox.IsChecked = True
                        Case 384
                            VitaDisplayInfoBarCheckBox.IsChecked = True
                            VitaColorInfoBarCheckBox.IsChecked = True
                        Case 386
                            VitaUseLibLocationCheckBox.IsChecked = True
                            VitaDisplayInfoBarCheckBox.IsChecked = True
                            VitaColorInfoBarCheckBox.IsChecked = True
                        Case 1024
                            VitaAppIsUpgradedableCheckBox.IsChecked = True
                        Case 1026
                            VitaUseLibLocationCheckBox.IsChecked = True
                            VitaColorInfoBarCheckBox.IsChecked = True
                        Case 2097152
                            VitaAddHealthInfoCheckBox.IsChecked = True
                        Case 33554432
                            VitaUseTWDialogCheckBox.IsChecked = True
                        Case 35652992
                            VitaUseLibLocationCheckBox.IsChecked = True
                            VitaDisplayInfoBarCheckBox.IsChecked = True
                            VitaColorInfoBarCheckBox.IsChecked = True
                            VitaAddHealthInfoCheckBox.IsChecked = True
                            VitaUseTWDialogCheckBox.IsChecked = True
                        Case 35652994
                            VitaUseLibLocationCheckBox.IsChecked = True
                            VitaDisplayInfoBarCheckBox.IsChecked = True
                            VitaColorInfoBarCheckBox.IsChecked = True
                            VitaAddHealthInfoCheckBox.IsChecked = True
                            VitaUseTWDialogCheckBox.IsChecked = True
                    End Select
                End If
            End If
            If TableEntry.Name = "BOOTABLE" Then
                If SFOVer = Param_SFO.PARAM_SFO.Playstation.ps3 Then
                    If TableEntry.Value = "0" Then
                        PS3BootableCheckBox.IsChecked = False
                    ElseIf TableEntry.Value = "1" Then
                        PS3BootableCheckBox.IsChecked = True
                        PS3BootableCheckBox.Content = "Bootable (Mode 1)"
                    ElseIf TableEntry.Value = "2" Then
                        PS3BootableCheckBox.IsChecked = True
                        PS3BootableCheckBox.Content = "Bootable (Mode 2)"
                    Else
                        PS3BootableCheckBox.IsChecked = True
                    End If
                ElseIf SFOVer = Param_SFO.PARAM_SFO.Playstation.psp Then
                    If TableEntry.Value = "0" Then
                        PSPBootableCheckBox.IsChecked = False
                    ElseIf TableEntry.Value = "1" Then
                        PSPBootableCheckBox.IsChecked = True
                        PSPBootableCheckBox.Content = "Bootable (Mode 1)"
                    ElseIf TableEntry.Value = "2" Then
                        PSPBootableCheckBox.IsChecked = True
                        PSPBootableCheckBox.Content = "Bootable (Mode 2)"
                    Else
                        PSPBootableCheckBox.IsChecked = True
                    End If
                End If
            End If
            If TableEntry.Name = "CONTENT_ID" Then
                ContentIDTextBox.Text = TableEntry.Value.Trim()
                ContentIDTextBox.Tag = TableEntry.Name
                ContentIDTextBox.MaxLength = Convert.ToInt32(TableEntry.Indextable.param_data_max_len)

                AddedParameters.Add(TableEntry.Name)
            End If
            If TableEntry.Name = "TITLE" Then
                TitleTextBox.Text = TableEntry.Value.Trim()
                TitleTextBox.Tag = TableEntry.Name
                TitleTextBox.MaxLength = Convert.ToInt32(TableEntry.Indextable.param_data_max_len)

                AddedParameters.Add(TableEntry.Name)
            End If
            If TableEntry.Name = "CATEGORY" Then
                If SFOVer = Param_SFO.PARAM_SFO.Playstation.ps4 Or SFOVer = Param_SFO.PARAM_SFO.Playstation.psvita Then
                    Dim hex As String = BitConverter.ToString(TableEntry.ValueBuffer, 0, Convert.ToInt32(TableEntry.Indextable.param_data_max_len)).ToString().Replace("-", String.Empty)
                    Dim temp As String = Convert.ToInt32(hex).ToString("X4")
                    Dim stringtemp = Encoding.ASCII.GetString(TableEntry.ValueBuffer).Replace("\0", "")

                    CategoryTextBox.Text = CType(Convert.ToInt32(hex), Param_SFO.PARAM_SFO.DataTypes).ToString()
                ElseIf SFOVer = Param_SFO.PARAM_SFO.Playstation.ps3 Then
                    CategoryTextBox.Text = CType(BitConverter.ToUInt16(Encoding.UTF8.GetBytes(TableEntry.Value), 0).ToString(), Param_SFO.PARAM_SFO.DataTypes).ToString
                Else
                    CategoryTextBox.Text = TableEntry.Value.ToString()
                End If

                CategoryTextBox.Tag = TableEntry.Name
                CategoryTextBox.MaxLength = Convert.ToInt32(TableEntry.Indextable.param_data_max_len)

                AddedParameters.Add(TableEntry.Name)
            End If
            If TableEntry.Name = "APP_VER" Then
                AppVerTextBox.Text = TableEntry.Value.Trim()
                AppVerTextBox.Tag = TableEntry.Name
                AppVerTextBox.MaxLength = Convert.ToInt32(TableEntry.Indextable.param_data_max_len)

                AddedParameters.Add(TableEntry.Name)
            End If
            If TableEntry.Name = "APP_TYPE" Then
                PS4AppTypeComboBox.Tag = TableEntry.Name
                PS4AppTypeComboBox.SelectedIndex = Convert.ToInt32(TableEntry.Value)

                AddedParameters.Add(TableEntry.Name)
            End If
            If TableEntry.Name = "VERSION" Then
                VerTextBox.Text = TableEntry.Value.Trim()
                VerTextBox.Tag = TableEntry.Name
                VerTextBox.MaxLength = Convert.ToInt32(TableEntry.Indextable.param_data_max_len)

                AddedParameters.Add(TableEntry.Name)
            End If
            If TableEntry.Name = "PARENTAL_LEVEL" Then
                If TableEntry.Value = "" Then
                    ParentalComboBox.SelectedIndex = 0
                Else
                    ParentalComboBox.Tag = TableEntry.Name
                    ParentalComboBox.SelectedIndex = Convert.ToInt32(TableEntry.Value)

                    AddedParameters.Add(TableEntry.Name)
                End If
            End If
#End Region

#Region "PS4"
            If TableEntry.Name = "SYSTEM_VER" Then
                Dim Value As Integer = Convert.ToInt32(TableEntry.Value)
                Dim hexOutput As String = String.Format("{0:X}", Value)

                SystemVersionTextBox.Text = TableEntry.Value
                SystemVersionTextBox.Tag = TableEntry.Name
                SystemVersionTextBox.MaxLength = Convert.ToInt32(TableEntry.Indextable.param_data_max_len)

                AddedParameters.Add(TableEntry.Name)
            End If
#End Region

#Region "PSP"
            If TableEntry.Name = "PSP_SYSTEM_VER" Then
                SystemVersionTextBox.Tag = TableEntry.Name
                SystemVersionTextBox.Text = TableEntry.Value
                SystemVersionTextBox.MaxLength = Convert.ToInt32(TableEntry.Indextable.param_data_max_len)

                AddedParameters.Add(TableEntry.Name)
            End If
            If TableEntry.Name = "DISC_VERSION" Then
                VerTextBox.Text = TableEntry.Value.Trim()
                VerTextBox.Tag = TableEntry.Name
                VerTextBox.MaxLength = Convert.ToInt32(TableEntry.Indextable.param_data_max_len)

                AddedParameters.Add(TableEntry.Name)
            End If
            If TableEntry.Name = "DISC_ID" Then
                IDTextBox.Text = TableEntry.Value.Trim()
                IDTextBox.Tag = TableEntry.Name
                IDTextBox.MaxLength = Convert.ToInt32(TableEntry.Indextable.param_data_max_len)

                AddedParameters.Add(TableEntry.Name)
            End If
#End Region


        Next

    End Sub

    Private Sub LoadSFOButton_Click(sender As Object, e As RoutedEventArgs) Handles LoadSFOButton.Click
        Dim NewOpenFileDialog As New Forms.OpenFileDialog() With {.Title = "Select a PARAM.SFO file", .Filter = "PARAM.SFO files (*.SFO)|*.SFO"}
        If NewOpenFileDialog.ShowDialog() = Forms.DialogResult.OK Then
            OpenPARAMSFO(Nothing, NewOpenFileDialog.FileName)
            CurrentSFO = New Param_SFO.PARAM_SFO(NewOpenFileDialog.FileName)
            CurrentSFOFilePath = NewOpenFileDialog.FileName
        End If
    End Sub

    Private Sub IDTextBox_TextChanged(sender As Object, e As Controls.TextChangedEventArgs) Handles IDTextBox.TextChanged
        For i As Integer = 0 To CurrentSFO.Tables.Count - 1
            If CurrentSFO.Tables(i).Name = "TITLE_ID" Then
                Dim TempItem = CurrentSFO.Tables(i)
                TempItem.Value = IDTextBox.Text.Trim()
                CurrentSFO.Tables(i) = TempItem
            End If
        Next
    End Sub

    Private Sub ContentIDTextBox_TextChanged(sender As Object, e As Controls.TextChangedEventArgs) Handles ContentIDTextBox.TextChanged
        For i As Integer = 0 To CurrentSFO.Tables.Count - 1
            If CurrentSFO.Tables(i).Name = "CONTENT_ID" Then
                Dim tempitem = CurrentSFO.Tables(i)
                tempitem.Value = ContentIDTextBox.Text.Trim()
                CurrentSFO.Tables(i) = tempitem
            End If
        Next
    End Sub

    Private Sub TitleTextBox_TextChanged(sender As Object, e As Controls.TextChangedEventArgs) Handles TitleTextBox.TextChanged
        For i As Integer = 0 To CurrentSFO.Tables.Count - 1
            If CurrentSFO.Tables(i).Name = "TITLE" Then
                Dim TempItem = CurrentSFO.Tables(i)
                TempItem.Value = TitleTextBox.Text.Trim()
                CurrentSFO.Tables(i) = TempItem
            End If
        Next
    End Sub

    Private Sub VerTextBox_TextChanged(sender As Object, e As Controls.TextChangedEventArgs) Handles VerTextBox.TextChanged
        For i As Integer = 0 To CurrentSFO.Tables.Count - 1
            If CurrentSFO.Tables(i).Name = VerTextBox.Tag.ToString() Then
                Dim TempItem = CurrentSFO.Tables(i)
                TempItem.Value = VerTextBox.Text.Trim()
                CurrentSFO.Tables(i) = TempItem
            End If
        Next
    End Sub

    Private Sub AppVerTextBox_TextChanged(sender As Object, e As Controls.TextChangedEventArgs) Handles AppVerTextBox.TextChanged
        For i As Integer = 0 To CurrentSFO.Tables.Count - 1
            If CurrentSFO.Tables(i).Name = AppVerTextBox.Tag.ToString() Then
                Dim TempItem = CurrentSFO.Tables(i)
                TempItem.Value = AppVerTextBox.Text.Trim()
                CurrentSFO.Tables(i) = TempItem
            End If
        Next
    End Sub

    Private Sub ParentalComboBox_SelectionChanged(sender As Object, e As Controls.SelectionChangedEventArgs) Handles ParentalComboBox.SelectionChanged
        For i As Integer = 0 To CurrentSFO.Tables.Count - 1
            If CurrentSFO.Tables(i).Name = ParentalComboBox.Tag.ToString() Then
                Dim TempItem = CurrentSFO.Tables(i)
                TempItem.Value = ParentalComboBox.Text.Trim()
                CurrentSFO.Tables(i) = TempItem
            End If
        Next
    End Sub

    Private Sub CategoryTextBox_TextChanged(sender As Object, e As Controls.TextChangedEventArgs) Handles CategoryTextBox.TextChanged
        For i As Integer = 0 To CurrentSFO.Tables.Count - 1
            If CurrentSFO.Tables(i).Name = "CATEGORY" Then
                Dim tempitem = CurrentSFO.Tables(i)
                tempitem.Value = CategoryTextBox.Text.Trim()
                CurrentSFO.Tables(i) = tempitem
            End If
        Next
    End Sub

    Private Sub SystemVersionTextBox_TextChanged(sender As Object, e As Controls.TextChangedEventArgs) Handles SystemVersionTextBox.TextChanged
        For i As Integer = 0 To CurrentSFO.Tables.Count - 1
            If CurrentSFO.Tables(i).Name = "SYSTEM_VER" OrElse CurrentSFO.Tables(i).Name = "PS3_SYSTEM_VER" Then
                Dim TempItem = CurrentSFO.Tables(i)
                TempItem.Value = SystemVersionTextBox.Text.Trim()
                CurrentSFO.Tables(i) = TempItem
            End If
        Next
    End Sub

    Private Sub PS4AppTypeComboBox_SelectionChanged(sender As Object, e As Controls.SelectionChangedEventArgs) Handles PS4AppTypeComboBox.SelectionChanged
        For i As Integer = 0 To CurrentSFO.Tables.Count - 1
            If CurrentSFO.Tables(i).Name = PS4AppTypeComboBox.Tag.ToString() Then
                Dim TempItem = CurrentSFO.Tables(i)
                TempItem.Value = PS4AppTypeComboBox.Text.Trim()
                CurrentSFO.Tables(i) = TempItem
            End If
        Next
    End Sub

    Public Function AddNewParam(Index As Integer, Name As String, Value As String, Format As Param_SFO.PARAM_SFO.FMT, Lenght As Integer, MaxLength As Integer, ParamTable As List(Of Param_SFO.PARAM_SFO.Table)) As List(Of Param_SFO.PARAM_SFO.Table)
        Dim IndexTable As New Param_SFO.PARAM_SFO.index_table With {.param_data_fmt = Format, .param_data_len = Convert.ToUInt32(Lenght), .param_data_max_len = Convert.ToUInt32(MaxLength)}
        Dim TableItem As New Param_SFO.PARAM_SFO.Table With {.index = Index, .Indextable = IndexTable, .Name = Name, .Value = Value}
        ParamTable.Add(TableItem)
        Return ParamTable
    End Function

    Private Sub NewSFOButton_Click(sender As Object, e As RoutedEventArgs) Handles NewSFOButton.Click

        Dim NewSFOType As String = ""

        If NewSFOType = "PS4" Or NewSFOType = "PSV" Then
            Dim ParamTables As New List(Of Param_SFO.PARAM_SFO.Table)()
            Dim NewItemIndex As Integer = 0

            NewItemIndex += 1
            AddNewParam(NewItemIndex, "APP_TYPE", "0", Param_SFO.PARAM_SFO.FMT.UINT32, 4, 4, ParamTables)
            NewItemIndex += 1
            AddNewParam(NewItemIndex, "ATTRIBUTE", "0", Param_SFO.PARAM_SFO.FMT.UINT32, 4, 4, ParamTables)
            NewItemIndex += 1
            AddNewParam(NewItemIndex, "APP_VER", "01.00", Param_SFO.PARAM_SFO.FMT.Utf8Null, 5, 8, ParamTables)
            NewItemIndex += 1
            AddNewParam(NewItemIndex, "CATEGORY", "gd", Param_SFO.PARAM_SFO.FMT.Utf8Null, 3, 4, ParamTables)
            NewItemIndex += 1
            AddNewParam(NewItemIndex, "CONTENT_ID", "XXYYYY-XXXXYYYYY_00-ZZZZZZZZZZZZZZZZ", Param_SFO.PARAM_SFO.FMT.Utf8Null, 37, 48, ParamTables)
            NewItemIndex += 1
            AddNewParam(NewItemIndex, "DOWNLOAD_DATA_SIZE", "0", Param_SFO.PARAM_SFO.FMT.UINT32, 4, 4, ParamTables)
            NewItemIndex += 1
            AddNewParam(NewItemIndex, "FORMAT", "obs", Param_SFO.PARAM_SFO.FMT.Utf8Null, 4, 4, ParamTables)
            NewItemIndex += 1
            AddNewParam(NewItemIndex, "PARENTAL_LEVEL", "1", Param_SFO.PARAM_SFO.FMT.UINT32, 4, 4, ParamTables)
            NewItemIndex += 1
            AddNewParam(NewItemIndex, "REMOTE_PLAY_KEY_ASSIGN", "1", Param_SFO.PARAM_SFO.FMT.UINT32, 4, 4, ParamTables)
            NewItemIndex += 1
            For i As Integer = 1 To 8 - 1
                AddNewParam(NewItemIndex, "SERVICE_ID_ADDCONT_ADD_" + i.ToString, "", Param_SFO.PARAM_SFO.FMT.Utf8Null, 1, 20, ParamTables)
            Next
            NewItemIndex += 1
            AddNewParam(NewItemIndex, "SYSTEM_VER", "0", Param_SFO.PARAM_SFO.FMT.UINT32, 4, 4, ParamTables)
            NewItemIndex += 1
            AddNewParam(NewItemIndex, "TITLE", "GameTitle ID", Param_SFO.PARAM_SFO.FMT.Utf8Null, 19, 128, ParamTables)
            NewItemIndex += 1
            AddNewParam(NewItemIndex, "TITLE_ID", "XXXXYYYYY", Param_SFO.PARAM_SFO.FMT.Utf8Null, 10, 12, ParamTables)
            NewItemIndex += 1
            AddNewParam(NewItemIndex, "VERSION", "01.00", Param_SFO.PARAM_SFO.FMT.Utf8Null, 6, 8, ParamTables)

            CurrentSFO = New Param_SFO.PARAM_SFO()
            Param_SFO.PARAM_SFO.Header.IndexTableEntries = Convert.ToUInt32(NewItemIndex)
            CurrentSFO.Tables = ParamTables
        End If

        OpenPARAMSFO(CurrentSFO)

    End Sub

    Private Sub SaveSFOButton_Click(sender As Object, e As RoutedEventArgs) Handles SaveSFOButton.Click
        Dim SFD As New SaveFileDialog With {.Filter = "PARAM.SFO (PARAM.SFO)|PARAM.SFO", .DefaultExt = "SFO", .AddExtension = True}
        If SFD.ShowDialog() = Forms.DialogResult.OK Then

            If CurrentSFO.PlaystationVersion = Param_SFO.PARAM_SFO.Playstation.ps4 Then
                Dim PubInfo As Boolean = False
                Dim PubVer As Boolean = False
                Dim NewItemIndex As Integer = 0

                For i As Integer = 0 To CurrentSFO.Tables.Count - 1
                    If CurrentSFO.Tables(i).Name = "PUBTOOLINFO" Then
                        PubInfo = True
                    End If
                    If CurrentSFO.Tables(i).Name = "PUBTOOLVER" Then
                        PubVer = True
                    End If
                Next

                NewItemIndex = CurrentSFO.Tables.Count

                If PubInfo = False Then
                    NewItemIndex += 1
                    AddNewParam(NewItemIndex, "PUBTOOLINFO", "c_date=20180504,sdk_ver=04008000,st_type=digital50,snd0_loud=-25.56,img0_l0_size=1032,img0_l1_size=0,img0_sc_ksize=4608,img0_pc_ksize=1344", Param_SFO.PARAM_SFO.FMT.Utf8Null, 139, 512, CurrentSFO.Tables)
                End If
                If PubVer = False Then
                    NewItemIndex += 1
                    AddNewParam(NewItemIndex, "PUBTOOLVER", "2890000", Param_SFO.PARAM_SFO.FMT.UINT32, 4, 4, CurrentSFO.Tables)
                End If
            End If

            Dim SortedList = CurrentSFO.Tables.OrderBy(Function(o) o.Name).ToList()
            For i As Integer = 0 To SortedList.Count - 1
                Dim temp = SortedList(i)
                temp.index = i
                SortedList(i) = temp
            Next
            CurrentSFO.Tables = SortedList

            CurrentSFO.SaveSFO(CurrentSFO, SFD.FileName)
        End If
    End Sub

End Class
