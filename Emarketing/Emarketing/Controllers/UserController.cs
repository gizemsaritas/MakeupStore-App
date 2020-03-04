using Emarketing.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Emarketing.Controllers
{
    public class UserController : Controller
    {
        EmarketingEntities db = new EmarketingEntities();
        // GET: User
        public ActionResult Index(int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.Tcategory.Where(x => x.category_status == 1).OrderByDescending(x => x.category_id).ToList();
            IPagedList<Tcategory> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }
        [HttpPost]
        public ActionResult Index(int? id, int? page, string search)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.Tcategory.Where(x => x.category_name.Contains(search)).OrderByDescending(x => x.category_id).ToList();
            IPagedList<Tcategory> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(Tuser uvm,HttpPostedFileBase imgfile)
        {
            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image couldnt upload";
            }
            else
            {
                Tuser user = new Tuser();
                user.Tuser_name = uvm.Tuser_name;
                user.Tuser_email = uvm.Tuser_email;
                user.Tuser_password = uvm.Tuser_password;
                user.Tuser_image =path;
                user.Tuser_contact = uvm.Tuser_contact;
                db.Tuser.Add(user);
                db.SaveChanges();
                return RedirectToAction("Login");

            }
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Tuser avm)
        {
            Tuser user = db.Tuser.Where(x => x.Tuser_email == avm.Tuser_email && x.Tuser_password == avm.Tuser_password).SingleOrDefault();
            if (user != null)
            {
                Session["Tuser_id"] = user.Tuser_id.ToString();
                List<Tproduct> products = new List<Tproduct>();
                Session["products"] = products;
                return RedirectToAction("index");

            }
            else
            {
                ViewBag.error = "Invalid username or password";
            }
            return View();
        }
        [HttpGet]
        public ActionResult CreateProduct()
        {
            List<Tcategory> list = db.Tcategory.ToList();
            ViewBag.categorylist = new SelectList(list, "category_id", "category_name");
            return View();
        }
       
        [HttpPost]
        public ActionResult CreateProduct(Tproduct pvm,HttpPostedFileBase imgfile)
        {
            List<Tcategory> list = db.Tcategory.ToList();
            ViewBag.categorylist = new SelectList(list, "category_id", "category_name");

            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image couldnt upload";
            }
            else
            {
                Tproduct product = new Tproduct();
                product.product_name = pvm.product_name;
                product.product_price = pvm.product_price;
                product.product_image = path;
                product.product_fk_category = pvm.product_fk_category;
                product.product_des = pvm.product_des;
                if (Session["Tuser_id"] != null) { product.product_fk_user = Convert.ToInt32(Session["Tuser_id"].ToString()); }
                
                db.Tproduct.Add(product);
                db.SaveChanges();
                Response.Redirect("index");
            }

                return View();

        }
        public ActionResult ViewProduct(int? id,int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.Tproduct.Where(x=>x.product_fk_category==id).OrderByDescending(x=>x.product_id).ToList();
            IPagedList<Tproduct> stu = list.ToPagedList(pageindex, pagesize);
            var deneme = db.Tproduct.Where(x => x.product_fk_category == id).Select(x => x.product_fk_category).First();
            Session["deneme"]= deneme;
            return View(stu);

        }
        [HttpPost]
        public ActionResult ViewProduct(int? id,int? page, string search)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            int a = (int)Session["deneme"];
            var list = db.Tproduct.Where(x => x.product_name.Contains(search)&&x.product_fk_category==a).OrderByDescending(x => x.product_id).ToList();
            IPagedList<Tproduct> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }
        public ActionResult ProductDetails(int? id)
        {
            ProductDetails productDetails = new ProductDetails();
            Tproduct product = db.Tproduct.Where(x => x.product_id == id).SingleOrDefault();
            productDetails.product_id = product.product_id;
            productDetails.product_name = product.product_name;
            productDetails.product_image = product.product_image;
            productDetails.product_price = product.product_price;
            Tcategory category = db.Tcategory.Where(x => x.category_id == product.product_fk_category).SingleOrDefault();
            productDetails.category_name = category.category_name;
            Tuser user = db.Tuser.Where(x => x.Tuser_id == product.product_fk_user).SingleOrDefault();
            productDetails.Tuser_name = user.Tuser_name;
            productDetails.Tuser_image = user.Tuser_image;
            productDetails.Tuser_contact = user.Tuser_contact;
            productDetails.product_fk_user = user.Tuser_id;
            return View(productDetails);
        }
       public ActionResult SignOut()
        {
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("index");
        }
        public ActionResult DeleteProduct(int? id)
        {
            Tproduct product = db.Tproduct.Where(x => x.product_id == id).SingleOrDefault();
            db.Tproduct.Remove(product);
            db.SaveChanges();
            return RedirectToAction("index");
        }
        public ActionResult AddtoCart(int? id) {
            Tproduct product = db.Tproduct.Where(x => x.product_id == id).SingleOrDefault();
            var idList = Session["products"] as List<Tproduct>;
            idList.Add(product);
            Session["products"] = idList;
            return RedirectToAction("index");
        }
        public ActionResult BuyProduct(int? id)
        {
            var productList = Session["products"] as List<Tproduct>;
            return View(productList);
        }
        public string uploadimgfile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {
                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);

                        //    ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....'); </script>");
                }
            }

            else
            {
                Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }



            return path;
        }
        
    }
}