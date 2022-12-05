using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using WE.Effect;
using WE.Support;
using WE.Manager;

namespace WE.UI
{
    public class UIChest : MonoBehaviour
    {
        public TypeChest chestType;
        [SerializeField] SkeletonGraphic skelGraphic;
        [SerializeField, SpineEvent(dataField = "skelGraphic")] string eventOpen;

        [SerializeField, SpineAnimation(dataField = "skelGraphic")] string animChestDrop;
        [SerializeField, SpineAnimation(dataField = "skelGraphic")] string animChestIdle;
        [SerializeField, SpineAnimation(dataField = "skelGraphic")] string animChestOpen;
        [SerializeField, SpineAnimation(dataField = "skelGraphic")] string animChestOpenStand;
        [SerializeField, SpineSkin(dataField = "skelGraphic")] string skinWooden;
        [SerializeField, SpineSkin(dataField = "skelGraphic")] string skinSilver;
        [SerializeField, SpineSkin(dataField = "skelGraphic")] string skinGolden;

        public GameObject glowChest;
        public Transform spawnTransform;
        public AnimationEffect fxOpen;
        public Transform fxPos;
        public System.Action OnOpenChest;
        private void Start()
        {
            string skin = string.Empty;
            switch (chestType)
            {
                case TypeChest.Wooden:
                    skin = skinWooden;
                    break;
                case TypeChest.Silver:
                    skin = skinSilver;
                    break;
                case TypeChest.Golden:
                    skin = skinGolden;
                    break;
                default:
                    break;
            }

            skelGraphic.Skeleton.SetSkin(skin);


            if (glowChest != null)
                glowChest.gameObject.SetActive(true);
            //skelGraphic.skeletonDataAsset.Clear();
            skelGraphic.AnimationState.Event += AnimationState_Event;
            var track = skelGraphic.AnimationState;
            track.ClearTracks();
            //track.SetAnimation(2, animChestIdle, true);
            track.SetAnimation(3, animChestDrop, false);
            track.AddAnimation(1, animChestIdle, true, 1);
            track.Apply(skelGraphic.Skeleton);
        }
        private void OnDisable()
        {
            skelGraphic.AnimationState.Event -= AnimationState_Event;
        }
        public void OnClickedOpenChest()
        {
            var track = skelGraphic.AnimationState;
            track.ClearTracks();
            track.SetAnimation(3, animChestOpen, false);
            track.AddAnimation(2, animChestOpenStand, true, 1);
            track.Apply(skelGraphic.Skeleton);
        }
        public void AnimationState_Event(Spine.TrackEntry trackEntry, Spine.Event e)
        {
            if (e.Data.Name == eventOpen)
            {
                Open();
            }
        }
        void Open()
        {
            SoundManager.Instance.PlaySoundFx(SoundManager.Instance.openChestSfx);
            Helper.SpawnEffect(fxOpen, fxPos.position, null);
            if (glowChest != null)
                glowChest.gameObject.SetActive(false);
            
            OnOpenChest?.Invoke();
        }
    }
}

