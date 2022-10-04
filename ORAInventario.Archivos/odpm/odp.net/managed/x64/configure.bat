@ECHO OFF
REM
REM configure.bat
REM
REM This .bat file configures ODP.NET, Managed Driver
REM

if /i {%1} == {-h} goto :Usage
if /i {%1} == {-help} goto :Usage

for %%a in ("%~dp0..") do set "PATH_ONE_LEVEL_UP="%%~fa""
REM echo %PATH_ONE_LEVEL_UP%

REM  take current dir
set "crt_dir=%~dp0"
REM go 3 levels up 
for %%I in ("%crt_dir%\..\..\..") do set "PATH_THREE_LEVEL_UP=%%~fI"

REM configure machine wide or not - default is false
set MACHINE_WIDE_CONFIGURATION=false
if /i {%1} == {true} set MACHINE_WIDE_CONFIGURATION=true

REM set TNS_ADMIN
if {%2} NEQ {} (
echo reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Oracle\ODP.NET.Managed\4.122.21.1" /v TNS_ADMIN /t REG_SZ /d %2 /f /reg:64
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Oracle\ODP.NET.Managed\4.122.21.1" /v TNS_ADMIN /t REG_SZ /d %2 /f /reg:64
)

if {%MACHINE_WIDE_CONFIGURATION%} == {true} (

REM Configure machine.config for ODP.NET, Managed Driver's configuration file section handler and client factory
echo.
echo OraProvCfg /action:config /product:odpm /frameworkversion:v4.0.30319 /providerpath:"%PATH_THREE_LEVEL_UP%\odp.net\managed\common\Oracle.ManagedDataAccess.dll"
OraProvCfg /action:config /product:odpm /frameworkversion:v4.0.30319 /providerpath:"%PATH_THREE_LEVEL_UP%\odp.net\managed\common\Oracle.ManagedDataAccess.dll" 

REM Place the ODP.NET, Managed Driver assemblies into the GAC
echo.
echo OraProvCfg /action:gac /providerpath:"%PATH_THREE_LEVEL_UP%\odp.net\managed\common\Oracle.ManagedDataAccess.dll"        
OraProvCfg /action:gac /providerpath:"%PATH_THREE_LEVEL_UP%\odp.net\managed\common\Oracle.ManagedDataAccess.dll"  

)      

REM Add a registry entry for enabling event logs
echo.
echo reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Application\Oracle Data Provider for .NET, Managed Driver" /v EventMessageFile /t REG_EXPAND_SZ /d %SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\EventLogMessages.dll /f
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Application\Oracle Data Provider for .NET, Managed Driver" /v EventMessageFile /t REG_EXPAND_SZ /d %SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\EventLogMessages.dll /f

REM Delete the old registry entry to add managed assembly in the Add Reference Dialog box in VS.NET
echo.
echo reg query HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\odp.net.managed\
reg query HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\odp.net.managed\ 1>NUL 2>NUL
if %ERRORLEVEL% EQU 0 (
echo reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\odp.net.managed" /f
reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\odp.net.managed" /f 1>NUL 2>NUL
)

REM Create a registry entry to add managed assembly in the Add Reference Dialog box in VS.NET
echo.
echo reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\Oracle.ManagedDataAccess" /ve /t REG_SZ /d %PATH_ONE_LEVEL_UP%\common /f
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\Oracle.ManagedDataAccess" /ve /t REG_SZ /d %PATH_ONE_LEVEL_UP%\common /f
echo.

goto :EOF

:Usage 
echo. 
echo Usage: 
echo   configure.bat [machine_wide_configuration] [tns_admin_location]
echo. 
echo Example: 
echo   configure.bat             (do not configure ODP.NET, Managed Driver at a machine wide level) 
echo   configure.bat true c:\tns (configure ODP.NET, Managed Driver at a machine wide level, set TNS_ADMIN to c:\tns) 
echo.
echo NOTE: By default, machine_wide_configuration=false.
echo.
echo NOTE: In order to specify tns_admin_location, machine_wide_configuration has to be specified.
goto :EOF
