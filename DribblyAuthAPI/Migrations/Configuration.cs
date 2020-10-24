namespace DribblyAuthAPI.Migrations
{
    using Dribbly.Authentication.Enums;
    using Dribbly.Authentication.Models;
    using Dribbly.Authentication.Models.Auth;
    using Dribbly.Core.Enums.Permissions;
    using Dribbly.Core.Helpers;
    using Dribbly.Model;
    using Dribbly.Model.Courts;
    using Dribbly.Model.Bookings;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<AuthContext>
    {
        AuthContext _context;

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AuthContext context)
        {
            _context = context;
            SeedClients();
            SeedPermissions();
            SeedSettings();
            //SeedBookings();
            //AddCourts();

            _context.SaveChanges();
            base.Seed(_context);
        }

        void SeedPermissions()
        {
            ApplicationUser adminUser = _context.Users.FirstOrDefault(u => u.UserName.Equals("test1",StringComparison.OrdinalIgnoreCase));
            if(adminUser != null)
            {
                List<UserPermissionModel> allPermissions =
                    EnumFunctions.GenerateUserPermissions<CourtPermission>(adminUser.Id)
                    .Union(EnumFunctions.GenerateUserPermissions<BookingPermission>(adminUser.Id))
                    .Union(EnumFunctions.GenerateUserPermissions<AccountPermission>(adminUser.Id))
                    .ToList();
                _context.UserPermissions.AddOrUpdate(allPermissions.ToArray());
            }
        }

        void SeedSettings()
        {
            List<SettingModel> settings = new List<SettingModel>()
            {
                new SettingModel(0, "googleMapApiKey", "Google Map Api Key", "AIzaSyCQwPkj7HcSjORBr6z8ZGf56e4uXNPHUuY"),
                new SettingModel(1, "maxVideoUploadMb", "Maximun size allowed for uploads in MB", "20")
            };

            foreach (SettingModel setting in settings)
            {
                var s = _context.Settings.FirstOrDefault(x => x.Key == setting.Key);
                if (s != null)
                {
                    s.Key = setting.Key;
                    s.Description = setting.Description;
                    s.DefaultValue = setting.DefaultValue;
                }
                else
                {
                    _context.Settings.Add(setting);
                }
            }
        }

        public void SeedBookings()
        {
            _context.Bookings.AddOrUpdate(
                new BookingModel
                {
                    Id = 1,
                    Start = DateTime.Now,
                    End = DateTime.Now.AddHours(3),
                    DateAdded = DateTime.Now,
                    CourtId = 4,
                    Title = "Cardinals Tune Up Booking"
                });
        }

        public void AddCourts()
        {
            _context.Courts.AddOrUpdate(
                new CourtModel
                {
                    Name = "Phoenix Sports Center Basketball Court",
                    Id = 1,
                    DateAdded = new DateTime(2020, 1, 2),
                    RatePerHour = 2500,
                    Rating = 0.6
                },
                new CourtModel
                {
                    Name = "Sports world Basketball Court",
                    Id = 2,
                    DateAdded = new DateTime(2020, 1, 5),
                    RatePerHour = 250,
                    Rating = 0.8
                },
                new CourtModel
                {
                    Name = "Balls World Basketball Court",
                    Id = 3,
                    DateAdded = new DateTime(2020, 1, 5),
                    RatePerHour = 450,
                    Rating = 0.3
                },
                new CourtModel
                {
                    Name = "Brgy Pag-asa Basketball Court",
                    Id = 4,
                    DateAdded = new DateTime(2020, 1, 5),
                    RatePerHour = 300,
                    Rating = 0.5
                },
                new CourtModel
                {
                    Name = "MOA Arena Basketball Court",
                    Id = 5,
                    DateAdded = new DateTime(2020, 1, 5),
                    RatePerHour = 250,
                    Rating = 0.6
                });
        }

        void SeedClients()
        {
            _context.Clients.AddOrUpdate(new Client
            {
                Id = "dribbly-web",
                Secret = Hash.GetHash("abc@123"),
                Name = "Dribbly Web Client",
                ApplicationType = ApplicationTypesEnum.JavaScript,
                Active = true,
                RefreshTokenLifeTime = 20160,
                AllowedOrigin = "*"
            });
        }

    }
}
