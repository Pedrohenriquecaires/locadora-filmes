import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LocacoesLista } from './locacoes-lista';

describe('LocacoesLista', () => {
  let component: LocacoesLista;
  let fixture: ComponentFixture<LocacoesLista>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LocacoesLista],
    }).compileComponents();

    fixture = TestBed.createComponent(LocacoesLista);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
