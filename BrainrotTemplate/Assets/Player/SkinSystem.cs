using System;
using System.Collections.Generic;
using UnityEngine;
using YG;

[Serializable]
public class Skin
{
    public int id;
    public Material material;
}

public class SkinSystem : MonoBehaviour
{
    public List<Skin> faces;
    public List<Skin> skinColor;

    public SkinnedMeshRenderer playerFace;
    public SkinnedMeshRenderer playerSkin;

    private int currentFaceId;
    private int currentSkinId;


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

    public void InitializeSkin()
    {
        ChangeFace(YG2.saves.savesFaceId);
        ChangeSkinColor(YG2.saves.savesSkinId);
    }
}
