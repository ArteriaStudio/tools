// See https://aka.ms/new-console-template for more information
using System.Text.RegularExpressions;

Console.WriteLine("Hello, World!");

var pSQL = "SELECT * FROM MAccounts WHERE Name IS NULL;";
var pMatch = Regex.Match(pSQL, @"[A-Z]*");
var pIsMatch = Regex.IsMatch(pSQL, @"[A-Z]*");

Console.WriteLine(pMatch);
Console.WriteLine(pIsMatch);
