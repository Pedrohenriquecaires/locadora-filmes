import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../servicos/auth';

export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.estaLogado()) {
    return true;
  }

  auth.logout();
  return router.createUrlTree(['/login']);
};
