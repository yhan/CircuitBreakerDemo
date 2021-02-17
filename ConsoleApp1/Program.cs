using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

class RangePartitionerDemo
{
    public static void Main()
    {
        var rnd = new Random();
        int breakIndex = rnd.Next(1, 11);

        Console.WriteLine($"Will call Break at iteration {breakIndex}\n");

        var result = Parallel.For(1, 101, (i, state) =>
        {
            Console.WriteLine($"Beginning iteration {i}");
            int delay;
            lock (rnd)
                delay = rnd.Next(1, 1001);
            Thread.Sleep(delay);

            if (state.ShouldExitCurrentIteration)
            {
                if (state.LowestBreakIteration < i)
                    return;
            }

            if (i == breakIndex)
            {
                Console.WriteLine($"Break in iteration {i}");
                state.Break();
            }

            Console.WriteLine($"Completed iteration {i}");
        });

        if (result.LowestBreakIteration.HasValue)
            Console.WriteLine($"\nLowest Break Iteration: {result.LowestBreakIteration}");
        else
            Console.WriteLine($"\nNo lowest break iteration.");
    }

    private static void ParallelCompare()
    {
        Stopwatch sw = null;

        long sum = 0;
        long SUMTOP = 10000000;

        // Try sequential for
        sw = Stopwatch.StartNew();
        for (long i = 0; i < SUMTOP; i++) sum += i;
        sw.Stop();
        Console.WriteLine("sequential for result = {0}, time = {1} ms", sum, sw.ElapsedMilliseconds);

        //Try parallel for --this is slow!
        sum = 0;
        sw.Restart();
        Parallel.For(0L, toExclusive: SUMTOP, (item) => Interlocked.Add(ref sum, item));
        sw.Stop();
        Console.WriteLine("parallel for  result = {0}, time = {1} ms", sum, sw.ElapsedMilliseconds);

        // Try parallel for with locals
        sum = 0;
        sw.Restart();
        Parallel.For(0L, SUMTOP, localInit: () => 0L, body: (item, state, prevLocal) => prevLocal + item, localFinally: local => Interlocked.Add(ref sum, local));
        sw.Stop();
        Console.WriteLine("parallel for w/locals result = {0}, time = {1} ms", sum, sw.ElapsedMilliseconds);

        // Try range partitioner
        sum = 0;
        sw.Restart();
        Parallel.ForEach(Partitioner.Create(0L, SUMTOP), range =>
        {
            long local = 0;
            Console.WriteLine($"{range.Item1} - {range.Item2}");
            for (long i = range.Item1; i < range.Item2; i++)
                local += i;

            Interlocked.Add(ref sum, local);
        });
        sw.Stop();
        Console.WriteLine("range partitioner result = {0}, time = {1} ms", sum, sw.ElapsedMilliseconds);
    }
}