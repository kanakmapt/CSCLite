using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCLite.Models
{
    public class Chassis_Status_Model
    {
        public string ID_CHASSIS { get; set; }
        public string INVOICE_COUNT { get; set; }
        public string INVOICE_NO { get; set; }
        public string INVOICE_COUNT_LOSE { get; set; }
        public int YEAR_LOSE { get; set; }
        public string INSTALL_DATE { get; set; }
        public string DEALER_CONTACT_NAME { get; set; }
        public string DEALER_CONTACT_PHONE { get; set; }
        public string ID_LICENSE { get; set; }
        public string ID_MEI { get; set; }

        public string ID_LICENSE_L { get; set; }
        public string ID_MEI_L { get; set; }
        public string INVOICE_NO_DATE { get; set; }

        public string INVOICE_PRICE { get; set; }
        
        public string INSTALL_YEAR { get; set; }
        public string INVOICE_NAME { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string CUSTOMER_PHONE { get; set; }
        public string INVOICE_CUSTOMER { get; set; }
        public string INVOICE_CUSTOMER_PHONE { get; set; }
        public  int ID { get; set; }
        public string CER_LICENSE { get; set; }
        public string STATUS_FLAG { get; set; }
        public DateTime INSTALL_DATE_ { get; set; }
        public Nullable<DateTime> INV_DATE_ { get; set; }
        public string INV_ID { get; set; }
        public string MONTH_SEARCH { get; set; }
        public string DAY_SEARCH { get; set; }

        public int? NOT_FOUND_IME { get; set; }
        public string STATUS_MEI { get; set; }
        public string STATUS_LICENSE { get; set; }

        public int? STATUS_DLT { get; set; }
    }

   
}
