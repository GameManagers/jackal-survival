using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Data;
using WE.Skill;
using WE.Unit;
using WE.Utils;
using Sirenix.OdinInspector;
using WE.Support;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using WE.Pooling;

namespace WE.Manager
{
    [ShowOdinSerializedPropertiesInInspector]
    public class SkillController : MonoBehaviour
    {
        [Flags]
        public enum EskillFlags
        {
            Lightning_Support = 1 << 0,
            Machine_Gun = 1 << 1,
            Rocket_Gun = 1 << 2,
            Short_Gun = 1 << 3,
            Gravity_Tower_Support = 1 << 4,
            Molotov_Support = 1 << 5,
            Cannon_Gun = 1 << 6,
            Lazer_Gun = 1 << 7,
            Flame_Gun = 1 << 8,
            Shock_Zone_Support = 1 << 9,
            EVA_Shield = 1 << 10,
            Saw_Blade_Support = 1 << 11,
            Zeus_Spear = 1 << 12,
            Tesla_Coil_Support = 1 << 13,
            Blade_Saw_Gun = 1 << 14,
            Shock_Wave_Support = 1 << 15,
            Drone_Type_A_Support = 1 << 16,
            Drone_Type_B_Support = 1 << 17,
            Cooldown_Increase = 1 << 18,
            Bullet_Speed_Increase = 1 << 19,
            Bullet_Damage_Increase = 1 << 20,
            Max_Hp_Increase = 1 << 21,
            Exp_Increase = 1 << 22,
            Item_Absorb_Range_Increase = 1 << 23,
            Effect_Area_Increase = 1 << 24,
            Effect_Duration_Increase = 1 << 25,
            Coin_Value_Increase = 1 << 26,
            Hp_Recovery_Increase = 1 << 27,
            Damage_Reviced_Reduction_Increase = 1 << 28,
            Luck_Increase = 1 << 29,
            Projectile_Number_Increase = 1 << 30,
            Move_Speed_Increase = 1 << 32,
            Crit_Incease = 1 << 33,
            Push_Back_Force_Increase = 1 << 34,
            Revival = 1 << 35,
        }
        public static SkillController Instance;
        private void Awake()
        {
            Instance = this;
        }
        public PlayerStats playerStats => Player.Instance.playerStats;

        public DataSkill dataSkill;

        //public Transform supportDroneRotateBase;
        //public float suportDroneRotateSpeed;
        public Transform GunTransform;
        public Transform GunFirePos;

        public EskillFlags skillVip;

        public ActiveSkill[] activeSkills;
        public ActiveSkill[] evoleSkills;

        public Dictionary<EActiveSkill, ActiveSkill> activeSkillPrefabs;
        public Dictionary<EEvoleSkill, ActiveSkill> evoleSkillPrefabs;
        Dictionary<EActiveSkill, ActiveSkill> activeSkillDict;
        Dictionary<EEvoleSkill, ActiveSkill> evoleSkillDict;

        public List<string> activeSkillList;
        public List<string> passiveSkillList;
        public Dictionary<EPassiveSkill, int> passiveSkillUpgradeDict => playerStats.supportSkillUpgradeDict;
        public Dictionary<string, int> dicStringCurrentSkill;
        public System.Action<EPassiveSkill> OnAddPassive;
        public System.Action<EActiveSkill> OnAddActive;
        public System.Action<EEvoleSkill> OnAddEvole;
        public System.Action OnAddUpgrade;
        public System.Action OnChangeSkill;

