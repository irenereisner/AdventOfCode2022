using System;
using System.IO;
using System.Linq;

namespace AdventOfCode2022
{
    [Day(4)]
    public class Day4 : Day
    {
        public override string RunPart1()
        {
            var lines = File.ReadAllLines(InputFile);

            return lines.Where(HasFullContainment).Count().ToString();
        }


        public override string RunPart2()
        {
            var lines = File.ReadAllLines(InputFile);

            return lines.Where(HasOverlap).Count().ToString();
        }

        public bool HasFullContainment(string line)
        {
            DivideIntoParts(line, out string part1, out string part2);
            GetBounds(part1, out int start1, out int end1);
            GetBounds(part2, out int start2, out int end2);

            if (start1 <= start2 && end1 >= end2)
                return true;

            if (start2 <= start1 && end2 >= end1)
                return true;

            return false;
        }

        public bool HasOverlap(string line)
        {
            DivideIntoParts(line, out string part1, out string part2);
            GetBounds(part1, out int start1, out int end1);
            GetBounds(part2, out int start2, out int end2);


            if (end1 < start2)
                return false;

            if (start2 > end2)
                return false;

            if (end2 < start1)
                return false;

            if (start2 > end1)
                return false;

            return true;
        }


        public void DivideIntoParts(string input, out string part1, out string part2)
        {
            var parts = input.Split(',');
            if (parts.Length != 2)
                throw new Exception("input error in line: " + input);

            part1 = parts[0];
            part2 = parts[1];
        }

        public void GetBounds(string section, out int start, out int end)
        {
            var parts = section.Split('-');
            if (parts.Length != 2)
                throw new Exception("input error in section: " + section);

            start = int.Parse(parts[0]);
            end = int.Parse(parts[1]);
        }
    }
}