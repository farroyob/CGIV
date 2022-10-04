@ECHO OFF
REM
REM unconfigure.bat
REM
REM This .bat file unconfigures ODP.NET, Managed Driver
REM

if /i {%1} == {-h} goto :Usage
if /i {%1} == {-help} goto :Usage

REM  take current dir
set "crt_dir=%~dp0"
REM go 3 levels up 
for %%I in ("%crt_dir%\..\..\..") do set "PATH_THREE_LEVEL_UP=%%~fI"

REM determine if the configuration is on a 32-bit or 64-bit OS
set ODAC_CFG_PREFIX=Wow6432Node\
if (%PROCESSOR_ARCHITECTURE%) == (x86) if (%PROCESSOR_ARCHITEW6432%) == () set ODAC_CFG_PREFIX=

REM unconfigure machine wide or not - default is false
set MACHINE_WIDE_UNCONFIGURATION=false
if /i {%1} == {true} set MACHINE_WIDE_UNCONFIGURATION=true

if {%MACHINE_WIDE_UNCONFIGURATION%} == {true} (

REM Unconfigure machine.config for ODP.NET, Managed Driver's configuration file section handler and client factory
echo.
echo OraProvCfg /action:unconfig /product:odpm /frameworkversion:v4.0.30319 /providerpath:"%PATH_THREE_LEVEL_UP%\odp.net\managed\common\Oracle.ManagedDataAccess.dll"
OraProvCfg /action:unconfig /product:odpm /frameworkversion:v4.0.30319 /providerpath:"%PATH_THREE_LEVEL_UP%\odp.net\managed\common\Oracle.ManagedDataAccess.dll"

REM Remove the ODP.NET, Managed Driver assemblies from the GAC
echo.
echo OraProvCfg /action:ungac /providerpath:"Oracle.ManagedDataAccess, Version=4.122.21.1"   
OraProvCfg /action:ungac /providerpath:"Oracle.ManagedDataAccess, Version=4.122.21.1"  

)

REM did the install create TNS_ADMIN reg value? If so, delete the regitry key
reg query HKEY_LOCAL_MACHINE\SOFTWARE\%ODAC_CFG_PREFIX%Oracle\ODP.NET.Managed\4.122.21.1\ /v TNS_ADMIN 1>NUL 2>NUL
if %ERRORLEVEL% EQU 0 (

REM remove registry entries for ODPM
echo Windows Registry Editor Version 5.00                     >  "%crt_dir%\odpm.reg"
echo [-HKEY_LOCAL_MACHINE\SOFTWARE\%ODAC_CFG_PREFIX%Oracle\ODP.NET.Managed\4.122.21.1]  >> "%crt_dir%\odpm.reg"
	
regedit /s "%crt_dir%\odpm.reg"
del /q "%crt_dir%\odpm.reg"

)

REM Remove the registry entry for enabling event logs
echo.
echo reg delete "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\eventlog\Application\Oracle Data Provider for .NET, Managed Driver" /f
reg delete "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\eventlog\Application\Oracle Data Provider for .NET, Managed Driver" /f


REM Delete the registry entry to remove managed assembly in the Add Reference Dialog box in VS.NET
echo.
echo reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\%ODAC_CFG_PREFIX%Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\Oracle.ManagedDataAccess" /f
reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\%ODAC_CFG_PREFIX%Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\Oracle.ManagedDataAccess" /f
echo.

goto :EOF

:Usage 
echo. 
echo Usage: 
echo   unconfigure.bat [machine_wide_unconfiguration]
echo. 
echo Example: 
echo   unconfigure.bat      (unconfigure ODP.NET, Managed Driver which was not configured at a machine wide level) 
echo   unconfigure.bat true (unconfigure ODP.NET, Managed Driver which was configured at a machine wide level)  
echo.
echo NOTE: By default, machine_wide_unconfiguration=false.
goto :EOF
