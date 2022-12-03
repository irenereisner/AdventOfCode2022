using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AdventOfCode2022
{

    public class Day5 : IDay
    {
        private string filename;
        private Dictionary<int, Stack<char>> stacksOfCrates;
        private bool isPart2;

        public Day5(string filename)
        {
            this.filename = filename;
        }

        public string RunPart1()
        {
            return Run();
        }


        public string RunPart2()
        {
            isPart2 = true;
            return Run();
        }

        private string Run()
        {
            var inputLines = File.ReadAllLines(filename);
            var stackLines = inputLines.TakeWhile(l => !string.IsNullOrWhiteSpace(l)).ToList();
            var instructions = inputLines.Skip(stackLines.Count + 1).ToList();

            ReadStackInfo(stackLines);
            PrintStacks();

            foreach (var line in instructions)
            {
                ApplyInstruction(line);
            }
            PrintStacks();
            return GetTops();
        }

        private string GetTops()
        {
            var output = "";
            foreach(var stack in stacksOfCrates)
            {
                output += stack.Value.Peek().ToString();
            }
            return output;
        }

        private void PrintStacks()
        {
            foreach (var stack in stacksOfCrates)
            {
                Console.WriteLine($"{stack.Key}: {new string(stack.Value.ToArray())} ({stack.Value.Count})");
            }
            Console.WriteLine("--------------");
        }

        private void ReadStackInfo(List<string> inputLines)
        {
            var maxLineWidth = inputLines.Max(s => s.Length);
            var stackCount = (maxLineWidth + 1) / 4;

            stacksOfCrates = new Dictionary<int, Stack<char>>();

            var grid = new List<List<char>>();
            for (int i = 0; i < stackCount; i++)
            {
                grid.Add(new List<char>());
            }

            for(int lineIdx = 0; lineIdx < inputLines.Count; lineIdx++)
            {
                var line = inputLines[lineIdx];
                var readIndices = !line.Contains('[');

                for (int i = 1; i < line.Length; i += 4)
                {
                    var crate = line[i];

                    if (readIndices)
                    {
                        grid[i / 4].Reverse();
                        var stack = new Stack<char>();
                        foreach (var c in grid[i / 4])
                            stack.Push(c);
                        int index = int.Parse(crate.ToString());
                        stacksOfCrates[index] = stack;

                    }
                    else if (crate != ' ')
                        grid[i / 4].Add(crate);
                }
            }
        }

        private void ApplyInstruction(string input)
        {
            GetInstruction(input, out int srcIdx, out int dstIdx, out int count);
            if (isPart2)
                MoveCombined(srcIdx, dstIdx, count);
            else
                MoveSingle(srcIdx, dstIdx, count);
        }

        private void GetInstruction(string input, out int srcIdx, out int dstIdx, out int count)
        {
            var parts = input.Split(' ');
            count = int.Parse(parts[1]);
            srcIdx = int.Parse(parts[3]);
            dstIdx = int.Parse(parts[5]);
        }

        private void MoveSingle(int srcIdx, int dstIdx, int count)
        {
            for(int i = 0; i < count; i++)
            {
                stacksOfCrates[dstIdx].Push(stacksOfCrates[srcIdx].Pop());
                
            }
        }
        private void MoveCombined(int srcIdx, int dstIdx, int count)
        {
            var list = new List<char>();
            for (int i = 0; i < count; i++)
            {
                list.Add(stacksOfCrates[srcIdx].Pop());
            }
            list.Reverse();
            for (int i = 0; i < count; i++)
            {
                stacksOfCrates[dstIdx].Push(list[i]);
            }
        }
    }
}