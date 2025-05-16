using UnityEngine;
using System.Collections;
public class CollectState2D : AgentState
{
    private ResourceBase2D resource;
    private bool isCollecting;
    private const float stopThreshold = 0.2f;
    public CollectState2D(Agent2D agent, ResourceBase2D res) : base(agent)
    {
        resource = res;
        isCollecting = false;
    }
    public override void OnEnter()
    {
        agent.MoveTo(resource.transform.position);
    }
    public override void Tick()
    {
        float dist = Vector2.Distance(agent.transform.position, resource.transform.position);
        if(!isCollecting && dist < stopThreshold) {
            isCollecting = true;
            // stop movement by setting target to current pos
            agent.MoveTo(agent.transform.position);
            agent.StartCoroutine(DoCollect());
        }
    }
    private IEnumerator DoCollect()
    {
        yield return new WaitForSeconds(agent.config.collectTime);
        agent.CollectResource(resource);
        GameObject.Destroy(resource.gameObject);
        agent.TransitionToState(new SearchState2D(agent));
    }
}
