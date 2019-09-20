using UnityEngine;
using System.Collections;

namespace DynamicFogAndMist {
				public class DemoFoWPlane : MonoBehaviour {

								public Terrain terrain;

								GUIStyle labelStyle;
								DynamicFogOfWar fow;

								void Start () {
												fow = DynamicFogOfWar.instance;

												// Smooth terrain borders crossing fog
												fow.SetFogOfWarTerrainBoundary(terrain, 10f);
								}

								void OnGUI () {
												if (labelStyle == null) {
																labelStyle = new GUIStyle (GUI.skin.label);
																labelStyle.normal.textColor = Color.white;
																labelStyle.fontSize = 20;
												}

												Rect rect = new Rect (10, 10, Screen.width - 20, 30);
												GUI.Label (rect, "Click and drag to clear fog at that position!", labelStyle);
								}

								void OnMouseDrag() {
												Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
												RaycastHit hit;
												if (Physics.Raycast(ray, out hit)) {
																fow.SetFogOfWarAlpha(hit.point, 8, 0f);
												}
								}
				}
}