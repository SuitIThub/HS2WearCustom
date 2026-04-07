using System;
using System.Reflection;
using BepInEx;
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
			Harmony harmony = new Harmony("Suit-Ji.HS2WearCustom");
			harmony.PatchAll(typeof(AIWCHooks));
			Type costumeInfo = typeof(MPCharCtrl).GetNestedType("CostumeInfo", AccessTools.all);
			MethodBase costumeInit = AccessTools.Method(costumeInfo, "Init");
			harmony.Patch(costumeInit, postfix: new HarmonyMethod(typeof(AIWCUI).GetMethod("InitUI", AccessTools.all)));
			harmony.Patch(costumeInit, postfix: new HarmonyMethod(typeof(StudioCharaListUtil).GetMethod("Install", AccessTools.all)));
			harmony.Patch(AccessTools.Method(typeof(CharaList), "InitCharaList"), postfix: new HarmonyMethod(typeof(StudioCharaListUtil).GetMethod("Install", AccessTools.all)));
		}

		internal new static ManualLogSource Logger;
	}
}