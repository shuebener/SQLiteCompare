;NSIS Modern User Interface
;Basic Example Script
;Written by Joost Verburg

;--------------------------------
;Include Modern UI and extra stuff

  !include "MUI2.nsh"

  !include WordFunc.nsh
  !insertmacro VersionCompare
  !include LogicLib.nsh
  !include nsDialogs.nsh


;--------------------------------
;Constants
  !define PRODUCT_NAME "SQLite Compare"
  !define PRODUCT_DISPLAY_NAME "SQLite Compare - Diff/Merge Utility"

;--------------------------------
;General

  ;Name and file
  Name "${PRODUCT_NAME}"
  OutFile "SQLiteCompareSetup.exe"

  ;Default installation folder
  InstallDir "$PROGRAMFILES\SQLiteCompare"

  ;Request application privileges for Windows Vista
  RequestExecutionLevel admin

  Var InstallDotNET
  Var InstallWindowsInstaller
  Var Result

;--------------------------------
;Interface Settings

  !define MUI_ABORTWARNING
  !define MUI_HEADERIMAGE
  !define MUI_HEADERIMAGE_BITMAP "installer_banner.bmp"
  !define MUI_WELCOMEFINISHPAGE_BITMAP "finish_banner.bmp"
  !define MUI_ICON "appicon.ico"
  !define MUI_UNICON "uninstallicon.ico"

;--------------------------------
;Pages

  !insertmacro MUI_PAGE_LICENSE "..\eula.txt"
  !insertmacro MUI_PAGE_DIRECTORY
  !insertmacro MUI_PAGE_INSTFILES

    # These indented statements modify settings for MUI_PAGE_FINISH
    !define MUI_FINISHPAGE_NOAUTOCLOSE
    !define MUI_FINISHPAGE_RUN
    !define MUI_FINISHPAGE_RUN_TEXT "Launch SQLite Compare now!"
    !define MUI_FINISHPAGE_RUN_FUNCTION "LaunchLink"
    !define MUI_FINISHPAGE_SHOWREADME_NOTCHECKED
    !define MUI_FINISHPAGE_SHOWREADME "$INSTDIR\readme.txt"
    !define MUI_FINISHPAGE_LINK "https://github.com/shuebener/SQLiteCompare"
    !define MUI_FINISHPAGE_LINK_LOCATION "https://github.com/shuebener/SQLiteCompare"
  !insertmacro MUI_PAGE_FINISH

  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES

;--------------------------------
;Languages

  !insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Installer Sections

Section "Install" Install

  SetOutPath "$INSTDIR"

  SetOverwrite ifnewer

  ; Get Windows Installer if required
