# Spesifikasi Kebutuhan Teknis & Rencana Kerja TDD Rinazs E-Commerce

Dokumen ini memuat spesifikasi teknis lengkap untuk backend API ASP.NET Core Clean Architecture dan frontend Next.js pada platform Rinazs E-Commerce. Seluruh perancangan mengacu pada use case pelanggan dan admin di `todo.md`, pemisahan ketat entitas `User` dan `UserProfile`, standardisasi copywriting anti-AI slop, serta alur implementasi berjenjang menggunakan metodologi Test-Driven Development (TDD).

---

## 1. Arsitektur Domain & Relasi User - UserProfile

Sesuai arahan arsitektur, akun otentikasi dipisahkan secara tegas dari profil personal dan operasional bisnis:

```
+------------------------------------+        1 : 1        +-----------------------------------------+
|                User                | ------------------> |               UserProfile               |
+------------------------------------+ (HasForeignKey:     +-----------------------------------------+
| Id (Guid, PK)                      |  UserProfile.UserId)| Id (Guid, PK)                           |
| Username (varchar 20, unique)      |                     | UserId (Guid, FK -> User.Id, unique)    |
| Email (varchar 256, unique)        |                     | FirstName (varchar 50, nullable)        |
| PasswordHash (text, not auditable) |                     | LastName (varchar 50, nullable)         |
| Role (Customer | Admin)            |                     | Avatar (text, nullable)                 |
| ProfileId (Guid)                   |                     | Address (text, nullable)                |
| CreatedAt, UpdatedAt, DeletedAt    |                     | DateOfBirth (date, nullable)            |
+------------------------------------+                     | PhoneNumber (varchar 20, nullable)      |
                                                           | Gender (bool, nullable)                 |
                                                           | Verified (bool, default false)          |
                                                           | CreatedAt, UpdatedAt, DeletedAt         |
                                                           +-----------------------------------------+
                                                                               |
                                      +--------------------+-------------------+--------------------+
                                      | 1:N                | 1:N               | 1:N                | 1:N
                                      v                    v                   v                    v
                                 [CartItems]            [Orders]            [Reviews]           [StockLogs]
```

### Aturan Domain:
1. `User` bertanggung jawab atas kredensial masuk (email, password hash, role JWT claim, audit sign-in).
2. `UserProfile` menyimpan identitas fisik pengiriman, penagihan, serta kepemilikan relasi transaksional (`Orders`, `CartItems`, `Reviews`, `Vouchers`).
3. Seluruh relasi transaksi di database mengaitkan `UserProfileId` (tersedia via `User.ProfileId`) untuk menjaga integritas data pelanggan meskipun kredensial login diubah.

---

## 2. Use Case Matrix & Spesifikasi REST API

### Ringkasan Konvensi API
- **Base URL:** `/api/v1`
- **Format Header:**
  - `Content-Type: application/json`
  - `Authorization: Bearer <jwt-token>` (untuk endpoint terproteksi)
- **Format Respon Standar:**
  - Sukses tunggal: `{ "data": { ... } }` atau instan object DTO.
  - Sukses paginasi:
    ```json
    {
      "items": [ ... ],
      "totalCount": 100,
      "pageNumber": 1,
      "pageSize": 10,
      "totalPages": 10,
      "hasPreviousPage": false,
      "hasNextPage": true
    }
    ```
  - Error:
    ```json
    {
      "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
      "title": "Validation Failed",
      "status": 400,
      "errors": {
        "Email": ["Format email tidak valid."]
      }
    }
    ```

---

### 2.1 Use Case Pelanggan (Customer Endpoints)

