﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DBAccess
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DBBTAEntities : DbContext
    {
        public DBBTAEntities()
            : base("name=DBBTAEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Accommodation> Accommodations { get; set; }
        public virtual DbSet<AccTraDesCategory> AccTraDesCategories { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<CTEmployee> CTEmployees { get; set; }
        public virtual DbSet<Destination> Destinations { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<Info> Infoes { get; set; }
        public virtual DbSet<LifeInCity> LifeInCities { get; set; }
        public virtual DbSet<Picture> Pictures { get; set; }
        public virtual DbSet<Provider> Providers { get; set; }
        public virtual DbSet<Transportation> Transportations { get; set; }
        public virtual DbSet<Admin_SMTP_parametere> Admin_SMTP_parameteres { get; set; }
        public virtual DbSet<Employee_Activation> Employee_Activations { get; set; }
    }
}