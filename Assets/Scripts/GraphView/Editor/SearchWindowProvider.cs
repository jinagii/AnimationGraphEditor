using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEditor.Experimental.GraphView;

public class SearchWindowProvider : ScriptableObject, ISearchWindowProvider
{
    private AnimGraphView _graphView;

    public void Initialize(AnimGraphView graphView)
    {
        this._graphView = graphView;        
    }

    List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
    {
        var entries = new List<SearchTreeEntry>();
        entries.Add(new SearchTreeGroupEntry(new GUIContent("Create Node")));

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsClass && !type.IsAbstract && (type.IsSubclassOf(typeof(TempBaseNode)))
                    && type != typeof(PlayOutputNode) && type != typeof(BaseNode))
                {
                    entries.Add(new SearchTreeEntry(new GUIContent(type.Name)) { level = 1, userData = type });
                }
            }
        }
        
        return entries;
    }

    bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        var type = searchTreeEntry.userData as System.Type;
        var node = Activator.CreateInstance(type) as TempBaseNode;
        node._playableGraph = _graphView._playableGraph; // PG 세팅
        _graphView.AddElement(node);
        
        return true;
    }
}