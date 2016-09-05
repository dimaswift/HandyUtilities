namespace HandyUtilities
{
    using UnityEngine;

    public class EnumFlagAttribute : PropertyAttribute
    {
        public string enumName;

        public EnumFlagAttribute() { }

        public EnumFlagAttribute(string name)
        {
            enumName = name;
        }
    }

    public class ReadOnlyAttribute : PropertyAttribute
    {

    }

}