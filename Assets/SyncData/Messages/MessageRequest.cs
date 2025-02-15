using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
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
public class SetNameRequest : MessageRequest
{
    public string DisplayName;
}

[Serializable]
[Message("SET_AVATAR")]
public class SetAvatarRequest : MessageRequest
{
    public TypeAvatar Avatar;
}

[Serializable]

public class PersionModel
{
    public TypeAvatar Avatar;
    public string UserId;
    public string CountryCode;
    public string DisplayName;
    public int UserRole;
    public string CurrentTime;
    public string CreatedAt;
}