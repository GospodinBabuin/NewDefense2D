using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FeonY.RainbowForest
{
public class Parallax : MonoBehaviour
{
    public Transform[] layers; // Array of background layers
    public float[] parallaxFactors; // Parallax factors for each layer
    public float smoothing = 1f; // Smoothing factor for parallax effect

    private Transform cameraTransform;
    private Vector3 previousCameraPosition;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        previousCameraPosition = cameraTransform.position;

        if (layers.Length != parallaxFactors.Length)
        {
            Debug.LogError("Number of layers and parallax factors must be the same!");
        }
    }

    void Update()
    {
        // Calculate the parallax offset for each layer
        for (int i = 0; i < layers.Length; i++)
        {
            float parallax = (previousCameraPosition.x - cameraTransform.position.x) * parallaxFactors[i];

            // Calculate the target position for the layer
            float backgroundTargetPosX = layers[i].position.x + parallax;

            // Create a target position vector
            Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, layers[i].position.y, layers[i].position.z);

            // Interpolate between the current position and the target position using smoothing
            layers[i].position = Vector3.Lerp(layers[i].position, backgroundTargetPos, smoothing * Time.deltaTime);
        }

        // Update the previous camera position
        previousCameraPosition = cameraTransform.position;
    }
}
}