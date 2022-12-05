using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstatntiateOBJ : MonoBehaviour
{
    public KhangEnemy PrefabsObj;
    private static InstatntiateOBJ _instance;
    public List<KhangEnemy> listEnemyA;
    public List<KhangEnemy> listEnemyB;

    public System.Action OnKillCount;

    public static InstatntiateOBJ Instance => _instance;

    private void Awake()
    {
        _instance = this;
    }
    private void OnDisable()
    {
        if (_instance != null)
        {
            _instance = null;
        }
    }
    private void Start()
    {
        //foreach (KhangEnemy obj in ListObj)
        //{
        //    obj.OnDesTroyObj = DeadState;
        //}
        foreach (var item in listEnemyA)
        {
            item.OnEnemyDie += OnEnemyDie;
        }
        foreach (var item in listEnemyB)
        {
            item.OnEnemyDie += OnEnemyDie;
        }
    }
    public  KhangEnemy GetEnemy(int id) 
    {
        if (id == 2)
        {
            return listEnemyA[0];
        }
        else
            return listEnemyB[0];
    }
    private void InitObj()
    {
        for (int i = 0; i < 10; ++i)
        {
            Vector3 randomPos = new Vector3(Random.Range(0, 10f), Random.Range(0, 10f), 0f);
            KhangEnemy g = Instantiate(PrefabsObj, randomPos, Quaternion.identity);
            g.id = i + 1;
            //ListObj.Add(g);
        }
    }
    public void DeadState(int idObj, KhangEnemy Obj)
    {
        if (Obj != null)
        {
            if (Obj.id == idObj)
            {
                Destroy(Obj);
                NewTarget();
            }
        }
    }
    public void OnEnemyDie(KhangEnemy e)
    {
        e.OnEnemyDie -= OnEnemyDie;
        if (listEnemyA.Contains(e))
        {
            listEnemyA.Remove(e);
        }else if (listEnemyB.Contains(e))
        {
            listEnemyB.Remove(e);
        }
    }
    public void NewTarget()
    {
        KhangEnemy e = new KhangEnemy();
    }

}
