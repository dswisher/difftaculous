# Build script for difftaculous.

properties { 
    $buildDir = Split-Path $psake.build_script_file	
    $baseDir  = "$buildDir\.."
    $artifactsDir = "$baseDir\artifacts\"
    $srcDir = "$baseDir\Src"
    $majorVersion = "0.1"
    $majorMinorVersion = "$majorVersion.3"
    $version = GetVersion $majorMinorVersion
}

$framework = '4.0x86'
FormatTaskName (("-"*25) + "[{0}]" + ("-"*25))

Task Default -Depends Clean, Build

Task Build -Depends Clean {
    Write-Host -ForegroundColor Green "Updating assembly version"
    Write-Host
    # Update-AssemblyInfoFiles $srcDir ($majorVersion + '.0.0') $version

    Write-Host "Building solution, version $version" -ForegroundColor Green
    # Exec { msbuild "$srcDir\difftaculous.sln" /t:Build /p:Configuration=Release /v:quiet /p:OutDir=$artifactsDir } 
}

Task Clean {
    Write-Host "Creating empty artifacts directory" -ForegroundColor Green
    if (Test-Path $artifactsDir) 
    {
        rd $artifactsDir -rec -force | out-null
    }

    mkdir $artifactsDir | out-null

    #Write-Host "Cleaning helloworld.sln" -ForegroundColor Green
    #Exec { msbuild "$code_dir\helloworld\helloworld.sln" /t:Clean /p:Configuration=Release /v:quiet } 
}


function GetVersion($majorVersion)
{
    $now = [DateTime]::Now
    
    $months = (($now.Year - 2014) * 12) + $now.Month
    $build = "{0}{1:00}{2:00}" -f $months, $now.Day, $now.Hour

    return $majorVersion + "." + $build
}


function Update-AssemblyInfoFiles ([string] $sourceDir, [string] $assemblyVersionNumber, [string] $fileVersionNumber)
{
    $assemblyVersionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
    $fileVersionPattern = 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
    $assemblyVersion = 'AssemblyVersion("' + $assemblyVersionNumber + '")';
    $fileVersion = 'AssemblyFileVersion("' + $fileVersionNumber + '")';

    Get-ChildItem -Path $sourceDir -r -filter AssemblyInfo.cs | ForEach-Object {

        $filename = $_.Directory.ToString() + '\' + $_.Name
        Write-Host $filename
        $filename + ' -> ' + $version

        (Get-Content $filename) | ForEach-Object {
            % {$_ -replace $assemblyVersionPattern, $assemblyVersion } |
            % {$_ -replace $fileVersionPattern, $fileVersion }
        } | Set-Content $filename
    }
}
