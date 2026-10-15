"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { Order, Product, OrderStatus } from "@/types";
import { api } from "@/lib/api";
import { useAuthStore } from "@/lib/authStore";
import { formatRupiah, formatDate } from "@/lib/format";
import Link from "next/link";
import { ArrowLeft, Shield, Package, RefreshCw, AlertCircle } from "lucide-react";

export default function AdminPage() {
  const router = useRouter();
  const { user, token, initialize } = useAuthStore();
  const [orders, setOrders] = useState<Order[]>([]);
  const [products, setProducts] = useState<Product[]>([]);
  const [activeTab, setActiveTab] = useState<"orders" | "inventory">("orders");
  const [isLoading, setIsLoading] = useState(true);
  const [updatingOrderId, setUpdatingOrderId] = useState<string | null>(null);
  const [restockProductId, setRestockProductId] = useState<string | null>(null);
  const [restockAmount, setRestockAmount] = useState<number>(10);
  const [restockNotes, setRestockNotes] = useState<string>("Penerimaan batch baru dari supplier");
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  useEffect(() => {
    initialize();
  }, [initialize]);

  const loadData = async () => {
    setIsLoading(true);
    setError(null);
    try {
      const [orderList, productList] = await Promise.all([
        api.getAdminOrders(),
        api.getProducts(),
      ]);
      setOrders(orderList);
      setProducts(productList);
    } catch (err) {
      setError((err as Error).message || "Gagal memuat data admin.");
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    if (!token && typeof window !== "undefined") {
      router.push("/auth/login?redirect=/admin");
      return;
    }
    if (user && user.role !== "Admin") {
      router.push("/");
      return;
    }
    if (token) {
      loadData();
    }
  }, [token, user, router]);

  const handleStatusChange = async (orderId: string, newStatus: OrderStatus) => {
    setUpdatingOrderId(orderId);
    setError(null);
    setSuccess(null);
    try {
      await api.updateOrderStatus(orderId, newStatus);
      setSuccess(`Status pesanan #${orderId.substring(0, 8)} berhasil diubah menjadi ${newStatus}.`);
      await loadData();
    } catch (err) {
      setError((err as Error).message || "Gagal memperbarui status pesanan.");
    } finally {
      setUpdatingOrderId(null);
    }
  };

  const handleRestock = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!restockProductId) return;

    setError(null);
    setSuccess(null);
    try {
      await api.restockProduct(restockProductId, restockAmount, restockNotes);
      setSuccess(`Stok produk berhasil ditambah ${restockAmount} unit.`);
      setRestockProductId(null);
      await loadData();
    } catch (err) {
      setError((err as Error).message || "Gagal menambah stok.");
    }
  };

  if (!token || user?.role !== "Admin") return null;

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 border-b border-frame-subtle pb-6">
        <div>
          <Link
            href="/"
            className="inline-flex items-center space-x-1.5 text-xs font-mono text-ink-secondary hover:text-obsidian"
          >
            <ArrowLeft className="w-3.5 h-3.5" />
            <span>Katalog Depan</span>
          </Link>
          <div className="flex items-center space-x-2 mt-2">
            <Shield className="w-6 h-6 text-terracotta" />
            <h1 className="font-serif text-3xl font-bold text-obsidian">
              Panel Pengendalian Admin
            </h1>
          </div>
          <p className="text-xs font-mono text-ink-secondary mt-1">
            Manajemen logistik pesanan lintas pelanggan dan pemantauan stok pergudangan.
          </p>
        </div>

        <button
          onClick={loadData}
          disabled={isLoading}
          className="inline-flex items-center space-x-1.5 px-3 py-1.5 border border-frame-subtle rounded-xs text-xs font-mono bg-white hover:border-obsidian"
        >
          <RefreshCw className={`w-3.5 h-3.5 ${isLoading ? "animate-spin" : ""}`} />
          <span>Segarkan Data</span>
        </button>
      </div>

      {success && (
        <div className="p-3 bg-green-50 border border-green-200 text-green-800 rounded-xs text-xs font-mono">
          {success}
        </div>
      )}

      {error && (
        <div className="p-3 bg-red-50 border border-red-200 text-red-700 rounded-xs text-xs font-mono">
          {error}
        </div>
      )}

      {/* Tabs */}
      <div className="flex border-b border-frame-subtle space-x-4">
        <button
          onClick={() => setActiveTab("orders")}
          className={`pb-3 text-xs font-mono uppercase tracking-wider font-semibold border-b-2 transition-colors ${
            activeTab === "orders"
              ? "border-obsidian text-obsidian"
              : "border-transparent text-ink-muted hover:text-obsidian"
          }`}
        >
          Pesanan Masuk ({orders.length})
        </button>
        <button
          onClick={() => setActiveTab("inventory")}
          className={`pb-3 text-xs font-mono uppercase tracking-wider font-semibold border-b-2 transition-colors ${
            activeTab === "inventory"
              ? "border-obsidian text-obsidian"
              : "border-transparent text-ink-muted hover:text-obsidian"
          }`}
        >
          Inventaris Stok ({products.length} SKU)
        </button>
      </div>

      {/* Tab: Orders */}
      {activeTab === "orders" && (
        <div className="bg-white border border-frame-subtle rounded-xs overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full text-left border-collapse text-xs">
              <thead>
                <tr className="bg-canvas border-b border-frame-subtle font-mono text-ink-secondary uppercase text-[11px]">
                  <th className="p-3">ID Pesanan</th>
                  <th className="p-3">Pelanggan</th>
                  <th className="p-3">Total Tagihan</th>
                  <th className="p-3">Tanggal</th>
                  <th className="p-3">Status Saat Ini</th>
                  <th className="p-3 text-right">Ubah Status</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-frame-subtle font-sans">
                {orders.map((order) => (
                  <tr key={order.id} className="hover:bg-slate-50/50">
                    <td className="p-3 font-mono font-semibold text-obsidian">
                      #{order.id.substring(0, 8).toUpperCase()}
                    </td>
                    <td className="p-3">
                      <p className="font-medium text-obsidian">{order.customerName}</p>
                      <p className="text-[11px] font-mono text-ink-muted">{order.customerEmail}</p>
                    </td>
                    <td className="p-3 font-mono font-semibold">
                      {formatRupiah(order.totalAmount)}
                    </td>
                    <td className="p-3 font-mono text-[11px] text-ink-muted">
                      {formatDate(order.createdAt)}
                    </td>
                    <td className="p-3">
                      <span className="font-mono text-[10px] px-2 py-0.5 rounded-xs border border-frame-subtle bg-canvas font-bold">
                        {order.status}
                      </span>
                    </td>
                    <td className="p-3 text-right">
                      <select
                        value={order.status}
                        disabled={updatingOrderId === order.id}
                        onChange={(e) => handleStatusChange(order.id, e.target.value as OrderStatus)}
                        className="p-1.5 bg-canvas border border-frame-subtle rounded-xs text-xs font-mono focus:outline-hidden focus:border-obsidian"
                      >
                        <option value="Pending">Pending</option>
                        <option value="Processing">Processing</option>
                        <option value="Shipped">Shipped</option>
                        <option value="Delivered">Delivered</option>
                        <option value="Cancelled">Cancelled</option>
                      </select>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* Tab: Inventory */}
      {activeTab === "inventory" && (
        <div className="space-y-4">
          <div className="bg-white border border-frame-subtle rounded-xs overflow-hidden">
            <div className="overflow-x-auto">
              <table className="w-full text-left border-collapse text-xs">
                <thead>
                  <tr className="bg-canvas border-b border-frame-subtle font-mono text-ink-secondary uppercase text-[11px]">
                    <th className="p-3">Nama Produk</th>
                    <th className="p-3">Kategori</th>
                    <th className="p-3">Harga Satuan</th>
                    <th className="p-3">Sisa Stok</th>
                    <th className="p-3 text-right">Tindakan</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-frame-subtle font-sans">
                  {products.map((p) => (
                    <tr key={p.id} className="hover:bg-slate-50/50">
                      <td className="p-3 font-medium text-obsidian">
                        {p.name}
                      </td>
                      <td className="p-3 font-mono text-ink-secondary text-[11px]">
                        {p.category?.name || "-"}
                      </td>
                      <td className="p-3 font-mono font-semibold">
                        {formatRupiah(p.price)}
                      </td>
                      <td className="p-3 font-mono">
                        <span className={`font-bold ${p.stock < 10 ? "text-amber-600" : "text-green-700"}`}>
                          {p.stock} unit
                        </span>
                      </td>
                      <td className="p-3 text-right">
                        <button
                          onClick={() => setRestockProductId(p.id)}
                          className="px-2.5 py-1 bg-obsidian text-white text-[11px] font-mono rounded-xs hover:bg-terracotta"
                        >
                          + Tambah Stok
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>

          {/* Restock Modal */}
          {restockProductId && (
            <div className="fixed inset-0 bg-black/40 backdrop-blur-xs flex items-center justify-center p-4 z-50">
              <div className="bg-white border border-frame-subtle rounded-xs p-6 max-w-md w-full space-y-4">
                <h3 className="font-serif text-lg font-bold text-obsidian">
                  Pencatatan Masuk Gudang (Restock)
                </h3>
                <p className="text-xs font-mono text-ink-secondary">
                  SKU: {products.find((p) => p.id === restockProductId)?.name}
                </p>

                <form onSubmit={handleRestock} className="space-y-3">
                  <div>
                    <label className="block text-xs font-mono text-ink-secondary mb-1">
                      Jumlah Unit Tambahan
                    </label>
                    <input
                      type="number"
                      required
                      min={1}
                      value={restockAmount}
                      onChange={(e) => setRestockAmount(parseInt(e.target.value) || 1)}
                      className="w-full p-2 bg-canvas border border-frame-subtle rounded-xs text-xs font-mono text-obsidian"
                    />
                  </div>

                  <div>
                    <label className="block text-xs font-mono text-ink-secondary mb-1">
                      Catatan Dokumen Penerimaan / PO
                    </label>
                    <input
                      type="text"
                      value={restockNotes}
                      onChange={(e) => setRestockNotes(e.target.value)}
                      className="w-full p-2 bg-canvas border border-frame-subtle rounded-xs text-xs font-sans text-obsidian"
                    />
                  </div>

                  <div className="flex space-x-2 pt-2">
                    <button
                      type="submit"
                      className="flex-1 py-2 bg-obsidian text-white text-xs font-mono uppercase tracking-wider rounded-xs hover:bg-terracotta"
                    >
                      Konfirmasi Stok Masuk
                    </button>
                    <button
                      type="button"
                      onClick={() => setRestockProductId(null)}
                      className="px-4 py-2 border border-frame-subtle text-xs font-mono rounded-xs hover:bg-canvas"
                    >
                      Batal
                    </button>
                  </div>
                </form>
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