| UC ID | Nama Fitur | Method & Route | Auth & Role | Request Payload / Params | Response DTO & Status Code |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **UC-01** | Register Akun | `POST /api/v1/auth/register` | Public | **Body (JSON):**<br>`{ "username": "rinaz_buyer", "email": "buyer@rinazs.com", "password": "Password123!", "firstName": "Rina", "lastName": "Suryani" }` | **201 Created**<br>`{ "userId": "3fa85f64...", "profileId": "8ba12c77...", "message": "Akun berhasil didaftarkan." }`<br>Error: 400 Bad Request, 409 Conflict (Email/Username terdaftar) |
| **UC-02** | Login Akun | `POST /api/v1/auth/login` | Public | **Body (JSON):**<br>`{ "email": "buyer@rinazs.com", "password": "Password123!" }` | **200 OK**<br>`{ "token": "eyJhbGciOi...", "expiresIn": 86400, "user": { "id": "3fa8...", "username": "rinaz_buyer", "email": "buyer@rinazs.com", "role": "Customer" } }`<br>Error: 401 Unauthorized |
| **UC-03** | Lihat Katalog Produk | `GET /api/v1/products` | Public | **Query Params:**<br>`pageNumber=1&pageSize=12&categoryId=guid&minPrice=50000&maxPrice=300000&sortBy=price&isAscending=true` | **200 OK**<br>`PagedResult<ProductSummaryResponse>`<br>`{ "items": [ { "id": "guid", "name": "Kaos Katun Combed 24s", "price": 89000, "stock": 45, "rating": 4.8, "categoryName": "Pakaian Pria", "thumbnailUrl": "https://..." } ], "totalCount": 48, ... }` |
| **UC-04** | Cari Produk | `GET /api/v1/products/search` | Public | **Query Params:**<br>`query=keyboard&pageNumber=1&pageSize=10` | **200 OK**<br>`PagedResult<ProductSummaryResponse>`<br>Error: 400 Bad Request (query kosong) |
| **UC-05** | Lihat Detail Produk | `GET /api/v1/products/{id}` | Public | **Path:** `id` (Guid) | **200 OK**<br>`ProductDetailResponse`<br>`{ "id": "guid", "name": "Mechanical Keyboard TKL", "description": "Layout 87 tombol...", "price": 420000, "stock": 18, "rating": 4.9, "category": { "id": "guid", "name": "Tech & Gadget" }, "supplierName": "PT Rina Perkasa", "images": [ { "id": "guid", "url": "...", "isPrimary": true } ] }`<br>Error: 404 Not Found |
| **UC-06** | Tambah ke Keranjang | `POST /api/v1/carts` | Customer | **Body (JSON):**<br>`{ "productId": "3fa85f64...", "quantity": 2 }` | **201 Created**<br>`{ "cartItemId": "guid", "productId": "guid", "quantity": 2, "subtotal": 178000 }`<br>Error: 400 Bad Request, 404 Product Not Found, 422 Stock Insufficient |
| **UC-07** | Lihat Keranjang Belanja | `GET /api/v1/carts` | Customer | *None* | **200 OK**<br>`CartResponse`<br>`{ "items": [ { "id": "guid", "productId": "guid", "productName": "Kaos Katun Combed 24s", "price": 89000, "quantity": 2, "subtotal": 178000, "imageUrl": "..." } ], "totalAmount": 178000, "itemCount": 2 }` |
| **UC-08** | Ubah Kuantitas Item Keranjang | `PUT /api/v1/carts/items/{id}` | Customer | **Path:** `id` (Guid - CartItemId)<br>**Body (JSON):**<br>`{ "quantity": 3 }` | **200 OK**<br>`{ "cartItemId": "guid", "quantity": 3, "subtotal": 267000 }`<br>Error: 400 Bad Request (qty <= 0), 404 Item Not Found, 422 Stock Insufficient |
| **UC-09** | Hapus Item dari Keranjang | `DELETE /api/v1/carts/items/{id}` | Customer | **Path:** `id` (Guid - CartItemId) | **204 No Content**<br>Error: 404 Item Not Found |
| **UC-10** | Buat Pesanan (Checkout) | `POST /api/v1/orders/checkout` | Customer | **Body (JSON):**<br>`{ "address": "Jl. Merdeka No. 45, Bandung", "notes": "Tolong kirim sebelum jam 5 sore", "paymentMethod": "BankTransfer" }` | **201 Created**<br>`{ "orderId": "guid", "totalAmount": 267000, "status": "Pending", "paymentStatus": "Pending", "createdAt": "2026-10-15T10:00:00Z" }`<br>Error: 400 Cart Empty, 422 Stock Run Out During Checkout |
| **UC-11** | Lihat Riwayat Pesanan | `GET /api/v1/orders` | Customer | **Query Params:**<br>`pageNumber=1&pageSize=10&status=Pending` | **200 OK**<br>`PagedResult<OrderListItemResponse>`<br>`{ "items": [ { "id": "guid", "createdAt": "2026-10-15...", "status": "Pending", "totalAmount": 267000, "totalItems": 1 } ] }` |
| **UC-12** | Lihat Detail Pesanan | `GET /api/v1/orders/{id}` | Customer | **Path:** `id` (Guid) | **200 OK**<br>`OrderDetailResponse`<br>`{ "id": "guid", "status": "Pending", "totalAmount": 267000, "address": "Jl. Merdeka No. 45", "items": [ { "productId": "guid", "productName": "Kaos...", "amount": 3, "priceAtPurchase": 89000, "subtotal": 267000 } ], "payment": { "method": "BankTransfer", "status": "Pending" } }`<br>Error: 404 Not Found, 403 Forbidden |
| **UC-13** | Batalkan Pesanan | `POST /api/v1/orders/{id}/cancel` | Customer | **Path:** `id` (Guid)<br>**Body (JSON, optional):**<br>`{ "reason": "Ingin mengganti alamat pengiriman" }` | **200 OK**<br>`{ "orderId": "guid", "status": "Cancelled", "message": "Pesanan berhasil dibatalkan dan stok dikembalikan." }`<br>Error: 400 Status Not Cancellable (sudah Shipped/Delivered), 404 Not Found |
| **UC-14** | Kelola Alamat Pengiriman | `PUT /api/v1/profile/address` | Customer | **Body (JSON):**<br>`{ "address": "Jl. Gatot Subroto No. 12, RT 02/05, Jakarta Selatan 12930", "phoneNumber": "081234567890" }` | **200 OK**<br>`{ "address": "Jl. Gatot Subroto...", "phoneNumber": "081234567890", "message": "Alamat utama diperbarui." }`<br>Error: 400 Validasi Alamat |
| **UC-15** | Ubah Profil Pelanggan | `PUT /api/v1/profile` | Customer | **Body (JSON):**<br>`{ "firstName": "Rina", "lastName": "Suryani", "phoneNumber": "081234567890", "dateOfBirth": "1998-05-20", "gender": true, "avatar": "https://..." }` | **200 OK**<br>`UserProfileResponse`<br>`{ "profileId": "guid", "userId": "guid", "firstName": "Rina", "lastName": "Suryani", "email": "buyer@rinazs.com", "phoneNumber": "081234567890", "verified": false }`<br>Error: 400 Bad Request |

