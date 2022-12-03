namespace AdventOfCode2022
{

    internal struct Vec2
    {
        public int X;
        public int Y;
        public Vec2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }

        public override string ToString()
        {
            return $"[{X},{Y}]";
        }

        public static Vec2 operator+(Vec2 left, Vec2 right)
        {
            return new Vec2(left.X + right.X, left.Y + right.Y);
        }

        public static Vec2 operator -(Vec2 left, Vec2 right)
        {
            return new Vec2(left.X - right.X, left.Y - right.Y);
        }

        public static Vec2 zero { get { return new Vec2(0, 0); } }
        public static Vec2 right { get { return new Vec2(1, 0); } }
        public static Vec2 left { get { return new Vec2(-1, 0); } }
        public static Vec2 up { get { return new Vec2(0, 1); } }
        public static Vec2 down { get { return new Vec2(0, -1); } }

        
    }

}
