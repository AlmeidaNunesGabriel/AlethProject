using UnityEngine;

public class SearchState2D : AgentState
{

    private Vector2 dest;
    public SearchState2D(Agent2D agent) : base(agent)
    {
    }

    public override void OnEnter()
    {
        PickDestination();
    }
    public override void Tick()
    {
        if (Vector2.Distance(agent .transform.position, dest)< 0.2f)
        {
            PickDestination();
        }
    }

    private void PickDestination()
    {
        // 1. Encontrar todas as gemas ativas
        Gem2D[] gems = Object.FindObjectsOfType<Gem2D>();

        if (gems.Length > 0)
        {
            // 2. Determinar a gema mais próxima
            float minDist = float.MaxValue;
            Transform nearest = null;
            Vector2 agentPos = agent.transform.position;

            foreach (var gem in gems)
            {
                float d = Vector2.Distance(agentPos, gem.transform.position);
                if (d < minDist)
                {
                    minDist = d;
                    nearest = gem.transform;
                }
            }

            // 3. Definir destino para essa gema
            dest = nearest.position;
        }
        else
        {
            var cfg = Object.FindAnyObjectByType<SpawnManager2D>().config;
            float x = Random.Range(-cfg.mapBounds.x, cfg.mapBounds.x);
            float y = Random.Range(-cfg.mapBounds.y, cfg.mapBounds.y);
            dest = new Vector2(x, y);
        }
        agent.MoveTo(dest);
    }
}
