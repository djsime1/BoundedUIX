using FrooxEngine.UIX;
using FrooxEngine;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elements.Core;

namespace BoundedUIX
{
    [HarmonyPatch(typeof(RectTransform))]
    internal static class RectTransformPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(RectTransform.BuildInspectorUI))]
        private static void BuildInspectorUIPostfix(RectTransform __instance, UIBuilder ui)
        {
            var visualizeButton = ui.Button("Visualize Preferred Area");
            var valueField = visualizeButton.Slot.AttachComponent<ValueField<bool>>().Value;

            var toggle = visualizeButton.Slot.AttachComponent<ButtonToggle>();
            toggle.TargetValue.Target = valueField;

            valueField.OnValueChange += field =>
            {
                visualizeButton.Enabled = false;

                __instance.StartTask(async () =>
                {
                    while (!visualizeButton.IsRemoved && !__instance.IsRemoved && (!__instance?.Canvas.IsRemoved ?? false))
                    {
                        var horizontal = __instance.GetHorizontalMetrics().preferred;
                        var vertical = __instance.GetVerticalMetrics().preferred;
                        var area = __instance.ComputeGlobalComputeRect();
                        var color = colorX.Blue;

                        if (horizontal <= 0 && vertical <= 0)
                        {
                            horizontal = area.width;
                            vertical = area.height;
                            color = colorX.Red;
                        }
                        else if (horizontal <= 0)
                        {
                            horizontal = __instance.Canvas.UnitScale;
                            color = colorX.Purple;
                        }
                        else if (vertical <= 0)
                        {
                            vertical = __instance.Canvas.UnitScale;
                            color = colorX.Purple;
                        }

                        var pos = __instance.Canvas.Slot.LocalPointToGlobal(new float3(area.Center / __instance.Canvas.UnitScale));
                        pos -= 0.5f * BoundedUIX.GizmoOffset * __instance.Canvas.Slot.Forward;

                        var size = __instance.Canvas.Slot.LocalScaleToGlobal(new float3(horizontal, vertical) / __instance.Canvas.UnitScale);

                        __instance.World.Debug.Box(pos, size, color.SetA(0.5f), __instance.Canvas.Slot.GlobalRotation);

                        await default(NextUpdate);
                    }
                });
            };

            var fixUIXButton = ui.Button("Fix UIX");

            fixUIXButton.Slot.ActiveSelf_Field.OverrideForUser(__instance.Slot.World.LocalUser, true).Default.Value = false;
            fixUIXButton.LocalPressed += (button, data) => {
                var canvas = AccessTools.Field(__instance.GetType(), "_registeredCanvas").GetValue(__instance) as Canvas;

                AccessTools.Method(typeof(Canvas), "RegisterDirtyTransform").Invoke(canvas, new object[] { __instance });
            };
        }
    }
}