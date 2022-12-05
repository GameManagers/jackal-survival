using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class KhangEnemy : MonoBehaviour
{
    public int id;
    public delegate void DesTroyObj(int idObj, KhangEnemy Obj);
    public DesTroyObj OnDesTroyObj;

    public System.Action<KhangEnemy> OnEnemyDie;
    private void Start()
    {
        if (InstatntiateOBJ.Instance == null)
        {
            Debug.Log("Vao day");
        }
    }
    [Button("Kill Enemy")]
    public void KillEnemy()
    {
        EnemyDie();
    }
    public void EnemyDie()
    {
        OnEnemyDie?.Invoke(this);
        gameObject.SetActive(false);
    }
}
