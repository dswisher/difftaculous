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

Task Default -Depends Test

Task Build -Depends Clean {
    Write-Host -ForegroundColor Green "Updating CommonAssemblyInfo"
    Write-Host
    Write-CommonAssembluInfo $srcDir ($majorVersion + '.0.0') $version

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


Task Test -Depends Build {
}


function GetVersion($majorVersion)
{
    $now = [DateTime]::Now
    
    $months = (($now.Year - 2014) * 12) + $now.Month
    $build = "{0}{1:00}" -f $months, $now.Day

    return $majorVersion + "." + $build
}


function Write-CommonAssembluInfo ([string] $sourceDir, [string] $assemblyVersionNumber, [string] $fileVersionNumber)
{
    $content = [Environment]::NewLine
    $content += '// Auto-generated content' + [Environment]::NewLine
    $content += [Environment]::NewLine
    $content += 'using System;' + [Environment]::NewLine
    $content += 'using System.Reflection;' + [Environment]::NewLine
    $content += [Environment]::NewLine
    $content += '[assembly: AssemblyCopyright("Copyright © Doug Swisher 2014")]' + [Environment]::NewLine
    $content += '[assembly: AssemblyCompany("difftaculous.net")]' + [Environment]::NewLine
    $content += '[assembly: AssemblyProduct("Difftaculous")]' + [Environment]::NewLine
    $content += [Environment]::NewLine
    $content += '[assembly: AssemblyVersionAttribute("' + $assemblyVersionNumber + '")]' + [Environment]::NewLine
    $content += '[assembly: AssemblyFileVersionAttribute("' + $fileVersionNumber + '")]' + [Environment]::NewLine

    $file = $sourceDir + '\CommonAssemblyInfo.cs'

    Write-Host $file

    $content | Set-Content $file
}
