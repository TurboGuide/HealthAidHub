using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HealthAid_Hub_Final_
{
    internal class CsvFileHandler
    {
        private static readonly string directoryPath = @"C:\Users\divan\OneDrive\Documents\HealthAid_Hub(Final)\Inventory\";
        public static void WriteToCsv(string fileName, List<string[]> data)

        {
            try
            {
                string filePath = Path.Combine(directoryPath, fileName);

                if (data == null || !data.Any())
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        Console.WriteLine($"File {fileName} deleted because it is empty.");
                    }
                    else
                    {
                        Console.WriteLine($"File {fileName} does not exist, so no deletion was necessary.");
                    }
                    return;
                }

                EnsureDirectoryAndFileExist(filePath);

                using (var writer = new StreamWriter(filePath, append: false))
                {
                    foreach (var record in data.Where(r => int.Parse(r[3]) > 0))
                    {
                        writer.WriteLine(string.Join(",", record));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to CSV ({fileName}): {ex.Message}");
            }
        }
        public static List<string[]> ReadFromCsv(string fileName)
        {
            List<string[]> data = new List<string[]>();
            string filePath = Path.Combine(directoryPath, fileName);

            try
            {
                if (File.Exists(filePath))
                {
                    using (var reader = new StreamReader(filePath))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                data.Add(line.Split(','));
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"File not found: {fileName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading from CSV ({fileName}): {ex.Message}");
            }

            return data;
        }
        public static void AppendToCsv(string fileName, string[] data)
        {
            try
            {
                string filePath = Path.Combine(directoryPath, fileName);

                EnsureDirectoryAndFileExist(filePath);

                using (var writer = new StreamWriter(filePath, append: true))
                {
                    writer.WriteLine(string.Join(",", data));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error appending to CSV ({fileName}): {ex.Message}");
            }
        }
        private static void EnsureDirectoryAndFileExist(string filePath)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                if (!File.Exists(filePath))
                {
                    using (var writer = new StreamWriter(filePath, append: false)) { }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ensuring directory/file existence ({filePath}): {ex.Message}");
            }
        }
    }
}
