namespace WGame.Ability
{
    public class AbilityStatus
    {
        private AbilityData _ability;
        private float _curTime;
        private int _millisecondTime;
        public bool IsEnable { get; private set; }

        public void Initialize(AbilityData ability)
        {
            _ability = ability;
            _curTime = 0;
            IsEnable = true;
            OnStart();
        }
        
        public void Process(float deltaTime)
        {
            _millisecondTime = (int)(_curTime * 1000);
            if (!IsEnable)
            {
                return;
            }
            if (_millisecondTime > _ability.TotalTime)
            {
                OnEnd();
                return;
            }
            
            // update

            _curTime += deltaTime;
        }
        
        private void OnEnd()
        {
            WLogger.Print($"End {_ability.Name}");
            IsEnable = false;
        }

        private void OnStart()
        {
            WLogger.Print($"start {_ability.Name}");
        }
    }
}