export interface ProductImage {
  id: string;
  imageUrl: string;
  isMain: boolean;
  displayOrder: number;
}

export interface Category {
  id: string;
  name: string;
  description?: string;
}

export interface Product {
  id: string;
  name: string;
  description: string;
  price: number;
  stock: number;
  categoryId: string;
  category?: Category;
  images?: ProductImage[];
  createdAt: string;
}

export interface CartItem {
  id: string;
  productId: string;
  productName: string;
  productImageUrl?: string;
  price: number;
  stock: number;
  quantity: number;
  subtotal: number;
}

export interface OrderDetail {
  id: string;
  productId: string;
  productName: string;
  productImageUrl?: string;
  amount: number;
  priceAtPurchase: number;
  subtotal: number;
}

export type OrderStatus = "Pending" | "Processing" | "Shipped" | "Delivered" | "Cancelled";

export interface Order {
  id: string;
  userId: string;
  customerName: string;
  customerEmail: string;
  address: string;
  description?: string;
  status: OrderStatus;
  totalAmount: number;
  createdAt: string;
  details: OrderDetail[];
}

export interface UserProfile {
  id: string;
  firstName: string;
  lastName: string;
  phone: string;
  address: string;
  email: string;
}

export interface AuthState {
  token: string | null;
  user: {
    id: string;
    name: string;
    email: string;
    role: "Admin" | "Customer";
    profileId: string;
  } | null;
}
