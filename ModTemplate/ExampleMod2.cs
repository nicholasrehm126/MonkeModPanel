using System.Collections.Generic;
using Monke_Mod_Panel.Attributes;
using UnityEngine;

namespace ModTemplate;

/// <summary>
/// Creates an example mod usable by MonkeModPanel
/// If you want this mod to be toggleable, add [Toggleable] above the class.
/// </summary>
[Toggleable]
public class ExampleMod2 : Monke_Mod_Panel.Mod
{
    /// <summary>
    /// The name of the mod thats visible in the mod panel.
    /// </summary>
    public override string Name => "Draw Mod (R)";

    List<GameObject> drawedObjects = new List<GameObject>();

    /// <summary>
    /// Runs every frame if the mod is toggled via the [Toggleable] attribute.
    /// </summary>
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f)
        {
            GameObject drawedObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            drawedObj.transform.localScale = Vector3.one * 0.1f;
            drawedObj.transform.position = GorillaTagger.Instance.rightHandTransform.position;
            drawedObj.GetComponent<Renderer>().material = new Material(Shader.Find("GorillaTag/UberShader"));
            
            Color.RGBToHSV(drawedObj.GetComponent<Renderer>().material.color, out float h, out float s, out float v);
            drawedObj.GetComponent<Renderer>().material.color = Color.HSVToRGB(drawedObjects.Count / 10, s, v);

            drawedObj.GetComponent<Collider>().Destroy();
            
            drawedObjects.Add(drawedObj);
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        foreach (GameObject drawedObj in drawedObjects)
            drawedObj.Destroy();
        
        drawedObjects.Clear();
    }
}