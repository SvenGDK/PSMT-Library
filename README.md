# PS Multi Tools .NET Library
The PS Multi Tools .NET Library contains various tools and downloads for almost any Playstation console or handheld. </br>

## Developer Notes
- Can be used on a .NET Framework 4.8.1 project
- Add a reference to "psmt-lib.dll", "HtmlAgilityPack.dll", NuGet Package "Newtonsoft.Json", "PARAM.SFO.dll and "PS4_Tools.dll"
- For WPF windows: Add the namespace ```xmlns:psmt_lib="clr-namespace:psmt_lib;assembly=psmt-lib>"``` to your window xaml
- Adding a menu: ```<psmt_lib:PSMENUNAME Height="20" VerticalAlignment="Top"></psmt_lib:PSMENUNAME>```
- Open a tool (SFO Editor for ex.):</br>```Imports psmt_lib```<br/>```Dim NewSFOEditor As New SFOEditor()```<br/>```NewSFOEditor.Show()```
