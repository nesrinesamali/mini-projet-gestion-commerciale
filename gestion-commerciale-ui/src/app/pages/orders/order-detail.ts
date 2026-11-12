import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { NotificationService } from '../../core/notification.service';
import { ORDER_STATUS_LABELS } from '../../core/order-status';
import { Order } from '../../models/models';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-order-detail',
  imports: [RouterLink, CurrencyPipe, DatePipe],
  templateUrl: './order-detail.html'
})
export class OrderDetailPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly orderService = inject(OrderService);
  private readonly notifications = inject(NotificationService);

  protected readonly order = signal<Order | null>(null);
  protected readonly statusLabels = ORDER_STATUS_LABELS;

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.orderService.getById(id).subscribe((order) => this.order.set(order));
  }

  protected validate(order: Order): void {
    if (!confirm(`Valider la commande ${order.orderNumber} ? Le stock sera mis à jour.`)) {
      return;
    }

    this.orderService.validate(order.id).subscribe((validated) => {
      this.order.set(validated);
      this.notifications.success('Commande validée, stock mis à jour.');
    });
  }
}