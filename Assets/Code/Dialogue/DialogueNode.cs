using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Dialogue.Editor{
    [System.Serializable]
    public class DialogueNode
    {
        public string uniqueID;
        public string text;
        public string[] children;
        public Rect rect;

    }
}