using System;

namespace MyGameplayAbilitySystem.StateMachine
{
    public class State
    {
        public Action Enter { get; set; } = () => { };
        public Action<StateMachine> Active { get; set; } = (_) => { };
        public Action Exit { get; set; } = () => { };

    }

    public class StateMachine
    {
        private State ActiveState;

        public StateMachine(State initialState)
        {
            this.ActiveState = initialState;
            initialState.Enter();
        }
        public void TickState()
        {
            ActiveState.Active(this);
        }


        public void NextState(State nextState)
        {
            ActiveState.Exit();
            nextState.Enter();
            this.ActiveState = nextState;
        }
    }
}