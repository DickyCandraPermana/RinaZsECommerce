import type { Metadata } from "next";
import "./globals.css";
import { Navbar } from "@/components/Navbar";
import { Footer } from "@/components/Footer";
import { CartDrawer } from "@/components/CartDrawer";

export const metadata: Metadata = {
  title: "RinaZs Studio & Hardware — E-Commerce Editorial",
  description: "Katalog fisik pakaian tahan lama, periferal mekanis presisi, dan perangkat keras fungsional. Transparan pada material, bebas klaim pemasaran kosong.",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="id">
      <body className="min-h-screen flex flex-col bg-canvas text-obsidian">
        <Navbar />
        <main className="flex-1 max-w-7xl w-full mx-auto px-4 sm:px-6 lg:px-8 py-8">
          {children}
        </main>
        <CartDrawer />
        <Footer />
      </body>
    </html>
  );
}
