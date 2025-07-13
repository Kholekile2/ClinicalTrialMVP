namespace ClinicalTrial2._0.Utilities
{
    /// <summary>
    /// Utility class for extracting numeric values from text strings.
    /// Used for parsing trial phases like "Phase 2" -> 2
    /// </summary>
    public static class ExtractNumber
    {
        /// <summary>
        /// Extracts the first numeric value found in a text string.
        /// </summary>
        /// <param name="text">The text to search for numeric values</param>
        /// <returns>The first numeric value found, or -1 if no valid number is found</returns>
        public static int GetNumber(string text)
        {
            if (string.IsNullOrEmpty(text))
                return -1;

            bool digitFound = false;
            int index = 0;
            int result = 0;

            // Find the first digit in the string
            while (index < text.Length && !digitFound)
            {
                char character = text[index];
                digitFound = char.IsDigit(character);

                if (!digitFound)
                    index++;
            }

            if (!digitFound)
                return -1;

            // Extract the numeric portion starting from the first digit
            var numberPortion = text.Substring(index);
            
            // Try to parse the number, stopping at the first non-digit character
            var digits = string.Empty;
            foreach (char c in numberPortion)
            {
                if (char.IsDigit(c))
                    digits += c;
                else
                    break;
            }

            if (int.TryParse(digits, out result))
                return result;
            else 
                return -1;
        }
    }
}
