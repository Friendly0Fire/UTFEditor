﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace UTFEditor
{
    // Derived from code by Michael Nash
    // http://www.indigo79.net/archives/27

    class MultimediaTimer
    {
        private delegate void TimerEventHandler(int id, int msg, IntPtr user, int dw1, int dw2);
        private TimerEventHandler handler;

        public event EventHandler Tick;

        private const int TIME_PERIODIC = 1;
        private const int EVENT_TYPE = TIME_PERIODIC;// + 0x100;

        [DllImport("winmm.dll")]
        private static extern int timeSetEvent(int delay, int resolution, TimerEventHandler handler, IntPtr user, int eventType);

        [DllImport("winmm.dll")]
        private static extern int timeKillEvent(int id);

        [DllImport("winmm.dll")]
        private static extern int timeBeginPeriod(int msec);

        [DllImport("winmm.dll")]
        private static extern int timeEndPeriod(int msec);

        private int interval = 10;
        public int Interval
        {
            get
            {
                return interval;
            }
            set
            {
                if (value <= 0) return;

                interval = value;
            }
        }

        private int timerID = 0;

        public MultimediaTimer()
        {
        }

        public void Start()
        {
            if (timerID != 0) return;

            timeBeginPeriod(1);
            handler = new TimerEventHandler(TimerHandler);
            timerID = timeSetEvent(interval, 0, handler, IntPtr.Zero, EVENT_TYPE);
        }

        private void TimerHandler(int id, int msg, IntPtr user, int dw1, int dw2)
        {
            if (Tick != null)
                Tick(this, new EventArgs());
        }

        public void Stop()
        {
            if (timerID == 0) return;

            int err = timeKillEvent(timerID);
            timeEndPeriod(1);
            timerID = 0;
        }

        public bool Enabled
        {
            get
            {
                return timerID != 0;
            }
        }
    }
}
