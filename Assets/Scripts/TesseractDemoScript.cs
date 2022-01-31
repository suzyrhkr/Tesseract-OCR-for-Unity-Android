using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using System.Drawing;
using System.IO;
using System.Threading;

public class TesseractDemoScript : MonoBehaviour
{
    [SerializeField] private Texture2D imageToRecognize;
    [SerializeField] private Text displayText;
    private TesseractDriver _tesseractDriver;
    private string _text = "";
    private Texture2D _texture;
    WebCamTexture camTexture;
    public RawImage cameraViewImage;

    public Quaternion baseRotation;

    public void CameraOn()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }

        if (WebCamTexture.devices.Length == 0)
        {
            Debug.Log("no camera!");
            return;
        }

        WebCamDevice[] devices = WebCamTexture.devices;
        int selectedCameraIndex = -1;

        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing == false)
            {
                selectedCameraIndex = i;
                break;
            }
        }

        if (selectedCameraIndex >= 0)
        {
            camTexture = new WebCamTexture(devices[selectedCameraIndex].name);
            camTexture.requestedFPS = 30;

            cameraViewImage.texture = camTexture;
            // baseRotation = cameraViewImage.gameObject.transform.rotation;
            // cameraViewImage.gameObject.transform.rotation = baseRotation * Quaternion.AngleAxis(camTexture.videoRotationAngle, new Vector3(0, 0, -90));
            camTexture.Play();
        }
    }

    private IEnumerator SaveImage()
    {
        //Create a Texture2D with the size of the rendered image on the screen.
        Texture2D texture = new Texture2D(cameraViewImage.texture.width, cameraViewImage.texture.height, TextureFormat.ARGB32, false);
        //Save the image to the Texture2D
        texture = RotateTexture(texture, 90);
        texture.SetPixels32(camTexture.GetPixels32());
    
        texture.Apply();
        yield return new WaitForEndOfFrame();

        _tesseractDriver = new TesseractDriver();
        Recoginze(texture);

        // Save the screenshot to Gallery/Photos
        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(texture, "CameraTest", "CaptureImage.png", (success, path) => Debug.Log("Media save result: " + success + " " + path));
        // To avoid memory leaks
        Destroy(texture);
    }

    public void clickCapture()
    {       
        StartCoroutine(SaveImage());
    }

    public void CameraOff()
    {
        if (camTexture != null)
        {
            camTexture.Stop();
            WebCamTexture.Destroy(camTexture);
            camTexture = null;
        }
    }

    private void Recoginze(Texture2D outputTexture)
    {
        _texture = outputTexture;
        ClearTextDisplay();
        _tesseractDriver.Setup(OnSetupCompleteRecognize);
    }

    private void OnSetupCompleteRecognize()
    {
        AddToTextDisplay(_tesseractDriver.Recognize(_texture));
        AddToTextDisplay(_tesseractDriver.GetErrorMessage(), true);
    }

    private void ClearTextDisplay()
    {
        _text = "";
    }

    private void AddToTextDisplay(string text, bool isError = false)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        _text += (string.IsNullOrWhiteSpace(displayText.text) ? "" : "\n") + text;

        if (isError)
            Debug.LogError(text);
        else
            Debug.Log(text);
    }

    private void LateUpdate()
    {
        displayText.text = _text;
    }

    Texture2D RotateTexture (Texture2D texture, float eulerAngles)
    {
        int x;
        int y;
        int i;
        int j;
        float phi = eulerAngles / (180 / Mathf.PI);
        float sn = Mathf.Sin (phi);
        float cs = Mathf.Cos (phi);
        Color32[] arr = texture.GetPixels32 ();
        Color32[] arr2 = new Color32[arr.Length];
        int W = texture.width;
        int H = texture.height;
        int xc = W / 2;
        int yc = H / 2;
        for (j=0; j<H; j++) {
            for (i=0; i<W; i++) {
                arr2 [j * W + i] = new Color32 (0, 0, 0, 0);
                x = (int)(cs * (i - xc) + sn * (j - yc) + xc);
                y = (int)(-sn * (i - xc) + cs * (j - yc) + yc);
                if ((x > -1) && (x < W) && (y > -1) && (y < H)) {
                        arr2 [j * W + i] = arr [y * W + x];
                }
            }
        }
        Texture2D newImg = new Texture2D (W, H);
        newImg.SetPixels32 (arr2);
        newImg.Apply ();
        return newImg;
    }
}