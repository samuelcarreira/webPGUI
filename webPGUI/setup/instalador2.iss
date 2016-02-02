; =======================================================================
; ============ WebP encoding tool GUI v.1.0 ALPHA =======================
; =======================================================================

#define use_dotnetfx40

#define MyAppName "WebP encoding tool GUI"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Samuel Carreira"
#define MyAppURL "https://github.com/samcpt/WebPGUI"
#define MyAppExeName "webpgui.exe"
#define MyAppPath "D:\Users\SamC\Documentos2\C#\webPGUI\webPGUI\"
#define MyYear "2016"

[Setup]
;AppId={{93A97849-386C-4C78-BD62-448F11EF8E3A}
AppId={{08416672-68f6-4276-b8e7-157d63b4ff2e}
AppMutex=08416672-68f6-4276-b8e7-157d63b4ff2e

AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
VersionInfoVersion={#MyAppVersion}
VersionInfoProductVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL=mailto:samuelcarreira@outlook.com?subject=[webPGUI]
AppContact=mailto:samuelcarreira@outlook.com
AppUpdatesURL={#MyAppURL}
VersionInfoCompany={#MyAppPublisher}
VersionInfoCopyright=Copyright © {#MyYear} {#MyAppPublisher}
VersionInfoProductName={#MyAppName}
AppCopyright=Copyright © {#MyYear} {#MyAppPublisher}

DefaultDirName={userpf}\WebP GUI
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes

OutputBaseFilename=setup-{#MyAppVersion}
OutputDir=bin
SetupIconFile={#MyAppPath}\setup\icon.ico
Compression=lzma/ultra
SolidCompression=yes

AllowUNCPath=False
PrivilegesRequired=admin
MinVersion=0,5.01sp3
ShowLanguageDialog=auto
WizardSmallImageFile={#MyAppPath}\setup\small.bmp
;WizardImageFile={#MyAppPath}\setup\banner_lateral.bmp

UninstallFilesDir={app}
UninstallDisplayIcon={#MyAppPath}\main_icon.ico

[Languages]
;Name: "portuguese"; MessagesFile: "compiler:Languages\Portuguese.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"


[Files]
Source: "{#MyAppPath}\bin\Release\webPGUI4.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppPath}\bin\Release\cwebp.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppPath}\bin\Release\ConsoleAppLauncher.dll"; DestDir: "{app}"; Flags: ignoreversion

;Source: "{#MyAppPath}\setup\dotNetFx40_Full_setup"; DestDir: "{tmp}"; Flags: ignoreversion {#IsExternal}; Check: NeedsFramework


[Icons]
Name: "{userstartmenu}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"; Comment: {#MyAppName} {#MyAppVersion}
Name: "{userdesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"; Comment: {#MyAppName} {#MyAppVersion}; Tasks: desktopicon


[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

#include "scripts\products.iss"

#include "scripts\products\stringversion.iss"
#include "scripts\products\winversion.iss"
#include "scripts\products\fileversion.iss"
#include "scripts\products\dotnetfxversion.iss"

#ifdef use_dotnetfx40
#include "scripts\products\dotnetfx40client.iss"
#include "scripts\products\dotnetfx40full.iss"
#endif

[CustomMessages]
win_sp_title=Windows %1 Service Pack %2

[Code]
function InitializeSetup(): boolean;
begin
	//init windows version
	initwinversion();

	// if no .netfx 4.0 is found, install the client (smallest)
  #ifdef use_dotnetfx40
    if (not netfxinstalled(NetFx40Client, '') and not netfxinstalled(NetFx40Full, '')) then
      dotnetfx40client();
  #endif

	Result := true;
end;
