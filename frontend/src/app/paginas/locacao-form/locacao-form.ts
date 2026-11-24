import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CurrencyPipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { forkJoin } from 'rxjs';
import { Cliente, ClienteService } from '../../servicos/cliente';
import { Filme, FilmeService } from '../../servicos/filme';
import { LocacaoService } from '../../servicos/locacao';
import { AvisosService } from '../../servicos/avisos';
import { mensagensDeErro } from '../../utils/erros';

@Component({
  selector: 'app-locacao-form',
  imports: [FormsModule, RouterLink, CurrencyPipe],
  template: `
    <main class="pagina" style="max-width: 640px">
      <a class="voltar" routerLink="/locacoes">← Voltar</a>
      <div class="cabecalho-pagina"><h2>Nova locação</h2></div>

      <div class="card">
        @if (erros().length > 0) {
          <div class="alerta-erro">
            <strong>Não foi possível registrar:</strong>
            <ul>
              @for (e of erros(); track $index) {
                <li>{{ e }}</li>
              }
            </ul>
          </div>
        }

        @if (carregando()) {
          <p class="vazio">Carregando...</p>
        } @else {
          <div class="form">
            <div class="campo">
              <label>Cliente</label>
              <select [(ngModel)]="clienteId" name="cliente">
                <option [ngValue]="null" disabled>Selecione um cliente...</option>
                @for (c of clientes(); track c.id) {
                  <option [ngValue]="c.id">{{ c.nome }}</option>
                }
              </select>
            </div>

            <div class="campo">
              <label>Filmes (até 10 por locação)</label>
              <div style="display: grid; gap: 6px; max-height: 260px; overflow-y: auto">
                @for (f of filmes(); track f.id) {
                  <label style="display: flex; gap: 10px; align-items: center; font-weight: 400;
                                padding: 8px 10px; border: 1px solid var(--border); border-radius: 8px;
                                cursor: pointer" [style.opacity]="f.quantidadeDisponivel === 0 ? 0.5 : 1">
                    <input type="checkbox" style="width: 18px; height: 18px"
                           [checked]="selecionados().includes(f.id!)"
                           [disabled]="f.quantidadeDisponivel === 0"
                           (change)="alternar(f.id!)" />
                    <span style="flex: 1">
                      {{ f.titulo }} ({{ f.anoLancamento }})
                      <span class="sub-linha"> · {{ f.quantidadeDisponivel }} disp.</span>
                    </span>
                    <strong>{{ f.precoDiaria | currency: 'BRL' }}</strong>
                  </label>
                }
              </div>
            </div>

            <div class="campo">
              <label>Quantidade de dias</label>
              <input type="number" min="1" max="30" [(ngModel)]="dias" name="dias" />
              <small>Entre 1 e 30 dias. Atraso gera multa de 50% da soma das diárias por dia.</small>
            </div>

            @if (selecionados().length > 0) {
              <div class="preview">
                <strong>{{ selecionados().length }} filme(s)</strong> · {{ somaDiarias() | currency: 'BRL' }}/dia
                × {{ dias() || 0 }} dia(s) = <strong>{{ estimado() | currency: 'BRL' }}</strong>
              </div>
            }

            <div class="form-botoes">
              <button class="btn btn-primario dash-cta" (click)="salvar()" [disabled]="salvando()">
                {{ salvando() ? 'Salvando...' : 'Registrar locação' }}
              </button>
              <a class="btn btn-sec" routerLink="/locacoes">Cancelar</a>
            </div>
          </div>
        }
      </div>
    </main>
  `
})
export class LocacaoForm implements OnInit {
  private clienteService = inject(ClienteService);
  private filmeService = inject(FilmeService);
  private service = inject(LocacaoService);
  private router = inject(Router);
  private avisos = inject(AvisosService);

  clientes = signal<Cliente[]>([]);
  filmes = signal<Filme[]>([]);
  selecionados = signal<number[]>([]);
  carregando = signal(true);
  salvando = signal(false);
  erros = signal<string[]>([]);

  clienteId: number | null = null;
  dias = signal(3);

  somaDiarias = computed(() =>
    this.filmes().filter(f => this.selecionados().includes(f.id!)).reduce((s, f) => s + f.precoDiaria, 0)
  );
  estimado = computed(() => this.somaDiarias() * (this.dias() || 0));

  ngOnInit(): void {
    forkJoin({ c: this.clienteService.listar(), f: this.filmeService.listar() }).subscribe({
      next: ({ c, f }) => {
        this.clientes.set(c);
        this.filmes.set(f);
        this.carregando.set(false);
      },
      error: (e: HttpErrorResponse) => {
        this.carregando.set(false);
        this.erros.set(mensagensDeErro(e));
      }
    });
  }

  alternar(id: number): void {
    this.selecionados.update(lista => (lista.includes(id) ? lista.filter(x => x !== id) : [...lista, id]));
  }

  salvar(): void {
    if (this.clienteId === null) {
      this.erros.set(['Selecione o cliente.']);
      return;
    }

    this.erros.set([]);
    this.salvando.set(true);

    this.service
      .criar({ clienteId: this.clienteId, filmeIds: this.selecionados(), diasLocacao: this.dias() })
      .subscribe({
        next: () => {
          this.avisos.sucesso('Locação registrada com sucesso!');
          this.router.navigate(['/locacoes']);
        },
        error: (e: HttpErrorResponse) => {
          this.salvando.set(false);
          this.erros.set(mensagensDeErro(e));
        }
      });
  }
}
