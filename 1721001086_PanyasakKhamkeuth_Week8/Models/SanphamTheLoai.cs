using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _1721001086_PanyasakKhamkeuth_Week8.Models
{
	public class SanphamTheLoai
	{
		[Key]
		[Column(Order = 1)]
		public int SanphamId { get; set; }

		[Key]
		[Column(Order = 2)]
		public int TheloaiId { get; set; }

		[ForeignKey("SanphamId")]
		[ValidateNever]
		public SanPham Sanpham { get; set; }

		[ForeignKey("TheloaiId")]
		[ValidateNever]
		public TheLoai Theloai { get; set; }
	}
}
