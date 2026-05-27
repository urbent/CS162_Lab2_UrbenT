using System;
using System.Collections.Generic;

namespace Yahtzee
{
    class Program
    {
        const int ONES = 0;
        const int TWOS = 1;
        const int THREES = 2;
        const int FOURS = 3;
        const int FIVES = 4;
        const int SIXES = 5;
        const int THREE_OF_A_KIND = 6;
        const int FOUR_OF_A_KIND = 7;
        const int FULL_HOUSE = 8;
        const int SMALL_STRAIGHT = 9;
        const int LARGE_STRAIGHT = 10;
        const int CHANCE = 11;
        const int YAHTZEE = 12;
        const int SUBTOTAL = 13;
        const int BONUS = 14;
        const int TOTAL = 15;

        static Random rng = new Random();

        static string[] labels = { "Ones", "Twos", "Threes", "Fours", "Fives", "Sixes",
                                    "3 of a Kind", "4 of a Kind", "Full House", "Small Straight",
                                    "Large Straight", "Chance", "Yahtzee", "Sub Total", "Bonus", "Total Score" };

        static void Main(string[] args)
        {
            int[] uScorecard = new int[16];
            int[] cScorecard = new int[16];
            int uCount = 0;
            int cCount = 0;
            bool userTurn = false;

            ResetScorecard(uScorecard, ref uCount);
            ResetScorecard(cScorecard, ref cCount);

            Console.WriteLine("Welcome to Yahtzee! You vs. the Computer.");

            do
            {
                userTurn = !userTurn;
                UpdateScorecard(uScorecard);
                UpdateScorecard(cScorecard);
                DisplayScoreCards(uScorecard, cScorecard);

                if (userTurn)
                    UserPlay(uScorecard, ref uCount);
                else
                    ComputerPlay(cScorecard, ref cCount);
            }
            while (uCount <= YAHTZEE && cCount <= YAHTZEE);

            UpdateScorecard(uScorecard);
            UpdateScorecard(cScorecard);
            DisplayScoreCards(uScorecard, cScorecard);

            int uTotal = uScorecard[TOTAL];
            int cTotal = cScorecard[TOTAL];
            if (uTotal > cTotal)
                Console.WriteLine($"You win! {uTotal} to {cTotal}.");
            else if (cTotal > uTotal)
                Console.WriteLine($"Computer wins! {cTotal} to {uTotal}.");
            else
                Console.WriteLine($"It's a tie! {uTotal} each.");

            Console.ReadLine();
        }

        static void ResetScorecard(int[] scorecard, ref int count)
        {
            for (int i = 0; i < scorecard.Length; i++)
                scorecard[i] = -1;
            count = 0;
        }

        static void UpdateScorecard(int[] scorecard)
        {
            scorecard[SUBTOTAL] = 0;
            scorecard[BONUS] = 0;
            for (int i = ONES; i <= SIXES; i++)
                if (scorecard[i] != -1)
                    scorecard[SUBTOTAL] += scorecard[i];

            if (scorecard[SUBTOTAL] >= 63)
                scorecard[BONUS] = 35;

            scorecard[TOTAL] = scorecard[SUBTOTAL] + scorecard[BONUS];
            for (int i = THREE_OF_A_KIND; i <= YAHTZEE; i++)
                if (scorecard[i] != -1)
                    scorecard[TOTAL] += scorecard[i];
        }

        static string FormatCell(int value)
        {
            return (value < 0) ? "" : value.ToString();
        }

