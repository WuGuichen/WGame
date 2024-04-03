using System;

namespace WGame.Ability
{
    public class StateMgr
    {
        public Action<int> onStateEnable;
        public Action<int> onStateDisable;
        
        public StateMgr()
        {
            CurStates = 0;
            _newStates = 0;
        }
        
        public void ResetStates()
        {
            CurStates = 0;
            _newStates = 0;
        }
        
        private int CurStates { get; set; }
        private int _newStates;

        public void EnableState(int mask)
        {
            _newStates |= mask;
        }
        
        public void DisableState(int mask)
        {
            var nMask = ~mask;
            _newStates &= nMask;
        }

        public void CheckStateChange()
        {
            var change = (CurStates ^ _newStates);
            // var change = 0;
            if (change != 0)
            {
                var add = change & _newStates;
                var remove = change & CurStates;
                CurStates &= ~remove;
                CurStates |= add;
                if (remove != 0)
                {
                    onStateDisable?.Invoke(remove);
                }

                if (add != 0)
                {
                    onStateEnable?.Invoke(add);
                }

            }
            _newStates = 0;
        }

        public bool CheckState(int mask)
        {
            return (CurStates & mask) != 0;
        }
    }
}