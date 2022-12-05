using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Data;
using WE.Unit;
using System;
using DG.Tweening;
using WE.Utils;
using TigerForge;
using WE.Support;
using WE.Game;
using WE.Pooling;
using UniRx;
using Dragon.SDK;
namespace WE.Manager
{
    public class GameplayManager : MonoBehaviour
    {
        public static GameplayManager Instance;
        public GameState State { get; private set; }

        public GameType CurrentGameplayType { get; private set; }
        int maxMapTime;
        public int CurrentCoinCount => currentCoinCount;
        public int CurrentKillCount => currentKillCount;
        public int CurrentTimePlay => currentTimePlay;
        public int MaxTimePlay => maxMapTime;
        int currentCoinCount;
        int currentKillCount;
        //int currentKeyCount;
        int currentTimePlay;
        public int CurrentMap => Player.Instance.CurrentMap;
        public int CurrentLevel => currentLevel;
        int currentLevel;
        bool isEndGameWave;
        bool lockExp;
        public int enemyDropExpPerSec;
        [Range(0,1)]
        public float chanceDropKey = 0.1f;
        int currentExpDrop;
        int cachedExp;
        bool canShowGameInter;
        public bool CanShowGameInter
        {
            get => canShowGameInter && CurrentGameplayType != GameType.Tutorial && !Player.Instance.IsOnNoAds() && !Constant.IS_TESTER_JACKAL;
        }
        public float CurrentTimeEvaluate => (float)currentTimePlay / MaxTimePlay;
        CompositeDisposable interAdsDispose;
        private void Awake()
        {
            Instance = this;
            Application.targetFrameRate = 300;
        }
        private void Start()
        {
            EventManager.StartListening(Constant.TIMER_TICK_EVENT, OnTick);
        }
        private void OnDisable()
        {
            EventManager.StopListening(Constant.TIMER_TICK_EVENT, OnTick);
        }
        public void StartGame(GameType type)
        {
            CurrentGameplayType = type;
            State = GameState.Gameplay;
            maxMapTime = DataManager.Instance.dataZoneMultiplier.zoneTimePlay;
            isEndGameWave = false;
            currentTimePlay = 0;
            currentCoinCount = 0;
            currentKillCount = 0;
            //currentKeyCount = 0;
            currentLevel = 1;
            Player.Instance.StartGame();
            CarController.Instance.CurrentVisual.InitVisual();
            ResolutionManager.Instance.ZoomInGamePannel();
            SoundManager.Instance.PlayMusicBattle();
            EnemySpawner.Instance.StartGame();
            if (type != GameType.Campaign)
                MapController.Instance.mapLooper.InitMap();
            ending = false;
            currentExpDrop = 0;
            cachedExp = 0;
            if (type != GameType.Tutorial)
            {
                UIManager.Instance.StartGame();
                for (int i = 0; i < 10; i++)
                {
                    DropExp(Helper.GetRandomPosInScreen(), 1);
                }
                Analytics.LogLevelStart(Player.Instance.CurrentMap);
                SkillController.Instance.GameInit();
                canShowGameInter = false;
                StartObserbInter();
            }
            else
            {
                TutorialController.Instance.StartTutorial();
            }
        }
        void StartObserbInter()
        {
            if (interAdsDispose != null)
                interAdsDispose.Dispose();
            interAdsDispose = new CompositeDisposable();
            Observable.Timer(System.TimeSpan.FromSeconds(45)).Subscribe(_ => OnAdsInterCoundown()).AddTo(interAdsDispose);
        }
        public void OnAdsInterCoundown()
        {
            canShowGameInter = true; 
            StartObserbInter();
        }
        public void OnAdsInterShow()
        {
            canShowGameInter = false;
            StartObserbInter();
        }
        public float GetMultiple()
        {
            ZoneConfig config = MapController.Instance.currentMapConfig;
            float ingameMulti = EnemySpawner.Instance.IngameMulti;
            if (CurrentGameplayType == GameType.Campaign)
                return config.zoneMultiplier * ingameMulti;
            else if (CurrentGameplayType == GameType.Endless)
                return ingameMulti;
            else return 1;
        }
        public float GetCoinMultiple()
        {
            if (CurrentGameplayType == GameType.Campaign)
                return GetMultiple();
            else if (CurrentGameplayType == GameType.Endless)
                return EnemySpawner.Instance.CoinMulti;
            else return 1;
        }
        public void ShowPopupEndGame(bool win)
        {
            if (win)
            {
                UIManager.Instance.ShowPopupWin();
            }
            else
            {
                if(CurrentGameplayType == GameType.Tutorial)
                    EventManager.EmitEvent(Constant.TUT_ON_QUIT_TUT);
                UIManager.Instance.ShowPopupLose();
            }
        }
        //public void ObserbShowPopup(bool win)
        //{

