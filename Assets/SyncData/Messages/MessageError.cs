using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MessageError
{
    public string ErrorMessage;
    public Dictionary<string, List<string>> ErrorDetails;
}
