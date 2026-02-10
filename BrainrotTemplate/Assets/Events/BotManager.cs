using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YG;

public class BotManager : MonoBehaviour
{
    public static BotManager Instance;

    [Header("Bots")]
    public GameObject[] botPrefabs; // Массив префабов ботов
    public int botCount = 14;
    public float mapRadius = 50f;
    public LayerMask groundMask;

    [Header("UI")]
    public TextMeshProUGUI playersCountText;
    public int maxPlayers = 15;

    private List<GameObject> spawnedBots = new List<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    public int TotalPlayers
    {
        get
        {
            return 1 + spawnedBots.Count;
        }
    }

    public void SpawnBots()
    {
        ClearBots();

        for (int i = 0; i < botCount; i++)
        {
            Vector3 pos = GetRandomPointOnMap();

            // Выбираем случайный префаб бота из массива
            GameObject randomBotPrefab = botPrefabs[Random.Range(0, botPrefabs.Length)];

            // Спавним выбранный префаб
            GameObject bot = Instantiate(randomBotPrefab, pos, Quaternion.identity);
            spawnedBots.Add(bot);
        }

        UpdatePlayersUI();
    }

    public void ClearBots()
    {
        for (int i = spawnedBots.Count - 1; i >= 0; i--)
        {
            if (spawnedBots[i] != null)
                Destroy(spawnedBots[i]);
        }

        spawnedBots.Clear();
        UpdatePlayersUI();
    }

    public void OnBotDied(GameObject bot)
    {
        if (spawnedBots.Contains(bot))
            spawnedBots.Remove(bot);

        UpdatePlayersUI();
    }

    void UpdatePlayersUI()
    {
        if (playersCountText == null) return;

        if (YG2.lang == "ru")
        {
            playersCountText.text = $"Игроков: {TotalPlayers} / {maxPlayers}";
        }
        if (YG2.lang == "en")
        {
            playersCountText.text = $"Players: {TotalPlayers} / {maxPlayers}";
        }
        if (YG2.lang == "tr")
        {
            playersCountText.text = $"Oyuncular: {TotalPlayers} / {maxPlayers}";
        }
    }

    Vector3 GetRandomPointOnMap()
    {
        for (int i = 0; i < 10; i++)
        {
            float x = Random.Range(-mapRadius, mapRadius);
            float z = Random.Range(-mapRadius, mapRadius);

            Vector3 origin = new Vector3(x, 100f, z);

            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 200f, groundMask))
            {
                return hit.point;
            }
        }

        return Vector3.zero;
    }
}
