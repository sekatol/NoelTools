using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using nel.gm;
using NoelTools.Main;

namespace NoelTools.Patchers;

[HarmonyPatch(typeof(UiBenchMenu), "setEnableBtns")]
public static class ForceEnableBenchCommands
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        CodeMatcher matcher = new(instructions, generator);

        matcher.MatchStartForward([
            new(OpCodes.Ldloc_S),
            new(OpCodes.Ldfld),
            new(OpCodes.Brfalse), // Brfalse_S should only be matched with Brfalse
            new(OpCodes.Ldloc_S),
            new(OpCodes.Ldfld),
            new(OpCodes.Brfalse), // Brfalse_S should only be matched with Brfalse
            new(OpCodes.Ldloc_S),
            new(OpCodes.Ldfld),
            new(OpCodes.Ldloc_1),
            new(OpCodes.Callvirt),
            new(OpCodes.Brfalse), // Brfalse_S should only be matched with Brfalse
            new(OpCodes.Ldloc_S),
            new(OpCodes.Ldfld),
            new(OpCodes.Brfalse), // Brfalse_S should only be matched with Brfalse
            new(OpCodes.Ldsfld),
            new(OpCodes.Isinst),
            new(OpCodes.Callvirt),
            new(OpCodes.Br), // Br_S should only be matched with Br
            new(OpCodes.Ldc_I4_1),
            new(OpCodes.Br), // Br_S should only be matched with Br
            new(OpCodes.Ldc_I4_0),
            new(OpCodes.Stfld),
        ]);

        if (matcher.IsValid)
        {
            for (int i = 0; i < 20; i++)
            {
                matcher.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop));
            }
            matcher.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_1));
        }
        else
        {
            NoelToolsMain.Logger.LogWarning("Couldn't enable bench commands! Unsupported game version.");
        }

        return matcher.InstructionEnumeration();
    }
}
