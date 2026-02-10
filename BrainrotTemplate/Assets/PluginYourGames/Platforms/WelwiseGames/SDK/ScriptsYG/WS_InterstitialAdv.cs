#if WelwiseGamesPlatform_yg && InterstitialAdv_yg
namespace YG
{
    using System;
    using System.Runtime.InteropServices;
    using YG.Insides;
    using AOT;

    public partial class PlatformYG2 : IPlatformsYG2
    {
        [DllImport("__Internal")]
        private static extern void Welwise_ShowMidgameAd(
            Action openCallback,
            Action closeCallback,
            Action errorCallback
        );

        [MonoPInvokeCallback(typeof(Action))]
        private static void OnInterstitialOpen()
        {
            YGInsides.OpenInterAdv();
        }

        [MonoPInvokeCallback(typeof(Action))]
        private static void OnInterstitialClose()
        {
            YGInsides.CloseInterAdv();
        }

        [MonoPInvokeCallback(typeof(Action))]
        private static void OnInterstitialError()
        {
            YGInsides.CloseInterAdv();
            YGInsides.ErrorInterAdv();
        }

        public void InterstitialAdvShow()
        {
            Welwise_ShowMidgameAd(
                OnInterstitialOpen,
                OnInterstitialClose,
                OnInterstitialError
            );
        }
    }
}
#endif