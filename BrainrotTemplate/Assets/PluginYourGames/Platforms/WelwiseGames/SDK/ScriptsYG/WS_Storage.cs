#if WelwiseGamesPlatform_yg && Storage_yg
using UnityEngine;
using YG.Insides;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using AOT;
#if NJSON_STORAGE_YG2
using Newtonsoft.Json;
#else
using UnityEngine.Scripting;
#endif

namespace YG
{
    public partial class SavesYG
    {
        public string playerName;

        public MetaverseSavesYG metaverseSavesYG = new();
    }

    [Serializable]
    public partial class MetaverseSavesYG
    {

    }
}

namespace YG
{
    public partial class PlatformYG2 : IPlatformsYG2
    {
        private const string GAME_SAVE_ID = "game_save";
        private const string METAVERSE_SAVE_ID = "metaverse_save";

        [DllImport("__Internal")]
        private static extern void Welwise_LoadPlayerData(Action<string> callback);

        [DllImport("__Internal")]
        private static extern void Welwise_SavePlayerData(string playerName, string gameData, Action callback);

        [DllImport("__Internal")]
        private static extern void Welwise_LoadMetaverseData(Action<string> callback);

        [DllImport("__Internal")]
        private static extern void Welwise_SaveMetaverseData(string playerName, string metaverseData, string gameData, Action callback);

        [DllImport("__Internal")]
        private static extern bool Welwise_IsMetaverseSupported();

        public void LoadCloud()
        {
            if (Welwise_IsMetaverseSupported())
            {
                Welwise_LoadMetaverseData(OnMetaverseDataLoaded);
            }
            else
            {
                Welwise_LoadPlayerData(OnPlayerDataLoaded);
            }
        }

