#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Net;
using System.Collections.Generic;

[CustomEditor(typeof(PathCreator), true)]
[InitializeOnLoad]
public class PathEditor : Editor
{ 
    public PathCreator pathTarget
    {
        get
        {
            return (PathCreator)target;
        }
    }
     

    protected virtual void OnSceneGUI()
    {
        Handles.color = Color.green;
        PathCreator t = (target as PathCreator);
  
        if (t.originalTransformPositionStatus == false)
        {
            return;
        } 

       
            ShowMousePos();

        GUIStyle style = new GUIStyle();
 

        for (int i = 0; i < t.List_Points.Count; i++)
        {
            EditorGUI.BeginChangeCheck();

            Vector3 oldPoint = t.originalTransformPosition + t.List_Points[i];
   

            style.normal.textColor =  Color.yellow;
            Handles.Label(t.originalTransformPosition + t.List_Points[i] + (Vector3.down * 0.4f) + (Vector3.right * 0.4f), "" + i, style);

            Vector3 newPoint = Handles.FreeMoveHandle(oldPoint, Quaternion.identity, .5f, new Vector3(.25f, .25f, .25f), Handles.CircleHandleCap);
 

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Free Move Handle");
                t.List_Points[i] = newPoint - t.originalTransformPosition;
            }

           
        }
    }

    void ShowMousePos()
    {

        if (!Event.current.control)
            return;
        PathCreator t = (target as PathCreator);

        Vector2 mousePosition = Event.current.mousePosition;
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        mousePosition = ray.origin;

        GUIStyle style = new GUIStyle();
         
        float nearestPoint = 1000;
        int indexNearestPoint = 0;
        Vector2 nearestPos = Vector3.zero;
         
        for (int i = 0; i < t.List_Points.Count-1; i++)
        {
            Vector2 mouseProject = HandleUtility.ProjectPointLine(mousePosition, t.originalTransformPosition + t.List_Points[i], t.originalTransformPosition + t.List_Points[i + 1]);
           
            if (nearestPoint > Vector2.Distance(mousePosition, mouseProject))
            {
                nearestPoint = Vector2.Distance(mousePosition, mouseProject);
                indexNearestPoint = i+1;
                nearestPos = mouseProject;
            } 
        }

        style.normal.textColor = Color.red;
        
        HandleUtility.Repaint();

        EditorGUI.BeginChangeCheck();
         
        Handles.DrawWireCube(nearestPos,  new Vector2(1.5f,1.5f) );
   
        if (Event.current.type == EventType.MouseDown  )
        {
            Undo.RecordObject(target, "Insert another point");
            t.List_Points.Insert(indexNearestPoint, mousePosition - (Vector2)t.originalTransformPosition); 
        } 
       
        EditorGUI.EndChangeCheck();
        
    }



}

#endif