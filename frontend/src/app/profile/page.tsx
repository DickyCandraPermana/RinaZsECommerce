"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { UserProfile } from "@/types";
import { api } from "@/lib/api";
import { useAuthStore } from "@/lib/authStore";
import Link from "next/link";
import { ArrowLeft, Save, CheckCircle2, User, MapPin } from "lucide-react";

export default function ProfilePage() {
  const router = useRouter();
  const { token, user, initialize } = useAuthStore();
  const [profile, setProfile] = useState<UserProfile | null>(null);
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [phone, setPhone] = useState("");
  const [address, setAddress] = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    initialize();
  }, [initialize]);

  useEffect(() => {
    if (!token && typeof window !== "undefined") {
      router.push("/auth/login?redirect=/profile");
      return;
    }

    async function loadProfile() {
      try {
        const data = await api.getProfile();
        setProfile(data);
        setFirstName(data.firstName || "");
        setLastName(data.lastName || "");
        setPhone(data.phone || "");
        setAddress(data.address || "");
      } catch (err) {
        setError((err as Error).message || "Gagal memuat profil.");
      } finally {
        setIsLoading(false);
      }
    }

    if (token) {
      loadProfile();
    }
  }, [token, router]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSaving(true);
    setError(null);
    setSuccessMessage(null);

    try {
      const updated = await api.updateProfile({
        firstName,
        lastName,
        phone,
        address,
      });
      setProfile(updated);
      setSuccessMessage("Profil dan alamat berhasil diperbarui.");
      setTimeout(() => setSuccessMessage(null), 3000);
    } catch (err) {
      setError((err as Error).message || "Gagal menyimpan perubahan.");
    } finally {
      setIsSaving(false);
    }
  };

  if (!token) return null;

  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <div>
        <Link
          href="/"
          className="inline-flex items-center space-x-1.5 text-xs font-mono text-ink-secondary hover:text-obsidian"
        >
          <ArrowLeft className="w-3.5 h-3.5" />
          <span>Kembali ke Katalog</span>
        </Link>
        <h1 className="font-serif text-3xl font-bold text-obsidian mt-2">
          Profil & Alamat Pengiriman
        </h1>
        <p className="text-xs font-mono text-ink-secondary mt-1">
          Kelola identitas penerima dan alamat pengiriman default untuk checkout kilat.
        </p>
      </div>

      {successMessage && (
        <div className="p-3 bg-green-50 border border-green-200 text-green-800 rounded-xs text-xs font-mono flex items-center space-x-2">
          <CheckCircle2 className="w-4 h-4 text-green-600" />
          <span>{successMessage}</span>
        </div>
      )}

      {error && (
        <div className="p-3 bg-red-50 border border-red-200 text-red-700 rounded-xs text-xs font-mono">
          {error}
        </div>
      )}

      <form onSubmit={handleSubmit} className="bg-white border border-frame-subtle p-6 rounded-xs space-y-6">
        <div className="space-y-4">
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label className="block text-xs font-mono text-ink-secondary mb-1">Nama Depan</label>
              <input
                type="text"
                required
                value={firstName}
                onChange={(e) => setFirstName(e.target.value)}
                className="w-full p-2.5 bg-canvas border border-frame-subtle rounded-xs text-xs font-sans text-obsidian focus:outline-hidden focus:border-obsidian"
              />
            </div>
            <div>
              <label className="block text-xs font-mono text-ink-secondary mb-1">Nama Belakang</label>
              <input
                type="text"
                value={lastName}
                onChange={(e) => setLastName(e.target.value)}
                className="w-full p-2.5 bg-canvas border border-frame-subtle rounded-xs text-xs font-sans text-obsidian focus:outline-hidden focus:border-obsidian"
              />
            </div>
          </div>

          <div>
            <label className="block text-xs font-mono text-ink-secondary mb-1">Nomor Telepon / WhatsApp</label>
            <input
              type="text"
              required
              value={phone}
              onChange={(e) => setPhone(e.target.value)}
              placeholder="08xxxxxxxxxx"
              className="w-full p-2.5 bg-canvas border border-frame-subtle rounded-xs text-xs font-sans text-obsidian focus:outline-hidden focus:border-obsidian"
            />
          </div>

          <div>
            <label className="block text-xs font-mono text-ink-secondary mb-1">Alamat Domisili / Pengiriman Default</label>
            <textarea
              required
              rows={4}
              value={address}
              onChange={(e) => setAddress(e.target.value)}
              className="w-full p-2.5 bg-canvas border border-frame-subtle rounded-xs text-xs font-sans text-obsidian focus:outline-hidden focus:border-obsidian"
            />
          </div>

          <div className="p-3 bg-canvas border border-frame-subtle rounded-xs text-xs font-mono text-ink-muted">
            <span className="font-bold text-obsidian">Email Terdaftar:</span> {profile?.email || user?.email} (Dikelola oleh subsistem autentikasi)
          </div>
        </div>

        <button
          type="submit"
          disabled={isSaving}
          className="inline-flex items-center space-x-2 px-5 py-2.5 bg-obsidian text-white text-xs font-mono uppercase tracking-wider rounded-xs hover:bg-terracotta disabled:opacity-50 transition-colors"
        >
          <Save className="w-3.5 h-3.5" />
          <span>{isSaving ? "Menyimpan..." : "Simpan Perubahan"}</span>
        </button>
      </form>
    </div>
  );
}
