using UnityEngine;

public class SpawnManager2D : MonoBehaviour
{
    public SpawnConfig config;
    public GameObject gemPrefab, armorPrefab;

    private void Start()
    {
        InvokeRepeating(nameof(TrySpawn), 1f, 2f);
    }

    void TrySpawn()
    {
        float r = Random.value;
        if (r < config.armorSpawnRate) Spawn(armorPrefab);
        else if(r < config.armorSpawnRate + config.gemSpawnRate) Spawn(gemPrefab);
    }

    void Spawn(GameObject prefab)
    {
        float x = Random.Range(-config.mapBounds.x, config.mapBounds.x);
        float y = Random.Range(-config.mapBounds.y, config.mapBounds.y);
        Instantiate(prefab, new Vector3 (x, y, 0), Quaternion.identity);
    }
}
