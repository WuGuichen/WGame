namespace WGame.Trigger
{
    public interface IWtEventDispatcher
    {
        void AddTrigger(int type, WtEventCallback0 callback);
        void AddTrigger(int type, WtEventCallback1 callback);

        void RemoveTrigger(int type, WtEventCallback0 callback);
        void RemoveTrigger(int type, WtEventCallback1 callback);

        bool DispatchTrigger(WtEventContext context);
        bool DispatchTrigger(int type);
        bool DispatchTrigger(int type, object data);
        bool DispatchTrigger(int type, object data, object initiator);
    }
}