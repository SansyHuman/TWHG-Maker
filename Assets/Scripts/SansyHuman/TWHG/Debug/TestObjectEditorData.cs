using System;
using SansyHuman.TWHG.Objects;
using SansyHuman.TWHG.UI;
using UnityEngine;

namespace SansyHuman.TWHG.Debug
{
    public class TestObjectEditorData : ObjectEditorData
    {
        [Inspectable] private int int1;
        public int int2;
        private int int3;
        [Inspectable(ReadOnly = true)] private Vector3 vector3;
        [Inspectable] private Vector2 vector2;
        [Inspectable] private float float1;
        [Inspectable] public string string1;
        [Inspectable] private TestStruct1 testStruct1;

        [SerializeField] private FieldContentsBase[] _customFields;
        
        public override FieldContentsBase[] CustomFieldContents => _customFields;
    }

    [Serializable]
    public struct TestStruct1
    {
        public int Integer1;
        public float Float1;
        public TestStruct2 Struct1;
        public Vector2 Vector1;
    }

    [Serializable]
    public struct TestStruct2
    {
        public short Short1;
        public long Long1;
    }
}
