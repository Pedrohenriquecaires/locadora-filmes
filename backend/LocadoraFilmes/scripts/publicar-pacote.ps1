# Executa os testes, empacota a biblioteca de validação (.nupkg + .snupkg)
# e publica no feed NuGet local (./nuget-local).
# Uso: ./scripts/publicar-pacote.ps1 [-Versao 1.0.0]
param([string]$Versao = "1.0.0")
$ErrorActionPreference = "Stop"

$Raiz  = Resolve-Path (Join-Path $PSScriptRoot "..")
$Feed  = Join-Path $Raiz "nuget-local"
$Saida = Join-Path $Raiz "artifacts/nupkg"

New-Item -ItemType Directory -Force -Path $Feed, $Saida | Out-Null
Remove-Item (Join-Path $Saida "*") -Force -ErrorAction SilentlyContinue

Write-Host ">> [1/4] Executando testes unitários (xUnit)"
dotnet test (Join-Path $Raiz "tests/LocadoraFilmes.Validation.Tests/LocadoraFilmes.Validation.Tests.csproj") -c Release
if ($LASTEXITCODE -ne 0) { throw "Testes falharam." }

Write-Host ">> [2/4] Gerando pacotes NuGet (versão $Versao)"
dotnet pack (Join-Path $Raiz "src/LocadoraFilmes.Validation/LocadoraFilmes.Validation.csproj") `
  -c Release "-p:Version=$Versao" -o $Saida
if ($LASTEXITCODE -ne 0) { throw "dotnet pack falhou." }

Write-Host ">> [3/4] Publicando no feed local: $Feed"
Get-ChildItem $Saida -Filter "*.nupkg" | Where-Object { $_.Extension -eq ".nupkg" } | ForEach-Object {
  dotnet nuget push $_.FullName --source $Feed
  if ($LASTEXITCODE -ne 0) { throw "dotnet nuget push falhou." }
}
Get-ChildItem $Saida -Filter "*.snupkg" | Copy-Item -Destination $Feed -Force

Write-Host ">> [4/4] Limpando cache local do pacote"
$Cache = Join-Path $env:USERPROFILE ".nuget/packages/locadorafilmes.validation"
if (Test-Path $Cache) { Remove-Item $Cache -Recurse -Force }

Write-Host "`nPacotes no feed local:"
Get-ChildItem $Feed | Where-Object { $_.Name -ne ".gitkeep" } | ForEach-Object { $_.Name }
