Imports System.IO
Imports System.Text

Public Class SFONew

    Public Shared Function ReadSFO(ParamSFO As Stream) As Dictionary(Of String, Object)
        Dim SFOValues As New Dictionary(Of String, Object)()
        Dim NewSFOContent As New Structures.ParamSFOContent()
        Dim Magic As UInteger = ReadUInt32(ParamSFO)
        Dim Version As UInteger = ReadUInt32(ParamSFO)
        Dim KeyOffset As UInteger = ReadUInt32(ParamSFO)
        Dim ValueOffset As UInteger = ReadUInt32(ParamSFO)
        Dim Count As UInteger = ReadUInt32(ParamSFO)

        If Magic = 1179865088 Then
            For i As Integer = 0 To CInt(Count - 1)
                Dim NameOffset As UShort = ReadUInt16(ParamSFO)
                Dim Alignment As Byte = CByte(ParamSFO.ReadByte())
                Dim Type As Byte = CByte(ParamSFO.ReadByte())
                Dim ValueSize As UInteger = ReadUInt32(ParamSFO)
                Dim TotalSize As UInteger = ReadUInt32(ParamSFO)
                Dim DataOffset As UInteger = ReadUInt32(ParamSFO)
                Dim KeyLocation As Integer = Convert.ToInt32(KeyOffset + NameOffset)
                Dim KeyName As String = ReadStringAt(ParamSFO, KeyLocation)
                Dim ValueLocation As Integer = Convert.ToInt32(ValueOffset + DataOffset)
                Dim Value As Object = "Unknown Type"

                Select Case Type
                    Case 2
                        Value = ReadStringAt(ParamSFO, ValueLocation)
                        NewSFOContent.ParamType = 2
                    Case 4
                        Value = ReadUint32At(ParamSFO, ValueLocation + i)
                        NewSFOContent.ParamType = 4
                    Case 0
                        Value = ReadBytesAt(ParamSFO, ValueLocation + i, Convert.ToInt32(ValueSize))
                        NewSFOContent.ParamType = 2
                End Select

                SFOValues(KeyName) = Value
            Next
        End If

        Return SFOValues
    End Function

    Public Shared Function ReadSFO(ParamSFO As Byte()) As Dictionary(Of String, Object)
        Dim SFOStream As New MemoryStream(ParamSFO)
        Return ReadSFO(SFOStream)
    End Function

    Public Shared Sub CopyString(str As Byte(), Text As String, Index As Integer)
        Dim TextBytes As Byte() = Encoding.UTF8.GetBytes(Text)
        Array.ConstrainedCopy(TextBytes, 0, str, Index, TextBytes.Length)
    End Sub

    Public Shared Sub CopyInt32(Str As Byte(), Value As Integer, Index As Integer)
        Dim ValueBytes As Byte() = BitConverter.GetBytes(Value)
        Array.ConstrainedCopy(ValueBytes, 0, Str, Index, ValueBytes.Length)
    End Sub

    Public Shared Sub CopyInt32BE(Str As Byte(), Value As Integer, Index As Integer)
        Dim ValueBytes As Byte() = BitConverter.GetBytes(Value)
        Dim ValueBytesBE As Byte() = ValueBytes.Reverse().ToArray()
        Array.ConstrainedCopy(ValueBytesBE, 0, Str, Index, ValueBytesBE.Length)
    End Sub

    Public Shared Function ReadUInt32(Str As Stream) As UInteger
        Dim IntBytes As Byte() = New Byte(3) {}
        Str.Read(IntBytes, &H0, IntBytes.Length)
        Return BitConverter.ToUInt32(IntBytes, &H0)
    End Function

    Public Shared Function ReadInt32(Str As Stream) As UInteger
        Dim IntBytes As Byte() = New Byte(3) {}
        Str.Read(IntBytes, &H0, IntBytes.Length)
        Return BitConverter.ToUInt32(IntBytes, &H0)
    End Function

    Public Shared Function ReadUInt64(Str As Stream) As ULong
        Dim IntBytes As Byte() = New Byte(7) {}
        Str.Read(IntBytes, &H0, IntBytes.Length)
        Return BitConverter.ToUInt64(IntBytes, &H0)
    End Function

    Public Shared Function ReadInt64(Str As Stream) As Long
        Dim IntBytes As Byte() = New Byte(7) {}
        Str.Read(IntBytes, &H0, IntBytes.Length)
        Return BitConverter.ToInt64(IntBytes, &H0)
    End Function

    Public Shared Function ReadUInt16(Str As Stream) As UShort
        Dim IntBytes As Byte() = New Byte(1) {}
        Str.Read(IntBytes, &H0, IntBytes.Length)
        Return BitConverter.ToUInt16(IntBytes, &H0)
    End Function

    Public Shared Function ReadInt16(Str As Stream) As Short
        Dim IntBytes As Byte() = New Byte(1) {}
        Str.Read(IntBytes, &H0, IntBytes.Length)
        Return BitConverter.ToInt16(IntBytes, &H0)
    End Function

    Public Shared Function ReadUint32At(Str As Stream, Location As Integer) As UInteger
        Dim OldPos As Long = Str.Position
        Str.Seek(Location, SeekOrigin.Begin)
        Dim Out As UInteger = ReadUInt32(Str)
        Str.Seek(OldPos, SeekOrigin.Begin)
        Return Out
    End Function

    Public Shared Function ReadBytesAt(Str As Stream, Location As Integer, Len As Integer) As Byte()
        Dim OldPos As Long = Str.Position
        Str.Seek(Location, SeekOrigin.Begin)
        Dim WorkBuffer As Byte() = New Byte(Len - 1) {}
        Str.Read(WorkBuffer, &H0, WorkBuffer.Length)
        Str.Seek(OldPos, SeekOrigin.Begin)
        Return WorkBuffer
    End Function

    Public Shared Function ReadStringAt(Str As Stream, Location As Integer) As String
        Dim OldPos As Long = Str.Position
        Str.Seek(Location, SeekOrigin.Begin)
        Dim Out As String = ReadString(Str)
        Str.Seek(OldPos, SeekOrigin.Begin)
        Return Out
    End Function

    Public Shared Function ReadString(Str As Stream) As String
        Dim NewMemoryStream As New MemoryStream()

        While True
            Dim c As Byte = CByte(Str.ReadByte())
            If c = 0 Then Exit While
            NewMemoryStream.WriteByte(c)
        End While

        NewMemoryStream.Seek(&H0, SeekOrigin.Begin)
        Dim Out As String = Encoding.UTF8.GetString(NewMemoryStream.ToArray())
        NewMemoryStream.Dispose()
        Return Out
    End Function

    Public Shared Sub WriteUInt32(Str As Stream, Numb As UInteger)
        Dim IntBytes As Byte() = BitConverter.GetBytes(Numb)
        Str.Write(IntBytes, &H0, IntBytes.Length)
    End Sub

    Public Shared Sub WriteInt32(Str As Stream, Numb As Integer)
        Dim IntBytes As Byte() = BitConverter.GetBytes(Numb)
        Str.Write(IntBytes, &H0, IntBytes.Length)
    End Sub

    Public Shared Sub WriteUInt64(DestStream As Stream, value As ULong)
        Dim ValueBytes As Byte() = BitConverter.GetBytes(value)
        DestStream.Write(ValueBytes, &H0, &H8)
    End Sub

    Public Shared Sub WriteInt64(DestStream As Stream, value As Long)
        Dim ValueBytes As Byte() = BitConverter.GetBytes(value)
        DestStream.Write(ValueBytes, &H0, &H8)
    End Sub

    Public Shared Sub WriteUInt16(DestStream As Stream, value As UShort)
        Dim ValueBytes As Byte() = BitConverter.GetBytes(value)
        DestStream.Write(ValueBytes, &H0, &H2)
    End Sub

    Public Shared Sub WriteInt16(DestStream As Stream, value As Short)
        Dim ValueBytes As Byte() = BitConverter.GetBytes(value)
        DestStream.Write(ValueBytes, &H0, &H2)
    End Sub

    Public Shared Sub WriteInt32BE(DestStream As Stream, Numb As Integer)
        Dim IntBytes As Byte() = BitConverter.GetBytes(Numb)
        Dim IntBytesBE As Byte() = IntBytes.Reverse().ToArray()
        DestStream.Write(IntBytesBE, &H0, IntBytesBE.Length)
    End Sub

    Public Shared Sub WriteString(DestStream As Stream, Text As String, Optional Len As Integer = -1)
        If Len < 0 Then Len = Text.Length
        Dim TextBytes As Byte() = Encoding.UTF8.GetBytes(Text)
        DestStream.Write(TextBytes, &H0, TextBytes.Length)
    End Sub

End Class