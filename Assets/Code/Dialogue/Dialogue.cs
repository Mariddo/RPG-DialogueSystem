using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue.Editor{
    [CreateAssetMenu(fileName="Dialogue", menuName = "RPG/Dialogue", order = 0)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] 
        List<DialogueNode> nodes = new List<DialogueNode>();

        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
        private void Awake()
        {
            if(nodes.Count == 0)
            {
                DialogueNode rootNode = new DialogueNode();
                rootNode.uniqueID = Guid.NewGuid().ToString();
                nodes.Add(rootNode);
            }

        }
        #endif


    public IEnumerable<DialogueNode> GetAllNodes()
    {

        return nodes;
    }

    public DialogueNode GetRootNode()
    {
        return nodes[0];
    }



    public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
    {
        foreach (string childID in parentNode.children)
        {
            if (nodeLookup.ContainsKey(childID))
            {
                yield return nodeLookup[childID];
            }
        }
    }

    public void CreateNode(DialogueNode parent)
    {
        
        DialogueNode newNode = new DialogueNode();
        newNode.uniqueID = Guid.NewGuid().ToString();
        parent.children.Add(newNode.uniqueID);
        nodes.Add(newNode);
        Validate();
    }

    public void Validate(){
        nodeLookup.Clear();
        foreach (DialogueNode node in GetAllNodes())
        {
            nodeLookup[node.uniqueID] = node;

        }

    }

    public void DeleteNode(DialogueNode nodeToDelete)
    {
        nodes.Remove(nodeToDelete);
        Validate();


        foreach (DialogueNode node in GetAllNodes())
        {
            node.children.Remove(nodeToDelete.uniqueID);
        }
    }

}



}