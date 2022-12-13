using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace AdventOfCode2022
{

    public class Day12 : IDay
    {
        private string filename;

        private Grid<char> grid;
        private Grid<int> distances;
        private Grid<int> predecessors;
        private Grid<bool> visited;

        public Day12(string filename)
        {
            this.filename = filename;
        }

        public string RunPart1()
        {
            var input = File.ReadAllLines(filename);
            grid = new Grid<char>(input[0].Length, input.Length,
                input.SelectMany(str => str.Select(c => c)));

            var startIndex = grid.GetIndex('S');
            var endIndex = grid.GetIndex('E');
            grid[startIndex] = 'a';
            grid[endIndex] = 'z';

            Init(startIndex);
            FindRoute();
            var count = GetResult(endIndex);


            return count.ToString();
        }


        public string RunPart2()
        {
            var input = File.ReadAllLines(filename);
            grid = new Grid<char>(input[0].Length, input.Length,
                input.SelectMany(str => str.Select(c => c)));

            var startIndex = grid.GetIndex('S');
            var endIndex = grid.GetIndex('E');
            grid[startIndex] = 'a';
            grid[endIndex] = 'z';

            Init(grid.GetIndices('a'));
            FindRoute();
            var count = GetResult(endIndex);


            return count.ToString();
        }

        private void Init(int startIndex)
        {
            distances = new Grid<int>(grid.Width, grid.Height, int.MaxValue);
            distances[startIndex] = 0;
            predecessors = new Grid<int>(grid.Width, grid.Height, -1);
            visited = new Grid<bool>(grid.Width, grid.Height, false);
        }

        private void Init(IEnumerable<int> startIndices)
        {
            distances = new Grid<int>(grid.Width, grid.Height, int.MaxValue);
            foreach(var startIndex in startIndices)
                distances[startIndex] = 0;
            predecessors = new Grid<int>(grid.Width, grid.Height, -1);
            visited = new Grid<bool>(grid.Width, grid.Height, false);
        }

        private void FindRoute()
        {
            var queue = new List<int>();
            for (int i = 0; i < grid.Width * grid.Height; i++)
                queue.Add(i);

            while (queue.Count > 0)
            {
                var minIdx = 0;
                for (int i = 0; i < queue.Count; i++)
                    if (distances[queue[i]] < distances[queue[minIdx]])
                        minIdx = i;

                var gridIdx = queue[minIdx];
                queue.RemoveAt(minIdx);
                visited[gridIdx] = true;


                foreach (var neighbor in grid.GetNeighborIndices(gridIdx))
                {
                    if (visited[neighbor]) continue;

                    if (grid[neighbor] - grid[gridIdx] <= 1 )
                    {
                        var newDistance = distances[gridIdx] + 1;
                        if(newDistance < distances[neighbor])
                        {
                            distances[neighbor] = newDistance;
                            predecessors[neighbor] = gridIdx;
                        }
                    }
                }
            }
        }

        private int GetResult(int endIndex)
        {
            var visualization = new Grid<char>(grid.Width, grid.Height, '.');

            var count = 0;
            var node = endIndex;
            while (predecessors[node] >= 0)
            {
                var pre = predecessors[node];

                visualization[pre] = GetVisChar(grid.ToPos(pre), grid.ToPos(node));
                Console.WriteLine($"{grid.ToPos(node)}({grid[node]})");

                node = predecessors[node];

                count++;
            }

            Console.WriteLine($"{grid.ToPos(node)}({grid[node]})");

            Console.WriteLine();
            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    Console.Write(visualization[x, y]);
                }
                Console.WriteLine();
            }

            return count;
        }

        private char GetVisChar(Vec2 predecessor, Vec2 current)
        {
            if (predecessor.X > current.X)
                return '<';
            if (predecessor.X < current.X)
                return '>';
            if (predecessor.Y < current.Y)
                return 'V';
            if (predecessor.Y > current.Y)
                return '^';
            return '_';
        }

    }
}