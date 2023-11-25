using UnityEngine;
using UnityHFSM;

public interface IAiAgentService
{
    public bool IsActing { get; set; }
    public void StartPath(Vector3 targetPosition);

    public void UpdateFSM();
    public void OnDetectCharacter(GameEntity entity);
    public void TriggerFSM(int type);
    public void TriggerFSM(string name,int type);
    public void SetFSM(string name, bool isRefresh = false);
    public void SetBTree(string name);
    public void UpdateBTree(string name);
    public void RemoveFSM(string name);
    public StateMachine<int,int,int> GetFSM(string name);
    public MoveAgent MoveAgent { get; }
    public FightAgent FightAgent { get; }
}
