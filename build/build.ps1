# Build script for difftaculous.

properties { 
    $buildDir = Split-Path $psake.build_script_file	
    $baseDir  = "$buildDir\.."
    $artifactsDir = "$baseDir\artifacts\"
    $srcDir = "$baseDir\Src"
}

$framework = '4.0x86'
# FormatTaskName (("-"*25) + "[{0}]" + ("-"*25))

Task Default -Depends Clean, Build

Task Build -Depends Clean {
    Write-Host "Building solution" -ForegroundColor Green
    # Exec { msbuild "$code_dir\helloworld\helloworld.sln" /t:Build /p:Configuration=Release /v:quiet /p:OutDir=$build_artifacts_dir } 
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
