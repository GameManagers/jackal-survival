using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Manager;
using Sirenix.OdinInspector;
using WE.Support;
using WE.Skill;

namespace WE.Data
{
    [CreateAssetMenu(fileName = "Data Skill", menuName = "WE/Data/Data Skill")]
    public class DataSkill : SerializedScriptableObject
    {
        public Dictionary<EEvoleSkill, EvoleSkillCondition> EvoleConditionData;
        public Dictionary<EActiveSkill, ActiveSkillConfig> ActiveSkillConfigData;
        public Dictionary<EEvoleSkill, ActiveSkillConfig> EvoleSkillConfigData;
        public Dictionary<EActiveSkill, Dictionary<int, string>> ActiveSkillLevelUpConfig;
        public Dictionary<EPassiveSkill, PassiveSkillConfig> PassiveSkillData;
        public Dictionary<string, float> DicSkillRate;
        public Dictionary<string, List<string>> DicEvole;
        
#if UNITY_EDITOR
        //#region Interaction Matrix
        //[Button("Load Data Evole Condition")]
        //public void LoadDataEvoleCondition()
        //{
        //    InteractionMatrix = new Dictionary<EActiveSkill, Dictionary<EPassiveSkill, bool>>();
        //    string url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vTBO_e7NjDFNq1K56OO5JB19OFqI3VyA8vIW345_YL1QnanxaV0MNvCKJBcpkkWF9DziZOWzuQmXqTG/pub?gid=485803857&single=true&output=csv";
        //    System.Action<string> actionComplete = new System.Action<string>((string str) =>
        //    {
        //        var data = CSVReader.ReadCSV(str);
        //        for (int i = 1; i < data.Count; i++)
        //        {
        //            var _data = data[i];
        //            EActiveSkill asType = EActiveSkill.Lightning_Support;
        //            if (!Helper.TryToEnum<EActiveSkill>(_data[0], out asType))
        //                Debug.LogError("Error IAS Data" + i);
        //            Dictionary<EPassiveSkill, bool> dicPassive = new Dictionary<EPassiveSkill, bool>();
        //            for (int j = 1; j < _data.Length; j++)
        //            {
        //                EPassiveSkill psType = EPassiveSkill.Cooldown_Increase;
        //                if (!Helper.TryToEnum<EPassiveSkill>(data[0][j], out psType))
        //                    Debug.LogError("Error IPS Data" + j);
        //                bool canInteraction = false;
        //                if (!bool.TryParse(_data[j], out canInteraction))
        //                    Debug.LogError("Error I Data" + i + " " + j);
        //                dicPassive.Add(psType, canInteraction);
        //            }
        //            InteractionMatrix.Add(asType, dicPassive);
        //        }
        //        UnityEditor.EditorUtility.SetDirty(this);
        //    }); 
        //    EditorCoroutine.start(Helper.IELoadData(url, actionComplete));
        //}
        //#endregion
        #region active skill data
        [Button("Load Data Active Skill Base Attribute")]
        public void LoadDataASBaseAttribute()
        {
            ActiveSkillConfigData = new Dictionary<EActiveSkill, ActiveSkillConfig>();
            string url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vTBO_e7NjDFNq1K56OO5JB19OFqI3VyA8vIW345_YL1QnanxaV0MNvCKJBcpkkWF9DziZOWzuQmXqTG/pub?gid=1546856341&single=true&output=csv";
            System.Action<string> actionComplete = new System.Action<string>((string str) =>
            {
                var data = CSVReader.ReadCSV(str);
                for (int i = 1; i < data.Count; i++)
                {
                    var _data = data[i];
                    EActiveSkill asType = EActiveSkill.Lightning_Support;
                    if (!Helper.TryToEnum<EActiveSkill>(_data[0], out asType))
                        Debug.LogError("Error Active Skill Base Data" + i);
                    ActiveSkillConfig config = new ActiveSkillConfig();
                    int.TryParse(_data[1],out config.BaseBulletNumb);
                    config.BaseCooldown = Helper.ParseFloat(_data[2]);
                    config.BaseDamage = Helper.ParseFloat(_data[3]);
                    config.BaseBulletSpeed = Helper.ParseFloat(_data[4]);
                    config.BaseAreaScale = Helper.ParseFloat(_data[5]);
                    config.BaseDuration = Helper.ParseFloat(_data[6]);
                    config.BaseHitPerSec = Helper.ParseFloat(_data[7]);
                    int.TryParse(_data[8],out config.BaseHitEffect);
                    config.PushBackForce = Helper.ParseFloat(_data[9]);
                    int.TryParse(_data[10], out config.MaxLevel);
                    int.TryParse(_data[11], out config.Priority);
                    bool.TryParse(_data[12].ToLower(), out config.IsFreeToDrop);
                    ActiveSkillConfigData.Add(asType, config);
                }
                UnityEditor.EditorUtility.SetDirty(this);
            });
            EditorCoroutine.start(Helper.IELoadData(url, actionComplete));
        }
        #endregion
        
