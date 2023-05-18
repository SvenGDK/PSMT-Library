Imports System.IO
Imports System.Windows.Forms
Imports System.Windows.Media.Imaging

Public Class XMBGameProject

    Private Sub BrowseSaveFolderButton_Click(sender As Object, e As Windows.RoutedEventArgs) Handles BrowseSaveFolderButton.Click
        Dim FBD As New FolderBrowserDialog() With {.Description = "Please select a folder to save your game project.", .ShowNewFolderButton = True}
        If FBD.ShowDialog() = Windows.Forms.DialogResult.OK Then
            ProjectDirectoryTextBox.Text = FBD.SelectedPath
        End If
    End Sub

    Private Sub BrowseGameISOButton_Click(sender As Object, e As Windows.RoutedEventArgs) Handles BrowseGameISOButton.Click
        Dim OFD As New OpenFileDialog() With {.Title = "Choose your .iso file.", .Filter = "iso files (*.iso)|*.iso"}
        If OFD.ShowDialog() = Windows.Forms.DialogResult.OK Then
            ProjectISOFileTextBox.Text = OFD.FileName

            If MsgBox("Do you want to load the game ID from the disc ?" + vbCrLf + "The Game ID will be required to install the game.", MsgBoxStyle.YesNo, "Load Game ID") = MsgBoxResult.Yes Then
                Using SevenZip As New Process()
                    SevenZip.StartInfo.FileName = My.Computer.FileSystem.CurrentDirectory + "\Tools\7z.exe"
                    SevenZip.StartInfo.Arguments = "l -ba """ + OFD.FileName + """"
                    SevenZip.StartInfo.RedirectStandardOutput = True
                    SevenZip.StartInfo.UseShellExecute = False
                    SevenZip.StartInfo.CreateNoWindow = True
                    SevenZip.Start()

                    'Read the output
                    Dim OutputReader As StreamReader = SevenZip.StandardOutput
                    Dim ProcessOutput As String() = OutputReader.ReadToEnd().Split(New String() {vbCrLf}, StringSplitOptions.None)

                    For Each Line As String In ProcessOutput
                        If Line.Contains("SLES_") Or Line.Contains("SLUS_") Or Line.Contains("SCES_") Or Line.Contains("SCUS_") Then
                            If Line.Contains("Volume:") Then 'ID found in the ISO Header
                                ProjectIDTextBox.Text = Line.Split(New String() {"Volume: "}, StringSplitOptions.RemoveEmptyEntries)(1)
                                Exit For
                            Else 'ID found in the ISO files
                                ProjectIDTextBox.Text = String.Join(" ", Line.Split(New Char() {}, StringSplitOptions.RemoveEmptyEntries)).Split(" "c)(5).Trim()
                                Exit For
                            End If
                        End If
                    Next
                End Using
            End If
        End If
    End Sub

    Private Sub BrowseIconButton_Click(sender As Object, e As Windows.RoutedEventArgs) Handles BrowseIconButton.Click
        Dim OFD As New OpenFileDialog() With {.Title = "Choose your .ico file.", .Filter = "ico files (*.ico)|*.ico"}
        If OFD.ShowDialog() = Windows.Forms.DialogResult.OK Then
            ProjectIconPathTextBox.Text = OFD.FileName
        End If
    End Sub

    Private Sub AdvancedSettingsButton_Click(sender As Object, e As Windows.RoutedEventArgs) Handles AdvancedSettingsButton.Click
        Dim NewGameEditor As New XMBGameEditor() With {.ProjectDirectory = ProjectDirectoryTextBox.Text, .Title = "Game Ressources Editor - " + ProjectDirectoryTextBox.Text}

        If Directory.Exists(ProjectDirectoryTextBox.Text) AndAlso Directory.Exists(ProjectDirectoryTextBox.Text + "\res") Then

            If File.Exists(ProjectDirectoryTextBox.Text + "\res\jkt_001.png") Then
                NewGameEditor.Cover1Image.Source = New BitmapImage(New Uri(ProjectDirectoryTextBox.Text + "\res\jkt_001.png", UriKind.RelativeOrAbsolute))
                NewGameEditor.Cover1Image.Tag = ProjectDirectoryTextBox.Text + "\res\jkt_001.png"
            End If
            If File.Exists(ProjectDirectoryTextBox.Text + "\res\jkt_002.png") Then
                NewGameEditor.Cover2Image.Source = New BitmapImage(New Uri(ProjectDirectoryTextBox.Text + "\res\jkt_002.png", UriKind.RelativeOrAbsolute))
                NewGameEditor.Cover2Image.Tag = ProjectDirectoryTextBox.Text + "\res\jkt_002.png"
            End If
            If File.Exists(ProjectDirectoryTextBox.Text + "\res\image\0.png") Then
                NewGameEditor.BackgroundImage.Source = New BitmapImage(New Uri(ProjectDirectoryTextBox.Text + "\res\image\0.png", UriKind.RelativeOrAbsolute))
                NewGameEditor.ScreenshotImage1.Tag = ProjectDirectoryTextBox.Text + "\res\image\0.png"
            End If
            If File.Exists(ProjectDirectoryTextBox.Text + "\res\image\1.png") Then
                NewGameEditor.ScreenshotImage1.Source = New BitmapImage(New Uri(ProjectDirectoryTextBox.Text + "\res\image\1.png", UriKind.RelativeOrAbsolute))
                NewGameEditor.ScreenshotImage1.Tag = ProjectDirectoryTextBox.Text + "\res\image\1.png"
            End If
            If File.Exists(ProjectDirectoryTextBox.Text + "\res\image\2.png") Then
                NewGameEditor.ScreenshotImage2.Source = New BitmapImage(New Uri(ProjectDirectoryTextBox.Text + "\res\image\2.png", UriKind.RelativeOrAbsolute))
                NewGameEditor.ScreenshotImage1.Tag = ProjectDirectoryTextBox.Text + "\res\image\2.png"
            End If

            If File.Exists(ProjectDirectoryTextBox.Text + "\res\info.sys") Then
                Dim GameInfos As String() = File.ReadAllLines(ProjectDirectoryTextBox.Text + "\res\info.sys")
                NewGameEditor.GameTitleTextBox.Text = GameInfos(0).Split("="c)(1).Trim()
                NewGameEditor.GameIDTextBox.Text = GameInfos(1).Split("="c)(1).Replace("_", "-").Replace(".", "").Trim()
                If Not GameInfos(2).Split("="c)(1).Trim() = "0" Then
                    NewGameEditor.ShowGameIDCheckBox.IsChecked = True
                End If
                NewGameEditor.GameReleaseDateTextBox.Text = GameInfos(3).Split("="c)(1).Trim()
                NewGameEditor.GameDeveloperTextBox.Text = GameInfos(4).Split("="c)(1).Trim()
                NewGameEditor.GamePublisherTextBox.Text = GameInfos(5).Split("="c)(1).Trim()
                NewGameEditor.GameNoteTextBox.Text = GameInfos(6).Split("="c)(1).Trim()
                NewGameEditor.GameWebsiteTextBox.Text = GameInfos(7).Split("="c)(1).Trim()
                If Not GameInfos(13).Split("="c)(1).Trim() = "0" Then
                    NewGameEditor.ShowCopyrightCheckBox.IsChecked = True
                End If
                NewGameEditor.GameGenreTextBox.Text = GameInfos(14).Split("="c)(1).Trim()
                NewGameEditor.GameRegionTextBox.Text = GameInfos(18).Split("="c)(1).Trim()
            Else
                NewGameEditor.GameIDTextBox.Text = ProjectIDTextBox.Text.Replace("_", "-").Replace(".", "").Trim()
            End If

            NewGameEditor.Show()
        Else
            Directory.CreateDirectory(ProjectDirectoryTextBox.Text + "\res")
            NewGameEditor.GameIDTextBox.Text = ProjectIDTextBox.Text.Replace("_", "-").Replace(".", "").Trim()
            NewGameEditor.Show()
        End If
    End Sub

    Private Sub SaveProjectButton_Click(sender As Object, e As Windows.RoutedEventArgs) Handles SaveProjectButton.Click
        'Write Project settings to .CFG
        Using ProjectWriter As New StreamWriter(My.Computer.FileSystem.CurrentDirectory + "\Projects\" + ProjectNameTextBox.Text + ".CFG", False)
            ProjectWriter.WriteLine("TITLE=" + ProjectNameTextBox.Text)
            ProjectWriter.WriteLine("ID=" + ProjectIDTextBox.Text)
            ProjectWriter.WriteLine("DIR=" + ProjectDirectoryTextBox.Text)
            ProjectWriter.WriteLine("ELForISO=" + ProjectISOFileTextBox.Text)
            ProjectWriter.WriteLine("TYPE=GAME")
            ProjectWriter.WriteLine("SIGNED=FALSE")
        End Using

        'Write SYSTEM.CNF to project directory
        Using CNFWriter As New StreamWriter(ProjectDirectoryTextBox.Text + "\SYSTEM.CNF", False)
            CNFWriter.WriteLine("BOOT2 = pfs:/EXECUTE.KELF") 'Loads EXECUTE.KELF
            CNFWriter.WriteLine("VER = 1.01")
            CNFWriter.WriteLine("VMODE = NTSC")
            CNFWriter.WriteLine("HDDUNITPOWER = NICHDD")
        End Using

        'Write icon.sys to project directory
        Using CNFWriter As New StreamWriter(ProjectDirectoryTextBox.Text + "\icon.sys", False)
            CNFWriter.WriteLine("PS2X")
            CNFWriter.WriteLine("title0=" + ProjectTitleTextBox.Text)
            CNFWriter.WriteLine("title1=" + ProjectSubTitleTextBox.Text)
            CNFWriter.WriteLine("bgcola=0")
            CNFWriter.WriteLine("bgcol0=0,0,0")
            CNFWriter.WriteLine("bgcol1=0,0,0")
            CNFWriter.WriteLine("bgcol2=0,0,0")
            CNFWriter.WriteLine("bgcol3=0,0,0")
            CNFWriter.WriteLine("lightdir0=1.0,-1.0,1.0")
            CNFWriter.WriteLine("lightdir1=-1.0,1.0,-1.0")
            CNFWriter.WriteLine("lightdir2=0.0,0.0,0.0")
            CNFWriter.WriteLine("lightcolamb=64,64,64")
            CNFWriter.WriteLine("lightcol0=64,64,64")
            CNFWriter.WriteLine("lightcol1=16,16,16")
            CNFWriter.WriteLine("lightcol2=0,0,0")
            CNFWriter.WriteLine("uninstallmes0=" + ProjectUninstallMsgTextBox.Text)
            CNFWriter.WriteLine("uninstallmes1=")
            CNFWriter.WriteLine("uninstallmes2=")
        End Using

        'Copy selected ico to project folder
        If Not ProjectIconPathTextBox.Text = "" Then
            File.Copy(ProjectIconPathTextBox.Text, ProjectDirectoryTextBox.Text + "\list.ico")
        End If

        If MsgBox("Game Project saved. Close this window ?", MsgBoxStyle.YesNo, "Project saved") = MsgBoxResult.Yes Then
            For Each OpenWin In Windows.Application.Current.Windows()
                If OpenWin.ToString = "psmt_lib.XMBInstaller" Then
                    Dim OpenXMBInstaller As XMBInstaller = CType(OpenWin, XMBInstaller)
                    OpenXMBInstaller.ReloadProjects()
                    Exit For
                End If
            Next
            Close()
        Else
            For Each OpenWin In Windows.Application.Current.Windows()
                If OpenWin.ToString = "psmt_lib.XMBInstaller" Then
                    Dim OpenXMBInstaller As XMBInstaller = CType(OpenWin, XMBInstaller)
                    OpenXMBInstaller.ReloadProjects()
                    Exit For
                End If
            Next
        End If
    End Sub

End Class
