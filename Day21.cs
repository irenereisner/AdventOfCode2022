using AdventOfCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2022
{
    [Day(21)]
    public class Day21 : Day
    {
        Dictionary<string, Job> monkeyJobs;

        private const string HumanMonkey = "humn";
        private const string RootMonkey = "root";

        public override string RunPart1()
        {
            ReadInput();

            var result = RunJob("root");

            return result.ToString();
        }
        public override string RunPart2()
        {
            ReadInput();
            monkeyJobs[RootMonkey].Operator = "=";
            monkeyJobs[HumanMonkey].IsHuman = true;

            var monkey1 = monkeyJobs[RootMonkey].Input1;
            var monkey2 = monkeyJobs[RootMonkey].Input2;

            
            long result = 0;
            if (JobNeedsHumanMonkey(monkey1))
                result = FindHumanInput(RunJob(monkey2), monkey1);
            else
                result = FindHumanInput(RunJob(monkey1), monkey2);
 
            return result.ToString();
        }

        private long FindHumanInput(long goal, string monkey)
        {
            if (monkey == HumanMonkey)
                return goal;

            var result = goal;
            var job = monkeyJobs[monkey];

            if(!JobNeedsHumanMonkey(job.Input1))
            {
                var number1 = RunJob(job.Input1);
                switch (job.Operator)
                {
                    case "+": result = result - number1; break;
                    case "*": result = result / number1; break;
                    case "-": result = number1 - result; break;
                    case "/": result = number1 / result; break;
                }
                return FindHumanInput(result, job.Input2);
            }
            else
            {
                var number2 = RunJob(job.Input2);
                switch (job.Operator)
                {
                    case "+": result = result - number2; break;
                    case "*": result = result / number2; break;
                    case "-": result = result + number2; break;
                    case "/": result = result * number2; break;
                }
                return FindHumanInput(result, job.Input1);
            }

        }

       
        private bool JobNeedsHumanMonkey(string monkey)
        {
            if (monkey == HumanMonkey) 
                return true;

            var job = monkeyJobs[monkey];
            if (job.HasNumber)
                return false;

            if (JobNeedsHumanMonkey(job.Input1))
                return true;
            if (JobNeedsHumanMonkey(job.Input2))
                return true;

            return false;
        }


        private long RunJob(string monkey)
        {
            var job = monkeyJobs[monkey];
            if (job.HasNumber)
                return job.Number;

            var number1 = RunJob(job.Input1);
            var number2 = RunJob(job.Input2);

            long result = 0;
            switch(job.Operator)
            {
                case "+": result = number1 + number2; break;
                case "-": result = number1 - number2; break;
                case "*": result = number1 * number2; break;
                case "/": result = number1 / number2; break;
                default: throw new System.Exception("input error " + job.Operator);
            }


            job.Number = result;
            job.HasNumber = true;

            //Console.WriteLine($"{monkey}: {job.Input1}({number1}) {job.Operator} {job.Input2}({number2}) = {job.Number}");

            return result;
        }

        private void ReadInput()
        {
            monkeyJobs = new Dictionary<string, Job>();
            var lines = File.ReadAllLines(InputFile);
            foreach(var line in lines)
            {
                var parts = line.Split(':');
                monkeyJobs[parts[0]] = new Job(parts[1]);
            }


        }

        private class Job
        {
            public bool HasNumber;
            public long Number;
            public string Operator;
            public string Input1;
            public string Input2;
            public bool IsHuman;

            public Job(string input)
            {
                input = input.Trim();
                HasNumber = long.TryParse(input, out Number);
                if(!HasNumber)
                {
                    var parts = input.Split(' ');
                    Input1 = parts[0];
                    Operator = parts[1];
                    Input2 = parts[2];
                }
            }
        }

    }
}