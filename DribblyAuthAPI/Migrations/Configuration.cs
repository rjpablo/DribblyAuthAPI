namespace DribblyAuthAPI.Migrations
{
    using DribblyAuthAPI.Enums;
    using DribblyAuthAPI.Models;
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
            base.Seed(context);
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
