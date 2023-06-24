using Dribbly.Model.Enums;
using Dribbly.Model.Fouls;

namespace Dribbly.Model
{
    public static partial class Constants
    {
        public static FoulModel[] Fouls = new FoulModel[]
        {
            // Defensive
            new FoulModel(1, "Reaching In", FoulTypeEnum.Personal, false, true),
            new FoulModel(2, "Blocking", FoulTypeEnum.Personal, false, true),
            new FoulModel(3, "Shooting", FoulTypeEnum.Personal, false, true),
            // Offensive
            new FoulModel(4, "Charging", FoulTypeEnum.Personal, true, false),
            new FoulModel(5, "Illegal Screen", FoulTypeEnum.Personal, true, false),
            // Offensive and Defensive
            new FoulModel(6, "Holding", FoulTypeEnum.Personal, true, true),
            new FoulModel(7, "Elbowing", FoulTypeEnum.Personal, true, true),
            new FoulModel(8, "Loose Ball", FoulTypeEnum.Personal, true, true),
            new FoulModel(9, "Tripping", FoulTypeEnum.Personal, true, true),
            new FoulModel(10, "Hand Check", FoulTypeEnum.Personal, true, true),
            new FoulModel(11, "Over the back", FoulTypeEnum.Personal, true, true),
            // Technical
            new FoulModel(12,"Unsportsmanlike Conduct", FoulTypeEnum.Personal, true, true, true),
            new FoulModel(13,"Delay of Game", FoulTypeEnum.Personal, true, true, true),
            new FoulModel(14,"Excessive Timeouts", FoulTypeEnum.Personal, true, true, true),
            new FoulModel(15,"Fighting Foul", FoulTypeEnum.Personal, true, true, true),
            new FoulModel(16,"Number of Players", FoulTypeEnum.Personal, true, true, true),
            new FoulModel(17,"Basket Ring, Backboard, or Support", FoulTypeEnum.Personal, true, true, true),
            //Flagrant
            new FoulModel(18,"Flagrant 1", FoulTypeEnum.Personal, true, true),
            new FoulModel(19,"Flagrant 2", FoulTypeEnum.Personal, true, true)
        };
    }
}
