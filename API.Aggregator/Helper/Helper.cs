namespace API.Aggregator.Helpers
{
    /// <summary>
    /// Helper class containing utility methods.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Converts temperature from Kelvin to Celsius (rounded to one decimal place).
        /// </summary>
        /// <param name="kelvin">Temperature in Kelvin.</param>
        /// <returns>Temperature in Celsius (rounded to one decimal place).</returns>
        public static double ConvertKelvinToCelsius(double kelvin)
        {
            return Math.Round(kelvin - 273.15, 1); // Convert Kelvin to Celsius (rounded to one decimal place)
        }
    }
}
