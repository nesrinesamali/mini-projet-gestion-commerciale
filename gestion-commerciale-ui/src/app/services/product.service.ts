import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { API_URL } from '../core/api.config';
import { Product, ProductForm } from '../models/models';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly http = inject(HttpClient);
  private readonly url = `${API_URL}/products`;

  getAll() {
    return this.http.get<Product[]>(this.url);
  }

  getById(id: number) {
    return this.http.get<Product>(`${this.url}/${id}`);
  }

  create(product: ProductForm) {
    return this.http.post<Product>(this.url, product);
  }

  update(id: number, product: ProductForm) {
    return this.http.put<Product>(`${this.url}/${id}`, product);
  }

  delete(id: number) {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}