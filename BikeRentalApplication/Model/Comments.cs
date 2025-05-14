using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRentalApplication.Model
{
    public class Comments
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Comment { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }


        [ForeignKey("BikeId")]
        public Bike Bike { get; set; }

        [Required]
        public bool Visibility { get; set; }
    } 
}
