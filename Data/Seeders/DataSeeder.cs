using CuaHangBangDiaNhac.Data;
using CuaHangBangDiaNhac.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CuaHangBangDiaNhac.Data.Seeders;

public class DataSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        var customerRole = await userManager.GetUsersInRoleAsync("Customer");
        if (!customerRole.Any())
        {
            var customers = new List<User>
            {
                new() { UserName = "customer1@gmail.com", Email = "customer1@gmail.com", FullName = "Nguyễn Văn An", DateOfBirth = new DateTime(1995, 5, 10), EmailConfirmed = true },
                new() { UserName = "customer2@gmail.com", Email = "customer2@gmail.com", FullName = "Trần Thị Bình", DateOfBirth = new DateTime(1998, 11, 25), EmailConfirmed = true },
                new() { UserName = "customer3@gmail.com", Email = "customer3@gmail.com", FullName = "Lê Văn Cường", DateOfBirth = new DateTime(1990, 2, 18), EmailConfirmed = true }
            };

            foreach (var user in customers)
            {
                await userManager.CreateAsync(user, "Customer@123");
                await userManager.AddToRoleAsync(user, "Customer");
            }
        }

        if (!context.Genres.Any())
        {
            var genres = new List<Genre>
            {
                new Genre { Name = "Rock", Styles = new List<Style>
                    {
                        new Style { Name = "Classic Rock" }, new Style { Name = "Grunge" }, new Style { Name = "Alternative Rock" }, new Style { Name = "Pop Rock" },
                        new Style { Name = "Thrash Metal" }, new Style { Name = "Nu Metal" }, new Style { Name = "Indie Rock" }, new Style { Name = "Soft Rock" }
                    }
                },
                new Genre { Name = "Pop", Styles = new List<Style>
                    {
                        new Style { Name = "V-Pop" }, new Style { Name = "Indie Pop" }
                    }
                },
                new Genre { Name = "Folk & Country", Styles = new List<Style>
                    {
                        new Style { Name = "Folk Rock" }, new Style { Name = "Contemporary Folk" }
                    }
                },
                new Genre { Name = "Jazz", Styles = new List<Style>
                    {
                        new Style { Name = "Vocal Jazz" }, new Style { Name = "Jazz Fusion" }
                    }
                },
                new Genre { Name = "Blues", Styles = new List<Style> { new Style { Name = "Blues Rock" } } }
            };
            await context.Genres.AddRangeAsync(genres);
            await context.SaveChangesAsync();
        }

        if (!context.Brands.Any())
        {
            var brands = new List<Brand>
            {
                new Brand { Name = "Indie / Tự phát hành" },
                new Brand { Name = "LP Club" },
                new Brand { Name = "Hãng Đĩa Thời Đại" },
                new Brand { Name = "Warner Music Vietnam" },
                new Brand { Name = "Apple Records" },
                new Brand { Name = "Geffen Records" },
                new Brand { Name = "Blackened Recordings" },
                new Brand { Name = "Warner Records" },
                new Brand { Name = "XL Recordings" },
                new Brand { Name = "Rocket Recordings" },
                new Brand { Name = "Reprise Records" },
                new Brand { Name = "Parlophone" },
                new Brand { Name = "DJM Records" }
            };
            await context.Brands.AddRangeAsync(brands);
            await context.SaveChangesAsync();
        }

        if (!context.Categories.Any())
        {
            await context.Categories.AddRangeAsync(
                new Category { Name = "Vinyl (Đĩa than)" },
                new Category { Name = "CD" },
                new Category { Name = "Cassette" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Artists.Any())
        {
            var artists = new List<Artist>
            {
                new Artist { Name = "Ngọt" }, new Artist { Name = "Cá Hồi Hoang" }, new Artist { Name = "Chillies" },
                new Artist { Name = "The Cassette" }, new Artist { Name = "Lý Bực" }, new Artist { Name = "The Flob, CHIN" },
                new Artist { Name = "Vũ." }, new Artist { Name = "Hoàng Dũng" },
                new Artist { Name = "Thắng" }, new Artist { Name = "Nirvana" }, new Artist { Name = "The Beatles" },
                new Artist { Name = "Radiohead" }, new Artist { Name = "Metallica" }, new Artist { Name = "Linkin Park" },
                new Artist { Name = "John Lennon" }, new Artist { Name = "Elton John" }, new Artist { Name = "Neil Young" }
            };
            await context.Artists.AddRangeAsync(artists);
            await context.SaveChangesAsync();
        }

        if (!context.Products.Any())
        {
            var vinyl = await context.Categories.FirstAsync(c => c.Name.Contains("Vinyl"));
            var cd = await context.Categories.FirstAsync(c => c.Name == "CD");
            var cassette = await context.Categories.FirstAsync(c => c.Name == "Cassette");

            var rock = await context.Genres.FirstAsync(g => g.Name == "Rock");
            var pop = await context.Genres.FirstAsync(g => g.Name == "Pop");
            var folkCountry = await context.Genres.FirstAsync(g => g.Name == "Folk & Country");


            var sGrunge = await context.Styles.FirstAsync(s => s.Name == "Grunge");
            var sAltRock = await context.Styles.FirstAsync(s => s.Name == "Alternative Rock");
            var sThrash = await context.Styles.FirstAsync(s => s.Name == "Thrash Metal");
            var sVPop = await context.Styles.FirstAsync(s => s.Name == "V-Pop");
            var sIndiePop = await context.Styles.FirstAsync(s => s.Name == "Indie Pop");
            var sClassicRock = await context.Styles.FirstAsync(s => s.Name == "Classic Rock");
            var sNuMetal = await context.Styles.FirstAsync(s => s.Name == "Nu Metal");
            var sIndieRock = await context.Styles.FirstAsync(s => s.Name == "Indie Rock");
            var sPopRock = await context.Styles.FirstAsync(s => s.Name == "Pop Rock");
            var sFolkRock = await context.Styles.FirstAsync(s => s.Name == "Folk Rock");

            var lpClubBrand = await context.Brands.FirstAsync(b => b.Name == "LP Club");
            var indieBrand = await context.Brands.FirstAsync(b => b.Name == "Indie / Tự phát hành");
            var timesRecords = await context.Brands.FirstAsync(b => b.Name == "Hãng Đĩa Thời Đại");
            var warnerVN = await context.Brands.FirstAsync(b => b.Name == "Warner Music Vietnam");
            var appleRec = await context.Brands.FirstAsync(b => b.Name == "Apple Records");
            var geffen = await context.Brands.FirstAsync(b => b.Name == "Geffen Records");
            var blackened = await context.Brands.FirstAsync(b => b.Name == "Blackened Recordings");
            var warnerUs = await context.Brands.FirstAsync(b => b.Name == "Warner Records");
            var reprise = await context.Brands.FirstAsync(b => b.Name == "Reprise Records");
            var djm = await context.Brands.FirstAsync(b => b.Name == "DJM Records");
            var parlophone = await context.Brands.FirstAsync(b => b.Name == "Parlophone");

            var nirvana = await context.Artists.FirstAsync(a => a.Name == "Nirvana");
            var ngot = await context.Artists.FirstAsync(a => a.Name == "Ngọt");
            var metallica = await context.Artists.FirstAsync(a => a.Name == "Metallica");
            var linkinPark = await context.Artists.FirstAsync(a => a.Name == "Linkin Park");
            var beatles = await context.Artists.FirstAsync(a => a.Name == "The Beatles");
            var vu = await context.Artists.FirstAsync(a => a.Name == "Vũ.");
            var chillies = await context.Artists.FirstAsync(a => a.Name == "Chillies");
            var theCassette = await context.Artists.FirstAsync(a => a.Name == "The Cassette");
            var caHoiHoang = await context.Artists.FirstAsync(a => a.Name == "Cá Hồi Hoang");
            var radiohead = await context.Artists.FirstAsync(a => a.Name == "Radiohead");
            var johnLennon = await context.Artists.FirstAsync(a => a.Name == "John Lennon");
            var eltonJohn = await context.Artists.FirstAsync(a => a.Name == "Elton John");
            var neilYoung = await context.Artists.FirstAsync(a => a.Name == "Neil Young");

            var products = new List<Product>
            {
                new Product {
                    Name = "Nevermind(Remastered)",
                    Description = "Phát hành gốc 1991. Đây là bản tái bản năm 2013 (Pallas Pressing) được đánh giá âm thanh cực tốt.",
                    Price = 1150000m, PromotionPrice = 862500m, Cost = 805000m, Quantity = 5, IsPublished = true,
                    Country = "US", ReleaseYear = 2013, Condition = "M (Mint)",
                    Artist = nirvana, Brand = geffen, Category = vinyl, Genre = rock, Style = sGrunge,
                    Tracklist = "Side 1:\n1. Smells Like Teen Spirit\n2. In Bloom\n3. Come As You Are\n4. Breed\n5. Lithium\n6. Polly\n\nSide 2:\n1. Territorial Pissings\n2. Drain You\n3. Lounge Act\n4. Stay Away\n5. On A Plain\n6. Something In The Way",
                    Images = new List<ProductImage> {
                        new ProductImage { Url = "/images/products/Nirvana/Nevermind(Remastered)/cover/c.jpg", IsPrimary = true, DisplayOrder = 1 },
                        new ProductImage { Url = "/images/products/Nirvana/Nevermind(Remastered)/gallery/g1.jpg", IsPrimary = false, DisplayOrder = 2 },
                        new ProductImage { Url = "/images/products/Nirvana/Nevermind(Remastered)/gallery/g2.jpg", IsPrimary = false, DisplayOrder = 3 },
                        new ProductImage { Url = "/images/products/Nirvana/Nevermind(Remastered)/gallery/g3.jpg", IsPrimary = false, DisplayOrder = 4 },
                        new ProductImage { Url = "/images/products/Nirvana/Nevermind(Remastered)/gallery/g4.jpg", IsPrimary = false, DisplayOrder = 5 }
                    }
                },

                new Product {
                    Name = "Master of Puppets(Remastered)",
                    Description = "Album thứ ba của Metallica, nhận được nhiều lời tán dương xung quanh nội dung về chính trị, một trong những album nhạc thrash xuất sắc và ảnh hướng nhất mọi thời đại.",
                    Price = 1050000m, Cost = 756000m, Quantity = 3, IsPublished = true,
                    Country = "US", ReleaseYear = 2017, Condition = "M (Mint)",
                    Artist = metallica, Brand = blackened, Category = vinyl, Genre = rock, Style = sThrash,
                    Tracklist = "Side One:\n1. Battery\n2. Master Of Puppets\n3. The Thing That Should Not Be\n4. Welcome Home (Sanitarium)\n\nSide Two:\n1. Disposable Heroes\n2. Leper Messiah\n3. Orion\n4. Damage, Inc.",
                    Images = new List<ProductImage> {
                        new ProductImage { Url = "/images/products/Metallica/MasterofPuppets(Remastered)/cover/c.jpg", IsPrimary = true, DisplayOrder = 1 },
                        new ProductImage { Url = "/images/products/Metallica/MasterofPuppets(Remastered)/gallery/g1.jpg", IsPrimary = false, DisplayOrder = 2 },
                        new ProductImage { Url = "/images/products/Metallica/MasterofPuppets(Remastered)/gallery/g2.jpg", IsPrimary = false, DisplayOrder = 3 },
                        new ProductImage { Url = "/images/products/Metallica/MasterofPuppets(Remastered)/gallery/g3.jpg", IsPrimary = false, DisplayOrder = 4 },
                        new ProductImage { Url = "/images/products/Metallica/MasterofPuppets(Remastered)/gallery/g4.jpg", IsPrimary = false, DisplayOrder = 5 }
                    }
                },

                new Product {
                    Name = "Abbey Road(EAS)",
                    Description = "Phát hành gốc 1969. Bản in tại Nhật năm 1976 (Series EAS). Có dải OBI.",
                    Price = 1450000m, Cost = 1044000m, Quantity = 1, IsPublished = true,
                    Country = "Japan", ReleaseYear = 1976, Condition = "VG+ (Very Good Plus)", IsUsed = true,
                    Artist = beatles, Brand = appleRec, Category = vinyl, Genre = rock, Style = sClassicRock,
                    Tracklist = "Side 1:\n1. Come Together\n2. Something\n3. Maxwell's Silver Hammer\n4. Oh! Darling\n5. Octopus's Garden\n6. I Want You (She's So Heavy)\n\nSide 2:\n1. Here Comes The Sun\n2. Because\n3. You Never Give Me Your Money\n4. Sun King\n5. Mean Mr. Mustard\n6. Polythene Pam\n7. She Came In Through The Bathroom Window\n8. Golden Slumbers\n9. Carry That Weight\n10. The End",
                    Images = new List<ProductImage> {
                        new ProductImage { Url = "/images/products/TheBeatles/AbbeyRoad(EAS)/cover/c.jpg", IsPrimary = true, DisplayOrder = 1 },
                        new ProductImage { Url = "/images/products/TheBeatles/AbbeyRoad(EAS)/gallery/g1.jpg", IsPrimary = false, DisplayOrder = 2 },
                        new ProductImage { Url = "/images/products/TheBeatles/AbbeyRoad(EAS)/gallery/g2.jpg", IsPrimary = false, DisplayOrder = 3 },
                        new ProductImage { Url = "/images/products/TheBeatles/AbbeyRoad(EAS)/gallery/g3.jpg", IsPrimary = false, DisplayOrder = 4 },
                        new ProductImage { Url = "/images/products/TheBeatles/AbbeyRoad(EAS)/gallery/g4.jpg", IsPrimary = false, DisplayOrder = 5 }
                    }
                },

                new Product {
                    Name = "Hybrid Theory(20th Anniversary Edition)",
                    Description = "Phát hành gốc 2000. Phiên bản kỷ niệm 20 năm (20th Anniversary Edition) màu đỏ (Red Vinyl).",
                    Price = 1350000m, Cost = 945000m, Quantity = 0, IsPublished = true,
                    Country = "US", ReleaseYear = 2020, Condition = "M (Mint)",
                    Artist = linkinPark, Brand = warnerUs, Category = vinyl, Genre = rock, Style = sNuMetal,
                    Tracklist = "Side 1:\n1. Papercut\n2. One Step Closer\n3. With You\n4. Points Of Authority\n5. Crawling\n6. Runaway\n\nSide 2:\n1. By Myself\n2. In The End\n3. A Place For My Head\n4. Forgotten\n5. Cure For The Itch\n6. Pushing Me Away",
                    Images = new List<ProductImage> {
                        new ProductImage { Url = "/images/products/LinkinPark/HybridTheory(20thAnniversaryEdition)/cover/c.jpg", IsPrimary = true, DisplayOrder = 1 },
                        new ProductImage { Url = "/images/products/LinkinPark/HybridTheory(20thAnniversaryEdition)/gallery/g1.jpg", IsPrimary = false, DisplayOrder = 2 },
                        new ProductImage { Url = "/images/products/LinkinPark/HybridTheory(20thAnniversaryEdition)/gallery/g2.jpg", IsPrimary = false, DisplayOrder = 3 },
                        new ProductImage { Url = "/images/products/LinkinPark/HybridTheory(20thAnniversaryEdition)/gallery/g3.jpg", IsPrimary = false, DisplayOrder = 4 },
                        new ProductImage { Url = "/images/products/LinkinPark/HybridTheory(20thAnniversaryEdition)/gallery/g4.jpg", IsPrimary = false, DisplayOrder = 5 }
                    }
                },

                new Product {
                    Name = "Gieo",
                    Description = "Album phòng thu thứ 4 của Ngọt. Phiên bản CD phát hành bởi LP Club.",
                    Price = 390000m, Cost = 253500m, Quantity = 20, IsPublished = true,
                    Country = "Việt Nam", ReleaseYear = 2022, Condition = "M (Mint)",
                    Artist = ngot, Brand = lpClubBrand, Category = cd, Genre = pop, Style = sIndiePop,
                    Tracklist = "1. Bạn thỏ TV nhỏ\n2. Mấy khi\n3. Em trang trí\n4. Điểm đến cuối cùng\n5. Em trong đầu\n6. Chào buổi sáng\n7. Thấy chưa\n8. Đá tan\n9. Đêm hôm qua\n10. Gieo\n11.  Mất tích",
                    Images = new List<ProductImage> {
                        new ProductImage { Url = "/images/products/Ngot/Gieo/cover/c.jpg", IsPrimary = true, DisplayOrder = 1 },
                        new ProductImage { Url = "/images/products/Ngot/Gieo/gallery/g1.jpg", IsPrimary = false, DisplayOrder = 2 },
                        new ProductImage { Url = "/images/products/Ngot/Gieo/gallery/g2.jpg", IsPrimary = false, DisplayOrder = 3 }
                    }
                },

                new Product {
                    Name = "Bảo Tàng Của Nuối Tiếc",
                    Description = "Album “BẢO TÀNG CỦA NUỐI TIẾC” đánh dấu cột mốc album phòng thu thứ 3 của Vũ. sau Vũ Trụ Song Song và Một Vạn Năm.",
                    Price = 390000m, Cost = 253500m, Quantity = 10, IsPublished = true,
                    Country = "Việt Nam", ReleaseYear = 2024, Condition = "M (Mint)",
                    Artist = vu, Brand = warnerVN, Category = cd, Genre = pop, Style = sIndiePop,
                    Tracklist = "1. Nếu Những Tiếc Nuối\n2. Mùa Mưa Ấy\n3. Ngồi Chờ Trong Vấn Vương (ft. Mỹ Anh)\n4. Dành Hết Xuân Thì Để Chờ Nhau (ft. Hà Anh Tuấn)\n5. Và Em Sẽ Luôn Là Người Tôi Yêu Nhất(ft. Khang Chillies)\n6. Những Chuyến Bay\n7. Mây Khóc Vì Điều Gì\n8. Những Lời Hứa Bỏ Quên(ft. Dear Jane)\n8. Không Yêu Em Thì Yêu Ai? (ft. Low G)\n10. bình yên (ft. Binz)",
                    Images = new List<ProductImage> {
                        new ProductImage { Url = "/images/products/Vu/BaoTangCuaNuoiTiec/cover/c.jpg", IsPrimary = true, DisplayOrder = 1 }
                    }
                },

                new Product {
                    Name = "Qua Khung Cửa Sổ",
                    Description = "Phiên bản vật lý album phòng thu đầu tay của Chillies.",
                    Price = 350000m, Cost = 238000m, Quantity = 15, IsPublished = true,
                    Country = "Việt Nam", ReleaseYear = 2021, Condition = "M (Mint)",
                    Artist = chillies, Brand = warnerVN, Category = cd, Genre = pop, Style = sVPop,
                    Tracklist = "1. Bao nhiêu\n2. Em đừng khóc\n3. Giá như\n4. Ms. May (ft. Magazine)\n5. Vùng ký ức\n6. Mộng du\n7. Qua khung cửa sổ\n8. Một cái tên (ft. Orange)\n9. Mascara\n10. Đường chân trời",
                    Images = new List<ProductImage> {
                        new ProductImage { Url = "/images/products/Chillies/QuaKhungCuaSo/cover/c.jpg", IsPrimary = true,  DisplayOrder = 1 },
                        new ProductImage { Url = "/images/products/Chillies/QuaKhungCuaSo/gallery/g1.jpg", IsPrimary = false, DisplayOrder = 2 }
                    }
                },

                new Product {
                    Name = "KIM",
                    Description = "Sau 4 năm kể từ album đầu tay “Qua khung cửa sổ”, Chillies chính thức trở lại với album phòng thu thứ hai mang tên “KIM” cùng phiên bản vật lý.",
                    Price = 390000m, Cost = 253500m, Quantity = 100, IsPublished = true,
                    Country = "Việt Nam", ReleaseYear = 2025, Condition = "M (Mint)",
                    IsPreOrder = true,
                    ReleaseDate = new DateTime(2025, 12, 20),
                    Artist = chillies, Brand = warnerVN, Category = cd, Genre = pop, Style = sVPop,
                    Tracklist = "1. Bao nhiêu\n2. Em đừng khóc\n3. Giá như\n4. Ms. May (ft. Magazine)\n5. Vùng ký ức\n6. Mộng du\n7. Qua khung cửa sổ\n8. Một cái tên (ft. Orange)\n9. Mascara\n10. Đường chân trời",
                    Images = new List<ProductImage> {
                        new ProductImage { Url = "/images/products/Chillies/KIM/cover/c.jpg", IsPrimary = true,  DisplayOrder = 1 },
                        new ProductImage { Url = "/images/products/Chillies/KIM/gallery/g1.jpg", IsPrimary = false, DisplayOrder = 2 },
                        new ProductImage { Url = "/images/products/Chillies/KIM/gallery/g2.jpg", IsPrimary = false, DisplayOrder = 3 },
                        new ProductImage { Url = "/images/products/Chillies/KIM/gallery/g3.jpg", IsPrimary = false, DisplayOrder = 4 }
                    }
                },

                new Product {
                    Name = "Rừng Đom Đóm",
                    Description = "Phiên bản cassette album đầu tay gồm 10 ca khúc của nhóm The Cassette đến từ Đà Nẵng do LP Club phát hành.",
                    Price = 280000m, Cost = 196000m, Quantity = 10, IsPublished = true,
                    Country = "Việt Nam", ReleaseYear = 2021, Condition = "M (Mint)",
                    Artist = theCassette, Brand = lpClubBrand, Category = cassette, Genre = rock, Style = sIndieRock,
                    Tracklist = "1. Cá rô\n2. Vùng đất linh hồn\n3. Nắng\n4. Espresso\n5. Ánh đèn phố\n6. Khoảng cách\n7. Nếu ngày mai tôi không trở về\n8. Tựa đêm nay\n9. Interlude: 9:00 PM\n10. Rừng đom đóm",
                    Images = new List<ProductImage> {
                        new ProductImage { Url = "/images/products/TheCassette/RungDomDom/cover/c.jpg", IsPrimary = true, DisplayOrder =1 },
                        new ProductImage { Url = "/images/products/TheCassette/RungDomDom/gallery/g1.jpg", IsPrimary = false, DisplayOrder = 2 },
                        new ProductImage { Url = "/images/products/TheCassette/RungDomDom/gallery/g2.jpg", IsPrimary = false, DisplayOrder = 3 }
                    }
                },

                new Product {
                    Name = "Hiệu Ứng Trốn Chạy(Deluxe Edition)",
                    Description = "Phát hành gốc 2017. Album thứ 3 của Cá Hồi Hoang phiên bản băng Cassette - Hiệu Ứng Trốn Chạy[Deluxe Edition] (+4 Bonus Tracks).",
                    Price = 220000m, Cost = 158400m, Quantity = 5, IsPublished = true,
                    Country = "Việt Nam", ReleaseYear = 2019, Condition = "M (Mint)",
                    Artist = caHoiHoang, Brand = timesRecords, Category = cassette, Genre = rock, Style = sAltRock,
                    Tracklist = "Mặt A:\n1. Hiệu Ứng Bắt Đầu\n2. 5AM\n3. Bin\n4. Inside Mr.Bin\n5. Khô Khan\n6. Cô Ấy\n7. Bên Trái\n8. Acid8\n9. Hiệu Ứng Ngược\n10. Hiệu Ứng Trốn Chạy\n11. Maru\n\nMặt B:\n1. Cần Một Ngày - Old Version (Bonus Track)\n2. Cần Một Ngày\n3. Quả Bóng Màu Hồng\n4. Tỉnh Táo\n5. Hiệu Ứng Bắt Đầu Lại\n6. Hiệu Ứng Cuối Cùng\n8. Cân Bằng (fx)\n9. GAP",
                    Images = new List<ProductImage> {
                        new ProductImage { Url = "/images/products/CaHoiHoang/HieuUngTronChay(DeluxeEdition)/cover/c.jpg", IsPrimary = true }
                    }
                },

                new Product {
                    Name = "In Utero",
                    Description = "Phát hành gốc 1993. Đây là bản băng Cassette cũ(Vintage) sản xuất tại Mỹ năm 1993.",
                    Price = 450000m, Cost = 315000m, Quantity = 2, IsPublished = true,
                    Country = "US", ReleaseYear = 1993, Condition = "VG+ (Very Good Plus)", IsUsed = true,
                    Artist = nirvana, Brand = geffen, Category = cassette, Genre = rock, Style = sGrunge,
                    Tracklist = "Side 1:\n1. Serve The Servants\n2. Scentless Apprentice\n3. Heart-Shaped Box\n4. Rape Me\n5. Frances Farmer Will Have Her Revenge On Seattle\n6. Dumb\n\nSide 2:\n1. Very Ape\n2. Milk It\n3. Pennyroyal Tea\n4. Radio Friendly Unit Shifter\n5. Tourette's\n6. All Apologies",
                    Images = new List<ProductImage> {
                        new ProductImage { Url = "/images/products/Nirvana/InUtero/cover/c.jpg", IsPrimary = true, DisplayOrder = 1 },
                        new ProductImage { Url = "/images/products/Nirvana/InUtero/gallery/g1.jpg", IsPrimary = false, DisplayOrder = 2 },
                        new ProductImage { Url = "/images/products/Nirvana/InUtero/gallery/g2.jpg", IsPrimary = false, DisplayOrder = 3 },
                        new ProductImage { Url = "/images/products/Nirvana/InUtero/gallery/g3.jpg", IsPrimary = false, DisplayOrder = 4 },
                        new ProductImage { Url = "/images/products/Nirvana/InUtero/gallery/g4.jpg", IsPrimary = false, DisplayOrder = 5 }
                    }
                },

                new Product {
                    Name = "OK Computer",
                    Description = "Phát hành 1997. OK Computer là album thứ ba của Radiohead, phát hành ban đầu tại Nhật 21 May 1997, UK 16 June 1997, US 1 July 1997. Đây là phiên bản album chính thống.",
                    Price = 1050000m, // VND
                    Cost = 735000m,
                    Quantity = 5,
                    IsPublished = true,
                    Country = "UK",
                    ReleaseYear = 1997, // phiên bản này: 1997
                    Condition = "M (Mint)",
                    Artist = radiohead,
                    Brand = parlophone,
                    Category = vinyl,
                    Genre = rock,
                    Style = sAltRock,
                    Tracklist = "1. Airbag\n2. Paranoid Android\n3. Subterranean Homesick Alien\n4. Exit Music (For a Film)\n5. Let Down\n6. Karma Police\n7. Fitter Happier\n8. Electioneering\n9. Climbing Up the Walls\n10. No Surprises\n11. Lucky\n12. The Tourist",
                    Images = new List<ProductImage> {
                        new ProductImage { Url = "/images/products/Radiohead/OKComputer/cover/c.jpg", IsPrimary = true, DisplayOrder = 1 },
                        new ProductImage { Url = "/images/products/Radiohead/OKComputer/gallery/g1.jpg", IsPrimary = false, DisplayOrder = 2 },
                        new ProductImage { Url = "/images/products/Radiohead/OKComputer/gallery/g2.jpg", IsPrimary = false, DisplayOrder = 3 },
                        new ProductImage { Url = "/images/products/Radiohead/OKComputer/gallery/g3.jpg", IsPrimary = false, DisplayOrder = 4 },
                        new ProductImage { Url = "/images/products/Radiohead/OKComputer/gallery/g4.jpg", IsPrimary = false, DisplayOrder = 5 }
                    }
                },

                // --- NEW: John Lennon - Imagine (1971) ---
                new Product {
                    Name = "Imagine",
                    Description = "Phát hành 1971. Imagine là album solo nổi tiếng của John Lennon, phát hành năm 1971 trên Apple Records.",
                    Price = 550000m,
                    Cost = 330000m,
                    Quantity = 4,
                    IsPublished = true,
                    Country = "UK/US",
                    ReleaseYear = 1971,
                    Condition = "M (Mint)",
                    Artist = johnLennon,
                    Brand = appleRec,
                    Category = vinyl,
                    Genre = rock,
                    Style = sPopRock,
                    Tracklist = "Side One:\n1. Imagine\n2. Crippled Inside\n3. Jealous Guy\n4. It's So Hard\n5. I Don't Wanna Be A Soldier Mama\n\nSide Two:\n1. Gimme Some Truth\n2. Oh My Love\n3. How Do You Sleep?\n4. How?\n5. Oh Yoko!",
                    Images = new List<ProductImage> {
                        new ProductImage { Url = "/images/products/JohnLennon/Imagine/cover/c.jpg", IsPrimary = true, DisplayOrder = 1 },
                        new ProductImage { Url = "/images/products/JohnLennon/Imagine/gallery/g1.jpg", IsPrimary = false, DisplayOrder = 2 },
                        new ProductImage { Url = "/images/products/JohnLennon/Imagine/gallery/g2.jpg", IsPrimary = false, DisplayOrder = 3 },
                        new ProductImage { Url = "/images/products/JohnLennon/Imagine/gallery/g3.jpg", IsPrimary = false, DisplayOrder = 4 },
                        new ProductImage { Url = "/images/products/JohnLennon/Imagine/gallery/g4.jpg", IsPrimary = false, DisplayOrder = 5 }
                    }
                },

                // --- NEW: Elton John - Goodbye Yellow Brick Road (1973) ---
                new Product {
                    Name = "Goodbye Yellow Brick Road",
                    Description = "Phát hành 1973. Đây là album đôi (double album) nổi tiếng của Elton John, phát hành 5 October 1973 trên DJM Records.",
                    Price = 650000m,
                    Cost = 455000m,
                    Quantity = 6,
                    IsPublished = true,
                    Country = "UK",
                    ReleaseYear = 1973,
                    Condition = "M (Mint)",
                    Artist = eltonJohn,
                    Brand = djm,
                    Category = vinyl,
                    Genre = rock,
                    Style = sPopRock,
                    Tracklist = "Side 1: \n1. Funeral for a Friend/Love Lies Bleeding\n2. Candle in the Wind\n3. Bennie and the Jets\n\nSide 2:\n1. Goodbye Yellow Brick Road\n2. This Song Has No Title\n3. Grey Seal\n4. Jamaica Jerk-off\n5. I've Seen That Movie Too\n\nSide 3: \n1. Sweet Painted Lady\n2. The Ballad Of Danny Bailey (1909-34)\n3. Dirty Little Girl\n4. All The Girls Love Alice\n\nSide 4: \n1. Your Sister Can't Twist (But She Can Rock 'n Roll)\n2. Saturday Night's Alright For Fighting\n3. Roy Rogers\n4. Social Disease\n5. Harmony",
                    Images = new List<ProductImage> {
                        new ProductImage { Url = "/images/products/EltonJohn/GoodbyeYellowBrickRoad/cover/c.jpg", IsPrimary = true, DisplayOrder = 1 },
                        new ProductImage { Url = "/images/products/EltonJohn/GoodbyeYellowBrickRoad/gallery/g1.jpg", IsPrimary = false, DisplayOrder = 2 },
                        new ProductImage { Url = "/images/products/EltonJohn/GoodbyeYellowBrickRoad/gallery/g2.jpg", IsPrimary = false, DisplayOrder = 3 },
                        new ProductImage { Url = "/images/products/EltonJohn/GoodbyeYellowBrickRoad/gallery/g3.jpg", IsPrimary = false, DisplayOrder = 4 },
                        new ProductImage { Url = "/images/products/EltonJohn/GoodbyeYellowBrickRoad/gallery/g4.jpg", IsPrimary = false, DisplayOrder = 5 },
                        new ProductImage { Url = "/images/products/EltonJohn/GoodbyeYellowBrickRoad/gallery/g5.jpg", IsPrimary = false, DisplayOrder = 6 },
                        new ProductImage { Url = "/images/products/EltonJohn/GoodbyeYellowBrickRoad/gallery/g6.jpg", IsPrimary = false, DisplayOrder = 7 },
                        new ProductImage { Url = "/images/products/EltonJohn/GoodbyeYellowBrickRoad/gallery/g7.jpg", IsPrimary = false, DisplayOrder = 8 }
                    }
                },

                // --- NEW: Neil Young - Harvest (1972) ---
                new Product {
                    Name = "Harvest",
                    Description = "Phát hành 1972. Harvest là album của Neil Young, phát hành ban đầu năm 1972.",
                    Price = 480000m,
                    Cost = 336000m,
                    Quantity = 3,
                    IsPublished = true,
                    Country = "US",
                    ReleaseYear = 1972,
                    Condition = "M (Mint)",
                    Artist = neilYoung,
                    Brand = reprise,
                    Category = vinyl,
                    Genre = folkCountry,
                    Style = sFolkRock, // note: sFolkRock existing under "Folk & Country" earlier; ensure variable exists in context in original file (if not, you can add mapping accordingly)
                    Tracklist = "Side 1: \n1. Out on the Weekend\n2. Harvest\n3. A Man Needs a Maid\n4. Heart of Gold\n5. Are You Ready for the Country?\n\nSide 2: \n1. Old Man\n2. There's a World\n3. Alabama\n4. The Needle and the Damage Done\n5. Words (Between the Lines of Age)",
                    Images = new List<ProductImage> {
                        new ProductImage { Url = "/images/products/NeilYoung/Harvest/cover/c.jpg", IsPrimary = true, DisplayOrder = 1 },
                        new ProductImage { Url = "/images/products/NeilYoung/Harvest/gallery/g1.jpg", IsPrimary = false, DisplayOrder = 2 },
                        new ProductImage { Url = "/images/products/NeilYoung/Harvest/gallery/g2.jpg", IsPrimary = false, DisplayOrder = 3 },
                        new ProductImage { Url = "/images/products/NeilYoung/Harvest/gallery/g3.jpg", IsPrimary = false, DisplayOrder = 4 },
                        new ProductImage { Url = "/images/products/NeilYoung/Harvest/gallery/g4.jpg", IsPrimary = false, DisplayOrder = 5 }
                    }
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}
