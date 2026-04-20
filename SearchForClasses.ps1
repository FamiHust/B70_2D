$workspace = "c:\B70_2D"
$results = @{
    "ItemData" = @()
    "ItemsCollection" = @()
    "MaxLevel" = @()
}

# Search through all C# files
$csFiles = Get-ChildItem -Path $workspace -Recurse -Filter "*.cs" -ErrorAction SilentlyContinue

Write-Host "Searching through $($csFiles.Count) C# files..." -ForegroundColor Cyan
Write-Host ""

foreach ($file in $csFiles) {
    $content = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue
    
    if ($content -match "class\s+ItemData\b") {
        $results["ItemData"] += $file.FullName
    }
    
    if ($content -match "class\s+ItemsCollection\b") {
        $results["ItemsCollection"] += $file.FullName
    }
    
    if ($content -match "(max\s*level|level\s*limit|MAX_LEVEL|MaxLevel|MAX_ITEM_LEVEL)" -and 
        $results["MaxLevel"] -notcontains $file.FullName) {
        $results["MaxLevel"] += $file.FullName
    }
}

Write-Host "=" * 80 -ForegroundColor Green
Write-Host "SEARCH RESULTS" -ForegroundColor Green
Write-Host "=" * 80 -ForegroundColor Green
Write-Host ""

Write-Host "1. ItemData class definition:" -ForegroundColor Yellow
if ($results["ItemData"].Count -gt 0) {
    foreach ($path in $results["ItemData"]) {
        Write-Host "   $path" -ForegroundColor White
    }
} else {
    Write-Host "   NOT FOUND" -ForegroundColor Red
}

Write-Host ""
Write-Host "2. ItemsCollection class definition:" -ForegroundColor Yellow
if ($results["ItemsCollection"].Count -gt 0) {
    foreach ($path in $results["ItemsCollection"]) {
        Write-Host "   $path" -ForegroundColor White
    }
} else {
    Write-Host "   NOT FOUND" -ForegroundColor Red
}

Write-Host ""
Write-Host "3. Files with max level / level limit logic:" -ForegroundColor Yellow
if ($results["MaxLevel"].Count -gt 0) {
    foreach ($path in $results["MaxLevel"]) {
        Write-Host "   $path" -ForegroundColor White
    }
} else {
    Write-Host "   NOT FOUND" -ForegroundColor Red
}

Write-Host ""
Write-Host "=" * 80 -ForegroundColor Green
Write-Host "EXTRACTION: Key Details from Found Files" -ForegroundColor Green
Write-Host "=" * 80 -ForegroundColor Green

# Extract and display details
foreach ($path in $results["ItemData"]) {
    Write-Host ""
    Write-Host "From $([System.IO.Path]::GetFileName($path)):" -ForegroundColor Cyan
    Write-Host "---" -ForegroundColor Cyan
    $content = Get-Content $path -Raw
    
    # Extract class definition
    if ($content -match "class\s+ItemData[^{]*\{") {
        $classBlock = [regex]::Match($content, "class\s+ItemData[^{]*\{([^}]*?)(?:public|private|protected|class|\Z)", [System.Text.RegularExpressions.RegexOptions]::Singleline)
        if ($classBlock.Groups[1].Value) {
            $props = $classBlock.Groups[1].Value | Select-String "public.*{.*}" | Select-Object -First 10
            Write-Host $props.Matches.Value
        }
    }
}
