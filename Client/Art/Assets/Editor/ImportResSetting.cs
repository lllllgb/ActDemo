using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MyEditor
{
    public class ImportResSetting : AssetPostprocessor
    {
        //模型导入之前调用  
        public void OnPreprocessModel()
        {
        }

        //模型导入之后调用  
        public void OnPostprocessModel(GameObject go)
        {
            ModelImporter tmpModelImporter = (ModelImporter)assetImporter;

            tmpModelImporter.animationType = ModelImporterAnimationType.Legacy;
        }
    }
}
