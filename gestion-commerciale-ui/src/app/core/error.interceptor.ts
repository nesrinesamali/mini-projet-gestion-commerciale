import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from './auth.service';
import { NotificationService } from './notification.service';

function extractMessage(error: HttpErrorResponse): string {
  if (error.status === 0) {
    return "Impossible de joindre l'API. Vérifiez que le back-end est démarré.";
  }

  const body = error.error;

  // Erreurs de validation ASP.NET : { errors: { champ: ["message"] } }
  if (body?.errors) {
    const first = Object.values(body.errors).flat()[0];
    if (typeof first === 'string') {
      return first;
    }
  }

  // Erreurs métier de notre middleware : { status, message }
  if (body?.message) {
    return body.message;
  }

  return 'Une erreur est survenue.';
}

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notifications = inject(NotificationService);
  const auth = inject(AuthService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const isLoginRequest = req.url.endsWith('/auth/login');

      if (error.status === 401 && !isLoginRequest) {
        auth.logout();
        notifications.error('Session expirée. Veuillez vous reconnecter.');
      } else {
        notifications.error(extractMessage(error));
      }

      return throwError(() => error);
    })
  );
};