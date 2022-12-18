using System;
using System.Linq;
using System.Reflection;

namespace AdventOfCode2022
{
    static class Program
    {
        static void Main(string[] args)
        {
            var day = int.Parse(args[0]);
            var part = int.Parse(args[1]);
            var test = bool.Parse(args[2]);

            Run(day, part, test);
        }

        private static void Run(int day, int part, bool test)
        {
            var dayObj = GetCurrentDay(day);

            var prefix = test ? "test" : "input";
            var filePath = $"../../Data/{prefix}{day}.txt";
            dayObj.InputFile = filePath;
            dayObj.IsTest = test;


            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            string result = default;
            switch(part)
            {
                case 1: result = dayObj.RunPart1(); break;
                case 2: result = dayObj.RunPart2(); break;
                default:  throw new NotSupportedException();
            }
            watch.Stop();
            Console.WriteLine($"execution time: {watch.ElapsedMilliseconds} ms");
            Console.WriteLine($"Day {day}: Result for Part {part}: {result}");
        }

        private static IDay GetCurrentDay(int day)
        {
            var allTypes = Assembly.GetExecutingAssembly().GetTypes();
            var dayTypes = allTypes.Where(t => t.IsDefined(typeof(DayAttribute)));
            var currentDayType = dayTypes.FirstOrDefault(t => t.GetCustomAttribute<DayAttribute>().Day == day);
            if(currentDayType != null)
                return (IDay)Activator.CreateInstance(currentDayType);
            
            throw new Exception($"Day {day} not found");
        }

    }
}