---

### 2.2 Use Case Administrator (Admin Endpoints)

| UC ID | Nama Fitur | Method & Route | Auth & Role | Request Payload / Params | Response DTO & Status Code |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **ADM-01** | Tambah Produk Baru | `POST /api/v1/admin/products` | Admin | **Body (JSON):**<br>`{ "name": "Jaket Parka Canvas Drill", "description": "Bahan katun drill...", "price": 245000, "stock": 30, "categoryId": "guid", "supplierId": "guid", "imageUrls": ["https://..."] }` | **201 Created**<br>`{ "productId": "guid", "message": "Produk berhasil ditambahkan ke katalog." }`<br>Error: 400 Bad Request (Field wajib kosong) |
| **ADM-02** | Ubah Produk | `PUT /api/v1/admin/products/{id}` | Admin | **Path:** `id` (Guid)<br>**Body (JSON):**<br>`{ "name": "Jaket Parka Canvas Drill Rev", "description": "Deskripsi baru...", "price": 255000, "categoryId": "guid", "status": true }` | **200 OK**<br>`{ "productId": "guid", "message": "Informasi produk berhasil diperbarui." }`<br>Error: 404 Not Found, 400 Bad Request |
| **ADM-03** | Hapus Produk | `DELETE /api/v1/admin/products/{id}` | Admin | **Path:** `id` (Guid) | **204 No Content** (Soft delete sets `DeletedAt = DateTime.UtcNow`)<br>Error: 404 Not Found |
| **ADM-04** | Kelola & Penyesuaian Stok | `POST /api/v1/admin/products/{id}/stock` | Admin | **Path:** `id` (Guid)<br>**Body (JSON):**<br>`{ "quantity": 50, "transactionType": "Restock", "notes": "Penerimaan PO #9928 dari supplier" }` | **200 OK**<br>`{ "productId": "guid", "currentStock": 80, "logId": "guid", "type": "Restock" }`<br>Error: 400 Invalid Adjustment (stok menjadi minus), 404 Not Found |
| **ADM-05** | Tambah & Kelola Kategori | `POST /api/v1/admin/categories` | Admin | **Body (JSON):**<br>`{ "name": "Pakaian Wanita", "description": "Koleksi atasan, dress, dan bawahan wanita" }` | **201 Created**<br>`{ "categoryId": "guid", "name": "Pakaian Wanita" }`<br>Error: 409 Category Name Already Exists |
| **ADM-06** | Lihat Semua Pesanan (Global) | `GET /api/v1/admin/orders` | Admin | **Query Params:**<br>`pageNumber=1&pageSize=20&status=Processing&startDate=2026-10-01&endDate=2026-10-15` | **200 OK**<br>`PagedResult<AdminOrderSummaryResponse>`<br>`{ "items": [ { "orderId": "guid", "customerName": "Rina Suryani", "totalAmount": 267000, "status": "Processing", "itemCount": 3, "createdAt": "..." } ] }` |
| **ADM-07** | Ubah Status Pesanan | `PATCH /api/v1/admin/orders/{id}/status` | Admin | **Path:** `id` (Guid)<br>**Body (JSON):**<br>`{ "status": "Shipped", "trackingNumber": "JNE-8839102", "notes": "Diserahkan ke kurir JNE Bandung" }` | **200 OK**<br>`{ "orderId": "guid", "previousStatus": "Processing", "newStatus": "Shipped", "updatedAt": "..." }`<br>Error: 400 Invalid Transition (e.g. Cancelled to Shipped), 404 Not Found |
| **ADM-08** | Kelola Pengguna & Peran | `PATCH /api/v1/admin/users/{id}` | Admin | **Path:** `id` (Guid - UserId)<br>**Body (JSON):**<br>`{ "role": "Admin", "isActive": false }` | **200 OK**<br>`{ "userId": "guid", "role": "Admin", "isActive": false, "message": "Status pengguna diperbarui." }`<br>Error: 400 Cannot Deactivate Own Account, 404 Not Found |

