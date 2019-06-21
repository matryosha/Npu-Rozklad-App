function Write-Info {
    param (
        [string]$message
    )
    Write-Host "i" -f DarkBlue -nonewline; Write-Host "  $message" -f blue;
}

function Write-Error {
    param (
        [string]$message
    )
    Write-Host "e" -f DarkRed -NoNewline; Write-Host "  $message" -f DarkRed
}


$env=$args[0]
$webapi_path= (Resolve-Path "..\RozkladNpuBot.WebApi\").Path
$webapi_output_folder_name="release"
$dotnet_configuraton = "Docker"
$environment_files_path = (Resolve-Path(".\env-files")).Path

if($env -ne 'Production')
{
    $env='Development'
    $dotnet_configuraton = "DockerDevel"
}


#dotnet
$webapi_output_folder = "$webapi_path$webapi_output_folder_name"


if([System.IO.Directory]::Exists($webapi_output_folder))
{
    Write-Info("Removing old release folder")
    Remove-Item -Path $webapi_output_folder -Recurse
}

Write-Info("Publishing app...")
Write-Info("Command: dotnet publish $webapi_path -c $dotnet_configuraton -o $webapi_output_folder`n")
$dotnet = dotnet publish $webapi_path -c $dotnet_configuraton -o $webapi_output_folder

if($dotnet -match '\w+\.cs\(.*error.*csproj]'){
    $dotnet_regex = [regex]"\w+\.cs\(.*error.*csproj]"
    $dotnet_match = $dotnet_regex.Match($dotnet);
    Write-Error("Error when publishing app:")
    Write-Host $dotnet_match -f Red
    Exit
}


#nginx certs w/ config
Write-Info("Copying nginx certs...")
Write-Info("From: .\nginx\nginx-certs\$env\* To .\nginx\nginx-build\certs\`n")

Copy-Item .\nginx\nginx-certs\$env\* -Destination .\nginx\nginx-build\certs\ -Recurse

Write-Info("Copying nginx configs...")
Write-Info("From: .\nginx\nginx-config\$env\* To .\nginx\nginx-build\`n")

Copy-Item .\nginx\nginx-config\$env\*  -Destination .\nginx\nginx-build\ -Recurse

#environment files

Write-Info("Writing to rozklad-app.env file...`n")
"ASPNETCORE_ENVIRONMENT=$dotnet_configuraton" | Out-File "$environment_files_path\rozklad-app.env" -Encoding utf8

if(![System.IO.File]::Exists("$environment_files_path\mysql-db.env"))
{
    Write-Error("Environment file for mysql does not exist")
    exit
}

if(![System.IO.File]::Exists("$environment_files_path\mongo.env"))
{
    Write-Error("Environment file for mongo does not exist")
    exit
}

#docker-compose build

Write-Info("Docker compose build...`n")
docker-compose.exe build 

Write-Info("Docker compose up...`n")
docker-compose.exe up 


