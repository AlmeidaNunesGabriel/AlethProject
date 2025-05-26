using UnityEngine;

public class CollectState2D : AgentState
{
    private ResourceBase2D targetResource;
    private float collectTimer;
    private float stopThreshold = 0.5f;

    public CollectState2D(Agent2D agent, ResourceBase2D resource) : base(agent)
    {
        targetResource = resource;
    }

    public override void OnEnter()
    {
        collectTimer = 0f;

        // Verificação inicial - se o recurso já foi destruído, volta para busca
        if (targetResource == null)
        {
            agent.TransitionToState(new SearchState2D(agent));
            return;
        }

        Debug.Log($"{agent.name} iniciando coleta de {targetResource.name}");
    }

    public override void Tick()
    {
        // Verificação contínua - se o recurso foi destruído durante a coleta
        if (targetResource == null)
        {
            Debug.Log($"{agent.name} perdeu o recurso alvo, voltando para busca");
            agent.TransitionToState(new SearchState2D(agent));
            return;
        }

        float distanceToResource = Vector2.Distance(agent.transform.position, targetResource.transform.position);

        // Se ainda não chegou no recurso, continua se movendo
        if (distanceToResource > stopThreshold)
        {
            agent.MoveTo(targetResource.transform.position);
            return;
        }

        // Parou próximo ao recurso, inicia contagem de coleta
        agent.MoveTo(agent.transform.position); // Para o movimento

        collectTimer += Time.deltaTime;

        // Verifica novamente se o recurso ainda existe antes de coletar
        if (collectTimer >= agent.config.collectTime)
        {
            if (targetResource != null) // Verificação final antes de coletar
            {
                agent.CollectResource(targetResource);
                Debug.Log($"{agent.name} coletou {targetResource.name}");

                // Destrói o recurso após coleta bem-sucedida
                Object.Destroy(targetResource.gameObject);
            }

            // Volta para busca independentemente
            agent.TransitionToState(new SearchState2D(agent));
        }
    }

    public override void OnExit()
    {
        collectTimer = 0f;
        Debug.Log($"{agent.name} saindo do estado de coleta");
    }
}