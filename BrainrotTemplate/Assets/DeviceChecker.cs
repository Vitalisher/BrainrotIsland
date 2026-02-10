using UnityEngine;

public class DeviceChecker : MonoBehaviour
{
    public static bool IsMobile;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

#if UNITY_ANDROID || UNITY_IOS
    IsMobile = true;
#else
        IsMobile = Application.isMobilePlatform;
#endif
        
    }
}