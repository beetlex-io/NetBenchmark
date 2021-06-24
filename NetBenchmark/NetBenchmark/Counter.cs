using System;
using System.Collections.Generic;
using System.Text;

namespace NetBenchmark
{
    public class Counter
    {
        public Counter(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        private long mQuantity { get; set; }

        private long mValue;

        private long mLastValue;

        private double mLastTime;

        private long mRps;

        private long mCount;

        private long mRpsCount;

        public long Value => mValue;

        public long Rps => mRps;

        public long Max { get; private set; }

        public long Avg { get; private set; }

        public long Min { get; private set; }

        public void Add(long value)
        {
            System.Threading.Interlocked.Add(ref mValue, value);
        }

        public void Calculate()
        {
            if (mValue > mLastValue)
            {
                double stime = BeetleX.TimeWatch.GetTotalSeconds();
                double time = stime - mLastTime;
                var svalue = mValue - mLastValue;
                mQuantity++;
                mRps = (long)(svalue / time);
                mLastValue = Value;
                mLastTime = stime;

                if (Rps > Max)
                    Max = Rps;
                if (mQuantity > 3)
                {
                    if (Rps < Min || Min == 0)
                        Min = Rps;
                }
                
                mCount++;
                mRpsCount += mRps;
                Avg = mRpsCount / mCount;
            }

        }

        public void Reset()
        {
            mLastTime = BeetleX.TimeWatch.GetTotalSeconds();
        }

        public void Print(StringBuilder sb, int baseValue = 1, string unit = "")
        {
            sb.Append("|");
            var value = $"{Name}|".PadLeft(18);
            sb.Append(value);

            value = $"{Max / baseValue:###,###,##0}|".PadLeft(10);
            sb.Append(value);

            value = $"{Avg / baseValue:###,###,##0}|".PadLeft(10);
            sb.Append(value);

            value = $"{Min / baseValue:###,###,##0}|".PadLeft(10);
            sb.Append(value);

            value = $"{mRps / baseValue:###,###,##0}/{mValue / baseValue:###,###,##0}{unit}|".PadLeft(26);
            sb.Append(value);
            sb.AppendLine("");

        }
    }
}
