using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using UnityEditor.AI;
using UnityEditor.SceneManagement;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;

[CustomEditor(typeof(BakeNavMesh))]
public class BakeNavMeshEditor : Editor {

    public override void OnInspectorGUI(){

        BakeNavMesh BakeNavMesh = (BakeNavMesh)target;

        // NavMeshSurface[] navMeshes = BakeNavMesh.gameObject.GetComponents<NavMeshSurface>();
        BakeNavMesh.NavMeshes = BakeNavMesh.gameObject.GetComponents<NavMeshSurface>();

        // NavMeshAssetManager.instance.

        if(GUILayout.Button("Bake all NavMeshSurface")){
            if(BakeNavMesh.NavMeshes.Length == 0){
                Debug.LogError("No NavMeshSurface to bake!");
                return;
            }
            NavMeshAssetManager.instance.StartBakingSurfaces(BakeNavMesh.NavMeshes);
            Debug.Log("Path bake successfuly");
        }

        if(GUILayout.Button("Clear all NavMeshSurface path")){
            if(BakeNavMesh.NavMeshes.Length == 0){
                Debug.LogError("No NavMeshSurface to clear!");
                return;
            }
            NavMeshAssetManager.instance.ClearSurfaces(BakeNavMesh.NavMeshes);
            SceneView.RepaintAll();
            Debug.Log("Path clear successfuly");
        }

    }
    
}
