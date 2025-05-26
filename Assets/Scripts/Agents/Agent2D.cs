using UnityEngine;

public abstract class Agent2D : MonoBehaviour
{
    [Header("Configuration")]
    public AgentConfig config;

    [Header("Runtime Status")]
    [SerializeField] protected float currentHealth;
    [SerializeField] private bool isDead;

    // State Machine
    private AgentState currentState;
    private AgentState pendingState; // Para transições seguras

    // Movement
    private Rigidbody2D rb;
    private Vector2 targetPosition;
    private bool isMoving;

    // Debug
    [Header("Debug Info")]
    [SerializeField] private string currentStateName;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (config == null)
        {
            Debug.LogError($"{name} não possui AgentConfig configurado!");
            return;
        }

        InitializeAgent();
    }

    protected virtual void Start()
    {
        // Inicia no estado de busca
        TransitionToState(new SearchState2D(this));
    }

    protected virtual void Update()
    {
        // Processa transição pendente no início do frame
        if (pendingState != null)
        {
            ProcessStateTransition();
        }

        // Executa estado atual
        if (currentState != null && !isDead)
        {
            currentState.Tick();
        }

        // Atualiza info de debug
        UpdateDebugInfo();
    }

    protected virtual void FixedUpdate()
    {
        ProcessMovement();
    }

    private void InitializeAgent()
    {
        currentHealth = config.health;
        isDead = false;
        isMoving = false;

        // Configurações do Rigidbody2D
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
    }

    public void TransitionToState(AgentState newState)
    {
        if (newState == null)
        {
            Debug.LogWarning($"{name} tentou transicionar para estado nulo!");
            return;
        }

        // Agenda transição para ser processada no próximo Update
        pendingState = newState;
    }

    private void ProcessStateTransition()
    {
        if (pendingState == null) return;

        // Sai do estado atual
        if (currentState != null)
        {
            currentState.OnExit();
        }

        // Muda para novo estado
        currentState = pendingState;
        pendingState = null;

        // Entra no novo estado
        currentState.OnEnter();

        Debug.Log($"{name} transicionou para {currentState.GetType().Name}");
    }

    public void MoveTo(Vector2 position)
    {
        if (isDead) return;

        targetPosition = position;
        isMoving = Vector2.Distance(transform.position, targetPosition) > 0.1f;
    }

    private void ProcessMovement()
    {
        if (!isMoving || isDead || rb == null) return;

        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * config.speed;

        // Para quando está próximo o suficiente
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
            rb.linearVelocity = Vector2.zero;
        }
    }

    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"{name} recebeu {damage} de dano. Vida: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        currentHealth = 0;

        // Para o movimento
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Sai do estado atual
        if (currentState != null)
        {
            currentState.OnExit();
            currentState = null;
        }

        Debug.Log($"{name} morreu!");

        // Opcional: destruir após alguns segundos
        Destroy(gameObject, 2f);
    }

    public bool IsDead()
    {
        return isDead;
    }

    public float GetHealthPercentage()
    {
        return config != null ? currentHealth / config.health : 0f;
    }

    public void AdjustCurrentHealth(float newHealth)
    {
        currentHealth = Mathf.Max(0f, newHealth);
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    // Métodos abstratos para serem implementados pelas subclasses
    public abstract void EngageCombat(Agent2D enemy);
    public abstract void CollectResource(ResourceBase2D resource);

    // Detecção de colisão para recursos e inimigos
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        // Não processa colisões se já está em combate
        if (currentState is CombatState2D) return;

        // Verifica por recursos
        ResourceBase2D resource = other.GetComponent<ResourceBase2D>();
        if (resource != null && !(currentState is CollectState2D))
        {
            TransitionToState(new CollectState2D(this, resource));
            return;
        }

        // Verifica por inimigos
        Agent2D otherAgent = other.GetComponent<Agent2D>();
        if (otherAgent != null && !otherAgent.IsDead() && IsEnemy(otherAgent))
        {
            TransitionToState(new CombatState2D(this, otherAgent));
        }
    }

    private bool IsEnemy(Agent2D otherAgent)
    {
        // Alethi vs Parshi
        return (this is Alethi2D && otherAgent is Parshi2D) ||
               (this is Parshi2D && otherAgent is Alethi2D);
    }

    private void UpdateDebugInfo()
    {
        currentStateName = currentState?.GetType().Name ?? "Nenhum";
    }

    // Método para debug visual
    protected virtual void OnDrawGizmos()
    {
        if (isMoving)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetPosition);
            Gizmos.DrawWireSphere(targetPosition, 0.2f);
        }

        // Mostra área de detecção
        Gizmos.color = isDead ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}