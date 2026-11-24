import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Cliente {
  id?: number;
  nome: string;
  cpf: string;
  email: string;
  telefone: string | null;
  dataNascimento: string; // yyyy-MM-dd
}

@Injectable({ providedIn: 'root' })
export class ClienteService {
  private http = inject(HttpClient);
  private url = `${environment.apiUrl}/clientes`;

  listar(nome?: string): Observable<Cliente[]> {
    let params = new HttpParams();
    if (nome) params = params.set('nome', nome);
    return this.http.get<Cliente[]>(this.url, { params });
  }

  obter(id: number): Observable<Cliente> {
    return this.http.get<Cliente>(`${this.url}/${id}`);
  }

  criar(cliente: Cliente): Observable<Cliente> {
    return this.http.post<Cliente>(this.url, cliente);
  }

  atualizar(id: number, cliente: Cliente): Observable<Cliente> {
    return this.http.put<Cliente>(`${this.url}/${id}`, cliente);
  }

  excluir(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
