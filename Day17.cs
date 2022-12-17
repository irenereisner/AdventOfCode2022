using AdventOfCode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AdventOfCode2022
{
    [Day(17)]
    public class Day17 : Day
    {
        Grid<char> grid;
        List<Grid<char>> rocks;
        private string jetstream;
        private int highestPos = 0;


        private int nextRockIdx = 0;
        private Grid<char> currentRock;
        private Vec2 currentRockPos;
        private int jetIdx;
        private long gridHeightOffset = 0;
        private int[] highestPositions;

        private long iterations = 0;


        public override string RunPart1()
        {
            Run(2022);
            return (gridHeightOffset + highestPos).ToString();


            // wrong answer: 3121
        }

        public override string RunPart2()
        {
            Run(1000000000000);
            return (gridHeightOffset + highestPos).ToString();
        }


        private void Run(long maxIterations)
        {
            InitializeGrid();
            ReadJetstream();
            CreateRocks();

            jetIdx = 0;
            nextRockIdx = 0;
            StartNewRock();
            iterations = 1;
            while (true)
            {
                MoveSide(GetJet());
                //Console.WriteLine($"rock-pos after side {currentRockPos}");
                if (!MoveDown())
                {
                    FillInRock();

                    if (iterations < 11)
                        Console.WriteLine(grid.ToStringUpsideDown());
                    UpdateHighestPosition();
                    ShrinkGrid();
                    if (iterations == maxIterations)
                    {
                        break;
                    }
                    TestCycle(maxIterations);
                    StartNewRock();
                    //Console.WriteLine($"rock-pos init {currentRockPos}");
                    iterations++;
                }
                else
                {
                    //Console.WriteLine($"rock-pos after down {currentRockPos}");
                }
            }
        }

        private void InitializeGrid()
        {
            grid = new Grid<char>(7, 6, '.');
            highestPositions = new int[grid.Width];
            for (int i = 0; i < grid.Width; i++)
                highestPositions[i] = 0;
        }

        private void ReadJetstream()
        {
            jetstream = File.ReadAllText(InputFile);
            Console.WriteLine(jetstream);
        }

        private char GetJet()
        {
            var jet = jetstream[jetIdx];
            if(jet != '<' && jet !='>')
            {
                Console.WriteLine("wrong input?");
                IncreaseJetIdx();
                return jetstream[jetIdx];
            }
            IncreaseJetIdx();

            return jet;
        }

        private void IncreaseJetIdx()
        {
            jetIdx++;
            if (jetIdx == jetstream.Length)
            {
                jetIdx = 0;
            }
        }

        private void CreateRocks()
        {
            rocks = new List<Grid<char>>();
            rocks.Add(new Grid<char>(4, 1, "####"));
            rocks.Add(new Grid<char>(3, 3, ".#.###.#."));
            rocks.Add(new Grid<char>(3, 3, "###..#..#"));
            rocks.Add(new Grid<char>(1, 4, "####"));
            rocks.Add(new Grid<char>(2, 2, "####"));
        }

        private void StartNewRock()
        {
            if (nextRockIdx >= rocks.Count)
                nextRockIdx = nextRockIdx % rocks.Count;
            currentRock = rocks[nextRockIdx];
            nextRockIdx++;

            currentRockPos = new Vec2(2, highestPos + 3);
            if (grid.Height <= currentRockPos.Y + currentRock.Height)
                grid.AddRows(currentRock.Height, '.');
        }

        private bool MoveDown()
        {
            return Move(Vec2.down);
        }

        private bool MoveSide(char input)
        {
            var direction = input == '>' ? Vec2.right : Vec2.left;
            return Move(direction);
        }

        private bool Move(Vec2 direction)
        {
            var nextPos = currentRockPos + direction;
            if (!IsCollision(nextPos))
            {
                currentRockPos = nextPos;
                return true;
            }
            return false;
        }

        private void FillInRock()
        {
            for (int y = 0; y < currentRock.Height; y++)
            {
                for (int x = 0; x < currentRock.Width; x++)
                {
                    var p = new Vec2(x, y);
                    var gridPos = p + currentRockPos;
                    if (grid[gridPos] != '#')
                        grid[gridPos] = currentRock[p];
                }
            }
        }


        private bool IsCollision(Vec2 testPos)
        {
            for(int y = 0; y < currentRock.Height; y++)
            {
                for(int x = 0; x < currentRock.Width; x++)
                {
                    var p = new Vec2(x, y);
                    var gridPos = p + testPos;
                    if (!grid.IsPosInside(gridPos))
                        return true;

                    if (currentRock[p] == '#' && grid[gridPos] == '#')
                        return true;
                }
            }
            return false;
        }

        private void UpdateHighestPosition()
        {
            var posOfRock = currentRockPos.Y + currentRock.Height;
            highestPos = Math.Max(highestPos, posOfRock);

            for (int y = 0; y < currentRock.Height; y++)
            {
                for (int x = 0; x < currentRock.Width; x++)
                {
                    var p = new Vec2(x, y);
                    if (currentRock[p] == '#')
                    {
                        var gridPos = p + currentRockPos;
                        highestPositions[gridPos.X] = Math.Max(highestPositions[gridPos.X], gridPos.Y);
                    }
                }
            }
        }

        private void ShrinkGrid()
        {
            var full = new bool[grid.Width];

            var lowestNecessaryRow = highestPositions.Min();

            if(lowestNecessaryRow > 1)
            {
                grid.RemoveRows(0, lowestNecessaryRow);
                gridHeightOffset += lowestNecessaryRow;
                highestPos -= lowestNecessaryRow;
                for (int i = 0; i < highestPositions.Length; i++)
                    highestPositions[i] -= lowestNecessaryRow;
            }
        }

        private Dictionary<int, Tuple<long, long>> states = new Dictionary<int, Tuple<long, long>>();
        private bool foundCycle = false;
        private void TestCycle(long maxIterations)
        {
            if (foundCycle)
                return;

            var hashCode = nextRockIdx.GetHashCode();
            hashCode = hashCode * 31 + jetIdx.GetHashCode();

            foreach (var p in highestPositions)
                hashCode = hashCode * 31 + (highestPos - p).GetHashCode();


            if(states.ContainsKey(hashCode))
            {
                var time = iterations - states[hashCode].Item1;
                var cycles = (maxIterations - iterations) / time;
                iterations += cycles * time;

                var offset = (gridHeightOffset + highestPos) - states[hashCode].Item2;
                gridHeightOffset += cycles * offset;
                
                foundCycle = true;
            }
            else
            {
                states.Add(hashCode, Tuple.Create(iterations, gridHeightOffset + highestPos));
            }
        }

    }
}
