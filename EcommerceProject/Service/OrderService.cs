using EcommerceProject.DBcontext;
using EcommerceProject.Models;
using EcommerceProject.Views.Dto;
using Microsoft.EntityFrameworkCore;

namespace EcommerceProject.Service
{

    public class OrderService
    {
        private readonly Database _database;
        private readonly IHttpContextAccessor _httpContextAccessor; // ✅ add this
        public OrderService(Database database, IHttpContextAccessor httpContextAccessor)
        {
            _database = database;
            _httpContextAccessor = httpContextAccessor;
        }

        public BuyNowDTO GetProductById(int id)
        {
            var product = _database.UploadTable
                          .Include(u => u.Attachments)
                           .FirstOrDefault(u => u.Id == id);
            if (product != null)
            {
                var dto = new BuyNowDTO
                {
                    ProductId = product.Id,   // 👈 FIX: set the ProductId
                    ProductName = product.ProductName,
                    Price = Convert.ToDecimal(product.Price),
                    ImagePath = product?.Attachments.FirstOrDefault()?.FilePath
                };

                return dto;
            }

            return null;
        }

        //public void PlaceOrder(BuyNowDTO dto)
        //{
        //    var order = new Order
        //    {
        //        OrderId = GenerateOrderId(),   // <-- FIX: generate unique order ID
        //        ProductId = dto.ProductId,
        //        //ProductName = dto.ProductName,
        //        //Price = (decimal)dto.Price,
        //        Quantity = dto.Quantity,
        //        Total = (decimal)(dto.Price * dto.Quantity),
        //        //CustomerName = dto.CustomerName,
        //        Address = dto.Address,
        //        City = dto.City,
        //        PostalCode = dto.PostalCode,
        //        OrderDate = DateTime.Now,
        //        Status = "Pending"  // <--- Set default value here
        //    };

        //    _database.Orders.Add(order);
        //    _database.SaveChanges();
        //}

        public void PlaceOrder(BuyNowDTO dto)
        {
            // Try to get customerId from session
            var customerIdStr = _httpContextAccessor.HttpContext.Session.GetString("currentUserId");
            int customerId = 0;

            if (!string.IsNullOrEmpty(customerIdStr) && int.TryParse(customerIdStr, out int parsedId))
            {
                customerId = parsedId;
            }
            else
            {
                // Fallback: use email
                var email = _httpContextAccessor.HttpContext.Session.GetString("CustomerEmail");
                if (!string.IsNullOrEmpty(email))
                {
                    var customerByEmail = _database.Customers.FirstOrDefault(c => c.Email == email);
                    if (customerByEmail != null)
                        customerId = customerByEmail.Id;
                }
            }

            if (customerId == 0)
                throw new Exception("Customer not found. Please log in.");

            // Fetch product
            var product = _database.UploadTable.FirstOrDefault(p => p.Id == dto.ProductId);
            if (product == null) throw new Exception("Product not found");

            var order = new Order
            {
                OrderId = GenerateOrderId(),
                ProductId = product.Id,
                Quantity = dto.Quantity,
                Total = Convert.ToDecimal(product.Price) * dto.Quantity, // safe decimal
                OrderDate = DateTime.Now,
                Status = "Ordered",
                CustomerId = customerId,
                SellerId = product.SellerId
            };

            _database.Orders.Add(order);
            _database.SaveChanges();
        }





        public string GenerateOrderId(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            string orderId;

            do
            {
                orderId = new string(Enumerable.Repeat(chars, length)
                                          .Select(s => s[random.Next(s.Length)])
                                          .ToArray());
            } while (_database.Orders.Any(o => o.OrderId == orderId)); // Ensure uniqueness

            return orderId;
        }

        public void CreateOrder(Order order)
        {
            order.OrderId = GenerateOrderId(); // Assign random ID
            _database.Orders.Add(order);
            _database.SaveChanges();
        }



        public List<MyOrdersDTO> GetOrdersByCustomerId(int customerId)
        {
            return _database.Orders
                .Include(o => o.Customer)
                .Include(o => o.Seller)
                .Include(o => o.Product)
                    .ThenInclude(p => p.Attachments)
                .Where(o => o.CustomerId == customerId)  // ✅ safer than email
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new MyOrdersDTO
                {
                    OrderId = o.OrderId,
                    ProductName = o.Product.ProductName,
                    ProductImage = o.Product.Attachments.FirstOrDefault().FilePath,
                    Price = (decimal)o.Product.Price,
                    Quantity = o.Quantity,
                    Total = o.Total,
                    OrderDate = o.OrderDate,
                    Status = o.Status
                })
                .ToList();
        }


        public Order GetOrderById(int id, string email)
        {
            return _database.Orders
                .Include(o => o.Customer)
                .Include(o => o.Seller)
                .FirstOrDefault(o => o.Id == id && o.Customer.Email == email);
        }

        public OrderDetailsDTO GetOrderDetailsById(string orderId, string email)
        {
            var order = _database.Orders
                .Include(o => o.Customer)
                .Include(o => o.Seller)
                .Include(o => o.Product)
                    .ThenInclude(p => p.Attachments)
                .Where(o => o.OrderId == orderId && o.Customer.Email == email)
                .Select(o => new OrderDetailsDTO
                {
                    OrderId = o.Id,
                    ProductId = o.Product.Id,
                    ProductName = o.Product.ProductName,
                    ProductImage = o.Product.Attachments.FirstOrDefault().FilePath,
                    SellerName = o.Seller.Login,
                    Price = (decimal)o.Product.Price,
                    Quantity = o.Quantity,
                    TotalAmount = o.Total,
                    OrderDate = o.OrderDate,
                    ConfirmedDate = o.Status == "Confirmed" ? o.OrderDate : null,
                    ShippedDate = o.Status == "Shipped" ? o.OrderDate : null,
                    DeliveredDate = o.Status == "Delivered" ? o.OrderDate : null,
                    CustomerName = o.Customer.Name,
                    //Address = o.Address,
                    Phone = o.Customer.Phone,
                    OrderStatus = o.Status
                }).FirstOrDefault();

            return order;
        }


    }
}
