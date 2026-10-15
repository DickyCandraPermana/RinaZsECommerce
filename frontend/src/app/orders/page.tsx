"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { Order } from "@/types";
import { api } from "@/lib/api";
import { useAuthStore } from "@/lib/authStore";
import { formatRupiah, formatDate } from "@/lib/format";
import Link from "next/link";
import { ArrowLeft, Clock, PackageCheck, Ban, ArrowUpRight } from "lucide-react";

export default function OrdersHistoryPage() {
  const router = useRouter();
  const { token, initialize } = useAuthStore();
  const [orders, setOrders] = useState<Order[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [cancellingId, setCancellingId] = useState<string | null>(null);

  useEffect(() => {
    initialize();
  }, [initialize]);

  const loadOrders = async () => {
    setIsLoading(true);
    try {
      const data = await api.getOrders();
      setOrders(data);
    } catch {
      setOrders([]);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    if (!token && typeof window !== "undefined") {
      router.push("/auth/login?redirect=/orders");
      return;
    }
    if (token) {
      loadOrders();
    }
  }, [token, router]);

  const handleCancel = async (orderId: string) => {
    if (!confirm("Apakah Anda yakin ingin membatalkan pesanan ini? Stok barang akan dikembalikan ke gudang secara otomatis.")) {
      return;
    }
    setCancellingId(orderId);
    try {
      await api.cancelOrder(orderId);
      await loadOrders();
    } catch (err) {
      alert("Gagal membatalkan pesanan: " + (err as Error).message);
    } finally {
      setCancellingId(null);
    }
  };

  const getStatusBadge = (status: string) => {
    switch (status) {
      case "Pending":
        return <span className="px-2 py-0.5 bg-amber-50 text-amber-800 border border-amber-200 text-[10px] font-mono rounded-xs">Menunggu Proses</span>;
      case "Processing":
        return <span className="px-2 py-0.5 bg-blue-50 text-blue-800 border border-blue-200 text-[10px] font-mono rounded-xs">Sedang Diproses</span>;
      case "Shipped":
        return <span className="px-2 py-0.5 bg-purple-50 text-purple-800 border border-purple-200 text-[10px] font-mono rounded-xs">Dalam Pengiriman</span>;
      case "Delivered":
        return <span className="px-2 py-0.5 bg-green-50 text-green-800 border border-green-200 text-[10px] font-mono rounded-xs">Telah Diterima</span>;
      case "Cancelled":
        return <span className="px-2 py-0.5 bg-red-50 text-red-800 border border-red-200 text-[10px] font-mono rounded-xs">Dibatalkan</span>;
      default:
        return <span className="px-2 py-0.5 bg-gray-50 text-gray-800 border border-gray-200 text-[10px] font-mono rounded-xs">{status}</span>;
    }
  };

  if (!token) return null;

  return (
    <div className="max-w-5xl mx-auto space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 border-b border-frame-subtle pb-6">
        <div>
          <Link
            href="/"
            className="inline-flex items-center space-x-1.5 text-xs font-mono text-ink-secondary hover:text-obsidian"
          >
            <ArrowLeft className="w-3.5 h-3.5" />
            <span>Katalog</span>
          </Link>
          <h1 className="font-serif text-3xl font-bold text-obsidian mt-1">
            Riwayat Pesanan Saya
          </h1>
          <p className="text-xs font-mono text-ink-secondary">
            Catatan transaksi pembelian dan status logistik pengiriman.
          </p>
        </div>
      </div>

      {isLoading ? (
        <div className="py-20 text-center font-mono text-xs text-ink-muted">
          Memuat daftar pesanan...
        </div>
      ) : orders.length === 0 ? (
        <div className="p-12 text-center bg-white border border-frame-subtle rounded-xs space-y-3">
          <Clock className="w-8 h-8 mx-auto text-ink-muted" />
          <h3 className="font-serif text-base font-bold text-obsidian">Belum Ada Pesanan</h3>
          <p className="text-xs text-ink-secondary max-w-sm mx-auto">
            Anda belum pernah melakukan pemesanan di studio ini. Silakan kunjungi katalog untuk melihat ketersediaan barang.
          </p>
          <Link
            href="/"
            className="inline-block px-4 py-2 bg-obsidian text-white text-xs font-mono uppercase tracking-wider rounded-xs hover:bg-terracotta"
          >
            Buka Katalog Produk
          </Link>
        </div>
      ) : (
        <div className="space-y-4">
          {orders.map((order) => (
            <div
              key={order.id}
              className="bg-white border border-frame-subtle rounded-xs p-6 space-y-4 hover:border-frame-dark transition-colors"
            >
              {/* Order Card Header */}
              <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-2 border-b border-frame-subtle pb-4">
                <div className="space-y-1">
                  <div className="flex items-center space-x-2">
                    <span className="font-mono text-xs font-bold text-obsidian">
                      #{order.id.substring(0, 8).toUpperCase()}
                    </span>
                    {getStatusBadge(order.status)}
                  </div>
                  <p className="text-[11px] font-mono text-ink-muted">
                    Waktu Pesan: {formatDate(order.createdAt)}
                  </p>
                </div>

                <div className="flex items-center space-x-3">
                  <span className="font-mono text-base font-bold text-obsidian">
                    {formatRupiah(order.totalAmount)}
                  </span>
                  <Link
                    href={`/orders/${order.id}`}
                    className="p-1.5 text-ink-muted hover:text-obsidian border border-frame-subtle rounded-xs"
                    title="Lihat Detail"
                  >
                    <ArrowUpRight className="w-4 h-4" />
                  </Link>
                </div>
              </div>

              {/* Order Items Preview */}
              <div className="space-y-2">
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-2">
                  {order.details?.map((detail) => (
                    <div
                      key={detail.id}
                      className="p-2.5 bg-canvas border border-frame-subtle rounded-xs flex items-center justify-between text-xs"
                    >
                      <div className="truncate pr-2">
                        <p className="font-medium text-obsidian truncate">{detail.productName}</p>
                        <p className="font-mono text-[11px] text-ink-muted">
                          {detail.amount} unit × {formatRupiah(detail.priceAtPurchase)}
                        </p>
                      </div>
                      <span className="font-mono font-semibold text-obsidian">
                        {formatRupiah(detail.subtotal)}
                      </span>
                    </div>
                  ))}
                </div>
              </div>

              {/* Footer actions */}
              <div className="flex flex-col sm:flex-row sm:items-center justify-between pt-2 border-t border-frame-subtle text-xs gap-2">
                <p className="text-ink-muted text-[11px] font-mono truncate max-w-md">
                  Alamat Tujuan: {order.address}
                </p>

                {order.status === "Pending" && (
                  <button
                    onClick={() => handleCancel(order.id)}
                    disabled={cancellingId === order.id}
                    className="inline-flex items-center space-x-1.5 text-red-600 hover:text-red-800 text-xs font-mono font-medium disabled:opacity-50"
                  >
                    <Ban className="w-3.5 h-3.5" />
                    <span>{cancellingId === order.id ? "Membatalkan..." : "Batalkan Pesanan"}</span>
                  </button>
                )}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
