import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../servicos/auth';
import { TemaService } from '../../servicos/tema';

@Component({
  selector: 'app-menu',
  imports: [RouterLink, RouterLinkActive],
  template: `
    <aside class="menu-lateral">
      <a class="logo" routerLink="/dashboard" aria-label="Estokly - início">
        <img class="logo-imagem" src="/assets/logo-estokly.png" alt="Estokly — gestão de estoque e locações" />
      </a>
      <div class="nav-grupo">
        <span class="nav-legenda">MENU PRINCIPAL</span>
        <nav aria-label="Navegação principal">
          <a routerLink="/dashboard" routerLinkActive="ativo" [routerLinkActiveOptions]="{ exact: true }"><span class="nav-icone">▦</span>Dashboard</a>
          <a routerLink="/clientes" routerLinkActive="ativo"><span class="nav-icone">♙</span>Clientes</a>
          <a routerLink="/filmes" routerLinkActive="ativo"><span class="nav-icone">▣</span>Filmes</a>
          <a routerLink="/locacoes" routerLinkActive="ativo"><span class="nav-icone">⇄</span>Locações</a>
        </nav>
      </div>
      <div class="menu-rodape">
        @if (auth.usuario(); as u) {
          <div class="usuario-chip"><span class="usuario-avatar">{{ u.nome.charAt(0).toUpperCase() }}</span><span class="usuario-info"><strong>{{ u.nome }}</strong><small>{{ u.perfil }}</small></span></div>
        }
        <button class="acao-tema" (click)="tema.alternar()"><span>{{ tema.escuro() ? '☀' : '☾' }}</span>{{ tema.escuro() ? 'Tema claro' : 'Tema escuro' }}</button>
        <button class="botao-sair" (click)="sair()"><span>↗</span>Sair da conta</button>
      </div>
    </aside>
  `,
  styleUrl: './menu.css'
})
export class Menu {
  auth = inject(AuthService);
  private router = inject(Router);
  tema = inject(TemaService);

  sair(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
