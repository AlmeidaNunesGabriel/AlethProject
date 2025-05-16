using UnityEngine;

public class CombatState2D : AgentState
{
    private Agent2D enemy;
    public CombatState2D(Agent2D agent, Agent2D enemy) : base(agent)
    {
        this.enemy = enemy;
    }
    public override void OnEnter()
    {
        agent.MoveTo(enemy.transform.position);
    }
    public override void Tick()
    {
        if (Vector2.Distance(agent.transform.position, enemy.transform.position) < 0.2f)
        {
            agent.EngageCombat(enemy);
            // Agora usamos IsDead() para verificar estado de vida
            if (enemy.IsDead())
            {
                agent.TransitionToState(new SearchState2D(agent));
            }
        }
    }
}