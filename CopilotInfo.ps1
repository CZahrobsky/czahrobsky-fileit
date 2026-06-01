$root = if ($PSScriptRoot) { $PSScriptRoot } else { Get-Location }
$matches = Get-ChildItem -Path $root -Recurse -Filter *.csproj -File -ErrorAction SilentlyContinue |
    Select-String -Pattern '<AzureFunctionsVersion' -SimpleMatch -CaseSensitive

if ($matches) {
    $matches | ForEach-Object { "$($_.Path):$($_.LineNumber) $($_.Line.Trim())" }
} else {
    Write-Output "No '<AzureFunctionsVersion' matches found under $root"
} 

/*

PS D:\Source\Hackathon\FileIt\cz-develop\FileIt.Module.Services\FileIt.Module.Services.App> D:\Source\Hackathon\FileIt\cz-develop\CopilotInfo.ps1
D:\Source\Hackathon\FileIt\cz-develop\FileIt.Module.Services\FileIt.Module.Services.Host\FileIt.Module.Services.Host.csproj:4 <AzureFunctionsVersion>v4</AzureFunction
sVersion>
D:\Source\Hackathon\FileIt\cz-develop\FileIt.Module.SimpleFlow\FileIt.Module.SimpleFlow.Host\FileIt.Module.SimpleFlow.Host.csproj:4 <AzureFunctionsVersion>v4</AzureFu
nctionsVersion>

PS D:\Source\Hackathon\FileIt\cz-develop\FileIt.Module.Services\FileIt.Module.Services.App> cd ..

PS D:\Source\Hackathon\FileIt\cz-develop\FileIt.Module.Services> cd ..

PS D:\Source\Hackathon\FileIt\cz-develop> D:\Source\Hackathon\FileIt\cz-develop\CopilotInfo.ps1
At D:\Source\Hackathon\FileIt\cz-develop\CopilotInfo.ps1:14 char:127
+ ... t.Module.Services.Host\FileIt.Module.Services.Host.csproj:4 <AzureFun ...
+                                                                 ~
The '<' operator is reserved for future use.
At D:\Source\Hackathon\FileIt\cz-develop\CopilotInfo.ps1:16 char:133
+ ... dule.SimpleFlow.Host\FileIt.Module.SimpleFlow.Host.csproj:4 <AzureFun ...
+                                                                 ~
The '<' operator is reserved for future use.
    + CategoryInfo          : ParserError: (:) [], ParentContainsErrorRecordException
    + FullyQualifiedErrorId : RedirectionNotSupported
 

PS D:\Source\Hackathon\FileIt\cz-develop> D:\Source\Hackathon\FileIt\cz-develop\CopilotInfo.ps1
D:\Source\Hackathon\FileIt\cz-develop\FileIt.Module.Services\FileIt.Module.Services.Host\FileIt.Module.Services.Host.csproj:4 <AzureFunctionsVersion>v4</AzureFunction
sVersion>
D:\Source\Hackathon\FileIt\cz-develop\FileIt.Module.SimpleFlow\FileIt.Module.SimpleFlow.Host\FileIt.Module.SimpleFlow.Host.csproj:4 <AzureFunctionsVersion>v4</AzureFu
nctionsVersion>

PS D:\Source\Hackathon\FileIt\cz-develop> 

*/