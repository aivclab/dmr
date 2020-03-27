using System;
using System.Collections.Generic;
using UnityEngine;

namespace Specular.Runtime.Scripts.Portal {
  public class PortalTraveller : MonoBehaviour {
    public GameObject graphicsObject;
    static readonly Int32 _slice_normal = Shader.PropertyToID("_SliceNormal");
    static readonly Int32 _slice_offset_dst = Shader.PropertyToID("_SliceOffsetDst");
    public GameObject GraphicsClone { get; set; }
    public Vector3 PreviousOffsetFromPortal { get; set; }

    public Material[] OriginalMaterials { get; set; }
    public Material[] CloneMaterials { get; set; }

    public virtual void Teleport(Transform from_portal, Transform to_portal, Vector3 pos, Quaternion rot) {
      this.transform.position = pos;
      this.transform.rotation = rot;
    }

    public virtual void EnterPortalThreshold() {
      // Called when first touches portal
      if (this.GraphicsClone == null) {
        this.GraphicsClone = Instantiate(original : this.graphicsObject);
        this.GraphicsClone.transform.parent = this.graphicsObject.transform.parent;
        this.GraphicsClone.transform.localScale = this.graphicsObject.transform.localScale;
        this.OriginalMaterials = GetMaterials(g : this.graphicsObject);
        this.CloneMaterials = GetMaterials(g : this.GraphicsClone);
      } else {
        this.GraphicsClone.SetActive(true);
      }
    }

    public virtual void ExitPortalThreshold() {
      // Called once no longer touching portal (excluding when teleporting)
      this.GraphicsClone.SetActive(false);
      // Disable slicing
      for (var i = 0; i < this.OriginalMaterials.Length; i++) {
        this.OriginalMaterials[i].SetVector(nameID : _slice_normal, value : Vector3.zero);
      }
    }

    public void SetSliceOffsetDst(float dst, bool clone) {
      for (var i = 0; i < this.OriginalMaterials.Length; i++) {
        if (clone) {
          this.CloneMaterials[i].SetFloat(nameID : _slice_offset_dst, value : dst);
        } else {
          this.OriginalMaterials[i].SetFloat(nameID : _slice_offset_dst, value : dst);
        }
      }
    }

    static Material[] GetMaterials(GameObject g) {
      var renderers = g.GetComponentsInChildren<MeshRenderer>();
      var mat_list = new List<Material>();
      for (var i = 0; i < renderers.Length; i++) {
        var renderer = renderers[i];
        for (var index = 0; index < renderer.materials.Length; index++) {
          var mat = renderer.materials[index];
          mat_list.Add(item : mat);
        }
      }

      return mat_list.ToArray();
    }
  }
}
