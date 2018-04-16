@echo off
git pull
nuget restore
msbuild makai.sln /t:Build /p:Configuration=Release /p:TargetFramework=v4.6.1
