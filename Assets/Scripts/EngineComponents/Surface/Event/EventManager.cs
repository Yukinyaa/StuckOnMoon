using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class EventManager : Singleton<EventManager>
{
    List<SurfaceEvent> Events = new List<SurfaceEvent>();//new SurfaceEventComparerByFrame()

    public void RegisterLocalEvent(SurfaceEvent s_event)
    {
        Events.Add(s_event);
    }
    public List<SurfaceEvent> PopEvents(ulong frameNo)
    {
        var frameEvents = (from ev in Events where (ev.RegistedFrame == frameNo || ev.RegistedFrame == null) select ev).ToList();
        Events.RemoveAll(ev => ev.RegistedFrame == frameNo || ev.RegistedFrame == null); //todo: 날잡고 최적화 씹가능

        return frameEvents;
    }
}
