﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CONN_CSCLITE : DbContext
    {
        public CONN_CSCLITE()
            : base("name=CONN_CSCLITE")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<DLTDetailChassi> DLTDetailChassis { get; set; }
        public virtual DbSet<DLTChassisImport> DLTChassisImports { get; set; }
        public virtual DbSet<DLTChassisInvoice> DLTChassisInvoices { get; set; }
        public virtual DbSet<DLTChassisInvoiceImport> DLTChassisInvoiceImports { get; set; }
        public virtual DbSet<DLTInvoice> DLTInvoices { get; set; }
        public virtual DbSet<DLTLicenseImport> DLTLicenseImports { get; set; }
        public virtual DbSet<DLTLicenseInvoiceImport> DLTLicenseInvoiceImports { get; set; }
        public virtual DbSet<DLTMeiImport> DLTMeiImports { get; set; }
        public virtual DbSet<DLTMeiInvoiceImport> DLTMeiInvoiceImports { get; set; }
        public virtual DbSet<DLTMeiInvoiceImportGroup> DLTMeiInvoiceImportGroups { get; set; }
        public virtual DbSet<DLTUser> DLTUsers { get; set; }
        public virtual DbSet<TABLE_REPORT_CUSTOMER> TABLE_REPORT_CUSTOMER { get; set; }
        public virtual DbSet<DLTChassisInvoice_Mei> DLTChassisInvoice_Mei { get; set; }
    }
}
