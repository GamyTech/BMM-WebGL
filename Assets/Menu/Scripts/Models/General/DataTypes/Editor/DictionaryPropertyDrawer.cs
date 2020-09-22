using UnityEngine;
using UnityEditor;


namespace GT.Collections
{
    [CustomPropertyDrawer(typeof(DrawableDictionary), true)]
    public class DictionaryPropertyDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                var keysProp = property.FindPropertyRelative("_keys");
                return (keysProp.arraySize + 4) * EditorGUIUtility.singleLineHeight;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool expanded = property.isExpanded;
            var r = GetNextRect(ref position);
            property.isExpanded = EditorGUI.Foldout(r, property.isExpanded, label);

            if (expanded)
            {
                EditorGUI.indentLevel++;

                var keysProp = property.FindPropertyRelative("_keys");
                var valuesProp = property.FindPropertyRelative("_values");

                int cnt = keysProp.arraySize;
                if (valuesProp.arraySize != cnt) valuesProp.arraySize = cnt;

                for (int i = 0; i < cnt; i++)
                {
                    r = GetNextRect(ref position);
                    r = EditorGUI.IndentedRect(r);
                    var w = r.width / 2f;
                    var r0 = new Rect(r.xMin, r.yMin, w, r.height);
                    var r1 = new Rect(r0.xMax, r.yMin, w, r.height);

                    var keyProp = keysProp.GetArrayElementAtIndex(i);
                    var valueProp = valuesProp.GetArrayElementAtIndex(i);
                    EditorGUI.PropertyField(r0, keyProp, GUIContent.none, false);
                    EditorGUI.PropertyField(r1, valueProp, GUIContent.none, false);
                }

                r = GetNextRect(ref position);
                var pRect = new Rect(r.xMax - 60f, r.yMin, 30f, EditorGUIUtility.singleLineHeight);
                var mRect = new Rect(r.xMax - 30f, r.yMin, 30f, EditorGUIUtility.singleLineHeight);

                if (GUI.Button(pRect, "+"))
                {
                    keysProp.arraySize++;
                    SetPropertyValue(keysProp.GetArrayElementAtIndex(keysProp.arraySize - 1), null);
                    valuesProp.arraySize = keysProp.arraySize;
                    SetPropertyValue(valuesProp.GetArrayElementAtIndex(valuesProp.arraySize - 1), null);
                }
                if (GUI.Button(mRect, "-"))
                {
                    keysProp.arraySize = Mathf.Max(keysProp.arraySize - 1, 0);
                    valuesProp.arraySize = keysProp.arraySize;
                }
            }
        }


        private Rect GetNextRect(ref Rect position)
        {
            var r = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
            var h = EditorGUIUtility.singleLineHeight + 1f;
            position = new Rect(position.xMin, position.yMin + h, position.width, position.height = h);
            return r;
        }

        public static void SetPropertyValue(SerializedProperty prop, object value)
        {
            if (prop == null) throw new System.ArgumentNullException("prop");

            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    prop.intValue = value == null ? 0 : value.ParseInt();
                    break;
                case SerializedPropertyType.Boolean:
                    prop.boolValue = value == null ? false : value.ParseBool();
                    break;
                case SerializedPropertyType.Float:
                    prop.floatValue = value == null ? 0 : value.ParseFloat();
                    break;
                case SerializedPropertyType.String:
                    prop.stringValue = value == null ? "" : value.ToString();
                    break;
                case SerializedPropertyType.Color:
                    prop.colorValue = (Color)value;
                    break;
                case SerializedPropertyType.ObjectReference:
                    prop.objectReferenceValue = value as Object;
                    break;
                case SerializedPropertyType.LayerMask:
                    prop.intValue = (value is LayerMask) ? ((LayerMask)value).value : value.ParseInt();
                    break;
                case SerializedPropertyType.Enum:
                    prop.enumValueIndex = value == null ? 0 : value.ParseInt();
                    break;
                case SerializedPropertyType.Vector2:
                    prop.vector2Value = (Vector2)value;
                    break;
                case SerializedPropertyType.Vector3:
                    prop.vector3Value = (Vector3)value;
                    break;
                case SerializedPropertyType.Vector4:
                    prop.vector4Value = (Vector4)value;
                    break;
                case SerializedPropertyType.Rect:
                    prop.rectValue = (Rect)value;
                    break;
                case SerializedPropertyType.ArraySize:
                    prop.arraySize = value == null ? 0 : value.ParseInt();
                    break;
                case SerializedPropertyType.Character:
                    prop.intValue = value == null ? 0 : value.ParseInt();
                    break;
                case SerializedPropertyType.AnimationCurve:
                    prop.animationCurveValue = value as AnimationCurve;
                    break;
                case SerializedPropertyType.Bounds:
                    prop.boundsValue = (Bounds)value;
                    break;
                case SerializedPropertyType.Gradient:
                    throw new System.InvalidOperationException("Can not handle Gradient types.");
            }
        }

    }
}
