using System;
using UnityEngine;

namespace SansyHuman.TWHG.UI
{
    /// <summary>
    /// Attribute on fields which are readable and writeable in the inspector. In the case of
    /// struct fields, struct should have Serializable attribute to read and write all fields.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class InspectableAttribute : Attribute
    {
        /// <summary>
        /// If true the field is read only.
        /// </summary>
        public bool ReadOnly;
        
        public InspectableAttribute()
        {
            ReadOnly = false;
        }
    }
}
