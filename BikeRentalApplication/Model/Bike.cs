using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRentalApplication.Model
{
    public class Bike
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string ImagePath { get; set; }
        public decimal Price { get; set; }
    }
}
