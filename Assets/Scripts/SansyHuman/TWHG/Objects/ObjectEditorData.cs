using SansyHuman.TWHG.UI;
using UnityEngine;

namespace SansyHuman.TWHG.Objects
{
    /// <summary>
    /// Component that contains data used in level editor.
    /// </summary>
    public class ObjectEditorData : MonoBehaviour
    {
        [Tooltip("The sprite to use as a brush icon in level editor.")]
        public Sprite editorBrushIcon;

        [Tooltip("Indicates whether the object has a gizmo only shown in level editor.")]
        public bool hasGizmo;
        
        [Tooltip("The sprite to use as a gizmo in level editor.")]
        public Sprite gizmoIcon;

        [Tooltip("The size of the gizmo in world space coordinates.")]
        public Vector2 gizmoSize;

        [Tooltip("Whether the object is selectable in level editor.")]
        public bool selectable = true;

        [Tooltip("Whether the object is destroyable in level editor.")]
        public bool destroyable = true;

        [Tooltip("Whether the object can have a parent.")]
        public bool canHaveParent = true;
        
        [Tooltip("Whether the object can have children.")]
        public bool canHaveChildren = true;

        [Tooltip("Whether the object can move in level editor.")]
        public bool canMove = true;

        [Tooltip("Whether the object can rotate in level editor.")]
        public bool canRotate = true;

        [Tooltip("Whether the object can change scale in level editor.")]
        public bool canScale = true;

        /// <summary>
        /// Overrides this to add custom field contents to show in the inspector.
        /// </summary>
        public virtual FieldContentsBase[] CustomFieldContents => null;
    }
}
