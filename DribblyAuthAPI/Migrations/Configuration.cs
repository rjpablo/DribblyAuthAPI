namespace DribblyAuthAPI.Migrations
{
    using DribblyAuthAPI.Enums;
    using DribblyAuthAPI.Models;
    using DribblyAuthAPI.Models.Courts;
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
            //AddCourts();
            SeedSettings();

            _context.SaveChanges();
            base.Seed(_context);
        }

        void SeedSettings()
        {
            List<SettingModel> settings = new List<SettingModel>()
            {
                new SettingModel(0, "googleMapApiKey", "Google Map Api Key", "AIzaSyCQwPkj7HcSjORBr6z8ZGf56e4uXNPHUuY")
            };

            foreach(SettingModel setting in settings)
            {
                var s = _context.Settings.FirstOrDefault(x => x.Key == setting.Key);
                if(s != null)
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

        public void AddCourts()
        {
            _context.Courts.AddOrUpdate(
                new CourtModel
                {
                    Name = "Phoenix Sports Center Basketball Court",
                    Id = 1,
                    DateAdded = new DateTime(2020, 1, 2),
                    PrimaryPhotoUrl = "./dest/images/test_images/courts/court_1.jpg",
                    RatePerHour = 2500,
                    Rating = 0.6M
                },
                new CourtModel
                {
                    Name = "Sports world Basketball Court",
                    Id = 2,
                    DateAdded = new DateTime(2020, 1, 5),
                    PrimaryPhotoUrl = "./dest/images/test_images/courts/court_2.jpg",
                    RatePerHour = 250,
                    Rating = 0.8M
                },
                new CourtModel
                {
                    Name = "Balls World Basketball Court",
                    Id = 3,
                    DateAdded = new DateTime(2020, 1, 5),
                    PrimaryPhotoUrl = "./dest/images/test_images/courts/court_3.jpg",
                    RatePerHour = 450,
                    Rating = 0.3M
                },
                new CourtModel
                {
                    Name = "Brgy Pag-asa Basketball Court",
                    Id = 4,
                    DateAdded = new DateTime(2020, 1, 5),
                    PrimaryPhotoUrl = "./dest/images/test_images/courts/court_4.jpg",
                    RatePerHour = 300,
                    Rating = 0.5M
                },
                new CourtModel
                {
                    Name = "MOA Arena Basketball Court",
                    Id = 5,
                    DateAdded = new DateTime(2020, 1, 5),
                    PrimaryPhotoUrl = "./dest/images/test_images/courts/court_5.jpg",
                    RatePerHour = 250,
                    Rating = 0.6M
                });
        }

        void SeedClients()
        {
            _context.Clients.AddOrUpdate(new Client
            {
                Id = "dribbly-web",
                Secret = Helper.GetHash("abc@123"),
                Name = "Dribbly Web Client",
                ApplicationType = ApplicationTypesEnum.JavaScript,
                Active = true,
                RefreshTokenLifeTime = 7200,
                AllowedOrigin = "*"
            });
        }

    }
}
