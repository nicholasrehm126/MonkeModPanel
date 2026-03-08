using Monke_Mod_Panel.Attributes;
using UnityEngine;

namespace ModTemplate;

[Toggleable]
public class UPNDN : Monke_Mod_Panel.Mod
{
    public override string Name => "Up & Down (L & R)";

    public override void OnUpdate()
    {
        if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.5)
        {
            GorillaLocomotion.GTPlayer.Instance.bodyCollider.attachedRigidbody.velocity += GorillaLocomotion.GTPlayer.Instance.bodyCollider.transform.up * 25f * Time.deltaTime;
        }
        if (ControllerInputPoller.instance.leftControllerIndexFloat > 0.5)
        {
            GorillaLocomotion.GTPlayer.Instance.bodyCollider.attachedRigidbody.velocity += GorillaLocomotion.GTPlayer.Instance.bodyCollider.transform.up * -25f * Time.deltaTime;
        }
    }
}