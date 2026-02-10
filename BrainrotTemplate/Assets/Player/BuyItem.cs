using System;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class BuyItem : MonoBehaviour
{
    [SerializeField] private int id; // Теперь id — это int
    public int price;
    public GameObject Target;
    public bool isPurchased;

    private void Start()
    {
        // Если id не задан в инспекторе, присваиваем уникальный (например, хэш от имени объекта)
        if (id == 0)
        {
            id = gameObject.name.GetHashCode();
            Debug.Log($"Generated new ID: {id}");
            YG2.saves.SaveData(id, isPurchased);
            YG2.SaveProgress();
        }

        LoadItem();
    }

    public void PurchaseForCoin()
    {
        if (!isPurchased)
        {
            if (PlayerStats.instance.coins >= price)
            {
                PlayerStats.instance.coins -= price;
                isPurchased = true;
                PlayerStats.instance.Save();
                PlayerStats.instance.UpdateText();
                gameObject.SetActive(false);
                SaveItem();
            }
        }
    }

    public void PurchaseForGem()
    {
        if (!isPurchased)
        {
            if (PlayerStats.instance.gems >= price)
            {
                PlayerStats.instance.gems -= price;
                isPurchased = true;
                PlayerStats.instance.Save();
                PlayerStats.instance.UpdateText();
                gameObject.SetActive(false);
                SaveItem();
            }
        }
    }

    public void SaveItem()
    {
        YG2.saves.SaveData(id, isPurchased);
        YG2.SaveProgress();
    }

    public void LoadItem()
    {
        if (YG2.saves.skinSaves == null)
            YG2.saves.skinSaves = new List<ItemData>();

        bool itemFound = false;
        foreach (var item in YG2.saves.skinSaves)
        {
            if (item.id == id)
            {
                isPurchased = item.isPurchase;
                if (isPurchased && Target != null)
                    Target.SetActive(false);
                itemFound = true;
                break;
            }
        }

        if (!itemFound)
        {
            YG2.saves.SaveData(id, isPurchased);
            YG2.SaveProgress();
        }
    }
}
