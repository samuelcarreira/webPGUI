# webPGUI
##### WebP encoding tool GUI

![screenshot](https://github.com/samcpt/webPGUI/blob/master/screenshots/webp_0000_Layer-1.png)

WebP encoding tool GUI is a simple Windows application created with the objective of assisting in the conversion of images to the [WebP format] (https://developers.google.com/speed/webp/) (this format created by Google, allows a much higher compression then JPEG format).
Because the tool provided by Google is hard to use (command-line only with numerous tuning options via [parameters] (https://developers.google.com/speed/webp/docs/cwebp) ), I decided to create this interface to personal use.

**NOTE:** This application is at an early stage of development and still contains **many errors/many features are not yet available**. If there is a large public interest, I can improve the application. This is my first C# application so use the application at your risk and **be friendly with your criticism** :+1:

## Installation
##### Download the installer from: https://dl.orangedox.com/5YMZB2Zy7il6MyEXmu (0.8 MB)
(Google Chrome flags this installer as malicious because it isn't signed, but you can safelly ignore the warning)

...or Compile the source code with Visual Studio 2015 and run the utility.

##### System Requirements
* 1 GHz CPU
* 512 MB RAM 
* Windows® XP SP3 or above (32 or 64 bits) (tested on Windows® 10) 
* .NET Framework 4
* 2 MB de free space (1.5 GB for the .NET Framework) 


## Usage
1. Install the application (if you don't have the .NET 4.0 framework installed, an internet connection is required to download the installer)
2. Choose a valid input image
3. Check the options (position the mouse pointer hover the options to check the tooltips included and learn more about the parameters)
3. Click ENCODE and see the output results

##### How to view webP images?
Google Chrome can open WebP files natively. Or install Windows CODEC https://developers.google.com/speed/webp/docs/webp_codec if you want to view WebP files in Windows Photo Viewer and their thumbnails in Windows Explorer

## Why Should I Use this App?
You can use one of many encoder tools and plugins available, but none of them have the same options of the official encoder tool provided by Google. Also, some plugins (like the Photoshop Plugin) are outdated and don’t produce the better output. If you want to encode your images with total control, you can use this app to easily access to the documentation and change all the little options to produce the better output.

## Screenshots
![screenshot](https://github.com/samcpt/webPGUI/blob/master/screenshots/webp_0001_Layer-4.png)
![screenshot](https://github.com/samcpt/webPGUI/blob/master/screenshots/webp_0002_Layer-6.png)
![screenshot](https://github.com/samcpt/webPGUI/blob/master/screenshots/webp_0003_Layer-2.png)
![screenshot](https://github.com/samcpt/webPGUI/blob/master/screenshots/webp_0003_Layer-5.png)
![screenshot](https://github.com/samcpt/webPGUI/blob/master/screenshots/webp_0004_Layer-3.png)
![screenshot](https://github.com/samcpt/webPGUI/blob/master/screenshots/webp_0006_Layer-2.png)


## Contributing
There is a lot to work to be done in the application, so if you are interested contact me. I will only continue the development of this application if there was a notorious interest on that. 

## Know bugs/Not implemented yet
- Maybe some bugs on save/read settings
- Check valid options
- Integrate webP library
- Resize/Crop
- A lot of ‘dumb/simple code’ (this is a very simple application) so don’t use this as a reference

## History
2016-04-12 - Bug fix (crash on background encoding)

2016-03-19 - Batch mode

2016-02-08 - Drag and Drop Support

2016-02-02 - First public version

## Credits
- Developed by: Samuel Carreira
- Includes ConsoleAppLauncher Library released under The MIT License (MIT). Copyright (c) 2013 Slava Guzenko 
- Includes cwebp.exe encoding tool v.0.5.0 - Copyright (c) 2010, Google Inc

## License
The MIT License (MIT)

Copyright (c) 2016 Samuel Carreira

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
