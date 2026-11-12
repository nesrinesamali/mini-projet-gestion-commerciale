import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NotificationService } from '../../core/notification.service';
import { ORDER_STATUS_LABELS } from '../../core/order-status';
import { Order } from '../../models/models';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-order-list',
  imports: [RouterLink, CurrencyPipe, DatePipe],
  templateUrl: './order-list.html'
})
export class OrderListPage implements OnInit {
  private readonly orderService = inject(OrderService);
  private readonly notifications = inject(NotificationService);

  protected readonly orders = signal<Order[]>([]);
  protected readonly statusLabels = ORDER_STATUS_LABELS;

  ngOnInit(): void {
    this.load();
  }

  protected validate(order: Order): void {
    if (!confirm(`Valider la commande ${order.orderNumber} ? Le stock sera mis à jour.`)) {
      return;
    }

    this.orderService.validate(order.id).subscribe(() => {
      this.notifications.success('Commande validée, stock mis à jour.');
      this.load();
    });
  }

  protected delete(order: Order): void {
    if (!confirm(`Supprimer la commande ${order.orderNumber} ?`)) {
      return;
    }

    this.orderService.delete(order.id).subscribe(() => {
      this.notifications.success('Commande supprimée.');
      this.load();
    });
  }

  private load(): void {
    this.orderService.getAll().subscribe((data) => this.orders.set(data));
  }
}