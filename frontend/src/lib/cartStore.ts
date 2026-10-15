import { create } from "zustand";
import { CartItem } from "@/types";
import { api } from "./api";

interface CartStore {
  items: CartItem[];
  isLoading: boolean;
  isOpen: boolean;
  fetchCart: () => Promise<void>;
  addItem: (productId: string, quantity?: number) => Promise<void>;
  updateQuantity: (cartItemId: string, quantity: number) => Promise<void>;
  removeItem: (cartItemId: string) => Promise<void>;
  clearCart: () => Promise<void>;
  setOpen: (open: boolean) => void;
  getTotal: () => number;
  getItemCount: () => number;
}

export const useCartStore = create<CartStore>((set, get) => ({
  items: [],
  isLoading: false,
  isOpen: false,

  setOpen: (open: boolean) => set({ isOpen: open }),

  fetchCart: async () => {
    set({ isLoading: true });
    try {
      const items = await api.getCart();
      set({ items, isLoading: false });
    } catch {
      set({ items: [], isLoading: false });
    }
  },

  addItem: async (productId: string, quantity = 1) => {
    set({ isLoading: true });
    try {
      await api.addToCart(productId, quantity);
      const items = await api.getCart();
      set({ items, isLoading: false, isOpen: true });
    } catch (err) {
      set({ isLoading: false });
      throw err;
    }
  },

  updateQuantity: async (cartItemId: string, quantity: number) => {
    if (quantity <= 0) {
      await get().removeItem(cartItemId);
      return;
    }
    set({ isLoading: true });
    try {
      await api.updateCartItem(cartItemId, quantity);
      const items = await api.getCart();
      set({ items, isLoading: false });
    } catch (err) {
      set({ isLoading: false });
      throw err;
    }
  },

  removeItem: async (cartItemId: string) => {
    set({ isLoading: true });
    try {
      await api.removeCartItem(cartItemId);
      const items = await api.getCart();
      set({ items, isLoading: false });
    } catch (err) {
      set({ isLoading: false });
      throw err;
    }
  },

  clearCart: async () => {
    set({ isLoading: true });
    try {
      await api.clearCart();
      set({ items: [], isLoading: false });
    } catch (err) {
      set({ isLoading: false });
      throw err;
    }
  },

  getTotal: () => {
    return get().items.reduce((sum, item) => sum + (item.subtotal || item.price * item.quantity), 0);
  },

  getItemCount: () => {
    return get().items.reduce((sum, item) => sum + item.quantity, 0);
  },
}));
