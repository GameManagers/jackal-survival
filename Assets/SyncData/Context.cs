using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class Context 
{
    public static PersionModel profile;

    public static PersionModel CurrentUserPlayfabProfile
    {
        get
        {
            if (profile == null) return new PersionModel();
            return profile;
        }
        set
        {
            profile = value;
        }
    }
}
