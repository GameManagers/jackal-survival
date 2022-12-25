using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WE.UI
{
    public class UIAvatar : UIBase
    {
        public override void InitUI()
        {

        }

        [SerializeField] private Text txtName;
        [SerializeField] private Text txtId;
        [SerializeField] private Button btnRename;
    }
}