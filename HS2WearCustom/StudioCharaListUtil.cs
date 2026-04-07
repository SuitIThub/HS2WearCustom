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
	public class StudioCharaListUtil : MonoBehaviour
	{
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

		public static Rect RectTransformToScreenSpace(RectTransform transform)
		{
			Vector2 vector = Vector2.Scale(transform.rect.size, transform.lossyScale);
			Rect result = new Rect(transform.position.x, (float)Screen.height - transform.position.y, vector.x, vector.y);
			result.x -= transform.pivot.x * vector.x;
			result.y -= (1f - transform.pivot.y) * vector.y;
			return result;
		}

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

		public void Clear()
		{
			this._namePattern = "";
		}

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

		private static FieldInfo f_charaFileSort = typeof(CharaList).GetField("charaFileSort", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField);

		private static FieldInfo f_sortType = typeof(CharaFileSort).GetField("sortType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField);

		private static FieldInfo f_costumeInfo = typeof(MPCharCtrl).GetField("costumeInfo", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField);

		private static Type t_CostumeInfo = typeof(MPCharCtrl).GetNestedType("CostumeInfo", BindingFlags.Public | BindingFlags.NonPublic);

		private static FieldInfo f_CostumeInfo_fileSort = StudioCharaListUtil.t_CostumeInfo.GetField("fileSort", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField);

		private static MethodInfo m_CostumeInfo_InitList = StudioCharaListUtil.t_CostumeInfo.GetMethod("InitList", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

		private static FieldInfo f_m_OCIChar = typeof(MPCharCtrl).GetField("m_OCIChar", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField);

		private string _namePattern = "";

		public RectTransform orgRect;

		public bool isCoordinate;

		private global::Studio.Studio studio;

		private CharaList charaList;

		private MPCharCtrl mpCharCtrl;

		private object costumeInfo;

		private CharaFileSort charaFileSort;

		public bool showGUI = true;

		private List<CharaFileInfo> listMatched = new List<CharaFileInfo>();

		private bool isMale;

		private int windowID = 8725;

		private const int panelWidth = 120;

		private const int panelHeight = 180;

		private const int panelHeightCoordinate = 180;

		private Rect windowRect = new Rect(200f, 100f, 120f, 150f);

		private string windowTitle = "";

		private bool replaceCharaHairOnly;

		private bool replaceCharaHeadOnly;

		private bool replaceCharaBodyOnly;

		private bool replaceCharaClothesOnly;

		private bool replaceCharaAccOnly;

		private Button changeButton;

		private static MethodInfo m_LoadFromFile = null;

		private static MethodInfo m_ClearModifiers = null;
	}
}
