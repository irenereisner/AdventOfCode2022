using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace AdventOfCode2022
{

    public class Day14 : IDay
    {
        private string filename;

        private Vec2 spawner = new Vec2(500, 0);
        private List<List<Vec2>> rocks = new List<List<Vec2>>();
        private Vec2 offset;
        private Grid<char> grid;

        public Day14(string filename)
        {
            this.filename = filename;
        }

        public string RunPart1()
        {
            ReadRocks();            
            CreateGrid();
            InitializeGrid();
            Console.WriteLine(grid);

            int count = 0;
            while (!SimulateSand())
            {
                count++;
            }

            Console.WriteLine(grid);

            return count.ToString();
        }


        public string RunPart2()
        {
            ReadRocks();
            AddBottomLine();
            CreateGrid();
            InitializeGrid();

            Console.WriteLine(grid);

            int count = 0;
            while (!SimulateSand())
            {
                count++;
            }

            Console.WriteLine(grid);

            var sandCount = grid.Values.Where(x => x == 'o').Count();
            return sandCount.ToString();
        }

        private void ReadRocks()
        {
            var lines = File.ReadAllLines(filename);

            foreach (var line in lines)
            {
                var points = line.Split(new[] { " -> " }, StringSplitOptions.None).Select(str => Vec2.Parse(str, ',')).ToList();
                rocks.Add(points);
            }
        }

        private void AddBottomLine()
        {
            ComputeMinAndMax(out int minX, out int minY, out int maxX, out int maxY);

            var y = maxY + 2;
            var length = (2 + maxY - minY);
            rocks.Add(new List<Vec2>()
            {
                new Vec2(minX-length, y),
                new Vec2(maxX+length, y)
            });
        }

        private void CreateGrid()
        {
            ComputeMinAndMax(out int minX, out int minY, out int maxX, out int maxY);


            offset = new Vec2(minX - 1, minY - 1);
            grid = new Grid<char>(2 + maxX - minX, 2 + maxY - minY, '.');

        }

        private void ComputeMinAndMax(out int minX, out int minY, out int maxX, out int maxY)
        {
            var allPoints = rocks.SelectMany(p => p).ToList();
            allPoints.Add(spawner);
            minX = allPoints.Select(p => p.X).Min();
            maxX = allPoints.Select(p => p.X).Max();
            minY = allPoints.Select(p => p.Y).Min();
            maxY = allPoints.Select(p => p.Y).Max();
        }

        private void InitializeGrid()
        {
            foreach (var path in rocks)
            {
                for (int i = 1; i < path.Count; i++)
                {
                    var start = path[i - 1] - offset;
                    var end = path[i] - offset;
                    grid[end] = '#';
                    grid[start] = '#';

                    var dir = end - start;
                    dir.X = Math.Sign(dir.X);
                    dir.Y = Math.Sign(dir.Y);
                    var x = start;
                    while (x != end)
                    {
                        x = x + dir;
                        grid[x] = '#';
                    }
                }
            }

            grid[spawner - offset] = '+';
        }

        private bool SimulateSand()
        {
            var start = spawner - offset;
            var sand = start;


            while(GetNextPos(ref sand))
            {
                if (sand.Y == grid.Height - 1)
                    return true;
            }

            grid[sand] = 'o';
            if (sand == start)
                return true;

            return false;
        }

        private bool GetNextPos(ref Vec2 pos)
        {
            var testPositions = new List<Vec2>()
            {
                pos + new Vec2(0, 1),
                pos + new Vec2(-1, 1),
                pos + new Vec2(1, 1)
            };

            foreach(var next in testPositions)
            {
                if (grid[next] == '.')
                {
                    pos = next;
                    return true;
                }
            }
            return false;
        }

    }
}