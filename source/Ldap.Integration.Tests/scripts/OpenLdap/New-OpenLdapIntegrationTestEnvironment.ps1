$composeFile = Join-Path $PSScriptRoot "docker-compose.yml"

Write-Output "Building local ldap container image with test data"
& docker-compose -f $composeFile build

Write-Output "Firing up openldap stack"
& docker-compose -f $composeFile up -d

$dockerExitCode = $LastExitCode

if ($dockerExitCode -ne 0) {
    throw "Failed! docker-compose returned $dockerExitCode"
}
    
Write-Output "DONE!"
exit 0