using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������Դ�嵥
/// </summary>
public class BuildinFileManifest : ScriptableObject
{
    [Serializable]
    public class Element
    {
        public string PackageName;
        public string FileName;
    }

    public List<Element> BuildinFiles = new List<Element>();
}