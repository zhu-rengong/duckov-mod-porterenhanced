using namespace System.Xml.Linq
using namespace System.Xml.XPath

Import-Module (Join-Path $PSScriptRoot "ConfigTool.psm1") -Scope Local -Force

$ModInfo = Get-ModInfo
$ModName = $ModInfo["Default"]["name"]

$ProjectElement = [XElement]::Load(".\Project\$ModName.csproj")
$SteamPath = [System.Xml.XPath.Extensions]::XPathSelectElement($ProjectElement, "/PropertyGroup/SteamPath").Value
$GamePath = "$SteamPath\steamapps\common\Escape from Duckov"
$RefsPath = Join-Path $GamePath "Duckov_Data" "Managed"
$PublicizedAssemblyRefsPath = "$PSScriptRoot\Project\Refs"

$References = @()
$References += Get-ChildItem -Path $RefsPath | Where-Object {
    $_.Name -like "TeamSoda.*.dll" -and
    $_.Name -notlike "*-publicized*"
}
$References += "$RefsPath\ItemStatsSystem.dll"
$References += "$RefsPath\SodaLocalization.dll"

assembly-publicizer.exe $References --output $PublicizedAssemblyRefsPath
