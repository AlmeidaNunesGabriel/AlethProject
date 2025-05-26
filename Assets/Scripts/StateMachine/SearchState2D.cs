using UnityEngine;

public class SearchState2D : AgentState
{
    private Vector2 currentDestination;
    private bool hasDestination;
    private float searchRadius = 10f;
    private float destinationThreshold = 0.5f;

    public SearchState2D(Agent2D agent) : base(agent) { }

    public override void OnEnter()
    {
        hasDestination = false;
        FindNewTarget();
        Debug.Log($"{agent.name} iniciando busca");
    }

    public override void Tick()
    {
        // Sempre procura por recursos primeiro (prioridade alta)
        ResourceBase2D nearestResource = FindNearestResource();

        if (nearestResource != null)
        {
            // Encontrou um recurso válido, muda para coleta
            agent.TransitionToState(new CollectState2D(agent, nearestResource));
            return;
        }

        // Verifica por inimigos próximos
        Agent2D nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            float distanceToEnemy = Vector2.Distance(agent.transform.position, nearestEnemy.transform.position);
            if (distanceToEnemy <= searchRadius)
            {
                agent.TransitionToState(new CombatState2D(agent, nearestEnemy));
                return;
            }
        }

        // Nenhum recurso ou inimigo encontrado, continua patrulhando
        if (!hasDestination || Vector2.Distance(agent.transform.position, currentDestination) < destinationThreshold)
        {
            FindNewRandomDestination();
        }

        agent.MoveTo(currentDestination);
    }

    public override void OnExit()
    {
        Debug.Log($"{agent.name} saindo do estado de busca");
    }

    private ResourceBase2D FindNearestResource()
    {
        ResourceBase2D nearestResource = null;
        float nearestDistance = float.MaxValue;

        // Busca por gemas (prioridade para Alethi)
        Gem2D[] gems = Object.FindObjectsByType<Gem2D>(FindObjectsSortMode.None);
        foreach (Gem2D gem in gems)
        {
            if (gem != null) // Verificação de segurança
            {
                float distance = Vector2.Distance(agent.transform.position, gem.transform.position);
                if (distance < nearestDistance && ShouldCollectResource(gem))
                {
                    nearestDistance = distance;
                    nearestResource = gem;
                }
            }
        }

        // Busca por armaduras (prioridade para Parshi)
        Armor2D[] armors = Object.FindObjectsByType<Armor2D>(FindObjectsSortMode.None);
        foreach (Armor2D armor in armors)
        {
            if (armor != null) // Verificação de segurança
            {
                float distance = Vector2.Distance(agent.transform.position, armor.transform.position);
                if (distance < nearestDistance && ShouldCollectResource(armor))
                {
                    nearestDistance = distance;
                    nearestResource = armor;
                }
            }
        }

        return nearestResource;
    }

    private bool ShouldCollectResource(ResourceBase2D resource)
    {
        // Alethi prefere gemas, Parshi prefere armaduras
        if (agent is Alethi2D && resource is Gem2D)
            return true;
        if (agent is Parshi2D && resource is Armor2D)
            return true;

        // Se não há recurso preferido disponível, coleta qualquer um
        return true;
    }

    private Agent2D FindNearestEnemy()
    {
        Agent2D nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        Agent2D[] allAgents = Object.FindObjectsByType<Agent2D>(FindObjectsSortMode.None);

        foreach (Agent2D otherAgent in allAgents)
        {
            if (otherAgent != null && otherAgent != agent && !otherAgent.IsDead())
            {
                // Verifica se é inimigo (Alethi vs Parshi)
                bool isEnemy = (agent is Alethi2D && otherAgent is Parshi2D) ||
                              (agent is Parshi2D && otherAgent is Alethi2D);

                if (isEnemy)
                {
                    float distance = Vector2.Distance(agent.transform.position, otherAgent.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestEnemy = otherAgent;
                    }
                }
            }
        }

        return nearestEnemy;
    }

    private void FindNewTarget()
    {
        ResourceBase2D resource = FindNearestResource();
        if (resource != null)
        {
            currentDestination = resource.transform.position;
            hasDestination = true;
        }
        else
        {
            FindNewRandomDestination();
        }
    }

    private void FindNewRandomDestination()
    {
        SpawnManager2D spawnManager = Object.FindAnyObjectByType<SpawnManager2D>();
        Vector2 mapBounds = spawnManager != null ? spawnManager.config.mapBounds : new Vector2(10f, 10f);

        currentDestination = new Vector2(
            Random.Range(-mapBounds.x, mapBounds.x),
            Random.Range(-mapBounds.y, mapBounds.y)
        );

        hasDestination = true;
    }
}