using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.UI;
using WE.Support;
using UnityEngine.UI;
using WE.Manager;
using WE.Pooling;

public class CommonButton : MonoBehaviour
{
    Button currentButton;
    private void OnValidate()
    {
        currentButton = GetComponent<Button>();
    }
    private void OnEnable()
    { 
        OnValidate();
        currentButton.onClick.AddListener(OnClicked);
    }
    private void OnDisable()
    {

        currentButton.onClick.RemoveListener(OnClicked);
    }
    public void OnClicked()
    {
        SoundManager.Instance.PlaySoundFx(SoundManager.Instance.buttonSfx);
    }
}