---

### 2.3 Endpoint Pembayaran & Webhook Sistem (Opsional Transaksional)

| Endpoint | Method | Role | Payload | Keterangan |
| :--- | :--- | :--- | :--- | :--- |
| `/api/v1/payments/process` | `POST` | Customer | `{ "orderId": "guid", "paymentMethod": "VirtualAccount" }` | Menginisiasi nomor VA simulasi untuk pelunasan pesanan. Status order menjadi `AwaitingPayment`. |
| `/api/v1/payments/webhook` | `POST` | System/Public | `{ "orderId": "guid", "status": "Settled", "signature": "sha256..." }` | Notifikasi instan pembayaran; mengubah status pembayaran jadi `Completed` dan order ke `Processing`. |

---

## 3. Anti-AI Slop Governance & Copywriting Guide

Platform Rinazs menerapkan standar penulisan tanpa basa-basi (*anti-AI slop*) berdasarkan pedoman `anti-ai-slop-writing` dan `humanizer`. Seluruh seed data, salinan katalog, dan microcopy antarmuka harus mematuhi aturan berikut.

### 3.1 Daftar Kata & Frasa Terlarang (Banned AI Vocabulary)
Dilarang keras memakai istilah berikut dalam UI, pesan notifikasi, maupun deskripsi produk:
- **Kata Basi AI:** *delve, tapestry, landscape, pivotal, vibrant, elevate, unleash, seamless, robust, supercharge, empower, intricate, meticulously, multifaceted, groundbreaking, revolutionary, testament, game-changing, synergy.*
- **Frasa Pengisi Basi:** *"In today's modern world", "It's not just about X, it's about Y", "Take your style to the next level", "Unlock the true power", "Whether you're a X or a Y", "Buckle up", "At the end of the day", "In a nutshell".*
- **Sapaan Robotik:** *"Great question!", "Certainly!", "I'd be happy to help", "Oops! Something went wrong in our universe".*

---

### 3.2 Aturan Deskripsi Katalog Produk (Seed Data)
Produk fashion dan teknologi harus dijelaskan dengan spesifikasi fisik riil: bahan, kerapatan tenun, ukuran, port, daya listrik, dan instruksi perawatan tanpa hiperbola promosi.

