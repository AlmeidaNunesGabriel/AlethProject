using UnityEngine;

public class Alethi2D : Agent2D
{
    [Header("Evolution System")]
    [SerializeField] private int gemsCollected = 0;
    [SerializeField] private float baseHealth;
    [SerializeField] private float baseSpeed;
    [SerializeField] private int baseDamage;

    [Header("Evolution Bonuses")]
    [SerializeField] private float healthBonusPerGem = 10f;
    [SerializeField] private float speedBonusPerGem = 0.2f;
    [SerializeField] private int damageBonusPerGem = 2;

    [Header("Visual Evolution")]
    private SpriteRenderer spriteRenderer;
    private Color baseColor = Color.blue;

    protected override void Awake()
    {
        base.Awake();

        // Armazena stats base
        if (config != null)
        {
            baseHealth = config.health;
            baseSpeed = config.speed;
            baseDamage = config.damage;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisualAppearance();
    }

    public override void CollectResource(ResourceBase2D resource)
    {
        if (resource is Gem2D)
        {
            gemsCollected++;
            Debug.Log($"{name} coletou gema #{gemsCollected}!");

            // Evolui o agente
            EvolveAgent();

            // Notifica o GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnAlethiCollectedGem();
            }

            UpdateVisualAppearance();
        }
        else if (resource is Armor2D)
        {
            // Alethi também pode coletar armaduras se não houver gemas
            Debug.Log($"{name} coletou armadura (não é preferência)");
        }
    }

    private void EvolveAgent()
    {
        if (config == null) return;

        // Calcula novos stats baseados nas gemas coletadas
        float newHealth = baseHealth + (healthBonusPerGem * gemsCollected);
        float newSpeed = baseSpeed + (speedBonusPerGem * gemsCollected);
        int newDamage = baseDamage + (damageBonusPerGem * gemsCollected);

        // Atualiza o config (cria uma cópia para não afetar outros agentes)
        AgentConfig evolvedConfig = ScriptableObject.CreateInstance<AgentConfig>();
        evolvedConfig.health = newHealth;
        evolvedConfig.speed = newSpeed;
        evolvedConfig.damage = newDamage;
        evolvedConfig.collectTime = config.collectTime;
        evolvedConfig.groupCombatRatio = config.groupCombatRatio;

        config = evolvedConfig;

        // Atualiza vida atual proporcionalmente (usando método público)
        float healthRatio = GetHealthPercentage();
        // A vida atual será ajustada através de um método público
        AdjustCurrentHealth(newHealth * healthRatio);

        Debug.Log($"{name} evoluiu! Vida: {newHealth}, Velocidade: {newSpeed}, Dano: {newDamage}");
    }

    private void UpdateVisualAppearance()
    {
        if (spriteRenderer == null) return;

        // Muda cor baseado no nível de evolução
        float evolutionLevel = gemsCollected / 5f; // Normalizado para cada 5 gemas

        // Cor evolui de azul para dourado
        Color evolvedColor = Color.Lerp(baseColor, Color.yellow, evolutionLevel);
        spriteRenderer.color = evolvedColor;

        // Aumenta tamanho ligeiramente
        float scaleMultiplier = 1f + (gemsCollected * 0.05f);
        transform.localScale = Vector3.one * Mathf.Min(scaleMultiplier, 1.5f); // Máximo 150% do tamanho
    }

    public override void EngageCombat(Agent2D enemy)
    {
        if (enemy == null || enemy.IsDead()) return;

        float damage = config.damage;

        // Bônus de dano baseado na evolução
        if (gemsCollected >= 5)
        {
            damage *= 1.5f; // 50% mais dano com 5+ gemas
            Debug.Log($"{name} usa poder das gemas! Dano aumentado para {damage}");
        }

        enemy.TakeDamage(damage);
        Debug.Log($"{name} (Gemas: {gemsCollected}) atacou {enemy.name} causando {damage} de dano");
    }

    // Método para outros sistemas consultarem o nível de evolução
    public int GetGemsCollected()
    {
        return gemsCollected;
    }

    public float GetEvolutionLevel()
    {
        return gemsCollected / 5f; // Retorna nível baseado em grupos de 5 gemas
    }

    public bool IsEvolved()
    {
        return gemsCollected >= 5;
    }

    // Override para mostrar informações de evolução
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        // Mostra nível de evolução
        if (gemsCollected > 0)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.5f + (gemsCollected * 0.1f));
        }
    }
}