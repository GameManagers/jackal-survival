using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.Support;
using WE.Manager;
using WE.Unit;
using DG.Tweening;
namespace WE.SkillEnemy
{
    public class SpidermanSkill : EnemyAttackSkill
    {
        public SpidermanRope spidermanRopePrefabs;
        public float offsetPos = 2;
        public override void ExcuteSkill()
        {
            Owner.mover.movingOnTween = true;
            SpidermanRope rope = Helper.Spawn<SpidermanRope>(spidermanRopePrefabs, transform.position, Quaternion.identity, null);
            Vector3 attractPos = new Vector3(Random.Range(ResolutionManager.Instance.ScreenLeftEdge + offsetPos, ResolutionManager.Instance.ScreenRightEdge - offsetPos), ResolutionManager.Instance.ScreenTopEdge + 2)
                - Player.Instance.transform.position;
            rope.transform.localScale = new Vector3(Vector3.Distance(attractPos, transform.position), 1,  1);
            float startAngle = Helper.Get2DAngle(transform.position - (attractPos + Player.Instance.transform.position));
            rope.transform.localEulerAngles = new Vector3(0, 0, startAngle);
            rope.transform.DOMove(attractPos + Player.Instance.transform.position, 0.2f).OnComplete(()=> {
                Vector3 cachedPos = rope.transform.position - Player.Instance.transform.position;
                rope.transform.DORotate(new Vector3(0, 0, -90), 1f, RotateMode.Fast).OnUpdate(() =>
                {
                    rope.transform.position = cachedPos + Player.Instance.transform.position;
                    Owner.transform.position = rope.childTransform.position;
                }).OnComplete(() => {
                    rope.Despawn();
                    Owner.transform.DOJump(transform.position + (Vector3)Random.insideUnitCircle.normalized * offsetPos, offsetPos, 1, 1);
                    Owner.transform.DORotate(new Vector3(0, 0, 1080), 1, RotateMode.WorldAxisAdd).OnComplete(() =>
                    {
                        Owner.transform.localEulerAngles = Vector3.zero;
                        Owner.mover.movingOnTween = false;
                    });
                });
            });
        }
        protected override void OnInit()
        {
            base.OnInit();
            ExcuteSkill();
        }
    }
}

