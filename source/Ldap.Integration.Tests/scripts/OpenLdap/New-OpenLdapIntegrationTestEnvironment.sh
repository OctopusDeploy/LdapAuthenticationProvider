#$composeFile = Join-Path $PSScriptRoot "docker-compose.yml"

echo "Building local ldap container image with test data"
docker-compose -f docker-compose.yml build

echo "Firing up openldap stack"
docker-compose -f docker-compose.yml up -d

#$dockerExitCode = $LastExitCode

#if ($dockerExitCode -ne 0) {
#    throw "Failed! docker-compose returned $dockerExitCode"
#}
    
echo "DONE!"
exit 0