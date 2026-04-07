using System;
using AIChara;
using CharaCustom;
using Illusion.Extensions;
using Studio;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace HS2WearCustom
{
	// Token: 0x02000009 RID: 9
	public class AIWearControllerUI : MonoBehaviour
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
					this.SafeReleaseCustomTexture();
				}
			}
		}

		public void Init(GameObject root, GameObject cgTrans)
		{
			this.selChar = null;
			this.selTextureInited = false;
			this.colorInfo = new AIWearControllerUI.ColorInfo[4];
			this.InitHairPreset();
			for (int i = 0; i < 4; i++)
			{
				int no = i;
				this.colorInfo[i] = new AIWearControllerUI.ColorInfo();
				Transform root2 = root.transform.Find("Image Color" + (i + 1) + " Background");
				this.colorInfo[i].Init(root2);
				this.colorInfo[i]._colorMain.buttonColor.OnClickAsObservable().Subscribe(delegate(Unit _)
				{
					this.OnClickColorMain(no);
				});
				this.colorInfo[i]._colorMain.buttonColorDefault.OnClickAsObservable().Subscribe(delegate(Unit _)
				{
					this.OnClickColorMainDef(no);
				});
				this.colorInfo[i].inputMetallic.slider.onValueChanged.AddListener(delegate(float f)
				{
					this.OnValueChangeMetallic(no, f);
				});
				this.colorInfo[i].inputMetallic.input.onEndEdit.AddListener(delegate(string s)
				{
					this.OnEndEditMetallic(no, s);
				});
				this.colorInfo[i].inputMetallic.buttonDefault.OnClickAsObservable().Subscribe(delegate(Unit _)
				{
					this.OnClickMetallicDef(no);
				});
				this.colorInfo[i].inputGlossiness.slider.onValueChanged.AddListener(delegate(float f)
				{
					this.OnValueChangeGlossiness(no, f);
				});
				this.colorInfo[i].inputGlossiness.input.onEndEdit.AddListener(delegate(string s)
				{
					this.OnEndEditGlossiness(no, s);
				});
				this.colorInfo[i].inputGlossiness.buttonDefault.OnClickAsObservable().Subscribe(delegate(Unit _)
				{
					this.OnClickGlossinessDef(no);
				});
				this.colorInfo[i]._buttonPattern.OnClickAsObservable().Subscribe(delegate(Unit _)
				{
					this.OnClickPattern(no);
				});
				this.colorInfo[i]._colorPattern.buttonColor.OnClickAsObservable().Subscribe(delegate(Unit _)
				{
					this.OnClickColorPattern(no);
				});
				this.colorInfo[i]._colorPattern.buttonColorDefault.OnClickAsObservable().Subscribe(delegate(Unit _)
				{
					this.OnClickColorPatternDef(no);
				});
				for (int j = 0; j < 4; j++)
				{
					int no2 = j;
					this.colorInfo[i][j].slider.onValueChanged.AddListener(delegate(float f)
					{
						this.OnValueChangeUT(no, no2, f);
					});
					this.colorInfo[i][j].input.onEndEdit.AddListener(delegate(string s)
					{
						this.OnEndEditUT(no, no2, s);
					});
				}
				this.colorInfo[i][4].slider.onValueChanged.AddListener(delegate(float f)
				{
					this.OnValueChangeRot(no, f);
				});
			}
			this.origSliderMin = new float[5];
			this.origSliderMax = new float[5];
			for (int si = 0; si < 5; si++)
			{
				this.origSliderMin[si] = this.colorInfo[0]._input[si].slider.minValue;
				this.origSliderMax[si] = this.colorInfo[0]._input[si].slider.maxValue;
			}
			this.hairAcsColors = new AIWearControllerUI.ColorCombination[3];
			Transform transform = root.transform.Find("Image Shadow");
			this.colorShadow = new AIWearControllerUI.ColorCombination();
			this.colorShadow.objRoot = transform.gameObject;
			this.colorShadow.imageColor = transform.Find("Button Shadow Color").GetComponent<Image>();
			this.colorShadow.buttonColor = this.colorShadow.imageColor.transform.GetComponent<Button>();
			this.colorShadow.buttonColorDefault = transform.Find("Button Shadow Color Default").GetComponent<Button>();
			transform.Find("TextMeshPro Shadow Color").GetComponent<TextMeshProUGUI>().text = "Acc Color 1";
			this.hairAcsColors[0] = this.colorShadow;
			Transform transform2 = root.transform.Find("Image Emission");
			this.emissionInfo = new AIWearControllerUI.ColorInputCombination();
			this.emissionInfo.objRoot = transform2.gameObject;
			Transform transform3 = transform2.Find("Color");
			this.emissionInfo.color.objRoot = transform3.gameObject;
			this.emissionInfo.color.imageColor = transform3.Find("Button Color").GetComponent<Image>();
			this.emissionInfo.color.buttonColor = this.emissionInfo.color.imageColor.transform.GetComponent<Button>();
			this.emissionInfo.color.buttonColorDefault = transform3.Find("Button Color Default").GetComponent<Button>();
			Transform transform4 = transform2.Find("Power");
			this.emissionInfo.input.Init(transform4);
			transform4.gameObject.SetActiveIfDifferent(false);
			transform3.Find("TextMeshPro").GetComponent<TextMeshProUGUI>().text = "Acc Color 2";
			this.hairAcsColors[1] = this.emissionInfo.color;
			Transform transform5 = root.transform.Find("Image Alpha");
			this.inputAlpha = new AIWearControllerUI.InputCombination();
			this.inputAlpha.objRoot = transform5.gameObject;
			this.inputAlpha.slider = transform5.Find("Slider Alpha").GetComponent<Slider>();
			this.inputAlpha.input = transform5.Find("TextMeshPro - InputField Alpha").GetComponent<TMP_InputField>();
			Transform transform6 = root.transform.Find("Image Light Cancel");
			this.inputLightCancel = new AIWearControllerUI.InputCombination();
			this.inputLightCancel.objRoot = transform6.gameObject;
			this.inputLightCancel.slider = transform6.Find("Slider Light Cancel").GetComponent<Slider>();
			this.inputLightCancel.slider.maxValue = 100f;
			this.inputLightCancel.input = transform6.Find("TextMeshPro - InputField Light Cancel").GetComponent<TMP_InputField>();
			this.inputLightCancel.buttonDefault = transform6.Find("Button Light Cancel Default").GetComponent<Button>();
			this.slotLabel = transform6.Find("TextMeshPro Light Cancel").GetComponent<TextMeshProUGUI>();
			this.slotLabel.text = "Break Rate";
			this.inputLightCancel.slider.onValueChanged.AddListener(delegate(float f)
			{
				this.OnValueChangeBreak(f);
			});
			this.inputLightCancel.input.onEndEdit.AddListener(delegate(string s)
			{
				this.OnEndEditSlotOrBreak(s);
			});
			Transform transform7 = root.transform.Find("Image Line");
			this.lineInfo = new AIWearControllerUI.ColorInputCombination();
			this.lineInfo.objRoot = transform7.gameObject;
			this.lineInfo.color.imageColor = transform7.Find("Button Color").GetComponent<Image>();
			this.lineInfo.color.buttonColor = this.lineInfo.color.imageColor.transform.GetComponent<Button>();
			this.lineInfo.color.buttonColorDefault = transform7.Find("Button Color Default").GetComponent<Button>();
			this.lineInfo.input.slider = transform7.Find("Slider Width").GetComponent<Slider>();
			this.lineInfo.input.input = transform7.Find("TextMeshPro - InputField Width").GetComponent<TMP_InputField>();
			this.lineInfo.input.buttonDefault = transform7.Find("Button Width Default").GetComponent<Button>();
			transform7.Find("TextMeshPro Width").gameObject.SetActiveIfDifferent(false);
			this.lineInfo.input.slider.gameObject.SetActiveIfDifferent(false);
			this.lineInfo.input.input.gameObject.SetActiveIfDifferent(false);
			this.lineInfo.input.buttonDefault.gameObject.SetActiveIfDifferent(false);
			transform7.Find("TextMeshPro Color").GetComponent<TextMeshProUGUI>().text = "Acc Color 3";
			this.hairAcsColors[2] = this.lineInfo.color;
			for (int k = 0; k < 3; k++)
			{
				int no = k;
				this.hairAcsColors[k].buttonColor.OnClickAsObservable().Subscribe(delegate(Unit _)
				{
					this.OnClickColorHairAcs(no);
				});
			}
			Transform transform8 = root.transform.Find("Image FK");
			this.hairSetInfo.objRoot = transform8.gameObject;
			this.hairSetInfo.toggleDetail = transform8.Find("Toggle FK").GetComponent<Toggle>();
			this.hairSetInfo.toggleSame = transform8.Find("Toggle Dynamic Bone").GetComponent<Toggle>();
			transform8.Find("TextMeshPro FK").GetComponent<TextMeshProUGUI>().text = "Detail Settings";
			transform8.Find("TextMeshPro Dynamic Bone").GetComponent<TextMeshProUGUI>().text = "Uniform F/B";
			Transform transform9 = root.transform.Find("Image Option");
			this.optionInfo.objRoot = transform9.gameObject;
			this.optionInfo.toggleAll = transform9.Find("ALL").GetComponent<Toggle>();
			root.transform.Find("Image Panel Background").gameObject.SetActiveIfDifferent(false);
			root.transform.Find("Image Anime").gameObject.SetActiveIfDifferent(false);
			this.cgPattern = cgTrans.transform.GetComponent<CanvasGroup>();
			this.hairSetInfo.toggleDetail.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				this.UpdateInfo();
			});
		}

		private void InitHairPreset()
		{
			if (this.items == null)
			{
				this.items = new CustomHairColorPreset.HairColorInfo[]
				{
					new CustomHairColorPreset.HairColorInfo(),
					new CustomHairColorPreset.HairColorInfo(),
					new CustomHairColorPreset.HairColorInfo(),
					new CustomHairColorPreset.HairColorInfo(),
					new CustomHairColorPreset.HairColorInfo(),
					new CustomHairColorPreset.HairColorInfo()
				};
				this.items[0].baseColor = new Color(0.0859375f, 0.0859375f, 0.0859375f);
				this.items[0].topColor = new Color(0.109375f, 0.109375f, 0.109375f);
				this.items[0].underColor = new Color(0.48828125f, 0.48828125f, 0.48828125f);
				this.items[0].specular = new Color(0.5546875f, 0.54296875f, 0.54296875f);
				this.items[0].metallic = 0.03f;
				this.items[4].baseColor = new Color(0.2890625f, 0.234375f, 0.21875f);
				this.items[4].topColor = new Color(0.1875f, 0.15234375f, 0.1328125f);
				this.items[4].underColor = new Color(0.6015625f, 0.55859375f, 0.53515625f);
				this.items[4].specular = new Color(0.5546875f, 0.4609375f, 0.41015625f);
				this.items[4].metallic = 0.314f;
				this.items[5].baseColor = new Color(0.58203125f, 0.4296875f, 0.3359375f);
				this.items[5].topColor = new Color(0.6171815f, 0.4453125f, 0.3515625f);
				this.items[5].underColor = new Color(0.82421875f, 0.66796875f, 0.578125f);
				this.items[5].specular = new Color(0.95703125f, 0.703125f, 0.546875f);
				this.items[5].metallic = 0.299f;
				this.items[1].baseColor = new Color(0.74609375f, 0.6796875f, 0.58203125f);
				this.items[1].topColor = new Color(0.77734375f, 0.76171875f, 0.7578125f);
				this.items[1].underColor = new Color(0.91796875f, 0.828125f, 0.7578125f);
				this.items[1].specular = new Color(0.8359375f, 0.73828125f, 0.65234375f);
				this.items[2].metallic = 0.185f;
				this.items[2].baseColor = new Color(0.6171875f, 0.55078125f, 0.46484375f);
				this.items[2].topColor = new Color(0.58984375f, 0.51953125f, 0.4296875f);
				this.items[2].underColor = new Color(0.81640625f, 0.71484375f, 0.640625f);
				this.items[2].specular = new Color(0.7578125f, 0.65234375f, 0.546875f);
				this.items[3].baseColor = new Color(0.3125f, 0.25f, 0.8203125f);
				this.items[3].topColor = new Color(0.3828125f, 0.3125f, 0.6015625f);
				this.items[3].underColor = new Color(0.9375f, 0.0625f, 0.328125f);
				this.items[3].specular = new Color(0.72265625f, 0.62109375f, 0.5078125f);
			}
		}

		private void SafeReleaseCustomTexture()
		{
			if (this.selChar != null && this.selTextureInited)
			{
				this.selChar.releaseCustomInputTexture = true;
				this.selChar.loadWithDefaultColorAndPtn = false;
				for (int i = 0; i < 8; i++)
				{
					this.ReleaseBaseCustomTextureClothes(i);
				}
				this.selTextureInited = false;
			}
		}

		private void ReleaseBaseCustomTextureClothes(int i)
		{
			try
			{
				this.selChar.CallPrivateMethod("ReleaseBaseCustomTextureClothes", new object[]
				{
					i,
					false
				});
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
		}

		private void InitBaseCustomTextureClothes(int i)
		{
			try
			{
				this.selChar.CallPrivateMethod("InitBaseCustomTextureClothes", new object[]
				{
					i
				});
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
		}

		public void UpdateInfo()
		{
			if (this.ociChar.charInfo != this.selChar)
			{
				this.SafeReleaseCustomTexture();
				this.selChar = this.ociChar.charInfo;
			}
			if (this.selChar == null)
			{
				return;
			}
			this.isUpdateInfo = true;
			for (int i = 0; i < 4; i++)
			{
				this.colorInfo[i].enable = false;
				this.colorInfo[i]._colorMain.active = true;
				this.colorInfo[i]._buttonPattern.interactable = true;
				if (this.origSliderMin != null)
				{
					for (int si = 0; si < 5; si++)
					{
						this.colorInfo[i]._input[si].slider.minValue = this.origSliderMin[si];
						this.colorInfo[i]._input[si].slider.maxValue = this.origSliderMax[si];
					}
				}
			}
			this.inputAlpha.active = false;
			this.optionInfo.Active = false;
			if ((this.aiWGList.select >= ChaListDefine.CategoryNo.mo_top && this.aiWGList.select <= ChaListDefine.CategoryNo.mo_shoes) || (this.aiWGList.select >= ChaListDefine.CategoryNo.fo_top && this.aiWGList.select <= ChaListDefine.CategoryNo.fo_shoes))
			{
				if (!this.selTextureInited)
				{
					this.selChar.releaseCustomInputTexture = false;
					this.selChar.loadWithDefaultColorAndPtn = false;
					for (int j = 0; j < 8; j++)
					{
						this.InitBaseCustomTextureClothes(j);
					}
					this.selTextureInited = true;
				}
				this.wearType = 1;
				CmpClothes customClothesComponent = this.selChar.GetCustomClothesComponent(this.aiWGList.selectKind);
				if (customClothesComponent != null)
				{
					ChaFileClothes.PartsInfo partsInfo = this.selChar.nowCoordinate.clothes.parts[this.aiWGList.selectKind];
					if (customClothesComponent.useColorN01 || customClothesComponent.useColorA01)
					{
						this.updateClothColorInfo(this.colorInfo[0], partsInfo.colorInfo[0]);
					}
					if (customClothesComponent.useColorN02 || customClothesComponent.useColorA02)
					{
						this.updateClothColorInfo(this.colorInfo[1], partsInfo.colorInfo[1]);
					}
					if (customClothesComponent.useColorN03 || customClothesComponent.useColorA03)
					{
						this.updateClothColorInfo(this.colorInfo[2], partsInfo.colorInfo[2]);
					}
					this.inputLightCancel.active = true;
					this.slotLabel.text = "Break Rate";
					this.inputLightCancel.slider.wholeNumbers = false;
					this.inputLightCancel.slider.minValue = 0f;
					this.inputLightCancel.slider.maxValue = 100f;
					this.inputLightCancel.value = partsInfo.breakRate;
				}
				else
				{
					this.inputLightCancel.active = false;
				}
				this.colorShadow.active = false;
				this.emissionInfo.active = false;
				this.lineInfo.active = false;
				this.hairSetInfo.Active = false;
			}
			else if (this.aiWGList.select >= ChaListDefine.CategoryNo.so_hair_b && this.aiWGList.select <= ChaListDefine.CategoryNo.so_hair_o)
			{
				this.wearType = 2;
				CmpHair customHairComponent = this.selChar.GetCustomHairComponent(this.aiWGList.selectKind);
				if (customHairComponent != null)
				{
					ChaFileHair.PartsInfo partsInfo2 = this.selChar.chaFile.custom.hair.parts[this.aiWGList.selectKind];
					this.colorInfo[0].enable = true;
					this.colorInfo[0].EnableMetallic = true;
					this.colorInfo[0].EnablePattern = false;
					this.colorInfo[0].colorMain = partsInfo2.baseColor;
					this.colorInfo[0].inputMetallic.value = partsInfo2.metallic;
					this.colorInfo[0].inputGlossiness.value = partsInfo2.smoothness;
					this.colorInfo[1].colorMain = partsInfo2.topColor;
					this.colorInfo[2].colorMain = partsInfo2.underColor;
					this.colorInfo[3].colorMain = partsInfo2.specular;
					for (int k = 1; k < 4; k++)
					{
						this.updateHairColorInfo(this.colorInfo[k], this.hairSetInfo.toggleDetail.isOn);
					}
					this.hairSetInfo.Active = true;
					this.colorShadow.active = customHairComponent.useAcsColor01;
					if (customHairComponent.useAcsColor01)
					{
						this.colorShadow.color = partsInfo2.acsColorInfo[0].color;
					}
					this.emissionInfo.active = customHairComponent.useAcsColor02;
					if (customHairComponent.useAcsColor02)
					{
						this.emissionInfo.color.color = partsInfo2.acsColorInfo[1].color;
					}
					this.lineInfo.active = customHairComponent.useAcsColor03;
					if (customHairComponent.useAcsColor03)
					{
						this.lineInfo.color.color = partsInfo2.acsColorInfo[2].color;
					}
				}
				else
				{
					this.hairSetInfo.Active = false;
					this.colorShadow.active = false;
					this.emissionInfo.active = false;
					this.lineInfo.active = false;
				}
				this.inputLightCancel.active = false;
			}
			else if (this.aiWGList.select >= ChaListDefine.CategoryNo.ao_head && this.aiWGList.select <= ChaListDefine.CategoryNo.ao_kokan)
			{
				this.wearType = 3;
				int slot = this.accessorySlot;
				CmpAccessory cmpAcs = null;
				bool isAccessory = false;
				if (slot >= 0 && slot < this.selChar.cmpAccessory.Length)
				{
					cmpAcs = this.selChar.cmpAccessory[slot];
					isAccessory = this.selChar.IsAccessory(slot);
				}
				ChaFileAccessory.PartsInfo accParts = this.selChar.nowCoordinate.accessory.parts[slot];

				bool[] useColor = new bool[4];
				bool[] useMetallic = new bool[4];
				if (cmpAcs != null && isAccessory)
				{
					useColor = new bool[] { cmpAcs.useColor01, cmpAcs.useColor02, cmpAcs.useColor03, cmpAcs.useGloss04 || cmpAcs.useMetallic04 };
					useMetallic = new bool[] { cmpAcs.useMetallic01, cmpAcs.useMetallic02, cmpAcs.useMetallic03, cmpAcs.useMetallic04 };
				}

				string[] transformLabels = new string[] { "Position", "Rotation", "Scale" };
				for (int c = 0; c < 3; c++)
				{
					this.colorInfo[c].enable = true;

					if (useColor[c])
					{
						this.colorInfo[c]._colorMain.active = true;
						this.colorInfo[c].EnableMetallic = useMetallic[c];
						this.colorInfo[c].colorMain = accParts.colorInfo[c].color;
						this.colorInfo[c].inputMetallic.value = accParts.colorInfo[c].metallicPower;
						this.colorInfo[c].inputGlossiness.value = accParts.colorInfo[c].glossPower;
					}
					else
					{
						this.colorInfo[c]._colorMain.active = false;
						this.colorInfo[c].EnableMetallic = false;
					}

					this.colorInfo[c].EnablePattern = true;
					this.colorInfo[c]._textPattern.text = transformLabels[c];
					this.colorInfo[c]._buttonPattern.interactable = false;
					this.colorInfo[c]._colorPattern.active = false;
					this.colorInfo[c]._toggleObj.SetActiveIfDifferent(false);
					this.colorInfo[c]._input[3].active = false;
					this.colorInfo[c]._input[4].active = false;
					this.colorInfo[c]._input[0].active = true;
					this.colorInfo[c]._input[1].active = true;
					this.colorInfo[c]._input[2].active = true;

					float minVal, maxVal;
					if (c == 0) { minVal = -0.5f; maxVal = 0.5f; }
					else if (c == 1) { minVal = -180f; maxVal = 180f; }
					else { minVal = 0.01f; maxVal = 5f; }

					for (int j = 0; j < 3; j++)
					{
						this.colorInfo[c]._input[j].slider.minValue = minVal;
						this.colorInfo[c]._input[j].slider.maxValue = maxVal;
					}

					if (isAccessory && accParts.addMove != null)
					{
						Vector3 val = accParts.addMove[0, c];
						this.colorInfo[c]._input[0].value = val.x;
						this.colorInfo[c]._input[1].value = val.y;
						this.colorInfo[c]._input[2].value = val.z;
					}
					else
					{
						this.colorInfo[c]._input[0].value = (c == 2) ? 1f : 0f;
						this.colorInfo[c]._input[1].value = (c == 2) ? 1f : 0f;
						this.colorInfo[c]._input[2].value = (c == 2) ? 1f : 0f;
					}
				}

				if (useColor[3])
				{
					this.colorInfo[3].enable = true;
					this.colorInfo[3].EnableMetallic = useMetallic[3];
					this.colorInfo[3].EnablePattern = false;
					this.colorInfo[3].colorMain = accParts.colorInfo[3].color;
					this.colorInfo[3].inputMetallic.value = accParts.colorInfo[3].metallicPower;
					this.colorInfo[3].inputGlossiness.value = accParts.colorInfo[3].glossPower;
				}

				this.inputLightCancel.active = false;

				this.colorShadow.active = false;
				this.emissionInfo.active = false;
				this.lineInfo.active = false;
				this.hairSetInfo.Active = false;
			}
			this.isUpdateInfo = false;
		}

		private void updateClothColorInfo(AIWearControllerUI.ColorInfo ci, ChaFileClothes.PartsInfo.ColorInfo pci)
		{
			ci.enable = true;
			ci.EnableMetallic = true;
			ci.EnablePattern = true;
			ci.EnablePattenDetail = (pci.pattern > 0);
			ci.colorMain = pci.baseColor;
			ci.inputMetallic.value = pci.metallicPower;
			ci.inputGlossiness.value = pci.glossPower;
			ci.textPattern = pci.pattern.ToString();
			ci.colorPattern = pci.patternColor;
			ci._input[0].value = pci.layout.x;
			ci._input[1].value = pci.layout.y;
			ci._input[2].value = pci.layout.z;
			ci._input[3].value = pci.layout.w;
			ci._input[4].value = pci.rotation;
		}

		private void updateHairColorInfo(AIWearControllerUI.ColorInfo ci, bool show)
		{
			ci.enable = show;
			ci.EnableMetallic = false;
			ci.EnablePattern = false;
		}

		private void OnClickColorMain(int _idx)
		{
			if (this.wearType == 1)
			{
				string text2 = string.Concat(new object[]
				{
					"Wear Color ",
					this.aiWGList.selectKind,
					" ",
					_idx
				});
				if (Singleton<global::Studio.Studio>.Instance.colorPalette.Check(text2))
				{
					Singleton<global::Studio.Studio>.Instance.colorPalette.visible = false;
					return;
				}
				ChaFileClothes.PartsInfo partsInfo = this.selChar.nowCoordinate.clothes.parts[this.aiWGList.selectKind];
				ChaFileClothes.PartsInfo orgInfo = this.selChar.chaFile.coordinate.clothes.parts[this.aiWGList.selectKind];
				Singleton<global::Studio.Studio>.Instance.colorPalette.Setup(text2, partsInfo.colorInfo[_idx].baseColor, delegate(Color _c)
				{
					partsInfo.colorInfo[_idx].baseColor = _c;
					orgInfo.colorInfo[_idx].baseColor = _c;
					this.selChar.ChangeCustomClothes(this.aiWGList.selectKind, true, false, false, false);
					this.colorInfo[_idx].colorMain = _c;
				}, true);
			}
			else if (this.wearType == 2)
			{
				string text = string.Concat(new object[]
				{
					"Hair Color ",
					this.aiWGList.selectKind,
					" ",
					_idx
				});
				if (Singleton<global::Studio.Studio>.Instance.colorPalette.Check(text))
				{
					Singleton<global::Studio.Studio>.Instance.colorPalette.visible = false;
					return;
				}
				ChaFileHair.PartsInfo[] parts = this.selChar.chaFile.custom.hair.parts;
				Singleton<global::Studio.Studio>.Instance.colorPalette.Setup(text, (_idx == 1) ? parts[this.aiWGList.selectKind].topColor : ((_idx == 2) ? parts[this.aiWGList.selectKind].underColor : ((_idx == 3) ? parts[this.aiWGList.selectKind].specular : parts[this.aiWGList.selectKind].baseColor)), delegate(Color _c)
				{
					this.SetHairColor(_idx, _c);
				}, true);
			}
			else if (this.wearType == 3)
			{
				string text3 = string.Concat(new object[]
				{
					"Acc Color ",
					this.accessorySlot,
					" ",
					_idx
				});
				if (Singleton<global::Studio.Studio>.Instance.colorPalette.Check(text3))
				{
					Singleton<global::Studio.Studio>.Instance.colorPalette.visible = false;
					return;
				}
				ChaFileAccessory.PartsInfo accInfo = this.selChar.nowCoordinate.accessory.parts[this.accessorySlot];
				ChaFileAccessory.PartsInfo accOrgInfo = this.selChar.chaFile.coordinate.accessory.parts[this.accessorySlot];
				Singleton<global::Studio.Studio>.Instance.colorPalette.Setup(text3, accInfo.colorInfo[_idx].color, delegate(Color _c)
				{
					accInfo.colorInfo[_idx].color = _c;
					accOrgInfo.colorInfo[_idx].color = _c;
					this.selChar.ChangeAccessoryColor(this.accessorySlot);
					this.colorInfo[_idx].colorMain = _c;
				}, true);
			}
		}

		private void SetHairColor(int _idx, Color _c)
		{
			ChaFileHair.PartsInfo[] parts = this.selChar.chaFile.custom.hair.parts;
			if (_idx == 0 && this.hairSetInfo.toggleDetail.isOn)
			{
				Color color;
				Color color2;
				Color color3;
				this.selChar.CreateHairColor(_c, out color, out color2, out color3);
				for (int i = 0; i < parts.Length; i++)
				{
					if (this.hairSetInfo.toggleSame.isOn || i == this.aiWGList.selectKind)
					{
						parts[i].baseColor = _c;
						parts[i].topColor = color;
						parts[i].underColor = color2;
						parts[i].specular = color3;
						this.selChar.ChangeSettingHairColor(i, true, this.hairSetInfo.toggleSame.isOn, this.hairSetInfo.toggleSame.isOn);
						this.selChar.ChangeSettingHairSpecular(i);
					}
				}
				this.colorInfo[1].colorMain = color;
				this.colorInfo[2].colorMain = color2;
				this.colorInfo[3].colorMain = color3;
			}
			else if (_idx == 0)
			{
				for (int j = 0; j < parts.Length; j++)
				{
					if (this.hairSetInfo.toggleSame.isOn || j == this.aiWGList.selectKind)
					{
						parts[j].baseColor = _c;
						this.selChar.ChangeSettingHairColor(j, true, this.hairSetInfo.toggleSame.isOn, this.hairSetInfo.toggleSame.isOn);
					}
				}
			}
			else if (_idx == 1)
			{
				for (int k = 0; k < parts.Length; k++)
				{
					if (this.hairSetInfo.toggleSame.isOn || k == this.aiWGList.selectKind)
					{
						parts[k].topColor = _c;
						this.selChar.ChangeSettingHairColor(k, false, true, false);
					}
				}
			}
			else if (_idx == 2)
			{
				for (int l = 0; l < parts.Length; l++)
				{
					if (this.hairSetInfo.toggleSame.isOn || l == this.aiWGList.selectKind)
					{
						parts[l].underColor = _c;
						this.selChar.ChangeSettingHairColor(l, false, false, true);
					}
				}
			}
			else if (_idx == 3)
			{
				for (int m = 0; m < parts.Length; m++)
				{
					if (this.hairSetInfo.toggleSame.isOn || m == this.aiWGList.selectKind)
					{
						parts[m].specular = _c;
						this.selChar.ChangeSettingHairSpecular(m);
					}
				}
			}
			this.colorInfo[_idx].colorMain = _c;
		}

		private void OnClickColorMainDef(int _idx)
		{
			if (this.wearType == 1)
			{
				ChaFileClothes.PartsInfo partsInfo = this.selChar.nowCoordinate.clothes.parts[this.aiWGList.selectKind];
				ChaFileClothes.PartsInfo partsInfo2 = this.selChar.chaFile.coordinate.clothes.parts[this.aiWGList.selectKind];
				ChaFileClothes.PartsInfo.ColorInfo clothesDefaultSetting = this.selChar.GetClothesDefaultSetting(this.aiWGList.selectKind, _idx);
				partsInfo.colorInfo[_idx].baseColor = clothesDefaultSetting.baseColor;
				partsInfo2.colorInfo[_idx].baseColor = clothesDefaultSetting.baseColor;
				this.selChar.ChangeCustomClothes(this.aiWGList.selectKind, true, false, false, false);
				this.colorInfo[_idx].colorMain = clothesDefaultSetting.baseColor;
			}
			else if (this.wearType == 2)
			{
				CustomHairColorPreset.HairColorInfo preset = this.items[_idx];
				this.ChangeHairDef(preset);
			}
			else if (this.wearType == 3)
			{
				Color defColor = default;
				float defGloss = 0f;
				float defMetallic = 0f;
				if (this.selChar.GetAccessoryDefaultColor(ref defColor, ref defGloss, ref defMetallic, this.accessorySlot, _idx))
				{
					ChaFileAccessory.PartsInfo accInfo = this.selChar.nowCoordinate.accessory.parts[this.accessorySlot];
					ChaFileAccessory.PartsInfo accOrgInfo = this.selChar.chaFile.coordinate.accessory.parts[this.accessorySlot];
					accInfo.colorInfo[_idx].color = defColor;
					accInfo.colorInfo[_idx].glossPower = defGloss;
					accInfo.colorInfo[_idx].metallicPower = defMetallic;
					accOrgInfo.colorInfo[_idx].color = defColor;
					accOrgInfo.colorInfo[_idx].glossPower = defGloss;
					accOrgInfo.colorInfo[_idx].metallicPower = defMetallic;
					this.selChar.ChangeAccessoryColor(this.accessorySlot);
					this.colorInfo[_idx].colorMain = defColor;
					this.colorInfo[_idx].inputMetallic.value = defMetallic;
					this.colorInfo[_idx].inputGlossiness.value = defGloss;
				}
			}
			Singleton<global::Studio.Studio>.Instance.colorPalette.visible = false;
		}

		private void ChangeHairDef(CustomHairColorPreset.HairColorInfo preset)
		{
			ChaFileHair.PartsInfo[] parts = this.selChar.chaFile.custom.hair.parts;
			for (int i = 0; i < parts.Length; i++)
			{
				if (this.hairSetInfo.toggleSame.isOn || i == this.aiWGList.selectKind)
				{
					parts[i].baseColor = preset.baseColor;
					parts[i].topColor = preset.topColor;
					parts[i].underColor = preset.underColor;
					parts[i].specular = preset.specular;
					parts[i].metallic = preset.metallic;
					parts[i].smoothness = preset.smoothness;
					this.selChar.ChangeSettingHairColor(i, true, true, true);
					this.selChar.ChangeSettingHairSpecular(i);
					this.selChar.ChangeSettingHairMetallic(i);
					this.selChar.ChangeSettingHairSmoothness(i);
				}
			}
			this.colorInfo[0].colorMain = preset.baseColor;
			this.colorInfo[1].colorMain = preset.topColor;
			this.colorInfo[2].colorMain = preset.underColor;
			this.colorInfo[3].colorMain = preset.specular;
			this.colorInfo[0].inputMetallic.value = preset.metallic;
			this.colorInfo[0].inputGlossiness.value = preset.smoothness;
		}

		private void OnValueChangeMetallic(int _idx, float _value)
		{
			if (this.isUpdateInfo)
			{
				return;
			}
			this.ChangeMetallic(_idx, _value);
		}

		private void OnEndEditMetallic(int _idx, string _text)
		{
			if (this.isUpdateInfo)
			{
				return;
			}
			float value = 0f;
			float.TryParse(_text, out value);
			float value2 = Mathf.Clamp(value, 0f, 1f);
			this.ChangeMetallic(_idx, value2);
		}

		private void OnClickMetallicDef(int _idx)
		{
			float value = 0f;
			if (this.wearType == 1)
			{
				ChaFileClothes.PartsInfo.ColorInfo clothesDefaultSetting = this.selChar.GetClothesDefaultSetting(this.aiWGList.selectKind, _idx);
				if (clothesDefaultSetting == null)
				{
					return;
				}
				value = clothesDefaultSetting.metallicPower;
			}
			else if (this.wearType == 2)
			{
				this.ChangeHairDef(this.items[4]);
				return;
			}
			else if (this.wearType == 3)
			{
				Color defColor = default;
				float defGloss = 0f;
				float defMetallic = 0f;
				if (this.selChar.GetAccessoryDefaultColor(ref defColor, ref defGloss, ref defMetallic, this.accessorySlot, _idx))
				{
					value = defMetallic;
				}
			}
			this.ChangeMetallic(_idx, value);
		}

		private void ChangeMetallic(int _idx, float _value)
		{
			if (this.wearType == 1)
			{
				ChaFileClothes.PartsInfo partsInfo = this.selChar.nowCoordinate.clothes.parts[this.aiWGList.selectKind];
				ChaFileClothes.PartsInfo partsInfo2 = this.selChar.chaFile.coordinate.clothes.parts[this.aiWGList.selectKind];
				partsInfo.colorInfo[_idx].metallicPower = _value;
				partsInfo2.colorInfo[_idx].metallicPower = _value;
				this.selChar.ChangeCustomClothes(this.aiWGList.selectKind, true, false, false, false);
			}
			else if (this.wearType == 2)
			{
				ChaFileHair.PartsInfo[] parts = this.selChar.chaFile.custom.hair.parts;
				for (int i = 0; i < parts.Length; i++)
				{
					if (this.hairSetInfo.toggleSame.isOn || i == this.aiWGList.selectKind)
					{
						parts[i].metallic = _value;
						this.selChar.ChangeSettingHairMetallic(i);
					}
				}
			}
			else if (this.wearType == 3)
			{
				ChaFileAccessory.PartsInfo accInfo = this.selChar.nowCoordinate.accessory.parts[this.accessorySlot];
				ChaFileAccessory.PartsInfo accOrgInfo = this.selChar.chaFile.coordinate.accessory.parts[this.accessorySlot];
				accInfo.colorInfo[_idx].metallicPower = _value;
				accOrgInfo.colorInfo[_idx].metallicPower = _value;
				this.selChar.ChangeAccessoryColor(this.accessorySlot);
			}
			this.colorInfo[_idx].inputMetallic.value = _value;
		}

		private void OnValueChangeGlossiness(int _idx, float _value)
		{
			if (this.isUpdateInfo)
			{
				return;
			}
			this.ChangeGlossiness(_idx, _value);
		}

		private void OnEndEditGlossiness(int _idx, string _text)
		{
			float value = 0f;
			float.TryParse(_text, out value);
			float value2 = Mathf.Clamp(value, 0f, 1f);
			this.ChangeGlossiness(_idx, value2);
		}

		private void OnClickGlossinessDef(int _idx)
		{
			if (this.isUpdateInfo)
			{
				return;
			}
			float value = 0f;
			if (this.wearType == 1)
			{
				ChaFileClothes.PartsInfo.ColorInfo clothesDefaultSetting = this.selChar.GetClothesDefaultSetting(this.aiWGList.selectKind, _idx);
				if (clothesDefaultSetting == null)
				{
					return;
				}
				value = clothesDefaultSetting.glossPower;
			}
			else if (this.wearType == 2)
			{
				this.ChangeHairDef(this.items[5]);
				return;
			}
			else if (this.wearType == 3)
			{
				Color defColor = default;
				float defGloss = 0f;
				float defMetallic = 0f;
				if (this.selChar.GetAccessoryDefaultColor(ref defColor, ref defGloss, ref defMetallic, this.accessorySlot, _idx))
				{
					value = defGloss;
				}
			}
			this.ChangeGlossiness(_idx, value);
		}

		private void ChangeGlossiness(int _idx, float _value)
		{
			if (this.wearType == 1)
			{
				ChaFileClothes.PartsInfo partsInfo = this.selChar.nowCoordinate.clothes.parts[this.aiWGList.selectKind];
				ChaFileClothes.PartsInfo partsInfo2 = this.selChar.chaFile.coordinate.clothes.parts[this.aiWGList.selectKind];
				partsInfo.colorInfo[_idx].glossPower = _value;
				partsInfo2.colorInfo[_idx].glossPower = _value;
				this.selChar.ChangeCustomClothes(this.aiWGList.selectKind, true, false, false, false);
			}
			else if (this.wearType == 2)
			{
				ChaFileHair.PartsInfo[] parts = this.selChar.chaFile.custom.hair.parts;
				for (int i = 0; i < parts.Length; i++)
				{
					if (this.hairSetInfo.toggleSame.isOn || i == this.aiWGList.selectKind)
					{
						parts[i].smoothness = _value;
						this.selChar.ChangeSettingHairSmoothness(i);
					}
				}
			}
			else if (this.wearType == 3)
			{
				ChaFileAccessory.PartsInfo accInfo = this.selChar.nowCoordinate.accessory.parts[this.accessorySlot];
				ChaFileAccessory.PartsInfo accOrgInfo = this.selChar.chaFile.coordinate.accessory.parts[this.accessorySlot];
				accInfo.colorInfo[_idx].glossPower = _value;
				accOrgInfo.colorInfo[_idx].glossPower = _value;
				this.selChar.ChangeAccessoryColor(this.accessorySlot);
			}
			this.colorInfo[_idx].inputGlossiness.value = _value;
		}

		private void OnClickPattern(int _idx)
		{
			if (this.wearType == 1)
			{
				if (this.cgPattern.alpha != 0f)
				{
					this.cgPattern.Enable(false, false, false);
					return;
				}
				Singleton<global::Studio.Studio>.Instance.patternSelectListCtrl.onChangeItemFunc = delegate(int _index)
				{
					ChaFileClothes.PartsInfo partsInfo = this.selChar.nowCoordinate.clothes.parts[this.aiWGList.selectKind];
					ChaFileClothes.PartsInfo partsInfo2 = this.selChar.chaFile.coordinate.clothes.parts[this.aiWGList.selectKind];
					partsInfo.colorInfo[_idx].pattern = _index;
					partsInfo2.colorInfo[_idx].pattern = _index;
					this.selChar.ChangeCustomClothes(this.aiWGList.selectKind, false, _idx == 0, 1 == _idx, 2 == _idx);
					this.colorInfo[_idx].textPattern = _index.ToString();
					this.colorInfo[_idx].EnablePattenDetail = (_index > 0);
					this.cgPattern.Enable(false, false, false);
				};
				this.cgPattern.Enable(true, false, false);
			}
		}

		private void OnClickColorPattern(int _idx)
		{
			if (this.wearType == 1)
			{
				string text = string.Concat(new object[]
				{
					"Wear Pattern ",
					this.aiWGList.selectKind,
					" ",
					_idx
				});
				if (Singleton<global::Studio.Studio>.Instance.colorPalette.Check(text))
				{
					Singleton<global::Studio.Studio>.Instance.colorPalette.visible = false;
					return;
				}
				ChaFileClothes.PartsInfo partsInfo = this.selChar.nowCoordinate.clothes.parts[this.aiWGList.selectKind];
				ChaFileClothes.PartsInfo orgInfo = this.selChar.chaFile.coordinate.clothes.parts[this.aiWGList.selectKind];
				Singleton<global::Studio.Studio>.Instance.colorPalette.Setup(text, partsInfo.colorInfo[_idx].patternColor, delegate(Color _c)
				{
					partsInfo.colorInfo[_idx].patternColor = _c;
					orgInfo.colorInfo[_idx].patternColor = _c;
					this.selChar.ChangeCustomClothes(this.aiWGList.selectKind, true, false, false, false);
					this.colorInfo[_idx].colorPattern = _c;
				}, true);
			}
		}

		private void OnClickColorPatternDef(int _idx)
		{
			if (this.wearType == 1)
			{
				ChaFileClothes.PartsInfo partsInfo = this.selChar.nowCoordinate.clothes.parts[this.aiWGList.selectKind];
				ChaFileClothes.PartsInfo partsInfo2 = this.selChar.chaFile.coordinate.clothes.parts[this.aiWGList.selectKind];
				ChaFileClothes.PartsInfo.ColorInfo clothesDefaultSetting = this.selChar.GetClothesDefaultSetting(this.aiWGList.selectKind, _idx);
				partsInfo.colorInfo[_idx].patternColor = clothesDefaultSetting.patternColor;
				partsInfo2.colorInfo[_idx].patternColor = clothesDefaultSetting.patternColor;
				this.selChar.ChangeCustomClothes(this.aiWGList.selectKind, true, false, false, false);
				this.colorInfo[_idx].colorPattern = clothesDefaultSetting.patternColor;
			}
			Singleton<global::Studio.Studio>.Instance.colorPalette.visible = false;
		}

		private void OnValueChangeUT(int _idx, int li, float _value)
		{
			if (this.isUpdateInfo)
			{
				return;
			}
			this.ChangeUT(_idx, li, _value);
		}

		private void OnEndEditUT(int _idx, int li, string _text)
		{
			if (this.isUpdateInfo)
			{
				return;
			}
			if (this.wearType == 1 || this.wearType == 3)
			{
				float value = 0f;
				float.TryParse(_text, out value);
				this.ChangeUT(_idx, li, value);
			}
		}

		private void ChangeUT(int _idx, int li, float _value)
		{
			if (this.wearType == 1)
			{
				ChaFileClothes.PartsInfo partsInfo = this.selChar.nowCoordinate.clothes.parts[this.aiWGList.selectKind];
				ChaFileClothes.PartsInfo partsInfo2 = this.selChar.chaFile.coordinate.clothes.parts[this.aiWGList.selectKind];
				Vector4 layout = partsInfo.colorInfo[_idx].layout;
				float[] array = new float[]
				{
					layout.x,
					layout.y,
					layout.z,
					layout.w
				};
				array[li] = _value;
				Vector4 layout2 = new Vector4(array[0], array[1], array[2], array[3]);
				partsInfo.colorInfo[_idx].layout = layout2;
				partsInfo2.colorInfo[_idx].layout = layout2;
				this.selChar.ChangeCustomClothes(this.aiWGList.selectKind, true, false, false, false);
			}
			else if (this.wearType == 3 && _idx < 3 && li < 3)
			{
				int slot = this.accessorySlot;
				ChaFileAccessory.PartsInfo accInfo = this.selChar.nowCoordinate.accessory.parts[slot];
				ChaFileAccessory.PartsInfo accOrgInfo = this.selChar.chaFile.coordinate.accessory.parts[slot];
				if (accInfo.addMove != null)
				{
					Vector3 v = accInfo.addMove[0, _idx];
					if (li == 0) v.x = _value;
					else if (li == 1) v.y = _value;
					else if (li == 2) v.z = _value;
					accInfo.addMove[0, _idx] = v;
					accOrgInfo.addMove[0, _idx] = v;
					this.selChar.UpdateAccessoryMoveFromInfo(slot);
				}
			}
			this.colorInfo[_idx][li].value = _value;
		}

		private void OnValueChangeRot(int _idx, float _value)
		{
			if (this.isUpdateInfo)
			{
				return;
			}
			if (this.wearType == 1)
			{
				ChaFileClothes.PartsInfo partsInfo = this.selChar.nowCoordinate.clothes.parts[this.aiWGList.selectKind];
				ChaFileClothes.PartsInfo partsInfo2 = this.selChar.chaFile.coordinate.clothes.parts[this.aiWGList.selectKind];
				partsInfo.colorInfo[_idx].rotation = _value;
				partsInfo2.colorInfo[_idx].rotation = _value;
				this.selChar.ChangeCustomClothes(this.aiWGList.selectKind, true, false, false, false);
			}
			this.colorInfo[_idx][4].value = _value;
		}

		private void OnValueChangeBreak(float _value)
		{
			if (this.isUpdateInfo)
			{
				return;
			}
			if (this.wearType == 1)
			{
				ChaFileClothes.PartsInfo partsInfo = this.selChar.nowCoordinate.clothes.parts[this.aiWGList.selectKind];
				ChaFileClothes.PartsInfo partsInfo2 = this.selChar.chaFile.coordinate.clothes.parts[this.aiWGList.selectKind];
				partsInfo.breakRate = _value;
				partsInfo2.breakRate = _value;
				this.selChar.ChangeBreakClothes(this.aiWGList.selectKind);
			}
			this.inputLightCancel.value = _value;
		}

		private void OnEndEditSlotOrBreak(string _text)
		{
			if (this.isUpdateInfo)
			{
				return;
			}
		}

		private void OnClickColorHairAcs(int _idx)
		{
			if (this.wearType == 2)
			{
				string text = string.Concat(new object[]
				{
					"Hair Acs ",
					this.aiWGList.selectKind,
					" ",
					_idx
				});
				if (Singleton<global::Studio.Studio>.Instance.colorPalette.Check(text))
				{
					Singleton<global::Studio.Studio>.Instance.colorPalette.visible = false;
					return;
				}
				ChaFileHair.PartsInfo partsInfo = this.selChar.chaFile.custom.hair.parts[this.aiWGList.selectKind];
				Singleton<global::Studio.Studio>.Instance.colorPalette.Setup(text, partsInfo.acsColorInfo[_idx].color, delegate(Color _c)
				{
					partsInfo.acsColorInfo[_idx].color = _c;
					this.selChar.ChangeSettingHairAcsColor(this.aiWGList.selectKind);
					this.hairAcsColors[_idx].color = _c;
				}, true);
			}
		}

		public OCIChar ociChar
		{
			get
			{
				return this.mpCharCtrl.ociChar;
			}
		}

		public ChaListDefine.CategoryNo selectGroup
		{
			get
			{
				return this.aiWGList.select;
			}
		}

		public MPCharCtrl mpCharCtrl;

		public AIWearGroupList aiWGList;

		[SerializeField]
		public AIWearControllerUI.ColorInfo[] colorInfo;

		[SerializeField]
		public AIWearControllerUI.ColorCombination colorShadow;

		[SerializeField]
		public AIWearControllerUI.ColorInputCombination emissionInfo;

		[SerializeField]
		public AIWearControllerUI.InputCombination inputAlpha;

		[SerializeField]
		public AIWearControllerUI.InputCombination inputLightCancel;

		[SerializeField]
		public AIWearControllerUI.ColorInputCombination lineInfo;

		[SerializeField]
		[Header("HairSet")]
		private AIWearControllerUI.HairSetInfo hairSetInfo = new AIWearControllerUI.HairSetInfo();

		[SerializeField]
		public AIWearControllerUI.OptionInfo optionInfo = new AIWearControllerUI.OptionInfo();

		[SerializeField]
		[Header("PatternModeSet")]
		public CanvasGroup cgPattern;

		private AIWearControllerUI.ColorCombination[] hairAcsColors;

		private bool isUpdateInfo;

		private int wearType = -1;

		private ChaControl selChar;

		private bool selTextureInited;

		public int accessorySlot;

		private TextMeshProUGUI slotLabel;

		private float[] origSliderMin;
		private float[] origSliderMax;

		private CustomHairColorPreset.HairColorInfo[] items;

		[Serializable]
		public class ColorInfo
		{
			public void Init(Transform root)
			{
				this.objRoot = root.gameObject;
				Transform root2 = root.Find("Color");
				this._colorMain.Init(root2);
				Transform transform = root.Find("Canvas Metallic");
				this.objMetallic = transform.gameObject;
				Transform root3 = transform.Find("Metallic");
				this.inputMetallic.Init(root3);
				Transform root4 = transform.Find("Glossiness");
				this.inputGlossiness.Init(root4);
				Transform transform2 = root.Find("Canvas Pattern");
				transform2.GetOrAddComponent<CanvasRenderer>();
				VerticalLayoutGroup orAddComponent = transform2.GetOrAddComponent<VerticalLayoutGroup>();
				orAddComponent.childForceExpandHeight = false;
				orAddComponent.childControlHeight = false;
				orAddComponent.spacing = 5f;
				orAddComponent.padding = new RectOffset(0, 0, 5, 5);
				this.objPattern = transform2.gameObject;
				this._buttonPattern = transform2.Find("Pattern/Button").GetComponent<Button>();
				this._textPattern = transform2.Find("Pattern/Button/TextMeshPro Text").GetComponent<TextMeshProUGUI>();
				Transform transform3 = transform2.Find("Pattern Color");
				this._colorPattern.objRoot = transform3.gameObject;
				this._colorPattern.imageColor = transform3.Find("Image/Button Color Pattern").GetComponent<Image>();
				this._colorPattern.buttonColor = transform3.Find("Image/Button Color Pattern").GetComponent<Button>();
				this._colorPattern.buttonColorDefault = transform3.Find("Button Default").GetComponent<Button>();
				this._toggleObj = transform2.Find("Clamp").gameObject;
				this._toggleClamp = transform2.Find("Clamp/Toggle").GetComponent<Toggle>();
				this._input = new AIWearControllerUI.InputCombination[5];
				Transform root5 = transform2.Find("UT");
				this._input[0] = new AIWearControllerUI.InputCombination();
				this._input[0].Init(root5);
				Transform root6 = transform2.Find("VT");
				this._input[1] = new AIWearControllerUI.InputCombination();
				this._input[1].Init(root6);
				Transform root7 = transform2.Find("US");
				this._input[2] = new AIWearControllerUI.InputCombination();
				this._input[2].Init(root7);
				Transform root8 = transform2.Find("VS");
				this._input[3] = new AIWearControllerUI.InputCombination();
				this._input[3].Init(root8);
				Transform root9 = transform2.Find("Rot");
				this._input[4] = new AIWearControllerUI.InputCombination();
				this._input[4].Init(root9);
			}

			public Color colorMain
			{
				set
				{
					this._colorMain.imageColor.color = value;
				}
			}

			public string textPattern
			{
				set
				{
					this._textPattern.text = value;
				}
			}

			public Color colorPattern
			{
				set
				{
					this._colorPattern.imageColor.color = value;
				}
			}

			public bool isOn
			{
				set
				{
					this._toggleClamp.isOn = value;
				}
			}

			public AIWearControllerUI.InputCombination this[int _idx]
			{
				get
				{
					return this._input.SafeGet(_idx);
				}
			}

			public bool enable
			{
				get
				{
					return this.objRoot.activeSelf;
				}
				set
				{
					if (this.objRoot.activeSelf != value)
					{
						this.objRoot.SetActive(value);
					}
				}
			}

			public bool EnableMetallic
			{
				set
				{
					this.objMetallic.SetActiveIfDifferent(value);
				}
			}

			public bool EnablePattern
			{
				set
				{
					this.objPattern.SetActiveIfDifferent(value);
				}
			}

			public bool EnablePattenDetail
			{
				set
				{
					this._colorPattern.active = value;
					this._toggleObj.SetActiveIfDifferent(value);
					for (int i = 0; i < 5; i++)
					{
						this._input[i].active = value;
					}
				}
			}

			public GameObject objRoot;

			public AIWearControllerUI.ColorCombination _colorMain = new AIWearControllerUI.ColorCombination();

			[Header("MetallicSet")]
			public GameObject objMetallic;

			public AIWearControllerUI.InputCombination inputMetallic = new AIWearControllerUI.InputCombination();

			public AIWearControllerUI.InputCombination inputGlossiness = new AIWearControllerUI.InputCombination();

			[Header("PatternSet")]
			public GameObject objPattern;

			public Button _buttonPattern;

			public TextMeshProUGUI _textPattern;

			public AIWearControllerUI.ColorCombination _colorPattern = new AIWearControllerUI.ColorCombination();

			public GameObject _toggleObj;

			public Toggle _toggleClamp;

			public AIWearControllerUI.InputCombination[] _input;
		}

		[Serializable]
		public class ColorCombination
		{
			public void Init(Transform root)
			{
				this.objRoot = root.gameObject;
				this.imageColor = root.Find("Image/Button Color").GetComponent<Image>();
				this.buttonColor = root.Find("Image/Button Color").GetComponent<Button>();
				Transform transform = root.Find("Button Default");
				if (transform != null)
				{
					this.buttonColorDefault = transform.GetComponent<Button>();
				}
			}

			public bool interactable
			{
				set
				{
					this.buttonColor.interactable = value;
					if (this.buttonColorDefault)
					{
						this.buttonColorDefault.interactable = value;
					}
				}
			}

			public Color color
			{
				get
				{
					return this.imageColor.color;
				}
				set
				{
					this.imageColor.color = value;
				}
			}

			public bool active
			{
				set
				{
					this.objRoot.SetActiveIfDifferent(value);
				}
			}

			public GameObject objRoot;

			public Image imageColor;

			public Button buttonColor;

			public Button buttonColorDefault;
		}

		[Serializable]
		public class InputCombination
		{
			public void Init(Transform root)
			{
				this.objRoot = root.gameObject;
				this.input = root.Find("TextMeshPro - InputField").GetComponent<TMP_InputField>();
				this.slider = root.Find("Slider").GetComponent<Slider>();
				Transform transform = root.Find("Button Default");
				if (transform != null)
				{
					this.buttonDefault = transform.GetComponent<Button>();
				}
			}

			public bool interactable
			{
				set
				{
					this.input.interactable = value;
					this.slider.interactable = value;
					if (this.buttonDefault)
					{
						this.buttonDefault.interactable = value;
					}
				}
			}

			public string text
			{
				get
				{
					return this.input.text;
				}
				set
				{
					this.input.text = value;
					float value2 = 0f;
					float.TryParse(value, out value2);
					this.slider.value = value2;
				}
			}

			public float value
			{
				get
				{
					return this.slider.value;
				}
				set
				{
					this.slider.value = value;
					this.input.text = value.ToString("0.0");
				}
			}

			public bool active
			{
				set
				{
					this.objRoot.SetActiveIfDifferent(value);
				}
			}

			public GameObject objRoot;

			public TMP_InputField input;

			public Slider slider;

			public Button buttonDefault;
		}

		[Serializable]
		public class ColorInputCombination
		{
			public bool active
			{
				set
				{
					this.objRoot.SetActiveIfDifferent(value);
				}
			}

			public GameObject objRoot;

			public AIWearControllerUI.ColorCombination color = new AIWearControllerUI.ColorCombination();

			public AIWearControllerUI.InputCombination input = new AIWearControllerUI.InputCombination();
		}

		[Serializable]
		public class OptionInfo
		{
			public bool Active
			{
				get
				{
					return this.objRoot.activeSelf;
				}
				set
				{
					this.objRoot.SetActiveIfDifferent(value);
				}
			}

			public GameObject objRoot;

			public Toggle toggleAll;
		}

		[Serializable]
		public class HairSetInfo
		{
			public bool Active
			{
				get
				{
					return this.objRoot.activeSelf;
				}
				set
				{
					this.objRoot.SetActiveIfDifferent(value);
				}
			}

			public GameObject objRoot;

			public Toggle toggleSame;

			public Toggle toggleDetail;
		}
	}
}
