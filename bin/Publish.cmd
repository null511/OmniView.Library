if exist "OmniView.Library\bin\Package\" rmdir /Q /S "OmniView.Library\bin\Package"
nuget pack "OmniView.Library\OmniView.Library.csproj" -OutputDirectory "OmniView.Library\bin\Package" -Build -Prop "Configuration=Release;Platform=AnyCPU"
nuget push "OmniView.Library\bin\Package\*.nupkg" -Source "https://www.nuget.org/api/v2/package" -NonInteractive
pause
