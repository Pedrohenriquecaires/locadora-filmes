# Executa a API.
#   ./scripts/executar-api.ps1                       -> biblioteca via ProjectReference
#   ./scripts/executar-api.ps1 -Pacote               -> pacote NuGet do feed local
#   ./scripts/executar-api.ps1 -Pacote -Versao 1.0.1
param([switch]$Pacote, [string]$Versao = "1.0.0")
$ErrorActionPreference = "Stop"

$Raiz    = Resolve-Path (Join-Path $PSScriptRoot "..")
$Projeto = Join-Path $Raiz "src/LocadoraFilmes.Api/LocadoraFilmes.Api.csproj"

$props = @()
if ($Pacote) {
  $props = @("-p:UsarPacoteValidacao=true", "-p:VersaoPacoteValidacao=$Versao")
  Write-Host ">> Usando o pacote LocadoraFilmes.Validation $Versao do feed local"
}

dotnet restore $Projeto @props
dotnet run --project $Projeto --no-restore @props
