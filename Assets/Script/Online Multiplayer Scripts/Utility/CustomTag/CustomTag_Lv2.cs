using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTag_Lv2 : MonoBehaviour
{
    [SerializeField]
    public List<string> tags = new List<string>();
    
    public bool HasTag(string tag)
    {
        return tags.Contains(tag);
    }
    
    public IEnumerable<string> GetTags()
    {
        return tags;
    }
    
    public void Rename(int index, string tagName)
    {
        tags[index] = tagName;
    }
    
    public string GetAtIndex(int index)
    {
        return tags[index];
    }
    
    public int Count
    {
        get { return tags.Count; }
    }
}
