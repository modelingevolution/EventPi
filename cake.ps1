New-Cake -Name "event-pi" -Root "./"

Add-CakeStep -Name "Build All" -Action {  Build-Dotnet -All  }
Add-CakeStep -Name "Publish to nuget.org" -Action { Publish-Nuget -SourceUrl "https://nuget.org" }
