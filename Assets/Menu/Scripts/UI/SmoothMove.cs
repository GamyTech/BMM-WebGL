using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class SmoothMove : UIBehaviour
    {
        public float velocity = 0.15f;
        public Vector2 targetPosition;
        private Action finishedAction;
        private RectTransform m_rectTransform;

        public RectTransform rectTransform
        {
            get
            {
                if (m_rectTransform == null)
                    m_rectTransform = GetComponent<RectTransform>();
                return m_rectTransform;
            }
        }

        protected virtual void LateUpdate()
        {
            AutoMove();
        }

        private void AutoMove()
        {
            if (rectTransform.anchoredPosition != targetPosition)
            {
                //Debug.Log(rectTransform.offsetMin.x + " " + targetPosition.x);
                if (!Utils.Approximately(rectTransform.anchoredPosition.x, targetPosition.x, 0.01f) || !Utils.Approximately(rectTransform.anchoredPosition.y, targetPosition.y, 0.01f))
                {
                    float newPositionX = Mathf.SmoothStep(rectTransform.anchoredPosition.x, targetPosition.x, velocity);
                    float newPositionY = Mathf.SmoothStep(rectTransform.anchoredPosition.y, targetPosition.y, velocity);
                    rectTransform.anchoredPosition = new Vector2(newPositionX, newPositionY);
                }
                else
                {
                    rectTransform.anchoredPosition = targetPosition;
                    if (finishedAction != null)
                    {
                        //Debug.Log("finishedAction");
                        finishedAction();
                        finishedAction = null;
                    }
                }
            }
        }

        #region User Functions

        public void SetAction(Action action)
        {
            finishedAction = action;
        }

        public void Move(float X, float Y, Action Endaction = null)
        {
            //Debug.Log(size + " ");
            targetPosition = new Vector2(rectTransform.anchoredPosition.x + X, rectTransform.anchoredPosition.y + Y);
            SetAction(Endaction);
        }

        public void SetPosistion(Vector2 newPos, Action action = null)
        {
            targetPosition = newPos;
            SetAction(action);
        }

        public void SetPosistionX(float X, Action action = null)
        {
            targetPosition = rectTransform.anchoredPosition;
            targetPosition.x = X;
            SetAction(action);
        }

        public void SetPosistionY(float Y, Action action = null)
        {
            targetPosition = rectTransform.anchoredPosition;
            targetPosition.y = Y;
            SetAction(action);
        }

        #endregion User Functions
    }

}
