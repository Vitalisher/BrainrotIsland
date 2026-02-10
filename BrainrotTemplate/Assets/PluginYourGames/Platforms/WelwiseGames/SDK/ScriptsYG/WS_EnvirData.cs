#if WelwiseGamesPlatform_yg && EnvirData_yg
namespace YG
{
    using System.Runtime.InteropServices;
    using UnityEngine;
    using YG.Insides;

    public partial class PlatformYG2 : IPlatformsYG2
    {
        [DllImport("__Internal")]
        private static extern System.IntPtr WelwiseGames_GetEnvirData();


        public void InitEnirData()
        {
            System.IntPtr envirDataPtr = WelwiseGames_GetEnvirData();
            string json = Marshal.PtrToStringUTF8(envirDataPtr);
            YGInsides.FreeBuffer(envirDataPtr);

            if (json == "NO_DATA")
            {
                Debug.LogError("Failed to get Welwise environment data");
                return;
            }

            YG2.EnvirData envirData = JsonUtility.FromJson<YG2.EnvirData>(json);

            YG2.envir.language = envirData.language?.ToLower() ?? "en";
            YG2.envir.browser = envirData.browser;
            YG2.envir.platform = envirData.platform;
            YG2.envir.deviceType = envirData.deviceType;
            YG2.envir.isMobile = envirData.isMobile;
            YG2.envir.isTablet = envirData.isTablet;
            YG2.envir.isDesktop = envirData.isDesktop;
        }

        public void GetEnvirData()
        {
            InitEnirData();
            YG2.GetDataInvoke();
        }
    }
}
#endif