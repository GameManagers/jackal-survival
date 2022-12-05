using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using WE.Manager;

namespace WE.UI
{
    public class PopupTut : UIBase
    {
        public TextMeshProUGUI textTutorial;
        public GameObject handPoint;
        public RectTransform rectTut;
        public GameObject blackBg;
        public RectTransform childRect;
        public GameObject testOBj;
        public Image Black;
        public Animator animZoom;
        Button currentButton;
        bool hideOnClick;
        public System.Action OnClicked;
        public override void InitUI()
        {
        }
        public override void AfterHideAction()
        {
            base.AfterHideAction();
            childRect.gameObject.transform.SetParent(this.transform);
            childRect.transform.position = Vector3.zero;
            if (currentButton != null)
            {
                currentButton.onClick?.Invoke();
                Canvas cv = currentButton.gameObject.GetComponent<Canvas>();
                if (cv != null)
                {
                    Destroy(cv);
                }
                currentButton = null;
            }
            if (GameplayManager.Instance.State != GameState.Pause)
            {
                Time.timeScale = 1;
            }
        }
        public void InitTut(string textTut, bool _showBlack,bool _overrideAnchor,Transform _targetAnchor,Vector2 anchorPosition, Button interActiveButton = null, bool activeHand = false, bool _hideOnClick = false)
        {
            textTutorial.text = textTut;
            handPoint.SetActive(activeHand);
            if (_showBlack)
            {
                Black.color = new Color(1, 1, 1, 0.9f);
            }
            else
            {
                Black.color = new Color(1, 1, 1, 0);
            }
            if (_overrideAnchor)
            {
                if (_targetAnchor != null)
                {
                    SetHighlight(_targetAnchor);
                }
                else
                {
                    rectTut.anchoredPosition = anchorPosition;
                }
            }
            currentButton = interActiveButton;
            if (interActiveButton != null)
            {
                Canvas cv = interActiveButton.gameObject.AddComponent<Canvas>();
                cv.overrideSorting = true;
                cv.sortingLayerName = "UI";
                cv.sortingOrder = 2;
            }
            if (GameplayManager.Instance.State != GameState.Pause)
            {
                Time.timeScale = 0;
            }
            hideOnClick = _hideOnClick;
            animZoom.Play(openAnim);
        }
        public void SetHighlight(Transform target)
        {
            childRect.gameObject.transform.SetParent(target);
            childRect.transform.localPosition = Vector3.zero;
            rectTut.transform.position = childRect.transform.position;
        }
        public void OnClick()
        {
            if (hideOnClick)
            {
                Hide();
            }
            OnClicked?.Invoke();
        }
        [Button("Test")]
        public void Test()
        {
            //blackBg.transform.position = new Vector3(testOBj.transform.position.x, testOBj.transform.position.y, blackBg.transform.position.z);
        }
    }
}

