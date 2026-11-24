import { Routes } from '@angular/router';
import { authGuard } from './guardas/auth-guard';

import { Login } from './paginas/login/login';
import { Dashboard } from './paginas/dashboard/dashboard';
import { ClientesLista } from './paginas/clientes-lista/clientes-lista';
import { ClienteForm } from './paginas/cliente-form/cliente-form';
import { ClienteDetalhes } from './paginas/cliente-detalhes/cliente-detalhes';
import { FilmesLista } from './paginas/filmes-lista/filmes-lista';
import { FilmeForm } from './paginas/filme-form/filme-form';
import { LocacoesLista } from './paginas/locacoes-lista/locacoes-lista';
import { LocacaoForm } from './paginas/locacao-form/locacao-form';

export const routes: Routes = [
  { path: 'login', component: Login },

  { path: 'dashboard', component: Dashboard, canActivate: [authGuard] },

  { path: 'clientes', component: ClientesLista, canActivate: [authGuard] },
  { path: 'clientes/novo', component: ClienteForm, canActivate: [authGuard] },
  { path: 'clientes/:id/editar', component: ClienteForm, canActivate: [authGuard] },
  { path: 'clientes/:id', component: ClienteDetalhes, canActivate: [authGuard] },

  { path: 'filmes', component: FilmesLista, canActivate: [authGuard] },
  { path: 'filmes/novo', component: FilmeForm, canActivate: [authGuard] },
  { path: 'filmes/:id/editar', component: FilmeForm, canActivate: [authGuard] },

  { path: 'locacoes', component: LocacoesLista, canActivate: [authGuard] },
  { path: 'locacoes/nova', component: LocacaoForm, canActivate: [authGuard] },

  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: '**', redirectTo: 'dashboard' }
];
