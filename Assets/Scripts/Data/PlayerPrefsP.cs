using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//===============================================================
//Developer:  CuongCT
//Company:    Rocket Studio
//Date:       2020
//================================================================
/// <summary>
/// Sử dụng mã hóa 
/// </summary>
public class PlayerPrefsP
{
    private static Dictionary<string, ObscuredInt> _ObscuredInt = new Dictionary<string, ObscuredInt>();
    private static Dictionary<string, ObscuredString> _ObscuredString = new Dictionary<string, ObscuredString>();
    private static Dictionary<string, ObscuredFloat> _ObscuredFloat = new Dictionary<string, ObscuredFloat>();
    private static Dictionary<string, ObscuredBool> _ObscuredBool = new Dictionary<string, ObscuredBool>();



    public static List<string> ProtectedKey = new List<string>();

    public static bool GetBool(string key, bool defaultValue = false)
    {
//#if UNITY_EDITOR || ROCKET_TEST
//        var _key = string.Format("BOOL|{0}", key);
//        if (!ProtectedKey.Contains(_key))
//        {
//            //string st = string.Format("=========================KEY {0} NOT FOUND=========================", _key);
//            RocketIO.Instance.SendMessageAsync(new AddKeyRequest() { Key = _key, Encrypt = true });
//            //DebugCustom.LogColor(st);
//        }
//#endif
        if (!_ObscuredBool.ContainsKey(key))
        {
            var value = ObscuredPrefs.GetBool(key, defaultValue);
            ObscuredBool IntData = value;
            if (!_ObscuredBool.ContainsKey(key))
            {
                _ObscuredBool.Add(key, IntData);
            }
        }
        //Save key and mark for cloud sync

        return _ObscuredBool[key];
    }
    public static void SetBool(string key, bool value)
    {
        //SetInt(key, value == true ? 1 : 0);

//#if UNITY_EDITOR || ROCKET_TEST
//        var _key = string.Format("BOOL|{0}", key);
//        if (!ProtectedKey.Contains(_key))
//        {
//            //string st = string.Format("=========================KEY {0} NOT FOUND=========================", _key);
//            RocketIO.Instance.SendMessageAsync(new AddKeyRequest() { Key = _key, Encrypt = true });
//           // DebugCustom.LogColor(st);
//        }
//#endif
        //if (!ObscuredPrefs.HasKey(key) || GetBool(key) != value)
        //{
        //    UserDataServices.Instance.ChangePrivateNewValue(key, value);
        //}

        if (!_ObscuredBool.ContainsKey(key))
        {
            ObscuredBool IntData = value;
            _ObscuredBool.Add(key, IntData);
        }
        else
        {
            _ObscuredBool[key] = value;
        }

        ObscuredPrefs.SetBool(key, value);
        ObscuredPrefs.Save();


    }

    public static void SetInt(string key, int value)
    {
//#if UNITY_EDITOR || ROCKET_TEST
//        var _key = string.Format("INT|{0}", key);
//        if (!ProtectedKey.Contains(_key))
//        {
//            //string st = string.Format("=========================KEY {0} NOT FOUND=========================", _key);
//            RocketIO.Instance.SendMessageAsync(new AddKeyRequest() { Key = _key, Encrypt = true });
//            //DebugCustom.LogColor(st);
//        }
//#endif
        //if (!ObscuredPrefs.HasKey(key) || GetInt(key) != value)
        //{
        //    UserDataServices.Instance.ChangePrivateNewValue(key, value);
        //}

        if (!_ObscuredInt.ContainsKey(key))
        {
            ObscuredInt IntData = value;
            _ObscuredInt.Add(key, IntData);
        }
        else
        {
            _ObscuredInt[key] = value;
        }

        ObscuredPrefs.SetInt(key, value);
        ObscuredPrefs.Save();
    }

    public static int GetInt(string key, int defaultValue = 0)
    {
//#if UNITY_EDITOR || ROCKET_TEST
//        var _key = string.Format("INT|{0}", key);
//        if (!ProtectedKey.Contains(_key))
//        {
//            //string st = string.Format("=========================KEY {0} NOT FOUND=========================", _key);
//            RocketIO.Instance.SendMessageAsync(new AddKeyRequest() { Key = _key, Encrypt = true });
//            //DebugCustom.LogColor(st);
//        }
//#endif
        if (!_ObscuredInt.ContainsKey(key))
        {
            var value = ObscuredPrefs.GetInt(key, defaultValue);
            ObscuredInt IntData = value;
            if (!_ObscuredInt.ContainsKey(key))
            {
                _ObscuredInt.Add(key, IntData);
            }
        }
        //Save key and mark for cloud sync

        return _ObscuredInt[key];
    }

