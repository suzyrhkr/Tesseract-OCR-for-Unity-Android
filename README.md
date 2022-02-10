# OCR for Unity Android using Tesseract 4.1.0

### Introduction
This is a sample project that shows OCR result from screenshot using Tesseract engine on Unity Android platform.

### Development Environment
- Unity 2020.3.23f1
- Tesseract 4.1.0
- macOS Big Sur/Monterey

### Requirements
- Unity
  
### Usage
1. Click "On" button to turn on device camera.
2. Click "Capture" button to capture image from camera. Tesseract recognizes text from image and shows output(OCR result) on the screen.
3. Click "Off" button to turn off device camera.

### Main Scene
<img src="https://user-images.githubusercontent.com/48341349/151914273-c7011733-7e4c-4944-aab8-2ba967917e63.jpg" width="500" height="250">

### Description
1. Recognize from a Texture 
   ```
     Recoginze(texture);
   ```

2. Save the screenshot to Gallery
   ```
    NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(texture, "CameraTest", "CaptureImage.png", (success, path) => Debug.Log("Media save result: " + success + " " + path));
   ```
If you want to save screenshot to gallery, you have to build project. (Unity-Remote-5 doesn't work.)      
You only need to refer to ./Assets/Scripts/TesseractDemoScript.cs.


### Note
This project is a modified version of [tesseract-unity](https://github.com/Neelarghya/tesseract-unity).
I hope this sample project will help you with your project.


### License
[Apache-2.0 License](./LICENSE)
