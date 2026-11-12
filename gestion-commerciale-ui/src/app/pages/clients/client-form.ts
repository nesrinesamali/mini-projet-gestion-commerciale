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
import { NotificationService } from '../../core/notification.service';
import { Address, ClientForm } from '../../models/models';
import { ClientService } from '../../services/client.service';

@Component({
  selector: 'app-client-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './client-form.html'
})
export class ClientFormPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly clientService = inject(ClientService);
  private readonly notifications = inject(NotificationService);

  protected readonly clientId = signal<number | null>(null);

  protected readonly addresses = new FormArray<FormGroup>([]);

  protected readonly form = this.fb.nonNullable.group({
    lastName: ['', [Validators.required, Validators.maxLength(100)]],
    firstNameOrCompany: ['', [Validators.required, Validators.maxLength(150)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(150)]],
    phone: ['', [Validators.required, Validators.pattern(/^[0-9+ ]{8,15}$/)]],
    addresses: this.addresses
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      this.clientId.set(Number(id));
      this.clientService.getById(Number(id)).subscribe((client) => {
        this.form.patchValue({
          lastName: client.lastName,
          firstNameOrCompany: client.firstNameOrCompany,
          email: client.email,
          phone: client.phone
        });
        client.addresses.forEach((address) => this.addresses.push(this.createAddress(address)));
      });
    } else {
      this.addAddress();
    }
  }

  protected addAddress(): void {
    this.addresses.push(this.createAddress());
  }

  protected removeAddress(index: number): void {
    this.addresses.removeAt(index);
  }

  protected invalid(control: AbstractControl | null): boolean {
    return !!control && control.invalid && control.touched;
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload = this.form.getRawValue() as ClientForm;
    const id = this.clientId();

    const request = id
      ? this.clientService.update(id, payload)
      : this.clientService.create(payload);

    request.subscribe(() => {
      this.notifications.success(id ? 'Client modifié.' : 'Client créé.');
      this.router.navigate(['/clients']);
    });
  }

  private createAddress(address?: Address): FormGroup {
    return this.fb.nonNullable.group({
      street: [address?.street ?? '', [Validators.required, Validators.maxLength(200)]],
      city: [address?.city ?? '', [Validators.required, Validators.maxLength(100)]],
      postalCode: [address?.postalCode ?? '', [Validators.maxLength(20)]],
      country: [address?.country ?? '', [Validators.required, Validators.maxLength(100)]]
    });
  }
}