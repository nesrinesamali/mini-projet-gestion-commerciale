export interface Address {
  id?: number;
  street: string;
  city: string;
  postalCode: string;
  country: string;
}

export interface Client {
  id: number;
  lastName: string;
  firstNameOrCompany: string;
  email: string;
  phone: string;
  createdAt: string;
  addresses: Address[];
}

export interface ClientForm {
  lastName: string;
  firstNameOrCompany: string;
  email: string;
  phone: string;
  addresses: Address[];
}

export interface Product {
  id: number;
  reference: string;
  name: string;
  description: string | null;
  unitPriceHt: number;
  stockQuantity: number;
  createdAt: string;
}

export interface ProductForm {
  reference: string;
  name: string;
  description: string | null;
  unitPriceHt: number;
  stockQuantity: number;
}

export interface OrderLine {
  id: number;
  productId: number;
  productReference: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}

export interface Order {
  id: number;
  orderNumber: string;
  clientId: number;
  clientName: string;
  orderDate: string;
  status: 'Draft' | 'Validated' | 'Cancelled';
  totalHt: number;
  totalTtc: number;
  lines: OrderLine[];
}

export interface OrderLineForm {
  productId: number;
  quantity: number;
}

export interface OrderForm {
  clientId: number;
  lines: OrderLineForm[];
}