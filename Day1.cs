using AdventOfCode;
using System.Collections.Generic;
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
            var caloriesPerElf = GetCaloriesPerElf();
            return caloriesPerElf.Max().ToString();
        }
        public string RunPart2()
        {
            var caloriesPerElf = GetCaloriesPerElf();
            return caloriesPerElf.OrderByDescending(c => c).Take(3).Sum().ToString();
        }


        private IEnumerable<int> GetCaloriesPerElf()
        {
            var calories = Parser.SplitByEmptyLines(filename, s => int.Parse(s));
            var caloriesPerElf = calories.Select(c => c.Sum());
            return caloriesPerElf;
        }
    }
}