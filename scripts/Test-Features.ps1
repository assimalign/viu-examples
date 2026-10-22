[CmdletBinding()]
param([switch]$SkipBuild,[switch]$SkipBrowserInstall,[string]$Feature)
$ErrorActionPreference='Stop'
$repositoryDirectory=Split-Path $PSScriptRoot -Parent
$features=Join-Path $repositoryDirectory 'examples/Features'
if(-not $SkipBuild){ & dotnet build (Join-Path $features 'Assimalign.Viu.Features.slnx'); if($LASTEXITCODE -ne 0){throw 'Feature build failed.'} }
$runner=Join-Path $PSScriptRoot 'FeatureSmoke/FeatureSmoke.csproj'
& dotnet build $runner --nologo -v:quiet
if($LASTEXITCODE -ne 0){throw 'Smoke runner build failed.'}
if(-not $SkipBrowserInstall){ & dotnet run --project $runner --no-build -- --install; if($LASTEXITCODE -ne 0){throw 'Chromium installation failed.'} }
$arguments=@($features); if($Feature){$arguments+=$Feature}
& dotnet run --project $runner --no-build -- @arguments
if($LASTEXITCODE -ne 0){throw 'Browser feature verification failed.'}
foreach($entry in (Get-Content (Join-Path $features 'examples.json') -Raw | ConvertFrom-Json | Where-Object Kind -eq 'Console')){
    $project=Get-ChildItem -LiteralPath (Join-Path $features $entry.Directory) -Filter '*.csproj' | Select-Object -First 1
    & dotnet run --project $project.FullName --no-build
    if($LASTEXITCODE -ne 0){throw "Console feature failed: $($entry.Feature)"}
}
