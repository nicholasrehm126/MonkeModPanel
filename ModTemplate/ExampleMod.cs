namespace ModTemplate;

/// <summary>
/// Creates an example mod usable by MonkeModPanel
/// If you want this mod to be toggleable, add [Toggleable] above the class.
/// </summary>
public class ExampleMod : Monke_Mod_Panel.Mod
{
    /// <summary>
    /// The name of the mod thats visible in the mod panel.
    /// </summary>
    public override string Name => "Leave Room";

    /// <summary>
    /// Runs whenever the player clicks on the mod.
    /// If the mod is toggleable, it will run either OnEnable or OnDisable depending on if its enabled.
    /// </summary>
    public override void OnClicked()
    {
        base.OnClicked();

        NetworkSystem.Instance.ReturnToSinglePlayer();
    }
}