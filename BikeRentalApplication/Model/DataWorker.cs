using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using BikeRentalApplication.Model.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharpVectors.Dom;

namespace BikeRentalApplication.Model
{
    public class DataWorker
    {
        #region USER DB
        public static async Task<bool> CreateUserAsync(string userName, string name, string surname, string patronymic, string phoneNumber, string password, string userStatus)
        {
            bool result = false;

            using (var db = new ApplicationContext())
            {
                using (var transaction = await db.Database.BeginTransactionAsync())
                {
                    try
                    {
                        bool checkIsExist = await db.Users.AnyAsync(el => el.UserName == userName);
                        if (!checkIsExist)
                        {
                            User newUser = new User
                            {
                                UserName = userName,
                                Name = name,
                                Surname = surname,
                                Patronymic = patronymic,
                                PhoneNumber = phoneNumber,
                                Password = PasswordService.HashPassword(password),
                                UserStatus = userStatus,
                                IsBlocked = false,
                                BonusCounter = 0
                            };

                            await db.Users.AddAsync(newUser);
                            await db.SaveChangesAsync();

                            await transaction.CommitAsync();
                            result = true;
                        }
                    }
                    catch
                    {
                        await transaction.RollbackAsync(); 
                        throw;
                    }
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
                if (checkIsExist)
                {
                    db.Users.Remove(user);
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
                var result = db.Users.Where(u => u.UserStatus != "admin").ToList();
                return result;
            }
        }

        public static User? GetUserById(int id)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return db.Users.FirstOrDefault(el => el.Id == id);
            }
        }

        public static User? GetUserByUsername(string userName)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return db.Users.FirstOrDefault(u => u.UserName == userName);
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
            using (ApplicationContext db = new ApplicationContext())
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




        #endregion

