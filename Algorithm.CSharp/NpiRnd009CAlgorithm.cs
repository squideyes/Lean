/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using Legomatic.Trading;
using QuantConnect.Data;
using QuantConnect.Data.Consolidators;
using QuantConnect.Data.Market;
using QuantConnect.Indicators;
using System;
using System.Collections.Generic;

namespace QuantConnect.Algorithm.CSharp
{
    /// <summary>
    /// NPI Rnd009C algorithm
    /// </summary>
    public class NpiRnd009CAlgorithm : QCAlgorithm
    {
        private Symbol _eurusd = QuantConnect.Symbol.Create(
            "EURUSD", SecurityType.Forex, Market.FXCM);

        private const int TOLERANCE = 3;

        private double CURRENCYSCALE = 0.0001;
        private int CURRENCYDIGITS = 5;
        private int currentBrickId = -1;
        private int BollingerLength = 20;

        private BollingerBands bb;
        private LegEvaluator evaluator;
        private double brickSize;
        private double ratePerPip;
        private int tradeEnterBar;
        private bool midLineExitArmed = false;

        // make these soft
        public double BrickToArmMidlineExit = 4;
        public double BricksToEnter = 6;

        private List<RenkoBar> bars = new List<RenkoBar>();

        /// <summary>
        /// Initialise the data and resolution required, as well as the cash and start-end dates for your algorithm. All algorithms must initialized.
        /// </summary>
        public override void Initialize()
        {
            SetStartDate(2015, 1, 1);  //Set Start Date
            SetEndDate(2016, 08, 31);    //Set End Date
            SetCash(100000);             //Set Strategy Cash

            AddForex("EURUSD", Resolution.Tick);

            var consolidator = new RenkoConsolidator(30m, RenkoType.Wicked);

            consolidator.DataConsolidated += (sender, consolidated) =>
            {
                bars.Insert(0, consolidated);
            };

            SubscriptionManager.AddConsolidator(_eurusd, consolidator);

            bb = GetBollingerBands(_eurusd, BollingerLength, 1,
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

        /// <summary>
        /// OnData event is the primary entry point for your algorithm. Each new data point will be pumped in here.
        /// </summary>
        /// <param name="data">Slice object keyed by symbol containing the stock data</param>
        public override void OnData(Slice data)
        {
            //if (!Portfolio.Invested)
            //{
            //    SetHoldings(_eurusd, 1);

            //    Debug("Purchased Stock");
            //}
        }


        private void HandleLong(int brickId)
        {
            //Print("Long Midline BrickId: {0}  tradeEnterBar: {1},  Bar.close[0]: {2:N5}  MidBand {3:N5}",
            //      currentBrickId, tradeEnterBar, Bars.Close[0], m_MidBand[0]);

            //Strategy Exit  armed
            if (!midLineExitArmed && Positions[0].OpenTrades[0].EntryOrder.Price + 
                (brickSize * BrickToArmMidlineExit) <= bars[0].Close)
            {
                midLineExitArmed = true;

                //Print("Long Midline Armed");

                return;
            }

            if (midLineExitArmed)
            {
                //Print("Long Midline Evaluate");

                if (bars[0].Close < bb.MiddleBand)
                {
                    //sellOrderMarket.Send("9BLXmid");
                    //Print("*Exit Long MidLine");
                }
                else
                {
                    return;
                }
            }


            int waveBars = evaluator[0].Count;

            //Print("waveBars:{0} ", waveBars);

            if (evaluator[0][waveBars - 1].Trend == Trend.Bear 
                && evaluator[0][waveBars - 2].Trend == Trend.Bear
                || (evaluator[0][waveBars - 1].Trend == Trend.Bear 
                && evaluator[0][waveBars - 2].Trend == Trend.Bull
                && evaluator[0][waveBars - 3].Trend == Trend.Bear 
                && evaluator[0][waveBars - 4].Trend == Trend.Bull))
            {
                //sellOrderMarket.Send("9BLX");

                //Print("*Exit Long");

            }
        }
    }
}

//﻿using Legomatic.Trading;
//using PowerLanguage.Function;
//using System;
//using System.Text;
//using System.Linq;

////Entry after n Bars of 100% effeciency.
////Exit Stop  - 2 bar reversal
////Exit Strategy - 2 bar reversal or up/down up/down last 4 bars.
////Exit Strategy - after n bars exit on Bollinger Band Mid line cross

//namespace PowerLanguage.Strategy
//{
//    public class a_RnD009C : SignalObject
//    {

//        private ISeries<double> Price { get; set; }





//        protected override void Create()
//        {

//            //Long entry/Exit orders
//            buyOrder = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, "BEMarket", EOrderAction.Buy));
//            sellOrderLimit = OrderCreator.Limit(new SOrderParameters(Contracts.Default, "SLimit", EOrderAction.Sell));
//            sellOrderMarket = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, "SMarket", EOrderAction.Sell));

//            //Short entry/Exit orders
//            sellShort = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, "SEMarket", EOrderAction.SellShort));
//            buyToCoverMarket = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, "BTCMarket", EOrderAction.BuyToCover));
//            buyToCoverLimit = OrderCreator.Limit(new SOrderParameters(Contracts.Default, "BTCLimit", EOrderAction.BuyToCover));

//            m_MidBand = new VariableSeries<Double>(this);

//        }


//        protected override void StartCalc()
//        {
//            evaluator = new LegEvaluator();

//            // assign inputs 
//            Price = Bars.Close;


//            if (Bars.Request.Symbol.Substring(3, 3) == "JPY")
//            {
//                CURRENCYSCALE = 0.01;
//                CURRENCYDIGITS = 3;
//            }

//            brickSize = Math.Round(Math.Abs(Bars.Open[2] - Bars.Close[2]), CURRENCYDIGITS);


//            //Print("CurrencyScale={0}  BrickSize={1}", CURRENCYSCALE.ToString(), brickSize.ToString());
//            //Print("Tick: {0}  MinMove: {1}  PriceScale: {2}", Bars.Info.MinMove / Bars.Info.PriceScale, Bars.Info.MinMove, Bars.Info.PriceScale);
//            //Print("BigPoint={0:N5}  ", Bars.Info.BigPointValue);
//            //Print();
//        }

//        protected override void CalcBar()
//        {
//            if (Bars.FullSymbolData.Current != currentBrickId)
//            {
//                var brick = new Brick(Bars.FullSymbolData.Current,
//                    Bars.FullSymbolData.Time[0],
//                    Math.Round(Bars.FullSymbolData.Open[0], 5),
//                    Math.Round(Bars.FullSymbolData.High[0], 5),
//                    Math.Round(Bars.FullSymbolData.Low[0], 5),
//                    Math.Round(Bars.FullSymbolData.Close[0], 5), Decimals.Five);


//                evaluator.HandleTick(TOLERANCE, brick);

//                ratePerPip = (evaluator[0].BrickSizeInRate / evaluator[0].BrickSizeInPips);

//                currentBrickId = Bars.FullSymbolData.Current;
//                m_MidBand.Value = Bars.Close.BollingerBandCustom(20, 0);
//                //PrintRenkoMoveInBricks();

//            }
//            //avg.Value = averagefc1[1];
//            //Print("MA brickId: {0}    avg.Value: {1:N5}   averagefc1: {2:N5} ", currentBrickId, avg.Value, averagefc1[1]);


//            switch (CurrentPosition.Side)
//            {
//                case EMarketPositionSide.Flat:
//                    HandleFlat(Bars.FullSymbolData.Current);
//                    break;
//                case EMarketPositionSide.Long:
//                    HandleLong(Bars.FullSymbolData.Current);
//                    break;
//                case EMarketPositionSide.Short:
//                    HandleShort(Bars.FullSymbolData.Current);
//                    break;
//            }

//        }



//        private void HandleShort(int brickId)
//        {

//            Print("Short Midline BrickId: {0}  tradeEnterBar: {1},  Bar.close[0]: {2:N5}  MidBand {3:N5} ArmPoint {4:N5}",
//                  currentBrickId, tradeEnterBar, Bars.Close[0], m_MidBand[0], Positions[0].OpenTrades[0].EntryOrder.Price - (brickSize * BrickToArmMidlineExit));


//            //Strategy Exit  armed
//            if (!midLineExitArmed && Positions[0].OpenTrades[0].EntryOrder.Price - (brickSize * BrickToArmMidlineExit) >= Bars.Close[0])
//            {
//                midLineExitArmed = true;
//                Print("Short Midline Armed");
//                return;
//            }

//            if (midLineExitArmed)
//            {
//                Print("Short Midline Evaluate");
//                if (Bars.Close[0] > m_MidBand[0])
//                {
//                    buyToCoverMarket.Send("9BSXmid");
//                    Print("*Exit Short MidLine");
//                }
//                else
//                {
//                    return;
//                }
//            }


//            int waveBars = evaluator[0].Count;
//            if (evaluator[0][waveBars - 1].Trend == Trend.Bull && evaluator[0][waveBars - 2].Trend == Trend.Bull
//               || (evaluator[0][waveBars - 1].Trend == Trend.Bull && evaluator[0][waveBars - 2].Trend == Trend.Bear
//                     && evaluator[0][waveBars - 3].Trend == Trend.Bull && evaluator[0][waveBars - 4].Trend == Trend.Bear ||
//                    (currentBrickId - tradeEnterBar >= BrickToArmMidlineExit && Bars.Close[0] > m_MidBand[0]))
//                )
//            {
//                buyToCoverMarket.Send("9BSX");
//                Print("*Exit Short ");

//            }

//        }

//        private void HandleFlat(int brickId)
//        {
//            {

//                try
//                {
//                    if (evaluator[0].Count < BricksToEnter)
//                        return;

//                    int waveBars = evaluator[0].Count;
//                    if (evaluator[0].Last().Trend == Trend.Bull)
//                    {
//                        for (int i = waveBars - 1; i >= waveBars - BricksToEnter; i--)
//                        {
//                            if (evaluator[0][i].Trend == Trend.Bear)
//                                return;

//                        }
//                        PrintLine();
//                        Print("Entry Bar:{0}", brickId);
//                        //Print("lastbar:{0}  00.low:{1} 1LastBar.low:{2} SizeInRate:{3} Subtraction: {4} ",
//                        //lastBar, evaluator[0][0].Low, evaluator[1][lastBar].Low, evaluator[0][0].SizeInRate, 
//                        //(evaluator[1][].Low - (evaluator[0][0].SizeInRate/2.0)));				
//                        buyOrder.Send("9BL");
//                        Print("* Long Strategy Entry");
//                        tradeEnterBar = currentBrickId;
//                        midLineExitArmed = false;

//                    }
//                    else
//                    {
//                        for (int i = waveBars - 1; i >= waveBars - BricksToEnter; i--)
//                        {
//                            if (evaluator[0][i].Trend == Trend.Bull)
//                                return;
//                        }
//                        sellShort.Send("9BS");
//                        tradeEnterBar = currentBrickId;
//                        midLineExitArmed = false;
//                        Print("* Short Strategy Entry");


//                    }


//                }
//                catch (Exception error)
//                {
//                    var sb = new StringBuilder();

//                    sb.AppendLine("Abort in HandleFlat()");
//                    sb.AppendLine("BrickID: " + brickId);
//                    sb.AppendLine("RenkoMoveInBricks[0]=" + evaluator[0].RenkoMoveInBricks);
//                    sb.AppendLine("RenkoMoveInBricks[1]=" + evaluator[1].RenkoMoveInBricks);
//                    sb.AppendLine("RenkoMoveInBricks[2]=" + evaluator[2].RenkoMoveInBricks);
//                    sb.Append("Brick: ");
//                    PrintBrick(1);
//                    sb.AppendLine("Error: " + error.Message);
//                }


//            }
//        }

//        #region Helpers

//        private void Abort(string format, params object[] args)
//        {
//            string message = string.Format(format, args);

//            ExecControl.Abort(message);
//        }

//        private void PrintLine()
//        {
//            Print(new string('=', 20));
//        }

//        private void PrintTradeInfo(string prefix)
//        {
//            PrintLine();

//            PrintBrick(1);

//            var sb = new StringBuilder();

//            sb.Append(prefix.ToUpper());

//            sb.AppendFormat("Symbol:{0} Resolution:{1} CurrentBar:{2} Date:{3}",
//                Bars.Request.Symbol, Bars.Request.Resolution.Size, Bars.CurrentBar, Bars.Time.Value);

//            Print(sb.ToString());

//            PrintLine();
//        }

//        private void PrintBrick(int index)
//        {
//            Print("{0:MM/dd/yyyy HH:mm:ss.fff} O:{1:N5} H:{2:N5} L:{3:N5} C:{4:N5}",
//                Bars.Time[index], Bars.Open[index], Bars.High[index], Bars.Low[index], Bars.Close[index]);
//        }

//        private void Print()
//        {
//            Output.WriteLine("");
//        }

//        private void Print(string format, params object[] args)
//        {
//            string message = string.Format(format, args);

//            Output.WriteLine(message);
//        }

//        private void PrintRenkoMoveInBricks()
//        {
//            if (evaluator.Count < 3)
//                return;

//            Print("New Brick: RenkoMoveInBricks  Bar:{0} L0:{1}       L1:{2}         L2:{3}",
//                    Bars.FullSymbolData.Current, evaluator[0].RenkoMoveInBricks, evaluator[1].RenkoMoveInBricks, evaluator[2].RenkoMoveInBricks);
//        }

//        #endregion
//    }
//}