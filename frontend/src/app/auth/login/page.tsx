"use client";

import { useState, Suspense } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { api } from "@/lib/api";
import { useAuthStore } from "@/lib/authStore";
import Link from "next/link";
import { ArrowLeft, LogIn, UserCheck, Shield } from "lucide-react";

function LoginForm() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const redirect = searchParams.get("redirect") || "/";

  const { login } = useAuthStore();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError(null);
    try {
      const res = await api.login(email, password);
      login(res.token);
      router.push(redirect);
    } catch (err) {
      setError((err as Error).message || "Email atau kata sandi tidak cocok.");
    } finally {
      setIsLoading(false);
    }
  };

  const autofill = (demoEmail: string, demoPass: string) => {
    setEmail(demoEmail);
    setPassword(demoPass);
  };

  return (
    <div className="space-y-6">
      {/* Demo Credentials Quick-Fill Cards */}
      <div className="p-4 bg-white border border-frame-subtle rounded-xs space-y-2">
        <span className="text-[11px] font-mono uppercase tracking-wider text-ink-muted block font-semibold">
          Akses Cepat Pengujian (Demo):
        </span>
        <div className="grid grid-cols-2 gap-2">
          <button
            type="button"
            onClick={() => autofill("customer@rinazs.local", "CustomerPass123!")}
            className="p-2 border border-frame-subtle rounded-xs text-left hover:border-obsidian transition-colors text-xs font-mono bg-canvas"
          >
            <div className="flex items-center space-x-1 text-obsidian font-bold">
              <UserCheck className="w-3.5 h-3.5 text-blue-600" />
              <span>Customer</span>
            </div>
            <p className="text-[10px] text-ink-muted truncate">customer@rinazs.local</p>
          </button>

          <button
            type="button"
            onClick={() => autofill("admin@rinazs.local", "AdminPass123!")}
            className="p-2 border border-frame-subtle rounded-xs text-left hover:border-obsidian transition-colors text-xs font-mono bg-canvas"
          >
            <div className="flex items-center space-x-1 text-obsidian font-bold">
              <Shield className="w-3.5 h-3.5 text-terracotta" />
              <span>Admin</span>
            </div>
            <p className="text-[10px] text-ink-muted truncate">admin@rinazs.local</p>
          </button>
        </div>
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
          <label className="block text-xs font-mono text-ink-secondary mb-1">Kata Sandi</label>
          <input
            type="password"
            required
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
          <LogIn className="w-4 h-4" />
          <span>{isLoading ? "Memvalidasi..." : "Masuk ke Studio"}</span>
        </button>

        <div className="text-center pt-2 text-xs text-ink-muted font-mono">
          Belum memiliki akun?{" "}
          <Link href="/auth/register" className="text-obsidian underline hover:text-terracotta">
            Daftar baru
          </Link>
        </div>
      </form>
    </div>
  );
}

export default function LoginPage() {
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
          Masuk ke Akun
        </h1>
        <p className="text-xs font-mono text-ink-secondary mt-1">
          Gunakan kredensial terdaftar untuk melanjutkan transaksi.
        </p>
      </div>

      <Suspense fallback={<div className="py-12 text-center font-mono text-xs text-ink-muted">Memuat formulir masuk...</div>}>
        <LoginForm />
      </Suspense>
    </div>
  );
}