        [Button("Gen Dic")]
        public void GenDic()
        {
            activeSkillPrefabs = new Dictionary<EActiveSkill, ActiveSkill>();
            for (int i = 0; i < activeSkills.Length; i++)
            {
                activeSkillPrefabs.Add(activeSkills[i].SkillType, activeSkills[i]);
            }
            evoleSkillPrefabs = new Dictionary<EEvoleSkill, ActiveSkill>();
            for (int i = 0; i < evoleSkills.Length; i++)
            {
                evoleSkillPrefabs.Add(evoleSkills[i].EvoleSkillType, evoleSkills[i]);
            }
        }
        public void GameInit()
        {
            GenDic();
            activeSkillDict = new Dictionary<EActiveSkill, ActiveSkill>();
            evoleSkillDict = new Dictionary<EEvoleSkill, ActiveSkill>();
            dicStringCurrentSkill = new Dictionary<string, int>();

            activeSkillList = new List<string>();
            passiveSkillList = new List<string>();

            AddSkill(CarController.Instance.dataVehicle.VehicleDict[Player.Instance.CurrentTank].LinkedSkill);
            TigerForge.EventManager.StartListening(Constant.TIMER_UPDATE_EVENT, OnUpdate);
            UIManager.Instance.uIInGame.InitSkillBar();
        }
        public void OnEndGame()
        {
            foreach (var item in activeSkillDict)
            {
                item.Value.Dispose();
            }
            foreach (var item in evoleSkillDict)
            {
                item.Value.Dispose();
            }
            TigerForge.EventManager.StopListening(Constant.TIMER_UPDATE_EVENT, OnUpdate);
        }
        public void OnUpdate()
        {
            if (GameplayManager.Instance.State != GameState.Gameplay)
                return;
            //float t = TigerForge.EventManager.GetFloat(Constant.TIMER_UPDATE_EVENT);
            transform.position = Player.Instance.transform.position;
        }
        public void StopAction()
        {
            foreach (var item in activeSkillDict)
            {
                item.Value.Stop();
            }
            foreach (var item in evoleSkillDict)
            {
                item.Value.Stop();
            }
        }
        public void ReturnAction()
        {
            foreach (var item in activeSkillDict)
            {
                item.Value.StartAction();
            }
            foreach (var item in evoleSkillDict)
            {
                item.Value.StartAction();
            }
        }
        public void AddSkill(EActiveSkill activeSkill)
        {
            if (activeSkillDict.ContainsKey(activeSkill))
            {
                activeSkillDict[activeSkill].AddLevel();
                dicStringCurrentSkill[activeSkill.ToString()] += 1;
                //DebugCustom.Log("Add Active Skill ", activeSkill, dicStringCurrentSkill[activeSkill.ToString()]);
            }
            else
            {
                ActiveSkill skill = activeSkillPrefabs[activeSkill];
                activeSkillDict.Add(activeSkill, skill);
                dicStringCurrentSkill.Add(activeSkill.ToString(), 1);
                activeSkillList.Add(activeSkill.ToString());
                skill.Init();
                OnChangeSkill?.Invoke();
            }
            OnAddActive?.Invoke(activeSkill);
            //OnAddUpgrade?.Invoke();
        }
        public void AddPassive(EPassiveSkill passiveSkill)
        {
            if (dicStringCurrentSkill.ContainsKey(passiveSkill.ToString()))
            {

                dicStringCurrentSkill[passiveSkill.ToString()]++;
                //DebugCustom.Log("AddPassive Skill ", passiveSkill, dicStringCurrentSkill[passiveSkill.ToString()]);
            }
            else
            {
                dicStringCurrentSkill.Add(passiveSkill.ToString(), 1);
                passiveSkillList.Add(passiveSkill.ToString());
                OnChangeSkill?.Invoke();
            }
            playerStats.AddPassive(passiveSkill);
            OnAddPassive?.Invoke(passiveSkill);
            OnAddUpgrade?.Invoke();
        }
        public void AddEvoleSkill(EEvoleSkill evoleSkill)
        {

            if (!evoleSkillDict.ContainsKey(evoleSkill))
            {
                string key1 = dataSkill.EvoleConditionData[evoleSkill].keyCondition1;
                string key2 = dataSkill.EvoleConditionData[evoleSkill].keyCondition2;

                EActiveSkill activeSkill = EActiveSkill.Lightning_Support;
                bool added = false;
                if (Helper.TryToEnum<EActiveSkill>(key1, out activeSkill))
                {
                    activeSkillDict[activeSkill].Dispose();
                    activeSkillDict.Remove(activeSkill);
                    dicStringCurrentSkill.Remove(key1);
                    if (!added)
                    {
                        int id = activeSkillList.IndexOf(activeSkill.ToString());
                        activeSkillList[id] = evoleSkill.ToString();
                        added = true;
                    }
                    else
                    {
                        activeSkillList.Remove(activeSkill.ToString());
                    }
                }
                if (Helper.TryToEnum<EActiveSkill>(key2, out activeSkill))
                {
                    activeSkillDict[activeSkill].Dispose();
                    activeSkillDict.Remove(activeSkill);
                    dicStringCurrentSkill.Remove(key2);
                    if (!added)
                    {
                        int id = activeSkillList.IndexOf(activeSkill.ToString());
                        activeSkillList[id] = evoleSkill.ToString();
                        added = true;
                    }
                    else
                    {
                        activeSkillList.Remove(activeSkill.ToString());
                    }
                }
                evoleSkillDict.Add(evoleSkill, evoleSkillPrefabs[evoleSkill]);
                evoleSkillDict[evoleSkill].Init();
                dicStringCurrentSkill.Add(evoleSkill.ToString(), 1);
            }
            OnAddEvole?.Invoke(evoleSkill);
            OnChangeSkill?.Invoke();
            //OnAddUpgrade?.Invoke();
        }
        public Dictionary<string, float> GetAviableSkillRateDic()
        {
            Dictionary<string, float> DicSkillRate = new Dictionary<string, float>();
            foreach (KeyValuePair<EEvoleSkill, ActiveSkillConfig> item in dataSkill.EvoleSkillConfigData)
            {
                if (!evoleSkillDict.ContainsKey(item.Key))
                {
                    EvoleSkillCondition con = dataSkill.EvoleConditionData[item.Key];
                    if (dicStringCurrentSkill.ContainsKey(con.keyCondition1) && dicStringCurrentSkill.ContainsKey(con.keyCondition2))
                    {
                        if (dicStringCurrentSkill[con.keyCondition1] >= con.valueCondition1 && dicStringCurrentSkill[con.keyCondition2] >= con.valueCondition2)
                        {
                            //DebugCustom.Log("Add evole" + item.Key);
                            DicSkillRate.Add(item.Key.ToString(), 500);
                        }
                    }
                }
            }
            foreach (KeyValuePair<EActiveSkill, ActiveSkillConfig> item in dataSkill.ActiveSkillConfigData)
            {
                if (activeSkillDict.ContainsKey(item.Key))
                {
                    if (activeSkillDict[item.Key].CurrentLevel < item.Value.MaxLevel)
                    {
                        //DebugCustom.Log("Add Active"+ item.Key);
                        DicSkillRate.Add(item.Key.ToString(), item.Value.Priority);
                    }
                }
                else if (activeSkillDict.Count + evoleSkillDict.Count < Constant.MAX_SKILL_STLOT && !IsEvoled(item.Key.ToString()) && item.Value.IsFreeToDrop)
                {
                    //DebugCustom.Log("Add Active"+ item.Key);
                    DicSkillRate.Add(item.Key.ToString(), item.Value.Priority);
                }

            }
            foreach (KeyValuePair<EPassiveSkill, PassiveSkillConfig> item in dataSkill.PassiveSkillData)
            {
                if (passiveSkillUpgradeDict != null)
                {
                    if (passiveSkillUpgradeDict.ContainsKey(item.Key))
                    {
                        if (passiveSkillUpgradeDict[item.Key] < item.Value.MaxLevel)
                        {
                            //DebugCustom.Log("Add Pasive"+ item.Key);
                            DicSkillRate.Add(item.Key.ToString(), item.Value.Priority);
                        }
                    }
                    else if (passiveSkillUpgradeDict.Count < Constant.MAX_SKILL_STLOT)
                    {

                        //DebugCustom.Log("Add Pasive"+ item.Key);
                        if (item.Key == EPassiveSkill.Revival)
                        {
                            if (!Player.Instance.IsGetRevieSkill)
                            {
                                DicSkillRate.Add(item.Key.ToString(), item.Value.Priority);
                            }
                        }
                        else
                            DicSkillRate.Add(item.Key.ToString(), item.Value.Priority);
                    }
                }
                else
                {
                    //DebugCustom.Log("Add Pasive"+ item.Key);
                    if (item.Key == EPassiveSkill.Revival)
                    {
                        if (!Player.Instance.IsGetRevieSkill)
                        {
                            DicSkillRate.Add(item.Key.ToString(), item.Value.Priority);
                        }
                    }
                    else
                        DicSkillRate.Add(item.Key.ToString(), item.Value.Priority);
                }
            }


            return DicSkillRate;
        }
        public List<UpgradeResult> GetChestSkill(int skillNumb)
        {
            List<UpgradeResult> result = new List<UpgradeResult>();
            //List<UpgradeResult> _result = new List<UpgradeResult>();
            Dictionary<string, int> dicUsed = new Dictionary<string, int>(dicStringCurrentSkill);

            int skillAdded = 0;
            int evoleAdded = 0;
            for (int i = 0; i < skillNumb; i++)
            {
                bool added = false;
                foreach (KeyValuePair<EEvoleSkill, ActiveSkillConfig> item in dataSkill.EvoleSkillConfigData)
                {
                    if (!evoleSkillDict.ContainsKey(item.Key))
                    {
                        EvoleSkillCondition con = dataSkill.EvoleConditionData[item.Key];
                        if (dicUsed.ContainsKey(con.keyCondition1) && dicUsed.ContainsKey(con.keyCondition2))
                        {
                            if (dicUsed[con.keyCondition1] >= con.valueCondition1 && dicUsed[con.keyCondition2] >= con.valueCondition2 && !IsAlreadyHasEvoleSkill(result, item.Key.ToString()))
                            {
                                UpgradeResult a = new UpgradeResult();
                                a.key = item.Key.ToString();
                                a.level = 1;
                                result.Add(a);
                                evoleAdded++;
                                skillAdded++;
                                added = true;
                                //DebugCustom.Log("Add Evole" + item.Key);
                                break;
                            }
                        }
                    }
                }
                if (!added)
                {
                    for (int j = 0; j < dicUsed.Count; j++)
                    {
                        int kId = Random.Range(0, dicUsed.Count);
                        KeyValuePair<string, int> item = dicUsed.ElementAt(kId);
                        kId++;
                        if (kId >= dicUsed.Count)
                        {
                            kId -= dicUsed.Count;
                        }
                        if (!IsEvoleSkill(item.Key) && item.Value < GetMaxLevel(item.Key))
                        {
                            dicUsed[item.Key]++;
                            UpgradeResult a = new UpgradeResult();
                            a.key = item.Key;
                            a.level = dicUsed[item.Key];
                            result.Add(a);
                            skillAdded++;
                            added = true;
                            //DebugCustom.Log("Add Skill" + item.Key);
                            break;
                        }
                    }
                }
                if (!added)
                {
                    UpgradeResult a = new UpgradeResult();
                    a.key = Constant.COIN_CHEST;
                    a.level = 1;
                    result.Add(a);
                }

            }
            return result;
            //if (skillAdded < skillNumb)
            //{
            //    int j = skillNumb - skillAdded;
            //    for (int i = 0; i < j; i++)
            //    {
            //        int kId = Random.Range(0, dicUsed.Count);
            //        for (int k = 0; k < dicUsed.Count; k++)
            //        {
            //            KeyValuePair<string, int> item = dicUsed.ElementAt(kId);
            //            kId++;
            //            if (kId >= dicUsed.Count)
            //            {
            //                kId -= dicUsed.Count;
            //            }
            //            if (!IsEvoleSkill(item.Key) && item.Value < GetMaxLevel(item.Key))
            //            {
            //                dicUsed[item.Key]++;
            //                UpgradeResult a = new UpgradeResult();
            //                a.key = item.Key;
            //                a.level = dicUsed[item.Key];
            //                result.Add(a);
            //                skillAdded++;

            //                //DebugCustom.Log("Add Skill" + item.Key);
            //                break;
            //            }
            //        }
            //    }
            //}

            //if (skillAdded < skillNumb)
            //{
            //    int j = skillNumb - skillAdded;
            //    for (int i = 0; i < j; i++)
            //    {
            //        UpgradeResult a = new UpgradeResult();
            //        a.key = Constant.COIN_CHEST;
            //        a.level = 1;
            //        result.Add(a);
            //    }
            //    _result.AddRange(result);
            //    return _result;
            //}
            //else if (skillAdded == skillNumb)
            //{
            //    //for (int i = 0; i < result.Count; i++)
            //    //{
            //    //    int rand = Random.Range(0, result.Count);
            //    //    var temp = result[rand];
            //    //    result[rand] = result[i];
            //    //    result[i] = temp;
            //    //}
            //    _result.AddRange(result);
            //    return _result;
            //}
            //else
            //{
            //    for (int i = 0; i < skillNumb - evoleAdded; i++)
            //    {
            //        _result.Add(result[0]);
            //        result.RemoveAt(0);
            //    }
            //    return _result;
            //}
        }
        public List<string> GetListSkillReRoll()
        {
            List<string> result = new List<string>();
            Dictionary<string, float> DicSkillRate = new Dictionary<string, float>();
            #region Add Evole
            foreach (KeyValuePair<EEvoleSkill, ActiveSkillConfig> item in dataSkill.EvoleSkillConfigData)
            {
                if (!evoleSkillDict.ContainsKey(item.Key))
                {
                    EvoleSkillCondition con = dataSkill.EvoleConditionData[item.Key];
                    if (dicStringCurrentSkill.ContainsKey(con.keyCondition1) && dicStringCurrentSkill.ContainsKey(con.keyCondition2))
                    {
                        if (dicStringCurrentSkill[con.keyCondition1] >= con.valueCondition1 && dicStringCurrentSkill[con.keyCondition2] >= con.valueCondition2)
                        {
                            result.Add(item.Key.ToString());
                            if (result.Count == 3)
                            {
                                return result;
                            }
                        }
                    }
                }
            }
            #endregion
            #region Add Used Skill
            foreach (KeyValuePair<EActiveSkill, ActiveSkillConfig> item in dataSkill.ActiveSkillConfigData)
            {
                if (activeSkillDict.ContainsKey(item.Key))
                {
                    if (activeSkillDict[item.Key].CurrentLevel < item.Value.MaxLevel)
                    {
                        //DebugCustom.Log("Add Active"+ item.Key);
                        DicSkillRate.Add(item.Key.ToString(), item.Value.Priority);
                    }
                }
                

            }
            foreach (KeyValuePair<EPassiveSkill, PassiveSkillConfig> item in dataSkill.PassiveSkillData)
            {
                if (passiveSkillUpgradeDict != null)
                {
                    if (passiveSkillUpgradeDict.ContainsKey(item.Key))
                    {
                        if (passiveSkillUpgradeDict[item.Key] < item.Value.MaxLevel)
                        {
                            //DebugCustom.Log("Add Pasive"+ item.Key);
                            DicSkillRate.Add(item.Key.ToString(), item.Value.Priority);
                        }
                    }
                }
            }
            if (DicSkillRate.Count >= 1)
            {
                List<string> _result = Helper.GetRandomListByPercent(DicSkillRate, 1);
                result.AddRange(_result);
            }
            if (result.Count == 3)
            {
                return result;
            }
            #endregion
            #region Add Support Evole
            else if ((passiveSkillUpgradeDict.Count < Constant.MAX_SKILL_STLOT))
            {
                DicSkillRate = new Dictionary<string, float>();
                foreach (KeyValuePair<EPassiveSkill, PassiveSkillConfig> item in dataSkill.PassiveSkillData)
                {
                    if (passiveSkillUpgradeDict != null)
                    {
                        if (!passiveSkillUpgradeDict.ContainsKey(item.Key))
                        {
                            if (dataSkill.DicEvole.ContainsKey(item.Key.ToString()))
                            {
                                var l = dataSkill.DicEvole[item.Key.ToString()];
                                for (int i = 0; i < l.Count; i++)
                                {
                                    if (dicStringCurrentSkill.ContainsKey(l[i]))
                                    {
                                        if (!result.Contains(item.Key.ToString()))
                                        {
                                            DicSkillRate.Add(item.Key.ToString(), item.Value.Priority);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (dataSkill.DicEvole.ContainsKey(item.Key.ToString()))
                        {
                            var l = dataSkill.DicEvole[item.Key.ToString()];
                            for (int i = 0; i < l.Count; i++)
                            {
                                if (dicStringCurrentSkill.ContainsKey(l[i]))
                                {
                                    if (!result.Contains(item.Key.ToString()))
                                    {
                                        DicSkillRate.Add(item.Key.ToString(), item.Value.Priority);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (DicSkillRate.Count >= 1)
            {
                List<string> _result = Helper.GetRandomListByPercent(DicSkillRate, 1);
                result.AddRange(_result);
            }
            if (result.Count == 3)
            {
                return result;
            }
            #endregion
            #region Add Vip Skill
            else
            {
                DicSkillRate = new Dictionary<string, float>();
                EskillFlags flag;
                foreach (KeyValuePair<EActiveSkill, ActiveSkillConfig> item in dataSkill.ActiveSkillConfigData)
                {
                    if (!activeSkillDict.ContainsKey(item.Key))
                    {
                        if (activeSkillDict.Count + evoleSkillDict.Count < Constant.MAX_SKILL_STLOT && !IsEvoled(item.Key.ToString()) && item.Value.IsFreeToDrop)
                        {
                            if (Helper.TryToEnum<EskillFlags>(item.Key.ToString(), out flag) && skillVip.HasFlag(flag))
                            {
                                if(!result.Contains(item.Key.ToString()))
                                DicSkillRate.Add(item.Key.ToString(), item.Value.Priority);
                            }
                        }
                    }

                }
                foreach (KeyValuePair<EPassiveSkill, PassiveSkillConfig> item in dataSkill.PassiveSkillData)
                {
                    if (passiveSkillUpgradeDict != null)
                    {
                        if (passiveSkillUpgradeDict.Count < Constant.MAX_SKILL_STLOT && !passiveSkillUpgradeDict.ContainsKey(item.Key))
                        {
                            if (Helper.TryToEnum<EskillFlags>(item.Key.ToString(), out flag) && skillVip.HasFlag(flag))
                            {
                                if (item.Key == EPassiveSkill.Revival)
                                {
                                    if (!Player.Instance.IsGetRevieSkill)
                                    {
                                        if (!result.Contains(item.Key.ToString()))
                                            DicSkillRate.Add(item.Key.ToString(), item.Value.Priority);
                                    }
                                }
                                else if (!result.Contains(item.Key.ToString()))
                                    DicSkillRate.Add(item.Key.ToString(), item.Value.Priority);
                            }
                            
                        }
                    }
                    else
                    {
                        if (Helper.TryToEnum<EskillFlags>(item.Key.ToString(), out flag) && skillVip.HasFlag(flag))
                        {
                            if (item.Key == EPassiveSkill.Revival)
                            {
                                if (!Player.Instance.IsGetRevieSkill)
                                {
                                    if (!result.Contains(item.Key.ToString()))
                                        DicSkillRate.Add(item.Key.ToString(), item.Value.Priority);
                                }
                            }
                            else if (!result.Contains(item.Key.ToString()))
                                DicSkillRate.Add(item.Key.ToString(), item.Value.Priority);
                        }
                    }
                }
            }
            if (DicSkillRate.Count >= 1)
            {
                List<string> _result = Helper.GetRandomListByPercent(DicSkillRate, 1);
                result.AddRange(_result);
            }
            if (result.Count == 3)
            {
                return result;
            }
            #endregion
            #region Add Random
            else
            {
                DicSkillRate = new Dictionary<string, float>();
                EskillFlags flag;
                foreach (KeyValuePair<EActiveSkill, ActiveSkillConfig> item in dataSkill.ActiveSkillConfigData)
                {
                    if (!activeSkillDict.ContainsKey(item.Key))
                    {
                        if (activeSkillDict.Count + evoleSkillDict.Count < Constant.MAX_SKILL_STLOT && !IsEvoled(item.Key.ToString()) && item.Value.IsFreeToDrop)
                        {
                            if (!Helper.TryToEnum<EskillFlags>(item.Key.ToString(), out flag) || !skillVip.HasFlag(flag))
                            {
                                if (!result.Contains(item.Key.ToString()))
                                    DicSkillRate.Add(item.Key.ToString(), item.Value.Priority);
                            }
                        }
                    }

                }
                foreach (KeyValuePair<EPassiveSkill, PassiveSkillConfig> item in dataSkill.PassiveSkillData)
                {
                    if (passiveSkillUpgradeDict != null)
                    {
                        if (passiveSkillUpgradeDict.Count < Constant.MAX_SKILL_STLOT && !passiveSkillUpgradeDict.ContainsKey(item.Key))
                        {
                            if (!Helper.TryToEnum<EskillFlags>(item.Key.ToString(), out flag) || !skillVip.HasFlag(flag))
                            {
                                if (item.Key == EPassiveSkill.Revival)
                                {
                                    if (!Player.Instance.IsGetRevieSkill)
                                    {
                                        if (!result.Contains(item.Key.ToString()))
                                            DicSkillRate.Add(item.Key.ToString(), item.Value.Priority);
                                    }
                                }
                                else if (!result.Contains(item.Key.ToString()))
                                    DicSkillRate.Add(item.Key.ToString(), item.Value.Priority);
                            }

                        }
                    }
                    else
                    {
                        if (!Helper.TryToEnum<EskillFlags>(item.Key.ToString(), out flag) || !skillVip.HasFlag(flag))
                        {
                            if (item.Key == EPassiveSkill.Revival)
                            {
                                if (!Player.Instance.IsGetRevieSkill)
                                {
                                    if (!result.Contains(item.Key.ToString()))
                                        DicSkillRate.Add(item.Key.ToString(), item.Value.Priority);
                                }
                            }
                            else if (!result.Contains(item.Key.ToString()))
                                DicSkillRate.Add(item.Key.ToString(), item.Value.Priority);
                        }
                    }
                }
            }
            #endregion
            if (DicSkillRate.Count >= 3 - result.Count)
            {
                List<string> _result = Helper.GetRandomListByPercent(DicSkillRate, 3 - result.Count);
                result.AddRange(_result);
                return result;
            }
            else
            {
                foreach (var item in DicSkillRate)
                {
                    result.Add(item.Key);
                }
                return result;
            }

        }
        public int GetCurrentLevel(string key)
        {
            if (dicStringCurrentSkill.ContainsKey(key))
                return dicStringCurrentSkill[key];
            else return 0;
        }
        public int GetMaxLevel(string key)
        {
            EActiveSkill acType;
            EPassiveSkill psType;
            EEvoleSkill evType;
            if (Helper.TryToEnum<EActiveSkill>(key, out acType))
            {
                return dataSkill.ActiveSkillConfigData[acType].MaxLevel;
            }
            else if (Helper.TryToEnum<EPassiveSkill>(key, out psType))
            {
                return dataSkill.PassiveSkillData[psType].MaxLevel;
            }
            else if (Helper.TryToEnum<EEvoleSkill>(key, out evType))
            {
                return dataSkill.EvoleSkillConfigData[evType].MaxLevel;
            }
            else
            {
                DebugCustom.LogError("Wrong input string");
                return 0;
            }
        }
        public void AddSkillByString(string key)
        {
            EActiveSkill acType;
            EPassiveSkill psType;
            EEvoleSkill evType;
            if (Helper.TryToEnum<EActiveSkill>(key, out acType))
            {
                AddSkill(acType);
            }
            else if (Helper.TryToEnum<EPassiveSkill>(key, out psType))
            {
                AddPassive(psType);
            }
            else if (Helper.TryToEnum<EEvoleSkill>(key, out evType))
            {
                AddEvoleSkill(evType);
            }
            else
            {
                DebugCustom.LogError("Null Input String");
            }
            TigerForge.EventManager.EmitEvent(Constant.ON_SKILL_CHANGE);
        }
        public bool IsEvoled(string asId)
        {
            foreach (KeyValuePair<EEvoleSkill, ActiveSkill> item in evoleSkillDict)
            {
                EvoleSkillCondition condition = dataSkill.EvoleConditionData[item.Key];
                if (asId == condition.keyCondition1 || asId == condition.keyCondition2)
                {
                    return true;
                }
            }
            return false;
        }
        public bool HaveEvoleSkill(out EEvoleSkill evoleSkill)
        {
            foreach (KeyValuePair<EEvoleSkill, ActiveSkillConfig> item in dataSkill.EvoleSkillConfigData)
            {
                if (!evoleSkillDict.ContainsKey(item.Key))
                {
                    EvoleSkillCondition con = dataSkill.EvoleConditionData[item.Key];
                    if (dicStringCurrentSkill.ContainsKey(con.keyCondition1) && dicStringCurrentSkill.ContainsKey(con.keyCondition2))
                    {
                        if (dicStringCurrentSkill[con.keyCondition1] >= con.valueCondition1 && dicStringCurrentSkill[con.keyCondition2] >= con.valueCondition2)
                        {
                            evoleSkill = item.Key;
                            return true;
                        }
                    }
                }
            }
            evoleSkill = EEvoleSkill.Lightning_Support_Evole;
            return false;
        }
        public Sprite GetSkillSprite(string skillCode)
        {
            EActiveSkill acType;
            EPassiveSkill psType;
            EEvoleSkill evType;
            Sprite icon = null;
            if (Helper.TryToEnum<EActiveSkill>(skillCode, out acType))
            {
                icon = GetActiveSprite(acType);
            }
            else if (Helper.TryToEnum<EPassiveSkill>(skillCode, out psType))
            {
                icon = GetPassiveSprite(psType);
            }
            else if (Helper.TryToEnum<EEvoleSkill>(skillCode, out evType))
            {
                icon = GetEvoleSprite(evType);
            }
            else
            {
                DebugCustom.LogError("Wrong Skill Code");
            }
            return icon;
        }
        public Sprite GetEvoleSprite(EEvoleSkill type)
        {
            return Helper.LoadFromPath<Sprite>("NewProject/UI_UX/icon_skill/skill_evolution" + "/" + type.ToString());
        }
        public Sprite GetActiveSprite(EActiveSkill type)
        {
            return Helper.LoadFromPath<Sprite>("NewProject/UI_UX/icon_skill/skill_normal" + "/" + type.ToString());
        }
        public Sprite GetPassiveSprite(EPassiveSkill type)
        {
            return Helper.LoadFromPath<Sprite>("NewProject/UI_UX/icon_skill/skill_support" + "/" + type.ToString());
        }
        public bool IsEvoleSkill(string key)
        {

            EEvoleSkill Type;
            if (Helper.TryToEnum<EEvoleSkill>(key, out Type))
            {
                return true;
            }
            return false;
        }
        public bool IsActiveSkill(string key)
        {
            EActiveSkill Type;
            if (Helper.TryToEnum<EActiveSkill>(key, out Type))
            {
                return true;
            }
            return false;

        }
        public bool IsPassiveSkill(string key)
        {
            EPassiveSkill Type;
            if (Helper.TryToEnum<EPassiveSkill>(key, out Type))
            {
                return true;
            }
            return false;

        }
        public List<string> GetLevelUpEffect(string key)
        {
            return GetLevelUpEffect(key, dicStringCurrentSkill[key] + 1);
        }
        public List<string> GetLevelUpEffect(string key, int level)
        {
            //DebugCustom.Log(key, level);
            List<string> result = new List<string>();
            EActiveSkill Type;
            if (Helper.TryToEnum<EActiveSkill>(key, out Type))
            {
                Dictionary<int, string> dic1 = dataSkill.ActiveSkillLevelUpConfig[Type];
                string data = dic1[level];
                Dictionary<string, string> dic2 = Helper.GetSubStats(data);
                foreach (KeyValuePair<string, string> item in dic2)
                {
                    result.Add(item.Key);
                }
                return result;
            }
            else
            {
                DebugCustom.LogError(" Null Here");
                return null;
            }
        }

        public bool IsAlreadyHasEvoleSkill(List<UpgradeResult> lista, string key)
        {
            for (int i = 0; i < lista.Count; i++)
            {
                if (key == lista[i].key)
                    return true;
            }
            return false;
        }
        public List<string> GetTutSkill(int lv)
        {
            List<string> result = new List<string>();
            Dictionary<string, float> dicrate = GetAviableSkillRateDic();

            if (lv == 1)
            {
                string r1 = EActiveSkill.Shock_Zone_Support.ToString();
                result.Add(r1);
                for (int i = 0; i < 2; i++)
                {
                    string r2 = Helper.GetRandomByPercent(dicrate);
                    while (result.Contains(r2))
                    {
                        r2 = Helper.GetRandomByPercent(dicrate);
                    }
                    result.Add(r2);
                }
            }
            else if (lv == 2)
            {
                string r1 = EActiveSkill.Machine_Gun.ToString();
                result.Add(r1); 
                for (int i = 0; i < 2; i++)
                {
                    string r2 = Helper.GetRandomByPercent(dicrate);
                    while (result.Contains(r2))
                    {
                        r2 = Helper.GetRandomByPercent(dicrate);
                    }
                    result.Add(r2);
                }
            }
            else if (lv == 3)
            {
                result.Add(EActiveSkill.Machine_Gun.ToString());
                result.Add(EPassiveSkill.Bullet_Speed_Increase.ToString());
                result.Add(EActiveSkill.Molotov_Support.ToString());
            }
            else
            {
                return GetListSkillReRoll();
            }
            return result;
        }
    }

    public class UpgradeResult
    {
        public string key;
        public int level;
    }
}

