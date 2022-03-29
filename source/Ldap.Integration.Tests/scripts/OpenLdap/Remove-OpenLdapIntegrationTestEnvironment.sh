#$composeFile = Join-Path $PSScriptRoot "docker-compose.yml"

echo "Tearing down openldap stack"
docker-compose -f docker-compose.yml down

#$dockerExitCode = $LastExitCode

#if ($dockerExitCode -ne 0) {
#    throw "Failed! docker-compose returned $dockerExitCode"
#}
    
echo "DONE!"
exit 0