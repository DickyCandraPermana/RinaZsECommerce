"use client";

import Link from "next/link";
import { useCartStore } from "@/lib/cartStore";
import { formatRupiah } from "@/lib/format";
import { X, Plus, Minus, Trash2, ArrowRight } from "lucide-react";

export function CartDrawer() {
  const { items, isOpen, setOpen, updateQuantity, removeItem, clearCart, getTotal, isLoading } = useCartStore();

  if (!isOpen) return null;

  const total = getTotal();

  return (
    <div className="fixed inset-0 z-50 overflow-hidden">
      {/* Backdrop */}
      <div
        className="absolute inset-0 bg-black/40 backdrop-blur-xs transition-opacity"
        onClick={() => setOpen(false)}
      />

      <div className="fixed inset-y-0 right-0 max-w-full flex pl-10">
        <div className="w-screen max-w-md bg-canvas border-l border-frame-subtle flex flex-col shadow-2xl">
          {/* Header */}
          <div className="px-6 py-5 border-b border-frame-subtle flex items-center justify-between">
            <div className="flex items-baseline space-x-2">
              <h2 className="font-serif text-lg font-bold text-obsidian">Keranjang Belanja</h2>
              <span className="text-xs font-mono text-ink-muted">({items.length} jenis)</span>
            </div>
            <button
              onClick={() => setOpen(false)}
              className="p-1 text-ink-muted hover:text-obsidian transition-colors"
            >
              <X className="w-5 h-5" />
            </button>
          </div>

          {/* Body */}
          <div className="flex-1 overflow-y-auto px-6 py-4 space-y-4">
            {items.length === 0 ? (
              <div className="text-center py-16 space-y-3">
                <p className="text-sm text-ink-secondary">Keranjang belanja Anda masih kosong.</p>
                <button
                  onClick={() => setOpen(false)}
                  className="text-xs font-mono uppercase tracking-wider text-terracotta hover:underline"
                >
                  Jelajahi Produk
                </button>
              </div>
            ) : (
              items.map((item) => (
                <div
                  key={item.id}
                  className="p-4 bg-white border border-frame-subtle rounded-sm flex space-x-4 items-center"
                >
                  <div className="w-16 h-16 bg-[#F4F4F2] border border-frame-subtle flex-shrink-0 flex items-center justify-center overflow-hidden rounded-xs">
                    {item.productImageUrl ? (
                      /* eslint-disable-next-line @next/next/no-img-element */
                      <img
                        src={item.productImageUrl}
                        alt={item.productName}
                        className="w-full h-full object-cover"
                      />
                    ) : (
                      <span className="text-[10px] font-mono text-ink-muted">NO IMG</span>
                    )}
                  </div>

                  <div className="flex-1 min-w-0">
                    <h4 className="text-sm font-medium text-obsidian truncate">{item.productName}</h4>
                    <p className="text-xs font-mono text-ink-primary font-semibold mt-0.5">
                      {formatRupiah(item.price)}
                    </p>

                    <div className="flex items-center space-x-2 mt-2">
                      <div className="flex items-center border border-frame-subtle rounded-xs bg-canvas">
                        <button
                          onClick={() => updateQuantity(item.id, item.quantity - 1)}
                          disabled={isLoading}
                          className="p-1 text-ink-muted hover:text-obsidian disabled:opacity-50"
                        >
                          <Minus className="w-3 h-3" />
                        </button>
                        <span className="px-2 font-mono text-xs font-medium">{item.quantity}</span>
                        <button
                          onClick={() => updateQuantity(item.id, item.quantity + 1)}
                          disabled={isLoading || item.quantity >= item.stock}
                          className="p-1 text-ink-muted hover:text-obsidian disabled:opacity-50"
                        >
                          <Plus className="w-3 h-3" />
                        </button>
                      </div>

                      <button
                        onClick={() => removeItem(item.id)}
                        disabled={isLoading}
                        className="p-1 text-ink-muted hover:text-red-600 transition-colors"
                        title="Hapus"
                      >
                        <Trash2 className="w-3.5 h-3.5" />
                      </button>
                    </div>
                  </div>

                  <div className="text-right">
                    <p className="text-xs font-mono font-bold text-obsidian">
                      {formatRupiah(item.subtotal || item.price * item.quantity)}
                    </p>
                  </div>
                </div>
              ))
            )}
          </div>

          {/* Footer */}
          {items.length > 0 && (
            <div className="p-6 border-t border-frame-subtle bg-white space-y-4">
              <div className="space-y-1.5">
                <div className="flex justify-between text-xs text-ink-secondary">
                  <span>Subtotal Barang</span>
                  <span className="font-mono">{formatRupiah(total)}</span>
                </div>
                <div className="flex justify-between text-xs text-ink-secondary">
                  <span>Biaya Pengiriman</span>
                  <span className="font-mono text-green-700">Dihitung saat checkout</span>
                </div>
                <div className="flex justify-between text-sm font-bold text-obsidian pt-2 border-t border-frame-subtle">
                  <span>Total Estimasi</span>
                  <span className="font-mono text-base">{formatRupiah(total)}</span>
                </div>
              </div>

              <div className="space-y-2">
                <Link
                  href="/checkout"
                  onClick={() => setOpen(false)}
                  className="w-full inline-flex items-center justify-center space-x-2 py-3 bg-obsidian text-white text-xs font-mono uppercase tracking-wider rounded-xs hover:bg-terracotta transition-colors"
                >
                  <span>Lanjut ke Formulir Checkout</span>
                  <ArrowRight className="w-4 h-4" />
                </Link>
                <button
                  onClick={() => clearCart()}
                  disabled={isLoading}
                  className="w-full text-center text-[11px] font-mono text-ink-muted hover:text-red-600 py-1"
                >
                  Kosongkan Seluruh Keranjang
                </button>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
