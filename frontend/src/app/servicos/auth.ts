import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Usuario {
  id: number;
  nome: string;
  email: string;
  perfil: 'Admin' | 'Atendente';
}

export interface LoginResposta {
  token: string;
  tipoToken: string;
  expiraEm: string;
  usuario: Usuario;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private readonly chaveToken = 'token';
  private readonly chaveExpira = 'expiraEm';
  private readonly chaveUsuario = 'usuario';

  logado = signal(this.estaLogado());
  usuario = signal<Usuario | null>(this.lerUsuario());
  ehAdmin = computed(() => this.usuario()?.perfil === 'Admin');

  login(email: string, senha: string): Observable<LoginResposta> {
    return this.http
      .post<LoginResposta>(`${environment.apiUrl}/auth/login`, { email, senha })
      .pipe(
        tap(r => {
          localStorage.setItem(this.chaveToken, r.token);
          localStorage.setItem(this.chaveExpira, r.expiraEm);
          localStorage.setItem(this.chaveUsuario, JSON.stringify(r.usuario));
          this.usuario.set(r.usuario);
          this.logado.set(true);
        })
      );
  }

  logout(): void {
    localStorage.removeItem(this.chaveToken);
    localStorage.removeItem(this.chaveExpira);
    localStorage.removeItem(this.chaveUsuario);
    this.usuario.set(null);
    this.logado.set(false);
  }

  obterToken(): string | null {
    return localStorage.getItem(this.chaveToken);
  }

  /** Logado = tem token e o token ainda não expirou. */
  estaLogado(): boolean {
    const token = localStorage.getItem(this.chaveToken);
    if (!token) return false;

    const expira = localStorage.getItem(this.chaveExpira);
    return !expira || Date.parse(expira) > Date.now();
  }

  private lerUsuario(): Usuario | null {
    try {
      const texto = localStorage.getItem(this.chaveUsuario);
      return texto ? (JSON.parse(texto) as Usuario) : null;
    } catch {
      return null;
    }
  }
}
