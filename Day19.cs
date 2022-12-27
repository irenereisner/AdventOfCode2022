using AdventOfCode;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace AdventOfCode2022
{
    [Day(19)]
    public class Day19 : Day
    {
        private List<Dictionary<Material, Robot>> blueprints;

        private Dictionary<Material, Robot> activeBlueprint;

        private int MaxMinutes;

        private Dictionary<int, State>[] cachePerMinute;

        private Dictionary<Material, int> maxNeededCounts;


        private int maxGeodeCount;

        public override string RunPart1()
        {
            ReadInput(); 
            MaxMinutes = 24;

            long sum = 0;
            for(int i = 0; i < blueprints.Count; i++)
            {
                sum += (i + 1) * GenerateMaxGeodes(i);
            }

            return sum.ToString();
        }

        public override string RunPart2()
        {
            ReadInput(); 
            MaxMinutes = 32;


            var count = Math.Min(3, blueprints.Count);
            long result = 1;
            for (int i = 0; i < count; i++)
            {
                result *= GenerateMaxGeodes(i);
            }

            return result.ToString();
        }


        private int GenerateMaxGeodes(int blueprintIndex)
        {
            maxGeodeCount = 0;
            activeBlueprint = blueprints[blueprintIndex];

            cachePerMinute = new Dictionary<int, State>[MaxMinutes + 1];
            for(int i = 0; i < cachePerMinute.Length; i++)
                cachePerMinute[i] = new Dictionary<int, State>();

            ComputeMaxNeededCount();

            State.Blueprint = activeBlueprint;

            var state = new State();
            state.Robots[Material.Ore] = 1;
            state.Minute = 0;

            var bestState = Update(state);

            Console.WriteLine($"-------------- {blueprintIndex + 1} --------------");
            //Console.WriteLine(bestState.History);
            Console.WriteLine(bestState.ToString());
            Console.WriteLine();

            return bestState.GeodeCount;
        }



        private IEnumerable<Material> GetUsefulRobots(State state)
        {
            if (MaxMinutes - state.Minute > 1)
            {
                if (state.CanMakeRobot(Material.Geode))
                {
                    yield return Material.Geode;
                }
                else if (MaxMinutes - state.Minute >= 2)
                {
                    if (state.CanMakeRobot(Material.Obsidian) && state.Robots[Material.Obsidian] < maxNeededCounts[Material.Obsidian])
                    {
                        yield return Material.Obsidian;
                    }
                    if (MaxMinutes - state.Minute >= 3)
                    {
                        if (state.CanMakeRobot(Material.Clay) && state.Robots[Material.Clay] < maxNeededCounts[Material.Clay])
                            yield return Material.Clay;

                        if (state.CanMakeRobot(Material.Ore) && state.Robots[Material.Ore] < maxNeededCounts[Material.Ore])
                            yield return Material.Ore;

                    }
                }
            }
        }

        private int ComputeMaxPossibleGeodes(State state)
        {
            var remainingMinutes = MaxMinutes - state.Minute;
            var newRobotGeodes = ((remainingMinutes - 1) * remainingMinutes) / 2;
            var newGeodes = state.Robots[Material.Geode] * remainingMinutes;
            return newRobotGeodes + newGeodes + state.GeodeCount;
        }


        private State Update(State input)
        {
            if (input.Minute >= MaxMinutes - 1)
            {
                var output = new State(input);
                output.Minute++;
                output.MakeMaterials();
                output.History = input.ToString() + output.History;
                maxGeodeCount = Math.Max(maxGeodeCount, output.GeodeCount);
                return output;
            }

            var remainingPossibleGeodes = ComputeMaxPossibleGeodes(input);
            if (remainingPossibleGeodes < maxGeodeCount)
            {
                return input;
            }

            var cacheKey = input.GetHashCode();
            if (cachePerMinute[input.Minute].TryGetValue(cacheKey, out State cachedState))
                return new State(cachedState);


            var outputs = new List<State>
            {
                new State(input)
            };


            var makeRobots = GetUsefulRobots(input);
            foreach(var makeRobot in makeRobots)
            {
                var newState = new State(input);
                newState.MakeRobot(makeRobot);
                outputs.Add(newState);
            }


            State maxGeodesState = null;
            foreach(var state in outputs)
            {
                state.MakeMaterials();
                state.Minute++;


                State result = Update(state);
                if (maxGeodesState == null || result.GeodeCount >= maxGeodesState.GeodeCount)
                {
                    maxGeodesState = result;
                }

            }

            maxGeodesState.History = input.ToString() + maxGeodesState.History;
            cachePerMinute[input.Minute][cacheKey] = maxGeodesState;

            return maxGeodesState;

        }

        private void ComputeMaxNeededCount()
        {
            maxNeededCounts = new Dictionary<Material, int>();
            
            maxNeededCounts[Material.Obsidian] = activeBlueprint.Values.Max(robot => robot.GetInputCount(Material.Obsidian));
            maxNeededCounts[Material.Clay] = activeBlueprint.Values.Max(robot => robot.GetInputCount(Material.Clay));
            maxNeededCounts[Material.Ore] = activeBlueprint.Values.Max(robot => robot.GetInputCount(Material.Ore));
        }

        private void ReadInput()
        {
            var lines = File.ReadAllLines(InputFile);
            blueprints = new List<Dictionary<Material, Robot>>(lines.Length);
            foreach (var line in lines)
                ReadBlueprint(line);
        }
        
        private void ReadBlueprint(string line)
        {
            var sentences = line.Split(':')[1].Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            var robots = sentences.Select(s => ReadRobot(s)).ToDictionary(r => r.Output);
            blueprints.Add(robots);
        }

        private Robot ReadRobot(string sentence)
        {
            var materials = new List<Material>();
            var numbers = new List<int>();
            foreach(var word in sentence.Split(' '))
            {
                switch(word)
                {
                    case "ore": materials.Add(Material.Ore); break;
                    case "clay": materials.Add(Material.Clay);break;
                    case "obsidian": materials.Add(Material.Obsidian);break;
                    case "geode":materials.Add(Material.Geode);break;
                    default:
                        if (int.TryParse(word, out int num))
                            numbers.Add(num);
                        break;
                }
            }

            return new Robot(materials.First(), materials.Skip(1).ToArray(), numbers.ToArray());
        }
        
        public enum Material 
        {
            Ore = 1,
            Clay = 2,
            Obsidian = 4,
            Geode = 8,
        }


        public class State
        {
            public static Dictionary<Material, Robot> Blueprint;
            public MaterialList Robots;
            public MaterialList Materials;
            public List<Material> RobotsInTheMaking;
            public List<Material> LockedRobots;
            public int Minute = 1;

            public string History = "";

            public State()
            {
                Robots = new MaterialList();
                Materials = new MaterialList();
                RobotsInTheMaking = new List<Material>();
                LockedRobots = new List<Material>();
                Minute = 1;
            }

            public State(State state)
            {
                Robots = new MaterialList(state.Robots);
                Materials = new MaterialList(state.Materials);
                RobotsInTheMaking = new List<Material>();
                LockedRobots = new List<Material>();
                Minute = state.Minute;
            }

            public int GeodeCount
            {
                get
                {
                    return Materials[Material.Geode];
                }
            }

            public bool CanMakeRobot(Material material)
            {
                if (LockedRobots.Contains(material))
                    return false;

                foreach (var input in Blueprint[material].Inputs)
                {
                    if (Materials[input.Key] < input.Value)
                        return false;
                }
                return true;
            }


            public override int GetHashCode()
            {
                var values = Robots.Values.Concat(Materials.Values);

                int hash = 1430287;
                foreach (var val in values)
                    hash = hash * 7302013 ^ val.GetHashCode();


                int locked = LockedRobots.Sum(m => (int)m);
                hash = hash * 7302013 ^ locked.GetHashCode();

                return hash;
            }
        
            public void MakeRobot(Material material)
            {
                foreach (var input in Blueprint[material].Inputs)
                {
                    Materials[input.Key] -= input.Value;
                }
                RobotsInTheMaking.Add(material);
                
            }

            public void MakeMaterials()
            {
                if (RobotsInTheMaking.Count == 0)
                    LockIgnoredRobots();
                else
                    LockedRobots.Clear();

                foreach (var robot in Robots)
                {
                    Materials[robot.Key] += robot.Value;
                }
                foreach (var robot in RobotsInTheMaking)
                    Robots[robot]++;
                RobotsInTheMaking.Clear();
            }


            private void LockIgnoredRobots()
            {
                foreach (var material in Robots.Keys)
                {
                    if (CanMakeRobot(material) && !RobotsInTheMaking.Contains(material))
                        LockedRobots.Add(material);
                }
            }
            


            public override string ToString()
            {
                return $"== Minute {Minute} ==\n" + 
                    $"Robots: {string.Join(", ", Robots.Select(m => $"{m.Value} {m.Key}"))}\n" +
                    $"Materials: {string.Join(", ", Materials.Select(m => $"{m.Value} {m.Key}"))}\n\n";
            }
        }

        public class MaterialList : Dictionary<Material, int>
        {
            public MaterialList()
                : base()
            {
                base[Material.Geode] = 0;
                base[Material.Obsidian] = 0;
                base[Material.Clay] = 0;
                base[Material.Ore] = 0;
            }

            public MaterialList(MaterialList other)
             : base(other)
            { }
        }

        public class Robot
        {
            public Material Output;
            public Dictionary<Material, int> Inputs;

            public Robot(Material output, Material[] inputs, int[] inputCounts)
            {
                Output = output;
                Inputs = new Dictionary<Material, int>();
                for (int i = 0; i < inputs.Length; i++)
                    Inputs[inputs[i]] = inputCounts[i];
            }

            public int GetInputCount(Material material)
            {
                if (Inputs.TryGetValue(material, out int result))
                    return result;
                return 0;
            }

            public override string ToString()
            {
                return $"{Output}: {string.Join(", ", Inputs.Select(i => i.Value + " " + i.Key))}";
            }
        }

    }
}
