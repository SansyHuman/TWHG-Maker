using SansyHuman.TWHG.Objects;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

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
        
        /// <summary>
        /// Gets and sets the connected object.
        /// </summary>
        public abstract ObjectEditorData ConnectedObject
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets the name of the field.
        /// </summary>
        public string FieldName
        {
            get => fieldName.text;
            set
            {
                fieldName.text = value;
                gameObject.name = value;
            }
        }

        /// <summary>
        /// Adds a listener called when the UI layout rebuild is required.
        /// </summary>
        /// <param name="callback">Callback to add.</param>
        public abstract void AddRefreshListener(UnityAction callback);
        
        /// <summary>
        /// Removes a listener called when the UI layout rebuild is required.
        /// </summary>
        /// <param name="callback">Callback to remove.</param>
        public abstract void RemoveRefreshListener(UnityAction callback);
    }
}
