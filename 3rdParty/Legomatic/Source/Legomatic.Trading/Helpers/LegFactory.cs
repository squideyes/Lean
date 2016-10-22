using System;
using System.Linq;

namespace Legomatic.Trading
{
    public class LegFactory
    {
        private int? lastBrickId = null;

        public class LegArgs : EventArgs
        {
            public LegArgs(Leg leg, bool insertLeg)
            {
                Leg = leg;
                InsertLeg = insertLeg;
            }

            public Leg Leg { get; private set; }
            public bool InsertLeg { get; private set; }
        }

        private int? tolerance = null;
        private Leg leg = null;
        private Decimals? decimals = null;

        public event EventHandler<LegArgs> OnLeg;

        internal void HandleTick(Brick brick, int tolerance)
        {
            if (!this.tolerance.HasValue)
                this.tolerance = tolerance;

            if (leg == null)
            {
                leg = new Leg(brick);

                decimals = brick.Decimals;

                OnLeg?.Invoke(this, new LegArgs(leg, true));
            }
            else
            {
                if (brick.Decimals != decimals)
                {
                    throw new InvalidOperationException(string.Format(
                        "brick.Decimals must be {0}", brick.Decimals));
                }

                if (brick.Id == lastBrickId)
                {
                    leg[leg.Count - 1] = brick;
                }
                else
                {
                    leg.Add(brick);

                    bool insertLeg = false;

                    if (leg.Reversal(leg.ToList(), this.tolerance.Value))
                    {
                        var bricks = leg.Close();

                        OnLeg?.Invoke(this, new LegArgs(leg, false));

                        insertLeg = true;

                        leg = new Leg(bricks);

                        this.tolerance = tolerance;
                    }

                    OnLeg?.Invoke(this, new LegArgs(leg, insertLeg));
                }
            }

            lastBrickId = brick.Id;
        }
    }
}
