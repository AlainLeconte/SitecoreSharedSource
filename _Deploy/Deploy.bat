@Echo off
@Echo ---------- Deploy.bat for Sitecore.SharedSource ----------
@Echo solutionDir is %1
@Echo projectDir is %2
@Echo TargetDir is %~3
set solutionDir=%1
set projectDir=%2
set targetDir=%~3
if "%~4"=="" (
	set configName=Debug
) else (
	set configName=%~4
)
@Echo configName is %configName%

set "excludeFile=%solutionDir:~2%_Deploy\exclude.txt"
@Echo excludeFile is %excludeFile%
REM if not x%projectDir:\Sitecore.SharedSource\=%==x%projectDir% del /F "%targetDir%bin\Sitecore.SharedSource.*.dll"
REM if not x%projectDir:\Sitecore.SharedSource\=%==x%projectDir% del /F "%targetDir%bin\Sitecore.SharedSource.*.pdb"
echo Target: %targetDir%bin\
xcopy /Y /EXCLUDE:%excludeFile% %projectDir%bin\*.dll "%targetDir%bin\"
xcopy /Y /EXCLUDE:%excludeFile% %projectDir%bin\*.pdb "%targetDir%bin\"
if exist "%projectDir%App_Config" xcopy /S /R /Y /I /EXCLUDE:%excludeFile% %projectDir%App_Config "%targetDir%App_Config\"
if exist "%projectDir%sitecore" xcopy /S /R /Y /I /EXCLUDE:%excludeFile% %projectDir%sitecore "%targetDir%sitecore\"
if exist "%projectDir%sitecore modules" xcopy /S /R /Y /I /EXCLUDE:%excludeFile% "%projectDir%sitecore modules" "%targetDir%sitecore modules\"
if exist "%projectDir%*.js" xcopy /S /R /Y /I /EXCLUDE:%excludeFile% %projectDir%*.js "%targetDir%"