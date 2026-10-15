"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useAuthStore } from "@/lib/authStore";
import { useCartStore } from "@/lib/cartStore";
import { api } from "@/lib/api";
import { formatRupiah } from "@/lib/format";
import Link from "next/link";
import { ArrowLeft, CheckCircle2, ShieldCheck, MapPin, Truck } from "lucide-react";

export default function CheckoutPage() {
  const router = useRouter();
  const { token, user, initialize } = useAuthStore();
  const { items, getTotal, clearCart, fetchCart } = useCartStore();

  const [address, setAddress] = useState("");
  const [description, setDescription] = useState("");
  const [isLoadingProfile, setIsLoadingProfile] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    initialize();
  }, [initialize]);

  useEffect(() => {
    if (!token && typeof window !== "undefined") {
      router.push("/auth/login?redirect=/checkout");
      return;
    }

    async function loadProfile() {
      try {
        const profile = await api.getProfile();
        if (profile?.address) {
          setAddress(profile.address);
        }
      } catch {
        // Fallback default Surakarta
        setAddress("Jl. Slamet Riyadi No. 120, Surakarta, Jawa Tengah 57126");
      } finally {
        setIsLoadingProfile(false);
      }
    }

    if (token) {
      loadProfile();
      fetchCart();
    }
  }, [token, router, fetchCart]);

  const total = getTotal();

  const handlePlaceOrder = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!address.trim()) {
      setError("Alamat pengiriman wajib diisi.");
      return;
    }
    if (items.length === 0) {
      setError("Keranjang belanja Anda kosong.");
      return;
    }

    setIsSubmitting(true);
    setError(null);
    try {
      const res = await api.checkout(address, description);
      await clearCart();
      router.push(`/orders/${res.orderId}`);
    } catch (err) {
      setError((err as Error).message || "Gagal memproses pesanan.");
      setIsSubmitting(false);
    }
  };

  if (!token) return null;

  return (
    <div className="max-w-4xl mx-auto space-y-8">
      <div>
        <Link
          href="/"
          className="inline-flex items-center space-x-1.5 text-xs font-mono text-ink-secondary hover:text-obsidian"
        >
          <ArrowLeft className="w-3.5 h-3.5" />
          <span>Kembali ke Belanja</span>
        </Link>
        <h1 className="font-serif text-3xl font-bold text-obsidian mt-2">
          Penyelesaian Pesanan (Checkout)
        </h1>
        <p className="text-xs font-mono text-ink-secondary mt-1">
          Verifikasi alamat tujuan dan rincian alokasi barang.
        </p>
      </div>

      {error && (
        <div className="p-4 bg-red-50 border border-red-200 rounded-xs text-xs font-mono text-red-700">
          {error}
        </div>
      )}

      <form onSubmit={handlePlaceOrder} className="grid grid-cols-1 lg:grid-cols-12 gap-8">
        {/* Left Column: Form */}
        <div className="lg:col-span-7 space-y-6">
          <div className="bg-white border border-frame-subtle p-6 rounded-xs space-y-4">
            <div className="flex items-center space-x-2 border-b border-frame-subtle pb-3">
              <MapPin className="w-4 h-4 text-terracotta" />
              <h2 className="text-sm font-bold text-obsidian font-serif uppercase tracking-wider">
                Alamat Pengiriman Fisik
              </h2>
            </div>

            <div className="space-y-3">
              <div>
                <label className="block text-xs font-mono text-ink-secondary mb-1">
                  Alamat Lengkap (Beserta Nomor Rumah / Kode Pos)
                </label>
                <textarea
                  required
                  rows={4}
                  value={address}
                  onChange={(e) => setAddress(e.target.value)}
                  placeholder="Contoh: Jl. Slamet Riyadi No. 120, RT 02/RW 04, Sriwedari, Laweyan, Surakarta, Jawa Tengah 57126"
                  className="w-full p-3 bg-canvas border border-frame-subtle rounded-xs text-xs font-sans text-obsidian focus:outline-hidden focus:border-obsidian"
                />
              </div>

              <div>
                <label className="block text-xs font-mono text-ink-secondary mb-1">
                  Catatan Tambahan untuk Kurir / Ekspedisi (Opsional)
                </label>
                <input
                  type="text"
                  value={description}
                  onChange={(e) => setDescription(e.target.value)}
                  placeholder="Titipkan di pos satpam / hubungi via WhatsApp saat tiba"
                  className="w-full p-2.5 bg-canvas border border-frame-subtle rounded-xs text-xs font-sans text-obsidian focus:outline-hidden focus:border-obsidian"
                />
              </div>
            </div>
          </div>

          <div className="bg-white border border-frame-subtle p-6 rounded-xs space-y-3">
            <div className="flex items-center space-x-2 border-b border-frame-subtle pb-3">
              <Truck className="w-4 h-4 text-ink-secondary" />
              <h2 className="text-sm font-bold text-obsidian font-serif uppercase tracking-wider">
                Metode Distribusi & Pembayaran
              </h2>
            </div>
            <div className="p-3 bg-canvas border border-frame-subtle rounded-xs text-xs font-mono space-y-1">
              <div className="flex justify-between font-bold text-obsidian">
                <span>Ekspedisi Standar Darat/Udara</span>
                <span className="text-green-700">Gratis (Subsidi Studio)</span>
              </div>
              <p className="text-[11px] text-ink-muted">
                Estimasi pengiriman 2-3 hari kerja dari warehouse Surakarta.
              </p>
            </div>
            <div className="p-3 bg-canvas border border-frame-subtle rounded-xs text-xs font-mono space-y-1">
              <div className="flex justify-between font-bold text-obsidian">
                <span>Metode Pembayaran</span>
                <span>Transfer Bank / Virtual Account</span>
              </div>
              <p className="text-[11px] text-ink-muted">
                Instruksi pembayaran otomatis diterbitkan saat pesanan terkonfirmasi.
              </p>
            </div>
          </div>
        </div>

        {/* Right Column: Order Summary */}
        <div className="lg:col-span-5 space-y-6">
          <div className="bg-white border border-frame-subtle p-6 rounded-xs space-y-4">
            <h2 className="text-sm font-bold text-obsidian font-serif uppercase tracking-wider border-b border-frame-subtle pb-3">
              Rincian Barang ({items.length})
            </h2>

            <div className="space-y-3 max-h-72 overflow-y-auto pr-1">
              {items.map((item) => (
                <div key={item.id} className="flex justify-between items-start text-xs border-b border-frame-subtle/50 pb-2">
                  <div className="space-y-0.5">
                    <p className="font-medium text-obsidian line-clamp-1">{item.productName}</p>
                    <p className="font-mono text-ink-muted text-[11px]">
                      {item.quantity} × {formatRupiah(item.price)}
                    </p>
                  </div>
                  <span className="font-mono font-bold text-obsidian">
                    {formatRupiah(item.subtotal || item.price * item.quantity)}
                  </span>
                </div>
              ))}
            </div>

            <div className="border-t border-frame-subtle pt-3 space-y-2 text-xs font-mono">
              <div className="flex justify-between text-ink-secondary">
                <span>Subtotal Barang</span>
                <span>{formatRupiah(total)}</span>
              </div>
              <div className="flex justify-between text-ink-secondary">
                <span>Biaya Pengiriman</span>
                <span className="text-green-700 font-bold">Rp 0</span>
              </div>
              <div className="flex justify-between text-sm font-bold text-obsidian pt-2 border-t border-frame-subtle">
                <span>Total Pembayaran</span>
                <span className="text-base text-terracotta">{formatRupiah(total)}</span>
              </div>
            </div>

            <button
              type="submit"
              disabled={isSubmitting || items.length === 0}
              className="w-full py-3.5 bg-obsidian text-white text-xs font-mono uppercase tracking-wider rounded-xs hover:bg-terracotta disabled:opacity-50 transition-colors flex items-center justify-center space-x-2"
            >
              <CheckCircle2 className="w-4 h-4" />
              <span>{isSubmitting ? "Mengalokasikan Pesanan..." : "Konfirmasi & Buat Pesanan"}</span>
            </button>
          </div>

          <div className="p-4 bg-canvas border border-frame-subtle rounded-xs text-[11px] font-mono text-ink-muted flex items-start space-x-2">
            <ShieldCheck className="w-4 h-4 flex-shrink-0 text-ink-secondary mt-0.5" />
            <span>
              Stok langsung dikurangi secara atomik di database PostgreSQL. Anda dapat membatalkan pesanan kapan saja selama berstatus Pending.
            </span>
          </div>
        </div>
      </form>
    </div>
  );
}
