using System.Security.Cryptography;
using System.Text;

namespace Test1attempt1;

public static class CheckSum
{
    public static byte[] Evaluate(string path)
    {
        if (!File.Exists(path) && !Directory.Exists(path))
        {
            throw new ArgumentException("invalid path");
        }
        if (Directory.Exists(path))
        {
            var dirs = Directory.GetDirectories(path).ToList();
            dirs.Sort();
            var files = Directory.GetFiles(path).ToList();
            files.Sort();
            var argument = Encoding.UTF8.GetBytes(path);
            foreach (var dir in dirs)
            {
                var hash = Evaluate(dir);
                argument = argument.Concat(hash).ToArray();
            }
            foreach (var file in files)
            {
                var hash = Evaluate(file);
                argument = argument.Concat(hash).ToArray();
            }
            return MD5.HashData(argument);
        }
        else
        {
            var content = File.ReadAllBytes(path);
            return MD5.HashData(content);
        }
    }

    public static async Task<byte[]> EvaluateParallel(string path)
    {
        if (!File.Exists(path) && !Directory.Exists(path))
        {
            throw new ArgumentException("invalid path");
        }
        var attr = File.GetAttributes(path);
        if (Directory.Exists(path))
        {
            var dirs = Directory.GetDirectories(path).ToList();
            dirs.Sort();
            var files = Directory.GetFiles(path).ToList();
            files.Sort();
            var argument = Encoding.UTF8.GetBytes(path);
            var tasks = new List<Task<byte[]>>();
            foreach (var dir in dirs)
            {
                tasks.Add(EvaluateParallel(dir));
            }
            foreach (var file in files)
            {
                tasks.Add(EvaluateParallel(file));
            }
            foreach (var task in tasks)
            {
                argument = argument.Concat(await task).ToArray();
            }
            return MD5.HashData(argument);
        }
        else
        {
            var content = await File.ReadAllBytesAsync(path);
            return MD5.HashData(content);
        }
    }
}
