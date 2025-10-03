using EcommerceProject.Models;
using System.Net.Mail;
using System.Net;
using EcommerceProject.DBcontext;
using Microsoft.EntityFrameworkCore;
using EcommerceProject.Views.Dto;

namespace EcommerceProject.Service
{
    public class CustomerService
    {
        private readonly Database _database;
        private readonly CustomerService _customerService;

        //Constructor - injects the database context

        public CustomerService(Database database)
        {
            _database = database;
         
        }

        // Check if the email is already registered in the database
        public async Task<bool> IsEmailRegisteredAsync(string email)
        {
            return await _database.Customers.AnyAsync(c => c.Email == email);
        }

        // Generate a random 4-digit OTP as string
        public string GenerateOtp()
        {
            return new Random().Next(1000, 9999).ToString();
        }

        //Send OTP to user's email
        public void SendOtpEmail(string toEmail, string otp)
        {
            var mail = new MailMessage();
            mail.To.Add(toEmail); // Recipient email
            mail.From = new MailAddress("vishwa2003t@gmail.com"); // Sender email
            mail.Subject = "Your OTP Code";
            mail.Body = $"Your OTP is: {otp}"; // Email content

            // SMTP (Gmail) settings
            var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("vishwa2003t@gmail.com", "qpwx vttj dduz yszp"), // App password
                EnableSsl = true
            };

            smtp.Send(mail); // Send the email
        }

        //public void SendOtpEmail(string toEmail, string otp)
        //{
        //    try
        //    {
        //        var mail = new MailMessage
        //        {
        //            From = new MailAddress("vishwa2003t@gmail.com"),
        //            Subject = "Your OTP Code",
        //            Body = $"Your OTP is: {otp}"
        //        };
        //        mail.To.Add(toEmail);

        //        using (var smtp = new SmtpClient("smtp.gmail.com", 587))
        //        {
        //            smtp.Credentials = new NetworkCredential("vishwa2003t@gmail.com", "qpwx vttj dduz yszp");
        //            smtp.EnableSsl = true;
        //            smtp.Send(mail);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Failed to send OTP: {ex.Message}");
        //        throw; // rethrow so controller can show error
        //    }
        //}




        // Save new user's email and OTP to the database after successful OTP verification
        public async Task<bool> SaveVerifiedEmailAsync(string email, string otp)
        {
            // If already registered, return false
            if (await IsEmailRegisteredAsync(email))
                return false;

            // Add the new verified user
            _database.Customers.Add(new CustomerLogin
            {
                Email = email,
                OtpCode = otp,
                OtpGeneratedAt = DateTime.Now
            });

            await _database.SaveChangesAsync(); // Save changes to DB
            return true;
        }

        // Verify if OTP is correct and within valid time (5 minutes)
        public async Task<bool> VerifyOtpAsync(string email, string otp)
        {
            var user = await _database.Customers.FirstOrDefaultAsync(c => c.Email == email);
            if (user == null) return false;

            // Check if OTP matches and is not expired
            bool isValid = user.OtpCode == otp && user.OtpGeneratedAt >= DateTime.Now.AddMinutes(-5);

            if (isValid)
            {
                user.OtpCode = null; // Clear OTP after successful verification
                await _database.SaveChangesAsync();
            }

            return isValid;
        }

        // Update OTP for login and send it to the user's email
        public async Task<bool> UpdateOtpForLoginAsync(string email, string otp)
        {
            var user = await _database.Customers.FirstOrDefaultAsync(c => c.Email == email);
            if (user == null) return false;

            user.OtpCode = otp; // Update OTP
            user.OtpGeneratedAt = DateTime.Now; // Update time

            await _database.SaveChangesAsync();
            SendOtpEmail(email, otp); // Send new OTP
            return true;
        }

        public async Task<CustomerLogin?> GetCustomerByEmailAsync(string email)
        {
            return await _database.Customers.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<bool> SaveCustomerDetailsAsync(string email, string name, string phone, 
            string otp ,string address, string city, string postalcode)
        {
            try
            {
                // Verify all required fields
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone))
                {
                    Console.WriteLine("Missing required fields");
                    return false;
                }

                var customer = new CustomerLogin
                {
                    Email = email,
                    Name = name,
                    Phone = phone,
                    OtpCode = otp,
                    OtpGeneratedAt = DateTime.UtcNow,
                    Address = address,
                    City = city,
                    PostalCode = postalcode,
                };

                _database.Customers.Add(customer);
                int affectedRows = await _database.SaveChangesAsync();

                Console.WriteLine($"Rows affected: {affectedRows}");

                return affectedRows > 0;
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database error: {dbEx.InnerException?.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                return false;
            }
        }

