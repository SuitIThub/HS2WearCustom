using System;
using HarmonyLib;
using Studio;

namespace HS2WearCustom
{
	public static class AIWCHooks
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(MPCharCtrl), "OnClickRoot", new Type[]
		{
			typeof(int)
		})]
		internal static void OnClickRootPostfix(int _idx)
		{
			AIWCUI.OnclickedRoot(_idx);
		}
	}
}
