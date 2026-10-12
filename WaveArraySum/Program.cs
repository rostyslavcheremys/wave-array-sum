namespace WaveArraySum;

static class Program
{
    public static void Main()
    {
        const int size = 1000000000;
        const int threadCount = 8;

        int[] array = new int[size];

        for (int i = 0; i < size; i++)
        {
            array[i] = 1;
        }

        long waveSum = WaveMethod.Run(array, threadCount, out long waveTime);

        Console.WriteLine("Wave method:");
        Console.WriteLine($"Sum  = {waveSum}");
        Console.WriteLine($"Time = {waveTime} ms");
    }
}