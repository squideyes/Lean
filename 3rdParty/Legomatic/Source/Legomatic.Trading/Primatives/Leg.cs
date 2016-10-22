using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Legomatic.Trading
{
    public class Leg : IEnumerable<Brick>
    {
        private List<Brick> bricks = new List<Brick>();
        private decimal? extreme = null;

        internal Leg(List<Brick> bricks)
        {
            Init(bricks[0]);

            foreach (var brick in bricks.Skip(1))
                Add(brick);
        }

        internal Leg(Brick brick)
        {
            Init(brick);
        }

        private void Init(Brick brick)
        {
            Trend = brick.Trend;

            Add(brick);
        }

        public Guid Guid { get; } = Guid.NewGuid();

        public Trend Trend { get; private set; }

        public bool IsClosed { get; private set; }

        public List<Leg> GetWaves(int tolerance, bool reversed = true)
        {
            var legs = new List<Leg>();

            var factory = new LegFactory();

            factory.OnLeg += (s, e) =>
            {
                if ((legs.Count == 0) || (e.InsertLeg))
                    legs.Insert(0, e.Leg);
                else
                    legs[0] = e.Leg;
            };

            foreach (var brick in this)
                factory.HandleTick(brick, tolerance);

            if (!reversed)
                return legs;

            var array = legs.ToArray();

            Array.Reverse(array);

            return array.ToList();
        }

        public decimal BrickSizeInRate => bricks[0].SizeInRate;

        public decimal BrickSizeInPips => bricks[0].SizeInPips;

        public Decimals Decimals => bricks[0].Decimals;

        public int CounterTrendBricks
        {
            get
            {
                var ctb = 0;

                for (int i = 1; i < Count; i++)
                {
                    if (bricks[i].Trend != Trend)
                        ctb++;
                }

                return ctb;
            }
        }

        public decimal? Extreme
        {
            get
            {
                if (extreme.HasValue)
                {
                    return extreme.Value;
                }
                else
                {
                    if (Trend == Trend.Bull)
                        return GetOhlc().High;
                    else
                        return GetOhlc().Low;
                }
            }
        }

        public Ohlc Ohlc => GetOhlc();

        private Ohlc GetOhlc(List<Brick> bricks = null)
        {
            if (Count == 0) //????????????????????????????????????
                return null;

            if (bricks == null)
                bricks = this.bricks;

            var ohlc = new Ohlc()
            {
                HighId = bricks[0].Id,
                LowId = bricks[0].Id,
                Open = bricks[0].Open,
                High = bricks[0].High,
                Low = bricks[0].Low,
                Close = bricks[0].Close
            };

            for (int i = 1; i < Count; i++)
            {
                var brick = bricks[i];

                if (Trend == Trend.Bull)
                {
                    if (brick.Close > ohlc.Close)
                    {
                        ohlc.Close = ohlc.High = brick.Close;
                        ohlc.HighId = brick.Id;
                    }
                }
                else
                {
                    if (brick.Close < ohlc.Close)
                    {
                        ohlc.Close = ohlc.Low = brick.Close;
                        ohlc.LowId = brick.Id;
                    }
                }
            }

            return ohlc;
        }

        public int TrendBricks => Count - CounterTrendBricks;

        public decimal FullMoveInBricks => Math.Round(
            FullMoveInPips / BrickSizeInPips, (int)Decimals);

        public decimal FullMoveInPips => FullMoveInRate.RateToPips(Decimals);

        public decimal FullMoveInRate
        {
            get
            {
                var ohlc = GetOhlc();

                return Math.Round(
                    Math.Abs(ohlc.Low - Extreme.Value), (int)Decimals);
            }
        }

        public decimal RenkoMoveInRate
        {
            get
            {
                var ohlc = GetOhlc();

                return Math.Round(
                    Math.Abs(ohlc.Open - ohlc.Close), (int)Decimals);
            }
        }

        public decimal RenkoMoveInPips =>
            RenkoMoveInRate.RateToPips(Decimals);

        public decimal RenkoMoveInBricks =>
            Math.Round(RenkoMoveInPips / BrickSizeInPips, (int)Decimals);

        public int Efficiency =>
            (int)Math.Round((decimal)TrendBricks / Count * 1000.0m);

        public Brick this[int index]
        {
            get
            {
                return bricks[index];
            }
            internal set
            {
                bricks[index] = value;
            }
        }

        public int Count => bricks.Count;

        internal void Add(Brick brick) => bricks.Add(brick);

        internal bool Reversal(List<Brick> bricks, int tolerance)
        {
            var ohlc = GetOhlc(bricks);

            if (this[Count - 1].Trend == Trend)
                return false;

            int count = 0;

            for (int i = Count - 1; i >= 0; i--)
            {
                var brick = this[i];

                if (Trend == Trend.Bull)
                {
                    if (brick.Id == ohlc.HighId)
                        return false;

                    if (brick.Trend == Trend.Bull)
                        count--;
                    else
                        count++;
                }
                else
                {
                    if (brick.Id == ohlc.LowId)
                        return false;

                    if (brick.Trend == Trend.Bear)
                        count--;
                    else
                        count++;
                }

                if (count > tolerance)
                    return true;
            }

            return false;
        }

        internal List<Brick> Close()
        {
            var reversal = new List<Brick>();

            var ohlc = GetOhlc(bricks);

            for (int i = Count - 1; i >= 0; i--)
            {
                if (Trend == Trend.Bull)
                {
                    if (this[i].Id == ohlc.HighId)
                    {
                        reversal = bricks.Skip(i + 1).Take(Count - i).ToList();

                        while (bricks.Count > (i + 1))
                            bricks.RemoveAt(bricks.Count - 1);

                        break;
                    }
                }
                else
                {
                    if (this[i].Id == ohlc.LowId)
                    {
                        reversal = bricks.Skip(i + 1).Take(Count - i).ToList();

                        while (bricks.Count > (i + 1))
                            bricks.RemoveAt(bricks.Count - 1);

                        break;
                    }
                }
            }

            if (Trend == Trend.Bull)
                extreme = reversal[0].High;
            else
                extreme = reversal[0].Low;

            IsClosed = true;

            return reversal;
        }

        public IEnumerator<Brick> GetEnumerator() => bricks.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        //public override string ToString() =>
        //    $"IDs: {this[0].Id:000} to {this[Count - 1].Id:000}, Count: {Count}";

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Ohlc.Trend);

            sb.Append(", ");

            sb.Append($"{Ohlc.Open:N5} to {Ohlc.Close:N5} (");

            sb.Append($"IDs: {this[0].Id:0000} to {this[Count - 1].Id:0000}, ");

            sb.Append($"Bricks: {Count})");

            return sb.ToString();
        }
    }
}
