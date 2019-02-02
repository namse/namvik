using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace namvik.FiniteStateMachine
{
    public class FiniteStateMachine<T>
    {
        private T _currentState;
        private readonly Dictionary<(T, T), Action> _transitionActions = new Dictionary<(T, T), Action>();

        public FiniteStateMachine(T initialState)
        {
            _currentState = initialState;
        }

        public FiniteStateMachine(T initialState, Dictionary<(T, T), Action> transitionActions): this(initialState)
        {
            _transitionActions = transitionActions;
        }

        public void ChangeState(T toState)
        {
            var fromState = _currentState;

            if (_transitionActions.TryGetValue((fromState, toState), out var action))
            {
                action();
            }

            _currentState = toState;
        }

        public void SetTransitionAction(T fromState, T toState, Action action)
        {
            _transitionActions.Add((fromState, toState), action);
        }
    }
}
