using UnityEngine;
using System.Collections.Generic;
using HandyUtilities;
using CodeGenerator;
namespace HandyUtilities
{
    public class ComponentPickerContainer : SerializedSingleton<ComponentPickerContainer>
    {
        public bool pendingScriptCompile;

        public List<ScreenBinding> screenBindings = new List<ScreenBinding>();

        public string newTypeName;
        public int targetID;
        public string elementsClassString;
        public ComponentContainer root;
        public Class pendingClass;
        public OperationType operationType;
        public enum OperationType { Create, Update }

        [System.Serializable]
        public class ScreenBinding
        {
            public string screenName;
            public string fieldName;
            public int targetGameObjectID;
        }
    }
}
