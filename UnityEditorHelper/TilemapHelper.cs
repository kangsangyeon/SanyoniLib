// https://gist.github.com/seobyeongky/d74e7bf2428a337771c1cbca1679f825

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[InitializeOnLoad]
public class TilemapHelper
{
    [MenuItem("Tilemap/SelectUpperTarget _PGUP")]
    public static void SelectUpperTarget()
    {
        if (Application.isPlaying)
            return;
        int idx = 0;
        var validTargets = ValidTargets();
        if (validTargets.Length == 0)
            return;
        var activeTarget = GetTarget();
        SortByHierarchyOrder(validTargets);
        for (int i = 0; i < validTargets.Length; i++)
        {
            if (validTargets[i] == activeTarget)
            {
                idx = i;
                break;
            }
        }
        idx--;
        if (idx < 0)
            idx = 0;

        SelectTarget(validTargets[idx]);
    }

    static void SortByHierarchyOrder(GameObject[] objs)
    {
        int maxDepth = 0;
        int[] sortOrder = new int[objs.Length];
        int[] depth = new int[objs.Length];
        int treeOrder = SceneManager.GetActiveScene().rootCount;

        for (int i = 0; i < objs.Length; i++)
        {
            var obj = objs[i];
            var p = obj.transform.parent;
            depth[i] = 0;
            while (p != null)
            {
                if (treeOrder < p.childCount)
                    treeOrder = p.childCount;
                p = p.parent;
                depth[i]++;
            }
            if (depth[i] > maxDepth)
                maxDepth = depth[i];
        }

        for (int i = 0; i < objs.Length; i++)
        {
            var obj = objs[i];
            var t = obj.transform;
            int base_ = Mathf.FloorToInt(Mathf.Pow(treeOrder, maxDepth - depth[i]));
            for (int j = 0; j <= depth[i]; j++)
            {
                int sibl = t.GetSiblingIndex();
                sortOrder[i] += base_ * sibl;
                t = t.parent;
                if (t == null)
                    break;
                base_ *= treeOrder;
            }
        }

        bool dirty = false;
        do
        {
            dirty = false;

            for (int i = 0; i < objs.Length - 1; i++)
            {
                if (sortOrder[i] > sortOrder[i + 1])
                {
                    var coke = sortOrder[i];
                    sortOrder[i] = sortOrder[i + 1];
                    sortOrder[i + 1] = coke;

                    var cola = objs[i];
                    objs[i] = objs[i + 1];
                    objs[i + 1] = cola;

                    dirty = true;
                }
            }
        }
        while (dirty);
    }

    [MenuItem("Tilemap/SelectLowerTarget _PGDN")]
    public static void SelectLowerTarget()
    {
        if (Application.isPlaying)
            return;
        int idx = 0;
        var validTargets = ValidTargets();
        if (validTargets.Length == 0)
            return;
        var activeTarget = GetTarget();
        SortByHierarchyOrder(validTargets);
        for (int i = 0; i < validTargets.Length; i++)
        {
            if (validTargets[i] == activeTarget)
            {
                idx = i;
                break;
            }
        }
        idx++;
        if (idx >= validTargets.Length)
            idx = validTargets.Length - 1;

        SelectTarget(validTargets[idx]);
    }

    [MenuItem("Tilemap/Highlight Selected Target _HOME")]
    public static void HighlightSelectedTarget()
    {
        if (Application.isPlaying)
            return;
        var target = GetTarget();
        if (target == null)
            return;

        var tilemap = target.GetComponent<Tilemap>();
        if (tilemap != null)
        {
            RunHighlightTilemapSeq(tilemap);
        }

        SelectAfterEffect(target);
    }

    public static EditorWindow GetFocusedWindow(string window)
    {
        FocusOnWindow(window);
        return EditorWindow.focusedWindow;
    }

    public static void FocusOnWindow(string window)
    {
        EditorApplication.ExecuteMenuItem("Window/" + window);
    }

    static List<GridBrushBase> Brushes()
    {
        var tyBrushes = GetType("UnityEditor.Tilemaps.GridPaletteBrushes");
        var brushes = (List<GridBrushBase>)tyBrushes.GetProperty("brushes").GetValue(null);
        return brushes;
    }

    static GridBrushBase GetBrush(string name)
    {
        var brushes = Brushes();
        return brushes.FirstOrDefault((x) => x.name == name);
    }

    static void SelectBrush(GridBrushBase brush)
    {
        var tyPaintingState = GetType("UnityEditor.Tilemaps.GridPaintingState");
        tyPaintingState.GetProperty("gridBrush").SetValue(null, brush);

        if (EditorTools.activeToolType.Namespace != "UnityEditor.Tilemaps")
        {
            // 덤으로 툴 선택까지 하기
            // 기본툴은 역시 브러쉬지
            EditorTools.SetActiveTool(GetType("UnityEditor.Tilemaps.PaintTool"));
        }
    }