        //}
        bool ending;
        public void EndGame(bool win)
        {
            if (!ending)
            {
                ending = true;
                StartCoroutine(IEEndGame(win));
            }
        }
        IEnumerator IEEndGame(bool win)
        {
            SetState(GameState.MainUi);

            if (interAdsDispose != null)
            {
                interAdsDispose.Dispose();
            }
            SkillController.Instance.OnEndGame();
            EnemySpawner.Instance.EndGame();

            yield return new WaitForSecondsRealtime(0.1f);
            ObjectPooler.Instance.ClearPool();
            yield return new WaitForSecondsRealtime(0.1f);

            Player.Instance.LevelEnd(win && CurrentGameplayType != GameType.Tutorial);
            UIManager.Instance.ReturnHome();
            //MapController.Instance.Init();
            yield return new WaitForSecondsRealtime(0.1f);
            ResolutionManager.Instance.ZoomInUiPanel();
            SoundManager.Instance.PlayMusicHome();
        }
        public void OnTick()
        {
            if (State == GameState.Gameplay)
            {
                if (CurrentGameplayType == GameType.Campaign)
                {
                    if (!isEndGameWave)
                    {
                        if (currentTimePlay < maxMapTime)
                        {
                            currentTimePlay++;
                        }
                        else
                        {
                            isEndGameWave = true;
                            SpawnLastWave();
                        }
                    }
                }
                else
                {
                    currentTimePlay++;
                }
                
                currentExpDrop = 0;
                Player.Instance.AutoRecovery();
                EventManager.EmitEvent(Constant.GAME_TICK_EVENT);
            }
        }
        public void SpawnLastWave()
        {
            EnemySpawner.Instance.SpawnLastWave();
        }
        public void SetState(GameState _state)
        {
            State = _state;
        }
        public void Vibrate()
        {
            if (Player.Instance.GetVibrateSetting())
            {
                Handheld.Vibrate();
            }
        }
        public void AddExp(float val)
        {
            if(!lockExp)
                EventManager.EmitEventData(Constant.ON_RECEVICE_EXP, val);
        }
        public void LevelUp()
        {
            currentLevel++;
            UIManager.Instance.ShowPopupSelectSkill();
        }
        public void OnEnemyDie(Enemy e)
        {
            currentKillCount++;
            EventManager.EmitEvent(Constant.ON_ENEMY_DIE);
            if (e.dontDropOnDie)
                return;
            int rand = UnityEngine.Random.Range(0, 100);
            if(rand <= e.ChanceToDropExp)
            {
                if (currentExpDrop < enemyDropExpPerSec)
                {
                    currentExpDrop++;
                    int valueXp = e.ExpDrop + cachedExp;
                    cachedExp = 0;
                    DropExp(e.transform.position, valueXp);
                }
                else
                {
                    cachedExp += e.ExpDrop;
                }
            }
        }
        public void AddInGameCoin(int val)
        {
            SoundManager.Instance.PlaySoundFx(SoundManager.Instance.revicedCoinSfx);
            currentCoinCount += val;
            EventManager.EmitEventData(Constant.ON_COINS_CHANGE_IN_GAME, val);
        }
        public void StartWarning()
        {

            UIManager.Instance.Warning(5);
            ResolutionManager.Instance.ZoomOutHighPannel();
        }
        //public void ChangeCamSize(float value, float delay = 0)
        //{
        //    if (delay <= 0)
        //    {
        //        ResolutionManager.Instance.OnChangeScreenSize();
        //    }
        //    else
        //    {
        //        float size = Camera.main.orthographicSize;
        //        DOTween.To(() => size, x => size = x, value, delay).OnUpdate(() => { ResolutionManager.Instance.OnChangeScreenSize(); }).OnComplete(() => { });
        //    }
        //}
        public void OnPlayerHeal(float value)
        {
            Helper.OnEnemyHit((int)value, Player.Instance.transform.position, false, true);
        }
        public void OnEnemyHit(Vector3 pos, float damage, bool isCrit)
        {
            Helper.OnEnemyHit(damage, pos, isCrit);
        }
        public void ReviceItem(EItemInGame item, int _value = 0)
        {
            int val = 0;
            switch (item)
            {
                case EItemInGame.Small_Coin:
                    val = (int)(DataManager.Instance.dataZoneChestDrop.ZoneChestDropData[EItemInGame.Small_Coin.ToString()].Quantity
                        * Player.Instance.CoinBonus * GetCoinMultiple());
                    AddInGameCoin(val);
                    break;
                case EItemInGame.Normal_Coin:
                    val = (int)(DataManager.Instance.dataZoneChestDrop.ZoneChestDropData[EItemInGame.Normal_Coin.ToString()].Quantity
                        * Player.Instance.CoinBonus * GetCoinMultiple());
                    AddInGameCoin(val);
                    break;
                case EItemInGame.Big_Coin:
                    val = (int)(DataManager.Instance.dataZoneChestDrop.ZoneChestDropData[EItemInGame.Big_Coin.ToString()].Quantity
                        * Player.Instance.CoinBonus * GetCoinMultiple());
                    AddInGameCoin(val);
                    break;
                case EItemInGame.Magnetic:
                    List<ItemInGame> listExp = ObjectPooler.Instance.listExp;
                    //for (int i = 0; i < listExp.Count; i++)
                    //{
                    //    listExp[i].Recevied();
                    //}
                    for (int i = 0; i < listExp.Count; i++)
                    {
                        listExp[i].Recevied();
                    }
                    //if (listExp.Count < 1000)
                    //{
                    //    for (int i = 0; i < listExp.Count; i++)
                    //    {
                    //        listExp[i].Recevied();
                    //    }
                    //}
                    //else
                    //{
                    //    float chance = 1000f / listExp.Count;
                    //    for (int i = 0; i < listExp.Count; i++)
                    //    {
                    //        float rand = UnityEngine.Random.Range(0f, 1f);
                    //        if (rand < chance)
                    //        {
                    //            listExp[i].Recevied();
                    //        }
                    //        else
                    //        {
                    //            listExp[i].Recevied(false);
                    //        }
                    //    }
                    //}
                    SoundManager.Instance.PlaySoundFx(SoundManager.Instance.revicedItemSfx);
                    Helper.SpawnEffect(ObjectPooler.Instance.fxReviceChest, Player.Instance.transform.position, null);
                    break;
                case EItemInGame.Bomb:
                    SoundManager.Instance.PlaySoundFx(SoundManager.Instance.revicedItemSfx);
                    UIManager.Instance.FlashScreen();
                    EnemySpawner.Instance.DealDamageToAllEnemy(Player.Instance.AttackDamage * 20);
                    break;
                case EItemInGame.Heal:
                    SoundManager.Instance.PlaySoundFx(SoundManager.Instance.revicedItemSfx);
                    val = DataManager.Instance.dataZoneChestDrop.ZoneChestDropData[EItemInGame.Heal.ToString()].Quantity;
                    //Helper.OnEnemyHit(val, Player.Instance.transform.position, false, true);
                    Player.Instance.RecoverHp(val);
                    OnPlayerHeal(val * Player.Instance.MaxHp / 100);
                    Helper.SpawnEffect(ObjectPooler.Instance.fxReviceChest, Player.Instance.transform.position, null);
                    break;
                case EItemInGame.Exp:
                    SoundManager.Instance.PlayRevicedExpSound();
                    float _vale = _value * (1 + Player.Instance.ExpBonus / 100);
                    AddExp(_vale);
                    break;
                case EItemInGame.Boss_Chest:
                    SoundManager.Instance.PlaySoundFx(SoundManager.Instance.revicedItemSfx);
                    UIManager.Instance.ShowPopupOpenChestBoss();
                    Helper.SpawnEffect(ObjectPooler.Instance.fxReviceChest, Player.Instance.transform.position, null);
                    break;
                case EItemInGame.EndlessKey:
                    //currentKeyCount++;
                    Player.Instance.AddEndlessKey(1);
                    SoundManager.Instance.PlaySoundFx(SoundManager.Instance.revicedItemSfx);
                    Helper.SpawnEffect(ObjectPooler.Instance.fxReviceChest, Player.Instance.transform.position, null);
                    break;
                default:
                    break;
            }
        }
        public void NormalChestDie(Vector3 pos)
        {
            string result = Helper.GetRandomByPercent(DataManager.Instance.dataZoneChestDrop.ZoneChestDropRate);
            if (Helper.IsLuckApply())
            {
                float firstRate = DataManager.Instance.dataZoneChestDrop.ZoneChestDropRate[result];
                string _result = Helper.GetRandomByPercent(DataManager.Instance.dataZoneChestDrop.ZoneChestDropRate);
                float secondRate = DataManager.Instance.dataZoneChestDrop.ZoneChestDropRate[_result];
                if (secondRate > firstRate)
                    result = _result;
            }
            EItemInGame type;
            Helper.TryToEnum<EItemInGame>(result, out type);
            DropItem(pos, type);
        }
        public void BossDie(Enemy boss)
        {
            if (boss.dontDropOnDie)
            {
                return;
            }
            Vector3 pos = boss.transform.position;
            int exp = boss.ExpDrop;
            if (CurrentGameplayType == GameType.Campaign)
            {
                bool dropKey = UnityEngine.Random.Range(0, 1) < chanceDropKey;
                if (!dropKey && Helper.IsLuckApply())
                    dropKey = UnityEngine.Random.Range(0, 1) < chanceDropKey;
                if (dropKey)
                    DropItem(pos + (Vector3)UnityEngine.Random.insideUnitCircle.normalized * 0.5f, EItemInGame.EndlessKey);
            }
            DropItem(pos + (Vector3)UnityEngine.Random.insideUnitCircle.normalized * 0.5f, EItemInGame.Big_Coin);
            if (CurrentGameplayType != GameType.Tutorial)
            {
                DropItem(pos + (Vector3)UnityEngine.Random.insideUnitCircle.normalized * 0.5f, EItemInGame.Boss_Chest);
                DropItem(pos + (Vector3)UnityEngine.Random.insideUnitCircle.normalized * 0.5f, EItemInGame.Magnetic);
                DropExp(pos + (Vector3)UnityEngine.Random.insideUnitCircle.normalized * 0.5f, exp);
            }
            else
            {
                for (int i = 0; i < 9; i++)
                {
                    DropItem(Player.Instance.transform.position + (Vector3)UnityEngine.Random.insideUnitCircle.normalized * 2, EItemInGame.Big_Coin);
                }
            }
        }
        public void DropItem(Vector3 pos, EItemInGame _item)
        {
            string result = _item.ToString();
            int val = 0;
            if(DataManager.Instance.dataZoneChestDrop.ZoneChestDropData.ContainsKey(result))
                val = DataManager.Instance.dataZoneChestDrop.ZoneChestDropData[result].Quantity;
            Helper.SpawnItemIngame(pos, val, _item);
        }
        public void DropExp(Vector3 pos, int value)
        {
            Helper.SpawnExp(pos, value);
        }
        public int GetBigCoinValue()
        {
           return (int)(DataManager.Instance.dataZoneChestDrop.ZoneChestDropData[EItemInGame.Big_Coin.ToString()].Quantity
                        * Player.Instance.CoinBonus * GetCoinMultiple());
        }
        public void LockExp()
        {
            lockExp = !lockExp;
        }
    }
}