        #region active skill level up data
        [Button("Load Data Active Skill Upgrade Attribute")]
        public void LoadDataASUpgradeAttribute()
        {
            ActiveSkillLevelUpConfig = new Dictionary<EActiveSkill, Dictionary<int, string>>();
            string url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vTBO_e7NjDFNq1K56OO5JB19OFqI3VyA8vIW345_YL1QnanxaV0MNvCKJBcpkkWF9DziZOWzuQmXqTG/pub?gid=1040138733&single=true&output=csv";
            System.Action<string> actionComplete = new System.Action<string>((string str) =>
            {
                var data = CSVReader.ReadCSV(str);
                for (int i = 1; i < data.Count; i++)
                {
                    var _data = data[i];
                    EActiveSkill asType = EActiveSkill.Lightning_Support;
                    if (!Helper.TryToEnum<EActiveSkill>(_data[0], out asType))
                        Debug.LogError("Error Active Skill Base Data" + i);
                    Dictionary<int, string> upgradeDic = new Dictionary<int, string>();
                    for (int j = 1; j < _data.Length; j++)
                    {
                        upgradeDic.Add(j + 1, _data[j]);
                    }
                    ActiveSkillLevelUpConfig.Add(asType,upgradeDic);
                }
                UnityEditor.EditorUtility.SetDirty(this);
            });
            EditorCoroutine.start(Helper.IELoadData(url, actionComplete));
        }
        #endregion
        #region Passive Skill Data
        [Button("Load Data Passive Skill")]
        public void LoadDataPassiveSkill()
        {
            PassiveSkillData = new Dictionary<EPassiveSkill, PassiveSkillConfig>();
            string url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vTBO_e7NjDFNq1K56OO5JB19OFqI3VyA8vIW345_YL1QnanxaV0MNvCKJBcpkkWF9DziZOWzuQmXqTG/pub?gid=669029588&single=true&output=csv";
            System.Action<string> actionComplete = new System.Action<string>((string str) =>
            {
                var data = CSVReader.ReadCSV(str);
                for (int i = 1; i < data.Count; i++)
                {
                    var _data = data[i];
                    EPassiveSkill asType = EPassiveSkill.Bullet_Speed_Increase;
                    if (!Helper.TryToEnum<EPassiveSkill>(_data[0], out asType))
                        Debug.LogError("Error Active Skill Base Data" + i);
                    PassiveSkillConfig config = new PassiveSkillConfig();
                    config.ValuePerLevel = Helper.ParseFloat(_data[1]);
                    int.TryParse(_data[2], out config.MaxLevel);
                    int.TryParse(_data[3], out config.Priority);
                    PassiveSkillData.Add(asType, config);
                }
                UnityEditor.EditorUtility.SetDirty(this);
            });
            EditorCoroutine.start(Helper.IELoadData(url, actionComplete));
        }
        #endregion
        #region Evole Skill Data
        [Button("Load Data Evole Skill Base Attribute")]
        public void LoadDataESBaseAttribute()
        {
            EvoleSkillConfigData = new Dictionary<EEvoleSkill, ActiveSkillConfig>();
            EvoleConditionData = new Dictionary<EEvoleSkill, EvoleSkillCondition>();
            DicEvole = new Dictionary<string, List<string>>();
            string url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vTBO_e7NjDFNq1K56OO5JB19OFqI3VyA8vIW345_YL1QnanxaV0MNvCKJBcpkkWF9DziZOWzuQmXqTG/pub?gid=1028131052&single=true&output=csv";
            System.Action<string> actionComplete = new System.Action<string>((string str) =>
            {
                var data = CSVReader.ReadCSV(str);
                for (int i = 1; i < data.Count; i++)
                {
                    var _data = data[i];
                    EEvoleSkill skillType = EEvoleSkill.Blade_Saw_Gun_Evole;
                    if (!Helper.TryToEnum<EEvoleSkill>(_data[0], out skillType))
                        Debug.LogError("Error Evole Skill Base Data" + i);
                    EvoleSkillCondition condition = new EvoleSkillCondition();
                    condition.keyCondition1 = _data[1];
                    int.TryParse(_data[2], out condition.valueCondition1);
                    condition.keyCondition2 = _data[3];
                    int.TryParse(_data[4], out condition.valueCondition2);
                    EvoleConditionData.Add(skillType, condition);
                    if (DicEvole.ContainsKey(condition.keyCondition2))
                    {
                        DicEvole[condition.keyCondition2].Add(condition.keyCondition1);
                    }
                    else
                    {
                        List<string> listEvole = new List<string>();
                        listEvole.Add(condition.keyCondition1);
                        DicEvole.Add(condition.keyCondition2,listEvole);
                    }
                    ActiveSkillConfig config = new ActiveSkillConfig();
                    int.TryParse(_data[5], out config.BaseBulletNumb);
                    config.BaseCooldown = Helper.ParseFloat(_data[6]);
                    config.BaseDamage = Helper.ParseFloat(_data[7]);
                    config.BaseBulletSpeed = Helper.ParseFloat(_data[8]);
                    config.BaseAreaScale = Helper.ParseFloat(_data[9]);
                    config.BaseDuration = Helper.ParseFloat(_data[10]);
                    config.BaseHitPerSec = Helper.ParseFloat(_data[11]);
                    int.TryParse(_data[12], out config.BaseHitEffect);
                    config.PushBackForce = Helper.ParseFloat(_data[13]);
                    int.TryParse(_data[14], out config.MaxLevel);
                    int.TryParse(_data[15], out config.Priority);
                    EvoleSkillConfigData.Add(skillType, config);
                }
                UnityEditor.EditorUtility.SetDirty(this);
            });
            EditorCoroutine.start(Helper.IELoadData(url, actionComplete));
        }
        #endregion
        #region Generate Skill Rate Dic
        [Button("Get Skill Rate")]
        public void GenSkillRate()
        {
            DicSkillRate = new Dictionary<string, float>();
            foreach (KeyValuePair<EActiveSkill, ActiveSkillConfig> item in ActiveSkillConfigData)
            {
                if (item.Value.IsFreeToDrop)
                {
                    DicSkillRate.Add(item.Key.ToString(), item.Value.Priority);
                }
            }
            foreach (KeyValuePair<EEvoleSkill, ActiveSkillConfig> item in EvoleSkillConfigData)
            {
                if (item.Value.IsFreeToDrop)
                {
                    DicSkillRate.Add(item.Key.ToString(), item.Value.Priority);
                }
            }
            foreach (KeyValuePair<EPassiveSkill, PassiveSkillConfig> item in PassiveSkillData)
            {
                DicSkillRate.Add(item.Key.ToString(), item.Value.Priority);
            }
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
        }
        #endregion
#endif

