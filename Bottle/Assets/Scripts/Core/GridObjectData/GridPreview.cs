using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Bottle.Core.Manager;
namespace Bottle.Core.GridObjectData
{
    public class GridPreview : GridObject
    {
        public GridTile gridTile;
        public GridEntity gridEntity;
        protected override void OnEnable()
        {
            base.OnEnable();
            if (gridTile)
                SetupPreviewGridObject(this.transform);
        }

        private void OnValidate()
        {
            //SetupPreviewGridObject(this.transform);
        }

        protected override void Start()
        {
            base.Start();
        }
        private Transform SetupPreviewGridObject(Transform transform)
        {
            // Attempt to get reference to GameObject Renderer
            Renderer meshRenderer = transform.gameObject.GetComponent<Renderer>();

            // If a Renderer was found
            if (meshRenderer != null)
            {
                // Define temporary Material used to create transparent copy of GameObject Material
                Material tempMat = new Material(Shader.Find("Bottle/Preview_Tiles_PBL"));
                Material[] tempMats = new Material[meshRenderer.sharedMaterials.Length];
                
                // Loop through each material in GameObject
                for (int i = 0; i < meshRenderer.sharedMaterials.Length; i++)
                {
                    // Get material from GameObject
                    tempMat = new Material(meshRenderer.sharedMaterials[i]);

                    // Change Shader to "Standard"
                    tempMat.shader = Shader.Find("Bottle/Preview_Tiles_PBL");
                    

                    // Replace GameObject Material with transparent one
                    tempMats[i] = tempMat;
                }
                
                meshRenderer.sharedMaterials = tempMats;
            }

            // Recursively run this method for each child transform
            foreach (Transform child in transform)
            {
                SetupPreviewGridObject(child);
            }

            return transform;
        }
    }
}