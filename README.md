# PS Multi Tools .NET Library
The PS Multi Tools .NET Library contains various tools and downloads for almost any Playstation console or handheld. </br>
Some tools are written by other developers and will be executed from an executable binary with arguments - please take a look at the code.

## General:
- FTP Browser
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
- PKG Infos Reader
- PUP Unpacker
- RCO Dumper
- SELF Reader
- PARAM.SFO Editor

## PS4
- PKG Sender (currently not working)
- PKG Infos Reader
- Payload Sender
- Show PSN Store Infos
- Decrypted PUP Unpacker
- USB Writer
- PARAM.SFO Editor

## PS5
- General ELF Payload Sender
- Mast1c0re Payload & PS2 ISO Sender
- Add the Internet Browser to the home menu for every profile with a simple click (requires FTPS5 loaded first)
- Rip the content of discs over FTP (requires FTPS5 loaded first)
- Burn BR .iso images to Blu Ray discs
- Notification Manager (requires FTPS5 loaded first)
- Clear the console's error history (requires FTPS5 loaded first)
- PARAM.JSON Creator & Editor
- GP5 Project Creator (requires pub tools at \Tools\PS5\...)
- PKG Builder (requires pub tools at \Tools\PS5\...)
- RCO Dumper (requires FTPS5 loaded first)
- RCO Extractor

## PSP
- ISO to CSO Converter
- CSO Decompressor
- PBP to ISO / ISO to PBP Converter
- PBP Packer/Unpacker

## PSVita
- PKG Extractor
- PKG Infos Reader
- PSVIMAGE Tools (currently not working)
- RCO Data Table Extractor

## Developer Notes
- Can be used on a .NET Framework 4.8.1 project
- Add a reference to "psmt-lib.dll", "HtmlAgilityPack.dll", NuGet Package "Newtonsoft.Json", "PARAM.SFO.dll and "PS4_Tools.dll"
- For WPF windows: Add the namespace ```xmlns:psmt_lib="clr-namespace:psmt_lib;assembly=psmt-lib>"``` to your window xaml
- Adding a menu: ```<psmt_lib:PSMENUNAME Height="20" VerticalAlignment="Top"></psmt_lib:PSMENUNAME>```
- Open a tool (SFO Editor for ex.):</br>```Imports psmt_lib```<br/>```Dim NewSFOEditor As New SFOEditor()```<br/>```NewSFOEditor.Show()```

## PSMT Library currently uses the following utilities/libraries from other developers
| Tool / Library | Created by | Repository |
| --- | --- | --- |
| `bchunk` | extramaster | https://github.com/extramaster/bchunk
| `binmerge` | putnam | https://github.com/putnam/binmerge
| `CEX2DEX` |  | 
| `costool` | naehrwert | 
| `dd` | John Newbigin | http://www.chrysocome.net/dd
| `dev_flash` | HSReina | 
| `DiscUtils` | DiscUtils | https://github.com/DiscUtils/DiscUtils
| `discore` |  | 
| `DotNetZip` |  | https://www.nuget.org/packages/DotNetZip/
| `elf2pbp` | loser | https://github.com/PSP-Archive/elf2pbp
| `esrpatch` |  | 
| `esrunpatch` |  | 
| `ffplay` | FFmpeg | https://github.com/FFmpeg/FFmpeg
| `fix_tar` | KaKaRoTo & cfwprpht | 
| `fwpkg` | evilsperm | https://github.com/evilsperm/fwtool
| `hdl_dump` |  | https://github.com/ps2homebrew/hdl-dump
| `hexdump` | di-mgt | https://www.di-mgt.com.au/hexdump-for-windows.html
| `HtmlAgilityPack` | ZZZ Projects | https://html-agility-pack.net/
| `IsoPbpConverter` | LMAN | 
| `klicencebruteforce` | MAGiC333X | 
| `make_fself` |  | 
| `maxcso` | unknownbrackets | https://github.com/unknownbrackets/maxcso
| `mCiso` | sindastra | https://github.com/sindastra/psp-mciso
| `Newtonsoft.Json` | Newtonsoft | https://www.newtonsoft.com/json
| `nQuant` | matt wrock | https://www.nuget.org/packages/nQuant
| `PARAM.SFO Library` | xXxTheDarkprogramerxXx | https://github.com/xXxTheDarkprogramerxXx/PS4_Tools
| `pbppacker` |  | 
| `pfsshell & pfsfuse` |  | https://github.com/ps2homebrew/pfsshell
| `pkg2zip` | lusid1 | https://github.com/lusid1/pkg2zip
| `ps3iso-utils` | bucanero | https://github.com/bucanero/ps3iso-utils
| `PS4_Tools Library` | xXxTheDarkprogramerxXx | https://github.com/xXxTheDarkprogramerxXx/PS4_Tools
| `PSN_get_pkg_info` | windsurfer1122 | https://github.com/windsurfer1122/PSN_get_pkg_info
| `psvpfstools` | motoharu-gosuto | https://github.com/motoharu-gosuto/psvpfstools
| `psxtract` | Hykem | https://github.com/mrlucas84/psxtract
| `pup_unpacker` | Zer0xFF | https://github.com/Zer0xFF/ps4-pup-unpacker
| `pupunpack` |  | 
| `rcomage` | ZiNgA BuRgA | 
| `readself` | Team fail0verflow | https://github.com/daryl317/fail0verflow-PS3-tools/tree/master
| `SCEDoormat_NoME` | krHACKen | 
| `scetool` | naehrwert | https://github.com/naehrwert/scetool
| `sfo` | hippie68 | https://github.com/hippie68/sfo
| `sngre` | cfwprophet | https://github.com/cfwprpht/Simply_Vita_RCO_Extractor
| `strings` | Mark Russinovich | https://learn.microsoft.com/en-us/sysinternals/downloads/strings
