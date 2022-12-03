using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AdventOfCode2022
{

    public class Day3 : IDay
    {
        private string filename;

        public Day3(string filename)
        {
            this.filename = filename;
        }

        public string RunPart1()
        {
            var lines = File.ReadAllLines(filename);
            var doubleItems = lines.Select(GetDoubleItem).ToList();
            var priorities = doubleItems.Select(GetPriority).ToList();

            //foreach(var item in doubleItems.Zip(priorities))
            //    Console.WriteLine($"{item.First} => {item.Second}");
            return priorities.Sum().ToString();
        }
        public string RunPart2()
        {
            var lines = File.ReadAllLines(filename);

            var badges = new List<char>();
            for (int i = 0; i < lines.Length; i += 3)
            {
                var group = lines.Skip(i).Take(3).ToList();
                badges.Add(FindBadge(group));
            }

            return badges.Select(GetPriority).Sum().ToString();
        }

        private char FindBadge(List<string> rucksacks)
        {
            foreach (var c in rucksacks[0])
            {
                if (rucksacks[1].Contains(c) && rucksacks[2].Contains(c))
                    return c;
            }
            throw new Exception("no common badge available");
        }

        private char GetDoubleItem(string input)
        {
            var compartments = SplitCompartments(input);
            foreach (var c in compartments[0])
                if (compartments[1].Contains(c))
                    return c;

            throw new Exception("No double item");
        }

        private int GetPriority(char c)
        {
            if (c > 'a')
                return (int)(c) - (int)('a') + 1;
            return (int)(c - 'A') + 27;
        }


        private string[] SplitCompartments(string rucksack)
        {
            var split = rucksack.Length / 2;
            return new string[]
            {
            rucksack.Substring(0, split),
            rucksack.Substring(split)
            };
        }


    }
}