;  ${If} $InstallWindowsInstaller == "Yes"
;    SetDetailsView hide
;    inetc::get /caption "Downloading Windows Installer 3.1" /canceltext "Cancel" "http://download.microsoft.com/download/1/4/7/147ded26-931c-4daf-9095-ec7baf996f46/WindowsInstaller-KB893803-v2-x86.exe" "$INSTDIR\WindowsInstaller-KB893803-v2-x86.exe" /end
;    Pop $1
;    ${If} $1 != "OK"
;      Delete "$INSTDIR\WindowsInstaller-KB893803-v2-x86.exe"
;      Abort "Installation cancelled."
;    ${EndIf}
;
;    Marquee::start /NOUNLOAD /step=5 /interval=60 /top=70 /height=18 /width=12 "Installing Windows Installer 3.1 (may take a few minutes)"
;    ExecWait '$INSTDIR\WindowsInstaller-KB893803-v2-x86.exe /quiet /norestart' $1
;    Marquee::stop
;    ${If} $1 != 0
;      ${If} $1 == 3010
;        SetRebootFlag true
;        DetailPrint "Windows Installer 3.1 requires a system reboot.."
;        Return
;      ${Else}
;        DetailPrint "Windows Installer 3.1 installation failed (error=$1). Please install Windows Installer 3.1 manually and rerun this setup."
;        Abort "Installation failed. Please install Windows Installer 3.1 manually and rerun this setup."
;      ${EndIf}
;    ${EndIf}
;    Delete "$INSTDIR\WindowsInstaller-KB893803-v2-x86.exe"
;    SetDetailsView show
;  ${EndIf}

  ; Get .NET if required
  ${If} $InstallDotNET == "Yes"
    SetDetailsView hide
    ;;inetc::get /caption "Downloading .NET Framework 2.0" /canceltext "Cancel" "http://download.microsoft.com/download/5/6/7/567758a3-759e-473e-bf8f-52154438565a/dotnetfx.exe" "$INSTDIR\dotnetfx.exe" /end
	inetc::get /caption "Downloading .NET Framework 4.6.2" /canceltext "Cancel" "https://download.microsoft.com/download/F/9/4/F942F07D-F26F-4F30-B4E3-EBD54FABA377/NDP462-KB3151800-x86-x64-AllOS-ENU.exe" "$INSTDIR\dotnetfx.exe" /end	
    Pop $1
    ${If} $1 != "OK"
      Delete "$INSTDIR\dotnetfx.exe"
      Abort "Installation cancelled."
    ${EndIf}

    Marquee::start /NOUNLOAD /step=5 /interval=60 /top=70 /height=18 /width=12 "Installing .NET framework (may take a few minutes)"
    ExecWait '$INSTDIR\dotnetfx.exe /q:a /c:$\"install /l /q$\"' $1
    Marquee::stop
    ${If} $1 != 0
      ${If} $1 == 8192
        SetRebootFlag true
        DetailPrint ".NET framework installation requires a system reboot.."
        Return
      ${Else}
        DetailPrint ".NET installation failed (error=$1). Please install .NET manually and rerun this setup."
        Abort ".NET installation failed. Please install .NET manually and rerun this setup."
      ${EndIf}
    ${EndIf}
    Delete "$INSTDIR\dotnetfx.exe"
    SetDetailsView show
  ${EndIf}

  ; Check one more time before continuing
  Call CheckDotNet
  ${If} $InstallDotNET == "Yes"
    Abort ".NET framework was not installed correctly. Setup cannot continue."
  ${EndIf}

  ; Install for all users
  SetShellVarContext all

  ; Add the application files
  File "..\SQLiteTurbo\bin\Release\Alsing.SyntaxBox.dll"
  File "..\SQLiteTurbo\bin\Release\System.Data.SQLite.dll"
  File "..\SQLiteTurbo\bin\Release\System.Data.SQLite.EF6.dll"
  File "..\SQLiteTurbo\bin\Release\System.Data.SQLite.Linq.dll"
  File "..\SQLiteTurbo\bin\Release\SQLiteCompare.exe.config"
  File "..\SQLiteTurbo\bin\Release\SQLiteCompare.exe"
  File "..\SQLiteTurbo\bin\Release\AutomaticUpdates.dll"
  File "..\SQLiteTurbo\bin\Release\Be.Windows.Forms.HexBox.dll"
  File "..\SQLiteTurbo\bin\Release\Common.dll"
  File "..\SQLiteTurbo\bin\Release\DiffControl.dll"
  File "..\SQLiteTurbo\bin\Release\FastGrid.dll"
  File "..\SQLiteTurbo\bin\Release\MultiPanelControl.dll"
  File "..\SQLiteTurbo\bin\Release\log4net.dll"
  File "..\SQLiteTurbo\bin\Release\ShiftReduceParser.dll"
  File "..\SQLiteTurbo\bin\Release\SQLiteParser.dll"
  File "..\SQLiteTurbo\bin\Release\UndoRedo.dll"
  File "..\SQLiteTurbo\bin\Release\EntityFramework.dll"
  File "..\SQLiteTurbo\bin\Release\EntityFramework.SqlServer.dll"
  File "..\readme.txt"
  File "..\LICENSE"
  
  SetOutPath "$INSTDIR\x64"
  File "..\SQLiteTurbo\bin\Release\x64\SQLite.Interop.dll"
  SetOutPath "$INSTDIR\x86"
  File "..\SQLiteTurbo\bin\Release\x86\SQLite.Interop.dll"  
  SetOutPath "$INSTDIR"

  ; Turn ON/OFF the check-updates flag
  WriteRegDWORD HKCU "SOFTWARE\SQLiteCompare" "CheckUpdatesOnStartup" 1

  ; ADD registry entries for ADD/REMOVE panel
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\SQLiteCompare" \
                 "DisplayName" "${PRODUCT_DISPLAY_NAME}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\SQLiteCompare" \
                 "DisplayIcon" "$\"$INSTDIR\uninstall.exe$\""
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\SQLiteCompare" \
                 "Publisher" "Sebastian Huebener"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\SQLiteCompare" \
                 "URLInfoAbout" "https://github.com/shuebener/SQLiteCompare"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\SQLiteCompare" \
                 "HelpLink" "https://github.com/shuebener/SQLiteCompare"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\SQLiteCompare" \
                 "UninstallString" "$\"$INSTDIR\uninstall.exe$\""

  ; Create uninstaller
  WriteUninstaller "$INSTDIR\Uninstall.exe"

  ; Create start menu shortcuts
  CreateDirectory "$SMPROGRAMS\${PRODUCT_NAME}"
  CreateShortCut "$SMPROGRAMS\${PRODUCT_NAME}\SQLite Compare.lnk" "$INSTDIR\SQLiteCompare.exe" \
  "" "$INSTDIR\SQLiteCompare.exe"
  CreateShortCut "$SMPROGRAMS\${PRODUCT_NAME}\Uninstall.lnk" "$INSTDIR\Uninstall.exe" \
  "" "$INSTDIR\Uninstall.exe"

  ; Create desktop shortcut
  ;CreateShortCut "$DESKTOP\SQLite Compare.lnk" "$INSTDIR\SQLiteCompare.exe" \
  ;"" "$INSTDIR\SQLiteCompare.exe"

