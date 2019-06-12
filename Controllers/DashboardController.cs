using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CSCLite.Models;
using Newtonsoft.Json.Linq;
using PagedList;
using System.Timers;
using System.Diagnostics;

namespace CSCLite.Controllers
{

    public class DashboardController : Controller
    {

        private CONN_CSCLITE db = new CONN_CSCLITE();

        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }
        
        // GET: Dashboard

        public ActionResult Index()
        {

            Session["session_users_password"] = "admin@gmail.com";
            Session["session_id"] = "123";
            Session["session_users"] = "admin@gmail.com";

            var login_users_password = this.Session["session_users_password"];
            ViewBag.username = login_users_password;
            if (login_users_password != null)
            {
             
                ViewBag.DLTChassis_InvoiceNot = db.DLTChassisInvoices.Where(a => a.INVOICE_NO == null && a.STATUS_DLT == 1).Count();
                ViewBag.DLTChassis_InvoiceOr = db.DLTChassisInvoices.Where(a => a.INVOICE_NO != null && a.YEAR_LOSE != 0 && a.YEAR_LOSE > 0 && a.STATUS_DLT == 1).Count();
                ViewBag.DLTChassis_InvoiceSc = db.DLTChassisInvoices.Where(a => a.INVOICE_NO != null && a.YEAR_LOSE == 0 && a.STATUS_DLT == 1).Count();
                ViewBag.DLTChassis_InvoiceNext = db.DLTChassisInvoices.Where(a => a.INVOICE_COUNT_LOSE != null && a.YEAR_LOSE < 0 && a.STATUS_DLT == 1).Count();
                ViewBag.table_Chassis = db.DLTChassisImports.Count();
                ViewBag.table_license = db.DLTLicenseImports.Count();
                ViewBag.table_Mei = db.DLTMeiImports.Count();
                var tb_grpup = db.DLTChassisInvoices
                    .GroupBy(i => i.INSTALL_YEAR)
                    .Select(group => new { year_group = group.Key });


                List<string> ls_year = new List<string>();

                foreach (var year_sort in tb_grpup)
                {
                    String year_split = year_sort.ToString().Split('=')[1].Split(' ')[1];

                    ls_year.Add(year_split);
                }

                ViewBag.tb_group_year = ls_year.OrderBy(item => item);



               return View(db.DLTChassisInvoices);
            }
           return Redirect("/Users/index");
        }
    }
}