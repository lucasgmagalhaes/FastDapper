﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using FastDapper;

namespace BenchmakFastDapper
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
            FastManager.Map<Customer>();
        }

        [Benchmark]
        public void BuildUpsertWithCache()
        {
            for (int i = 0; i < 10; i++)
            {
                Builder.BuildUpsertStatement<Customer>(3, true, u => new { u.Id, u.Name }, true);
            }
        }

        [Benchmark]
        public void BuildUpsertWithoutCache()
        {
            for (int i = 0; i < 10; i++)
            {
                Builder.BuildUpsertStatement<Customer>(3, true, u => new { u.Id, u.Name }, false);
            }
        }
    }

    public static class Program
    {
        public static void Main(string[] _)
        {
            //FastDapper.Map<Customer>();
            //new Benchmak().BuildUpsertWithCache();
            BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}