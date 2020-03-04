using Emarketing.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace Emarketing.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin

        EmarketingEntities db = new EmarketingEntities();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Tadmin avm)
        {
            Tadmin ad = db.Tadmin.Where(x => x.admin_username == avm.admin_username && x.admin_password == avm.admin_password).SingleOrDefault();
            if (ad != null)
            {
                Session["admin_id"] = ad.admin_id.ToString();
                return RedirectToAction("Create");

            }
            else {
                ViewBag.error = "Invalid username or password";

            }
            return View();
        }
        
        public ActionResult Create()
        {
            if (Session["admin_id"] == null) {
                return RedirectToAction("Login");
                
            }

            return View();
        }
        [HttpPost]
        public ActionResult Create(Tcategory cvm,HttpPostedFileBase imgfile)
        {
            string path = uploadimgfile(imgfile);
            if(path.Equals("-1"))
            {
                ViewBag.error = "Image couldnt upload";
            }
            else
            {
                Tcategory category = new Tcategory();
                category.category_name = cvm.category_name;
                category.category_image = path;
                category.category_status = 1;
                category.category_fk_admin = Convert.ToInt32(Session["admin_id"].ToString());
                db.Tcategory.Add(category);
                db.SaveChanges();
                return RedirectToAction("ViewCategory");

            }
            return View();

        }
        public ActionResult ViewCategory(int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.Tcategory.Where(x => x.category_status == 1).OrderByDescending(x => x.category_id).ToList();
            IPagedList<Tcategory> stu = list.ToPagedList(pageindex, pagesize);

            return View(stu);

        }
        public ActionResult Delete(int? id)
        {
            Tcategory tcategory = db.Tcategory.Where(x => x.category_id == id).SingleOrDefault();
            db.Tcategory.Remove(tcategory);
            db.SaveChanges();
            return RedirectToAction("ViewCategory");
        }
        public ActionResult ViewUser()
        {
            List<Tuser> users = db.Tuser.ToList();
            if(users!=null && users.Count > 0) { return View(users); }
            else return View();
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