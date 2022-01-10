using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using DapperOperations;

namespace BenchmakDapperOperation
{
    public class Customer
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public bool Active { get; set; }
    }

    [MemoryDiagnoser]
    public class Benchmak
    {

        [GlobalSetup]
        public void Setup()
        {
            DapperOperation.Map<Customer>();
        }

        [Benchmark]
        public void BuildUpsertWithCache()
        {
            for (int i = 0; i < 300; i++)
            {
                Builder.BuildUpsertStatement<Customer>(3, true, u => new { u.Id, u.Name }, true);
            }
        }

        [Benchmark]
        public void BuildUpsertWithoutCache()
        {
            for (int i = 0; i < 300; i++)
            {
                Builder.BuildUpsertStatement<Customer>(3, true, u => new { u.Id, u.Name }, false);
            }
        }
    }

    public static class Program
    {
        public static void Main(string[] _)
        {
            //DapperOperation.Map<Customer>();
            //new Benchmark().BuildUpsertWithCache();
            BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}