    static GridBrushBase GetCurrentBrush()
    {
        var tyPaintingState = GetType("UnityEditor.Tilemaps.GridPaintingState");
        return (GridBrushBase)tyPaintingState.GetProperty("gridBrush").GetValue(null);
    }

    static GameObject[] ValidTargets()
    {
        var tyPaintingState = GetType("UnityEditor.Tilemaps.GridPaintingState");
        return (GameObject[])tyPaintingState.GetProperty("validTargets").GetValue(null);
    }

    static GameObject GetTarget()
    {
        var tyPaintingState = GetType("UnityEditor.Tilemaps.GridPaintingState");
        return (GameObject)tyPaintingState.GetProperty("scenePaintTarget").GetValue(null);
    }

    static void SelectTarget(GameObject target)
    {
        var tyPaintingState = GetType("UnityEditor.Tilemaps.GridPaintingState");
        tyPaintingState.GetProperty("scenePaintTarget").SetValue(null, target);

        SelectAfterEffect(target);
    }

    static void SelectAfterEffect(GameObject target)
    {
        EditorGUIUtility.PingObject(target);

        var tilemap = target.GetComponent<Tilemap>();
        if (tilemap != null)
        {
            RunHighlightTilemapSeq(tilemap);
        }
    }

    class HighlightContext
    {
        public Tilemap tilemap;
        public Color orgColor;
        public int orgLayer = -1;
        public float t = 0;
    }

    static List<HighlightContext> tilemapsHighlightRunning = new List<HighlightContext>();

    static void RunHighlightTilemapSeq(Tilemap tilemap)
    {
        foreach (var cx in tilemapsHighlightRunning)
        {
            if (cx.tilemap == tilemap)
            {
                cx.t = 0f;
                return;
            }
        }

        {
            var cx = new HighlightContext { tilemap = tilemap, orgColor = tilemap.color };
            var R = tilemap.GetComponent<TilemapRenderer>();
            if (R != null) cx.orgLayer = R.sortingLayerID;
            R.sortingLayerID = SortingLayer.layers[SortingLayer.layers.Length - 1].id;
            tilemapsHighlightRunning.Add(cx);
            if (tilemapsHighlightRunning.Count == 1)
            {
                EditorApplication.update += HighlightRunUpdate;
            }
        }
    }

    static void HighlightRunUpdate()
    {
        foreach (var cx in tilemapsHighlightRunning)
        {
            var orgColor = cx.orgColor;
            var color2 = new Color(Mathf.Round(1 - orgColor.r)
                            , Mathf.Round(1 - orgColor.g)
                            , Mathf.Round(1 - orgColor.b)
                            , 1);

            cx.t += Time.deltaTime;
            var p = cx.t / 0.5f;
            cx.tilemap.color = Color.Lerp(color2, orgColor, p);
        }

        for (int i = tilemapsHighlightRunning.Count - 1; i >= 0; i--)
        {
            var cx = tilemapsHighlightRunning[i];
            if (cx.t >= 0.5f)
            {
                cx.tilemap.color = cx.orgColor;
                var R = cx.tilemap.GetComponent<TilemapRenderer>();
                if (R != null)
                    R.sortingLayerID = cx.orgLayer;
                tilemapsHighlightRunning.RemoveAt(i);
            }
        }

        if (tilemapsHighlightRunning.Count == 0)
        {
            EditorApplication.update -= HighlightRunUpdate;
        }
    }

