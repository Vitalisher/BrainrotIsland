using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
//
// public class LootBox : MonoBehaviour
// {
//     [Header("HP Settings")]
//     public int maxHP = 100;
//     private int currentHP;
//
//     [Header("Loot Settings")]
//     public List<LootItem> lootItems; 
//     public Transform dropPosition; 
//
//     void Start()
//     {
//         currentHP = maxHP;
//     }
//
//     public void TakeDamage(int damage)
//     {
//         currentHP -= damage;
//
//         if (currentHP <= 0)
//         {
//             Die();
//         }
//     }
//
//     private void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.R))
//         {
//             TakeDamage(1);
//         }
//     }
//
//     void Die()
//     {
//         DropLoot();
//         Destroy(gameObject); 
//     }
//
//     void DropLoot()
//     {
//         int totalWeight = 0;
//         foreach (LootItem item in lootItems)
//         {
//             totalWeight += item.weight;
//         }
//
//         int randomValue = Random.Range(0, totalWeight);
//         int weightSum = 0;
//
//         foreach (LootItem item in lootItems)
//         {
//             weightSum += item.weight;
//             if (randomValue < weightSum)
//             {
//                 GameObject droppedItem = Instantiate(item.itemPrefab, dropPosition.position, Quaternion.identity);
//
//                 ShowRarityEffect(droppedItem, item.rarityColor);
//
//                 break;
//             }
//         }
//     }
//
//     void ShowRarityEffect(GameObject item, Color rarityColor)
//     {
//         GameObject effect = new GameObject("RarityEffect");
//         ParticleSystem particles = effect.AddComponent<ParticleSystem>();
//         particles.startColor = rarityColor;
//         effect.transform.position = item.transform.position;
//         Destroy(effect, 2f); 
//     }
// }