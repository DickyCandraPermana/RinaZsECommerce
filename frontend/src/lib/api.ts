import { Product, Category, CartItem, Order, UserProfile, OrderStatus } from "@/types";

const API_BASE = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5086/api";

function getToken(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem("rinazs_token");
}

async function request<T>(endpoint: string, options: RequestInit = {}): Promise<T> {
  const token = getToken();
  const headers: HeadersInit = {
    "Content-Type": "application/json",
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
    ...options.headers,
  };

  const response = await fetch(`${API_BASE}${endpoint}`, {
    ...options,
    headers,
  });

  if (!response.ok) {
    let errorDetail = "";
    try {
      const errJson = await response.json();
      errorDetail = errJson.message || errJson.title || JSON.stringify(errJson);
    } catch {
      errorDetail = await response.text();
    }
    throw new Error(errorDetail || `Request gagal dengan status ${response.status}`);
  }

  // Handle empty bodies (204 No Content or empty text)
  const text = await response.text();
  if (!text) return {} as T;
  return JSON.parse(text) as T;
}

export const api = {
  // Auth
  async login(email: string, password: string): Promise<{ token: string }> {
    return request<{ token: string }>("/auth/login", {
      method: "POST",
      body: JSON.stringify({ email, password }),
    });
  },

  async register(email: string, username: string, password: string): Promise<{ token: string }> {
    return request<{ token: string }>("/auth/register", {
      method: "POST",
      body: JSON.stringify({ email, username, password }),
    });
  },

  // Products & Categories
  async getProducts(params?: { categoryId?: string; search?: string }): Promise<Product[]> {
    const query = new URLSearchParams();
    if (params?.categoryId) query.append("categoryId", params.categoryId);
    if (params?.search) query.append("search", params.search);
    const queryString = query.toString() ? `?${query.toString()}` : "";
    return request<Product[]>(`/products${queryString}`);
  },

  async getProductById(id: string): Promise<Product> {
    return request<Product>(`/products/${id}`);
  },

  async getCategories(): Promise<Category[]> {
    return request<Category[]>("/categories");
  },

  // Profile
  async getProfile(): Promise<UserProfile> {
    return request<UserProfile>("/profile");
  },

  async updateProfile(data: { firstName: string; lastName: string; phone: string; address: string }): Promise<UserProfile> {
    return request<UserProfile>("/profile", {
      method: "PUT",
      body: JSON.stringify(data),
    });
  },

  // Cart
  async getCart(): Promise<CartItem[]> {
    return request<CartItem[]>("/carts");
  },

  async addToCart(productId: string, quantity: number = 1): Promise<{ cartItemId: string }> {
    return request<{ cartItemId: string }>("/carts/items", {
      method: "POST",
      body: JSON.stringify({ productId, quantity }),
    });
  },

  async updateCartItem(cartItemId: string, quantity: number): Promise<{ success: boolean }> {
    return request<{ success: boolean }>(`/carts/items/${cartItemId}`, {
      method: "PUT",
      body: JSON.stringify({ quantity }),
    });
  },

  async removeCartItem(cartItemId: string): Promise<{ success: boolean }> {
    return request<{ success: boolean }>(`/carts/items/${cartItemId}`, {
      method: "DELETE",
    });
  },

  async clearCart(): Promise<{ success: boolean }> {
    return request<{ success: boolean }>("/carts/clear", {
      method: "POST",
    });
  },

  // Orders
  async checkout(address: string, description?: string): Promise<{ orderId: string }> {
    return request<{ orderId: string }>("/orders/checkout", {
      method: "POST",
      body: JSON.stringify({ address, description }),
    });
  },

  async getOrders(): Promise<Order[]> {
    return request<Order[]>("/orders");
  },

  async getOrderById(id: string): Promise<Order> {
    return request<Order>(`/orders/${id}`);
  },

  async cancelOrder(id: string): Promise<{ success: boolean }> {
    return request<{ success: boolean }>(`/orders/${id}/cancel`, {
      method: "POST",
    });
  },

  // Admin
  async getAdminOrders(): Promise<Order[]> {
    return request<Order[]>("/admin/orders");
  },

  async updateOrderStatus(orderId: string, status: OrderStatus): Promise<{ message: string }> {
    return request<{ message: string }>(`/admin/orders/${orderId}/status`, {
      method: "PUT",
      body: JSON.stringify({ status }),
    });
  },

  async getAdminProducts(): Promise<Product[]> {
    return request<Product[]>("/admin/products");
  },

  async restockProduct(productId: string, amount: number, notes?: string): Promise<{ success: boolean }> {
    return request<{ success: boolean }>(`/admin/products/${productId}/restock`, {
      method: "POST",
      body: JSON.stringify({ amount, notes }),
    });
  },
};
