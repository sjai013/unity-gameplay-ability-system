using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.CodeDom.Compiler;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class TimelinePlayableWizard : EditorWindow
{
    public class Variable : IComparable
    {
        public string name;
        public UsableType usableType;

        int m_TypeIndex;

        public string NameAsPrivate
        {
            get
            {
                string returnVal = "m_" + name[0].ToString().ToUpper();
                for (int i = 1; i < name.Length; i++)
                {
                    returnVal += name[i];
                }
                return returnVal;
            }
        }

        public Variable (string name, UsableType usableType)
        {
            this.name = name;
            this.usableType = usableType;
        }

        public bool GUI (UsableType[] usableTypes)
        {
            bool removeThis = false;
            EditorGUILayout.BeginHorizontal();
            name = EditorGUILayout.TextField(name);
            m_TypeIndex = EditorGUILayout.Popup(m_TypeIndex, UsableType.GetNamewithSortingArray (usableTypes));
            usableType = usableTypes[m_TypeIndex];
            if (GUILayout.Button("Remove", GUILayout.Width(60f)))
            {
                removeThis = true;
            }
            EditorGUILayout.EndHorizontal();

            return removeThis;
        }

        public int CompareTo (object obj)
        {
            if (obj == null)
                return 1;

            UsableType other = (UsableType)obj;

            if (other == null)
                throw new ArgumentException("This object is not a Variable.");

            return name.ToLower().CompareTo(other.name.ToLower());
        }

        public static UsableType[] GetUsableTypesFromVariableArray (Variable[] variables)
        {
            UsableType[] usableTypes = new UsableType[variables.Length];
            for (int i = 0; i < usableTypes.Length; i++)
            {
                usableTypes[i] = variables[i].usableType;
            }
            return usableTypes;
        }
    }


    public class UsableType : IComparable
    {
        public readonly string name;
        public readonly string nameWithSorting;
        public readonly string additionalNamespace;
        public readonly GUIContent guiContent;
        public readonly GUIContent guiContentWithSorting;
        public readonly Type type;

        public readonly string[] unrequiredNamespaces =
        {
            "UnityEngine",
            "UnityEngine.Timeline",
            "UnityEngine.Playables"
        };
        public const string blankAdditionalNamespace = "";

        const string k_NameForNullType = "None";

        public UsableType (Type usableType)
        {
            type = usableType;

            if (type != null)
            {
                name = usableType.Name;
                nameWithSorting = name.ToUpper ()[0] + "/" + name;
                additionalNamespace = unrequiredNamespaces.All (t => usableType.Namespace != t) ? usableType.Namespace : blankAdditionalNamespace;
            }
            else
            {
                name = k_NameForNullType;
                nameWithSorting = k_NameForNullType;
                additionalNamespace = blankAdditionalNamespace;
            }

            guiContent = new GUIContent(name);
            guiContentWithSorting = new GUIContent(nameWithSorting);
        }

        public UsableType (string name)
        {
            this.name = name;
            nameWithSorting = name.ToUpper()[0] + "/" + name;
            additionalNamespace = blankAdditionalNamespace;
            guiContent = new GUIContent(name);
            guiContentWithSorting = new GUIContent(nameWithSorting);
        }

        public int CompareTo (object obj)
        {
            if (obj == null)
                return 1;

            UsableType other = (UsableType)obj;
            
            if(other == null)
                throw new ArgumentException("This object is not a UsableType.");

            return name.ToLower().CompareTo (other.name.ToLower());
        }

        public static UsableType[] GetUsableTypeArray (Type[] types, params UsableType[] additionalUsableTypes)
        {
            List<UsableType> usableTypeList = new List<UsableType> ();
            for (int i = 0; i < types.Length; i++)
            {
                usableTypeList.Add (new UsableType (types[i]));
            }
            usableTypeList.AddRange (additionalUsableTypes);
            return usableTypeList.ToArray ();
        }

        public static UsableType[] AmalgamateUsableTypes (UsableType[] usableTypeArray, params UsableType[] usableTypes)
        {
            List<UsableType> usableTypeList = new List<UsableType> ();
            for (int i = 0; i < usableTypes.Length; i++)
            {
                usableTypeList.Add (usableTypes[i]);
            }
            usableTypeList.AddRange(usableTypeArray);
            return usableTypeList.ToArray();
        }

        public static string[] GetNamewithSortingArray(UsableType[] usableTypes)
        {
            if (usableTypes == null || usableTypes.Length == 0)
                return new string[0];

            string[] displayNames = new string[usableTypes.Length];
            for (int i = 0; i < displayNames.Length; i++)
            {
                displayNames[i] = usableTypes[i].nameWithSorting;
            }
            return displayNames;
        }

        public static GUIContent[] GetGUIContentWithSortingArray (UsableType[] usableTypes)
        {
            if(usableTypes == null || usableTypes.Length == 0)
                return new GUIContent[0];

            GUIContent[] guiContents = new GUIContent[usableTypes.Length];
            for (int i = 0; i < guiContents.Length; i++)
            {
                guiContents[i] = usableTypes[i].guiContentWithSorting;
            }
            return guiContents;
        }

        public static string[] GetDistinctAdditionalNamespaces (UsableType[] usableTypes)
        {
            if (usableTypes == null || usableTypes.Length == 0)
                return new string[0];

            string[] namespaceArray = new string[usableTypes.Length];
            for (int i = 0; i < namespaceArray.Length; i++)
            {
                namespaceArray[i] = usableTypes[i].additionalNamespace;
            }
            return namespaceArray.Distinct().ToArray();
        }
    }


    public class UsableProperty : IComparable
    {
        public enum Usability
        {
            Blendable, Assignable, Not
        }

        public string type;
        public string name;
        public string defaultValue;
        public Usability usability;
        public PropertyInfo propertyInfo;

        int m_TypeIndex;
        
        public string NameWithCaptial
        {
            get { return name.First().ToString().ToUpper() + name.Substring(1); }
        }

        public string NameAsPrivateDefault
        {
            get { return "m_Default" + NameWithCaptial; }
        }

        public string NameAsLocalBlended
        {
            get { return "blended" + NameWithCaptial; }
        }

        public string NameAsLocalSerializedProperty
        {
            get { return name + "Prop"; }
        }
        
        public UsableProperty (PropertyInfo propertyInfo)
        {
            this.propertyInfo = propertyInfo;

            if (propertyInfo.PropertyType.Name == "Single")
                type = "float";
            else if (propertyInfo.PropertyType.Name == "Int32")
                type = "int";
            else if (propertyInfo.PropertyType.Name == "Double")
                type = "double";
            else if (propertyInfo.PropertyType.Name == "Boolean")
                type = "bool";
            else if (propertyInfo.PropertyType.Name == "String")
                type = "string";
            else
                type = propertyInfo.PropertyType.Name;

            name = propertyInfo.Name;

            if (IsTypeBlendable(propertyInfo.PropertyType))
                usability = Usability.Blendable;
            else if (IsTypeAssignable(propertyInfo.PropertyType))
                usability = Usability.Assignable;
            else
                usability = Usability.Not;
        }

        public string ZeroValueAsString ()
        {
            if(usability != Usability.Blendable)
                throw new UnityException("UsableType is not blendable, shouldn't be looking for zero value as string.");

            switch (type)
            {
                case "float":
                    return "0f";
                case "int":
                    return "0";
                case "double":
                    return "0.0";
                case "Vector2":
                    return "Vector2.zero";
                case "Vector3":
                    return "Vector3.zero";
                case "Color":
                    return "Color.clear";
            }
            return "";
        }

        public void CreateSettingDefaultValueString (Component defaultValuesComponent)
        {
            if (defaultValuesComponent == null)
            {
                defaultValue = "";
                return;
            }
            
            object defaultValueObj = propertyInfo.GetValue (defaultValuesComponent, null);
            
            switch (type)
            {
                case "float":
                    float defaultFloatValue = (float)defaultValueObj;
                    defaultValue = defaultFloatValue + "f";
                    break;
                case "int":
                    int defaultIntValue = (int)defaultValueObj;
                    defaultValue = defaultIntValue.ToString();
                    break;
                case "double":
                    double defaultDoubleValue = (double)defaultValueObj;
                    defaultValue = defaultDoubleValue.ToString();
                    break;
                case "Vector2":
                    Vector2 defaultVector2Value = (Vector2)defaultValueObj;
                    defaultValue = "new Vector2(" + defaultVector2Value.x + "f, " + defaultVector2Value.y + "f)";
                    break;
                case "Vector3":
                    Vector3 defaultVector3Value = (Vector3)defaultValueObj;
                    defaultValue = "new Vector3(" + defaultVector3Value.x + "f, " + defaultVector3Value.y + "f, " + defaultVector3Value.z + "f)";
                    break;
                case "Color":
                    Color defaultColorValue = (Color)defaultValueObj;
                    defaultValue = "new Color(" + defaultColorValue.r + "f, " + defaultColorValue.g + "f, " + defaultColorValue.b + "f, " + defaultColorValue.a + "f)";
                    break;
                case "string":
                    defaultValue = "\"" + defaultValueObj + "\"";
                    break;
                case "Quaternion":
                    Quaternion defaultQuaternionValue = (Quaternion)defaultValueObj;
                    defaultValue = "new Quaternion(" + defaultQuaternionValue.x + "f, " + defaultQuaternionValue.y + "f, " + defaultQuaternionValue.z + "f, " + defaultQuaternionValue.w + "f)";
                    break;
                case "bool":
                    bool defaultBoolValue = (bool)defaultValueObj;
                    defaultValue = defaultBoolValue.ToString ().ToLower();
                    break;
                default:
                    Enum defaultEnumValue = (Enum)defaultValueObj;
                    Type enumSystemType = defaultEnumValue.GetType ();
                    string[] splits = enumSystemType.ToString().Split('+');
                    string enumType = splits[splits.Length - 1];
                    string enumConstantName = Enum.GetName (enumSystemType, defaultEnumValue);
                    defaultValue = enumType + "." + enumConstantName;
                    break;
            }
        }

        public string AssignmentAsSerializedPropertyFromTrackBinding ()
        {
            switch (type)
            {
                case "float":
                    return NameAsLocalSerializedProperty + ".floatValue = trackBinding." + name;
                case "int":
                    return NameAsLocalSerializedProperty + ".intValue = trackBinding." + name;
                case "double":
                    return NameAsLocalSerializedProperty + ".doubleValue = trackBinding." + name;
                case "Vector2":
                    return NameAsLocalSerializedProperty + ".vector2Value = trackBinding." + name;
                case "Vector3":
                    return NameAsLocalSerializedProperty + ".vector3Value = trackBinding." + name;
                case "Color":
                    return NameAsLocalSerializedProperty + ".colorValue = trackBinding." + name;
                case "string":
                    return NameAsLocalSerializedProperty + ".stringValue = trackBinding." + name;
                case "Quaternion":
                    return NameAsLocalSerializedProperty + ".quaternionValue = trackBinding." + name;
                case "bool":
                    return NameAsLocalSerializedProperty + ".boolValue = trackBinding." + name;
                default:
                    return NameAsLocalSerializedProperty + ".enumValueIndex = (int)trackBinding." + name;
            }
        }

        public bool GUI (UsableProperty[] allUsableProperties)
        {
            bool removeThis = false;
            EditorGUILayout.BeginHorizontal();
            m_TypeIndex = EditorGUILayout.Popup(m_TypeIndex, GetNameWithSortingArray (allUsableProperties));
            type = allUsableProperties[m_TypeIndex].type;
            name = allUsableProperties[m_TypeIndex].name;
            propertyInfo = allUsableProperties[m_TypeIndex].propertyInfo;
            usability = allUsableProperties[m_TypeIndex].usability;
            if (GUILayout.Button("Remove", GUILayout.Width(60f)))
            {
                removeThis = true;
            }
            EditorGUILayout.EndHorizontal();
            return removeThis;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            UsableType other = (UsableType)obj;

            if (other == null)
                throw new ArgumentException("This object is not a UsableProperty.");

            return name.ToLower().CompareTo(other.name.ToLower());
        }

        public static string[] GetNameWithSortingArray (UsableProperty[] usableProperties)
        {
            string[] returnVal = new string[usableProperties.Length];
            for (int i = 0; i < returnVal.Length; i++)
            {
                returnVal[i] = usableProperties[i].name;
            }
            return returnVal;
        }

        public static UsableProperty[] GetUsableProperties (PropertyInfo[] propertyInfos)
        {
            UsableProperty[] usableProperties = new UsableProperty[propertyInfos.Length];
            for (int i = 0; i < usableProperties.Length; i++)
            {
                usableProperties[i] = new UsableProperty (propertyInfos[i]);
            }
            return usableProperties;
        }
    }

    public enum CreationError
    {
        NoError,
        PlayableAssetAlreadyExists,
        PlayableBehaviourAlreadyExists,
        PlayableBehaviourMixerAlreadyExists,
        TrackAssetAlreadyExists,
        PlayableDrawerAlreadyExists,
    }


    public bool showHelpBoxes = true;
    public string playableName = "";
    public bool isStandardBlendPlayable;
    public UsableType trackBinding;
    public Component defaultValuesComponent;
    public List<Variable> exposedReferences = new List<Variable> ();
    public List<Variable> playableBehaviourVariables = new List<Variable> ();
    public List<UsableProperty> standardBlendPlayableProperties = new List<UsableProperty> ();
    public ClipCaps clipCaps;
    /*public bool setClipDefaults;
    public float clipDefaultDurationSeconds = 5f;
    public float clipDefaultEaseInSeconds;
    public float clipDefaultEaseOutSeconds;
    public float clipDefaultClipInSeconds;
    public float clipDefaultSpeedMultiplier = 1f;*/
    public Color trackColor = new Color(0.855f, 0.8623f, 0.870f);

    int m_TrackBindingTypeIndex;
    int m_ComponentBindingTypeIndex;
    PropertyInfo[] m_TrackBindingProperties;
    UsableProperty[] m_UsableProperties;
    bool m_CreateDrawer;
    bool m_CreateButtonPressed;
    Vector2 m_ScrollViewPos;
    CreationError m_CreationError;
    
    readonly GUIContent m_ShowHelpBoxesContent = new GUIContent("Show Help", "Do you want to see the help boxes as part of this wizard?");
    readonly GUIContent m_PlayableNameContent = new GUIContent("Playable Name", "This is the name that will represent the playable.  E.G. TransformTween.  It will be the basis for the class names so it is best not to use the postfixes: 'Clip', 'Behaviour', 'MixerBehaviour' or 'Drawer'.");
    readonly GUIContent m_StandardBlendPlayableContent = new GUIContent("Standard Blend Playable", "Often when creating a playable it's intended purpose is just to briefly override the properties of a component for the playable's duration and then blend back to the defaults.  For example a playable that changes the color of a Light but changes it back.  To make a playable with this functionality, check this box.");
    readonly GUIContent m_TrackBindingTypeContent = new GUIContent("Track Binding Type", "This is the type of object the Playable will affect.  E.G. To affect the position choose Transform.");
    readonly GUIContent m_DefaultValuesComponentContent = new GUIContent("Default Values", "When the scripts are created, each of the selected properties are assigned a default from the selected Component.  If this is left blank no defaults will be used.");
    readonly GUIContent m_ExposedReferencesContent = new GUIContent("Exposed References", "Exposed References are references to objects in a scene that your Playable needs. For example, if you want to tween between two Transforms, they will need to be Exposed References.");
    readonly GUIContent m_BehaviourVariablesContent = new GUIContent("Behaviour Variables", "Behaviour Variables are all the variables you wish to use in your playable that do NOT need a reference to something in a scene.  For example a float for speed.");
    readonly GUIContent m_TrackColorContent = new GUIContent("Track Color", "Timeline tracks have a colored outline, use this to select that color for your track.");
    readonly GUIContent m_CreateDrawerContent = new GUIContent("Create Drawer?", "Checking this box will enable the creation of a PropertyDrawer for your playable.  Having this script will make it easier to customise how your playable appears in the inspector.");
    readonly GUIContent m_StandardBlendPlayablePropertiesContent = new GUIContent("Standard Blend Playable Properties", "Having already selected a Track Binding type, you can select the properties of the bound component you want the playable to affect.  For example, if your playable is bound to a Transform, you can affect the position property.  Note that changing the component binding will clear the list of properties.");
    readonly GUIContent m_ClipCapsContent = new GUIContent("Clip Caps", "Clip Caps are used to change the way Timelines work with your playables.  For example, enabling Blending will mean that your playables can blend when they overlap and have ease in and out durations.  To find out a little about each hover the cursor over the options.  For details, please see the documentation.");
    readonly GUIContent m_CCNoneContent = new GUIContent("None", "Your playable supports none of the features below.");
    readonly GUIContent m_CCLoopingContent = new GUIContent("Looping", "Your playable has a specified time that it takes and will start again after it finishes until the clip's duration has played.");
    readonly GUIContent m_CCExtrapolationContent = new GUIContent("Extrapolation", "Your playable will persist beyond its end time and its results will continue until the next clip is encountered.");
    readonly GUIContent m_CCClipInContent = new GUIContent("Clip In", "Your playable need not be at the start of the Timeline.");
    readonly GUIContent m_CCSpeedMultiplierContent = new GUIContent("Speed Multiplier", "Your playable supports changes to the time scale.");
    readonly GUIContent m_CCBlendingContent = new GUIContent("Blending", "Your playable supports overlapping of clips to blend between them.");
    readonly GUIContent m_CCAllContent = new GUIContent("All", "Your playable supports all of the above features.");
    /*readonly GUIContent m_SetClipDefaultsContent = new GUIContent("Set Clip Defaults", "Do you want to set the default timings and other settings for clips when they are first created?");
    readonly GUIContent m_ClipDefaultsContent = new GUIContent("Clip Defaults");
    readonly GUIContent m_CDClipTimingContent = new GUIContent("Clip Timing", "Various settings that affect the durations over which the playable will be active.");
    readonly GUIContent m_CDDurationContent = new GUIContent("Duration", "The default length of the clip in seconds.");
    readonly GUIContent m_CDEaseInContent = new GUIContent("Ease In Duration", "The default duration over which the clip's weight increases to one.  When clips are overlapped, this is controlled by their overlap.  A clip requires the Blending ClipCap to support this.");
    readonly GUIContent m_CDEaseOutContent = new GUIContent("Ease Out Duration", "The default duration over which the clip's weight decreases to zero.  When clips are overlapped, this is controlled by their overlap.  A clip requires the Blending ClipCap to support this.");
    readonly GUIContent m_CDClipInContent = new GUIContent("Clip In", "The length of time after the start that the clip should start.  A clip requires the ClipIn ClipCap to support this.");
    readonly GUIContent m_CDSpeedMultiplierContent = new GUIContent("Speed Multiplier", "The amount a clip's time dependent aspects will speed up or slow down by.  A clip requires the SpeedMultiplier ClipCap to support this.");
    */
    const string k_Tab = "    ";
    const string k_ShowHelpBoxesKey = "TimelinePlayableWizard_ShowHelpBoxes";
    const string k_TimelineClipAssetSuffix = "Clip";
    const string k_TimelineClipBehaviourSuffix = "Behaviour";
    const string k_PlayableBehaviourMixerSuffix = "MixerBehaviour";
    const string k_TrackAssetSuffix = "Track";
    const string k_PropertyDrawerSuffix = "Drawer";
    const int k_PlayableNameCharLimit = 64;
    const float k_WindowWidth = 500f;
    const float k_MaxWindowHeight = 800f;
    const float k_ScreenSizeWindowBuffer = 50f;

    static UsableType[] s_ComponentTypes;
    static UsableType[] s_TrackBindingTypes;
    static UsableType[] s_ExposedReferenceTypes;
    static UsableType[] s_BehaviourVariableTypes;
    static Type[] s_BlendableTypes =
    {
        typeof(float), typeof(int), typeof(double), typeof(Vector2), typeof(Vector3), typeof(Color), 
    };
    static Type[] s_AssignableTypes =
    {
        typeof(string), typeof(Quaternion), typeof(bool)
    };

    [MenuItem("Window/Timeline Playable Wizard...")]
    static void CreateWindow ()
    {
        TimelinePlayableWizard wizard = GetWindow<TimelinePlayableWizard>(true, "Timeline Playable Wizard", true);
        
        Vector2 position = Vector2.zero;
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView != null)
            position = new Vector2(sceneView.position.x, sceneView.position.y);
        wizard.position = new Rect(position.x + k_ScreenSizeWindowBuffer, position.y + k_ScreenSizeWindowBuffer, k_WindowWidth, Mathf.Min(Screen.currentResolution.height - k_ScreenSizeWindowBuffer, k_MaxWindowHeight));
        
        wizard.showHelpBoxes = EditorPrefs.GetBool (k_ShowHelpBoxesKey);
        wizard.Show();
        
        Init ();
    }

    static void Init ()
    {
        Type[] componentTypes = typeof(Component).Assembly.GetTypes().Where(t => typeof(Component).IsAssignableFrom(t)).ToArray();
        UsableType[] componentUsableTypes = UsableType.GetUsableTypeArray(componentTypes);

        UsableType gameObjectUsableType = new UsableType(typeof(GameObject));
        UsableType[] defaultUsableTypes = UsableType.GetUsableTypeArray(componentTypes, gameObjectUsableType);

        Type[] uiTypes = typeof(Text).Assembly.GetTypes().Where(t => typeof(Component).IsAssignableFrom(t)).ToArray();
        UsableType[] uiUsableTypes = UsableType.GetUsableTypeArray(uiTypes);

        List<UsableType> componentTypesList = UsableType.AmalgamateUsableTypes(componentUsableTypes, uiUsableTypes).ToList();
        componentTypesList.Sort();
        s_ComponentTypes = componentTypesList.ToArray();

        List<UsableType> exposedRefTypeList = UsableType.AmalgamateUsableTypes(defaultUsableTypes, uiUsableTypes).ToList();
        exposedRefTypeList.Sort();
        s_ExposedReferenceTypes = exposedRefTypeList.ToArray();

        UsableType noneType = new UsableType((Type)null);
        s_TrackBindingTypes = UsableType.AmalgamateUsableTypes(s_ExposedReferenceTypes, noneType);

        s_BehaviourVariableTypes = UsableType.AmalgamateUsableTypes
            (
                s_ExposedReferenceTypes,
                new UsableType("int"),
                new UsableType("bool"),
                new UsableType("float"),
                new UsableType("Color"),
                new UsableType("double"),
                new UsableType("string"),
                new UsableType("Vector2"),
                new UsableType("Vector3"),
                new UsableType("AudioClip"),
                new UsableType("Quaternion"),
                new UsableType("AnimationCurve")
            );
        List<UsableType> scriptVariableTypeList = s_BehaviourVariableTypes.ToList();
        scriptVariableTypeList.Sort();
        s_BehaviourVariableTypes = scriptVariableTypeList.ToArray();
    }

    void OnGUI ()
    {
        if(s_ComponentTypes == null || s_TrackBindingTypes == null || s_ExposedReferenceTypes == null || s_BehaviourVariableTypes == null)
            Init ();

        m_ScrollViewPos = EditorGUILayout.BeginScrollView (m_ScrollViewPos);

        bool oldShowHelpBoxes = showHelpBoxes;
        showHelpBoxes = EditorGUILayout.Toggle (m_ShowHelpBoxesContent, showHelpBoxes);
        if (oldShowHelpBoxes != showHelpBoxes)
        {
            EditorPrefs.SetBool (k_ShowHelpBoxesKey, showHelpBoxes);
            EditorGUILayout.Space ();
        }

        if (showHelpBoxes)
        {
            EditorGUILayout.HelpBox("This wizard is used to create the basics of a custom playable for the Timeline. "
                               + "It will create 4 scripts that you can then edit to complete their functionality. "
                               + "The purpose is to setup the boilerplate code for you.  If you are already familiar "
                               + "with playables and the Timeline, you may which to create your own scripts instead.", MessageType.None);
            EditorGUILayout.Space();
        }

        EditorGUILayout.Space ();
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical (GUI.skin.box);
        if (showHelpBoxes)
        {
            EditorGUILayout.HelpBox(m_PlayableNameContent.tooltip, MessageType.Info);
            EditorGUILayout.Space();
        }
        playableName = EditorGUILayout.TextField (m_PlayableNameContent, playableName);

        bool playableNameNotEmpty = !string.IsNullOrEmpty (playableName);
        bool playableNameFormatted = CodeGenerator.IsValidLanguageIndependentIdentifier(playableName);
        if (!playableNameNotEmpty || !playableNameFormatted)
        {
            EditorGUILayout.HelpBox ("The Playable needs a name which starts with a capital letter and contains no spaces or special characters.", MessageType.Error);
        }
        bool playableNameTooLong = playableName.Length > k_PlayableNameCharLimit;
        if (playableNameTooLong)
        {
            EditorGUILayout.HelpBox ("The Playable needs a name which is fewer than " + k_PlayableNameCharLimit + " characters long.", MessageType.Error);
        }
        EditorGUILayout.EndVertical ();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        if(showHelpBoxes)
        {
            EditorGUILayout.HelpBox(m_StandardBlendPlayableContent.tooltip, MessageType.Info);
            EditorGUILayout.Space();
        }
        bool oldStandardBlendPlayable = isStandardBlendPlayable;
        isStandardBlendPlayable = EditorGUILayout.Toggle (m_StandardBlendPlayableContent, isStandardBlendPlayable);
        EditorGUILayout.EndVertical ();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        if(showHelpBoxes)
        {
            EditorGUILayout.HelpBox(m_TrackBindingTypeContent.tooltip, MessageType.Info);
            EditorGUILayout.Space();
        }
        int oldIndex = -1;
        if (isStandardBlendPlayable)
        {
            oldIndex = m_ComponentBindingTypeIndex;

            m_ComponentBindingTypeIndex = EditorGUILayout.Popup (m_TrackBindingTypeContent, m_ComponentBindingTypeIndex, UsableType.GetGUIContentWithSortingArray (s_ComponentTypes));
            trackBinding = s_ComponentTypes[m_ComponentBindingTypeIndex];

            EditorGUILayout.Space ();

            defaultValuesComponent = EditorGUILayout.ObjectField (m_DefaultValuesComponentContent, defaultValuesComponent, trackBinding.type, true) as Component;
        }
        else
        {
            m_TrackBindingTypeIndex = EditorGUILayout.Popup(m_TrackBindingTypeContent, m_TrackBindingTypeIndex, UsableType.GetGUIContentWithSortingArray(s_TrackBindingTypes));
            trackBinding = s_TrackBindingTypes[m_TrackBindingTypeIndex];
        }
        EditorGUILayout.EndVertical ();
        
        bool exposedVariablesNamesValid = true;
        bool scriptVariablesNamesValid = true;
        bool allUniqueVariableNames = true;
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (isStandardBlendPlayable)
        {
            StandardBlendPlayablePropertyGUI(oldIndex != m_ComponentBindingTypeIndex || oldStandardBlendPlayable != isStandardBlendPlayable);
        }
        else
        {
            exposedVariablesNamesValid = VariableListGUI(exposedReferences, s_ExposedReferenceTypes, m_ExposedReferencesContent, "newExposedReference");

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            scriptVariablesNamesValid = VariableListGUI(playableBehaviourVariables, s_BehaviourVariableTypes, m_BehaviourVariablesContent, "newBehaviourVariable");

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            allUniqueVariableNames = AllVariablesUniquelyNamed();
            if (!allUniqueVariableNames)
            {
                EditorGUILayout.HelpBox("Your variables to not have unique names.  Make sure all of your Exposed References and Behaviour Variables have unique names.", MessageType.Error);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            ClipCapsGUI();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        /*ClipDefaultsGUI ();

        EditorGUILayout.Space ();
        EditorGUILayout.Space ();*/

        EditorGUILayout.BeginVertical(GUI.skin.box);
        if (showHelpBoxes)
        {
            EditorGUILayout.HelpBox(m_TrackColorContent.tooltip, MessageType.Info);
            EditorGUILayout.Space();
        }
        trackColor = EditorGUILayout.ColorField(m_TrackColorContent, trackColor);
        EditorGUILayout.EndVertical ();

        if (!isStandardBlendPlayable)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            if (showHelpBoxes)
            {
                EditorGUILayout.HelpBox(m_CreateDrawerContent.tooltip, MessageType.Info);
                EditorGUILayout.Space();
            }
            m_CreateDrawer = EditorGUILayout.Toggle(m_CreateDrawerContent, m_CreateDrawer);
            EditorGUILayout.EndVertical ();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        if (playableNameNotEmpty && playableNameFormatted && allUniqueVariableNames && exposedVariablesNamesValid && scriptVariablesNamesValid && !playableNameTooLong)
        {
            if (GUILayout.Button("Create", GUILayout.Width(60f)))
            {
                m_CreateButtonPressed = true;

                for (int i = 0; i < standardBlendPlayableProperties.Count; i++)
                {
                    standardBlendPlayableProperties[i].CreateSettingDefaultValueString (defaultValuesComponent);
                }

                m_CreationError = CreateScripts();
                
                if(m_CreationError == CreationError.NoError)
                    Close ();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        if (m_CreateButtonPressed)
        {
            switch (m_CreationError)
            {
                case CreationError.NoError:
                    EditorGUILayout.HelpBox ("Playable was successfully created.", MessageType.Info);
                    break;
                case CreationError.PlayableAssetAlreadyExists:
                    EditorGUILayout.HelpBox ("The type " + playableName + k_TimelineClipAssetSuffix + " already exists, no files were created.", MessageType.Error);
                    break;
                case CreationError.PlayableBehaviourAlreadyExists:
                    EditorGUILayout.HelpBox ("The type " + playableName + k_TimelineClipBehaviourSuffix + " already exists, no files were created.", MessageType.Error);
                    break;
                case CreationError.PlayableBehaviourMixerAlreadyExists:
                    EditorGUILayout.HelpBox ("The type " + playableName + k_PlayableBehaviourMixerSuffix + " already exists, no files were created.", MessageType.Error);
                    break;
                case CreationError.TrackAssetAlreadyExists:
                    EditorGUILayout.HelpBox ("The type " + playableName + k_TrackAssetSuffix + " already exists, no files were created.", MessageType.Error);
                    break;
                case CreationError.PlayableDrawerAlreadyExists:
                    EditorGUILayout.HelpBox ("The type " + playableName + k_PropertyDrawerSuffix + " already exists, no files were created.", MessageType.Error);
                    break;
            }
        }

        if (GUILayout.Button ("Reset", GUILayout.Width (60f)))
        {
            ResetWindow ();
        }

        EditorGUILayout.EndScrollView ();
    }

    void StandardBlendPlayablePropertyGUI (bool findNewProperties)
    {
        if (findNewProperties || m_TrackBindingProperties == null)
        {
            IEnumerable<PropertyInfo> propertyInfos = trackBinding.type.GetProperties (BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty);
            propertyInfos = propertyInfos.Where(x => IsTypeBlendable(x.PropertyType) || IsTypeAssignable(x.PropertyType)).OrderBy(x => x.Name);
            propertyInfos = propertyInfos.Where (x => x.CanWrite && x.CanRead);
            // Uncomment the below to stop Obsolete properties being selectable.
            //propertyInfos = propertyInfos.Where (x => !Attribute.IsDefined (x, typeof(ObsoleteAttribute)));
            m_TrackBindingProperties = propertyInfos.ToArray();
            m_UsableProperties = UsableProperty.GetUsableProperties(m_TrackBindingProperties);
            standardBlendPlayableProperties.Clear ();
        }

        EditorGUILayout.BeginVertical (GUI.skin.box);

        if (showHelpBoxes)
        {
            EditorGUILayout.HelpBox(m_StandardBlendPlayablePropertiesContent.tooltip, MessageType.Info);
            EditorGUILayout.Space();
        }

        EditorGUILayout.LabelField(m_StandardBlendPlayablePropertiesContent);

        int indexToRemove = -1;
        for (int i = 0; i < standardBlendPlayableProperties.Count; i++)
        {
            if (standardBlendPlayableProperties[i].GUI(m_UsableProperties))
                indexToRemove = i;
        }
        if (indexToRemove != -1)
            standardBlendPlayableProperties.RemoveAt(indexToRemove);

        if (GUILayout.Button("Add", GUILayout.Width(40f)))
            standardBlendPlayableProperties.Add(new UsableProperty(m_TrackBindingProperties[0]));

        if (standardBlendPlayableProperties.Any(x => Attribute.IsDefined(x.propertyInfo, typeof(ObsoleteAttribute))))
            EditorGUILayout.HelpBox ("One or more of your chosen properties are marked 'Obsolete'.  Consider changing them to avoid deprecation with future versions of Unity.", MessageType.Warning);

        EditorGUILayout.EndVertical ();
    }

    static bool IsTypeBlendable (Type type)
    {
        for (int i = 0; i < s_BlendableTypes.Length; i++)
        {
            if (type == s_BlendableTypes[i])
                return true;
        }
        return false;
    }

    static bool IsTypeAssignable (Type type)
    {
        for (int i = 0; i < s_AssignableTypes.Length; i++)
        {
            if (type == s_AssignableTypes[i] || type.IsEnum)
                return true;
        }
        return false;
    }

    bool VariableListGUI (List<Variable> variables, UsableType[] usableTypes, GUIContent guiContent, string newName)
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);

        if (showHelpBoxes)
        {
            EditorGUILayout.HelpBox(guiContent.tooltip, MessageType.Info);
            EditorGUILayout.Space();
        }

        EditorGUILayout.LabelField(guiContent);

        int indexToRemove = -1;
        bool allNamesValid = true;
        for (int i = 0; i < variables.Count; i++)
        {
            if (variables[i].GUI(usableTypes))
                indexToRemove = i;

            if (!CodeGenerator.IsValidLanguageIndependentIdentifier(variables[i].name))
            {
                allNamesValid = false;
            }
        }

        if (indexToRemove != -1)
            variables.RemoveAt(indexToRemove);

        if (GUILayout.Button("Add", GUILayout.Width(40f)))
            variables.Add(new Variable(newName, usableTypes[0]));

        if (!allNamesValid)
            EditorGUILayout.HelpBox("One of the variables has an invalid character, make sure they don't contain any spaces or special characters.", MessageType.Error);

        EditorGUILayout.EndVertical();

        return allNamesValid;
    }

    bool AllVariablesUniquelyNamed ()
    {
        for (int i = 0; i < exposedReferences.Count; i++)
        {
            string exposedRefName = exposedReferences[i].name;

            for (int j = 0; j < exposedReferences.Count; j++)
            {
                if (i != j && exposedRefName == exposedReferences[j].name)
                    return false;
            }

            for (int j = 0; j < playableBehaviourVariables.Count; j++)
            {
                if (exposedRefName == playableBehaviourVariables[j].name)
                    return false;
            }
        }

        for (int i = 0; i < playableBehaviourVariables.Count; i++)
        {
            string scriptPlayableVariableName = playableBehaviourVariables[i].name;

            for (int j = 0; j < exposedReferences.Count; j++)
            {
                if (scriptPlayableVariableName == exposedReferences[j].name)
                    return false;
            }

            for (int j = 0; j < playableBehaviourVariables.Count; j++)
            {
                if (i != j && scriptPlayableVariableName == playableBehaviourVariables[j].name)
                    return false;
            }
        }
        return true;
    }

    void ClipCapsGUI ()
    {
        EditorGUILayout.BeginVertical (GUI.skin.box);

        if (showHelpBoxes)
        {
            EditorGUILayout.HelpBox(m_ClipCapsContent.tooltip, MessageType.Info);
            EditorGUILayout.Space();
        }

        EditorGUILayout.LabelField (m_ClipCapsContent);

        bool isLooping = (clipCaps & ClipCaps.Looping) == ClipCaps.Looping;
        bool isExtrapolation = (clipCaps & ClipCaps.Extrapolation) == ClipCaps.Extrapolation;
        bool isClipIn = (clipCaps & ClipCaps.ClipIn) == ClipCaps.ClipIn;
        bool isSpeedMultiplier = (clipCaps & ClipCaps.SpeedMultiplier) == ClipCaps.SpeedMultiplier;
        bool isBlending = (clipCaps & ClipCaps.Blending) == ClipCaps.Blending;

        bool isNone = !isLooping && !isExtrapolation && !isClipIn && !isSpeedMultiplier && !isBlending;
        bool isAll = isLooping && isExtrapolation && isClipIn && isSpeedMultiplier && isBlending;

        EditorGUI.BeginChangeCheck ();
        isNone = EditorGUILayout.ToggleLeft (m_CCNoneContent, isNone);
        if (EditorGUI.EndChangeCheck ())
        {
            if (isNone)
            {
                isLooping = false;
                isExtrapolation = false;
                isClipIn = false;
                isSpeedMultiplier = false;
                isBlending = false;
                isAll = false;
            }
        }

        EditorGUI.BeginChangeCheck ();
        isLooping = EditorGUILayout.ToggleLeft (m_CCLoopingContent, isLooping);
        isExtrapolation = EditorGUILayout.ToggleLeft (m_CCExtrapolationContent, isExtrapolation);
        isClipIn = EditorGUILayout.ToggleLeft (m_CCClipInContent, isClipIn);
        isSpeedMultiplier = EditorGUILayout.ToggleLeft (m_CCSpeedMultiplierContent, isSpeedMultiplier);
        isBlending = EditorGUILayout.ToggleLeft (m_CCBlendingContent, isBlending);
        if (EditorGUI.EndChangeCheck ())
        {
            isNone = !isLooping && !isExtrapolation && !isClipIn && !isSpeedMultiplier && !isBlending;
            isAll = isLooping && isExtrapolation && isClipIn && isSpeedMultiplier && isBlending;
        }

        EditorGUI.BeginChangeCheck ();
        isAll = EditorGUILayout.ToggleLeft (m_CCAllContent, isAll);
        if (EditorGUI.EndChangeCheck ())
        {
            if (isAll)
            {
                isNone = false;
                isLooping = true;
                isExtrapolation = true;
                isClipIn = true;
                isSpeedMultiplier = true;
                isBlending = true;
            }
        }

        EditorGUILayout.EndVertical();

        clipCaps = ClipCaps.None;

        if (isNone)
            return;

        if (isAll)
        {
            clipCaps = ClipCaps.All;
            return;
        }

        if (isLooping)
            clipCaps |= ClipCaps.Looping;

        if (isExtrapolation)
            clipCaps |= ClipCaps.Extrapolation;

        if (isClipIn)
            clipCaps |= ClipCaps.ClipIn;

        if (isSpeedMultiplier)
            clipCaps |= ClipCaps.SpeedMultiplier;

        if (isBlending)
            clipCaps |= ClipCaps.Blending;
    }

    /*void ClipDefaultsGUI ()
    {
        EditorGUILayout.BeginVertical (GUI.skin.box);

        setClipDefaults = EditorGUILayout.Toggle (m_SetClipDefaultsContent, setClipDefaults);

        if (!setClipDefaults)
        {
            EditorGUILayout.EndVertical ();
            return;
        }

        if (showHelpBoxes)
        {
            EditorGUILayout.HelpBox (m_ClipDefaultsContent.tooltip, MessageType.Info);
        }

        EditorGUILayout.LabelField (m_ClipDefaultsContent);

        EditorGUILayout.Space ();

        EditorGUILayout.LabelField (m_CDClipTimingContent);
        EditorGUI.indentLevel++;
        clipDefaultDurationSeconds = EditorGUILayout.FloatField(m_CDDurationContent, clipDefaultDurationSeconds);
        
        EditorGUILayout.Space ();
        
        clipDefaultEaseInSeconds = EditorGUILayout.FloatField(m_CDEaseInContent, clipDefaultEaseInSeconds);
        clipDefaultEaseOutSeconds = EditorGUILayout.FloatField (m_CDEaseOutContent, clipDefaultEaseOutSeconds);

        if (isStandardBlendPlayable)
        {
            EditorGUILayout.EndVertical();
            return;
        }

        EditorGUILayout.Space();
        
        clipDefaultClipInSeconds = EditorGUILayout.FloatField(m_CDClipInContent, clipDefaultClipInSeconds);
        
        EditorGUILayout.Space();
        
        clipDefaultSpeedMultiplier = EditorGUILayout.FloatField(m_CDSpeedMultiplierContent, clipDefaultSpeedMultiplier);
        EditorGUI.indentLevel--;
        
        EditorGUILayout.EndVertical();
    }*/

    CreationError CreateScripts ()
    {
        if (ScriptAlreadyExists(playableName + k_TimelineClipAssetSuffix))
            return CreationError.PlayableAssetAlreadyExists;

        if (ScriptAlreadyExists(playableName + k_TimelineClipBehaviourSuffix))
            return CreationError.PlayableBehaviourAlreadyExists;

        if (ScriptAlreadyExists(playableName + k_PlayableBehaviourMixerSuffix))
            return CreationError.PlayableBehaviourMixerAlreadyExists;

        if (ScriptAlreadyExists(playableName + k_TrackAssetSuffix))
            return CreationError.TrackAssetAlreadyExists;

        if (m_CreateDrawer && ScriptAlreadyExists(playableName + k_PropertyDrawerSuffix))
            return CreationError.PlayableDrawerAlreadyExists;

        AssetDatabase.CreateFolder ("Assets", playableName);

        if (isStandardBlendPlayable)
        {
            CreateScript (playableName + k_TimelineClipAssetSuffix, StandardBlendPlayableAsset());
            CreateScript (playableName + k_TimelineClipBehaviourSuffix, StandardBlendPlayableBehaviour ());
            CreateScript (playableName + k_PlayableBehaviourMixerSuffix, StandardBlendPlayableBehaviourMixer ());
            CreateScript (playableName + k_TrackAssetSuffix, StandardBlendTrackAssetScript ());

            AssetDatabase.CreateFolder ("Assets/" + playableName, "Editor");

            string path = Application.dataPath + "/" + playableName + "/Editor/" + playableName + k_PropertyDrawerSuffix + ".cs";
            using (StreamWriter writer = File.CreateText (path))
            {
                writer.Write (StandardBlendPlayableDrawer ());
            }
        }
        else
        {
            CreateScript(playableName + k_TimelineClipAssetSuffix, PlayableAsset());
            CreateScript(playableName + k_TimelineClipBehaviourSuffix, PlayableBehaviour());
            CreateScript(playableName + k_PlayableBehaviourMixerSuffix, PlayableBehaviourMixer());
            CreateScript(playableName + k_TrackAssetSuffix, TrackAssetScript());

            if (m_CreateDrawer)
            {
                AssetDatabase.CreateFolder("Assets/" + playableName, "Editor");

                string path = Application.dataPath + "/" + playableName + "/Editor/" + playableName + k_PropertyDrawerSuffix + ".cs";
                using (StreamWriter writer = File.CreateText(path))
                {
                    writer.Write(PlayableDrawer());
                }
            }
        }

        AssetDatabase.SaveAssets ();
        AssetDatabase.Refresh ();

        return CreationError.NoError;
    }

    bool ScriptAlreadyExists(string scriptName)
    {
        string[] guids = AssetDatabase.FindAssets(scriptName);

        if (guids.Length == 0)
            return false;

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            Type assetType = AssetDatabase.GetMainAssetTypeAtPath(path);
            if (assetType == typeof(MonoScript))
                return true;
        }

        return false;
    }

    void CreateScript (string fileName, string content)
    {
        string path = Application.dataPath + "/" + playableName + "/" + fileName + ".cs";
        using (StreamWriter writer = File.CreateText (path))
            writer.Write (content);
    }

    void ResetWindow ()
    {
        playableName = "";
        isStandardBlendPlayable = false;
        trackBinding = s_TrackBindingTypes[0];
        defaultValuesComponent = null;
        exposedReferences = new List<Variable>();
        playableBehaviourVariables = new List<Variable>();
        standardBlendPlayableProperties = new List<UsableProperty>();
        clipCaps = ClipCaps.None;
        /*setClipDefaults = false;
        clipDefaultDurationSeconds = 5f;
        clipDefaultEaseInSeconds = 0f;
        clipDefaultEaseOutSeconds = 0f;
        clipDefaultClipInSeconds = 0f;
        clipDefaultSpeedMultiplier = 1f;*/
        trackColor = new Color(0.855f, 0.8623f, 0.870f);

        m_TrackBindingTypeIndex = 0;
        m_ComponentBindingTypeIndex = 0;
        m_TrackBindingProperties = null;
        m_UsableProperties = null;
        m_CreateDrawer = false;
    }

    string TrackAssetScript ()
    {
        return 
            "using UnityEngine;\n" +
            "using UnityEngine.Playables;\n" +
            "using UnityEngine.Timeline;\n" +
            AdditionalNamespacesToString() + 
            "\n" +
            "[TrackColor(" + trackColor.r + "f, " + trackColor.g + "f, " + trackColor.b + "f)]\n" +
            "[TrackClipType(typeof(" + playableName + k_TimelineClipAssetSuffix + "))]\n" +
            TrackBindingToString () +
            "public class " + playableName + k_TrackAssetSuffix + " : TrackAsset\n" +
            "{\n" +
            k_Tab + "public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)\n" +
            k_Tab + "{\n" +
            k_Tab + k_Tab + "return ScriptPlayable<" + playableName + k_PlayableBehaviourMixerSuffix + ">.Create (graph, inputCount);\n" +
            k_Tab + "}\n" +
            "}\n";
    }

    string PlayableAsset ()
    {
        return 
            "using System;\n" +
            "using UnityEngine;\n" +
            "using UnityEngine.Playables;\n" +
            "using UnityEngine.Timeline;\n" +
            AdditionalNamespacesToString() +
            "\n" +
            "[Serializable]\n" +
            "public class " + playableName + k_TimelineClipAssetSuffix + " : PlayableAsset, ITimelineClipAsset\n" +
            "{\n" +
            k_Tab + "public " + playableName + k_TimelineClipBehaviourSuffix + " template = new " + playableName + k_TimelineClipBehaviourSuffix + " ();\n" +
            ExposedReferencesToString () +
            "\n" +
            k_Tab + "public ClipCaps clipCaps\n" +
            k_Tab + "{\n" +
            k_Tab + k_Tab + "get { return " + ClipCapsToString () + "; }\n" +
            k_Tab + "}\n" +
            "\n" +
            k_Tab + "public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)\n" +
            k_Tab + "{\n" +
            k_Tab + k_Tab + "var playable = ScriptPlayable<" + playableName + k_TimelineClipBehaviourSuffix + ">.Create (graph, template);\n" +
            ExposedReferencesResolvingToString () +
            k_Tab + k_Tab + "return playable;\n" +
            k_Tab + "}\n" +
            "}\n";
    }

    string PlayableBehaviour ()
    {
        return 
            "using System;\n" +
            "using UnityEngine;\n" +
            "using UnityEngine.Playables;\n" +
            "using UnityEngine.Timeline;\n" +
            AdditionalNamespacesToString() +
            "\n" +
            "[Serializable]\n" +
            "public class " + playableName + k_TimelineClipBehaviourSuffix + " : PlayableBehaviour\n" +
            "{\n" +
            ExposedReferencesAsScriptVariablesToString () +
            PlayableBehaviourVariablesToString () +
            "\n" +
            k_Tab + "public override void OnGraphStart (Playable playable)\n" +
            k_Tab + "{\n" +
            k_Tab + k_Tab + "\n" +
            k_Tab + "}\n" +
            "}\n";
    }

    string PlayableBehaviourMixer ()
    {
        return 
            "using System;\n" +
            "using UnityEngine;\n" +
            "using UnityEngine.Playables;\n" +
            "using UnityEngine.Timeline;\n" +
            AdditionalNamespacesToString() +
            "\n" +
            "public class " + playableName + k_PlayableBehaviourMixerSuffix + " : PlayableBehaviour\n" +
            "{\n" +
            k_Tab + "// NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.\n" +
            k_Tab + "public override void ProcessFrame(Playable playable, FrameData info, object playerData)\n" +
            k_Tab + "{\n" +
            MixerTrackBindingLocalVariableToString () +
            k_Tab + k_Tab + "int inputCount = playable.GetInputCount ();\n" +
            "\n" +
            k_Tab + k_Tab + "for (int i = 0; i < inputCount; i++)\n" +
            k_Tab + k_Tab + "{\n" +
            k_Tab + k_Tab + k_Tab + "float inputWeight = playable.GetInputWeight(i);\n" +
            k_Tab + k_Tab + k_Tab + "ScriptPlayable<" + playableName + k_TimelineClipBehaviourSuffix + "> inputPlayable = (ScriptPlayable<" + playableName + k_TimelineClipBehaviourSuffix + ">)playable.GetInput(i);\n" +
            k_Tab + k_Tab + k_Tab + playableName + k_TimelineClipBehaviourSuffix + " input = inputPlayable.GetBehaviour ();\n" +
            k_Tab + k_Tab + k_Tab + "\n" +
            k_Tab + k_Tab + k_Tab + "// Use the above variables to process each frame of this playable.\n" +
            k_Tab + k_Tab + k_Tab + "\n" +
            k_Tab + k_Tab + "}\n" +
            k_Tab + "}\n" +
            "}\n";
    }

    string PlayableDrawer ()
    {
        return 
            "using UnityEditor;\n" +
            "using UnityEngine;\n" +
            "\n" +
            "[CustomPropertyDrawer(typeof(" + playableName + k_TimelineClipBehaviourSuffix + "))]\n" +
            "public class " + playableName + k_PropertyDrawerSuffix + " : PropertyDrawer\n" +
            "{\n" +
            k_Tab + "public override float GetPropertyHeight (SerializedProperty property, GUIContent label)\n" +
            k_Tab + "{\n" +
            k_Tab + k_Tab + "int fieldCount = " + playableBehaviourVariables.Count +";\n" +
            k_Tab + k_Tab + "return fieldCount * EditorGUIUtility.singleLineHeight;\n" +
            k_Tab + "}\n" +
            "\n" +
            k_Tab + "public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)\n" +
            k_Tab + "{\n" +
            ScriptVariablesAsSerializedPropAssignmentToString () +
            "\n" + 
            k_Tab + k_Tab + "Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);\n" +
            ScriptVariablesAsSerializedPropGUIToString () +
            k_Tab + "}\n" +
            "}\n";
    }

    string TrackBindingToString ()
    {
        if (m_TrackBindingTypeIndex != 0)
            return "[TrackBindingType(typeof(" + trackBinding.name + "))]\n";
        return "";
    }

    string AdditionalNamespacesToString ()
    {
        UsableType[] exposedReferenceTypes = Variable.GetUsableTypesFromVariableArray (exposedReferences.ToArray ());
        UsableType[] behaviourVariableTypes = Variable.GetUsableTypesFromVariableArray (playableBehaviourVariables.ToArray ());
        UsableType[] allUsedTypes = new UsableType[exposedReferenceTypes.Length + behaviourVariableTypes.Length + 1];
        for (int i = 0; i < exposedReferenceTypes.Length; i++)
        {
            allUsedTypes[i] = exposedReferenceTypes[i];
        }
        for (int i = 0; i < behaviourVariableTypes.Length; i++)
        {
            allUsedTypes[i + exposedReferenceTypes.Length] = behaviourVariableTypes[i];
        }
        allUsedTypes[allUsedTypes.Length - 1] = trackBinding;

        string[] distinctNamespaces = UsableType.GetDistinctAdditionalNamespaces (allUsedTypes).Where (x => !string.IsNullOrEmpty (x)).ToArray ();
        string returnVal = "";
        for (int i = 0; i < distinctNamespaces.Length; i++)
        {
            returnVal += "using " + distinctNamespaces[i] + ";\n";
        }
        return returnVal;
    }

    string ExposedReferencesToString ()
    {
        string expRefText = "";
        foreach (var expRef in exposedReferences)
            expRefText += k_Tab + "public ExposedReference<" + expRef.usableType.name + "> " + expRef.name + ";\n";
        return expRefText;
    }

    string ExposedReferencesResolvingToString ()
    {
        string returnVal = "";
        returnVal += k_Tab + k_Tab + playableName + k_TimelineClipBehaviourSuffix + " clone = playable.GetBehaviour ();\n";
        for (int i = 0; i < exposedReferences.Count; i++)
        {
            returnVal += k_Tab + k_Tab + "clone." + exposedReferences[i].name + " = " + exposedReferences[i].name + ".Resolve (graph.GetResolver ());\n";
        }
        return returnVal;
    }

    /*string OnCreateFunctionToString ()
    {
        if (!setClipDefaults)
            return "";

        string returnVal = "\n";
            returnVal += k_Tab + "public override void OnCreate ()\n";
            returnVal += k_Tab + "{\n";
            returnVal += k_Tab + k_Tab + "owner.duration = " + clipDefaultDurationSeconds + ";\n";
            returnVal += k_Tab + k_Tab + "owner.easeInDuration = " + clipDefaultEaseInSeconds + ";\n";
            returnVal += k_Tab + k_Tab + "owner.easeOutDuration = " + clipDefaultEaseOutSeconds + ";\n";
            returnVal += k_Tab + k_Tab + "owner.clipIn = " + clipDefaultClipInSeconds + ";\n";
            returnVal += k_Tab + k_Tab + "owner.timeScale = " + clipDefaultSpeedMultiplier + ";\n";
            returnVal += k_Tab + "}\n";
        return returnVal;
    }*/

    string ClipCapsToString ()
    {
        string message = clipCaps.ToString ();
        string[] splits = message.Split (' ');

        for (int i = 0; i < splits.Length; i++)
        {
            if (splits[i][splits[i].Length - 1] == ',')
                splits[i] = splits[i].Substring (0, splits[i].Length - 1);
        }

        string returnVal = "";

        for (int i = 0; i < splits.Length; i++)
        {
            returnVal += "ClipCaps." + splits[i];

            if (i < splits.Length - 1)
                returnVal += " | ";
        }

        return returnVal;
    }

    string ExposedReferencesAsScriptVariablesToString ()
    {
        string returnVal = "";
        for (int i = 0; i < exposedReferences.Count; i++)
        {
            returnVal += k_Tab + "public " + exposedReferences[i].usableType.name + " " + exposedReferences[i].name + ";\n";
        }
        return returnVal;
    }

    string PlayableBehaviourVariablesToString ()
    {
        string returnVal = "";
        for (int i = 0; i < playableBehaviourVariables.Count; i++)
        {
            returnVal += k_Tab + "public " + playableBehaviourVariables[i].usableType.name + " " + playableBehaviourVariables[i].name + ";\n";
        }
        return returnVal;
    }

    string MixerTrackBindingLocalVariableToString ()
    {
        if (m_TrackBindingTypeIndex != 0)
            return
                k_Tab + k_Tab + trackBinding.name + " trackBinding = playerData as " + trackBinding.name + ";\n\n" +
                k_Tab + k_Tab + "if (!trackBinding)\n" +
                k_Tab + k_Tab + k_Tab + "return;\n" +
                "\n";
        return "";
    }

    string ScriptVariablesAsSerializedPropAssignmentToString ()
    {
        string returnVal = "";
        for (int i = 0; i < playableBehaviourVariables.Count; i++)
        {
            returnVal += k_Tab + k_Tab + "SerializedProperty " + playableBehaviourVariables[i].name + "Prop = property.FindPropertyRelative(\"" + playableBehaviourVariables[i].name + "\");\n";
        }
        return returnVal;
    }

    string ScriptVariablesAsSerializedPropGUIToString ()
    {
        string returnVal = "";
        for (int i = 0; i < playableBehaviourVariables.Count; i++)
        {
            returnVal += k_Tab + k_Tab + "EditorGUI.PropertyField(singleFieldRect, " + playableBehaviourVariables[i].name + "Prop);\n";

            if (i < playableBehaviourVariables.Count - 1)
            {
                returnVal += "\n";
                returnVal += k_Tab + k_Tab + "singleFieldRect.y += EditorGUIUtility.singleLineHeight;\n";
            }
        }
        return returnVal;
    }

    string StandardBlendPlayableAsset ()
    {
        return
            "using System;\n" +
            "using UnityEngine;\n" +
            "using UnityEngine.Playables;\n" +
            "using UnityEngine.Timeline;\n" +
            "\n" +
            "[Serializable]\n" +
            "public class " + playableName + k_TimelineClipAssetSuffix + " : PlayableAsset, ITimelineClipAsset\n" +
            "{\n" +
            k_Tab + "public " + playableName + k_TimelineClipBehaviourSuffix + " template = new " + playableName + k_TimelineClipBehaviourSuffix + " ();\n" +
            "\n" +
            k_Tab + "public ClipCaps clipCaps\n" +
            k_Tab + "{\n" +
            k_Tab + k_Tab + "get { return ClipCaps.Blending; }\n" +
            k_Tab + "}\n" +
            "\n" +
            k_Tab + "public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)\n" +
            k_Tab + "{\n" +
            k_Tab + k_Tab + "var playable = ScriptPlayable<" + playableName + k_TimelineClipBehaviourSuffix + ">.Create (graph, template);\n" +
            k_Tab + k_Tab + "return playable;\n" +
            k_Tab + "}\n" +
            "}\n";
    }

    string StandardBlendPlayableBehaviour ()
    {
        return
            "using System;\n" +
            "using UnityEngine;\n" +
            "using UnityEngine.Playables;\n" +
            "using UnityEngine.Timeline;\n" +
            AdditionalNamespacesToString() +
            "\n" +
            "[Serializable]\n" +
            "public class " + playableName + k_TimelineClipBehaviourSuffix + " : PlayableBehaviour\n" +
            "{\n" +
            StandardBlendScriptPlayablePropertiesToString () +
            "}\n";
    }

    string StandardBlendPlayableBehaviourMixer ()
    {
        return
            "using System;\n" +
            "using UnityEngine;\n" +
            "using UnityEngine.Playables;\n" +
            "using UnityEngine.Timeline;\n" +
            AdditionalNamespacesToString() +
            "\n" +
            "public class " + playableName + k_PlayableBehaviourMixerSuffix + " : PlayableBehaviour\n" +
            "{\n" +
            StandardBlendTrackBindingPropertiesDefaultsDeclarationToString () +
            "\n" +
            k_Tab + trackBinding.name + " m_TrackBinding;\n" +
            k_Tab + "bool m_FirstFrameHappened;\n" +
            "\n" +
            k_Tab + "public override void ProcessFrame(Playable playable, FrameData info, object playerData)\n" +
            k_Tab + "{\n" +
            k_Tab + k_Tab + "m_TrackBinding = playerData as " + trackBinding.name + ";\n" +
            "\n" +
            k_Tab + k_Tab + "if (m_TrackBinding == null)\n" +
            k_Tab + k_Tab + k_Tab + "return;\n" +
            "\n" +
            k_Tab + k_Tab + "if (!m_FirstFrameHappened)\n" +
            k_Tab + k_Tab + "{\n" +
            StandardBlendTrackBindingPropertiesDefaultsAssignmentToString () +
            k_Tab + k_Tab + k_Tab + "m_FirstFrameHappened = true;\n" +
            k_Tab + k_Tab + "}\n" +
            "\n" +
            k_Tab + k_Tab + "int inputCount = playable.GetInputCount ();\n" +
            "\n" +
            StandardBlendBlendedVariablesCreationToString () +
            k_Tab + k_Tab + "float totalWeight = 0f;\n" +
            k_Tab + k_Tab + "float greatestWeight = 0f;\n" +
            k_Tab + k_Tab + "int currentInputs = 0;\n" +
            "\n" +
            k_Tab + k_Tab + "for (int i = 0; i < inputCount; i++)\n" +
            k_Tab + k_Tab + "{\n" +
            k_Tab + k_Tab + k_Tab + "float inputWeight = playable.GetInputWeight(i);\n" +
            k_Tab + k_Tab + k_Tab + "ScriptPlayable<" + playableName + k_TimelineClipBehaviourSuffix + "> inputPlayable = (ScriptPlayable<" + playableName + k_TimelineClipBehaviourSuffix + ">)playable.GetInput(i);\n" +
            k_Tab + k_Tab + k_Tab + playableName + k_TimelineClipBehaviourSuffix + " input = inputPlayable.GetBehaviour ();\n" +
            k_Tab + k_Tab + k_Tab + "\n" +
            StandardBlendBlendedVariablesWeightedIncrementationToString () +
            k_Tab + k_Tab + k_Tab + "totalWeight += inputWeight;\n" +
            "\n" +
            StandardBlendAssignableVariablesAssignedBasedOnGreatestWeightToString () +
            "\n" +
            k_Tab + k_Tab + k_Tab + "if (!Mathf.Approximately (inputWeight, 0f))\n" +
            k_Tab + k_Tab + k_Tab + k_Tab + "currentInputs++;\n" +
            k_Tab + k_Tab + "}\n" +
            "\n" +
            StandardBlendTrackBindingPropertiesBlendedAssignmentToString () +
            StandardBlendTrackBindingPropertiesAssignableAssignmentToString () +
            k_Tab + "}\n" +
            "\n" +
            k_Tab + "public override void OnGraphStop (Playable playable)\n" +
            k_Tab + "{\n" +
            k_Tab + k_Tab + "m_FirstFrameHappened = false;\n" +
            "\n" +
            k_Tab + k_Tab + "if (m_TrackBinding == null)\n" +
            k_Tab + k_Tab + k_Tab + "return;\n" +
            "\n" +
            StandardBlendTrackBindingPropertiesDefaultAssignmentToString() +
            k_Tab + "}\n" +
            "}\n";
    }

    string StandardBlendTrackAssetScript ()
    {
        return
            "using UnityEngine;\n" +
            "using UnityEngine.Playables;\n" +
            "using UnityEngine.Timeline;\n" +
            "using System.Collections.Generic;\n" +
            AdditionalNamespacesToString() +
            "\n" +
            "[TrackColor(" + trackColor.r + "f, " + trackColor.g + "f, " + trackColor.b + "f)]\n" +
            "[TrackClipType(typeof(" + playableName + k_TimelineClipAssetSuffix + "))]\n" +
            StandardBlendComponentBindingToString () +
            "public class " + playableName + k_TrackAssetSuffix + " : TrackAsset\n" +
            "{\n" +
            k_Tab + "public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)\n" +
            k_Tab + "{\n" +
            k_Tab + k_Tab + "return ScriptPlayable<" + playableName + k_PlayableBehaviourMixerSuffix + ">.Create (graph, inputCount);\n" +
            k_Tab + "}\n" +
            "\n" +
            k_Tab + "public override void GatherProperties (PlayableDirector director, IPropertyCollector driver)\n" +
            k_Tab + "{\n" +
            "#if UNITY_EDITOR\n" +
            k_Tab + k_Tab + trackBinding.name + " trackBinding = director.GetGenericBinding(this) as " + trackBinding.name + ";\n" +
            k_Tab + k_Tab + "if (trackBinding == null)\n" +
            k_Tab + k_Tab + k_Tab + "return;\n" +
            "\n" +
            k_Tab + k_Tab + "var serializedObject = new UnityEditor.SerializedObject (trackBinding);\n" +
            k_Tab + k_Tab + "var iterator = serializedObject.GetIterator();\n" +
            k_Tab + k_Tab + "while (iterator.NextVisible(true))\n" +
            k_Tab + k_Tab + "{\n" +
            k_Tab + k_Tab + k_Tab + "if (iterator.hasVisibleChildren)\n" +
            k_Tab + k_Tab + k_Tab + k_Tab + "continue;\n" +
            "\n" +
            k_Tab + k_Tab + k_Tab + "driver.AddFromName<" + trackBinding.name + ">(trackBinding.gameObject, iterator.propertyPath);\n" +
            k_Tab + k_Tab + "}\n" +
            "#endif\n" +
            k_Tab + k_Tab + "base.GatherProperties (director, driver);\n" +
            k_Tab + "}\n" +
            "}\n";
    }
    
    string StandardBlendPlayableDrawer ()
    {
        return
            "using UnityEditor;\n" +
            "using UnityEngine;\n" +
            "using UnityEngine.Playables;\n" +
            AdditionalNamespacesToString() +
            "\n" +
            "[CustomPropertyDrawer(typeof(" + playableName + k_TimelineClipBehaviourSuffix + "))]\n" +
            "public class " + playableName + k_PropertyDrawerSuffix + " : PropertyDrawer\n" +
            "{\n" +
            k_Tab + "public override float GetPropertyHeight (SerializedProperty property, GUIContent label)\n" +
            k_Tab + "{\n" +
            k_Tab + k_Tab + "int fieldCount = " + standardBlendPlayableProperties.Count + ";\n" +
            k_Tab + k_Tab + "return fieldCount * EditorGUIUtility.singleLineHeight;\n" +
            k_Tab + "}\n" +
            "\n" +
            k_Tab + "public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)\n" +
            k_Tab + "{\n" +
            StandardBlendTrackBindingPropertiesAsSerializedPropsDeclarationToString () +
            "\n" +
            k_Tab + k_Tab + "Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);\n" +
            StandardBlendSerializedPropertyGUIToString () +
            k_Tab + "}\n" +
            "}\n";
    }
    
    string StandardBlendScriptPlayablePropertiesToString ()
    {
        string returnVal = "";
        for (int i = 0; i < standardBlendPlayableProperties.Count; i++)
        {
            UsableProperty prop = standardBlendPlayableProperties[i];
            if(prop.defaultValue == "")
                returnVal += k_Tab + "public " + prop.type + " " + prop.name + ";\n";
            else
            {
                returnVal += k_Tab + "public " + prop.type + " " + prop.name + " = " + prop.defaultValue + ";\n";
            }
        }
        return returnVal;
    }

    string StandardBlendTrackBindingPropertiesDefaultsDeclarationToString ()
    {
        string returnVal = "";
        for (int i = 0; i < standardBlendPlayableProperties.Count; i++)
        {
            returnVal += k_Tab + standardBlendPlayableProperties[i].type + " " + standardBlendPlayableProperties[i].NameAsPrivateDefault + ";\n";
        }
        return returnVal;
    }

    string StandardBlendTrackBindingPropertiesDefaultsAssignmentToString ()
    {
        string returnVal = "";
        for (int i = 0; i < standardBlendPlayableProperties.Count; i++)
        {
            returnVal += k_Tab + k_Tab + k_Tab + standardBlendPlayableProperties[i].NameAsPrivateDefault + " = m_TrackBinding." + standardBlendPlayableProperties[i].name + ";\n";
        }
        return returnVal;
    }

    string StandardBlendBlendedVariablesCreationToString ()
    {
        string returnVal = "";
        for (int i = 0; i < standardBlendPlayableProperties.Count; i++)
        {
            if(standardBlendPlayableProperties[i].usability != UsableProperty.Usability.Blendable)
                continue;

            UsableProperty prop = standardBlendPlayableProperties[i];
            string type = prop.type == "int" ? "float" : prop.type;
            string zeroVal = prop.type == "int" ? "0f" : prop.ZeroValueAsString ();
            returnVal += k_Tab + k_Tab + type + " " + prop.NameAsLocalBlended + " = " + zeroVal + ";\n";
        }
        return returnVal;
    }

    string StandardBlendBlendedVariablesWeightedIncrementationToString ()
    {
        string returnVal = "";
        for (int i = 0; i < standardBlendPlayableProperties.Count; i++)
        {
            if(standardBlendPlayableProperties[i].usability == UsableProperty.Usability.Blendable)
                returnVal += k_Tab + k_Tab + k_Tab + standardBlendPlayableProperties[i].NameAsLocalBlended + " += input." + standardBlendPlayableProperties[i].name + " * inputWeight;\n";
        }
        return returnVal;
    }

    string StandardBlendAssignableVariablesAssignedBasedOnGreatestWeightToString ()
    {
        if (standardBlendPlayableProperties.Count == 0)
            return "";

        string returnVal = k_Tab + k_Tab + k_Tab + "if (inputWeight > greatestWeight)\n";
        returnVal += k_Tab + k_Tab + k_Tab + "{\n";
        for (int i = 0; i < standardBlendPlayableProperties.Count; i++)
        {
            if (standardBlendPlayableProperties[i].usability == UsableProperty.Usability.Assignable)
                returnVal += k_Tab + k_Tab + k_Tab + k_Tab + "m_TrackBinding." + standardBlendPlayableProperties[i].name + " = input." + standardBlendPlayableProperties[i].name + ";\n";
        }
        returnVal += k_Tab + k_Tab + k_Tab + k_Tab + "greatestWeight = inputWeight;\n";
        returnVal += k_Tab + k_Tab + k_Tab + "}\n";
        return returnVal;
    }

    string StandardBlendTrackBindingPropertiesBlendedAssignmentToString ()
    {
        string returnVal = "";
        for (int i = 0; i < standardBlendPlayableProperties.Count; i++)
        {
            UsableProperty prop = standardBlendPlayableProperties[i];
            if (prop.usability != UsableProperty.Usability.Blendable)
                continue;

            if (prop.type == "int")
                returnVal += k_Tab + k_Tab + "m_TrackBinding." + prop.name + " = Mathf.RoundToInt (" + prop.NameAsLocalBlended + " + " + prop.NameAsPrivateDefault + " * (1f - totalWeight));\n";
            else
                returnVal += k_Tab + k_Tab + "m_TrackBinding." + prop.name + " = " + prop.NameAsLocalBlended + " + " + prop.NameAsPrivateDefault + " * (1f - totalWeight);\n";
        }
        return returnVal;
    }

    string StandardBlendTrackBindingPropertiesAssignableAssignmentToString ()
    {
        if (standardBlendPlayableProperties.Count == 0)
            return "";

        if (standardBlendPlayableProperties.Any (x => x.usability == UsableProperty.Usability.Assignable))
        {
            string returnVal = k_Tab + k_Tab + "if (currentInputs != 1 && 1f - totalWeight > greatestWeight)\n";
            returnVal += k_Tab + k_Tab + "{\n";
            for (int i = 0; i < standardBlendPlayableProperties.Count; i++)
            {
                if (standardBlendPlayableProperties[i].usability != UsableProperty.Usability.Assignable)
                    continue;

                returnVal += k_Tab + k_Tab + k_Tab + "m_TrackBinding." + standardBlendPlayableProperties[i].name + " = " + standardBlendPlayableProperties[i].NameAsPrivateDefault + ";\n";
            }
            returnVal += k_Tab + k_Tab + "}\n";
            return returnVal;
        }

        return "";
    }

    string StandardBlendTrackBindingPropertiesDefaultAssignmentToString ()
    {
        string returnVal = "";
        for (int i = 0; i < standardBlendPlayableProperties.Count; i++)
        {
            returnVal += k_Tab + k_Tab + "m_TrackBinding." + standardBlendPlayableProperties[i].name + " = " + standardBlendPlayableProperties[i].NameAsPrivateDefault + ";\n";
        }
        return returnVal;
    }

    string StandardBlendComponentBindingToString ()
    {
        return "[TrackBindingType(typeof(" + trackBinding.name + "))]\n";
    }

    string StandardBlendTrackBindingPropertiesAsSerializedPropsDeclarationToString ()
    {
        string returnVal = "";
        for (int i = 0; i < standardBlendPlayableProperties.Count; i++)
        {
            returnVal += k_Tab + k_Tab + "SerializedProperty " + standardBlendPlayableProperties[i].NameAsLocalSerializedProperty + " = property.FindPropertyRelative(\"" + standardBlendPlayableProperties[i].name + "\");\n";
        }
        return returnVal;
    }

    string StandardBlendSerializedPropertyGUIToString ()
    {
        string returnVal = "";
        for (int i = 0; i < standardBlendPlayableProperties.Count; i++)
        {
            if (i != 0)
            {
                returnVal += "\n";
                returnVal += k_Tab + k_Tab + "singleFieldRect.y += EditorGUIUtility.singleLineHeight;\n";
            }

            returnVal += k_Tab + k_Tab + "EditorGUI.PropertyField(singleFieldRect, " + standardBlendPlayableProperties[i].NameAsLocalSerializedProperty + ");\n";
        }
        return returnVal;
    }
}
