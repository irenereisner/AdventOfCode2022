using System;

namespace AdventOfCode2022
{
    public interface IDay
    {
        string InputFile { get; set; }
        bool IsTest { get; set; }

        string RunPart1();
        string RunPart2();
    }

    public abstract class Day : IDay
    {

        public Day() { }
        
        public string InputFile { get; set; }
        public bool IsTest { get; set; }
        public abstract string RunPart1();
        public abstract string RunPart2();
    }


    public class DayAttribute : Attribute
    {
        public int Day;

        public DayAttribute(int day)
        {
            Day = day;
        }
    }

}