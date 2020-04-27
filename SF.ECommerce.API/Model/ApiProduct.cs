using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.ECommerce.API.Model
{
    public class ApiProduct
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

        public int Availability { get; set; }
    }
}
