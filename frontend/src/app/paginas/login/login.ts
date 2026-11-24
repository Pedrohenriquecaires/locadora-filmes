import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../servicos/auth';
import { mensagensDeErro } from '../../utils/erros';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  template: `
    <div class="login-fundo">
      <div class="login-card">
        <div class="login-marca">
          <img class="login-logo" src="/assets/logo-estokly.png" alt="Estokly — Gestão de Estoque e Locações" />
        </div>
        <h1>Acesse sua conta</h1>
        <p class="sub">Gestão de estoque e locações em um só lugar</p>

        @if (erro) {
          <div class="alerta-erro">{{ erro }}</div>
        }

        <div class="form">
          <div class="campo">
            <label>E-mail</label>
            <input [(ngModel)]="email" name="email" placeholder="admin@locadora.com" (keyup.enter)="entrar()" />
          </div>
          <div class="campo">
            <label>Senha</label>
            <input type="password" [(ngModel)]="senha" name="senha" placeholder="••••••••" (keyup.enter)="entrar()" />
          </div>
          <button class="btn btn-primario btn-largo" (click)="entrar()" [disabled]="carregando">
            {{ carregando ? 'Entrando...' : 'Entrar' }}
          </button>
        </div>
      </div>
    </div>
  `,
  styleUrl: './login.css'
})
export class Login {
  private auth = inject(AuthService);
  private router = inject(Router);

  email = '';
  senha = '';
  erro = '';
  carregando = false;

  entrar(): void {
    this.erro = '';
    this.carregando = true;

    this.auth.login(this.email, this.senha).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: (e: HttpErrorResponse) => {
        this.carregando = false;
        this.erro = mensagensDeErro(e).join(' ');
      }
    });
  }
}
