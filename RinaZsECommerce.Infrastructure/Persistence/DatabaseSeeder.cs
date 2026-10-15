using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RinaZsECommerce.Domain.Entities;

namespace RinaZsECommerce.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.MigrateAsync();

        // 1. Seed Suppliers
        if (!await context.Suppliers.AnyAsync())
        {
            var supplierTekstil = new Supplier
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "PT Rina Jaya Tekstil",
                Description = "Penyedia bahan katun combed OEKO-TEX, kain drill, dan konveksi garmen lokal Surakarta."
            };

            var supplierHardware = new Supplier
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "PT Sinar Digital Nusantara",
                Description = "Distributor periferal komputer, switch mekanikal, kabel braided, dan adaptor daya bersertifikat SNI."
            };

            await context.Suppliers.AddRangeAsync(supplierTekstil, supplierHardware);
            await context.SaveChangesAsync();
        }

        // 2. Seed Categories
        if (!await context.Categories.AnyAsync())
        {
            var catApparel = new Category
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Name = "Apparel & Daily Wear",
                Description = "Pakaian kasual pria dan wanita dengan standar gramasi benang presisi dan daya tahan tinggi."
            };

            var catKeyboards = new Category
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Name = "Mechanical Keyboards & Tech",
                Description = "Papan ketik mekanikal hot-swap, keycaps PBT tahan pudar, dan aksesori meja kerja produktif."
            };

            var catPower = new Category
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Name = "Hardware & Power Accessories",
                Description = "Kepala charger Gallium Nitride (GaN), kabel berdaya 100W PD, dan adaptor transmisi data cepat."
            };

            await context.Categories.AddRangeAsync(catApparel, catKeyboards, catPower);
            await context.SaveChangesAsync();
        }

        // 3. Seed Users (Admin & Customer)
        var hasher = new PasswordHasher<User>();
        if (!await context.Users.AnyAsync(u => u.Email == "admin@rinazs.local"))
        {
            var adminId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var adminProfileId = Guid.Parse("00000000-0000-0000-0000-000000000002");

            var adminUser = new User
            {
                Id = adminId,
                Username = "admin",
                Email = "admin@rinazs.local",
                Role = "Admin",
                ProfileId = adminProfileId,
                PasswordHash = ""
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "AdminPass123!");

            var adminProfile = new UserProfile
            {
                Id = adminProfileId,
                UserId = adminId,
                FirstName = "Admin",
                LastName = "RinaZs",
                Address = "Gudang Pusat RinaZs, Jebres, Surakarta",
                PhoneNumber = "08122334455",
                Verified = true
            };

            adminUser.UserProfile = adminProfile;
            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();
        }

        if (!await context.Users.AnyAsync(u => u.Email == "customer@rinazs.local"))
        {
            var custId = Guid.Parse("00000000-0000-0000-0000-000000000003");
            var custProfileId = Guid.Parse("00000000-0000-0000-0000-000000000004");

            var custUser = new User
            {
                Id = custId,
                Username = "customer",
                Email = "customer@rinazs.local",
                Role = "Customer",
                ProfileId = custProfileId,
                PasswordHash = ""
            };
            custUser.PasswordHash = hasher.HashPassword(custUser, "CustomerPass123!");

            var custProfile = new UserProfile
            {
                Id = custProfileId,
                UserId = custId,
                FirstName = "Budi",
                LastName = "Santoso",
                Address = "Jl. Slamet Riyadi No. 120, Surakarta",
                PhoneNumber = "081234567890",
                Verified = true
            };

            custUser.UserProfile = custProfile;
            await context.Users.AddAsync(custUser);
            await context.SaveChangesAsync();
        }

        // 4. Seed Products (Anti-AI Slop: data faktual fisik)
        if (!await context.Products.AnyAsync())
        {
            var catApparelId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var catKeyboardsId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
            var catPowerId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

            var supTekstilId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var supDigitalId = Guid.Parse("22222222-2222-2222-2222-222222222222");

            var p1Id = Guid.NewGuid();
            var p2Id = Guid.NewGuid();
            var p3Id = Guid.NewGuid();
            var p4Id = Guid.NewGuid();
            var p5Id = Guid.NewGuid();
            var p6Id = Guid.NewGuid();
            var p7Id = Guid.NewGuid();
            var p8Id = Guid.NewGuid();

            var products = new List<Product>
            {
                new Product
                {
                    Id = p1Id,
                    Name = "Kaos Polos Heavyweight Combed 24s",
                    Description = "Bahan 100% cotton combed 24s gramasi 190 gsm. Pola regular fit dengan jahitan rantai ganda pada pundak dan rib leher katun spandeks elastis 2.5 cm. Tidak menyusut setelah pencucian pertama.",
                    Price = 95000m,
                    Stock = 50,
                    Rating = 4.8m,
                    Status = true,
                    CategoryId = catApparelId,
                    SupplierId = supTekstilId,
                    Images = new List<ProductImage>
                    {
                        new ProductImage { ProductId = p1Id, ImageUrl = "https://images.unsplash.com/photo-1521572267360-ee0c2909d518?w=800&q=80", ImageAltDesc = "Kaos Polos Heavyweight Combed 24s Hitam" }
                    }
                },
                new Product
                {
                    Id = p2Id,
                    Name = "Parka Field Jacket Kanvas Drill",
                    Description = "Kain kanvas drill katun tebal tahan angin dengan furing dalam furing asahi halus. Memiliki 4 saku kargo depan berkancing snap kuningan dan ritsleting logam YKK No. 5. Tali serut di pinggang.",
                    Price = 285000m,
                    Stock = 25,
                    Rating = 4.9m,
                    Status = true,
                    CategoryId = catApparelId,
                    SupplierId = supTekstilId,
                    Images = new List<ProductImage>
                    {
                        new ProductImage { ProductId = p2Id, ImageUrl = "https://images.unsplash.com/photo-1544923246-77307dd654cb?w=800&q=80", ImageAltDesc = "Parka Field Jacket Kanvas Drill Hijau Army" }
                    }
                },
                new Product
                {
                    Id = p3Id,
                    Name = "Celana Chino Slim Straight Cotton Twill",
                    Description = "Katun stretch twill 98% katun 2% elastane berat kain 11.5 oz. Potongan slim straight dengan saku samping miring dan saku belakang paspoal ganda. Kancing pengunci bartack pada titik beban tegangan tinggi.",
                    Price = 175000m,
                    Stock = 40,
                    Rating = 4.7m,
                    Status = true,
                    CategoryId = catApparelId,
                    SupplierId = supTekstilId,
                    Images = new List<ProductImage>
                    {
                        new ProductImage { ProductId = p3Id, ImageUrl = "https://images.unsplash.com/photo-1473966968600-fa801b869a1a?w=800&q=80", ImageAltDesc = "Celana Chino Slim Straight Khaki" }
                    }
                },
                new Product
                {
                    Id = p4Id,
                    Name = "Keyboard Mekanikal TKL 87-Key Hot-Swap RGB",
                    Description = "Tata letak Tenkeyless 87 tombol dengan soket 5-pin hot-swappable universal. Pre-installed Linear Red Switch 45g actuation force. Plate mounted stabilizer pre-lubed Krytox GPL 205g0 dengan peredam busa EVA ganda. Koneksi kabel Type-C detachable.",
                    Price = 549000m,
                    Stock = 30,
                    Rating = 4.9m,
                    Status = true,
                    CategoryId = catKeyboardsId,
                    SupplierId = supDigitalId,
                    Images = new List<ProductImage>
                    {
                        new ProductImage { ProductId = p4Id, ImageUrl = "https://images.unsplash.com/photo-1587829741301-dc798b83add3?w=800&q=80", ImageAltDesc = "Keyboard Mekanikal TKL 87-Key Hot-Swap RGB" }
                    }
                },
                new Product
                {
                    Id = p5Id,
                    Name = "Keycaps PBT Dye-Sub Cherry Profile 128-Key",
                    Description = "PBT murni ketebalan dinding 1.5 mm dengan cetakan legenda sublimasi termal tahan minyak dan anti-pudar. Profil Cherry ergonomis kompatibel untuk layout ANSI 60%, 65%, 75%, TKL, hingga Full Size.",
                    Price = 189000m,
                    Stock = 35,
                    Rating = 4.8m,
                    Status = true,
                    CategoryId = catKeyboardsId,
                    SupplierId = supDigitalId,
                    Images = new List<ProductImage>
                    {
                        new ProductImage { ProductId = p5Id, ImageUrl = "https://images.unsplash.com/photo-1595225476474-87563907a212?w=800&q=80", ImageAltDesc = "Keycaps PBT Dye-Sub Cherry Profile Minimalis" }
                    }
                },
                new Product
                {
                    Id = p6Id,
                    Name = "Deskmat Cordura Speed-Control 900x400mm",
                    Description = "Permukaan anyaman Cordura tahan percikan air (water-repellent) ukuran 900 x 400 x 4 mm. Alas karet anti-slip bertekstur herringbone dengan pinggiran jahitan mikro obras rapat tahan robek.",
                    Price = 135000m,
                    Stock = 60,
                    Rating = 4.9m,
                    Status = true,
                    CategoryId = catKeyboardsId,
                    SupplierId = supDigitalId,
                    Images = new List<ProductImage>
                    {
                        new ProductImage { ProductId = p6Id, ImageUrl = "https://images.unsplash.com/photo-1616440347437-b1c73416efc2?w=800&q=80", ImageAltDesc = "Deskmat Cordura 900x400mm Hitam Polos" }
                    }
                },
                new Product
                {
                    Id = p7Id,
                    Name = "Charger GaN 65W Dual USB-C + USB-A",
                    Description = "Teknologi Gallium Nitride generasi III dengan output daya maksimal 65W. Mendukung protokol Power Delivery 3.0, Quick Charge 4+, dan PPS. Port ganda Type-C dan satu USB-A dengan proteksi termal otomatis.",
                    Price = 220000m,
                    Stock = 45,
                    Rating = 4.9m,
                    Status = true,
                    CategoryId = catPowerId,
                    SupplierId = supDigitalId,
                    Images = new List<ProductImage>
                    {
                        new ProductImage { ProductId = p7Id, ImageUrl = "https://images.unsplash.com/photo-1583863788434-e58a36330cf0?w=800&q=80", ImageAltDesc = "Charger GaN 65W Fast Charging Kompak" }
                    }
                },
                new Product
                {
                    Id = p8Id,
                    Name = "Kabel Braided USB-C to C 100W PD E-Marker 2 Meter",
                    Description = "Kabel pengisian daya cepat 100W (20V/5A) dilengkapi chip E-Marker aktif untuk manajemen arus aman. Lapisan pelindung anyaman nilon rapat tahan tekuk 20.000 kali dengan cangkang konektor paduan aluminium.",
                    Price = 45000m,
                    Stock = 80,
                    Rating = 4.8m,
                    Status = true,
                    CategoryId = catPowerId,
                    SupplierId = supDigitalId,
                    Images = new List<ProductImage>
                    {
                        new ProductImage { ProductId = p8Id, ImageUrl = "https://images.unsplash.com/photo-1544716278-ca5e3f4abd8c?w=800&q=80", ImageAltDesc = "Kabel Braided USB-C to C 100W Hitam" }
                    }
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}
