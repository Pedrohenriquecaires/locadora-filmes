import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin } from 'rxjs';
import { environment } from '../../environments/environment';
import { Locacao, LocacaoService } from './locacao';

export interface Dashboard {
  totalClientes: number;
  totalFilmes: number;
  locacoesAtivas: number;
  locacoesAtrasadas: number;
  receitaTotal: number;
  topFilmes: { titulo: string; quantidade: number }[];
  rankingClientes: { clienteId: number; nome: string; quantidade: number }[];
  alugueisPorMes: { mes: string; quantidade: number }[];
}

export interface DashboardCompleto {
  resumo: Dashboard;
  locacoes: Locacao[];
}

/** O relatório é calculado pela API (GET /api/relatorios/dashboard). */
@Injectable({ providedIn: 'root' })
export class DashboardService {
  private http = inject(HttpClient);
  private locacoes = inject(LocacaoService);

  obter(): Observable<DashboardCompleto> {
    return forkJoin({
      resumo: this.http.get<Dashboard>(`${environment.apiUrl}/relatorios/dashboard`),
      locacoes: this.locacoes.listar() // usada na lista "Últimas locações"
    });
  }
}