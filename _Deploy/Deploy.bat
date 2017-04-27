REM echo off
echo ---------- Deploy.bat for Sitecore.SharedSource ----------
echo solutionDir is %1
echo projectDir is %2
echo TargetDir is %~3
set solutionDir=%1
set projectDir=%2
set targetDir=%~3
if "%~4"=="" (
	set configName=Debug
) else (
	set configName=%~4
)
echo configName is %configName%

set "excludeFile=%solutionDir:~2%_Deploy\exclude.txt"
REM if not x%projectDir:\Sitecore.SharedSource\=%==x%projectDir% del /F "%targetDir%bin\Sitecore.SharedSource.*.dll"
REM if not x%projectDir:\Sitecore.SharedSource\=%==x%projectDir% del /F "%targetDir%bin\Sitecore.SharedSource.*.pdb"
echo Target: %targetDir%bin\
xcopy /Y %projectDir%bin\*.dll "%targetDir%bin\"
xcopy /Y %projectDir%bin\*.pdb "%targetDir%bin\"
if exist "%projectDir%Ajax" xcopy /S /R /Y /I /EXCLUDE:%excludeFile% %projectDir%Ajax "%targetDir%Ajax\"
if exist "%projectDir%App_Browsers" xcopy /S /R /Y /I /EXCLUDE:%excludeFile% %projectDir%App_Browsers "%targetDir%App_Browsers\"
if exist "%projectDir%App_Config" xcopy /S /R /Y /I /EXCLUDE:%excludeFile% %projectDir%App_Config "%targetDir%App_Config\"
if exist "%projectDir%Layouts" xcopy /S /R /Y /I /EXCLUDE:%excludeFile% %projectDir%Layouts "%targetDir%Layouts\"
if exist "%projectDir%Sublayouts" xcopy /S /R /Y /I /EXCLUDE:%excludeFile% %projectDir%Sublayouts "%targetDir%Sublayouts\"
if exist "%projectDir%xsl" xcopy /S /R /Y /I /EXCLUDE:%excludeFile% %projectDir%xsl "%targetDir%xsl\"
if exist "%projectDir%themes" xcopy /S /R /Y /I /EXCLUDE:%excludeFile% %projectDir%themes "%targetDir%themes\"
if exist "%projectDir%sitecore" xcopy /S /R /Y /I /EXCLUDE:%excludeFile% %projectDir%sitecore "%targetDir%sitecore\"
if exist "%projectDir%sitecore modules" xcopy /S /R /Y /I /EXCLUDE:%excludeFile% "%projectDir%sitecore modules" "%targetDir%sitecore modules\"
if exist "%projectDir%*.html" xcopy /S /R /Y /I /EXCLUDE:%excludeFile% %projectDir%*.html "%targetDir%"
if exist "%projectDir%*.sitedown" xcopy /S /R /Y /I /EXCLUDE:%excludeFile% %projectDir%*.sitedown "%targetDir%"
if exist "%projectDir%*.aspx" xcopy /S /R /Y /I /EXCLUDE:%excludeFile% %projectDir%*.aspx "%targetDir%"
if exist "%projectDir%*.css" xcopy /S /R /Y /I /EXCLUDE:%excludeFile% %projectDir%*.css "%targetDir%"
if exist "%projectDir%*.js" xcopy /S /R /Y /I /EXCLUDE:%excludeFile% %projectDir%*.js "%targetDir%"
if exist "%projectDir%*.ico" xcopy /S /R /Y /I /EXCLUDE:%excludeFile% %projectDir%*.ico "%targetDir%"
if exist "%projectDir%global.asax" xcopy /S /R /Y /I /EXCLUDE:%excludeFile% %projectDir%global.asax "%targetDir%"
if exist "%projectDir%web.config" xcopy /S /R /Y /I /EXCLUDE:%excludeFile% %projectDir%web.config "%targetDir%