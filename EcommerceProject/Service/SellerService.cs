using EcommerceProject.DBcontext;
using EcommerceProject.DTOs;
using EcommerceProject.Models;
using EcommerceProject.Views.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;



namespace EcommerceProject.Service
{
    public class SellerService
    {
        private readonly Database _database;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SellerService(Database database, IHttpContextAccessor httpContextAccessor)
        {
            _database = database;
            _httpContextAccessor = httpContextAccessor; // ✅ Now this works
        }



        //public bool CheckUser(SellerLoginDTO data)
        //{
        //    var employee = _database.Seller.FirstOrDefault(x => x.Login == data.Login);
        //    if (employee != null)
        //    {
        //        bool result = employee.Password == data.Password;
        //        if (result)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}




        // This function checks if seller exists (for login)
        public SellerLogin CheckUser(SellerLoginDTO data)
        {
            var seller = _database.Seller.FirstOrDefault(x => x.Login == data.Login);

            if (seller != null && seller.Password == data.Password)
            {
                return seller; // ✅ Login success
            }
            return null; // ❌ Login failed
        }


        // This function saves product uploaded by seller
        public bool CheckProduct(UploadDTO upload, string currentUserId)
        {
            try
            {
                // Create folder if it doesn’t exist
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Save image file
                var fileName = Path.GetFileName(upload.ImageFile.FileName);
                var filePath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    upload.ImageFile.CopyTo(stream);
                }

                // Create new product entry
                var Product = new Upload
                {
                    ProductName = upload.ProductName,
                    Description = upload.Description,
                    Price = upload.Price,
                    SellerId = int.Parse(currentUserId) // ✅ Link product to logged-in seller
                };

                _database.UploadTable.Add(Product);
                _database.SaveChanges();

                // Get last inserted product id
                int lastUpdatedId = _database.UploadTable
                    .OrderByDescending(i => i.Id)
                    .Select(i => i.Id)
                    .FirstOrDefault();

                // Save attachment info (file path)
                var attachment = new Attachment
                {
                    UploadId = lastUpdatedId,
                    FilePath = "/uploads/" + fileName,
                    FileName = fileName
                };

                _database.Attachments.Add(attachment);
                _database.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        // Show all products
        public List<UploadDTO> GetAllProducts()
        {
            var products = _database.UploadTable
                .Include(u => u.Attachments)
                .ToList();

            var result = products.Select(u => new UploadDTO
            {
                Id = u.Id,
                ProductName = u.ProductName,
                Description = u.Description,
                Price = u.Price,
                ImagePath = u.Attachments.FirstOrDefault()?.FilePath
            }).ToList();

            return result;
        }


        // Dashboard info for seller
        public SellerDashboardDTO GetDashboardData(int sellerId)
        {
            var seller = _database.Seller.FirstOrDefault(x => x.id == sellerId);

            // ✅ FIX: Count only products of this seller
            var totalProducts = _database.UploadTable.Count(x => x.SellerId == sellerId);

            // Orders linked to this seller
            var totalOrders = _database.Orders.Count(x => x.SellerId == sellerId);
            var pendingOrders = _database.Orders.Count(x => x.SellerId == sellerId && x.Status == "Pending");
            var revenue = _database.Orders.Where(x => x.SellerId == sellerId).Sum(x => (decimal?)x.Total) ?? 0;

            return new SellerDashboardDTO
            {
                SellerName = seller?.Name ?? "Seller",
                TotalProducts = totalProducts,
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                TotalRevenue = revenue
            };
        }


        // Show seller products only
        public List<Upload> GetSellerProducts(int sellerId)
        {
            return _database.UploadTable.Where(x => x.SellerId == sellerId).ToList();
        }


        // Show seller orders only
        public List<Order> GetSellerOrders(int sellerId)
        {
            return _database.Orders.Where(x => x.SellerId == sellerId).ToList();
        }


          public int GetTotalProducts(int sellerId)
    {
        // Counts only products uploaded by this seller
        return _database.UploadTable.Count(x => x.SellerId == sellerId);
    }





        // Show product details by Id
        public UploadDTO Productlayer(int Id)
        {
            var product = _database.UploadTable
                .Include(u => u.Attachments)
                .FirstOrDefault(u => u.Id == Id);

            if (product != null)
            {
                var dto = new UploadDTO
                {
                    Id = product.Id,
                    ProductName = product.ProductName,
                    Description = product.Description,
                    Price = product.Price,
                    ImagePath = product.Attachments.FirstOrDefault()?.FilePath
                };

                return dto;
            }

            return null;
        }




        public bool AddCart(int productId, string currentUserId)
        {
            if (string.IsNullOrWhiteSpace(currentUserId))
                return false;

            // Check if item already exists in DB
            var cartItem = _database.Carts
                .FirstOrDefault(c => c.UserId == currentUserId && c.UploadId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += 1; // increment quantity if exists
            }
            else
            {
                _database.Carts.Add(new Cart
                {
                    UserId = currentUserId,
                    UploadId = productId,
                    Quantity = 1
                });
            }

            _database.SaveChanges();
            return true;
        }



        public List<UploadDTO> GetCartProducts(string currentUserId)
        {
            if (string.IsNullOrWhiteSpace(currentUserId))
                return new List<UploadDTO>();

            var cartItems = _database.Carts
                .Where(c => c.UserId == currentUserId)
                .ToList();

            if (!cartItems.Any())
                return new List<UploadDTO>();

            var productIds = cartItems.Select(c => c.UploadId).ToList();

            var products = _database.UploadTable
                .Include(u => u.Attachments)
                .Where(u => productIds.Contains(u.Id))
                .ToList();

            return products.Select(p =>
            {
                var ci = cartItems.First(c => c.UploadId == p.Id);
                return new UploadDTO
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    Price = p.Price,
                    ImagePath = p.Attachments.FirstOrDefault()?.FilePath,
                    Quantity = ci.Quantity
                };
            }).ToList();
        }





        public int GetCartCount(string currentUserId)
        {
            if (string.IsNullOrWhiteSpace(currentUserId))
                return 0;

            return _database.Carts
                .Where(c => c.UserId == currentUserId)
                .Sum(c => c.Quantity); // ✅ total items (sums all quantities)
        }



        public bool RemoveCartItem(int productId, string currentUserId)
        {
            if (string.IsNullOrWhiteSpace(currentUserId))
                return false;

            var cartItem = _database.Carts
                .FirstOrDefault(c => c.UserId == currentUserId && c.UploadId == productId);

            if (cartItem != null)
            {
                _database.Carts.Remove(cartItem);
                _database.SaveChanges();
                return true;
            }

            return false;
        }




        public bool registration(SellerRegistrationDTO userLoginDTO)
        {

            var employee = _database.Seller.FirstOrDefault(x => x.Login == userLoginDTO.Email);
            if (employee == null)
            {
                var registration = new SellerLogin
                {
                    Login = userLoginDTO.Email,
                    Password = userLoginDTO.Password,
                    Name = userLoginDTO.CompanyName,
                    GSTNO = userLoginDTO.GSTNO,
                    MobileNo = userLoginDTO.MobileNumber,
                };

                _database.Seller.Add(registration);
                _database.SaveChanges();

                return true;
            }
            return false;
        }

        public Upload GetProductById(int id)
        {
            return _database.UploadTable
                .Include(p => p.Attachments) // so you also get image paths
                .FirstOrDefault(p => p.Id == id);
        }




    }

}
