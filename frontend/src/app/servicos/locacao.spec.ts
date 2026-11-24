import { TestBed } from '@angular/core/testing';
import { LocacaoService } from './locacao';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('LocacaoService', () => {
  let service: LocacaoService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });

    service = TestBed.inject(LocacaoService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});