#### Contoh Seed Produk 1: Fashion / Pakaian Pria
- **Nama Produk:** Kaos Polos Katun Combed 24s Regular Fit
- **Kategori:** Pakaian Pria
- **Harga:** Rp 89.000 (Stok: 65)
- **Deskripsi Anti-Slop:**
  > Kaos polos bahan 100% katun combed 24s dengan gramasi kain 180-190 gsm. Jahitan pundak menggunakan rantai ganda, rib leher tidak mudah melar setelah pencucian berulang. Potongan regular fit cocok dipakai harian atau lapisan dalam kemeja.
  >
  > Panduan Ukuran (Lebar Dada x Panjang Baju):
  > - S: 48 cm x 68 cm
  > - M: 50 cm x 70 cm
  > - L: 52 cm x 72 cm
  > - XL: 55 cm x 75 cm
  >
  > Perawatan: Cuci mesin air dingin, jangan gunakan pemutih pakaian klorin, jemur terbalik.

#### Contoh Seed Produk 2: Fashion / Outerwear
- **Nama Produk:** Jaket Parka Canvas Drill Tahan Angin
- **Kategori:** Jaket & Outerwear
- **Harga:** Rp 245.000 (Stok: 24)
- **Deskripsi Anti-Slop:**
  > Jaket parka berbahan canvas drill tebal dilapisi furing katun kotak-kotak di bagian dalam. Dilengkapi penutup resleting logam YKK, kancing tekan kuningan, saku luar 4 buah, dan 1 saku dalam untuk dompet. Tali serut di pinggang dan tudung kepala bisa diatur sesuai kebutuhan.

#### Contoh Seed Produk 3: Tech & Aksesoris
- **Nama Produk:** Mechanical Keyboard TKL 87 Tombol Hot-Swap
- **Kategori:** Tech & Gadget
- **Harga:** Rp 420.000 (Stok: 15)
- **Deskripsi Anti-Slop:**
  > Keyboard mekanikal format Tenkeyless (TKL) tanpa numpad dengan koneksi kabel USB-C ke USB-A lepas-pasang (panjang kabel 1.6 meter). Socket PCB hot-swappable 3-pin dan 5-pin, kompatibel dengan switch Outemu, Gateron, dan Cherry MX. Keycaps berbahan plastik PBT doubleshot tahan kilap. Lampu latar LED putih polos dengan 12 mode kecerahan.
  >
  > Paket penjualan: unit keyboard, kabel braided USB-C, keycap puller, switch puller, dan 4 switch cadangan.

#### Contoh Seed Produk 4: Tech / Charging Gear
- **Nama Produk:** Kepala Charger GaN 65W Dual USB-C & USB-A
- **Kategori:** Tech & Gadget
- **Harga:** Rp 175.000 (Stok: 40)
- **Deskripsi Anti-Slop:**
  > Charger dinding teknologi Gallium Nitride (GaN) ukuran 52 x 31 x 31 mm dengan bobot 98 gram. Mendukung protokol Power Delivery (PD 3.0), Quick Charge (QC 4.0), dan PPS 45W untuk pengisian cepat Samsung, iPhone, iPad, serta laptop hingga 65 Watt. Steker tipe EU standar stopkontak Indonesia.
  >
  > Pembagian daya:
  > - Port USB-C tunggal: maksimal 65W
  > - Port USB-C + USB-A bersamaan: USB-C 45W + USB-A 18W

---

### 3.3 Standar Microcopy Antarmuka (UI Copywriting)

| Komponen UI | Jangan Gunakan (AI Slop) | Gunakan Ini (Tegas & Jelas) |
| :--- | :--- | :--- |
| **Tombol Aksi Utama** | *Unleash Style Anda!*<br>*Mulai Petualangan Belanja* | **Beli Sekarang**<br>**Tambah ke Keranjang** |
| **Tombol Checkout** | *Wujudkan Impian Anda* | **Lanjut ke Pembayaran** |
| **Alert Sukses Simpan** | *Luar biasa! Profil Anda telah berevolusi.* | **Perubahan profil berhasil disimpan.** |
| **Alert Sukses Pesanan** | *Selamat! Perjalanan belanja baru saja dimulai.* | **Pesanan #RNZ-20261015 dibuat. Menunggu konfirmasi pembayaran.** |
| **Empty State Keranjang** | *Wah, keranjang Anda kesepian di hamparan belanja ini.* | **Keranjang belanja masih kosong.**<br>Pilih produk dari katalog untuk mulai belanja. |
| **Empty State Pesanan** | *Belum ada jejak transaksi dalam lembaran riwayat Anda.* | **Belum ada riwayat transaksi.**<br>Pesanan yang Anda buat akan tercatat di sini. |
| **Error Validasi Form** | *Upsie! Sepertinya ada energi negatif di formulir ini.* | **Email tidak valid.** Masukkan alamat dengan format nama@domain.com. |
| **Error Stok Habis** | *Produk ini sangat diminati oleh semesta sehingga lenyap.* | **Stok tidak mencukupi.** Tersisa 2 barang di gudang. |
| **Error 404 Halaman** | *Anda tersesat di labirin semesta digital Rinazs.* | **Halaman tidak ditemukan.** Alamat tautan mungkin salah atau produk telah dihapus. |
| **Error 500 Server** | *Sistem kami sedang menyelaraskan frekuensi internal.* | **Layanan sedang mengalami gangguan teknis.** Silakan coba kembali dalam beberapa menit. |

