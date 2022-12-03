using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2022
{

    public class Day8 : IDay
    {
        private string filename;
        private TreeMatrix trees;


        public Day8(string filename)
        {
            this.filename = filename;
        }

        public string RunPart1()
        {
            ReadTrees();
            var visibleCount = trees.GetVisibleTreeCount();

            return visibleCount.ToString();
        }


        public string RunPart2()
        {
            ReadTrees();

            return trees.GetBestSpotScore().ToString();
        }

        private void ReadTrees()
        {
            var lines = File.ReadAllLines(filename);

            var height = lines.Length;
            var width = lines[0].Length;
            var values = lines.SelectMany(x => x.AsEnumerable()).Select(c => int.Parse(new string(c, 1)));

            trees = new TreeMatrix(width, height, values);
        }


        private class Matrix<T>
        {
            public T[] Values { get; set; }
            protected int width;
            protected int height;

            public Matrix(int width, int height, IEnumerable<T> values)
            {
                this.Values = values.ToArray();
                this.width = width;
                this.height = height;
            }

            public T GetValue(int x, int y)
            {
                return Values[x + y * width];
            }

            public T GetValue(Vec2 pos)
            {
                return Values[pos.X + pos.Y * width];
            }
        }

        private class TreeMatrix : Matrix<int>
        {
            public TreeMatrix(int width, int height, IEnumerable<int> values) : base(width, height, values)
            {
            }

            public int GetVisibleTreeCount()
            {
                var count = 0;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (IsVisible(x, y))
                            count++;
                    }
                }
                return count;
            }

            public int GetBestSpotScore()
            {
                var bestScore = 0;
                for (int y = 1; y < height-1; y++)
                {
                    for (int x = 1; x < width-1; x++)
                    {
                        var v = GetValue(x, y);
                        var score = CountVisibleTrees(v, GetViewsLeft(x, y))
                            * CountVisibleTrees(v, GetViewsRight(x, y))
                            * CountVisibleTrees(v, GetViewsUp(x, y))
                            * CountVisibleTrees(v, GetViewsDown(x, y));

                        if(score > bestScore)
                        {
                            bestScore = score;
                        }
                    }
                }
                return bestScore;
            }

            private int CountVisibleTrees(int compValue, IEnumerable<Vec2> sightPositions)
            {
                int count = 0;
                foreach (var p in sightPositions)
                {
                    count++;
                    if (GetValue(p) >= compValue)
                        break;
                }
                return count;
            }

            public bool IsVisible(int x, int y)
            {
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                    return true;

                var value = GetValue(x, y);
                return IsVisible(value, GetViewsLeft(x, y)) 
                    || IsVisible(value, GetViewsRight(x, y)) 
                    || IsVisible(value, GetViewsUp(x, y)) 
                    || IsVisible(value, GetViewsDown(x, y));
            }

            private bool IsVisible(int compValue, IEnumerable<Vec2> sightPositions)
            {
                foreach (var pos in sightPositions)
                    if (GetValue(pos) >= compValue)
                        return false;
                return true;
            }


            private IEnumerable<Vec2> GetViewsLeft(int x, int y)
            {
                for (int i = x - 1; i >= 0; i--)
                    yield return new Vec2(i, y);
            }
            private IEnumerable<Vec2> GetViewsRight(int x, int y)
            {
                for (int i = x + 1; i < width; i++)
                    yield return new Vec2(i, y);
            }
            private IEnumerable<Vec2> GetViewsUp(int x, int y)
            {
                for (int i = y - 1; i >= 0; i--)
                    yield return new Vec2(x, i);
            }
            private IEnumerable<Vec2> GetViewsDown(int x, int y)
            {
                for (int i = y + 1; i < height; i++)
                    yield return new Vec2(x, i);
            }
        }
    }
}