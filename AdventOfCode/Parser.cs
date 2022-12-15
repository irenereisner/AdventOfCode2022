using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    public static class Parser
    {
        public static IEnumerable<List<string>> SplitByEmptyLines(string file)
        {
            return SplitByEmptyLines(file, s => s);
        }

        public static IEnumerable<List<T>> SplitByEmptyLines<T>(string file, Func<string, T> convert)
        {
            var allLines = File.ReadAllLines(file);

            var currentList = new List<T>();
            foreach (var line in allLines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    yield return currentList;
                    currentList = new List<T>();
                }
                else
                {
                    currentList.Add(convert(line));
                }
            }
            if (currentList.Count > 0)
                yield return currentList;
        }

        public static IEnumerable<List<string>> SplitLinesBySpaces(string file)
        {
            return SplitLinesBySpaces(file, s => s);
        }

        public static IEnumerable<List<T>> SplitLinesBySpaces<T>(string file, Func<string, T> convert)
        {
            return SplitLines(file, ' ', convert);
        }

        public static IEnumerable<List<T>> SplitLines<T>(string file, char separator, Func<string, T> convert)
        {
            var allLines = File.ReadAllLines(file);
            foreach (var line in allLines)
            {
                yield return line.Split(separator).Select(s => convert(s)).ToList();
            }
        }

    }
}
