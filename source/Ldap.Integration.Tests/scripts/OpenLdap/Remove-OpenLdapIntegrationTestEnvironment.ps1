$composeFile = Join-Path $PSScriptRoot "docker-compose.yml"

Write-Output "Tearing down openldap stack"
& docker-compose -f $composeFile down

$dockerExitCode = $LastExitCode

if ($dockerExitCode -ne 0) {
    throw "Failed! docker-compose returned $dockerExitCode"
}
    
Write-Output "DONE!"
exit 0