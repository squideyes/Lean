namespace Legomatic.Trading
{
    public class Ohlc
    {
        public decimal Open { get; internal set; }
        public decimal High { get; internal set; }
        public decimal Low { get; internal set; }
        public decimal Close { get; internal set; }
        public int HighId { get; internal set; }
        public int LowId { get; internal set; }

        public Trend Trend => (Open < Close) ? Trend.Bull : Trend.Bear;

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3} (HighId: {4}, LowId: {5})",
                Open, High, Low, Close, HighId, LowId);
        }
    }
}
