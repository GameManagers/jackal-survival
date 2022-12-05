using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace WE.UI
{
    public class IconStar : MonoBehaviour
    {
        public Image IconShow;

        public Animator anim;

        public void StarBlink()
        {
            anim.Play("StarBlink");
        }

        public void InitIcon(bool show)
        {
            IconShow.gameObject.SetActive(show);
        }
    }
}

