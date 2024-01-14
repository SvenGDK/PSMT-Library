# PS Multi Tools .NET Library
The PS Multi Tools .NET Library contains all tools, downloads and other stuff for almost any Playstation console or handheld. </br>
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
- PKG Merger

## PS5
- General ELF & BIN Payload Sender
- Mast1c0re ELF Payload & PS2 ISO Sender
- APP.DB Modifier (Add the Internet Browser to the home menu for every profile with a simple click (requires FTPS5 loaded first))
- Burn Blu Ray ISO images to Blu Ray discs
- NOTIFICATION2.DB Manager (requires FTPS5 loaded first)
- Clear the console's error history (requires FTPS5 loaded first)
- PARAM.JSON Creator & Editor
- MANIFEST.JSON Creator & Editor
- GP5 Project Creator (requires prospero-pub-cmd at \Tools\PS5\ )
- PKG Builder (requires prospero-pub-cmd at \Tools\PS5\ )
- RCO Dumper (requires FTPS5 loaded first)
- RCO Extractor
- AT9 <-> WAV Audio Converter (requires at9tool at \Tools\PS5\ )
- FTP Grabber
  - Allows dumping of games (/mnt/sandbox/pfsmnt) (detects the remote game folder automatically)
  - Allows dumping of game metadata (/user/appmeta/ + /system_data/priv/appmeta/ & /user/np_uds/nobackup/conf/ + /user/trophy2/nobackup/conf/)
  - Allows dumping SELF files using sleirsgoevy's ps5-self-dumper payload (https://github.com/sleirsgoevy/ps4jb-payloads/tree/bd-jb/ps5-self-dumper)
- PS5 Game Patches Downloader
- Unofficial patches loader (libhijack fork of illusion)
- PKG Merger
- Make fSELF tool to fake sign SELF files of created dumps
  - Based on EchoStretch's Make_FSELF_PY3.bat & LightningMods updated make_fself by Flatz
- etaHEN Configurator
- klog Viewer

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

## The PSMT Library currently uses the following tools & libraries from other developers
| Tool / Library | Created by | Repository | Info |
| --- | --- | --- | --- |
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
| `kill_daemon` | illusion0001 | https://github.com/illusion0001/libhijacker | Python script -> single .exe
| `klicencebruteforce` | MAGiC333X | 
| `make_fself` | (PS3) ? | 
| `Make_FSELF_PY3` | EchoStretch |  | Batch script translated to VB
| `make_fself_python3-1` | Flatz (updated by LightningMods) |  | Python script -> single .exe
| `maxcso` | unknownbrackets | https://github.com/unknownbrackets/maxcso
| `mCiso` | sindastra | https://github.com/sindastra/psp-mciso
| `Newtonsoft.Json` | Newtonsoft | https://www.newtonsoft.com/json
| `nQuant` | matt wrock | https://www.nuget.org/packages/nQuant
| `PARAM.SFO Library` | xXxTheDarkprogramerxXx | https://github.com/xXxTheDarkprogramerxXx/PS4_Tools
| `pbppacker` |  | 
| `pfsshell & pfsfuse` |  | https://github.com/ps2homebrew/pfsshell
| `pkg2zip` | lusid1 | https://github.com/lusid1/pkg2zip
| `pkg_merge` | aldo-o & Tustin | https://github.com/aldo-o/pkg-merge
| `ps3iso-utils` | bucanero | https://github.com/bucanero/ps3iso-utils
| `PS4_Tools Library` | xXxTheDarkprogramerxXx | https://github.com/xXxTheDarkprogramerxXx/PS4_Tools
| `PSN_get_pkg_info` | windsurfer1122 | https://github.com/windsurfer1122/PSN_get_pkg_info | Python script -> Single .exe
| `psvpfstools` | motoharu-gosuto | https://github.com/motoharu-gosuto/psvpfstools
| `psxtract` | Hykem | https://github.com/mrlucas84/psxtract
| `pup_unpacker` | Zer0xFF | https://github.com/Zer0xFF/ps4-pup-unpacker
| `pupunpack` |  | 
| `rcomage` | ZiNgA BuRgA | 
| `readself` | Team fail0verflow | https://github.com/daryl317/fail0verflow-PS3-tools/tree/master
| `SCEDoormat_NoME` | krHACKen | 
| `scetool` | naehrwert | https://github.com/naehrwert/scetool | 
| `send_elf` | illusion0001 | https://github.com/illusion0001/libhijacker | Python script -> Single .exe
| `sfo` | hippie68 | https://github.com/hippie68/sfo
| `sngre` | cfwprophet | https://github.com/cfwprpht/Simply_Vita_RCO_Extractor
| `strings` | Mark Russinovich | https://learn.microsoft.com/en-us/sysinternals/downloads/strings
