import { Component, inject } from '@angular/core';
import { AvisosService } from '../../servicos/avisos';

@Component({
  selector: 'app-avisos',
  template: `
    <div class="toasts">
      @for (t of avisos.toasts(); track t.id) {
        <div class="toast" [class]="'toast ' + t.tipo" (click)="avisos.fechar(t.id)">
          {{ t.texto }}
        </div>
      }
    </div>

    @if (avisos.confirmacao(); as c) {
      <div class="fundo-modal" (click)="c.resolver(false)">
        <div class="modal" (click)="$event.stopPropagation()">
          <h3>{{ c.titulo }}</h3>
          <p>{{ c.texto }}</p>
          <div class="modal-botoes">
            <button class="btn btn-sec" (click)="c.resolver(false)">Cancelar</button>
            <button class="btn btn-primario dash-cta" (click)="c.resolver(true)">{{ c.textoBotao }}</button>
          </div>
        </div>
      </div>
    }
  `
})
export class Avisos {
  avisos = inject(AvisosService);
}
