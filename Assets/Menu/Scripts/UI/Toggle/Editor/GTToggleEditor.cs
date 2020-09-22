using System;
using System.Runtime.CompilerServices;
using UnityEditor.AnimatedValues;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CanEditMultipleObjects, CustomEditor(typeof(GTToggle), true)]
    public class GTToggleEditor : SelectableEditor
    {
        private SerializedProperty m_OnValueChangedProperty;
        private SerializedProperty m_TransitionProperty;
        private SerializedProperty m_GraphicProperty;
        private SerializedProperty m_GroupProperty;
        private SerializedProperty m_ToggleTransitionStateProperty;
        private SerializedProperty m_IsOnProperty;

        private SerializedProperty m_SwapGraphicProperty;
        private SerializedProperty m_UpressedSpriteProperty;
        private SerializedProperty m_PressedSpriteProperty;

        private SerializedProperty m_GraphicsListProperty;
        private SerializedProperty m_PressedColorProperty;
        private SerializedProperty m_UnpressedColorProperty;
        private SerializedProperty m_DurationProperty;

        private SerializedProperty m_TextToChangeProperty;
        private SerializedProperty m_OnTextProperty;
        private SerializedProperty m_OffTextProperty;

        private AnimBool m_ShowIsOnSpriteTrasition = new AnimBool();
        private AnimBool m_ShowIsOnAnimationTransition = new AnimBool();
        private AnimBool m_ShowIsOnColorTransition = new AnimBool();
        private AnimBool m_ShowIsOnTextTransition = new AnimBool();

        protected override void OnEnable()
        {
            base.OnEnable();
            m_TransitionProperty = base.serializedObject.FindProperty("toggleTransition");
            m_GraphicProperty = base.serializedObject.FindProperty("graphic");
            m_GroupProperty = base.serializedObject.FindProperty("m_Group");
            m_IsOnProperty = base.serializedObject.FindProperty("m_IsOn");

            m_SwapGraphicProperty = base.serializedObject.FindProperty("SwapGraphic");
            m_ToggleTransitionStateProperty = base.serializedObject.FindProperty("m_ToggleTransitionState");
            m_UpressedSpriteProperty = base.serializedObject.FindProperty("UnpressedSprite");
            m_PressedSpriteProperty = base.serializedObject.FindProperty("PressedSprite");

            m_GraphicsListProperty = base.serializedObject.FindProperty("graphicsToChange");
            m_PressedColorProperty = base.serializedObject.FindProperty("toggleColorOn");
            m_UnpressedColorProperty = base.serializedObject.FindProperty("toggleColorOff");
            m_DurationProperty = base.serializedObject.FindProperty("fadeDuration");

            m_TextToChangeProperty = base.serializedObject.FindProperty("TextToChange");
            m_OnTextProperty = base.serializedObject.FindProperty("OnText");
            m_OffTextProperty = base.serializedObject.FindProperty("OffText");

            m_OnValueChangedProperty = base.serializedObject.FindProperty("onValueChanged");

            GTToggle.ToggleTransitionState transition = GetTransition(this.m_ToggleTransitionStateProperty);

            m_ShowIsOnSpriteTrasition.value = (transition == GTToggle.ToggleTransitionState.SpriteSwap);
            m_ShowIsOnSpriteTrasition.valueChanged.AddListener(new UnityAction(base.Repaint));

            m_ShowIsOnAnimationTransition.value = (transition == GTToggle.ToggleTransitionState.Animation);
            m_ShowIsOnAnimationTransition.valueChanged.AddListener(new UnityAction(base.Repaint));

            m_ShowIsOnColorTransition.value = (transition == GTToggle.ToggleTransitionState.ColorSwap);
            m_ShowIsOnColorTransition.valueChanged.AddListener(new UnityAction(base.Repaint));

            m_ShowIsOnTextTransition.value = (transition == GTToggle.ToggleTransitionState.TextSwap);
            m_ShowIsOnTextTransition.valueChanged.AddListener(new UnityAction(base.Repaint));
        }

        protected override void OnDisable()
        {
            m_ShowIsOnSpriteTrasition.valueChanged.RemoveListener(new UnityAction(base.Repaint));
            m_ShowIsOnAnimationTransition.valueChanged.RemoveListener(new UnityAction(base.Repaint));
            m_ShowIsOnColorTransition.valueChanged.RemoveListener(new UnityAction(base.Repaint));
            m_ShowIsOnTextTransition.valueChanged.RemoveListener(new UnityAction(base.Repaint));
            base.OnDisable();
        }

        public override void OnInspectorGUI()
        {
            GTToggle.ToggleTransitionState transition = GetTransition(m_ToggleTransitionStateProperty);

            m_ShowIsOnSpriteTrasition.target = (!m_TransitionProperty.hasMultipleDifferentValues && transition == GTToggle.ToggleTransitionState.SpriteSwap);
            m_ShowIsOnAnimationTransition.target = (!m_TransitionProperty.hasMultipleDifferentValues && transition == GTToggle.ToggleTransitionState.Animation);
            m_ShowIsOnColorTransition.target = (!m_TransitionProperty.hasMultipleDifferentValues && transition == GTToggle.ToggleTransitionState.ColorSwap);
            m_ShowIsOnTextTransition.target = (!m_TransitionProperty.hasMultipleDifferentValues && transition == GTToggle.ToggleTransitionState.TextSwap);
            Animator animator = (target as Selectable).GetComponent<Animator>();

            base.OnInspectorGUI();
            EditorGUILayout.Space();
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(m_IsOnProperty, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(m_TransitionProperty, new GUILayoutOption[0]);

            EditorGUILayout.PropertyField(m_GraphicProperty, new GUILayoutOption[0]);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_ToggleTransitionStateProperty, new GUILayoutOption[0]);
            EditorGUI.indentLevel++;
            if (EditorGUILayout.BeginFadeGroup(m_ShowIsOnSpriteTrasition.faded))
            {
                EditorGUILayout.PropertyField(m_SwapGraphicProperty, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(m_UpressedSpriteProperty, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(m_PressedSpriteProperty, new GUILayoutOption[0]);
            }
            EditorGUILayout.EndFadeGroup();

            if (EditorGUILayout.BeginFadeGroup(m_ShowIsOnColorTransition.faded))
            {
                EditorGUILayout.PropertyField(m_GraphicsListProperty, true, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(m_PressedColorProperty, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(m_UnpressedColorProperty, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(m_DurationProperty, new GUILayoutOption[0]);
            }
            EditorGUILayout.EndFadeGroup();

            if (EditorGUILayout.BeginFadeGroup(m_ShowIsOnTextTransition.faded))
            {
                EditorGUILayout.PropertyField(m_TextToChangeProperty, true, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(m_OnTextProperty, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(m_OffTextProperty, new GUILayoutOption[0]);
            }
            EditorGUILayout.EndFadeGroup();

            if (EditorGUILayout.BeginFadeGroup(this.m_ShowIsOnAnimationTransition.faded))
            {
                if (animator == null || animator.runtimeAnimatorController == null)
                {
                    Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
                    controlRect.xMin += EditorGUIUtility.labelWidth;
                    if (GUI.Button(controlRect, "Auto Generate Animation", EditorStyles.miniButton))
                    {
                        AnimatorController animatorController = GenerateSelectableAnimatorContoller((target as Selectable).animationTriggers, target as Selectable);
                        AddToggleAnimatorContoller((target as GTToggle).toggleAnimationTriggers, animatorController); // this.target as GTToggle);
                        if (animatorController != null)
                        {
                            if (animator == null)
                                animator = (target as Selectable).gameObject.AddComponent<Animator>();
                            
                            AnimatorController.SetAnimatorController(animator, animatorController);
                        }
                    }
                }
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.Space();
            EditorGUI.indentLevel--;

            EditorGUILayout.PropertyField(this.m_GroupProperty, new GUILayoutOption[0]);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.m_OnValueChangedProperty, new GUILayoutOption[0]);
            //this.ChildClassPropertiesGUI();
            base.serializedObject.ApplyModifiedProperties();
        }

        private void ChildClassPropertiesGUI()
        {
            if (this.IsDerivedSelectableEditor())
                return;
            
            //Editor.DrawPropertiesExcluding(base.serializedObject, this.m_PropertyPathToExcludeForChildClasses);
        }

        private bool IsDerivedSelectableEditor()
        {
            return base.GetType() != typeof(SelectableEditor);
        }

        private static GTToggle.ToggleTransitionState GetTransition(SerializedProperty transition)
        {
            return (GTToggle.ToggleTransitionState)transition.enumValueIndex;
        }

        private static AnimatorController GenerateSelectableAnimatorContoller(AnimationTriggers animationTriggers, Selectable target)
        {
            if (target == null)
                return null;
            
            string saveControllerPath = GetSaveControllerPath(target);
            if (string.IsNullOrEmpty(saveControllerPath))
                return null;
            
            string name = (!string.IsNullOrEmpty(animationTriggers.normalTrigger)) ? animationTriggers.normalTrigger : "Normal";
            string name2 = (!string.IsNullOrEmpty(animationTriggers.highlightedTrigger)) ? animationTriggers.highlightedTrigger : "Highlighted";
            string name3 = (!string.IsNullOrEmpty(animationTriggers.pressedTrigger)) ? animationTriggers.pressedTrigger : "Pressed";
            string name4 = (!string.IsNullOrEmpty(animationTriggers.disabledTrigger)) ? animationTriggers.disabledTrigger : "Disabled";
            AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(saveControllerPath);
            GenerateTriggerableTransition(name, animatorController);
            GenerateTriggerableTransition(name2, animatorController);
            GenerateTriggerableTransition(name3, animatorController);
            GenerateTriggerableTransition(name4, animatorController);
            AssetDatabase.ImportAsset(saveControllerPath);
            return animatorController;
        }

        private static AnimationClip GenerateTriggerableTransition(string name, AnimatorController controller)
        {
            AnimationClip animationClip = AnimatorController.AllocateAnimatorClip(name);
            AssetDatabase.AddObjectToAsset(animationClip, controller);
            AnimatorState destinationState = controller.AddMotion(animationClip);
            controller.AddParameter(name, AnimatorControllerParameterType.Trigger);
            AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
            AnimatorStateTransition animatorStateTransition = stateMachine.AddAnyStateTransition(destinationState);
            animatorStateTransition.AddCondition(AnimatorConditionMode.If, 0f, name);
            return animationClip;
        }

        private static void AddToggleAnimatorContoller(ToggleAnimationTriggers animationTriggers, AnimatorController animatorController)
        {
            string name1 = (!string.IsNullOrEmpty(animationTriggers.unpressedTrigger)) ? animationTriggers.unpressedTrigger : "Off";
            string name2 = (!string.IsNullOrEmpty(animationTriggers.pressedTrigger)) ? animationTriggers.pressedTrigger : "On";
            string param = "IsPressed";
            animatorController.AddParameter(param, AnimatorControllerParameterType.Bool);
            GenerateBooleanTransition(name1, name2, param, animatorController);
        }

        private static string GetSaveControllerPath(Selectable target)
        {
            string name = target.gameObject.name;
            string message = string.Format("Create a new animator for the game object '{0}':", name);
            return EditorUtility.SaveFilePanelInProject("New Animation Contoller", name + "Controller", "controller", message);
        }

        private static void GenerateBooleanTransition(string name1, string name2, string paramName, AnimatorController controller)
        {
            AnimationClip animationClip1 = AnimatorController.AllocateAnimatorClip(name1);
            AnimationClip animationClip2 = AnimatorController.AllocateAnimatorClip(name2);
            AssetDatabase.AddObjectToAsset(animationClip1, controller);
            AssetDatabase.AddObjectToAsset(animationClip2, controller);

            AnimatorControllerLayer animatorControllerLayer = new AnimatorControllerLayer();
            animatorControllerLayer.name = "ToggleLayer";
            animatorControllerLayer.stateMachine = new AnimatorStateMachine();
            animatorControllerLayer.stateMachine.name = animatorControllerLayer.name;
            animatorControllerLayer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
            animatorControllerLayer.defaultWeight = 1;
            if (AssetDatabase.GetAssetPath(controller) != string.Empty)
            {
                AssetDatabase.AddObjectToAsset(animatorControllerLayer.stateMachine, AssetDatabase.GetAssetPath(controller));
            }
            controller.AddLayer(animatorControllerLayer);
            
            AnimatorState state1 = controller.AddMotion(animationClip1, 1);
            AnimatorState state2 = controller.AddMotion(animationClip2, 1);

            // Transitions
            AnimatorStateTransition transition1 = state1.AddTransition(state2);
            AnimatorStateTransition transition2 = state2.AddTransition(state1);

            transition1.AddCondition(AnimatorConditionMode.If, 0f, paramName);
            transition2.AddCondition(AnimatorConditionMode.IfNot, 0f, paramName);

            
        }
    }
}
