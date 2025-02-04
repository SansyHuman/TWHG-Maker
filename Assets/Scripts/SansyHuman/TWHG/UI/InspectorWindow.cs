using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using SansyHuman.TWHG.Core.Collections;
using SansyHuman.TWHG.Objects;
using Unity.VisualScripting;
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
        private IndexedLinkedList<FieldContentsBase, string> _fields;

        [SerializeField] private ObjectEditorData selectedObject;

        // Information of inspectable fields or properties.
        private struct Inspectable
        {
            public enum FieldType
            {
                Simple, Struct
            }

            public FieldType Type;
            public FieldInfo FieldInfo;
            public List<Inspectable> StructFields;
            public bool ReadOnly;
        }

        private Dictionary<Type, List<Inspectable>> _inspectorCache;
        private HashSet<Type> _inspectableTypes;

        public ObjectEditorData SelectedObject
        {
            get => selectedObject;
            set
            {
                Type oldType = selectedObject?.GetType();
                selectedObject = value;

                FieldContents transformField = (FieldContents)_fields["transform"];
                transformField.gameObject.SetActive(selectedObject);
                if (selectedObject)
                {
                    transformField.GetSubfield("position").IsReadOnly = !selectedObject.canMove;
                    transformField.GetSubfield("rotation").IsReadOnly = !selectedObject.canRotate;
                    transformField.GetSubfield("scale").IsReadOnly = !selectedObject.canScale;
                }

                BuildInspector(selectedObject, oldType);
                foreach (var field in _fields)
                {
                    field.ConnectedObject = selectedObject;
                }
                
                Refresh();
            }
        }

        // Builds inspector of object type.
        private void BuildInspector(ObjectEditorData obj, Type oldType)
        {
            if (!obj)
            {
                // Remove all fields except transform.
                while (_fields.Count > 1)
                {
                    Destroy(_fields.Last.Value.gameObject);
                    _fields.RemoveLast();
                }

                return;
            }

            Type objType = obj.GetType();
            if (objType == oldType)
            {
                return;
            }
            
            if (!_inspectorCache.ContainsKey(objType))
            {
                _inspectorCache.Add(objType, BuildInspectorCache(objType));
            }

            // Remove all fields except transform.
            while (_fields.Count > 1)
            {
                Destroy(_fields.Last.Value.gameObject);
                _fields.RemoveLast();
            }
            List<Inspectable> inspectables = _inspectorCache[objType];

            for (int i = 0; i < inspectables.Count; i++)
            {
                FieldContentsBase field = null;
                
                if (inspectables[i].Type == Inspectable.FieldType.Simple)
                {
                    field = BuildSimpleField(inspectables[i], inspectorWindow);
                }
                else // inspectables[i].Type == Inspectable.FieldType.Struct
                {
                    field = BuildStructField(inspectables[i], inspectorWindow);
                }

                _fields.AddLast(field.FieldName, field);
            }

            FieldContentsBase[] customFields = obj.CustomFieldContents;
            if (customFields == null)
            {
                return;
            }

            for (int i = 0; i < customFields.Length; i++)
            {
                FieldContentsBase field = CreateField(customFields[i], inspectorWindow);
                _fields.AddLast(field.FieldName, field);
            }
        }

        private FieldContentsBase BuildSimpleField(in Inspectable inspectable, Transform parent, FieldContents.ReadAction parentObjectReader = null, FieldContents.WriteAction parentObjectWriter = null)
        {
            FieldContentsBase field = null;
            FieldInfo fieldInfo = inspectable.FieldInfo;
            
            if (fieldInfo.FieldType == typeof(Vector2))
            {
                
                Vector2FieldContents v2Field = CreateField(vector2FieldPrefab, inspectorWindow);
                Vector2FieldContents.ReadAction v2FieldReader = null;
                Vector2FieldContents.WriteAction v2FieldWriter = null;

                if (parentObjectReader == null)
                {
                    v2FieldReader = o => (Vector2)fieldInfo.GetValue(o);
                }
                else
                {
                    v2FieldReader = o =>
                    {
                        object parentObject = parentObjectReader(o);
                        return (Vector2)fieldInfo.GetValue(parentObject);
                    };
                }

                if (parentObjectWriter == null)
                {
                    v2FieldWriter = (o, value) =>
                    {
                        fieldInfo.SetValue(o, value);
                    };
                }
                else
                {
                    v2FieldWriter = (o, value) =>
                    {
                        object parentObject = parentObjectReader(o);
                        fieldInfo.SetValue(parentObject, value);
                        parentObjectWriter(o, parentObject);
                    };
                }
                
                v2Field.SetFieldActions(v2FieldReader, v2FieldWriter);
                field = v2Field;
            }
            else if (fieldInfo.FieldType == typeof(Vector3))
            {
                Vector3FieldContents v3Field = CreateField(vector3FieldPrefab, inspectorWindow);
                Vector3FieldContents.ReadAction v3FieldReader = null;
                Vector3FieldContents.WriteAction v3FieldWriter = null;

                if (parentObjectReader == null)
                {
                    v3FieldReader = o => (Vector3)fieldInfo.GetValue(o);
                }
                else
                {
                    v3FieldReader = o =>
                    {
                        object parentObject = parentObjectReader(o);
                        return (Vector3)fieldInfo.GetValue(parentObject);
                    };
                }

                if (parentObjectWriter == null)
                {
                    v3FieldWriter = (o, value) =>
                    {
                        fieldInfo.SetValue(o, value);
                    };
                }
                else
                {
                    v3FieldWriter = (o, value) =>
                    {
                        object parentObject = parentObjectReader(o);
                        fieldInfo.SetValue(parentObject, value);
                        parentObjectWriter(o, parentObject);
                    };
                }
                
                v3Field.SetFieldActions(v3FieldReader, v3FieldWriter);
                field = v3Field;
            }
            else
            {
                FieldContents sField = CreateField(defaultFieldPrefab, inspectorWindow);
                FieldContents.ReadAction fieldReader = null;
                FieldContents.WriteAction fieldWriter = null;

                if (parentObjectReader == null)
                {
                    fieldReader = o => fieldInfo.GetValue(o);
                }
                else
                {
                    fieldReader = o =>
                    {
                        object parentObject = parentObjectReader(o);
                        return fieldInfo.GetValue(parentObject);
                    };
                }

                if (parentObjectWriter == null)
                {
                    fieldWriter = (o, value) =>
                    {
                        fieldInfo.SetValue(o, value);
                    };
                }
                else
                {
                    fieldWriter = (o, value) =>
                    {
                        object parentObject = parentObjectReader(o);
                        fieldInfo.SetValue(parentObject, value);
                        parentObjectWriter(o, parentObject);
                    };
                }
                
                sField.SetFieldType(FieldContents.TypeToFieldType(fieldInfo.FieldType), fieldReader, fieldWriter);
                field = sField;
            }
            
            field.FieldName = inspectable.FieldInfo.Name;
            field.IsReadOnly = inspectable.ReadOnly;

            return field;
        }

        private FieldContentsBase BuildStructField(Inspectable inspectable, Transform parent, FieldContents.ReadAction parentObjectReader = null, FieldContents.WriteAction parentObjectWriter = null)
        {
            FieldContents field = CreateField(defaultFieldPrefab, parent);
            field.SetFieldType(FieldContents.FieldType.Struct, null, null);
            
            FieldContents.ReadAction structReader = null;
            FieldContents.WriteAction structWriter = null;

            if (parentObjectReader == null)
            {
                structReader = o => inspectable.FieldInfo.GetValue(o);
            }
            else
            {
                structReader = o =>
                {
                    object parentObject = parentObjectReader(o);
                    return inspectable.FieldInfo.GetValue(parentObject);
                };
            }

            if (parentObjectWriter == null)
            {
                structWriter = (o, value) =>
                {
                    inspectable.FieldInfo.SetValue(o, value);
                };
            }
            else
            {
                structWriter = (o, value) =>
                {
                    object parentObject = parentObjectReader(o);
                    inspectable.FieldInfo.SetValue(parentObject, value);
                    parentObjectWriter(o, parentObject);
                };
            }

            for (int i = 0; i < inspectable.StructFields.Count; i++)
            {
                FieldContentsBase subfield = null;
                if (inspectable.StructFields[i].Type == Inspectable.FieldType.Simple)
                {
                    subfield = BuildSimpleField(inspectable.StructFields[i], field.transform, structReader, structWriter);
                }
                else // inspectable.StructFields[i].Type == Inspectable.FieldType.Struct
                {
                    subfield = BuildStructField(inspectable.StructFields[i], field.transform, structReader, structWriter);
                }
                
                field.AddSubfields((subfield, subfield.FieldName));
            }
            
            field.FieldName = inspectable.FieldInfo.Name;
            field.IsReadOnly = inspectable.ReadOnly;

            return field;
        }

        // Builds inspector cache of object type.
        private List<Inspectable> BuildInspectorCache(Type objType)
        {
            List<Inspectable> inspectables = new List<Inspectable>();

            FieldInfo[] fields =
                objType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < fields.Length; i++)
            {
                InspectableAttribute attribute = fields[i].GetCustomAttribute<InspectableAttribute>();
                if (attribute == null)
                {
                    continue;
                }
                
                Inspectable inspectable = new Inspectable();
                inspectable.FieldInfo = fields[i];
                if (_inspectableTypes.Contains(fields[i].FieldType))
                {
                    inspectable.Type = Inspectable.FieldType.Simple;
                }
                else if (fields[i].FieldType.IsStruct())
                {
                    SerializableAttribute serializable =
                        fields[i].FieldType.GetCustomAttribute<SerializableAttribute>();
                    if (serializable == null)
                    {
                        continue;
                    }
                    
                    BuildStructInspectable(fields[i].FieldType, ref inspectable);
                }
                else
                {
                    continue;
                }
                
                inspectable.ReadOnly = attribute.ReadOnly;
                inspectables.Add(inspectable);
            }
            
            return inspectables;
        }

        private void BuildStructInspectable(Type structType, ref Inspectable inspectable)
        {
            inspectable.Type = Inspectable.FieldType.Struct;
            inspectable.StructFields = new List<Inspectable>();

            FieldInfo[] fields = structType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            for (int i = 0; i < fields.Length; i++)
            {
                Inspectable subfield = new Inspectable();
                subfield.FieldInfo = fields[i];
                if (_inspectableTypes.Contains(fields[i].FieldType))
                {
                    subfield.Type = Inspectable.FieldType.Simple;
                }
                else if (fields[i].FieldType.IsStruct())
                {
                    SerializableAttribute serializable =
                        fields[i].FieldType.GetCustomAttribute<SerializableAttribute>();
                    if (serializable == null)
                    {
                        continue;
                    }

                    BuildStructInspectable(fields[i].FieldType, ref subfield);
                }
                else
                {
                    continue;
                }

                inspectable.StructFields.Add(subfield);
            }
        }

        void Awake()
        {
            _fields = new IndexedLinkedList<FieldContentsBase, string>();
            _inspectorCache = new Dictionary<Type, List<Inspectable>>();
            _inspectableTypes = new HashSet<Type>();
            _inspectableTypes.AddRange(new []
            {
                typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong),
                typeof(float), typeof(double), typeof(string), typeof(Vector2), typeof(Vector3)
            });
        }

        void Start()
        {
            // Adds a default transform fields.

            FieldContents transformField = CreateField(defaultFieldPrefab, inspectorWindow);
            transformField.FieldName = "Transform";
            transformField.SetFieldType(FieldContents.FieldType.Struct, null, null);
            _fields.AddLast("transform", transformField);
            
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
            
            transformField.AddSubfields((positionField, "position"), (scaleField, "scale"), (rotationField, "rotation"));
            
            transformField.ConnectedObject = selectedObject;
            
            transformField.gameObject.SetActive(false);

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
