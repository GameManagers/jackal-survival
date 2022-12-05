using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using WE.Pooling;
using DG.Tweening;
using UnityEngine.UI;

public class UITextPopup : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image bg;
    private void Start()
    {
        //text.gameObject.SetActive(false);
        //bg.gameObject.SetActive(false);
    }
    public void Show(float t = 1.5f)
    {
        text.gameObject.SetActive(true);
        bg.gameObject.SetActive(true);
        bg.DOKill();
        text.DOKill();
        bg.DOFade(0.5f, 0.1f);
        text.DOFade(1f, 0.1f).OnComplete(()=> {
            Fade(t);
        });
    }
    public void Fade(float t)
    {
        bg.DOKill();
        text.DOKill();
        text.DOFade(0f, t);
        bg.DOFade(0f, t).OnComplete(()=> {
            text.gameObject.SetActive(false);
            bg.gameObject.SetActive(false);
        });
    }

}
