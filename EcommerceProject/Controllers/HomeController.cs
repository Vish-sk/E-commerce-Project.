
using EcommerceProject.DBcontext;
using EcommerceProject.Service;
using EcommerceProject.Views.Dto;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly SellerService _sellerService;
        private readonly CustomerService _customerService;
        private readonly OrderService _orderService;
        private readonly Database _database;

        public HomeController(SellerService sellerService, CustomerService customerService, OrderService orderService, Database database)
        {
            _sellerService = sellerService;
            _customerService = customerService;
            _orderService = orderService;
            _database = database;
        }

        public IActionResult SellerLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SellerLogin(SellerLoginDTO data)
        {


            // Check if seller exists and password is correct
            var seller = _sellerService.CheckUser(data);

            if (seller != null)
            {
                HttpContext.Session.SetString("currentUserId", Convert.ToString(seller?.id) ?? "");
                // Store seller login in a cookie
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddDays(7); // Cookie valid for 7 days
                Response.Cookies.Append("SellerLogin", seller.Login, option);
                Response.Cookies.Append("SellerId", seller.id.ToString(), option);


                return Redirect("/home/UploadDTO");
            }

            // Login failed, show error
            TempData["Error"] = "Invalid login or password";
            return Redirect("/home/SellerLogin");
        }

        // Logout: remove cookies
        public IActionResult SellerLogout()
        {
            Response.Cookies.Delete("SellerLogin");
            Response.Cookies.Delete("SellerId");

            return Redirect("/home/SellerLogin");
        }


        //[HttpGet]
        //public IActionResult SellerDashboard()
        //{
        //    // get sellerId from cookie if available
        //    var sellerIdCookie = Request.Cookies["SellerId"];
        //    int sellerId = string.IsNullOrEmpty(sellerIdCookie) ? 1 : int.Parse(sellerIdCookie);

        //    var data = _sellerService.GetDashboardData(sellerId);

        //    return View(data); // ✅ Now your cshtml has a model
        //}


        //[HttpPost]
        //public IActionResult SellerDashboard(int sellerId = 1) // default sellerId=1 for testing
        //{
        //    var data = _sellerService.GetDashboardData(sellerId);
        //    return View(data);
        //}


        //[HttpGet]
        public IActionResult ManageProducts(int sellerId = 1)
        {
            var products = _sellerService.GetSellerProducts(sellerId);
            return View(products);
        }
        //[HttpGet]
        public IActionResult ManageOrders(int sellerId = 1)
        {
            var orders = _sellerService.GetSellerOrders(sellerId);
            return View(orders);
        }



        public IActionResult UploadDTO()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadDTO(UploadDTO reupload)
        {
            string currentUserId = HttpContext.Session.GetString("currentUserId");

            bool result = _sellerService.CheckProduct(reupload, currentUserId);

            if (result)
            {
                return Redirect("ProductView");

            }

            return Redirect("UploadDTO");
        }

        [HttpGet]
        public IActionResult Productlayer(int Id)
        {

            System.Diagnostics.Debug.WriteLine("Received Id: " + Id); // this shows in Output window
            UploadDTO result = _sellerService.Productlayer(Id);

            if (result != null)
            {
                return View(result);
            }
            return Redirect("ProductView");
        }


        //[HttpPost("/Home/AddToCart/{id}")]
        //public IActionResult AddToCart(int id)
        //{

        //    string currentUserId = HttpContext.Session.GetString("currentUserId");

        //    var result = _sellerService.AddCart(id);

        //    //int count = _sellerService.GetCartCount();

        //    if (result != null)
        //    {
        //        return Ok(); // JS will handle success

        //    }

        //    return BadRequest();
        //}

        //[HttpPost("/Home/AddToCart/{id}")]
        //public IActionResult AddToCart(int id)
        //{
        //    string currentUserId = HttpContext.Session.GetString("currentUserId");

        //    var result = _sellerService.AddCart(id, currentUserId);

        //    if (result)
        //        return Ok(); // JS handles success

        //    return BadRequest(); // user not logged in or error
        //}


        //[HttpPost("/Home/AddToCart/{id}")]
        //public IActionResult AddToCart(int id)
        //{
        //    string currentUserId = HttpContext.Session.GetString("currentUserId");

        //    var result = _sellerService.AddCart(id, currentUserId);

        //    if (result)
        //        return Ok(); // ✅ JS checks for res.ok

        //    return BadRequest();
        //}





        //[HttpGet]
        //public IActionResult Cart()
        //{
        //    var cartProducts = _sellerService.GetCartProducts();
        //    return View(cartProducts);
        //}


        //[HttpGet]
        //public IActionResult CartCount()
        //{
        //    string currentUserId = HttpContext.Session.GetString("currentUserId");

        //    int cartCount = _sellerService.GetCartCount(currentUserId);

        //    return Ok(cartCount); // Return JSON count for badge
        //}





        //[HttpPost("/Home/RemoveCart/{id}")]
        //public IActionResult RemoveCart(int id)
        //{
        //    bool result = _sellerService.RemoveCartItem(id);

        //    if (result)
        //    {
        //        return Ok(); // JS will handle success
        //    }

        //    return BadRequest();
        //}


        [HttpPost("/Home/AddToCart/{id}")]
        public IActionResult AddToCart(int id)
        {
            string currentUserId = HttpContext.Session.GetString("currentUserId");

            if (_sellerService.AddCart(id, currentUserId))
                return Ok();

            return BadRequest();
        }


        [HttpPost("/Home/RemoveCartItem/{id}")]
        public IActionResult RemoveCartItem(int id)
        {
            string currentUserId = HttpContext.Session.GetString("currentUserId");

            if (_sellerService.RemoveCartItem(id, currentUserId))
                return Ok();

            return BadRequest();
        }



        [HttpGet]
        public IActionResult CartCount()
        {
            string currentUserId = HttpContext.Session.GetString("currentUserId");
            int count = _sellerService.GetCartCount(currentUserId);
            return Ok(count);
        }

        [HttpGet]
        public IActionResult Cart()
        {
            string currentUserId = HttpContext.Session.GetString("currentUserId");
            var products = _sellerService.GetCartProducts(currentUserId);
            return View(products);
        }


        [HttpGet]
        public IActionResult SelleRregistration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SelleRregistration(SellerRegistrationDTO sellerRegistration)
        {
            bool result = _sellerService.registration(sellerRegistration);

            if (result)
            {
                TempData["Success"] = "Registration successful.";
                return RedirectToAction("SelleRregistration");
            }
            else
            {
                TempData["Error"] = "User already registered. Please go to login page.";
                return RedirectToAction("SelleRregistration");
            }
        }


        [HttpGet]
        public IActionResult BuyNow(int id)
        {

            BuyNowDTO result = _orderService.GetProductById(id);

            if (result != null)
            {
                return View(result);
            }
            return Redirect("ProductView");
        }



        [HttpPost]
        public IActionResult BuyNow(BuyNowDTO model)
        {
            // Save DTO into Session instead of TempData
            var json = System.Text.Json.JsonSerializer.Serialize(model);
            HttpContext.Session.SetString("BuyNowData", json);

            return RedirectToAction("ProceedToPayment");
        }

        [HttpGet]
        public IActionResult ProceedToPayment()
        {
            var json = HttpContext.Session.GetString("BuyNowData");
            if (string.IsNullOrEmpty(json))
                return RedirectToAction("ProductView");

            var dto = System.Text.Json.JsonSerializer.Deserialize<BuyNowDTO>(json);

            return View("RazorpayPayment", dto);
        }

        //public IActionResult PaymentSuccess(string paymentId)
        //{
        //    var json = HttpContext.Session.GetString("BuyNowData");
        //    if (string.IsNullOrEmpty(json))
        //        return RedirectToAction("ProductView");

        //    var dto = System.Text.Json.JsonSerializer.Deserialize<BuyNowDTO>(json);

        //    _orderService.PlaceOrder(dto);

        //    // Clear after success
        //    HttpContext.Session.Remove("BuyNowData");

        //    ViewBag.PaymentId = paymentId;
        //    return View("PaymentSuccess");
        //}

        public IActionResult PaymentSuccess(string paymentId)
        {
            var json = HttpContext.Session.GetString("BuyNowData");
            if (string.IsNullOrEmpty(json))
                return RedirectToAction("ProductView");

            var dto = System.Text.Json.JsonSerializer.Deserialize<BuyNowDTO>(json);

            //  Double-check customer before placing order
            var customerIdStr = HttpContext.Session.GetString("currentUserId");
            var customerEmail = HttpContext.Session.GetString("CustomerEmail");

            if (string.IsNullOrEmpty(customerIdStr) && string.IsNullOrEmpty(customerEmail))
            {
                TempData["Error"] = "Session expired. Please log in again.";
                return RedirectToAction("CustomerLogin");
            }

            try
            {
                _orderService.PlaceOrder(dto);

                // Clear session BuyNow data after success
                HttpContext.Session.Remove("BuyNowData");

                ViewBag.PaymentId = paymentId;
                return View("PaymentSuccess");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("CustomerLogin");
            }
        }




        // --- Home / products ---
        public IActionResult ProductView()
        {
            var products = _sellerService.GetAllProducts();
            return View(products);
        }

        // --- Customer login (GET) ---
        public IActionResult CustomerLogin()
        {
            return View();
        }

        //// --- Initial POST from CustomerLogin (email submit)
        //// This handles: if existing email -> send OTP for login; if new -> redirect to PhoneName to collect name/phone
        //[HttpPost]
        //public async Task<IActionResult> NewLogin(CustomerLoginDTO dto)
        //{
        //    if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
        //    {
        //        ViewBag.Error = "Please enter a valid email.";
        //        return View("CustomerLogin");
        //    }

        //    bool isRegistered = await _customerService.IsEmailRegisteredAsync(dto.Email);

        //    if (isRegistered)
        //    {
        //        // Existing user -> generate OTP, update user, send email, redirect to VerifyOtp
        //        string otp = _customerService.GenerateOtp();
        //        await _customerService.UpdateOtpForLoginAsync(dto.Email, otp); // updates DB and sends email

        //        TempData["VerifyEmail"] = dto.Email;
        //        TempData["OTP"] = otp;

        //        // Fetch name for display if you want to show it on verify page
        //        var existingCustomer = await _customerService.GetCustomerByEmailAsync(dto.Email);
        //        if (existingCustomer != null) TempData["Name"] = existingCustomer.Name;

        //        // Keep keys for subsequent requests (ResendOtp, Verify post)
        //        TempData.Keep("VerifyEmail");
        //        TempData.Keep("Name");
        //        TempData.Keep("OTP");

        //        return RedirectToAction("VerifyOtp");
        //    }
        //    else
        //    {
        //        // New user -> save email temporarily and ask for phone+name
        //        TempData["RegEmail"] = dto.Email;
        //        TempData.Keep("RegEmail");
        //        return RedirectToAction("PhoneName");
        //    }
        //}


        [HttpPost]
        public async Task<IActionResult> NewLogin(CustomerLoginDTO dto, string actionType)   ///////new code to check
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
            {
                ViewBag.Error = "Please enter a valid email."; //to check user enter email or not
                return View("CustomerLogin");
            }

            bool isRegistered = await _customerService.IsEmailRegisteredAsync(dto.Email);

            //for login
            if (actionType == "Login")
            {
                if (!isRegistered)
                {
                    ViewBag.Error = "Email not found. Please register first.";
                    return View("CustomerLogin");
                }

                // Send OTP for login
                string otp = _customerService.GenerateOtp();
                await _customerService.UpdateOtpForLoginAsync(dto.Email, otp);

                TempData["VerifyEmail"] = dto.Email;
                TempData["OTP"] = otp;

                var existingCustomer = await _customerService.GetCustomerByEmailAsync(dto.Email);
                if (existingCustomer != null) TempData["Name"] = existingCustomer.Name;
                TempData.Keep("VerifyEmail");
                TempData.Keep("Name");
                TempData.Keep("OTP");
                HttpContext.Session.SetString("currentUserId", Convert.ToString(existingCustomer?.Id) ?? "");

                return RedirectToAction("VerifyOtp");
            }
            //for register 
            else if (actionType == "Register")
            {
                if (isRegistered)
                {
                    ViewBag.Error = "This email is already registered. Please login instead.";
                    return View("CustomerLogin");
                }

                TempData["RegEmail"] = dto.Email;
                TempData.Keep("RegEmail");
                return RedirectToAction("PhoneName");
            }

            ViewBag.Error = "Invalid action.";
            return View("CustomerLogin");
        }



        public IActionResult PhoneName()
        {
            var email = TempData["RegEmail"] as string;
            if (string.IsNullOrEmpty(email))
            {
                // Safety: If no email, redirect to login
                return RedirectToAction("CustomerLogin");
            }

            // Keep email in TempData for next request too
            TempData.Keep("RegEmail");

            // Pass it into model so hidden field has a value
            var model = new CustomerRegisterDTO { Email = email };
            return View(model);
        }


        // --- Save phone & name (POST) from PhoneName.cshtml ---
        [HttpPost]
        public async Task<IActionResult> SavePhoneName(CustomerRegisterDTO model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View("PhoneName", model);
            //}


            // generate OTP and send email
            string otp = _customerService.GenerateOtp();
            _customerService.SendOtpEmail(model.Email, otp);

            // store everything in TempData for VerifyOtp
            TempData["VerifyEmail"] = model.Email;
            TempData["Name"] = model.Name;
            TempData["Phone"] = model.Phone;
            TempData["OTP"] = otp;
            TempData["Address"] = model.Address;
            TempData["PostalCode"] = model.PostalCode;
            TempData["City"] = model.City;


            // Keep for subsequent requests (GET VerifyOtp and Resend)
            TempData.Keep("VerifyEmail");
            TempData.Keep("Name");
            TempData.Keep("Phone");
            TempData.Keep("OTP");
            TempData.Keep("Address");
            TempData.Keep("PostalCode");
            TempData.Keep("City");

            return RedirectToAction("VerifyOtp");


        }

        // --- Verify OTP (GET) ---
        [HttpGet]
        public IActionResult VerifyOtp()
        {
            // Keep TempData so it is available for AJAX resend and POST
            TempData.Keep("VerifyEmail");
            TempData.Keep("Name");
            TempData.Keep("Phone");
            TempData.Keep("OTP");

            return View();
        }

        // --- Verify OTP (POST) ---
        [HttpPost]
        public async Task<IActionResult> VerifyOtp(VerifyOtpDTO model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.OtpCode))
            {
                ViewBag.Error = "Please enter the OTP.";
                return View(model);
            }

            string email = TempData["VerifyEmail"]?.ToString();
            string name = TempData["Name"]?.ToString();
            string phone = TempData["Phone"]?.ToString();
            string tempOtp = TempData["OTP"]?.ToString();
            string address = TempData["Address"]?.ToString();
            string postalCode = TempData["PostalCode"]?.ToString();
            string city = TempData["City"]?.ToString();

            if (string.IsNullOrEmpty(email))
            {
                // Something expired; ask user to restart
                ViewBag.Error = "Session expired. Please start again.";
                return RedirectToAction("CustomerLogin");
            }

            bool isRegistered = await _customerService.IsEmailRegisteredAsync(email);

            if (isRegistered)
            {
                // Existing user => verify against DB (this method clears OTP in DB on success)
                bool valid = await _customerService.VerifyOtpAsync(email, model.OtpCode);
                if (!valid)
                {
                    // keep the TempData for retry or resend
                    TempData.Keep("VerifyEmail");
                    TempData.Keep("Name");
                    TempData.Keep("Phone");
                    TempData.Keep("OTP");

                    ViewBag.Error = "Invalid OTP. Please try again.";
                    return View();
                }

                // OTP valid => mark user as logged in via Session
                var customer = await _customerService.GetCustomerByEmailAsync(email);
                if (customer != null)
                {
                    HttpContext.Session.SetString("currentUserId", customer.Id.ToString()); //new


                    HttpContext.Session.SetString("CustomerName", customer.Name ?? "");
                    HttpContext.Session.SetString("CustomerEmail", customer.Email);
                }

                return RedirectToAction("ProductView");
            }
            else
            {


                // New user registration flow -> verify OTP against TempData (we haven't written user to DB yet)
                if (model.OtpCode != tempOtp)
                {
                    TempData.Keep("VerifyEmail");
                    TempData.Keep("Name");
                    TempData.Keep("Phone");
                    TempData.Keep("OTP");

                    ViewBag.Error = "Invalid OTP. Please try again.";
                    return View();
                }

                // OTP matches -> save new customer into DB
                var saved = await _customerService.SaveCustomerDetailsAsync(email, name, phone, tempOtp, address, city, postalCode);
                if (!saved)
                {
                    ViewBag.Error = "Registration failed. Please try again.";
                    return View();
                }

                // fetch freshly saved customer and set session
                var customer = await _customerService.GetCustomerByEmailAsync(email);
                if (customer != null)
                {
                    HttpContext.Session.SetString("currentUserId", customer.Id.ToString()); ///neew

                    HttpContext.Session.SetString("CustomerName", customer.Name ?? "");
                    HttpContext.Session.SetString("CustomerEmail", customer.Email);
                }

                return RedirectToAction("ProductView");
            }
        }

        // --- Resend OTP (AJAX POST) ---
        [HttpPost]
        public async Task<IActionResult> ResendOtp()
        {
            string email = TempData["VerifyEmail"]?.ToString();
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Session expired" });
            }

            string newOtp = _customerService.GenerateOtp();

            if (await _customerService.IsEmailRegisteredAsync(email))
            {
                // updates DB and sends email
                await _customerService.UpdateOtpForLoginAsync(email, newOtp);
            }
            else
            {
                // registration flow: just send email (DB write happens after OTP verify)
                _customerService.SendOtpEmail(email, newOtp);
            }

            TempData["OTP"] = newOtp;

            // Keep everything alive for subsequent actions
            TempData.Keep("VerifyEmail");
            TempData.Keep("Name");
            TempData.Keep("Phone");
            TempData.Keep("OTP");

            return Json(new { success = true });
        }

        // --- Logout (clear session) ---
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("CustomerName");
            HttpContext.Session.Remove("CustomerEmail");

            HttpContext.Session.Remove("currentUserId");
            HttpContext.Session.Remove("ProductId");
            return RedirectToAction("ProductView");

        }

        // --- the rest of your controller (SellerLogin, UploadDTO, Cart, AddToCart, etc.)
        // keep your existing methods unchanged below this point so we don't break anything else.


        [HttpPost("/Home/AddWishlist/{id}")]
        public IActionResult AddWishlist(int id)
        {
            string currentUserId = HttpContext.Session.GetString("currentUserId");
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var result = _customerService.ToggleWishlist(id, currentUserId);

            if (result)
            {
                int count = _customerService.GetWishlistCount(currentUserId);
                return Json(new { success = true, count });
            }

            return BadRequest();
        }


        [HttpPost("/Home/RemoveFromWishlist/{id}")]
        public IActionResult RemoveFromWishlist(int id)
        {
            string currentUserId = HttpContext.Session.GetString("currentUserId");
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var result = _customerService.RemoveFromWishlist(id, currentUserId);

            int count = _customerService.GetWishlistCount(currentUserId);

            return Json(new { success = result, count });
        }



        [HttpGet]
        public IActionResult Wishlist()
        {
            string currentUserId = HttpContext.Session.GetString("currentUserId");
            var items = _customerService.GetWishlistProducts(currentUserId);
            return View(items);  // model = List<WishlistDTO>
        }



        [HttpGet("/Home/GetWishlistCount")]
        public IActionResult GetWishlistCount()
        {
            string currentUserId = HttpContext.Session.GetString("currentUserId");
            if (string.IsNullOrEmpty(currentUserId)) return Json(new { count = 0 });

            int count = _customerService.GetWishlistCount(currentUserId);
            return Json(new { count });
        }


        //[HttpGet]
        //public IActionResult MyOrders()
        //{
        //    string currentUserEmail = HttpContext.Session.GetString("CustomerEmail");
        //    if (string.IsNullOrEmpty(currentUserEmail))
        //        return RedirectToAction("CustomerLogin");

        //    var orders = _orderService.GetOrdersByCustomerEmail(currentUserEmail);
        //    return View(orders);
        //}

        // --- My Orders Page ---


        [HttpGet]
        public IActionResult MyOrders()
        {
            var currentUserIdStr = HttpContext.Session.GetString("currentUserId");
            if (string.IsNullOrEmpty(currentUserIdStr))
                return RedirectToAction("CustomerLogin");

            int customerId = int.Parse(currentUserIdStr);
            var orders = _orderService.GetOrdersByCustomerId(customerId);
            return View(orders);
        }



        // --- Order Details Page ---
        [HttpGet]
        public IActionResult OrderDetails(string orderId)
        {
            string currentUserEmail = HttpContext.Session.GetString("CustomerEmail");
            if (string.IsNullOrEmpty(currentUserEmail))
                return RedirectToAction("CustomerLogin");

            var orderDetails = _orderService.GetOrderDetailsById(orderId, currentUserEmail);
            if (orderDetails == null)
                return RedirectToAction("MyOrders"); // fallback if not found

            return View(orderDetails);
        }


        public async Task<ActionResult> CustomerDashboard()
        {
            var email = HttpContext.Session.GetString("CustomerEmail"); // ✅ use Session, not Identity

            if (string.IsNullOrEmpty(email))
            {
                // Redirect to login if not logged in
                return RedirectToAction("CustomerDashBoard", "Home");
            }

            var model = await _customerService.GetDashboardAsync(email);
            return View(model);
        }

        [HttpPost]

        public async Task <ActionResult> UpdateProfile(CustomerDashBoardDTO dto)
        {
            var success = await _customerService.UpdateProfileAsync( dto.EmailAddress,dto.Name, dto.Phone);

     
            return Json(new { success, message = success ? "Profile updated" : "Failed to update" });
        }

        [HttpPost]

        public async Task<ActionResult> UpdateAddress(CustomerDashBoardDTO dto)
        {

            var success = await _customerService.UpdateAddressAsync(dto.EmailAddress, dto.Address,dto.City, dto.PostalCode);
            return Json(new { success, message = success ? "Address updated" : "Failed to update" });
        }


    }

}