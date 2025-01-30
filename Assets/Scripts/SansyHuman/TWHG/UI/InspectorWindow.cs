using System.Collections;
using SansyHuman.TWHG.Core.Collections;
using SansyHuman.TWHG.Objects;
using UnityEngine;
using UnityEngine.UI;

namespace SansyHuman.TWHG.UI
{
    /// <summary>
    /// Component of inspector window.
    /// </summary>
    public class InspectorWindow : MonoBehaviour
    {
        [Tooltip("Object that shows the fields.")]
        [SerializeField] private RectTransform inspectorWindow;
        
        [Tooltip("Default field prefab.")]
        [SerializeField] private FieldContents defaultFieldPrefab;

        [Tooltip("Vector2 field prefab.")]
        [SerializeField] private Vector2FieldContents vector2FieldPrefab;

        [Tooltip("Vector3 field prefab.")]
        [SerializeField] private Vector3FieldContents vector3FieldPrefab;

        // All primary fields.
        private IndexedLinkedList<FieldContentsBase, FieldContentsBase> _fields;

        [SerializeField] private ObjectEditorData selectedObject;

        void Awake()
        {
            _fields = new IndexedLinkedList<FieldContentsBase, FieldContentsBase>();
        }

        void Start()
        {
            // Adds a default transform fields.

            FieldContents transformField = CreateField(defaultFieldPrefab, inspectorWindow);
            transformField.FieldName = "Transform";
            transformField.SetFieldType(FieldContents.FieldType.Struct, null, null);
            _fields.AddLast(transformField, transformField);
            
            Vector2FieldContents positionField = CreateField(vector2FieldPrefab, transformField.transform);
            positionField.FieldName = "Position";
            positionField.SetFieldActions(
                obj => obj.transform.localPosition,
                (obj, value) =>
                {
                    obj.transform.localPosition = value;
                });
            
            Vector2FieldContents scaleField = CreateField(vector2FieldPrefab, transformField.transform);
            scaleField.FieldName = "Scale";
            scaleField.SetFieldActions(
                obj => obj.transform.localScale,
                (obj, value) =>
                {
                    obj.transform.localScale = value;
                });
            
            FieldContents rotationField = CreateField(defaultFieldPrefab, transformField.transform);
            rotationField.FieldName = "Rotation";
            rotationField.SetFieldType(FieldContents.FieldType.Float,
                obj => obj.transform.localEulerAngles.z,
                (obj, value) =>
                {
                    Vector3 eulerAngles = obj.transform.localEulerAngles;
                    eulerAngles.z = (float)value;
                    obj.transform.localEulerAngles = eulerAngles;
                });
            
            transformField.AddSubfields(positionField, scaleField, rotationField);
            
            transformField.ConnectedObject = selectedObject;

            Refresh();
        }

        private T CreateField<T>(T prefab, Transform parent) where T : FieldContentsBase
        {
            T field = Instantiate(prefab, parent);
            field.AddRefreshListener(Refresh);
            return field;
        }

        /// <summary>
        /// Updates the layout of the window.
        /// </summary>
        public void Refresh()
        {
            StartCoroutine(RebuildLayout(inspectorWindow));
        }
        
        private readonly WaitForEndOfFrame _wait = new WaitForEndOfFrame();
        private IEnumerator RebuildLayout(RectTransform tr)
        {
            yield return _wait;
            LayoutRebuilder.ForceRebuildLayoutImmediate(tr);
            Canvas.ForceUpdateCanvases();
            
            yield return _wait;
            LayoutRebuilder.ForceRebuildLayoutImmediate(tr);
            Canvas.ForceUpdateCanvases();
        }
    }
}
