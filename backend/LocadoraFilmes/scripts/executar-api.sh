#!/usr/bin/env bash
# Executa a API.
#   ./scripts/executar-api.sh              -> usa a biblioteca via ProjectReference
#   ./scripts/executar-api.sh --pacote     -> usa o pacote NuGet do feed local (rode publicar-pacote.sh antes)
#   ./scripts/executar-api.sh --pacote 1.0.1
set -euo pipefail

RAIZ="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PROJETO="$RAIZ/src/LocadoraFilmes.Api/LocadoraFilmes.Api.csproj"

if [[ "${1:-}" == "--pacote" ]]; then
  VERSAO="${2:-1.0.0}"
  PROPS=(-p:UsarPacoteValidacao=true -p:VersaoPacoteValidacao="$VERSAO")
  echo ">> Usando o pacote LocadoraFilmes.Validation $VERSAO do feed local"
else
  PROPS=()
fi

dotnet restore "$PROJETO" "${PROPS[@]}"
dotnet run --project "$PROJETO" --no-restore "${PROPS[@]}"
