﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRentalApplication.Model
{
    public class User
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Patronymic { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
        public required string UserStatus { get; set; }
        public bool IsBlocked { get; set; }
        public int BonusCounter { get; set; }
    }
}
