Imports System.IO
Imports System.Windows.Forms

Public Class VitaRCOMage

    Private Sub BrowseRCOFileButton_Click(sender As Object, e As Windows.RoutedEventArgs) Handles BrowseRCOFileButton.Click
        Dim OFD As New OpenFileDialog() With {.CheckFileExists = True, .Filter = "rco files (*.rco)|*.rco", .Multiselect = False}
        If OFD.ShowDialog() = Windows.Forms.DialogResult.OK Then
            SelectedRCOFileTextBox.Text = OFD.FileName
        End If
    End Sub

    Private Sub ExtractButton_Click(sender As Object, e As Windows.RoutedEventArgs) Handles ExtractButton.Click
        If Not String.IsNullOrEmpty(SelectedRCOFileTextBox.Text) And File.Exists(SelectedRCOFileTextBox.Text) Then
            Using SNGRE As New Process()
                SNGRE.StartInfo.FileName = My.Computer.FileSystem.CurrentDirectory + "\Tools\sngre.exe"
                SNGRE.StartInfo.Arguments = "-f " + """" + SelectedRCOFileTextBox.Text + """"
                SNGRE.StartInfo.RedirectStandardOutput = True
                SNGRE.StartInfo.RedirectStandardError = True
                SNGRE.StartInfo.UseShellExecute = False
                SNGRE.StartInfo.CreateNoWindow = True
                SNGRE.Start()

                'Read the output
                Dim OutputReader As StreamReader = SNGRE.StandardOutput
                Dim ProcessOutput As String = OutputReader.ReadToEnd()

                If ProcessOutput.Contains("Extracted:") Then
                    If MsgBox("RCO extracted! Do you want to open the folder ?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                        Process.Start("explorer", My.Computer.FileSystem.CurrentDirectory + "\Tools")
                    End If
                Else
                    MsgBox("Could not extract the selected .rco file", MsgBoxStyle.Critical)
                End If
            End Using
        End If

    End Sub

End Class
