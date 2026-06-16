using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Bfree
{
    public class TilemapObjectPlacer : EditorWindow
    {
        private Grid grid;
        private GameObject prefabToPlace;
        private bool isPlacing = false;
        private bool showGridGizmos = true;
        private bool showCoordinates = true;
        private bool showPlacedCount = true;
        private bool showPrefabPreview = true; // NEW
        private Color gridColor = new Color(0, 1, 0, 0.3f);
        private Color hoverColor = new Color(1, 1, 0, 0.5f);
        private Color placedColor = new Color(0, 0.5f, 1, 0.4f);
        private Vector3 pivotOffset = Vector3.zero;

        // Track placed objects
        private Dictionary<Vector3Int, GameObject> placedObjects = new Dictionary<Vector3Int, GameObject>();
        private Vector3Int currentHoveredCell;
        private bool isHovering = false;

        // Parent organization
        private string parentObjectName = "PlacedObjects";
        private Transform parentTransform;

        // Multi-prefab support
        private List<GameObject> prefabPalette = new List<GameObject>();
        private int selectedPrefabIndex = 0;
        private Vector2 scrollPosition;

        // Preview
        private const float PREVIEW_SIZE = 64f; // NEW

        [MenuItem("Tools/Tilemap Object Placer")]
        public static void ShowWindow()
        {
            GetWindow<TilemapObjectPlacer>("Object Placer");
        }

        void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUILayout.Label("Tilemap Object Placement", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Grid Setup
            EditorGUILayout.LabelField("Grid Setup", EditorStyles.boldLabel);
            grid = (Grid)EditorGUILayout.ObjectField("Grid", grid, typeof(Grid), true);

            if (grid == null)
            {
                EditorGUILayout.HelpBox("Please assign a Grid to begin placing objects.", MessageType.Warning);
            }

            EditorGUILayout.Space();

            // Prefab Selection
            EditorGUILayout.LabelField("Prefab Selection", EditorStyles.boldLabel);

            // Single prefab mode (legacy support)
            EditorGUILayout.BeginHorizontal();
            prefabToPlace = (GameObject)EditorGUILayout.ObjectField("Single Prefab", prefabToPlace, typeof(GameObject), false);

            // NEW: Show preview for single prefab
            if (prefabToPlace != null)
            {
                Sprite sprite = GetPrefabSprite(prefabToPlace);
                if (sprite != null)
                {
                    Rect previewRect = GUILayoutUtility.GetRect(PREVIEW_SIZE, PREVIEW_SIZE, GUILayout.Width(PREVIEW_SIZE));
                    DrawSpritePreview(sprite, previewRect);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Or use Prefab Palette:", EditorStyles.miniLabel);

            // NEW: Preview toggle
            showPrefabPreview = EditorGUILayout.Toggle("Show Prefab Previews", showPrefabPreview);

            EditorGUILayout.Space();

            // Prefab palette
            for (int i = 0; i < prefabPalette.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                bool isSelected = (i == selectedPrefabIndex);
                if (GUILayout.Toggle(isSelected, $"[{i}]", GUILayout.Width(40)))
                {
                    selectedPrefabIndex = i;
                    prefabToPlace = prefabPalette[i];
                }

                prefabPalette[i] = (GameObject)EditorGUILayout.ObjectField(
                    prefabPalette[i], typeof(GameObject), false);

                // NEW: Show preview in palette
                if (showPrefabPreview && prefabPalette[i] != null)
                {
                    Sprite sprite = GetPrefabSprite(prefabPalette[i]);
                    if (sprite != null)
                    {
                        Rect previewRect = GUILayoutUtility.GetRect(PREVIEW_SIZE, PREVIEW_SIZE, GUILayout.Width(PREVIEW_SIZE));

                        // Highlight selected prefab
                        if (isSelected)
                        {
                            EditorGUI.DrawRect(new Rect(previewRect.x - 2, previewRect.y - 2,
                                previewRect.width + 4, previewRect.height + 4), Color.yellow);
                        }

                        DrawSpritePreview(sprite, previewRect);
                    }
                }

                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    prefabPalette.RemoveAt(i);
                    if (selectedPrefabIndex >= prefabPalette.Count)
                        selectedPrefabIndex = Mathf.Max(0, prefabPalette.Count - 1);
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+ Add Prefab Slot"))
            {
                prefabPalette.Add(null);
            }

            // NEW: Large preview of currently selected prefab
            EditorGUILayout.Space();
            if (prefabToPlace != null)
            {
                EditorGUILayout.LabelField("Selected Prefab Preview", EditorStyles.boldLabel);
                Sprite sprite = GetPrefabSprite(prefabToPlace);
                if (sprite != null)
                {
                    float largePreviewSize = 128f;
                    Rect largePreviewRect = GUILayoutUtility.GetRect(largePreviewSize, largePreviewSize);
                    largePreviewRect.x = (EditorGUIUtility.currentViewWidth - largePreviewSize) / 2f; // Center it

                    // Background
                    EditorGUI.DrawRect(largePreviewRect, new Color(0.2f, 0.2f, 0.2f, 1f));

                    DrawSpritePreview(sprite, largePreviewRect);

                    // Prefab name below preview
                    GUIStyle centeredStyle = new GUIStyle(EditorStyles.label);
                    centeredStyle.alignment = TextAnchor.MiddleCenter;
                    centeredStyle.fontStyle = FontStyle.Bold;
                    EditorGUILayout.LabelField(prefabToPlace.name, centeredStyle);
                }
                else
                {
                    EditorGUILayout.HelpBox("No SpriteRenderer found in prefab root", MessageType.Info);
                }
            }

            EditorGUILayout.Space();

            // Pivot Offset
            EditorGUILayout.LabelField("Placement Settings", EditorStyles.boldLabel);
            pivotOffset = EditorGUILayout.Vector3Field("Pivot Offset", pivotOffset);
            parentObjectName = EditorGUILayout.TextField("Parent Object Name", parentObjectName);

            EditorGUILayout.Space();

            // Gizmo Settings
            GUILayout.Label("Gizmo Settings", EditorStyles.boldLabel);
            showGridGizmos = EditorGUILayout.Toggle("Show Grid Gizmos", showGridGizmos);
            showCoordinates = EditorGUILayout.Toggle("Show Coordinates", showCoordinates);
            showPlacedCount = EditorGUILayout.Toggle("Show Placed Indicator", showPlacedCount);

            EditorGUI.BeginDisabledGroup(!showGridGizmos);
            gridColor = EditorGUILayout.ColorField("Grid Color", gridColor);
            hoverColor = EditorGUILayout.ColorField("Hover Color", hoverColor);
            placedColor = EditorGUILayout.ColorField("Placed Color", placedColor);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            // Place Mode Toggle
            Color originalColor = GUI.backgroundColor;
            GUI.backgroundColor = isPlacing ? Color.green : originalColor;
            isPlacing = GUILayout.Toggle(isPlacing, isPlacing ? "✓ Place Mode Active" : "Place Mode (Click to Enable)",
                "Button", GUILayout.Height(30));
            GUI.backgroundColor = originalColor;

            if (isPlacing && prefabToPlace == null && (prefabPalette.Count == 0 || prefabPalette.All(p => p == null)))
            {
                EditorGUILayout.HelpBox("Place Mode is active but no prefab is assigned!", MessageType.Warning);
            }

            EditorGUILayout.Space();

            // Controls Info
            EditorGUILayout.HelpBox(
                "Controls:\n" +
                "• Shift + Click: Place object\n" +
                "• Ctrl + Shift + Click: Remove object\n" +
                "• Number Keys (0-9): Quick-select prefab from palette\n" +
                "• Hold Alt: Temporarily disable placement",
                MessageType.Info);

            EditorGUILayout.Space();

            // Actions
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh Placed Objects"))
            {
                RefreshPlacedObjects();
            }

            if (GUILayout.Button("Find/Create Parent"))
            {
                FindOrCreateParent();
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Clear All Placed Objects", GUILayout.Height(25)))
            {
                ClearPlacedObjects();
            }

            EditorGUILayout.Space();

            // Stats
            EditorGUILayout.LabelField("Statistics", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Placed Objects: {placedObjects.Count}");
            if (grid != null)
            {
                EditorGUILayout.LabelField($"Cell Size: {grid.cellSize}");
            }

            EditorGUILayout.EndScrollView();
        }

        // NEW: Get sprite from prefab
        private Sprite GetPrefabSprite(GameObject prefab)
        {
            if (prefab == null) return null;

            // Try to get SpriteRenderer from root
            SpriteRenderer spriteRenderer = prefab.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                return spriteRenderer.sprite;
            }

            // Try to get from first child
            spriteRenderer = prefab.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                return spriteRenderer.sprite;
            }

            return null;
        }

        // NEW: Draw sprite preview with proper aspect ratio
        private void DrawSpritePreview(Sprite sprite, Rect rect)
        {
            if (sprite == null) return;

            Texture2D texture = sprite.texture;
            Rect spriteRect = sprite.rect;

            // Calculate aspect ratio
            float spriteAspect = spriteRect.width / spriteRect.height;
            float rectAspect = rect.width / rect.height;

            Rect finalRect = rect;

            // Fit sprite to rect while maintaining aspect ratio
            if (spriteAspect > rectAspect)
            {
                // Sprite is wider
                float newHeight = rect.width / spriteAspect;
                finalRect.y += (rect.height - newHeight) / 2f;
                finalRect.height = newHeight;
            }
            else
            {
                // Sprite is taller
                float newWidth = rect.height * spriteAspect;
                finalRect.x += (rect.width - newWidth) / 2f;
                finalRect.width = newWidth;
            }

            // Convert sprite rect to UV coordinates
            Rect uvRect = new Rect(
                spriteRect.x / texture.width,
                spriteRect.y / texture.height,
                spriteRect.width / texture.width,
                spriteRect.height / texture.height
            );

            // Draw the sprite
            GUI.DrawTextureWithTexCoords(finalRect, texture, uvRect);

            // Optional: Draw border around preview
            Handles.BeginGUI();
            Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
            Handles.DrawSolidRectangleWithOutline(rect, Color.clear, new Color(0.3f, 0.3f, 0.3f, 1f));
            Handles.EndGUI();
        }

        void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            RefreshPlacedObjects();

            // Initialize with single prefab if palette is empty
            if (prefabPalette.Count == 0 && prefabToPlace != null)
            {
                prefabPalette.Add(prefabToPlace);
            }
        }

        void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        void OnSceneGUI(SceneView sceneView)
        {
            if (grid == null) return;

            Event e = Event.current;

            // Number key shortcuts for prefab selection (0-9)
            if (e.type == EventType.KeyDown && e.keyCode >= KeyCode.Alpha0 && e.keyCode <= KeyCode.Alpha9)
            {
                int index = e.keyCode - KeyCode.Alpha0;
                if (index < prefabPalette.Count && prefabPalette[index] != null)
                {
                    selectedPrefabIndex = index;
                    prefabToPlace = prefabPalette[index];
                    Repaint();
                    e.Use();
                }
            }

            // Always update hover position when mouse moves
            if (e.type == EventType.MouseMove || e.type == EventType.MouseDrag)
            {
                Vector3 mousePos = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;
                mousePos.z = 0;

                currentHoveredCell = grid.WorldToCell(mousePos);
                isHovering = true;
                SceneView.RepaintAll();
            }

            // Draw all gizmos if enabled
            if (showGridGizmos)
            {
                DrawPlacedObjectGizmos();

                if (isHovering)
                {
                    DrawHoverGizmo();
                }
            }

            // Placement logic (disabled if Alt is held)
            if (!isPlacing || prefabToPlace == null || e.alt) return;

            if (e.shift && e.type == EventType.MouseDown && e.button == 0 && !e.control)
            {
                PlaceObject();
                e.Use();
            }

            // Remove object with Ctrl+Shift+Click
            if (e.shift && e.control && e.type == EventType.MouseDown && e.button == 0)
            {
                RemoveObject();
                e.Use();
            }
        }

        void DrawHoverGizmo()
        {
            if (grid == null) return;

            Vector3 snappedPos = grid.GetCellCenterWorld(currentHoveredCell);

            // Check if cell is occupied
            bool isOccupied = placedObjects.ContainsKey(currentHoveredCell);
            Color currentHoverColor = isOccupied ? new Color(1f, 0.3f, 0.3f, 0.5f) : hoverColor;

            // Draw filled rectangle for hover
            Handles.color = currentHoverColor;
            Handles.DrawSolidRectangleWithOutline(
                GetCellCorners(snappedPos),
                currentHoverColor,
                isOccupied ? Color.red : Color.yellow
            );

            // Draw coordinates
            if (showCoordinates)
            {
                GUIStyle labelStyle = new GUIStyle()
                {
                    normal = new GUIStyleState() { textColor = Color.white },
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };

                Handles.Label(snappedPos + Vector3.up * 0.3f,
                    $"X: {currentHoveredCell.x}, Y: {currentHoveredCell.y}",
                    labelStyle);

                // Show what will be placed
                if (prefabToPlace != null && !isOccupied)
                {
                    labelStyle.normal.textColor = Color.green;
                    Handles.Label(snappedPos + Vector3.down * 0.3f,
                        $"Place: {prefabToPlace.name}",
                        labelStyle);
                }
                else if (isOccupied)
                {
                    labelStyle.normal.textColor = Color.red;
                    GameObject existing = placedObjects[currentHoveredCell];
                    Handles.Label(snappedPos + Vector3.down * 0.3f,
                        existing != null ? $"Occupied: {existing.name}" : "Occupied",
                        labelStyle);
                }
            }

            // Draw preview of prefab at offset position
            if (prefabToPlace != null && !isOccupied && pivotOffset != Vector3.zero)
            {
                Vector3 finalPos = snappedPos + pivotOffset;
                Handles.color = Color.cyan;
                Handles.DrawWireCube(finalPos, Vector3.one * 0.1f);
                Handles.DrawDottedLine(snappedPos, finalPos, 2f);
            }
        }

        void DrawPlacedObjectGizmos()
        {
            if (grid == null) return;

            foreach (var kvp in placedObjects)
            {
                if (kvp.Value == null)
                {
                    continue;
                }

                Vector3 cellCenter = grid.GetCellCenterWorld(kvp.Key);

                // Draw grid outline for placed objects
                Handles.color = placedColor;
                Handles.DrawSolidRectangleWithOutline(
                    GetCellCorners(cellCenter),
                    new Color(placedColor.r, placedColor.g, placedColor.b, 0.15f),
                    placedColor
                );

                // Draw small dot at center
                if (showPlacedCount)
                {
                    Handles.color = Color.cyan;
                    Handles.DrawSolidDisc(cellCenter, Vector3.forward, 0.05f);
                }
            }
        }

        Vector3[] GetCellCorners(Vector3 cellCenter)
        {
            Vector3 halfSize = grid.cellSize / 2f;
            return new Vector3[]
            {
            cellCenter + new Vector3(-halfSize.x, -halfSize.y, 0),
            cellCenter + new Vector3(halfSize.x, -halfSize.y, 0),
            cellCenter + new Vector3(halfSize.x, halfSize.y, 0),
            cellCenter + new Vector3(-halfSize.x, halfSize.y, 0)
            };
        }

        void PlaceObject()
        {
            Vector3 snappedPos = grid.GetCellCenterWorld(currentHoveredCell) + pivotOffset;

            // Check if already occupied
            if (placedObjects.ContainsKey(currentHoveredCell))
            {
                Debug.LogWarning($"Cell {currentHoveredCell} already occupied!");
                return;
            }

            // Find or create parent
            FindOrCreateParent();

            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefabToPlace);
            obj.transform.position = snappedPos;
            obj.transform.parent = parentTransform;
            Undo.RegisterCreatedObjectUndo(obj, "Place Object");

            placedObjects[currentHoveredCell] = obj;
            Debug.Log($"Placed {prefabToPlace.name} at {currentHoveredCell}");
        }

        void RemoveObject()
        {
            if (placedObjects.ContainsKey(currentHoveredCell))
            {
                GameObject obj = placedObjects[currentHoveredCell];
                Undo.DestroyObjectImmediate(obj);
                placedObjects.Remove(currentHoveredCell);
                Debug.Log($"Removed object at {currentHoveredCell}");
            }
            else
            {
                Debug.LogWarning($"No object at {currentHoveredCell} to remove");
            }
        }

        void FindOrCreateParent()
        {
            if (grid == null) return;

            parentTransform = grid.transform.Find(parentObjectName);

            if (parentTransform == null)
            {
                GameObject parent = new GameObject(parentObjectName);
                parent.transform.parent = grid.transform;
                parent.transform.localPosition = Vector3.zero;
                parentTransform = parent.transform;
                Undo.RegisterCreatedObjectUndo(parent, "Create Parent Object");
                Debug.Log($"Created parent object: {parentObjectName}");
            }
        }

        void RefreshPlacedObjects()
        {
            placedObjects.Clear();

            if (grid == null)
            {
                Debug.LogWarning("Cannot refresh: Grid is null");
                return;
            }

            // Find parent
            FindOrCreateParent();

            if (parentTransform != null)
            {
                foreach (Transform child in parentTransform)
                {
                    Vector3Int cellPos = grid.WorldToCell(child.position - pivotOffset);

                    // Handle duplicates
                    if (placedObjects.ContainsKey(cellPos))
                    {
                        Debug.LogWarning($"Duplicate object found at {cellPos}: {child.name}. Keeping first occurrence.");
                        continue;
                    }

                    placedObjects[cellPos] = child.gameObject;
                }
            }

            Debug.Log($"Refreshed: Found {placedObjects.Count} placed objects");
        }

        void ClearPlacedObjects()
        {
            if (placedObjects.Count == 0)
            {
                Debug.Log("No objects to clear");
                return;
            }

            if (EditorUtility.DisplayDialog("Clear All Objects",
                $"Are you sure you want to delete {placedObjects.Count} placed objects?",
                "Yes", "Cancel"))
            {
                List<GameObject> objectsToDelete = new List<GameObject>(placedObjects.Values);

                foreach (var obj in objectsToDelete)
                {
                    if (obj != null)
                    {
                        Undo.DestroyObjectImmediate(obj);
                    }
                }
                placedObjects.Clear();
                Debug.Log("Cleared all placed objects");
            }
        }
    }
}