using AdventOfCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2022
{
    [Day(18)]
    public class Day18 : Day
    {
        List<Vec3> cubes;
        Grid3<bool> grid;
        Grid3<bool> handled;
        public override string RunPart1()
        {
            cubes = File.ReadAllLines(InputFile).Select(line => Vec3.Parse(line, ',')).ToList();
            CreateGrid();
            var result = ComputeSurfaceArea();
            return result.ToString();
        }

        public override string RunPart2()
        {
            cubes = File.ReadAllLines(InputFile).Select(line => Vec3.Parse(line, ',')).ToList();
            CreateGrid();

            long count = 0;
            var neighborOffsets = GetOffsets().ToList();
            var queue = new Queue<int>();
            foreach (var p in GetBorders(1))
                queue.Enqueue(p);


            handled = new Grid3<bool>(grid.Width, grid.Height, grid.Depth, false);
            foreach (var p in GetBorders(0))
                handled[p] = true;

            while(queue.Count > 0)
            {
                var idx = queue.Dequeue();
                if (handled[idx]) continue;

                handled[idx] = true;
                var v = grid[idx];
                foreach (var n in neighborOffsets)
                {
                    if (v != grid[idx + n])
                        count++;
                    else
                        queue.Enqueue(idx + n);
                }
            }

            return count.ToString();
        }


        private void CreateGrid()
        {
            Vec3 minVec = Vec3.MaxValue;
            Vec3 maxVec = Vec3.MinValue;
            foreach (var p in cubes)
            {
                minVec.X = Math.Min(minVec.X, p.X);
                minVec.Y = Math.Min(minVec.Y, p.Y);
                minVec.Z = Math.Min(minVec.Z, p.Z);
                maxVec.X = Math.Max(maxVec.X, p.X);
                maxVec.Y = Math.Max(maxVec.Y, p.Y);
                maxVec.Z = Math.Max(maxVec.Z, p.Z);
            }
            var offset = minVec - new Vec3(2, 2, 2);
            grid = new Grid3<bool>(new Vec3(5,5,5) + maxVec - minVec, false);

            for (int i = 0; i < cubes.Count; i++)
                cubes[i] = cubes[i] - offset;

            foreach (var cube in cubes)
            {
                grid[cube] = true;
            }
        }

        private long ComputeSurfaceArea()
        {
            var neighborOffsets = GetOffsets().ToList();
            long count = 0;
            foreach(var c in cubes)
            {
                var idx = grid.ToIndex(c);
                var v = grid[idx];
                foreach(var n in neighborOffsets)
                {
                    if (v != grid[idx + n])
                        count++;
                }
            }
            return count;
        }

        private IEnumerable<int> GetOffsets()
        {
            yield return -1;
            yield return +1;
            yield return -grid.Width;
            yield return +grid.Width;
            yield return -grid.Width * grid.Height;
            yield return +grid.Width * grid.Height;
        }

        private IEnumerable<int> GetBorders(int shrink = 0)
        {
            for(int z = 0; z < grid.Depth; z++)
            {
                for(int y = 0; y < grid.Height; y++)
                {
                    for(int x = 0; x < grid.Width; x++)
                    {
                        if (x == shrink || y == shrink || z == shrink 
                            || x == grid.Width - 1 - shrink || y == grid.Height - 1 - shrink || z == grid.Depth - 1 - shrink)
                            yield return grid.ToIndex(new Vec3(x, y, z));
                    }

                }
            }
        }
    }




}
