#if WelwiseGamesPlatform_yg
namespace YG
{
    using System;
    using UnityEngine;
    using AOT;
#if UNITY_WEBGL
    using System.Runtime.InteropServices;
#endif

    public static partial class YG2
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void Welwise_GoToGame(int id, Action<IntPtr> callback);
#endif
        
        public static void GoToWelwiseGame(int id)
        {
#if Storage_yg
            SaveProgress();
#endif
            
#if UNITY_WEBGL && !UNITY_EDITOR
            Welwise_GoToGame(id, OnGoToGameCompleted);
#else
            Debug.Log($"Go to Welwise Game {id}");
#endif
        }
        
#if UNITY_WEBGL && !UNITY_EDITOR
        [MonoPInvokeCallback(typeof(Action<IntPtr>))]
        private static void OnGoToGameCompleted(IntPtr resultPtr)
        {
            if (resultPtr == IntPtr.Zero)
            {
                Debug.Log($"Successfully navigated to game");
            }
            else
            {
                string error = Marshal.PtrToStringUTF8(resultPtr);
                Debug.LogError($"Navigation error: {error}");
                Marshal.FreeHGlobal(resultPtr);
            }
        }
#endif
    }
}
#endif