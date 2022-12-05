using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageRequest: MessageBase
{

}

[Serializable]
[Message("MOBILE_LOGIN_REQUEST")]
public class MobieLoginRequest: MessageRequest
{
    public string DeviceId;
    public int AppVersion;
    public string OS;
    public string DeviceModel;
    public int Platform;
}


[Serializable]
[Message("SET_NAME")]
public class SetNameRequest : MessageBase
{
    public string DisplayName;
}

[Serializable]

public class PersionModel
{
    public string UserId;
    public string CountryCode;
    public string DisplayName;
    public int UserRole;
    public string CurrentTime;
    public string CreatedAt;
}