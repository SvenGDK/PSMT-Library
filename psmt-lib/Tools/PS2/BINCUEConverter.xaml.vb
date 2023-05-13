Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Windows.Forms

Public Class BINCUEConverter

    Public NewBaseName As String = ""

    Private Sub BrowseButton_Click(sender As Object, e As Windows.RoutedEventArgs) Handles BrowseButton.Click
        Dim OFD As New OpenFileDialog() With {.CheckFileExists = True, .Filter = "BIN files (*.bin)|*.bin|CUE files (*.cue)|*.cue", .Multiselect = False}
        If OFD.ShowDialog() = Windows.Forms.DialogResult.OK Then
            SelectedCueOrBinTextBox.Text = OFD.FileName
        End If
    End Sub

    Private Sub ConverToISOOutputDataRecieved(sender As Object, e As DataReceivedEventArgs)
        If Not String.IsNullOrEmpty(e.Data) Then
            If Dispatcher.CheckAccess() = False Then
                Dispatcher.BeginInvoke(Sub() ConvertStatusTextBlock.Text = e.Data)
            End If
            If e.Data.Contains("End of Conversion") Then
                If File.Exists(NewBaseName + "01.iso") Then
                    File.Move(NewBaseName + "01.iso", My.Computer.FileSystem.CurrentDirectory + "\Converted\ISO\" + NewBaseName + "01.iso")
                    If MsgBox("Converted ! Do you want the open the folder containing the new ISO file ?", MsgBoxStyle.YesNo, "Completed") = MsgBoxResult.Yes Then
                        Process.Start("explorer", My.Computer.FileSystem.CurrentDirectory + "\Converted\ISO")
                    End If
                Else
                    If MsgBox("Converted, but the file could not be found. Do you want to check the Tools folder ?", MsgBoxStyle.YesNo, "Completed") = MsgBoxResult.Yes Then
                        Process.Start("explorer", My.Computer.FileSystem.CurrentDirectory + "\Tools")
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub ConvertButton_Click(sender As Object, e As Windows.RoutedEventArgs) Handles ConvertButton.Click
        If Not String.IsNullOrEmpty(SelectedCueOrBinTextBox.Text) And File.Exists(SelectedCueOrBinTextBox.Text) Then

            Dim NewRegexReplace As New Regex("(?<=\().*(?=\))")
            Dim RegexReplaceStr As String = ""

            NewBaseName = NewRegexReplace.Replace(Path.GetFileNameWithoutExtension(SelectedCueOrBinTextBox.Text), RegexReplaceStr, RegexOptions.IgnoreCase)
            NewBaseName = NewBaseName.Replace("()", "").Trim()

            Dim BINFile As String = ""
            Dim CUEFile As String = ""

            If Path.GetExtension(SelectedCueOrBinTextBox.Text) = ".bin" Then
                CUEFile = Path.ChangeExtension(SelectedCueOrBinTextBox.Text, ".cue")
                BINFile = SelectedCueOrBinTextBox.Text
            ElseIf Path.GetExtension(SelectedCueOrBinTextBox.Text) = ".cue" Then
                CUEFile = SelectedCueOrBinTextBox.Text
                BINFile = Path.ChangeExtension(SelectedCueOrBinTextBox.Text, ".bin")
            End If

            'Create Converted folder if not exists
            If Not Directory.Exists(My.Computer.FileSystem.CurrentDirectory + "\Converted") Then
                Directory.CreateDirectory(My.Computer.FileSystem.CurrentDirectory + "\Converted")
                If Not Directory.Exists(My.Computer.FileSystem.CurrentDirectory + "\Converted\ISO") Then
                    Directory.CreateDirectory(My.Computer.FileSystem.CurrentDirectory + "\Converted\ISO")
                End If
            End If

            Using BChunk As New Process()
                BChunk.StartInfo.FileName = My.Computer.FileSystem.CurrentDirectory + "\Tools\bchunk.exe"
                If IsForPSXCheckBox.IsChecked Then
                    BChunk.StartInfo.Arguments = "-p """ + BINFile + """ """ + CUEFile + """ """ + NewBaseName + """"
                Else
                    BChunk.StartInfo.Arguments = """" + BINFile + """ """ + CUEFile + """ """ + NewBaseName + """"
                End If
                BChunk.StartInfo.RedirectStandardOutput = True
                BChunk.StartInfo.RedirectStandardError = True
                BChunk.StartInfo.UseShellExecute = False
                BChunk.StartInfo.CreateNoWindow = True
                BChunk.EnableRaisingEvents = True

                AddHandler BChunk.OutputDataReceived, AddressOf ConverToISOOutputDataRecieved

                BChunk.Start()
                BChunk.BeginOutputReadLine()
            End Using
        End If
    End Sub

End Class
