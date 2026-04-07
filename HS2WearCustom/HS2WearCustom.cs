using System;
using BepInEx;
using BepInEx.Harmony;
using BepInEx.Logging;
using HarmonyLib;
using Studio;

namespace HS2WearCustom
{
	[BepInProcess("StudioNEOV2")]
	// Original plugin by GarryWu
	[BepInPlugin("Suit-Ji.HS2WearCustom", "Studio Wear Custom", "0.2.0")]
	public class HS2WearCustom : BaseUnityPlugin
	{
		internal void Main()
		{
			HS2WearCustom.Logger = base.Logger;
			Harmony harmony = Harmony.CreateAndPatchAll(typeof(AIWCHooks), null);
			harmony.Patch(typeof(MPCharCtrl).GetNestedType("CostumeInfo", AccessTools.all).GetMethod("Init"), null, new HarmonyMethod(typeof(AIWCUI).GetMethod("InitUI", AccessTools.all)), null, null);
			harmony.Patch(typeof(MPCharCtrl).GetNestedType("CostumeInfo", AccessTools.all).GetMethod("Init"), null, new HarmonyMethod(typeof(StudioCharaListUtil).GetMethod("Install", AccessTools.all)), null, null);
			harmony.Patch(typeof(CharaList).GetMethod("InitCharaList"), null, new HarmonyMethod(typeof(StudioCharaListUtil).GetMethod("Install", AccessTools.all)), null, null);
		}

		internal new static ManualLogSource Logger;
	}
}
