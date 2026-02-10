using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YG;

public class EventManager : MonoBehaviour
{
    public List<WorldEvent> events;
    public TextMeshProUGUI eventText;

    public GameObject winGamePanel;
    public GameObject loseGamePanel;
    public int maxEvents = 5;

    int currentEventIndex = 0;
    int playedEvents = 0;

    public int timeBeforeStart = 10;

    public GameManager gameManager;

    [Header("Coin Spawn")]
    public Transform[] coinSpawnPoints;
    public GameObject coinPrefab;
    public int coinsToSpawn = 4;
    private Coroutine currentEventCoroutine;
    public void StartRound()
    {
        ShuffleEvents();
        currentEventIndex = 0;
        playedEvents = 0;

        winGamePanel.SetActive(false);

        BotManager.Instance.SpawnBots();
        SpawnCoins();

        StartCoroutine(EventWarning());
    }

    void SpawnCoins()
    {
        GameObject[] existingCoins = GameObject.FindGameObjectsWithTag("Coin");
        foreach (GameObject coin in existingCoins)
            Destroy(coin);

        List<int> usedIndexes = new List<int>();
        int spawned = 0;

        while (spawned < coinsToSpawn && spawned < coinSpawnPoints.Length)
        {
            int index = Random.Range(0, coinSpawnPoints.Length);

            if (usedIndexes.Contains(index))
                continue;

            usedIndexes.Add(index);

            GameObject newCoin = Instantiate(
                coinPrefab,
                coinSpawnPoints[index].position,
                Quaternion.identity
            );

            newCoin.tag = "Coin"; 

            spawned++;
        }
    }

    IEnumerator EventWarning()
    {
        if (YG2.lang == "ru")
            eventText.text = "Переживи 3 события";
        else if (YG2.lang == "en")
            eventText.text = "Survive 3 events";
        else if (YG2.lang == "tr")
            eventText.text = "3 olayı atlat.";

        yield return new WaitForSeconds(5);

        StartCoroutine(EventSequence());
    }


    IEnumerator EventSequence()
    {
        while (currentEventIndex < events.Count && playedEvents < maxEvents)
        {
            WorldEvent e = events[currentEventIndex];

            eventText.color = Color.red;
            eventText.gameObject.SetActive(true);

            for (int i = timeBeforeStart; i > 0; i--)
            {
                if (YG2.lang == "ru")
                    eventText.text = e.eventNameRu + " через " + i;
                else if (YG2.lang == "en")
                    eventText.text = e.eventNameEng + " start in " + i;
                else if (YG2.lang == "tr")
                    eventText.text = e.eventNameTr + " başlayacak " + i;

                yield return new WaitForSeconds(1f);
            }

            e.StartEvent();

            currentEventCoroutine = StartCoroutine(EventTimer(e));
            yield return currentEventCoroutine;

            eventText.gameObject.SetActive(false);
            e.EndEvent();

            currentEventIndex++;
            playedEvents++;
        }

        EndGame();
    }

    IEnumerator EventTimer(WorldEvent e)
    {
        for (int i = Mathf.CeilToInt(e.duration); i > 0; i--)
        {
            eventText.text = "" + i;
            yield return new WaitForSeconds(1f);
        }
    }

    void EndGame()
    {
        StopAllCoroutines();
        BotManager.Instance.ClearBots();

        PlayerStats.instance.cups += 1;
        PlayerStats.instance.UpdateText();

        winGamePanel.SetActive(true);
        gameManager.Lobby();

        StartCoroutine(Delay(3));
    }

    public IEnumerator Delay(int sec)
    {
        yield return new WaitForSeconds(sec);
        winGamePanel.SetActive(false);
        loseGamePanel.SetActive(false);
    }

    void ShuffleEvents()
    {
        for (int i = 0; i < events.Count; i++)
        {
            int randomIndex = Random.Range(i, events.Count);
            WorldEvent temp = events[i];
            events[i] = events[randomIndex];
            events[randomIndex] = temp;
        }
    }

    public void PlayerDeath()
    {
        if (currentEventCoroutine != null)
            StopCoroutine(currentEventCoroutine);

        StopAllCoroutines();

        eventText.gameObject.SetActive(false);

        if (currentEventIndex < events.Count && currentEventIndex >= 0)
            events[currentEventIndex].EndEvent();

        BotManager.Instance.ClearBots();

        loseGamePanel.SetActive(true);

        currentEventIndex = 0;
        playedEvents = 0;

        StartCoroutine(Delay(4));
    }


    private void OnEnable()
    {
        Health.onPlayerDied += PlayerDeath;
    }

    private void OnDisable()
    {
        Health.onPlayerDied -= PlayerDeath;
    }
}
