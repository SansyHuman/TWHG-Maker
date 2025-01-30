using System;
using SansyHuman.TWHG.Objects;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SansyHuman.TWHG.UI
{
    /// <summary>
    /// Component of vector 2 fields.
    /// </summary>
    public class Vector2FieldContents : FieldContentsBase
    {
        [Tooltip("The x value of the field.")]
        [SerializeField] private TMP_InputField xValue;
        
        [Tooltip("The y value of the field.")]
        [SerializeField] private TMP_InputField yValue;

        [Tooltip("Object connected to this field.")]
        [SerializeField] private ObjectEditorData connectedObject;

        /// <summary>
        /// Read action.
        /// </summary>
        /// <param name="obj">Object to read Vector2 value.</param>
        /// <returns>Read value.</returns>
        public delegate Vector2 ReadAction(ObjectEditorData obj);
        
        /// <summary>
        /// Write action.
        /// </summary>
        /// <param name="obj">Object to write Vector2 value.</param>
        /// <param name="value">Value to write.</param>
        public delegate void WriteAction(ObjectEditorData obj, Vector2 value);
        
        private ReadAction _readAction;
        private WriteAction _writeAction;
        
        public override bool IsReadOnly
        {
            get => xValue.readOnly;
            set
            {
                xValue.readOnly = value;
                yValue.readOnly = value;
            }
        }

        public override ObjectEditorData ConnectedObject
        {
            get => connectedObject;
            set => connectedObject = value;
        }

        public override void AddRefreshListener(UnityAction callback)
        {
            
        }

        public override void RemoveRefreshListener(UnityAction callback)
        {
            
        }

        void Awake()
        {
            xValue.contentType = TMP_InputField.ContentType.DecimalNumber;
            yValue.contentType = TMP_InputField.ContentType.DecimalNumber;
        }

        private void Update()
        {
            if (!xValue.isFocused && !yValue.isFocused)
            {
                // Read ony when the field is not editing.
                if (connectedObject)
                {
                    Vector2 value = _readAction(connectedObject);
                    xValue.SetTextWithoutNotify(value.x.ToString("F2"));
                    yValue.SetTextWithoutNotify(value.y.ToString("F2"));
                }
            }
        }

        /// <summary>
        /// Sets field read and write actions and resets ConnectedObject to null.
        /// </summary>
        /// <param name="readAction">Delegate to read field value from the object.</param>
        /// <param name="writeAction">Delegate to write to field of the object.</param>
        public void SetFieldActions(ReadAction readAction, WriteAction writeAction)
        {
            _readAction = readAction;
            _writeAction = writeAction;
            ConnectedObject = null;
        }

        public void OnEndXEdit(string xContent)
        {
            if (connectedObject)
            {
                Vector2 value = _readAction(connectedObject);
                value.x = float.Parse(xContent);
                _writeAction(connectedObject, value);
            }
        }

        public void OnEndYEdit(string yContent)
        {
            if (connectedObject)
            {
                Vector2 value = _readAction(connectedObject);
                value.y = float.Parse(yContent);
                _writeAction(connectedObject, value);
            }
        }
    }
}
