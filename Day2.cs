using System;
using System.IO;
using System.Linq;

namespace AdventOfCode2022
{

    public class Day2 : IDay
    {
        private string filename;

        public Day2(string filename)
        {
            this.filename = filename;
        }

        public string RunPart1()
        {
            var lines = File.ReadAllLines(filename);
            return lines.Sum(ComputeScore).ToString();
        }
        public string RunPart2()
        {
            var lines = File.ReadAllLines(filename);
            return lines.Sum(ComputeScorePart2).ToString();
        }

        private int ComputeScore(string inputLine)
        {
            var shapes = inputLine.Split(' ');
            var otherShape = char.Parse(shapes[0]);
            var myShape = char.Parse(shapes[1]);

            var myScore = GetScoreForShape(myShape);
            var otherScore = GetScoreForShape(otherShape);
            var winScore = GetWinScore(myScore, otherScore);

            return myScore + winScore;
        }

        private int ComputeScorePart2(string inputLine)
        {
            var shapes = inputLine.Split(' ');
            var otherShape = char.Parse(shapes[0]);
            var outcome = char.Parse(shapes[1]);


            var otherScore = GetScoreForShape(otherShape);
            var myScore = GetAnswer(otherScore, outcome);

            var winScore = GetWinScore(myScore, otherScore);
            //Console.WriteLine($"{inputLine}: {myScore} vs. {otherScore} = {winScore}");
            return myScore + winScore;
        }

        private int GetAnswer(int otherScore, char outcome)
        {
            switch (outcome)
            {
                case 'X':
                    var loseAnswer = otherScore - 1;
                    if (loseAnswer == 0) loseAnswer = 3;
                    return loseAnswer;
                case 'Y':
                    return otherScore;
                case 'Z':
                    var winAnswer = otherScore + 1;
                    if (winAnswer > 3) winAnswer = 1;
                    return winAnswer;
            }
            throw new NotSupportedException();
        }

        private int GetWinScore(int myScore, int otherScore)
        {
            if (myScore == otherScore)
                return 3;

            if (myScore - otherScore == 1 || (myScore == 1 && otherScore == 3))
                return 6;

            return 0;
        }

        private int GetScoreForShape(char shape)
        {
            switch (shape)
            {
                case 'A':
                case 'X':
                    return 1; // rock
                case 'B':
                case 'Y':
                    return 2; // paper
                case 'C':
                case 'Z':
                    return 3; // scissor

            }
            throw new NotSupportedException();
        }


    }
}