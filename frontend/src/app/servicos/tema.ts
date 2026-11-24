import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class TemaService {
  escuro = signal(localStorage.getItem('tema') !== 'claro');

  constructor() {
    this.aplicar();
  }

  alternar(): void {
    this.escuro.update(v => !v);
    this.aplicar();
  }

  private aplicar(): void {
    const tema = this.escuro() ? 'escuro' : 'claro';
    const raiz = document.documentElement;
    raiz.setAttribute('data-tema', tema);
    raiz.classList.toggle('dark', this.escuro()); // o Spartan/Tailwind usa a classe .dark
    localStorage.setItem('tema', tema);
  }
}
