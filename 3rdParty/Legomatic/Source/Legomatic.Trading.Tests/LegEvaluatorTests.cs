using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Legomatic.Trading.Tests
{
    [TestClass]
    public class LegEvaluatorTests
    {
        private const int TOLERANCE = 2;

        private List<Brick> GetBullBricks()
        {
            return new List<Brick>()
            {
                new Brick(1, DateTime.Parse("8/7/2015 10:40:08"), 1.08971m, 1.09271m, 1.08568m, 1.09271m, Decimals.Five),
                new Brick(2, DateTime.Parse("8/7/2015 10:51:42"), 1.09271m, 1.09571m, 1.09242m, 1.09571m, Decimals.Five),
                new Brick(3, DateTime.Parse("8/10/2015 11:26:27"), 1.09571m, 1.09871m, 1.09266m, 1.09871m, Decimals.Five),
                new Brick(4, DateTime.Parse("8/10/2015 12:39:32"), 1.09871m, 1.10171m, 1.09855m, 1.10171m, Decimals.Five),
                new Brick(5, DateTime.Parse("8/11/2015 04:14:19"), 1.10171m, 1.10471m, 1.09620m, 1.10471m, Decimals.Five),
                new Brick(6, DateTime.Parse("8/11/2015 09:11:27"), 1.10471m, 1.10771m, 1.10207m, 1.10771m, Decimals.Five),
                new Brick(7, DateTime.Parse("8/11/2015 12:49:58"), 1.10471m, 1.10895m, 1.10171m, 1.10171m, Decimals.Five),
                new Brick(8, DateTime.Parse("8/12/2015 01:05:29"), 1.10471m, 1.10771m, 1.10117m, 1.10771m, Decimals.Five),
                new Brick(9, DateTime.Parse("8/12/2015 03:36:20"), 1.10771m, 1.11071m, 1.10712m, 1.11071m, Decimals.Five),
                new Brick(10, DateTime.Parse("8/12/2015 04:08:00"), 1.11071m, 1.11371m, 1.11054m, 1.11371m, Decimals.Five),
                new Brick(11, DateTime.Parse("8/12/2015 08:20:11"), 1.11371m, 1.11671m, 1.11249m, 1.11671m, Decimals.Five),
                new Brick(12, DateTime.Parse("8/12/2015 10:51:41"), 1.11671m, 1.11971m, 1.11427m, 1.11971m, Decimals.Five),
                new Brick(13, DateTime.Parse("8/12/2015 22:44:21"), 1.11671m, 1.12149m, 1.11371m, 1.11371m, Decimals.Five),
                new Brick(14, DateTime.Parse("8/13/2015 07:30:10"), 1.11371m, 1.11542m, 1.11071m, 1.11071m, Decimals.Five),
                new Brick(15, DateTime.Parse("8/13/2015 16:14:04"), 1.11371m, 1.11671m, 1.10812m, 1.11671m, Decimals.Five),
                new Brick(16, DateTime.Parse("8/14/2015 10:58:06"), 1.11371m, 1.11899m, 1.11071m, 1.11071m, Decimals.Five),
                new Brick(17, DateTime.Parse("8/17/2015 03:05:31"), 1.11071m, 1.11405m, 1.10771m, 1.10771m, Decimals.Five),
                new Brick(18, DateTime.Parse("8/18/2015 08:36:00"), 1.10771m, 1.11261m, 1.10471m, 1.10471m, Decimals.Five)
            };
        }

        private List<Brick> GetBearBricks()
        {
            return new List<Brick>()
            {
                new Brick(1, DateTime.Parse("10/15/2015,30:46.00"),1.144m,1.15m,1.141m,1.141m, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/15/2015,43:57.30"),1.141,1.142,1.138,1.138, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/16/2015,17:31.10"),1.138,1.142,1.135,1.135, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/19/2015,37:56.70"),1.135,1.139,1.132,1.132, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/20/2015,42:38.70"),1.135,1.138,1.131,1.138, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/22/2015,17:07.00"),1.135,1.139,1.132,1.132, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/22/2015,28:37.20"),1.132,1.133,1.129,1.129, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/22/2015,29:24.20"),1.129,1.129,1.126,1.126, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/22/2015,30:08.00"),1.126,1.126,1.123,1.123, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/22/2015,49:29.00"),1.123,1.127,1.12,1.12, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/22/2015,21:03.00"),1.12,1.121,1.117,1.117, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/22/2015,56:30.70"),1.117,1.119,1.114,1.114, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/22/2015,48:50.40"),1.114,1.115,1.111,1.111, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/22/2015,38:56.60"),1.111,1.112,1.108,1.108, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/23/2015,26:08.20"),1.108,1.114,1.105,1.105, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/23/2015,54:00.00"),1.105,1.106,1.102,1.102, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/28/2015,40:23.50"),1.105,1.108,1.1,1.108, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/28/2015,00:07.80"),1.105,1.11,1.102,1.102, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/28/2015,02:07.90"),1.102,1.103,1.099,1.099, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/28/2015,06:59.90"),1.099,1.1,1.096,1.096, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/28/2015,10:05.70"),1.096,1.097,1.093,1.093, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/28/2015,48:49.40"),1.093,1.096,1.09,1.09, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/29/2015,15:31.60"),1.093,1.096,1.09,1.096, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/29/2015,44:19.70"),1.096,1.099,1.092,1.099, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/30/2015,30:13.80"),1.099,1.102,1.097,1.102, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/30/2015,06:45.50"),1.102,1.105,1.1,1.105, Decimals.Five),
                //new Brick(1, DateTime.Parse("10/30/2015,18:11.50"),1.102,1.107,1.099,1.099, Decimals.Five),
                //new Brick(1, DateTime.Parse("11/03/2015,28:29.20"),1.099,1.105,1.096,1.096, Decimals.Five),
                //new Brick(1, DateTime.Parse("11/04/2015,10:39.60"),1.096,1.097,1.093,1.093, Decimals.Five),
                //new Brick(1, DateTime.Parse("11/04/2015,57:37.20"),1.093,1.094,1.09,1.09, Decimals.Five),
                //new Brick(1, DateTime.Parse("11/04/2015,36:42.50"),1.09,1.091,1.087,1.087, Decimals.Five),
                //new Brick(1, DateTime.Parse("11/04/2015,49:49.20"),1.087,1.09,1.084,1.084, Decimals.Five),
                //new Brick(1, DateTime.Parse("11/06/2015,29:57.60"),1.084,1.09,1.081,1.081, Decimals.Five),
                //new Brick(1, DateTime.Parse("11/06/2015,30:03.10"),1.081,1.081,1.078,1.078, Decimals.Five),
                //new Brick(1, DateTime.Parse("11/06/2015,30:09.60"),1.078,1.078,1.075,1.075, Decimals.Five),
                //new Brick(1, DateTime.Parse("11/06/2015,30:28.40"),1.075,1.076,1.072,1.072, Decimals.Five),
                //new Brick(1, DateTime.Parse("11/09/2015,38:14.60"),1.075,1.078,1.071,1.078, Decimals.Five)
            };
        }

        private LegEvaluator GetStandardTwoLegEvaluator()
        {
            var bricks = GetBullBricks();

            var evaluator = new LegEvaluator();

            foreach (var brick in bricks)
                evaluator.HandleTick(TOLERANCE, brick);

            return evaluator;
        }

        [TestMethod]
        public void LegEvaluatorTests_LoadDataFromFileTest()
        {
            var bricks = GetBricks();

            var evaluator = new LegEvaluator();

            foreach (var brick in bricks)
                evaluator.HandleTick(1, brick);

            Assert.AreEqual(evaluator.Count, 1361);
        }

        private List<Brick> GetBricks()
        {
            var bricks = new List<Brick>();

            using (var reader = new StreamReader(@"Data\EURUSD.csv"))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(',');

                    var id = int.Parse(parts[0]);
                    var closeOn = DateTime.Parse(parts[1]);
                    var open = decimal.Parse(parts[2]);
                    var high = decimal.Parse(parts[3]);
                    var low = decimal.Parse(parts[4]);
                    var close = decimal.Parse(parts[5]);

                    bricks.Add(new Brick(id, closeOn, open, high, low, close, Decimals.Five));
                }
            }

            return bricks;
        }

        [TestMethod]
        public void LegEvaluatorTests_BearTest()
        {
            const int TOLERANCE = 0;

            var evaluator = new LegEvaluator();

            foreach (var brick in GetBearBricks())
                evaluator.HandleTick(TOLERANCE, brick);
        }

        [TestMethod]
        public void LegEvaluatorTests_ToleranceZeroWorks()
        {
            const int TOLERANCE = 0;

            var evaluator = new LegEvaluator();

            foreach (var brick in GetBullBricks())
                evaluator.HandleTick(TOLERANCE, brick);

            Assert.IsTrue(evaluator.Evaluate(0, legs => legs[0].Count == 3));
            Assert.IsTrue(evaluator.Evaluate(1, legs => legs[1].Count == 1));
            Assert.IsTrue(evaluator.Evaluate(2, legs => legs[2].Count == 2));
            Assert.IsTrue(evaluator.Evaluate(3, legs => legs[3].Count == 5));
        }

        [TestMethod]
        public void LegEvaluatorTests_OpenCanBeParsed()
        {
            var evaluator = new LegEvaluator();

            evaluator.HandleTick(TOLERANCE,
                new Brick(1, DateTime.Now, 1.36871m, 1.37175m, 1.36640m, 1.37171m, Decimals.Five));

            Assert.IsTrue(evaluator.Evaluate(0, legs => legs[0].Ohlc.Open == 1.36871m));
        }

        [TestMethod]
        public void LegEvaluatorTests_HighCanBeParsed()
        {
            var evaluator = new LegEvaluator();

            evaluator.HandleTick(TOLERANCE,
                new Brick(1, DateTime.Now, 1.36871m, 1.37175m, 1.36640m, 1.37171m, Decimals.Five));

            Assert.IsTrue(evaluator.Evaluate(0, legs => legs[0].Ohlc.High == 1.37175m));
        }

        [TestMethod]
        public void LegEvaluatorTests_LowCanBeParsed()
        {
            var evaluator = new LegEvaluator();

            evaluator.HandleTick(TOLERANCE,
                new Brick(1, DateTime.Now, 1.36871m, 1.37175m, 1.36640m, 1.37171m, Decimals.Five));

            Assert.IsTrue(evaluator.Evaluate(0, legs => legs[0].Ohlc.Low == 1.36640m));
        }

        [TestMethod]
        public void LegEvaluatorTests_CloseCanBeParsed()
        {
            var evaluator = new LegEvaluator();

            evaluator.HandleTick(TOLERANCE,
                new Brick(1, DateTime.Now, 1.36871m, 1.37175m, 1.36640m, 1.37171m, Decimals.Five));

            Assert.IsTrue(evaluator.Evaluate(0, legs => legs[0].Ohlc.Close == 1.37171m));
        }

        [TestMethod]
        public void LegEvaluatorTests_LegCountWorks()
        {
            var evaluator = GetStandardTwoLegEvaluator();

            Assert.AreEqual(evaluator.Count, 2);
        }

        [TestMethod]
        public void LegEvaluatorTests_GoodIndexWorks()
        {
            var evaluator = GetStandardTwoLegEvaluator();

            var leg = evaluator[1];
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void LegEvaluatorTests_BadIndexFails()
        {
            var evaluator = GetStandardTwoLegEvaluator();

            var leg = evaluator[2];
        }

        [TestMethod]
        public void LegEvaluatorTests_TrendCanBeParsed()
        {
            var evaluator = new LegEvaluator();

            evaluator.HandleTick(TOLERANCE,
                new Brick(1, DateTime.Now, 1.36871m, 1.37175m, 1.36640m, 1.37171m, Decimals.Five));

            Assert.IsTrue(evaluator.Evaluate(0, legs => legs[0].Trend == Trend.Bull));
            Assert.IsTrue(evaluator.Evaluate(0, legs => legs[0].Trend != Trend.Bear));
        }

        [TestMethod]
        public void LegEvaluatorTests_ExtremeInClosedLeg()
        {
            var evaluator = GetStandardTwoLegEvaluator();

            Assert.IsTrue(evaluator.Evaluate(1, legs => legs[1].Extreme == 1.12149m));
        }

        [TestMethod]
        public void LegEvaluatorTests_ExtremeInOpenLeg()
        {
            var evaluator = new LegEvaluator();

            evaluator.HandleTick(TOLERANCE,
                new Brick(1, DateTime.Parse("8/7/2015 10:40:08"), 1.08971m, 1.09271m, 1.08568m, 1.09271m, Decimals.Five));
            evaluator.HandleTick(TOLERANCE,
                new Brick(2, DateTime.Parse("8/7/2015 10:51:42"), 1.09271m, 1.09571m, 1.09242m, 1.09571m, Decimals.Five));
            evaluator.HandleTick(TOLERANCE,
                new Brick(3, DateTime.Parse("8/10/2015 11:26:27"), 1.09571m, 1.09871m, 1.09266m, 1.09871m, Decimals.Five));
            evaluator.HandleTick(TOLERANCE,
                new Brick(4, DateTime.Parse("8/10/2015 12:39:32"), 1.09871m, 1.10171m, 1.09855m, 1.10171m, Decimals.Five));

            Assert.IsTrue(evaluator.Evaluate(0, legs => legs[0].Extreme == 1.10171m));
        }

        [TestMethod]
        public void LegEvaluatorTests_LegParsingWorks()
        {
            var evaluator = new LegEvaluator();

            foreach (var brick in GetBullBricks())
                evaluator.HandleTick(TOLERANCE, brick);

            Assert.IsTrue(evaluator.Evaluate(1, legs => legs[1].Count == 12 && legs[0].Count == 6));
        }

        [TestMethod]
        public void LegEvaluatorTests_TrendBricksAndCounterTrendBricksCanBeParsed()
        {
            var evaluator = new LegEvaluator();

            foreach (var brick in GetBullBricks())
                evaluator.HandleTick(TOLERANCE, brick);

            Assert.IsTrue(evaluator.Evaluate(1, legs =>
                legs[0].TrendBricks == 5 && legs[1].TrendBricks == 11 &&
                legs[0].CounterTrendBricks == 1 && legs[1].CounterTrendBricks == 1));
        }

        [TestMethod]
        public void LegEvaluatorTests_FullMoveInRateCanBeParsed()
        {
            var evaluator = new LegEvaluator();

            foreach (var brick in GetBullBricks())
                evaluator.HandleTick(TOLERANCE, brick);

            Assert.IsTrue(evaluator.Evaluate(1, legs => legs[1].FullMoveInRate == 0.03581m));
        }

        [TestMethod]
        public void LegEvaluatorTests_IgnoreBadIndexTest()
        {
            var evaluator = new LegEvaluator();

            foreach (var brick in GetBullBricks())
                evaluator.HandleTick(TOLERANCE, brick);

            Assert.IsFalse(evaluator.Evaluate(5, legs => legs[5].Count == 99));
            Assert.IsFalse(evaluator.Evaluate(1, legs => legs[1].Count == 5));
        }

        [TestMethod]
        public void LegEvaluatorTests_FullMoveInPipsCanBeParsed()
        {
            var evaluator = new LegEvaluator();

            foreach (var brick in GetBullBricks())
                evaluator.HandleTick(TOLERANCE, brick);

            Assert.IsTrue(evaluator.Evaluate(1, legs => legs[1].FullMoveInPips == 358.1m));
        }

        [TestMethod]
        public void LegEvaluatorTests_FullMoveInBricksCanBeParsed()
        {
            var evaluator = new LegEvaluator();

            foreach (var brick in GetBullBricks())
                evaluator.HandleTick(TOLERANCE, brick);

            Assert.IsTrue(evaluator.Evaluate(1, legs => legs[1].FullMoveInBricks == 11.93667m));
        }

        [TestMethod]
        public void LegEvaluatorTests_RenkoMoveInRateCanBeParsed()
        {
            var evaluator = new LegEvaluator();

            foreach (var brick in GetBullBricks())
                evaluator.HandleTick(TOLERANCE, brick);

            Assert.IsTrue(evaluator.Evaluate(1, legs => legs[1].RenkoMoveInRate == 0.03m));
        }

        [TestMethod]
        public void LegEvaluatorTests_RenkoMoveInPipsCanBeParsed()
        {
            var evaluator = new LegEvaluator();

            foreach (var brick in GetBullBricks())
                evaluator.HandleTick(TOLERANCE, brick);

            Assert.IsTrue(evaluator.Evaluate(1, legs => legs[1].RenkoMoveInPips == 300.0m));
        }

        [TestMethod]
        public void LegEvaluatorTests_RenkoMoveInBricksCanBeParsed()
        {
            var evaluator = new LegEvaluator();

            foreach (var brick in GetBullBricks())
                evaluator.HandleTick(TOLERANCE, brick);

            Assert.IsTrue(evaluator.Evaluate(1, legs => legs[1].RenkoMoveInBricks == 10.0m));
        }

        [TestMethod]
        public void LegEvaluatorTests_EfficiencyCanBeParsed()
        {
            var evaluator = new LegEvaluator();

            foreach (var brick in GetBullBricks())
                evaluator.HandleTick(TOLERANCE, brick);

            Assert.IsTrue(evaluator.Evaluate(1, legs => legs[1].Efficiency == 917));
        }

        [TestMethod]
        public void LegEvaluatorTests_BetweenTest()
        {
            var evaluator = new LegEvaluator();

            foreach (var brick in GetBullBricks())
                evaluator.HandleTick(TOLERANCE, brick);

            Assert.IsTrue(evaluator.Evaluate(1, legs =>
                legs[1].Efficiency >= 900 && legs[1].Efficiency <= 1000));
        }

        [TestMethod]
        public void LegEvaluatorTests_WavesFormCorrectly()
        {
            var factory = new LegFactory();

            var legs = new List<Leg>();

            factory.OnLeg += (s, e) =>
            {
                if ((legs.Count == 0) || (e.InsertLeg))
                    legs.Insert(0, e.Leg);
                else
                    legs[0] = e.Leg;
            };

            foreach (var brick in GetBullBricks())
                factory.HandleTick(brick, 2);

            var waves = legs[0].GetWaves(0);

            Assert.AreEqual(waves[0].Count, 2);
            Assert.AreEqual(waves[1].Count, 1);
            Assert.AreEqual(waves[2].Count, 3);
        }
    }
}
