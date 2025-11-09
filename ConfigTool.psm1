function Get-ModInfo {
    $ini = @{ "Default" = @{} }
    switch -regex -file (Join-Path $PSScriptRoot "Content" "info.ini") {
        "^\[(.+)\]$" {
            $section = $matches[1].Trim()
            $ini[$section] = @{}
        }
        "(.+?)=(.+)" {
            $key, $value = $matches[1..2]
            $ini[$section ?? "Default"][$key.Trim()] = $value.Trim()
        }
    }
    
    return $ini
}
