using Legomatic.Trading;
using QuantConnect.Data;
using QuantConnect.Data.Consolidators;
using QuantConnect.Data.Market;
using QuantConnect.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuantConnect.Algorithm.CSharp
{
    public class NPI_010G1_Algorithm : QCAlgorithm
    {
        private const Decimals DECIMALS = Decimals.Five;

        private const int TOLERANCE = 3;
        private double CURRENCYSCALE = 0.0001;
        private int CURRENCYDIGITS = 5;

        private int currentBrickId = -1;
        private double brickSize;

        private bool shortEntryArmed = false;
        private bool longEntryArmed = false;
        private bool exitArmed = false;

        private bool directionFilterLong = false;
        private bool directionFilterShort = false;

        private bool firstMidlineCross = false;

        private double ratePerPip;
        private double stopPrice;

        private string tradeDirectionFilter = "all";  //all, long, short

        private Symbol _symbol;

        private List<Brick> bricks = new List<Brick>();

        private LegEvaluator evaluator;

        public override void Initialize()
        {
            QuantConnect.Symbol.Create(Ticker, SecurityType.Forex, Market.FXCM);

            SetStartDate(2013, 10, 07);  //Set Start Date
            SetEndDate(2013, 10, 11);    //Set End Date
            SetCash(100000);             //Set Strategy Cash

            AddForex(_symbol.Value, Resolution.Tick);

            var consolidator = new RenkoConsolidator(30m, RenkoType.Wicked);

            consolidator.DataConsolidated += (sender, bar) =>
            {
                var brick = new Brick(currentBrickId++, bar.EndTime, (double)bar.Open,
                    (double)bar.High, (double)bar.Low, (double)bar.Close, DECIMALS);

                bricks.Insert(0, brick);

                evaluator.HandleTick(TOLERANCE, brick);
            };

            SubscriptionManager.AddConsolidator(_symbol, consolidator);
        }

        [Parameter("npi-010G1-ticker")]
        public string Ticker = "EURUSD";

        [Parameter("npi-010G1-bollinger-length")]
        public int BollingerLength = 20;

        [Parameter("npi-010G1-bollinger-sdv")]
        public double BollingerSDV = 1.75;

        [Parameter("npi-010G1-confirmation-bricks")]
        public int ConfirmationBricks = 2;

        public override void OnData(Slice data)
        {
            if (!Portfolio.Invested)
            {
                SetHoldings(_symbol, 1);
                Debug("Purchased Stock");
            }
        }
    }
}