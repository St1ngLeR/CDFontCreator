# CDFontCreator
![image](https://github.com/St1ngLeR/CDFontCreator/assets/63962772/1f483452-c58a-44f4-8f12-bd995e97070a)
## Introduction
**CDFontCreator** is an application to create fonts for Crashday. The application is remade from internal (dev-only) tool "FontCreator" or a.k.a. "PROJECT1". Supposedly this tool on Delphi wasn't intended to be public and was forgotten by the devs in `textures/fonts` folder. This remade version is done in C# with WinForms and has some advantages compared to it's original version from 2004-2005 by devs:
- Saving three necessary font files prepared for ingame use (*.tga - font texture, *.tex - shader, *.wid - characters width)
- Ability to render font texture from 512x512 to 4096x4096
- "Redline Edition" support

## Usage
1. Select the desired font (and it's styles if you want - bold & italic).
2. Select the desired game type ("Classic" or "Redline").
3. Select the desired texture resolution (2048x2048 is recommended).
4. Click on "Create font" button to generate the font texture.
5. Click on "Save image" button to save files for your font.
6. Open "fontinfo.dbs" (`textures/fonts` for Classic & `fonts` for Redline) with any text editor and change the parameters to make your font suitable in game.

## TODO
- Remake the character width calculation (It's not so good for some fonts, need to think about it...)

## 3rd-party plugins
The application is using [Costura.Fody](https://github.com/Fody/Costura/) to compile all resources into a single executable file.
