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

                mRps = (long)(svalue / time);
                mLastValue = Value;
                mLastTime = stime;
                if (Rps > Max)
                    Max = Rps;
                if (Rps < Min || Min == 0)
                    Min = Rps;
                mCount++;
                mRpsCount += mRps;
                Avg = mRpsCount / mCount;
            }

        }

        public void Reset()
        {
            mLastTime = BeetleX.TimeWatch.GetTotalSeconds();
        }

        public void Print(int baseValue = 1, string unit = "")
        {
            Console.Write("|");
            var value = $"{Name}|".PadLeft(18);
            Console.Write(value);

            value = $"{Max / baseValue:###,###,##0}|".PadLeft(10);
            Console.Write(value);

            value = $"{Avg / baseValue:###,###,##0}|".PadLeft(10);
            Console.Write(value);

            value = $"{Min / baseValue:###,###,##0}|".PadLeft(10);
            Console.Write(value);

            value = $"{mRps / baseValue:###,###,##0}/{mValue / baseValue:###,###,##0}{unit}|".PadLeft(26);
            Console.Write(value);
            Console.WriteLine("");

        }
    }
}
