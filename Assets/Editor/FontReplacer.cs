using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class FontReplacer : EditorWindow
{
    Font newFont;

    [MenuItem("Tools/Font Replacer")]
    public static void ShowWindow()
    {
        GetWindow<FontReplacer>("Font Replacer");
    }

    void OnGUI()
    {
        GUILayout.Label("Replace all UI.Text fonts", EditorStyles.boldLabel);
        newFont = (Font)EditorGUILayout.ObjectField("New Font", newFont, typeof(Font), false);

        if (newFont == null)
        {
            EditorGUILayout.HelpBox("Assign a Font asset first.", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("Replace in Active Scenes"))
            ReplaceInOpenScenes();

        if (GUILayout.Button("Replace in All Prefabs"))
            ReplaceInPrefabs();
    }

    void ReplaceInOpenScenes()
    {
        // Start a single undo group for all scene changes
        int group = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Replace Font + Scale Size in Scenes");

        // Loop through all open scenes
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (!scene.isLoaded) continue;

            // Find all Text in this scene
            var allTexts = new List<Text>(Resources.FindObjectsOfTypeAll<Text>());
            var sceneTexts = allTexts.FindAll(t => t.gameObject.scene == scene);

            if (sceneTexts.Count == 0) continue;

            // Record undo for all these Text components
            Undo.RecordObjects(sceneTexts.ToArray(), "Replace Font + Scale Size");

            // Apply swap & resize
            foreach (var t in sceneTexts)
            {
                t.font = newFont;
                t.fontSize = Mathf.RoundToInt(t.fontSize * 0.85f);
                EditorUtility.SetDirty(t);
            }

            EditorSceneManager.MarkSceneDirty(scene);
        }

        // Collapse undo operations into that single group
        Undo.CollapseUndoOperations(group);

        EditorUtility.DisplayDialog("Font Replacer", "Replaced fonts & resized in open scenes.\nUse Undo to revert.", "OK");
    }

    void ReplaceInPrefabs()
    {
        // Start a single undo group for all prefab changes
        int group = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Replace Font + Scale Size in Prefabs");

        var prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        int count = 0;

        foreach (var guid in prefabGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var texts = prefab.GetComponentsInChildren<Text>(true);

            if (texts.Length == 0) continue;

            // Record undo on the prefab asset itself
            Undo.RegisterCompleteObjectUndo(prefab, "Replace Font + Scale Size");

            foreach (var t in texts)
            {
                t.font = newFont;
                t.fontSize = Mathf.RoundToInt(t.fontSize * 0.85f);
            }

            EditorUtility.SetDirty(prefab);
            count++;
        }

        AssetDatabase.SaveAssets();
        Undo.CollapseUndoOperations(group);

        EditorUtility.DisplayDialog("Font Replacer", $"Replaced fonts & resized in {count} prefabs.\nUse Undo to revert.", "OK");
    }
}
