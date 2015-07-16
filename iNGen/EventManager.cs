using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iNGen
{
    public class EventManager
    {
        public List<TimedEvent> Events {get; set;}

        public EventManager()
        {
            Events = new List<TimedEvent>();
        }

        public void Update()
        {
            for (int iEvent = 0; iEvent < Events.Count; ++iEvent)
            {
                var timedEvent = Events[iEvent];
                if (timedEvent.IsReady)
                {
                    timedEvent.Invoke();
                    if (timedEvent.RunOnce)
                    {
                        Events.RemoveAt(iEvent);
                        --iEvent;
                    }
                }
            }
        }
    }

    public class TimedEvent
    {
        public Stopwatch Stopwatch { get; set; }
        public int EventPeriod { get; set; }
        public Action Event { get; set; }
        public bool RunOnce { get; set; }

        public TimedEvent(int eventPeriodMilliseconds, Action action, bool runOnce = false)
        {
            Stopwatch = new Stopwatch();
            Stopwatch.Restart();

            EventPeriod = eventPeriodMilliseconds;
            Event = action;
            RunOnce = runOnce;
        }

        public bool IsReady
        {
            get
            {
                return Stopwatch.Elapsed.TotalMilliseconds >= EventPeriod;
            }
        }

        public void Invoke()
        {
            Event();
            Reset();
        }

        public void Reset()
        {
            Stopwatch.Restart();
        }
    }
}
