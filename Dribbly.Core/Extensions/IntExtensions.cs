namespace Dribbly.Core.Extensions
{
    public static class IntExtensions
    {
        public static double DivideBy(this int dividend, int divisor)
        {
            return divisor == 0 ? 0 : (double)dividend / divisor;
        }
    }
}
