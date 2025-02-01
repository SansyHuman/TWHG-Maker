using SansyHuman.TWHG.Objects;
using SansyHuman.TWHG.UI;
using UnityEngine;

namespace SansyHuman.TWHG.Debug
{
    public class TestObjectEditorData : ObjectEditorData
    {
        [Inspectable] private int int1;
        [Inspectable(ReadOnly = true)] private Vector3 vector3;
        [Inspectable] private Vector2 vector2;
        [Inspectable] private float float1;
        [Inspectable] public string string1;
    }
}
