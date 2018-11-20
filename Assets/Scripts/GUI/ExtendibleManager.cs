using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExtendibleManager : MonoBehaviour {

    public Extendible[] extendiblePanels;
    public float percentPerSecond = 1.0f;
    public bool alongX = false;

    void Update()
    {
        float offset = 0;

        //NOTE All panels need to be listed in reverse order of transparency
        foreach(Extendible ex in extendiblePanels)
        {
            //Precheck, no panel if Button disbaled
            ex.panelActive = (!ex.buttonActive) ? false : ex.panelActive;

            //Start with Button
            RectTransform buttonTrans = (RectTransform)ex.button.transform;
            Rect buttonSize = buttonTrans.rect;
            Vector3 buttonPos = buttonTrans.localPosition;

            float buttonMovePercent = ex.buttonPercent;
            if (ex.buttonActive && ex.buttonPercent != 1.0f)
            {
                buttonMovePercent = Mathf.Min(ex.buttonPercent + (percentPerSecond * Time.deltaTime), 1.0f);
            }
            else if (!ex.buttonActive && ex.buttonPercent != 0.0f)
            {
                buttonMovePercent = Mathf.Max(ex.buttonPercent - (percentPerSecond * Time.deltaTime), 0.0f);
            }

            buttonTrans.localPosition = new Vector3(((alongX) ? offset + (buttonSize.width * buttonMovePercent * -1) : buttonPos.x), ((!alongX) ? offset + (buttonSize.height * buttonMovePercent * -1) + (buttonSize.height / 2) : buttonPos.y), 0);
            Image buttonImage = ex.button.GetComponent<Image>();
            if (buttonImage != null)
            {
                Color buttonColor = buttonImage.color;
                buttonColor.a = buttonMovePercent * 2;
                buttonImage.color = buttonColor;
            }
            ex.buttonPercent = buttonMovePercent;

            Rect buttonRect = ((RectTransform)ex.button.transform).rect;
            offset -= ((alongX) ? buttonRect.width : buttonRect.height) * ex.buttonPercent;

            //Then move Panel
            RectTransform panelTrans = (RectTransform)ex.panel.transform;
            Rect panelSize = panelTrans.rect;
            Vector3 panelPos = panelTrans.localPosition;

            float panelMovePercent = ex.panelPercent;
            if (ex.panelActive && ex.panelPercent != 1.0f)
            {
                panelMovePercent = Mathf.Min(ex.panelPercent + (percentPerSecond * Time.deltaTime), 1.0f);
            }
            else if (!ex.panelActive && ex.panelPercent != 0.0f)
            {
                panelMovePercent = Mathf.Max(ex.panelPercent - (percentPerSecond * Time.deltaTime), 0.0f);
            }

            panelTrans.localPosition = new Vector3(((alongX) ? offset + (panelSize.width * panelMovePercent * -1) : panelPos.x), ((!alongX) ? offset + (panelSize.height * panelMovePercent * -1) + (panelSize.height / 2) : panelPos.y), 0);
            Image panelImage = ex.panel.GetComponent<Image>();
            if(panelImage != null)
            {
                Color panelColor = panelImage.color;
                panelColor.a = panelMovePercent * 2;
                panelImage.color = panelColor;
            }
            ex.panelPercent = panelMovePercent;

            Rect panelRect = ((RectTransform)ex.panel.transform).rect;
            offset -= ((alongX) ? panelRect.width : panelRect.height) * ex.panelPercent;
        }
    }
}
