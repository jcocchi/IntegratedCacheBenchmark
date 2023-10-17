using System;
using System.Collections.Generic;
using System.Text;

namespace IntegratedCacheDemo
{
    class Result
    {
        public long Latency;
        public double RU;

        public Result(long latency, double ru)
        {
            Latency = latency;
            RU = ru;
        }
    }

    public class ResultSummary
    {
        public string testType;
        public string testName;
        public string averageLatency;
        public string averageRu;

        public ResultSummary(string testT, string testN, string latency, string Ru)
        {
            testType = testT;
            testName = testN;
            averageLatency = latency;
            averageRu = Ru;
        }
    }
}
