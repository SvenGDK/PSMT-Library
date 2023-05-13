# PS Multi Tools .NET Library
The PS Multi Tools .NET Library contains various tools and downloads for almost any Playstation console or handheld. </br>

## General:
- FTP Browser
- PARAM.SFO Editor
- Downloader

## PS1
- Merge BIN files

## PS2
- Convert BIN/CUE to ISO
- PSX DVR XMB Games and Homebrew Installer

## PS3
- Core_OS Tools
- Fix Tar Tool
- PS3 ISO Tools
- PKG Extractor
- PUP Unpacker
- RCO Dumper
- SELF Reader

## PS4
- PKG Sender (currently not working)
- Payload Sender
- Show PSN Store Infos
- Decrypted PUP Unpacker
- USB Writer

## PS5
- Mast1c0re Payload & ISO Sender

## PSP
- ISO to CSO Converter
- CSO Decompressor
- PBP to ISO / ISO to PBP Converter
- PBP Packer/Unpacker

## PSVita
- PKG Extractor
- PSVIMAGE Tools (currently not working)
- RCO Data Table Extractor

## Developer Notes
- Can be used on a .NET Framework 4.8.1 project
- Add a reference to "psmt-lib.dll", "HtmlAgilityPack.dll", NuGet Package "Newtonsoft.Json", "PARAM.SFO.dll and "PS4_Tools.dll"
- For WPF windows: Add the namespace ```xmlns:psmt_lib="clr-namespace:psmt_lib;assembly=psmt-lib>"``` to your window xaml
- Adding a menu: ```<psmt_lib:PSMENUNAME Height="20" VerticalAlignment="Top"></psmt_lib:PSMENUNAME>```
- Open a tool (SFO Editor for ex.):</br>```Imports psmt_lib```<br/>```Dim NewSFOEditor As New SFOEditor()```<br/>```NewSFOEditor.Show()```
