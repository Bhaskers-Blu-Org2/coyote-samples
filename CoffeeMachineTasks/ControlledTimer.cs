﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading;
using Microsoft.Coyote.Tasks;

namespace Microsoft.Coyote.Samples.CoffeeMachineTasks
{
    /// <summary>
    /// This class provides a Timer that is similar to how the Actor model timers work that
    /// has delays that can be controlled by Coyote tester.
    /// </summary>
    internal class ControlledTimer
    {
        private readonly CancellationTokenSource Source = new CancellationTokenSource();
        private TimeSpan StartDelay;
        private TimeSpan? Interval;
        private readonly Action Handler;
        private bool Stopped;

        public ControlledTimer(TimeSpan startDelay, TimeSpan interval, Action handler)
        {
            this.StartDelay = startDelay;
            this.Interval = interval;
            this.Handler = handler;
            this.StartTimer(startDelay);
        }

        public ControlledTimer(TimeSpan dueTime, Action handler)
        {
            this.StartDelay = dueTime;
            this.Handler = handler;
            this.StartTimer(dueTime);
        }

        private void StartTimer(TimeSpan dueTime)
        {
            ControlledTask.Run(async () =>
            {
                await ControlledTask.Delay(dueTime, this.Source.Token);
                this.OnTick();
            });
        }

        private void OnTick()
        {
            if (!this.Stopped)
            {
                this.Handler();
                if (this.Interval.HasValue)
                {
                    this.StartTimer(this.Interval.Value);
                }
            }
        }

        public void Stop()
        {
            this.Stopped = true;
            this.Source.Cancel();
        }
    }
}