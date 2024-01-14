Imports System.IO
Imports FluentFTP
Imports System.Security.Authentication
Imports System.Windows
Imports psmt_lib.INI

Public Class PS5etaHENConfigurator

    Public ConsoleIP As String

    Private Sub GetConfigButton_Click(sender As Object, e As Windows.RoutedEventArgs) Handles GetConfigButton.Click
        If Not Directory.Exists(My.Computer.FileSystem.CurrentDirectory + "\Cache") Then Directory.CreateDirectory(My.Computer.FileSystem.CurrentDirectory + "\Cache")

        If Not String.IsNullOrEmpty(ConsoleIP) Then
            Try
                Using conn As New FtpClient(ConsoleIP, "anonymous", "anonymous", 1337)
                    'Configurate the FTP connection
                    conn.Config.EncryptionMode = FtpEncryptionMode.None
                    conn.Config.SslProtocols = SslProtocols.None
                    conn.Config.DataConnectionEncryption = False

                    'Connect
                    conn.Connect()

                    'Get config.ini
                    conn.DownloadFile(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini", "/data/etaHEN/config.ini", FtpLocalExists.Overwrite)

                    'Disconnect
                    conn.Disconnect()
                End Using

                'Load config after download
                If File.Exists(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini") Then
                    Dim ConfigInfo As New FileInfo(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini")
                    Dim ConfigDateTime As Date = ConfigInfo.LastWriteTime
                    ConfigStatusTextBlock.Text = "etaHEN config found" + " - " + ConfigDateTime.ToString()

                    Dim etaHENConfig As New IniFile(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini")
                    If Not String.IsNullOrEmpty(etaHENConfig.IniReadValue("Settings", "libhijacker_cheats")) Then
                        If etaHENConfig.IniReadValue("Settings", "libhijacker_cheats") = "0" Then
                            UseCheatsCheckBox.IsChecked = False
                        Else
                            UseCheatsCheckBox.IsChecked = True
                        End If
                    End If
                    If Not String.IsNullOrEmpty(etaHENConfig.IniReadValue("Settings", "PS5Debug")) Then
                        If etaHENConfig.IniReadValue("Settings", "PS5Debug") = "0" Then
                            PS5DebugAutoLoadCheckBox.IsChecked = False
                        Else
                            PS5DebugAutoLoadCheckBox.IsChecked = True
                        End If
                    End If
                    If Not String.IsNullOrEmpty(etaHENConfig.IniReadValue("Settings", "FTP")) Then
                        If etaHENConfig.IniReadValue("Settings", "FTP") = "0" Then
                            FTPCheckBox.IsChecked = False
                        Else
                            FTPCheckBox.IsChecked = True
                        End If
                    End If
                    If Not String.IsNullOrEmpty(etaHENConfig.IniReadValue("Settings", "launch_itemzflow")) Then
                        If etaHENConfig.IniReadValue("Settings", "launch_itemzflow") = "0" Then
                            LaunchItemzflowCheckBox.IsChecked = False
                        Else
                            LaunchItemzflowCheckBox.IsChecked = True
                        End If
                    End If
                    If Not String.IsNullOrEmpty(etaHENConfig.IniReadValue("Settings", "discord_rpc")) Then
                        If etaHENConfig.IniReadValue("Settings", "discord_rpc") = "0" Then
                            DiscordCheckBox.IsChecked = False
                        Else
                            DiscordCheckBox.IsChecked = True
                        End If
                    End If
                End If

                MsgBox("Config file retrieved!" + vbCrLf + "You can change the checkboxes now and reupload the config.ini", MsgBoxStyle.Information)
            Catch ex As Exception
                MsgBox("Could not get the config.ini, please verify your connection.", MsgBoxStyle.Exclamation)
            End Try
        End If
    End Sub

    Private Sub SaveAndUploadButton_Click(sender As Object, e As Windows.RoutedEventArgs) Handles SaveAndUploadButton.Click
        If File.Exists(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini") Then
            Try
                Using conn As New FtpClient(ConsoleIP, "anonymous", "anonymous", 1337)
                    'Configurate the FTP connection
                    conn.Config.EncryptionMode = FtpEncryptionMode.None
                    conn.Config.SslProtocols = SslProtocols.None
                    conn.Config.DataConnectionEncryption = False

                    'Connect
                    conn.Connect()

                    'Upload config.ini
                    conn.UploadFile(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini", "/data/etaHEN/config.ini", FtpRemoteExists.NoCheck)

                    'Disconnect
                    conn.Disconnect()
                End Using

                MsgBox("etaHEN config uploaded & updated on the PS5!", MsgBoxStyle.Information)
            Catch ex As Exception
                MsgBox("Could upload the config.ini, please verify your connection.", MsgBoxStyle.Exclamation)
            End Try
        Else
            MsgBox("No local config.ini file found.", MsgBoxStyle.Exclamation)
        End If
    End Sub

    Private Sub PS5etaHENConfigurator_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        'Load existing config
        If File.Exists(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini") Then
            Dim etaHENConfig As New IniFile(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini")
            Dim ConfigInfo As New FileInfo(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini")
            Dim ConfigDateTime As Date = ConfigInfo.LastWriteTime
            ConfigStatusTextBlock.Text = "etaHEN config found" + " - " + ConfigDateTime.ToString()

            If Not String.IsNullOrEmpty(etaHENConfig.IniReadValue("Settings", "libhijacker_cheats")) Then
                If etaHENConfig.IniReadValue("Settings", "libhijacker_cheats") = "0" Then
                    UseCheatsCheckBox.IsChecked = False
                Else
                    UseCheatsCheckBox.IsChecked = True
                End If
            End If
            If Not String.IsNullOrEmpty(etaHENConfig.IniReadValue("Settings", "PS5Debug")) Then
                If etaHENConfig.IniReadValue("Settings", "PS5Debug") = "0" Then
                    PS5DebugAutoLoadCheckBox.IsChecked = False
                Else
                    PS5DebugAutoLoadCheckBox.IsChecked = True
                End If
            End If
            If Not String.IsNullOrEmpty(etaHENConfig.IniReadValue("Settings", "FTP")) Then
                If etaHENConfig.IniReadValue("Settings", "FTP") = "0" Then
                    FTPCheckBox.IsChecked = False
                Else
                    FTPCheckBox.IsChecked = True
                End If
            End If
            If Not String.IsNullOrEmpty(etaHENConfig.IniReadValue("Settings", "launch_itemzflow")) Then
                If etaHENConfig.IniReadValue("Settings", "launch_itemzflow") = "0" Then
                    LaunchItemzflowCheckBox.IsChecked = False
                Else
                    LaunchItemzflowCheckBox.IsChecked = True
                End If
            End If
            If Not String.IsNullOrEmpty(etaHENConfig.IniReadValue("Settings", "discord_rpc")) Then
                If etaHENConfig.IniReadValue("Settings", "discord_rpc") = "0" Then
                    DiscordCheckBox.IsChecked = False
                Else
                    DiscordCheckBox.IsChecked = True
                End If
            End If
        End If
    End Sub

#Region "Check changes"

    Private Sub DiscordCheckBox_Checked(sender As Object, e As RoutedEventArgs) Handles DiscordCheckBox.Checked
        If File.Exists(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini") Then
            Dim etaHENConfig As New IniFile(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini")
            etaHENConfig.IniWriteValue("Settings", "discord_rpc", "1")
        End If
    End Sub

    Private Sub DiscordCheckBox_Unchecked(sender As Object, e As RoutedEventArgs) Handles DiscordCheckBox.Unchecked
        If File.Exists(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini") Then
            Dim etaHENConfig As New IniFile(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini")
            etaHENConfig.IniWriteValue("Settings", "discord_rpc", "0")
        End If
    End Sub

    Private Sub FTPCheckBox_Checked(sender As Object, e As RoutedEventArgs) Handles FTPCheckBox.Checked
        If File.Exists(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini") Then
            Dim etaHENConfig As New IniFile(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini")
            etaHENConfig.IniWriteValue("Settings", "FTP", "1")
        End If
    End Sub

    Private Sub FTPCheckBox_Unchecked(sender As Object, e As RoutedEventArgs) Handles FTPCheckBox.Unchecked
        If File.Exists(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini") Then
            Dim etaHENConfig As New IniFile(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini")
            etaHENConfig.IniWriteValue("Settings", "FTP", "0")
        End If
    End Sub

    Private Sub LaunchItemzflowCheckBox_Checked(sender As Object, e As RoutedEventArgs) Handles LaunchItemzflowCheckBox.Checked
        If File.Exists(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini") Then
            Dim etaHENConfig As New IniFile(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini")
            etaHENConfig.IniWriteValue("Settings", "launch_itemzflow", "1")
        End If
    End Sub

    Private Sub LaunchItemzflowCheckBox_Unchecked(sender As Object, e As RoutedEventArgs) Handles LaunchItemzflowCheckBox.Unchecked
        If File.Exists(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini") Then
            Dim etaHENConfig As New IniFile(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini")
            etaHENConfig.IniWriteValue("Settings", "launch_itemzflow", "0")
        End If
    End Sub

    Private Sub PS5DebugAutoLoadCheckBox_Checked(sender As Object, e As RoutedEventArgs) Handles PS5DebugAutoLoadCheckBox.Checked
        If File.Exists(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini") Then
            Dim etaHENConfig As New IniFile(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini")
            etaHENConfig.IniWriteValue("Settings", "PS5Debug", "1")
        End If
    End Sub

    Private Sub PS5DebugAutoLoadCheckBox_Unchecked(sender As Object, e As RoutedEventArgs) Handles PS5DebugAutoLoadCheckBox.Unchecked
        If File.Exists(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini") Then
            Dim etaHENConfig As New IniFile(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini")
            etaHENConfig.IniWriteValue("Settings", "PS5Debug", "0")
        End If
    End Sub

    Private Sub UseCheatsCheckBox_Checked(sender As Object, e As RoutedEventArgs) Handles UseCheatsCheckBox.Checked
        If File.Exists(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini") Then
            Dim etaHENConfig As New IniFile(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini")
            etaHENConfig.IniWriteValue("Settings", "libhijacker_cheats", "1")
        End If
    End Sub

    Private Sub UseCheatsCheckBox_Unchecked(sender As Object, e As RoutedEventArgs) Handles UseCheatsCheckBox.Unchecked
        If File.Exists(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini") Then
            Dim etaHENConfig As New IniFile(My.Computer.FileSystem.CurrentDirectory + "\Cache\config.ini")
            etaHENConfig.IniWriteValue("Settings", "libhijacker_cheats", "0")
        End If
    End Sub

#End Region

End Class
