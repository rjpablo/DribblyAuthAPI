namespace Dribbly.Core.Extensions
{
    public static class IntExtensions
    {
        public static double? DivideBy(this int dividend, int? divisor)
        {
            if (divisor.HasValue && divisor != 0)
            {
                return (double)dividend / divisor;
            }
            return null;
        }
    }
}
