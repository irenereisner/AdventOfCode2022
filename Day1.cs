using AdventOfCode;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022
{
    [Day(1)]
    public class Day1 : Day
    {
        public override string RunPart1()
        {
            var caloriesPerElf = GetCaloriesPerElf();
            return caloriesPerElf.Max().ToString();
        }
        public override string RunPart2()
        {
            var caloriesPerElf = GetCaloriesPerElf();
            return caloriesPerElf.OrderByDescending(c => c).Take(3).Sum().ToString();
        }


        private IEnumerable<int> GetCaloriesPerElf()
        {
            var calories = Parser.SplitByEmptyLines(InputFile, s => int.Parse(s));
            var caloriesPerElf = calories.Select(c => c.Sum());
            return caloriesPerElf;
        }
    }
}