using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTargeting : MonoBehaviour
{
    public KhangEnemy Owner;
    public KhangEnemy currentTarget;
    bool newTarget;
    private void Start()
    {
        NewTarget();
    }
    
    public void NewTarget()
    {
        newTarget = true;
    }
    private void Update()
    {
        if (newTarget)
        {
            currentTarget = InstatntiateOBJ.Instance.GetEnemy(Owner.id);
            currentTarget.OnEnemyDie += OnTargetDie;
            newTarget = false;
        }
    }
    public void OnTargetDie(KhangEnemy e)
    {
        //Get new Target
        e.OnEnemyDie -= OnTargetDie;
        NewTarget();
    }
}
