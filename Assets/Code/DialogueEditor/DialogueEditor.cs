using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace RPG.Dialogue.Editor{

public class DialogueEditor : EditorWindow
{
    Dialogue selectedDialogue = null;
    GUIStyle nodeStyle;

    DialogueNode draggingNode = null;

    Vector2 draggingOffset; 

    DialogueNode creatingNode;


    [MenuItem("Window/Dialogue Editor")]
    public static void ShowEditorWindow()
    {
        GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");

    }

    [OnOpenAsset(1)]
    public static bool OpenDialogue(int instanceID, int line)
    {

        Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;

        if(dialogue != null)
        {

            ShowEditorWindow();
            return true;
        }

        return false;
    }

    private void OnGUI()
    {
        if(selectedDialogue == null)
        {
            EditorGUILayout.LabelField("No Dialogue Selected.");

        }
        else
        {
            ProcessEvents();

            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                DrawConnections(node);
            }
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                OnGUINode(node);
            }
            if (creatingNode != null)
            {
                Undo.RecordObject(selectedDialogue, "Added new Child Node");
                selectedDialogue.CreateNode(creatingNode);
                creatingNode = null;
            }

        }
        
    }

    private void ProcessEvents()
    {
        if(Event.current.type == EventType.MouseDown && draggingNode == null)
        {
            draggingNode = GetNodeAtPoint(Event.current.mousePosition);

            if(draggingNode != null)
            {
                draggingOffset = draggingNode.rect.position - Event.current.mousePosition;
            }

        }
        else if(Event.current.type == EventType.MouseDrag && draggingNode != null)
        {
            Undo.RecordObject(selectedDialogue, "Drag Root Node");
            draggingNode.rect.position = Event.current.mousePosition + draggingOffset;
            GUI.changed = true;
        }
        else if(Event.current.type == EventType.MouseUp && draggingNode != null)
        {
            draggingNode = null;
        }

    }

    private void OnGUINode(DialogueNode node)
    {
        GUILayout.BeginArea(node.rect, nodeStyle);
        
        EditorGUI.BeginChangeCheck();
        
        EditorGUILayout.LabelField("Node: ");
        string newText = EditorGUILayout.TextField(node.text);

        string newUniqueID = EditorGUILayout.TextField(node.uniqueID);

        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(selectedDialogue, "Update Dialogue Text");

            node.text = newText;
            node.uniqueID = newUniqueID;
            
        }

        if(GUILayout.Button("+"))
        {
            creatingNode = node;
        }

        foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
        {
            EditorGUILayout.LabelField(childNode.text);
        }

        GUILayout.EndArea();

    }


    private void OnEnable(){
        Selection.selectionChanged += OnSelectionChanged;

        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
        nodeStyle.padding = new RectOffset(20,20,20,20);
        nodeStyle.border = new RectOffset(12,12,12,12);
    }

    private void OnSelectionChanged()
    {

        Dialogue newDialogue = Selection.activeObject as Dialogue;
        if (newDialogue != null)
        {
            selectedDialogue = newDialogue;
            Repaint();
        }
    }

    private DialogueNode GetNodeAtPoint(Vector2 point)
    {
        DialogueNode dn = null;
        
        foreach (DialogueNode node in selectedDialogue.GetAllNodes())
        {
            if(node.rect.Contains(point))
            {
                dn = node;
            }
        }

        return dn;
    }

    private void DrawConnections(DialogueNode node)
    {
        //Get the start position
        Vector3 startPosition = new Vector2(node.rect.xMax, node.rect.center.y);

        //Draw a curve for each node.
        foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
        {
            EditorGUILayout.LabelField(childNode.text);
            Vector3 endPosition = new Vector2(childNode.rect.xMin, childNode.rect.center.y);
            Vector3 controlPointOffset = endPosition - startPosition;
            controlPointOffset.y = 0;
            controlPointOffset.x *= 0.8f;
            Handles.DrawBezier(
                startPosition, endPosition, 
                startPosition + controlPointOffset, 
                endPosition - controlPointOffset, 
                Color.white, null, 4f);
        }


    }



}

}
