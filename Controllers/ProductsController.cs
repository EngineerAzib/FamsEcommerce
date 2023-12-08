using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using FamsEcommerce.Context;

namespace FamsEcommerce.Controllers
{
    public class ProductsController : Controller
    {
        private FamsDBEntities db = new FamsDBEntities();

        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Catagory);
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.P_Catagory = new SelectList(db.Catagories, "Id", "Catagory_name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "P_Id,P_Description,P_Price,P_Quantity,P_Disc_Price,P_Catagory,P_Image")] Product product,HttpPostedFileBase ImageFile)
        {
            string filename=Path.GetFileNameWithoutExtension(ImageFile.FileName);
            string extension=Path.GetExtension(ImageFile.FileName);
            filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
            string imagePath = Path.Combine(Server.MapPath("~/All_Image/"), filename);
            ImageFile.SaveAs(imagePath);
            product.P_Image = "~/All_Image/" + filename;
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.P_Catagory = new SelectList(db.Catagories, "Id", "Catagory_name", product.P_Catagory);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.P_Catagory = new SelectList(db.Catagories, "Id", "Catagory_name", product.P_Catagory);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "P_Id,P_Description,P_Price,P_Quantity,P_Disc_Price,P_Catagory,P_Image")] Product product,HttpPostedFileBase ImageFile)
        {
            string filename = Path.GetFileNameWithoutExtension(ImageFile.FileName);
            string extension = Path.GetExtension(ImageFile.FileName);
            filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
            string imagePath = Path.Combine(Server.MapPath("~/All_Image/"), filename);
            ImageFile.SaveAs(imagePath);
            product.P_Image = "~/All_Image/" + filename;
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.P_Catagory = new SelectList(db.Catagories, "Id", "Catagory_name", product.P_Catagory);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
