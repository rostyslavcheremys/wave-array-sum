using System.Diagnostics;

namespace WaveArraySum;

public static class WaveMethod
{
    public static long Run(int[] array, int threadCount, out long elapsedMilliseconds)
    {
        if (array == null) {
            throw new ArgumentNullException(nameof(array));
        }

        if (threadCount < 1) {
            throw new ArgumentOutOfRangeException(nameof(threadCount));
        }

        Stopwatch stopwatch = Stopwatch.StartNew();

        int activeLength = array.Length;

        if (activeLength == 0) {
            stopwatch.Stop();
            elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            return 0;
        }

        threadCount = Math.Min(threadCount, Math.Max(1, activeLength / 2));

        using Barrier barrier = new Barrier(threadCount + 1);

        int nextIndex = 0;
        int pairCount = 0;
        bool finished = false;

        Thread[] workers = new Thread[threadCount];

        for (int workerId = 0; workerId < threadCount; workerId++) {
            workers[workerId] = new Thread(() => {
                while (true) {
                    barrier.SignalAndWait();

                    if (finished) break;

                    while (true) {
                        int index = Interlocked.Increment(ref nextIndex) - 1;

                        if (index >= pairCount) break;

                        int symmetricIndex = activeLength - 1 - index;

                        array[index] += array[symmetricIndex];
                    }
                    
                    barrier.SignalAndWait();
                }
            });

            workers[workerId].Start();
        }

        while (activeLength > 1) {
            pairCount = activeLength / 2;
            nextIndex = 0;
            
            barrier.SignalAndWait();
            
            barrier.SignalAndWait();
            
            activeLength = (activeLength + 1) / 2;
        }

        finished = true;
        
        barrier.SignalAndWait();

        foreach (Thread worker in workers) {
            worker.Join();
        }

        long result = array[0];

        stopwatch.Stop();
        elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

        return result;
    }
}