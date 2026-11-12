import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { API_URL } from '../core/api.config';
import { Order, OrderForm } from '../models/models';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private readonly http = inject(HttpClient);
  private readonly url = `${API_URL}/orders`;

  getAll() {
    return this.http.get<Order[]>(this.url);
  }

  getById(id: number) {
    return this.http.get<Order>(`${this.url}/${id}`);
  }

  create(order: OrderForm) {
    return this.http.post<Order>(this.url, order);
  }

  update(id: number, order: OrderForm) {
    return this.http.put<Order>(`${this.url}/${id}`, order);
  }

  delete(id: number) {
    return this.http.delete<void>(`${this.url}/${id}`);
  }

  validate(id: number) {
    return this.http.post<Order>(`${this.url}/${id}/validate`, {});
  }
}