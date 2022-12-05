using System.Collections.Generic;
using UnityEngine;
//===============================================================
//Developer:  CuongCT
//Company:    Rocket
//Date:       2018
//================================================================
/// <summary>
/// Không sử dụng mã hóa
/// </summary>
public static class PlayerPrefsC
{
    public static List<string> AllKey = new List<string>();

    public static bool GetBool(string key, bool defaultValue = false)
    {
        int _intValue = GetInt(key, defaultValue == true ? 1 : 0);
        return _intValue == 0 ? false : true;
    }
    public static void SetBool(string key, bool value)
    {
        SetInt(key, value == true ? 1 : 0);
    }
    public static void SetInt(string key, int value)
    {
//#if UNITY_EDITOR || ROCKET_TEST
//        if (!AllKey.Contains(key))
//        {
//            // string st = string.Format("=========================KEY {0} NOT FOUND=========================", key);
//            RocketIO.Instance.SendMessageAsync(new AddKeyRequest() { Key = key });
//            //Debug.LogError(st);
//        }
//#endif
//        if (!PlayerPrefs.HasKey(key) || GetInt(key) != value)
//        {
//            UserDataServices.Instance.ChangePrivateNewValue(key, value);
//        }

        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }

    public static int GetInt(string key, int defaultValue = 0)
    {
        //Save key and mark for cloud sync
        var value = PlayerPrefs.GetInt(key, defaultValue);
        return value;
    }

    public static void SetString(string key, string value)
    {
//#if UNITY_EDITOR || ROCKET_TEST
//        if (!AllKey.Contains(key))
//        {
//            // string st = string.Format("=========================KEY {0} NOT FOUND=========================", key);
//            RocketIO.Instance.SendMessageAsync(new AddKeyRequest() { Key = key });
//            // Debug.LogError(st);
//        }
//#endif

//        if (!PlayerPrefs.HasKey(key) || GetString(key).CompareTo(value) != 0)
//        {
//            UserDataServices.Instance.ChangePrivateNewValue(key, value);
//        }

        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }

    public static string GetString(string key, string defaultValue = "")
    {
        var value = PlayerPrefs.GetString(key, defaultValue);
        return value;
    }

    public static void SetFloat(string key, float value)
    {
//#if UNITY_EDITOR || ROCKET_TEST
//        if (!AllKey.Contains(key))
//        {
//            // string st = string.Format("=========================KEY {0} NOT FOUND=========================", key);
//            RocketIO.Instance.SendMessageAsync(new AddKeyRequest() { Key = key });
//            // Debug.LogError(st);
//        }
//#endif

//        if (!PlayerPrefs.HasKey(key) || GetFloat(key) != value)
//        {
//            UserDataServices.Instance.ChangePrivateNewValue(key, value);
//        }
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    public static float GetFloat(string key, float defaultValue = 0)
    {
        var value = PlayerPrefs.GetFloat(key, defaultValue);
        return value;
    }

    public static void DeleteAll()
    {
        //
    }
}