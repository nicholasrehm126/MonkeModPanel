using Monke_Mod_Panel.Attributes;
using UnityEngine;

namespace Monke_Mod_Panel.Mods;

[Toggleable]
public class Platforms : Mod
{
    public override string Name => "Platforms";

    private GameObject lPlatform;
    private GameObject rPlatform;
    
    public override void OnUpdate()
    {
        Vector3 platformScale = new Vector3(0.02f, 0.2f, 0.2f);
        Transform leftHand = GorillaTagger.Instance.leftHandTransform;
        Transform rightHand = GorillaTagger.Instance.rightHandTransform;
        
        if (ControllerInputPoller.instance.leftGrab && !lPlatform)
        {
            lPlatform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            lPlatform.transform.SetParent(leftHand, false);
            lPlatform.transform.localRotation = Quaternion.identity;
            lPlatform.transform.SetParent(null, true);
            lPlatform.transform.localScale = platformScale;

            lPlatform.GetComponent<Renderer>().material = new Material(Shader.Find("GorillaTag/UberShader"));
        }
        else if (!ControllerInputPoller.instance.leftGrab && lPlatform)
        {
            GameObject.Destroy(lPlatform);
            lPlatform = null;
        }

        if (ControllerInputPoller.instance.rightGrab && !rPlatform)
        {
            rPlatform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            rPlatform.transform.SetParent(rightHand, false);
            rPlatform.transform.localRotation = Quaternion.identity;
            rPlatform.transform.SetParent(null, true);
            rPlatform.transform.localScale = platformScale;

            rPlatform.GetComponent<Renderer>().material = new Material(Shader.Find("GorillaTag/UberShader"));
        }
        else if (!ControllerInputPoller.instance.rightGrab && rPlatform)
        {
            GameObject.Destroy(rPlatform);
            rPlatform = null;
        }
    }

    public override void OnDisable()
    {
        if (lPlatform)
            lPlatform.Destroy();

        if (rPlatform)
            rPlatform.Destroy();
    }
}