using UnityEngine;
using TMPro;
using YG;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour, IUpdateText
{
    public static PlayerStats instance;

    public int cups;
    public Text cupsText;
    public int coins;
    public TMP_Text coinsText;
    public int gems;
    public TMP_Text gemsText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Сохраняем объект при загрузке новых сцен
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Load();
    }

    private void OnEnable()
    {
        YG2.onGetSDKData += Load;
        YG2.onPurchaseSuccess += SuccessPurchased;
        YG2.onPurchaseFailed += FailedPurchased;
    }

    private void OnDisable()
    {
        YG2.onGetSDKData -= Load;
        YG2.onPurchaseSuccess -= SuccessPurchased;
        YG2.onPurchaseFailed -= FailedPurchased; // Добавлено отписывание от события неуспешной покупки
    }

    public void UpdateText()
    {
        coinsText.text = coins.ToString();
        cupsText.text = cups.ToString();
        gemsText.text = gems.ToString();
    }

    public void AddCoins(int count, string valueName)
    {
        switch (valueName)
        {
            case "coins":
                coins += count;
                break;
            case "gems":
                gems += count;
                break;
            default:
                Debug.LogWarning("Unknown value name: " + valueName);
                break;
        }

        Save();
        UpdateText();
    }

    public void AddCoinsTest(int gold)
    {
        coins += gold;
        Save(); // Добавлено сохранение после изменения монет
        UpdateText();
    }

    void Load()
    {
        if (YG2.saves != null) // Проверка на null
        {
            cups = YG2.saves.cups;
            coins = YG2.saves.coins;
            gems = YG2.saves.gems;
        }

        UpdateText();
    }

    public void Save()
    {
        if (YG2.saves != null) // Проверка на null
        {
            YG2.saves.cups = cups;
            YG2.saves.coins = coins;
            YG2.saves.gems = gems;
        }

        YG2.SetLeaderboard("wins", cups);
        YG2.SaveProgress();
    }

    public void SuccessPurchased(string id)
    {
        switch (id)
        {
            case "gems":
                gems += 100;
                break;
            case "gems1000":
                gems += 1000;
                break;
            default:
                Debug.LogWarning("Unknown purchase ID: " + id);
                break;
        }

        Save();
        UpdateText();
    }

    private void FailedPurchased(string id)
    {
        Debug.LogError("Purchase failed for ID: " + id);
    }
}
