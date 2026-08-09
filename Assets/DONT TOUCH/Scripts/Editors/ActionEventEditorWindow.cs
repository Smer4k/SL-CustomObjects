using System;
using System.Collections.Generic;
using System.Globalization;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.BlockComponents;
using DONT_TOUCH.Scripts.BlockSerialization;
using DONT_TOUCH.Scripts.Extensions;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using AnimatorControllerLayer = UnityEditor.Animations.AnimatorControllerLayer;
using AnimatorParameter = UnityEngine.AnimatorControllerParameter;
using AnimatorParameterType = UnityEngine.AnimatorControllerParameterType;

namespace DONT_TOUCH.Scripts.Editors
{
    public class ActionEventEditorWindow : EditorWindow
    {
        private const float RowSpacing = 2f;
        private const float InnerPadding = 2f;
        private const float HelpBoxHeight = 34f;
        private const string EventListsPropertyName = "ActionEvents";

        private MonoBehaviour _target;
        private SerializedObject _serializedTarget;
        private SerializedProperty _eventListsProperty;
        private ReorderableList _actionsList;
        private Vector2 _scrollPosition;
        private int _selectedEventIndex;
        private string _windowTitle = "Action Events";

        public static void Open(MonoBehaviour target, string windowTitle = "Action Events")
        {
            if (target == null)
                return;

            ActionEventEditorWindow window = GetWindow<ActionEventEditorWindow>(windowTitle);
            window.minSize = new Vector2(560f, 380f);
            window.Bind(target, windowTitle);
            window.Show();
        }

        public static bool DrawOpenButton(
            MonoBehaviour target,
            string buttonText = "Open Actions Editor",
            string windowTitle = "Action Events",
            params GUILayoutOption[] options)
        {
            EditorGUI.BeginDisabledGroup(target == null);
            bool clicked = GUILayout.Button(buttonText, options);
            EditorGUI.EndDisabledGroup();

            if (!clicked || target == null)
                return false;

            Open(target, windowTitle);
            return true;
        }

        private void OnEnable()
        {
            if (_target != null)
                return;

            TryBindFromSelection();
        }

        private void Bind(MonoBehaviour target, string windowTitle)
        {
            _target = target;
            _windowTitle = string.IsNullOrWhiteSpace(windowTitle) ? "Action Events" : windowTitle;
            titleContent = new GUIContent(_windowTitle);

            _serializedTarget = _target != null ? new SerializedObject(_target) : null;
            _eventListsProperty = _serializedTarget?.FindProperty(EventListsPropertyName);

            RefreshActionsList();
        }

        private void RefreshActionsList()
        {
            _actionsList = null;

            if (_eventListsProperty == null || _eventListsProperty.arraySize == 0)
                return;

            _selectedEventIndex = Mathf.Clamp(_selectedEventIndex, 0, _eventListsProperty.arraySize - 1);

            SerializedProperty eventElement = _eventListsProperty.GetArrayElementAtIndex(_selectedEventIndex);
            SerializedProperty actionsProperty = eventElement.FindPropertyRelative(nameof(ActionEventList.Actions));

            if (actionsProperty == null)
            {
                _actionsList = null;
                return;
            }

            _actionsList = new ReorderableList(_serializedTarget, actionsProperty, true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    string eventName = GetEventDisplayName(_selectedEventIndex);
                    EditorGUI.LabelField(rect, $"Actions: {eventName}");
                },
                drawElementCallback = (rect, index, isActive, isFocused) =>
                    DrawActionElement(actionsProperty, rect, index, isActive, isFocused),
                elementHeightCallback = index => GetActionElementHeight(actionsProperty, index),
                onAddCallback = _ => OnAddAction(actionsProperty),
            };
        }

        private string GetEventDisplayName(int index)
        {
            if (_eventListsProperty == null || index < 0 || index >= _eventListsProperty.arraySize)
                return "Event";

            SerializedProperty eventElement = _eventListsProperty.GetArrayElementAtIndex(index);
            SerializedProperty displayNameProperty =
                eventElement.FindPropertyRelative(nameof(ActionEventList.DisplayName));

            string name = displayNameProperty?.stringValue;
            return string.IsNullOrWhiteSpace(name) ? $"Event {index + 1}" : name;
        }