---

## 4. Rencana Kerja TDD Berjenjang (6 Tahap)

Penerapan mengikuti prinsip siklus **RED-GREEN-REFACTOR**:
1. **RED:** Tulis unit test atau integration test terlebih dahulu, jalankan, dan pastikan test *gagal* sesuai skenario yang diinginkan.
2. **GREEN:** Tulis implementasi minimal hanya untuk membuat test berhasil.
3. **REFACTOR:** Rapikan struktur kode, hilangkan duplikasi, pastikan semua pengujian tetap *hijau*.

### Ringkasan Alur Kerja

```
[Tahap 1: Migration & Seed]
   │ (PostgreSQL, Initial Schema, Anti-Slop Seed Data)
   ▼
[Tahap 2: Cart & Order Endpoints]
   │ (TDD AddToCart, UpdateQty, RemoveItem, Checkout, Stock Deduction)
   ▼
[Tahap 3: Category & Profile Endpoints]
   │ (TDD Category CRUD, Profile Read/Update, Delivery Address)
   ▼
[Tahap 4: Admin Endpoints]
   │ (TDD Product Catalog Admin, Restock Engine, Order Status Machine, User Ban)
   ▼
[Tahap 5: Setup & Build Frontend Next.js]
   │ (Next.js App Router, Tailwind CSS, TanStack Query, Anti-Slop UI)
   ▼
[Tahap 6: Verification & End-to-End Test]
   │ (Full Flow Automation, Playwright/Integration, Smoke Test)
   ▼
[PRODUKSI / RELEASE SIAP PAKAI]
```

---

### Tahap 1: Migration & EF Core Seed Data
- **Fokus Pekerjaan:**
  1. Menyiapkan database PostgreSQL lokal / Neon DB via ConnectionString `appsettings.json`.
  2. Menjalankan `dotnet ef migrations add InitialMigration -p RinaZsECommerce.Infrastructure -s RinaZsECommerce.API`.
  3. Menguji relasi foreign key `User` (Id) ke `UserProfile` (UserId) agar konsisten 1-to-1.
  4. Menulis class seeder `DatabaseSeeder.cs` di Infrastructure:
     - Kategori: Pakaian Pria, Pakaian Wanita, Jaket & Outerwear, Tech & Gadget, Aksesoris Harian.
     - Supplier: PT Rina Tekstil Nusantara, CV Digital Kreasi Bandung.
     - Produk Seed: 8 produk fashion & teknologi dengan deskripsi anti-slop lengkap (ukuran, bahan, port, garansi).
     - Akun Default: 1 Admin (`admin@rinazs.com` / `AdminPass123!`) dan 1 Customer tester (`tester@rinazs.com` / `TesterPass123!`).
- **Verifikasi TDD:**
  - `DatabaseSeederTests.cs`: Memastikan jika database kosong, seeder mengisi jumlah entitas secara tepat dan relasi User-UserProfile terhubung tanpa exception.

---

