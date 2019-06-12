

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
using System.Drawing;

namespace CSCLite.Controllers
{
    public class ReportController : Controller
    {


        private CONN_CSCLITE db = new CONN_CSCLITE();

        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }
        // GET: Report

        public class group_array_invoice
        {
            public string ID_CHASSIS { get; set; }
            public string ID_INVOICE { get; set; }
            public string DATE_YEAR { get; set; }
            public string YEAR { get; set; }
            public string DATE_INVOICE { get; set; }
            public string YEAR_INVOICE { get; set; }
            public string PRICE_INVOICE { get; set; }
            public int COUNT { get; set; }
            public string YEAR_DUP { get; set; }
        }

        public class ls_year_report
        {

            public string year { get; set; }
            public string count { get; set; }

        }
        public class group_dup
        {

            public string ID { get; set; }
            public string YEAR { get; set; }

        }
        public ActionResult Index_Mei(
                         String CHASSIS_SEARCH,
                        String CUSTOMER_SEARCH,
                        String DELALER_SEARCH,
                        String STATUS_SEARCH,
                        String ORDER_SEARCH,
                        String MAXMIN_SEARCH,
                        String MONTH_SEARCH,
                        String YEAR_SEARCH,
                        String STARTDAY_SEARCH,
                        String ENDDAY_SEARCH,
                        String INV_CUSTOMER_SEARCH,
                        int? page,
                        int? pageSize,
                        string SORTSQL,
                        string SORTAD,
                        string LICENSE_SEARCH,
                        string MEI_SEARCH,
                        string INVOICE_NO_SEARCH
                    )
        {

            Session["session_users_password"] = "admin@gmail.com";
            Session["session_id"] = "123";
            Session["session_users"] = "admin@gmail.com";

            ViewBag.Total_Customer = "0";
            ViewBag.Total_Dealer = "0";
            ViewBag.Total_Customer = "";
            ViewBag.Total_Dealer = "";
            ViewBag.Total_Price_New = "";
            ViewBag.CHASSIS_SEARCH = "";
            ViewBag.CUSTOMER_SEARCH = "";
            ViewBag.DELALER_SEARCH = "";
            ViewBag.STATUS_SEARCH = "";
            ViewBag.ORDER_SEARCH = "";
            ViewBag.MAXMIN_SEARCH = "";
            ViewBag.MONTH_SEARCH = "";
            ViewBag.YEAR_SEARCH = "";
            ViewBag.STARTDAY_SEARCH = "";
            ViewBag.ENDDAY_SEARCH = "";
            ViewBag.userid = 0;
            string Staring_LICENSE = "";
            string StaringMEI = "";
            decimal mod = 0;

            var login_users_password = this.Session["session_users_password"];
            var login_session_id = this.Session["session_id"];


            ViewBag.username = login_users_password;
            ViewBag.userid = login_session_id;
            string Start_year = "2016";
            if (string.IsNullOrEmpty(Start_year))
            {
                Start_year = "2016";
            }


            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            int defaSize = (pageSize ?? 10);
            ViewBag.psize = defaSize;
            TotalItems = 0;
            CurrentPage = 0;
            TotalPages = 0;
            StartPage = 0;
            EndPage = 0;

            if (pageSize == null)
            {
                pageSize = 0;
            }

            if (pageSize == 0)
            {
                pageSize = 10;
            }
            ViewBag.PageSize = new List<SelectListItem>()
         {

             new SelectListItem() { Value="10", Text= "10" },
             new SelectListItem() { Value="15", Text= "15" },
             new SelectListItem() { Value="25", Text= "25" },
             new SelectListItem() { Value="50", Text= "50" },
         };

            List<ls_year_report> tbls_year_report = new List<ls_year_report>();
            IPagedList<DLTChassisInvoice_Mei> ChassisInvoice_Table = null;
            string param = "ID";

            ChassisInvoice_Table = db.DLTChassisInvoice_Mei.Where(r=>r.MEI_NOT_FOUND==1 && r.STATUS_DLT==1).OrderBy(r => r.ID).ToPagedList(pageIndex, defaSize);

            Regex charsToDestroy = new Regex(@"[^\d|\,\-]");

            var ChassisInvoiceTable_MEI = db.DLTChassisInvoice_Mei.Where(r => r.MEI_NOT_FOUND == 1 && r.STATUS_DLT == 1).ToList();
            string myStringOutput = String.Join(",", db.DLTDetailChassis.Select(p => p.INVOICE_FLAG.ToString()).ToArray());
            //   DateTime data_time = Convert.ToDateTime(ChassisInvoiceTable_MEI.Where(r => r.ID == 481382).FirstOrDefault().INVOICE_NO_DATE.Split('(')[1].Split(')')[0]);

            // Convert.ToDateTime(INVOICE_NO_DATE.Split('(')[1].Split(')')[0])
            var ChassisInvoiceTable = from dltcha in ChassisInvoiceTable_MEI
                                      join dltdetail in db.DLTDetailChassis on dltcha.ID_CHASSIS equals dltdetail.ID_CHASSIS into d
                                      from table in d.DefaultIfEmpty()

                                      select new Chassis_Status_Model
                                      {
                                          STATUS_FLAG = table == null ? "" : table.STATUS_FLAG,
                                          ID_CHASSIS = dltcha.ID_CHASSIS,
                                          INVOICE_COUNT =
                                          table == null ? dltcha.INVOICE_COUNT : (Convert.ToInt32(dltcha.INVOICE_COUNT.ToString()) + table.INVOICE_FLAG.Split(',').Count()).ToString(),
                                          INVOICE_NO = table == null ? dltcha.INVOICE_NO : dltcha.INVOICE_NO + String.Join(",", db.DLTDetailChassis.Where(r => r.ID_CHASSIS == dltcha.ID_CHASSIS).Select(p => p.INVOICE_FLAG.ToString()).ToArray()).ToString(),
                                          INVOICE_COUNT_LOSE = dltcha.INVOICE_COUNT_LOSE,
                                          YEAR_LOSE = Convert.ToInt32(dltcha.YEAR_LOSE),
                                          INSTALL_DATE = dltcha.INSTALL_DATE,
                                          DEALER_CONTACT_NAME = dltcha.DEALER_CONTACT_NAME,
                                          DEALER_CONTACT_PHONE = dltcha.DEALER_CONTACT_PHONE,
                                          ID_LICENSE = dltcha.ID_LICENSE,
                                          ID_MEI = dltcha.ID_MEI,
                                          ID_LICENSE_L = dltcha.CER_LICENSE,
                                          ID_MEI_L = dltcha.CER_MEI,
                                          INVOICE_NO_DATE = table == null ? dltcha.INVOICE_NO_DATE : dltcha.INVOICE_NO_DATE + String.Join(",", db.DLTDetailChassis.Where(r => r.ID_CHASSIS == dltcha.ID_CHASSIS).Select(p => p.UP_DATE_INVOICE_FLAG.ToString()).ToArray()).ToString() + ",",
                                          INSTALL_YEAR = dltcha.INSTALL_YEAR,
                                          INVOICE_PRICE = table == null ? dltcha.INVOICE_PRICE : dltcha.INVOICE_PRICE + String.Join(",", db.DLTDetailChassis.Where(r => r.ID_CHASSIS == dltcha.ID_CHASSIS).Select(p => p.UP_DATE_PRICE_FLAG.ToString()).ToArray()).ToString(),
                                          CUSTOMER_NAME = dltcha.CUSTOMER_NAME,
                                          CUSTOMER_PHONE = dltcha.CUSTOMER_PHONE,
                                          INVOICE_CUSTOMER = dltcha.INVOICE_CUSTOMER,
                                          INVOICE_CUSTOMER_PHONE = dltcha.INVOICE_CUSTOMER_PHONE,
                                          ID = dltcha.ID,
                                          CER_LICENSE = dltcha.CER_LICENSE,
                                          INSTALL_DATE_ = Convert.ToDateTime(dltcha.INSTALL_DATE),
                                          MONTH_SEARCH = Convert.ToDateTime(dltcha.INSTALL_DATE).Month.ToString(),
                                          NOT_FOUND_IME = dltcha.MEI_NOT_FOUND

                                      };

            //   ChassisInvoiceTable= ChassisInvoiceTable.Where(r => r.NOT_FOUND_IME == 1).Select(c => { c.ID_MEI = null; return c; }).ToList();
            //   ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.NOT_FOUND_IME == 1).Select(c => { c.INVOICE_NO_DATE = null; return c; }).ToList();
            //    ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.NOT_FOUND_IME == 1).Select(c => { c.ID_LICENSE = null; return c; }).ToList();
            //    ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.NOT_FOUND_IME == 1).Select(c => { c.INVOICE_NO = null; return c; }).ToList();
            //  ChassisInvoiceTable =  ChassisInvoiceTable.Where(r => r.NOT_FOUND_IME == 1).Select(c => { c.INVOICE_COUNT = "0"; return c; }).ToList();
            var tb_grpup = db.DLTChassisInvoice_Mei
                .GroupBy(i => i.INSTALL_YEAR)
                .Select(group => new { year_group = group.Key });
            List<string> ls_year = new List<string>();
        
            foreach (var year_sort in tb_grpup)
            {
      
                String year_split = year_sort.ToString().Split('=')[1].Split(' ')[1];

                ls_year.Add(year_split);
            }


            foreach (var year_sort in tb_grpup)
            {
                String year_split = year_sort.ToString().Split('=')[1].Split(' ')[1];

                if (!string.IsNullOrEmpty(year_split))
                {
                    if (Convert.ToInt32(year_split) >= Convert.ToInt32(Start_year))
                    {
                    
                        int total_count = Convert.ToInt32(DateTime.Now.Year.ToString()) - Convert.ToInt32(Start_year) + 2;
                        tbls_year_report.Add(new ls_year_report { year = year_split, count = total_count.ToString() });
                    }
                }
            }
      
            ViewBag.tb_group_year_report = tbls_year_report.ToList().OrderBy(i => i.year);
            var group_year_report = tbls_year_report.ToList().OrderBy(i => i.year);
            ViewBag.tb_group_year = ls_year.OrderBy(item => item);
            List<group_array_invoice> list_group_array_invoice = new List<group_array_invoice>();
            List<group_dup> list_group_dup = new List<group_dup>();
            list_group_array_invoice.Clear();

            decimal sum_all = 0;
            decimal sum_outs = 0;
            

            var Export_TB = new System.Data.DataTable();

            Export_TB.Columns.Add("1", typeof(int));
            Export_TB.Columns.Add("2", typeof(string));
            Export_TB.Columns.Add("3", typeof(string));
            Export_TB.Columns.Add("4", typeof(string));
            Export_TB.Columns.Add("5", typeof(string));
            Export_TB.Columns.Add("6", typeof(string));
            Export_TB.Columns.Add("7", typeof(string));
            Export_TB.Columns.Add("8", typeof(string));
            Export_TB.Columns.Add("9", typeof(string));
            Export_TB.Columns.Add("10", typeof(string));
            Export_TB.Columns.Add("11", typeof(string));
            Export_TB.Columns.Add("12", typeof(string));
            Export_TB.Columns.Add("13", typeof(string));
            Export_TB.Columns.Add("14", typeof(string));
            Export_TB.Columns.Add("15", typeof(string));
            Export_TB.Columns.Add("16", typeof(string));
            Export_TB.Columns.Add("17", typeof(string));
            Export_TB.Columns.Add("18", typeof(string));
            Export_TB.Columns.Add("19", typeof(string));
            Export_TB.Columns.Add("20", typeof(string));
            Export_TB.Columns.Add("21", typeof(string));
            Export_TB.Columns.Add("22", typeof(string));
            Export_TB.Columns.Add("23", typeof(string));
            Export_TB.Columns.Add("24", typeof(string));
            Export_TB.Columns.Add("25", typeof(string));
            Export_TB.Columns.Add("26", typeof(string));
            Export_TB.Columns.Add("27", typeof(string));
            Export_TB.Columns.Add("28", typeof(string));
            Export_TB.Columns.Add("29", typeof(int));


            ViewBag.page_this = 1;
            ViewBag.TotalPage = 2;
            ViewBag.CHASSIS_SEARCH = "";
            ViewBag.CUSTOMER_SEARCH = "";
            ViewBag.INV_CUSTOMER_SEARCH = "";
            ViewBag.MONTH_SEARCH = "";
            ViewBag.DELALER_SEARCH = "";
            ViewBag.STATUS_SEARCH = "";
            ViewBag.ORDER_SEARCH = "";
            ViewBag.YEAR_SEARCH = "";
            ViewBag.STARTDAY_SEARCH = "";
            ViewBag.ENDDAY_SEARCH = "";



            /* Begin Search*/
            if (!string.IsNullOrEmpty(CHASSIS_SEARCH))
            {

                ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.ID_CHASSIS == CHASSIS_SEARCH);
                if (ChassisInvoiceTable.Count() == 0)
                {
                    DataRow dr = Export_TB.NewRow();
                    dr[1] = 1;
                    dr[14] = "---ไม่พบข้อมูล---";
                    Export_TB.Rows.Add(dr);
                    return View(Export_TB);

                }
            }
            if (!string.IsNullOrEmpty(INV_CUSTOMER_SEARCH))
            {
                ChassisInvoiceTable = ChassisInvoiceTable.Where(i => i.INVOICE_CUSTOMER != null);
                ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.INVOICE_CUSTOMER.Contains(INV_CUSTOMER_SEARCH));
                if (ChassisInvoiceTable.Count() == 0)
                {
                    DataRow dr = Export_TB.NewRow();
                    dr[1] = 1;
                    dr[14] = "---ไม่พบข้อมูล---";
                    Export_TB.Rows.Add(dr);
                    return View(Export_TB);

                }
            }
            if (!string.IsNullOrEmpty(CUSTOMER_SEARCH))
            {
                ChassisInvoiceTable = ChassisInvoiceTable.Where(i => i.CUSTOMER_NAME != null);
                ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.CUSTOMER_NAME.Contains(CUSTOMER_SEARCH));
                if (ChassisInvoiceTable.Count() == 0)
                {
                    DataRow dr = Export_TB.NewRow();
                    dr[1] = 1;
                    dr[14] = "---ไม่พบข้อมูล---";
                    Export_TB.Rows.Add(dr);
                    return View(Export_TB);

                }
            }

            if (!string.IsNullOrEmpty(DELALER_SEARCH))
            {
                ChassisInvoiceTable = ChassisInvoiceTable.Where(i => i.DEALER_CONTACT_NAME != null);
                ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.DEALER_CONTACT_NAME.Contains(DELALER_SEARCH));
                if (ChassisInvoiceTable.Count() == 0)
                {
                    DataRow dr = Export_TB.NewRow();
                    dr[1] = 1;
                    dr[14] = "---ไม่พบข้อมูล---";
                    Export_TB.Rows.Add(dr);
                    return View(Export_TB);

                }
            }



            if (!string.IsNullOrEmpty(STARTDAY_SEARCH))
            {
                // string c = ChassisInvoiceTable.Where(r => r.ID == 475867).FirstOrDefault().INSTALL_DATE_.ToString();
                DateTime STARTDAY = Convert.ToDateTime(STARTDAY_SEARCH);

                ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.INSTALL_DATE_ >= STARTDAY);
                if (ChassisInvoiceTable.Count() == 0)
                {
                    DataRow dr = Export_TB.NewRow();
                    dr[1] = 1;
                    dr[14] = "---ไม่พบข้อมูล---";
                    Export_TB.Rows.Add(dr);
                    return View(Export_TB);

                }
            }

            if (!string.IsNullOrEmpty(ENDDAY_SEARCH))
            {
                DateTime ENDDAY = Convert.ToDateTime(ENDDAY_SEARCH);
                ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.INSTALL_DATE_ <= ENDDAY);
                if (ChassisInvoiceTable.Count() == 0)
                {
                    DataRow dr = Export_TB.NewRow();
                    dr[1] = 1;
                    dr[14] = "---ไม่พบข้อมูล---";
                    Export_TB.Rows.Add(dr);
                    return View(Export_TB);

                }
            }
            if (!string.IsNullOrEmpty(MONTH_SEARCH))
            {

                ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.MONTH_SEARCH == MONTH_SEARCH);
                if (ChassisInvoiceTable.Count() == 0)
                {
                    DataRow dr = Export_TB.NewRow();
                    dr[1] = 1;
                    dr[14] = "---ไม่พบข้อมูล---";
                    Export_TB.Rows.Add(dr);
                    return View(Export_TB);

                }
            }

            if (!string.IsNullOrEmpty(YEAR_SEARCH))
            {

                ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.INSTALL_YEAR == YEAR_SEARCH);
                if (ChassisInvoiceTable.Count() == 0)
                {
                    DataRow dr = Export_TB.NewRow();
                    dr[1] = 1;
                    dr[14] = "---ไม่พบข้อมูล---";
                    Export_TB.Rows.Add(dr);
                    return View(Export_TB);

                }
            }

            if (!string.IsNullOrEmpty(LICENSE_SEARCH))
            {
                ChassisInvoiceTable = ChassisInvoiceTable.Where(i => i.ID_LICENSE != null);
                ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.ID_LICENSE.Contains(LICENSE_SEARCH));
                if (ChassisInvoiceTable.Count() == 0)
                {
                    DataRow dr = Export_TB.NewRow();
                    dr[1] = 1;
                    dr[14] = "---ไม่พบข้อมูล---";
                    Export_TB.Rows.Add(dr);
                    return View(Export_TB);

                }
            }
            if (!string.IsNullOrEmpty(MEI_SEARCH))
            {
                ChassisInvoiceTable = ChassisInvoiceTable.Where(i => i.ID_MEI != null);
                ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.ID_MEI.Contains(MEI_SEARCH));
                if (ChassisInvoiceTable.Count() == 0)
                {
                    DataRow dr = Export_TB.NewRow();
                    dr[1] = 1;
                    dr[14] = "---ไม่พบข้อมูล---";
                    Export_TB.Rows.Add(dr);
                    return View(Export_TB);

                }
            }
            if (!string.IsNullOrEmpty(INVOICE_NO_SEARCH))
            {

                ChassisInvoiceTable = ChassisInvoiceTable.Where(i => i.INVOICE_NO != null);
                ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.INVOICE_NO.Contains(INVOICE_NO_SEARCH));
                if (ChassisInvoiceTable.Count() == 0)
                {
                    DataRow dr = Export_TB.NewRow();
                    dr[1] = 1;
                    dr[14] = "---ไม่พบข้อมูล---";
                    Export_TB.Rows.Add(dr);
                    return View(Export_TB);

                }
            }



            ViewBag.TotalPage = ChassisInvoiceTable.Count() / pageSize;
            /* END Search*/

            /* Begin Sort*/
            if (SORTSQL == "ID_CHASSIS")
            {
                if (SORTAD == "ASC")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderBy(r => r.ID_CHASSIS);
                }
                if (SORTAD == "DES")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderByDescending(r => r.ID_CHASSIS);
                }
            }
            if (SORTSQL == "MEI")
            {
                if (SORTAD == "ASC")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderBy(r => r.ID_MEI);
                }
                if (SORTAD == "DES")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderByDescending(r => r.ID_MEI);
                }
            }
            if (SORTSQL == "ID_LICENSE")
            {
                if (SORTAD == "ASC")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderBy(r => r.ID_LICENSE);
                }
                if (SORTAD == "DES")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderByDescending(r => r.ID_LICENSE);
                }
            }
            if (SORTSQL == "INSTALL_DATE")
            {
                if (SORTAD == "ASC")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderBy(r => r.INSTALL_DATE);
                }
                if (SORTAD == "DES")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderByDescending(r => r.INSTALL_DATE);
                }
            }
            if (SORTSQL == "INVOICE_COUNT")
            {
                if (SORTAD == "ASC")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderBy(r => r.INVOICE_COUNT);
                }
                if (SORTAD == "DES")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderByDescending(r => r.INVOICE_COUNT);
                }
            }
            if (SORTSQL == "CUSTOMER_NAME")
            {
                if (SORTAD == "ASC")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderBy(r => r.CUSTOMER_NAME);
                }
                if (SORTAD == "DES")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderByDescending(r => r.CUSTOMER_NAME);
                }
            }
            if (SORTSQL == "DEALER_CONTACT_NAME")
            {
                if (SORTAD == "ASC")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderBy(r => r.DEALER_CONTACT_NAME);
                }
                if (SORTAD == "DES")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderByDescending(r => r.DEALER_CONTACT_NAME);
                }
            }
            if (SORTSQL == "INVOICE_CUSTOMER")
            {
                if (SORTAD == "ASC")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderBy(r => r.INVOICE_CUSTOMER);
                }
                if (SORTAD == "DES")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderByDescending(r => r.INVOICE_CUSTOMER);
                }
            }

            if (SORTSQL == "INVOICE_CUSTOMER")
            {
                if (SORTAD == "ASC")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderBy(r => r.INVOICE_CUSTOMER);
                }
                if (SORTAD == "DES")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderByDescending(r => r.INVOICE_CUSTOMER);
                }
            }
            /* END Sort*/

            foreach (var tb in ChassisInvoiceTable)
            {

                string year_get = "";
                string idchassis_get = "";
                decimal sum_out_total = 0;
                decimal sum_out_year = 0;
                string year_dup = "";
                list_group_dup.Clear();
                if (!string.IsNullOrEmpty(tb.INVOICE_NO_DATE))
                {

                    for (int i = 0; i < tb.INVOICE_NO_DATE.Split(',').Length - 1; i++)
                    {
                        String INVOICE_PRICE_ = "0";
                        if (!string.IsNullOrEmpty(tb.INVOICE_PRICE.Split(',')[i]))
                        {
                            if (!tb.INVOICE_PRICE.Contains("SELECT"))
                            {

                                INVOICE_PRICE_ = tb.INVOICE_PRICE.Split(',')[i].Split('(')[1].Split(')')[0].ToString();

                            }
                            string year_inv = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0];
                            string year_compare = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0];
                            String year_dup_set = "";
                            string di = "0";


                            if (year_dup == "")
                            {

                                year_dup = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0];

                                list_group_dup.Add(new group_dup { ID = tb.ID_CHASSIS, YEAR = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0] });
                                di = "1";
                            }

                            if (list_group_dup.Count > 0)
                            {
                                bool exists = list_group_dup.Any(x => x.YEAR == year_compare);

                                if (exists == false)
                                {
                                    list_group_dup.Add(new group_dup { ID = tb.ID_CHASSIS, YEAR = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0] });
                                    di = "1";
                                }
                                year_dup_set = exists.ToString();
                            }

                            // if (check_dup ==true) { year_dup_set = "1"; }

                            /// System.Diagnostics.Debug.WriteLine("hhh" + tb.ID_CHASSIS + "ddsd" + tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0]);
                            if (di == "1")
                            {
                                list_group_array_invoice.Add(new group_array_invoice
                                {
                                    ID_CHASSIS = tb.ID_CHASSIS,
                                    ID_INVOICE = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[0] + "(" + tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0] + ")",
                                    DATE_YEAR = tb.INSTALL_DATE,
                                    YEAR = tb.INSTALL_YEAR,
                                    YEAR_INVOICE = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0],
                                    DATE_INVOICE = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0],
                                    COUNT = tb.INVOICE_NO_DATE.Split(',').Length,
                                    PRICE_INVOICE = INVOICE_PRICE_,
                                    YEAR_DUP = di
                                });
                            }
                            if (di == "0")
                            {
                                list_group_array_invoice.Where(r => r.ID_CHASSIS == tb.ID_CHASSIS && r.YEAR_INVOICE == tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0]).Select(c => { c.ID_INVOICE = c.ID_INVOICE + "br" + tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[0] + "(" + tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0] + ")"; return c; }).ToList();
                                list_group_array_invoice.Where(r => r.ID_CHASSIS == tb.ID_CHASSIS && r.YEAR_INVOICE == tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0]).Select(c => { c.YEAR_INVOICE = c.YEAR_INVOICE + "br" + tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0]; return c; });
                                list_group_array_invoice.Where(r => r.ID_CHASSIS == tb.ID_CHASSIS && r.YEAR_INVOICE == tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0]).Select(c => { c.DATE_INVOICE = c.DATE_INVOICE + "br" + tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0]; return c; });
                                list_group_array_invoice.Where(r => r.ID_CHASSIS == tb.ID_CHASSIS && r.YEAR_INVOICE == tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0]).Select(c => { c.PRICE_INVOICE = c.PRICE_INVOICE + "br" + INVOICE_PRICE_; return c; });
                            }

                            // if (tb.YEAR_LOSE >= 0) { sum_all = sum_all + Convert.ToDecimal(tb.INVOICE_PRICE.Split(',')[i].Split('(')[1].Split(')')[0]); }
                            if (tb.YEAR_LOSE >= 0)
                            {
                                try
                                {
                                    sum_all = sum_all + Convert.ToDecimal(tb.INVOICE_PRICE.Split(',')[i].Split('(')[1].Split(')')[0]);
                                }
                                catch { }

                            }
                        }
                    }
                    // if (tb.ID_CHASSIS == "010037") { sum_all = sum_all + sum_out_total*Convert.ToDecimal(tb.INVOICE_COUNT)  ; }

                    foreach (var strYear in tbls_year_report)
                    {
                        if (!tb.INVOICE_NO_DATE.Contains(strYear.year))
                        {
                            list_group_array_invoice.Add(new group_array_invoice { ID_CHASSIS = tb.ID_CHASSIS, ID_INVOICE = "-", DATE_YEAR = tb.INSTALL_DATE, YEAR = tb.INSTALL_YEAR, YEAR_INVOICE = strYear.year, DATE_INVOICE = "-", COUNT = tb.INVOICE_NO_DATE.Split(',').Length });

                        }
                    }
                }
                if (string.IsNullOrEmpty(tb.INVOICE_NO_DATE))
                {
                    foreach (var strYear in tbls_year_report)
                    {

                        list_group_array_invoice.Add(new group_array_invoice { ID_CHASSIS = tb.ID_CHASSIS, ID_INVOICE = "-", DATE_YEAR = tb.INSTALL_DATE, YEAR = tb.INSTALL_YEAR, DATE_INVOICE = "-", COUNT = 0, YEAR_INVOICE = "" });
                    }
                }
            }

            list_group_array_invoice = list_group_array_invoice.Distinct().ToList();

            int sort_table_id = 2;
            decimal sum_re = 0;


            ViewBag.DLTChassis_InvoiceNot = ChassisInvoiceTable.Where(tb_ => tb_.YEAR_LOSE >= 0 && tb_.INVOICE_NO == null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "1").Count();
            ViewBag.DLTChassis_InvoiceOr = ChassisInvoiceTable.Where(tb_ => tb_.YEAR_LOSE > 0 && tb_.INVOICE_NO != null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "2").Count();
            ViewBag.DLTChassis_InvoiceSc = ChassisInvoiceTable.Where(tb_ => tb_.YEAR_LOSE == 0 && tb_.INVOICE_NO != null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "3").Count();
            ViewBag.DLTChassis_InvoiceNext = ChassisInvoiceTable.Count(tb_ => tb_.YEAR_LOSE < 0 && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "4");
            ViewBag.Total_Dealer = ChassisInvoiceTable.GroupBy(n => n.DEALER_CONTACT_NAME).Count();
            ViewBag.Total_Customer = ChassisInvoiceTable.GroupBy(n => n.CUSTOMER_NAME).Count();
            ViewBag.Total_Price_New = "-";
            ChassisInvoiceTable = ChassisInvoiceTable.ToPagedList(pageIndex, defaSize);

          


            ViewBag.group_year = tbls_year_report.ToList().OrderBy(i => i.year);
            ViewBag.group_year = 1;
            if (string.IsNullOrEmpty(page.ToString()))
            {
                page = 1;
            }
            ViewBag.page_this = page;
            ViewBag.list_group_array_invoice = list_group_array_invoice.ToList().OrderBy(i => i.YEAR_INVOICE);
            ViewBag.Page = pageIndex;
            ViewBag.PageMax = pageSize;



            decimal Outstanding = 0;

            List<string> id_lic = new List<string>();
            List<string> distinct = new List<string>();
            foreach (var tb_ in ChassisInvoiceTable)
            {
                DataRow dr = Export_TB.NewRow();
                Outstanding = 0;
                decimal Sum_Total = 0;
                mod = 0;

                /*COLOR*/
                string color = "";
                if (tb_.YEAR_LOSE >= 0 && tb_.INVOICE_NO == null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "1")
                {
                    color = "odd bg-orange";
                }
                if (tb_.YEAR_LOSE > 0 && tb_.INVOICE_NO != null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "2")
                {
                    color = "odd bg-yellow";
                }
                if (tb_.YEAR_LOSE == 0 && tb_.INVOICE_NO != null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "3")
                {
                    color = "odd bg-green";
                }
                if (tb_.YEAR_LOSE < 0 && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "4")
                {
                    color = "odd bg-purple";
                }

                dr[1] = color;


                /*INVOICE_GET*/
                int row_group_customer = 0;
                foreach (var item in group_year_report.OrderBy(r => r.year))
                {
                    int i = 1;
                    int x_inv = -1;
                    int dmy_inv = 1;
                    foreach (var item_group in list_group_array_invoice.OrderBy(r => r.YEAR_INVOICE))
                    {
                        if (item_group.ID_CHASSIS == tb_.ID_CHASSIS)
                        {
                            i++;
                            x_inv++;

                            String inv = "";
                            String inv_year = "";
                            String inv_date = "";
                            String price = "";
                            String inv_price = "";
                            inv = item_group.ID_INVOICE;
                            inv_date = item_group.DATE_INVOICE;
                            inv_year = item_group.YEAR_INVOICE;
                            inv_price = item_group.PRICE_INVOICE;
                            price = "x";
                            /*INVOICENO*/


                            string day_inv = "";
                            if (inv_date.Split('/').Length > 1)
                            {
                                day_inv = inv_date.Split('/')[1].ToString();
                            }
                            /*INVOICEDATE*/
                            string month_inv = "";
                            if (inv_date.Split('/').Length > 1)
                            {
                                month_inv = "/" + inv_date.Split('/')[2].ToString();
                            }

                            string year_inv = "";
                            if (inv_date.Split('/').Length > 1)
                            {
                                int year_invsum = Convert.ToInt32(inv_date.Split('/')[0]) + 543;
                                year_inv = "/" + year_invsum.ToString();
                            }
                            dr[2 + x_inv] = inv.Replace("br", Environment.NewLine);
                            dr[3 + x_inv] = day_inv + month_inv + year_inv;
                            dr[4 + x_inv] = inv_price;
                            x_inv += 2;

                            row_group_customer = x_inv + 2;

                            if (!string.IsNullOrEmpty(inv_price))
                            {
                                if (inv_price.Length > 1)
                                {
                                    Sum_Total = Sum_Total + Convert.ToDecimal(inv_price);
                                    Outstanding = Convert.ToDecimal(inv_price);
                                }
                            }
                            else
                            {
                                mod = mod + 1;
                            }
                        }

                        if (i.ToString() == item.count)
                        {
                            break;
                        }

                    }
                    break;
                }


                ///*INVOICE_CUSTOMER*/
                dr[row_group_customer + 1] = tb_.INVOICE_CUSTOMER;

                ///*INVOICE_CUSTOMER_PHONE*/
                dr[row_group_customer + 2] = tb_.INVOICE_CUSTOMER_PHONE;




                ///*INSTALL_DATE*/
                string day_ = tb_.INSTALL_DATE.Replace("  ", " ").Split(' ')[1];
                string month_ = tb_.INSTALL_DATE.Replace("  ", " ").Split(' ')[0];
                int year_ = Convert.ToInt32(tb_.INSTALL_DATE.Replace("  ", " ").Split(' ')[2]) + 543;

                dr[row_group_customer + 3] = day_ + "" + month_ + "" + year_;

                ///*ID_LICENSE*/
                Staring_LICENSE = "";
                try
                {
                    if (!string.IsNullOrEmpty(tb_.ID_LICENSE))
                    {

                        foreach (var s in tb_.ID_LICENSE.Split(','))
                        {
                            id_lic.Add(s.ToString() + ",");
                        }
                        distinct = id_lic.Distinct().ToList();
                        foreach (var s in distinct)
                        {
                            Staring_LICENSE = Staring_LICENSE + s;
                        }
                    }
                    else { }
                }
                catch
                {
                    Staring_LICENSE = tb_.ID_LICENSE;

                }

                id_lic.Clear();
                distinct.Clear();

                if (Staring_LICENSE.Replace(",,", "").Length > 3)
                {
                    Staring_LICENSE.Replace(",,", "");
                }
                string[] LICENSE_List = Staring_LICENSE.Split(',');
                string table_LICENSE = "";
                for (int i = 0; i < LICENSE_List.Length; i++)
                {
                    if (LICENSE_List[i].Length < 13)
                    {
                        table_LICENSE = table_LICENSE + "," + LICENSE_List[i];

                    }

                }
                dr[row_group_customer + 4] = table_LICENSE;
                ///*ID_CHASSIS*/
                dr[row_group_customer + 5] = tb_.ID_CHASSIS;

                ///*ID_MEI*/
                StaringMEI = "";
                List<string> id_lic_MEI = new List<string>();
                List<string> distinct_MEI = id_lic.Distinct().ToList();

                if (!string.IsNullOrEmpty(tb_.ID_MEI))
                {


                    foreach (var s in tb_.ID_MEI.Split(','))
                    {

                        id_lic.Add(s.ToString() + ",");

                    }
                    distinct_MEI = id_lic.Distinct().ToList();
                    foreach (var s in distinct_MEI)
                    {
                        StaringMEI = StaringMEI + s;
                    }
                }
                id_lic_MEI.Clear();
                distinct_MEI.Clear();

                if (StaringMEI.Replace(",,", "").Length > 3)
                { StaringMEI.Replace(",,", ""); }


                dr[row_group_customer + 6] = StaringMEI;

                ///*INVOICE_COUNT*/
                dr[row_group_customer + 7] = tb_.INVOICE_COUNT;
                //////decimal Outstanding_ = 0;

                int year_ins = Convert.ToInt32(DateTime.Now.Year) - Convert.ToInt32(tb_.INSTALL_DATE_.Year);
                decimal y = year_ins - mod;
                if (y > 0)
                {


                    ////Outstanding_ = Outstanding * y;
                }
                if (y <= 0)
                {

                    ////Outstanding_ = Outstanding;
                }
                dr[row_group_customer + 8] = tb_.DEALER_CONTACT_NAME;
                dr[row_group_customer + 9] = tb_.DEALER_CONTACT_PHONE;

                dr[row_group_customer + 10] = tb_.ID_MEI_L;
                dr[row_group_customer + 11] = tb_.ID_LICENSE_L;

                dr[row_group_customer + 12] = tb_.CUSTOMER_NAME;
                dr[row_group_customer + 13] = tb_.CUSTOMER_PHONE;

                Export_TB.Rows.Add(dr);
            }

            ViewBag.CHASSIS_SEARCH = CHASSIS_SEARCH;
            ViewBag.CUSTOMER_SEARCH = CUSTOMER_SEARCH;
            ViewBag.INV_CUSTOMER_SEARCH = INV_CUSTOMER_SEARCH;
            ViewBag.MONTH_SEARCH = MONTH_SEARCH;
            ViewBag.DELALER_SEARCH = DELALER_SEARCH;
            ViewBag.STATUS_SEARCH = STATUS_SEARCH;
            ViewBag.ORDER_SEARCH = ORDER_SEARCH;
            ViewBag.YEAR_SEARCH = YEAR_SEARCH;
            ViewBag.STARTDAY_SEARCH = STARTDAY_SEARCH;
            ViewBag.ENDDAY_SEARCH = ENDDAY_SEARCH;
            //return View(Export_TB);

            return View(Export_TB);
            //return Redirect("/Users/index");
        }

        public ActionResult Index(
                 String CHASSIS_SEARCH,
                String CUSTOMER_SEARCH,
                String DELALER_SEARCH,
                String STATUS_SEARCH,
                String ORDER_SEARCH,
                String MAXMIN_SEARCH,
                String MONTH_SEARCH,
                String YEAR_SEARCH,
                String STARTDAY_SEARCH,
                String ENDDAY_SEARCH,
                String INV_CUSTOMER_SEARCH,
                int? page,
                int? pageSize,
                string SORTSQL,
                string SORTAD,
                string LICENSE_SEARCH,
                string MEI_SEARCH,
                string INVOICE_NO_SEARCH,
                string NOT_MEI,
                string NOT_LICENSE,
                string MEI_CERTIF,
                string LICENSE_CERTIF
            )
        {

        
            Session["session_users_password"] = "admin@gmail.com";
            Session["session_id"] = "123";
            Session["session_users"] = "admin@gmail.com";

            ViewBag.Total_Customer = "0";
            ViewBag.Total_Dealer = "0";
            ViewBag.Total_Customer = "";
            ViewBag.Total_Dealer = "";
            ViewBag.Total_Price_New = "";
            ViewBag.CHASSIS_SEARCH = "";
            ViewBag.CUSTOMER_SEARCH = "";
            ViewBag.DELALER_SEARCH = "";
            ViewBag.STATUS_SEARCH = "";
            ViewBag.ORDER_SEARCH = "";
            ViewBag.MAXMIN_SEARCH = "";
            ViewBag.MONTH_SEARCH = "";
            ViewBag.YEAR_SEARCH = "";
            ViewBag.STARTDAY_SEARCH = "";
            ViewBag.ENDDAY_SEARCH = "";
            ViewBag.userid = 0;
            string Staring_LICENSE = "";
            string StaringMEI = "";
            decimal mod = 0;

            var login_users_password = this.Session["session_users_password"];
            var login_session_id = this.Session["session_id"];


            ViewBag.username = login_users_password;
            ViewBag.userid = login_session_id;
            string Start_year = "2016";
            if (string.IsNullOrEmpty(Start_year))
            {
                Start_year = "2016";
            }


            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            int defaSize = (pageSize ?? 10);
            ViewBag.psize = defaSize;
            TotalItems = 0;
            CurrentPage = 0;
            TotalPages = 0;
            StartPage = 0;
            EndPage = 0;

            if (pageSize == null)
            {
                pageSize = 0;
            }

            if (pageSize == 0)
            {
                pageSize = 10;
            }
            ViewBag.PageSize = new List<SelectListItem>()
         {

             new SelectListItem() { Value="10", Text= "10" },
             new SelectListItem() { Value="15", Text= "15" },
             new SelectListItem() { Value="25", Text= "25" },
             new SelectListItem() { Value="50", Text= "50" },
         };
          
            List<ls_year_report> tbls_year_report = new List<ls_year_report>();
            IPagedList<DLTChassisInvoice> ChassisInvoice_Table = null;
            string param = "ID";

            ChassisInvoice_Table = db.DLTChassisInvoices.OrderBy(r => r.ID).ToPagedList(pageIndex, defaSize);

            Regex charsToDestroy = new Regex(@"[^\d|\,\-]");

            var ChassisInvoiceTable_MEI = db.DLTChassisInvoices.ToList();
            string myStringOutput = String.Join(",", db.DLTDetailChassis.Select(p => p.INVOICE_FLAG.ToString()).ToArray());
            //   DateTime data_time = Convert.ToDateTime(ChassisInvoiceTable_MEI.Where(r => r.ID == 481382).FirstOrDefault().INVOICE_NO_DATE.Split('(')[1].Split(')')[0]);

            // Convert.ToDateTime(INVOICE_NO_DATE.Split('(')[1].Split(')')[0])
            var ChassisInvoiceTable = from dltcha in ChassisInvoiceTable_MEI
                                      join dltdetail in db.DLTDetailChassis on dltcha.ID_CHASSIS equals dltdetail.ID_CHASSIS into d
                                      from table in d.DefaultIfEmpty()

                                      select new Chassis_Status_Model
                                      {
                                          STATUS_FLAG = table == null ? "" : table.STATUS_FLAG,
                                          ID_CHASSIS = dltcha.ID_CHASSIS,
                                          INVOICE_COUNT =
                                          table == null ? dltcha.INVOICE_COUNT : (Convert.ToInt32(dltcha.INVOICE_COUNT.ToString()) + table.INVOICE_FLAG.Split(',').Count()).ToString(),
                                          INVOICE_NO = table == null ? dltcha.INVOICE_NO : dltcha.INVOICE_NO + String.Join(",", db.DLTDetailChassis.Where(r => r.ID_CHASSIS == dltcha.ID_CHASSIS).Select(p => p.INVOICE_FLAG.ToString()).ToArray()).ToString(),
                                          INVOICE_COUNT_LOSE = dltcha.INVOICE_COUNT_LOSE,
                                          YEAR_LOSE = Convert.ToInt32(dltcha.YEAR_LOSE),
                                          INSTALL_DATE = dltcha.INSTALL_DATE,
                                          DEALER_CONTACT_NAME = dltcha.DEALER_CONTACT_NAME,
                                          DEALER_CONTACT_PHONE = dltcha.DEALER_CONTACT_PHONE,
                                          ID_LICENSE = dltcha.ID_LICENSE,
                                          ID_MEI = dltcha.ID_MEI,
                                          ID_LICENSE_L = dltcha.CER_LICENSE,
                                          ID_MEI_L = dltcha.CER_MEI,
                                          INVOICE_NO_DATE = table == null ? dltcha.INVOICE_NO_DATE : dltcha.INVOICE_NO_DATE + String.Join(",", db.DLTDetailChassis.Where(r => r.ID_CHASSIS == dltcha.ID_CHASSIS).Select(p => p.UP_DATE_INVOICE_FLAG.ToString()).ToArray()).ToString() + ",",
                                          INSTALL_YEAR = dltcha.INSTALL_YEAR,
                                          INVOICE_PRICE = table == null ? dltcha.INVOICE_PRICE : dltcha.INVOICE_PRICE + String.Join(",", db.DLTDetailChassis.Where(r => r.ID_CHASSIS == dltcha.ID_CHASSIS).Select(p => p.UP_DATE_PRICE_FLAG.ToString()).ToArray()).ToString(),
                                          CUSTOMER_NAME = dltcha.CUSTOMER_NAME,
                                          CUSTOMER_PHONE = dltcha.CUSTOMER_PHONE,
                                          INVOICE_CUSTOMER = dltcha.INVOICE_CUSTOMER,
                                          INVOICE_CUSTOMER_PHONE = dltcha.INVOICE_CUSTOMER_PHONE,
                                          ID = dltcha.ID,
                                          CER_LICENSE = dltcha.CER_LICENSE,
                                          INSTALL_DATE_ = Convert.ToDateTime(dltcha.INSTALL_DATE),
                                          MONTH_SEARCH = Convert.ToDateTime(dltcha.INSTALL_DATE).Month.ToString(),
                                          NOT_FOUND_IME = dltcha.MEI_NOT_FOUND,
                                          STATUS_LICENSE =dltcha.STATUS_LICENSE,
                                          STATUS_MEI = dltcha.STATUS_MEI
                                      };

        //   ChassisInvoiceTable= ChassisInvoiceTable.Where(r => r.NOT_FOUND_IME == 1).Select(c => { c.ID_MEI = null; return c; }).ToList();
         //   ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.NOT_FOUND_IME == 1).Select(c => { c.INVOICE_NO_DATE = null; return c; }).ToList();
        //    ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.NOT_FOUND_IME == 1).Select(c => { c.ID_LICENSE = null; return c; }).ToList();
        //    ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.NOT_FOUND_IME == 1).Select(c => { c.INVOICE_NO = null; return c; }).ToList();
          //  ChassisInvoiceTable =  ChassisInvoiceTable.Where(r => r.NOT_FOUND_IME == 1).Select(c => { c.INVOICE_COUNT = "0"; return c; }).ToList();
            var tb_grpup = db.DLTChassisInvoices
                .GroupBy(i => i.INSTALL_YEAR)
                .Select(group => new { year_group = group.Key });
            List<string> ls_year = new List<string>();
    
            foreach (var year_sort in tb_grpup)
            {
             
                String year_split = year_sort.ToString().Split('=')[1].Split(' ')[1];

                ls_year.Add(year_split);
            }


            foreach (var year_sort in tb_grpup)
            {
                String year_split = year_sort.ToString().Split('=')[1].Split(' ')[1];

                if (!string.IsNullOrEmpty(year_split))
                {
                    if (Convert.ToInt32(year_split) >= Convert.ToInt32(Start_year))
                    {
                     
                        int total_count = Convert.ToInt32(DateTime.Now.Year.ToString()) - Convert.ToInt32(Start_year) + 2;
                        tbls_year_report.Add(new ls_year_report { year = year_split, count = total_count.ToString() });
                    }
                }
            }
        
            ViewBag.tb_group_year_report = tbls_year_report.ToList().OrderBy(i => i.year);
            var group_year_report = tbls_year_report.ToList().OrderBy(i => i.year);
            ViewBag.tb_group_year = ls_year.OrderBy(item => item);
            List<group_array_invoice> list_group_array_invoice = new List<group_array_invoice>();
            List<group_dup> list_group_dup = new List<group_dup>();
            list_group_array_invoice.Clear();

            decimal sum_all = 0;
            decimal sum_outs = 0;
           

            var Export_TB = new System.Data.DataTable();

            Export_TB.Columns.Add("1", typeof(int));
            Export_TB.Columns.Add("2", typeof(string));
            Export_TB.Columns.Add("3", typeof(string));
            Export_TB.Columns.Add("4", typeof(string));
            Export_TB.Columns.Add("5", typeof(string));
            Export_TB.Columns.Add("6", typeof(string));
            Export_TB.Columns.Add("7", typeof(string));
            Export_TB.Columns.Add("8", typeof(string));
            Export_TB.Columns.Add("9", typeof(string));
            Export_TB.Columns.Add("10", typeof(string));
            Export_TB.Columns.Add("11", typeof(string));
            Export_TB.Columns.Add("12", typeof(string));
            Export_TB.Columns.Add("13", typeof(string));
            Export_TB.Columns.Add("14", typeof(string));
            Export_TB.Columns.Add("15", typeof(string));
            Export_TB.Columns.Add("16", typeof(string));
            Export_TB.Columns.Add("17", typeof(string));
            Export_TB.Columns.Add("18", typeof(string));
            Export_TB.Columns.Add("19", typeof(string));
            Export_TB.Columns.Add("20", typeof(string));
            Export_TB.Columns.Add("21", typeof(string));
            Export_TB.Columns.Add("22", typeof(string));
            Export_TB.Columns.Add("23", typeof(string));
            Export_TB.Columns.Add("24", typeof(string));
            Export_TB.Columns.Add("25", typeof(string));
            Export_TB.Columns.Add("26", typeof(string));
            Export_TB.Columns.Add("27", typeof(string));
            Export_TB.Columns.Add("28", typeof(string));
            Export_TB.Columns.Add("29", typeof(string));
            Export_TB.Columns.Add("30", typeof(string));
            Export_TB.Columns.Add("31", typeof(string));
            Export_TB.Columns.Add("32", typeof(string));

            ViewBag.page_this = 1;
            ViewBag.TotalPage = 2;
            ViewBag.CHASSIS_SEARCH = "";
            ViewBag.CUSTOMER_SEARCH = "";
            ViewBag.INV_CUSTOMER_SEARCH = "";
            ViewBag.MONTH_SEARCH = "";
            ViewBag.DELALER_SEARCH = "";
            ViewBag.STATUS_SEARCH = "";
            ViewBag.ORDER_SEARCH = "";
            ViewBag.YEAR_SEARCH = "";
            ViewBag.STARTDAY_SEARCH = "";
            ViewBag.ENDDAY_SEARCH = "";
            Session["group_1"] = "collapsed-box";

            /* Begin Search*/
            if (!string.IsNullOrEmpty(CHASSIS_SEARCH))
            {

                Session["group_1"] = "";
                ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.ID_CHASSIS == CHASSIS_SEARCH);
          
                if (ChassisInvoiceTable.Count() == 0)
                    {
          
                    DataRow dr = Export_TB.NewRow();
                        dr[1] = 1;
                        dr[14] = "---ไม่พบข้อมูล---";
                        Export_TB.Rows.Add(dr);
                        return View(Export_TB);

                    }
              
            }
            
            if (!string.IsNullOrEmpty(INV_CUSTOMER_SEARCH))
            {
                Session["group_1"] = "";
                ChassisInvoiceTable = ChassisInvoiceTable.Where(i => i.INVOICE_CUSTOMER != null);
                ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.INVOICE_CUSTOMER.Contains(INV_CUSTOMER_SEARCH));
                if (ChassisInvoiceTable.Count() == 0)
                {
                    DataRow dr = Export_TB.NewRow();
                    dr[1] = 1;
                    dr[14] = "---ไม่พบข้อมูล---";
                    Export_TB.Rows.Add(dr);
                    return View(Export_TB);

                }
            }
            if (!string.IsNullOrEmpty(CUSTOMER_SEARCH))
            {
                Session["group_1"] = "";
                ChassisInvoiceTable = ChassisInvoiceTable.Where(i => i.CUSTOMER_NAME != null);
                ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.CUSTOMER_NAME.Contains(CUSTOMER_SEARCH));
                if (ChassisInvoiceTable.Count() == 0)
                {
                    DataRow dr = Export_TB.NewRow();
                    dr[1] = 1;
                    dr[14] = "---ไม่พบข้อมูล---";
                    Export_TB.Rows.Add(dr);
                    return View(Export_TB);

                }
            }

            if (!string.IsNullOrEmpty(DELALER_SEARCH))
            {
                Session["group_1"] = "";
                ChassisInvoiceTable = ChassisInvoiceTable.Where(i => i.DEALER_CONTACT_NAME != null);
                ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.DEALER_CONTACT_NAME.Contains(DELALER_SEARCH));
                if (ChassisInvoiceTable.Count() == 0)
                {
                    DataRow dr = Export_TB.NewRow();
                    dr[1] = 1;
                    dr[14] = "---ไม่พบข้อมูล---";
                    Export_TB.Rows.Add(dr);
                    return View(Export_TB);

                }

            }
            if (!string.IsNullOrEmpty(LICENSE_SEARCH))
            {
                Session["group_1"] = "";
                ChassisInvoiceTable = ChassisInvoiceTable.Where(i => i.ID_LICENSE != null);
                ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.ID_LICENSE.Contains(LICENSE_SEARCH));
                if (ChassisInvoiceTable.Count() == 0)
                {
                    DataRow dr = Export_TB.NewRow();
                    dr[1] = 1;
                    dr[14] = "---ไม่พบข้อมูล---";
                    Export_TB.Rows.Add(dr);
                    return View(Export_TB);

                }
            }
            if (!string.IsNullOrEmpty(MEI_SEARCH))
            {
                Session["group_1"] = "";
                ChassisInvoiceTable = ChassisInvoiceTable.Where(i => i.ID_MEI != null);
                ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.ID_MEI.Contains(MEI_SEARCH));
                if (ChassisInvoiceTable.Count() == 0)
                {
                    DataRow dr = Export_TB.NewRow();
                    dr[1] = 1;
                    dr[14] = "---ไม่พบข้อมูล---";
                    Export_TB.Rows.Add(dr);
                    return View(Export_TB);

                }
            }
            if (!string.IsNullOrEmpty(MEI_CERTIF))
            {
                Session["group_1"] = "";
                ChassisInvoiceTable = ChassisInvoiceTable.Where(i => i.ID_MEI_L != null);
                ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.ID_MEI_L.Contains(MEI_CERTIF));
                if (ChassisInvoiceTable.Count() == 0)
                {
                    DataRow dr = Export_TB.NewRow();
                    dr[1] = 1;
                    dr[14] = "---ไม่พบข้อมูล---";
                    Export_TB.Rows.Add(dr);
                    return View(Export_TB);

                }
            }
            if (!string.IsNullOrEmpty(LICENSE_CERTIF))
            {
                Session["group_1"] = "";
                ChassisInvoiceTable = ChassisInvoiceTable.Where(i => i.ID_LICENSE_L != null);
                ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.ID_LICENSE_L.Contains(LICENSE_CERTIF));
                if (ChassisInvoiceTable.Count() == 0)
                {
                    DataRow dr = Export_TB.NewRow();
                    dr[1] = 1;
                    dr[14] = "---ไม่พบข้อมูล---";
                    Export_TB.Rows.Add(dr);
                    return View(Export_TB);

                }
            }


            if (!string.IsNullOrEmpty(INVOICE_NO_SEARCH))
            {
                Session["group_1"] = "";
                ChassisInvoiceTable = ChassisInvoiceTable.Where(i => i.INVOICE_NO != null);
                ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.INVOICE_NO.Contains(INVOICE_NO_SEARCH));
                if (ChassisInvoiceTable.Count() == 0)
                {
                    DataRow dr = Export_TB.NewRow();
                    dr[1] = 1;
                    dr[14] = "---ไม่พบข้อมูล---";
                    Export_TB.Rows.Add(dr);
                    return View(Export_TB);

                }
            }
            Session["group_2"] = "collapsed-box";
            if (!string.IsNullOrEmpty(STARTDAY_SEARCH))
            {
                Session["group_2"] = "";
                // string c = ChassisInvoiceTable.Where(r => r.ID == 475867).FirstOrDefault().INSTALL_DATE_.ToString();
                DateTime STARTDAY = Convert.ToDateTime(STARTDAY_SEARCH);

                ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.INSTALL_DATE_ >= STARTDAY);
                if (ChassisInvoiceTable.Count() == 0)
                {
                    DataRow dr = Export_TB.NewRow();
                    dr[1] = 1;
                    dr[14] = "---ไม่พบข้อมูล---";
                    Export_TB.Rows.Add(dr);
                    return View(Export_TB);

                }
            }

            if (!string.IsNullOrEmpty(ENDDAY_SEARCH))
            {
                Session["group_2"] = "";
                DateTime ENDDAY = Convert.ToDateTime(ENDDAY_SEARCH);
                ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.INSTALL_DATE_ <= ENDDAY);
                if (ChassisInvoiceTable.Count() == 0)
                {
                    DataRow dr = Export_TB.NewRow();
                    dr[1] = 1;
                    dr[14] = "---ไม่พบข้อมูล---";
                    Export_TB.Rows.Add(dr);
                    return View(Export_TB);

                }
            }
            if (!string.IsNullOrEmpty(MONTH_SEARCH))
            {
                Session["group_2"] = "";
                ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.MONTH_SEARCH == MONTH_SEARCH);
                if (ChassisInvoiceTable.Count() == 0)
                {
                    DataRow dr = Export_TB.NewRow();
                    dr[1] = 1;
                    dr[14] = "---ไม่พบข้อมูล---";
                    Export_TB.Rows.Add(dr);
                    return View(Export_TB);

                }
            }

            if (!string.IsNullOrEmpty(YEAR_SEARCH))
            {
                Session["group_2"] = "";
                ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.INSTALL_YEAR == YEAR_SEARCH);
                if (ChassisInvoiceTable.Count() == 0)
                {
                    DataRow dr = Export_TB.NewRow();
                    dr[1] = 1;
                    dr[14] = "---ไม่พบข้อมูล---";
                    Export_TB.Rows.Add(dr);
                    return View(Export_TB);

                }
            }
            Session["group_3"] = "collapsed-box";
            if (!string.IsNullOrEmpty(STATUS_SEARCH))
            {
           
                if (STATUS_SEARCH == "All")
                    {
                        ChassisInvoiceTable = ChassisInvoiceTable;
                    }
                    if (STATUS_SEARCH == "0")
                    {
                    Session["group_3"] = "";
                    ChassisInvoiceTable = ChassisInvoiceTable.Where(i => i.INVOICE_NO == null || i.STATUS_FLAG == "1");
                    }
                    if (STATUS_SEARCH == "1")
                    {
                    Session["group_3"] = "";
                    ChassisInvoiceTable = ChassisInvoiceTable.Where(i => i.YEAR_LOSE > 0 && i.INVOICE_NO != null && i.INVOICE_COUNT != "0" || i.STATUS_FLAG == "2");
                    }
                    if (STATUS_SEARCH == "2")
                    {
                    Session["group_3"] = "";
                    ChassisInvoiceTable = ChassisInvoiceTable.Where(i => i.INVOICE_NO != null && i.YEAR_LOSE == 0 || i.STATUS_FLAG == "3");
                    }
                    if (STATUS_SEARCH == "3")
                    {
                    Session["group_3"] = "";
                    ChassisInvoiceTable = ChassisInvoiceTable.Where(i => i.YEAR_LOSE < 0 || i.STATUS_FLAG == "4");
                    }
                if (ChassisInvoiceTable.Count() == 0)
                {
                    DataRow dr = Export_TB.NewRow();
                    dr[1] = 1;
                    dr[14] = "---ไม่พบข้อมูล---";
                    Export_TB.Rows.Add(dr);
                    return View(Export_TB);

                }

            }
            if (!string.IsNullOrEmpty(NOT_MEI))
            {
               
                if (NOT_MEI=="1")
                {
                    Session["group_3"] = "";
                    ChassisInvoiceTable = ChassisInvoiceTable.Where(i => i.STATUS_MEI != null);
                    ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.STATUS_MEI == "1");
                    if (ChassisInvoiceTable.Count() == 0)
                    {
                        DataRow dr = Export_TB.NewRow();
                        dr[1] = 1;
                        dr[14] = "---ไม่พบข้อมูล---";
                        Export_TB.Rows.Add(dr);
                        return View(Export_TB);

                    }
                }
                if (NOT_MEI == "2")
                {
                    Session["group_3"] = "";
                    ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.STATUS_MEI == null);
                    if (ChassisInvoiceTable.Count() == 0)
                    {
                        DataRow dr = Export_TB.NewRow();
                        dr[1] = 1;
                        dr[14] = "---ไม่พบข้อมูล---";
                        Export_TB.Rows.Add(dr);
                        return View(Export_TB);

                    }
                }
            }
            if (!string.IsNullOrEmpty(NOT_LICENSE))
            {
                if (NOT_LICENSE=="1") {

                    Session["group_3"] = "";
                    ChassisInvoiceTable = ChassisInvoiceTable.Where(i => i.STATUS_LICENSE != null);
                    ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.STATUS_LICENSE == "1");
                        if (ChassisInvoiceTable.Count() == 0)
                        {
                            DataRow dr = Export_TB.NewRow();
                            dr[1] = 1;
                            dr[14] = "---ไม่พบข้อมูล---";
                            Export_TB.Rows.Add(dr);
                            return View(Export_TB);

                        }
                    }
                    if (NOT_LICENSE=="2") {

                    Session["group_3"] = "";
                    ChassisInvoiceTable = ChassisInvoiceTable.Where(r => r.STATUS_LICENSE == null);
                        if (ChassisInvoiceTable.Count() == 0)
                        {
                            DataRow dr = Export_TB.NewRow();
                            dr[1] = 1;
                            dr[14] = "---ไม่พบข้อมูล---";
                            Export_TB.Rows.Add(dr);
                            return View(Export_TB);

                        }
                    }
            }
          
            //  string NOT_MEI,
            //   string NOT_LICENSE,
            //    string MEI_CERTIF,
            //   string LICENSE_CERTIF
            ViewBag.TotalPage = ChassisInvoiceTable.Count() / pageSize;
            /* END Search*/

            /* Begin Sort*/
            if (SORTSQL == "ID_CHASSIS")
            {
                if (SORTAD == "ASC")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderBy(r => r.ID_CHASSIS);
                }
                if (SORTAD == "DES")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderByDescending(r => r.ID_CHASSIS);
                }
            }
            if (SORTSQL == "MEI")
            {
                if (SORTAD == "ASC")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderBy(r => r.ID_MEI);
                }
                if (SORTAD == "DES")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderByDescending(r => r.ID_MEI);
                }
            }
            if (SORTSQL == "ID_LICENSE")
            {
                if (SORTAD == "ASC")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderBy(r => r.ID_LICENSE);
                }
                if (SORTAD == "DES")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderByDescending(r => r.ID_LICENSE);
                }
            }
            if (SORTSQL == "INSTALL_DATE")
            {
                if (SORTAD == "ASC")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderBy(r => r.INSTALL_DATE_);
                }
                if (SORTAD == "DES")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderByDescending(r => r.INSTALL_DATE_);
                }
            }
            if (SORTSQL == "INVOICE_COUNT")
            {
                if (SORTAD == "ASC")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderBy(r => r.INVOICE_COUNT);
                }
                if (SORTAD == "DES")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderByDescending(r => r.INVOICE_COUNT);
                }
            }
            if (SORTSQL == "CUSTOMER_NAME")
            {
                if (SORTAD == "ASC")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderBy(r => r.CUSTOMER_NAME);
                }
                if (SORTAD == "DES")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderByDescending(r => r.CUSTOMER_NAME);
                }
            }
            if (SORTSQL == "DEALER_CONTACT_NAME")
            {
                if (SORTAD == "ASC")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderBy(r => r.DEALER_CONTACT_NAME);
                }
                if (SORTAD == "DES")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderByDescending(r => r.DEALER_CONTACT_NAME);
                }
            }
            if (SORTSQL == "INVOICE_CUSTOMER")
            {
                if (SORTAD == "ASC")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderBy(r => r.INVOICE_CUSTOMER);
                }
                if (SORTAD == "DES")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderByDescending(r => r.INVOICE_CUSTOMER);
                }
            }

            if (SORTSQL == "INVOICE_CUSTOMER")
            {
                if (SORTAD == "ASC")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderBy(r => r.INVOICE_CUSTOMER);
                }
                if (SORTAD == "DES")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.OrderByDescending(r => r.INVOICE_CUSTOMER);
                }
            }
            /* END Sort*/

       

         

            foreach (var tb in ChassisInvoiceTable)
            {

                string year_get = "";
                string idchassis_get = "";
                decimal sum_out_total = 0;
                decimal sum_out_year = 0;
                string year_dup = "";
                list_group_dup.Clear();
            
                if (!string.IsNullOrEmpty(tb.INVOICE_NO_DATE))
                {

                    for (int i = 0; i < tb.INVOICE_NO_DATE.Split(',').Length - 1; i++)
                    {
                        String INVOICE_PRICE_ = "0";
                        if (!string.IsNullOrEmpty(tb.INVOICE_PRICE.Split(',')[i]))
                        {
                            if (!tb.INVOICE_PRICE.Contains("SELECT"))
                            {

                                INVOICE_PRICE_ = tb.INVOICE_PRICE.Split(',')[i].Split('(')[1].Split(')')[0].ToString();

                            }
                            string year_inv = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0];
                            string year_compare = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0];
                            String year_dup_set = "";
                            string di = "0";


                            if (year_dup == "")
                            {

                                year_dup = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0];

                                list_group_dup.Add(new group_dup { ID = tb.ID_CHASSIS, YEAR = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0] });
                                di = "1";
                            }

                            if (list_group_dup.Count > 0)
                            {
                                bool exists = list_group_dup.Any(x => x.YEAR == year_compare);

                                if (exists == false)
                                {
                                    list_group_dup.Add(new group_dup { ID = tb.ID_CHASSIS, YEAR = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0] });
                                    di = "1";
                                }
                                year_dup_set = exists.ToString();
                            }

                            // if (check_dup ==true) { year_dup_set = "1"; }

                            /// System.Diagnostics.Debug.WriteLine("hhh" + tb.ID_CHASSIS + "ddsd" + tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0]);
                            if (di == "1")
                            {
                                list_group_array_invoice.Add(new group_array_invoice
                                {
                                    ID_CHASSIS = tb.ID_CHASSIS,
                                    ID_INVOICE = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[0] + "(" + tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0] + ")",
                                    DATE_YEAR = tb.INSTALL_DATE,
                                    YEAR = tb.INSTALL_YEAR,
                                    YEAR_INVOICE = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0],
                                    DATE_INVOICE = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0],
                                    COUNT = tb.INVOICE_NO_DATE.Split(',').Length,
                                    PRICE_INVOICE = INVOICE_PRICE_,
                                    YEAR_DUP = di
                                });
                            }
                            if (di == "0")
                            {
                                list_group_array_invoice.Where(r => r.ID_CHASSIS == tb.ID_CHASSIS && r.YEAR_INVOICE == tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0]).Select(c => { c.ID_INVOICE = c.ID_INVOICE + "br" + tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[0] + "(" + tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0] + ")"; return c; }).ToList();
                                list_group_array_invoice.Where(r => r.ID_CHASSIS == tb.ID_CHASSIS && r.YEAR_INVOICE == tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0]).Select(c => { c.YEAR_INVOICE = c.YEAR_INVOICE + "br" + tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0]; return c; });
                                list_group_array_invoice.Where(r => r.ID_CHASSIS == tb.ID_CHASSIS && r.YEAR_INVOICE == tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0]).Select(c => { c.DATE_INVOICE = c.DATE_INVOICE + "br" + tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0]; return c; });
                                list_group_array_invoice.Where(r => r.ID_CHASSIS == tb.ID_CHASSIS && r.YEAR_INVOICE == tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0]).Select(c => { c.PRICE_INVOICE = c.PRICE_INVOICE + "br" + INVOICE_PRICE_; return c; });
                            }

                            // if (tb.YEAR_LOSE >= 0) { sum_all = sum_all + Convert.ToDecimal(tb.INVOICE_PRICE.Split(',')[i].Split('(')[1].Split(')')[0]); }
                            if (tb.YEAR_LOSE >= 0)
                            {
                                try
                                {
                                    sum_all = sum_all + Convert.ToDecimal(tb.INVOICE_PRICE.Split(',')[i].Split('(')[1].Split(')')[0]);
                                }
                                catch { }

                            }
                        }
                    }
                    // if (tb.ID_CHASSIS == "010037") { sum_all = sum_all + sum_out_total*Convert.ToDecimal(tb.INVOICE_COUNT)  ; }

                    foreach (var strYear in tbls_year_report)
                    {
                        if (!tb.INVOICE_NO_DATE.Contains(strYear.year))
                        {
                            list_group_array_invoice.Add(new group_array_invoice { ID_CHASSIS = tb.ID_CHASSIS, ID_INVOICE = "-", DATE_YEAR = tb.INSTALL_DATE, YEAR = tb.INSTALL_YEAR, YEAR_INVOICE = strYear.year, DATE_INVOICE = "-", COUNT = tb.INVOICE_NO_DATE.Split(',').Length });

                        }
                    }
                }
                if (string.IsNullOrEmpty(tb.INVOICE_NO_DATE))
                {
                    foreach (var strYear in tbls_year_report)
                    {

                        list_group_array_invoice.Add(new group_array_invoice { ID_CHASSIS = tb.ID_CHASSIS, ID_INVOICE = "-", DATE_YEAR = tb.INSTALL_DATE, YEAR = tb.INSTALL_YEAR, DATE_INVOICE = "-", COUNT = 0, YEAR_INVOICE = "" });
                    }
                }
            }
        
            list_group_array_invoice = list_group_array_invoice.Distinct().ToList();
            
            int sort_table_id = 2;
            decimal sum_re = 0;
          

            ViewBag.DLTChassis_InvoiceNot = ChassisInvoiceTable.Where(tb_ => tb_.YEAR_LOSE >= 0 && tb_.INVOICE_NO == null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "1" ).Count();
            ViewBag.DLTChassis_InvoiceOr = ChassisInvoiceTable.Where(tb_ => tb_.YEAR_LOSE > 0 && tb_.INVOICE_NO != null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "2" ).Count();
            ViewBag.DLTChassis_InvoiceSc = ChassisInvoiceTable.Where(tb_ => tb_.YEAR_LOSE == 0 && tb_.INVOICE_NO != null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "3" ).Count();
            ViewBag.DLTChassis_InvoiceNext = ChassisInvoiceTable.Count(tb_ => tb_.YEAR_LOSE < 0 && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "4");
            ViewBag.Total_Dealer = ChassisInvoiceTable.GroupBy(n => n.DEALER_CONTACT_NAME).Count();
            ViewBag.Total_Customer = ChassisInvoiceTable.GroupBy(n => n.CUSTOMER_NAME).Count();
            ViewBag.Total_Price_New = "-";
            ChassisInvoiceTable = ChassisInvoiceTable.ToPagedList(pageIndex, defaSize);



            
            ViewBag.group_year = tbls_year_report.ToList().OrderBy(i => i.year);
                ViewBag.group_year = 1;
            if (string.IsNullOrEmpty(page.ToString())) {
                page = 1;
            }
                ViewBag.page_this = page;
                ViewBag.list_group_array_invoice = list_group_array_invoice.ToList().OrderBy(i => i.YEAR_INVOICE);
                ViewBag.Page = pageIndex;
                ViewBag.PageMax = pageSize;
             
             
           
                decimal Outstanding = 0;
                
                List<string> id_lic = new List<string>();
                List<string> distinct = new List<string>();
           
            foreach (var tb_ in ChassisInvoiceTable)
                {
                DataRow dr = Export_TB.NewRow();
               
                Outstanding = 0;
                    decimal Sum_Total = 0;
                    mod = 0;

                /*COLOR*/
                string color = "";
                if (tb_.YEAR_LOSE >= 0 && tb_.INVOICE_NO == null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "1")
                {
                    color = "odd bg-orange";
                }
                if (tb_.YEAR_LOSE > 0 && tb_.INVOICE_NO != null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "2")
                {
                    color = "odd bg-yellow";
                }
                 if (tb_.YEAR_LOSE == 0 && tb_.INVOICE_NO != null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "3")
                {
                    color = "odd bg-green";
                }
               if (tb_.YEAR_LOSE < 0 && tb_.STATUS_FLAG == "" ||tb_.STATUS_FLAG == "4")
                {
                    color = "odd bg-purple";
                }
              
                dr[1] = color+":"+tb_.STATUS_MEI+":"+tb_.STATUS_LICENSE;

                System.Diagnostics.Debug.WriteLine("setazzzzz_" + page + "_ss" + tb_.ID_CHASSIS);
                /*INVOICE_GET*/
                int row_group_customer = 0;
                int y_check_last = 0;
                foreach (var item in group_year_report.OrderBy(r=>r.year))
                {
                    
                    int i = 1;
                    int x_inv = -1;
                    int dmy_inv = 1;
                    foreach (var item_group in list_group_array_invoice.OrderBy(r=>r.YEAR_INVOICE))
                    {
                        if (item_group.ID_CHASSIS == tb_.ID_CHASSIS)
                        {
                            i++;
                            x_inv++;

                            String inv = "";
                            String inv_year = "";
                            String inv_date = "";
                            String price = "";
                            String inv_price = "";
                            inv = item_group.ID_INVOICE;
                            inv_date = item_group.DATE_INVOICE;
                            inv_year = item_group.YEAR_INVOICE;
                            inv_price = item_group.PRICE_INVOICE;
                            price = "x";
                            /*INVOICENO*/
                           

                            string day_inv = "";
                            if (inv_date.Split('/').Length > 1)
                            {
                                day_inv = inv_date.Split('/')[1].ToString();
                            }
                            /*INVOICEDATE*/
                            string month_inv = "";
                            if (inv_date.Split('/').Length > 1)
                            {
                                month_inv = "/" + inv_date.Split('/')[2].ToString();
                            }

                            string year_inv = "";
                            if (inv_date.Split('/').Length > 1)
                            {
                                int year_invsum = Convert.ToInt32(inv_date.Split('/')[0]) + 543;
                                year_inv = "/" + year_invsum.ToString();
                            }
                            dr[2 + x_inv] = inv.Replace("br", Environment.NewLine);
                            dr[3 + x_inv] = day_inv + month_inv + year_inv;
                            dr[4 + x_inv] = inv_price;
                            x_inv += 2;
                         
                            row_group_customer = x_inv + 2;

                            //if (!string.IsNullOrEmpty(inv_price))
                            //{
                            //    if (inv_price.Length > 1)
                            //    {
                            //        Sum_Total = Sum_Total + Convert.ToDecimal(inv_price);
                            //        Outstanding = Convert.ToDecimal(inv_price);
                            //    }
                            //}
                            //else
                            //{
                            //    mod = mod + 1;
                            //}
                            //if (Convert.ToInt32(DateTime.Now.Year) == Convert.ToInt32(inv_year)) {
                            //    if (!string.IsNullOrEmpty(inv_price))
                            //    {
                            //        y_check_last = 1;
                                  
                            //    }
                            //}
                        }
                       
                        if (i.ToString() == item.count)
                        {
                            break;
                        }

                    }
                    break;
                }

                System.Diagnostics.Debug.WriteLine(tb_.INVOICE_CUSTOMER + "eeeeeeeeeeeeewwwwwwwwwwwww" + pageIndex);
                ///*INVOICE_CUSTOMER*/
                dr[row_group_customer+1] = tb_.INVOICE_CUSTOMER;

                ///*INVOICE_CUSTOMER_PHONE*/
                dr[row_group_customer + 2] = tb_.INVOICE_CUSTOMER_PHONE;

             


                ///*INSTALL_DATE*/
                string day_ = tb_.INSTALL_DATE.Replace("  ", " ").Split(' ')[1];
                string month_ = tb_.INSTALL_DATE.Replace("  ", " ").Split(' ')[0];
                int year_ = Convert.ToInt32(tb_.INSTALL_DATE.Replace("  ", " ").Split(' ')[2]) + 543;

                dr[row_group_customer + 3] = day_ + "" + month_ + "" + year_;

                ///*ID_LICENSE*/
                Staring_LICENSE = "";
                try
                {
                    if (!string.IsNullOrEmpty(tb_.ID_LICENSE))
                    {

                        foreach (var s in tb_.ID_LICENSE.Split(','))
                        {
                            id_lic.Add(s.ToString() + ",");
                        }
                        distinct = id_lic.Distinct().ToList();
                        foreach (var s in distinct)
                        {
                            Staring_LICENSE = Staring_LICENSE + s;
                        }
                    }
                    else { }
                }
                catch
                {
                    Staring_LICENSE = tb_.ID_LICENSE;

                }

                id_lic.Clear();
                distinct.Clear();

                if (Staring_LICENSE.Replace(",,", "").Length > 3)
                {
                    Staring_LICENSE.Replace(",,", "");
               }
                string[] LICENSE_List = Staring_LICENSE.Split(',');
                string table_LICENSE = "";
               for (int i = 0; i < LICENSE_List.Length; i++)
                {
                   if (LICENSE_List[i].Length < 13)
                    {
                       table_LICENSE = table_LICENSE + "," + LICENSE_List[i];

                   }
                   
                }
                dr[row_group_customer+4] = table_LICENSE;
                ///*ID_CHASSIS*/
                dr[row_group_customer + 5] = tb_.ID_CHASSIS;
          
                ///*ID_MEI*/
                StaringMEI = "";
                List<string> id_lic_MEI = new List<string>();
                List<string> distinct_MEI = id_lic.Distinct().ToList();

                if (!string.IsNullOrEmpty(tb_.ID_MEI))
                {


                    foreach (var s in tb_.ID_MEI.Split(','))
                    {

                        id_lic.Add(s.ToString() + ",");

                    }
                    distinct_MEI = id_lic.Distinct().ToList();
                    foreach (var s in distinct_MEI)
                    {
                        StaringMEI = StaringMEI + s;
                    }
                }
                id_lic_MEI.Clear();
                distinct_MEI.Clear();
                string set = "x";
                if (StaringMEI.Replace(",,", "").Length > 3)
                { StaringMEI.Replace(",,", ""); }


                dr[row_group_customer + 6] = StaringMEI;

                ///*INVOICE_COUNT*/
                dr[row_group_customer + 7] = tb_.INVOICE_COUNT;
                ////decimal Outstanding_ = 0;

                int year_ins = Convert.ToInt32(DateTime.Now.Year) - Convert.ToInt32(tb_.INSTALL_DATE_.Year);
                decimal y = year_ins - mod- y_check_last;
                if (y > 0)
                {
                    ////Outstanding_ = Outstanding * y;
                }
                if (y <= 0)
                {
                
                    //////Outstanding_ = Outstanding;
                } 
                dr[row_group_customer + 8] = tb_.DEALER_CONTACT_NAME;
                dr[row_group_customer + 9] = tb_.DEALER_CONTACT_PHONE;

                dr[row_group_customer + 10] = tb_.ID_MEI_L;
                dr[row_group_customer + 11] = tb_.ID_LICENSE_L;
                y_check_last = 0;
                dr[row_group_customer + 12] = tb_.CUSTOMER_NAME;
                dr[row_group_customer + 13] = tb_.CUSTOMER_PHONE;

                Export_TB.Rows.Add(dr);
            }

            ViewBag.CHASSIS_SEARCH = CHASSIS_SEARCH;
            ViewBag.CUSTOMER_SEARCH = CUSTOMER_SEARCH;
            ViewBag.INV_CUSTOMER_SEARCH = INV_CUSTOMER_SEARCH;
            ViewBag.MONTH_SEARCH = MONTH_SEARCH;
            ViewBag.DELALER_SEARCH = DELALER_SEARCH;
            ViewBag.STATUS_SEARCH = STATUS_SEARCH;
            ViewBag.ORDER_SEARCH = ORDER_SEARCH;
            ViewBag.YEAR_SEARCH = YEAR_SEARCH;
            ViewBag.STARTDAY_SEARCH = STARTDAY_SEARCH;
            ViewBag.ENDDAY_SEARCH = ENDDAY_SEARCH;
            ViewBag.MEI_CERTIF = MEI_CERTIF;
            ViewBag.LICENSE_CERTIF = LICENSE_CERTIF;
            ViewBag.NOT_MEI = NOT_MEI;
            ViewBag.NOT_LICENSE = NOT_LICENSE;
            return View(Export_TB);
            //return Redirect("/Users/index");
        }
  
        [HttpPost]

        public ActionResult RespondToDetail(string id_detail)
        {
            var login_session_id = this.Session["session_id"];
            ViewBag.userid = login_session_id;
            if (id_detail != null)
            {

                var DLTDetailChassis = from t1 in db.DLTDetailChassis
                                       where t1.ID_CHASSIS == id_detail
                                       select t1;


                foreach (var value in DLTDetailChassis)
                {

                    return Json(new { flag = value.STATUS_FLAG, title = value.TITLE_DETAIL, user_detail = value.ID_USERS, textremark = value.REMARK, invoice_flag = value.INVOICE_FLAG, up_date_invoice_flag = value.UP_DATE_INVOICE_FLAG, up_date_flag_price = value.UP_DATE_PRICE_FLAG });
                }
                return Json(new { foo = "" + DLTDetailChassis.Count() });
                //    return Json(new { idchassis = tbdlt.ID, status = table_chassis.STATUS_FLAG, title = table_chassis.TITLE_DETAIL, iduser = table_chassis.ID_USERS, remark = table_chassis.REMARK });
            }
            return Json(new { foo = "" + id_detail });
        }
   
        public ActionResult ExcelOutput(string Status_,string d_,string m_,string y_)
        {
            Session["session_users_password"] = "admin@gmail.com";
            Session["session_id"] = "123";
            Session["session_users"] = "admin@gmail.com";

            ViewBag.Total_Customer = "0";
            ViewBag.Total_Dealer = "0";
            ViewBag.Total_Customer = "";
            ViewBag.Total_Dealer = "";
            ViewBag.Total_Price_New = "";
            ViewBag.CHASSIS_SEARCH = "";
            ViewBag.CUSTOMER_SEARCH = "";
            ViewBag.DELALER_SEARCH = "";
            ViewBag.STATUS_SEARCH = "";
            ViewBag.ORDER_SEARCH = "";
            ViewBag.MAXMIN_SEARCH = "";
            ViewBag.MONTH_SEARCH = "";
            ViewBag.YEAR_SEARCH = "";
            ViewBag.STARTDAY_SEARCH = "";
            ViewBag.ENDDAY_SEARCH = "";
            ViewBag.userid = 0;
            string Staring_LICENSE = "";
            string StaringMEI = "";
            decimal mod = 0;

            var login_users_password = this.Session["session_users_password"];
            var login_session_id = this.Session["session_id"];


            ViewBag.username = login_users_password;
            ViewBag.userid = login_session_id;
            string Start_year = "2016";
            if (string.IsNullOrEmpty(Start_year))
            {
                Start_year = "2016";
            }
        

            ViewBag.DLTChassis_InvoiceNot = db.DLTChassisInvoices.Where(a => a.INVOICE_NO == null).Count();
            ViewBag.DLTChassis_InvoiceOr = db.DLTChassisInvoices.Where(a => a.INVOICE_NO != null && a.YEAR_LOSE >= 0).Count();
            ViewBag.DLTChassis_InvoiceSc = db.DLTChassisInvoices.Where(a => a.INVOICE_NO != null && a.YEAR_LOSE == 0).Count();
            ViewBag.DLTChassis_InvoiceNext = db.DLTChassisInvoices.Where(a => a.YEAR_LOSE < 0).Count();

            int pageIndex = 1;
          

            List<ls_year_report> tbls_year_report = new List<ls_year_report>();
            IPagedList<DLTChassisInvoice> ChassisInvoice_Table = null;
            string param = "ID";



            Regex charsToDestroy = new Regex(@"[^\d|\,\-]");

            var ChassisInvoiceTable_MEI = db.DLTChassisInvoices.ToList();

       
            string myStringOutput = String.Join(",", db.DLTDetailChassis.Select(p => p.INVOICE_FLAG.ToString()).ToArray());
            //System.Diagnostics.Debug.WriteLine(myStringOutput + "aeeeeeeeeeeeeeeeeeeeeaaww");
            var ChassisInvoiceTable = from dltcha in ChassisInvoiceTable_MEI
                                      join dltdetail in db.DLTDetailChassis on dltcha.ID_CHASSIS equals dltdetail.ID_CHASSIS into d
                                      from table in d.DefaultIfEmpty()

                                      select new Chassis_Status_Model
                                      {
                                          STATUS_FLAG = table == null ? "" : table.STATUS_FLAG,
                                          ID_CHASSIS = dltcha.ID_CHASSIS,
                                          INVOICE_COUNT = table == null ? dltcha.INVOICE_COUNT : (Convert.ToInt32(dltcha.INVOICE_COUNT.ToString()) + table.INVOICE_FLAG.Split(',').Count()).ToString(),
                                          INVOICE_NO = table == null ? dltcha.INVOICE_NO : dltcha.INVOICE_NO + String.Join(",", db.DLTDetailChassis.Where(r => r.ID_CHASSIS == dltcha.ID_CHASSIS).Select(p => p.INVOICE_FLAG.ToString()).ToArray()).ToString(),
                                          INVOICE_COUNT_LOSE = dltcha.INVOICE_COUNT_LOSE,
                                          YEAR_LOSE = Convert.ToInt32(dltcha.YEAR_LOSE),
                                          INSTALL_DATE = dltcha.INSTALL_DATE,
                                          DEALER_CONTACT_NAME = dltcha.DEALER_CONTACT_NAME,
                                          DEALER_CONTACT_PHONE = dltcha.DEALER_CONTACT_PHONE,
                                          ID_LICENSE = dltcha.ID_LICENSE,
                                          ID_MEI = dltcha.ID_MEI,
                                          ID_LICENSE_L = dltcha.CER_LICENSE,
                                          ID_MEI_L = dltcha.CER_MEI,
                                          INVOICE_NO_DATE = table == null ? dltcha.INVOICE_NO_DATE : dltcha.INVOICE_NO_DATE + String.Join(",", db.DLTDetailChassis.Where(r => r.ID_CHASSIS == dltcha.ID_CHASSIS).Select(p => p.UP_DATE_INVOICE_FLAG.ToString()).ToArray()).ToString() + ",",
                                          INSTALL_YEAR = dltcha.INSTALL_YEAR,
                                          INVOICE_PRICE = table == null ? dltcha.INVOICE_PRICE : dltcha.INVOICE_PRICE + String.Join(",", db.DLTDetailChassis.Where(r => r.ID_CHASSIS == dltcha.ID_CHASSIS).Select(p => p.UP_DATE_PRICE_FLAG.ToString()).ToArray()).ToString(),
                                          CUSTOMER_NAME = dltcha.CUSTOMER_NAME,
                                          CUSTOMER_PHONE = dltcha.CUSTOMER_PHONE,
                                          INVOICE_CUSTOMER = dltcha.INVOICE_CUSTOMER,
                                          INVOICE_CUSTOMER_PHONE = dltcha.INVOICE_CUSTOMER_PHONE,
                                          ID = dltcha.ID,
                                          CER_LICENSE = dltcha.CER_LICENSE,
                                          INSTALL_DATE_ = Convert.ToDateTime(dltcha.INSTALL_DATE),
                                          INV_DATE_ = table == null ? DateTime.Now : Convert.ToDateTime(dltcha.INVOICE_NO_DATE.Split('(')[1].Split(')')[0]),
                                          INV_ID = table == null ? dltcha.INVOICE_NO : dltcha.INVOICE_NO.Split(',')[0].ToString(),
                                          MONTH_SEARCH = Convert.ToDateTime(dltcha.INSTALL_DATE).Month.ToString(),
                                          DAY_SEARCH = Convert.ToDateTime(dltcha.INSTALL_DATE).Day.ToString(),
                                          STATUS_DLT = dltcha.STATUS_DLT

                                      };

        
            var tb_grpup = db.DLTChassisInvoices
            .GroupBy(i => i.INSTALL_YEAR)
            .Select(group => new { year_group = group.Key });
            List<string> ls_year = new List<string>();

            foreach (var year_sort in tb_grpup)
            {
                String year_split = year_sort.ToString().Split('=')[1].Split(' ')[1];

                ls_year.Add(year_split);
            }


            foreach (var year_sort in tb_grpup)
            {
                String year_split = year_sort.ToString().Split('=')[1].Split(' ')[1];

                if (!string.IsNullOrEmpty(year_split))
                {
                    if (Convert.ToInt32(year_split) >= Convert.ToInt32(Start_year))
                    {
                        int total_count = Convert.ToInt32(DateTime.Now.Year.ToString()) - Convert.ToInt32(Start_year) + 2;
                        tbls_year_report.Add(new ls_year_report { year = year_split, count = total_count.ToString() });
                    }
                }
            }
            ViewBag.tb_group_year_report = tbls_year_report.ToList().OrderBy(i => i.year);
            var group_year_report = tbls_year_report.ToList().OrderBy(i => i.year);
            ViewBag.tb_group_year = ls_year.OrderBy(item => item);
            List<group_array_invoice> list_group_array_invoice = new List<group_array_invoice>();
            List<group_dup> list_group_dup = new List<group_dup>();
            list_group_array_invoice.Clear();

            decimal sum_all = 0;
            decimal sum_outs = 0;

   
            foreach (var tb in ChassisInvoiceTable)
            {

                string year_get = "";
                string idchassis_get = "";
                decimal sum_out_total = 0;
                decimal sum_out_year = 0;
                string year_dup = "";
                list_group_dup.Clear();
                if (!string.IsNullOrEmpty(tb.INVOICE_NO_DATE))
                {


                    for (int i = 0; i < tb.INVOICE_NO_DATE.Split(',').Length - 1; i++)
                    {

                        String INVOICE_PRICE_ = "0";
                        if (!string.IsNullOrEmpty(tb.INVOICE_PRICE.Split(',')[i]))
                        {
                            if (!tb.INVOICE_PRICE.Contains("SELECT"))
                            {

                                INVOICE_PRICE_ = tb.INVOICE_PRICE.Split(',')[i].Split('(')[1].Split(')')[0].ToString();

                            }
                            string year_inv = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0];
                            string year_compare = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0];
                            String year_dup_set = "";
                            string di = "0";


                            if (year_dup == "")
                            {

                                year_dup = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0];

                                list_group_dup.Add(new group_dup { ID = tb.ID_CHASSIS, YEAR = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0] });
                                di = "1";
                            }

                            if (list_group_dup.Count > 0)
                            {
                                bool exists = list_group_dup.Any(x => x.YEAR == year_compare);

                                if (exists == false)
                                {
                                    list_group_dup.Add(new group_dup { ID = tb.ID_CHASSIS, YEAR = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0] });
                                    di = "1";
                                }
                                year_dup_set = exists.ToString();
                            }

                            // if (check_dup ==true) { year_dup_set = "1"; }

                            /// System.Diagnostics.Debug.WriteLine("hhh" + tb.ID_CHASSIS + "ddsd" + tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0]);
                            if (di == "1")
                            {
                                list_group_array_invoice.Add(new group_array_invoice
                                {
                                    ID_CHASSIS = tb.ID_CHASSIS,
                                    ID_INVOICE = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[0] + "(" + tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0] + ")",
                                    DATE_YEAR = tb.INSTALL_DATE,
                                    YEAR = tb.INSTALL_YEAR,
                                    YEAR_INVOICE = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0],
                                    DATE_INVOICE = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0],
                                    COUNT = tb.INVOICE_NO_DATE.Split(',').Length,
                                    PRICE_INVOICE = INVOICE_PRICE_,
                                    YEAR_DUP = di
                                });
                            }
                            if (di == "0")
                            {
                                list_group_array_invoice.Where(r => r.ID_CHASSIS == tb.ID_CHASSIS && r.YEAR_INVOICE == tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0]).Select(c => { c.ID_INVOICE = c.ID_INVOICE + "br" + tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[0] + "(" + tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0] + ")"; return c; }).ToList();
                                list_group_array_invoice.Where(r => r.ID_CHASSIS == tb.ID_CHASSIS && r.YEAR_INVOICE == tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0]).Select(c => { c.YEAR_INVOICE = c.YEAR_INVOICE + "br" + tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0]; return c; });
                                list_group_array_invoice.Where(r => r.ID_CHASSIS == tb.ID_CHASSIS && r.YEAR_INVOICE == tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0]).Select(c => { c.DATE_INVOICE = c.DATE_INVOICE + "br" + tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0]; return c; });
                                list_group_array_invoice.Where(r => r.ID_CHASSIS == tb.ID_CHASSIS && r.YEAR_INVOICE == tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0]).Select(c => { c.PRICE_INVOICE = c.PRICE_INVOICE + "br" + INVOICE_PRICE_; return c; });
                            }

                            // if (tb.YEAR_LOSE >= 0) { sum_all = sum_all + Convert.ToDecimal(tb.INVOICE_PRICE.Split(',')[i].Split('(')[1].Split(')')[0]); }
                            if (tb.YEAR_LOSE >= 0)
                            {
                                try
                                {
                                    sum_all = sum_all + Convert.ToDecimal(tb.INVOICE_PRICE.Split(',')[i].Split('(')[1].Split(')')[0]);
                                }
                                catch { }

                            }
                        }
                    }
                    // if (tb.ID_CHASSIS == "010037") { sum_all = sum_all + sum_out_total*Convert.ToDecimal(tb.INVOICE_COUNT)  ; }

                    foreach (var strYear in tbls_year_report)
                    {
                        if (!tb.INVOICE_NO_DATE.Contains(strYear.year))
                        {
                            list_group_array_invoice.Add(new group_array_invoice { ID_CHASSIS = tb.ID_CHASSIS, ID_INVOICE = "-", DATE_YEAR = tb.INSTALL_DATE, YEAR = tb.INSTALL_YEAR, YEAR_INVOICE = strYear.year, DATE_INVOICE = "-", COUNT = tb.INVOICE_NO_DATE.Split(',').Length });

                        }
                    }
                }
                if (string.IsNullOrEmpty(tb.INVOICE_NO_DATE))
                {
                    foreach (var strYear in tbls_year_report)
                    {

                        list_group_array_invoice.Add(new group_array_invoice { ID_CHASSIS = tb.ID_CHASSIS, ID_INVOICE = "-", DATE_YEAR = tb.INSTALL_DATE, YEAR = tb.INSTALL_YEAR, DATE_INVOICE = "-", COUNT = 0, YEAR_INVOICE = "" });
                    }
                }
            }
    
            list_group_array_invoice = list_group_array_invoice.Distinct().ToList();

            int sort_table_id = 2;
            decimal sum_re = 0;
       


            ViewBag.DLTChassis_InvoiceNot = ChassisInvoiceTable.Where(tb_ => tb_.YEAR_LOSE >= 0 && tb_.INVOICE_NO == null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "1" && tb_.STATUS_DLT==1).Count();
            ViewBag.DLTChassis_InvoiceOr = ChassisInvoiceTable.Where(tb_ => tb_.YEAR_LOSE > 0 && tb_.INVOICE_NO != null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "2" && tb_.STATUS_DLT == 1).Count();
            ViewBag.DLTChassis_InvoiceSc = ChassisInvoiceTable.Where(tb_ => tb_.YEAR_LOSE == 0 && tb_.INVOICE_NO != null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "3" && tb_.STATUS_DLT == 1).Count();
            ViewBag.DLTChassis_InvoiceNext = ChassisInvoiceTable.Count(tb_ => tb_.YEAR_LOSE < 0 && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "4");
            ViewBag.Total_Dealer = ChassisInvoiceTable.GroupBy(n => n.DEALER_CONTACT_NAME).Count();
            ViewBag.Total_Customer = ChassisInvoiceTable.GroupBy(n => n.CUSTOMER_NAME).Count();
            ViewBag.Total_Price_New = "-";


            var Export_TB = new System.Data.DataTable();

            Export_TB.Columns.Add("1", typeof(string));
            Export_TB.Columns.Add("2", typeof(string));
            Export_TB.Columns.Add("3", typeof(string));
            Export_TB.Columns.Add("4", typeof(string));
            Export_TB.Columns.Add("5", typeof(string));
            Export_TB.Columns.Add("6", typeof(string));
            Export_TB.Columns.Add("7", typeof(string));
            Export_TB.Columns.Add("8", typeof(string));
            Export_TB.Columns.Add("9", typeof(string));
            Export_TB.Columns.Add("10", typeof(string));
            Export_TB.Columns.Add("11", typeof(string));
            Export_TB.Columns.Add("12", typeof(string));
            Export_TB.Columns.Add("13", typeof(string));
            Export_TB.Columns.Add("14", typeof(string));
            Export_TB.Columns.Add("15", typeof(string));
            Export_TB.Columns.Add("16", typeof(string));
            Export_TB.Columns.Add("17", typeof(string));
            Export_TB.Columns.Add("18", typeof(string));
            Export_TB.Columns.Add("19", typeof(string));
            Export_TB.Columns.Add("20", typeof(string));
            Export_TB.Columns.Add("21", typeof(string));
            Export_TB.Columns.Add("22", typeof(string));
            Export_TB.Columns.Add("23", typeof(string));
            Export_TB.Columns.Add("24", typeof(string));
            Export_TB.Columns.Add("25", typeof(string));
          
          

            ViewBag.group_year = tbls_year_report.ToList().OrderBy(i => i.year);
            ViewBag.group_year = 1;


            decimal Outstanding = 0;

            List<string> id_lic = new List<string>();
            List<string> distinct = new List<string>();
       
            if (!string.IsNullOrEmpty(Status_))
            {

                if (Status_ == "orange_except")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.Where(tb_ => tb_.YEAR_LOSE <= 0 && tb_.INVOICE_NO != null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG != "1" && tb_.STATUS_DLT == 1);
                }
                if (Status_ == "orange")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.Where(tb_ => tb_.YEAR_LOSE >= 0 && tb_.INVOICE_NO == null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "1" && tb_.STATUS_DLT == 1);
                }
                if (Status_ == "yellow")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.Where(tb_ => tb_.YEAR_LOSE > 0 && tb_.INVOICE_NO != null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "2" && tb_.STATUS_DLT == 1);

                }
                if (Status_ == "green")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.Where(tb_ => tb_.YEAR_LOSE == 0 && tb_.INVOICE_NO != null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "3" && tb_.STATUS_DLT == 1);
                }
                if (Status_ == "purple")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.Where(tb_ => tb_.YEAR_LOSE < 0 && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "4" && tb_.STATUS_DLT == 1);
                }
            }
            if (!string.IsNullOrEmpty(y_) && y_.Length > 0)
            {
               
                ChassisInvoiceTable.Where(i => i.INSTALL_YEAR == y_);

            }
            if (!string.IsNullOrEmpty(m_) && m_.Length > 0)
            {
             
                ChassisInvoiceTable = ChassisInvoiceTable.Where(tb_ => tb_.MONTH_SEARCH == m_);
            }

            if (!string.IsNullOrEmpty(d_) && d_.Length > 0)
            {
            
                ChassisInvoiceTable.Where(i => i.DAY_SEARCH == d_);
            }



            /// ChassisInvoiceTable = ChassisInvoiceTable.Where(i => i.ID == 482866);
            //    string Status_,string d_,string m_,string y_
            //  System.Diagnostics.Debug.WriteLine(ChassisInvoiceTable.Where(i => i.ID <= 475867).Count() + "aaeewew");
      
            foreach (var tb_ in ChassisInvoiceTable.OrderBy(r => r.INV_DATE_).OrderBy(r => r.INV_ID).OrderBy(r => r.INSTALL_DATE_).OrderBy(r => r.ID_LICENSE).OrderBy(r => r.ID_CHASSIS))
            {
                DataRow dr = Export_TB.NewRow();
                Outstanding = 0;
                decimal Sum_Total = 0;
                mod = 0;

                /*COLOR*/
                string color = "";
                if (tb_.YEAR_LOSE < 0 && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "4")
                {
                    color = "odd bg-purple";
                }

                if (tb_.YEAR_LOSE == 0 && tb_.INVOICE_NO != null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "3")
                {
                    color = "odd bg-green";
                }
                if (tb_.YEAR_LOSE > 0 && tb_.INVOICE_NO != null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "2")
                {
                    color = "odd bg-yellow";
                }
                if (tb_.YEAR_LOSE >= 0 && tb_.INVOICE_NO == null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "1")
                {
                    color = "odd bg-orange";
                }
      
               
                /*INVOICE_GET*/
                int row_group_customer = 0;
                foreach (var item in group_year_report.OrderBy(r => r.year))
                {

                    int i = 1;
                    int x_inv = -1;
                    int dmy_inv = 1;
                    foreach (var item_group in list_group_array_invoice.OrderBy(r => r.YEAR_INVOICE))
                    {
                        if (item_group.ID_CHASSIS == tb_.ID_CHASSIS)
                        {
                            i++;
                            x_inv++;

                            String inv = "";
                            String inv_year = "";
                            String inv_date = "";
                            String price = "";
                            String inv_price = "";
                            inv = item_group.ID_INVOICE;
                            inv_date = item_group.DATE_INVOICE;
                            inv_year = item_group.YEAR_INVOICE;
                            inv_price = item_group.PRICE_INVOICE;
                            price = "x";
                            /*INVOICENO*/


                            string day_inv = "";
                            if (inv_date.Split('/').Length > 1)
                            {
                                day_inv = inv_date.Split('/')[1].ToString();
                            }
                            /*INVOICEDATE*/
                            string month_inv = "";
                            if (inv_date.Split('/').Length > 1)
                            {
                                month_inv = "/" + inv_date.Split('/')[2].ToString();
                            }

                            string year_inv = "";
                            if (inv_date.Split('/').Length > 1)
                            {
                                int year_invsum = Convert.ToInt32(inv_date.Split('/')[0]) + 543;
                                year_inv = "/" + year_invsum.ToString();
                            }
                            dr[0 + x_inv] = inv.Replace("br", Environment.NewLine);
                            dr[1 + x_inv] = day_inv + month_inv + year_inv;
                            dr[2 + x_inv] = inv_price;
                            x_inv += 2;

                            row_group_customer = x_inv;

                            //if (!string.IsNullOrEmpty(inv_price))
                            //{
                            //    if (inv_price.Length > 1)
                            //    {
                            //        Sum_Total = Sum_Total + Convert.ToDecimal(inv_price);
                            //        Outstanding = Convert.ToDecimal(inv_price);
                            //    }
                            //}
                            //else
                            //{
                            //    mod = mod + 1;
                            //}
                        }

                        if (i.ToString() == item.count)
                        {

                            break;
                        }

                    }
                    break;
                }


                ///*INVOICE_CUSTOMER*/
                dr[row_group_customer + 1] = tb_.INVOICE_CUSTOMER;

                ///*INVOICE_CUSTOMER_PHONE*/
                dr[row_group_customer + 2] = tb_.INVOICE_CUSTOMER_PHONE;




                ///*INSTALL_DATE*/
                string day_ = tb_.INSTALL_DATE.Replace("  ", " ").Split(' ')[1];
                string month_ = tb_.INSTALL_DATE.Replace("  ", " ").Split(' ')[0];
                int year_ = Convert.ToInt32(tb_.INSTALL_DATE.Replace("  ", " ").Split(' ')[2]) + 543;

                dr[row_group_customer + 3] = day_ + "" + month_ + "" + year_;

                ///*ID_LICENSE*/
                Staring_LICENSE = "";
                try
                {
                    if (!string.IsNullOrEmpty(tb_.ID_LICENSE))
                    {

                        foreach (var s in tb_.ID_LICENSE.Split(','))
                        {
                            id_lic.Add(s.ToString() + ",");
                        }
                        distinct = id_lic.Distinct().ToList();
                        foreach (var s in distinct)
                        {
                            Staring_LICENSE = Staring_LICENSE + s;
                        }
                    }
                    else { }
                }
                catch
                {
                    Staring_LICENSE = tb_.ID_LICENSE;

                }

                id_lic.Clear();
                distinct.Clear();

                if (Staring_LICENSE.Replace(",,", "").Length > 3)
                {
                    Staring_LICENSE.Replace(",,", "");
                }
                string[] LICENSE_List = Staring_LICENSE.Split(',');
                string table_LICENSE = "";
                for (int i = 0; i < LICENSE_List.Length; i++)
                {
                    if (LICENSE_List[i].Length < 13)
                    {
                        table_LICENSE = table_LICENSE + "," + LICENSE_List[i];

                    }

                }
                dr[row_group_customer + 4] = table_LICENSE;
                ///*ID_CHASSIS*/
                dr[row_group_customer + 5] = tb_.ID_CHASSIS;
             
                ///*ID_MEI*/
                StaringMEI = "";
                List<string> id_lic_MEI = new List<string>();
                List<string> distinct_MEI = id_lic.Distinct().ToList();

                if (!string.IsNullOrEmpty(tb_.ID_MEI))
                {


                    foreach (var s in tb_.ID_MEI.Split(','))
                    {

                        id_lic.Add(s.ToString() + ",");

                    }
                    distinct_MEI = id_lic.Distinct().ToList();
                    foreach (var s in distinct_MEI)
                    {
                        StaringMEI = StaringMEI + s;
                    }
                }
                id_lic_MEI.Clear();
                distinct_MEI.Clear();

                if (StaringMEI.Replace(",,", "").Length > 3)
                { StaringMEI.Replace(",,", ""); }


                dr[row_group_customer + 6] = StaringMEI;

                ///*INVOICE_COUNT*/
                dr[row_group_customer + 7] = tb_.INVOICE_COUNT;

               
                dr[row_group_customer + 8] = tb_.DEALER_CONTACT_NAME;
                dr[row_group_customer + 9] = tb_.DEALER_CONTACT_PHONE;

              
                dr[row_group_customer + 10] = tb_.ID_MEI_L;
                dr[row_group_customer + 11] = tb_.ID_LICENSE_L;

                dr[row_group_customer + 12] = tb_.CUSTOMER_NAME;
                dr[row_group_customer + 13] = tb_.CUSTOMER_PHONE;

                //Outstanding = Outstanding * mod;
                //dr[row_group_customer + 1] = tb_.INVOICE_CUSTOMER;
                //dr[row_group_customer + 2] = tb_.INVOICE_CUSTOMER_PHONE;
                //dr[row_group_customer + 3] = tb_.DEALER_CONTACT_NAME;
                //dr[row_group_customer + 4] = tb_.DEALER_CONTACT_PHONE;
                //dr[row_group_customer + 5] = Sum_Total;
                //dr[row_group_customer + 6] = Outstanding;
                //dr[row_group_customer + 7] = tb_.CUSTOMER_NAME;
                //dr[row_group_customer + 8] = tb_.CUSTOMER_PHONE;

                Export_TB.Rows.Add(dr);
            }
            string Total_Dealer = ChassisInvoiceTable.GroupBy(n => n.DEALER_CONTACT_NAME).Count().ToString();
            string Total_Customer = ChassisInvoiceTable.GroupBy(n => n.CUSTOMER_NAME).Count().ToString();
            string Heard_top = "<div><table  border=\"1\">" +

                "<th> รายปี </th>" +
             
                "<th> 2559  </th>" +
                "<th></th>" +
                "<th></th>" +
                "<th>2560</th>" +
                "<th></th>" +
                "<th></th>" +
                "<th>2561</th>" +
                "<th></th>" +
                "<th></th>" +
                "<th>2562</th>" +
                "<th></th>" +
                  "<th></th>" +
                    "<th></th>" +
                      "<th></th>" +
                        "<th></th>" +
                          "<th></th>" +
                            "<th></th>" +
                              "<th></th>" +
                                "<th></th>" +
                                 "<th></th>" +
                            "<th></th>" +
                              "<th></th>" +
                                "<th></th>" +
                "<th></th></table></div>";


            string Heard_top2 = "<div><table border=\"1\" >" +
                        "<th>InvoiceNo</th>" +
                        "<th>Date</th>" +
                        "<th>Price</th>" +
                        "<th>InvoiceNo</th>" +
                        "<th>Date</th>" +
                        "<th>Price</th>" +
                        "<th>InvoiceNo</th>" +
                        "<th>Date</th>" +
                        "<th>Price</th>" +
                        "<th>InvoiceNo</th>" +
                        "<th>Date</th>" +
                        "<th>Price</th>" +
                         "<th>INVOICE_CUSTOMER</th>" +
                         "<th>INVOICE_CUSTOMER_PHONE</th>" +
                          "<th>INSTALL_DATE	</th>" +
                        "<th>ID_LICENSE	</th>" +
                        "<th>ID_CHASSIS</th>" +
                        "<th>	MEI	</th>" +
                        "<th> INVOICE_COUNT</th>"+
                        "<th> CUSTOMER_NAME</th>" +
                        "<th> CUSTOMER_PHONE</th>" +
                        "<th> MEI_CERTIF	</th>" +
                        "<th> LICENSE_CERTIF	</th>" +
                        "<th> DEALER_CONTACT_NAME	</th>" +
                        "<th> DEALER_CONTACT_PHONE	</th>" +
                                                      
                        "</table></div>";

            string Heard_ = "<div><table  border=\"1\">" +
             "<th>Dealer: " + Total_Dealer + " จำนวน</th>" +
              "<th>Customer: " + Total_Customer + " จำนวน</th>" +
             "</table></div>";


            var grid = new GridView();
            grid.ShowHeader = false;
            grid.DataSource = Export_TB;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            string D = DateTime.Now.Date.ToString();
            string Mon = DateTime.Now.Month.ToString();
            string M = DateTime.Now.Millisecond.ToString();
            Response.AddHeader("content-disposition", "attachment; filename=" + D + "" + Mon + "" + M + "Output.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(Heard_ + Heard_top + Heard_top2 + sw.ToString().Replace("<td>&nbsp;</td><td>&#39;", "<td>"));

            Response.Flush();
            Response.End();

            return RedirectToAction("Index", "ExportData");
            //var grid = new GridView();
            //grid.ShowHeader = false;
            //grid.DataSource = Export_TB;
            //grid.DataBind();

            //Response.ClearContent();
            //Response.Buffer = true;
            //string D = DateTime.Now.Date.ToString();
            //string Mon = DateTime.Now.Month.ToString();
            //string M = DateTime.Now.Millisecond.ToString();
            //Response.AddHeader("content-disposition", "attachment; filename=" + D + "" + Mon + "" + M + "Output.xls");
            //Response.ContentType = "application/ms-excel";

            //Response.Charset = "";
            //StringWriter sw = new StringWriter();
            //HtmlTextWriter htw = new HtmlTextWriter(sw);


            //for (int i = 0; i < grid.Rows.Count; i++)
            //{
            //    GridViewRow row = grid.Rows[i];
            //    //Apply text style to each Row
            //    row.Attributes.Add("class", "textmode");
            //}
            //string style = @"<style> .textmode { mso-number-format:\@; } </style>";
            //Response.Write(style);

            //grid.RenderControl(htw);
            //int j = 1;
            ////This loop is used to apply stlye to cells based on particular row
            //foreach (GridViewRow gvrow in grid.Rows)
            //{
            //    gvrow.BackColor = Color.White;
            //    if (j <= grid.Rows.Count)
            //    {
            //        if (j % 2 != 0)
            //        {
            //            for (int k = 0; k < gvrow.Cells.Count; k++)
            //            {
            //             //   gvrow.Cells[k].Style.Add("background-color", "#EFF3FB");
            //            }
            //        }
            //    }
            //    j++;
            //}


            //grid.RenderControl(htw);
            //Response.Output.Write(Heard_ + Heard_top + Heard_top2 + sw.ToString().Replace("<td>&nbsp;</td><td>&#39;", "<td>"));

            //Response.Flush();
            //Response.End();

            //return RedirectToAction("Index", "ExportData");
            ////return Redirect("/Users/index");


        }
        public ActionResult ExcelOutputMei(string Status_, string d_, string m_, string y_)
        {
            Session["session_users_password"] = "admin@gmail.com";
            Session["session_id"] = "123";
            Session["session_users"] = "admin@gmail.com";

            ViewBag.Total_Customer = "0";
            ViewBag.Total_Dealer = "0";
            ViewBag.Total_Customer = "";
            ViewBag.Total_Dealer = "";
            ViewBag.Total_Price_New = "";
            ViewBag.CHASSIS_SEARCH = "";
            ViewBag.CUSTOMER_SEARCH = "";
            ViewBag.DELALER_SEARCH = "";
            ViewBag.STATUS_SEARCH = "";
            ViewBag.ORDER_SEARCH = "";
            ViewBag.MAXMIN_SEARCH = "";
            ViewBag.MONTH_SEARCH = "";
            ViewBag.YEAR_SEARCH = "";
            ViewBag.STARTDAY_SEARCH = "";
            ViewBag.ENDDAY_SEARCH = "";
            ViewBag.userid = 0;
            string Staring_LICENSE = "";
            string StaringMEI = "";
            decimal mod = 0;

            var login_users_password = this.Session["session_users_password"];
            var login_session_id = this.Session["session_id"];


            ViewBag.username = login_users_password;
            ViewBag.userid = login_session_id;
            string Start_year = "2016";
            if (string.IsNullOrEmpty(Start_year))
            {
                Start_year = "2016";
            }
 

            ViewBag.DLTChassis_InvoiceNot = db.DLTChassisInvoice_Mei.Where(a => a.INVOICE_NO == null && a.MEI_NOT_FOUND ==1 && a.STATUS_DLT == 1).Count();
            ViewBag.DLTChassis_InvoiceOr = db.DLTChassisInvoice_Mei.Where(a => a.INVOICE_NO != null && a.YEAR_LOSE >= 0 && a.MEI_NOT_FOUND == 1 && a.STATUS_DLT == 1).Count();
            ViewBag.DLTChassis_InvoiceSc = db.DLTChassisInvoice_Mei.Where(a => a.INVOICE_NO != null && a.YEAR_LOSE == 0 && a.MEI_NOT_FOUND == 1 && a.STATUS_DLT == 1).Count();
            ViewBag.DLTChassis_InvoiceNext = db.DLTChassisInvoice_Mei.Where(a => a.YEAR_LOSE < 0 && a.MEI_NOT_FOUND == 1 && a.STATUS_DLT == 1).Count();

            int pageIndex = 1;
  

            List<ls_year_report> tbls_year_report = new List<ls_year_report>();
            IPagedList<DLTChassisInvoice> ChassisInvoice_Table = null;
            string param = "ID";



            Regex charsToDestroy = new Regex(@"[^\d|\,\-]");

            var ChassisInvoiceTable_MEI = db.DLTChassisInvoice_Mei.Where(a=>a.MEI_NOT_FOUND == 1 && a.STATUS_DLT == 1).ToList();

    
            string myStringOutput = String.Join(",", db.DLTDetailChassis.Select(p => p.INVOICE_FLAG.ToString()).ToArray());
  
            var ChassisInvoiceTable = from dltcha in ChassisInvoiceTable_MEI
                                      join dltdetail in db.DLTDetailChassis on dltcha.ID_CHASSIS equals dltdetail.ID_CHASSIS into d
                                      from table in d.DefaultIfEmpty()

                                      select new Chassis_Status_Model
                                      {
                                          STATUS_FLAG = table == null ? "" : table.STATUS_FLAG,
                                          ID_CHASSIS = dltcha.ID_CHASSIS,
                                          INVOICE_COUNT = table == null ? dltcha.INVOICE_COUNT : (Convert.ToInt32(dltcha.INVOICE_COUNT.ToString()) + table.INVOICE_FLAG.Split(',').Count()).ToString(),
                                          INVOICE_NO = table == null ? dltcha.INVOICE_NO : dltcha.INVOICE_NO + String.Join(",", db.DLTDetailChassis.Where(r => r.ID_CHASSIS == dltcha.ID_CHASSIS).Select(p => p.INVOICE_FLAG.ToString()).ToArray()).ToString(),
                                          INVOICE_COUNT_LOSE = dltcha.INVOICE_COUNT_LOSE,
                                          YEAR_LOSE = Convert.ToInt32(dltcha.YEAR_LOSE),
                                          INSTALL_DATE = dltcha.INSTALL_DATE,
                                          DEALER_CONTACT_NAME = dltcha.DEALER_CONTACT_NAME,
                                          DEALER_CONTACT_PHONE = dltcha.DEALER_CONTACT_PHONE,
                                          ID_LICENSE = dltcha.ID_LICENSE,
                                          ID_MEI = dltcha.ID_MEI,
                                          ID_MEI_L =dltcha.CER_MEI,
                                          ID_LICENSE_L = dltcha.CER_LICENSE,
                                          INVOICE_NO_DATE = table == null ? dltcha.INVOICE_NO_DATE : dltcha.INVOICE_NO_DATE + String.Join(",", db.DLTDetailChassis.Where(r => r.ID_CHASSIS == dltcha.ID_CHASSIS).Select(p => p.UP_DATE_INVOICE_FLAG.ToString()).ToArray()).ToString() + ",",
                                          INSTALL_YEAR = dltcha.INSTALL_YEAR,
                                          INVOICE_PRICE = table == null ? dltcha.INVOICE_PRICE : dltcha.INVOICE_PRICE + String.Join(",", db.DLTDetailChassis.Where(r => r.ID_CHASSIS == dltcha.ID_CHASSIS).Select(p => p.UP_DATE_PRICE_FLAG.ToString()).ToArray()).ToString(),
                                          CUSTOMER_NAME = dltcha.CUSTOMER_NAME,
                                          CUSTOMER_PHONE = dltcha.CUSTOMER_PHONE,
                                          INVOICE_CUSTOMER = dltcha.INVOICE_CUSTOMER,
                                          INVOICE_CUSTOMER_PHONE = dltcha.INVOICE_CUSTOMER_PHONE,
                                          ID = dltcha.ID,

                                          CER_LICENSE = dltcha.CER_LICENSE,
                                          INSTALL_DATE_ = Convert.ToDateTime(dltcha.INSTALL_DATE),
                                          INV_DATE_ = table == null ? DateTime.Now : Convert.ToDateTime(dltcha.INVOICE_NO_DATE.Split('(')[1].Split(')')[0]),
                                          INV_ID = table == null ? dltcha.INVOICE_NO : dltcha.INVOICE_NO.Split(',')[0].ToString(),
                                          MONTH_SEARCH = Convert.ToDateTime(dltcha.INSTALL_DATE).Month.ToString(),
                                          DAY_SEARCH = Convert.ToDateTime(dltcha.INSTALL_DATE).Day.ToString()

                                      };

            var tb_grpup = db.DLTChassisInvoice_Mei
            .GroupBy(i => i.INSTALL_YEAR)
            .Select(group => new { year_group = group.Key });
            List<string> ls_year = new List<string>();

            foreach (var year_sort in tb_grpup)
            {
                String year_split = year_sort.ToString().Split('=')[1].Split(' ')[1];

                ls_year.Add(year_split);
            }


            foreach (var year_sort in tb_grpup)
            {
                String year_split = year_sort.ToString().Split('=')[1].Split(' ')[1];

                if (!string.IsNullOrEmpty(year_split))
                {
                    if (Convert.ToInt32(year_split) >= Convert.ToInt32(Start_year))
                    {
                        int total_count = Convert.ToInt32(DateTime.Now.Year.ToString()) - Convert.ToInt32(Start_year) + 2;
                        tbls_year_report.Add(new ls_year_report { year = year_split, count = total_count.ToString() });
                    }
                }
            }
            ViewBag.tb_group_year_report = tbls_year_report.ToList().OrderBy(i => i.year);
            var group_year_report = tbls_year_report.ToList().OrderBy(i => i.year);
            ViewBag.tb_group_year = ls_year.OrderBy(item => item);
            List<group_array_invoice> list_group_array_invoice = new List<group_array_invoice>();
            List<group_dup> list_group_dup = new List<group_dup>();
            list_group_array_invoice.Clear();

            decimal sum_all = 0;
            decimal sum_outs = 0;

      
            foreach (var tb in ChassisInvoiceTable)
            {

                string year_get = "";
                string idchassis_get = "";
                decimal sum_out_total = 0;
                decimal sum_out_year = 0;
                string year_dup = "";
                list_group_dup.Clear();
                if (!string.IsNullOrEmpty(tb.INVOICE_NO_DATE))
                {


                    for (int i = 0; i < tb.INVOICE_NO_DATE.Split(',').Length - 1; i++)
                    {

                        String INVOICE_PRICE_ = "0";
                        if (!string.IsNullOrEmpty(tb.INVOICE_PRICE.Split(',')[i]))
                        {
                            if (!tb.INVOICE_PRICE.Contains("SELECT"))
                            {

                                INVOICE_PRICE_ = tb.INVOICE_PRICE.Split(',')[i].Split('(')[1].Split(')')[0].ToString();

                            }
                            string year_inv = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0];
                            string year_compare = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0];
                            String year_dup_set = "";
                            string di = "0";


                            if (year_dup == "")
                            {

                                year_dup = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0];

                                list_group_dup.Add(new group_dup { ID = tb.ID_CHASSIS, YEAR = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0] });
                                di = "1";
                            }

                            if (list_group_dup.Count > 0)
                            {
                                bool exists = list_group_dup.Any(x => x.YEAR == year_compare);

                                if (exists == false)
                                {
                                    list_group_dup.Add(new group_dup { ID = tb.ID_CHASSIS, YEAR = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0] });
                                    di = "1";
                                }
                                year_dup_set = exists.ToString();
                            }

                            // if (check_dup ==true) { year_dup_set = "1"; }

                            /// System.Diagnostics.Debug.WriteLine("hhh" + tb.ID_CHASSIS + "ddsd" + tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0]);
                            if (di == "1")
                            {
                                list_group_array_invoice.Add(new group_array_invoice
                                {
                                    ID_CHASSIS = tb.ID_CHASSIS,
                                    ID_INVOICE = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[0] + "(" + tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0] + ")",
                                    DATE_YEAR = tb.INSTALL_DATE,
                                    YEAR = tb.INSTALL_YEAR,
                                    YEAR_INVOICE = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0],
                                    DATE_INVOICE = tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0],
                                    COUNT = tb.INVOICE_NO_DATE.Split(',').Length,
                                    PRICE_INVOICE = INVOICE_PRICE_,
                                    YEAR_DUP = di
                                });
                            }
                            if (di == "0")
                            {
                                list_group_array_invoice.Where(r => r.ID_CHASSIS == tb.ID_CHASSIS && r.YEAR_INVOICE == tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0]).Select(c => { c.ID_INVOICE = c.ID_INVOICE + "br" + tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[0] + "(" + tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0] + ")"; return c; }).ToList();
                                list_group_array_invoice.Where(r => r.ID_CHASSIS == tb.ID_CHASSIS && r.YEAR_INVOICE == tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0]).Select(c => { c.YEAR_INVOICE = c.YEAR_INVOICE + "br" + tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0]; return c; });
                                list_group_array_invoice.Where(r => r.ID_CHASSIS == tb.ID_CHASSIS && r.YEAR_INVOICE == tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0]).Select(c => { c.DATE_INVOICE = c.DATE_INVOICE + "br" + tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0]; return c; });
                                list_group_array_invoice.Where(r => r.ID_CHASSIS == tb.ID_CHASSIS && r.YEAR_INVOICE == tb.INVOICE_NO_DATE.Split(',')[i].Split('(')[1].Split(')')[0].Split('/')[0]).Select(c => { c.PRICE_INVOICE = c.PRICE_INVOICE + "br" + INVOICE_PRICE_; return c; });
                            }

                            // if (tb.YEAR_LOSE >= 0) { sum_all = sum_all + Convert.ToDecimal(tb.INVOICE_PRICE.Split(',')[i].Split('(')[1].Split(')')[0]); }
                            if (tb.YEAR_LOSE >= 0)
                            {
                                try
                                {
                                    sum_all = sum_all + Convert.ToDecimal(tb.INVOICE_PRICE.Split(',')[i].Split('(')[1].Split(')')[0]);
                                }
                                catch { }

                            }
                        }
                    }
                    // if (tb.ID_CHASSIS == "010037") { sum_all = sum_all + sum_out_total*Convert.ToDecimal(tb.INVOICE_COUNT)  ; }

                    foreach (var strYear in tbls_year_report)
                    {
                        if (!tb.INVOICE_NO_DATE.Contains(strYear.year))
                        {
                            list_group_array_invoice.Add(new group_array_invoice { ID_CHASSIS = tb.ID_CHASSIS, ID_INVOICE = "-", DATE_YEAR = tb.INSTALL_DATE, YEAR = tb.INSTALL_YEAR, YEAR_INVOICE = strYear.year, DATE_INVOICE = "-", COUNT = tb.INVOICE_NO_DATE.Split(',').Length });

                        }
                    }
                }
                if (string.IsNullOrEmpty(tb.INVOICE_NO_DATE))
                {
                    foreach (var strYear in tbls_year_report)
                    {

                        list_group_array_invoice.Add(new group_array_invoice { ID_CHASSIS = tb.ID_CHASSIS, ID_INVOICE = "-", DATE_YEAR = tb.INSTALL_DATE, YEAR = tb.INSTALL_YEAR, DATE_INVOICE = "-", COUNT = 0, YEAR_INVOICE = "" });
                    }
                }
            }
        
            list_group_array_invoice = list_group_array_invoice.Distinct().ToList();

            int sort_table_id = 2;
            decimal sum_re = 0;
        


            ViewBag.DLTChassis_InvoiceNot = ChassisInvoiceTable.Where(tb_ => tb_.YEAR_LOSE >= 0 && tb_.INVOICE_NO == null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "1").Count();
            ViewBag.DLTChassis_InvoiceOr = ChassisInvoiceTable.Where(tb_ => tb_.YEAR_LOSE > 0 && tb_.INVOICE_NO != null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "2").Count();
            ViewBag.DLTChassis_InvoiceSc = ChassisInvoiceTable.Where(tb_ => tb_.YEAR_LOSE == 0 && tb_.INVOICE_NO != null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "3").Count();
            ViewBag.DLTChassis_InvoiceNext = ChassisInvoiceTable.Count(tb_ => tb_.YEAR_LOSE < 0 && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "4");
            ViewBag.Total_Dealer = ChassisInvoiceTable.GroupBy(n => n.DEALER_CONTACT_NAME).Count();
            ViewBag.Total_Customer = ChassisInvoiceTable.GroupBy(n => n.CUSTOMER_NAME).Count();
            ViewBag.Total_Price_New = "-";


            var Export_TB = new System.Data.DataTable();
        
            Export_TB.Columns.Add("1", typeof(string));
            Export_TB.Columns.Add("2", typeof(string));
            Export_TB.Columns.Add("3", typeof(string));
            Export_TB.Columns.Add("4", typeof(string));
            Export_TB.Columns.Add("5", typeof(string));
            Export_TB.Columns.Add("6", typeof(string));
            Export_TB.Columns.Add("7", typeof(string));
            Export_TB.Columns.Add("8", typeof(string));
            Export_TB.Columns.Add("9", typeof(string));
            Export_TB.Columns.Add("10", typeof(string));
            Export_TB.Columns.Add("11", typeof(string));
            Export_TB.Columns.Add("12", typeof(string));
            Export_TB.Columns.Add("13", typeof(string));
            Export_TB.Columns.Add("14", typeof(string));
            Export_TB.Columns.Add("15", typeof(string));
            Export_TB.Columns.Add("16", typeof(string));
            Export_TB.Columns.Add("17", typeof(string));
            Export_TB.Columns.Add("18", typeof(string));
            Export_TB.Columns.Add("19", typeof(string));
            Export_TB.Columns.Add("20", typeof(string));
            Export_TB.Columns.Add("21", typeof(string));
            Export_TB.Columns.Add("22", typeof(string));
            Export_TB.Columns.Add("23", typeof(string));
            Export_TB.Columns.Add("24", typeof(string));
            Export_TB.Columns.Add("25", typeof(string));
         





            ViewBag.group_year = tbls_year_report.ToList().OrderBy(i => i.year);
            ViewBag.group_year = 1;




            decimal Outstanding = 0;
        
            List<string> id_lic = new List<string>();
            List<string> distinct = new List<string>();

            if (!string.IsNullOrEmpty(Status_))
            {
             
                if (Status_ == "orange")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.Where(tb_ => tb_.YEAR_LOSE >= 0 && tb_.INVOICE_NO == null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "1");
                }
                if (Status_ == "yellow")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.Where(tb_ => tb_.YEAR_LOSE > 0 && tb_.INVOICE_NO != null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "2");

                }
                if (Status_ == "green")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.Where(tb_ => tb_.YEAR_LOSE == 0 && tb_.INVOICE_NO != null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "3");
                }
                if (Status_ == "purple")
                {
                    ChassisInvoiceTable = ChassisInvoiceTable.Where(tb_ => tb_.YEAR_LOSE < 0 && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "4");
                }
            }
            
            if (!string.IsNullOrEmpty(y_) && y_.Length > 0)
            {
           
                ChassisInvoiceTable.Where(i => i.INSTALL_YEAR == y_);

            }
            if (!string.IsNullOrEmpty(m_) && m_.Length > 0)
            {
       
                ChassisInvoiceTable = ChassisInvoiceTable.Where(tb_ => tb_.MONTH_SEARCH == m_);
            }

            if (!string.IsNullOrEmpty(d_) && d_.Length > 0)
            {
          
                ChassisInvoiceTable.Where(i => i.DAY_SEARCH == d_);
            }
         
            /// ChassisInvoiceTable = ChassisInvoiceTable.Where(i => i.ID == 482866);
            //    string Status_,string d_,string m_,string y_
            //  System.Diagnostics.Debug.WriteLine(ChassisInvoiceTable.Where(i => i.ID <= 475867).Count() + "aaeewew");
            foreach (var tb_ in ChassisInvoiceTable.OrderBy(r => r.INV_DATE_).OrderBy(r => r.INV_ID).OrderBy(r => r.INSTALL_DATE_).OrderBy(r => r.ID_LICENSE).OrderBy(r => r.ID_CHASSIS))
            {
                DataRow dr = Export_TB.NewRow();
                Outstanding = 0;
                decimal Sum_Total = 0;
                mod = 0;

                /*COLOR*/
                string color = "";
                if (tb_.YEAR_LOSE < 0 && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "4")
                {
                    color = "odd bg-purple";
                }

                if (tb_.YEAR_LOSE == 0 && tb_.INVOICE_NO != null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "3")
                {
                    color = "odd bg-green";
                }
                if (tb_.YEAR_LOSE > 0 && tb_.INVOICE_NO != null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "2")
                {
                    color = "odd bg-yellow";
                }
                if (tb_.YEAR_LOSE >= 0 && tb_.INVOICE_NO == null && tb_.STATUS_FLAG == "" || tb_.STATUS_FLAG == "1")
                {
                    color = "odd bg-orange";
                }


                /*INVOICE_GET*/
                int row_group_customer = 0;
                foreach (var item in group_year_report.OrderBy(r => r.year))
                {

                    int i = 1;
                    int x_inv = -1;
                    int dmy_inv = 1;
                    foreach (var item_group in list_group_array_invoice.OrderBy(r => r.YEAR_INVOICE))
                    {
                        if (item_group.ID_CHASSIS == tb_.ID_CHASSIS)
                        {
                            i++;
                            x_inv++;

                            String inv = "";
                            String inv_year = "";
                            String inv_date = "";
                            String price = "";
                            String inv_price = "";
                            inv = item_group.ID_INVOICE;
                            inv_date = item_group.DATE_INVOICE;
                            inv_year = item_group.YEAR_INVOICE;
                            inv_price = item_group.PRICE_INVOICE;
                            price = "x";
                            /*INVOICENO*/


                            string day_inv = "";
                            if (inv_date.Split('/').Length > 1)
                            {
                                day_inv = inv_date.Split('/')[1].ToString();
                            }
                            /*INVOICEDATE*/
                            string month_inv = "";
                            if (inv_date.Split('/').Length > 1)
                            {
                                month_inv = "/" + inv_date.Split('/')[2].ToString();
                            }

                            string year_inv = "";
                            if (inv_date.Split('/').Length > 1)
                            {
                                int year_invsum = Convert.ToInt32(inv_date.Split('/')[0]) + 543;
                                year_inv = "/" + year_invsum.ToString();
                            }
                            dr[0 + x_inv] = inv.Replace("br", Environment.NewLine);
                            dr[1 + x_inv] = day_inv + month_inv + year_inv;
                            dr[2 + x_inv] = inv_price;
                            x_inv += 2;

                            row_group_customer = x_inv;

                            //if (!string.IsNullOrEmpty(inv_price))
                            //{
                            //    if (inv_price.Length > 1)
                            //    {
                            //        Sum_Total = Sum_Total + Convert.ToDecimal(inv_price);
                            //        Outstanding = Convert.ToDecimal(inv_price);
                            //    }
                            //}
                            //else
                            //{
                            //    mod = mod + 1;
                            //}
                        }

                        if (i.ToString() == item.count)
                        {

                            break;
                        }

                    }
                    break;
                }


                ///*INVOICE_CUSTOMER*/
                dr[row_group_customer + 1] = tb_.INVOICE_CUSTOMER;

                ///*INVOICE_CUSTOMER_PHONE*/
                dr[row_group_customer + 2] = tb_.INVOICE_CUSTOMER_PHONE;




                ///*INSTALL_DATE*/
                string day_ = tb_.INSTALL_DATE.Replace("  ", " ").Split(' ')[1];
                string month_ = tb_.INSTALL_DATE.Replace("  ", " ").Split(' ')[0];
                int year_ = Convert.ToInt32(tb_.INSTALL_DATE.Replace("  ", " ").Split(' ')[2]) + 543;

                dr[row_group_customer + 3] = day_ + "" + month_ + "" + year_;

                ///*ID_LICENSE*/
                Staring_LICENSE = "";
                try
                {
                    if (!string.IsNullOrEmpty(tb_.ID_LICENSE))
                    {

                        foreach (var s in tb_.ID_LICENSE.Split(','))
                        {
                            id_lic.Add(s.ToString() + ",");
                        }
                        distinct = id_lic.Distinct().ToList();
                        foreach (var s in distinct)
                        {
                            Staring_LICENSE = Staring_LICENSE + s;
                        }
                    }
                    else { }
                }
                catch
                {
                    Staring_LICENSE = tb_.ID_LICENSE;

                }

                id_lic.Clear();
                distinct.Clear();

                if (Staring_LICENSE.Replace(",,", "").Length > 3)
                {
                    Staring_LICENSE.Replace(",,", "");
                }
                string[] LICENSE_List = Staring_LICENSE.Split(',');
                string table_LICENSE = "";
                for (int i = 0; i < LICENSE_List.Length; i++)
                {
                    if (LICENSE_List[i].Length < 13)
                    {
                        table_LICENSE = table_LICENSE + "," + LICENSE_List[i];

                    }

                }
                dr[row_group_customer + 4] = table_LICENSE;
                ///*ID_CHASSIS*/
                dr[row_group_customer + 5] = tb_.ID_CHASSIS;

                ///*ID_MEI*/
                StaringMEI = "";
                List<string> id_lic_MEI = new List<string>();
                List<string> distinct_MEI = id_lic.Distinct().ToList();

                if (!string.IsNullOrEmpty(tb_.ID_MEI))
                {


                    foreach (var s in tb_.ID_MEI.Split(','))
                    {

                        id_lic.Add(s.ToString() + ",");

                    }
                    distinct_MEI = id_lic.Distinct().ToList();
                    foreach (var s in distinct_MEI)
                    {
                        StaringMEI = StaringMEI + s;
                    }
                }
                id_lic_MEI.Clear();
                distinct_MEI.Clear();

                if (StaringMEI.Replace(",,", "").Length > 3)
                { StaringMEI.Replace(",,", ""); }


                dr[row_group_customer + 6] = StaringMEI;

                ///*INVOICE_COUNT*/
                dr[row_group_customer + 7] = tb_.INVOICE_COUNT;



                dr[row_group_customer + 8] = tb_.DEALER_CONTACT_NAME;
                dr[row_group_customer + 9] = tb_.DEALER_CONTACT_PHONE;

                dr[row_group_customer + 10] = tb_.ID_MEI_L;
                dr[row_group_customer + 11] = tb_.ID_LICENSE_L;

                dr[row_group_customer + 12] = tb_.CUSTOMER_NAME;
                dr[row_group_customer + 13] = tb_.CUSTOMER_PHONE;

             

                Export_TB.Rows.Add(dr);
            }
            string Total_Dealer = ChassisInvoiceTable.GroupBy(n => n.DEALER_CONTACT_NAME).Count().ToString();
            string Total_Customer = ChassisInvoiceTable.GroupBy(n => n.CUSTOMER_NAME).Count().ToString();
            string Heard_top = "<div><table  border=\"1\">" +
            "<th> รายปี </th>" +
            "<th> 2559  </th>" +
            "<th></th>" +
            "<th></th>" +
            "<th>2560</th>" +
            "<th></th>" +
            "<th></th>" +
            "<th>2561</th>" +
            "<th></th>" +
            "<th></th>" +
            "<th>2562</th>" +
            "<th></th>" +
              "<th></th>" +
                "<th></th>" +
                  "<th></th>" +
                    "<th></th>" +
                      "<th></th>" +
                        "<th></th>" +
                          "<th></th>" +
                            "<th></th>" +
                             "<th></th>" +
                        "<th></th>" +
                          "<th></th>" +
                            "<th></th>" +
            "<th></th></table></div>";


            string Heard_top2 = "<div><table border=\"1\" >" +
                        "<th>InvoiceNo</th>" +
                        "<th>Date</th>" +
                        "<th>Price</th>" +
                        "<th>InvoiceNo</th>" +
                        "<th>Date</th>" +
                        "<th>Price</th>" +
                        "<th>InvoiceNo</th>" +
                        "<th>Date</th>" +
                        "<th>Price</th>" +
                        "<th>InvoiceNo</th>" +
                        "<th>Date</th>" +
                        "<th>Price</th>" +
                         "<th>INVOICE_CUSTOMER</th>" +
                         "<th>INVOICE_CUSTOMER_PHONE</th>" +
                          "<th>INSTALL_DATE	</th>" +
                        "<th>ID_LICENSE	</th>" +
                        "<th>ID_CHASSIS</th>" +
                        "<th>	MEI	</th>" +
                        "<th> INVOICE_COUNT</th>" +
                        "<th> CUSTOMER_NAME</th>" +
                        "<th> CUSTOMER_PHONE</th>" +
                        "<th> MEI_CERTIF	</th>" +
                        "<th> LICENSE_CERTIF	</th>" +
                        "<th> DEALER_CONTACT_NAME	</th>" +
                        "<th> DEALER_CONTACT_PHONE	</th>" +

                        "</table></div>";


            string Heard_ = "<div><table  border=\"1\">" +
             "<th>Dealer: " + Total_Dealer + " จำนวน</th>" +
              "<th>Customer: " + Total_Customer + " จำนวน</th>" +
             "</table></div>";
            var grid = new GridView();
            grid.ShowHeader = false;
            grid.DataSource = Export_TB;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            string D = DateTime.Now.Date.ToString();
            string Mon = DateTime.Now.Month.ToString();
            string M = DateTime.Now.Millisecond.ToString();
            Response.AddHeader("content-disposition", "attachment; filename=" + D + "" + Mon + "" + M + "Output.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(Heard_ + Heard_top + Heard_top2 + sw.ToString().Replace("<td>&nbsp;</td><td>&#39;", "<td>"));
  
            Response.Flush();
            Response.End();

            return RedirectToAction("Index", "ExportData");
            //return Redirect("/Users/index");


        }
        public ActionResult RespondToDetailEdit(string id_detail, string inv_no, string inv_no_date)
        {
    
            Console.WriteLine("test");
            var DLTDetailChassis = from t1 in db.DLTDetailChassis
                                   where t1.ID_CHASSIS == id_detail
                                   select t1;
            DLTDetailChassi tbDLTDetailChassi = db.DLTDetailChassis.Where(s => s.ID_CHASSIS == id_detail).FirstOrDefault();
            UpdateModel(tbDLTDetailChassi.INVOICE_FLAG = inv_no);
            db.SaveChanges();
            UpdateModel(tbDLTDetailChassi.UP_DATE_INVOICE_FLAG = inv_no_date);
            db.SaveChanges();
            return Json(new { foo = "" + id_detail });
        }

        public ActionResult RespondToDetailSave(string id_detail, string flag, string title, int user_detail, string textremark, string up_date_invoice, string invoice_no)
        {
               
            if (invoice_no.Length < 10)
            {
                invoice_no = "";
            }
            if (id_detail != null)
            {

                var DLTDetailChassis = from t1 in db.DLTDetailChassis
                                       where t1.ID_CHASSIS == id_detail
                                       select t1;

              
                if (DLTDetailChassis.Count() < 1)
                {

                    if (invoice_no.Length > 5)
                    {
                      
                            string iDate = up_date_invoice;
                            DateTime oDate = Convert.ToDateTime(iDate);
                        
                        DateTime datetime_inv = Convert.ToDateTime(up_date_invoice);

                        int login_ID = Convert.ToInt32(this.Session["session_ID"].ToString());

                    
                        db.DLTDetailChassis.Add(new DLTDetailChassi
                        {
                            ID_CHASSIS = id_detail,
                            REMARK = textremark,
                            ID_USERS = login_ID,
                            TITLE_DETAIL = title,
                            STATUS_FLAG = flag,
                            INVOICE_FLAG = invoice_no = invoice_no + ",",
                            UP_DATE_INVOICE_FLAG = invoice_no + "(" + oDate.ToString("yyyy/MM/dd") + ")",
                            UP_DATE = DateTime.Now,
                            UP_DATE_PRICE_FLAG = invoice_no + "(0),"
                        });
                        db.SaveChanges();
                    }
                    if (invoice_no.Length < 5)
                    {


                        db.DLTDetailChassis.Add(new DLTDetailChassi
                        {
                            ID_CHASSIS = id_detail,
                            UP_DATE = DateTime.Now,
                            STATUS_FLAG = flag,
                            TITLE_DETAIL = title,
                            REMARK = textremark,
                            INVOICE_FLAG = "0",

                        });
                        db.SaveChanges();

                    }

                }
                else
                {

                    if (invoice_no.Length > 5)
                    {
                        string iDate = up_date_invoice;
                        DateTime oDate = Convert.ToDateTime(iDate);
                        DateTime datetime_inv = Convert.ToDateTime(up_date_invoice);


                     
                        DLTDetailChassi tbDLTDetailChassi = db.DLTDetailChassis.Where(s => s.ID_CHASSIS == id_detail).FirstOrDefault();
                        UpdateModel(tbDLTDetailChassi.STATUS_FLAG = flag);
                        db.SaveChanges();
                        UpdateModel(tbDLTDetailChassi.TITLE_DETAIL = title);
                        db.SaveChanges();
                        UpdateModel(tbDLTDetailChassi.UP_DATE_INVOICE_FLAG = invoice_no == "" ? tbDLTDetailChassi.UP_DATE_INVOICE_FLAG : DLTDetailChassis.FirstOrDefault().UP_DATE_INVOICE_FLAG + "," + invoice_no + "(" + oDate.ToString("yyyy/MM/dd") + "),");

                        db.SaveChanges();
                        UpdateModel(tbDLTDetailChassi.INVOICE_FLAG = invoice_no == "" ? tbDLTDetailChassi.INVOICE_FLAG : DLTDetailChassis.FirstOrDefault().INVOICE_FLAG + invoice_no + ",");
                        db.SaveChanges();
                        UpdateModel(tbDLTDetailChassi.UP_DATE_PRICE_FLAG = DLTDetailChassis.FirstOrDefault().UP_DATE_PRICE_FLAG + invoice_no + "(0),");
                        db.SaveChanges();
                    }


                    if (invoice_no.Length < 5)
                    {
                    
                        DLTDetailChassi tbDLTDetailChassi = db.DLTDetailChassis.Where(s => s.ID_CHASSIS == id_detail).FirstOrDefault();
                        UpdateModel(tbDLTDetailChassi.STATUS_FLAG = flag);
                        db.SaveChanges();
                        UpdateModel(tbDLTDetailChassi.TITLE_DETAIL = title);
                        db.SaveChanges();


                    }

                }
            }
            return Json(new { foo = "" + id_detail });
        }

    }

}