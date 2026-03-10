namespace Monke_Mod_Panel;

public abstract class Mod
{
    /// <summary>
    /// The name that shows up in the panel.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Set by the mod loader if the Toggleable attribute exists.
    /// </summary>
    public bool Toggleable { get; set; }

    /// <summary>
    /// Current enabled state.
    /// </summary>
    public bool Enabled { get; private set; }

    public virtual void OnEnable() { }
    public virtual void OnUpdate() { }
    public virtual void OnDisable() { }
    
    public virtual void OnModdedLeave(){}
    public virtual void OnModdedJoin(){}
    
    public virtual void OnClicked()
    {
        if (Toggleable)
        {
            Enabled = !Enabled;

            if (Enabled)
                OnEnable();
            else
                OnDisable();
        }
    }
}