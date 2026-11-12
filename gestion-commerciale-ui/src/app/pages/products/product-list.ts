import { CurrencyPipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NotificationService } from '../../core/notification.service';
import { Product } from '../../models/models';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-product-list',
  imports: [RouterLink, CurrencyPipe],
  templateUrl: './product-list.html'
})
export class ProductListPage implements OnInit {
  private readonly productService = inject(ProductService);
  private readonly notifications = inject(NotificationService);

  protected readonly products = signal<Product[]>([]);

  ngOnInit(): void {
    this.load();
  }

  protected delete(product: Product): void {
    if (!confirm(`Supprimer le produit ${product.name} ?`)) {
      return;
    }

    this.productService.delete(product.id).subscribe(() => {
      this.notifications.success('Produit supprimé.');
      this.load();
    });
  }

  private load(): void {
    this.productService.getAll().subscribe((data) => this.products.set(data));
  }
}