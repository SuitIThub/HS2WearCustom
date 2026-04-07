using System;
using BepInEx;
using BepInEx.Harmony;
using BepInEx.Logging;
using HarmonyLib;
using Studio;

namespace HS2WearCustom
{
	// Token: 0x02000008 RID: 8
	[BepInProcess("StudioNEOV2")]
	[BepInPlugin("GarryWu.HS2WearCustom", "Studio Wear Custom", "0.1.5")]
	public class HS2WearCustom : BaseUnityPlugin
	{
		// Token: 0x0600001C RID: 28 RVA: 0x0000282C File Offset: 0x00000A2C
		internal void Main()
		{
			HS2WearCustom.Logger = base.Logger;
			Harmony harmony = Harmony.CreateAndPatchAll(typeof(AIWCHooks), null);
			harmony.Patch(typeof(MPCharCtrl).GetNestedType("CostumeInfo", AccessTools.all).GetMethod("Init"), null, new HarmonyMethod(typeof(AIWCUI).GetMethod("InitUI", AccessTools.all)), null, null);
			harmony.Patch(typeof(MPCharCtrl).GetNestedType("CostumeInfo", AccessTools.all).GetMethod("Init"), null, new HarmonyMethod(typeof(StudioCharaListUtil).GetMethod("Install", AccessTools.all)), null, null);
			harmony.Patch(typeof(CharaList).GetMethod("InitCharaList"), null, new HarmonyMethod(typeof(StudioCharaListUtil).GetMethod("Install", AccessTools.all)), null, null);
		}

		// Token: 0x04000010 RID: 16
		internal new static ManualLogSource Logger;
	}
}
