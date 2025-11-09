using namespace System.Xml.Linq
using namespace System.Xml.XPath

Import-Module (Join-Path $PSScriptRoot "ConfigTool.psm1") -Scope Local -Force

$ModInfo = Get-ModInfo
$ModName = $ModInfo["Default"]["name"]

$ProjectElement = [XElement]::Load(".\Project\$ModName.csproj")
$SteamPath = [System.Xml.XPath.Extensions]::XPathSelectElement($ProjectElement, "/PropertyGroup/SteamPath").Value
$GamePath = "$SteamPath\steamapps\common\Escape from Duckov"
$ModFolder = Join-Path $GamePath "Duckov_Data" "Mods" $ModName

$Option = Read-Host (
    "Choices" `
        + " ($($PSStyle.Foreground.Yellow)$($PSStyle.Blink)D$($PSStyle.Reset)eploy/" `
        + "$($PSStyle.Foreground.Yellow)$($PSStyle.Blink)U$($PSStyle.Reset)ndeploy/" `
        + "$($PSStyle.Foreground.Yellow)$($PSStyle.Blink)E$($PSStyle.Reset)xit)"
)

switch ($Option.ToLower()) {
    { $_ -eq 'd' -or $_ -eq 'deploy' } { 
        Robocopy.exe (Join-Path $PSScriptRoot "Content") $ModFolder /MIR /XC /FP
    }
    { $_ -eq 'u' -or $_ -eq 'undeploy' } { 
        Remove-Item -Path $ModFolder -Recurse
    }
    { $_ -eq 'e' -or $_ -eq 'exit' } { 
        exit
    }
}
