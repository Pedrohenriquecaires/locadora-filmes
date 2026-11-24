import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FilmesLista } from './filmes-lista';

describe('FilmesLista', () => {
  let component: FilmesLista;
  let fixture: ComponentFixture<FilmesLista>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FilmesLista],
    }).compileComponents();

    fixture = TestBed.createComponent(FilmesLista);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
