using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class PlayerStats : MonoBehaviour, IUpdateText
{
    public static PlayerStats instance;

    public int cups;
    public Text cupsText;
    public int coins;
    public TMP_Text coinsText;
    public int gems;
    public TMP_Text gemsText;


    public void UpdateText()
    {
        coinsText.text = coins.ToString();
        cupsText.text = cups.ToString();
        gemsText.text = gems.ToString();
    }

    public void AddCoins(int count, string valueName)
    {
        if (valueName == "coins")
        {
            coins += count;
        }
        if (valueName == "gems")
        {
            gems += count;
        }

        Save();
        UpdateText();
    }

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Load();
    }

    private void OnEnable()
    {
        YG2.onGetSDKData += Load;
        YG2.onPurchaseSuccess += SuccessPurchased;
    }

    private void OnDisable()
    {
        YG2.onGetSDKData -= Load;
        YG2.onPurchaseFailed -= FailedPurchased;
    }

    void Load()
    {
        cups = YG2.saves.cups;
        coins = YG2.saves.coins;
        gems = YG2.saves.gems;

        UpdateText();
    }

    public void Save()
    {
        YG2.saves.cups = cups;
        YG2.saves.coins = coins;
        YG2.saves.gems = gems;

        YG2.SetLeaderboard("wins", cups);

        YG2.SaveProgress();
    }

    public void AddCoinsTest(int gold)
    {
        coins += gold;
        UpdateText();
    }

    private void SuccessPurchased(string id)
    {
        if (id == "gems")
        {
            gems += 100;
            Save();
            UpdateText();
        }
        if (id == "gems1000")
        {
            gems += 1000;
            Save();
            UpdateText();
        }
    }
    private void FailedPurchased(string id)
    {
        // ������� �� ���� ���������
    }

}
