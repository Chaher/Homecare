//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Homecare_Eksame_project.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Customer()
        {
            this.DrivingPlans = new HashSet<DrivingPlan>();
        }
    
        public int customerID { get; set; }
        public string customerFullName { get; set; }
        public string street { get; set; }
        public string postnr { get; set; }
        public string byen { get; set; }
        public string alder { get; set; }
        public string koen { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DrivingPlan> DrivingPlans { get; set; }
    }
}
