using UnityEngine;

[CreateAssetMenu(fileName = "NewAgentConfig", menuName = "Configs/AgentConfig")]
public class AgentConfig : ScriptableObject
{
    [Header("Base Stats")]
    public float health = 100f;
    public float speed = 3f;
    public int damage = 10;
    public float collectTime = 2f;
    public int groupCombatRatio = 2;

    [Header("Balance Notes")]
    [TextArea(3, 5)]
    public string notes = "Configurações base do agente. Alethi evoluem com gemas coletadas.";

    private void OnValidate()
    {
        // Garante valores mínimos
        health = Mathf.Max(1f, health);
        speed = Mathf.Max(0.1f, speed);
        damage = Mathf.Max(1, damage);
        collectTime = Mathf.Max(0.1f, collectTime);
        groupCombatRatio = Mathf.Max(1, groupCombatRatio);
    }
}