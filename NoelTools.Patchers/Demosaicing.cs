using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using nel;
using NoelTools.Main;
using XX;

namespace NoelTools.Patchers;

[HarmonyPatch(typeof(MosaicShower), "setTarget")]
public static class Demosaicing
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        CodeMatcher matcher = new(instructions, generator);

        matcher.MatchStartForward([
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldfld, AccessTools.Field(typeof(MosaicShower), "Targ")),
            new(OpCodes.Brfalse),
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldfld, AccessTools.Field(typeof(MosaicShower), "Targ")),
            new(OpCodes.Call, AccessTools.PropertyGetter(typeof(X), "SENSITIVE")),
            new(OpCodes.Callvirt, AccessTools.Method(typeof(IMosaicDescriptor), "countMosaic", [typeof(bool)])),
            new(OpCodes.Br),
            new(OpCodes.Ldc_I4_0),
            new(OpCodes.Stloc_1)
        ]);

        if (matcher.IsValid)
        {
            for (int i = 0; i < 8; i++)
            {
                matcher.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop));
            }
        }
        else
        {
            NoelToolsMain.Logger.LogWarning("Couldn't demosaic! Unsupported game version.");
        }

        return matcher.InstructionEnumeration();
    }
}
