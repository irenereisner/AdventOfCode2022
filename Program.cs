using System;

namespace AdventOfCode2022
{
    static class Program
    {
        static void Main(string[] args)
        {
            var day = 11;
            var part = 2;
            var test = false;
            Run(day, part, test);
        }

        private static void Run(int day, int part, bool test)
        {
            var dayObj = GetCurrentDay(day, test);

            string result = default;
            switch(part)
            {
                case 1: result = dayObj.RunPart1(); break;
                case 2: result = dayObj.RunPart2(); break;
                default:  throw new NotSupportedException();
            }

            Console.WriteLine($"Day {day}: Result for Part {part}: {result}");
        }

        private static IDay GetCurrentDay(int day, bool test)
        {
            var prefix = test ? "test" : "input";
            var filePath = $"../../Data/{prefix}{day}.txt";

            switch(day)
            {
                case 1: return new Day1(filePath);
                case 2: return new Day2(filePath);
                case 3: return new Day3(filePath);
                case 4: return new Day4(filePath);
                case 5: return new Day5(filePath);
                case 6: return new Day6(filePath);
                case 7: return new Day7(filePath);
                case 8: return new Day8(filePath);
                case 9: return new Day9(filePath);
                case 10: return new Day10(filePath);
                case 11: return new Day11(filePath);
                case 12: return new Day12(filePath);
                case 13: return new Day13(filePath);
                case 14: return new Day14(filePath);
                default: throw new NotImplementedException();
            };
        }

    }
}
