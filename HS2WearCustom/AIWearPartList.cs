using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using AIChara;
using CharaCustom;
using Manager;
using Studio;
using UnityEngine;
using UnityEngine.UI;

namespace HS2WearCustom
{
	public class AIWearPartList : MonoBehaviour
	{
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

		public void updateInfo()
		{
		}

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
				base.StartCoroutine(this.ApplyAccessoryAfterLoad(charInfo, slot, accType, info.id, parentKey));
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
				return null;
			}
			ChaListControl clc = mc.chaListCtrl;
			if (clc == null)
			{
				return null;
			}
			return clc.GetListInfo(categoryNo, listId);
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
					return text;
				}
			}
			catch
			{
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
				}
			}
			catch
			{
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
				}
			}
			catch
			{
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
				return;
			}
		}

		private static string ResolveAccessoryParentKey(ChaControl cha, int slot, int accType, int id)
		{
			if (id <= 0)
			{
				return string.Empty;
			}
			try
			{
				string key = cha.GetAccessoryDefaultParentStr(accType, id);
				if (!string.IsNullOrEmpty(key))
				{
					return key;
				}
			}
			catch
			{
			}
			try
			{
				string key = cha.GetAccessoryDefaultParentStr(slot);
				if (!string.IsNullOrEmpty(key))
				{
					return key;
				}
			}
			catch
			{
			}
			return string.Empty;
		}

		private IEnumerator ApplyAccessoryAfterLoad(ChaControl cha, int slot, int accType, int id, string parentKey)
		{
			// Pipeline: (1) unify chaFile.coordinate with nowCoordinate, (2) write ChaFileAccessory.parts[slot], (3) AssignCoordinate,
			// (4) ChaControl.Reload(false,true,true,true,true) + AssignCoordinate — same as StudioCharaListUtil after editing nowCoordinate,
			// (5) only if IsAccessory still false, run ChangeAccessoryAsync (slot,…). Async/wait alone never fixed Studio without Reload.
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
			try
			{
				cha.AssignCoordinate();
			}
			catch
			{
			}
			AIWearPartList.TryUnifyCoordinateReferences(cha);
			AIWearPartList.TryUnifyAccessoryBuffers(cha);
			AIWearPartList.ApplyAccessorySlotParts(cha, slot, accType, id, parentKey);
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
			AIWearPartList.ApplyAccessorySlotParts(cha, slot, accType, id, parentKey);
			AIWearPartList.EnsureChaListCtrlForAccessoryLoad(cha);
			AIWearPartList.TryEnsureLoadControlForAccessory(cha);
			cha.InitializeAccessoryParent();
			cha.SetAccessoryState(slot, true);
			bool prevEnabled = cha.enabled;
			cha.enabled = true;
			try
			{
				if (!cha.IsAccessory(slot))
				{
					yield return cha.StartCoroutine(cha.ChangeAccessoryAsync(slot, accType, id, parentKey, true, false));
					if (!cha.IsAccessory(slot))
					{
						const int maxFrames = 180;
						for (int i = 0; i < maxFrames; i++)
						{
							yield return null;
							if (cha.IsAccessory(slot))
							{
								break;
							}
						}
					}
				}
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
				catch
				{
				}
			}
			if (string.IsNullOrEmpty(parentResolved))
			{
				try
				{
					parentResolved = cha.GetAccessoryDefaultParentStr(accType, id);
				}
				catch
				{
				}
			}
			if (!string.IsNullOrEmpty(parentResolved))
			{
				cha.ChangeAccessoryParent(slot, parentResolved);
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
				catch
				{
				}
			}
			this.mpCharCtrl.CallPrivateMethod("UpdateInfo");
			this.wearCtrl.UpdateInfo();
			AIWearPartList.TryStudioCostumeInfoUpdate(this.mpCharCtrl, oci);
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

		private static void TryEnsureLoadControlForAccessory(ChaControl cha)
		{
			if (cha == null || cha.objBodyBone != null)
			{
				return;
			}
			try
			{
				cha.CallPrivateMethod("InitializeControlLoadObject");
			}
			catch
			{
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
				}
			}
			catch
			{
			}
		}

		private Dictionary<int, Image> dicNode;

		public Transform transformRoot;

		public GameObject objectPrefab;

		public ScrollRect scrollRect;

		public MPCharCtrl mpCharCtrl;

		public AIWearControllerUI wearCtrl;

		private List<CustomSelectInfo> partLst;

		private ChaListDefine.CategoryNo selectType;

		private int partKind;

		private int select;

		private int wearType = -1;
	}
}
