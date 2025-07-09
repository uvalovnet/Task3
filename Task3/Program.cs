using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Task3;


var jsonData = File.ReadAllText(@"appsettings.json");
var Configuration = JsonSerializer.Deserialize<SettingsModel>(jsonData);

Console.WriteLine("Введите путь к лог файлу");
var log = Console.ReadLine();
var standardizer = new LogStandardizer(log, Configuration.LogsFile, Configuration.NotValidLogsFile);
standardizer.ProcessLogs();