import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { Location } from '@angular/common';
import { Cliente, ClienteService } from '../../servicos/cliente';
import { AvisosService } from '../../servicos/avisos';
import { mensagensDeErro } from '../../utils/erros';

@Component({
  selector: 'app-cliente-form',
  imports: [FormsModule, RouterLink],
  template: `
    <main class="pagina" style="max-width: 640px">
      <a class="voltar" routerLink="/clientes">← Voltar</a>
      <div class="cabecalho-pagina">
        <h2>{{ id ? 'Editar cliente' : 'Novo cliente' }}</h2>
      </div>

      <div class="card">
        @if (erros().length > 0) {
          <div class="alerta-erro">
            <strong>Corrija os problemas abaixo:</strong>
            <ul>
              @for (e of erros(); track $index) {
                <li>{{ e }}</li>
              }
            </ul>
          </div>
        }

        <div class="form">
          <div class="campo">
            <label>Nome completo</label>
            <input [(ngModel)]="cliente.nome" name="nome" placeholder="Ex.: Maria da Silva" />
          </div>

          <div class="grid-2">
            <div class="campo">
              <label>CPF</label>
              <input [(ngModel)]="cliente.cpf" name="cpf" maxlength="14" placeholder="000.000.000-00" />
            </div>
            <div class="campo">
              <label>Data de nascimento</label>
              <input type="date" [(ngModel)]="cliente.dataNascimento" name="nascimento" />
              <small>O cliente precisa ter 18 anos ou mais.</small>
            </div>
          </div>

          <div class="grid-2">
            <div class="campo">
              <label>Email</label>
              <input [(ngModel)]="cliente.email" name="email" placeholder="nome@email.com" />
            </div>
            <div class="campo">
              <label>Telefone (opcional)</label>
              <input [(ngModel)]="cliente.telefone" name="telefone" maxlength="15" placeholder="DDD + número" />
            </div>
          </div>

          <div class="form-botoes">
            <button class="btn btn-primario dash-cta" (click)="salvar()" [disabled]="salvando()">
              {{ salvando() ? 'Salvando...' : 'Salvar' }}
            </button>
            <a class="btn btn-sec" routerLink="/clientes">Cancelar</a>
          </div>
        </div>
      </div>
    </main>
  `
})
export class ClienteForm implements OnInit {
  private service = inject(ClienteService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private avisos = inject(AvisosService);
  private location = inject(Location);

  id: number | null = null;
  cliente: Cliente = { nome: '', cpf: '', email: '', telefone: '', dataNascimento: '' };

  erros = signal<string[]>([]);
  salvando = signal(false);

  ngOnInit(): void {
    const idRota = this.route.snapshot.paramMap.get('id');
    if (idRota) {
      this.id = +idRota;
      const estado = this.location.getState() as { cliente?: Cliente };
      if (estado.cliente && Number(estado.cliente.id) === this.id) {
        this.cliente = {
          ...estado.cliente,
          dataNascimento: estado.cliente.dataNascimento
            ? String(estado.cliente.dataNascimento).slice(0, 10)
            : ''
        };
      } else {
        this.service.obter(this.id).subscribe({
          next: c => (this.cliente = {
            ...c,
            dataNascimento: c.dataNascimento ? String(c.dataNascimento).slice(0, 10) : ''
          }),
          error: (e: HttpErrorResponse) => this.erros.set(mensagensDeErro(e))
        });
      }
    }
  }

  salvar(): void {
    this.erros.set([]);
    this.salvando.set(true);

    // A API aceita data em branco como ausente; envia null para dar a mensagem de validação correta.
    const dados: Cliente = { ...this.cliente, dataNascimento: this.cliente.dataNascimento || (null as unknown as string) };

    const requisicao = this.id
      ? this.service.atualizar(this.id, dados)
      : this.service.criar(dados);

    requisicao.subscribe({
      next: () => {
        this.avisos.sucesso('Cliente salvo com sucesso!');
        this.router.navigate(['/clientes']);
      },
      error: (e: HttpErrorResponse) => {
        this.salvando.set(false);
        this.erros.set(mensagensDeErro(e));
      }
    });
  }
}
