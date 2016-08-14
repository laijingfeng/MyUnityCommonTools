using UnityEditor;
using System;
using System.Reflection;
using UnityEditor.Animations;

public class FixMask
{
    [MenuItem("Assets/Fix Animation Masks")]
    private static void Init()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        foreach (UnityEngine.Object obj in selection)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            ModelImporter mi = AssetImporter.GetAtPath(path) as ModelImporter;

            Type modelImporterType = typeof(ModelImporter);

            MethodInfo updateTransformMaskMethodInfo = modelImporterType.GetMethod("UpdateTransformMask", BindingFlags.NonPublic | BindingFlags.Static);

            ModelImporterClipAnimation[] clipAnimations = mi.clipAnimations;
            SerializedObject so = new SerializedObject(mi);
            SerializedProperty clips = so.FindProperty("m_ClipAnimations");

            AvatarMask avatarMask = new AvatarMask();
            avatarMask.transformCount = mi.transformPaths.Length;
            for (int i = 0; i < mi.transformPaths.Length; i++)
            {
                avatarMask.SetTransformPath(i, mi.transformPaths[i]);
                avatarMask.SetTransformActive(i, true);
            }

            for (int i = 0; i < clipAnimations.Length; i++)
            {
                SerializedProperty transformMaskProperty = clips.GetArrayElementAtIndex(i).FindPropertyRelative("transformMask");
                updateTransformMaskMethodInfo.Invoke(mi, new System.Object[] { avatarMask, transformMaskProperty });
            }

            so.ApplyModifiedProperties();

            AssetDatabase.ImportAsset(path);
        }
    }
}