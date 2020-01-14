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
                    City = "Mandaluyong City",
                    Id = 1,
                    DateAdded = new DateTime(2020, 1, 2),
                    PrimaryPhotoUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTt94BvQ4bMGZ2lV0niViDfaE-VhgDe79_7z5Xz8i4oY0VBfW9jpw&s",
                    RatePerHour = 2500,
                    Rating = 0.6M
                },
                new CourtModel
                {
                    Name = "Sports world Basketball Court",
                    City = "Makati City",
                    Id = 2,
                    DateAdded = new DateTime(2020, 1, 5),
                    PrimaryPhotoUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQMxs6CAw2EWIe4-DuZf0y5CKWRlbokJEcb-o0C2Wt68zyzDP5C&s",
                    RatePerHour = 250,
                    Rating = 0.8M
                },
                new CourtModel
                {
                    Name = "Balls World Basketball Court",
                    City = "San Juan City",
                    Id = 3,
                    DateAdded = new DateTime(2020, 1, 5),
                    PrimaryPhotoUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSXd5jc-QAAcaI9ep-qICHghaR16EivD8LFzQbH1td7pFXqDy6R&s",
                    RatePerHour = 450,
                    Rating = 0.3M
                },
                new CourtModel
                {
                    Name = "Brgy Pag-asa Basketball Court",
                    City = "San Juan City",
                    Id = 4,
                    DateAdded = new DateTime(2020, 1, 5),
                    PrimaryPhotoUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQqwRPjxCjGIZK7l3Y6A9k7e0327hcI2Ds6ElWvJbJxDY_nu9whsQ&s",
                    RatePerHour = 300,
                    Rating = 0.5M
                },
                new CourtModel
                {
                    Name = "MOA Arena Basketball Court",
                    City = "Pasay City",
                    Id = 5,
                    DateAdded = new DateTime(2020, 1, 5),
                    PrimaryPhotoUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQPwCRmDgp9P_dAC2Kr3KG2EqaGMNAx0iyKPFKgZr04Haqa7H-76Q&s",
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
