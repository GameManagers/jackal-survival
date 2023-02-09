using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Message("READY_PVP")]
public class ReadyPVPMessage : MessageBase
{
    public string SessionId;
    public string DisplayName;
    public TypeAvatar AvatarUrl;
    public string UserId;
    public int Status;
    public float Atk;
    public float Level;
    public string TankId;
    
    public int Elo;
    public int CountMatching;
}
[Message("START_GAME")]
public class StartGamePVPMessage : MessageBase
{
    public int Time = 0;
    public string[] Players;
}

[Message("PREPARE_PVP")]
public class PreparePVPMessage : MessageBase
{
    public int Level;
    public int Time;
}

[Message("END_GAME")]
public class EndGameMessage : MessageBase
{
    public EndGameData LoserData;
    public EndGameData WinnerData;
    public bool IsDraw;
    public EWinTypePVP WinType;

}

[Message("NEED_GAME_START")]
public class NeedGameStartPVPMessage : MessageBase
{
    public int NeedSendStart;
}

[Message("GAME_SCORE_UPDATE")]
public class GetScorePVPMessage : MessageBase
{
    public Dictionary<string, GameScoreData> GameScores;
}

[Message("SEND_GAME_SCORE")]
public class SendScorePVPMessage : MessageBase
{
    public float Score;
    public float CurHP;
    public float MaxHP;
}


[Message("PLAYER_DIE")]
public class PlayerDieMessage : MessageBase
{
}

[Message("PLAYER_LEFT")]
public class PlayerLeftMessage : MessageBase
{
    public string SessionID;
    public int GameState;
}


[Message("GET_TOPSCORE_PVP")]
public class LeaderBoardPVP : MessageRequest
{
    public string LeaderBoardName = "PVP";
}
//-------------------------------------------DATA----------------------------------------------

[Serializable]
public class EndGameData
{
    public string DisplayName;
    public TypeAvatar AvatarUrl;
    public string UserId;
    public string SessionId;
    public int Score;
    public int eloPre;
    public int eloCur;
    public Dictionary<string, int> Rewards;
}

[Serializable]
public class ErrorMatching
{
    public int ErrorCode;
    public string Message;
}

[Serializable]
public class GameScoreData
{
    public string RocketId = "";
    public string SessionId = "";

    public int Score;
    public float CurHP;
    public float MaxHP;
}



[Serializable]
public class DataPVPRanking
{
    public List<InfoPVPRanking> TopUser;
    public InfoMyPVPRanking YourScore;
    public int Season;
    public long EndTime;
    public long CurrentTime;
}

[Serializable]
public class UserPVPRanking
{
    //public string UserId;
    public string DisplayName;
    public TypeAvatar AvatarUrl;
}

[Serializable]
public class InfoPVPRanking
{
    public int RankNumber;
    public string UserId;
    public int Score;
    public UserPVPRanking PlayerData;
}

[Serializable]
public class InfoMyPVPRanking
{
    public int Score;
    public int RankNumber;
}

[Serializable]
public class MatchMakingPVPResponse
{
    /// <summary>
    /// 0:JoinOrCreate, 1: Join, 2: Create
    /// </summary>
    public int type;
    public string roomName;
    public int status;
}