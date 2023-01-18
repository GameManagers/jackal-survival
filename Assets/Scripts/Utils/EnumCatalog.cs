using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GameState
{
    MainUi,
    Gameplay,
    Pause,
}
public enum GameType
{
    Campaign,
    Tutorial,
    Endless,
    PVP,
}
public enum EWinTypePVP
{
    DIE = 0,
    SCORE = 1,
    DISCONNECT = 2,
}
public enum EAnimationState
{
    None = 0,
    Idle = 1,
    Move = 2
}
public enum EVehicleType
{
    Machine_Gun_Vehicle,
    Flame_Gun_Vehicle,
    Rocket_Gun_Vehicle,
    Canon_Gun_Vehicle,
    Short_Gun_Vehicle,
    Blade_Saw_Gun_Vehicle,
    Lazer_Gun_Vehicle,
}
public enum EActiveSkill
{
    Lightning_Support,
    Machine_Gun,
    Rocket_Gun,
    Short_Gun,
    Gravity_Tower_Support,
    Molotov_Support,
    Cannon_Gun,
    Lazer_Gun,
    Flame_Gun,
    Shock_Zone_Support,
    EVA_Shield,
    Saw_Blade_Support,
    Zeus_Spear,
    Tesla_Coil_Support,
    Blade_Saw_Gun,
    Shock_Wave_Support,
    Drone_Type_A_Support,
    Drone_Type_B_Support,
}
public enum EEvoleSkill
{
    Lightning_Support_Evole,
    Machine_Gun_Evole,
    Rocket_Gun_Evole,
    Short_Gun_Evole,
    Gravity_Tower_Support_Evole,
    Molotov_Support_Evole,
    Cannon_Gun_Evole,
    Lazer_Gun_Evole,
    Flame_Gun_Evole,
    Shock_Zone_Support_Evole,
    EVA_Shield_Evole,
    Saw_Blade_Support_Evole,
    Zeus_Spear_Evole,
    Tesla_Coil_Support_Evole,
    Blade_Saw_Gun_Evole,
    Shock_Wave_Support_Evole,
    Drone_Support_Evole,
}
public enum EPassiveSkill
{
    Cooldown_Increase,
    Bullet_Speed_Increase,
    Bullet_Damage_Increase,
    Max_Hp_Increase,
    Exp_Increase,
    Item_Absorb_Range_Increase,
    Effect_Area_Increase,
    Effect_Duration_Increase,
    Coin_Value_Increase,
    Hp_Recovery_Increase,
    Damage_Reviced_Reduction_Increase,
    Luck_Increase,
    Projectile_Number_Increase,
    Move_Speed_Increase,
    Crit_Incease,
    Push_Back_Force_Increase,
    Revival,
}
public enum EUnlockType
{
    Coin,
    Kill_Count,
    Spend_Coin,
    Level_Reach,
    Ads
}
public enum EVehicleBaseMainAtribute
{
    Base_Hp,
    Base_Attack,
    Base_Move_Speed,
}
public enum EVehicleUpgrade
{
    Hp_Increase,
    Attack_Increase,
    //Move_Speed_Increase,
    //Hp_Recovery_Increase,
    //Cooldown_Increase,
    //Bullet_Speed_Increase,
    //Effect_Area_Increase,
    //Effect_Duration_Increase,
    //Push_Back_Force_Increase,
    //Crit_Rate_Increase,
    //Projectile_Number_Add,
    //Item_Absorb_Range_Increase,
    //Luck_Increase,
    //Exp_Increase,
    //Reward_Increase,
    //Damage_Reviced_Reduction_Increase,
    //Revial,
}
public enum ELevelUpEffect
{
    Add_Projectile,
    Reduce_Cooldown,
    Increase_Damage,
    Increase_Projectile_Speed,
    Increase_Effect_Radius,
    Increase_Effect_Duration,
    Increase_Hit_Per_Sec,
    Increase_Number_Of_Effect,
    Increase_Push_Back_Force,
}

public enum TypeChest
{
    Wooden,
    Silver,
    Golden
}
public enum GolobalUpgrade
{
    Hp_Increase,
    Attack_Increase,
    Reward_Increase,
}
public enum EItemInGame
{
    Small_Coin,
    Normal_Coin,
    Big_Coin,
    Magnetic,
    Bomb,
    Heal,
    Exp,
    Boss_Chest,
    EndlessKey,
}


#region Mail Enum

enum MailType
{
    System,
    Update,
    Reward,
}
enum TypeReward
{
    PVP = 1,
    UpdateVersion = 2,
    AdminPush = 3,
}

enum MailStatus
{
    NEW = 0,
    READ = 1,
    COLLECTED = 2,
    DELETED = 3,
}

#endregion