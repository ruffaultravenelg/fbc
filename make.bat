@echo off
SET PROJECT_PATH="fbc\fbc.vbproj"
SET OUTPUT_PATH="build"

SET TARGET_RUNTIME=win-x86

REM win-x86        : Windows 32 bits
REM win-x64        : Windows 64 bits
REM win-arm        : Windows ARM
REM linux-x64      : Linux 64 bits
REM linux-arm      : Linux ARM
REM osx-x64        : macOS 64 bits
REM osx-arm64      : macOS ARM

dotnet publish %PROJECT_PATH% -c Release -r %TARGET_RUNTIME% --framework net8.0 --self-contained true /p:PublishSingleFile=true -o %OUTPUT_PATH%
