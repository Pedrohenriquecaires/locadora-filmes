import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Genero {
  id: number;
  nome: string;
}

@Injectable({ providedIn: 'root' })
export class GeneroService {
  private http = inject(HttpClient);

  listar(): Observable<Genero[]> {
    return this.http.get<Genero[]>(`${environment.apiUrl}/generos`);
  }
}
