using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Monke_Mod_Panel.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

[assembly: MelonInfo(typeof(Monke_Mod_Panel.Core), "Monke Mod Panel", "1.0.0", "Estatic & biotest05", null)]
[assembly: MelonGame("Another Axiom", "Gorilla Tag")]

namespace Monke_Mod_Panel
{
    public class Core : MelonMod
    {
        public static Core Instance;
        
        public static List<Mod> Mods = new List<Mod>();
        public GameObject ButtonPresser;

        GameObject menu;
        GameObject clicker;
        List<GameObject> btnObj = new List<GameObject>();

        Vector3 targetScale;
        Vector3 closedScale = Vector3.zero;
        Vector3 openScale = new Vector3(0.2f, 0.3f, 0.02f);

        bool menuOpen = false;
        bool animating = false;
        float animSpeed = 8f;

        private int currentPage = 0;
        private bool prevLeftPrimary = false;
        private bool prevLeftSecondary = false;
        private bool openRequested = false;

        public override void OnEarlyInitializeMelon()
        {
            Instance = this;
        }

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
            
            LoadMods();
            GorillaTagger.OnPlayerSpawned(CreateMenu);
            
            if (AudioUtil.GetClip("WristMenu.Resources.close.wav") == null)
                LoggerInstance.Error("Could not find WritstMenu.Resources.close.wav");
            
            if (AudioUtil.GetClip("WristMenu.Resources.open.wav") == null)
                LoggerInstance.Error("Could not find WritstMenu.Resources.open.wav");
        }

        public override void OnUpdate()
        {
            if (!menu) return;
            
            const int modsPerPage = 5;
            
            if (ControllerInputPoller.instance.leftGrab)
            {
                if (!openRequested)
                {
                    openRequested = true;
                    targetScale = openScale;
                    animating = true;
                    menu.SetActive(true);

                    AudioUtil.PlayClip("WristMenu.Resources.open.wav", menu.transform.position);
                }
            }
            else
            {
                if (openRequested)
                {
                    openRequested = false;
                    targetScale = closedScale;
                    animating = true;

                    AudioUtil.PlayClip("WristMenu.Resources.close.wav", menu.transform.position);
                }
            }

            if (Mods.Count > modsPerPage)
            {
                bool primaryPressed = ControllerInputPoller.instance.leftControllerPrimaryButton;
                bool secondaryPressed = ControllerInputPoller.instance.leftControllerSecondaryButton;

                if (primaryPressed && !prevLeftPrimary)
                {
                    currentPage--;
                    if (currentPage < 0) currentPage = (Mods.Count - 1) / modsPerPage;
                    RefreshButtons();
                    AudioUtil.PlayClip("WristMenu.Resources.woosj.wav", menu.transform.position);
                }

                if (secondaryPressed && !prevLeftSecondary)
                {
                    currentPage++;
                    if (currentPage > (Mods.Count - 1) / modsPerPage) currentPage = 0;
                    RefreshButtons();
                    AudioUtil.PlayClip("WristMenu.Resources.woosj.wav", menu.transform.position);
                }

                prevLeftPrimary = primaryPressed;
                prevLeftSecondary = secondaryPressed;
            }
            
            if (animating)
            {
                animSpeed = targetScale == openScale ? 8f : 16f;
                
                menu.transform.localScale = Vector3.Lerp(
                    menu.transform.localScale,
                    targetScale,
                    1f - Mathf.Exp(-animSpeed * Time.deltaTime)
                );

                if (Vector3.Distance(menu.transform.localScale, targetScale) < 0.01f)
                {
                    menu.transform.localScale = targetScale;
                    animating = false;

                    if (targetScale == closedScale)
                        menu.SetActive(false);
                }
            }

            for (int i = 0; i < Mods.Count; i++)
            {
                if (Mods[i].Enabled) Mods[i].OnUpdate();
            }
        }

