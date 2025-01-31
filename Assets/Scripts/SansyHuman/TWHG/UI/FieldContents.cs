using System;
using System.Collections.Generic;
using SansyHuman.TWHG.Core.Collections;
using SansyHuman.TWHG.Objects;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SansyHuman.TWHG.UI
{
    /// <summary>
    /// Component of basic inspector fields.
    /// </summary>
    public class FieldContents : FieldContentsBase
    {
        [Tooltip("The value of the field.")]
        [SerializeField] private TMP_InputField fieldValue;

        [Serializable]
        public enum FieldType
        {
            SByte, Byte, Short, UShort, Int, UInt, Long, ULong,
            Float, Double, String, Struct
        }

        [Tooltip("The type of the field.")]
        [SerializeField] private FieldType fieldType;

        [Tooltip("The button to expand the subfields if the type is struct.")]
        [SerializeField] private Button expandButton;

        [Tooltip("Object connected to this field.")]
        [SerializeField] private ObjectEditorData connectedObject;

        /// <summary>
        /// Read action.
        /// </summary>
        /// <param name="obj">Object to read value.</param>
        /// <returns>Read value.</returns>
        public delegate object ReadAction(ObjectEditorData obj);
        
        /// <summary>
        /// Wrtie action.
        /// </summary>
        /// <param name="obj">Object to write value.</param>
        /// <param name="value">Value to write.</param>
        public delegate void WriteAction(ObjectEditorData obj, object value);

        private ReadAction _readAction;
        private WriteAction _writeAction;
        
        // Subfields if the field type is struct.
        private IndexedLinkedList<FieldContentsBase, string> _structFields;

        private bool _structExpanded;

        public override ObjectEditorData ConnectedObject
        {
            get => connectedObject;
            set
            {
                connectedObject = value;
                foreach (var subfields in _structFields)
                {
                    subfields.ConnectedObject = value;
                }
            }
        }

        public override bool IsReadOnly
        {
            get => fieldValue.readOnly;
            set => fieldValue.readOnly = value;
        }

        /// <summary>
        /// Gets a subfield in this field.
        /// </summary>
        /// <param name="name">Name of the field used in AddSubfields.</param>
        /// <returns>Subfield. Null if does not exist.</returns>
        public FieldContentsBase GetSubfield(string name)
        {
            if (!_structFields.ContainsKey(name))
            {
                return null;
            }
            
            return _structFields[name];
        }

        /// <summary>
        /// Gets names of all subfields.
        /// </summary>
        public IEnumerable<string> SubfieldNames => _structFields.Keys;

        void Awake()
        {
            _structFields = new IndexedLinkedList<FieldContentsBase, string>();
            expandButton.interactable = false;
            _structExpanded = true;
            ExpandButtonUpdate();
        }

        private void Update()
        {
            if (!fieldValue.isFocused)
            {
                // Read only when the field is not editing.
                if (fieldType != FieldType.Struct && connectedObject)
                {
                    fieldValue.SetTextWithoutNotify(_readAction(connectedObject).ToString());
                }
            }
        }

        public void OnExpandButtonClick()
        {
            _structExpanded = !_structExpanded;
            ExpandButtonUpdate();
            foreach (var subfields in _structFields)
            {
                subfields.gameObject.SetActive(_structExpanded);
            }
        }

        private void ExpandButtonUpdate()
        {
            RectTransform buttonTr = expandButton.GetComponent<RectTransform>();
            Quaternion rotation = buttonTr.rotation;
            rotation.eulerAngles = new Vector3(0, 0, _structExpanded ? -180 : -90);
            buttonTr.rotation = rotation;
        }

        /// <summary>
        /// Sets field type and resets ConnectedObject to null.
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="fieldReadAction">Delegate to read field value from the object.</param>
        /// <param name="fieldWriteAction">Delegate to write to field of the object.</param>
        public void SetFieldType(FieldType fieldType, ReadAction fieldReadAction,
            WriteAction fieldWriteAction)
        {
            this.fieldType = fieldType;
            _readAction = fieldReadAction;
            _writeAction = fieldWriteAction;
            ConnectedObject = null;

            fieldValue.gameObject.SetActive(true);
            expandButton.interactable = false;
            switch (fieldType)
            {
                case FieldType.SByte:
                case FieldType.Byte:
                case FieldType.Short:
                case FieldType.UShort:
                case FieldType.Int:
                case FieldType.UInt:
                case FieldType.Long:
                case FieldType.ULong:
                    fieldValue.contentType = TMP_InputField.ContentType.IntegerNumber;
                    break;
                case FieldType.Float:
                case FieldType.Double:
                    fieldValue.contentType = TMP_InputField.ContentType.DecimalNumber;
                    break;
                case FieldType.String:
                    fieldValue.contentType = TMP_InputField.ContentType.Standard;
                    break;
                case FieldType.Struct:
                    fieldValue.gameObject.SetActive(false);
                    expandButton.interactable = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fieldType), fieldType, null);
            }
        }

        /// <summary>
        /// Changes the field type to struct and adds subfields to the field(internal use only).
        /// </summary>
        /// <param name="subfields">Subfields and their name to add. Name is used to get subfields.</param>
        public void AddSubfields(params (FieldContentsBase field, string name)[] subfields)
        {
            if (subfields.Length == 0)
            {
                return;
            }

            if (fieldType != FieldType.Struct)
            {
                SetFieldType(FieldType.Struct, null, null);
            }

            for (int i = 0; i < subfields.Length; i++)
            {
                _structFields.AddLast(subfields[i].name, subfields[i].field);
                subfields[i].field.transform.SetParent(transform);
                subfields[i].field.ConnectedObject = connectedObject;
            }
        }
        
        public void OnEndEdit(string fieldContent)
        {
            if (fieldType != FieldType.Struct && connectedObject)
            {
                _writeAction(connectedObject, ParseValue(fieldContent));
            }
        }

        private object ParseValue(string value)
        {
            switch (fieldType)
            {
                case FieldType.SByte:
                    return sbyte.Parse(value);
                case FieldType.Byte:
                    return byte.Parse(value);
                case FieldType.Short:
                    return short.Parse(value);
                case FieldType.UShort:
                    return ushort.Parse(value);
                case FieldType.Int:
                    return int.Parse(value);
                case FieldType.UInt:
                    return uint.Parse(value);
                case FieldType.Long:
                    return long.Parse(value);
                case FieldType.ULong:
                    return ulong.Parse(value);
                case FieldType.Float:
                    return float.Parse(value);
                case FieldType.Double:
                    return double.Parse(value);
                case FieldType.String:
                    return fieldValue;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void AddRefreshListener(UnityAction callback)
        {
            expandButton.onClick.AddListener(callback);
        }

        public override void RemoveRefreshListener(UnityAction callback)
        {
            expandButton.onClick.RemoveListener(callback);
        }
    }
}
