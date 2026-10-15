"use client";

import Link from "next/link";
import { Product } from "@/types";
import { formatRupiah } from "@/lib/format";
import { useCartStore } from "@/lib/cartStore";
import { useAuthStore } from "@/lib/authStore";
import { Plus } from "lucide-react";
import { useState } from "react";

interface ProductCardProps {
  product: Product;
}

export function ProductCard({ product }: ProductCardProps) {
  const { addItem, isLoading } = useCartStore();
  const { token } = useAuthStore();
  const [adding, setAdding] = useState(false);

  const mainImage = product.images?.find((img) => img.isMain)?.imageUrl || product.images?.[0]?.imageUrl;

  const handleAddToCart = async (e: React.MouseEvent) => {
    e.preventDefault();
    if (!token) {
      window.location.href = "/auth/login";
      return;
    }
    setAdding(true);
    try {
      await addItem(product.id, 1);
    } catch (err) {
      alert("Gagal menambahkan ke keranjang: " + (err as Error).message);
    } finally {
      setAdding(false);
    }
  };

  return (
    <div className="group relative bg-white border border-frame-subtle rounded-xs flex flex-col justify-between transition-all duration-200 hover:border-frame-dark">
      <Link href={`/products/${product.id}`} className="block">
        {/* Image Frame */}
        <div className="relative aspect-square w-full bg-[#F4F4F2] overflow-hidden border-b border-frame-subtle flex items-center justify-center">
          {mainImage ? (
            /* eslint-disable-next-line @next/next/no-img-element */
            <img
              src={mainImage}
              alt={product.name}
              className="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105"
            />
          ) : (
            <span className="font-mono text-xs text-ink-muted">NO IMAGE</span>
          )}

          {/* Stock Indicator Badge */}
          <div className="absolute top-2.5 left-2.5">
            {product.stock > 0 ? (
              <span className="inline-block bg-white/95 backdrop-blur-xs border border-frame-subtle text-ink-primary font-mono text-[10px] px-2 py-0.5 rounded-xs">
                Sisa {product.stock}
              </span>
            ) : (
              <span className="inline-block bg-red-600 text-white font-mono text-[10px] px-2 py-0.5 rounded-xs font-semibold">
                Habis
              </span>
            )}
          </div>
        </div>

        {/* Content */}
        <div className="p-4 space-y-2">
          {product.category && (
            <span className="text-[11px] font-mono uppercase tracking-wider text-ink-muted block">
              {product.category.name}
            </span>
          )}

          <h3 className="font-sans font-medium text-sm text-obsidian line-clamp-1 group-hover:text-terracotta transition-colors">
            {product.name}
          </h3>

          <p className="text-xs text-ink-secondary line-clamp-2 leading-relaxed font-sans">
            {product.description}
          </p>
        </div>
      </Link>

      {/* Bottom Bar: Price & CTA */}
      <div className="p-4 pt-0 mt-auto flex items-center justify-between border-t border-frame-subtle/50">
        <span className="font-mono text-sm font-bold text-obsidian">
          {formatRupiah(product.price)}
        </span>

        <button
          onClick={handleAddToCart}
          disabled={adding || isLoading || product.stock <= 0}
          className="inline-flex items-center space-x-1 px-3 py-1.5 bg-obsidian text-white text-xs font-mono rounded-xs hover:bg-terracotta disabled:opacity-40 transition-colors"
          title="Tambah ke Keranjang"
        >
          <Plus className="w-3.5 h-3.5" />
          <span>{adding ? "..." : "Tambah"}</span>
        </button>
      </div>
    </div>
  );
}
