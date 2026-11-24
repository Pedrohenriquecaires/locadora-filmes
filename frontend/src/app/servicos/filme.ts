import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export const CLASSIFICACOES = ['L', '10', '12', '14', '16', '18'];

export interface Filme {
  id?: number;
  titulo: string;
  sinopse: string | null;
  anoLancamento: number;
  duracaoMinutos: number;
  classificacaoIndicativa: string;
  precoDiaria: number;
  quantidadeTotal: number;
  generoId: number;
  // somente leitura (vêm da API)
  quantidadeDisponivel?: number;
  genero?: string;
}

@Injectable({ providedIn: 'root' })
export class FilmeService {
  private http = inject(HttpClient);
  private url = `${environment.apiUrl}/filmes`;

  listar(titulo?: string, generoId?: number): Observable<Filme[]> {
    let params = new HttpParams();
    if (titulo) params = params.set('titulo', titulo);
    if (generoId) params = params.set('generoId', generoId);
    return this.http.get<Filme[]>(this.url, { params });
  }

  obter(id: number): Observable<Filme> {
    return this.http.get<Filme>(`${this.url}/${id}`);
  }

  criar(filme: Filme): Observable<Filme> {
    return this.http.post<Filme>(this.url, filme);
  }

  atualizar(id: number, filme: Filme): Observable<Filme> {
    return this.http.put<Filme>(`${this.url}/${id}`, filme);
  }

  excluir(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
