using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

[VFXBinder("Transform/LocalTransform")]
public class VFXLocalTransformBinder : VFXBinderBase
{
    public string Property { get { return (string)m_Property; } set { m_Property = value; UpdateSubProperties(); } }

    [VFXPropertyBinding("UnityEditor.VFX.Transform"), SerializeField, UnityEngine.Serialization.FormerlySerializedAs("m_Parameter")]
    protected ExposedProperty m_Property = "Transform";
    public Transform Target = null;
    private ExposedProperty Position;
    private ExposedProperty Angles;
    private ExposedProperty Scale;



    protected override void OnEnable()
    {
        base.OnEnable();
        UpdateSubProperties();
    }

    private void OnValidate()
    {
        UpdateSubProperties();
    }

    void UpdateSubProperties()
    {
        Position = m_Property + "_position";
        Angles = m_Property + "_angles";
        Scale = m_Property + "_scale";
    }

    public override bool IsValid(VisualEffect component)
    {
        return Target != null && component.HasVector3((int)Position) && component.HasVector3((int)Angles) && component.HasVector3((int)Scale);
    }

    public override void UpdateBinding(VisualEffect component)
    {
        // Gives Local Position instead of world position
        component.SetVector3((int)Position, Target.localPosition);
        component.SetVector3((int)Angles, Target.localRotation.eulerAngles);
        component.SetVector3((int)Scale, Target.localScale);
    }

    public override string ToString()
    {
        return string.Format("Transform : '{0}' -> {1}", m_Property, Target == null ? "(null)" : Target.name);
    }
}