    public static void SetString(string key, string value)
    {
//#if UNITY_EDITOR || ROCKET_TEST
//        var _key = string.Format("STR|{0}", key);
//        if (!ProtectedKey.Contains(_key))
//        {
//            //string st = string.Format("=========================KEY {0} NOT FOUND=========================", _key);
//            RocketIO.Instance.SendMessageAsync(new AddKeyRequest() { Key = string.Format("STR|{0}", key), Encrypt = true });
//            //DebugCustom.LogColor(st);
//        }
//#endif

        //if (!ObscuredPrefs.HasKey(key) || GetString(key).CompareTo(value) != 0)
        //{
        //    UserDataServices.Instance.ChangePrivateNewValue(key, value);
        //}

        if (!_ObscuredString.ContainsKey(key))
        {
            ObscuredString StrData = value;
            _ObscuredString.Add(key, StrData);
        }
        else
        {
            _ObscuredString[key] = value;
        }
        ObscuredPrefs.SetString(key, value);
        ObscuredPrefs.Save();
    }

    public static string GetString(string key, string defaultValue = "")
    {
//#if UNITY_EDITOR || ROCKET_TEST
//        var _key = string.Format("STR|{0}", key);
//        if (!ProtectedKey.Contains(_key))
//        {
//            //string st = string.Format("=========================KEY {0} NOT FOUND=========================", _key);
//            RocketIO.Instance.SendMessageAsync(new AddKeyRequest() { Key = _key, Encrypt = true });
//           //DebugCustom.LogColor(st);
//        }
//#endif
        if (!_ObscuredString.ContainsKey(key))
        {
            var value = ObscuredPrefs.GetString(key, defaultValue);
            ObscuredString StrData = value;
            _ObscuredString.Add(key, StrData);
        }

        return _ObscuredString[key];
    }

    public static void SetFloat(string key, float value)
    {
//#if UNITY_EDITOR || ROCKET_TEST
//        var _key = string.Format("FLO|{0}", key);
//        if (!ProtectedKey.Contains(_key))
//        {
//            //string st = string.Format("=========================KEY {0} NOT FOUND=========================", _key);
//            RocketIO.Instance.SendMessageAsync(new AddKeyRequest() { Key = _key, Encrypt = true });
//            //DebugCustom.LogColor(st);
//        }
//#endif

        //if (!ObscuredPrefs.HasKey(key) || GetFloat(key) != value)
        //{
        //    UserDataServices.Instance.ChangePrivateNewValue(key, value);
        //}

        if (!_ObscuredFloat.ContainsKey(key))
        {
            ObscuredFloat FloatData = value;
            _ObscuredFloat.Add(key, FloatData);
        }
        else
        {
            _ObscuredFloat[key] = value;
        }

        ObscuredPrefs.SetFloat(key, value);
        ObscuredPrefs.Save();
    }

    public static float GetFloat(string key, float defaultValue = 0)
    {
//#if UNITY_EDITOR || ROCKET_TEST
//        var _key = string.Format("FLO|{0}", key);
//        if (!ProtectedKey.Contains(_key))
//        {
//            //string st = string.Format("=========================KEY {0} NOT FOUND=========================", _key);
//            RocketIO.Instance.SendMessageAsync(new AddKeyRequest() { Key = _key, Encrypt = true });
//            //DebugCustom.LogColor(st);
//        }
//#endif
        if (!_ObscuredFloat.ContainsKey(key))
        {
            var value = ObscuredPrefs.GetFloat(key, defaultValue);
            ObscuredFloat FloatData = value;
            _ObscuredFloat.Add(key, FloatData);
        }

        return _ObscuredFloat[key];
    }

    public static bool HasKey(string key)
    {
        var encrypted = ObscuredPrefs.EncryptKey(key);
        return PlayerPrefs.HasKey(encrypted);
    }
    public static void DeleteAll()
    {
        _ObscuredFloat.Clear();
        _ObscuredInt.Clear();
        _ObscuredString.Clear();
        _ObscuredBool.Clear();
    }
}
