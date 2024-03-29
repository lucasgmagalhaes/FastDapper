﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Running;
using Bogus;
using Dapper;
using FastDapper;
using Npgsql;
using System.Data;

namespace BenchmakFastDapper
{
    [Table("customers2")]
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("active")]
        public bool Active { get; set; }
    }

    [MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvMeasurementsExporter, RPlotExporter]
    [MemoryDiagnoser]
    public class Benchmak
    {
        private IDbConnection? con1;
        private IDbConnection? con2;
        private Faker<Customer>? faker;

        [Params(1, 5, 10, 50)]
        public int Count { get; set; }

        private List<Customer>? GetUsers()
        {
            return faker?.Generate(Count);
        }

        [GlobalSetup]
        public void Setup()
        {
            var map = FastManager.CreateEmptyMap<Customer>();
            map.Table("customers");
            map.PrimaryKey(u => u.Id, "id");
            map.Column(u => u.Name);
            map.Column(u => u.Email);
            map.Column(u => u.Active);

            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);
            con1 = new NpgsqlConnection("Host=localhost;Port=5432;Database=b2bg;User ID=postgres;Password=123;Pooling=true;SearchPath=b2b.monitoring");
            con2 = new NpgsqlConnection("Host=localhost;Port=5432;Database=b2bg;User ID=postgres;Password=123;Pooling=true;SearchPath=b2b.monitoring");

            faker = new Faker<Customer>()
                .RuleFor(c => c.Name, f => f.Name.FullName())
                .RuleFor(c => c.Email, f => f.Internet.Email());
        }

        [Benchmark]
        public async Task Insert_DapperSimpleCrud()
        {
            foreach (var item in GetUsers() ?? Array.Empty<Customer>().ToList())
            {
                await con1.InsertAsync(item);
            }
        }

        [Benchmark]
        public async Task Insert_FastDapper()
        {
            foreach (var item in GetUsers() ?? Array.Empty<Customer>().ToList())
            {
                await con2.ExecuteAsync(Builder.BuildInsertStatement<Customer>(), item);
            }
        }
    }

    public static class Program
    {
        public static void Main(string[] _)
        {
            Randomizer.Seed = new Random(8675309);
            BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}