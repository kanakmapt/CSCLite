//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CSCLite.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DLTInvoice
    {
        public int ID { get; set; }
        public string InvoiceNO { get; set; }
        public string InvoiceOpenDate { get; set; }
        public string InvoiceDueDate { get; set; }
        public string InvoiceCustomerCode { get; set; }
        public string InvoiceCustomerName { get; set; }
        public string InvoiceContactName { get; set; }
        public string InvoiceContactAddress { get; set; }
        public string InvoiceReference { get; set; }
        public string InvoicePeriod { get; set; }
        public string InvoiceMonth { get; set; }
        public string InvoiceYear { get; set; }
        public string InvoiceAmountBeforeVat { get; set; }
        public string InvoiceVat { get; set; }
        public string InvoiceTotal { get; set; }
        public string InvoiceTotalPayment { get; set; }
        public string InvoiceTotalRemain { get; set; }
        public string InvoiceTotalCreditNote { get; set; }
        public string InvoiceItemDescription { get; set; }
        public string InvoiceItemBillQuantity { get; set; }
        public string InvoiceItemBillUnitPrice { get; set; }
        public string InvoiceItemBillAmount { get; set; }
        public string InvoiceType { get; set; }
        public string InvoiceProcess { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerContactName { get; set; }
        public string CustomerContactPhone { get; set; }
        public string CustomerContactEmail { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectOpenDate { get; set; }
        public string ProjectContractNO { get; set; }
        public string ProjectContractDate { get; set; }
        public string ProjectContractExpireDate { get; set; }
        public string ProjectQuotationNO { get; set; }
        public string ProjectQuotationDate { get; set; }
        public string ProjectPurchaseOrderNO { get; set; }
        public string ProjectPurchaseOrderDate { get; set; }
        public string ProjectPromotion { get; set; }
        public string ProjectGuarantee { get; set; }
        public string ProjectRemark { get; set; }
        public string ProjectType { get; set; }
        public string ProjectGroup { get; set; }
        public string ProjectSellType { get; set; }
        public string ProjectServiceType { get; set; }
        public string ProjectPrice { get; set; }
        public string ProjectVat { get; set; }
        public string ProjectNetPrice { get; set; }
        public string ProjectTotalQuantity { get; set; }
        public string ProjectPickupQuantity { get; set; }
        public string ProjectRemainQuantity { get; set; }
    }
}