### Tahap 2: Cart & Order Endpoints (TDD Core Business)
- **Siklus Uji TDD:**
  1. **Ubah Kuantitas Item Keranjang (UC-08):**
     - *RED:* Buat `UpdateCartItemHandlerTests.cs`. Skenario: Kuantitas melebihi sisa stok produk -> harus melempar `InsufficientStockException`.
     - *GREEN:* Implementasikan `UpdateCartItemCommand`, `UpdateCartItemValidator`, dan `UpdateCartItemHandler`.
     - *REFACTOR:* Optimasi query pengecekan stok dengan tracking entity.
  2. **Hapus Item Keranjang (UC-09):**
     - *RED:* Buat test `RemoveCartItemHandlerTests.cs` (hapus item yang bukan milik profil user -> melempar `UnauthorizedAccessException` atau `NotFoundException`).
     - *GREEN:* Implementasikan `RemoveCartItemCommand` dan `RemoveCartItemHandler`.
  3. **Checkout Order & Pengurangan Stok (UC-10):**
     - *RED:* Buat `CheckoutHandlerTests.cs`. Skenario:
       - Keranjang kosong -> melempar `EmptyCartException`.
       - Total order harus sama persis dengan akumulasi `Price * Quantity`.
       - Setelah order dibuat, stok produk harus berkurang dan item di keranjang terhapus.
     - *GREEN:* Perbaiki `CheckoutHandler` yang sudah ada agar terhubung ke pengurangan stok dan pencatatan detail order secara atomik via `IUnitOfWork.CompleteAsync()`.
     - *REFACTOR:* Pindahkan kalkulasi subtotal ke Value Object atau Domain Entity Method `Order.CreateFromCart()`.
  4. **Riwayat & Detail Pesanan (UC-11 & UC-12):**
     - *RED:* Buat `GetOrdersHandlerTests.cs` dan `GetOrderByIdHandlerTests.cs` dengan verifikasi isolasi data (pelanggan A tidak bisa melihat pesanan pelanggan B).
     - *GREEN:* Implementasikan query handler dengan `ApplyFilter` pada `OrderRepository`.
  5. **Batalkan Pesanan (UC-13):**
     - *RED:* Buat `CancelOrderHandlerTests.cs`. Jika pesanan sudah berstatus `Shipped`, pembatalan ditolak. Jika masih `Pending`, status berubah menjadi `Cancelled` dan stok produk otomatis direstorasi.
     - *GREEN:* Implementasikan `CancelOrderCommand` dan handler.

---

### Tahap 3: Category & Profile Endpoints
- **Siklus Uji TDD:**
  1. **Kategori Publik (UC-03 Support):**
     - *RED:* `GetAllCategoriesQueryTests.cs` menguji pengambilan seluruh kategori aktif.
     - *GREEN:* Implementasikan `GetAllCategoriesQuery` dan handler.
  2. **Lihat & Ubah Profil (UC-14 & UC-15):**
     - *RED:* `GetUserProfileHandlerTests.cs` & `UpdateUserProfileHandlerTests.cs`.
       - Test validasi format nomor telepon Indonesia (harus diawali 08 atau +62).
       - Test pembaruan field nama depan, nama belakang, dan alamat pengiriman tanpa merusak entitas `User`.
     - *GREEN:* Implementasikan `UpdateUserProfileCommand`, `UpdateUserProfileValidator`, dan `UpdateUserProfileHandler`.
     - *REFACTOR:* Pastikan entitas `User` dan `UserProfile` dipanggil lewat `IUserRepository` dengan eager loading yang efisien.

---

### Tahap 4: Admin Endpoints
- **Siklus Uji TDD:**
  1. **Katalog Produk Admin (ADM-01, ADM-02, ADM-03):**
     - *RED:* `CreateProductHandlerTests.cs` (kategori tidak ditemukan melempar `NotFoundException`), `DeleteProductHandlerTests.cs` (soft delete menguji `DeletedAt != null`).
     - *GREEN:* Lengkapi handler `CreateProductHandler`, `UpdateProductHandler`, `DeleteProductHandler`.
  2. **Manajemen Stok Gudang (ADM-04):**
     - *RED:* `AdjustStockHandlerTests.cs` menguji pencatatan historis di tabel `StockLog` setiap kali admin menambah stok restock atau penyesuaian manual.
     - *GREEN:* Implementasikan `AdjustStockCommand` dan integrasikan ke `IProductRepository.UpdateStockAsync()`.
  3. **Manajemen Pesanan Global & Transisi Status (ADM-06 & ADM-07):**
     - *RED:* `UpdateOrderStatusHandlerTests.cs` menguji state machine status pesanan:
       - `Pending` -> `Processing` (Valid)
       - `Processing` -> `Shipped` (Wajib sertakan resi)
       - `Cancelled` -> `Shipped` (Harus InvalidOperationException)
     - *GREEN:* Implementasikan `UpdateOrderStatusCommand` dan handler.
  4. **Kelola Pengguna (ADM-08):**
     - *RED:* `UpdateUserStatusHandlerTests.cs` menguji admin tidak dapat menonaktifkan akun miliknya sendiri.
     - *GREEN:* Implementasikan handler dan tambahkan pengecekan `CurrentUserId != TargetUserId`.

