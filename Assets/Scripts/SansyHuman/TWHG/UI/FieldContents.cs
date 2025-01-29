using System;
using SansyHuman.TWHG.Core.Collections;
using SansyHuman.TWHG.Objects;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

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
        private IndexedLinkedList<FieldContentsBase, FieldContentsBase> _structFields;

        /// <summary>
        /// Gets and sets the connected object.
        /// </summary>
        public ObjectEditorData ConnectedObject
        {
            get => connectedObject;
            set => connectedObject = value;
        }

        public override bool IsReadOnly
        {
            get => fieldValue.readOnly;
            set => fieldValue.readOnly = value;
        }

        void Awake()
        {
            _structFields = new IndexedLinkedList<FieldContentsBase, FieldContentsBase>();
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

        public void SetFieldType(FieldType fieldType, ReadAction fieldReadAction,
            WriteAction fieldWriteAction)
        {
            this.fieldType = fieldType;
            _readAction = fieldReadAction;
            _writeAction = fieldWriteAction;

            fieldValue.gameObject.SetActive(true);
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
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fieldType), fieldType, null);
            }
        }
        
        public void OnEndEdit(string fieldContent)
        {
            UnityEngine.Debug.Log(fieldContent);
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
    }
}
