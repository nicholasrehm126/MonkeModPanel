using System;
using MelonLoader;
using ModTemplate;


[assembly: MelonInfo(typeof(Mod), "Monke Mod Extras!", "1.0.0", "Estatic", null)]
[assembly: MelonGame("Another Axiom", "Gorilla Tag")]

namespace ModTemplate;

/// <summary>
/// Creates a melon mod (needed for creating a MonkeModPanel mod)
/// </summary>

public class Mod : MelonMod;