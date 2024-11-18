using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public abstract class Timer
    {
        protected float initialTime;
        protected float Time { get; set; }
        public bool IsRunning {  get; private set; }
        public float Progress => Time / initialTime;

        public Action OnTimerStart = delegate { };
        public Action OnTimerStop = delegate { };

        protected Timer(float value)
        {
            initialTime = value;
            IsRunning = false;
        }

        public void Start()
        {
            Time = initialTime;
            if(!IsRunning)
            {
                IsRunning = true;
                OnTimerStart?.Invoke();
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                OnTimerStop?.Invoke();
            }
        }

        public void Resume() => IsRunning = true;
        public void Pause() => IsRunning = false;
        public abstract void Tick(float deltaTime);
    }

    public class CountDownTimer : Timer
    {
        public CountDownTimer(float value) : base(value) { }
        public override void Tick(float deltaTime)
        {
            if(IsRunning && Time > 0) { 
                Time -= deltaTime;
            }

            if(IsRunning && Time <= 0) { 
                Stop();
            }
        }
        public bool IsFinished() => Time <= 0;
        public void Reset() => Time = initialTime;
        public void Reset(float value)
        {
            initialTime = value;
            Reset();
        }
    }

    public class StopWatchTimer : Timer
    {
        public StopWatchTimer(float value) : base(value) { }
        public override void Tick(float deltaTime)
        {
            if (IsRunning)
            {
                Time += deltaTime;
            }
        }
        public void Reset() => Time = 0;
        public float GetTime() => Time;
    }
}
