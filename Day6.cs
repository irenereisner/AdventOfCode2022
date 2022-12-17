using System;
using System.IO;
using System.Linq;

namespace AdventOfCode2022
{
    [Day(6)]
    public class Day6 : Day
    {
        private int markerLength = 4;

        public override string RunPart1()
        {
            markerLength = 4;
            var input = File.ReadAllText(InputFile);
            return GetMarkerPosition(input).ToString();
        }


        public override string RunPart2()
        {
            markerLength = 14;
            var input = File.ReadAllText(InputFile);
            return GetMarkerPosition(input).ToString();
        }

        private int GetMarkerPosition(string input)
        {
            for(int i = markerLength; i < input.Length; i++)
            {
                var possibleMarker = input.Substring(i - markerLength, markerLength);
                if (IsMarker(possibleMarker))
                    return i;
            }
            throw new NotSupportedException("No marker found");
        }

        private bool IsMarker(string text)
        {
            return text.Distinct().Count() == text.Length;
        }
    }
}