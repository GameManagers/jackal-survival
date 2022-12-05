using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Manager;

namespace WE.Utils
{
    public class Constant
    {
        #region DataConstant
        public static string PLAYER_DATA => "PLAYER_DATA_TEST_4";
        public static bool IS_TESTER_JACKAL => UIManager.Instance.tester;

        //public static int MAX_ZONE => 8;
        #endregion
        #region Event Constant
        public static string TIMER_TICK_EVENT => "TIMER_TICK_EVENT";
        public static string GAME_TICK_EVENT => "GAME_TICK_EVENT";
        public static string TIMER_UPDATE_EVENT => "TIMER_UPDATE_EVENT";


        public static string ON_ENEMY_SPAWNED => "ON_ENEMY_SPAWNED";
        public static string ON_ENEMY_TAKE_DAMAGE => "ON_ENEMY_TAKE_DAMAGE";
        public static string ON_ENEMY_DIE => "ON_ENEMY_DIE";


        public static string ON_COINS_CHANGE => "ON_COINS_CHANGE";
        public static string ON_COINS_CHANGE_IN_GAME => "ON_COINS_CHANGE_IN_GAME";
        public static string ON_CHANGE_ZONE => "ON_CHANGE_ZONE";

        public static string ON_RECEVICE_EXP => "ON_RECEVICE_EXP";

        public static string ON_ADD_GLOBAL_UPGRADE => "ON_ADD_GLOBAL_UPGRADE";

        public static string ON_SKILL_CHANGE => "ON_SKILL_CHANGE";

        public static string ON_SOUND_SETTING_CHANGE => "ON_SOUND_SETTING_CHANGE";


        public static string TUT_ON_FIRST_ENEMY_IN_SCENE => "TUT_ON_FIRST_ENEMY_IN_SCENE";
        public static string TUT_ON_FIRST_ENEMY_DIE => "TUT_ON_FIRST_ENEMY_DIE";
        public static string TUT_ON_FIRST_LEVEL_REACH => "TUT_ON_FIRST_LEVEL_REACH";
        public static string TUT_ON_SECONDS_LEVEL_REACH => "TUT_ON_SECONDS_LEVEL_REACH";
        public static string TUT_ON_THIRD_LEVEL_REACH => "TUT_ON_THIRD_LEVEL_REACH";
        public static string TUT_ON_FIRST_SKILL_MAX => "TUT_ON_FIRST_SKILL_MAX";
        public static string TUT_ON_LEVEL_TUT_END => "TUT_ON_LEVEL_TUT_END";
        public static string TUT_ON_UI_INIT_DONE => "TUT_ON_UI_INIT_DONE";
        public static string TUT_ON_QUIT_TUT => "TUT_ON_QUIT_TUT";

        #endregion
        #region IngameConstant
        public static int CRIT_MULTIPLE = 200;
        public static int MAX_SKILL_STLOT = 6;
        public static int TIME_DELAY_NO_THANKS = 3;
        public static float RETURN_UNSCALE_TIME = 2;
        #endregion
        #region Renderer Constant
        public static string SORTING_LAYER_BackGround = "BackGround";
        public static string SORTING_LAYER_OBSTACLE = "Obstacle";
        public static string SORTING_LAYER_Object = "Object";
        public static string SORTING_LAYER_Bullet = "Bullet";
        public static string SORTING_LAYER_Fx = "Fx";
        public static string SORTING_LAYER_TopObstacle = "TopObstacle";
        public static string SORTING_LAYER_Flying = "Flying";
        public static string SORTING_LAYER_FxFlying = "FxFlying";
        public static string SORTING_LAYER_UI = "UI";
        #endregion
        #region Physic Layer Constant
        public static string LAYER_Player = "Player";
        public static string LAYER_ENEMY = "Enemy";
        public static string LAYER_EnemyBullet = "EnemyBullet";
        public static string LAYER_PlayerBullet = "PlayerBullet";
        public static string LAYER_Obstacle = "Obstacle";
        public static string LAYER_EnemyImpactZone = "EnemyImpactZone";
        public static string LAYER_BlockPlayerAndEnemy = "BlockPlayerAndEnemy";
        public static string LAYER_FlyingEnemy = "FlyingEnemy";
        #endregion
        #region Show Text
        public static string TXT_NO_ADS = "No ads available";
        public static string TXT_NOT_ENOUGH_COIN = "Not enough coins";
        #endregion
        #region string Set
        public static string HP_CHEST = "Hp_Chest";
        public static string COIN_CHEST = "Coin_Chest";
        #endregion
    }
}

