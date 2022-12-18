using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public class Grid3<T>
    {
        public int Depth { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        public List<T> Values { get; private set; }

        private int Slice;
        public Grid3(int width, int height, int depth, IEnumerable<T> values)
        {
            Width = width;
            Height = height;
            Depth = depth;
            Slice = width * height;
            this.Values = values.ToList();
        }
        public Grid3(int width, int height, int depth, T value)
        {
            Width = width;
            Height = height;
            Depth = depth;
            Slice = width * height;
            var length = width * height * depth;
            Values = new List<T>(length);
            for (int i = 0; i < length; i++)
            {
                Values.Add(value);
            }
        }
        public Grid3(Vec3 size, T value)
            : this(size.X, size.Y, size.Z, value)
        {
        }

        public Vec3 ToPos(int index)
        {
            var xy = (index % Slice);
            return new Vec3(xy % Width, xy / Width, index / Slice);
        }
        public int ToIndex(Vec3 pos)
        {
            return ToIndex(pos.X, pos.Y, pos.Z);
        }
        public int ToIndex(int x, int y, int z)
        {
            return x + y * Width + z * Slice;
        }

        public T this[Vec3 pos]
        {
            get { return Values[ToIndex(pos)]; }
            set { Values[ToIndex(pos)] = value; }
        }
        public T this[int x, int y, int z]
        {
            get { return Values[ToIndex(x, y, z)]; }
            set { Values[ToIndex(x, y, z)] = value; }
        }

        public T this[int index]
        {
            get { return Values[index]; }
            set { Values[index] = value; }
        }
    }
}
