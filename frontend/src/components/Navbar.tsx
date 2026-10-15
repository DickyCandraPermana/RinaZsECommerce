"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { useAuthStore } from "@/lib/authStore";
import { useCartStore } from "@/lib/cartStore";
import { ShoppingBag, User as UserIcon, LogOut, Shield } from "lucide-react";

export function Navbar() {
  const { user, token, logout, initialize } = useAuthStore();
  const { getItemCount, setOpen, fetchCart } = useCartStore();
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    initialize();
    setMounted(true);
  }, [initialize]);

  useEffect(() => {
    if (token) {
      fetchCart();
    }
  }, [token, fetchCart]);

  const itemCount = mounted ? getItemCount() : 0;

  return (
    <header className="sticky top-0 z-40 bg-[#FBFBF9]/90 backdrop-blur-md border-b border-frame-subtle">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-16 flex items-center justify-between">
        {/* Brand */}
        <div className="flex items-center space-x-6">
          <Link href="/" className="flex items-baseline space-x-2 group">
            <span className="font-serif text-2xl font-bold tracking-tight text-obsidian">RinaZs</span>
            <span className="text-xs font-mono uppercase tracking-widest text-ink-muted">Studio</span>
          </Link>
          <nav className="hidden md:flex items-center space-x-6 text-sm font-medium">
            <Link href="/" className="text-ink-primary hover:text-terracotta transition-colors">
              Katalog Fisik
            </Link>
            {token && (
              <Link href="/orders" className="text-ink-secondary hover:text-ink-primary transition-colors">
                Pesanan Saya
              </Link>
            )}
            {user?.role === "Admin" && (
              <Link
                href="/admin"
                className="inline-flex items-center space-x-1 text-xs font-mono uppercase px-2 py-1 bg-obsidian text-white rounded hover:bg-terracotta transition-colors"
              >
                <Shield className="w-3 h-3" />
                <span>Admin Panel</span>
              </Link>
            )}
          </nav>
        </div>

        {/* Actions */}
        <div className="flex items-center space-x-4">
          {mounted && (
            <>
              {token ? (
                <div className="flex items-center space-x-3 text-sm">
                  <Link
                    href="/profile"
                    className="inline-flex items-center space-x-1.5 text-ink-secondary hover:text-ink-primary"
                  >
                    <UserIcon className="w-4 h-4" />
                    <span className="hidden sm:inline font-mono text-xs">{user?.name}</span>
                  </Link>
                  <button
                    onClick={() => logout()}
                    title="Keluar"
                    className="p-1.5 text-ink-muted hover:text-red-600 transition-colors"
                  >
                    <LogOut className="w-4 h-4" />
                  </button>
                </div>
              ) : (
                <Link
                  href="/auth/login"
                  className="text-xs font-mono uppercase tracking-wider px-3 py-1.5 border border-frame-dark text-obsidian hover:bg-obsidian hover:text-white transition-all rounded-sm"
                >
                  Masuk Akun
                </Link>
              )}

              {/* Cart Trigger */}
              <button
                onClick={() => setOpen(true)}
                className="relative p-2 text-obsidian hover:text-terracotta transition-colors"
                aria-label="Keranjang Belanja"
              >
                <ShoppingBag className="w-5 h-5" />
                {itemCount > 0 && (
                  <span className="absolute -top-1 -right-1 bg-terracotta text-white font-mono text-[10px] w-4 h-4 rounded-full flex items-center justify-center font-bold">
                    {itemCount}
                  </span>
                )}
              </button>
            </>
          )}
        </div>
      </div>
    </header>
  );
}
