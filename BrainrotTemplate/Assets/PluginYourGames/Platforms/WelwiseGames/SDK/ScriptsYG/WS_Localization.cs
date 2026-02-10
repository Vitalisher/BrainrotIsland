#if WelwiseGamesPlatform_yg && Localization_yg

namespace YG
{
    using System;
    using System.Runtime.InteropServices;
    using YG.Insides;

    public partial class PlatformYG2 : IPlatformsYG2
    {
        [DllImport("__Internal")]
        private static extern IntPtr WelwiseGames_GetLanguage();

        public string GetLanguage()
        {
            var langPtr = WelwiseGames_GetLanguage();
            var lang = Marshal.PtrToStringUTF8(langPtr);
            YGInsides.FreeBuffer(langPtr);
            return lang;
        }
    }
}
#endif