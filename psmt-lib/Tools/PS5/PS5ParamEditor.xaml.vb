﻿Imports System.IO
Imports psmt_lib.PS5ParamClass
Imports psmt_lib.Utils
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Forms
Imports Newtonsoft.Json

Public Class PS5ParamEditor

    Dim CurrentParamJsonPath As String = String.Empty
    Public CurrentParamJson As PS5Param = Nothing

    Private Sub NewParamMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles NewParamMenuItem.Click

        'Clear previous data
        ParamsListView.Items.Clear()
        CurrentParamJsonPath = String.Empty

        Dim NewPS5Param As New PS5Param() With {
            .AgeLevel = New AgeLevel() With {
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
            .DownloadDataSize = 0,
            .LocalizedParameters = New LocalizedParameters() With {
                .DefaultLanguage = "en-US",
                .EnUS = New EnUS() With {.TitleName = "Title Name"},
                .ArAE = New ArAE() With {.TitleName = "Title Name"},
                .CsCZ = New CsCZ() With {.TitleName = "Title Name"},
                .DaDK = New DaDK() With {.TitleName = "Title Name"},
                .DeDE = New DeDE() With {.TitleName = "Title Name"},
                .ElGR = New ElGR() With {.TitleName = "Title Name"},
                .EnGB = New EnGB() With {.TitleName = "Title Name"},
                .Es419 = New Es419() With {.TitleName = "Title Name"},
                .EsES = New EsES() With {.TitleName = "Title Name"},
                .FiFI = New FiFI() With {.TitleName = "Title Name"},
                .FrCA = New FrCA() With {.TitleName = "Title Name"},
                .FrFR = New FrFR() With {.TitleName = "Title Name"},
                .HuHU = New HuHU() With {.TitleName = "Title Name"},
                .IdID = New IdID() With {.TitleName = "Title Name"},
                .ItIT = New ItIT() With {.TitleName = "Title Name"},
                .JaJP = New JaJP() With {.TitleName = "Title Name"},
                .KoKR = New KoKR() With {.TitleName = "Title Name"},
                .NlNL = New NlNL() With {.TitleName = "Title Name"},
                .NoNO = New NoNO() With {.TitleName = "Title Name"},
                .PlPL = New PlPL() With {.TitleName = "Title Name"},
                .PtBR = New PtBR() With {.TitleName = "Title Name"},
                .PtPT = New PtPT() With {.TitleName = "Title Name"},
                .RoRO = New RoRO() With {.TitleName = "Title Name"},
                .RuRU = New RuRU() With {.TitleName = "Title Name"},
                .SvSE = New SvSE() With {.TitleName = "Title Name"},
                .ThTH = New ThTH() With {.TitleName = "Title Name"},
                .TrTR = New TrTR() With {.TitleName = "Title Name"},
                .ViVN = New ViVN() With {.TitleName = "Title Name"},
                .ZhHans = New ZhHans() With {.TitleName = "Title Name"},
                .ZhHant = New ZhHant() With {.TitleName = "Title Name"}},
            .MasterVersion = "01.00",
            .TitleId = "CUSA99999"}

        CurrentParamJson = NewPS5Param

        For Each Parameter In NewPS5Param.GetType().GetProperties()
            Dim NewParamType As String
            Dim NewParamValue As String = String.Empty
            Select Case Parameter.Name
                Case "AgeLevel"
                    NewParamType = "Object"
                    NewParamValue = "Open in advanced editor"
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
                    NewParamValue = "Open in advanced editor"
                Case "MasterVersion"
                    NewParamType = "String"
                Case "RequiredSystemSoftwareVersion"
                    NewParamType = "String"
                Case "TitleId"
                    NewParamType = "String"
                Case "VersionFileUri"
                    NewParamType = "String"
                Case Else
                    NewParamType = "Unknown"
            End Select

            'Add to ParamsListView
            Dim NewParamLVItem As New ParamListViewItem() With {.ParamName = Parameter.Name, .ParamType = NewParamType}
            If Parameter.GetValue(NewPS5Param, Nothing) IsNot Nothing Then

                If Not String.IsNullOrEmpty(NewParamValue) Then
                    NewParamLVItem.ParamValue = NewParamValue
                Else
                    NewParamLVItem.ParamValue = Parameter.GetValue(NewPS5Param, Nothing).ToString
                End If

                ParamsListView.Items.Add(NewParamLVItem)
            End If

        Next

        AddParamButton.IsEnabled = True
        SaveModifiedValueButton.IsEnabled = True
        RemoveParamButton.IsEnabled = True
    End Sub

    Private Sub LoadParamMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles LoadParamMenuItem.Click

        'Clear previous data
        ParamsListView.Items.Clear()

        Dim OFD As New OpenFileDialog() With {.Filter = "Param JSON (param.json)|param.json", .Title = "Please select a param.json file", .Multiselect = False}
        If OFD.ShowDialog() = Forms.DialogResult.OK Then

            'Read the file and create a PS5Param
            Dim JSONData As String = File.ReadAllText(OFD.FileName)
            Dim ParamData As PS5Param

            Try
                ParamData = JsonConvert.DeserializeObject(Of PS5Param)(JSONData)

                'Set current param for saving
                CurrentParamJson = ParamData
                CurrentParamJsonPath = OFD.FileName

                For Each Parameter In ParamData.GetType().GetProperties()
                    Dim NewParamType As String
                    Dim NewParamValue As String = String.Empty
                    Select Case Parameter.Name
                        Case "AgeLevel"
                            NewParamType = "Object"
                            NewParamValue = "Open in advanced editor"
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
                            NewParamValue = "Open in advanced editor"
                        Case "MasterVersion"
                            NewParamType = "String"
                        Case "RequiredSystemSoftwareVersion"
                            NewParamType = "String"
                        Case "TitleId"
                            NewParamType = "String"
                        Case "VersionFileUri"
                            NewParamType = "String"
                        Case Else
                            NewParamType = "Unknown"
                    End Select

                    'Add to ParamsListView
                    Dim NewParamLVItem As New ParamListViewItem() With {.ParamName = Parameter.Name, .ParamType = NewParamType}
                    If Parameter.GetValue(ParamData, Nothing) IsNot Nothing Then

                        If Not String.IsNullOrEmpty(NewParamValue) Then
                            NewParamLVItem.ParamValue = NewParamValue
                        Else
                            NewParamLVItem.ParamValue = Parameter.GetValue(ParamData, Nothing).ToString
                        End If

                        ParamsListView.Items.Add(NewParamLVItem)
                    End If
                Next

                AddParamButton.IsEnabled = True
                SaveModifiedValueButton.IsEnabled = True
                RemoveParamButton.IsEnabled = True

            Catch ex As Exception
                MsgBox("Could not parse the selected param.json file.", MsgBoxStyle.Critical, "Error")
            End Try
        End If
    End Sub

    Private Sub ParamsListView_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles ParamsListView.SelectionChanged
        If ParamsListView.SelectedItem IsNot Nothing Then
            Dim SelectedParam As ParamListViewItem = CType(ParamsListView.SelectedItem, ParamListViewItem)

            If Not String.IsNullOrEmpty(SelectedParam.ParamValue) Then
                ModifyValueTextBox.Text = SelectedParam.ParamValue
            Else
                ModifyValueTextBox.Text = ""
            End If

            Select Case SelectedParam.ParamName
                Case "AgeLevel"
                    AdvancedEditorButton.IsEnabled = True
                    SaveModifiedValueButton.IsEnabled = False
                Case "LocalizedParameters"
                    AdvancedEditorButton.IsEnabled = True
                    SaveModifiedValueButton.IsEnabled = False
                Case Else
                    AdvancedEditorButton.IsEnabled = False
                    SaveModifiedValueButton.IsEnabled = True
            End Select

        End If
    End Sub

    Private Sub SaveModifiedValueButton_Click(sender As Object, e As RoutedEventArgs) Handles SaveModifiedValueButton.Click
        If ParamsListView.SelectedItem IsNot Nothing And Not String.IsNullOrEmpty(ModifyValueTextBox.Text) Then

            Dim SelectedParam As ParamListViewItem = CType(ParamsListView.SelectedItem, ParamListViewItem)

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

                Case "MasterVersion"

                    CurrentParamJson.MasterVersion = ModifyValueTextBox.Text
                    SelectedParam.ParamValue = ModifyValueTextBox.Text

                Case "RequiredSystemSoftwareVersion"

                    If IsHex(ModifyValueTextBox.Text) Then
                        CurrentParamJson.RequiredSystemSoftwareVersion = ModifyValueTextBox.Text
                        SelectedParam.ParamValue = ModifyValueTextBox.Text
                    Else
                        MsgBox("Please enter a correct RequiredSystemSoftwareVersion like 0x0114000000000000", MsgBoxStyle.Exclamation, "RequiredSystemSoftwareVersion not in correct format")
                    End If

                Case "TitleId"
                    If ModifyValueTextBox.Text.Length = 9 Then
                        CurrentParamJson.TitleId = ModifyValueTextBox.Text
                        SelectedParam.ParamValue = ModifyValueTextBox.Text
                    Else
                        MsgBox("Please enter a correct TitleId like CUSA99999", MsgBoxStyle.Exclamation, "TitleId not in correct format")
                    End If

                Case "VersionFileUri"

                    CurrentParamJson.VersionFileUri = ModifyValueTextBox.Text
                    SelectedParam.ParamValue = ModifyValueTextBox.Text

            End Select

            ParamsListView.Items.Refresh()
            MsgBox("Value updated. Do not forget to save the changes.", MsgBoxStyle.Information)

        End If
    End Sub

    Private Sub AddParamButton_Click(sender As Object, e As RoutedEventArgs) Handles AddParamButton.Click
        If CurrentParamJson IsNot Nothing And ParamsComboBox.SelectedItem IsNot Nothing Then

            Dim SelectedParam As String = ParamsComboBox.Text

            For Each ParameterItem In ParamsListView.Items

                Dim ParamLVItem As ParamListViewItem = CType(ParameterItem, ParamListViewItem)

                If ParamLVItem.ParamName = SelectedParam Then
                    MsgBox("Parameter already exists.", MsgBoxStyle.Exclamation)
                    Exit For
                Else
                    Select Case ParamsComboBox.Text
                        Case "AgeLevel"

                            'Will be updated on next release
                            MsgBox("Not completely supported yet. Will be added with a default value of 0 years.", MsgBoxStyle.Information)

                            CurrentParamJson.AgeLevel = New AgeLevel() With {.[Default] = 0, .US = 0}
                            ParamsListView.Items.Add(New ParamListViewItem() With {.ParamName = "AgeLevel", .ParamType = "Object", .ParamValue = "0"})
                            Exit For
                        Case "ApplicationCategoryType"

                            If IsInt(ParamValueTextBox.Text) Then
                                CurrentParamJson.ApplicationCategoryType = CInt(ParamValueTextBox.Text)
                                ParamsListView.Items.Add(New ParamListViewItem() With {.ParamName = "ApplicationCategoryType", .ParamType = "Integer", .ParamValue = ParamValueTextBox.Text})
                            Else
                                MsgBox("Only numbers are allowed.", MsgBoxStyle.Exclamation, "Integer value required")
                            End If
                            Exit For
                        Case "ApplicationDrmType"

                            Select Case ParamValueTextBox.Text
                                Case "standard", "demo", "upgradable", "free"
                                    CurrentParamJson.ApplicationDrmType = ParamValueTextBox.Text
                                    ParamsListView.Items.Add(New ParamListViewItem() With {.ParamName = "ApplicationDrmType", .ParamType = "String", .ParamValue = ParamValueTextBox.Text})
                                Case Else
                                    MsgBox("Only standard, demo, upgradable & free are currently available.", MsgBoxStyle.Exclamation, "Other value required")
                            End Select
                            Exit For
                        Case "Attribute"

                            If IsInt(ParamValueTextBox.Text) Then
                                CurrentParamJson.Attribute = CInt(ParamValueTextBox.Text)
                                ParamsListView.Items.Add(New ParamListViewItem() With {.ParamName = "Attribute", .ParamType = "Integer", .ParamValue = ParamValueTextBox.Text})
                            Else
                                MsgBox("Only numbers are allowed.", MsgBoxStyle.Exclamation, "Integer value required")
                            End If
                            Exit For
                        Case "Attribute2"

                            If IsInt(ParamValueTextBox.Text) Then
                                CurrentParamJson.Attribute2 = CInt(ParamValueTextBox.Text)
                                ParamsListView.Items.Add(New ParamListViewItem() With {.ParamName = "Attribute2", .ParamType = "Integer", .ParamValue = ParamValueTextBox.Text})
                            Else
                                MsgBox("Only numbers are allowed.", MsgBoxStyle.Exclamation, "Integer value required")
                            End If
                            Exit For
                        Case "Attribute3"

                            If IsInt(ParamValueTextBox.Text) Then
                                CurrentParamJson.Attribute3 = CInt(ParamValueTextBox.Text)
                                ParamsListView.Items.Add(New ParamListViewItem() With {.ParamName = "Attribute3", .ParamType = "Integer", .ParamValue = ParamValueTextBox.Text})
                            Else
                                MsgBox("Only numbers are allowed.", MsgBoxStyle.Exclamation, "Integer value required")
                            End If
                            Exit For
                        Case "ConceptId"

                            CurrentParamJson.ConceptId = ParamValueTextBox.Text
                            ParamsListView.Items.Add(New ParamListViewItem() With {.ParamName = "ConceptId", .ParamType = "String", .ParamValue = ParamValueTextBox.Text})
                            Exit For
                        Case "ContentBadgeType"

                            If IsInt(ParamValueTextBox.Text) Then
                                CurrentParamJson.ContentBadgeType = CInt(ParamValueTextBox.Text)
                                ParamsListView.Items.Add(New ParamListViewItem() With {.ParamName = "ContentBadgeType", .ParamType = "Integer", .ParamValue = ParamValueTextBox.Text})
                            Else
                                MsgBox("Only numbers are allowed.", MsgBoxStyle.Exclamation, "Integer value required")
                            End If
                            Exit For
                        Case "ContentId"

                            If ParamValueTextBox.Text.Length = 36 Then
                                CurrentParamJson.ContentId = ParamValueTextBox.Text
                                ParamsListView.Items.Add(New ParamListViewItem() With {.ParamName = "ContentId", .ParamType = "String", .ParamValue = ParamValueTextBox.Text})
                            Else
                                MsgBox("Please enter a correct ContentID like IV9999-CUSA99999_00-XXXXXXXXXXXXXXXX", MsgBoxStyle.Exclamation, "ContentID not in correct format")
                            End If
                            Exit For
                        Case "ContentVersion"

                            CurrentParamJson.ContentVersion = ParamValueTextBox.Text
                            ParamsListView.Items.Add(New ParamListViewItem() With {.ParamName = "ContentVersion", .ParamType = "String", .ParamValue = ParamValueTextBox.Text})
                            Exit For
                        Case "DownloadDataSize"

                            If IsInt(ParamValueTextBox.Text) Then
                                CurrentParamJson.DownloadDataSize = CInt(ParamValueTextBox.Text)
                                ParamsListView.Items.Add(New ParamListViewItem() With {.ParamName = "DownloadDataSize", .ParamType = "Integer", .ParamValue = ParamValueTextBox.Text})
                            Else
                                MsgBox("Only numbers are allowed.", MsgBoxStyle.Exclamation, "Integer value required")
                            End If
                            Exit For
                        Case "DeeplinkUri"

                            CurrentParamJson.DeeplinkUri = ParamValueTextBox.Text
                            ParamsListView.Items.Add(New ParamListViewItem() With {.ParamName = "DeeplinkUri", .ParamType = "String", .ParamValue = ParamValueTextBox.Text})
                            Exit For
                        Case "LocalizedParameters"

                            'Will be updated on next release
                            MsgBox("Not completely supported yet. The default language will be set to 'enUS' with with the Title Name: " + ParamValueTextBox.Text, MsgBoxStyle.Information)

                            CurrentParamJson.LocalizedParameters = New LocalizedParameters() With {.DefaultLanguage = "en-US",
                    .EnUS = New EnUS() With {.TitleName = ParamValueTextBox.Text},
                    .ArAE = New ArAE() With {.TitleName = ParamValueTextBox.Text},
                    .CsCZ = New CsCZ() With {.TitleName = ParamValueTextBox.Text},
                    .DaDK = New DaDK() With {.TitleName = ParamValueTextBox.Text},
                    .DeDE = New DeDE() With {.TitleName = ParamValueTextBox.Text},
                    .ElGR = New ElGR() With {.TitleName = ParamValueTextBox.Text},
                    .EnGB = New EnGB() With {.TitleName = ParamValueTextBox.Text},
                    .Es419 = New Es419() With {.TitleName = ParamValueTextBox.Text},
                    .EsES = New EsES() With {.TitleName = ParamValueTextBox.Text},
                    .FiFI = New FiFI() With {.TitleName = ParamValueTextBox.Text},
                    .FrCA = New FrCA() With {.TitleName = ParamValueTextBox.Text},
                    .FrFR = New FrFR() With {.TitleName = ParamValueTextBox.Text},
                    .HuHU = New HuHU() With {.TitleName = ParamValueTextBox.Text},
                    .IdID = New IdID() With {.TitleName = ParamValueTextBox.Text},
                    .ItIT = New ItIT() With {.TitleName = ParamValueTextBox.Text},
                    .JaJP = New JaJP() With {.TitleName = ParamValueTextBox.Text},
                    .KoKR = New KoKR() With {.TitleName = ParamValueTextBox.Text},
                    .NlNL = New NlNL() With {.TitleName = ParamValueTextBox.Text},
                    .NoNO = New NoNO() With {.TitleName = ParamValueTextBox.Text},
                    .PlPL = New PlPL() With {.TitleName = ParamValueTextBox.Text},
                    .PtBR = New PtBR() With {.TitleName = ParamValueTextBox.Text},
                    .PtPT = New PtPT() With {.TitleName = ParamValueTextBox.Text},
                    .RoRO = New RoRO() With {.TitleName = ParamValueTextBox.Text},
                    .RuRU = New RuRU() With {.TitleName = ParamValueTextBox.Text},
                    .SvSE = New SvSE() With {.TitleName = ParamValueTextBox.Text},
                    .ThTH = New ThTH() With {.TitleName = ParamValueTextBox.Text},
                    .TrTR = New TrTR() With {.TitleName = ParamValueTextBox.Text},
                    .ViVN = New ViVN() With {.TitleName = ParamValueTextBox.Text},
                    .ZhHans = New ZhHans() With {.TitleName = ParamValueTextBox.Text},
                    .ZhHant = New ZhHant() With {.TitleName = ParamValueTextBox.Text}}

                            ParamsListView.Items.Add(New ParamListViewItem() With {.ParamName = "LocalizedParameters", .ParamType = "Object", .ParamValue = ParamValueTextBox.Text})
                            Exit For
                        Case "MasterVersion"

                            CurrentParamJson.MasterVersion = ParamValueTextBox.Text
                            ParamsListView.Items.Add(New ParamListViewItem() With {.ParamName = "MasterVersion", .ParamType = "String", .ParamValue = ParamValueTextBox.Text})
                            Exit For
                        Case "RequiredSystemSoftwareVersion"

                            If IsHex(ParamValueTextBox.Text) Then
                                CurrentParamJson.RequiredSystemSoftwareVersion = ParamValueTextBox.Text
                                ParamsListView.Items.Add(New ParamListViewItem() With {.ParamName = "RequiredSystemSoftwareVersion", .ParamType = "String", .ParamValue = ParamValueTextBox.Text})
                            Else
                                MsgBox("Please enter a correct RequiredSystemSoftwareVersion like 0x0114000000000000", MsgBoxStyle.Exclamation, "RequiredSystemSoftwareVersion not in correct format")
                            End If
                            Exit For
                        Case "TitleId"

                            If ParamValueTextBox.Text.Length = 9 Then
                                CurrentParamJson.TitleId = ParamValueTextBox.Text
                                ParamsListView.Items.Add(New ParamListViewItem() With {.ParamName = "TitleId", .ParamType = "String", .ParamValue = ParamValueTextBox.Text})
                            Else
                                MsgBox("Please enter a correct TitleId like CUSA99999", MsgBoxStyle.Exclamation, "TitleId not in correct format")
                            End If
                            Exit For
                        Case "VersionFileUri"

                            CurrentParamJson.VersionFileUri = ParamValueTextBox.Text
                            ParamsListView.Items.Add(New ParamListViewItem() With {.ParamName = "VersionFileUri", .ParamType = "String", .ParamValue = ParamValueTextBox.Text})
                            Exit For
                    End Select

                End If

            Next

            ParamsListView.Items.Refresh()
        End If
    End Sub

    Private Sub SaveMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles SaveMenuItem.Click
        If CurrentParamJson IsNot Nothing Then
            Dim SFD As New SaveFileDialog() With {.Filter = "param.json (*.json)|*.json", .OverwritePrompt = True, .Title = "Select a save location"}
            If SFD.ShowDialog() = Forms.DialogResult.OK Then

                Try
                    Dim RawDataJSON As String = JsonConvert.SerializeObject(CurrentParamJson, Formatting.Indented, New JsonSerializerSettings With {.NullValueHandling = NullValueHandling.Ignore})
                    File.WriteAllText(SFD.FileName, RawDataJSON)
                    MsgBox("File saved!", MsgBoxStyle.Information)
                Catch ex As Exception
                    MsgBox("Cannot save the param.json file, please report the next error.", MsgBoxStyle.Critical, "Error")
                    MsgBox(ex.Message)
                    Exit Sub
                End Try

            End If
        Else
            MsgBox("No param.json file loaded!", MsgBoxStyle.Exclamation)
        End If
    End Sub

    Private Sub LoadPSDevWikiMenuItem_Click(sender As Object, e As RoutedEventArgs) Handles LoadPSDevWikiMenuItem.Click
        Process.Start("https://www.psdevwiki.com/ps5/Param.json")
    End Sub

    Private Sub RemoveParamButton_Click(sender As Object, e As RoutedEventArgs) Handles RemoveParamButton.Click
        If ParamsListView.SelectedItem IsNot Nothing Then
            If CurrentParamJson IsNot Nothing Then

                Dim SelectedParam As ParamListViewItem = CType(ParamsListView.SelectedItem, ParamListViewItem)

                'Remove from param.json
                Select Case SelectedParam.ParamName
                    Case "AgeLevel"
                        CurrentParamJson.AgeLevel = Nothing
                        MsgBox("AgeLevel removed from param.json. Do not forget to save the changes.", MsgBoxStyle.Information)
                    Case "ApplicationCategoryType"
                        CurrentParamJson.ApplicationCategoryType = Nothing
                        MsgBox("ApplicationCategoryType removed from param.json. Do not forget to save the changes.", MsgBoxStyle.Information)
                    Case "ApplicationDrmType"
                        CurrentParamJson.ApplicationDrmType = Nothing
                        MsgBox("ApplicationDrmType removed from param.json. Do not forget to save the changes.", MsgBoxStyle.Information)
                    Case "Attribute"
                        CurrentParamJson.Attribute = Nothing
                        MsgBox("Attribute removed from param.json. Do not forget to save the changes.", MsgBoxStyle.Information)
                    Case "Attribute2"
                        CurrentParamJson.Attribute2 = Nothing
                        MsgBox("Attribute2 removed from param.json. Do not forget to save the changes.", MsgBoxStyle.Information)
                    Case "Attribute3"
                        CurrentParamJson.Attribute3 = Nothing
                        MsgBox("Attribute3 removed from param.json. Do not forget to save the changes.", MsgBoxStyle.Information)
                    Case "ConceptId"
                        CurrentParamJson.ConceptId = Nothing
                        MsgBox("ConceptId removed from param.json. Do not forget to save the changes.", MsgBoxStyle.Information)
                    Case "ContentBadgeType"
                        CurrentParamJson.ContentBadgeType = Nothing
                        MsgBox("ContentBadgeType removed from param.json. Do not forget to save the changes.", MsgBoxStyle.Information)
                    Case "ContentId"
                        CurrentParamJson.ContentId = Nothing
                        MsgBox("ContentId removed from param.json. Do not forget to save the changes.", MsgBoxStyle.Information)
                    Case "ContentVersion"
                        CurrentParamJson.ContentVersion = Nothing
                        MsgBox("ContentVersion removed from param.json. Do not forget to save the changes.", MsgBoxStyle.Information)
                    Case "DownloadDataSize"
                        CurrentParamJson.DownloadDataSize = Nothing
                        MsgBox("DownloadDataSize removed from param.json. Do not forget to save the changes.", MsgBoxStyle.Information)
                    Case "DeeplinkUri"
                        CurrentParamJson.DeeplinkUri = Nothing
                        MsgBox("DeeplinkUri removed from param.json. Do not forget to save the changes.", MsgBoxStyle.Information)
                    Case "LocalizedParameters"
                        CurrentParamJson.LocalizedParameters = Nothing
                        MsgBox("LocalizedParameters removed from param.json. Do not forget to save the changes.", MsgBoxStyle.Information)
                    Case "MasterVersion"
                        CurrentParamJson.MasterVersion = Nothing
                        MsgBox("MasterVersion removed from param.json. Do not forget to save the changes.", MsgBoxStyle.Information)
                    Case "RequiredSystemSoftwareVersion"
                        CurrentParamJson.RequiredSystemSoftwareVersion = Nothing
                        MsgBox("RequiredSystemSoftwareVersion removed from param.json. Do not forget to save the changes.", MsgBoxStyle.Information)
                    Case "TitleId"
                        CurrentParamJson.TitleId = Nothing
                        MsgBox("TitleId removed from param.json. Do not forget to save the changes.", MsgBoxStyle.Information)
                    Case "VersionFileUri"
                        CurrentParamJson.VersionFileUri = Nothing
                        MsgBox("VersionFileUri removed from param.json. Do not forget to save the changes.", MsgBoxStyle.Information)
                End Select

                'Remove from the ParamsListView
                ParamsListView.Items.Remove(ParamsListView.SelectedItem)

            Else
                MsgBox("No param.json file loaded!", MsgBoxStyle.Exclamation)
            End If
        Else
            MsgBox("No parameter selected.", MsgBoxStyle.Exclamation)
        End If
    End Sub

    Private Sub AdvancedEditorButton_Click(sender As Object, e As RoutedEventArgs) Handles AdvancedEditorButton.Click
        If ParamsListView.SelectedItem IsNot Nothing Then

            Dim SelectedParam As ParamListViewItem = CType(ParamsListView.SelectedItem, ParamListViewItem)
            Dim NewAdvParamEditor As New PS5ParamAdvanced() With {.CurrentParamJsonPath = CurrentParamJsonPath, .CurrentParamJson = CurrentParamJson}

            Select Case SelectedParam.ParamName
                Case "AgeLevel"
                    NewAdvParamEditor.TitleTextBlock.Text = "Modifying ageLevel"
                    NewAdvParamEditor.AdvancedParam = "AgeLevel"
                Case "LocalizedParameters"
                    NewAdvParamEditor.TitleTextBlock.Text = "Modifying localizedParameters"
                    NewAdvParamEditor.AdvancedParam = "LocalizedParameters"
            End Select

            NewAdvParamEditor.Show()
        End If

    End Sub

End Class
