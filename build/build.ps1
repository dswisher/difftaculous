# Build script for difftaculous.

properties { 
    $buildDir = Split-Path $psake.build_script_file	
    $baseDir  = "$buildDir\.."
    $artifactsDir = "$baseDir\artifacts\"
    $srcDir = "$baseDir\Src"
    $majorVersion = "0.1"
    $majorMinorVersion = "$majorVersion.5"
    $version = GetVersion $majorMinorVersion
}

$framework = '4.0x86'
FormatTaskName (("-"*25) + "[{0}]" + ("-"*25))


Task Default -Depends Package


Task Build -Depends Clean {
    Write-Host -ForegroundColor Green "Updating CommonAssemblyInfo..."
    Write-Host
    Write-CommonAssembluInfo $srcDir ($majorVersion + '.0.0') $version

    Write-Host "Building solution, version $version" -ForegroundColor Green
    Exec { msbuild "$srcDir\difftaculous.sln" /t:Build /p:Configuration=Release /v:quiet /p:OutDir=$artifactsDir } 
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
    Write-Host -ForegroundColor Green "Running tests..."
    Write-Host
    Exec { ..\Tools\NUnit\nunit-console.exe "$artifactsDir\Difftaculous.Test.dll" /framework=net-4.0 /xml:$artifactsDir\tests.xml | Out-Default }
}


Task Package -Depends Test {
    $fileName = $artifactsDir + "difftaculous.nuspec"

    Write-Host -ForegroundColor Green "Creating nuspec file..."
    Write-NuSpec $fileName $majorMinorVersion

    Write-Host -ForegroundColor Green "Creating nuget package..."
    # exec { ..\Tools\NuGet\NuGet.exe pack $workingDir\NuGet\Newtonsoft.Json.nuspec -Symbols }
    exec { ..\Tools\NuGet\NuGet.exe pack -OutputDirectory $artifactsDir -Verbosity detailed $fileName  }
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
    $content += '[assembly: AssemblyCopyright("Copyright (c) Doug Swisher 2014")]' + [Environment]::NewLine
    $content += '[assembly: AssemblyCompany("difftaculous.net")]' + [Environment]::NewLine
    $content += '[assembly: AssemblyProduct("Difftaculous")]' + [Environment]::NewLine
    $content += [Environment]::NewLine
    $content += '[assembly: AssemblyVersionAttribute("' + $assemblyVersionNumber + '")]' + [Environment]::NewLine
    $content += '[assembly: AssemblyFileVersionAttribute("' + $fileVersionNumber + '")]' + [Environment]::NewLine

    $fileName = $sourceDir + '\CommonAssemblyInfo.cs'

    Write-Host $fileName

    $content | Set-Content $fileName
}


function Write-NuSpec ([string] $fileName, [string] $version)
{
    Write-Host "Writing nuspec file to" $fileName

    $content = '<?xml version="1.0" encoding="utf-8"?>' + [Environment]::NewLine
    $content += '<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">' + [Environment]::NewLine
    $content += '  <metadata>' + [Environment]::NewLine
    $content += '    <id>difftaculous</id>' + [Environment]::NewLine
    $content += '    <version>' + $version + '-alpha</version>' + [Environment]::NewLine
    $content += '    <title>difftaculous</title>' + [Environment]::NewLine
    $content += '    <authors>Doug Swisher</authors>' + [Environment]::NewLine
    $content += '    <projectUrl>http://difftaculous.net/</projectUrl>' + [Environment]::NewLine
    $content += '    <requireLicenseAcceptance>false</requireLicenseAcceptance>' + [Environment]::NewLine
    $content += '    <description>Library for computing differences between JSON and/or XML</description>' + [Environment]::NewLine
    $content += '    <copyright>Copyright (c) Doug Swisher 2014</copyright>' + [Environment]::NewLine
    $content += '    <tags>json xml diff</tags>' + [Environment]::NewLine
    $content += '    <dependencies>' + [Environment]::NewLine
    # TODO - how far back can we go for the Json.Net dependency?
    $content += '      <dependency id="Newtonsoft.Json" version="5.0.8" />' + [Environment]::NewLine
    $content += '    </dependencies>' + [Environment]::NewLine
    $content += '  </metadata>' + [Environment]::NewLine
    $content += '  <files>' + [Environment]::NewLine
    $content += '    <file src="Difftaculous.dll" target="lib\net45" />' + [Environment]::NewLine
    $content += '    <file src="Difftaculous.pdb" target="lib\net45" />' + [Environment]::NewLine
    $content += '    <file src="Difftaculous.xml" target="lib\net45" />' + [Environment]::NewLine
    $content += '  </files>' + [Environment]::NewLine
    $content += '</package>' + [Environment]::NewLine

    $content | Set-Content $fileName
}
