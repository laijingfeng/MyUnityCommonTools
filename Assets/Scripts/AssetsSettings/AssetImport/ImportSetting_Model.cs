using UnityEngine;
using UnityEditor;
using System.Collections;

public class ImportSetting_Model : ImportSetting_Base
{
    private enum DrawType
    {
        Rig,
        Model,
        Animations,
    }

    private ModelImporter m_CurImpoter;
    private DrawType m_DrawType;

    public static ImportSetting_Model CreateImportSetting_Model(string name)
    {
        ImportSetting_Model set = ImportSetting_Model.CreateInstance<ImportSetting_Model>();

        set.Init(name);

        return set;
    }

    public override void Draw()
    {
        base.Draw();

        DrawBtn();

        switch (m_DrawType)
        {
            case DrawType.Rig:
                {
                    DrawRig();
                }
                break;
            case DrawType.Model:
                {
                    DrawModel();
                }
                break;
            case DrawType.Animations:
                {
                    DrawAnimatios();
                }
                break;
        }
    }

    private void DrawBtn()
    {
        EditorGUILayout.BeginHorizontal();

        if (m_DrawType == DrawType.Model)
        {
            GUI.color = Color.green;
        }
        else
        {
            GUI.color = Color.white;
        }

        if (GUILayout.Button("Model"))
        {
            m_DrawType = DrawType.Model;
        }

        if (m_DrawType == DrawType.Rig)
        {
            GUI.color = Color.green;
        }
        else
        {
            GUI.color = Color.white;
        }

        if (GUILayout.Button("Rig"))
        {
            m_DrawType = DrawType.Rig;
        }

        if (m_DrawType == DrawType.Animations)
        {
            GUI.color = Color.green;
        }
        else
        {
            GUI.color = Color.white;
        }

        if (GUILayout.Button("Animations"))
        {
            m_DrawType = DrawType.Animations;
        }

        GUI.color = Color.white;

        EditorGUILayout.EndHorizontal();
    }

    #region Rig

    public ModelImporterAnimationType m_animationType;
    
    private void InitRig()
    {
        m_animationType = ModelImporterAnimationType.None;
    }

    private void ApplyRig()
    {
        m_CurImpoter.animationType = m_animationType;
    }

    private void DrawRig()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Animation Type");
        m_animationType = (ModelImporterAnimationType)EditorGUILayout.EnumPopup(m_animationType);
        EditorGUILayout.EndHorizontal();
    }

    #endregion Rig

    #region Model

    public float m_globalScale;
    public ModelImporterMeshCompression m_meshCompression;
    public bool m_isReadable;
    public bool m_optimizeMesh;
    public bool m_importBlendShapes;
    public bool m_importMaterials;
    public ModelImporterMaterialName m_materialName;
    public ModelImporterMaterialSearch m_materialSearch;

    private void InitModel()
    {
        m_globalScale = 1f;
        m_meshCompression = ModelImporterMeshCompression.Off;
        m_isReadable = true;
        m_optimizeMesh = true;
        m_importBlendShapes = true;
        //...

        m_importMaterials = true;
        m_materialName = ModelImporterMaterialName.BasedOnTextureName;
        m_materialSearch = ModelImporterMaterialSearch.RecursiveUp;
    }

    private void ApplyModel()
    {
        m_CurImpoter.globalScale = m_globalScale;
        m_CurImpoter.meshCompression = m_meshCompression;
        m_CurImpoter.isReadable = m_isReadable;
        m_CurImpoter.optimizeMesh = m_optimizeMesh;
        m_CurImpoter.importBlendShapes = m_importBlendShapes;
        //...

        m_CurImpoter.importMaterials = m_importMaterials;
        if (m_CurImpoter.importMaterials == true)
        {
            m_CurImpoter.materialName = m_materialName;
            m_CurImpoter.materialSearch = m_materialSearch;
        }
    }

    private void DrawModel()
    {
        EditorGUILayout.BeginVertical();

        GUILayout.Label("Meshes");
        m_globalScale = EditorGUILayout.FloatField("Scale Factor", m_globalScale);
        m_meshCompression = (ModelImporterMeshCompression)EditorGUILayout.EnumPopup("Mesh Compression", m_meshCompression);
        m_isReadable = EditorGUILayout.Toggle("Read/Write Enabled", m_isReadable);
        m_optimizeMesh = EditorGUILayout.Toggle("Optimize Mesh", m_optimizeMesh);
        m_importBlendShapes = EditorGUILayout.Toggle("Import BlendShapes", m_importBlendShapes);
        //...

        GUILayout.Label("Materials");
        m_importMaterials = EditorGUILayout.Toggle("Import Materials", m_importMaterials);
        if (m_importMaterials == true)
        {
            m_materialName = (ModelImporterMaterialName)EditorGUILayout.EnumPopup("Material Naming", m_materialName);
            m_materialSearch = (ModelImporterMaterialSearch)EditorGUILayout.EnumPopup("Material Search", m_materialSearch);
        }
        
        EditorGUILayout.EndVertical();
    }

    #endregion Model

    #region Animations

    public bool m_importAnimation;

    private void InitAnimations()
    {
        m_importAnimation = false;
    }

    private void ApplyAnimations()
    {
        m_CurImpoter.importAnimation = m_importAnimation;
    }

    private void DrawAnimatios()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Import Animation");
        m_importAnimation = EditorGUILayout.Toggle(m_importAnimation);
        EditorGUILayout.EndHorizontal();
    }

    #endregion Animations

    public override void Init(string name)
    {
        base.Init(name);

        m_TypeFilter = FilterType.MODEL;
        m_DrawType = DrawType.Model;

        InitRig();
        InitAnimations();
        InitModel();
    }

    public override bool ApplySettings(UnityEditor.AssetImporter importer)
    {
        m_CurImpoter = importer as ModelImporter;

        if (m_CurImpoter == null)
        {
            return false;
        }

        ApplyRig();
        ApplyAnimations();
        ApplyModel();

        return true;
    }
}
