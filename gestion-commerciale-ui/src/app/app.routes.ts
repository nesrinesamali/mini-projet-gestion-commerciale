import { Routes } from '@angular/router';
import { authGuard } from './core/auth.guard';
import { ClientFormPage } from './pages/clients/client-form';
import { ClientListPage } from './pages/clients/client-list';
import { LoginPage } from './pages/login/login';
import { OrderDetailPage } from './pages/orders/order-detail';
import { OrderFormPage } from './pages/orders/order-form';
import { OrderListPage } from './pages/orders/order-list';
import { ProductFormPage } from './pages/products/product-form';
import { ProductListPage } from './pages/products/product-list';

export const routes: Routes = [
  { path: 'login', component: LoginPage },
  {
    path: '',
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'clients', pathMatch: 'full' },
      { path: 'clients', component: ClientListPage },
      { path: 'clients/new', component: ClientFormPage },
      { path: 'clients/:id/edit', component: ClientFormPage },
      { path: 'products', component: ProductListPage },
      { path: 'products/new', component: ProductFormPage },
      { path: 'products/:id/edit', component: ProductFormPage },
      { path: 'orders', component: OrderListPage },
      { path: 'orders/new', component: OrderFormPage },
      { path: 'orders/:id/edit', component: OrderFormPage },
      { path: 'orders/:id', component: OrderDetailPage }
    ]
  },
  { path: '**', redirectTo: '' }
];