        static void DisplayScoreCards(int[] uScorecard, int[] cScorecard)
        {
            Console.WriteLine($"{"Category",-18} {"You",6} {"Computer",10}");
            for (int i = ONES; i <= SIXES; i++)
                Console.WriteLine($"{i,2}. {labels[i],-15} {FormatCell(uScorecard[i]),6} {FormatCell(cScorecard[i]),10}");
            Console.WriteLine($"{"Sub Total",-18} {FormatCell(uScorecard[SUBTOTAL]),6} {FormatCell(cScorecard[SUBTOTAL]),10}");
            Console.WriteLine($"{"Bonus",-18} {FormatCell(uScorecard[BONUS]),6} {FormatCell(cScorecard[BONUS]),10}");
            for (int i = THREE_OF_A_KIND; i <= YAHTZEE; i++)
                Console.WriteLine($"{i,2}. {labels[i],-15} {FormatCell(uScorecard[i]),6} {FormatCell(cScorecard[i]),10}");
            Console.WriteLine($"{"Total Score",-18} {FormatCell(uScorecard[TOTAL]),6} {FormatCell(cScorecard[TOTAL]),10}");
        }

        static void Roll(int numDice, List<int> dice)
        {
            for (int i = 0; i < numDice; i++)
                dice.Add(rng.Next(1, 7));
        }

        static void DisplayDice(List<int> dice)
        {
            Console.WriteLine(string.Join(" ", dice));
        }

        static int GetComputerScorecardItem(int[] scorecard, List<int> keeping)
        {
            int indexOfMax = 0;
            int max = 0;

            for (int i = ONES; i <= YAHTZEE; i++)
            {
                if (scorecard[i] == -1)
                {
                    int score = Score(i, keeping);
                    if (score >= max)
                    {
                        max = score;
                        indexOfMax = i;
                    }
                }
            }

            return indexOfMax;
        }

        static void ComputerPlay(int[] cScorecard, ref int cScorecardCount)
        {
            List<int> keeping = new List<int>();

            Roll(5, keeping);
            Console.Write("Computer rolled: ");
            DisplayDice(keeping);

            int itemIndex = GetComputerScorecardItem(cScorecard, keeping);
            cScorecard[itemIndex] = Score(itemIndex, keeping);
            cScorecardCount++;

            Console.WriteLine($"Computer scored {cScorecard[itemIndex]} in {labels[itemIndex]}.");
        }

