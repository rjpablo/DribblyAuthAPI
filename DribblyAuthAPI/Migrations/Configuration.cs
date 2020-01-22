namespace DribblyAuthAPI.Migrations
{
    using DribblyAuthAPI.Enums;
    using DribblyAuthAPI.Models;
    using DribblyAuthAPI.Models.Courts;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<AuthContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AuthContext context)
        {
            context.Clients.AddOrUpdate(BuildClientsList()[0]);
            AddCourts(context);
            base.Seed(context);
        }

        public void AddCourts(AuthContext context)
        {
            context.Courts.AddOrUpdate(
                new CourtModel
                {
                    Name = "Phoenix Sports Center Basketball Court",
                    Id = 1,
                    DateAdded = new DateTime(2020, 1, 2),
                    PrimaryPhotoUrl = "./images/test_images/courts/court_1.jpg",
                    RatePerHour = 2500,
                    Rating = 0.6M
                },
                new CourtModel
                {
                    Name = "Sports world Basketball Court",
                    Id = 2,
                    DateAdded = new DateTime(2020, 1, 5),
                    PrimaryPhotoUrl = "./images/test_images/courts/court_2.jpg",
                    RatePerHour = 250,
                    Rating = 0.8M
                },
                new CourtModel
                {
                    Name = "Balls World Basketball Court",
                    Id = 3,
                    DateAdded = new DateTime(2020, 1, 5),
                    PrimaryPhotoUrl = "./images/test_images/courts/court_3.jpg",
                    RatePerHour = 450,
                    Rating = 0.3M
                },
                new CourtModel
                {
                    Name = "Brgy Pag-asa Basketball Court",
                    Id = 4,
                    DateAdded = new DateTime(2020, 1, 5),
                    PrimaryPhotoUrl = "./images/test_images/courts/court_4.jpg",
                    RatePerHour = 300,
                    Rating = 0.5M
                },
                new CourtModel
                {
                    Name = "MOA Arena Basketball Court",
                    Id = 5,
                    DateAdded = new DateTime(2020, 1, 5),
                    PrimaryPhotoUrl = "./images/test_images/courts/court_5.jpg",
                    RatePerHour = 250,
                    Rating = 0.6M
                });
        }

        private static List<Client> BuildClientsList()
        {

            List<Client> clients = new List<Client>
            {
                new Client
                { Id = "dribbly-web",
                    Secret= Helper.GetHash("abc@123"),
                    Name="Dribbly Web Client",
                    ApplicationType =  ApplicationTypesEnum.JavaScript,
                    Active = true,
                    RefreshTokenLifeTime = 7200,
                    AllowedOrigin = "*"
                }
            };

            return clients;
        }

    }
}