        //public bool CanInteractiveEffect(EActiveSkill activeSkill, EPassiveSkill supportSkill)
        //{
        //    return InteractionMatrix[activeSkill][supportSkill];
        //}
        
    }
    [Serializable]
    public class ActiveSkillConfig
    {
        public int BaseBulletNumb;
        public float BaseCooldown;
        public float BaseDamage;
        public float BaseBulletSpeed;
        public float BaseAreaScale;
        public float BaseDuration;
        public float BaseHitPerSec;
        public int BaseHitEffect;
        public float PushBackForce;
        public int MaxLevel;
        public int Priority;
        public bool IsFreeToDrop;
        public void CoppyTo(ActiveSkillConfig config)
        {
            config.BaseBulletNumb = this.BaseBulletNumb;
            config.BaseCooldown = this.BaseCooldown;
            config.BaseDamage = this.BaseDamage;
            config.BaseBulletSpeed = this.BaseBulletSpeed;
            config.BaseAreaScale = this.BaseAreaScale;
            config.BaseDuration = this.BaseDuration;
            config.BaseHitPerSec = this.BaseHitPerSec;
            config.BaseHitEffect = this.BaseHitEffect;
            config.PushBackForce = this.PushBackForce;
            config.MaxLevel = this.MaxLevel;
            config.Priority = this.Priority;
            config.IsFreeToDrop = this.IsFreeToDrop;
        }
    }
    [Serializable]
    public class PassiveSkillConfig
    {
        public float ValuePerLevel;
        public int MaxLevel;
        public int Priority;
    }
    [Serializable]
    public class EvoleSkillCondition
    {
        public string keyCondition1;
        public string keyCondition2;
        public int valueCondition1;
        public int valueCondition2;
    }
}

