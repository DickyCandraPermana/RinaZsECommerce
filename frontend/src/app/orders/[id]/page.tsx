"use client";

import { useEffect, useState, use } from "react";
import { useRouter } from "next/navigation";
import { Order } from "@/types";
import { api } from "@/lib/api";
import { useAuthStore } from "@/lib/authStore";
import { formatRupiah, formatDate } from "@/lib/format";
import Link from "next/link";
import { ArrowLeft, Ban, CheckCircle2, Package, MapPin, Truck } from "lucide-react";

export default function OrderDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = use(params);
  const router = useRouter();
  const { token, initialize } = useAuthStore();
  const [order, setOrder] = useState<Order | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isCancelling, setIsCancelling] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    initialize();
  }, [initialize]);

  const loadOrder = async () => {
    setIsLoading(true);
    try {
      const data = await api.getOrderById(id);
      setOrder(data);
    } catch (err) {
      setError((err as Error).message || "Pesanan tidak ditemukan.");
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    if (!token && typeof window !== "undefined") {
      router.push("/auth/login");
      return;
    }
    if (token) {
      loadOrder();
    }
  }, [token, id, router]);

  const handleCancel = async () => {
    if (!confirm("Konfirmasi pembatalan pesanan ini? Stok akan dikembalikan otomatis.")) return;
    setIsCancelling(true);
    try {
      await api.cancelOrder(id);
      await loadOrder();
    } catch (err) {
      alert("Gagal membatalkan: " + (err as Error).message);
    } finally {
      setIsCancelling(false);
    }
  };

  if (!token) return null;

  if (isLoading) {
    return <div className="py-24 text-center font-mono text-xs text-ink-muted">Memuat rincian pesanan...</div>;
  }

  if (error || !order) {
    return (
      <div className="py-16 text-center space-y-4">
        <p className="text-sm font-mono text-red-600">{error || "Data pesanan tidak tersedia."}</p>
        <Link href="/orders" className="text-xs font-mono underline text-obsidian">
          Kembali ke Riwayat Pesanan
        </Link>
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto space-y-8">
      <div>
        <Link
          href="/orders"
          className="inline-flex items-center space-x-1.5 text-xs font-mono text-ink-secondary hover:text-obsidian"
        >
          <ArrowLeft className="w-3.5 h-3.5" />
          <span>Kembali ke Semua Pesanan</span>
        </Link>
        <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mt-2">
          <div>
            <h1 className="font-serif text-3xl font-bold text-obsidian">
              Pesanan #{order.id.substring(0, 8).toUpperCase()}
            </h1>
            <p className="text-xs font-mono text-ink-secondary mt-0.5">
              Dibuat pada {formatDate(order.createdAt)}
            </p>
          </div>
          <div className="flex items-center space-x-3">
            <span className="font-mono text-xs px-3 py-1 bg-white border border-frame-subtle rounded-xs font-bold text-obsidian">
              Status: {order.status}
            </span>
            {order.status === "Pending" && (
              <button
                onClick={handleCancel}
                disabled={isCancelling}
                className="inline-flex items-center space-x-1.5 px-3 py-1 bg-red-600 text-white text-xs font-mono rounded-xs hover:bg-red-700 disabled:opacity-50"
              >
                <Ban className="w-3.5 h-3.5" />
                <span>{isCancelling ? "Membatalkan..." : "Batalkan"}</span>
              </button>
            )}
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* Left Column: Items */}
        <div className="lg:col-span-2 space-y-6">
          <div className="bg-white border border-frame-subtle p-6 rounded-xs space-y-4">
            <h2 className="text-sm font-bold text-obsidian font-serif uppercase tracking-wider border-b border-frame-subtle pb-3">
              Daftar Barang ({order.details?.length || 0})
            </h2>

            <div className="space-y-4">
              {order.details?.map((detail) => (
                <div
                  key={detail.id}
                  className="p-4 bg-canvas border border-frame-subtle rounded-xs flex items-center justify-between"
                >
                  <div className="space-y-1">
                    <p className="font-medium text-sm text-obsidian">{detail.productName}</p>
                    <p className="font-mono text-xs text-ink-muted">
                      {detail.amount} unit × {formatRupiah(detail.priceAtPurchase)}
                    </p>
                  </div>
                  <span className="font-mono text-sm font-bold text-obsidian">
                    {formatRupiah(detail.subtotal)}
                  </span>
                </div>
              ))}
            </div>

            <div className="border-t border-frame-subtle pt-4 space-y-2 text-xs font-mono">
              <div className="flex justify-between text-ink-secondary">
                <span>Subtotal Barang</span>
                <span>{formatRupiah(order.totalAmount)}</span>
              </div>
              <div className="flex justify-between text-ink-secondary">
                <span>Ongkos Kirim</span>
                <span className="text-green-700 font-bold">Rp 0 (Gratis)</span>
              </div>
              <div className="flex justify-between text-sm font-bold text-obsidian pt-2 border-t border-frame-subtle">
                <span>Total Tagihan</span>
                <span className="text-base text-terracotta">{formatRupiah(order.totalAmount)}</span>
              </div>
            </div>
          </div>
        </div>

        {/* Right Column: Shipping & Info */}
        <div className="space-y-6">
          <div className="bg-white border border-frame-subtle p-6 rounded-xs space-y-4 text-xs font-mono">
            <div className="flex items-center space-x-2 border-b border-frame-subtle pb-3">
              <MapPin className="w-4 h-4 text-terracotta" />
              <h3 className="font-serif text-sm font-bold text-obsidian uppercase tracking-wider">
                Tujuan Pengiriman
              </h3>
            </div>
            <div className="space-y-1">
              <p className="font-bold text-obsidian">{order.customerName}</p>
              <p className="text-ink-secondary">{order.customerEmail}</p>
              <p className="text-ink-secondary mt-2 leading-relaxed">{order.address}</p>
              {order.description && (
                <p className="text-ink-muted italic pt-2">Catatan: &ldquo;{order.description}&rdquo;</p>
              )}
            </div>
          </div>

          <div className="p-4 bg-canvas border border-frame-subtle rounded-xs text-[11px] font-mono text-ink-muted space-y-2">
            <div className="flex items-center space-x-2 text-obsidian font-bold">
              <Truck className="w-4 h-4" />
              <span>Gudang Distribusi Surakarta</span>
            </div>
            <p>
              Pesanan dipacking dengan standar industri dan disegel tamper-evident.
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
