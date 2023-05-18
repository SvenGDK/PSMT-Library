Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Threading
Imports System.Windows.Media.Imaging

Public Class Utils

    Public Declare Auto Function PlaySound Lib "winmm.dll" (pszSound As String, hmod As IntPtr, fdwSound As Integer) As Boolean
    Public Declare Auto Function PlaySound Lib "winmm.dll" (pszSound As Byte(), hmod As IntPtr, fdwSound As PlaySoundFlags) As Boolean

    Public Enum PlaySoundFlags As Integer
        SND_SYNC = 0
        SND_ASYNC = 1
        SND_NODEFAULT = 2
        SND_MEMORY = 4
        SND_LOOP = 8
        SND_NOSTOP = 16
        SND_NOWAIT = 8192
        SND_FILENAME = 131072
        SND_RESOURCE = 262148
    End Enum

    Public Shared Sub PlaySND(SoundFile As String)
        Dim SoundFileInfo As New FileInfo(SoundFile)
        PlaySound(SoundFileInfo.FullName, IntPtr.Zero, PlaySoundFlags.SND_ASYNC)
    End Sub

    Public Shared Sub PlaySND(SoundData As Byte())
        PlaySound(SoundData, IntPtr.Zero, PlaySoundFlags.SND_ASYNC Or PlaySoundFlags.SND_MEMORY)
    End Sub

    Public Shared Sub StopSND()
        'Set NULL to stop playing
        PlaySound(Nothing, New IntPtr(), PlaySoundFlags.SND_NODEFAULT)
    End Sub

    Public Shared Function IncrementArray(ByRef SourceArray As Byte(), Position As Integer) As Boolean
        If SourceArray(Position) = &HFF Then
            If Position <> 0 Then
                If IncrementArray(SourceArray, Position - 1) Then
                    SourceArray(Position) = &H0
                    Return True
                Else
                    Return False
                End If
            Else
                Return False
            End If
        Else
            SourceArray(Position) += CByte(&H1)
            Return True
        End If
    End Function

    Public Shared Function HexStringToAscii(HexString As String, CleanEndOfString As Boolean) As String
        Dim ascii As String

        Try
            Dim str = ""

            While HexString.Length > 0
                str += Convert.ToChar(Convert.ToUInt32(HexString.Substring(0, 2), 16)).ToString()
                HexString = HexString.Substring(2, HexString.Length - 2)
            End While

            If CleanEndOfString Then str = str.Replace(vbNullChar, "")
            ascii = str
        Catch ex As Exception
            ascii = Nothing
        End Try

        Return ascii
    End Function

    Public Shared Function ByteArrayToAscii(ByteArray As Byte(), StartPos As Integer, Length As Integer, CleanEndOfString As Boolean) As String
        Dim NumArray As Byte() = New Byte(Length - 1 + 1 - 1) {}
        Array.Copy(ByteArray, StartPos, numArray, 0, numArray.Length)
        Return HexStringToAscii(ByteArrayToHexString(numArray), CleanEndOfString)
    End Function

    Public Shared Function ByteArrayToHexString(ByteArray As Byte()) As String
        Dim HexString = ""
        Dim Num As Integer = ByteArray.Length - 1
        Dim Index = 0

        While index <= num
            hexString += ByteArray(index).ToString("X2")
            index += 1
        End While

        Return hexString
    End Function

    Public Shared Function DirSize(SourceDir As String, Recurse As Boolean) As Long
        Dim Size As Long = 0
        Dim FileEntries As String() = Directory.GetFiles(SourceDir)

        For Each FileName As String In FileEntries
            Interlocked.Add(Size, New FileInfo(FileName).Length)
        Next

        If Recurse Then
            Dim SubdirEntries As String() = Directory.GetDirectories(SourceDir)
            Parallel.[For](Of Long)(0, subdirEntries.Length, Function() 0, Function(i, [loop], subtotal)

                                                                               If (File.GetAttributes(subdirEntries(i)) And FileAttributes.ReparsePoint) <> FileAttributes.ReparsePoint Then
                                                                                   subtotal += DirSize(subdirEntries(i), True)
                                                                                   Return subtotal
                                                                               End If

                                                                               Return 0
                                                                           End Function, Function(x) Interlocked.Add(size, x))
        End If

        Return Size
    End Function

    Public Shared Sub CreateWorkingDirectories()
        If Not Directory.Exists(My.Computer.FileSystem.CurrentDirectory + "\Downloads") Then
            With Directory.CreateDirectory(My.Computer.FileSystem.CurrentDirectory + "\Downloads")
                .CreateSubdirectory(My.Computer.FileSystem.CurrentDirectory + "\exdata")
                .CreateSubdirectory(My.Computer.FileSystem.CurrentDirectory + "\pkgs")
            End With
        End If
        If Not Directory.Exists(My.Computer.FileSystem.CurrentDirectory + "\Extractions") Then
            Directory.CreateDirectory(My.Computer.FileSystem.CurrentDirectory + "\Extractions")
        End If
        If Not Directory.Exists(My.Computer.FileSystem.CurrentDirectory + "\Decryptions") Then
            Directory.CreateDirectory(My.Computer.FileSystem.CurrentDirectory + "\Decryptions")
        End If
    End Sub

    Public Shared Function GetFileSize(Size As Long) As String
        Dim DoubleBytes As Double
        Try
            Select Case Size
                Case Is >= 1099511627776
                    DoubleBytes = Size / 1099511627776 'TB
                    Return FormatNumber(DoubleBytes, 2) & " TB"
                Case 1073741824 To 1099511627775
                    DoubleBytes = Size / 1073741824 'GB
                    Return FormatNumber(DoubleBytes, 2) & " GB"
                Case 1048576 To 1073741823
                    DoubleBytes = Size / 1048576 'MB
                    Return FormatNumber(DoubleBytes, 2) & " MB"
                Case 1024 To 1048575
                    DoubleBytes = Size / 1024 'KB
                    Return FormatNumber(DoubleBytes, 2) & " KB"
                Case 0 To 1023
                    DoubleBytes = Size ' Bytes
                    Return FormatNumber(DoubleBytes, 2) & " Bytes"
                Case Else
                    Return ""
            End Select
        Catch
            Return ""
        End Try
    End Function

    Public Shared Function GetFileSizeAndDate(FileSize As String, TheDate As String) As Structures.PackageInfo
        Dim PKGSizeStr As Long
        Long.TryParse(FileSize.ToString.Trim, PKGSizeStr)

        Dim PKGDate As Date
        Date.TryParseExact(TheDate, "yyyy-MM-dd HH:mm:ss", Nothing, DateTimeStyles.None, PKGDate)

        Return New Structures.PackageInfo With {.FileSize = GetFileSize(PKGSizeStr), .FileDate = CStr(PKGDate.Date)}
    End Function

    Public Shared Function GetPKGTitleID(PKGFilePath As String) As String
        Dim PKGID As String = ""
        Try
            Dim NewStringBuilder As New StringBuilder
            If PKGFilePath.ToLower.EndsWith(".pkg") Then
                Using PKGBinaryReader As New BinaryReader(New StreamReader(PKGFilePath).BaseStream)
                    PKGBinaryReader.BaseStream.Position = &H30
                    Dim PKGBytes As Byte() = PKGBinaryReader.ReadBytes(36)
                    PKGBinaryReader.Close()
                    Dim ASCIIString As String = Encoding.ASCII.GetString(PKGBytes)
                    If ASCIIString.Trim.Replace(ChrW(0), "").Length >= 7 Then
                        NewStringBuilder.AppendLine(ASCIIString.Substring(7, 9))
                    Else
                        NewStringBuilder.AppendLine("XXXX#####")
                    End If
                End Using
                Return NewStringBuilder.ToString.Trim()
            Else
                Return PKGID
            End If

        Catch ex As Exception
            Return PKGID
        End Try
    End Function

    Public Shared Function BitmapSourceFromByteArray(Buffer As Byte()) As BitmapSource
        Dim NewBitmap As New BitmapImage()

        Using NewMemoryStream As New MemoryStream(Buffer)
            NewBitmap.BeginInit()
            NewBitmap.CacheOption = BitmapCacheOption.OnLoad
            NewBitmap.StreamSource = NewMemoryStream
            NewBitmap.EndInit()
        End Using

        NewBitmap.Freeze()
        Return NewBitmap
    End Function

    Public Shared Sub ReCreateDirectoryStructure(SourceDirectory As String, TargetDirectory As String, Optional RootDirectory As String = "")
        If String.IsNullOrEmpty(RootDirectory) Then
            RootDirectory = SourceDirectory
        End If
        Dim AllFolders() As String = Directory.GetDirectories(SourceDirectory)
        For Each Folder As String In AllFolders
            Directory.CreateDirectory(Folder.Replace(RootDirectory, TargetDirectory))
            ReCreateDirectoryStructure(Folder, TargetDirectory, RootDirectory)
        Next
    End Sub

    Public Shared Function GetBackupFolders(DestinationPath As String) As Structures.BackupFolders
        Dim DestinationBackupStructure As New Structures.BackupFolders()

        If Directory.Exists(DestinationPath + "GAMES") Then
            DestinationBackupStructure.IsGAMESPresent = True
        End If
        If Directory.Exists(DestinationPath + "GAMEZ") Then
            DestinationBackupStructure.IsGAMEZPresent = True
        End If
        If Directory.Exists(DestinationPath + "packages") Then
            DestinationBackupStructure.IspackagesPresent = True
        End If
        If Directory.Exists(DestinationPath + "exdata") Then
            DestinationBackupStructure.IsexdataPresent = True
        End If

        Return DestinationBackupStructure
    End Function

    Public Shared Function IsWindowOpen(WindowName As String) As Boolean
        Dim WinFound As Boolean = False
        For Each OpenWin In Windows.Application.Current.Windows()
            If OpenWin.ToString = "psmt_lib." + WindowName Then
                WinFound = True
                Exit For
            Else
                WinFound = False
            End If
        Next
        Return WinFound
    End Function

    Public Shared Function IsURLValid(Url As String) As Boolean
        Try
            Dim NewWebRequest As HttpWebRequest = CType(WebRequest.Create(Url), HttpWebRequest)
            Using WebRequestResponse As HttpWebResponse = CType(NewWebRequest.GetResponse(), HttpWebResponse)
                If WebRequestResponse.StatusCode = HttpStatusCode.OK Then
                    Return True
                ElseIf WebRequestResponse.StatusCode = HttpStatusCode.Found Then
                    Return True
                ElseIf WebRequestResponse.StatusCode = HttpStatusCode.NotFound Then
                    Return False
                ElseIf WebRequestResponse.StatusCode = HttpStatusCode.Unauthorized Then
                    Return False
                ElseIf WebRequestResponse.StatusCode = HttpStatusCode.Forbidden Then
                    Return False
                ElseIf WebRequestResponse.StatusCode = HttpStatusCode.BadGateway Then
                    Return False
                ElseIf WebRequestResponse.StatusCode = HttpStatusCode.BadRequest Then
                    Return False
                End If
                Return False
            End Using
        Catch Ex As Exception
            Return False
        End Try
    End Function

    Public Shared Function CleanTitle(Title As String) As String
        Return Title.Replace("¢", "").Replace("„", "").Replace("â", "").Replace("Â", "").Replace("Ô", "").Replace("Ê", "").Replace("ô", "").Replace("ê", "").Replace(",", "").Replace(";", "")
    End Function

End Class