        private string[] GetTabLabels()
        {
            if (_eventListsProperty == null || _eventListsProperty.arraySize == 0)
                return System.Array.Empty<string>();

            string[] labels = new string[_eventListsProperty.arraySize];
            for (int i = 0; i < _eventListsProperty.arraySize; i++)
            {
                SerializedProperty eventElement = _eventListsProperty.GetArrayElementAtIndex(i);
                SerializedProperty displayNameProperty =
                    eventElement.FindPropertyRelative(nameof(ActionEventList.DisplayName));
                SerializedProperty actionsProperty = eventElement.FindPropertyRelative(nameof(ActionEventList.Actions));

                string displayName = displayNameProperty?.stringValue;
                if (string.IsNullOrWhiteSpace(displayName))
                    displayName = $"Event {i + 1}";

                int actionsCount = actionsProperty?.arraySize ?? 0;
                labels[i] = $"{displayName} ({actionsCount})";
            }

            return labels;
        }

        private void OnGUI()
        {
            if (_target == null)
            {
                EditorGUILayout.HelpBox("Select an object with ActionEvents and open this window from its inspector.",
                    UnityEditor.MessageType.Info);

                if (TryBindFromSelection())
                    Repaint();

                return;
            }

            if (_serializedTarget == null || _serializedTarget.targetObject == null)
            {
                Bind(_target, _windowTitle);
            }

            if (_serializedTarget == null || _eventListsProperty == null)
            {
                EditorGUILayout.HelpBox("Target does not contain ActionEvents list.", UnityEditor.MessageType.Error);
                return;
            }

            _serializedTarget.Update();

            EditorGUILayout.ObjectField("Target", _target, typeof(MonoBehaviour), true);
            GUILayout.Space(4f);

            if (_eventListsProperty.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No action event lists found. Define ActionEvents in component code.",
                    UnityEditor.MessageType.Warning);
            }
            else
            {
                int previousTab = _selectedEventIndex;
                _selectedEventIndex = Mathf.Clamp(_selectedEventIndex, 0, _eventListsProperty.arraySize - 1);
                _selectedEventIndex = GUILayout.Toolbar(_selectedEventIndex, GetTabLabels());

                if (previousTab != _selectedEventIndex || _actionsList == null)
                    RefreshActionsList();
            }

            GUILayout.Space(4f);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            if (_actionsList == null)
            {
                EditorGUILayout.HelpBox("Select an available tab to edit actions.", UnityEditor.MessageType.Info);
            }
            else
            {
                _actionsList.DoLayoutList();
            }

            EditorGUILayout.EndScrollView();

