using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTToolbox
{
    public class TimePeriod
    {
        public DateTime BeginTime
        {
            get;
            private set;
        }

        public DateTime EndTime
        {
            get { return BeginTime + Span; }
        }

        public TimeSpan Span{
            get;
            private set;
        }


        public TimePeriod(DateTime begin, DateTime end)
            :this(begin, end - begin)
        {
        }

        public TimePeriod(DateTime begin, TimeSpan span)
        {
            BeginTime = begin;
            Span = span;
        }

        public bool Contains(DateTime time)
        {
            return BeginTime <= time && time <= EndTime;
        }

        public bool IsOverlap(TimePeriod p)
        {
            //return p.Contains(BeginTime) || Contains(p.BeginTime);
            return (this.BeginTime < p.EndTime) && (this.EndTime > p.BeginTime);
        }

        public TimePeriod Union(TimePeriod rhs)
        {
            return new TimePeriod(
                Utils.Min(this.BeginTime, rhs.BeginTime),
                Utils.Max(this.EndTime, rhs.EndTime));
        }

        public TimePeriod Intersection(TimePeriod rhs)
        {
            if (!IsOverlap(rhs))
                return null;

            return new TimePeriod(
                Utils.Max(this.BeginTime, rhs.BeginTime),
                Utils.Min(this.EndTime, rhs.EndTime));
        }

    }

}
