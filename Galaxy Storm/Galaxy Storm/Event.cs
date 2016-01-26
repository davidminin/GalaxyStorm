using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Galaxy_Storm
{
    class Event
    {
        //Holds the time the event must go off at in relation to the song in milliseconds
        int time;
        public int Time
        {
            get { return time; }
        }

        //Holds the Action of the event
        string action;
        public string Action
        {
            get { return action; }
        }

        //Constructor
        public Event(int t, string a)
        {
            time = t;
            action = a;
        }
    }
}
