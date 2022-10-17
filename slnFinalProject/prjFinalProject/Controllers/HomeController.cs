using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PagedList;
using System.Web.Security;
using prjFinalProject.Models;

namespace prjFinalProject.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        NationalParkEntities db = new NationalParkEntities();
        int pageSize = 3;

        // GET: Home

        // [Authorize] 設定此屬性，則Wang, Lin, Chen 都可執行 Index 動作方法
        [Authorize(Users = "Wang, Lin, Chen")]
        public ActionResult Index(int page = 1)
        {
            int currentPage = page < 1 ? 1 : page;
            var np = db.TableNPs1091761.ToList();
            var result = np.ToPagedList(currentPage, pageSize);
            return View(result);
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string txtUid, string txtPwd)
        {
            string[] uidAry = new string[] { "Wang", "Lin", "Chen" };
            string[] pwdAry = new string[] { "0000", "6666", "8888" };

            // 循序搜尋法
            int index = -1;
            for (int i = 0; i < uidAry.Length; i++)
            {
                if (uidAry[i] == txtUid && pwdAry[i] == txtPwd)
                {
                    index = i;
                    break;
                }
            }
            if (index == -1)
            {
                ViewBag.Err = "帳密錯誤!";
            }
            else
            {
                // 表單驗證服務，授權指定的帳號
                FormsAuthentication.RedirectFromLoginPage(txtUid, true);
                return RedirectToAction("Index");
            }
            return View();
        }

        [Authorize(Users = "Wang, Lin, Chen")]
        public ActionResult City(string city = "臺北市")
        {
            var result = (from m in db.TableNPs1091761
                          where m.縣市 == city
                          select m).ToList();
            return View(result);
        }

        public ActionResult Edit(int Count)
        {
            var park = db.TableNPs1091761
                .Where(m => m.編號 == Count).FirstOrDefault();
            return View(park);
        }
        [HttpPost]
        public ActionResult Edit(TableNPs1091761 num)
        {
            if (ModelState.IsValid)
            {
                var temp = (from m in db.TableNPs1091761
                            where m.編號 == num.編號
                            select m).FirstOrDefault();
                temp.編號 = num.編號;
                temp.名稱 = num.名稱;
                temp.縣市 = num.縣市;
                temp.地址 = num.地址;
                temp.電話 = num.電話;
                temp.網址 = num.網址;
                temp.資源特色 = num.資源特色;
                temp.介紹 = num.介紹;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(num);
        }

        public ActionResult Delete(int Number)
        {
            var numder = (from m in db.TableNPs1091761
                          where m.編號 == Number
                          select m).FirstOrDefault();
            db.TableNPs1091761.Remove(numder);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Users = "Wang, Lin, Chen")]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(TableNPs1091761 NPark)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Error = false;

                var temp = (from m in db.TableNPs1091761
                            where m.編號 == NPark.編號
                            select m)
                    .FirstOrDefault();
                if (temp != null)
                {
                    ViewBag.Error = true;
                    return View(NPark);
                }
                db.TableNPs1091761.Add(NPark);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(NPark);
        }

        [Authorize(Users = "Wang, Lin, Chen")]
        public ActionResult Number()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Number(int txtNum)
        {
            ViewBag.txtNum = txtNum;
            return RedirectToAction("NumResult", new { txtNum = txtNum });
        }

        public ActionResult NumResult(int txtNum)
        {
            ViewData["txtNum"] = txtNum;
            var result = (from m in db.TableNPs1091761
                          where m.編號 == txtNum
                          select m).ToList();
            return View(result);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();   // 登出
            return RedirectToAction("Login");
        }
    }
}