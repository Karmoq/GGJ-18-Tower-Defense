using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CableCurve))]
public class CableEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CableCurve _cableScript = (CableCurve)target;
        if (GUILayout.Button("GenerateCable"))
        {
            _cableScript.GenerateCurve();
        }

        if (_cableScript.GetLockState())
        {

            if (GUILayout.Button("Unlock mid & end position"))
            {
                _cableScript.LockPositions(false);
            }
        }
        else
        {
            if (GUILayout.Button("Lock mid & end position"))
            {
                _cableScript.LockPositions(true);
            }
        }


    }
}
