using System;
using System.Collections.Generic;
using UnityEngine;

namespace YG
{
    [Serializable]
    public struct ItemData
    {
        public int id; // ������ id � ��� int
        public bool isPurchase;

        public ItemData(int id, bool isPurchase)
        {
            this.id = id;
            this.isPurchase = isPurchase;
        }
    }

    public partial class SavesYG
    {
        public int cups;
        public int coins;
        public int gems;

        ///skins
        public int savesFaceId;
        public int savesSkinId;
        public int currentPetId;


        public List<ItemData> skinSaves = new List<ItemData>();

        public ItemData SaveData(int id, bool isPurchase) 
        {
            for (int i = 0; i < skinSaves.Count; i++)
            {
                if (skinSaves[i].id == id)
                {
                    ItemData updatedItem = new ItemData(id, isPurchase);
                    skinSaves[i] = updatedItem;
                    return updatedItem;
                }
            }

            ItemData newItem = new ItemData(id, isPurchase);
            skinSaves.Add(newItem);
            return newItem;
        }
    }
}
