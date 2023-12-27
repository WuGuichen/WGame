using UnityEngine;
using UnityHFSM;

public interface IAiAgentService
{
    public bool IsActing { get; set; }
    public void StartPath(Vector3 targetPosition);
    public void Destroy();
    public void UpdateFSM();
    public void OnDetectCharacter(GameEntity entity);
    public void TriggerFSM(int type);
    public void TriggerFSM(string name,int type);
    public void SetBTree(string name);
    public void TickBTree(string name);
    public MoveAgent MoveAgent { get; }
    public FightAgent FightAgent { get; }
    public FSMAgent FSMAgent { get; }
}
