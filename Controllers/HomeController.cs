using FamsEcommerce.Context;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FamsEcommerce.Controllers
{
    public class HomeController : Controller
    {
        FamsDBEntities dbObj= new FamsDBEntities();
       
        public ActionResult Index()
        {
            if (User.IsInRole("admin"))
            {
                return View("Admin");
            }
            else
            {

                var cart = dbObj.CartTables.ToList();
                var product = dbObj.Products.ToList();
                var viewModel = new ProductCartViewModel
                {
                    Products = product,
                    CartItems = cart
                };
                return View(viewModel);
            }
          
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = dbObj.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        [Authorize(Roles ="admin")]
        public ActionResult Admin()
        {

            return View();
        }
        public ActionResult  Catagory()
        {

            return View();
        }
        //public ActionResult Cart(int id, CartTable model)
        //{
        //    int productId = id; // Assuming the ID is provided as a parameter

        //    // Load the related AspNetUser entity
        //    model.AspNetUser = dbObj.AspNetUsers.Find(model.User_id);

        //    // Retrieve product details from the Products table based on the provided product_id
        //    Product selectedProduct = dbObj.Products.Find(productId);

        //    if (selectedProduct != null && model.AspNetUser != null)
        //    {
        //        CartTable cartItem = new CartTable
        //        {
        //            Name = model.AspNetUser.Name,
        //            Email = model.AspNetUser.Email,
        //            Address = model.AspNetUser.Address,
        //            Phone = model.AspNetUser.Phone,
        //            P_Desc = selectedProduct.P_Description,
        //            P_img = selectedProduct.P_Image,
        //            P_Price = selectedProduct.P_Price,
        //            P_Qyt = model.P_Qyt,
        //            User_id = model.AspNetUser.Id,
        //            product_id = selectedProduct.P_Id
        //        };

        //        dbObj.CartTables.Add(cartItem);
        //        dbObj.SaveChanges();

        //        return RedirectToAction("Index");
        //    }

        //    // If the product or user was not found
        //    // Handle the error appropriately
        //    return RedirectToAction("Error");
        //}
        [HttpPost]
        [Authorize]
        public ActionResult Addtocard(int id, int P_Qyt)
        {
            Product product = dbObj.Products.FirstOrDefault(x => x.P_Id == id);

            // Assuming you're using ASP.NET Identity for authentication
            string userId = User.Identity.GetUserId();
            AspNetUser user = dbObj.AspNetUsers.FirstOrDefault(x => x.Id == userId);

            if (product != null && user != null)
            {
                CartTable cartItem = new CartTable
                {
                    Name = user.Name,
                    Email = user.Email,
                    Address = user.Address,
                    Phone = user.Phone,
                    P_Desc = product.P_Description,
                    P_img = product.P_Image,
                    
                   
                    P_Qyt = P_Qyt,
                    User_id = user.Id,
                    product_id = product.P_Id
                };
                if (product.P_Disc_Price != 0)
                {
                    cartItem.P_Price = product.P_Disc_Price; // Use discount price
                }
                else
                {
                    cartItem.P_Price = product.P_Price; // Use regular price
                }

                dbObj.CartTables.Add(cartItem);
                dbObj.SaveChanges();

                return RedirectToAction("Index");
            }

            // If the product or user was not found
            // Handle the error appropriately
            return RedirectToAction("Error");
        }
      
        [Authorize]
        [HttpPost]
        public ActionResult Order()
        {
            string userId = User.Identity.GetUserId();
            AspNetUser user = dbObj.AspNetUsers.FirstOrDefault(x => x.Id == userId);
            var cartItems = dbObj.CartTables.Where(x => x.User_id == userId).ToList();

            foreach (var cartItem in cartItems)
            {
               

                Order orderItem = new Order
                {
                    Name = user.Name,
                    Email = user.Email,
                    Address = user.Address,
                    Phone = user.Phone,
                    P_Desc = cartItem.P_Desc,
                    P_Img = cartItem.P_img,
                    P_Qty = cartItem.P_Qyt,
                    P_Price=cartItem.P_Price,
                    Delivery_Status="panding",
                    Payment_Status="Remaining",
                    User_id = user.Id,
                    product_id = cartItem.product_id
                    
                };

              
                // Add the order entry to the Order table
                dbObj.Orders.Add(orderItem);
            }

            // Save changes to the database
            dbObj.SaveChanges();

            // After moving items to the order table, you can optionally clear the cart table
            foreach (var cartItem in cartItems)
                dbObj.CartTables.RemoveRange(cartItems);
            dbObj.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Showcard()
        {
            string userId = User.Identity.GetUserId();
            AspNetUser user = dbObj.AspNetUsers.FirstOrDefault(x => x.Id == userId);

            // Retrieve cart items for the current user
            var cartItems = dbObj.CartTables.Where(x => x.User_id == userId).ToList();

            return View(cartItems);
        }





        [HttpPost]  
        public ActionResult AddCatagory(Catagory model)
        {
          if(ModelState.IsValid)
            {
                Catagory obj = new Catagory();
                obj.Catagory_name = model.Catagory_name;
                dbObj.Catagories.Add(obj);
                dbObj.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult ShowCatagory()
        {
            var res=dbObj.Catagories.ToList();
            return View(res);
        }
        public ActionResult DeleteCard(int id)
        {
            var res = dbObj.CartTables.Where(x => x.Id == id).First();
            dbObj.CartTables.Remove(res);
            dbObj.SaveChanges();
            var list = dbObj.CartTables.ToList();
            return View("Showcard", list);
        }
        public ActionResult Delete(int id)
        {
            var res = dbObj.Catagories.Where(x=>x.Id==id).First();
            dbObj.Catagories.Remove(res);
            dbObj.SaveChanges();
            var list = dbObj.Catagories.ToList();
            return View("ShowCatagory", list);
        }
        public ActionResult ShowOrder()
        {
            var res = dbObj.Orders.ToList();
            return View(res);
        }

        public ActionResult DeliverOrder(int id)
        {
            var order = dbObj.Orders.Find(id);
            
                order.Delivery_Status = "Delivered"; // Correct the status spelling if needed
                dbObj.SaveChanges();
            
            return RedirectToAction("ShowOrder");
        }


    }
}