        public async Task<CustomerLogin?> GetCustomerByNameAsync(string name)
        {
            return await _database.Customers.FirstOrDefaultAsync(c => c.Name == name);
        }



        //public UploadDTO Addwishlist(int productId)
        //{
        //    var existingCartItem = _database.Wishlist
        //        .FirstOrDefault(x => x.ProductId == productId);

        //    if (existingCartItem != null)
        //        return null;
        //    var newItem = new Wishlist
        //    {
        //        ProductId = productId,
        //    };
        //    _database.Wishlist.Add(newItem);
        //    _database.SaveChanges();

        //    return new UploadDTO();
        //}



        //public bool ToggleWishlist(int productId, string currentUser)
        //{
        //    // Check if the product is already in the wishlist
        //    var existingItem = _database.Wishlist
        //        .FirstOrDefault(x => x.ProductId == productId);

        //    if (existingItem != null)
        //    {
        //        // If exists, remove it (unlike your current code which returns null)
        //        _database.Wishlist.Remove(existingItem);
        //    }
        //    else
        //    {
        //        // If not exists, add it
        //        var newItem = new Wishlist
        //        {
        //            ProductId = productId,
        //            CurrentUserId = int.Parse(currentUser)
        //        };
        //        _database.Wishlist.Add(newItem);
        //    }

        //    _database.SaveChanges();
        //    return true; // always return true to indicate success
        //}


        public bool ToggleWishlist(int productId, string currentUser)
        {
            if (string.IsNullOrWhiteSpace(currentUser) || !int.TryParse(currentUser, out int userId))
                return false;

            var existingItem = _database.Wishlist
                .FirstOrDefault(x => x.ProductId == productId && x.CurrentUserId == userId);

            if (existingItem != null)
            {
                _database.Wishlist.Remove(existingItem);
            }
            else
            {
                _database.Wishlist.Add(new Wishlist { ProductId = productId, CurrentUserId = userId });
            }

            _database.SaveChanges();
            return true;
        }

        public List<WishlistDTO> GetWishlistProducts(string currentUser)
        {
            if (string.IsNullOrWhiteSpace(currentUser) || !int.TryParse(currentUser, out int userId))
                return new List<WishlistDTO>();

            var items = _database.Wishlist
                .Where(w => w.CurrentUserId == userId)
                .Include(w => w.Upload)
                    .ThenInclude(u => u.Attachments) // join product + images
                .Select(w => new WishlistDTO
                {
                    Id = w.Id,
                    ProductId = w.ProductId,
                    ProductName = w.Upload.ProductName,
                    Description = w.Upload.Description,
                    Price = w.Upload.Price,
                    ImagePath = w.Upload.Attachments.FirstOrDefault().FilePath
                })
                .ToList();

            return items;
        }


        public int GetWishlistCount(string currentUser)
        {
            if (string.IsNullOrWhiteSpace(currentUser) || !int.TryParse(currentUser, out int userId))
                return 0;

            return _database.Wishlist.Count(w => w.CurrentUserId == userId);
        }

        public bool RemoveFromWishlist(int productId, string currentUser)
        {
            if (string.IsNullOrWhiteSpace(currentUser) || !int.TryParse(currentUser, out int userId))
                return false;

            var item = _database.Wishlist.FirstOrDefault(w => w.ProductId == productId && w.CurrentUserId == userId);
            if (item != null)
            {
                _database.Wishlist.Remove(item);
                _database.SaveChanges();
                return true;
            }

            return false;
        }

        public async Task<CustomerDashBoardDTO?> GetDashboardAsync(string email)
        {

            var customer = await _database.Customers.FirstOrDefaultAsync(c => c.Email == email);

            if (customer == null) return null;

            return new CustomerDashBoardDTO
            {
                Name = customer.Name,
                EmailAddress = customer.Email,
                Address = customer.Address,
                Phone = customer.Phone,
                City = customer.City,
                PostalCode = customer.PostalCode,
            };
       
        }

        public async Task<bool> UpdateProfileAsync(string email, string name, string phone)
        {
            var customer = await _database.Customers.FirstOrDefaultAsync(c => c.Email == email);

            if (customer == null) return false;

            customer.Name = name;
            customer.Phone = phone;

            await _database.SaveChangesAsync();
            return true;

        }

        public async Task<bool> UpdateAddressAsync(string email ,string address, string postalcode ,string city )
        {
            var customer = await _database.Customers.FirstOrDefaultAsync(c => c.Email == email);
            if (customer == null) return false;

            customer.Address = address;
            customer.City = city;
            customer.PostalCode = postalcode;

            await _database.SaveChangesAsync();
            return true;

        }



    }
}