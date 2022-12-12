using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2022
{

    public class Day11 : IDay
    {
        private string filename;

        private Dictionary<int, Monkey> monkeys = new Dictionary<int, Monkey>();

        public Day11(string filename)
        {
            this.filename = filename;
        }

        public string RunPart1()
        {
            var lines = File.ReadAllLines(filename);

            ReadInput(lines);
            RunOperations(20, true);

            var inspections = monkeys.Values.Select(m => m.Inspections).OrderByDescending(m => m).Take(2).ToList();

            return (inspections[0] * inspections[1]).ToString();
        }


        public string RunPart2()
        {
            var lines = File.ReadAllLines(filename);
            ReadInput(lines);
            RunOperations(10000, false);

            var inspections = monkeys.Values.Select(m => m.Inspections).OrderByDescending(m => m).Take(2).ToList();

            return ((long)inspections[0] * inspections[1]).ToString();
        }


        private int GetMonkeyIndex(string line)
        {
            var part2 = line.Substring(line.IndexOf(' ')+1);
            part2 = part2.Substring(0, part2.Length - 1);
            return int.Parse(part2);
        }

        private void ReadInput(string[] lines)
        {
            for (int i = 0; i < lines.Length; i += 7)
            {
                var monkeyIndex = GetMonkeyIndex(lines[i]);
                var monkey = new Monkey(monkeyIndex);
                monkey.ReadThrowRoutine(lines.Skip(i + 3).Take(3).ToList());
                monkey.ReadStartingItems(lines[i + 1]);
                monkey.ReadOperation(lines[i + 2]);
                monkeys[monkeyIndex] = monkey;
            }


            var allDivisors = monkeys.Select(m => m.Value.Divisor).ToList();
            foreach(var monkey in monkeys)
            {
                foreach (var item in monkey.Value.Items)
                    item.AddDivisors(allDivisors);
            }
        }

        private void RunOperations(int rounds, bool reduceWorryLevel)
        {
            for (int round = 0; round < rounds; round++)
            {
                for (int monkeyIndex = 0; monkeyIndex < monkeys.Count; monkeyIndex++)
                {
                    var monkey = monkeys[monkeyIndex];

                    monkey.DoOperation();
                    if(reduceWorryLevel) 
                        monkey.GetBored();
                    monkey.TestAndThrow(monkeys);

                }

                if(round < 20 || (round+1) % 1000 == 0)
                {
                    Console.WriteLine($"== After round {round+1} ==");
                    foreach (var monkey in monkeys.Values)
                    {
                        Console.WriteLine(monkey);
                    }
                    Console.WriteLine();
                }
            }
        }


        private class Monkey
        {
            public int Index;
            public List<Number> Items = new List<Number>();
            public int Inspections = 0;
            public string Operand1;
            public string Operand2;
            public string Operation;
            public int Divisor = 1;
            public int ThrowIndexTrue;
            public int ThrowIndexFalse;

            public Monkey(int index)
            {
                Index = index;
            }

            public void ReadStartingItems(string input)
            {
                Items = input.Split(':').Last().Split(',').Select(str => 
                    new Number(int.Parse(str.Trim()))).ToList();
            }

            public void ReadOperation(string input)
            {
                var op = input.Split(':').Last().Trim();
                var calc = op.Split('=').Last().Trim();

                var calcItems = calc.Split(' ').Select(str => str.Trim()).ToList();
                Operand1 = calcItems[0];
                Operation = calcItems[1];
                Operand2 = calcItems[2];
            }

            public void DoOperation()
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if(Operation == "+")
                    {
                        if (Operand1 == "old" && Operand2 == "old")
                        {
                            Items[i].Multiply(2);
                        }
                        else if (Operand1 == "old")
                        {
                            Items[i].Add(int.Parse(Operand2));
                        }
                        else
                            throw new NotImplementedException();
                    }
                    else if(Operation == "*")
                    {
                        if (Operand1 == "old" && Operand2 == "old")
                        {
                            Items[i].Square();
                        }
                        else if (Operand1 == "old")
                        {
                            Items[i].Multiply(int.Parse(Operand2));
                        }
                        else
                            throw new NotImplementedException();
                    }

                    Inspections++;
                }
            }

            public void GetBored()
            {
                for(int i = 0; i < Items.Count; i++)
                {
                    Items[i].Divide(3);
                }
            }

            public void ReadThrowRoutine(List<string> inputs)
            {
                Divisor = int.Parse(inputs[0].Split(' ').Last());
                ThrowIndexTrue = int.Parse(inputs[1].Split(' ').Last());
                ThrowIndexFalse = int.Parse(inputs[2].Split(' ').Last());
            }
            
            public void TestAndThrow(Dictionary<int, Monkey> allMonkeys)
            {
                for(int i = 0; i < Items.Count; i++)
                {
                    var isDivisible = Items[i].IsDivisible(Divisor);
                    var monkeyIndex = isDivisible ? ThrowIndexTrue : ThrowIndexFalse;
                    allMonkeys[monkeyIndex].Items.Add(Items[i]);
                }
                // all items are thrown away
                Items.Clear();
            }

            public override string ToString()
            {
                //return $"Monkey {Index}: {Inspections} inspections";
                return $"Monkey {Index}: [{string.Join(",", Items)}] ({Inspections} inspections)";
            }
        }



        private class Number
        {
            public HashSet<int> divisors = new HashSet<int>();
            public int divisorProduct = 1;

            public long Value;

            public Number(int number)
            {
                Value = number;
            }

            public bool IsDivisible(int divisor)
            {
                return Value % divisor == 0;
            }

            public void AddDivisors(IEnumerable<int> divisors)
            {

                foreach (var divisor in divisors)
                {
                    if (!this.divisors.Contains(divisor))
                    {
                        this.divisors.Add(divisor);
                        divisorProduct *= divisor;
                    }
                }
            }

            public long GetNumber()
            {
                return Value;
            }

            public void Add(int number)
            {
                Value += number;
                FixValue();
            }
            public void Multiply(int number)
            {
                Value *= number;
                FixValue();
            }
            public void Divide(int number)
            {
                Value /= number;
            }


            public void Square()
            {
                Value = Value * Value;
                FixValue();
            }

            private void FixValue()
            {

                if (Value > divisorProduct)
                    Value = Value % divisorProduct;
            }

            public override string ToString()
            {
                return Value.ToString();
            }
        }

    }
}