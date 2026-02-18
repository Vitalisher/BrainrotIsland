using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using YG;

[Serializable]
public class Skin
{
    public int id;
    public Material material;
    public GameObject gameObject;
}

public class ItemInstance : MonoBehaviour
{
    public int itemId;
}


public class SkinSystem : MonoBehaviour
{
    public List<Skin> faces;
    public List<Skin> skinColor;
    public List<Skin> items;

    public SkinnedMeshRenderer playerFace;
    public SkinnedMeshRenderer playerSkin;

    private int currentFaceId;
    private int currentSkinId;

    private int currentItemId;
    private GameObject currentItem;

    public Transform hatPos;


    public void ChangeFace(int id)
    {
        foreach (var face in faces)
        {
            if (face.id == id)
            {
                playerFace.material = faces[id].material;
                currentFaceId = face.id;
                YG2.saves.savesFaceId = currentFaceId;
                YG2.SaveProgress();
            }
        }

    }

    public void ChangeSkinColor(int id)
    {
        foreach (var skin in skinColor)
        {
            if (skin.id == id)
            {
                playerSkin.material = skinColor[id].material;
                currentSkinId = skin.id;
                YG2.saves.savesSkinId = currentSkinId;
                YG2.SaveProgress();
            }
        }
    }

    public void ChangeItem(int id)
    {
        // 1. —начала ищем нужный предмет
        foreach (var item in items)
        {
            if (item.id == id)
            {
                // 2. ѕровер€ем Ч уже надет этот же предмет?
                if (currentItem != null &&
                    currentItem.GetComponent<ItemInstance>().itemId == id)
                {
                    return; // тот же предмет Ч ничего не делаем
                }

                // 3. ”дал€ем старый только если нашли новый
                if (currentItem != null)
                    Destroy(currentItem);

                // 4. —павним новый Ч ќƒ»Ќ –ј«
                currentItem = Instantiate(item.gameObject, hatPos);
                var instance = currentItem.AddComponent<ItemInstance>();
                instance.itemId = id;

                currentItemId = id;
                YG2.saves.saveItemId = currentItemId;
                YG2.SaveProgress();
                return; // выходим Ч нашли и создали
            }
        }
    }



    public void InitializeSkin()
    {
        ChangeFace(YG2.saves.savesFaceId);
        ChangeSkinColor(YG2.saves.savesSkinId);
        ChangeItem(YG2.saves.saveItemId);
    }
}
