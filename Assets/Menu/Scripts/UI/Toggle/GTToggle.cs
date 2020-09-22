using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/GTToggle", 35), RequireComponent(typeof(RectTransform)), ExecuteInEditMode]
    public class GTToggle : Toggle
    {
        public enum ToggleTransitionState
        {
            None = 0,
            SpriteSwap = 1,
            Animation = 2,
            ColorSwap = 3,
            TextSwap = 4,
        }

        public Graphic SwapGraphic;
        public Sprite UnpressedSprite;
        public Sprite PressedSprite;

        public List<Graphic> graphicsToChange;
        public Color toggleColorOn;
        public Color toggleColorOff;
        public float fadeDuration = .1f;

        public Text TextToChange;
        public string OnText;
        public string OffText;


        [FormerlySerializedAs("animationTriggers"), SerializeField]
        private ToggleAnimationTriggers m_ToggleAnimationTriggers = new ToggleAnimationTriggers();

        [SerializeField]
        private ToggleTransitionState m_ToggleTransitionState = ToggleTransitionState.SpriteSwap;

        private List<UnityAction<bool>> m_onToggleEvents = new List<UnityAction<bool>>();

        public ToggleAnimationTriggers toggleAnimationTriggers
        {
            get
            {
                return m_ToggleAnimationTriggers;
            }
            set
            {
                if (SetPropertyUtility.SetClass(ref m_ToggleAnimationTriggers, value))
                    m_ToggleAnimationTriggers = value;
            }
        }

        public ToggleTransitionState toggleTransitionState
        {
            get
            {
                return m_ToggleTransitionState;
            }
            set
            {
                if (m_ToggleTransitionState != value)
                    m_ToggleTransitionState = value;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            onValueChanged.AddListener(onChanged);
            RefreshToggle();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            onValueChanged.RemoveListener(onChanged);
        }

        public void AddListener(UnityAction<bool> toggleAction)
        {
            onValueChanged.AddListener(toggleAction);
            m_onToggleEvents.Add(toggleAction);
        }

        public void RemoveListener(UnityAction<bool> toggleAction)
        {
            onValueChanged.RemoveListener(toggleAction);
            m_onToggleEvents.Remove(toggleAction);
        }

        public void RemoveAllListeners()
        {
            for (int i = 0; i < m_onToggleEvents.Count; i++)
                m_onToggleEvents.Remove(m_onToggleEvents[i]);
            m_onToggleEvents.Clear();
        }

        private void onChanged(bool isOn)
        {
            RefreshToggle();
        }

        private void RefreshToggle()
        {
            switch (m_ToggleTransitionState)
            {
                case ToggleTransitionState.None:
                    break;
                case ToggleTransitionState.SpriteSwap:
                    if (SwapGraphic != null)
                        (SwapGraphic as Image).sprite = isOn ? PressedSprite : UnpressedSprite;
                    break;
                case ToggleTransitionState.Animation:
                    TriggerToggleAnimation(isOn);
                    break;
                case ToggleTransitionState.ColorSwap:
                    SwapColor(isOn);
                    break;
                case ToggleTransitionState.TextSwap:
                    if (TextToChange != null)
                        TextToChange.text = isOn ? OnText : OffText;
                    break;
            }
        }

        private void TriggerToggleAnimation(bool isPressed)
        {
            if (animator == null || !animator.enabled || !animator.isActiveAndEnabled || animator.runtimeAnimatorController == null)
                return;

            animator.SetBool("IsPressed", isPressed);
        }

        private void SwapColor(bool isOn)
        {
            Color toColor = isOn ? toggleColorOn : toggleColorOff;
            for (int i = 0; i < graphicsToChange.Count; i++)
                graphicsToChange[i].CrossFadeColor(toColor, fadeDuration, false, true);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            RefreshToggle();
        }
#endif
    }

    [Serializable]
    public class ToggleAnimationTriggers
    {
        [SerializeField]
        private const string kDefaultUnpressedAnimName = "Off";

        [SerializeField]
        private const string kDefaultPressedAnimName = "On";


        [FormerlySerializedAs("unpressedTrigger"), FormerlySerializedAs("m_SelectedTrigger"), SerializeField]
        private string m_UnpressedTrigger = kDefaultUnpressedAnimName;

        [FormerlySerializedAs("pressedTrigger"), SerializeField]
        private string m_PressedTrigger = kDefaultPressedAnimName;

        [SerializeField]
        public string unpressedTrigger
        {
            get
            {
                return m_UnpressedTrigger;
            }
            set
            {
                m_UnpressedTrigger = value;
            }
        }

        [SerializeField]
        public string pressedTrigger
        {
            get
            {
                return m_PressedTrigger;
            }
            set
            {
                m_PressedTrigger = value;
            }
        }
    }
}
