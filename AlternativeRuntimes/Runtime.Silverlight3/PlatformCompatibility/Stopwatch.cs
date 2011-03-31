using System;

namespace System.Diagnostics
{
    /// <summary>
    /// Stopwatch is used to measure the general performance of Silverlight functionality. Silverlight
    /// does not currently provide a high resolution timer as is available in many operating systems,
    /// so the resolution of this timer is limited to milliseconds. This class is best used to measure
    /// the relative performance of functions over many iterations.
    /// </summary>
    public sealed class Stopwatch
    {
        private long _startTick;
        private long _elapsed;
        private bool _isRunning;

        /// <summary>
        /// Creates a new instance of the class and starts the watch immediately.
        /// </summary>
        /// <returns>An instance of Stopwatch, running.</returns>
        public static Stopwatch StartNew()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            return sw;
        }

        /// <summary>
        /// Creates an instance of the Stopwatch class.
        /// </summary>
        public Stopwatch() { }

        /// <summary>
        /// Completely resets and deactivates the timer.
        /// </summary>
        public void Reset()
        {
            _elapsed = 0;
            _isRunning = false;
            _startTick = 0;
        }

        /// <summary>
        /// Begins the timer.
        /// </summary>
        public void Start()
        {
            if (!_isRunning)
            {
                _startTick = GetCurrentTicks();
                _isRunning = true;
            }
        }

        /// <summary>
        /// Stops the current timer.
        /// </summary>
        public void Stop()
        {
            if (_isRunning)
            {
                _elapsed += GetCurrentTicks() - _startTick;
                _isRunning = false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the instance is currently recording.
        /// </summary>
        public bool IsRunning
        {
            get { return _isRunning; }
        }

        /// <summary>
        /// Gets the Elapsed time as a Timespan.
        /// </summary>
        public TimeSpan Elapsed
        {
            get { return TimeSpan.FromMilliseconds(ElapsedMilliseconds); }
        }

        /// <summary>
        /// Gets the Elapsed time as the total number of milliseconds.
        /// </summary>
        public long ElapsedMilliseconds
        {
            get { return GetCurrentElapsedTicks() / TimeSpan.TicksPerMillisecond; }
        }

        /// <summary>
        /// Gets the Elapsed time as the total number of ticks (which is faked
        /// as Silverlight doesn't have a way to get at the actual "Ticks")
        /// </summary>
        public long ElapsedTicks
        {
            get { return GetCurrentElapsedTicks(); }
        }

        private long GetCurrentElapsedTicks()
        {
            return (long)(this._elapsed + (IsRunning ? (GetCurrentTicks() - _startTick) : 0));
        }

        private long GetCurrentTicks()
        {
            // TickCount: Gets the number of milliseconds elapsed since the system started.
            return Environment.TickCount * TimeSpan.TicksPerMillisecond;
        }
    }

}

