using UnityEngine;

public class AgentState
{
    protected Agent2D agent;

    public AgentState(Agent2D agent)
    {
        this.agent = agent;
    }

    public virtual void OnEnter() { }
    public virtual void Tick() { }
    public virtual void OnExit() { }
}
