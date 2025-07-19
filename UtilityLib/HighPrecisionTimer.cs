using System.Runtime.InteropServices;

namespace UtilityLib
{
    /// <summary>
    /// Microsecond-resolution timer using native Windows API with an event loop that checks for elapsed time.
    /// Busy-waiting is CPU intensive. so to minimize CPU usage while maintaining high accuracy, below approach may help:
    /// 1) Sleeping for most of the time using Thread.Sleep(0) or Thread.SpinWait()
    /// 2) Then using spin-waiting for the final few microseconds
    /// 
    /// Thread.Sleep(0):
    ///    What it does:
    ///     - Yields the current thread’s remaining time slice voluntarily, allowing other threads of equal priority to run.
    ///     - If no other threads are ready, it continues almost immediately.

    ///    Useful when:
    ///     - You want to yield CPU briefly to avoid starving other threads.
    ///     - You’re in a wait loop but don’t want to waste CPU by spinning aggressively.
    ///    
    /// Thread.SpinWait(int iterations)
    ///    What it does:
    ///     - Performs a busy-wait loop for the specified number of CPU iterations.
    ///     - Extremely lightweight compared to Sleep, it never yields the thread.
    ///     - Very useful for sub-millisecond precision delays (i.e., microseconds).

    ///    Useful when:
    ///     - You need very short delays (~10–100 microseconds).
    ///     - You’re in the final microseconds of a wait and want very fine-grained control.
    /// 
    /// </summary>
    public class HighPrecisionTimer
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        private long _start;
        private long _frequency;

        public HighPrecisionTimer()
        {
            QueryPerformanceFrequency(out _frequency);
            QueryPerformanceCounter(out _start);
        }

        public long ElapsedMicroseconds
        {
            get
            {
                QueryPerformanceCounter(out long now);
                return (now - _start) * 1_000_000 / _frequency;
            }
        }

        public void Reset()
        {
            QueryPerformanceCounter(out _start);
        }

        public static void WaitMicroseconds(long microseconds)
        {
            HighPrecisionTimer timer = new HighPrecisionTimer();
            // Estimate how long we can afford to sleep
            long sleepThreshold = microseconds - 50; // 50 µs buffer

            while (timer.ElapsedMicroseconds < sleepThreshold)
            {
                Thread.Sleep(0);  // Yield CPU
            }

            // Final wait for precise timing
            while (timer.ElapsedMicroseconds < microseconds)
            {
                Thread.SpinWait(10);
            }
        }
    }
}
