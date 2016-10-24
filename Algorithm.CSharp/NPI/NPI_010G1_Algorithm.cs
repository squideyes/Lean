using Legomatic.Trading;
using QuantConnect.Data;
using QuantConnect.Data.Consolidators;
using QuantConnect.Data.Market;
using QuantConnect.Indicators;
using QuantConnect.Parameters;
using System;
using System.Collections.Generic;

namespace QuantConnect.Algorithm.CSharp
{
    public class NPI_010G1_Algorithm : QCAlgorithm
    {
        private LegEvaluator evaluator = new LegEvaluator();
        private List<Brick> bricks = new List<Brick>();
        private int currentBrickId = 0;
        private bool exitArmed = false;
        private bool directionFilterLong = false;
        private bool directionFilterShort = false;
        private bool firstMidlineCross = false;
        private IndicatorDataPoint lastUpperBand = null;
        private IndicatorDataPoint lastMiddleBand = null;
        private IndicatorDataPoint lastLowerBand = null;

        private BollingerBands bb;
        private Symbol symbol;
        private decimal stopPrice;

        [Parameter("npi-010G1-tolerance")]
        public int Tolerance = 3;

        [Parameter("npi-010G1-ticker")]
        public string Ticker = "EURUSD";

        [Parameter("npi-010G1-ticker")]
        public DateTime FirstDate = new DateTime(2016, 1, 2);

        [Parameter("npi-010G1-bollinger-length")]
        public int BollingerLength = 20;

        [Parameter("npi-010G1-bollinger-sdv")]
        public double BollingerSDV = 1.75;

        [Parameter("npi-010G1-confirmation-bricks")]
        public int ConfirmationBricks = 2;

        [Parameter("npi-010G1-brick-size-in-ticks")]
        public decimal BrickSizeInTicks = 0.003m;

        public override void Initialize()
        {
            symbol = QuantConnect.Symbol.Create(Ticker, SecurityType.Forex, Market.FXCM);

            SetStartDate(FirstDate);
            SetEndDate(DateTime.Now.Date.AddDays(-1));

            SetCash(100000);

            AddForex(symbol.Value, Resolution.Tick);

            var consolidator = new RenkoConsolidator(BrickSizeInTicks, RenkoType.Wicked);

            consolidator.DataConsolidated += (sender, bar) =>
            {
                var brick = new Brick(currentBrickId++, bar.EndTime,
                    bar.Open, bar.High, bar.Low, bar.Close,
                    Ticker == "USDJPY" ? Decimals.Three : Decimals.Five);

                bricks.Insert(0, brick);

                evaluator.HandleTick(Tolerance, brick);

                lastUpperBand = bb.UpperBand.Current;
                lastMiddleBand = bb.MiddleBand.Current;
                lastLowerBand = bb.LowerBand.Current;

                bb.Update(bar.EndTime, bar.Close);  // Is Close right????????????

                if (!bb.IsReady)
                    return;

                //ratePerPip = (evaluator[0].BrickSizeInRate / evaluator[0].BrickSizeInPips);

                //currentBrickId = Bars.FullSymbolData.Current;

                if (bricks[0].Close > bb.MiddleBand.Current.Value)
                    directionFilterLong = false;

                if (bricks[0].Close < bb.MiddleBand.Current.Value)
                    directionFilterShort = false;

                //switch (CurrentPosition.Side)
                //{
                //    case EMarketPositionSide.Flat:
                //        HandleFlat(Bars.FullSymbolData.Current);
                //        break;
                //    case EMarketPositionSide.Long:
                //        HandleLong(Bars.FullSymbolData.Current);
                //        break;
                //    case EMarketPositionSide.Short:
                //        HandleShort(Bars.FullSymbolData.Current);
                //        break;
                //}
            };

            SubscriptionManager.AddConsolidator(symbol, consolidator);

            bb = GetBollingerBands(symbol, BollingerLength, 1,
                MovingAverageType.Simple, Resolution.Tick, Field.Close);
        }

        private BollingerBands GetBollingerBands(Symbol symbol, int period, decimal k,
            MovingAverageType movingAverageType = MovingAverageType.Simple,
            Resolution? resolution = null, Func<BaseData, decimal> selector = null)
        {
            var name = CreateIndicatorName(
                symbol, string.Format("BB({0},{1})", period, k), resolution);

            var bb = new BollingerBands(name, period, k, movingAverageType);

            RegisterIndicator(symbol, bb, resolution, selector);

            return bb;
        }

        public override void OnData(Slice data)
        {
        }

        // Improve this !!!!!!!!!!!!!!!!!!!!!!!!!!!
        private int Units(BuyOrSell buyOrSell)
        {
            const int UNITS = 100000;

            return UNITS * (int)buyOrSell;
        }

        private void Debug(string format, params object[] args)
        {
            var prefix = string.Format("CBID: {0:000000} - ", currentBrickId);

            Debug(prefix = " " + string.Format(format, args));
        }

        private void HandleLong(int brickId)
        {
            if (bricks[0].Close <= stopPrice)
            {
                var ticket = MarketOrder(
                    symbol, Units(BuyOrSell.Sell), asynchronous: false);

                Debug("Stop Exit (Long)");

                if (!firstMidlineCross)
                    directionFilterLong = true;

                return;
            }

            if (bricks[0].Close >= bb.MiddleBand.Current.Value ||
                bricks[0].High > bb.MiddleBand.Current.Value)
            {
                //stopPrice = CurrentPosition.OpenTrades[0].EntryOrder.Price;

                firstMidlineCross = true;

                Debug("Over mid-line stop price: {1:N5}", stopPrice);
            }

            if (!exitArmed && bricks[0].Close > lastUpperBand.Value)
            {
                exitArmed = true;

                return;
            }

            if (exitArmed && bricks[0].Close < bricks[1].Close)
            {
                var ticket = MarketOrder(
                    symbol, Units(BuyOrSell.Sell), asynchronous: false);

                Debug("Strategy Exit (Long)");
            }
        }

        private void HandleShort(int brickId)
        {
            if (bricks[0].Close>= stopPrice)
            {
                var ticket = MarketOrder(
                    symbol, Units(BuyOrSell.Buy), asynchronous: false);

                Debug("Stop Exit (Short)");

                if (!firstMidlineCross)
                    directionFilterShort = true;

                return;
            }

            if (bricks[0].Close <= lastMiddleBand.Value || bricks[0].Low < lastMiddleBand.Value)
            {
                //stopPrice = CurrentPosition.OpenTrades[0].EntryOrder.Price;

                firstMidlineCross = true;

                Debug("Under mid-line stop price: {1:N5}", stopPrice);
            }

            if (!exitArmed && bricks[0].Close < lastMiddleBand.Value)
            {
                exitArmed = true;

                return;
            }

            if (exitArmed && bricks[0].Close > bricks[1].Close)
            {
                var ticket = MarketOrder(
                    symbol, Units(BuyOrSell.Buy), asynchronous: false);

                Debug("Strategy Exit (Short)");
            }
        }
    }
}