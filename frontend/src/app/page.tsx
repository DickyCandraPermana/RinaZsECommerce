"use client";

import { useEffect, useState, useMemo } from "react";
import { Product, Category } from "@/types";
import { api } from "@/lib/api";
import { ProductCard } from "@/components/ProductCard";
import { Search, SlidersHorizontal, RefreshCw } from "lucide-react";

export default function CatalogPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [selectedCategory, setSelectedCategory] = useState<string>("all");
  const [searchQuery, setSearchQuery] = useState<string>("");
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const loadData = async () => {
    setIsLoading(true);
    setError(null);
    try {
      const [prods, cats] = await Promise.all([
        api.getProducts(),
        api.getCategories(),
      ]);
      setProducts(prods);
      setCategories(cats);
    } catch (err) {
      setError((err as Error).message || "Gagal memuat katalog produk.");
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const filteredProducts = useMemo(() => {
    return products.filter((p) => {
      const matchCategory =
        selectedCategory === "all" || p.categoryId === selectedCategory;
      const matchSearch =
        searchQuery === "" ||
        p.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
        p.description.toLowerCase().includes(searchQuery.toLowerCase());
      return matchCategory && matchSearch;
    });
  }, [products, selectedCategory, searchQuery]);

  return (
    <div className="space-y-8">
      {/* Editorial Hero Statement (Anti-AI Slop: Factual, concise, grounded) */}
      <section className="border-b border-frame-subtle pb-8 pt-2">
        <div className="max-w-3xl space-y-3">
          <span className="text-xs font-mono uppercase tracking-widest text-terracotta">
            Katalog Fisik 2026 / Batch 01
          </span>
          <h1 className="font-serif text-3xl sm:text-4xl font-bold tracking-tight text-obsidian">
            Barang fungsional dengan spesifikasi material terbuka.
          </h1>
          <p className="text-sm text-ink-secondary leading-relaxed font-sans">
            Setiap item dirancang untuk keandalan harian: katun combed bergramasi tebal, pelat kuningan, sakelar mekanis berumur 50 juta siklus, dan semikonduktor pengisian daya efisiensi tinggi.
          </p>
        </div>
      </section>

      {/* Filter and Search Controls */}
      <section className="space-y-4">
        <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
          {/* Categories Pill Navigation */}
          <div className="flex flex-wrap gap-2">
            <button
              onClick={() => setSelectedCategory("all")}
              className={`px-3 py-1.5 text-xs font-mono rounded-xs transition-colors border ${
                selectedCategory === "all"
                  ? "bg-obsidian text-white border-obsidian"
                  : "bg-white text-ink-secondary border-frame-subtle hover:border-frame-dark"
              }`}
            >
              Semua ({products.length})
            </button>
            {categories.map((cat) => (
              <button
                key={cat.id}
                onClick={() => setSelectedCategory(cat.id)}
                className={`px-3 py-1.5 text-xs font-mono rounded-xs transition-colors border ${
                  selectedCategory === cat.id
                    ? "bg-obsidian text-white border-obsidian"
                    : "bg-white text-ink-secondary border-frame-subtle hover:border-frame-dark"
                }`}
              >
                {cat.name}
              </button>
            ))}
          </div>

          {/* Search Box */}
          <div className="relative w-full sm:w-64">
            <Search className="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-ink-muted" />
            <input
              type="text"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              placeholder="Cari item atau material..."
              className="w-full pl-9 pr-3 py-1.5 bg-white border border-frame-subtle rounded-xs text-xs font-sans text-obsidian placeholder:text-ink-muted focus:outline-hidden focus:border-obsidian"
            />
          </div>
        </div>
      </section>

      {/* Product Grid */}
      <section>
        {isLoading ? (
          <div className="py-20 flex flex-col items-center justify-center space-y-3">
            <RefreshCw className="w-6 h-6 animate-spin text-ink-muted" />
            <p className="text-xs font-mono text-ink-muted">Memuat data dari PostgreSQL...</p>
          </div>
        ) : error ? (
          <div className="p-6 bg-red-50 border border-red-200 rounded-xs text-center space-y-2">
            <p className="text-xs text-red-700 font-mono">{error}</p>
            <button
              onClick={loadData}
              className="text-xs font-mono underline text-red-900 hover:text-red-700"
            >
              Coba lagi
            </button>
          </div>
        ) : filteredProducts.length === 0 ? (
          <div className="py-16 text-center border border-dashed border-frame-subtle rounded-xs p-8 space-y-2">
            <SlidersHorizontal className="w-6 h-6 mx-auto text-ink-muted" />
            <p className="text-sm text-ink-secondary">Tidak ada produk yang cocok dengan kriteria pencarian.</p>
            <button
              onClick={() => {
                setSelectedCategory("all");
                setSearchQuery("");
              }}
              className="text-xs font-mono text-terracotta underline"
            >
              Reset filter
            </button>
          </div>
        ) : (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
            {filteredProducts.map((product) => (
              <ProductCard key={product.id} product={product} />
            ))}
          </div>
        )}
      </section>
    </div>
  );
}
