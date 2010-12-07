using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neat.Game.SideScroller
{
    public class State
    {
        public Action Activate;
        public Action<int> Update;
        public Action Finish;
        public string NextState;
        public int Duration = -1;

        public State(Action _activate, Action<int> _update, Action _finish, string _nextState, int _duration)
        {
            Activate = _activate;
            Update = _update;
            Finish = _finish;
            NextState = _nextState;
            Duration = _duration;
        }

        public State(Action _activate, Action<int> _update, Action _finish)
        {
            Activate = _activate;
            Update = _update;
            Finish = _finish;
        }

        public State()
        {
        }

        public State(Action _activate)
        {
            Activate = _activate;
        }

        public State(Action _activate, string _nextState, int _duration)
        {
            Activate = _activate;
            NextState = _nextState;
            Duration = _duration;
        }

        public State(Action _activate, Action _finish)
        {
            Activate = _activate;
            Finish = _finish;
        }

        public State(Action _activate, Action _finish, string _nextState, int _duration)
        {
            Activate = _activate;
            Finish = _finish;
            NextState = _nextState;
            Duration = _duration;
        }

        public State(Action _activate, Action<int> _update)
        {
            Activate = _activate;
            Update = _update;
        }

        public State(Action _activate, Action<int> _update, string _nextState, int _duration)
        {
            Activate = _activate;
            Update = _update;
            NextState = _nextState;
            Duration = _duration;
        }

        public State(Action<int> _update, Action _finish)
        {
            Update = _update;
            Finish = _finish;
        }

        public State(Action<int> _update, Action _finish, string _nextState, int _duration)
        {
            Update = _update;
            Finish = _finish;
            NextState = _nextState;
            Duration = _duration;
        }

        public State(Action<int> _update)
        {
            Update = _update;
        }

        public State(Action<int> _update, string _nextState, int _duration)
        {
            Update = _update;
            NextState = _nextState;
            Duration = _duration;
        }
        /*
        public State(Action _finish)
        {
            Finish = _finish;
        }

        public State(Action _finish, string _nextState, int _duration)
        {
            Finish = _finish;
            NextState = _nextState;
            Duration = _duration;
        }*/
    }
}
