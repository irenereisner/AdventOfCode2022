using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{

    public class Grid<T> where T : IEquatable<T>
    {
        public int Height { get; private set; }
        public int Width { get; private set; }
        public List<T> Values { get; private set; }

        public Grid(int width, int height, IEnumerable<T> values)
        {
            Width = width;
            Height = height;
            this.Values = values.ToList();
        }
        public Grid(int width, int height, T value)
        {
            Width = width;
            Height = height;
            var length = width * height;
            Values = new List<T>(length);
            for (int i = 0; i < length; i++)
            {
                Values.Add(value);
            }
        }


        public void AddRows(int count, T value)
        {
            Values.AddRange(Enumerable.Range(0, count * Width).Select(x => value));
            Height += count;
        }

        public void RemoveRows(int index, int count)
        {
            var vIndex = index * Width;
            var vCount = count * Width;
            Values.RemoveRange(vIndex, vCount);
            Height = Height - count;
        }

        public int GetIndex(T value)
        {
            return Values.IndexOf(value);
        }

        public IEnumerable<int> GetIndices(T value)
        {
            for(int i = 0; i < Width * Height; i++)
            {
                if (Values[i].Equals(value))
                    yield return i;
            }
        }

        public Vec2 ToPos(int index)
        {
            return new Vec2(index % Width, index / Width);
        }
        public int ToIndex(Vec2 pos)
        {
            return ToIndex(pos.X, pos.Y);
        }
        public int ToIndex(int x, int y)
        {
            return x + y * Width;
        }

        public T this[Vec2 pos]
        {
            get { return Values[ToIndex(pos)]; }
            set { Values[ToIndex(pos)] = value; }
        }
        public T this[int x, int y]
        {
            get { return Values[ToIndex(x, y)]; }
            set { Values[ToIndex(x, y)] = value; }
        }

        public T this[int index]
        {
            get { return Values[index]; }
            set { Values[index] = value; }
        }

        public IEnumerable<int> GetNeighborIndices(int index)
        {
            return GetNeighborIndices(ToPos(index)).Select(ToIndex);
        }

        public IEnumerable<Vec2> GetNeighborIndices(Vec2 pos)
        {

            if (pos.X > 0) yield return new Vec2(pos.X - 1, pos.Y);
            if (pos.Y > 0) yield return new Vec2(pos.X, pos.Y - 1);
            if (pos.X < Width - 1) yield return new Vec2(pos.X + 1, pos.Y);
            if (pos.Y < Height - 1) yield return new Vec2(pos.X, pos.Y + 1);
        }


        public bool IsPosInside(Vec2 pos)
        {
            return pos.X >= 0 && pos.Y >= 0 && pos.X < Width && pos.Y < Height;
        }


        public override string ToString()
        {
            var str = "";
            IEnumerable<T> iter = Values;

            for(int y = 0; y < Height; y++)
            {
                str += string.Join("", iter.Take(Width));
                str += "\n";
                iter = iter.Skip(Width);
            }
            return str;
        }

        public string ToStringUpsideDown()
        {
            var str = "";
            IEnumerable<T> iter = Values;

            for (int y = 0; y < Height; y++)
            {
                str = string.Join("", iter.Take(Width)) + "\n" + str;
                iter = iter.Skip(Width);
            }
            return str;
        }
    }
}
