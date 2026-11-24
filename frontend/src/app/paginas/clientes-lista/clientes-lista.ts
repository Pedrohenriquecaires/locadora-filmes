import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { Cliente, ClienteService } from '../../servicos/cliente';
import { AuthService } from '../../servicos/auth';
import { AvisosService } from '../../servicos/avisos';
import { mensagensDeErro } from '../../utils/erros';

@Component({
  selector: 'app-clientes-lista',
  imports: [RouterLink],
  template: `
    <main class="pagina">
      <div class="cabecalho-pagina">
        <div>
          <h2>Clientes</h2>
          <p class="subtitulo">{{ clientes().length }} cadastrados</p>
        </div>
        <a class="btn btn-primario dash-cta" routerLink="/clientes/novo">Novo cliente</a>
      </div>

      @if (erro()) {
        <div class="alerta-erro">{{ erro() }}</div>
      }

      <div class="card">
        <div class="toolbar">
          <input class="busca" placeholder="Buscar por nome, CPF ou e-mail..."
                 (input)="buscar($any($event.target).value)" />
        </div>

        @if (carregando()) {
          <p class="vazio">Carregando...</p>
        } @else if (filtrados().length === 0) {
          <p class="vazio">Nenhum cliente encontrado.</p>
        } @else {
          <div class="tabela-wrap">
            <table class="tabela-resp">
              <thead>
                <tr>
                  <th class="ordenavel" (click)="asc.set(!asc())">Cliente {{ asc() ? '▲' : '▼' }}</th>
                  <th>CPF</th>
                  <th>Telefone</th>
                  <th class="dir">Ações</th>
                </tr>
              </thead>
              <tbody>
                @for (c of paginados(); track c.id) {
                  <tr>
                    <td data-label="Cliente">
                      <div class="pessoa">
                        <div class="avatar">{{ c.nome.charAt(0).toUpperCase() }}</div>
                        <div style="text-align: left">
                          {{ c.nome }}<br />
                          <span class="sub-linha">{{ c.email }}</span>
                        </div>
                      </div>
                    </td>
                    <td data-label="CPF">{{ c.cpf }}</td>
                    <td data-label="Telefone">{{ c.telefone || '—' }}</td>
                    <td class="acoes">
                      <a class="btn btn-sec btn-sm" [routerLink]="['/clientes', c.id]">Detalhes</a>
                      <a class="btn btn-sec btn-sm" [routerLink]="['/clientes', c.id, 'editar']" [state]="{ cliente: c }">Editar</a>
                      @if (auth.ehAdmin()) {
                        <button class="btn btn-perigo btn-sm" (click)="excluir(c)">Excluir</button>
                      }
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>

          <div class="paginacao">
            <span>{{ filtrados().length }} resultado(s)</span>
            <div style="display: flex; gap: 10px; align-items: center">
              <button class="btn btn-sec btn-sm" [disabled]="pagina() === 1" (click)="pagina.set(pagina() - 1)">‹ Anterior</button>
              <span>Página {{ pagina() }} de {{ totalPaginas() }}</span>
              <button class="btn btn-sec btn-sm" [disabled]="pagina() >= totalPaginas()" (click)="pagina.set(pagina() + 1)">Próxima ›</button>
            </div>
          </div>
        }
      </div>
    </main>
  `
})
export class ClientesLista implements OnInit {
  private service = inject(ClienteService);
  private avisos = inject(AvisosService);
  auth = inject(AuthService);

  clientes = signal<Cliente[]>([]);
  busca = signal('');
  asc = signal(true);
  pagina = signal(1);
  tamanho = 5;
  carregando = signal(true);
  erro = signal('');

  filtrados = computed(() => {
    const termo = this.busca().toLowerCase().trim();
    const lista = this.clientes().filter(c =>
      c.nome.toLowerCase().includes(termo) ||
      c.cpf.includes(termo) ||
      c.email.toLowerCase().includes(termo)
    );
    lista.sort((a, b) => a.nome.localeCompare(b.nome));
    return this.asc() ? lista : lista.reverse();
  });

  totalPaginas = computed(() => Math.max(1, Math.ceil(this.filtrados().length / this.tamanho)));

  paginados = computed(() => {
    const inicio = (Math.min(this.pagina(), this.totalPaginas()) - 1) * this.tamanho;
    return this.filtrados().slice(inicio, inicio + this.tamanho);
  });

  ngOnInit(): void {
    this.carregar();
  }

  buscar(valor: string): void {
    this.busca.set(valor);
    this.pagina.set(1);
  }

  carregar(): void {
    this.carregando.set(true);
    this.service.listar().subscribe({
      next: lista => {
        this.clientes.set(lista);
        this.carregando.set(false);
      },
      error: (e: HttpErrorResponse) => {
        this.carregando.set(false);
        this.erro.set(mensagensDeErro(e).join(' '));
      }
    });
  }

  async excluir(c: Cliente): Promise<void> {
    const ok = await this.avisos.confirmar('Excluir cliente', `Deseja realmente excluir ${c.nome}?`, 'Excluir');
    if (!ok) return;

    this.service.excluir(c.id!).subscribe({
      next: () => {
        this.avisos.sucesso('Cliente excluído.');
        this.carregar();
      },
      error: (e: HttpErrorResponse) => this.avisos.erro(mensagensDeErro(e).join(' '))
    });
  }
}
