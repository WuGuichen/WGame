using UnityEngine;

public interface IAiAgentService : IBaseService
{
    public bool IsActing { get; set; }
    public void StartPath(Vector3 targetPosition);
    public void UpdateFSM();
    public void TriggerFSM(int type);
    public void TriggerFSM(string name,int type);
    public MoveAgent MoveAgent { get; }
    public FSMAgent FSMAgent { get; }
}
