using AdventOfCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2022
{

    public class Day9 : IDay
    {
        private string filename;

        private Vec2[] rope;
        private HashSet<Vec2> visitedPositions = new HashSet<Vec2>();

        public Day9(string filename)
        {
            this.filename = filename;
        }

        public string RunPart1()
        {
            var lines = File.ReadAllLines(filename);

            InitializeRope(2);

            foreach (var line in lines)
                MoveStep(line);

            return visitedPositions.Count.ToString();
        }


        public string RunPart2()
        {
            var lines = File.ReadAllLines(filename);

            InitializeRope(10);

            foreach (var line in lines)
                MoveStep(line);

            return visitedPositions.Count.ToString();
        }

        private void InitializeRope(int count)
        {
            rope = new Vec2[count];
            for (int i = 0; i < rope.Length; i++)
                rope[i] = Vec2.zero;


            visitedPositions.Clear();
            visitedPositions.Add(rope.Last());
        }


        private void MoveStep(string instruction)
        {
            var parts = instruction.Split(' ');
            var dir = GetDir(parts[0]);
            var count = int.Parse(parts[1]);
            for(int i = 0; i < count; i++)
            {
                rope[0] += dir;
                FollowRope();
            }
            Console.WriteLine($"{instruction} => head={rope.First()} tail={rope.Last()}");
        }

        private void FollowRope()
        {
            for(int i = 1; i < rope.Length; i++)
            {
                rope[i] = FollowTail(rope[i - 1], rope[i]);
            }
            visitedPositions.Add(rope.Last());
        }

        private Vec2 FollowTail(Vec2 head, Vec2 tail)
        {
            var diff = head - tail;
            if (Math.Abs(diff.X) > 1 || Math.Abs(diff.Y) > 1)
            {

                tail += new Vec2(Math.Sign(diff.X), Math.Sign(diff.Y));
            }
            return tail;
        }


        private Vec2 GetDir(string s)
        {
            if (s == "R") return Vec2.right;
            if (s == "L") return Vec2.left;
            if (s == "U") return Vec2.up;
            if (s == "D") return Vec2.down;
            throw new NotSupportedException("invalid direction");
        }

    }
}