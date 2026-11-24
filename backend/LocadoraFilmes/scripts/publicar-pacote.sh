#!/usr/bin/env bash
# Executa os testes, empacota a biblioteca de validação (.nupkg + .snupkg)
# e publica no feed NuGet local (./nuget-local).
# Uso: ./scripts/publicar-pacote.sh [versao]   (padrão: 1.0.0)
set -euo pipefail

RAIZ="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
VERSAO="${1:-1.0.0}"
FEED="$RAIZ/nuget-local"
SAIDA="$RAIZ/artifacts/nupkg"

mkdir -p "$FEED" "$SAIDA"
rm -f "$SAIDA"/*

echo ">> [1/4] Executando testes unitários (xUnit)"
dotnet test "$RAIZ/tests/LocadoraFilmes.Validation.Tests/LocadoraFilmes.Validation.Tests.csproj" -c Release

echo ">> [2/4] Gerando pacotes NuGet (versão $VERSAO)"
dotnet pack "$RAIZ/src/LocadoraFilmes.Validation/LocadoraFilmes.Validation.csproj" \
  -c Release -p:Version="$VERSAO" -o "$SAIDA"

echo ">> [3/4] Publicando no feed local: $FEED"
dotnet nuget push "$SAIDA"/*.nupkg --source "$FEED"
cp "$SAIDA"/*.snupkg "$FEED"/ 2>/dev/null || true

echo ">> [4/4] Limpando cache local do pacote (para o restore usar a versão recém-publicada)"
rm -rf "$HOME/.nuget/packages/locadorafilmes.validation"

echo
echo "Pacotes disponíveis no feed local:"
ls -1 "$FEED" | grep -v '^.gitkeep$' || true
