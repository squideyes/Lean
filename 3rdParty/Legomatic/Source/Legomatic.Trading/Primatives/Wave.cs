using System.Collections;
using System.Collections.Generic;

namespace Legomatic.Trading
{
    public class Wave : IEnumerable<Brick>
    {
        private List<Brick> bricks = new List<Brick>();

        internal Wave(int id, Brick brick)
        {
            Id = id;
            Trend = brick.Trend;

            bricks.Add(brick);
        }

        public void Add(Brick bricks)
        {
        }

        public int Id { get;}
        public Trend Trend { get; }

        public IEnumerator<Brick> GetEnumerator()
        {
            return bricks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
