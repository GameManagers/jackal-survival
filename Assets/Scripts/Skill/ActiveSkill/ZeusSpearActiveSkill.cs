using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Bullet;
using WE.Support;
using Sirenix.OdinInspector;
namespace WE.Skill
{
    public class ZeusSpearActiveSkill : ActiveSkill
    {
        public ZeusSpearChainLightning chainPrefabs;
        public List<ZeusSpearBullet> activeZeusSpear;
        public List<ZeusSpearChainLightning> activeChain;
        public override void Init()
        {
            base.Init();
            activeZeusSpear = new List<ZeusSpearBullet>();
            activeChain = new List<ZeusSpearChainLightning>();
        }

        protected override void ExcuteSkill()
        {
            for (int i = 0; i < BaseBulletNumb; i++)
            {
                Vector3 tarPos = transform.position 
                    + (Vector3)Random.insideUnitCircle * Maxdistance;
                ZeusSpearBullet bullet = Helper.SpawnBullet<ZeusSpearBullet>(bulletPrefabs, transform.position, Quaternion.identity, null);
                bullet.targetPos = tarPos;
                bullet.parentSkill = this;
                bullet.IsEvole = IsEvoleSkill;
                InitBullet(bullet);
            }
        }
        public void StopSpear(ZeusSpearBullet spear)
        {
            if (activeZeusSpear.Contains(spear))
            {
                activeZeusSpear.Remove(spear);
                SetChain();
            }
        }
        public void SetChain()
        {
            for (int i = 0; i < activeChain.Count; i++)
            {
                activeChain[i].Despawn();
            }
            activeChain.Clear();
            for (int i = 0; i < activeZeusSpear.Count; i++)
            {
                activeZeusSpear[i].listChain.Clear();
            }
            if (activeZeusSpear.Count > 1)
            {
                for (int i = 0; i < activeZeusSpear.Count; i++)
                {
                    int newChain = 0;
                    if (BaseHitEffect > activeZeusSpear.Count)
                        newChain = activeZeusSpear.Count - activeZeusSpear[i].listChain.Count;
                    else
                        newChain = BaseHitEffect - activeZeusSpear[i].listChain.Count;
                    for (int j = 0; j < newChain; j++)
                    {
                        int id = Random.Range(0, activeZeusSpear.Count);
                        int t = 0;
                        while (id == i && activeZeusSpear[id].listChain.Count >= BaseHitEffect)
                        {
                            id = Random.Range(0, activeZeusSpear.Count);
                            t++;
                            if(t > 100)
                            {
                                DebugCustom.LogError("Null Here, Check Logic");
                            }
                        }
                        ZeusSpearChainLightning chain = Helper.SpawnBullet<ZeusSpearChainLightning>(chainPrefabs, activeZeusSpear[i].transform.position, Quaternion.identity, null);
                        Vector3 dir = activeZeusSpear[id].transform.position - activeZeusSpear[i].transform.position;
                        float angle = Helper.Get2DAngle(dir);
                        chain.transform.localEulerAngles = new Vector3(0, 0, angle);
                        chain.distance = Vector3.Distance(activeZeusSpear[i].transform.position, activeZeusSpear[id].transform.position);
                        InitBullet(chain);
                        activeZeusSpear[i].listChain.Add(chain);
                        activeZeusSpear[id].listChain.Add(chain);
                        activeChain.Add(chain);
                    }
                }
            }
        }
    }
}

