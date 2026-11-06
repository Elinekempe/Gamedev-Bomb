using UnityEngine;

[System.Serializable]
public class BombSpawner
{
    [Header("Spawnen")]
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private Transform bombContainer;
    [SerializeField] private float spawnInterval;
    [SerializeField] private float spawnXMin;
    [SerializeField] private float spawnXMax;
    [SerializeField] private float spawnY;
    [SerializeField] private float bottomY;

    [Header("Bom-verdeling (gewichten)")]
    [SerializeField] private int realWeight;
    [SerializeField] private int fakeWeight;
    [SerializeField] private int goldWeight;
    [SerializeField] private int bigWeight;

    [Header("Stats per type")]
    [SerializeField] private BombStats realStats;
    [SerializeField] private BombStats fakeStats;
    [SerializeField] private BombStats goldStats;
    [SerializeField] private BombStats bigStats;

    private float timer;

    public void Tick(GameManager gameManager, float deltaTime)
    {
        if (bombPrefab == null || gameManager == null) return;

        // De timer voorkomt dat er iedere frame een nieuwe bom verschijnt.
        timer -= deltaTime;
        if (timer > 0f) return;

        SpawnBomb(gameManager);
        timer = spawnInterval;
    }

    private void SpawnBomb(GameManager gameManager)
    {
        // Iedere bom krijgt een willekeurig type met een instelbare spawnkans.
        float x = Random.Range(spawnXMin, spawnXMax);
        GameObject bombObject = Object.Instantiate(
            bombPrefab,
            new Vector3(x, spawnY, 0f),
            Quaternion.identity,
            bombContainer);

        Bomb bomb = bombObject.GetComponent<Bomb>();
        if (bomb != null)
        {
            bomb.Init(PickRandomStats(), gameManager, bottomY);
        }
    }

    private BombStats PickRandomStats()
    {
        // De gewichten bepalen de verhouding tussen de verschillende bomtypes.
        int totalWeight = realWeight + fakeWeight + goldWeight + bigWeight;
        if (totalWeight <= 0) return realStats;

        int randomValue = Random.Range(0, totalWeight);
        if (randomValue < realWeight) return realStats;

        randomValue -= realWeight;
        if (randomValue < fakeWeight) return fakeStats;

        randomValue -= fakeWeight;
        if (randomValue < goldWeight) return goldStats;

        return bigStats;
    }
}