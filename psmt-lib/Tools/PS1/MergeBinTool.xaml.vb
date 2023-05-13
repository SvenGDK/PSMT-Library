Imports System.IO
Imports System.Windows.Forms

Public Class MergeBinTool

    Private Sub BrowseCUEFilesButton_Click(sender As Object, e As Windows.RoutedEventArgs) Handles BrowseCUEFilesButton.Click
        Dim OFD As New OpenFileDialog() With {.CheckFileExists = True, .Multiselect = True, .Filter = "cue files (*.cue)|*.cue"}
        If OFD.ShowDialog() = Windows.Forms.DialogResult.OK Then

            If OFD.FileNames.Count > 1 Then
                For Each SelectedCUE In OFD.FileNames
                    Dim NewCUELVItem As New ListViewItem() With {.Text = SelectedCUE}
                    CUEsListView.Items.Add(NewCUELVItem)
                Next
            Else
                Dim NewCUELVItem As New ListViewItem() With {.Text = OFD.FileName}
                CUEsListView.Items.Add(NewCUELVItem)
            End If

        End If
    End Sub

    Private Sub MergeSelectedButton_Click(sender As Object, e As Windows.RoutedEventArgs) Handles MergeSelectedButton.Click
        If CUEsListView.SelectedItem IsNot Nothing Then
            Cursor = Windows.Input.Cursors.Wait

            Dim SelectedCUEFile As ListViewItem = CUEsListView.SelectedItem
            Dim NewBaseNameTitle As String = Path.GetFileNameWithoutExtension(SelectedCUEFile.Text) + "_merged"
            Dim OutputPath As String = Path.GetDirectoryName(SelectedCUEFile.Text)

            If MergeBINs(SelectedCUEFile.Text, NewBaseNameTitle) = True Then
                If MsgBox("The .bin files have been merged for the selected .cue file." + vbCrLf + "Open the folder ?", MsgBoxStyle.YesNo) Then
                    Process.Start("explorer", OutputPath)
                End If
            Else
                MsgBox("Could not merge the .bin files for the selected game.", MsgBoxStyle.Critical)
            End If

            Cursor = Windows.Input.Cursors.Arrow
        End If
    End Sub

    Private Sub MergeAllButton_Click(sender As Object, e As Windows.RoutedEventArgs) Handles MergeAllButton.Click

        Dim FailCount As Integer = 0

        If Not CUEsListView.Items.Count = 0 Then
            Cursor = Windows.Input.Cursors.Wait

            For Each CUE As ListViewItem In CUEsListView.Items
                Dim NewBaseNameTitle As String = Path.GetFileNameWithoutExtension(CUE.Text) + "_merged"
                If MergeBINs(CUE.Text, NewBaseNameTitle) = False Then
                    FailCount += 1
                End If
            Next

            Cursor = Windows.Input.Cursors.Arrow

            If Not FailCount = 0 Then
                MsgBox("Could not merge some .bin files. Please check all games.", MsgBoxStyle.Critical)
            End If
        End If
    End Sub

    Public Function MergeBINs(CueFile As String, NewBaseName As String) As Boolean
        Using BINMerge As New Process()
            BINMerge.StartInfo.FileName = My.Computer.FileSystem.CurrentDirectory + "\Tools\binmerge.exe"
            BINMerge.StartInfo.Arguments = """" + CueFile + """ " + """" + NewBaseName + """"
            BINMerge.StartInfo.RedirectStandardOutput = True
            BINMerge.StartInfo.RedirectStandardError = True
            BINMerge.StartInfo.UseShellExecute = False
            BINMerge.StartInfo.CreateNoWindow = True
            BINMerge.Start()

            'Read the output
            Dim OutputReader As StreamReader = BINMerge.StandardOutput
            Dim ProcessOutput As String = OutputReader.ReadToEnd()

            If ProcessOutput.Contains("Wrote new cue:") Then
                Return True
            Else
                Return False
            End If
        End Using
    End Function

End Class
