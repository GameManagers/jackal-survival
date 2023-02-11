using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WE.Unit
{
    public class HpBarPlayer : MonoBehaviour
    {
        public SpriteRenderer bar;

        public Sprite fullHpSprite;
        public Sprite lowHpSprite;
        public void Init()
        {
            bar.transform.localScale = Vector3.one;
            bar.sprite = fullHpSprite;
            Player.Instance.OnHpChange += OnHpChange;
            gameObject.SetActive(true);
        }
        public void Despawn()
        {

            Player.Instance.OnHpChange -= OnHpChange;
            gameObject.SetActive(false);
        }
        public void OnHpChange()
        {
            float val = Player.Instance.CurrentHp / Player.Instance.MaxHp;
            if (val < 0.3f)
                bar.sprite = lowHpSprite;
            else
                bar.sprite = fullHpSprite;
            bar.transform.localScale = new Vector3(val, 1, 1);
        }
    }
}

