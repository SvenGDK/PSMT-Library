Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Forms
Imports Newtonsoft.Json

Public Class PS5ParamEditor

    Dim CurrentParamJsonPath As String = Nothing
    Dim CurrentParamJson As PS5ParamClass.PS5Param = Nothing

    Private Sub NewParamMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles NewParamMenuItem.Click

        'Clear previous data
        ParamsListView.Items.Clear()

        Dim NewPS5Param As New PS5ParamClass.PS5Param() With {
            .AgeLevel = New PS5ParamClass.AgeLevel() With {
                .[Default] = 0, .US = 0},
            .ApplicationCategoryType = 0,
            .ApplicationDrmType = "default",
            .Attribute = 0,
            .Attribute2 = 0,
            .Attribute3 = 0,
            .ConceptId = "99999",
            .ContentBadgeType = 0,
            .ContentId = "IV9999-CUSA99999_00-XXXXXXXXXXXXXXXX",
            .ContentVersion = "01.000.000",
            .DeeplinkUri = "",
            .DownloadDataSize = 0,
            .LocalizedParameters = New PS5ParamClass.LocalizedParameters() With {
                .DefaultLanguage = "en-US",
                .EnUS = New PS5ParamClass.EnUS() With {.TitleName = "Title Name"},
                .ArAE = New PS5ParamClass.ArAE() With {.TitleName = "Title Name"},
                .CsCZ = New PS5ParamClass.CsCZ() With {.TitleName = "Title Name"},
                .DaDK = New PS5ParamClass.DaDK() With {.TitleName = "Title Name"},
                .DeDE = New PS5ParamClass.DeDE() With {.TitleName = "Title Name"},
                .ElGR = New PS5ParamClass.ElGR() With {.TitleName = "Title Name"},
                .EnGB = New PS5ParamClass.EnGB() With {.TitleName = "Title Name"},
                .Es419 = New PS5ParamClass.Es419() With {.TitleName = "Title Name"},
                .EsES = New PS5ParamClass.EsES() With {.TitleName = "Title Name"},
                .FiFI = New PS5ParamClass.FiFI() With {.TitleName = "Title Name"},
                .FrCA = New PS5ParamClass.FrCA() With {.TitleName = "Title Name"},
                .FrFR = New PS5ParamClass.FrFR() With {.TitleName = "Title Name"},
                .HuHU = New PS5ParamClass.HuHU() With {.TitleName = "Title Name"},
                .IdID = New PS5ParamClass.IdID() With {.TitleName = "Title Name"},
                .ItIT = New PS5ParamClass.ItIT() With {.TitleName = "Title Name"},
                .JaJP = New PS5ParamClass.JaJP() With {.TitleName = "Title Name"},
                .KoKR = New PS5ParamClass.KoKR() With {.TitleName = "Title Name"},
                .NlNL = New PS5ParamClass.NlNL() With {.TitleName = "Title Name"},
                .NoNO = New PS5ParamClass.NoNO() With {.TitleName = "Title Name"},
                .PlPL = New PS5ParamClass.PlPL() With {.TitleName = "Title Name"},
                .PtBR = New PS5ParamClass.PtBR() With {.TitleName = "Title Name"},
                .PtPT = New PS5ParamClass.PtPT() With {.TitleName = "Title Name"},
                .RoRO = New PS5ParamClass.RoRO() With {.TitleName = "Title Name"},
                .RuRU = New PS5ParamClass.RuRU() With {.TitleName = "Title Name"},
                .SvSE = New PS5ParamClass.SvSE() With {.TitleName = "Title Name"},
                .ThTH = New PS5ParamClass.ThTH() With {.TitleName = "Title Name"},
                .TrTR = New PS5ParamClass.TrTR() With {.TitleName = "Title Name"},
                .ViVN = New PS5ParamClass.ViVN() With {.TitleName = "Title Name"},
                .ZhHans = New PS5ParamClass.ZhHans() With {.TitleName = "Title Name"},
                .ZhHant = New PS5ParamClass.ZhHant() With {.TitleName = "Title Name"}},
            .MasterVersion = "01.00",
            .TitleId = "CUSA99999"}

        CurrentParamJson = NewPS5Param

        For Each Parameter In NewPS5Param.GetType().GetProperties()
            Dim NewParamType As String
            Select Case Parameter.Name
                Case "AgeLevel"
                    NewParamType = "Object"
                Case "ApplicationCategoryType"
                    NewParamType = "Integer"
                Case "ApplicationDrmType"
                    NewParamType = "String"
                Case "Attribute"
                    NewParamType = "Integer"
                Case "Attribute2"
                    NewParamType = "Integer"
                Case "Attribute3"
                    NewParamType = "Integer"
                Case "ConceptId"
                    NewParamType = "String"
                Case "ContentBadgeType"
                    NewParamType = "Integer"
                Case "ContentId"
                    NewParamType = "String"
                Case "ContentVersion"
                    NewParamType = "String"
                Case "DownloadDataSize"
                    NewParamType = "Integer"
                Case "DeeplinkUri"
                    NewParamType = "String"
                Case "LocalizedParameters"
                    NewParamType = "Object"
                Case "MasterVersion"
                    NewParamType = "String"
                Case "TitleId"
                    NewParamType = "String"
                Case Else
                    NewParamType = "Unknown"
            End Select

            'Add to ParamsListView
            Dim NewParamLVItem As New PS5ParamClass.ParamListViewItem() With {.ParamName = Parameter.Name, .ParamType = NewParamType, .ParamValue = Parameter.GetValue(NewPS5Param, Nothing).ToString}
            ParamsListView.Items.Add(NewParamLVItem)
        Next

        AddParamButton.IsEnabled = True
        SaveModifiedValueButton.IsEnabled = True
    End Sub

    Private Sub LoadParamMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles LoadParamMenuItem.Click

        'Clear previous data
        ParamsListView.Items.Clear()

        Dim OFD As New OpenFileDialog() With {.Filter = "Param JSON (param.json)|param.json", .Title = "Please select a param.json file", .Multiselect = False}
        If OFD.ShowDialog() = Forms.DialogResult.OK Then

            'Read the file and create a PS5Param
            Dim JSONData As String = File.ReadAllText(OFD.FileName)
            Dim ParamData As PS5ParamClass.PS5Param

            Try
                ParamData = JsonConvert.DeserializeObject(Of PS5ParamClass.PS5Param)(JSONData)

                'Set current param for saving
                CurrentParamJson = ParamData
                CurrentParamJsonPath = OFD.FileName

                For Each Parameter In ParamData.GetType().GetProperties()
                    Dim NewParamType As String
                    Select Case Parameter.Name
                        Case "AgeLevel"
                            NewParamType = "Object"
                        Case "ApplicationCategoryType"
                            NewParamType = "Integer"
                        Case "ApplicationDrmType"
                            NewParamType = "String"
                        Case "Attribute"
                            NewParamType = "Integer"
                        Case "Attribute2"
                            NewParamType = "Integer"
                        Case "Attribute3"
                            NewParamType = "Integer"
                        Case "ConceptId"
                            NewParamType = "String"
                        Case "ContentBadgeType"
                            NewParamType = "Integer"
                        Case "ContentId"
                            NewParamType = "String"
                        Case "ContentVersion"
                            NewParamType = "String"
                        Case "DownloadDataSize"
                            NewParamType = "Integer"
                        Case "DeeplinkUri"
                            NewParamType = "String"
                        Case "LocalizedParameters"
                            NewParamType = "Object"
                        Case "MasterVersion"
                            NewParamType = "String"
                        Case "TitleId"
                            NewParamType = "String"
                        Case Else
                            NewParamType = "Unknown"
                    End Select

                    'Add to ParamsListView
                    Dim NewParamLVItem As New PS5ParamClass.ParamListViewItem() With {.ParamName = Parameter.Name, .ParamType = NewParamType}

                    If Parameter.GetValue(ParamData, Nothing) Is Nothing Then
                        NewParamLVItem.ParamValue = ""
                    Else
                        NewParamLVItem.ParamValue = Parameter.GetValue(ParamData, Nothing).ToString
                    End If

                    ParamsListView.Items.Add(NewParamLVItem)
                Next

                AddParamButton.IsEnabled = True
                SaveModifiedValueButton.IsEnabled = True

            Catch ex As Exception
                MsgBox("Could not parse the selected param.json file.", MsgBoxStyle.Critical, "Error")
            End Try
        End If
    End Sub

    Private Sub ParamsListView_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles ParamsListView.SelectionChanged
        If ParamsListView.SelectedItem IsNot Nothing Then
            Dim SelectedParam As PS5ParamClass.ParamListViewItem = CType(ParamsListView.SelectedItem, PS5ParamClass.ParamListViewItem)
            If Not String.IsNullOrEmpty(SelectedParam.ParamValue) Then
                ModifyValueTextBox.Text = SelectedParam.ParamValue
            End If
        End If
    End Sub

    Private Sub SaveModifiedValueButton_Click(sender As Object, e As RoutedEventArgs) Handles SaveModifiedValueButton.Click
        If ParamsListView.SelectedItem IsNot Nothing And Not String.IsNullOrEmpty(ModifyValueTextBox.Text) Then

            Dim SelectedParam As PS5ParamClass.ParamListViewItem = CType(ParamsListView.SelectedItem, PS5ParamClass.ParamListViewItem)

            Select Case SelectedParam.ParamName
                Case "AgeLevel"



                Case "ApplicationCategoryType"
                    If IsInt(ModifyValueTextBox.Text) Then
                        CurrentParamJson.ApplicationCategoryType = CInt(ModifyValueTextBox.Text)
                        SelectedParam.ParamValue = ModifyValueTextBox.Text
                    Else
                        MsgBox("Only numbers are allowed.", MsgBoxStyle.Exclamation, "Integer value required")
                    End If
                Case "ApplicationDrmType"
                    Select Case ModifyValueTextBox.Text
                        Case "standard", "demo", "upgradable", "free"
                            CurrentParamJson.ApplicationDrmType = ModifyValueTextBox.Text
                            SelectedParam.ParamValue = ModifyValueTextBox.Text
                        Case Else
                            MsgBox("Only standard, demo, upgradable & free are currently available.", MsgBoxStyle.Exclamation, "Other value required")
                    End Select
                Case "Attribute"
                    If IsInt(ModifyValueTextBox.Text) Then
                        CurrentParamJson.Attribute = CInt(ModifyValueTextBox.Text)
                        SelectedParam.ParamValue = ModifyValueTextBox.Text
                    Else
                        MsgBox("Only numbers are allowed.", MsgBoxStyle.Exclamation, "Integer value required")
                    End If
                Case "Attribute2"
                    If IsInt(ModifyValueTextBox.Text) Then
                        CurrentParamJson.Attribute2 = CInt(ModifyValueTextBox.Text)
                        SelectedParam.ParamValue = ModifyValueTextBox.Text
                    Else
                        MsgBox("Only numbers are allowed.", MsgBoxStyle.Exclamation, "Integer value required")
                    End If
                Case "Attribute3"
                    If IsInt(ModifyValueTextBox.Text) Then
                        CurrentParamJson.Attribute3 = CInt(ModifyValueTextBox.Text)
                        SelectedParam.ParamValue = ModifyValueTextBox.Text
                    Else
                        MsgBox("Only numbers are allowed.", MsgBoxStyle.Exclamation, "Integer value required")
                    End If
                Case "ConceptId"
                    CurrentParamJson.ConceptId = ModifyValueTextBox.Text
                    SelectedParam.ParamValue = ModifyValueTextBox.Text
                Case "ContentBadgeType"
                    If IsInt(ModifyValueTextBox.Text) Then
                        CurrentParamJson.ContentBadgeType = CInt(ModifyValueTextBox.Text)
                        SelectedParam.ParamValue = ModifyValueTextBox.Text
                    Else
                        MsgBox("Only numbers are allowed.", MsgBoxStyle.Exclamation, "Integer value required")
                    End If
                Case "ContentId"
                    If ModifyValueTextBox.Text.Length = 36 Then
                        CurrentParamJson.ContentId = ModifyValueTextBox.Text
                        SelectedParam.ParamValue = ModifyValueTextBox.Text
                    Else
                        MsgBox("Please enter a correct ContentID like IV9999-CUSA99999_00-XXXXXXXXXXXXXXXX", MsgBoxStyle.Exclamation, "ContentID not in correct format")
                    End If
                Case "ContentVersion"
                    CurrentParamJson.ContentVersion = ModifyValueTextBox.Text
                    SelectedParam.ParamValue = ModifyValueTextBox.Text
                Case "DownloadDataSize"
                    If IsInt(ModifyValueTextBox.Text) Then
                        CurrentParamJson.DownloadDataSize = CInt(ModifyValueTextBox.Text)
                        SelectedParam.ParamValue = ModifyValueTextBox.Text
                    Else
                        MsgBox("Only numbers are allowed.", MsgBoxStyle.Exclamation, "Integer value required")
                    End If
                Case "DeeplinkUri"
                    CurrentParamJson.DeeplinkUri = ModifyValueTextBox.Text
                    SelectedParam.ParamValue = ModifyValueTextBox.Text
                Case "LocalizedParameters"

                    'Will be updated on next release
                    MsgBox("Not completely supported yet. The default language will be set to 'en-US' with with the Title Name: " + ModifyValueTextBox.Text, MsgBoxStyle.Information)

                    CurrentParamJson.LocalizedParameters = New PS5ParamClass.LocalizedParameters() With {.DefaultLanguage = "en-US",
                .EnUS = New PS5ParamClass.EnUS() With {.TitleName = ModifyValueTextBox.Text},
                .ArAE = New PS5ParamClass.ArAE() With {.TitleName = ModifyValueTextBox.Text},
                .CsCZ = New PS5ParamClass.CsCZ() With {.TitleName = ModifyValueTextBox.Text},
                .DaDK = New PS5ParamClass.DaDK() With {.TitleName = ModifyValueTextBox.Text},
                .DeDE = New PS5ParamClass.DeDE() With {.TitleName = ModifyValueTextBox.Text},
                .ElGR = New PS5ParamClass.ElGR() With {.TitleName = ModifyValueTextBox.Text},
                .EnGB = New PS5ParamClass.EnGB() With {.TitleName = ModifyValueTextBox.Text},
                .Es419 = New PS5ParamClass.Es419() With {.TitleName = ModifyValueTextBox.Text},
                .EsES = New PS5ParamClass.EsES() With {.TitleName = ModifyValueTextBox.Text},
                .FiFI = New PS5ParamClass.FiFI() With {.TitleName = ModifyValueTextBox.Text},
                .FrCA = New PS5ParamClass.FrCA() With {.TitleName = ModifyValueTextBox.Text},
                .FrFR = New PS5ParamClass.FrFR() With {.TitleName = ModifyValueTextBox.Text},
                .HuHU = New PS5ParamClass.HuHU() With {.TitleName = ModifyValueTextBox.Text},
                .IdID = New PS5ParamClass.IdID() With {.TitleName = ModifyValueTextBox.Text},
                .ItIT = New PS5ParamClass.ItIT() With {.TitleName = ModifyValueTextBox.Text},
                .JaJP = New PS5ParamClass.JaJP() With {.TitleName = ModifyValueTextBox.Text},
                .KoKR = New PS5ParamClass.KoKR() With {.TitleName = ModifyValueTextBox.Text},
                .NlNL = New PS5ParamClass.NlNL() With {.TitleName = ModifyValueTextBox.Text},
                .NoNO = New PS5ParamClass.NoNO() With {.TitleName = ModifyValueTextBox.Text},
                .PlPL = New PS5ParamClass.PlPL() With {.TitleName = ModifyValueTextBox.Text},
                .PtBR = New PS5ParamClass.PtBR() With {.TitleName = ModifyValueTextBox.Text},
                .PtPT = New PS5ParamClass.PtPT() With {.TitleName = ModifyValueTextBox.Text},
                .RoRO = New PS5ParamClass.RoRO() With {.TitleName = ModifyValueTextBox.Text},
                .RuRU = New PS5ParamClass.RuRU() With {.TitleName = ModifyValueTextBox.Text},
                .SvSE = New PS5ParamClass.SvSE() With {.TitleName = ModifyValueTextBox.Text},
                .ThTH = New PS5ParamClass.ThTH() With {.TitleName = ModifyValueTextBox.Text},
                .TrTR = New PS5ParamClass.TrTR() With {.TitleName = ModifyValueTextBox.Text},
                .ViVN = New PS5ParamClass.ViVN() With {.TitleName = ModifyValueTextBox.Text},
                .ZhHans = New PS5ParamClass.ZhHans() With {.TitleName = ModifyValueTextBox.Text},
                .ZhHant = New PS5ParamClass.ZhHant() With {.TitleName = ModifyValueTextBox.Text}}

                    SelectedParam.ParamValue = ModifyValueTextBox.Text

                Case "MasterVersion"
                    CurrentParamJson.MasterVersion = ModifyValueTextBox.Text
                    SelectedParam.ParamValue = ModifyValueTextBox.Text
                Case "TitleId"
                    If ModifyValueTextBox.Text.Length = 9 Then
                        CurrentParamJson.TitleId = ModifyValueTextBox.Text
                        SelectedParam.ParamValue = ModifyValueTextBox.Text
                    Else
                        MsgBox("Please enter a correct TitleId like CUSA99999", MsgBoxStyle.Exclamation, "TitleId not in correct format")
                    End If
            End Select

            ParamsListView.Items.Refresh()

        End If
    End Sub

    Private Sub AddParamButton_Click(sender As Object, e As RoutedEventArgs) Handles AddParamButton.Click

        For Each ParameterItem As PS5ParamClass.ParamListViewItem In ParamsListView.Items

            If ParameterItem.ParamName = ParamsComboBox.Text Then
                MsgBox("Parameter already exists.", MsgBoxStyle.Exclamation)
                Exit For
            Else
                Select Case ParameterItem.ParamName
                    Case "AgeLevel"

                        'Will be updated on next release
                        MsgBox("Not completely supported yet. Will be added with a default value of 0 years.", MsgBoxStyle.Information)

                        CurrentParamJson.AgeLevel = New PS5ParamClass.AgeLevel() With {.[Default] = 0, .US = 0}
                        ParamsListView.Items.Add(New PS5ParamClass.ParamListViewItem() With {.ParamName = "AgeLevel", .ParamType = "Object", .ParamValue = "0"})

                    Case "ApplicationCategoryType"

                        If IsInt(ParamValueTextBox.Text) Then
                            CurrentParamJson.ApplicationCategoryType = CInt(ParamValueTextBox.Text)
                            ParamsListView.Items.Add(New PS5ParamClass.ParamListViewItem() With {.ParamName = "ApplicationCategoryType", .ParamType = "Integer", .ParamValue = ParamValueTextBox.Text})
                        Else
                            MsgBox("Only numbers are allowed.", MsgBoxStyle.Exclamation, "Integer value required")
                        End If

                    Case "ApplicationDrmType"

                        Select Case ParamValueTextBox.Text
                            Case "standard", "demo", "upgradable", "free"
                                CurrentParamJson.ApplicationDrmType = ParamValueTextBox.Text
                                ParamsListView.Items.Add(New PS5ParamClass.ParamListViewItem() With {.ParamName = "ApplicationDrmType", .ParamType = "String", .ParamValue = ParamValueTextBox.Text})
                            Case Else
                                MsgBox("Only standard, demo, upgradable & free are currently available.", MsgBoxStyle.Exclamation, "Other value required")
                        End Select

                    Case "Attribute"

                        If IsInt(ParamValueTextBox.Text) Then
                            CurrentParamJson.Attribute = CInt(ParamValueTextBox.Text)
                            ParamsListView.Items.Add(New PS5ParamClass.ParamListViewItem() With {.ParamName = "Attribute", .ParamType = "Integer", .ParamValue = ParamValueTextBox.Text})
                        Else
                            MsgBox("Only numbers are allowed.", MsgBoxStyle.Exclamation, "Integer value required")
                        End If

                    Case "Attribute2"

                        If IsInt(ParamValueTextBox.Text) Then
                            CurrentParamJson.Attribute2 = CInt(ParamValueTextBox.Text)
                            ParamsListView.Items.Add(New PS5ParamClass.ParamListViewItem() With {.ParamName = "Attribute2", .ParamType = "Integer", .ParamValue = ParamValueTextBox.Text})
                        Else
                            MsgBox("Only numbers are allowed.", MsgBoxStyle.Exclamation, "Integer value required")
                        End If

                    Case "Attribute3"

                        If IsInt(ParamValueTextBox.Text) Then
                            CurrentParamJson.Attribute3 = CInt(ParamValueTextBox.Text)
                            ParamsListView.Items.Add(New PS5ParamClass.ParamListViewItem() With {.ParamName = "Attribute3", .ParamType = "Integer", .ParamValue = ParamValueTextBox.Text})
                        Else
                            MsgBox("Only numbers are allowed.", MsgBoxStyle.Exclamation, "Integer value required")
                        End If

                    Case "ConceptId"

                        CurrentParamJson.ConceptId = ParamValueTextBox.Text
                        ParamsListView.Items.Add(New PS5ParamClass.ParamListViewItem() With {.ParamName = "ConceptId", .ParamType = "String", .ParamValue = ParamValueTextBox.Text})

                    Case "ContentBadgeType"

                        If IsInt(ParamValueTextBox.Text) Then
                            CurrentParamJson.ContentBadgeType = CInt(ParamValueTextBox.Text)
                            ParamsListView.Items.Add(New PS5ParamClass.ParamListViewItem() With {.ParamName = "ContentBadgeType", .ParamType = "Integer", .ParamValue = ParamValueTextBox.Text})
                        Else
                            MsgBox("Only numbers are allowed.", MsgBoxStyle.Exclamation, "Integer value required")
                        End If

                    Case "ContentId"

                        If ParamValueTextBox.Text.Length = 36 Then
                            CurrentParamJson.ContentId = ParamValueTextBox.Text
                            ParamsListView.Items.Add(New PS5ParamClass.ParamListViewItem() With {.ParamName = "ContentId", .ParamType = "String", .ParamValue = ParamValueTextBox.Text})
                        Else
                            MsgBox("Please enter a correct ContentID like IV9999-CUSA99999_00-XXXXXXXXXXXXXXXX", MsgBoxStyle.Exclamation, "ContentID not in correct format")
                        End If

                    Case "ContentVersion"

                        CurrentParamJson.ContentVersion = ParamValueTextBox.Text
                        ParamsListView.Items.Add(New PS5ParamClass.ParamListViewItem() With {.ParamName = "ContentVersion", .ParamType = "String", .ParamValue = ParamValueTextBox.Text})

                    Case "DownloadDataSize"

                        If IsInt(ParamValueTextBox.Text) Then
                            CurrentParamJson.DownloadDataSize = CInt(ParamValueTextBox.Text)
                            ParamsListView.Items.Add(New PS5ParamClass.ParamListViewItem() With {.ParamName = "DownloadDataSize", .ParamType = "Integer", .ParamValue = ParamValueTextBox.Text})
                        Else
                            MsgBox("Only numbers are allowed.", MsgBoxStyle.Exclamation, "Integer value required")
                        End If

                    Case "DeeplinkUri"

                        CurrentParamJson.DeeplinkUri = ParamValueTextBox.Text
                        ParamsListView.Items.Add(New PS5ParamClass.ParamListViewItem() With {.ParamName = "DeeplinkUri", .ParamType = "String", .ParamValue = ParamValueTextBox.Text})

                    Case "LocalizedParameters"

                        'Will be updated on next release
                        MsgBox("Not completely supported yet. The default language will be set to 'enUS' with with the Title Name: " + ParamValueTextBox.Text, MsgBoxStyle.Information)

                        CurrentParamJson.LocalizedParameters = New PS5ParamClass.LocalizedParameters() With {.DefaultLanguage = "en-US",
                .EnUS = New PS5ParamClass.EnUS() With {.TitleName = ParamValueTextBox.Text},
                .ArAE = New PS5ParamClass.ArAE() With {.TitleName = ParamValueTextBox.Text},
                .CsCZ = New PS5ParamClass.CsCZ() With {.TitleName = ParamValueTextBox.Text},
                .DaDK = New PS5ParamClass.DaDK() With {.TitleName = ParamValueTextBox.Text},
                .DeDE = New PS5ParamClass.DeDE() With {.TitleName = ParamValueTextBox.Text},
                .ElGR = New PS5ParamClass.ElGR() With {.TitleName = ParamValueTextBox.Text},
                .EnGB = New PS5ParamClass.EnGB() With {.TitleName = ParamValueTextBox.Text},
                .Es419 = New PS5ParamClass.Es419() With {.TitleName = ParamValueTextBox.Text},
                .EsES = New PS5ParamClass.EsES() With {.TitleName = ParamValueTextBox.Text},
                .FiFI = New PS5ParamClass.FiFI() With {.TitleName = ParamValueTextBox.Text},
                .FrCA = New PS5ParamClass.FrCA() With {.TitleName = ParamValueTextBox.Text},
                .FrFR = New PS5ParamClass.FrFR() With {.TitleName = ParamValueTextBox.Text},
                .HuHU = New PS5ParamClass.HuHU() With {.TitleName = ParamValueTextBox.Text},
                .IdID = New PS5ParamClass.IdID() With {.TitleName = ParamValueTextBox.Text},
                .ItIT = New PS5ParamClass.ItIT() With {.TitleName = ParamValueTextBox.Text},
                .JaJP = New PS5ParamClass.JaJP() With {.TitleName = ParamValueTextBox.Text},
                .KoKR = New PS5ParamClass.KoKR() With {.TitleName = ParamValueTextBox.Text},
                .NlNL = New PS5ParamClass.NlNL() With {.TitleName = ParamValueTextBox.Text},
                .NoNO = New PS5ParamClass.NoNO() With {.TitleName = ParamValueTextBox.Text},
                .PlPL = New PS5ParamClass.PlPL() With {.TitleName = ParamValueTextBox.Text},
                .PtBR = New PS5ParamClass.PtBR() With {.TitleName = ParamValueTextBox.Text},
                .PtPT = New PS5ParamClass.PtPT() With {.TitleName = ParamValueTextBox.Text},
                .RoRO = New PS5ParamClass.RoRO() With {.TitleName = ParamValueTextBox.Text},
                .RuRU = New PS5ParamClass.RuRU() With {.TitleName = ParamValueTextBox.Text},
                .SvSE = New PS5ParamClass.SvSE() With {.TitleName = ParamValueTextBox.Text},
                .ThTH = New PS5ParamClass.ThTH() With {.TitleName = ParamValueTextBox.Text},
                .TrTR = New PS5ParamClass.TrTR() With {.TitleName = ParamValueTextBox.Text},
                .ViVN = New PS5ParamClass.ViVN() With {.TitleName = ParamValueTextBox.Text},
                .ZhHans = New PS5ParamClass.ZhHans() With {.TitleName = ParamValueTextBox.Text},
                .ZhHant = New PS5ParamClass.ZhHant() With {.TitleName = ParamValueTextBox.Text}}

                        ParamsListView.Items.Add(New PS5ParamClass.ParamListViewItem() With {.ParamName = "LocalizedParameters", .ParamType = "Object", .ParamValue = ParamValueTextBox.Text})

                    Case "MasterVersion"

                        CurrentParamJson.MasterVersion = ParamValueTextBox.Text
                        ParamsListView.Items.Add(New PS5ParamClass.ParamListViewItem() With {.ParamName = "MasterVersion", .ParamType = "String", .ParamValue = ParamValueTextBox.Text})

                    Case "TitleId"

                        If ParamValueTextBox.Text.Length = 9 Then
                            CurrentParamJson.TitleId = ParamValueTextBox.Text
                            ParamsListView.Items.Add(New PS5ParamClass.ParamListViewItem() With {.ParamName = "TitleId", .ParamType = "String", .ParamValue = ParamValueTextBox.Text})
                        Else
                            MsgBox("Please enter a correct TitleId like CUSA99999", MsgBoxStyle.Exclamation, "TitleId not in correct format")
                        End If

                End Select
            End If

        Next

        ParamsListView.Items.Refresh()

    End Sub

    Private Sub SaveMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles SaveMenuItem.Click
        If CurrentParamJson IsNot Nothing Then
            Dim SFD As New SaveFileDialog() With {.Filter = "param.json (*.json)|*.json", .OverwritePrompt = True, .Title = "Select a save location"}
            If SFD.ShowDialog() = Forms.DialogResult.OK Then
                Dim rawDataJSON As String = JsonConvert.SerializeObject(CurrentParamJson)
                File.WriteAllText(SFD.FileName, rawDataJSON)
                MsgBox("File saved!", MsgBoxStyle.Information)
            End If
        Else
            MsgBox("No param.json file loaded!", MsgBoxStyle.Exclamation)
        End If
    End Sub

    Private Sub LoadPSDevWikiMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles LoadPSDevWikiMenuItem.Click
        Process.Start("https://www.psdevwiki.com/ps5/Param.json")
    End Sub

    Private Function IsInt(Input As String) As Boolean
        Dim DigitOnly As New Regex("^\d+$")
        Return DigitOnly.Match(Input).Success
    End Function

End Class
