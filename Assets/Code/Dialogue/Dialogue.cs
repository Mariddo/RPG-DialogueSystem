using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue.Editor{
    [CreateAssetMenu(fileName="Dialogue", menuName = "RPG/Dialogue", order = 0)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] 
        List<DialogueNode> nodes;

        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
        private void Awake()
        {
            if(nodes.Count == 0)
            {
                nodes.Add(new DialogueNode());
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

    private void OnValidate(){
        nodeLookup.Clear();
        foreach (DialogueNode node in GetAllNodes())
        {
            nodeLookup[node.uniqueID] = node;

        }

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

}



}