import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Dashboard as DashboardDados, DashboardService } from '../../servicos/dashboard';
import { Locacao } from '../../servicos/locacao';
import { mensagensDeErro } from '../../utils/erros';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { HlmCardImports } from '@spartan-ng/helm/card';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink, CurrencyPipe, DatePipe, ...HlmButtonImports, ...HlmCardImports],
  styleUrl: './dashboard.css',
  template: `
    <main class="pagina dashboard-page">
      <section class="dash-boas-vindas">
        <div>
          <span class="eyebrow">PAINEL DE CONTROLE</span>
          <h1>Visão geral</h1>
          <p class="subtitulo">Acompanhe o desempenho da sua locadora em um só lugar.</p>
        </div>
        <a hlmBtn routerLink="/locacoes/nova" class="dash-cta"><span aria-hidden="true">＋</span> Nova locação</a>
      </section>

      @if (erro()) {
        <div class="alerta-erro" role="alert">{{ erro() }}</div>
      }

      @if (dados(); as d) {
        <section class="stats" aria-label="Indicadores da locadora">
          <article hlmCard class="card stat stat-clientes">
            <div class="stat-topo"><span class="stat-icone">♙</span><span class="stat-tag">CADASTRO</span></div>
            <div class="num">{{ d.totalClientes }}</div><div class="rot">Clientes cadastrados</div>
          </article>
          <article hlmCard class="card stat stat-filmes">
            <div class="stat-topo"><span class="stat-icone">▣</span><span class="stat-tag">CATÁLOGO</span></div>
            <div class="num">{{ d.totalFilmes }}</div><div class="rot">Filmes disponíveis</div>
          </article>
          <article hlmCard class="card stat stat-ativas">
            <div class="stat-topo"><span class="stat-icone">⇄</span><span class="stat-tag">EM ANDAMENTO</span></div>
            <div class="num">{{ d.locacoesAtivas }}</div><div class="rot">Locações ativas</div>
          </article>
          <article hlmCard class="card stat stat-atrasadas">
            <div class="stat-topo"><span class="stat-icone">◷</span><span class="stat-tag">ATENÇÃO</span></div>
            <div class="num">{{ d.locacoesAtrasadas }}</div><div class="rot">Locações em atraso</div>
          </article>
          <article hlmCard class="card stat stat-receita">
            <div class="stat-topo"><span class="stat-icone">R$</span><span class="stat-tag">RESULTADO</span></div>
            <div class="num receita-num">{{ d.receitaTotal | currency: 'BRL' }}</div><div class="rot">Receita de devoluções</div>
          </article>
        </section>

        <section class="dash-grid dash-grid-principal">
          <article hlmCard class="card dash-card grafico-card">
            <div class="dash-card-heading">
              <div><span class="eyebrow">ATIVIDADE DA LOCADORA</span><h2>Filmes alugados</h2><p class="card-descricao">Quantidade de títulos alugados nos últimos 6 meses</p></div>
              <span class="grafico-legenda"><i></i> Aluguéis</span>
            </div>
            <div class="grafico-barras" role="img" aria-label="Gráfico de filmes alugados nos últimos seis meses">
              @for (m of d.alugueisPorMes; track m.mes; let i = $index) {
                <div class="grafico-coluna">
                  <strong>{{ m.quantidade }}</strong>
                  <div class="grafico-trilho"><span [style.height.%]="(m.quantidade / maxMes()) * 100"></span></div>
                  <small>{{ m.mes }}</small>
                </div>
              }
            </div>
          </article>
          <article hlmCard class="card dash-card ranking-card">
            <div class="dash-card-heading">
              <div><span class="eyebrow">CLIENTES EM DESTAQUE</span><h2>Ranking de clientes</h2><p class="card-descricao">Quem mais alugou filmes</p></div>
              <a routerLink="/clientes" class="link-discreto">Ver todos ↗</a>
            </div>
            <div class="ranking-lista">
              @for (c of d.rankingClientes; track c.clienteId; let i = $index) {
                <div class="ranking-cliente">
                  <span class="ranking-pos" [class.top-tres]="i < 3">{{ (i + 1).toString().padStart(2, '0') }}</span>
                  <span class="ranking-avatar">{{ c.nome.charAt(0).toUpperCase() }}</span>
                  <strong>{{ c.nome }}</strong>
                  <span class="ranking-total">{{ c.quantidade }} <small>{{ c.quantidade === 1 ? 'filme' : 'filmes' }}</small></span>
                </div>
              } @empty { <p class="vazio">Ainda não há aluguéis para montar o ranking.</p> }
            </div>
          </article>
        </section>
        <section class="dash-grid dash-grid-secundaria">
          <article hlmCard class="card dash-card">
            <div class="dash-card-heading"><div><span class="eyebrow">DESTAQUES DO CATÁLOGO</span><h2>Filmes mais locados</h2></div><a routerLink="/filmes" class="link-discreto">Ver catálogo ↗</a></div>
            @if (d.topFilmes.length === 0) { <p class="vazio">Ainda não existem dados de locação para exibir.</p> }
            @for (t of d.topFilmes; track t.titulo; let i = $index) {
              <div class="barra-linha"><div class="barra-topo"><span class="filme-ranking"><span class="ranking-num">{{ (i + 1).toString().padStart(2, '0') }}</span>{{ t.titulo }}</span><strong>{{ t.quantidade }} <small>locações</small></strong></div><div class="barra"><span [style.width.%]="(t.quantidade / maxTop()) * 100"></span></div></div>
            }
          </article>
          <article hlmCard class="card dash-card">
            <div class="dash-card-heading"><div><span class="eyebrow">MOVIMENTAÇÕES</span><h2>Últimas locações</h2></div><a routerLink="/locacoes" class="link-discreto">Ver todas ↗</a></div>
            <div class="lista-simples">
              @for (l of recentes(); track l.id) {
                <div class="item"><div class="locacao-item-info"><span class="locacao-mini-icone">▣</span><div><strong>{{ l.filmesTexto }}</strong><small>{{ l.clienteNome }} · {{ l.dataLocacao | date: 'dd/MM/yyyy' }}</small></div></div><span class="status" [class]="'status ' + l.status.toLowerCase()">{{ l.status }}</span></div>
              } @empty { <p class="vazio">Nenhuma locação registrada até o momento.</p> }
            </div>
          </article>
        </section>
      } @else if (carregando()) {
        <section class="dashboard-loading"><span class="loading-dot"></span><p>Preparando os dados da sua locadora...</p></section>
      }
    </main>
  `,
})
export class Dashboard implements OnInit {
  private service = inject(DashboardService);

  dados = signal<DashboardDados | null>(null);
  locacoes = signal<Locacao[]>([]);
  carregando = signal(true);
  erro = signal('');

  recentes = computed(() => this.locacoes().slice(0, 5));
  maxTop = computed(() => Math.max(1, ...(this.dados()?.topFilmes.map(t => t.quantidade) ?? [1])));
  maxMes = computed(() => Math.max(1, ...(this.dados()?.alugueisPorMes.map(m => m.quantidade) ?? [1])));

  ngOnInit(): void {
    this.service.obter().subscribe({
      next: r => {
        this.dados.set(r.resumo);
        this.locacoes.set(r.locacoes);
        this.carregando.set(false);
      },
      error: (e: HttpErrorResponse) => {
        this.carregando.set(false);
        this.erro.set(mensagensDeErro(e).join(' '));
      }
    });
  }
}
