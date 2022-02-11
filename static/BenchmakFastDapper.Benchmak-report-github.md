``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1526 (21H2)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  DefaultJob : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT


```
|                  Method | Count |        Mean |     Error |      StdDev |      Median |  Gen 0 | Allocated |
|------------------------ |------ |------------:|----------:|------------:|------------:|-------:|----------:|
| **Insert_DapperSimpleCrud** |     **1** |    **337.3 μs** |   **6.75 μs** |     **5.98 μs** |    **337.2 μs** | **1.4648** |      **7 KB** |
|       Insert_FastDapper |     1 |    284.5 μs |   4.75 μs |     4.44 μs |    283.8 μs | 0.9766 |      5 KB |
| **Insert_DapperSimpleCrud** |     **5** |  **2,249.8 μs** |  **75.83 μs** |   **221.21 μs** |  **2,193.3 μs** |      **-** |     **41 KB** |
|       Insert_FastDapper |     5 |  1,256.4 μs |  25.13 μs |    31.78 μs |  1,247.5 μs | 3.9063 |     21 KB |
| **Insert_DapperSimpleCrud** |    **10** |  **3,834.7 μs** |  **91.87 μs** |   **265.07 μs** |  **3,787.9 μs** |      **-** |     **73 KB** |
|       Insert_FastDapper |    10 |  2,400.3 μs |  34.05 μs |    30.18 μs |  2,399.2 μs | 7.8125 |     41 KB |
| **Insert_DapperSimpleCrud** |    **50** | **15,450.6 μs** | **394.02 μs** | **1,149.36 μs** | **15,113.4 μs** |      **-** |    **349 KB** |
|       Insert_FastDapper |    50 | 13,063.8 μs | 261.03 μs |   744.74 μs | 12,915.0 μs |      - |    207 KB |
