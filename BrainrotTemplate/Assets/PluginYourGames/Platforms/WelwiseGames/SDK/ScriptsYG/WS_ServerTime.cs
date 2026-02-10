#if WelwiseGamesPlatform_yg && ServerTime_yg
namespace YG
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using AOT;

    public partial class PlatformYG2 : IPlatformsYG2
    {
        [DllImport("__Internal")]
        private static extern void Welwise_InitServerTime(Action<long> callback);

        [DllImport("__Internal")]
        private static extern void Welwise_GetServerTime(Action<long> callback);

        private static long serverTime;
        private static double lastUpdateTime;
        private static bool initialized;

        [MonoPInvokeCallback(typeof(Action<long>))]
        private static void OnServerTimeInitialized(long time)
        {
            serverTime = time;
            lastUpdateTime = Time.realtimeSinceStartupAsDouble;
            initialized = true;
            Debug.Log($"Server time initialized: {time}");
        }

        [MonoPInvokeCallback(typeof(Action<long>))]
        private static void OnServerTimeUpdated(long time)
        {
            serverTime = time;
            lastUpdateTime = Time.realtimeSinceStartupAsDouble;
            Debug.Log($"Server time updated: {time}");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            Welwise_InitServerTime(OnServerTimeInitialized);

            InvokeRepeating(nameof(UpdateServerTime), 600f, 600f);
        }

        private static void UpdateServerTime()
        {
            if (initialized)
            {
                Welwise_GetServerTime(OnServerTimeUpdated);
            }
        }

        public long ServerTime()
        {
#if UNITY_EDITOR
            return YG2.infoYG.ServerTime.serverTime;
#else
            if (!initialized)
            {
                Debug.LogWarning("Server time not initialized yet");
                return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
            
            double elapsedSeconds = Time.realtimeSinceStartupAsDouble - lastUpdateTime;
            return serverTime + (long)(elapsedSeconds * 1000);
#endif
        }

        private static void InvokeRepeating(string methodName, float delay, float repeatRate)
        {
            GameObject invoker = new GameObject("ServerTimeInvoker");
            UnityEngine.Object.DontDestroyOnLoad(invoker);
            invoker.AddComponent<ServerTimeUpdater>().StartInvoking(methodName, delay, repeatRate);
        }

        private class ServerTimeUpdater : MonoBehaviour
        {
            public void StartInvoking(string methodName, float delay, float repeatRate)
            {
                InvokeRepeating(methodName, delay, repeatRate);
            }
        }
    }
}
#endif