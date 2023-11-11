using System.Text.RegularExpressions;

string pattern1 = @"^\d+ \d+$";

string path = "../../../test.txt";

var line = File.ReadAllLines(path)[0];

Console.WriteLine(Regex.IsMatch(line, pattern1)) ;


//Console.WriteLine(Regex.IsMatch(line, pattern1));