        void CreateMenu()
        {
            menu = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ButtonPresser = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            ButtonPresser.transform.SetParent(GorillaTagger.Instance.rightHandTriggerCollider.transform, false);
            ButtonPresser.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

            menu.transform.localScale = closedScale;
            menu.transform.SetParent(GorillaTagger.Instance.leftHandTransform, false);
            menu.transform.localRotation = Quaternion.Euler(0, 90, 90);
            menu.transform.localPosition = new Vector3(0.05f, 0f, 0f);
            
            menu.GetComponent<Renderer>().material = new Material(Shader.Find("GorillaTag/UberShader"));
            menu.GetComponent<Renderer>().material.color = Color.black;
            ButtonPresser.GetComponent<Renderer>().material = new Material(Shader.Find("GorillaTag/UberShader"));
            ButtonPresser.GetComponent<Renderer>().material.color = Color.red;

            menu.GetComponent<Collider>().Destroy();
            
            Rigidbody rb = ButtonPresser.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
            ButtonPresser.GetComponent<Collider>().isTrigger = true;

            GameObject textObject = new GameObject("Title");
            textObject.transform.SetParent(menu.transform, false);

            TextMeshPro tmp = textObject.AddComponent<TextMeshPro>();
            tmp.text = "Monke Mod Panel";
            tmp.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            tmp.color = Color.white;
            tmp.fontSize = 0.7f;
            tmp.fontStyle = FontStyles.Normal;
            tmp.alignment = TextAlignmentOptions.Center;
            
            textObject.transform.localScale = new Vector3(5f / 3f, 3f / 3f, 50f / 3f);
            textObject.transform.localRotation = Quaternion.Euler(0, 180, 0);
            textObject.transform.localPosition += new Vector3(0f, 0.45f, 0.85f);
            
            RefreshButtons();
            
            menu.SetActive(false);
        }
        
        /// <summary>
        /// This is used for:
        /// 1. When running the function for the first time, it will create the buttons.
        /// 2. When running the function a second time (or more), it will destroy all previous buttons and create new ones.
        /// </summary>
        void RefreshButtons()
        {
            if (menu == null) return;
            const int modsPerPage = 5;

            foreach (GameObject btn in btnObj)
                GameObject.Destroy(btn);
            btnObj.Clear();

            int startIndex = currentPage * modsPerPage;
            int endIndex = Mathf.Min(startIndex + modsPerPage, Mods.Count);

            float buttonHeight = 0.1f;
            float spacing = buttonHeight + 0.06f;
            float startY = 0.35f;

            for (int i = startIndex; i < endIndex; i++)
            {
                Mod mod = Mods[i];
                
                GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cube);
                button.transform.SetParent(menu.transform, false);
                // 0.2f, 0.3f, 0.02f
                button.transform.localScale = new Vector3(0.75f, buttonHeight, 0.75f);
                button.transform.localPosition = new Vector3(0f, startY - (i - startIndex) * spacing, 0.75f);
                
                Renderer rend = button.GetComponent<Renderer>();
                rend.material = new Material(Shader.Find("GorillaTag/UberShader"));
                rend.material.color = mod.Enabled ? Color.green : Color.red;
                
                BoxCollider collider = button.GetComponent<BoxCollider>();
                collider.isTrigger = true;

                ModButtonTrigger trigger = button.AddComponent<ModButtonTrigger>();
                trigger.Mod = mod;
                trigger.Renderer = rend;
                
                GameObject textObject = new GameObject("Text");
                textObject.transform.SetParent(button.transform, false);

                TextMeshPro tmp = textObject.AddComponent<TextMeshPro>();
                tmp.text = mod.Name;
                tmp.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
                tmp.color = Color.black;
                tmp.fontSize = 0.35f;
                tmp.fontStyle = FontStyles.Normal;
                tmp.alignment = TextAlignmentOptions.Center;

                textObject.transform.localScale = new Vector3(3f, 16f, 150f);
                textObject.transform.localRotation = Quaternion.Euler(0, 180, 0);
                textObject.transform.localPosition = new Vector3(0f, 0f, 0.76f);

                btnObj.Add(button);
            }
        }

        void LoadMods()
        {
            LoggerInstance.Msg("Loading mods...");

            IEnumerable<Assembly> assemblies = MelonMod.RegisteredMelons.Select(x => x.MelonAssembly.Assembly);

            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.IsAbstract)
                            continue;

                        if (!typeof(Mod).IsAssignableFrom(type))
                            continue;

                        Mod mod = (Mod)Activator.CreateInstance(type);
                        mod.Toggleable = type.GetCustomAttributes(typeof(ToggleableAttribute), true).Length > 0;
                        Mods.Add(mod);

                        LoggerInstance.Msg($"Loaded mod: {mod.Name}");
                    }
                }
                catch (Exception ex)
                {
                    LoggerInstance.Warning($"Failed scanning {assembly.FullName}\n{ex}");
                }
            }
            
            LoggerInstance.Msg($"Loaded {Mods.Count} mods.");
        }
    }
}