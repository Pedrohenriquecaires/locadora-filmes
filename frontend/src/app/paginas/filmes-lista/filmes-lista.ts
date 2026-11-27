import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CurrencyPipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Filme, FilmeService } from '../../servicos/filme';
import { AuthService } from '../../servicos/auth';
import { AvisosService } from '../../servicos/avisos';
import { mensagensDeErro } from '../../utils/erros';

@Component({
  selector: 'app-filmes-lista',
  imports: [RouterLink, CurrencyPipe],
  template: `
    <main class="pagina">
      <div class="cabecalho-pagina">
        <div>
          <h2>Filmes</h2>
          <p class="subtitulo">{{ filmes().length }} no catálogo</p>
        </div>
        <a class="btn btn-primario dash-cta" routerLink="/filmes/novo">Novo filme</a>
      </div>

      @if (erro()) {
        <div class="alerta-erro">{{ erro() }}</div>
      }

      <div class="toolbar">
        <input class="busca" placeholder="Buscar filme..." (input)="busca.set($any($event.target).value)" />
        <select (change)="genero.set($any($event.target).value)">
          <option value="">Todos os gêneros</option>
          @for (g of generos(); track g) {
            <option [value]="g">{{ g }}</option>
          }
        </select>
        <select (change)="ordem.set($any($event.target).value)">
          <option value="titulo">Ordenar: título</option>
          <option value="ano">Ordenar: mais recentes</option>
          <option value="preco">Ordenar: menor preço</option>
        </select>
      </div>

      @if (carregando()) {
        <p class="vazio">Carregando...</p>
      } @else if (visiveis().length === 0) {
        <div class="card vazio">Nenhum filme encontrado.</div>
      } @else {
        <div class="grade-filmes">
          @for (f of visiveis(); track f.id) {
            <article class="filme">
              <div class="filme-corpo">
                <div style="display: flex; gap: 6px; flex-wrap: wrap">
                  <span class="badge">{{ f.genero }}</span>
                  <span class="badge">Class. {{ f.classificacaoIndicativa }}</span>
                </div>
                <h3>{{ f.titulo }}</h3>
                @if (f.diretor) {
                  <div class="filme-info"><span>Dir. {{ f.diretor }}</span></div>
                }
                <div class="filme-info">
                  <span>{{ f.anoLancamento }} · {{ f.duracaoMinutos }} min</span>
                </div>
                <div class="filme-info">
                  <span [style.color]="f.quantidadeDisponivel === 0 ? 'var(--destructive)' : ''">
                    {{ f.quantidadeDisponivel }}/{{ f.quantidadeTotal }} disponíveis
                  </span>
                  <span class="preco">{{ f.precoDiaria | currency: 'BRL' }}<small>/dia</small></span>
                </div>
                <div class="filme-acoes">
                  <a class="btn btn-sec btn-sm" [routerLink]="['/filmes', f.id, 'editar']" [state]="{ filme: f }">Editar</a>
                  @if (auth.ehAdmin()) {
                    <button class="btn btn-perigo btn-sm" (click)="excluir(f)">Excluir</button>
                  }
                </div>
              </div>
            </article>
          }
        </div>
      }
    </main>
  `
})
export class FilmesLista implements OnInit {
  private service = inject(FilmeService);
  private avisos = inject(AvisosService);
  auth = inject(AuthService);

  filmes = signal<Filme[]>([]);
  busca = signal('');
  genero = signal('');
  ordem = signal('titulo');
  carregando = signal(true);
  erro = signal('');

  generos = computed(() => [...new Set(this.filmes().map(f => f.genero ?? ''))].filter(Boolean).sort());

  visiveis = computed(() => {
    const termo = this.busca().toLowerCase().trim();
    const lista = this.filmes().filter(f =>
      f.titulo.toLowerCase().includes(termo) && (!this.genero() || f.genero === this.genero())
    );
    switch (this.ordem()) {
      case 'ano': return lista.sort((a, b) => b.anoLancamento - a.anoLancamento);
      case 'preco': return lista.sort((a, b) => a.precoDiaria - b.precoDiaria);
      default: return lista.sort((a, b) => a.titulo.localeCompare(b.titulo));
    }
  });

  ngOnInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.carregando.set(true);
    this.service.listar().subscribe({
      next: lista => {
        this.filmes.set(lista);
        this.carregando.set(false);
      },
      error: (e: HttpErrorResponse) => {
        this.carregando.set(false);
        this.erro.set(mensagensDeErro(e).join(' '));
      }
    });
  }

  async excluir(f: Filme): Promise<void> {
    const ok = await this.avisos.confirmar('Excluir filme', `Deseja realmente excluir "${f.titulo}"?`, 'Excluir');
    if (!ok) return;

    this.service.excluir(f.id!).subscribe({
      next: () => {
        this.avisos.sucesso('Filme excluído.');
        this.carregar();
      },
      error: (e: HttpErrorResponse) => this.avisos.erro(mensagensDeErro(e).join(' '))
    });
  }
}
