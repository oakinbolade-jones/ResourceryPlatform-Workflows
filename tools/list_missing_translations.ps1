$enPath = 'c:/Projects/ResourceryPlatform-Workflows/services/workflow/src/ResourceryPlatformWorkflow.Workflow.Domain.Shared/Localization/Workflow/en.json'
$ptPath = 'c:/Projects/ResourceryPlatform-Workflows/services/workflow/src/ResourceryPlatformWorkflow.Workflow.Domain.Shared/Localization/Workflow/pt-BR.json'
$en = Get-Content $enPath -Raw | ConvertFrom-Json
$pt = Get-Content $ptPath -Raw | ConvertFrom-Json
$enTexts = $en.texts | Get-Member -MemberType NoteProperty | Select-Object -ExpandProperty Name
$ptTexts = $pt.texts | Get-Member -MemberType NoteProperty | Select-Object -ExpandProperty Name
$missingKeys = $enTexts | Where-Object { $ptTexts -notcontains $_ }
$hash = @{}
foreach ($k in $missingKeys) { $hash[$k] = $en.texts.$k }
$hash | ConvertTo-Json -Depth 10 | Out-File -Encoding utf8 tools/missing_translations.json
Write-Output "WROTE tools/missing_translations.json"
