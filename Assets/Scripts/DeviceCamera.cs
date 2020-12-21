/*
 * References:
 *  - camera: https://www.youtube.com/watch?v=c6NXkZWXHnc
 *  - dropdown: https://www.youtube.com/watch?v=Q4NYCSIOamY
 */


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeviceCamera : MonoBehaviour
{
    // Camera
    private bool camAvailable;
	private WebCamTexture cameraTexture;
    private WebCamDevice[] devices;

    // Background and Display
    public RawImage cameraDisplay;
    public AspectRatioFitter fitter;
    private Texture defaultBackground;

    // Dropdown to choose between cameras
    public Dropdown devicesDropdown;
    List<string> deviceOptions = new List<string> { "None" };

	
    private void Start()
    {
        // Set default background to whatever is at the scene
        defaultBackground = cameraDisplay.texture;
        devices = WebCamTexture.devices;

        // At least one camera has to be detected
        if (devices.Length != 0)
        {
            camAvailable = true;
            for (int i = 0; i < devices.Length; i++)
            {
                var currentDevice = devices[i];

                // Populate dropdown option list
                deviceOptions.Add(currentDevice.name);
            }

            // Populate dropdown
            devicesDropdown.AddOptions(deviceOptions);

            // Render default camera on the background texture
            devicesDropdown.value = 1;

        }
        else
        {
            Debug.Log("No camera detected");
            camAvailable = false;
            return;
        }
    }


    private void Update()
    {
        if (!camAvailable)
			return;
		
        // Update orientation of the camera
		float ratio = (float)cameraTexture.width / (float)cameraTexture.height;
		fitter.aspectRatio = ratio;
		
        // Check if camera is mirrored. If yes, swap it
		float scaleY = cameraTexture.videoVerticallyMirrored ? -1f: 1f;
		cameraDisplay.rectTransform.localScale = new Vector3(1f, scaleY, 1f);
		
		int orient = -cameraTexture.videoRotationAngle;
		cameraDisplay.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
    }


    public void Dropdown_IndexChanged(int index)
    {
        Debug.Log("Index " + index + ": " + deviceOptions[index]);

        if (index != 0)
        {
            // index has 1 more option than we have devicess
            var selectedDevice = devices[index - 1];
            cameraTexture = new WebCamTexture(selectedDevice.name, Screen.width, Screen.height);
            cameraTexture.Play();
            cameraDisplay.texture = cameraTexture;
        }
        else
        {
            cameraDisplay.texture = defaultBackground;

            // Stop the old device
            if (cameraTexture.isPlaying)
            {
                cameraTexture.Stop();
            }
        }
    }
}
