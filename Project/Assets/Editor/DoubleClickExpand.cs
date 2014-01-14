using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
 
[InitializeOnLoad]
public static class DoubleClickExpand
{
    static Assembly unityEditorAssembly;
 
    static Type objectBrowserType;
    static Type treeViewType;
    static Type treeViewDataType;
 
    static MethodInfo isFolderMethod;
    static MethodInfo findNodeByIDMethod;
    static MethodInfo isExpandableMethod;
    static MethodInfo isExpandedMethod;
    static MethodInfo setExpandedWithChildrenMethod;
    static MethodInfo setExpandedMethod;
    static MethodInfo getInstanceIDFromGUIDMethod;
 
    static DoubleClickExpand()
    {
        Init();
    }
  public  static void Init()
    {
        bool result = (unityEditorAssembly = Assembly.GetAssembly(typeof(Editor))) != null;
        if (result)
        {
            result &= (objectBrowserType = unityEditorAssembly.GetType("UnityEditor.ObjectBrowser")) != null;
            result &= (treeViewType = unityEditorAssembly.GetType("UnityEditor.TreeView")) != null;
            result &= (treeViewDataType = unityEditorAssembly.GetType("UnityEditor.ITreeViewDataSource")) != null;
            if (result)
            {
                result &= (isFolderMethod = objectBrowserType.GetMethod("IsFolder", BindingFlags.Static | BindingFlags.Public)) != null;
                result &= (findNodeByIDMethod = treeViewDataType.GetMethod("FindNodeByID", BindingFlags.Instance | BindingFlags.Public)) != null;
                result &= (isExpandableMethod = treeViewDataType.GetMethod("IsExpandable", BindingFlags.Instance | BindingFlags.Public)) != null;
                result &= (isExpandedMethod = treeViewDataType.GetMethod("IsExpanded", BindingFlags.Instance | BindingFlags.Public)) != null;
                result &= (setExpandedWithChildrenMethod = treeViewDataType.GetMethod("SetExpandedWithChildren", BindingFlags.Instance | BindingFlags.Public)) != null;
                result &= (setExpandedMethod = treeViewDataType.GetMethod("SetExpanded", BindingFlags.Instance | BindingFlags.Public)) != null;
                result &= (getInstanceIDFromGUIDMethod = typeof(AssetDatabase).GetMethod("GetInstanceIDFromGUID", BindingFlags.Static | BindingFlags.NonPublic)) != null;
                //Debug.Log("result:" + result);
                if (result)
                {
                    EditorApplication.projectWindowItemOnGUI += ProjectWindowItem_OnGUI;
                }
            }
        }
    }
 
    public static object GetFieldValue(object obj, Type type, string fieldName, BindingFlags flags)
    {
        FieldInfo field = type.GetField(fieldName, flags);
        return field != null ? field.GetValue(obj) : null;
    }
 
    static void ProjectWindowItem_OnGUI(string pGUID, Rect pDrawingRect)
    {
        if (Event.current.type == EventType.MouseDown
            && Event.current.clickCount == 2
            && pDrawingRect.Contains(Event.current.mousePosition))
        {
            object objectBrowser = GetFieldValue(null, objectBrowserType, "s_LastInteractedObjectBrowser", BindingFlags.Static | BindingFlags.Public);
            if (objectBrowser != null)
            {
                object assetTree = GetFieldValue(objectBrowser, objectBrowserType, "m_AssetTree", BindingFlags.Instance | BindingFlags.NonPublic);
                object folderTree = GetFieldValue(objectBrowser, objectBrowserType, "m_FolderTree", BindingFlags.Instance | BindingFlags.NonPublic);
                object treeData = null;
 
                if (assetTree != null)
                    treeData = GetFieldValue(assetTree, treeViewType, "m_Data", BindingFlags.Instance | BindingFlags.Public);
                else if (folderTree != null)
                    treeData = GetFieldValue(folderTree, treeViewType, "m_Data", BindingFlags.Instance | BindingFlags.Public);
 
                if (treeData != null)
                {
                    int instanceID = (int)getInstanceIDFromGUIDMethod.Invoke(null, new object[] { pGUID });
                    bool isFolder = (bool)isFolderMethod.Invoke(null, new object[] { instanceID });
                    if (isFolder)
                    {
                        object node = findNodeByIDMethod.Invoke(treeData, new object[] { instanceID });
                        if (node != null)
                        {
                            bool isExpandable = (bool)isExpandableMethod.Invoke(treeData, new object[] { node });
                            bool isExpanded = (bool)isExpandedMethod.Invoke(treeData, new object[] { node });
                            Debug.Log("isExpandable:" + isExpandable);
                            if (isExpandable)
                            {
                                if (Event.current.alt)
                                {
                                    if (isExpanded)
                                        setExpandedWithChildrenMethod.Invoke(treeData, new object[] { node, false });
                                    else
                                        setExpandedWithChildrenMethod.Invoke(treeData, new object[] { node, true });
                                }
                                else
                                {
                                    if (isExpanded)
                                        setExpandedMethod.Invoke(treeData, new object[] { node, false });
                                    else
                                        setExpandedMethod.Invoke(treeData, new object[] { node, true });
                                }
                            }
                            else
                            {
                                //itemPath = itemPath.Replace(@"/", @"\");   // explorer doesn't like front slashes
                                //System.Diagnostics.Process.Start("explorer.exe", "/select,"+itemPath);
                            }
 
                            //Event.current.Use();
                        }
                    }
                }
            }
        }
    }
}