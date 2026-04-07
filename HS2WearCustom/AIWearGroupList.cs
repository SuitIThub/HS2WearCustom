using System;
using System.Collections;
using System.Collections.Generic;
using AIChara;
using Studio;
using UnityEngine;
using UnityEngine.UI;

namespace HS2WearCustom
{
	public class AIWearGroupList : MonoBehaviour
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
					if (!value)
					{
						this.wearPartList.active = false;
						this.wearCtrl.active = false;
					}
				}
			}
		}

		public void InitList(int _sex)
		{
			this.Init();
			this.scrollRect.verticalNormalizedPosition = 1f;
			this.dicNode.Clear();
			this.sex = _sex;
			this.select = ChaListDefine.CategoryNo.unknown;
			if (this.sex != 0)
			{
				this.CreateNode(ChaListDefine.CategoryNo.fo_top, "Top", 0);
				this.CreateNode(ChaListDefine.CategoryNo.fo_bot, "Bottom", 1);
				this.CreateNode(ChaListDefine.CategoryNo.fo_inner_t, "Inner Top", 2);
				this.CreateNode(ChaListDefine.CategoryNo.fo_inner_b, "Inner Bot", 3);
				this.CreateNode(ChaListDefine.CategoryNo.fo_gloves, "Gloves", 4);
				this.CreateNode(ChaListDefine.CategoryNo.fo_panst, "Pantyhose", 5);
				this.CreateNode(ChaListDefine.CategoryNo.fo_socks, "Socks", 6);
				this.CreateNode(ChaListDefine.CategoryNo.fo_shoes, "Shoes", 7);
				this.CreateNode(ChaListDefine.CategoryNo.so_hair_b, "Hair Back", 0);
				this.CreateNode(ChaListDefine.CategoryNo.so_hair_f, "Hair Front", 1);
				this.CreateNode(ChaListDefine.CategoryNo.so_hair_s, "Hair Side", 2);
				this.CreateNode(ChaListDefine.CategoryNo.so_hair_o, "Hair Ext", 3);
			}
			else
			{
				this.CreateNode(ChaListDefine.CategoryNo.mo_top, "Top", 0);
				this.CreateNode(ChaListDefine.CategoryNo.mo_bot, "Bottom", 1);
				this.CreateNode(ChaListDefine.CategoryNo.mo_gloves, "Gloves", 4);
				this.CreateNode(ChaListDefine.CategoryNo.mo_shoes, "Shoes", 7);
				this.CreateNode(ChaListDefine.CategoryNo.so_hair_b, "Hair Back", 0);
				this.CreateNode(ChaListDefine.CategoryNo.so_hair_f, "Hair Front", 1);
				this.CreateNode(ChaListDefine.CategoryNo.so_hair_s, "Hair Side", 2);
				this.CreateNode(ChaListDefine.CategoryNo.so_hair_o, "Hair Ext", 3);
			}
			this.wearPartList.active = false;
		}

		private void OnSelect(ChaListDefine.CategoryNo _no, int partN)
		{
			if (this.select != ChaListDefine.CategoryNo.unknown && this.dicNode.TryGetValue(this.select, out Image imgOld) && imgOld != null)
			{
				imgOld.color = Color.white;
			}
			this.select = _no;
			this.selectKind = partN;
			Image imgNew;
			if (this.dicNode.TryGetValue(this.select, out imgNew) && imgNew != null)
			{
				imgNew.color = Color.green;
			}
			this.wearPartList.InitList(this.sex, this.select, partN);
			this.wearPartList.active = true;
			this.wearCtrl.active = true;
			this.wearCtrl.UpdateInfo();
		}

		private void Init()
		{
			int childCount = this.transformRoot.childCount;
			for (int i = 0; i < childCount; i++)
			{
				UnityEngine.Object.Destroy(this.transformRoot.GetChild(i).gameObject);
			}
			this.transformRoot.DetachChildren();
			this.dicNode = new Dictionary<ChaListDefine.CategoryNo, Image>();
		}

		private static readonly ChaListDefine.CategoryNo[] AccessoryCategoryOrder = new ChaListDefine.CategoryNo[]
		{
			ChaListDefine.CategoryNo.ao_head,
			ChaListDefine.CategoryNo.ao_ear,
			ChaListDefine.CategoryNo.ao_glasses,
			ChaListDefine.CategoryNo.ao_face,
			ChaListDefine.CategoryNo.ao_neck,
			ChaListDefine.CategoryNo.ao_shoulder,
			ChaListDefine.CategoryNo.ao_chest,
			ChaListDefine.CategoryNo.ao_waist,
			ChaListDefine.CategoryNo.ao_back,
			ChaListDefine.CategoryNo.ao_arm,
			ChaListDefine.CategoryNo.ao_hand,
			ChaListDefine.CategoryNo.ao_leg,
			ChaListDefine.CategoryNo.ao_kokan
		};

		private static int CategoryToPartKind(ChaListDefine.CategoryNo cat)
		{
			for (int i = 0; i < AIWearGroupList.AccessoryCategoryOrder.Length; i++)
			{
				if (AIWearGroupList.AccessoryCategoryOrder[i] == cat)
				{
					return i + 1;
				}
			}
			return 1;
		}

		public void InitAccessorySlotList(int _sex)
		{
			this.Init();
			this.scrollRect.verticalNormalizedPosition = 1f;
			this.dicNode.Clear();
			this.sex = _sex;
			this.select = ChaListDefine.CategoryNo.unknown;
			this.wearPartList.active = false;
			this.wearCtrl.active = false;

			ChaControl charInfo = this.wearCtrl.mpCharCtrl.ociChar.charInfo;
			ChaFileAccessory.PartsInfo[] accParts = charInfo.nowCoordinate.accessory.parts;

			for (int i = 0; i < ChaFileDefine.AccessorySlotNum; i++)
			{
				int slotIdx = i;
				string itemName = AIWearPartList.GetAccessoryItemDisplayName(charInfo, i);
				string label = !string.IsNullOrEmpty(itemName) ? itemName : ("Slot " + (i + 1) + " – Empty");
				this.CreateActionNode(label, delegate
				{
					this.wearCtrl.accessorySlot = slotIdx;
					this.ShowAccessoryTypes();
				});
			}
		}

		private void ShowAccessoryTypes()
		{
			this.Init();
			this.scrollRect.verticalNormalizedPosition = 1f;
			this.dicNode.Clear();
			this.select = ChaListDefine.CategoryNo.unknown;
			this.wearPartList.active = false;

			this.CreateActionNode("< Back", delegate
			{
				this.InitAccessorySlotList(this.sex);
			});

			this.CreateActionNode("None", delegate
			{
				ChaControl cha = this.wearCtrl.mpCharCtrl.ociChar.charInfo;
				int slot = this.wearCtrl.accessorySlot;
				this.wearPartList.StartCoroutine(this.ClearAccessoryNoneThenRefresh(cha, slot));
			});

			this.CreateNode(ChaListDefine.CategoryNo.ao_head, "Head", 1);
			this.CreateNode(ChaListDefine.CategoryNo.ao_ear, "Ear", 2);
			this.CreateNode(ChaListDefine.CategoryNo.ao_glasses, "Glasses", 3);
			this.CreateNode(ChaListDefine.CategoryNo.ao_face, "Face", 4);
			this.CreateNode(ChaListDefine.CategoryNo.ao_neck, "Neck", 5);
			this.CreateNode(ChaListDefine.CategoryNo.ao_shoulder, "Shoulder", 6);
			this.CreateNode(ChaListDefine.CategoryNo.ao_chest, "Chest", 7);
			this.CreateNode(ChaListDefine.CategoryNo.ao_waist, "Waist", 8);
			this.CreateNode(ChaListDefine.CategoryNo.ao_back, "Back", 9);
			this.CreateNode(ChaListDefine.CategoryNo.ao_arm, "Arm", 10);
			this.CreateNode(ChaListDefine.CategoryNo.ao_hand, "Hand", 11);
			this.CreateNode(ChaListDefine.CategoryNo.ao_leg, "Leg", 12);
			this.CreateNode(ChaListDefine.CategoryNo.ao_kokan, "Crotch", 13);
			this.TryHighlightLoadedAccessoryForCurrentSlot();
		}

		private IEnumerator ClearAccessoryNoneThenRefresh(ChaControl cha, int slot)
		{
			yield return this.wearPartList.ClearAccessorySlotCoroutine(cha, slot);
			this.ShowAccessoryTypes();
		}

		private void TryHighlightLoadedAccessoryForCurrentSlot()
		{
			ChaControl charInfo = this.wearCtrl.mpCharCtrl.ociChar.charInfo;
			int slot = this.wearCtrl.accessorySlot;
			ChaFileAccessory.PartsInfo part = charInfo.nowCoordinate.accessory.parts[slot];
			if (part.id <= 0 || part.type <= 0)
			{
				return;
			}
			ChaListDefine.CategoryNo cat = (ChaListDefine.CategoryNo)part.type;
			if (cat < ChaListDefine.CategoryNo.ao_head || cat > ChaListDefine.CategoryNo.ao_kokan)
			{
				return;
			}
			if (!this.dicNode.ContainsKey(cat))
			{
				return;
			}
			foreach (KeyValuePair<ChaListDefine.CategoryNo, Image> kv in this.dicNode)
			{
				if (kv.Value != null)
				{
					kv.Value.color = Color.white;
				}
			}
			this.select = cat;
			this.selectKind = AIWearGroupList.CategoryToPartKind(cat);
			Image img;
			if (this.dicNode.TryGetValue(cat, out img) && img != null)
			{
				img.color = Color.green;
			}
			this.wearPartList.InitList(this.sex, cat, this.selectKind);
			this.wearPartList.active = true;
			this.wearCtrl.active = true;
			this.wearCtrl.UpdateInfo();
		}

		private void CreateActionNode(string name, Action onClick)
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
				onClick();
			});
			component.text = name;
		}

		private void CreateNode(ChaListDefine.CategoryNo no, string name, int partN)
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
				this.OnSelect(no, partN);
			});
			component.text = name;
			this.dicNode.Add(no, component.image);
		}

		private bool isInit;

		private Dictionary<ChaListDefine.CategoryNo, Image> dicNode;

		public ChaListDefine.CategoryNo select = ChaListDefine.CategoryNo.unknown;

		public int selectKind = -1;

		public int sex = -1;

		public Transform transformRoot;

		public GameObject objectPrefab;

		public ScrollRect scrollRect;

		public AIWearPartList wearPartList;

		public AIWearControllerUI wearCtrl;
	}
}
