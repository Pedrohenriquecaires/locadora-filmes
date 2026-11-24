import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './servicos/auth';
import { TemaService } from './servicos/tema';
import { Menu } from './componentes/menu/menu';
import { Avisos } from './componentes/avisos/avisos';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Menu, Avisos],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  auth = inject(AuthService);
  anoAtual = new Date().getFullYear();
  constructor() {
    inject(TemaService); // aplica o tema salvo assim que o app abre
  }
}
