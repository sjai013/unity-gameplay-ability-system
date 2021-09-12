using System;

namespace MyGameplayAbilitySystem.StateMachine
{
    public class State
    {
        public readonly int Id;

        public State(int id)
        {
            this.Id = id;
        }
        public Action Enter { get; set; } = () => { };
        public Action<StateMachine> Active { get; set; } = (_) => { };
        public Action Exit { get; set; } = () => { };

    }

    public class StateMachine
    {
        private State ActiveState;
        private State m_InitialState;

        public bool IsState(int stateId)
        {
            return this.ActiveState.Id == stateId;
        }

        public StateMachine(State initialState)
        {
            this.m_InitialState = initialState;
            this.ActiveState = initialState;
            initialState.Enter();
        }
        public void TickState()
        {
            PreTick();
            ActiveState.Active(this);
        }


        public void NextState(State nextState)
        {
            ActiveState.Exit();
            nextState.Enter();
            this.ActiveState = nextState;
        }

        public Action PreTick { get; set; } = () => { };

    }
}