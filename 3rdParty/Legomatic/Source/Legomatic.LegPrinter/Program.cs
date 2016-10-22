using Legomatic.Trading;
using System;
using System.Collections.Generic;
using System.IO;

namespace Legomatic.LegPrinter
{
    class Program
    {
        static void Main(string[] args)
        {
            const string FILEPATH = "EURUSD_20150101_20151224_BAR.csv";

            var evaluator = new LegEvaluator();

            var bricks = GetBricks(FILEPATH);

            foreach (var brick in bricks)
                evaluator.HandleTick(1, brick);

            int count = 0;

            for (int i = evaluator.Count - 1; i >= 0; i--)
                Console.WriteLine($"Leg {++count:000} - {evaluator[i]}");

            Console.WriteLine();
            Console.Write("Press any key to terminate...");

            Console.ReadKey(true);
        }

        private static List<Brick> GetBricks(string filePath)
        {
            var bricks = new List<Brick>();

            using (var reader = new StreamReader(filePath))
            {
                string line;

                int id = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(',');

                    var closeOn = DateTime.Parse(parts[0]);
                    var open = double.Parse(parts[1]);
                    var high = double.Parse(parts[2]);
                    var low = double.Parse(parts[3]);
                    var close = double.Parse(parts[4]);

                    bricks.Add(new Brick(++id, closeOn, open, high, low, close, Decimals.Five));
                }
            }

            return bricks;
        }
    }
}
