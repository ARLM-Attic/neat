using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neat;

namespace Neat.Game.SideScroller
{
    public class StateMachine : Dictionary<string, State>
    {
        NeatGame game;
        int timer;

        public State CurrentState;
        
        public void Activate(string key)
        {
            if (!ContainsKey(key))
            {
                game.SayMessage("StateMachine.Activate(): The specified key does not exist in the StateMachine. key=" + key);
                return;
            }
            if (CurrentState != null)
            {
                if (CurrentState.Finish != null) CurrentState.Finish();
                timer = 0;
                CurrentState = this[key];
                if (CurrentState.Activate != null) CurrentState.Activate();
            }
        }

        public void Update()
        {
            if (timer++ == CurrentState.Duration)
            {
                if (CurrentState.NextState != null) Activate(CurrentState.NextState);
                return;
            }
            if (CurrentState.Update != null) CurrentState.Update(timer);
        }
    }
}
