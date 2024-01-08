#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
namespace EditorTool
{
    /// <summary>
    /// 用于更改纹理的尺寸
    /// 操作方式：拖拽到指定位置，然后进行操作
    /// </summary>
    public class MenuItem_TextureEx : EditorWindow
    {
        private string tipName = "需要4化的文件夹路径/文件";

        private List<string> _listPaths = new List<string>();

        private bool IsToBig = true; //向着更大缩放
        private bool IsFloder = false;


        //背景资源宽高4倍处理
        [MenuItem("Tools/背景4倍化处理", priority = 101)]
        private static void TextureEx()
        {
            EditorWindow.GetWindowWithRect<MenuItem_TextureEx>(new Rect(Screen.width / 2, Screen.height / 2, 500, 500));
        }


        void OnGUI()
        {
            //实现拖拽
            Rect drawRect = EditorGUILayout.BeginHorizontal();
            GUILayout.Box(tipName, GUILayout.MinHeight(40), GUILayout.MinWidth(500));
            EditorGUILayout.EndHorizontal();
            for (int i = 0; i < _listPaths.Count; i++)
            {
                GUILayout.Label(_listPaths[i]);
            }

            UnityEngine.Event currentEvent = UnityEngine.Event.current;
            //拖拽范围内
            if (drawRect.Contains(currentEvent.mousePosition))
            {
                switch (currentEvent.type)
                {
                    case EventType.DragUpdated:
                        DragAndDrop.visualMode = DragAndDropVisualMode.Generic; //到达目标区域的显示方式
                        break;
                    case EventType.DragPerform:
                        string[] paths = DragAndDrop.paths;
                        for (int i = 0; i < paths.Length; i++)
                        {
                            if (!_listPaths.Contains(paths[i]))
                            {
                                _listPaths.Add(paths[i]);
                            }
                        }

                        break;
                }
            }


            GUILayout.BeginHorizontal();

            IsToBig = GUILayout.Toggle(IsToBig, "向着更大4化处理");
            IsFloder = GUILayout.Toggle(IsFloder, "文件夹(true) 文件(false)");

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            if (GUILayout.Button("清除"))
            {
                _listPaths.Clear();
            }

            if (GUILayout.Button("执行"))
            {
                if (IsFloder)
                    ExFloder();
                else
                    ExFile();
            }
            GUILayout.EndHorizontal();
        }

        private void ExFile()
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("处理过路径：");

                for (int i = 0; i < _listPaths.Count; i++)
                {
                    EditorUtility.DisplayProgressBar("开始处理", $"{i + 1}/{_listPaths.Count}", (float)i / _listPaths.Count);
                    string path = _listPaths[i];
                    ExTex(path);

                    builder.AppendLine($"{path}");
                }

                Debug.Log(builder.ToString());
            }
            catch (Exception e)
            {
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private void ExFloder()
        {
            try
            {
                string[] guids = AssetDatabase.FindAssets("t:texture ", _listPaths.ToArray());

                StringBuilder builder = new StringBuilder();
                builder.AppendLine("处理过路径：");

                for (int i = 0; i < guids.Length; i++)
                {
                    EditorUtility.DisplayProgressBar("开始处理", $"{i + 1}/{guids.Length}", (float)i / guids.Length);
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    ExTex(path);

                    builder.AppendLine($"{path}");
                }

                Debug.Log(builder.ToString());
            }
            catch (Exception e)
            {
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }


        public void ExTex(string path)
        {
            if (AssetDatabase.LoadMainAssetAtPath(path) is Texture2D tex)
            {
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                //更改图片属性，可读，否则无法获取Pixel
                importer.isReadable = true;
                importer.SaveAndReimport();

                if (tex.width % 4 == 0 && tex.height % 4 == 0)
                {
                    return;
                }

                Vector2Int v2 = GetFourSize(tex.width, tex.height);
                var texCopy = new Texture2D(v2.x, v2.y);
                //从原来图像上根据现在的大小计算像素点
                for (int h = 0; h < v2.y; h++)
                {
                    for (int w = 0; w < v2.x; w++)
                    {
                        var pixel = tex.GetPixelBilinear(w / (v2.x * 1.0f), h / (v2.y * 1.0f));

                        /*if (info.IsContain(i, j))
                        {
                            pixel.a = 0;
                        }*/

                        texCopy.SetPixel(w, h, pixel);
                    }
                }

                texCopy.Apply();
                File.WriteAllBytes(path, texCopy.EncodeToPNG());
                //恢复不可读
                importer.isReadable = false;
                importer.SaveAndReimport();
                AssetDatabase.Refresh();
                _listPaths.Clear();
            }
        }

        /// <summary>
        /// 目标尺寸，宽高整数4处理
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Vector2Int GetFourSize(int width, int height)
        {
            if (IsToBig)
            {
                while (width % 4 != 0)
                {
                    width++;
                }

                while (height % 4 != 0)
                {
                    height++;
                }
            }
            else
            {
                while (width % 4 != 0)
                {
                    width--;
                }

                while (height % 4 != 0)
                {
                    height--;
                }
            }

            return new Vector2Int(Mathf.Max(4, width), Mathf.Max(4, height));
        }
    }
}
#endif
