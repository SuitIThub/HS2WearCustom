using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Studio;

namespace HS2WearCustom
{
	[BepInProcess("StudioNEOV2")]
	// Original plugin by GarryWu
	[BepInPlugin("Suit-Ji.HS2WearCustom", "Studio Wear Custom", "0.2.1")]
	public class HS2WearCustom : BaseUnityPlugin
	{
		private void Awake()
		{
			HS2WearCustom.Logger = base.Logger;
			WearCustomSearchBarConfig.Log = base.Logger;
			WearCustomSearchBarConfig.AdditionalParentPaths = Config.Bind(
				"Search Bars",
				"Additional Parent Paths",
				"",
				"Optional extra GameObject paths (one per line) for injected list search bars. Each line is a full hierarchy path from the scene root, with segments separated by '/', as used by GameObject.Find. Discover paths with Unity Explorer / RuntimeUnityEditor or similar.");

			Harmony harmony = new Harmony("Suit-Ji.HS2WearCustom");
			harmony.PatchAll(typeof(AIWCHooks));
			Type costumeInfo = typeof(MPCharCtrl).GetNestedType("CostumeInfo", AccessTools.all);
			MethodBase costumeInit = AccessTools.Method(costumeInfo, "Init");
			harmony.Patch(costumeInit, postfix: new HarmonyMethod(typeof(AIWCUI).GetMethod("InitUI", AccessTools.all)));
			harmony.Patch(costumeInit, postfix: new HarmonyMethod(typeof(StudioCharaListUtil).GetMethod("Install", AccessTools.all)));
			harmony.Patch(AccessTools.Method(typeof(CharaList), "InitCharaList"), postfix: new HarmonyMethod(typeof(StudioCharaListUtil).GetMethod("Install", AccessTools.all)));
		}

		private void Start()
		{
			gameObject.AddComponent<MultiPathSearchBarManager>();
		}

		internal new static ManualLogSource Logger;
	}
}