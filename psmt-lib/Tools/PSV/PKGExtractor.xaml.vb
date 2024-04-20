Imports System.IO
Imports System.Net
Imports System.Windows.Forms

Public Class PKGExtractor

    Dim DownloadsList As New List(Of Structures.Package)()
    Dim SelectedPKGContentID As String

    Private Sub BrowsePKGButton_Click(sender As Object, e As Windows.RoutedEventArgs) Handles BrowsePKGButton.Click
        Dim OFD As New OpenFileDialog With {.Multiselect = False, .Filter = "", .CheckFileExists = True}
        If OFD.ShowDialog() = Windows.Forms.DialogResult.OK Then
            SelectedPKGTextBox.Text = OFD.FileName

            Using SFOReader As New Process()
                SFOReader.StartInfo.FileName = My.Computer.FileSystem.CurrentDirectory + "\Tools\PSN_get_pkg_info.exe"
                SFOReader.StartInfo.Arguments = """" + OFD.FileName + """"
                SFOReader.StartInfo.RedirectStandardOutput = True
                SFOReader.StartInfo.UseShellExecute = False
                SFOReader.StartInfo.CreateNoWindow = True
                SFOReader.Start()

                Dim OutputReader As StreamReader = SFOReader.StandardOutput
                Dim ProcessOutput As String() = OutputReader.ReadToEnd().Split(New String() {vbCrLf}, StringSplitOptions.RemoveEmptyEntries)

                If ProcessOutput.Count > 0 Then
                    'Load game infos
                    For Each Line In ProcessOutput
                        If Line.StartsWith("Content ID:") Then
                            SelectedPKGContentID = Line.Split(":"c)(1).Trim(""""c).Trim()
                            Exit For
                        End If
                    Next
                End If
            End Using
        End If
    End Sub

    Private Sub BrowseOutputFolderButton_Click(sender As Object, e As Windows.RoutedEventArgs) Handles BrowseOutputFolderButton.Click
        Dim FBD As New FolderBrowserDialog() With {.RootFolder = Environment.SpecialFolder.Desktop, .ShowNewFolderButton = True}
        If FBD.ShowDialog() = Windows.Forms.DialogResult.OK Then
            OutputFolderTextBox.Text = FBD.SelectedPath
        End If
    End Sub

    Private Async Sub GetzRIFKeyButton_Click(sender As Object, e As Windows.RoutedEventArgs) Handles GetzRIFKeyButton.Click
        If MsgBox("Load from the latest database ?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
            Using NewWebClient As New WebClient
                Dim GamesList As String = Await NewWebClient.DownloadStringTaskAsync(New Uri("https://nopaystation.com/tsv/PSV_GAMES.tsv"))
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
            If File.Exists(My.Computer.FileSystem.CurrentDirectory + "\Databases\PSV_GAMES.tsv") Then
                Dim FileReader As String() = File.ReadAllLines(My.Computer.FileSystem.CurrentDirectory + "\Databases\PSV_GAMES.tsv", Text.Encoding.UTF8)
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

        'Check if we have a zRIF for the selected .pkg
        For Each AvailablePKG As Structures.Package In DownloadsList
            If AvailablePKG.PackageContentID = SelectedPKGContentID Then
                If AvailablePKG.PackageRAP IsNot Nothing Then
                    zRIFTextBox.Text = AvailablePKG.PackageRAP
                End If
            End If
        Next
    End Sub

    Private Sub ExtractButton_Click(sender As Object, e As Windows.RoutedEventArgs) Handles ExtractButton.Click
        If Not String.IsNullOrEmpty(SelectedPKGTextBox.Text) And Not String.IsNullOrEmpty(OutputFolderTextBox.Text) And Not String.IsNullOrEmpty(zRIFTextBox.Text) Then
            If File.Exists(SelectedPKGTextBox.Text) And Directory.Exists(OutputFolderTextBox.Text) Then
                Using PKG2ZIP As New Process()
                    PKG2ZIP.StartInfo.FileName = My.Computer.FileSystem.CurrentDirectory + "\Tools\pkg2zip.exe"
                    PKG2ZIP.StartInfo.Arguments = "-x " + """" + SelectedPKGTextBox.Text + """ " + """" + zRIFTextBox.Text + """"
                    PKG2ZIP.StartInfo.RedirectStandardOutput = True
                    PKG2ZIP.StartInfo.RedirectStandardError = True
                    PKG2ZIP.StartInfo.UseShellExecute = False
                    PKG2ZIP.StartInfo.CreateNoWindow = True
                    PKG2ZIP.Start()

                    'Read the output
                    Dim OutputReader As StreamReader = PKG2ZIP.StandardOutput
                    Dim ProcessOutput As String = OutputReader.ReadToEnd()

                    If ProcessOutput.Contains("done!") Then
                        If MsgBox("PKG extracted! Do you want to open the folder containing the extracted folder ?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                            Process.Start("explorer", My.Computer.FileSystem.CurrentDirectory + "\Tools\app")
                        End If
                    Else
                        MsgBox("Could not extract the selected .pkg file.", MsgBoxStyle.Critical)
                    End If
                End Using
            End If
        End If
    End Sub

End Class
