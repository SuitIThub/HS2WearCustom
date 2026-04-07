# HS2 Wear Custom (Studio Wear Custom)

A [BepInEx](https://bepinex.dev/) plugin for **Honey Select 2 Studio** (`StudioNEOV2`) that adds an in-scene **Wear Custom** panel so you can change **clothes**, **hair**, and **accessories** on selected characters without leaving Studio.

**Plugin ID:** `Suit-Ji.HS2WearCustom`  
**Display name:** Studio Wear Custom

---

## Features

- **Clothing & hair** — Pick categories (tops, bottoms, hair parts, etc.) and items from the same list data the character maker uses.
- **Accessories** — Dedicated flow:
  - Choose a **slot** (1–20); slots show the **item name** when something is equipped, or **Slot N – Empty**.
  - Pick a **category** (Head, Ear, Glasses, …); **None** clears the current slot.
  - When a slot already has an item, the correct **category** and **list item** are highlighted.
- **Apply path** — Writes `ChaFileAccessory` / coordinate data using the game’s list **category numbers** (not UI indices), then runs the same **`Reload` + `AssignCoordinate` + `ChangeAccessoryAsync`** style pipeline the engine expects in Studio, including **`ChangeAccessoryColor`** so materials/textures refresh.

---

## Requirements

- **Honey Select 2** with **Studio** (`StudioNEOV2.exe`).
- **[BepInEx 5.x](https://github.com/BepInEx/BepInEx)** for IL2CPP/Mono as appropriate for your HS2 setup.
- **.NET SDK** (for example .NET 8 SDK) to build the project; game assemblies come from **IllusionLibs** NuGet packages (see **Building**).

This plugin patches `MPCharCtrl.CostumeInfo.Init`, `CharaList.InitCharaList`, and `MPCharCtrl.OnClickRoot`. If another mod alters the same methods, load order may matter.

---

## Installation

1. Build or download **`HS2WearCustom.dll`** (Release output: `HS2WearCustom\bin\Release\`).
2. Copy **`HS2WearCustom.dll`** into your HS2 **`BepInEx\plugins`** folder.
3. Start **Studio** and open the **Manipulate → Character** area; use the **Wear Custom** / **Accessories** entries added to the character menu (see in-game UI).

---

## Building from source

1. Clone this repository.
2. Ensure **`nuget.config`** at the repo root is present (it registers the **BepInEx** and **IllusionMods** feeds used by **IllusionLibs** packages).
3. From the repo root, run **`dotnet build HS2WearCustom\HS2WearCustom.csproj -c Release`** (or open **`HS2WearCustom.sln`** in Visual Studio and build **Release**).
4. The project targets **.NET Framework 4.6.2** (`net462`) and pulls **`Assembly-CSharp`**, Unity, **BepInEx**, and **Harmony** from NuGet—no local game `HintPath` references are required.
5. Copy **`HS2WearCustom\bin\Release\HS2WearCustom.dll`** to **`BepInEx\plugins`** (only the plugin DLL is needed at runtime; game assemblies are not copied next to the output by the IllusionLibs package targets).

---

## Configuration & logs

- Logging uses the BepInEx logger; look for **`Studio Wear Custom`** in `BepInEx\LogOutput.log` (or your configured log path).

---

## Credits

- Original plugin by **GarryWu**; maintained and extended by **Suit-Ji** and contributors.  
- See [LICENSE](LICENSE) (MIT).

---

## License

This project is licensed under the **MIT License** — see [LICENSE](LICENSE).
