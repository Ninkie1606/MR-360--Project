# MR-360 Project: Immersive 360° Stop Motion Storytelling

Dit is een schoolproject voor het vak **Mixed Reality**, met een sterke focus op *immersive storytelling*. Het project maakt gebruik van een unieke **360° stop motion video** om de gebruiker volledig onder te dompelen in het verhaal. Omdat de video in 4K resolutie is (en daardoor te groot voor GitHub), hebben we het project zo opgezet dat de code hier te vinden is, maar de videofile apart gedownload moet worden.

## 🚀 Hoe werkt het?

Omdat GitHub geen videobestanden groter dan 100MB toestaat, moet je de video handmatig downloaden en aan het Unity project toevoegen. Volg deze simpele stappen om de applicatie lokaal te draaien:

### Stap 1: Download het project
Clone of download deze repository naar je eigen computer en open de map `My project` in **Unity**.

### Stap 2: Download de 360° Video
Download het hoofdbestand (`Untitled1.mp4`) via de onderstaande MEGA link:
👉 **[Download de 360° Video hier](https://mega.nz/file/PDoDzZqS#mT_DWT4mpedd6otJg42rqGCxZ9J3bpUC2yBdEP111wk)**

### Stap 3: Plaats de video in Unity
1. Pak de gedownloade video.
2. Sleep hem in Unity in de volgende map: `Assets/Videos/`.
3. Ga in je open scene naar de `VR_360_Player` in de Hierarchy.
4. Sleep de video in het **Video Clip** veld van het `Video360Player` script.
5. Zorg ervoor dat het veld **Audio Source** in datzelfde script helemaal leeg is (`None`), zodat Unity de geïntegreerde audio perfect synchroon afspeelt.

### Stap 4: Speel af of Build
Druk op **Play** in de editor, of ga naar *File > Build Settings* om een naadloze applicatie te bouwen voor je PC of VR-bril! 

---
*Gemaakt voor het vak Mixed Reality.*