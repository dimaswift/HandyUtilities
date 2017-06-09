using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandyUtilities
{
    public class HandyEditorSettings : SerializedSingleton<HandyEditorSettings>
    {
        [HideInInspector]
        public object[] confirmationToolObjects = new object[0];
    }

}
