using System;
using System.Collections;
using AIChara;
using BepInEx.Logging;
using HarmonyLib;
using Studio;
using UnityEngine;

namespace HS2WearCustom
{
	// Token: 0x02000003 RID: 3
	public static class AIWCHooks
	{
		private static void LogDiag(string msg)
		{
			ManualLogSource logger = HS2WearCustom.Logger;
			if (logger != null)
			{
				logger.LogInfo("[WearCustom/Diag] " + msg);
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002188 File Offset: 0x00000388
		[HarmonyPostfix]
		[HarmonyPatch(typeof(MPCharCtrl), "OnClickRoot", new Type[]
		{
			typeof(int)
		})]
		internal static void OnClickRootPostfix(int _idx)
		{
			AIWCUI.OnclickedRoot(_idx);
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(ChaControl), "ChangeAccessoryAsync", new Type[]
		{
			typeof(int),
			typeof(int),
			typeof(int),
			typeof(string),
			typeof(bool),
			typeof(bool)
		})]
		internal static void ChangeAccessoryAsyncSlotPrefix(ChaControl __instance, int slotNo, int type, int id, string parentKey, bool forceChange, bool asyncFlags)
		{
			LogDiag(string.Format("ChaControl.ChangeAccessoryAsync(slot={0}, type={1}, id={2}, parent=\"{3}\", force={4}, async={5}) sex={6} gameObject={7}",
				slotNo, type, id, parentKey ?? "", forceChange, asyncFlags, __instance.sex, __instance.gameObject.name));
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(ChaControl), "ChangeAccessoryAsync", new Type[]
		{
			typeof(int),
			typeof(int),
			typeof(int),
			typeof(string),
			typeof(bool),
			typeof(bool)
		})]
		internal static void ChangeAccessoryAsyncSlotPostfix(IEnumerator __result)
		{
			LogDiag("ChaControl.ChangeAccessoryAsync(slot,...) returned IEnumerator=" + ((__result != null) ? __result.GetType().FullName : "null"));
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(ChaControl), "LoadCharaFbxDataAsync", new Type[]
		{
			typeof(Action<GameObject>),
			typeof(int),
			typeof(int),
			typeof(string),
			typeof(bool),
			typeof(byte),
			typeof(Transform),
			typeof(int),
			typeof(bool),
			typeof(bool)
		})]
		internal static void LoadCharaFbxDataAsyncPrefix(Action<GameObject> __0, int __1, int __2, string __3, bool __4, byte __5, Transform __6, int __7, bool __8, bool __9)
		{
			LogDiag(string.Format("LoadCharaFbxDataAsync(category={0}, id={1}, createName=\"{2}\", copyDynamicBone={3}, copyWeights={4}, parent={5}, defaultId={6}, asyncFlags={7}, worldPositionStays={8}, callback={9})",
				__1, __2, __3 ?? "", __4, __5, (__6 != null) ? __6.name : "null", __7, __8, __9, (__0 != null)));
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(ChaControl), "LoadCharaFbxDataAsync", new Type[]
		{
			typeof(Action<GameObject>),
			typeof(int),
			typeof(int),
			typeof(string),
			typeof(bool),
			typeof(byte),
			typeof(Transform),
			typeof(int),
			typeof(bool),
			typeof(bool)
		})]
		internal static void LoadCharaFbxDataAsyncPostfix(IEnumerator __result)
		{
			LogDiag("LoadCharaFbxDataAsync returned IEnumerator=" + ((__result != null) ? __result.GetType().FullName : "null"));
		}
	}
}