SectionEnd

;--------------------------------
;Descriptions

  ;Language strings
  LangString DESC_SecDummy ${LANG_ENGLISH} "A test section."

  ;Assign language strings to sections
  !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${Install} $(DESC_SecDummy)
  !insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------
;Uninstaller Section

Section "Uninstall"

  ;ADD YOUR OWN FILES HERE...

  ; UnInstall for all users
  SetShellVarContext all

  Delete "$INSTDIR\Uninstall.exe"
  Delete "$INSTDIR\Alsing.SyntaxBox.dll"
  Delete "$INSTDIR\System.Data.SQLite.dll"
  Delete "$INSTDIR\System.Data.SQLite.EF6.dll"
  Delete "$INSTDIR\System.Data.SQLite.Linq.dll"
  Delete "$INSTDIR\x64\SQLite.Interop.dll"
  Delete "$INSTDIR\x86\SQLite.Interop.dll"
  Delete "$INSTDIR\SQLiteCompare.exe.config"
  Delete "$INSTDIR\SQLiteCompare.exe"
  Delete "$INSTDIR\AutomaticUpdates.dll"
  Delete "$INSTDIR\Be.Windows.Forms.HexBox.dll"
  Delete "$INSTDIR\Common.dll"
  Delete "$INSTDIR\DiffControl.dll"
  Delete "$INSTDIR\FastGrid.dll"
  Delete "$INSTDIR\MultiPanelControl.dll"
  Delete "$INSTDIR\log4net.dll"
  Delete "$INSTDIR\ShiftReduceParser.dll"
  Delete "$INSTDIR\SQLiteParser.dll"
  Delete "$INSTDIR\UndoRedo.dll"
  Delete "$INSTDIR\EntityFramework.dll"
  Delete "$INSTDIR\EntityFramework.SqlServer.dll"
  Delete "$INSTDIR\readme.txt"
  Delete "$INSTDIR\LICENSE"

  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\SQLiteCompare"

  ; Delete start menu shortcuts
  Delete "$SMPROGRAMS\${PRODUCT_NAME}\SQLite Compare.lnk"
  Delete "$SMPROGRAMS\${PRODUCT_NAME}\Uninstall.lnk"
  RMDir "$SMPROGRAMS\${PRODUCT_NAME}"

  ;Delete "$DESKTOP\SQLite Compare.lnk"

  RMDir "$INSTDIR"

SectionEnd

;---------------------------------
; Functions

