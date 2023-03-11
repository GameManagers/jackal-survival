using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WE.UI;
using WE.Unit;
using static MailController;

public class UIPopupReward: MonoBehaviour
{
    [SerializeField] private UIResourceItem _prefabReward;

    private int Coins = 0;

    public void Show(List<RewardMail> gifts)
    {
        for (int i = 0; i < gifts.Count; i++)
        {
            Coins += gifts[i].value;
        }
        _prefabReward.SetValue(Coins);
        gameObject.SetActive(true);
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
        Player.Instance.AddCoin(Coins); 
        Coins = 0;
    }
}
