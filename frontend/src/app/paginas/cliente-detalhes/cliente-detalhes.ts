import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { forkJoin } from 'rxjs';
import { Cliente, ClienteService } from '../../servicos/cliente';
import { Locacao, LocacaoService } from '../../servicos/locacao';
import { mensagensDeErro } from '../../utils/erros';

@Component({
  selector: 'app-cliente-detalhes',
  imports: [RouterLink, CurrencyPipe, DatePipe],
  template: `
    <main class="pagina">
      <a class="voltar" routerLink="/clientes">← Voltar para clientes</a>

      @if (erro()) {
        <div class="alerta-erro">{{ erro() }}</div>
      }

      @if (carregando()) {
        <p class="vazio">Carregando...</p>
      } @else if (cliente(); as c) {
        <div class="card perfil">
          <div class="avatar grande">{{ c.nome.charAt(0).toUpperCase() }}</div>
          <div class="perfil-dados">
            <h2>{{ c.nome }}</h2>
            <span>{{ c.email }}</span>
            <span>Telefone: {{ c.telefone || 'não informado' }}</span>
            <span>CPF: {{ c.cpf }} · Nascimento: {{ c.dataNascimento | date: 'dd/MM/yyyy' }}</span>
          </div>
        </div>

        <div class="resumo">
          <div class="card">
            <div class="rotulo">Locações</div>
            <div class="valor">{{ locacoes().length }}</div>
          </div>
          <div class="card">
            <div class="rotulo">Em aberto</div>
            <div class="valor">{{ emAberto() }}</div>
          </div>
          <div class="card">
            <div class="rotulo">Total gasto</div>
            <div class="valor verde">{{ totalGasto() | currency: 'BRL' }}</div>
          </div>
        </div>

        <div class="card">
          <h3 style="margin-bottom: 8px">Histórico de locações</h3>

          @if (locacoes().length === 0) {
            <p class="vazio">Este cliente ainda não fez nenhuma locação.</p>
          } @else {
            <div class="tabela-wrap">
              <table class="tabela-resp">
                <thead>
                  <tr>
                    <th>Filme(s)</th>
                    <th>Locação</th>
                    <th>Devolução</th>
                    <th>Status</th>
                    <th class="dir">Valor</th>
                  </tr>
                </thead>
                <tbody>
                  @for (l of locacoes(); track l.id) {
                    <tr>
                      <td data-label="Filme(s)"><strong>{{ l.filmesTexto }}</strong></td>
                      <td data-label="Locação">{{ l.dataLocacao | date: 'dd/MM/yyyy' }}</td>
                      <td data-label="Devolução">
                        {{ l.dataDevolucao ? (l.dataDevolucao | date: 'dd/MM/yyyy') : 'Previsto ' + (l.dataPrevistaDevolucao | date: 'dd/MM/yyyy') }}
                      </td>
                      <td data-label="Status">
                        <span class="status" [class]="'status ' + l.status.toLowerCase()">{{ l.status }}</span>
                      </td>
                      <td data-label="Valor" class="dir">
                        {{ l.totalPago | currency: 'BRL' }}
                        @if (l.valorMulta > 0) {
                          <span class="sub-linha"> (multa {{ l.valorMulta | currency: 'BRL' }})</span>
                        }
                      </td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          }
        </div>
      }
    </main>
  `
})
export class ClienteDetalhes implements OnInit {
  private clienteService = inject(ClienteService);
  private locacaoService = inject(LocacaoService);
  private route = inject(ActivatedRoute);

  cliente = signal<Cliente | null>(null);
  locacoes = signal<Locacao[]>([]);
  carregando = signal(true);
  erro = signal('');

  // Relatório calculado no front a partir de GET /api/locacoes?clienteId=
  emAberto = computed(() => this.locacoes().filter(l => l.status !== 'Devolvida').length);
  totalGasto = computed(() => this.locacoes().reduce((soma, l) => soma + l.totalPago, 0));

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    forkJoin({ c: this.clienteService.obter(id), l: this.locacaoService.listar(id) }).subscribe({
      next: ({ c, l }) => {
        this.cliente.set(c);
        this.locacoes.set(l);
        this.carregando.set(false);
      },
      error: (e: HttpErrorResponse) => {
        this.carregando.set(false);
        this.erro.set(mensagensDeErro(e).join(' '));
      }
    });
  }
}
