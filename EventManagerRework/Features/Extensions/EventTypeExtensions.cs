using EventManagerRework.Events.GunGame;
using EventManagerRework.Features.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagerRework.Features.Extensions
{
    public static class EventTypeExtensions
    {
        public static bool Launch(this EventType @event, out string response)
        {
            switch (@event)
            {
                case EventType.GunGame:
                    response = GunGame.Start();
                    if (!GunGame.StartingInProgress)
                        return false;
                    return true;
                default:
                    response = "event is none or not implemented yet.";
                    return false;
            }
        }
    }
}
