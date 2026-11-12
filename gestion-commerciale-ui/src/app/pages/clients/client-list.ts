import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NotificationService } from '../../core/notification.service';
import { Client } from '../../models/models';
import { ClientService } from '../../services/client.service';

@Component({
  selector: 'app-client-list',
  imports: [RouterLink, DatePipe],
  templateUrl: './client-list.html'
})
export class ClientListPage implements OnInit {
  private readonly clientService = inject(ClientService);
  private readonly notifications = inject(NotificationService);

  protected readonly clients = signal<Client[]>([]);

  ngOnInit(): void {
    this.load();
  }

  protected delete(client: Client): void {
    const name = `${client.firstNameOrCompany} ${client.lastName}`;
    if (!confirm(`Supprimer le client ${name} ?`)) {
      return;
    }

    this.clientService.delete(client.id).subscribe(() => {
      this.notifications.success('Client supprimé.');
      this.load();
    });
  }

  private load(): void {
    this.clientService.getAll().subscribe((data) => this.clients.set(data));
  }
}