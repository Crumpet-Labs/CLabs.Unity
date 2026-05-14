using System;
using UnityEngine;

namespace CLabs.Adapters
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class RequireInterfaceAttribute : PropertyAttribute
    {
        public Type InterfaceType { get; }

        public RequireInterfaceAttribute(Type interfaceType)
        {
            InterfaceType = interfaceType;
        }
    }
}