            if (_serializedTarget.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(_target);
                RefreshActionsList();
            }
        }

        private bool TryBindFromSelection()
        {
            if (Selection.activeGameObject == null)
                return false;

            if (!TryFindActionEventTarget(Selection.activeGameObject, out MonoBehaviour target))
                return false;

            Bind(target, _windowTitle);
            return true;
        }

        private static bool TryFindActionEventTarget(GameObject gameObject, out MonoBehaviour target)
        {
            target = null;

            if (gameObject == null)
                return false;

            foreach (MonoBehaviour component in gameObject.GetComponents<MonoBehaviour>())
            {
                if (component == null)
                    continue;

                SerializedObject serializedComponent = new SerializedObject(component);
                SerializedProperty actionEventsProperty = serializedComponent.FindProperty(EventListsPropertyName);
                if (actionEventsProperty == null || !actionEventsProperty.isArray)
                    continue;

                target = component;
                return true;
            }

            return false;
        }

        private static void OnAddAction(SerializedProperty actionsProperty)
        {
            int index = actionsProperty.arraySize;
            actionsProperty.InsertArrayElementAtIndex(index);

            // SerializedProperty element = actionsProperty.GetArrayElementAtIndex(index);
            // element.FindPropertyRelative(nameof(ActionGame.Type)).intValue = (int)ActionType.Command;
            // element.FindPropertyRelative(nameof(ActionGame.ActionDelay)).floatValue = 0;
            // element.FindPropertyRelative(nameof(ActionGame.Value)).stringValue = string.Empty;
            // element.FindPropertyRelative(nameof(ActionGame.Param)).stringValue = string.Empty;
            // element.FindPropertyRelative(nameof(ActionGame.ParamType)).intValue = 0;
            // element.FindPropertyRelative(nameof(ActionGame.TargetId)).intValue = 0;
            // element.FindPropertyRelative(nameof(ActionGame.Target)).objectReferenceValue = null;
            //
            // SerializedProperty expandedProperty = element.FindPropertyRelative(nameof(ActionGame.EditorIsExpanded));
            // if (expandedProperty != null) expandedProperty.boolValue = true;
            //
            // SerializedProperty nameProperty = element.FindPropertyRelative(nameof(ActionGame.Name));
            // if (nameProperty != null) nameProperty.stringValue = string.Empty;
        }

        private static float GetActionElementHeight(SerializedProperty actionsProperty, int index)
        {
            if (actionsProperty == null || index < 0 || index >= actionsProperty.arraySize)
                return EditorGUIUtility.singleLineHeight + 6f;

            SerializedProperty element = actionsProperty.GetArrayElementAtIndex(index);

            float height = LineWithSpacing(); // Header

            SerializedProperty expandedProperty = element.FindPropertyRelative(nameof(ActionGame.EditorIsExpanded));
            if (expandedProperty != null && !expandedProperty.boolValue)
            {
                return height + InnerPadding;
            }

            SerializedProperty typeProperty = element.FindPropertyRelative(nameof(ActionGame.Type));
            ActionType actionType = (ActionType)typeProperty.intValue;

            height += LineWithSpacing();
            height += LineWithSpacing();
            height += LineWithSpacing();

            if (actionType == ActionType.Animation)
            {
                SerializedProperty targetProperty = element.FindPropertyRelative(nameof(ActionGame.Target));
                SerializedProperty paramProperty = element.FindPropertyRelative(nameof(ActionGame.Param));

                height += LineWithSpacing();
                height += LineWithSpacing();

                Animator animator = GetAnimator(targetProperty.objectReferenceValue as GameObject);
                
                if (animator == null)
                {
                    height += HelpBoxHeight + RowSpacing;
                }
                else if (ShouldShowAnimationValueField(animator, paramProperty.stringValue))
                {
                    height += LineWithSpacing();
                    if (((AnimatorController)animator.runtimeAnimatorController).layers.Length == 0)
                        height += HelpBoxHeight + RowSpacing;
                }
            }
            else if (actionType == ActionType.SetComponentProperty)
            {
                height += LineWithSpacing(); // Target
                height += LineWithSpacing(); // Param
                height += LineWithSpacing(); // Value
            }
            else
            {
                height += LineWithSpacing();
            }

            return height + InnerPadding;
        }

        private static void DrawActionElement(SerializedProperty actionsProperty, Rect rect, int index, bool isActive,
            bool isFocused)
        {
            if (actionsProperty == null || index < 0 || index >= actionsProperty.arraySize)
                return;

            SerializedProperty element = actionsProperty.GetArrayElementAtIndex(index);
            SerializedProperty typeProperty = element.FindPropertyRelative(nameof(ActionGame.Type));
            SerializedProperty actionDelayProperty = element.FindPropertyRelative(nameof(ActionGame.ActionDelay));
            SerializedProperty valueProperty = element.FindPropertyRelative(nameof(ActionGame.Value));
            SerializedProperty targetProperty = element.FindPropertyRelative(nameof(ActionGame.Target));
            SerializedProperty targetIdProperty = element.FindPropertyRelative(nameof(ActionGame.TargetId));
            SerializedProperty paramProperty = element.FindPropertyRelative(nameof(ActionGame.Param));
            SerializedProperty paramTypeProperty = element.FindPropertyRelative(nameof(ActionGame.ParamType));

            SerializedProperty expandedProperty = element.FindPropertyRelative(nameof(ActionGame.EditorIsExpanded));
            SerializedProperty nameProperty = element.FindPropertyRelative(nameof(ActionGame.Name));

            ActionType actionType = (ActionType)typeProperty.intValue;
            float y = rect.y + InnerPadding;

            bool isExpanded = expandedProperty == null || expandedProperty.boolValue;
            string customName = nameProperty != null ? nameProperty.stringValue : string.Empty;
            string displayHeader = string.IsNullOrWhiteSpace(customName) ? $"{actionType} {index + 1}" : customName;

            GUIContent header = new GUIContent(displayHeader, GetTypeIcon(actionType));

            Rect headerRect = new Rect(rect.x + 10, y, rect.width - 10, EditorGUIUtility.singleLineHeight);

            if (expandedProperty != null)
            {
                expandedProperty.boolValue =
                    EditorGUI.Foldout(new Rect(rect.x, y, 20, EditorGUIUtility.singleLineHeight),
                        expandedProperty.boolValue, GUIContent.none);
            }
            else
            {
                EditorGUI.LabelField(new Rect(rect.x, y, 20, EditorGUIUtility.singleLineHeight), "►");
            }

            EditorGUI.LabelField(new Rect(rect.x + 20, y, rect.width, EditorGUIUtility.singleLineHeight), header,
                EditorStyles.boldLabel);

            y += LineWithSpacing();

            if (!isExpanded)
                return;

            if (nameProperty != null)
            {
                GUI.SetNextControlName($"ActionName{index}");
                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                    nameProperty);
                y += LineWithSpacing();
            }

            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), typeProperty);
            y += LineWithSpacing();

            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                actionDelayProperty);
            y += LineWithSpacing();

            if (actionType == ActionType.Command)
            {
                if (paramTypeProperty.intValue != 0) paramTypeProperty.intValue = 0;
                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                    valueProperty);
                return;
            }

            if (actionType == ActionType.SetComponentProperty)
            {
                if (paramTypeProperty.intValue != 0) paramTypeProperty.intValue = 0;

                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                    targetProperty);
                GameObject targetPropObj = targetProperty.objectReferenceValue as GameObject;
                var newTargetIdProp = targetPropObj != null ? targetPropObj.GetId() : 0;
