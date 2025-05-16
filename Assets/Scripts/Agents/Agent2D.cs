using UnityEditorInternal;
using UnityEngine;

public abstract class Agent2D : MonoBehaviour
{
    public AgentConfig config;
    protected Rigidbody2D rb;
    protected float currentHealth;
    protected AgentState currentState;
    protected Vector2 target;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = config.health;
        TransitionToState(new SearchState2D(this));
    }

    protected virtual void Update()
    {
        currentState?.Tick();
    }

    protected virtual void FixedUpdate()
    {
        if (target != (Vector2)rb.position)
        {
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, config.speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }
    }
    public void TransitionToState(AgentState newState)
    {
        currentState?.OnExit();
        currentState = newState;
        currentState?.OnEnter();
    }
    public void MoveTo(Vector2 destination)
    {
        target = destination;
    }
    public abstract void EngageCombat (Agent2D other);
    public abstract void CollectResource(ResourceBase2D resource);

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;
    }
    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<ResourceBase2D>(out var res))
        {
            TransitionToState(new CollectState2D(this, res));
        }
        else if (other.TryGetComponent<Agent2D>(out var ag) && ag.GetType() != this.GetType())
        {
            TransitionToState(new CombatState2D(this, ag));
        }
    }
}