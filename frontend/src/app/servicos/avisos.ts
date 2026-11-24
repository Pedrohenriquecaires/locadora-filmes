import { Injectable, signal } from '@angular/core';

export interface Toast { id: number; tipo: 'ok' | 'erro' | 'info'; texto: string; }
export interface Confirmacao {
  titulo: string; texto: string; textoBotao: string; resolver: (v: boolean) => void;
}

@Injectable({ providedIn: 'root' })
export class AvisosService {
  toasts = signal<Toast[]>([]);
  confirmacao = signal<Confirmacao | null>(null);
  private seq = 0;

  private mostrar(tipo: Toast['tipo'], texto: string): void {
    const id = ++this.seq;
    this.toasts.update(t => [...t, { id, tipo, texto }]);
    setTimeout(() => this.fechar(id), 4500);
  }
  sucesso(texto: string) { this.mostrar('ok', texto); }
  erro(texto: string) { this.mostrar('erro', texto); }
  info(texto: string) { this.mostrar('info', texto); }

  fechar(id: number): void {
    this.toasts.update(t => t.filter(x => x.id !== id));
  }

  confirmar(titulo: string, texto: string, textoBotao = 'Confirmar'): Promise<boolean> {
    return new Promise(resolve =>
      this.confirmacao.set({
        titulo, texto, textoBotao,
        resolver: v => { this.confirmacao.set(null); resolve(v); }
      })
    );
  }
}
