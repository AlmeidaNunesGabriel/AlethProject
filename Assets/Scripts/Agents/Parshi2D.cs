using UnityEngine;

public class Parshi2D : Agent2D
{
    [Header("Collection System")]
    [SerializeField] private int armorsCollected = 0;

    [Header("Visual")]
    private SpriteRenderer spriteRenderer;
    private Color baseColor = Color.red;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisualAppearance();
    }

    public override void CollectResource(ResourceBase2D resource)
    {
        if (resource is Armor2D)
        {
            armorsCollected++;
            Debug.Log($"{name} coletou armadura #{armorsCollected}!");

            // Notifica o GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnParshiCollectedArmor();
            }

            UpdateVisualAppearance();
        }
        else if (resource is Gem2D)
        {
            // Parshi tamb�m pode coletar gemas se n�o houver armaduras
            Debug.Log($"{name} coletou gema (n�o � prefer�ncia)");
        }
    }

    private void UpdateVisualAppearance()
    {
        if (spriteRenderer == null) return;

        // Parshi fica mais escuro conforme coleta armaduras
        float collectionLevel = armorsCollected / 5f;
        Color evolvedColor = Color.Lerp(baseColor, Color.black, collectionLevel * 0.5f);
        spriteRenderer.color = evolvedColor;

        // Aumenta tamanho ligeiramente (menos que Alethi)
        float scaleMultiplier = 1f + (armorsCollected * 0.03f);
        transform.localScale = Vector3.one * Mathf.Min(scaleMultiplier, 1.3f);
    }

    public override void EngageCombat(Agent2D enemy)
    {
        if (enemy == null || enemy.IsDead()) return;

        float damage = config.damage;

        // Parshi tem dano base mais alto mas n�o evolui tanto quanto Alethi
        if (armorsCollected >= 3)
        {
            damage *= 1.2f; // 20% mais dano com 3+ armaduras
            Debug.Log($"{name} usa prote��o das armaduras! Dano aumentado para {damage}");
        }

        enemy.TakeDamage(damage);
        Debug.Log($"{name} (Armaduras: {armorsCollected}) atacou {enemy.name} causando {damage} de dano");
    }

    // M�todos para consulta
    public int GetArmorsCollected()
    {
        return armorsCollected;
    }

    // Override para mostrar informa��es visuais
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        // Mostra n�vel de coleta
        if (armorsCollected > 0)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, 0.4f + (armorsCollected * 0.05f));
        }
    }
}