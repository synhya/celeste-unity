using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class LUTConverterWindow : EditorWindow
{
    const string WINDOW_TITLE = "LUTConverterWindow";
    const string MENU_ITEM = "Tools/" + WINDOW_TITLE;

    private Sprite targetSprite;
    private List<PairColor> colorTable;

    private static Vector2 windowMinSize = Vector2.one * 500f;
    private static Rect listRect = new Rect(Vector2.zero, windowMinSize);
        
    private ReorderableList reorderList;
    private LUTData colorData;

    private HashSet<Color> spriteColorSet;
    private int lutTextureWidth = -1;
    private string assetPath;
    
    [SerializeField] private Vector2 scrollPos = Vector2.zero;

    [MenuItem("Tools/LUTConverterWindow")]
    public static void Open()
    {
        var window = GetWindow<LUTConverterWindow>(false, WINDOW_TITLE, true);
        window.Show();
    }

    [Serializable]
    public struct PairColor
    {
        public Color from;
        public Color to;

        public PairColor(Color from, Color to)
        {
            this.from = from;
            this.to = to;
        }
    }

    private void OnEnable()
    {
        colorTable ??= new List<PairColor>();
        
        reorderList = new ReorderableList(colorTable, typeof(PairColor), true, true, true, true)
        {
            drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Colors"),
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += 2f;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.width /= 2f;
                var nextR = rect;
                nextR.x += rect.width;
                var from = EditorGUI.ColorField(rect, ((PairColor)reorderList.list[index]).from);
                var to = EditorGUI.ColorField(nextR, ((PairColor)reorderList.list[index]).to);
                PairColor elem = new PairColor(from, to);

                colorTable[index] = elem;
            },
        };
        
        colorData = Resources.Load<LUTData>("LUTDataSettings");
        if (colorData && colorTable.Count == 0)
        {
            var prevTable = colorData.Table;
            colorTable.AddRange(prevTable);
        }
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        GUILayout.Label("Settings", EditorStyles.label);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Texture", GUILayout.MaxWidth(125));
        targetSprite = (Sprite)EditorGUILayout.ObjectField(targetSprite, typeof(Sprite), false);
        GUILayout.EndHorizontal();

        if (colorTable != null)
        {
            var t = listRect;
            t.y += EditorGUIUtility.singleLineHeight * 2;
            reorderList.DoLayoutList();
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("convert"))
        {
            Convert();
        }
        if (GUILayout.Button("generate table from sprite"))
        {
            GenerateTable();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndScrollView();
    }
    private void GenerateTable()
    {
        if(targetSprite == null) return;
        
        colorTable.Clear();

        spriteColorSet ??= new HashSet<Color>();
        spriteColorSet.Clear();

        var from = targetSprite.texture.GetPixels();
        foreach (var t in from)
        {
            var elem = t;
            if (elem.a == 0)
            {
                // remove different colors with alpha 0.
                elem = Color.clear;
            }
            spriteColorSet.Add(elem);
        }
        
        var total = spriteColorSet.Count;
        lutTextureWidth = Mathf.CeilToInt(Mathf.Sqrt(total));
        var fromColors = spriteColorSet.ToList(); 

        for (int i = 0; i < lutTextureWidth; i++)
        {
            for (int j = 0; j < lutTextureWidth; j++)
            {
                int curIdx = i * lutTextureWidth + j;
                if (curIdx < total)
                {
                    // as 16bit texture has no alpha
                    // blue is used as alpha. red, green is for uv -> red is j green is i
                    Color toColor = new Color(j * 1f / lutTextureWidth,i * 1f / lutTextureWidth,fromColors[curIdx].a,1);
                    
                    colorTable.Add(new PairColor(fromColors[curIdx], toColor));
                }
            }
        }
    }

    void Convert()
    {
        if(lutTextureWidth < 0) return;
        
        Dictionary<Color, Color> colorDict = new Dictionary<Color, Color>();
        // Convert Original Sprite to Map
        {
            foreach (var pairColor in colorTable)
            {
                colorDict.Add(pairColor.from, pairColor.to);
            }
        
            if (targetSprite)
            {
                var tex = targetSprite.texture;
                var keys = tex.GetPixels();
                var to = new Color[keys.Length];
                for(int i = 0; i < keys.Length; i++)
                {
                    if (colorDict.TryGetValue(keys[i], out var val)) // out of index
                    {
                        to[i] = val; //val;
                    }
                }
        
                tex.SetPixels(to);
                tex.Apply();
                assetPath = Application.dataPath.Split("Assets")[0] + AssetDatabase.GetAssetPath(tex);
                Debug.Log("saved as: " + assetPath);
                System.IO.File.WriteAllBytes(assetPath, tex.EncodeToPNG());
            }
        }
        // Generate Lookup Texture based on Table
        {
            var list = colorTable.Select(pc => pc.from).ToList();
            var total = list.Count;
            
            // make texture
            Texture2D tex = new Texture2D(lutTextureWidth,lutTextureWidth, TextureFormat.RGBA32,false);
            
            for (int i = 0; i < lutTextureWidth; i++)
            {
                for (int j = 0; j < lutTextureWidth; j++)
                {
                    int curIdx = i * lutTextureWidth + j;
                    if (curIdx < total)
                    {
                        tex.SetPixel(j, i, list[curIdx]);
                    }
                }
            }
            
            tex.Apply();
            string path = assetPath.Split(".")[0] + "_LUT.png";
            Debug.Log("saved as: " + path);
            System.IO.File.WriteAllBytes(path, tex.EncodeToPNG());
        }
    }
    
    private void OnDisable()
    {
        if (colorData)
        {
            var saveTable = colorData.Table;
            saveTable.Clear();
            saveTable.AddRange(colorTable);
        }
    }
}
