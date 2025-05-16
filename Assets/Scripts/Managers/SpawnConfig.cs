using UnityEngine;

[CreateAssetMenu(fileName = "SpawnConfig", menuName = "Scriptable Objects/SpawnConfig")]
public class SpawnConfig : ScriptableObject
{
    [Range(0, 1)] public float gemSpawnRate;
    [Range(0, 1)] public float armorSpawnRate;
    public Vector2 mapBounds;
}
