using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using CSCLite.Models;
using Newtonsoft.Json.Linq;
using PagedList;

namespace CSCLite.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        private CONN_CSCLITE db = new CONN_CSCLITE();
        public ActionResult Index()
        {
            System.Diagnostics.Debug.WriteLine("xDDDDDDDDDDDDDDDDDDDDDDDDDDxooooxx");
            return View();
        }
        [HttpPost]
        public ActionResult login(string users , string password)
        {
          /*  string myString = "863835021277491ddd,863835021282889,";
            Regex charsToDestroy = new Regex(@"[^\d|\,\-]");
            string moneyString = charsToDestroy.Replace(myString, "");

            System.Diagnostics.Debug.WriteLine(moneyString + "ssssssssss");*/
            try
            {
                var tbDLTUsers = db.DLTUsers.Where(i => i.USERS == users & i.PASSWORD == password);
                var tbDLTUsersID = db.DLTUsers.Where(i => i.USERS == users & i.PASSWORD == password).FirstOrDefault().ID;

                var tbDLTUsersName = db.DLTUsers.Where(i => i.USERS == users & i.PASSWORD == password).FirstOrDefault().NAME;
                System.Diagnostics.Debug.WriteLine(tbDLTUsers.Count() + "eeee");
                if (tbDLTUsers.Count() == 1)
                {
                    Session["session_users_password"] = tbDLTUsersID;
                    Session["session_id"] = tbDLTUsersID;
                    Session["session_users"] = tbDLTUsersName;
                    return Redirect("../Dashboard");
                }
            }
            catch { }
            
            return Redirect("index");
        }
        public ActionResult Signout(string users, string password)
        {
            Session["session_users_password"] ="";
            Session.Clear();
            return Redirect("index");
        }
      
        public ActionResult Create(string name_user, string email_user, string password_user, string password_retype_user)
        {
            System.Diagnostics.Debug.WriteLine(name_user + "_email_" + email_user + "_ggg_" + password_user + "d_" + password_retype_user);
            if (password_user == password_retype_user) {
                if (name_user != null && email_user != null && password_user != null && password_retype_user != null) {
                    db.DLTUsers.Add(new DLTUser
                    {
                        USERS = email_user,
                        NAME = name_user,
                        PASSWORD = password_user
                    });
                    db.SaveChanges();
                   
                }
            }
           
            
            return View();
        }
        public ActionResult Createinser(string name_user, string email_user, string password_user, string password_retype_user)
        {
            System.Diagnostics.Debug.WriteLine(name_user + "_email_" + email_user + "_ggg_" + password_user + "d_" + password_retype_user);
           


            return View();
        }


    }
}