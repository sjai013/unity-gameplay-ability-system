using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TimeMachineClip))]
public class TimeMachineClipInspector : Editor
{
	private SerializedProperty actionProp, conditionProp;

	private void OnEnable()
	{
		actionProp = serializedObject.FindProperty("action");
		conditionProp = serializedObject.FindProperty("condition");
	}

	public override void OnInspectorGUI()
	{
		bool isMarker = false; //if it's a marker we don't need to draw any Condition or parameters

		//Action
		EditorGUILayout.PropertyField(actionProp);

		//change the int into an enum
		int index = actionProp.enumValueIndex;
		TimeMachineBehaviour.TimeMachineAction actionType = (TimeMachineBehaviour.TimeMachineAction)index;

		//Draws only the appropriate information based on the Action Type
		switch(actionType)
		{
			case TimeMachineBehaviour.TimeMachineAction.Marker:
				isMarker = true;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("markerLabel"));
				break;

			case TimeMachineBehaviour.TimeMachineAction.JumpToMarker:
				EditorGUILayout.PropertyField(serializedObject.FindProperty("markerToJumpTo"));
				break;
			
			case TimeMachineBehaviour.TimeMachineAction.JumpToTime:
				EditorGUILayout.PropertyField(serializedObject.FindProperty("timeToJumpTo"));
				break;
		}


		if(!isMarker)
		{
			//Condition
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Logic", EditorStyles.boldLabel);
			
			//change the int into an enum
			index = conditionProp.enumValueIndex;
			TimeMachineBehaviour.Condition conditionType = (TimeMachineBehaviour.Condition)index;
			
			//Draws only the appropriate information based on the Condition type
			switch(conditionType)
			{
				case TimeMachineBehaviour.Condition.Always:
					EditorGUILayout.HelpBox("The above action will always be executed.", MessageType.Warning);
					EditorGUILayout.PropertyField(conditionProp);
					break;
				
				case TimeMachineBehaviour.Condition.Never:
					EditorGUILayout.HelpBox("The above action will never be executed. Practically, it's as if clip was disabled.", MessageType.Warning);
					EditorGUILayout.PropertyField(conditionProp);
					break;
				
			}
		}

		serializedObject.ApplyModifiedProperties();
	}
}
