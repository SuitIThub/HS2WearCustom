using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AIChara;
using MessagePack;
using Studio;
using UnityEngine;
using UnityEngine.UI;

namespace HS2WearCustom
{
	// Token: 0x0200001E RID: 30
	public class StudioCharaListUtil : MonoBehaviour
	{
		// Token: 0x06000095 RID: 149 RVA: 0x00005F14 File Offset: 0x00004114
		public static void Install()
		{
			if (Singleton<global::Studio.Studio>.Instance == null)
			{
				return;
			}
			Transform transform = GameObject.Find("StudioScene/Canvas Main Menu").transform;
			GameObject gameObject = transform.Find("01_Add/00_Female").gameObject;
			if (gameObject.GetComponent<StudioCharaListUtil>() == null)
			{
				gameObject.AddComponent<StudioCharaListUtil>().Init(true, false);
			}
			GameObject gameObject2 = transform.Find("01_Add/01_Male").gameObject;
			if (gameObject2.GetComponent<StudioCharaListUtil>() == null)
			{
				gameObject2.AddComponent<StudioCharaListUtil>().Init(true, true);
			}
			Transform transform2 = transform.Find("02_Manipulate/00_Chara");
			if (transform2 != null && transform2.gameObject.GetComponent<StudioCharaListUtil>() == null)
			{
				transform2.gameObject.AddComponent<StudioCharaListUtil>().Init(false, false);
			}
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00005FD0 File Offset: 0x000041D0
		private void OnGUI()
		{
			if (this.showGUI && this.changeButton != null && this.changeButton.gameObject.activeInHierarchy && this.changeButton.interactable)
			{
				Rect rect = StudioCharaListUtil.RectTransformToScreenSpace(this.orgRect);
				if (this.isCoordinate)
				{
					this.windowRect = new Rect(rect.x, rect.y + rect.height + 5f, rect.width, 100f);
				}
				else
				{
					this.windowRect = new Rect(rect.x, rect.y + rect.height + 5f, rect.width, 180f);
				}
				this.windowRect = GUI.Window(this.windowID, this.windowRect, new GUI.WindowFunction(this.FuncWindowGUI), this.windowTitle);
			}
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000060C4 File Offset: 0x000042C4
		public static Rect RectTransformToScreenSpace(RectTransform transform)
		{
			Vector2 vector = Vector2.Scale(transform.rect.size, transform.lossyScale);
			Rect result = new Rect(transform.position.x, (float)Screen.height - transform.position.y, vector.x, vector.y);
			result.x -= transform.pivot.x * vector.x;
			result.y -= (1f - transform.pivot.y) * vector.y;
			return result;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00006168 File Offset: 0x00004368
		private void FuncWindowGUI(int winID)
		{
			try
			{
				GUI.enabled = true;
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				if (this.isCoordinate)
				{
					if (GUILayout.Button("ClothOnly", new GUILayoutOption[]
					{
						GUILayout.Width(100f),
						GUILayout.Height(25f)
					}))
					{
						this.LoadAndChangeCloth(true, false);
					}
					if (GUILayout.Button("AccOnly", new GUILayoutOption[]
					{
						GUILayout.Width(100f),
						GUILayout.Height(25f)
					}))
					{
						this.LoadAndChangeCloth(false, true);
					}
				}
				else
				{
					this.replaceCharaHairOnly = GUILayout.Toggle(this.replaceCharaHairOnly, "Hair", new GUILayoutOption[0]);
					this.replaceCharaHeadOnly = GUILayout.Toggle(this.replaceCharaHeadOnly, "Face", new GUILayoutOption[0]);
					this.replaceCharaBodyOnly = GUILayout.Toggle(this.replaceCharaBodyOnly, "Body", new GUILayoutOption[0]);
					this.replaceCharaClothesOnly = GUILayout.Toggle(this.replaceCharaClothesOnly, "Clothes", new GUILayoutOption[0]);
					this.replaceCharaAccOnly = GUILayout.Toggle(this.replaceCharaAccOnly, "Acc", new GUILayoutOption[0]);
					if ((this.replaceCharaHairOnly || this.replaceCharaHeadOnly || this.replaceCharaBodyOnly || this.replaceCharaClothesOnly || this.replaceCharaAccOnly) && GUILayout.Button("Replace", new GUILayoutOption[]
					{
						GUILayout.Width(100f),
						GUILayout.Height(20f)
					}))
					{
						this.ChangeChara();
					}
				}
				GUILayout.EndVertical();
				GUI.DragWindow();
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00006314 File Offset: 0x00004514
		private void Init(bool isCharaList, bool isMale)
		{
			this.studio = Singleton<global::Studio.Studio>.Instance;
			if (isCharaList)
			{
				this.charaList = base.GetComponent<CharaList>();
				this.charaFileSort = (StudioCharaListUtil.f_charaFileSort.GetValue(this.charaList) as CharaFileSort);
				this.changeButton = base.gameObject.transform.Find("Button Change").GetComponent<Button>();
				this.orgRect = this.changeButton.GetComponent<RectTransform>();
				this.isMale = isMale;
				return;
			}
			this.isCoordinate = true;
			this.mpCharCtrl = base.gameObject.GetComponent<MPCharCtrl>();
			this.costumeInfo = StudioCharaListUtil.f_costumeInfo.GetValue(this.mpCharCtrl);
			this.charaFileSort = (StudioCharaListUtil.f_CostumeInfo_fileSort.GetValue(this.costumeInfo) as CharaFileSort);
			this.changeButton = base.gameObject.transform.Find("05_Costume/Button Load").GetComponent<Button>();
			this.orgRect = this.changeButton.GetComponent<RectTransform>();
			this.isMale = isMale;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00006410 File Offset: 0x00004610
		public void Clear()
		{
			this._namePattern = "";
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00006420 File Offset: 0x00004620
		public void Refresh()
		{
			if (this.isCoordinate)
			{
				OCIChar ocichar = StudioCharaListUtil.f_m_OCIChar.GetValue(this.mpCharCtrl) as OCIChar;
				if (ocichar != null)
				{
					StudioCharaListUtil.m_CostumeInfo_InitList.Invoke(this.costumeInfo, new object[]
					{
						ocichar.charInfo.sex
					});
					return;
				}
			}
			else
			{
				if (this.isMale)
				{
					this.charaList.LoadCharaMale();
					return;
				}
				this.charaList.LoadCharaFemale();
			}
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00006498 File Offset: 0x00004698
		public void LoadAndChangeCloth(bool clothOnly, bool accessoryOnly)
		{
			OCIChar ocichar = StudioCharaListUtil.f_m_OCIChar.GetValue(this.mpCharCtrl) as OCIChar;
			if (ocichar == null)
			{
				return;
			}
			string selectPath = this.charaFileSort.selectPath;
			if (!File.Exists(selectPath))
			{
				return;
			}
			byte sex = ocichar.charInfo.sex;
			ChaControl charInfo = ocichar.charInfo;
			byte[] bytes = null;
			byte[] bytes2 = null;
			if (clothOnly)
			{
				bytes = MessagePackSerializer.Serialize<ChaFileAccessory>(charInfo.nowCoordinate.accessory);
			}
			else if (accessoryOnly)
			{
				bytes2 = MessagePackSerializer.Serialize<ChaFileClothes>(charInfo.nowCoordinate.clothes);
			}
			charInfo.nowCoordinate.LoadFile(selectPath);
			if (clothOnly)
			{
				charInfo.nowCoordinate.accessory = MessagePackSerializer.Deserialize<ChaFileAccessory>(bytes);
			}
			if (accessoryOnly)
			{
				charInfo.nowCoordinate.clothes = MessagePackSerializer.Deserialize<ChaFileClothes>(bytes2);
			}
			charInfo.Reload(false, true, true, true, true);
			charInfo.AssignCoordinate();
			if (charInfo.sex == 1)
			{
				charInfo.UpdateBustSoftnessAndGravity();
				ocichar.skirtDynamic = AddObjectFemale.GetSkirtDynamic(charInfo.objClothes);
				ocichar.ActiveFK(OIBoneInfo.BoneGroup.Skirt, ocichar.oiCharInfo.activeFK[6], ocichar.oiCharInfo.activeFK[6]);
			}
		}

		// Token: 0x0600009D RID: 157 RVA: 0x000065A8 File Offset: 0x000047A8
		private void ChangeChara()
		{
			foreach (OCIChar ociChar in (from v in Singleton<GuideObjectManager>.Instance.selectObjectKey
			select global::Studio.Studio.GetCtrlInfo(v) as OCIChar into v
			where v != null
			where v.oiCharInfo.sex == 1
			select v).ToArray<OCIChar>())
			{
				this.ChangeChara(ociChar);
			}
		}

		// Token: 0x0600009E RID: 158 RVA: 0x0000664C File Offset: 0x0000484C
		private void ChangeChara(OCIChar ociChar)
		{
			foreach (OCIChar.BoneInfo boneInfo in (from v in ociChar.listBones
			where v.boneGroup == OIBoneInfo.BoneGroup.Hair
			select v).ToList<OCIChar.BoneInfo>())
			{
				Singleton<GuideObjectManager>.Instance.Delete(boneInfo.guideObject, true);
			}
			ociChar.listBones = (from v in ociChar.listBones
			where v.boneGroup != OIBoneInfo.BoneGroup.Hair
			select v).ToList<OCIChar.BoneInfo>();
			int[] array = (from b in ociChar.oiCharInfo.bones
			where b.Value.@group == OIBoneInfo.BoneGroup.Hair
			select b.Key).ToArray<int>();
			for (int k = 0; k < array.Length; k++)
			{
				ociChar.oiCharInfo.bones.Remove(array[k]);
			}
			ociChar.skirtDynamic = null;
			ChaControl charInfo = ociChar.charInfo;
			byte[] bytes = null;
			byte[] bytes2 = null;
			bool flag = this.replaceCharaClothesOnly || this.replaceCharaAccOnly;
			if (flag)
			{
				if (!this.replaceCharaAccOnly)
				{
					bytes = MessagePackSerializer.Serialize<ChaFileAccessory>(charInfo.nowCoordinate.accessory);
				}
				else if (!this.replaceCharaClothesOnly)
				{
					bytes2 = MessagePackSerializer.Serialize<ChaFileClothes>(charInfo.nowCoordinate.clothes);
				}
			}
			bool parameter = this.replaceCharaHeadOnly && this.replaceCharaBodyOnly;
			charInfo.chaFile.LoadFileLimited(this.charaFileSort.selectPath, byte.MaxValue, this.replaceCharaHeadOnly, this.replaceCharaBodyOnly, this.replaceCharaHairOnly, parameter, flag);
			if (flag)
			{
				if (!this.replaceCharaAccOnly)
				{
					charInfo.chaFile.coordinate.accessory = MessagePackSerializer.Deserialize<ChaFileAccessory>(bytes);
				}
				if (!this.replaceCharaClothesOnly)
				{
					charInfo.chaFile.coordinate.clothes = MessagePackSerializer.Deserialize<ChaFileClothes>(bytes2);
				}
			}
			charInfo.ChangeNowCoordinate(false, true);
			charInfo.Reload(!flag, !this.replaceCharaHeadOnly, !this.replaceCharaHairOnly, !this.replaceCharaBodyOnly, true);
			for (int j = 0; j < 2; j++)
			{
				GameObject gameObject = ociChar.charInfo.objHair.SafeGet(j);
				if (gameObject != null)
				{
					AddObjectAssist.ArrangeNames(gameObject.transform);
				}
			}
			ociChar.treeNodeObject.textName = ociChar.charInfo.chaFile.parameter.fullname;
			AddObjectAssist.InitHairBone(ociChar, Singleton<Info>.Instance.dicBoneInfo);
			ociChar.skirtDynamic = AddObjectFemale.GetSkirtDynamic(ociChar.charInfo.objClothes);
			ociChar.InitFK(null);
			foreach (var fkPart in FKCtrl.parts.Select((OIBoneInfo.BoneGroup p, int i) => new
			{
				p,
				i
			}))
			{
				ociChar.ActiveFK(fkPart.p, ociChar.oiCharInfo.activeFK[fkPart.i], ociChar.oiCharInfo.activeFK[fkPart.i]);
			}
			ociChar.ActiveKinematicMode(OICharInfo.KinematicMode.FK, ociChar.oiCharInfo.enableFK, true);
			ociChar.UpdateFKColor(new OIBoneInfo.BoneGroup[]
			{
				OIBoneInfo.BoneGroup.Hair
			});
			ociChar.ChangeEyesOpen(ociChar.charFileStatus.eyesOpenMax);
			ociChar.ChangeBlink(ociChar.charFileStatus.eyesBlink);
			ociChar.ChangeMouthOpen(ociChar.oiCharInfo.mouthOpen);
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00006A10 File Offset: 0x00004C10
		private void RemoveBoneModifier(OCIChar ociChar)
		{
			Component[] components = ociChar.charInfo.gameObject.GetComponents<Component>();
			int i = 0;
			while (i < components.Length)
			{
				Component component = components[i];
				if (component.GetType().Name == "BoneController")
				{
					if (StudioCharaListUtil.m_ClearModifiers == null)
					{
						StudioCharaListUtil.m_ClearModifiers = component.GetType().GetMethod("ClearModifiers", new Type[0]);
					}
					if (StudioCharaListUtil.m_ClearModifiers != null)
					{
						StudioCharaListUtil.m_ClearModifiers.Invoke(component, new object[0]);
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00006A9F File Offset: 0x00004C9F
		private IEnumerator TryLoadBoneModifierCo(ChaControl charInfo)
		{
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			Component[] components = charInfo.gameObject.GetComponents<Component>();
			int i = 0;
			while (i < components.Length)
			{
				Component component = components[i];
				if (component.GetType().Name == "BoneController")
				{
					if (StudioCharaListUtil.m_LoadFromFile == null)
					{
						StudioCharaListUtil.m_LoadFromFile = component.GetType().GetMethod("LoadFromFile", new Type[0]);
					}
					if (StudioCharaListUtil.m_LoadFromFile != null)
					{
						StudioCharaListUtil.m_LoadFromFile.Invoke(component, new object[0]);
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			yield break;
		}

		// Token: 0x0400006E RID: 110
		private static FieldInfo f_charaFileSort = typeof(CharaList).GetField("charaFileSort", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField);

		// Token: 0x0400006F RID: 111
		private static FieldInfo f_sortType = typeof(CharaFileSort).GetField("sortType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField);

		// Token: 0x04000070 RID: 112
		private static FieldInfo f_costumeInfo = typeof(MPCharCtrl).GetField("costumeInfo", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField);

		// Token: 0x04000071 RID: 113
		private static Type t_CostumeInfo = typeof(MPCharCtrl).GetNestedType("CostumeInfo", BindingFlags.Public | BindingFlags.NonPublic);

		// Token: 0x04000072 RID: 114
		private static FieldInfo f_CostumeInfo_fileSort = StudioCharaListUtil.t_CostumeInfo.GetField("fileSort", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField);

		// Token: 0x04000073 RID: 115
		private static MethodInfo m_CostumeInfo_InitList = StudioCharaListUtil.t_CostumeInfo.GetMethod("InitList", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

		// Token: 0x04000074 RID: 116
		private static FieldInfo f_m_OCIChar = typeof(MPCharCtrl).GetField("m_OCIChar", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField);

		// Token: 0x04000075 RID: 117
		private string _namePattern = "";

		// Token: 0x04000076 RID: 118
		public RectTransform orgRect;

		// Token: 0x04000077 RID: 119
		public bool isCoordinate;

		// Token: 0x04000078 RID: 120
		private global::Studio.Studio studio;

		// Token: 0x04000079 RID: 121
		private CharaList charaList;

		// Token: 0x0400007A RID: 122
		private MPCharCtrl mpCharCtrl;

		// Token: 0x0400007B RID: 123
		private object costumeInfo;

		// Token: 0x0400007C RID: 124
		private CharaFileSort charaFileSort;

		// Token: 0x0400007D RID: 125
		public bool showGUI = true;

		// Token: 0x0400007E RID: 126
		private List<CharaFileInfo> listMatched = new List<CharaFileInfo>();

		// Token: 0x0400007F RID: 127
		private bool isMale;

		// Token: 0x04000080 RID: 128
		private int windowID = 8725;

		// Token: 0x04000081 RID: 129
		private const int panelWidth = 120;

		// Token: 0x04000082 RID: 130
		private const int panelHeight = 180;

		// Token: 0x04000083 RID: 131
		private const int panelHeightCoordinate = 180;

		// Token: 0x04000084 RID: 132
		private Rect windowRect = new Rect(200f, 100f, 120f, 150f);

		// Token: 0x04000085 RID: 133
		private string windowTitle = "";

		// Token: 0x04000086 RID: 134
		private bool replaceCharaHairOnly;

		// Token: 0x04000087 RID: 135
		private bool replaceCharaHeadOnly;

		// Token: 0x04000088 RID: 136
		private bool replaceCharaBodyOnly;

		// Token: 0x04000089 RID: 137
		private bool replaceCharaClothesOnly;

		// Token: 0x0400008A RID: 138
		private bool replaceCharaAccOnly;

		// Token: 0x0400008B RID: 139
		private Button changeButton;

		// Token: 0x0400008C RID: 140
		private static MethodInfo m_LoadFromFile = null;

		// Token: 0x0400008D RID: 141
		private static MethodInfo m_ClearModifiers = null;
	}
}
