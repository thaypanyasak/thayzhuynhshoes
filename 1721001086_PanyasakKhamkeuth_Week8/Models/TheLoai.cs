using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace _1721001086_PanyasakKhamkeuth_Week8.Models
{
	public class TheLoai
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime DateCreated { get; set; }

    }
}
