using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2022
{

    public class Day1 : IDay
    {
        private string filename;

        public Day1(string filename)
        {
            this.filename = filename;
        }

        public string RunPart1()
        {
            var lines = File.ReadAllLines(filename);
            var caloriesPerElf = GetCaloriesPerElf(lines);
            return caloriesPerElf.Max().ToString();
        }
        public string RunPart2()
        {
            string[] lines = File.ReadAllLines(filename);
            var caloriesPerElf = GetCaloriesPerElf(lines);
            return caloriesPerElf.OrderByDescending(c => c).Take(3).Sum().ToString();
        }


        private IEnumerable<int> GetCaloriesPerElf(string[] lines)
        {
            var caloriesPerElf = new List<int>();
            var currentSum = 0;
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace((line)))
                {
                    caloriesPerElf.Add(currentSum);
                    currentSum = 0;
                }
                else
                {
                    currentSum += int.Parse(line);
                }
            }
            if (currentSum > 0)
                caloriesPerElf.Add(currentSum);

            return caloriesPerElf;
        }
    }
}