        #region BIKE DB
        public static bool CreateBike(string name, string description, string fullDescription, string imagePath, decimal price)
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
                        FullDescription = fullDescription,
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
            using (ApplicationContext db = new ApplicationContext())
            {
                bool checkIsExist = db.Bikes.Any(id => id.Name == bike.Name);
                if (checkIsExist)
                {
                    db.Bikes.Remove(bike);
                    db.SaveChanges();
                    result = true;
                }
                return result;
            }

        }
        public static bool EditBike(Bike oldBike, string newName, string newDescription, string newFullDescription, string newImagePath, decimal newPrice)
        {
            bool result = false;
            using (ApplicationContext db = new ApplicationContext())
            {
                Bike Bike = db.Bikes.FirstOrDefault(el => el.Id == oldBike.Id);
                if (Bike != null)
                {
                    Bike.Name = newName;
                    Bike.Description = newDescription;
                    Bike.FullDescription = newFullDescription;
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
            using (ApplicationContext db = new ApplicationContext())
            {
                var result = db.Bikes.ToList();
                return result;
            }
        }

        public static Bike? GetBikeById(int id)
        {
            using(ApplicationContext db = new ApplicationContext())
            {
                return db.Bikes.FirstOrDefault(el => el.Id == id);
            }
        }

        #endregion

        #region BUKING DB
        public static string CreateBikeBooking(int userId, int bikeId, DateTime startDateTime, DateTime endDateTime, string? comment, string status, decimal price, bool isPaid)
        {
            string result = "Данное время уже занято!";
            if (startDateTime <= DateTime.Now)
                return "Вы не можете выбрать время раньше актуального сейчас!";

            using (ApplicationContext db = new ApplicationContext())
            {
                bool isOverlap = db.BikeBookings.Any(b =>
                    b.BikeId == bikeId &&
                    ((startDateTime >= b.StartDateTime && startDateTime < b.EndDateTime) ||
                     (endDateTime > b.StartDateTime && endDateTime <= b.EndDateTime) ||
                     (startDateTime <= b.StartDateTime && endDateTime >= b.EndDateTime)));

                if (!isOverlap)
                {
                    BikeBooking newBooking = new BikeBooking
                    {
                        UserId = userId,
                        BikeId = bikeId,
                        StartDateTime = startDateTime,
                        EndDateTime = endDateTime,
                        Comment = comment,
                        BookingStatus = status,
                        Price = price,
                        IsPaid = isPaid
                    };

                    db.BikeBookings.Add(newBooking);
                    db.SaveChanges();
                    result = "Успешно забронировано!";
                }
            }
            return result;
        }


        public static bool DeleteBikeBooking(BikeBooking bikeBooking)
        {
            bool result = false;
            using (ApplicationContext db = new ApplicationContext())
            {
                bool checkIsExist = db.BikeBookings.Any(id => id.Id== bikeBooking.Id);
                if (checkIsExist)
                {
                    db.BikeBookings.Remove(bikeBooking);
                    db.SaveChanges();
                    result = true;
                }
                return result;
            }
        }

        public static List<BikeBooking> GetUserBookings(User user)
        {
            if (user == null)
            {
                return new List<BikeBooking>();
            }

            using (ApplicationContext db = new ApplicationContext())
            {
                return db.BikeBookings
                            .Where(b => b.UserId == user.Id)
                            .Include(b => b.Bike)
                            .OrderByDescending(b => b.StartDateTime)
                            .ToList();
            }
        }

        public static List<BikeBooking> GetAllBookings()
        {
            using(ApplicationContext db = new ApplicationContext())
            {
                return db.BikeBookings.Include(b => b.User).ToList();
            }
        }

        public static void UpdateBookingStatusIfNeeded(BikeBooking booking)
        {
            var now = DateTime.Now;

            string newStatus;

            if (now < booking.StartDateTime)
                newStatus = "Забронировано";
            else if (now >= booking.StartDateTime && now <= booking.EndDateTime)
                newStatus = "Активно";
            else
                newStatus = "Завершено";

            if (booking.BookingStatus != newStatus)
            {
                using (var context = new ApplicationContext())
                {
                    var bookingInDb = context.BikeBookings.FirstOrDefault(b => b.Id == booking.Id);
                    if (bookingInDb != null)
                    {
                        bookingInDb.BookingStatus = newStatus;
                        context.SaveChanges();
                    }
                }
            }
        }

        public static bool SetBunusCounterUp(User currentUser, decimal price)
        {
            if (currentUser == null)
                return false;

            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var userInDb = db.Users.FirstOrDefault(u => u.Id == currentUser.Id);
                    if (userInDb == null)
                        return false;
               
                    userInDb.BonusCounter += (int)price;
                    db.SaveChanges();
                    currentUser.BonusCounter = userInDb.BonusCounter;
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SetBunusCounterDown(BikeBooking booking, User currentUser)
        {
            if (currentUser == null)
                return false;

            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var userInDb = db.Users.FirstOrDefault(u => u.Id == currentUser.Id);
                    var bookingInDb = db.BikeBookings.FirstOrDefault(b => b.Id == booking.Id);
                    if (userInDb == null || bookingInDb == null)
                        return false;
                    if(userInDb.BonusCounter > (int)bookingInDb.Price)
                    {
                        userInDb.BonusCounter -= (int)bookingInDb.Price;
                        db.SaveChanges();
                        currentUser.BonusCounter = userInDb.BonusCounter;
                        return true;
                    }
                    else
                    {
                        userInDb.BonusCounter = 0;
                        db.SaveChanges();
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool SetBonusCounterDownBooking(int price, User user)
        {
            if (price == 0 || user == null)
            {
                MessageBox.Show("3");
                return false;
            }

            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var userInDb = db.Users.FirstOrDefault(u => u.Id == user.Id);
                    if (userInDb == null)
                    {
                        MessageBox.Show("2");
                        return false;
                    }

                    userInDb.BonusCounter = Math.Max(0, userInDb.BonusCounter - price);
                    db.SaveChanges();

                    user.BonusCounter = userInDb.BonusCounter;
                    return true;
                }
            }
            catch
            {
                MessageBox.Show("1");
                return false;
            }
        }

        #endregion

        #region COMMENTS DB
        public static string AddComment(Bike bike, User user, string comment,bool visibility)
        {
            if (bike == null || user == null || string.IsNullOrWhiteSpace(comment))
                return "Ошибка: Недостаточно данных для добавления комментария.";
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var newComment = new Comments
                    {
                        Comment = comment,
                        Bike = db.Bikes.FirstOrDefault(b => b.Id == bike.Id),
                        User = db.Users.FirstOrDefault(u => u.Id == user.Id),
                        Visibility = false
                    };

                    db.Comments.Add(newComment);
                    db.SaveChanges();

                    return "Комментарий успешно добавлен.";
                }
            }
            catch (Exception ex)
            {
                return $"Ошибка при добавлении комментария: {ex.Message}";
            }
        }

        public static bool DeleteComment(Comments comment)
        {
            bool result = false;
            using (ApplicationContext db = new ApplicationContext())
            {
                bool checkIsExist = db.Comments.Any(c => c.Id == comment.Id);
                if (checkIsExist)
                {
                    db.Comments.Remove(comment);
                    db.SaveChanges();
                    result = true;
                }
                return result;
            }
        }


        public static List<Comments> GetCommentsByBikeId(int bikeId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return db.Comments
                  .Where(c => c.Bike.Id == bikeId && c.Visibility) 
                 .Include(c => c.User) 
                 .ToList();
            }
        }
        public static List<Comments> GetAllComments()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return db.Comments
                 .Include(c => c.User)
                 .Include(c => c.Bike)
                 .ToList();
            }
        }

        public static bool ChangeIsVisibilityComment(Comments oldComment)
        {
            bool result = false;
            using (ApplicationContext db = new ApplicationContext())
            {

                Comments comment = db.Comments.FirstOrDefault(c => c.Id == oldComment.Id);
                if (comment != null)
                {
                    comment.Visibility = !comment.Visibility;
                    db.SaveChanges();
                    result = true;
                }
            }
            return result;
        }
        #endregion
    }
}
