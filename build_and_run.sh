#!/bin/bash
dotnet build && cp bin/Debug/net46/NoelTools.dll ../BepInEx/plugins/ && wine ../AliceInCradle.exe
