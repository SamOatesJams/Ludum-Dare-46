using System.Collections.Generic;
using System.Text;

namespace SamOatesGames.Systems
{
    public static class CSVReader
    {
        /// <summary>
        /// Parse a .CSV file
        /// </summary>
        /// <param name="contents">The contents of the file</param>
        /// <returns>an array of all the records in the file</returns>
        public static string[][] ParseFile(string contents)
        {
            var lines = contents.Split('\n');

            if (lines.Length <= 1)
            {
                return new string[0][];
            }

            var result = new string[lines.Length][];
            var count = 0;

            var inQuotes = false;
            var escaped = false;
            var record = new StringBuilder();

            foreach (var line in lines)
            {
                var parsedLine = new List<string>();

                foreach (var c in line)
                {
                    if (!escaped)
                    {
                        if (c.Equals('\"'))
                        {
                            inQuotes = !inQuotes;
                            continue;
                        }

                        if (c.Equals('\\'))
                        {
                            escaped = true;
                            continue;
                        }

                        if (!inQuotes && c.Equals(','))
                        {
                            parsedLine.Add(record.ToString());
                            record = new StringBuilder();
                            continue;
                        }
                    }

                    record.Append(c);
                    escaped = false;
                }

                parsedLine.Add(record.ToString());
                record = new StringBuilder();

                escaped = false;
                inQuotes = false;
                result[count++] = parsedLine.ToArray();
            }

            return result;
        }
    }
}
