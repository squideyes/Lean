using System;
using System.Collections;
using System.Collections.Generic;

namespace Legomatic.Trading
{
    public class LegEvaluator : IEnumerable<Leg>
    {
        private LegFactory factory = new LegFactory();
        private List<Leg> legs = new List<Leg>();

        public event EventHandler OnEvaluate;

        public LegEvaluator()
        {
            factory.OnLeg += (s, e) =>
            {
                if ((legs.Count == 0) || (e.InsertLeg))
                    legs.Insert(0, e.Leg);
                else
                    legs[0] = e.Leg;

                if (e.InsertLeg)
                    OnEvaluate?.Invoke(this, new EventArgs());
            };
        }

        public void HandleTick(int tolerance, Brick brick)
        {
            if (tolerance < 0)
                throw new ArgumentOutOfRangeException(nameof(tolerance));

            if (brick == null)
                throw new ArgumentNullException(nameof(brick));

            factory.HandleTick(brick, tolerance);
        }

        public int Count
        {
            get
            {
                return legs.Count;
            }
        }

        public Leg this[int index]
        {
            get
            {
                return legs[index];
            }
        }

        public bool Evaluate(int largestLegsIndex, Func<List<Leg>, bool> func)
        {
            if (largestLegsIndex < 0)
                return false;

            if (func == null)
                return false;

            if (legs.Count < largestLegsIndex + 1)
                return false;

            return func(legs);
        }

        public IEnumerator<Leg> GetEnumerator()
        {
            return legs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
