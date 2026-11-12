import { Injectable, signal } from '@angular/core';

export interface Notification {
  type: 'success' | 'error';
  text: string;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  readonly message = signal<Notification | null>(null);
  private timer: ReturnType<typeof setTimeout> | undefined;

  success(text: string): void {
    this.show({ type: 'success', text });
  }

  error(text: string): void {
    this.show({ type: 'error', text });
  }

  private show(notification: Notification): void {
    this.message.set(notification);
    clearTimeout(this.timer);
    this.timer = setTimeout(() => this.message.set(null), 5000);
  }
}