import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { Location } from '@angular/common';
import { CLASSIFICACOES, Filme, FilmeService } from '../../servicos/filme';
import { Genero, GeneroService } from '../../servicos/genero';
import { AvisosService } from '../../servicos/avisos';
import { mensagensDeErro } from '../../utils/erros';

@Component({
  selector: 'app-filme-form',
  imports: [FormsModule, RouterLink],
  template: `
    <main class="pagina" style="max-width: 640px">
      <a class="voltar" routerLink="/filmes">← Voltar</a>
      <div class="cabecalho-pagina">
        <h2>{{ id ? 'Editar filme' : 'Novo filme' }}</h2>
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
            <label>Título</label>
            <input [(ngModel)]="filme.titulo" name="titulo" placeholder="Ex.: Matrix" />
          </div>

          <div class="campo">
            <label>Sinopse (opcional)</label>
            <input [(ngModel)]="filme.sinopse" name="sinopse" placeholder="Resumo do filme" />
          </div>

          <div class="campo">
            <label>Diretor (opcional)</label>
            <input [(ngModel)]="filme.diretor" name="diretor" placeholder="Ex.: Christopher Nolan" />
          </div>

          <div class="grid-2">
            <div class="campo">
              <label>Gênero</label>
              <select [(ngModel)]="filme.generoId" name="genero">
                <option [ngValue]="0" disabled>Selecione...</option>
                @for (g of generos(); track g.id) {
                  <option [ngValue]="g.id">{{ g.nome }}</option>
                }
              </select>
            </div>
            <div class="campo">
              <label>Classificação indicativa</label>
              <select [(ngModel)]="filme.classificacaoIndicativa" name="classificacao">
                @for (c of classificacoes; track c) {
                  <option [value]="c">{{ c === 'L' ? 'Livre' : c + ' anos' }}</option>
                }
              </select>
            </div>
          </div>

          <div class="grid-2">
            <div class="campo">
              <label>Ano de lançamento</label>
              <input type="number" [(ngModel)]="filme.anoLancamento" name="ano" />
            </div>
            <div class="campo">
              <label>Duração (minutos)</label>
              <input type="number" [(ngModel)]="filme.duracaoMinutos" name="duracao" />
            </div>
          </div>

          <div class="grid-2">
            <div class="campo">
              <label>Preço da diária (R$)</label>
              <input type="number" step="0.01" [(ngModel)]="filme.precoDiaria" name="preco" />
            </div>
            <div class="campo">
              <label>Quantidade de cópias</label>
              <input type="number" [(ngModel)]="filme.quantidadeTotal" name="quantidade" />
            </div>
          </div>

          <div class="form-botoes">
            <button class="btn btn-primario dash-cta" (click)="salvar()" [disabled]="salvando()">
              {{ salvando() ? 'Salvando...' : 'Salvar' }}
            </button>
            <a class="btn btn-sec" routerLink="/filmes">Cancelar</a>
          </div>
        </div>
      </div>
    </main>
  `
})
export class FilmeForm implements OnInit {
  private service = inject(FilmeService);
  private generoService = inject(GeneroService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private avisos = inject(AvisosService);
  private location = inject(Location);

  classificacoes = CLASSIFICACOES;
  generos = signal<Genero[]>([]);

  id: number | null = null;
  filme: Filme = {
    titulo: '',
    sinopse: '',
    diretor: '',
    anoLancamento: new Date().getFullYear(),
    duracaoMinutos: 90,
    classificacaoIndicativa: 'L',
    precoDiaria: 0,
    quantidadeTotal: 1,
    generoId: 0
  };

  erros = signal<string[]>([]);
  salvando = signal(false);

  ngOnInit(): void {
    this.generoService.listar().subscribe({
      next: g => this.generos.set(g),
      error: (e: HttpErrorResponse) => this.erros.set(mensagensDeErro(e))
    });

    const idRota = this.route.snapshot.paramMap.get('id');
    if (idRota) {
      this.id = +idRota;
      const estado = this.location.getState() as { filme?: Filme };
      if (estado.filme && Number(estado.filme.id) === this.id) {
        this.filme = { ...estado.filme };
      } else {
        this.service.obter(this.id).subscribe({
          next: f => (this.filme = { ...f }),
          error: (e: HttpErrorResponse) => this.erros.set(mensagensDeErro(e))
        });
      }
    }
  }

  salvar(): void {
    this.erros.set([]);
    this.salvando.set(true);

    const requisicao = this.id
      ? this.service.atualizar(this.id, this.filme)
      : this.service.criar(this.filme);

    requisicao.subscribe({
      next: () => {
        this.avisos.sucesso('Filme salvo com sucesso!');
        this.router.navigate(['/filmes']);
      },
      error: (e: HttpErrorResponse) => {
        this.salvando.set(false);
        this.erros.set(mensagensDeErro(e));
      }
    });
  }
}
