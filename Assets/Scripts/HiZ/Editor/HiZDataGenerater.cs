using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HiZ.Editor
{
    public class HiZDataGenerator : EditorWindow
    {
        private List<GameObject> collectRoots = new List<GameObject>();
        private bool convertTerrainTree = true;
        private bool convertTerrainDetails = true;

        [MenuItem("Tools/HiZ")]
        public static void ShowWindow()
        {
            GetWindow<HiZDataGenerator>("HiZ Data Generator");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("HiZ Data Generator", EditorStyles.boldLabel);

            // 收集根节点
            EditorGUILayout.LabelField("Collect Roots:");
            EditorGUI.indentLevel++;
            for (int i = 0; i < collectRoots.Count; i++)
            {
                collectRoots[i] = (GameObject)EditorGUILayout.ObjectField(
                    collectRoots[i], typeof(GameObject), true);
            }
            if (GUILayout.Button("Add Root"))
            {
                collectRoots.Add(null);
            }
            EditorGUI.indentLevel--;

            // 地形处理选项
            convertTerrainTree = EditorGUILayout.Toggle("Convert Terrain Trees", convertTerrainTree);
            convertTerrainDetails = EditorGUILayout.Toggle("Convert Terrain Details", convertTerrainDetails);

            // 操作按钮
            if (GUILayout.Button("Generate HiZ Data", GUILayout.Height(30)))
            {
                GenerateHiZData();
            }
        }

        private void GenerateHiZData()
        {
            // 1. 场景备份与恢复（保持与原版一致）
            Scene originalScene = SceneManager.GetActiveScene();
            string originalPath = originalScene.path;
            EditorSceneManager.MarkSceneDirty(originalScene);
            EditorSceneManager.SaveScene(originalScene);

            string hizScenePath = originalPath.Replace(".unity", "_hiz.unity");
            EditorSceneManager.SaveScene(originalScene, hizScenePath);
            Scene hizScene = EditorSceneManager.OpenScene(hizScenePath, OpenSceneMode.Single);

            // 2. 地形数据处理（需根据实际需求实现）
            ProcessTerrainData();

            // 3. 收集Prefab数据（示例框架）
            Dictionary<string, DrawParams> allDraws = CollectPrefabData();

            // 4. 保存Hi-Z数据（需自定义序列化）
            string dataPath = hizScenePath.Replace(".unity", "_data.asset");
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<HiZRunTime.HiZData>(), dataPath);

            // 5. 清理与恢复
            EditorSceneManager.OpenScene(originalPath, OpenSceneMode.Single);
            AssetDatabase.Refresh();
        }

        private void ProcessTerrainData()
        {
            // 示例地形处理框架（需根据原版逻辑重构）
            Terrain terrain = FindObjectOfType<Terrain>();
            if (terrain != null)
            {
                // 1. 复制地形资产
                // 2. 处理碰撞器
                // 3. 处理树木/细节（需根据原版逻辑实现）
            }
        }

        private Dictionary<string, DrawParams> CollectPrefabData()
        {
            Dictionary<string, DrawParams> draws = new Dictionary<string, DrawParams>();
            
            // 示例收集框架（需根据原版逻辑重构）
            foreach (GameObject root in collectRoots)
            {
                MeshRenderer[] renderers = root.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer mr in renderers)
                {
                    Material[] materials = mr.sharedMaterials;
                    MeshFilter mf = mr.GetComponent<MeshFilter>();
                    
                    if (mf != null && mf.sharedMesh != null)
                    {
                        string key = GenerateDrawKey(mf, materials);
                        if (!draws.ContainsKey(key))
                        {
                            draws[key] = new DrawParams();
                        }
                        
                        // 计算包围盒和变换矩阵
                        Bounds bounds = mr.bounds;
                        Matrix4x4 matrix = mr.transform.localToWorldMatrix;
                        
                        // 添加ClusterData和InstanceData
                        draws[key].AddClusterData(bounds, matrix);
                    }
                }
            }

            return draws;
        }

        private string GenerateDrawKey(MeshFilter mf, Material[] materials)
        {
            string meshPath = AssetDatabase.GetAssetPath(mf.sharedMesh);
            string materialPath = materials.Length > 0 
                ? AssetDatabase.GetAssetPath(materials[0]) 
                : "None";
            
            return $"{meshPath}_{materials.Length}";
        }

        // 自定义数据类（需保持与原版一致）
        [Serializable]
        public class DrawParams
        {
            public string meshPath;
            public Material material;
            public List<ClusterData> clusters = new List<ClusterData>();
            
            public void AddClusterData(Bounds bounds, Matrix4x4 matrix)
            {
                clusters.Add(new ClusterData(bounds, matrix));
            }
        }

        [Serializable]
        public class ClusterData
        {
            public Bounds bounds;
            public Matrix4x4 matrix;
            
            public ClusterData(Bounds bounds, Matrix4x4 matrix)
            {
                this.bounds = bounds;
                this.matrix = matrix;
            }
        }
    }
}