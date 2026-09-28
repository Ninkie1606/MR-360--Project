# Projectdocumentatie: VR 360° Video Omgeving

Welkom bij de documentatie van het **MR 360 Video Project**. Dit project is gebouwd in **Unity 6 (6000.4)** met de **Universal Render Pipeline (URP)** en **OpenXR**, specifiek gericht op de **Meta Quest** (standalone en PC VR) met ingebouwde preview-ondersteuning voor desktop.

---

## 1. Doel & Concept
Het doel van deze mixed reality omgeving is het tonen van meeslepende 360° videocontent. Zoals beschreven in het concept (`ideas.md`), richt het project zich op het visualiseren van scenario's (bijv. IT-educatie op school vs. op de werkvloer) waarbij de volledige 360° ruimte wordt benut (inclusief eventuele 180° gesplitste contrast-scenario's).

---

## 2. Architectuur & Scripts

De C# scripts bevinden zich in `Assets/Scripts/VR360/`:

### A. `Video360Player.cs`
Het kerncomponent dat de videoweergave aanstuurt:
- **Automatische RenderTexture**: Maakt bij het opstarten een dynamische render texture aan (standaard 4096×2048 voor maximale scherpte).
- **Panoramic Skybox**: Koppelt de render texture direct aan Unity's `Skybox/Panoramic` shader en stelt `RenderSettings.skybox` in.
- **Audio Routing**: Ondersteunt koppeling met een `AudioSource` voor stereo- of ruimtelijke audio.
- **Auto Camera Setup**: Detecteert automatisch of de `Main Camera` voorzien is van het rondkijk-script (`VRCameraLook`) en voegt dit zo nodig toe.
- **Besturing & Sneltoetsen**: Ondersteunt `Play()`, `Pause()`, `TogglePlayPause()`, `Restart()`, en `SetClip()`.

### B. `VRCameraLook.cs`
Maakt het mogelijk om in de Unity Editor (zonder VR-headset op) 360° rond te kijken:
- **New Input System**: Volledig compatibel met Unity's New Input System (`UnityEngine.InputSystem`).
- **Muisbesturing**: Houd de **linker-** of **rechtermuisknop** ingedrukt en beweeg de muis om rond te kijken (Pitch & Yaw).
- **Toetsenbordbesturing**: Gebruik **WASD** of de **Pijltjestoetsen** om vloeiend rond te draaien.
- **VR Seamless Switch**: Schakelt automatisch de rotatieovername uit zodra de VR-headset tracking actief is.

### C. `InvertedSphere.cs` *(Optioneel)*
- Genereert een bol met naar binnen gerichte normalen en omgekeerde driehoeken.
- Handig wanneer je de video niet via de oneindige Skybox wilt projecteren, maar op een fysiek 3D-object in de wereld (bijv. voor belichtingseffecten, split-180 projecties of 3D UI-elementen rondom de gebruiker).

### D. `Editor/SetupVR360Scene.cs`
- Voegt een menu-item toe aan de Unity Editor: **Tools > VR 360 > Setup 360 Video Player in Scene**.
- Maakt in één klik het `VR_360_Player` GameObject aan, configureert de camera en koppelt de audio.

---

## 3. Besturing & Sneltoetsen (Play Mode in Editor)

| Actie | Toets / Input |
|---|---|
| **Rondkijken (Muis)** | Linker- of rechtermuisknop ingedrukt houden + muis bewegen |
| **Rondkijken (Toetsenbord)** | `W` / `A` / `S` / `D` of `Pijltjestoetsen` |
| **Pauzeren / Afspelen** | `Spatiebalk` |
| **Video herstarten** | `R` |

---

## 4. VR & OpenXR Configuratie

Het project maakt gebruik van:
- `com.unity.xr.management` (4.5.1)
- `com.unity.xr.openxr` (1.14.3)
- `com.unity.xr.interaction.toolkit` (3.0.7)
- `XR Device Simulator` (onder `Assets/Samples/` voor gesimuleerde VR-controllers in editor)

### Meta Quest testen via Quest Link / AirLink (PC/Mac VR)
1. Verbind je Meta Quest via Link-kabel of AirLink met je computer.
2. Zorg dat in Unity onder **Project Settings > XR Plug-in Management > Standalone tab** het vakje **OpenXR** is aangevinkt.
3. Druk op **Play** in Unity: het beeld en de tracking worden direct naar je headset gestreamd.

### Meta Quest Standalone Build (Android APK)
1. Ga in Unity naar **File > Build Profiles** (of **Build Settings**).
2. Schakel over naar het platform **Android** (**Switch Platform**).
3. Ga naar **Project Settings > XR Plug-in Management > Android tab** en vink **OpenXR** aan.
4. Klik op het uitroepteken naast OpenXR om eventuele configuraties te fixen (Meta Quest Support feature inschakelen).
5. Klik op **Build and Run** met je Quest via USB aangesloten (in Developer Mode).

---

## 5. Aanbevolen Videospecificaties

Plaats video's in de map `Assets/Videos/`.

- **Container**: `.mp4`
- **Videocodec**: H.264 of H.265 (HEVC)
- **Projectie**: Equirectangular 360° (2:1 verhouding, bijv. 3840×1920 of 4096×2048)
- **Framerate**: 30 fps of 60 fps
- **Audio**: Stereo AAC (48 kHz)

---

## 6. Stappenplan: Nieuwe video toevoegen

1. Sleep je `.mp4` bestand naar de map `Assets/Videos/`.
2. Klik op het videobestand in Unity en controleer in de Inspector of de import-instellingen goed staan.
3. Selecteer het `VR_360_Player` object in de Hierarchy.
4. Sleep je videoclip in het veld **Video Clip** van het component `Video 360 Player`.
5. Druk op **Play**!