    private static Type GetType(string TypeName)
    {
        var type = Type.GetType(TypeName);
        if (type != null)
            return type;

        if (TypeName.Contains("."))
        {
            var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));
            var assembly = Assembly.Load(assemblyName);
            if (assembly == null)
                return null;
            type = assembly.GetType(TypeName);
            if (type != null)
                return type;
        }

        var currentAssembly = Assembly.GetExecutingAssembly();
        var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
        foreach (var assemblyName in referencedAssemblies)
        {
            var assembly = Assembly.Load(assemblyName);
            if (assembly != null)
            {
                type = assembly.GetType(TypeName);
                if (type != null)
                    return type;
            }
        }
        return null;
    }

    /////

    static TilemapHelper()
    {
        if (Application.isPlaying)
            return;

        SceneView.duringSceneGui += OnSceneGUI;
    }

    static Outliner outliner = new Outliner();


    static void OnSceneGUI(SceneView sceneView)
    {
        if (EditorTools.activeToolType.Namespace == "UnityEditor.Tilemaps")
        {
            var activeTarget = GetTarget();
            var tilemap = activeTarget.GetComponent<Tilemap>();
            if (tilemap != null)
            {
                outliner.UpdateGeometry(tilemap);
                outliner.DrawLines();
            }
        }
    }

    public static Tool CurrentTool
    {
        get { return (Tool)mTools_current.GetValue(null, null); }
        set { mTools_current.SetValue(null, (int)value, null); }
    }
    // "Sorry Virginia, there is no private."
    private static PropertyInfo mTools_current = typeof(Tools).GetProperty("current", BindingFlags.Static | BindingFlags.NonPublic);

    class Outliner
    {
        bool[] checkMap = new bool[1024 * 1024];
        Vector2Int bridge;
        Tilemap tilemap;
        BoundsInt cellBounds;
        int cachedHash;
        Queue<Vector2Int> que = new Queue<Vector2Int>();
        List<LineSegment> segments = new List<LineSegment>();
        Vector2Int[] DELTAS = new Vector2Int[] {
            new Vector2Int(1, 0)
            , new Vector2Int(-1, 0)
            , new Vector2Int(0, 1)
            , new Vector2Int(0, -1)
        };
        Vector2Int[] DELTAOS = new Vector2Int[] {
            new Vector2Int(1, 1)
            , new Vector2Int(0, 1)
            , new Vector2Int(1, 1)
            , new Vector2Int(0, 0)
        };
        Vector2Int[] DELTANS = new Vector2Int[] {
            new Vector2Int(0, 1)
            , new Vector2Int(0, 1)
            , new Vector2Int(1, 0)
            , new Vector2Int(-1, 0)
        };

        struct LineSegment
        {
            public Vector3 a;
            public Vector3 b;
        }

        public void UpdateGeometry(Tilemap tilemap)
        {
            int hash = 0;
            this.tilemap = tilemap;
            cellBounds = tilemap.cellBounds;

            for (int y = cellBounds.yMin; y <= cellBounds.yMax; y++)
            {
                for (int x = cellBounds.xMin; x <= cellBounds.xMax; x++)
                {
                    var pos = new Vector3Int(x, y, 0);
                    if (tilemap.HasTile(pos))
                    {
                        hash += (x + 9) * (y + 17);
                    }
                }
            }

            if (hash != cachedHash)
            {
                RecacheGeometry(tilemap);
                cachedHash = hash;
            }
        }

        public void DrawLines()
        {
            var c = Color.yellow;
            c.a = 0.5f;
            Handles.color = c;

            foreach (var seg in segments)
                Handles.DrawLine(seg.a, seg.b);
        }

        void RecacheGeometry(Tilemap tilemap)
        {
            bridge = new Vector2Int(512 - cellBounds.xMin - cellBounds.size.x / 2
                , 512 - cellBounds.yMin - cellBounds.size.y / 2);
            segments.Clear();

            for (int y = cellBounds.yMin; y <= cellBounds.yMax; y++)
            {
                for (int x = cellBounds.xMin; x <= cellBounds.xMax; x++)
                {
                    SetCheck(x, y, false);
                }
            }

            for (int y = cellBounds.yMin; y <= cellBounds.yMax; y++)
            {
                for (int x = cellBounds.xMin; x <= cellBounds.xMax; x++)
                {
                    var pos = new Vector3Int(x, y, 0);
                    if (tilemap.HasTile(pos)
                        && !HasChecked(x, y))
                    {
                        BFS(x, y);
                    }
                }
            }
        }

        void BFS(int x, int y)
        {
            que.Enqueue(new Vector2Int(x, y));

            while (que.Count > 0)
            {
                var now = que.Dequeue();
                for (int i = 0; i < DELTAS.Length; i++)
                {
                    var next = now + DELTAS[i];
                    if (!tilemap.HasTile(new Vector3Int(next.x, next.y, 0)))
                    {
                        var a = now + DELTAOS[i];
                        /*
                        if (DELTAS[i].x > 0)
                            a.x++;
                        if (DELTAS[i].y > 0)
                            a.y++;
                            */
                        var b = a - DELTANS[i];
                        LineSegment seg = new LineSegment();
                        seg.a = tilemap.CellToWorld(new Vector3Int(a.x, a.y, 0));
                        seg.b = tilemap.CellToWorld(new Vector3Int(b.x, b.y, 0));
                        segments.Add(seg);
                    }
                    else if (!HasChecked(next.x, next.y))
                    {
                        SetCheck(next.x, next.y, true);
                        que.Enqueue(next);
                    }
                }
            }
        }

        bool HasChecked(int x, int y)
        {
            var bx = x + bridge.x;
            var by = y + bridge.y;
            return checkMap[bx + by * cellBounds.size.x];
        }

        void SetCheck(int x, int y, bool set)
        {
            var bx = x + bridge.x;
            var by = y + bridge.y;
            checkMap[bx + by * cellBounds.size.x] = set;
        }
    }
}