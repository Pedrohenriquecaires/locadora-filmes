import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../servicos/auth';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const token = auth.obterToken();

  if (token) {
    req = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
  }

  return next(req).pipe(
    catchError((e: HttpErrorResponse) => {
      // 401 fora do login = token ausente, inválido ou expirado -> volta para o login
      if (e.status === 401 && !req.url.includes('/auth/login')) {
        auth.logout();
        router.navigate(['/login']);
      }
      return throwError(() => e);
    })
  );
};
