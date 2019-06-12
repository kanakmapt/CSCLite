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
using System.IO;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Web.UI.WebControls;


namespace CSCLite.Controllers
{
    
    public class UpdateInfoController : Controller
    {
        private CONN_CSCLITE db = new CONN_CSCLITE();
        // GET: UpdateInfo
        public ActionResult Index( string INOVICE_NAME,string INOVICE_DAY ,string INOVICE_MONTH, string INOVICE_YEAR, string INOVICE_NO)
        {

            var ChassisInvoice_Table = db.DLTInvoices.ToList();
            var ChassisInvoice_Table_Groupby = db.DLTInvoices.GroupBy(r=>r.InvoiceCustomerName).ToList();
            ViewBag.Customer = ChassisInvoice_Table_Groupby.Select(r => r.Key).ToList();
            ViewBag.Invoice_ = ChassisInvoice_Table.Select(r=>r.InvoiceNO).ToList();
            if (string.IsNullOrEmpty(INOVICE_NAME) && string.IsNullOrEmpty(INOVICE_DAY) && string.IsNullOrEmpty(INOVICE_MONTH) && string.IsNullOrEmpty(INOVICE_YEAR) && string.IsNullOrEmpty(INOVICE_NO))
            {
                ChassisInvoice_Table = ChassisInvoice_Table.Where(r => r.ID == 0).ToList();
            }
            if (!string.IsNullOrEmpty(INOVICE_NO))
            {
                ChassisInvoice_Table = ChassisInvoice_Table.Where(r => r.InvoiceNO == INOVICE_NO).ToList();
            }

            if (!string.IsNullOrEmpty(INOVICE_NAME)) {
                ChassisInvoice_Table = ChassisInvoice_Table.Where(r => r.InvoiceCustomerName == INOVICE_NAME).ToList();
            }
            if (!string.IsNullOrEmpty(INOVICE_DAY))
            {
                ChassisInvoice_Table = ChassisInvoice_Table.Where(r => r.InvoiceOpenDate.EndsWith(INOVICE_DAY)).ToList();
            }
            if (!string.IsNullOrEmpty(INOVICE_MONTH))
            {
                ChassisInvoice_Table = ChassisInvoice_Table.Where(r => r.InvoiceMonth == INOVICE_MONTH).ToList();
            }
            if (!string.IsNullOrEmpty(INOVICE_YEAR))
            {
                ChassisInvoice_Table = ChassisInvoice_Table.Where(r => r.InvoiceYear == INOVICE_YEAR).ToList();
            }
           
            ViewBag.DLTInvoice = ChassisInvoice_Table.Select(r=>r.InvoiceNO).ToList();
       
            return View();
        }
    }
}