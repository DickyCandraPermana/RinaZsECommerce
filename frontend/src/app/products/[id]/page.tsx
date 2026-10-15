"use client";

import { useEffect, useState, use } from "react";
import { Product } from "@/types";
import { api } from "@/lib/api";
import { formatRupiah } from "@/lib/format";
import { useCartStore } from "@/lib/cartStore";
import { useAuthStore } from "@/lib/authStore";
import Link from "next/link";
import { ArrowLeft, Check, Plus, Minus, ShoppingBag, ShieldCheck } from "lucide-react";

export default function ProductDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = use(params);
  const [product, setProduct] = useState<Product | null>(null);
  const [quantity, setQuantity] = useState(1);
  const [isLoading, setIsLoading] = useState(true);
  const [isAdding, setIsAdding] = useState(false);
  const [addedSuccess, setAddedSuccess] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const { addItem } = useCartStore();
  const { token } = useAuthStore();

  useEffect(() => {
    async function fetchProduct() {
      try {
        const data = await api.getProductById(id);
        setProduct(data);
      } catch (err) {
        setError((err as Error).message || "Produk tidak ditemukan.");
      } finally {
        setIsLoading(false);
      }
    }
    fetchProduct();
  }, [id]);

  const handleAddToCart = async () => {
    if (!token) {
      window.location.href = "/auth/login";
      return;
    }
    if (!product) return;

    setIsAdding(true);
    try {
      await addItem(product.id, quantity);
      setAddedSuccess(true);
      setTimeout(() => setAddedSuccess(false), 3000);
    } catch (err) {
      alert("Gagal menambahkan item: " + (err as Error).message);
    } finally {
      setIsAdding(false);
    }
  };

  if (isLoading) {
    return (
      <div className="py-24 text-center">
        <p className="font-mono text-xs text-ink-muted">Memuat rincian spesifikasi barang...</p>
      </div>
    );
  }

  if (error || !product) {
    return (
      <div className="py-16 text-center space-y-4">
        <p className="text-sm text-red-600 font-mono">{error || "Produk tidak ditemukan."}</p>
        <Link href="/" className="inline-flex items-center space-x-2 text-xs font-mono text-obsidian underline">
          <ArrowLeft className="w-3.5 h-3.5" />
          <span>Kembali ke Katalog</span>
        </Link>
      </div>
    );
  }

  const mainImage = product.images?.find((i) => i.isMain)?.imageUrl || product.images?.[0]?.imageUrl;

  return (
    <div className="space-y-8">
      {/* Back button */}
      <div>
        <Link
          href="/"
          className="inline-flex items-center space-x-1.5 text-xs font-mono text-ink-secondary hover:text-obsidian"
        >
          <ArrowLeft className="w-3.5 h-3.5" />
          <span>Kembali ke Katalog</span>
        </Link>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-12">
        {/* Product Imagery */}
        <div className="space-y-4">
          <div className="aspect-square bg-[#F4F4F2] border border-frame-subtle rounded-xs overflow-hidden flex items-center justify-center">
            {mainImage ? (
              /* eslint-disable-next-line @next/next/no-img-element */
              <img
                src={mainImage}
                alt={product.name}
                className="w-full h-full object-cover"
              />
            ) : (
              <span className="font-mono text-xs text-ink-muted">NO PREVIEW AVAILABLE</span>
            )}
          </div>
        </div>

        {/* Product Details & Purchase Form */}
        <div className="space-y-6">
          <div className="border-b border-frame-subtle pb-6 space-y-2">
            {product.category && (
              <span className="text-xs font-mono uppercase tracking-widest text-ink-muted">
                {product.category.name}
              </span>
            )}
            <h1 className="font-serif text-2xl sm:text-3xl font-bold text-obsidian">
              {product.name}
            </h1>
            <p className="font-mono text-xl font-bold text-obsidian pt-1">
              {formatRupiah(product.price)}
            </p>
          </div>

          {/* Description */}
          <div className="space-y-2">
            <h3 className="text-xs font-mono uppercase tracking-wider text-obsidian font-semibold">
              Karakter & Spesifikasi Material
            </h3>
            <p className="text-xs sm:text-sm text-ink-secondary leading-relaxed font-sans">
              {product.description}
            </p>
          </div>

          {/* Stock Status */}
          <div className="p-3 bg-white border border-frame-subtle rounded-xs flex items-center justify-between text-xs font-mono">
            <span className="text-ink-secondary">Ketersediaan Gudang:</span>
            {product.stock > 0 ? (
              <span className="text-green-700 font-bold">Tersedia ({product.stock} unit siap kirim)</span>
            ) : (
              <span className="text-red-600 font-bold">Stok Habis</span>
            )}
          </div>

          {/* Quantity and Add to Cart */}
          {product.stock > 0 && (
            <div className="space-y-4 pt-2">
              <div className="flex items-center space-x-4">
                <span className="text-xs font-mono text-ink-secondary">Jumlah:</span>
                <div className="flex items-center border border-frame-subtle rounded-xs bg-white">
                  <button
                    onClick={() => setQuantity((q) => Math.max(1, q - 1))}
                    disabled={quantity <= 1}
                    className="p-2 text-ink-muted hover:text-obsidian disabled:opacity-30"
                  >
                    <Minus className="w-3.5 h-3.5" />
                  </button>
                  <span className="px-4 font-mono text-xs font-bold">{quantity}</span>
                  <button
                    onClick={() => setQuantity((q) => Math.min(product.stock, q + 1))}
                    disabled={quantity >= product.stock}
                    className="p-2 text-ink-muted hover:text-obsidian disabled:opacity-30"
                  >
                    <Plus className="w-3.5 h-3.5" />
                  </button>
                </div>
                <span className="text-xs font-mono text-ink-muted">
                  Subtotal: {formatRupiah(product.price * quantity)}
                </span>
              </div>

              <div className="flex space-x-3">
                <button
                  onClick={handleAddToCart}
                  disabled={isAdding}
                  className="flex-1 py-3 px-6 bg-obsidian text-white text-xs font-mono uppercase tracking-wider rounded-xs hover:bg-terracotta disabled:opacity-50 transition-colors flex items-center justify-center space-x-2"
                >
                  {addedSuccess ? (
                    <>
                      <Check className="w-4 h-4 text-green-400" />
                      <span>Berhasil Ditambahkan!</span>
                    </>
                  ) : (
                    <>
                      <ShoppingBag className="w-4 h-4" />
                      <span>{isAdding ? "Menyimpan..." : "Masukkan Keranjang"}</span>
                    </>
                  )}
                </button>
              </div>
            </div>
          )}

          {/* Guarantee / Policy */}
          <div className="border-t border-frame-subtle pt-6 space-y-2 text-xs text-ink-muted font-mono">
            <div className="flex items-center space-x-2">
              <ShieldCheck className="w-4 h-4 text-ink-secondary" />
              <span>Verifikasi mutu fisik dan inspeksi dimensional sebelum packing.</span>
            </div>
            <p className="text-[11px] text-ink-muted pl-6">
              Pengiriman aman dengan kardus bergelombang ganda dan pelindung sudut tebal.
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
