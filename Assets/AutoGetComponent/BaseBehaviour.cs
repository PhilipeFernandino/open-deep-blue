using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class BaseBehaviour : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        InjectGetComponent();
    }

    private void InjectGetComponent()
    {
        var fields = GetFieldsWithAttribute(typeof(GetComponentAttribute));
        foreach (var field in fields)
        {
            var type = field.FieldType;
            var component = GetComponent(type);
            if (component == null)
            {
                Debug.LogWarning("GetComponent typeof(" + type.Name + ") in game object '" + gameObject.name + "' is null");
                continue;
            }
            field.SetValue(this, component);
        }
    }

    private IEnumerable<FieldInfo> GetFieldsWithAttribute(Type attributeType)
    {
        var fields = GetType()
            .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(field => field.GetCustomAttributes(attributeType, true).FirstOrDefault() != null);

        return fields;
    }
}