using UnityEditor;
using UnityEngine;

public class BetterBuilderGUI : EditorWindow
{
    [MenuItem("Tools/Better Builder")]
    public static void ShowWindow()
    {
        GetWindow<BetterBuilderGUI>("Better Builder");
    }
    void OnEnable()
    {
        BetterBuilder.GetCurrentProjectPath();
        BetterBuilder.LoadExcludedScenes();
    }
    void OnDisable()
    {
        BetterBuilder.SaveExcludedScenes();
    }
    void OnGUI()
    {
        DragAndDropField();
        GUILayout.Label("Excluded scenes: ", EditorStyles.boldLabel);

        // Excluded scenes list

        for (int i = 0; i < BetterBuilder.excludedScenes.Count; i++)
        {
            ExcludedSceneListElement(i);
        }
        if(GUILayout.Button("Add all scenes to build"))
        {
            BetterBuilder.AddProjectScenesToBuild();
        }
    }

    private void DragAndDropField()
    {
        Event guiEvent = Event.current;
        Rect area = GUILayoutUtility.GetRect(0f, 50f, GUILayout.ExpandWidth(true));

        // Background color
        Color originalColor = GUI.color;
        GUI.color = new Color (0,0,0,0.5f);
        GUI.DrawTexture(area, Texture2D.whiteTexture);
        GUI.color = originalColor;

        // Center text
        GUIStyle textCenter = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter
        };

        // Text
        GUI.Label(area,"Drag & drop scenes here to exclude them from build", textCenter);

        // Handling drag and drop events
        switch (guiEvent.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if(!area.Contains(guiEvent.mousePosition))
                {
                    return;
                }
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (guiEvent.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        if(draggedObject is SceneAsset)
                        {
                            if(!BetterBuilder.excludedScenes.Contains(draggedObject))
                            {
                                BetterBuilder.excludedScenes.Add(draggedObject);
                            }
                        }
                        else
                        {
                           Debug.LogError($"{draggedObject.name} is not a scene");
                        }
                    }
                }
                break;
        }
    }
    void ExcludedSceneListElement(int sceneIndex)
    {
        EditorGUILayout.BeginHorizontal();
        if(BetterBuilder.excludedScenes[sceneIndex] != null)
        {
            GUILayout.Label(BetterBuilder.excludedScenes[sceneIndex].name);
        }

        if(GUILayout.Button("Remove", GUILayout.Width(60)))
        {
            BetterBuilder.excludedScenes.RemoveAt(sceneIndex);
        }
        EditorGUILayout.EndHorizontal();
    } 
}