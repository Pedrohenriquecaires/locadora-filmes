import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Locacao, LocacaoService } from '../../servicos/locacao';
import { AuthService } from '../../servicos/auth';
import { AvisosService } from '../../servicos/avisos';
import { mensagensDeErro } from '../../utils/erros';

type Filtro = 'Todas' | 'Ativa' | 'Atrasada' | 'Devolvida';

@Component({
  selector: 'app-locacoes-lista',
  imports: [RouterLink, CurrencyPipe, DatePipe],
  template: `
    <main class="pagina">
      <div class="cabecalho-pagina">
        <div>
          <h2>Locações</h2>
          <p class="subtitulo">Acompanhe empréstimos, atrasos e devoluções</p>
        </div>
        <a class="btn btn-primario dash-cta" routerLink="/locacoes/nova">Nova locação</a>
      </div>

      @if (erro()) {
        <div class="alerta-erro">{{ erro() }}</div>
      }

      <div class="chips">
        @for (f of filtros; track f) {
          <button class="chip" [class.ativo]="filtro() === f" (click)="filtro.set(f)">
            {{ rotulos[f] }} ({{ contar(f) }})
          </button>
        }
      </div>

      <div class="card">
        <div class="toolbar">
          <input class="busca" placeholder="Buscar por cliente ou filme..."
                 (input)="busca.set($any($event.target).value)" />
        </div>

        @if (carregando()) {
          <p class="vazio">Carregando...</p>
        } @else if (filtradas().length === 0) {
          <p class="vazio">Nenhuma locação encontrada.</p>
        } @else {
          <div class="tabela-wrap">
            <table class="tabela-resp">
              <thead>
                <tr>
                  <th>Filme(s) / Cliente</th>
                  <th>Período</th>
                  <th>Valor</th>
                  <th>Status</th>
                  <th class="dir">Ações</th>
                </tr>
              </thead>
              <tbody>
                @for (l of filtradas(); track l.id) {
                  <tr>
                    <td data-label="Filme(s)">
                      <div style="text-align: left">
                        <strong>{{ l.filmesTexto }}</strong><br />
                        <span class="sub-linha">{{ l.clienteNome }}</span>
                      </div>
                    </td>
                    <td data-label="Período">
                      {{ l.dataLocacao | date: 'dd/MM' }} → {{ (l.dataDevolucao ?? l.dataPrevistaDevolucao) | date: 'dd/MM' }}
                    </td>
                    <td data-label="Valor">
                      {{ l.totalPago | currency: 'BRL' }}
                      @if (l.valorMulta > 0) {
                        <span class="sub-linha"> (multa {{ l.valorMulta | currency: 'BRL' }})</span>
                      }
                    </td>
                    <td data-label="Status">
                      <span class="status" [class]="'status ' + l.status.toLowerCase()">{{ l.status }}</span>
                    </td>
                    <td class="acoes">
                      @if (l.status !== 'Devolvida') {
                        <button class="btn btn-primario btn-sm dash-cta" (click)="devolver(l)">Registrar devolução</button>
                      }
                      @if (auth.ehAdmin()) {
                        <button class="btn btn-perigo btn-sm" (click)="excluir(l)">Excluir</button>
                      }
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        }
      </div>
    </main>
  `
})
export class LocacoesLista implements OnInit {
  private service = inject(LocacaoService);
  private avisos = inject(AvisosService);
  auth = inject(AuthService);

  filtros: Filtro[] = ['Todas', 'Ativa', 'Atrasada', 'Devolvida'];
  rotulos: Record<Filtro, string> = {
    Todas: 'Todas', Ativa: 'Ativas', Atrasada: 'Atrasadas', Devolvida: 'Devolvidas'
  };

  locacoes = signal<Locacao[]>([]);
  filtro = signal<Filtro>('Todas');
  busca = signal('');
  carregando = signal(true);
  erro = signal('');

  filtradas = computed(() => {
    const termo = this.busca().toLowerCase().trim();
    return this.locacoes().filter(l =>
      (this.filtro() === 'Todas' || l.status === this.filtro()) &&
      (l.clienteNome.toLowerCase().includes(termo) || l.filmesTexto.toLowerCase().includes(termo))
    );
  });

  ngOnInit(): void {
    this.carregar();
  }

  contar(f: Filtro): number {
    return f === 'Todas' ? this.locacoes().length : this.locacoes().filter(l => l.status === f).length;
  }

  carregar(): void {
    this.carregando.set(true);
    this.service.listar().subscribe({
      next: lista => {
        this.locacoes.set(lista);
        this.carregando.set(false);
      },
      error: (e: HttpErrorResponse) => {
        this.carregando.set(false);
        this.erro.set(mensagensDeErro(e).join(' '));
      }
    });
  }

  async devolver(l: Locacao): Promise<void> {
    const ok = await this.avisos.confirmar(
      'Registrar devolução',
      `Confirmar a devolução de "${l.filmesTexto}" por ${l.clienteNome}?`,
      'Devolver'
    );
    if (!ok) return;

    this.service.devolver(l.id).subscribe({
      next: r => {
        const multa = r.valorMulta > 0 ? ` (inclui multa de ${this.brl(r.valorMulta)})` : '';
        this.avisos.sucesso(`Devolução registrada. Total: ${this.brl(r.totalPago)}${multa}`);
        this.carregar();
      },
      error: (e: HttpErrorResponse) => this.avisos.erro(mensagensDeErro(e).join(' '))
    });
  }

  async excluir(l: Locacao): Promise<void> {
    const ok = await this.avisos.confirmar('Excluir locação', `Excluir a locação #${l.id} de ${l.clienteNome}?`, 'Excluir');
    if (!ok) return;

    this.service.excluir(l.id).subscribe({
      next: () => {
        this.avisos.sucesso('Locação excluída.');
        this.carregar();
      },
      error: (e: HttpErrorResponse) => this.avisos.erro(mensagensDeErro(e).join(' '))
    });
  }

  private brl(v: number): string {
    return 'R$ ' + v.toFixed(2).replace('.', ',');
  }
}