Function un.onInit

    # call userInfo plugin to get user info.  The plugin puts the result in the stack
    userInfo::getAccountType

    # pop the result from the stack into $0
    pop $0

    # compare the result with the string "Admin" to see if the user is admin.
    # If match, jump 3 lines down.
    strCmp $0 "Admin" +3

    # if there is not a match, print message and return
    messageBox MB_OK "Please run the uninstaller with administrator permissions."
    Abort

   ; Make sure no other installer instance is running
   System::Call 'kernel32::CreateMutexA(i 0, i 0, t "SqliteCompareSetupMutex") i .r1 ?e'
   Pop $R0
   StrCmp $R0 0 +3
     MessageBox MB_OK|MB_ICONEXCLAMATION "The installer is already running."
     Abort

   FindProcDLL::FindProc "SQLiteCompare.exe"
   ${If} $R0 == "1"
     MessageBox MB_OK|MB_ICONEXCLAMATION "Please close the SQLiteCompare application before continuing to uninstall it."
     Abort
   ${EndIf}
FunctionEnd


Function .onInit

    # call userInfo plugin to get user info.  The plugin puts the result in the stack
    userInfo::getAccountType

    # pop the result from the stack into $0
    pop $0

    # compare the result with the string "Admin" to see if the user is admin.
    # If match, jump 3 lines down.
    strCmp $0 "Admin" +3

    # if there is not a match, print message and return
    messageBox MB_OK "Please run the installer with administrator permissions."
    Abort

   ; Make sure no other installer instance is running
   System::Call 'kernel32::CreateMutexA(i 0, i 0, t "SqliteCompareSetupMutex") i .r1 ?e'
   Pop $R0
   StrCmp $R0 0 +3
     MessageBox MB_OK|MB_ICONEXCLAMATION "The installer is already running."
     Abort

  !insertmacro MUI_LANGDLL_DISPLAY

  ; Make sure this is NT platform
  Version::IsWindowsPlatformNT
  Pop $Result
  StrCmp $Result "1" Label_ItIsWindowsNtPlatform Label_ItIsNotNtPlatform

Label_ItIsNotNtPlatform:
  MessageBox MB_OK|MB_ICONEXCLAMATION "SQLite Compare is not supported on this platform"
  Abort

Label_ItIsWindowsNtPlatform:

  ; Check Windows Installer version
  StrCpy $InstallWindowsInstaller "No"
  GetDllVersion "$SYSDIR\msi.dll" $R0 $R1
  ${If} $R0 == ''
    StrCpy $InstallWindowsInstaller "Yes"
  ${Else}
    IntOp $R2 $R0 >> 16
    IntOp $R2 $R2 & 0x0000FFFF ; $R2 now contains major version
    IntOp $R3 $R0 & 0x0000FFFF ; $R3 now contains minor version
    ${If} $R2 < 3
      StrCpy $InstallWindowsInstaller "Yes"
    ${EndIf}
  ${EndIf}

  ${If} $InstallWindowsInstaller == "Yes"
    MessageBox MB_OK|MB_ICONINFORMATION "${PRODUCT_NAME} requires that the Windows Installer 3.1 is installed. Please download the file from Microsoft and install it before restarting this installer."
    Abort
  ${EndIf}

  ; Check .NET version
  Call CheckDotNet
  ${If} $InstallDotNet == "Yes"
    MessageBox MB_OK|MB_ICONINFORMATION "${PRODUCT_NAME} requires that the .NET Framework 4.6 is installed. The .NET Framework will be downloaded and installed automatically during installation of ${PRODUCT_NAME}."
    Return
  ${EndIf}
FunctionEnd

Function CheckDotNet
  StrCpy $InstallDotNET "No"
  ;ReadRegDWORD $0 HKLM 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v2.0.50727' Install
  ReadRegDWORD $0 HKLM 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full' Install
  ${If} $0 == ''
    StrCpy $InstallDotNET "Yes"
  ${EndIf}

FunctionEnd

Function LaunchLink
  ExecShell "" "$SMPROGRAMS\${PRODUCT_NAME}\SQLite Compare.lnk"
FunctionEnd
