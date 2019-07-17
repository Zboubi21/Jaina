using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using UnityEditor.AI;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(BakeNavMesh))]
public class BakeNavMeshEditor : Editor {

    public override void OnInspectorGUI(){

        BakeNavMesh BakeNavMesh = (BakeNavMesh)target;

        NavMeshSurface[] navMeshes = BakeNavMesh.gameObject.GetComponents<NavMeshSurface>();

        if(GUILayout.Button("Bake all NavMeshSurface")){
            if(navMeshes.Length == 0){
                Debug.LogError("No NavMeshSurface to bake!");
                return;
            }
            NavMeshAssetManager.instance.StartBakingSurfaces(navMeshes);
        }

        if(GUILayout.Button("Clear all NavMeshSurface path")){
            if(navMeshes.Length == 0){
                Debug.LogError("No NavMeshSurface to clear!");
                return;
            }
            NavMeshAssetManager.instance.ClearSurfaces(navMeshes);
            SceneView.RepaintAll();
            Debug.Log("Path clear successfuly");
        }

    }
    
}
