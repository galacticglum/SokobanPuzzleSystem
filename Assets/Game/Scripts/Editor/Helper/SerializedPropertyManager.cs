using System.Collections.Generic;
using UnityEditor;

public class SerializedPropertyManager
{
    public SerializedObject Target { get; }
    public SerializedProperty this[string name] => Get(name);

    private readonly Dictionary<string, SerializedProperty> properties;

    public SerializedPropertyManager(SerializedObject target)
    {
        Target = target;
        properties = new Dictionary<string, SerializedProperty>();
    }

    public SerializedProperty Get(string name)
    {
        if (!properties.ContainsKey(name))
        {
            properties[name] = Target.FindProperty(name);
        }

        return properties[name];
    }
}
