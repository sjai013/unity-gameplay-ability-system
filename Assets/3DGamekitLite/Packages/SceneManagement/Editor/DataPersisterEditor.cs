using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gamekit3D
{
    public abstract class DataPersisterEditor : Editor
    {
        IDataPersister m_DataPersister;

        protected virtual void OnEnable()
        {
            m_DataPersister = (IDataPersister)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            DataPersisterGUI(m_DataPersister);
        }

        public static void DataPersisterGUI(IDataPersister dataPersister)
        {
            DataSettings dataSettings = dataPersister.GetDataSettings();

            DataSettings.PersistenceType persistenceType = (DataSettings.PersistenceType)EditorGUILayout.EnumPopup("Persistence Type", dataSettings.persistenceType);
            string dataTag = EditorGUILayout.TextField("Data Tag", dataSettings.dataTag);

            dataPersister.SetDataSettings(dataTag, persistenceType);
        }
    }
}