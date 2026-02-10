using System;
using System.Collections.Generic;
using UnityEngine;
using YG;

[Serializable]
public class Pet
{
    public int id;
    public GameObject petObject;
}

public class GemPets : MonoBehaviour
{
    public List<Pet> pets;
    public Transform petPosition;

    private GameObject currentPet;
    private int currentPetId;

    public void ChosePet(int id)
    {
        foreach (var pet in pets)
        {
            if (pet.id == id && currentPet != null)
            {
                if (currentPet.GetComponent<PetInstance>().petId == id)
                {
                    return;
                }
            }
        }

        if (currentPet != null)
        {
            Destroy(currentPet);
        }

        foreach (var pet in pets)
        {
            if (pet.id == id)
            {
                var p = Instantiate(pet.petObject, petPosition.position, petPosition.rotation);
                p.transform.SetParent(petPosition);
                currentPet = p;

                var petInstance = p.AddComponent<PetInstance>();
                petInstance.petId = id;

                currentPetId = petInstance.petId;
                YG2.saves.currentPetId = currentPetId;
                YG2.SaveProgress();
            }
        }
    }

    public void ClearPet()
    {
        if (currentPet != null)
        {
            Destroy(currentPet);
            currentPetId = 0;
            YG2.saves.currentPetId = currentPetId;
            YG2.SaveProgress();
        }
    }

    public void InitializePet()
    {
        ChosePet(YG2.saves.currentPetId);
    }
}

public class PetInstance : MonoBehaviour
{
    public int petId;
}
