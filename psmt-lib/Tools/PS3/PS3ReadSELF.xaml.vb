﻿Imports System.IO
Imports System.Windows
Imports System.Windows.Forms

Public Class PS3ReadSELF

    Private Sub BrowseButton_Click(sender As Object, e As Windows.RoutedEventArgs) Handles BrowseButton.Click
        Dim OFD As New OpenFileDialog() With {.CheckFileExists = True, .Multiselect = False}
        If OFD.ShowDialog() = Forms.DialogResult.OK Then
            SelectedFileTextBox.Text = OFD.FileName
        End If
    End Sub

    Private Sub ReadSelfButton_Click(sender As Object, e As Windows.RoutedEventArgs) Handles ReadSelfButton.Click
        If Not String.IsNullOrEmpty(SelectedFileTextBox.Text) And File.Exists(SelectedFileTextBox.Text) Then
            Dim ProcessOutput As String = ""
            Using ReadSelf As New Process()
                ReadSelf.StartInfo.FileName = My.Computer.FileSystem.CurrentDirectory + "\Tools\readself.exe"
                ReadSelf.StartInfo.Arguments = """" + SelectedFileTextBox.Text + """"
                ReadSelf.StartInfo.RedirectStandardOutput = True
                ReadSelf.StartInfo.UseShellExecute = False
                ReadSelf.StartInfo.CreateNoWindow = True
                ReadSelf.Start()

                'Read the output
                Dim OutputReader As StreamReader = ReadSelf.StandardOutput
                ProcessOutput = OutputReader.ReadToEnd()
            End Using
            OutputTextBox.Text += ProcessOutput
        End If
    End Sub

End Class
