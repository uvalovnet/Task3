using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Task3
{
    public class LogStandardizer
    {
        private readonly string inputFile;
        private readonly string outputFile;
        private readonly string problemsFile;

        public LogStandardizer(string inputFile, string outputFile, string problemsFile)
        {
            this.inputFile = inputFile ?? throw new ArgumentNullException(nameof(inputFile));
            this.outputFile = outputFile ?? throw new ArgumentNullException(nameof(outputFile));
            this.problemsFile = problemsFile ?? throw new ArgumentNullException(nameof(problemsFile));
        }

        public string ProcessLogs()
        {
            try
            {
                using (StreamReader reader = new StreamReader(inputFile))
                using (StreamWriter writer = new StreamWriter(outputFile))
                using (StreamWriter problemsWriter = new StreamWriter(problemsFile))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (TryParseFormat1(line, out var logEntry) || TryParseFormat2(line, out logEntry))
                        {
                            string standardizedLevel = StandardizeLogLevel(logEntry.Level);
                            string formattedDate = FormatDate(logEntry.Date);
                            writer.WriteLine($"{formattedDate}\t{logEntry.Time}\t{standardizedLevel}\t{logEntry.Method}\t{logEntry.Message}");
                        }
                        else
                        {
                            problemsWriter.WriteLine(line);
                        }
                    }
                }

                return "Log standardization completed successfully.";
            }
            catch (Exception ex)
            {
                return $"An error occurred: {ex.Message}";
            }
        }

        private bool TryParseFormat1(string line, out LogEntry logEntry)
        {
            var pattern = @"^(\d{2})\.(\d{2})\.(\d{4})\s(\d{2}:\d{2}:\d{2}\.\d+)\s+(INFORMATION|WARNING|ERROR|DEBUG)\s+(.+)$";
            var match = Regex.Match(line, pattern);

            if (match.Success)
            {
                logEntry = new LogEntry
                {
                    Date = $"{match.Groups[3].Value}-{match.Groups[2].Value}-{match.Groups[1].Value}",
                    Time = match.Groups[4].Value,
                    Level = match.Groups[5].Value,
                    Method = "DEFAULT",
                    Message = match.Groups[6].Value
                };
                return true;
            }

            logEntry = null;
            return false;
        }

        private bool TryParseFormat2(string line, out LogEntry logEntry)
        {
            var pattern = @"^(\d{4}-\d{2}-\d{2})\s(\d{2}:\d{2}:\d{2}\.\d+)\|\s*(INFO|WARN|ERROR|DEBUG)\|\d+\|([^|]+)\|(.+)$";
            var match = Regex.Match(line, pattern);

            if (match.Success)
            {
                logEntry = new LogEntry
                {
                    Date = match.Groups[1].Value,
                    Time = match.Groups[2].Value,
                    Level = match.Groups[3].Value,
                    Method = match.Groups[4].Value.Trim(),
                    Message = match.Groups[5].Value.Trim()
                };
                return true;
            }

            logEntry = null;
            return false;
        }

        private string StandardizeLogLevel(string level)
        {
            return level.ToUpper() switch
            {
                "INFORMATION" => "INFO",
                "WARNING" => "WARN",
                _ => level.ToUpper()
            };
        }

        private string FormatDate(string date)
        {
            if (date.Contains("-"))
            {
                var parts = date.Split('-');
                return $"{parts[2]}-{parts[1]}-{parts[0]}";
            }

            return date;
        }
    }
}
