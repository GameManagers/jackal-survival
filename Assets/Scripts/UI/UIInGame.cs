using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using WE.UI;
using WE.Manager;
using TigerForge;
using WE.Utils;
using WE.Support;
using DG.Tweening;
using UnityEngine.UI;
using Spine.Unity;
using WE.Unit;

namespace WE.UI
{
	public class UIInGame : UIBase
	{
		public TextMeshProUGUI textCount;
		public TextMeshProUGUI textCoin;
		public TextMeshProUGUI textKill;
		public TextMeshProUGUI textLevel;
		public Transform expBar;
		public Image expBarFlash;
		public CurrentSkillBar skillBar;


		public SkeletonGraphic skeletonGraphic;

		[SerializeField, SpineAnimation(dataField = "skelGraphic")] string animIdle;
		[SerializeField, SpineAnimation(dataField = "skelGraphic")] string animShow;
		[SerializeField, SpineAnimation(dataField = "skelGraphic")] string animIdleShow;
		[SerializeField, SpineAnimation(dataField = "skelGraphic")] string animHide;
		float currentExp;
		float nextExpLevel;
		public override void InitUI()
		{
			EventManager.StartListening(Constant.ON_COINS_CHANGE_IN_GAME, OnCoinChange);
			EventManager.StartListening(Constant.GAME_TICK_EVENT, OnTick);
			EventManager.StartListening(Constant.ON_ENEMY_DIE, OnEnemyKill);
			EventManager.StartListening(Constant.ON_RECEVICE_EXP, OnReceiveExp);
			EventManager.StartListening(Constant.ON_SKILL_CHANGE, InitSkillBar);
			expBar.localScale = new Vector3(0, 1, 1);
			currentExp = 0;
			nextExpLevel = DataManager.Instance.dataGlobalUpgrade.GetNextLevelExp(0, GameplayManager.Instance.CurrentLevel);
			textLevel.text = "LV.1";
			textCount.text = "00:00";
			textCoin.text = "0";
			textKill.text = "0";
			InitSkillBar();
			cachedFrame = false;
			cachedExp = 0;
		}
		private void OnDisable()
		{
			EventManager.StopListening(Constant.ON_COINS_CHANGE_IN_GAME, OnCoinChange);
			EventManager.StopListening(Constant.GAME_TICK_EVENT, OnTick);
			EventManager.StopListening(Constant.ON_ENEMY_DIE, OnEnemyKill);
			EventManager.StopListening(Constant.ON_RECEVICE_EXP, OnReceiveExp);
			EventManager.StopListening(Constant.ON_SKILL_CHANGE, InitSkillBar);
		}
		public void InitSkillBar()
		{
			skillBar.InitUI();
		}
		public void OnClick()
		{
			PauseGame();
		}
		public void OnTick()
		{
			textCount.text = Helper.ConvertTimer(GameplayManager.Instance.CurrentTimePlay);
		}
		public void OnEnemyKill()
		{
			textKill.text = GameplayManager.Instance.CurrentKillCount.ToString();
		}
		public void OnCoinChange()
		{
			textCoin.text = GameplayManager.Instance.CurrentCoinCount.ToString();
		}
		public void PauseGame()
		{
			UIManager.Instance.PauseGame();
		}
		bool cachedFrame = false;
		float cachedExp;
		public void OnReceiveExp()
		{
			float exp = EventManager.GetFloat(Constant.ON_RECEVICE_EXP);
			AddExp(exp);
		}
		public void AddExp(float exp)
		{
			if (!cachedFrame)
			{
				currentExp += exp;
				expBar.transform.DOKill();
				expBarFlash.DOKill();
				if (currentExp >= nextExpLevel)
				{
					cachedFrame = true;
					GameplayManager.Instance.LevelUp();
					textLevel.text = "LV." + GameplayManager.Instance.CurrentLevel.ToString();
					currentExp -= nextExpLevel;
					nextExpLevel = DataManager.Instance.dataGlobalUpgrade.GetNextLevelExp(nextExpLevel, GameplayManager.Instance.CurrentLevel);
					expBar.transform.localScale = new Vector3(0, 1, 1);
				}
				expBar.transform.DOScale(new Vector3((currentExp / nextExpLevel), 1, 1), 0.2f);
				expBarFlash.color = new Color(1, 1, 1, 0.6f);
				expBarFlash.DOFade(0, 0.2f);
			}
			else
			{
				cachedExp += exp;
			}
		}
		public void Warning(float timer)
		{
			StartCoroutine(IEWarning(timer));
		}
		IEnumerator IEWarning(float t)
		{
			SoundManager.Instance.PlaySoundFx(SoundManager.Instance.warningSfx);
			var track = skeletonGraphic.AnimationState;
			track.ClearTracks();
			track.AddAnimation(2, animShow, false, 0);
			track.AddAnimation(3, animIdleShow, true, 0);
			track.Apply(skeletonGraphic.Skeleton);
			skeletonGraphic.gameObject.SetActive(true);
			yield return new WaitForSeconds(t);
			track.ClearTracks();
			track.AddAnimation(0, animHide, false, 0);
			track.AddAnimation(1, animIdle, true, 0);
			track.Apply(skeletonGraphic.Skeleton);
			yield return new WaitForSeconds(1);
			skeletonGraphic.gameObject.SetActive(false);
		}
		#region Hack
					
		public void SpawnExp()
		{
			UIManager.Instance.SpawnExp();
		}
		public void GetMagnetic()
		{
			GameplayManager.Instance.ReviceItem(EItemInGame.Magnetic);
		}
		public void ToggleImortal()
		{
			Player.Instance.ToggleImortal();
		}
		public void ToggleKillZone()
		{
			Player.Instance.ToggleKillZone();
		}
		public Enemy enmyToSpawn;
		bool spawning = false;
		Coroutine spawnCoroutine;
		public TextMeshProUGUI textCountEnemy;
		private void Update()
		{
			textCountEnemy.text = EnemySpawner.Instance.CurrentEnemy.ToString();
			if (cachedFrame)
			{
				if (GameplayManager.Instance.State == GameState.Gameplay)
				{
					cachedFrame = false;
					float val = nextExpLevel - currentExp;
					if (cachedExp > val)
					{
						AddExp(val);
						cachedExp -= val;
					}
					else
					{
						AddExp(cachedExp);
						cachedExp = 0;
					}
				}
			}
			
		}
		public void ToggleSpawnEnemy()
		{
			if (!spawning)
			{
				StartSpawn();
				spawning = true;
			}
			else
			{
				StopSpawn();
				spawning = false;
			}
		}
		public void StartSpawn()
		{
			spawnCoroutine = StartCoroutine(IESpawn());
		}
		IEnumerator IESpawn()
		{
			while (gameObject.activeInHierarchy)
			{
				yield return new WaitForSeconds(1);
				for (int i = 0; i < 100; i++)
				{
					Helper.SpawnEnemy(enmyToSpawn);
				}
			}
		}
		public void StopSpawn()
		{
			StopCoroutine(spawnCoroutine);
		}
		public void LockExp()
		{
			GameplayManager.Instance.LockExp();
		}
		#endregion
	}
}

