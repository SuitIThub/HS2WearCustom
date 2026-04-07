using System;
using Studio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HS2WearCustom
{
	// Token: 0x02000005 RID: 5
	internal static class AIWCUI
	{
		// Token: 0x0600000E RID: 14 RVA: 0x00002284 File Offset: 0x00000484
		internal static void InitUI()
		{
			if (AIWCUI.isUIInited)
			{
				return;
			}
			GameObject gameObject = GameObject.Find("StudioScene/Canvas Main Menu/02_Manipulate/00_Chara/00_Root/Viewport/Content");
			GameObject chara = GameObject.Find("StudioScene/Canvas Main Menu/02_Manipulate/00_Chara");
			AIWCUI.CreatePanel(chara);
			AIWCUI.CreateMenuEntry(gameObject, chara);
			AIWCUI.isUIInited = true;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000022C0 File Offset: 0x000004C0
		internal static void InitCharUI()
		{
			if (AIWCUI.isCharUIInited)
			{
				return;
			}
			GameObject.Find("StudioScene/Canvas Main Menu/01_Add/00_Female/Button Change");
			AIWCUI.isCharUIInited = true;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000022DC File Offset: 0x000004DC
		private static void CreateMenuEntry(GameObject listmenu, GameObject chara0)
		{
			GameObject gameObject = GameObject.Find("StudioScene/Canvas Main Menu/02_Manipulate/00_Chara/00_Root/Viewport/Content/Cos");
			if (listmenu == null || gameObject == null)
			{
				return;
			}
			AIWCUI.selectButtons = new Button[6];
			Button[] componentsInChildren = listmenu.GetComponentsInChildren<Button>();
			AIWCUI.CreatButton("WearCustom", "Clothing", listmenu, gameObject, chara0, 201, false);
			AIWCUI.CreatButton("WearAccessories", "Accessories", listmenu, gameObject, chara0, 202, true);
			Button[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].onClick.AddListener(delegate()
				{
					if (AIWCUI.wcUIPanel.activeSelf)
					{
						AIWCUI.wcUIPanel.SetActive(false);
					}
					for (int j = 0; j < AIWCUI.selectButtons.Length; j++)
					{
						if (AIWCUI.selectButtons[j] != null)
						{
							AIWCUI.selectButtons[j].image.color = Color.white;
						}
					}
				});
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x0000236C File Offset: 0x0000056C
		private static void CreatButton(string name, string text, GameObject parent, GameObject button, GameObject chara0, int _idx, bool accessoriesOnly)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(button, parent.transform, true);
			gameObject.name = name;
			(gameObject.transform.GetChild(0).GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI).text = text;
			Button component = gameObject.GetComponent<Button>();
			AIWCUI.selectButtons[_idx - 201] = component;
			MPCharCtrl mcc = chara0.GetComponent<MPCharCtrl>();
			component.onClick.RemoveAllListeners();
			component.onClick.AddListener(delegate()
			{
				mcc.OnClickRoot(_idx);
				if (!AIWCUI.wcUIPanel.activeSelf)
				{
					AIWCUI.wcUIPanel.SetActive(true);
				}
				for (int i = 0; i < AIWCUI.selectButtons.Length; i++)
				{
					if (AIWCUI.selectButtons[i] != null)
					{
						AIWCUI.selectButtons[i].image.color = ((_idx == i + 201) ? Color.green : Color.white);
					}
				}
				if (accessoriesOnly)
					AIWCUI.wearGroupList.InitAccessorySlotList(mcc.ociChar.oiCharInfo.sex);
				else
					AIWCUI.wearGroupList.InitList(mcc.ociChar.oiCharInfo.sex);
				AIWCUI.wearGroupList.active = true;
			});
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000240C File Offset: 0x0000060C
		private static void CreatePanel(GameObject chara0)
		{
			GameObject gameObject = GameObject.Find("StudioScene/Canvas Main Menu/02_Manipulate");
			GameObject original = GameObject.Find("StudioScene/Canvas Main Menu/02_Manipulate/03_Anime");
			GameObject.Find("StudioScene/Canvas Main Menu/02_Manipulate/00_Chara/05_Costume/Image Thumbnail image");
			GameObject.Find("StudioScene/Canvas Main Menu/02_Manipulate/00_Chara/05_Costume/Button Load");
			GameObject original2 = GameObject.Find("StudioScene/Canvas Main Menu/02_Manipulate/01_Item");
			GameObject cgTrans = GameObject.Find("StudioScene/Canvas Pattern");
			AIWCUI.wcUIPanel = UnityEngine.Object.Instantiate<GameObject>(original, gameObject.transform, true);
			AIWCUI.wcUIPanel.name = "51_WearCustom";
			AIWCUI.wcCtrlPanel = UnityEngine.Object.Instantiate<GameObject>(original2, AIWCUI.wcUIPanel.transform, true);
			AIWCUI.wcCtrlPanel.name = "Custom Controll";
			AIWCUI.wcCtrlPanel.transform.localPosition = new Vector3(AIWCUI.wcCtrlPanel.transform.localPosition.x + 400f, AIWCUI.wcCtrlPanel.transform.localPosition.y, AIWCUI.wcCtrlPanel.transform.localPosition.z);
			Transform transform = AIWCUI.wcUIPanel.transform.Find("Group Panel");
			Transform transform2 = AIWCUI.wcUIPanel.transform.Find("Category Panel");
			UnityEngine.Object.Destroy(AIWCUI.wcUIPanel.transform.Find("Anime Panel").gameObject);
			AIWCUI.wearGroupList = transform.GetOrAddComponent<AIWearGroupList>();
			AIWCUI.wearPartList = transform2.GetOrAddComponent<AIWearPartList>();
			AIWCUI.wearPartCtrl = AIWCUI.wcCtrlPanel.GetOrAddComponent<AIWearControllerUI>();
			AIWCUI.wearGroupList.transformRoot = AIWCUI.wearGroupList.transform.Find("Viewport/Content");
			AIWCUI.wearGroupList.objectPrefab = AIWCUI.wearGroupList.transform.Find("node").gameObject;
			AIWCUI.wearGroupList.scrollRect = AIWCUI.wearGroupList.GetComponent<ScrollRect>();
			AIWCUI.wearGroupList.wearPartList = AIWCUI.wearPartList;
			AIWCUI.wearGroupList.wearCtrl = AIWCUI.wearPartCtrl;
			AIWCUI.wearPartList.transformRoot = AIWCUI.wearPartList.transform.Find("Viewport/Content");
			AIWCUI.wearPartList.objectPrefab = AIWCUI.wearGroupList.objectPrefab;
			AIWCUI.wearPartList.scrollRect = AIWCUI.wearPartList.GetComponent<ScrollRect>();
			AIWCUI.wearPartList.mpCharCtrl = chara0.GetComponent<MPCharCtrl>();
			AIWCUI.wearPartList.wearCtrl = AIWCUI.wearPartCtrl;
			AIWCUI.wearPartCtrl.mpCharCtrl = AIWCUI.wearPartList.mpCharCtrl;
			AIWCUI.wearPartCtrl.aiWGList = AIWCUI.wearGroupList;
			AIWCUI.wearPartCtrl.Init(AIWCUI.wcCtrlPanel, cgTrans);
			UnityEngine.Object.Destroy(transform.GetComponent<AnimeGroupList>());
			UnityEngine.Object.Destroy(transform2.GetComponent<AnimeCategoryList>());
			UnityEngine.Object.Destroy(AIWCUI.wcCtrlPanel.GetComponent<MPItemCtrl>());
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002690 File Offset: 0x00000890
		public static void OnclickedRoot(int _idx)
		{
			if (AIWCUI.isUIInited)
			{
				if (_idx == 201 || _idx == 202)
				{
					AIWCUI.UpdataInfo();
					return;
				}
				for (int i = 0; i < AIWCUI.selectButtons.Length; i++)
				{
					if (AIWCUI.selectButtons[i] != null)
					{
						AIWCUI.selectButtons[i].image.color = Color.white;
					}
				}
				AIWCUI.active = false;
			}
		}

		// Token: 0x17000003 RID: 3
		// (set) Token: 0x06000014 RID: 20 RVA: 0x000026C3 File Offset: 0x000008C3
		public static bool active
		{
			set
			{
				if (AIWCUI.isUIInited)
				{
					AIWCUI.wearGroupList.active = value;
					if (AIWCUI.wcUIPanel.activeSelf != value)
					{
						AIWCUI.wcUIPanel.SetActive(value);
					}
				}
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000026EF File Offset: 0x000008EF
		public static void UpdataInfo()
		{
			if (AIWCUI.wearPartList.active)
			{
				AIWCUI.wearPartCtrl.UpdateInfo();
			}
		}

		// Token: 0x04000003 RID: 3
		private static bool isUIInited;

		// Token: 0x04000004 RID: 4
		private static bool isCharUIInited;

		// Token: 0x04000005 RID: 5
		private static GameObject listmenu;

		// Token: 0x04000006 RID: 6
		private static Button[] selectButtons;

		// Token: 0x04000007 RID: 7
		public static GameObject wcUIPanel;

		// Token: 0x04000008 RID: 8
		private static GameObject wcCtrlPanel;

		// Token: 0x04000009 RID: 9
		public static AIWearGroupList wearGroupList;

		// Token: 0x0400000A RID: 10
		private static AIWearPartList wearPartList;

		// Token: 0x0400000B RID: 11
		private static AIWearControllerUI wearPartCtrl;
	}
}
