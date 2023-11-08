using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _1721001086_PanyasakKhamkeuth_Week8.Models
{
    public class FavorProduct
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        [Required]
        public int SanPhamId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        [ForeignKey("SanPhamId")]
        public SanPham SanPham { get; set; }
    }
}