        public void SaveCloud()
        {
            if (Welwise_IsMetaverseSupported())
            {
#if NJSON_STORAGE_YG2
                string metaverseData = JsonConvert.SerializeObject(YG2.saves.metaverseSavesYG);
                string gameData = JsonConvert.SerializeObject(YG2.saves);
#else
                string metaverseData = JsonUtility.ToJson(YG2.saves.metaverseSavesYG);
                string gameData = JsonUtility.ToJson(YG2.saves);
#endif

                gameData = System.Text.RegularExpressions.Regex.Replace(
                    gameData,
                    @",?\s*""metaverseSavesYG""\s*:\s*{[^}]*}",
                    ""
                );

                Welwise_SaveMetaverseData(YG2.saves.playerName, metaverseData, gameData, OnSaveComplete);
            }
            else
            {
#if NJSON_STORAGE_YG2
                string gameData = JsonConvert.SerializeObject(YG2.saves);
#else
                string gameData = JsonUtility.ToJson(YG2.saves);
#endif

                gameData = System.Text.RegularExpressions.Regex.Replace(
                    gameData,
                    @",?\s*""metaverseSavesYG""\s*:\s*{[^}]*}",
                    ""
                );

                Welwise_SavePlayerData(YG2.saves.playerName, gameData, OnSaveComplete);
            }
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnPlayerDataLoaded(string jsonData)
        {
            try
            {
                if (string.IsNullOrEmpty(jsonData) || jsonData == "null")
                {
                    YG2.SetDefaultSaves();
                    return;
                }

#if NJSON_STORAGE_YG2
                PlayerDataResponse data = JsonConvert.DeserializeObject<PlayerDataResponse>(jsonData);
#else
                PlayerDataResponse data = JsonUtility.FromJson<PlayerDataResponse>(jsonData);
#endif

                ParsePlayerData(data);
            }
            catch (Exception e)
            {
                Debug.LogError($"Player data load error: {e.Message}");
                YG2.SetDefaultSaves();
            }
            finally
            {
                YGInsides.GetDataInvoke();
            }
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnMetaverseDataLoaded(string jsonData)
        {
            try
            {
                if (string.IsNullOrEmpty(jsonData)) return;

#if NJSON_STORAGE_YG2
                var response = JsonConvert.DeserializeObject<MetaverseGameDataResponse>(jsonData);
#else
                var response = JsonUtility.FromJson<MetaverseGameDataResponse>(jsonData);
#endif

                ParseMetaverseData(response);
            }
            catch (Exception e)
            {
                Debug.LogError($"Metaverse data load error: {e.Message}");
                YG2.SetDefaultSaves();
            }
            finally
            {
                YGInsides.GetDataInvoke();
            }
        }

        [MonoPInvokeCallback(typeof(Action))]
        private static void OnSaveComplete()
        {
            Debug.Log("Cloud save completed");
        }

        private static void ParsePlayerData(PlayerDataResponse data)
        {
            try
            {
                if (data == null || data.playerGameData == null)
                {
                    Debug.LogWarning("Player data is missing required fields");
                    YG2.SetDefaultSaves();
                    return;
                }

                string playerName = data.playerName ?? "";
                string gameSave = FindDataValue(data.playerGameData, GAME_SAVE_ID);

                if (!string.IsNullOrEmpty(gameSave))
                {
#if NJSON_STORAGE_YG2
                    YG2.saves = JsonConvert.DeserializeObject<SavesYG>(gameSave);
#else
                    YG2.saves = JsonUtility.FromJson<SavesYG>(gameSave);
#endif
                }
                else
                {
                    Debug.LogWarning("Game save data not found in response");
                    YG2.SetDefaultSaves();
                }

                YG2.saves.playerName = playerName;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error parsing player data: {ex.Message}");
                YG2.SetDefaultSaves();
            }
        }

        private static void ParseMetaverseData(MetaverseGameDataResponse data)
        {
            string playerName = data.playerName;
            string gameSave = FindDataValue(data.playerGameData, GAME_SAVE_ID);
            string metaverseSave = FindDataValue(data.playerMetaverseData, METAVERSE_SAVE_ID);

            if (!string.IsNullOrEmpty(gameSave))
            {
                try
                {
#if NJSON_STORAGE_YG2
                    YG2.saves = JsonConvert.DeserializeObject<SavesYG>(gameSave);
#else
                    YG2.saves = JsonUtility.FromJson<SavesYG>(gameSave);
#endif
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Game data parse error: {ex.Message}\nData: {gameSave}");
                    YG2.SetDefaultSaves();
                }
            }
            else
            {
                YG2.SetDefaultSaves();
            }

            if (!string.IsNullOrEmpty(metaverseSave))
            {
                try
                {
#if NJSON_STORAGE_YG2
                    YG2.saves.metaverseSavesYG = JsonConvert.DeserializeObject<MetaverseSavesYG>(metaverseSave);
#else
                    YG2.saves.metaverseSavesYG = JsonUtility.FromJson<MetaverseSavesYG>(metaverseSave);
#endif
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Metaverse data parse error: {ex.Message}\nData: {metaverseSave}");
                    YG2.saves.metaverseSavesYG = new MetaverseSavesYG();
                }
            }

            if (!string.IsNullOrEmpty(playerName))
            {
                YG2.saves.playerName = playerName;
            }
        }

        private static string FindDataValue(PlayerDataItem[] items, string identifier)
        {
            if (items == null) return null;

            foreach (var item in items)
            {
                if (item.identifier == identifier)
                {
                    return item.value;
                }
            }
            return null;
        }

        [Serializable]
        private class PlayerDataResponse
        {
            public string playerName;
            public PlayerDataItem[] playerGameData;
        }

        [Serializable]
        private class MetaverseGameDataResponse
        {
            public string playerName;
            public PlayerDataItem[] playerMetaverseData;
            public PlayerDataItem[] playerGameData;
        }

        [Serializable]
        private class PlayerDataItem
        {
            public string identifier;
            public string value;
            public object values;
        }
    }
}
#endif