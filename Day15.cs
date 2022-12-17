using AdventOfCode;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022
{
    [Day(15)]
    public class Day15 : Day
    {
        private List<Vec2> sensors;
        private List<Vec2> beacons;
        private List<int> distances;


        int requestedY = 2000000;
        int minValue = 0;
        int maxValue = 4000000;
        int multiplier = 4000000;

        public Day15(bool test)
        {
            if (test)
            {
                maxValue = 20;
                requestedY = 10;
            }
        }

        public override string RunPart1()
        {
            ReadInput();

            var min = sensors.Concat(beacons).SelectMany(v => new[] { v.X, v.Y }).Min()-distances.Max();
            var max = sensors.Concat(beacons).SelectMany(v => new[] { v.X, v.Y }).Max()+distances.Max();


            var bsCount = GetSensorsAndBeacons(requestedY).Count();
            var free = GetFreePositions(requestedY, min, max);
            var occupied = (1 + max - min) - free.Count() - bsCount;

            // original pixel-based algorithm
            //var occupiedList = GetOccupiedPositions(requestedY);
            //occupied = occupiedList.Count();
            //Console.WriteLine(string.Join(", ", set.OrderBy(x => x)));

            return occupied.ToString();
        }


        public override string RunPart2()
        {
            ReadInput();

            var results = new List<Vec2>();

            for(int y = minValue; y <= maxValue; y++)
            {
                var values = GetFreePositions(y, minValue, maxValue).ToList();
                if (values.Count > 0)
                {
                    var x = values.First();
                    Console.WriteLine(new Vec2(x, y) + " " + (x * multiplier + y));
                    

                    results.Add(new Vec2(x, y));
                }
            }

            return (results[0].X * multiplier + results[0].Y).ToString();



            // wrong: 1012239654 (too low)
            /*var vectors = GetFreePosition().ToList();
            var vec = vectors[0];
            foreach (var v in vectors)
            {
                Console.WriteLine($"v: {v}");
            }
            return (vec.X * multiplier + vec.Y).ToString();*/
        }


        private IEnumerable<int> GetSensorsAndBeacons(int y)
        {
            var set = new HashSet<int>();
            foreach(var vec in sensors.Concat(beacons))
            {
                if (vec.Y == y)
                    set.Add(vec.X);

            }
            return set;
        }

        private void ReadInput()
        {
            sensors = new List<Vec2>();
            beacons = new List<Vec2>();
            distances = new List<int>();
            foreach(var values in Parser.ParseAllInts(InputFile))
            {
                var sensor = new Vec2(values[0], values[1]);
                var beacon = new Vec2(values[2], values[3]);
                sensors.Add(sensor);
                beacons.Add(beacon);
                distances.Add((sensor - beacon).ManhattanLength);
            }
        }

        private IEnumerable<int> GetOccupiedPositions(int requestedY)
        {
            var set = new HashSet<int>();
            var bsSet = new HashSet<int>();
            foreach (var s in sensors.Concat(beacons))
                if (s.Y == requestedY)
                    bsSet.Add(s.X);

            for (int i = 0; i < sensors.Count; i++)
            {
                var distance = distances[i];

                if (requestedY > sensors[i].Y - distance &&
                    requestedY < sensors[i].Y + distance)
                {

                    var maxX = distance - Math.Abs(sensors[i].Y - requestedY);
                    for (int x = -maxX; x <= maxX; x++)
                    {
                        var xVal = sensors[i].X + x;

                        if (!bsSet.Contains(xVal))
                            set.Add(xVal);
                    }

                }
            }
            return set;
        }

        private IEnumerable<int> GetFreePositions(int requestedY, int minVal, int maxVal)
        {

            var freeRange = new List<Range>();
            freeRange.Add(new Range(minVal, maxVal));


            for (int i = 0; i < sensors.Count; i++)
            {
                var distance = distances[i];

                if (requestedY > sensors[i].Y - distance &&
                    requestedY < sensors[i].Y + distance)
                {

                    var maxX = distance - Math.Abs(sensors[i].Y - requestedY);
                    var range = new Range(-maxX + sensors[i].X, maxX + sensors[i].X);

                    var newList = new List<Range>();
                    foreach(var existing in freeRange)
                    {
                        newList.AddRange(existing.RemoveSubRange(range));
                    }
                    freeRange = newList;
                }
                if (freeRange.Count == 0)
                    break;
            }

            return freeRange.SelectMany(range => range.GetValues());
        }



        private Vec2 GetFreePosition()
        {
            for (int i = 0; i < sensors.Count; i++)
            {
                foreach (var pos in GetBorder(sensors[i], distances[i] + 1))
                {
                    if (IsInsideArea(pos) && IsFree(pos))
                        return pos;
                }
            }
            throw new Exception("no free position");
        }

        private IEnumerable<Vec2> GetBorder(Vec2 sensor, int distance)
        {
            var y = 0;
            for(int x = -distance; x<=distance;x++)
            {
                yield return new Vec2(x, y) + sensor;
                yield return new Vec2(x, -y) + sensor;
                y++;
                if (y > distance)
                    y = -distance+1;
            }

        }


        private bool IsInsideArea(Vec2 pos)
        {
            return pos.X >= minValue && pos.Y >= minValue && pos.X <= maxValue && pos.Y <= maxValue;
        }

        private bool IsFree(Vec2 pos)
        {
            for(int i = 0; i < sensors.Count; i++)
            {
                if ((sensors[i] - pos).ManhattanLength <= distances[i])
                    return false;
            }
            return true;
        }

    }
}