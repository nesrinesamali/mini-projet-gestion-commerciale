import { Component, OnInit, inject, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NotificationService } from '../../core/notification.service';
import { ProductForm } from '../../models/models';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-product-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './product-form.html'
})
export class ProductFormPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly productService = inject(ProductService);
  private readonly notifications = inject(NotificationService);

  protected readonly productId = signal<number | null>(null);

  protected readonly form = this.fb.nonNullable.group({
    reference: ['', [Validators.required, Validators.maxLength(50)]],
    name: ['', [Validators.required, Validators.maxLength(150)]],
    description: ['', [Validators.maxLength(500)]],
    unitPriceHt: [0, [Validators.required, Validators.min(0.01)]],
    stockQuantity: [0, [Validators.required, Validators.min(0)]]
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      this.productId.set(Number(id));
      this.productService.getById(Number(id)).subscribe((product) => {
        this.form.patchValue({
          reference: product.reference,
          name: product.name,
          description: product.description ?? '',
          unitPriceHt: product.unitPriceHt,
          stockQuantity: product.stockQuantity
        });
      });
    }
  }

  protected invalid(control: AbstractControl | null): boolean {
    return !!control && control.invalid && control.touched;
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload = this.form.getRawValue() as ProductForm;
    const id = this.productId();

    const request = id
      ? this.productService.update(id, payload)
      : this.productService.create(payload);

    request.subscribe(() => {
      this.notifications.success(id ? 'Produit modifié.' : 'Produit créé.');
      this.router.navigate(['/products']);
    });
  }
}