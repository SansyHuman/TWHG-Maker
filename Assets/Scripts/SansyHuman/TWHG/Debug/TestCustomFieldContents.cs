using System;
using SansyHuman.TWHG.Objects;
using SansyHuman.TWHG.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SansyHuman.TWHG.Debug
{
    public class TestCustomFieldContents : FieldContentsBase
    {
        [SerializeField] private TMP_InputField fieldValue;

        private TestObjectEditorData _connectedObject;

        public override bool IsReadOnly
        {
            get => fieldValue.readOnly;
            set => fieldValue.readOnly = value;
        }

        public override ObjectEditorData ConnectedObject
        {
            get => _connectedObject;
            set => _connectedObject = value as TestObjectEditorData;
        }
        
        public override void AddRefreshListener(UnityAction callback)
        {
            
        }

        public override void RemoveRefreshListener(UnityAction callback)
        {
            
        }

        void Awake()
        {
            fieldName.text = "int2";
            fieldValue.contentType = TMP_InputField.ContentType.IntegerNumber;
        }

        private void Update()
        {
            if (!fieldValue.isFocused)
            {
                if (_connectedObject)
                {
                    fieldValue.SetTextWithoutNotify(_connectedObject.int2.ToString());
                }
            }
        }

        public void OnEndEdit(string fieldContent)
        {
            _connectedObject.int2 = int.Parse(fieldContent);
        }
    }
}
