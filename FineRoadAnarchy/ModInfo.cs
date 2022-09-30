using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using System;
using System.Reflection;
using UnityEngine;

[assembly: AssemblyVersion("2.0.2.1")]
namespace FineRoadAnarchy
{
    public class ModInfo :  LoadingExtensionBase, IUserMod
    {
        public ModInfo()
        {
            try
            {
                // Creating setting file
                GameSettings.AddSettingsFile(new SettingsFile[] { new SettingsFile() { fileName = FineRoadAnarchy.settingsFileName } });
            }
            catch (Exception e)
            {
                DebugUtils.Log("Couldn't load/create the setting file.");
                DebugUtils.LogException(e);
            }
        }

        public string Name => "Klyte's Fine Road Anarchy " + Version;

        public string Description => "This mod adds additional options when building road";

        public void OnEnabled() {
            if (LoadingManager.instance.m_loadingComplete) {
                OnLevelLoaded((LoadMode)SimulationManager.instance.m_metaData.m_updateMode);
            }
        }

        public void OnDisabled() => GameObject.Destroy(FineRoadAnarchy.instance);

        public override void OnLevelLoaded(LoadMode mode) {
            if (FineRoadAnarchy.instance == null) {
                // Creating the instance
                FineRoadAnarchy.instance = new GameObject("FineRoadAnarchy").AddComponent<FineRoadAnarchy>();
            } else {
                FineRoadAnarchy.instance.Start();
            }

            if (mode == LoadMode.LoadAsset || mode == LoadMode.NewAsset) {
                GameAreaManager.instance.m_maxAreaCount = GameAreaManager.AREAGRID_RESOLUTION * GameAreaManager.AREAGRID_RESOLUTION;
                for (int i = 0; i < GameAreaManager.instance.m_maxAreaCount; i++) {
                    GameAreaManager.instance.m_areaGrid[i] = i + 1;
                }
                GameAreaManager.instance.m_areaCount = GameAreaManager.instance.m_maxAreaCount;
            }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            try
            {
                var group = helper.AddGroup(Name) as UIHelper;
                var panel = group.self as UIPanel;

                var checkBox = (UICheckBox) group.AddCheckbox("Disable debug messages logging", DebugUtils.hideDebugMessages.value, (b) =>
                 {
                     DebugUtils.hideDebugMessages.value = b;
                 });
                checkBox.tooltip = "If checked, debug messages won't be logged.";

                group.AddSpace(10);

                panel.gameObject.AddComponent<OptionsKeymapping>();

                group.AddSpace(10);
            }
            catch (Exception e)
            {
                DebugUtils.Log("OnSettingsUI failed");
                DebugUtils.LogException(e);
            }
        }
        public static string MinorVersion => MajorVersion + "." + typeof(ModInfo).Assembly.GetName().Version.Build;
        public static string MajorVersion => typeof(ModInfo).Assembly.GetName().Version.Major + "." + typeof(ModInfo).Assembly.GetName().Version.Minor;
        public static string FullVersion => MinorVersion + " r" + typeof(ModInfo).Assembly.GetName().Version.Revision;

        public static string Version
        {
            get {
                if (typeof(ModInfo).Assembly.GetName().Version.Minor == 0 && typeof(ModInfo).Assembly.GetName().Version.Build == 0)
                {
                    return typeof(ModInfo).Assembly.GetName().Version.Major.ToString();
                }
                if (typeof(ModInfo).Assembly.GetName().Version.Build > 0)
                {
                    return MinorVersion;
                }
                else
                {
                    return MajorVersion;
                }
            }
        }
    }
}
