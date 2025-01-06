using SansyHuman.TWHG.Objects;
using SansyHuman.TWHG.UI;
using UnityEngine;

namespace SansyHuman.TWHG.Debug
{
    public class HierarchyTest : MonoBehaviour
    {
        public ObjectEditorData[] addingObjects;
        public ObjectHierarchyWindow hierarchyWindow;
        
        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < addingObjects.Length; i++)
            {
                hierarchyWindow.AddObject(addingObjects[i]);
            }
        }
    }
}
