using System;

namespace AdventOfCode
{

    public struct Vec2
    {
        public int X;
        public int Y;
        public Vec2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Vec2 Parse(string input, char separation)
        {
            var parts = input.Split(separation);
            return new Vec2(int.Parse(parts[0].Trim()), int.Parse(parts[1].Trim()));
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }

        public override string ToString()
        {
            return $"[{X},{Y}]";
        }

        public int ManhattanLength
        {
            get { return Math.Abs(X) + Math.Abs(Y); }
        }

        public static Vec2 operator+(Vec2 left, Vec2 right)
        {
            return new Vec2(left.X + right.X, left.Y + right.Y);
        }

        public static Vec2 operator -(Vec2 left, Vec2 right)
        {
            return new Vec2(left.X - right.X, left.Y - right.Y);
        }

        public static bool operator==(Vec2 left, Vec2 right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator!=(Vec2 left, Vec2 right)
        {
            return left.X != right.X || left.Y != right.Y;
        }

        public static Vec2 zero { get { return new Vec2(0, 0); } }
        public static Vec2 right { get { return new Vec2(1, 0); } }
        public static Vec2 left { get { return new Vec2(-1, 0); } }
        public static Vec2 up { get { return new Vec2(0, 1); } }
        public static Vec2 down { get { return new Vec2(0, -1); } }

        
    }

}