---

### Tahap 5: Setup & Build Frontend Next.js
- **Spesifikasi Frontend:**
  - **Framework:** Next.js 15 (App Router), TypeScript, Tailwind CSS.
  - **State Management & Fetching:** TanStack Query (React Query) v5 + Zustand untuk keranjang belanja lokal.
  - **Form Validation:** React Hook Form + Zod.
  - **Arsitektur Halaman:**
    - `app/(shop)/page.tsx`: Landing page katalog dengan banner produk unggulan dan grid filter cepat.
    - `app/(shop)/products/[id]/page.tsx`: Detail produk, galeri gambar thumbnail, pemilih jumlah, spesifikasi fisik, dan tombol *Tambah ke Keranjang*.
    - `app/(shop)/cart/page.tsx`: Drawer / Halaman keranjang belanja dengan ringkasan subtotal instan.
    - `app/(shop)/checkout/page.tsx`: Formulir alamat pengiriman, opsi pembayaran, dan konfirmasi pesanan.
    - `app/(shop)/orders/page.tsx` & `[id]/page.tsx`: Riwayat transaksi pelanggan dan detail status pelacakan.
    - `app/admin/...`: Dashboard ringkasan stok, tabel pesanan masuk, form tambah/edit produk, dan log perubahan stok.
- **Penerapan Anti-AI Slop:**
  - Seluruh microcopy tombol, status pesanan, notifikasi toast, dan empty state menggunakan kamus copywriter di Bagian 3.
  - Komponen Toast menggunakan teks lugas: "Barang ditambahkan ke keranjang" bukan "Hore! Produk impian Anda siap dibawa pulang".

---

### Tahap 6: Verification & End-to-End Test
- **Rencana Pengujian:**
  1. **Suite Unit & Integration Test Backend:**
     - Menjalankan `dotnet test` pada seluruh project test (`tests/RinaZsECommerce.UnitTests`).
     - Target code coverage: >85% pada folder `Application/Features`.
  2. **E2E Testing (Playwright / Integration Scenario):**
     - Flow 1: Registrasi -> Login -> Jelajah Katalog -> Masukkan Keranjang -> Ubah Kuantitas -> Checkout -> Periksa Pesanan.
     - Flow 2: Admin Login -> Tambah Produk Baru -> Restock Barang -> Pelanggan Membeli -> Admin Mengubah Status ke `Shipped`.
     - Flow 3: Pembatalan pesanan sebelum dikirim -> verifikasi stok gudang otomatis kembali.
  3. **Audit Konten Anti-AI Slop:**
     - Skrip linter teks otomatis memindai seluruh file json seed, database migration, dan file `.tsx` frontend untuk memastikan 0 kemunculan kata-kata terlarang (*delve, tapestry, landscape, elevate, unleash*, dsb).

---

## 5. Ringkasan Kesiapan & Langkah Lanjutan

| Modul | Status Saat Ini | Kebutuhan Implementasi Berikutnya |
| :--- | :--- | :--- |
| **Domain Entities** | User & UserProfile sudah terpisah; entity CartItem, Order, Product sudah ada | Tambahkan factory method domain pada `Order` untuk pembatalan & restock |
| **Persistence (EF Core)** | AppDbContext & Configurations sudah ada | Buat migration baru PostgreSQL & `DatabaseSeeder` anti-slop |
| **Application Handlers** | Sebagian handler auth, cart, order sudah ada | Tambahkan validator FluentValidation, Admin command handlers, Category query |
| **API Controllers** | Auth, Carts, Orders, Products dasar tersedia | Lengkapi endpoint Admin, Profile, dan Category sesuai Use Case Matrix |
| **Frontend Next.js** | Belum diinisiasi | Inisiasi proyek Next.js dengan Tailwind CSS dan modul antarmuka anti-slop |