#if UNITY_6000_5_OR_NEWER
                if (targetIdProperty.ulongValue != newTargetIdProp) targetIdProperty.ulongValue = newTargetIdProp;
#else
                if (targetIdProperty.intValue != newTargetIdProp) targetIdProperty.intValue = newTargetIdProp;
#endif
                y += LineWithSpacing();

                DrawComponentPropertySelection(paramProperty, valueProperty, targetPropObj, rect.x, ref y, rect.width);
                return;
            }

            if (actionType == ActionType.Destroy)
            {
                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                    targetProperty);
                GameObject targetPropObj = targetProperty.objectReferenceValue as GameObject;
                var newTargetIdProp = targetPropObj != null ? targetPropObj.GetId() : 0;
#if UNITY_6000_5_OR_NEWER
                if (targetIdProperty.ulongValue != newTargetIdProp) targetIdProperty.ulongValue = newTargetIdProp;
#else
                if (targetIdProperty.intValue != newTargetIdProp) targetIdProperty.intValue = newTargetIdProp;
#endif
                return;
            }

            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), targetProperty);
            GameObject targetObject = targetProperty.objectReferenceValue as GameObject;
            var newTargetId = targetObject != null ? targetObject.GetId() : 0;
#if UNITY_6000_5_OR_NEWER
            if (targetIdProperty.ulongValue != newTargetId) targetIdProperty.ulongValue = newTargetId;
#else
            if (targetIdProperty.intValue != newTargetId) targetIdProperty.intValue = newTargetId;
