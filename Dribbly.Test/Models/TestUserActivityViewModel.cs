using Dribbly.Model.UserActivities;
using Dribbly.Service.Enums;
using System;

namespace Dribbly.Test.Model
{
    public class TestUserActivityViewModel
    {
        public long UserId { get; set; }
        public string ActivityTypeName { get; set; }

        public TestUserActivityViewModel()
        {

        }

    }

    public static class UserActivityExtension
    {
        public static TestUserActivityViewModel ToTestViewModel(this UserActivityModel a)
        {
            return new TestUserActivityViewModel
            {
                UserId = a.UserId,
                ActivityTypeName = Enum.GetName(typeof(UserActivityTypeEnum), a.Type)
            };
        }
    }
}
