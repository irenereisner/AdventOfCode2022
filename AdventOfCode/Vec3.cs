using System;

namespace AdventOfCode
{

    public class Vec3
    {
        public int X;
        public int Y;
        public int Z;
        public Vec3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 23 + X.GetHashCode();
            hash = hash * 23 + Y.GetHashCode();
            hash = hash * 23 + Z.GetHashCode();
            return hash;
        }

        public static Vec3 Parse(string input, char separation)
        {
            var parts = input.Split(separation);
            return new Vec3(int.Parse(parts[0].Trim()), int.Parse(parts[1].Trim()), int.Parse(parts[2].Trim()));
        }
        public override string ToString()
        {
            return $"[{X},{Y},{Z}]";
        }

        public int ManhattanLength
        {
            get { return Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z); }
        }

        public static Vec3 operator +(Vec3 left, Vec3 right)
        {
            return new Vec3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static Vec3 operator -(Vec3 left, Vec3 right)
        {
            return new Vec3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static bool operator ==(Vec3 left, Vec3 right)
        {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
        }

        public static bool operator !=(Vec3 left, Vec3 right)
        {
            return left.X != right.X || left.Y != right.Y || left.Z != right.Z;
        }

        public static Vec3 zero => new Vec3(0, 0, 0);
        public static Vec3 one => new Vec3(1, 1, 1);

        public static Vec3 MaxValue => new Vec3(int.MaxValue, int.MaxValue, int.MaxValue);
        public static Vec3 MinValue => new Vec3(int.MinValue, int.MinValue, int.MinValue);
    }
}
