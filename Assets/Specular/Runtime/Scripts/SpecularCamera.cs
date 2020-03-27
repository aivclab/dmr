using UnityEngine;

namespace Specular.Runtime.Scripts {
  public abstract class SpecularCamera : MonoBehaviour {
    /// <summary>
    /// Called before any portal cameras are rendered for the current frame
    /// </summary>
    /// <param name="receiver"></param>
    /// <returns></returns>
    protected internal abstract void PreSpecularRender(Camera receiver);
    /// <summary>
    /// Manually render the camera attached to this SpecularCamera
    /// Called after PrePortalRender, and before PostPortalRender
    /// </summary>
    /// <param name="receiver"></param>
    /// <returns></returns>
    protected internal abstract void SpecularRender(Camera receiver);
    /// <summary>
    /// Called once all portals have been rendered, but before the player camera renders
    /// </summary>
    /// <param name="receiver"></param>
    /// <returns></returns>
    protected internal abstract void PostSpecularRender(Camera receiver);
  }
}
