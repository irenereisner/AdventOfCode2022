using System;
using System.IO;
using System.Linq;

namespace AdventOfCode2022
{
    [Day(10)]
    public class Day10 : Day
    {
        private int cycle = 0;
        private int xValue = 1;
        private int signalStrength = 0;

        private char[] output = new char[240];
        private int rowLength = 40;


        public override string RunPart1()
        {
            foreach (var line in File.ReadAllLines(InputFile))
                ProcessInput(line);
            return signalStrength.ToString();
        }


        public override string RunPart2()
        {
            InitOutput();

            foreach (var line in File.ReadAllLines(InputFile))
                ProcessInput(line);

            var outputString = "\n";
            for(int i = 0; i < output.Length; i+= rowLength)
            {
                outputString += new string(output.Skip(i).Take(rowLength).ToArray());
                outputString += "\n";
            }

            return outputString;
        }

        private void InitOutput()
        {
            for (int i = 0; i < output.Length; i++)
                output[i] = '.';
        }


        private void ProcessInput(string line)
        {
            var parts = line.Split(' ');
            if (parts[0] == "noop")
            {
                IncreaseCycle();
            }
            else if (parts[0] == "addx")
            {
                IncreaseCycle();
                IncreaseCycle();
                xValue += int.Parse(parts[1]);
            }


        }

        private void IncreaseCycle()
        {
            RenderPixel();
            cycle++;


            if ((cycle - 20) % 40 == 0)
            {
                var signal = cycle * xValue;
                signalStrength += signal;
            }
        }

        private void RenderPixel()
        {
            var column = cycle % 40;
            if (Math.Abs(column - xValue) <= 1)
                output[cycle] = '#';
        }


    }
}