﻿using System;
using System.Collections;
using System.Text;
using HarmonyLib;
using ResoniteModLoader;

namespace BoundedUIX
{
    public class BoundedUIX : ResoniteMod
    {
        public static ModConfiguration Config;

        internal const string TargetSlotNamePlaceholder = "{TargetName}";

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<string> childSlotNameKey = new("ChildSlotName", "Default name for child Slots in UIX hierarchies. Use {TargetName} to get the parent's name.", () => "Panel", valueValidator: name => !string.IsNullOrWhiteSpace(name));

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> enableUIXGizmosKey = new("EnableUIXGizmos", "Enable modifying Slot Gizmo behavior in UIX hierarchies to move the elements instead.", () => true);

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> enableUIXSelectionKey = new("EnableUIXSelection", "Enable selecting elements in UIX hierarchies directly with the Developer ToolTip.", () => true);

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<float> gizmoOffsetKey = new("GizmoOffset", "Distance to raise the movement gizmos from the surface of the canvas. World scale.", () => 0.02f, valueValidator: v => v >= 0);

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> ignoreAlreadySelectedKey = new("IgnoreAlreadySelected", "Skip already selected elements in the targeting process. Helps with layered elements.", () => true);

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> moveTransformToParentKey = new("MoveTransformToParent", "Move RectTransform values up when creating a parent (analog to Slot transforms).", () => true);

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<string> parentSlotNameKey = new("ParentSlotName", "Default name for parent Slots in UIX hierarchies. Use {TargetName} to get the child-to-be's name.", () => "{TargetName} Space", valueValidator: name => !string.IsNullOrWhiteSpace(name));

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<string> pivotSlotNameKey = new("PivotSlotName", "Default name for pivot Slots in UIX hierarchies. Use {TargetName} to get the child-to-be's name.", () => "{TargetName} Space", valueValidator: name => !string.IsNullOrWhiteSpace(name));

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> prioritizeHierarchyDepthKey = new("PrioritizeHierarchyDepth", "Prioritize the hierarchy depth of a potentially hit RectTransform over the layout order. Can help instead of or in addition to skipping already selected elements.", () => false);
        
        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> prioritizeButtonsKey = new("PrioritizeButtons", "When searching for a potentially hit RectTransform, prioritize slots with Button components at a higher hierarchy.", () => true);

        public static string ChildSlotName => Config.GetValue(childSlotNameKey);
        public static bool EnableUIXGizmos => Config.GetValue(enableUIXGizmosKey);
        public static bool EnableUIXSelection => Config.GetValue(enableUIXSelectionKey);
        public static float GizmoOffset => Config.GetValue(gizmoOffsetKey);
        public static bool IgnoreAlreadySelected => Config.GetValue(ignoreAlreadySelectedKey);
        public static bool MoveTransformToParent => Config.GetValue(moveTransformToParentKey);
        public static string ParentSlotName => Config.GetValue(parentSlotNameKey);
        public static string PivotSlotName => Config.GetValue(pivotSlotNameKey);
        public static bool PrioritizeHierarchyDepth => Config.GetValue(prioritizeHierarchyDepthKey);
        public static bool PrioritizeButtons => Config.GetValue(prioritizeButtonsKey);

        public override string Author => "Banane9 & djsime1";
        public override string Link => "https://github.com/djsime1/BoundedUIX";
        public override string Name => "BoundedUIX";
        public override string Version => "1.1.2";

        public override void OnEngineInit()
        {
            var harmony = new Harmony("je.dj.BoundedUIX");
            Config = GetConfiguration();
            Config.Save(true);
            harmony.PatchAll();
        }
    }
}