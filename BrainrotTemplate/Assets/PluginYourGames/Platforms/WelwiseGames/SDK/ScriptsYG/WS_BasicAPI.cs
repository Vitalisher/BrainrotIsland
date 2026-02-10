
#if WelwiseGamesPlatform_yg
namespace YG
{
    using System;
    using UnityEngine;
    using AOT;
#if UNITY_WEBGL
    using System.Runtime.InteropServices;
#endif

    public partial class PlatformYG2 : IPlatformsYG2
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void WelwiseGames_Init(Action successCallback, Action<string> errorCallback);
#endif
        public void InitAwake()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            try
            {
                WelwiseGames_Init(
                    SuccessCallback,
                    ErrorCallback
                );
            }
            catch (Exception e)
            {
                Debug.LogError("WelwiseGames init exception: " + e.Message);
                YG2.SyncInitialization();
            }
#else
            YG2.SyncInitialization();
#endif
        }

        [MonoPInvokeCallback(typeof(Action))]
        private static void SuccessCallback()
        {
            YG2.SyncInitialization();
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void ErrorCallback(string error)
        {
            Debug.LogError("WelwiseGames SDK init error: " + error);
            YG2.SyncInitialization();
        }
    }
}
#endif