#endif
            y += LineWithSpacing();

            Animator animator = GetAnimator(targetObject);
            if (animator == null)
            {
                if (paramProperty.stringValue != string.Empty) paramProperty.stringValue = string.Empty;
                if (paramTypeProperty.intValue != 0) paramTypeProperty.intValue = 0;
                if (valueProperty.stringValue != string.Empty) valueProperty.stringValue = string.Empty;
                EditorGUI.HelpBox(new Rect(rect.x, y, rect.width, HelpBoxHeight),
                    "Target must have an Animator component.", UnityEditor.MessageType.Info);
                return;
            }

            AnimatorParameter[] parameters = GetAnimatorParameters(animator);
            if (parameters.Length == 0)
            {
                if (paramTypeProperty.intValue != 0) paramTypeProperty.intValue = 0;
                if (valueProperty.stringValue != string.Empty) valueProperty.stringValue = string.Empty;
            }

            var allStateNames = GetAllStateNames(animator.runtimeAnimatorController as AnimatorController);

            DrawParameterPopup(paramProperty, parameters, allStateNames, rect.x, y, rect.width);
            y += LineWithSpacing();

            if (!TryGetAnimatorParamType(animator, paramProperty.stringValue, out AnimatorParameterType parameterType))
            {
                if (paramTypeProperty.intValue != 0) paramTypeProperty.intValue = 0;
                return;
            }

            if (paramTypeProperty.intValue != (int)parameterType)
            {
                paramTypeProperty.intValue = (int)parameterType;
                GUI.changed = true;
            }

            DrawAnimationValueField(parameterType, valueProperty, ref y, rect.x, rect.width);
            
            if (animator.runtimeAnimatorController is AnimatorController controller && controller.layers.Length == 0)
            {
                EditorGUI.HelpBox(new Rect(rect.x, y, rect.width, HelpBoxHeight),
                    "Animator loaded from JSON. Only parameter modification is available.\n(May contain bugs. Load animations directly from Assets instead of from a pre-built schematic.)", UnityEditor.MessageType.Warning);
            }
        }

        private static void DrawComponentPropertySelection(SerializedProperty paramProperty,
            SerializedProperty valueProperty, GameObject target, float x, ref float y, float width)
        {
            if (target == null)
            {
                EditorGUI.PropertyField(new Rect(x, y, width, EditorGUIUtility.singleLineHeight), paramProperty,
                    new GUIContent("Property Name"));
                y += LineWithSpacing();
                EditorGUI.PropertyField(new Rect(x, y, width, EditorGUIUtility.singleLineHeight), valueProperty,
                    new GUIContent("New Value"));
                y += LineWithSpacing();
                return;
            }

            List<string> displayOptions = new List<string>();
            List<string> propertyNames = new List<string>();
            List<Type> propertyTypes = new List<Type>();
            
            var comp = target.GetComponent<SchematicBlock>();
            Type type = comp.GetType();
            foreach (var field in type.GetFields(System.Reflection.BindingFlags.Public |
                                                 System.Reflection.BindingFlags.Instance))
            {
                if (field.Name is nameof(ActionEventHostBlockBase.ActionEvents) or nameof(DoorComponent.DoorType))
                    continue;
                displayOptions.Add($"{field.Name}");
                propertyNames.Add(field.Name);
                propertyTypes.Add(field.FieldType);
            }
            displayOptions.Add("Play");
            propertyNames.Add("Play");
            propertyTypes.Add(typeof(object));
            if (displayOptions.Count == 0)
            {
                EditorGUI.PropertyField(new Rect(x, y, width, EditorGUIUtility.singleLineHeight), paramProperty,
                    new GUIContent("Property Name"));
                y += LineWithSpacing();
                EditorGUI.PropertyField(new Rect(x, y, width, EditorGUIUtility.singleLineHeight), valueProperty,
                    new GUIContent("New Value"));
                y += LineWithSpacing();
                return;
            }

            int selectedIndex = propertyNames.IndexOf(paramProperty.stringValue);
            if (selectedIndex < 0) selectedIndex = 0;

            int newIndex = EditorGUI.Popup(new Rect(x, y, width, EditorGUIUtility.singleLineHeight), "Property",
                selectedIndex, displayOptions.ToArray());
            paramProperty.stringValue = propertyNames[newIndex];
            y += LineWithSpacing();

            Type selectedType = propertyTypes[newIndex];
            DrawPropertyValueFieldByType(selectedType, valueProperty, ref y, x, width);
        }

        private static void DrawPropertyValueFieldByType(Type type, SerializedProperty valueProperty, ref float y,
            float x, float width)
        {
            if (type == typeof(bool))
            {
                bool boolValue = string.Equals(valueProperty.stringValue, bool.TrueString,
                    StringComparison.OrdinalIgnoreCase);
                int selected = EditorGUI.Popup(new Rect(x, y, width, EditorGUIUtility.singleLineHeight), "Value",
                    boolValue ? 1 : 0, new[] { bool.FalseString, bool.TrueString });
                string newVal = selected == 1 ? bool.TrueString : bool.FalseString;
                if (valueProperty.stringValue != newVal) valueProperty.stringValue = newVal;
            }
            else if (type == typeof(int))
            {
                int.TryParse(valueProperty.stringValue, NumberStyles.Integer, CultureInfo.InvariantCulture,
                    out int intValue);
                int newInt = EditorGUI.IntField(new Rect(x, y, width, EditorGUIUtility.singleLineHeight), "Value",
                    intValue);
                string newStr = newInt.ToString(CultureInfo.InvariantCulture);
                if (valueProperty.stringValue != newStr) valueProperty.stringValue = newStr;
            }
            else if (type == typeof(float))
            {
                float.TryParse(valueProperty.stringValue, NumberStyles.Float, CultureInfo.InvariantCulture,
                    out float floatValue);
                float newFloat = EditorGUI.FloatField(new Rect(x, y, width, EditorGUIUtility.singleLineHeight), "Value",
                    floatValue);
                string newStr = newFloat.ToString(CultureInfo.InvariantCulture);
                if (valueProperty.stringValue != newStr) valueProperty.stringValue = newStr;
            }
            else if (type == typeof(Color))
            {
                Color colorValue = Color.white;
                if (!string.IsNullOrEmpty(valueProperty.stringValue))
                {
                    if (valueProperty.stringValue.Contains(":"))
                    {
                        var parts = valueProperty.stringValue.Split(':');
                        if (parts.Length >= 3)
                        {
                            float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float r);
                            float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float g);
                            float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float b);
                            float a = 1f;
                            if (parts.Length >= 4)
                                float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out a);
                            colorValue = new Color(r / 255f, g / 255f, b / 255f, a);
                        }
                    }
                    else
                    {
                        ColorUtility.TryParseHtmlString(
                            valueProperty.stringValue.StartsWith("#")
                                ? valueProperty.stringValue
                                : "#" + valueProperty.stringValue, out colorValue);
                    }
                }

                colorValue = EditorGUI.ColorField(new Rect(x, y, width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Value"), colorValue, true, true, true);
                string colorStr;
                if (colorValue.r <= 1 && colorValue.g <= 1 && colorValue.b <= 1)
                    colorStr = ColorUtility.ToHtmlStringRGBA(colorValue);
                else
                    colorStr = string.Format(CultureInfo.InvariantCulture, "{0}:{1}:{2}:{3}", colorValue.r * 255f,
                        colorValue.g * 255f, colorValue.b * 255f, colorValue.a);
                if (valueProperty.stringValue != colorStr) valueProperty.stringValue = colorStr;
            }
            else if (type.IsEnum)
            {
                if (Attribute.IsDefined(type, typeof(FlagsAttribute)))
                {
                    Enum enumValue;
                    try
                    {
                        enumValue = (Enum)Enum.Parse(type,
                            string.IsNullOrEmpty(valueProperty.stringValue) ? "0" : valueProperty.stringValue, true);
                    }
                    catch
                    {
                        enumValue = (Enum)Activator.CreateInstance(type);
                    }

                    enumValue = EditorGUI.EnumFlagsField(new Rect(x, y, width, EditorGUIUtility.singleLineHeight),
                        "Value", enumValue);
                    string newEnumStr = enumValue.ToString();
                    if (valueProperty.stringValue != newEnumStr) valueProperty.stringValue = newEnumStr;
                }
                else
                {
                    string[] names = Enum.GetNames(type);
                    int idx = System.Array.IndexOf(names, valueProperty.stringValue);
                    if (idx < 0) idx = 0;
                    idx = EditorGUI.Popup(new Rect(x, y, width, EditorGUIUtility.singleLineHeight), "Value", idx,
                        names);
                    if (names.Length > 0)
                    {
                        string newEnumName = names[idx];
                        if (valueProperty.stringValue != newEnumName) valueProperty.stringValue = newEnumName;
                    }
                }
            }
            else if (type == typeof(SerializableVector) || type == typeof(Vector3))
            {
                Vector3 vecValue = Vector3.zero;
                if (!string.IsNullOrEmpty(valueProperty.stringValue))
                {
                    var parts = valueProperty.stringValue.Split(':');
                    if (parts.Length >= 2)
                    {
                        float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float px);
                        float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float py);
                        float pz = 0f;
                        if (parts.Length >= 3)
                            float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out pz);
                        vecValue = new Vector3(px, py, pz);
                    }
                }

                vecValue = EditorGUI.Vector3Field(new Rect(x, y, width - 100, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Value"), vecValue);
                string newVecStr = string.Format(CultureInfo.InvariantCulture, "{0}:{1}:{2}", vecValue.x, vecValue.y,
                    vecValue.z);
                if (valueProperty.stringValue != newVecStr) valueProperty.stringValue = newVecStr;
            }
            else if (type == typeof(string))
            {
                EditorGUI.PropertyField(new Rect(x, y, width, EditorGUIUtility.singleLineHeight), valueProperty,
                    new GUIContent("Value"));
            }
            else
            {
                EditorGUI.LabelField(new Rect(x, y, width, EditorGUIUtility.singleLineHeight), new GUIContent("Value"));
            }

            y += LineWithSpacing();
        }

        private static void DrawParameterPopup(SerializedProperty paramProperty, AnimatorParameter[] parameters, List<string> allStateNames,
            float x, float y, float width)
        {
            string[] options = new string[allStateNames.Count + parameters.Length + 2];
            for (int i = 0; i < parameters.Length; i++)
            {
                options[i] = $"Parameters/{parameters[i].name}";
            }

            for (int i = 0; i < allStateNames.Count; i++)
            {
                options[parameters.Length + i] = $"Animation/{allStateNames[i]}";
            }
            
            options[parameters.Length + allStateNames.Count] = "Pause";
            options[parameters.Length + allStateNames.Count + 1] = "Resume";

            int selectedIndex = System.Array.IndexOf(options, paramProperty.stringValue);
            if (selectedIndex < 0)
                selectedIndex = 0;

            int newIndex = EditorGUI.Popup(new Rect(x, y, width, EditorGUIUtility.singleLineHeight), "Param",
                selectedIndex, options);
            string newParam = newIndex >= 0 && newIndex < options.Length ? options[newIndex] : string.Empty;
            if (paramProperty.stringValue != newParam)
            {
                paramProperty.stringValue = newParam;
                GUI.changed = true;
            }
        }

        private static void DrawAnimationValueField(AnimatorParameterType parameterType,
            SerializedProperty valueProperty, ref float y, float x, float width)
        {
            switch (parameterType)
            {
                case AnimatorParameterType.Trigger:
                    if (valueProperty.stringValue != string.Empty) valueProperty.stringValue = string.Empty;
                    break;

                case AnimatorParameterType.Bool:
                {
                    bool boolValue = string.Equals(valueProperty.stringValue, bool.TrueString,
                        StringComparison.OrdinalIgnoreCase);
                    int selected = EditorGUI.Popup(
                        new Rect(x, y, width, EditorGUIUtility.singleLineHeight),
                        "Value",
                        boolValue ? 1 : 0,
                        new[] { bool.FalseString, bool.TrueString });
                    string newVal = selected == 1 ? bool.TrueString : bool.FalseString;
                    if (valueProperty.stringValue != newVal) valueProperty.stringValue = newVal;
                    y += LineWithSpacing();
                    break;
                }

                case AnimatorParameterType.Int:
                {
                    int.TryParse(valueProperty.stringValue, NumberStyles.Integer, CultureInfo.InvariantCulture,
                        out int intValue);
                    int newInt = EditorGUI.IntField(new Rect(x, y, width, EditorGUIUtility.singleLineHeight), "Value",
                        intValue);
                    string newStr = newInt.ToString(CultureInfo.InvariantCulture);
                    if (valueProperty.stringValue != newStr) valueProperty.stringValue = newStr;
                    y += LineWithSpacing();
                    break;
                }

                case AnimatorParameterType.Float:
                {
                    float.TryParse(valueProperty.stringValue, NumberStyles.Float, CultureInfo.InvariantCulture,
                        out float floatValue);
                    float newFloat = EditorGUI.FloatField(new Rect(x, y, width, EditorGUIUtility.singleLineHeight),
                        "Value", floatValue);
                    string newStrf = newFloat.ToString(CultureInfo.InvariantCulture);
                    if (valueProperty.stringValue != newStrf) valueProperty.stringValue = newStrf;
                    y += LineWithSpacing();
                    break;
                }
                default:
                    if (valueProperty.stringValue != string.Empty) valueProperty.stringValue = string.Empty;
                    y += LineWithSpacing();
                    break;
            }
        }

        private static bool ShouldShowAnimationValueField(Animator animator, string parameterName)
        {
            return TryGetAnimatorParamType(animator, parameterName, out AnimatorParameterType parameterType) &&
                   parameterType != AnimatorParameterType.Trigger;
        }

        private static bool TryGetAnimatorParamType(Animator animator, string targetName,
            out AnimatorParameterType parameterType)
        {
            parameterType = AnimatorParameterType.Trigger;

            if (animator == null || string.IsNullOrEmpty(targetName))
                return false;

            var split = targetName.Split('/');
            if (split.Length < 2)
                return false;
            foreach (AnimatorParameter parameter in GetAnimatorParameters(animator))
            {
                if (parameter.name != split[1])
                    continue;

                parameterType = parameter.type;
                return true;
            }

            return false;
        }

        private static Animator GetAnimator(GameObject gameObject)
        {
            if (gameObject == null)
                return null;

            Animator animator = gameObject.GetComponent<Animator>();
            return animator != null
                ? animator
                : gameObject.GetComponentInChildren<Animator>(true);
        }

        private static AnimatorParameter[] GetAnimatorParameters(Animator animator)
        {
            if (animator == null)
                return System.Array.Empty<AnimatorParameter>();

            AnimatorParameter[] parameters = animator.parameters;
            if (parameters != null && parameters.Length > 0)
                return parameters;

            RuntimeAnimatorController runtimeController = animator.runtimeAnimatorController;
            if (runtimeController == null)
                return System.Array.Empty<AnimatorParameter>();

            if (runtimeController is AnimatorOverrideController overrideController)
                runtimeController = overrideController.runtimeAnimatorController;

            if (runtimeController is AnimatorController animatorController &&
                animatorController.parameters != null &&
                animatorController.parameters.Length > 0)
            {
                return animatorController.parameters;
            }

            return parameters ?? System.Array.Empty<AnimatorParameter>();
        }

        private static Texture GetTypeIcon(ActionType type)
        {
            string iconName = type switch
            {
                ActionType.Command => "console.infoicon.sml",
                ActionType.Animation => "Animation Icon",
                ActionType.Destroy => "d_console.erroricon.sml",
                _ => "FilterByType",
            };

            return EditorGUIUtility.IconContent(iconName)?.image;
        }

        private static float LineWithSpacing()
        {
            return EditorGUIUtility.singleLineHeight + RowSpacing;
        }
        
        private static List<string> GetAllStateNames(AnimatorController controller)
        {
            List<string> stateNames = new List<string>();
            if (controller == null) 
                return stateNames;

            foreach (AnimatorControllerLayer layer in controller.layers)
            {
                if (layer.stateMachine != null)
                    CollectStatesRecursive(layer.stateMachine, stateNames);
            }

            return stateNames;
        }

        private static void CollectStatesRecursive(AnimatorStateMachine stateMachine, List<string> collected)
        {
            foreach (ChildAnimatorState child in stateMachine.states)
            {
                if (child.state != null)
                    collected.Add(child.state.name);
            }

            foreach (ChildAnimatorStateMachine childSM in stateMachine.stateMachines)
            {
                if (childSM.stateMachine != null)
                    CollectStatesRecursive(childSM.stateMachine, collected);
            }
        }
    }
}