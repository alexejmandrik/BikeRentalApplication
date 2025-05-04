using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BikeRentalApplication.Model.Data;
using Microsoft.AspNetCore.Identity;

namespace BikeRentalApplication.Model
{ 
    class DataWorker
    {
        public static bool CreateUser(string userName, string name, string surname, string patronymic, string phoneNumber, string password, string userStatus)
        {
            bool result = false;
            using (ApplicationContext db = new ApplicationContext())
            {
                bool checkIsExist = db.Users.Any(el => el.UserName == userName);
                if(!checkIsExist)
                {
                    User newUser = new User {
                        UserName = userName,
                        Name = name,
                        Surname = surname,
                        Patronymic = patronymic,
                        PhoneNumber = phoneNumber,
                        Password = PasswordService.HashPassword(password),
                        UserStatus = userStatus,
                        IsBlocked = false
                    };
                    
                    db.Users.Add(newUser);
                    db.SaveChanges();
                    result = true;
                }
            }
            return result;
        }

        public static bool DeleteUser(User user)
        {
            bool result = false;
            using (ApplicationContext db = new ApplicationContext())
            {
                bool checkIsExist = db.Users.Any(el => el.UserName == user.UserName);
                if(checkIsExist)
                {
                    db.Users.Remove(user);
                    db.SaveChanges();
                    result = true;
                }
            }
            return result;
        }

        public static bool EditUser(User oldUser, string NewUserName, string NewName, string NewSurname, string NewPatronymic, string NewPhoneNumber, string NewPassword, string NewUserStatus)
        {
            bool result = false;
            using (ApplicationContext db = new ApplicationContext())
            {
                
                User User = db.Users.FirstOrDefault(u => u.Id == oldUser.Id);
                if (User != null)
                {
                    User.UserName = NewUserName;
                    User.Name = NewName;
                    User.Surname = NewSurname;
                    User.Patronymic = NewPatronymic;
                    User.PhoneNumber = NewPhoneNumber;
                    User.Password = NewPassword;
                    User.UserStatus = NewUserStatus;
                    db.SaveChanges();
                    result = true;
                }
            }
            return result;
        }
        public static bool ChangeIsBlockedUser(User oldUser)
        {
            bool result = false;
            using (ApplicationContext db = new ApplicationContext())
            {

                User User = db.Users.FirstOrDefault(u => u.Id == oldUser.Id);
                if (User != null)
                {
                    User.IsBlocked = !User.IsBlocked;
                    db.SaveChanges();
                    result = true;
                }
            }
            return result;
        }

        public static List<User> GetAllUsers()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var result = db.Users.ToList();
                return result;
            }
        }

        public static bool SearchUserByUserName(string username)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                bool result = false;
                User user = db.Users.FirstOrDefault(u => u.UserName == username);
                if (user != null)
                    result = true;
                return result;
            }
        }

        public static bool CheckBlocking(string username)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                bool result = false;
                User user = db.Users.FirstOrDefault(u => u.UserName == username);
                if (user.IsBlocked)
                    result = true;
                return result;
            }
        }

        public static string GetUserRole(string username)
        {
            using(ApplicationContext db = new ApplicationContext())
            {
                string result = "user";
                User User = db.Users.FirstOrDefault(u => u.UserName == username);
                if (User.UserStatus == "admin")
                    result = "admin";
                return result;
            }
        }

        public static bool AuthenticateUser(string userName, string password)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var user = db.Users.FirstOrDefault(u => u.UserName == userName);
                if (user == null) return false;

                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(user, user.Password, password);
                return result == PasswordVerificationResult.Success;
            }
        }
        public static bool CreateBike(string name, string description, string imagePath, decimal price)
        {
            bool result = false;
            using (ApplicationContext db = new ApplicationContext())
            {
                bool checkIsExist = db.Bikes.Any(el => el.Name == name);
                if (!checkIsExist)
                {
                    Bike newBike = new Bike()
                    {
                        Name = name,
                        Description = description,
                        ImagePath = imagePath,
                        Price = price
                    };
                    db.Bikes.Add(newBike);
                    db.SaveChanges();
                    result = true;
                }
            }
            return result;
        }
        public static bool DeleteBike(Bike bike)
        {
            bool result = false;
            using(ApplicationContext db = new ApplicationContext())
            {
                bool checkIsExist = db.Bikes.Any(id => id.Name == bike.Name);
                if(checkIsExist)
                {
                    db.Bikes.Remove(bike);
                    db.SaveChanges();
                    result = true;
                }
                return result;
            }

        }

        public static bool EditBike(Bike oldBike, string newName, string newDescription, string newImagePath, decimal newPrice)
        {
            bool result = false;
            using (ApplicationContext db = new ApplicationContext())
            {
                Bike Bike = db.Bikes.FirstOrDefault(el => el.Id == oldBike.Id);
                if(Bike != null)
                {
                    Bike.Name = newName;
                    Bike.Description = newDescription;
                    Bike.ImagePath = newImagePath;
                    Bike.Price = newPrice;
                    db.SaveChanges();
                    result = true;
                }
            }
            return result;
        }
        public static List<Bike> GetAllBikes()
        {
            using(ApplicationContext db = new ApplicationContext())
            {
                var result = db.Bikes.ToList();
                return result;
            }
        }
    }
}
