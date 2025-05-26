using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject alethiPrefab;
    public GameObject parshiPrefab;

    [Header("Spawn Settings")]
    public Vector2 mapBounds = new Vector2(10f, 10f);
    public int agentsPerSpawn = 5; // Quantas gemas para spawnar novo agente

    [Header("Team Counters")]
    [SerializeField] private int alethiGemsCollected = 0;
    [SerializeField] private int parshiArmorsCollected = 0;
    [SerializeField] private int alethiAgentsCreated = 0;
    [SerializeField] private int parshiAgentsCreated = 0;

    // Singleton
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Spawna agentes iniciais
        SpawnInitialAgents();
    }

    void SpawnInitialAgents()
    {
        // Spawna alguns agentes iniciais
        for (int i = 0; i < 2; i++)
        {
            SpawnAlethi();
            SpawnParshi();
        }
    }

    public void OnAlethiCollectedGem()
    {
        alethiGemsCollected++;

        Debug.Log($"Alethi coletou gema! Total: {alethiGemsCollected}");

        // Verifica se deve spawnar novo agente
        int expectedAgents = (alethiGemsCollected / agentsPerSpawn) + 2; // +2 pelos iniciais
        if (expectedAgents > alethiAgentsCreated)
        {
            SpawnAlethi();
            Debug.Log($"Novo Alethi spawnado! Total de agentes: {alethiAgentsCreated}");
        }
    }

    public void OnParshiCollectedArmor()
    {
        parshiArmorsCollected++;

        Debug.Log($"Parshi coletou armadura! Total: {parshiArmorsCollected}");

        // Verifica se deve spawnar novo agente
        int expectedAgents = (parshiArmorsCollected / agentsPerSpawn) + 2; // +2 pelos iniciais
        if (expectedAgents > parshiAgentsCreated)
        {
            SpawnParshi();
            Debug.Log($"Novo Parshi spawnado! Total de agentes: {parshiAgentsCreated}");
        }
    }

    void SpawnAlethi()
    {
        if (alethiPrefab == null)
        {
            Debug.LogError("Alethi Prefab não configurado no GameManager!");
            return;
        }

        Vector2 spawnPosition = GetRandomSpawnPosition();
        GameObject newAlethi = Instantiate(alethiPrefab, spawnPosition, Quaternion.identity);
        newAlethi.name = $"Alethi_{alethiAgentsCreated + 1}";

        alethiAgentsCreated++;
    }

    void SpawnParshi()
    {
        if (parshiPrefab == null)
        {
            Debug.LogError("Parshi Prefab não configurado no GameManager!");
            return;
        }

        Vector2 spawnPosition = GetRandomSpawnPosition();
        GameObject newParshi = Instantiate(parshiPrefab, spawnPosition, Quaternion.identity);
        newParshi.name = $"Parshi_{parshiAgentsCreated + 1}";

        parshiAgentsCreated++;
    }

    Vector2 GetRandomSpawnPosition()
    {
        return new Vector2(
            Random.Range(-mapBounds.x, mapBounds.x),
            Random.Range(-mapBounds.y, mapBounds.y)
        );
    }

    // Getters para outras classes
    public int GetAlethiGemsCollected() { return alethiGemsCollected; }
    public int GetParshiArmorsCollected() { return parshiArmorsCollected; }
    public int GetAlethiAgentsCount() { return alethiAgentsCreated; }
    public int GetParshiAgentsCount() { return parshiAgentsCreated; }

    // Debug info
    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 200, 100), "Team Stats");
        GUI.Label(new Rect(20, 30, 180, 20), $"Alethi Gemas: {alethiGemsCollected}");
        GUI.Label(new Rect(20, 50, 180, 20), $"Alethi Agentes: {alethiAgentsCreated}");
        GUI.Label(new Rect(20, 70, 180, 20), $"Parshi Armaduras: {parshiArmorsCollected}");
        GUI.Label(new Rect(20, 90, 180, 20), $"Parshi Agentes: {parshiAgentsCreated}");
    }
}