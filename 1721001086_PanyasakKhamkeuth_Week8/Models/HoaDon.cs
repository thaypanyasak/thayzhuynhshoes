using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _1721001086_PanyasakKhamkeuth_Week8.Models
{
    public class HoaDon
    {
        [Key]
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        public double Total {  get; set; }
        public DateTime OrderDate { get; set; }
        public string? OrderStatus { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string OrderNumber { get; set; }

        public double TaxAmount { get; set; }  // Add TaxAmount property
        public double ShippingFee { get; set; }

    }
}
