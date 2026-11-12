import { CurrencyPipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { NotificationService } from '../../core/notification.service';
import { Client, OrderForm, Product } from '../../models/models';
import { ClientService } from '../../services/client.service';
import { OrderService } from '../../services/order.service';
import { ProductService } from '../../services/product.service';

const VAT_RATE = 0.19;

@Component({
  selector: 'app-order-form',
  imports: [ReactiveFormsModule, RouterLink, CurrencyPipe],
  templateUrl: './order-form.html'
})
export class OrderFormPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly clientService = inject(ClientService);
  private readonly productService = inject(ProductService);
  private readonly orderService = inject(OrderService);
  private readonly notifications = inject(NotificationService);

  protected readonly clients = signal<Client[]>([]);
  protected readonly products = signal<Product[]>([]);
  protected readonly orderId = signal<number | null>(null);

  protected readonly lines = new FormArray<FormGroup>([], [Validators.minLength(1)]);

  protected readonly form = this.fb.nonNullable.group({
    clientId: [0, [Validators.min(1)]],
    lines: this.lines
  });

  ngOnInit(): void {
    forkJoin({
      clients: this.clientService.getAll(),
      products: this.productService.getAll()
    }).subscribe(({ clients, products }) => {
      this.clients.set(clients);
      this.products.set(products);
      this.initForm();
    });
  }

  protected addLine(): void {
    this.lines.push(this.createLine());
  }

  protected removeLine(index: number): void {
    this.lines.removeAt(index);
  }

  protected invalid(control: AbstractControl | null): boolean {
    return !!control && control.invalid && control.touched;
  }

  protected unitPrice(index: number): number {
    return this.findProduct(index)?.unitPriceHt ?? 0;
  }

  protected stockOf(index: number): number {
    return this.findProduct(index)?.stockQuantity ?? 0;
  }

  protected lineTotal(index: number): number {
    return this.unitPrice(index) * (this.lines.at(index).value.quantity ?? 0);
  }

  protected exceedsStock(index: number): boolean {
    const product = this.findProduct(index);
    return !!product && this.lines.at(index).value.quantity > product.stockQuantity;
  }

  protected totalHt(): number {
    return this.lines.controls.reduce((sum, _, index) => sum + this.lineTotal(index), 0);
  }

  protected totalTtc(): number {
    return Math.round(this.totalHt() * (1 + VAT_RATE) * 100) / 100;
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notifications.error('Choisissez un client et ajoutez au moins une ligne valide.');
      return;
    }

    const hasStockProblem = this.lines.controls.some((_, index) => this.exceedsStock(index));
    if (hasStockProblem) {
      this.notifications.error('Une quantité dépasse le stock disponible.');
      return;
    }

    const payload: OrderForm = {
      clientId: this.form.controls.clientId.value,
      lines: this.lines.controls.map((line) => ({
        productId: line.value.productId,
        quantity: line.value.quantity
      }))
    };

    const id = this.orderId();
    const request = id
      ? this.orderService.update(id, payload)
      : this.orderService.create(payload);

    request.subscribe(() => {
      this.notifications.success(id ? 'Commande modifiée.' : 'Commande créée.');
      this.router.navigate(['/orders']);
    });
  }

  private initForm(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      this.addLine();
      return;
    }

    this.orderId.set(Number(id));
    this.orderService.getById(Number(id)).subscribe((order) => {
      if (order.status !== 'Draft') {
        this.notifications.error('Seule une commande en brouillon peut être modifiée.');
        this.router.navigate(['/orders']);
        return;
      }

      this.form.controls.clientId.setValue(order.clientId);
      order.lines.forEach((line) =>
        this.lines.push(this.createLine(line.productId, line.quantity))
      );
    });
  }

  private createLine(productId = 0, quantity = 1): FormGroup {
    return this.fb.nonNullable.group({
      productId: [productId, [Validators.min(1)]],
      quantity: [quantity, [Validators.required, Validators.min(1), Validators.pattern(/^\d+$/)]]
    });
  }

  private findProduct(index: number): Product | undefined {
    const productId = this.lines.at(index).value.productId;
    return this.products().find((product) => product.id === productId);
  }
}