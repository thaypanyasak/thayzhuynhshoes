using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _1721001086_PanyasakKhamkeuth_Week8.Models
{
	public class SanPham
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public int Price { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        [Range(0, 5)] // Ensure the rating is between 0 and 5
        public double Rating { get; set; }
        [Required(AllowEmptyStrings = true)]
        public string VideoUrl {  get; set; }

        public int NhaCungCapId { get; set; }

        [ForeignKey("NhaCungCapId")]
        [ValidateNever]
        public NhaCungCap NhaCungCap { get; set; }


        public ICollection<SanphamTheLoai> Theloais { get; set; }
        public List<ProductImage> Images { get; set; }

        


    }
}
