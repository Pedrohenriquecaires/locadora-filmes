import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';

/** A API só conhece Ativa/Devolvida. "Atrasada" é calculada aqui: Ativa com prazo vencido. */
export type StatusApi = 'Ativa' | 'Devolvida';
export type StatusLocacao = 'Ativa' | 'Atrasada' | 'Devolvida';

export interface LocacaoItem {
  filmeId: number;
  titulo: string;
  precoDiaria: number;
}

interface LocacaoApi {
  id: number;
  clienteId: number;
  clienteNome: string;
  dataLocacao: string;
  dataPrevistaDevolucao: string;
  dataDevolucao: string | null;
  valorTotal: number;
  valorMulta: number;
  status: StatusApi;
  itens: LocacaoItem[];
}

export interface Locacao extends Omit<LocacaoApi, 'status'> {
  status: StatusLocacao;
  filmesTexto: string;
  /** valorTotal + multa (o que o cliente efetivamente paga) */
  totalPago: number;
}

export interface NovaLocacao {
  clienteId: number;
  filmeIds: number[];
  diasLocacao: number;
}

function enriquecer(l: LocacaoApi): Locacao {
  const atrasada = l.status === 'Ativa' && Date.parse(l.dataPrevistaDevolucao) < Date.now();
  return {
    ...l,
    status: atrasada ? 'Atrasada' : l.status,
    filmesTexto: l.itens.map(i => i.titulo).join(', '),
    totalPago: l.valorTotal + l.valorMulta
  };
}

@Injectable({ providedIn: 'root' })
export class LocacaoService {
  private http = inject(HttpClient);
  private url = `${environment.apiUrl}/locacoes`;

  listar(clienteId?: number): Observable<Locacao[]> {
    let params = new HttpParams();
    if (clienteId) params = params.set('clienteId', clienteId);
    return this.http.get<LocacaoApi[]>(this.url, { params }).pipe(map(l => l.map(enriquecer)));
  }

  criar(nova: NovaLocacao): Observable<Locacao> {
    return this.http.post<LocacaoApi>(this.url, nova).pipe(map(enriquecer));
  }

  devolver(id: number): Observable<Locacao> {
    return this.http.post<LocacaoApi>(`${this.url}/${id}/devolucao`, {}).pipe(map(enriquecer));
  }

  excluir(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
