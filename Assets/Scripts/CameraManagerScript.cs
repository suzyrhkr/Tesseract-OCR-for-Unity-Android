using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using System.IO;
using System.Threading;

public class CameraManagerScript : MonoBehaviour
{
    WebCamTexture camTexture;
    public RawImage cameraViewImage;

    public void CameraOn(){
        if(!Permission.HasUserAuthorizedPermission(Permission.Camera)){
            Permission.RequestUserPermission(Permission.Camera);
        }

        if(WebCamTexture.devices.Length == 0){
            Debug.Log("no camera!");
            return;
        }

        WebCamDevice[] devices = WebCamTexture.devices;
        int selectedCameraIndex = -1;

        for(int i=0; i<devices.Length; i++){
            if(devices[i].isFrontFacing == false){
                selectedCameraIndex = i;
                break;
            }
        }

        if(selectedCameraIndex >= 0){
            camTexture = new WebCamTexture(devices[selectedCameraIndex].name);
            camTexture.requestedFPS = 30;

            cameraViewImage.texture = camTexture;
            camTexture.Play();
        }
    }

    private IEnumerator SaveImage()
    {
    //Create a Texture2D with the size of the rendered image on the screen.
    Texture2D texture = new Texture2D(cameraViewImage.texture.width, cameraViewImage.texture.height, TextureFormat.ARGB32, false);
    //Save the image to the Texture2D
    texture.SetPixels(camTexture.GetPixels());
    // texture = RotateTexture(texture, -90);
    texture.Apply();
    yield return new WaitForEndOfFrame();
    // Save the screenshot to Gallery/Photos
    NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(texture, "CameraTest", "CaptureImage.png", (success, path) => Debug.Log("Media save result: " + success + " " + path));
    // To avoid memory leaks
    Destroy(texture);
    }

    public void clickCapture()
    {
    StartCoroutine(SaveImage());
    }

    public void CameraOff(){

        if(camTexture!= null){
            camTexture.Stop();
            WebCamTexture.Destroy(camTexture);
            camTexture = null;
        }
    }

}
