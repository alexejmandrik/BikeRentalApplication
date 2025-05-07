using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRentalApplication.Model
{
    public static class SessionManager
    {
        public static User? CurrentUser { get; set; }
    }

}
