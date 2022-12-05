public class DragonConfig
{
    public const string VersionName = "0.0.2";
    public const int versionCode = 102;
    public const string ProductName = "Jackal Survial IO";
    public const string PackageName = "com.rocket.jackal.surival";
#if UNITY_ANDROID
    public const string OPEN_LINK_RATE = "market://details?id=" + PackageName;
#else 
    public static string OPEN_LINK_RATE = "itms-apps://itunes.apple.com/app/id"+Apple_App_ID;
#endif
}
