import Link from "next/link";

export function Footer() {
  return (
    <footer className="bg-white border-t border-frame-subtle mt-20">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
          <div className="md:col-span-2 space-y-3">
            <span className="font-serif text-xl font-bold tracking-tight text-obsidian">RinaZs Atelier</span>
            <p className="text-xs text-ink-secondary leading-relaxed max-w-md font-sans">
              Menyediakan produk fisik kurasi spesifik: pakaian berdaya tahan tinggi, periferal mekanis, dan perangkat keras fungsional. Transparan pada material, jujur pada spesifikasi, bebas klaim kosong.
            </p>
            <p className="text-[11px] font-mono text-ink-muted">
              Distribusi Logistik: Pergudangan Surakarta, Jawa Tengah (57126).
            </p>
          </div>

          <div>
            <h4 className="text-xs font-mono uppercase tracking-wider text-obsidian mb-3 font-semibold">Navigasi</h4>
            <ul className="space-y-2 text-xs text-ink-secondary">
              <li><Link href="/" className="hover:text-terracotta">Katalog Lengkap</Link></li>
              <li><Link href="/orders" className="hover:text-terracotta">Pelacakan Pesanan</Link></li>
              <li><Link href="/profile" className="hover:text-terracotta">Profil & Alamat</Link></li>
            </ul>
          </div>

          <div>
            <h4 className="text-xs font-mono uppercase tracking-wider text-obsidian mb-3 font-semibold">Integritas Teknis</h4>
            <ul className="space-y-1.5 text-xs text-ink-muted font-mono">
              <li>.NET 8 Clean Architecture</li>
              <li>PostgreSQL ACID Transactions</li>
              <li>Next.js 15 App Router</li>
              <li>Standar Anti-AI Slop</li>
            </ul>
          </div>
        </div>

        <div className="border-t border-frame-subtle mt-8 pt-8 flex flex-col sm:flex-row items-center justify-between text-xs text-ink-muted font-mono">
          <p>© {new Date().getFullYear()} RinaZs Atelier. Hak cipta dilindungi undang-undang.</p>
          <p className="mt-2 sm:mt-0">Kode & Infrastruktur Terverifikasi.</p>
        </div>
      </div>
    </footer>
  );
}
