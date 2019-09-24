using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameFramePro.UI
{
    public class NoDrawingRayCast : UnityEngine.UI.Graphic
    {
        public override void SetMaterialDirty()
        {
        }
        public override void SetVerticesDirty()
        {
        }

#pragma warning disable CS0672 // Member overrides obsolete member
        protected override void OnFillVBO(List<UIVertex> vbo)
#pragma warning restore CS0672 // Member overrides obsolete member
        {
            vbo.Clear();
        }
    }
}
