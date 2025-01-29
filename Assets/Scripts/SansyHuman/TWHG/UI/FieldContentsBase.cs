using TMPro;
using UnityEngine;

namespace SansyHuman.TWHG.UI
{
    /// <summary>
    /// Base class of field contents objects.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public abstract class FieldContentsBase : MonoBehaviour
    {
        [Tooltip("The name of the field.")]
        [SerializeField] protected TextMeshProUGUI fieldName;

        /// <summary>
        /// Gets and sets whether the field is read only.
        /// </summary>
        public abstract bool IsReadOnly
        {
            get;
            set;
        }
    }
}