        static void GetKeeping(List<int> rolling, List<int> keeping)
        {
            Console.Write("Keep which dice? Enter positions (e.g. 1 3 5) or Enter for none: ");
            string input = Console.ReadLine()?.Trim() ?? "";

            List<int> toKeep = new List<int>();
            foreach (string token in input.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                if (int.TryParse(token, out int pos) && pos >= 1 && pos <= rolling.Count)
                    toKeep.Add(pos - 1);

            toKeep.Sort();
            for (int i = toKeep.Count - 1; i >= 0; i--)
            {
                keeping.Add(rolling[toKeep[i]]);
                rolling.RemoveAt(toKeep[i]);
            }
        }

        static void MoveRollToKeep(List<int> rolling, List<int> keeping)
        {
            foreach (int d in rolling)
                keeping.Add(d);
            rolling.Clear();
        }

        static int GetScorecardItem(int[] scorecard)
        {
            Console.WriteLine("Available categories:");
            for (int i = ONES; i <= YAHTZEE; i++)
                if (scorecard[i] == -1)
                    Console.WriteLine($"  {i,2}. {labels[i]}");

            while (true)
            {
                Console.Write("Choose a category number: ");
                string input = Console.ReadLine()?.Trim() ?? "";
                if (int.TryParse(input, out int choice) && choice >= ONES && choice <= YAHTZEE)
                {
                    if (scorecard[choice] == -1)
                        return choice;
                    Console.WriteLine("Already used. Pick another.");
                }
                else
                {
                    Console.WriteLine("Invalid. Enter a number 0-12.");
                }
            }
        }

        static void UserPlay(int[] uScorecard, ref int uScorecardCount)
        {
            List<int> rolling = new List<int>();
            List<int> keeping = new List<int>();
            int numRolls = 0;

            do
            {
                Roll(5 - keeping.Count, rolling);
                numRolls++;

                Console.Write($"Roll {numRolls}: ");
                DisplayDice(rolling);

                if (keeping.Count > 0)
                {
                    Console.Write("Keeping: ");
                    DisplayDice(keeping);
                }

                if (numRolls < 3)
                    GetKeeping(rolling, keeping);
                else
                    MoveRollToKeep(rolling, keeping);
            }
            while (numRolls < 3 && keeping.Count < 5);

            if (rolling.Count > 0)
                MoveRollToKeep(rolling, keeping);

            Console.Write("Final dice: ");
            DisplayDice(keeping);

            int item = GetScorecardItem(uScorecard);
            int scored = Score(item, keeping);
            uScorecard[item] = scored;
            uScorecardCount++;

            Console.WriteLine($"You scored {scored} in {labels[item]}.");
        }

        static int Count(int value, List<int> dice)
        {
            int count = 0;
            foreach (int d in dice)
                if (d == value) count++;
            return count;
        }

        static int[] GetCounts(List<int> dice)
        {
            int[] counts = new int[6];
            for (int i = ONES; i <= SIXES; i++)
                counts[i] = Count(i + 1, dice);
            return counts;
        }

        static int Sum(int[] counts)
        {
            int sum = 0;
            for (int i = ONES; i <= SIXES; i++)
                sum += (i + 1) * counts[i];
            return sum;
        }

        static bool HasCount(int howMany, int[] counts)
        {
            foreach (int count in counts)
                if (howMany == count)
                    return true;
            return false;
        }

        static int ScoreOnes(int[] counts) => counts[ONES] * 1;
        static int ScoreTwos(int[] counts) => counts[TWOS] * 2;
        static int ScoreThrees(int[] counts) => counts[THREES] * 3;
        static int ScoreFours(int[] counts) => counts[FOURS] * 4;
        static int ScoreFives(int[] counts) => counts[FIVES] * 5;
        static int ScoreSixes(int[] counts) => counts[SIXES] * 6;
        static int ScoreChance(int[] counts) => Sum(counts);

        static int ScoreThreeOfAKind(int[] counts)
        {
            foreach (int c in counts)
                if (c >= 3) return Sum(counts);
            return 0;
        }

        static int ScoreFourOfAKind(int[] counts)
        {
            foreach (int c in counts)
                if (c >= 4) return Sum(counts);
            return 0;
        }

        static int ScoreYahtzee(int[] counts)
        {
            foreach (int c in counts)
                if (c == 5) return 50;
            return 0;
        }

        static int ScoreFullHouse(int[] counts)
        {
            if (HasCount(2, counts) && HasCount(3, counts))
                return 25;
            return 0;
        }

        static int ScoreSmallStraight(int[] counts)
        {
            for (int i = THREES; i <= FOURS; i++)
                if (counts[i] == 0) return 0;
            if ((counts[ONES] >= 1 && counts[TWOS] >= 1) ||
                (counts[TWOS] >= 1 && counts[FIVES] >= 1) ||
                (counts[FIVES] >= 1 && counts[SIXES] >= 1))
                return 30;
            return 0;
        }

        static int ScoreLargeStraight(int[] counts)
        {
            for (int i = TWOS; i <= FIVES; i++)
                if (counts[i] == 0) return 0;
            if (counts[ONES] == 1 || counts[SIXES] == 1)
                return 40;
            return 0;
        }

        static int Score(int whichElement, List<int> dice)
        {
            int[] counts = GetCounts(dice);
            switch (whichElement)
            {
                case ONES: return ScoreOnes(counts);
                case TWOS: return ScoreTwos(counts);
                case THREES: return ScoreThrees(counts);
                case FOURS: return ScoreFours(counts);
                case FIVES: return ScoreFives(counts);
                case SIXES: return ScoreSixes(counts);
                case THREE_OF_A_KIND: return ScoreThreeOfAKind(counts);
                case FOUR_OF_A_KIND: return ScoreFourOfAKind(counts);
                case FULL_HOUSE: return ScoreFullHouse(counts);
                case SMALL_STRAIGHT: return ScoreSmallStraight(counts);
                case LARGE_STRAIGHT: return ScoreLargeStraight(counts);
                case CHANCE: return ScoreChance(counts);
                case YAHTZEE: return ScoreYahtzee(counts);
                default: return 0;
            }
        }
    }
}