using UnityEngine;

public class CombatState2D : AgentState
{
    private Agent2D targetEnemy;
    private float attackThreshold = 1.0f;
    private float combatTimeout = 10f; // Timeout para evitar combate infinito
    private float combatTimer;

    public CombatState2D(Agent2D agent, Agent2D enemy) : base(agent)
    {
        targetEnemy = enemy;
    }

    public override void OnEnter()
    {
        combatTimer = 0f;

        // Verifica��o inicial - se o inimigo j� foi destru�do, volta para busca
        if (targetEnemy == null || targetEnemy.IsDead())
        {
            agent.TransitionToState(new SearchState2D(agent));
            return;
        }

        Debug.Log($"{agent.name} iniciando combate com {targetEnemy.name}");
    }

    public override void Tick()
    {
        combatTimer += Time.deltaTime;

        // Timeout de combate para evitar loops infinitos
        if (combatTimer >= combatTimeout)
        {
            Debug.Log($"{agent.name} timeout de combate, voltando para busca");
            agent.TransitionToState(new SearchState2D(agent));
            return;
        }

        // Verifica��o cont�nua - se o inimigo foi destru�do ou morreu
        if (targetEnemy == null || targetEnemy.IsDead())
        {
            Debug.Log($"{agent.name} inimigo derrotado ou desapareceu, voltando para busca");
            agent.TransitionToState(new SearchState2D(agent));
            return;
        }

        float distanceToEnemy = Vector2.Distance(agent.transform.position, targetEnemy.transform.position);

        // Se o inimigo est� muito longe, desiste do combate
        if (distanceToEnemy > attackThreshold * 3f)
        {
            Debug.Log($"{agent.name} inimigo muito longe, voltando para busca");
            agent.TransitionToState(new SearchState2D(agent));
            return;
        }

        // Se ainda n�o chegou no inimigo, continua se movendo
        if (distanceToEnemy > attackThreshold)
        {
            agent.MoveTo(targetEnemy.transform.position);
            return;
        }

        // Est� pr�ximo o suficiente, executa ataque
        agent.MoveTo(agent.transform.position); // Para o movimento

        // Verifica novamente se o inimigo ainda existe antes de atacar
        if (targetEnemy != null && !targetEnemy.IsDead())
        {
            agent.EngageCombat(targetEnemy);
            Debug.Log($"{agent.name} atacou {targetEnemy.name}");

            // Verifica se o inimigo morreu ap�s o ataque
            if (targetEnemy.IsDead())
            {
                Debug.Log($"{agent.name} derrotou {targetEnemy.name}");
                agent.TransitionToState(new SearchState2D(agent));
                return;
            }
        }
        else
        {
            // Inimigo desapareceu durante o ataque
            agent.TransitionToState(new SearchState2D(agent));
        }
    }

    public override void OnExit()
    {
        combatTimer = 0f;
        Debug.Log($"{agent.name} saindo do estado de combate");
    }
}