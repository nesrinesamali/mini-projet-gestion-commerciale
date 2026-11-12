import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { API_URL } from '../core/api.config';
import { Client, ClientForm } from '../models/models';

@Injectable({ providedIn: 'root' })
export class ClientService {
  private readonly http = inject(HttpClient);
  private readonly url = `${API_URL}/clients`;

  getAll() {
    return this.http.get<Client[]>(this.url);
  }

  getById(id: number) {
    return this.http.get<Client>(`${this.url}/${id}`);
  }

  create(client: ClientForm) {
    return this.http.post<Client>(this.url, client);
  }

  update(id: number, client: ClientForm) {
    return this.http.put<Client>(`${this.url}/${id}`, client);
  }

  delete(id: number) {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}