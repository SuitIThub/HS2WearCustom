using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using AIChara;
using BepInEx.Logging;
using CharaCustom;
using Manager;
using Studio;
using UnityEngine;
using UnityEngine.UI;

namespace HS2WearCustom
{
	// Token: 0x0200001C RID: 28
	public class AIWearPartList : MonoBehaviour
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600008A RID: 138 RVA: 0x00002925 File Offset: 0x00000B25
		// (set) Token: 0x0600008B RID: 139 RVA: 0x00005B82 File Offset: 0x00003D82
		public bool active
		{
			get
			{
				return base.gameObject.activeSelf;
			}
			set
			{
				if (base.gameObject.activeSelf != value)
				{
					base.gameObject.SetActive(value);
				}
			}
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00005BA0 File Offset: 0x00003DA0
		public void InitList(int _sex, ChaListDefine.CategoryNo typeNo, int kind)
		{
			this.Init();
			this.scrollRect.verticalNormalizedPosition = 1f;
			this.selectType = typeNo;
			this.partKind = kind;
			this.select = -1;
			this.partLst = CvsBase.CreateSelectList(typeNo, ChaListDefine.KeyType.Unknown);
			int num = -1;
			if ((this.selectType >= ChaListDefine.CategoryNo.mo_top && this.selectType <= ChaListDefine.CategoryNo.mo_shoes) || (this.selectType >= ChaListDefine.CategoryNo.fo_top && this.selectType <= ChaListDefine.CategoryNo.fo_shoes))
			{
				this.wearType = 1;
				num = this.mpCharCtrl.ociChar.charInfo.nowCoordinate.clothes.parts[kind].id;
			}
			else if (this.selectType >= ChaListDefine.CategoryNo.so_hair_b && this.selectType <= ChaListDefine.CategoryNo.so_hair_o)
			{
				this.wearType = 2;
				num = this.mpCharCtrl.ociChar.charInfo.chaFile.custom.hair.parts[kind].id;
			}
			else if (this.selectType >= ChaListDefine.CategoryNo.ao_head && this.selectType <= ChaListDefine.CategoryNo.ao_kokan)
			{
				this.wearType = 3;
				int slot = this.wearCtrl.accessorySlot;
				ChaFileAccessory.PartsInfo slotParts = this.mpCharCtrl.ociChar.charInfo.nowCoordinate.accessory.parts[slot];
				if (slotParts.type == (int)typeNo)
				{
					num = slotParts.id;
				}
			}
			for (int i = 0; i < this.partLst.Count; i++)
			{
				this.CreateNode(this.partLst[i], i, num == this.partLst[i].id);
			}
			if (this.select >= 0)
			{
				this.dicNode[this.select].color = Color.green;
			}
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00005CF9 File Offset: 0x00003EF9
		public void updateInfo()
		{
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00005CFC File Offset: 0x00003EFC
		private void Init()
		{
			int childCount = this.transformRoot.childCount;
			for (int i = 0; i < childCount; i++)
			{
				UnityEngine.Object.Destroy(this.transformRoot.GetChild(i).gameObject);
			}
			this.transformRoot.DetachChildren();
			this.dicNode = new Dictionary<int, Image>();
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00005D50 File Offset: 0x00003F50
		private void OnSelect(int infoNo)
		{
			if (this.select != infoNo)
			{
				if (this.select >= 0)
				{
					this.dicNode[this.select].color = Color.white;
				}
				this.select = infoNo;
				this.dicNode[this.select].color = Color.green;
				this.ChangeClothLink(this.partLst[infoNo]);
			}
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00005DC0 File Offset: 0x00003FC0
		private void CreateNode(CustomSelectInfo info, int no, bool isF)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.objectPrefab);
			if (!gameObject.activeSelf)
			{
				gameObject.SetActive(true);
			}
			gameObject.transform.SetParent(this.transformRoot, false);
			ListNode component = gameObject.GetComponent<ListNode>();
			component.AddActionToButton(delegate
			{
				this.OnSelect(no);
			});
			component.text = info.name;
			if (isF)
			{
				this.select = no;
			}
			component.image.color = Color.white;
			this.dicNode.Add(no, component.image);
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00005E6C File Offset: 0x0000406C
		private void ChangeClothLink(CustomSelectInfo info)
		{
			if (info == null)
			{
				return;
			}
			ChaControl charInfo = this.mpCharCtrl.ociChar.charInfo;
			if (this.wearType == 1)
			{
				charInfo.ChangeClothes(this.partKind, info.id, true);
				charInfo.AssignCoordinate();
			}
			else if (this.wearType == 2)
			{
				charInfo.ChangeHair(this.partKind, info.id, false);
				charInfo.SetHairAcsDefaultColorParameterOnly(this.partKind);
				charInfo.ChangeSettingHairAcsColor(this.partKind);
			}
			else if (this.wearType == 3)
			{
				this.wearCtrl.UpdateInfo();
				int slot = this.wearCtrl.accessorySlot;
				int accType = (int)this.selectType;
				ListInfoBase listInfo = AIWearPartList.TryGetAccessoryListInfo(this.selectType, info.id);
				string parentKey = AIWearPartList.TryGetParentKeyFromListInfo(listInfo);
				if (string.IsNullOrEmpty(parentKey))
				{
					parentKey = AIWearPartList.ResolveAccessoryParentKey(charInfo, slot, accType, info.id);
				}
				AIWearPartList.LogAcc(string.Format(
					"ChangeClothLink: slot={0} accType={1} listItem id={2} name=\"{3}\" category(selectType)={4} listInfo={5} resolvedParentKey=\"{6}\"",
					slot,
					accType,
					info.id,
					info.name ?? "",
					(int)this.selectType,
					(listInfo != null) ? "ok" : "null",
					parentKey ?? ""));
				base.StartCoroutine(this.ApplyAccessoryAfterLoad(charInfo, slot, accType, info.id, parentKey, listInfo));
				return;
			}
			this.wearCtrl.UpdateInfo();
		}

		private static ListInfoBase TryGetAccessoryListInfo(ChaListDefine.CategoryNo categoryNo, int listId)
		{
			if (listId <= 0)
			{
				return null;
			}
			Character mc = UnityEngine.Object.FindObjectOfType<Character>();
			if (mc == null)
			{
				AIWearPartList.LogAccWarn("TryGetAccessoryListInfo: Manager.Character not found in scene.");
				return null;
			}
			ChaListControl clc = mc.chaListCtrl;
			if (clc == null)
			{
				AIWearPartList.LogAccWarn("TryGetAccessoryListInfo: chaListCtrl is null.");
				return null;
			}
			ListInfoBase listInfo = clc.GetListInfo(categoryNo, listId);
			if (listInfo == null)
			{
				AIWearPartList.LogAccWarn(string.Format("TryGetAccessoryListInfo: GetListInfo(category={0}, id={1}) returned null.", (int)categoryNo, listId));
			}
			else
			{
				AIWearPartList.LogAcc(string.Format("TryGetAccessoryListInfo: GetListInfo(category={0}, id={1}) ok, Name=\"{2}\"", (int)categoryNo, listId, listInfo.Name ?? ""));
			}
			return listInfo;
		}

		private static string TryGetParentKeyFromListInfo(ListInfoBase listInfo)
		{
			if (listInfo == null)
			{
				return string.Empty;
			}
			try
			{
				string text = listInfo.GetInfo(ChaListDefine.KeyType.Parent);
				if (!string.IsNullOrEmpty(text))
				{
					AIWearPartList.LogAcc("TryGetParentKeyFromListInfo: KeyType.Parent -> \"" + text + "\"");
					return text;
				}
			}
			catch (Exception ex)
			{
				AIWearPartList.LogAccWarn("TryGetParentKeyFromListInfo: " + ex.Message);
			}
			return string.Empty;
		}

		private static void TryUnifyCoordinateReferences(ChaControl cha)
		{
			if (cha == null)
			{
				return;
			}
			try
			{
				if (!object.ReferenceEquals(cha.nowCoordinate, cha.chaFile.coordinate))
				{
					cha.chaFile.coordinate = cha.nowCoordinate;
					AIWearPartList.LogAcc("ApplyAccessory: chaFile.coordinate = nowCoordinate (ChaControl must use one ChaFileCoordinate; accessory FBX path reads chaFile.coordinate)");
				}
			}
			catch (Exception ex)
			{
				AIWearPartList.LogAccWarn("ApplyAccessory: TryUnifyCoordinateReferences: " + ex.Message);
			}
		}

		private static void TryUnifyAccessoryBuffers(ChaControl cha)
		{
			if (cha == null)
			{
				return;
			}
			try
			{
				if (!object.ReferenceEquals(cha.nowCoordinate.accessory, cha.chaFile.coordinate.accessory))
				{
					cha.chaFile.coordinate.accessory = cha.nowCoordinate.accessory;
					AIWearPartList.LogAcc("ApplyAccessory: chaFile.coordinate.accessory shares nowCoordinate.accessory (fallback when coordinate refs still differ)");
				}
			}
			catch (Exception ex)
			{
				AIWearPartList.LogAccWarn("ApplyAccessory: TryUnifyAccessoryBuffers: " + ex.Message);
			}
		}

		private static void ApplyAccessorySlotParts(ChaControl cha, int slot, int accType, int id, string parentKey)
		{
			if (cha == null || slot < 0)
			{
				return;
			}
			ChaFileAccessory.PartsInfo pNow = cha.nowCoordinate.accessory.parts[slot];
			pNow.type = accType;
			pNow.id = id;
			pNow.parentKey = parentKey;
			if (!object.ReferenceEquals(cha.nowCoordinate.accessory, cha.chaFile.coordinate.accessory))
			{
				ChaFileAccessory.PartsInfo pFile = cha.chaFile.coordinate.accessory.parts[slot];
				pFile.type = accType;
				pFile.id = id;
				pFile.parentKey = parentKey;
			}
		}

		private static void EnsureChaListCtrlForAccessoryLoad(ChaControl cha)
		{
			if (cha == null)
			{
				return;
			}
			if (cha.lstCtrl != null)
			{
				AIWearPartList.LogAcc("ApplyAccessory: ChaControl.lstCtrl already set.");
				return;
			}
			Manager.Character mc = UnityEngine.Object.FindObjectOfType<Manager.Character>();
			if (mc != null && mc.chaListCtrl != null)
			{
				PropertyInfo pi = typeof(ChaControl).GetProperty("lstCtrl", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (pi != null)
				{
					pi.SetValue(cha, mc.chaListCtrl, null);
				}
				AIWearPartList.LogAcc("ApplyAccessory: ChaControl.lstCtrl was null; assigned Manager.Character.chaListCtrl (needed for accessory FBX load).");
				return;
			}
			AIWearPartList.LogAccWarn("ApplyAccessory: ChaControl.lstCtrl is null and Manager.Character.chaListCtrl unavailable; accessory load will likely fail.");
		}

		private static string ResolveAccessoryParentKey(ChaControl cha, int slot, int accType, int id)
		{
			if (id <= 0)
			{
				AIWearPartList.LogAcc("ResolveParent: id<=0, returning empty parent key.");
				return string.Empty;
			}
			try
			{
				string key = cha.GetAccessoryDefaultParentStr(accType, id);
				if (!string.IsNullOrEmpty(key))
				{
					AIWearPartList.LogAcc(string.Format("ResolveParent: GetAccessoryDefaultParentStr(type={0}, id={1}) -> \"{2}\"", accType, id, key));
					return key;
				}
				AIWearPartList.LogAcc(string.Format("ResolveParent: GetAccessoryDefaultParentStr(type={0}, id={1}) returned empty.", accType, id));
			}
			catch (Exception ex)
			{
				AIWearPartList.LogAccWarn(string.Format("ResolveParent: GetAccessoryDefaultParentStr(type={0}, id={1}) failed: {2}", accType, id, ex.Message));
			}
			try
			{
				string key = cha.GetAccessoryDefaultParentStr(slot);
				if (!string.IsNullOrEmpty(key))
				{
					AIWearPartList.LogAcc(string.Format("ResolveParent: GetAccessoryDefaultParentStr(slot={0}) -> \"{1}\"", slot, key));
					return key;
				}
				AIWearPartList.LogAcc(string.Format("ResolveParent: GetAccessoryDefaultParentStr(slot={0}) returned empty.", slot));
			}
			catch (Exception ex)
			{
				AIWearPartList.LogAccWarn(string.Format("ResolveParent: GetAccessoryDefaultParentStr(slot={0}) failed: {1}", slot, ex.Message));
			}
			return string.Empty;
		}

		private IEnumerator ApplyAccessoryAfterLoad(ChaControl cha, int slot, int accType, int id, string parentKey, ListInfoBase listInfo)
		{
			// Pipeline: (1) unify chaFile.coordinate with nowCoordinate, (2) write ChaFileAccessory.parts[slot], (3) AssignCoordinate,
			// (4) ChaControl.Reload(false,true,true,true,true) + AssignCoordinate — same as StudioCharaListUtil after editing nowCoordinate,
			// (5) only if IsAccessory still false, run ChangeAccessoryAsync (slot,…). Async/wait alone never fixed Studio without Reload.
			AIWearPartList.LogAcc(string.Format("ApplyAccessory: start slot={0} type={1} id={2} parentKey=\"{3}\"", slot, accType, id, parentKey ?? ""));
			AIWearPartList.TryUnifyCoordinateReferences(cha);
			AIWearPartList.TryUnifyAccessoryBuffers(cha);
			ChaFileAccessory.PartsInfo pNow = cha.nowCoordinate.accessory.parts[slot];
			ChaFileAccessory.PartsInfo pFile = cha.chaFile.coordinate.accessory.parts[slot];
			bool samePartRef = object.ReferenceEquals(pNow, pFile);
			bool needClear = pNow.type == 0 || pNow.type != accType;
			if (needClear)
			{
				pNow.MemberInit();
				if (!samePartRef)
				{
					pFile.MemberInit();
				}
			}
			pNow.type = accType;
			pNow.id = id;
			pNow.parentKey = parentKey;
			if (!samePartRef)
			{
				pFile.type = accType;
				pFile.id = id;
				pFile.parentKey = parentKey;
			}
			AIWearPartList.LogAcc(string.Format("ApplyAccessory: parts set (sameRef={0}) type={1} id={2} parentKey=\"{3}\"", samePartRef, pNow.type, pNow.id, pNow.parentKey ?? ""));
			bool sameCoordRef = object.ReferenceEquals(cha.nowCoordinate, cha.chaFile.coordinate);
			AIWearPartList.LogAcc(string.Format("ApplyAccessory: refEq(nowCoordinate, chaFile.coordinate)={0}", sameCoordRef));
			try
			{
				bool assignOk = cha.AssignCoordinate();
				AIWearPartList.LogAcc("ApplyAccessory: AssignCoordinate() -> " + assignOk);
			}
			catch (Exception ex)
			{
				AIWearPartList.LogAccWarn("ApplyAccessory: AssignCoordinate() failed: " + ex.Message);
			}
			AIWearPartList.TryUnifyCoordinateReferences(cha);
			AIWearPartList.TryUnifyAccessoryBuffers(cha);
			AIWearPartList.ApplyAccessorySlotParts(cha, slot, accType, id, parentKey);
			AIWearPartList.LogAcc("ApplyAccessory: slot parts re-applied after AssignCoordinate (in case it replaced coordinate data)");
			try
			{
				bool reloadOk = cha.Reload(false, true, true, true, true);
				AIWearPartList.LogAcc("ApplyAccessory: Reload(false,true,true,true,true) -> " + reloadOk + " (same sequence as StudioCharaListUtil.LoadAndChangeCloth after coord edit)");
			}
			catch (Exception ex)
			{
				AIWearPartList.LogAccWarn("ApplyAccessory: Reload: " + ex.Message);
			}
			try
			{
				bool assignAfterReload = cha.AssignCoordinate();
				AIWearPartList.LogAcc("ApplyAccessory: AssignCoordinate() after Reload -> " + assignAfterReload);
			}
			catch (Exception ex)
			{
				AIWearPartList.LogAccWarn("ApplyAccessory: AssignCoordinate after Reload: " + ex.Message);
			}
			AIWearPartList.TryUnifyCoordinateReferences(cha);
			AIWearPartList.TryUnifyAccessoryBuffers(cha);
			AIWearPartList.ApplyAccessorySlotParts(cha, slot, accType, id, parentKey);
			AIWearPartList.EnsureChaListCtrlForAccessoryLoad(cha);
			AIWearPartList.LogAccessoryDiagnostics(cha, listInfo);
			AIWearPartList.TryEnsureLoadControlForAccessory(cha);
			cha.InitializeAccessoryParent();
			cha.SetAccessoryState(slot, true);
			AIWearPartList.LogAcc(string.Format("ApplyAccessory: before load objTop={0} activeInHierarchy={1} enabled={2}", (cha.objTop != null) ? "ok" : "null", cha.gameObject.activeInHierarchy, cha.enabled));
			bool prevEnabled = cha.enabled;
			cha.enabled = true;
			try
			{
				if (cha.IsAccessory(slot))
				{
					AIWearPartList.LogAcc("ApplyAccessory: IsAccessory true after Reload+AssignCoordinate — skipping ChangeAccessoryAsync");
				}
				else
				{
					AIWearPartList.LogAcc("ApplyAccessory: still not loaded after Reload — ChangeAccessoryAsync(slot, type, id, parent, force:true, async:false)");
					yield return cha.StartCoroutine(cha.ChangeAccessoryAsync(slot, accType, id, parentKey, true, false));
					AIWearPartList.LogAccessoryRuntimeState(cha, slot, "right after ChangeAccessoryAsync(slot)");
					if (!cha.IsAccessory(slot))
					{
						const int maxFrames = 180;
						for (int i = 0; i < maxFrames; i++)
						{
							yield return null;
							if (cha.IsAccessory(slot))
							{
								AIWearPartList.LogAcc(string.Format("ApplyAccessory: IsAccessory true after {0} frames (post ChangeAccessoryAsync)", i + 1));
								break;
							}
						}
						if (!cha.IsAccessory(slot))
						{
							AIWearPartList.LogAccWarn("ApplyAccessory: still not loaded. BonesFramework patches LoadCharaFbxDataAsync (try disabling it). HS2_BepisFixHS / IllusionFixes can also affect ChaControl.");
						}
					}
				}
				AIWearPartList.LogAccessoryRuntimeState(cha, slot, "after accessory load attempt");
			}
			finally
			{
				cha.enabled = prevEnabled;
			}
			string parentResolved = parentKey;
			if (string.IsNullOrEmpty(parentResolved))
			{
				try
				{
					parentResolved = cha.GetAccessoryDefaultParentStr(slot);
				}
				catch (Exception ex)
				{
					AIWearPartList.LogAccWarn("post-wait GetAccessoryDefaultParentStr(slot): " + ex.Message);
				}
			}
			if (string.IsNullOrEmpty(parentResolved))
			{
				try
				{
					parentResolved = cha.GetAccessoryDefaultParentStr(accType, id);
				}
				catch (Exception ex)
				{
					AIWearPartList.LogAccWarn("post-wait GetAccessoryDefaultParentStr(type,id): " + ex.Message);
				}
			}
			if (!string.IsNullOrEmpty(parentResolved))
			{
				AIWearPartList.LogAcc("ApplyAccessory: ChangeAccessoryParent(\"" + parentResolved + "\")");
				cha.ChangeAccessoryParent(slot, parentResolved);
			}
			else
			{
				AIWearPartList.LogAccWarn("ApplyAccessory: parentResolved still empty; skipping ChangeAccessoryParent.");
			}
			cha.SetAccessoryDefaultColor(slot);
			cha.UpdateAccessoryMoveFromInfo(slot);
			cha.AssignCoordinate();
			cha.SetAccessoryState(slot, true);
			OCIChar oci = this.mpCharCtrl.ociChar;
			if (oci != null)
			{
				oci.ShowAccessory(slot, true);
			}
			if (cha.IsAccessory(slot))
			{
				try
				{
					cha.ChangeAccessoryColor(slot);
				}
				catch (Exception ex)
				{
					AIWearPartList.LogAccWarn("ApplyAccessory: ChangeAccessoryColor: " + ex.Message);
				}
			}
			this.mpCharCtrl.CallPrivateMethod("UpdateInfo");
			this.wearCtrl.UpdateInfo();
			AIWearPartList.TryStudioCostumeInfoUpdate(this.mpCharCtrl, oci);
			AIWearPartList.LogAccessoryRuntimeState(cha, slot, "after SetDefaultColor+UpdateMove+AssignCoordinate+ShowAccessory+UpdateInfo");
		}

		public static string GetAccessoryItemDisplayName(ChaControl cha, int slot)
		{
			if (cha == null || slot < 0 || slot >= ChaFileDefine.AccessorySlotNum)
			{
				return null;
			}
			ChaFileAccessory.PartsInfo part = cha.nowCoordinate.accessory.parts[slot];
			if (part.id <= 0 || part.type <= 0)
			{
				return null;
			}
			ListInfoBase listInfo = AIWearPartList.TryGetAccessoryListInfo((ChaListDefine.CategoryNo)part.type, part.id);
			if (listInfo == null || string.IsNullOrEmpty(listInfo.Name))
			{
				return null;
			}
			return listInfo.Name;
		}

		public IEnumerator ClearAccessorySlotCoroutine(ChaControl cha, int slot)
		{
			if (cha == null || slot < 0)
			{
				yield break;
			}
			AIWearPartList.TryUnifyCoordinateReferences(cha);
			AIWearPartList.TryUnifyAccessoryBuffers(cha);
			ChaFileAccessory.PartsInfo pNow = cha.nowCoordinate.accessory.parts[slot];
			ChaFileAccessory.PartsInfo pFile = cha.chaFile.coordinate.accessory.parts[slot];
			pNow.MemberInit();
			if (!object.ReferenceEquals(pNow, pFile))
			{
				pFile.MemberInit();
			}
			int emptyCategory = (int)ChaListDefine.CategoryNo.ao_head - 1;
			pNow.type = emptyCategory;
			pNow.id = 0;
			pNow.parentKey = string.Empty;
			if (!object.ReferenceEquals(pNow, pFile))
			{
				pFile.type = emptyCategory;
				pFile.id = 0;
				pFile.parentKey = string.Empty;
			}
			try
			{
				cha.AssignCoordinate();
			}
			catch
			{
			}
			AIWearPartList.TryUnifyCoordinateReferences(cha);
			AIWearPartList.TryUnifyAccessoryBuffers(cha);
			try
			{
				cha.Reload(false, true, true, true, true);
			}
			catch
			{
			}
			try
			{
				cha.AssignCoordinate();
			}
			catch
			{
			}
			AIWearPartList.TryUnifyCoordinateReferences(cha);
			AIWearPartList.TryUnifyAccessoryBuffers(cha);
			AIWearPartList.EnsureChaListCtrlForAccessoryLoad(cha);
			cha.InitializeAccessoryParent();
			cha.SetAccessoryState(slot, true);
			bool prevEnabled = cha.enabled;
			cha.enabled = true;
			try
			{
				yield return cha.StartCoroutine(cha.ChangeAccessoryAsync(slot, emptyCategory, 0, string.Empty, true, false));
			}
			finally
			{
				cha.enabled = prevEnabled;
			}
			OCIChar oci = this.mpCharCtrl.ociChar;
			this.mpCharCtrl.CallPrivateMethod("UpdateInfo");
			this.wearCtrl.UpdateInfo();
			AIWearPartList.TryStudioCostumeInfoUpdate(this.mpCharCtrl, oci);
		}

		private static void LogAccessoryDiagnostics(ChaControl cha, ListInfoBase listInfo)
		{
			if (cha == null)
			{
				return;
			}
			try
			{
				string possess = "";
				if (listInfo != null)
				{
					try
					{
						possess = listInfo.GetInfo(ChaListDefine.KeyType.Possess) ?? "";
					}
					catch
					{
						possess = "(GetInfo Possess failed)";
					}
				}
				AIWearPartList.LogAcc(string.Format("AccessoryDiag: cha.sex={0} list.Possess=\"{1}\" objTop={2} objBodyBone={3} objHead={4} cmpBoneBody={5}", new object[]
				{
					cha.sex,
					possess,
					cha.objTop != null,
					cha.objBodyBone != null,
					cha.objHead != null,
					cha.cmpBoneBody != null
				}));
			}
			catch (Exception ex)
			{
				AIWearPartList.LogAccWarn("AccessoryDiag: " + ex.Message);
			}
		}

		private static void TryEnsureLoadControlForAccessory(ChaControl cha)
		{
			if (cha == null || cha.objBodyBone != null)
			{
				return;
			}
			try
			{
				AIWearPartList.LogAccWarn("AccessoryDiag: objBodyBone is null before accessory FBX load; calling InitializeControlLoadObject().");
				cha.CallPrivateMethod("InitializeControlLoadObject");
			}
			catch (Exception ex)
			{
				AIWearPartList.LogAccWarn("TryEnsureLoadControlForAccessory: " + ex.Message);
			}
		}

		private static FieldInfo _fiMpCostumeInfo;

		private static MethodInfo _miCostumeInfoUpdateInfo;

		private static void TryStudioCostumeInfoUpdate(MPCharCtrl mp, OCIChar oci)
		{
			if (mp == null || oci == null)
			{
				return;
			}
			try
			{
				if (AIWearPartList._miCostumeInfoUpdateInfo == null)
				{
					Type t = typeof(MPCharCtrl).GetNestedType("CostumeInfo", BindingFlags.Public | BindingFlags.NonPublic);
					if (t != null)
					{
						AIWearPartList._fiMpCostumeInfo = typeof(MPCharCtrl).GetField("costumeInfo", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						AIWearPartList._miCostumeInfoUpdateInfo = t.GetMethod("UpdateInfo", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
						{
							typeof(OCIChar)
						}, null);
					}
				}
				if (AIWearPartList._fiMpCostumeInfo == null || AIWearPartList._miCostumeInfoUpdateInfo == null)
				{
					return;
				}
				object costumeInfo = AIWearPartList._fiMpCostumeInfo.GetValue(mp);
				if (costumeInfo != null)
				{
					AIWearPartList._miCostumeInfoUpdateInfo.Invoke(costumeInfo, new object[]
					{
						oci
					});
					AIWearPartList.LogAcc("TryStudioCostumeInfoUpdate: CostumeInfo.UpdateInfo(oci) ok.");
				}
			}
			catch (Exception ex)
			{
				AIWearPartList.LogAccWarn("TryStudioCostumeInfoUpdate: " + ex.Message);
			}
		}

		private static void LogAccessoryRuntimeState(ChaControl cha, int slot, string phase)
		{
			bool isAcc = false;
			try
			{
				isAcc = cha.IsAccessory(slot);
			}
			catch (Exception ex)
			{
				AIWearPartList.LogAccWarn(phase + ": IsAccessory threw: " + ex.Message);
				return;
			}
			bool hasObj = false;
			bool hasCmp = false;
			GameObject[] objs = cha.objAccessory;
			if (objs != null && slot >= 0 && slot < objs.Length)
			{
				hasObj = objs[slot] != null;
			}
			CmpAccessory[] cmps = cha.cmpAccessory;
			if (cmps != null && slot >= 0 && slot < cmps.Length)
			{
				hasCmp = cmps[slot] != null;
			}
			ListInfoBase[] infos = cha.infoAccessory;
			bool hasInfo = infos != null && slot >= 0 && slot < infos.Length && infos[slot] != null;
			AIWearPartList.LogAcc(string.Format("ApplyAccessory [{0}]: IsAccessory={1} objAccessory[{2}]={3} cmpAccessory[{4}]={5} infoAccessory[{6}]={7}", new object[]
			{
				phase,
				isAcc,
				slot,
				hasObj,
				slot,
				hasCmp,
				slot,
				hasInfo
			}));
		}

		private static void LogAcc(string msg)
		{
			ManualLogSource log = HS2WearCustom.Logger;
			if (log != null)
			{
				log.LogInfo("[WearCustom/Accessory] " + msg);
			}
		}

		private static void LogAccWarn(string msg)
		{
			ManualLogSource log = HS2WearCustom.Logger;
			if (log != null)
			{
				log.LogWarning("[WearCustom/Accessory] " + msg);
			}
		}

		// Token: 0x04000061 RID: 97
		private Dictionary<int, Image> dicNode;

		// Token: 0x04000062 RID: 98
		public Transform transformRoot;

		// Token: 0x04000063 RID: 99
		public GameObject objectPrefab;

		// Token: 0x04000064 RID: 100
		public ScrollRect scrollRect;

		// Token: 0x04000065 RID: 101
		public MPCharCtrl mpCharCtrl;

		// Token: 0x04000066 RID: 102
		public AIWearControllerUI wearCtrl;

		// Token: 0x04000067 RID: 103
		private List<CustomSelectInfo> partLst;

		// Token: 0x04000068 RID: 104
		private ChaListDefine.CategoryNo selectType;

		// Token: 0x04000069 RID: 105
		private int partKind;

		// Token: 0x0400006A RID: 106
		private int select;

		// Token: 0x0400006B RID: 107
		private int wearType = -1;
	}
}
