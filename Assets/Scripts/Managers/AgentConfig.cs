using UnityEngine;

[CreateAssetMenu(fileName = "AgentConfig", menuName = "Scriptable Objects/AgentConfig")]
public class AgentConfig : ScriptableObject
{
    public float health;
    public float speed;
    public int damage;
    public float collectTime;     // segundos para coletar recurso
    public int groupCombatRatio;  // ex: 2 Alethi = 1 Parshi
}

