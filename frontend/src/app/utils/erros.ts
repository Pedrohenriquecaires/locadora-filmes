import { HttpErrorResponse } from '@angular/common/http';

/**
 * Converte qualquer erro HTTP da API (ASP.NET ProblemDetails) em uma lista de mensagens.
 *
 *  - 400: { errors: { Campo: ["mensagem", ...] } }   (mensagens da biblioteca de validação)
 *  - 404 / 409 / 422 / 401 (login): { title, detail }
 *  - 401 / 403 vindos do middleware JWT: corpo vazio
 */
export function mensagensDeErro(e: HttpErrorResponse): string[] {
  const corpo = e.error;

  if (corpo && typeof corpo === 'object') {
    if (corpo.errors && typeof corpo.errors === 'object') {
      const lista = (Object.values(corpo.errors) as string[][]).flat();
      if (lista.length > 0) return lista;
    }
    if (corpo.detail) return [corpo.detail];
    if (corpo.title) return [corpo.title];
  }

  switch (e.status) {
    case 0: return ['Não foi possível conectar à API. Verifique se ela está rodando.'];
    case 401: return ['Sessão expirada. Faça login novamente.'];
    case 403: return ['Você não tem permissão para realizar esta ação.'];
    case 404: return ['Registro não encontrado.'];
    default: return [`Erro inesperado (${e.status}).`];
  }
}
