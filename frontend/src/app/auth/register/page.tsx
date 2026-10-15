"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { api } from "@/lib/api";
import { useAuthStore } from "@/lib/authStore";
import Link from "next/link";
import { ArrowLeft, UserPlus } from "lucide-react";

export default function RegisterPage() {
  const router = useRouter();
  const { login } = useAuthStore();
  const [email, setEmail] = useState("");
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError(null);
    try {
      const res = await api.register(email, username, password);
      login(res.token);
      router.push("/");
    } catch (err) {
      setError((err as Error).message || "Pendaftaran gagal. Periksa kembali format data.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="max-w-md mx-auto py-8 space-y-6">
      <div>
        <Link
          href="/"
          className="inline-flex items-center space-x-1.5 text-xs font-mono text-ink-secondary hover:text-obsidian"
        >
          <ArrowLeft className="w-3.5 h-3.5" />
          <span>Kembali ke Toko</span>
        </Link>
        <h1 className="font-serif text-3xl font-bold text-obsidian mt-2">
          Pendaftaran Akun
        </h1>
        <p className="text-xs font-mono text-ink-secondary mt-1">
          Buat akun pelanggan baru untuk mulai berbelanja di RinaZs Atelier.
        </p>
      </div>

      {error && (
        <div className="p-3 bg-red-50 border border-red-200 text-red-700 rounded-xs text-xs font-mono">
          {error}
        </div>
      )}

      <form onSubmit={handleSubmit} className="bg-white border border-frame-subtle p-6 rounded-xs space-y-4">
        <div>
          <label className="block text-xs font-mono text-ink-secondary mb-1">Alamat Email</label>
          <input
            type="email"
            required
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="nama@email.com"
            className="w-full p-2.5 bg-canvas border border-frame-subtle rounded-xs text-xs font-sans text-obsidian focus:outline-hidden focus:border-obsidian"
          />
        </div>

        <div>
          <label className="block text-xs font-mono text-ink-secondary mb-1">Username</label>
          <input
            type="text"
            required
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            placeholder="username_anda"
            className="w-full p-2.5 bg-canvas border border-frame-subtle rounded-xs text-xs font-sans text-obsidian focus:outline-hidden focus:border-obsidian"
          />
        </div>

        <div>
          <label className="block text-xs font-mono text-ink-secondary mb-1">Kata Sandi (Min. 6 Karakter)</label>
          <input
            type="password"
            required
            minLength={6}
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="••••••••"
            className="w-full p-2.5 bg-canvas border border-frame-subtle rounded-xs text-xs font-sans text-obsidian focus:outline-hidden focus:border-obsidian"
          />
        </div>

        <button
          type="submit"
          disabled={isLoading}
          className="w-full py-3 bg-obsidian text-white text-xs font-mono uppercase tracking-wider rounded-xs hover:bg-terracotta disabled:opacity-50 transition-colors flex items-center justify-center space-x-2"
        >
          <UserPlus className="w-4 h-4" />
          <span>{isLoading ? "Mendaftarkan..." : "Daftar Akun Sekarang"}</span>
        </button>

        <div className="text-center pt-2 text-xs text-ink-muted font-mono">
          Sudah memiliki akun?{" "}
          <Link href="/auth/login" className="text-obsidian underline hover:text-terracotta">
            Masuk di sini
          </Link>
        </div>
      </form>
    </div>
  );
}
