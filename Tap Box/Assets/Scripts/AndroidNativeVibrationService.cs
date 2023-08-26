using UnityEngine;

public class AndroidNativeVibrationService
{
#if UNITY_ANDROID && !UNITY_EDITOR
        public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#else
    public static AndroidJavaClass unityPlayer;
    public static AndroidJavaObject currentActivity;
    public static AndroidJavaObject vibrator;
#endif
    public void Vibrate()
    {
        if (isAndroid())
            vibrator.Call("vibrate");
            
#if UNITY_EDITOR
        else
            Handheld.Vibrate();
#endif
    }
                
    public void Vibrate(long milliseconds)
    {
        if (isAndroid())
            vibrator.Call("vibrate", milliseconds);
           
#if UNITY_EDITOR
        else
            Handheld.Vibrate();
#endif
    }
        
    public void Vibrate(long[] pattern, int repeat)
    {
        if (isAndroid())
            vibrator.Call("vibrate", pattern, repeat);
            
#if UNITY_EDITOR
        else
            Handheld.Vibrate();
#endif
    }
        
    public bool HasVibrator()
    {
        return isAndroid();
    }
        
    public void Cancel()
    {
        if (isAndroid())
            vibrator.Call("cancel");
    }
        
    private bool isAndroid()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            return true;
#else
        return false;
#endif
    }
}