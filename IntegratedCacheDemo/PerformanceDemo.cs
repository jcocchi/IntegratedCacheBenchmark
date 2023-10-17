using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntegratedCacheDemo
{
    class PerformanceDemo
    {
        private static List<Benchmark> benchmarks = null;
        public PerformanceDemo()
        {
            try
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                        .AddJsonFile("AppSettings.json")
                        .Build();

                //Define new Benchmarks
                benchmarks = new List<Benchmark>
                {
                    new Benchmark
                    {
                        benchmarkType = BenchmarkType.Write,
                        testName = "dedicated gateway",
                        testDescription = $"Write sample items",
                        accountEndpoint = configuration["dedicatedGatewayAccountEndpoint"],
                        accountKey = configuration["dedicatedGatewayAccountKey"],
                        databaseId = configuration["databaseId"],
                        containerId = configuration["containerId"],
                        partitionKeyPath = configuration["partitionKeyPath"],
                        partitionKeyValue = configuration["partitionKeyValue"],
                        connectionMode = ConnectionMode.Gateway
                    },

                    new Benchmark
                 {
                        benchmarkType = BenchmarkType.PointRead,
                        testName = "without integrated cache",
                        testDescription = $"Do 100 test point reads without caching",
                        accountEndpoint = configuration["regularAccountEndpoint"],
                        accountKey = configuration["regularAccountKey"],
                        databaseId = configuration["databaseId"],
                        containerId = configuration["containerId"],
                        partitionKeyPath = configuration["partitionKeyPath"],
                        partitionKeyValue = configuration["partitionKeyValue"],
                        connectionMode = ConnectionMode.Gateway
                 },
                        new Benchmark
                 {
                        benchmarkType = BenchmarkType.PointRead,
                        testName = "with integrated cache",
                        testDescription = $"Do 100 test point reads with caching",
                        accountEndpoint = configuration["dedicatedGatewayAccountEndpoint"],
                        accountKey = configuration["dedicatedGatewayAccountKey"],
                        databaseId = configuration["databaseId"],
                        containerId = configuration["containerId"],
                        partitionKeyPath = configuration["partitionKeyPath"],
                        partitionKeyValue = configuration["partitionKeyValue"],
                        connectionMode = ConnectionMode.Gateway
                 },
                        new Benchmark
                 {
                        benchmarkType = BenchmarkType.Query,
                        testName = "without integrated cache",
                        testDescription = $"Do 100 test queries without caching",
                        accountEndpoint = configuration["regularAccountEndpoint"],
                        accountKey = configuration["regularAccountKey"],
                        databaseId = configuration["databaseId"],
                        containerId = configuration["containerId"],
                        partitionKeyPath = configuration["partitionKeyPath"],
                        partitionKeyValue = configuration["partitionKeyValue"],
                        connectionMode = ConnectionMode.Gateway
                 },
                        new Benchmark
                 {
                        benchmarkType = BenchmarkType.Query,
                        testName = "with integrated cache",
                        testDescription = $"Do 100 test queries with caching",
                        accountEndpoint = configuration["dedicatedGatewayAccountEndpoint"],
                        accountKey = configuration["dedicatedGatewayAccountKey"],
                        databaseId = configuration["databaseId"],
                        containerId = configuration["containerId"],
                        partitionKeyPath = configuration["partitionKeyPath"],
                        partitionKeyValue = configuration["partitionKeyValue"],
                        connectionMode = ConnectionMode.Gateway
                 }
                };

                foreach (Benchmark benchmark in benchmarks)
                {
                    benchmark.client = new CosmosClient(benchmark.accountEndpoint,benchmark.accountKey, new CosmosClientOptions { ConnectionMode = benchmark.connectionMode });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\nPress any key to continue");
                Console.ReadKey();
            }
        }

        public async Task Initialize()
        {
            try
            {
                foreach (Benchmark benchmark in benchmarks)
                {
                    await Benchmark.InitializeBenchmark(benchmark);
                }
            }
           catch (Exception e)
           {
               Console.WriteLine(e.Message + "\nPress any key to continue");
               Console.ReadKey();
            }
        }

        public async Task RunBenchmarks()
        {
            try
            {
                //Run benchmarks, collect results
                foreach (Benchmark benchmark in benchmarks)
                {
                    if (benchmark.benchmarkType == BenchmarkType.Write)
                        await Benchmark.WriteBenchmark(benchmark);
                    else if (benchmark.benchmarkType == BenchmarkType.Query)
                        await Benchmark.QueryBenchmark(benchmark);
                    else
                        await Benchmark.PointReadBenchmark(benchmark);
                }

                //Summarize the results
               
                foreach (Benchmark benchmark in benchmarks)
                {
                    ResultSummary r = benchmark.resultSummary;
                    Console.WriteLine("Test: {0,-12} {1,-25} Average Latency(ms): {2,-4} Average RU: {3,-4}", r.testType, r.testName, r.averageLatency, r.averageRu);
                }
                Console.WriteLine($"\nTest concluded. Press any key to continue\n...");
                Console.ReadKey(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\nPress any key to continue");
                Console.ReadKey();
            }
        }
        public async Task CleanUp()
        {
            await Benchmark.CleanUp(benchmarks);
        }
    }
}

