namespace System.Int
{
    public static class IntExtensions
    {
        public static bool IsWithin(this int value, int minimum, int maximum)
        {
            return value >= minimum && value <= maximum;
        } 
    }
}