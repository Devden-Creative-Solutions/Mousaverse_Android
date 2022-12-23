using System.Collections;
using System.Linq;
using TensorFlowLite;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(WebCamInput))]
public sealed class FaceMeshSample : MonoBehaviour
{
    [SerializeField, FilePopup("*.tflite")]
    private string faceModelFile = "coco_ssd_mobilenet_quant.tflite";

    [SerializeField, FilePopup("*.tflite")]
    private string faceMeshModelFile = "coco_ssd_mobilenet_quant.tflite";

    [SerializeField]
    private bool useLandmarkToDetection = true;

    [SerializeField]
    private RawImage cameraView = null;

    [SerializeField]
    private RawImage croppedView = null;

    [SerializeField]
    private Material faceMaterial = null;

    [Space]

    private FaceDetect faceDetect;
    private FaceMesh faceMesh;
    private PrimitiveDraw draw;
    [SerializeField] private MeshFilter faceMeshFilter;
    [SerializeField] private Vector3[] faceKeypoints;
    [SerializeField] private FaceDetect.Result detectionResult;
    [SerializeField] private FaceMesh.Result meshResult;
    private readonly Vector3[] rtCorners = new Vector3[4];

    [SerializeField] private GameObject pointParent;

    [Space]
    [Header("MeshDistance")]
    [SerializeField] float lipsUpperMaxDistacnce = 0.14f;
    [SerializeField] float lipsLowerMinDistacnce = 0.04f;

    [Space]
    [Header("ShapeBlends")]
    [SerializeField] SkinnedMeshRenderer mouthBlendShape;
    [SerializeField] SkinnedMeshRenderer eyebrowBlendShape;
    [SerializeField] SkinnedMeshRenderer eyesBlendShape;

    private void Start()
    {
        faceDetect = new FaceDetect(faceModelFile);
        faceMesh = new FaceMesh(faceMeshModelFile);
        draw = new PrimitiveDraw(Camera.main, gameObject.layer);

        // Create Face Mesh Renderer
        {
            var go = new GameObject("Face");
            go.transform.SetParent(transform);
            var faceRenderer = go.AddComponent<MeshRenderer>();
            faceRenderer.material = faceMaterial;

            faceMeshFilter = go.AddComponent<MeshFilter>();
            faceMeshFilter.sharedMesh = FaceMeshBuilder.CreateMesh();

            faceMeshFilter.GetComponent<MeshRenderer>().enabled = false;
            faceKeypoints = new Vector3[FaceMesh.KEYPOINT_COUNT];
        }

        var webCamInput = GetComponent<WebCamInput>();
        webCamInput.OnTextureUpdate.AddListener(OnTextureUpdate);

        if (WebCamTexture.devices.Length == 0)
            cameraView.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        var webCamInput = GetComponent<WebCamInput>();
        webCamInput.OnTextureUpdate.RemoveListener(OnTextureUpdate);

        faceDetect?.Dispose();
        faceMesh?.Dispose();
        draw?.Dispose();
    }

    private void Update()
    {
        DrawResults(detectionResult, meshResult);
    }

    private void OnTextureUpdate(Texture texture)
    {
        if (detectionResult == null || !useLandmarkToDetection)
        {
            faceDetect.Invoke(texture);
            cameraView.material = faceDetect.transformMat;
            detectionResult = faceDetect.GetResults().FirstOrDefault();

            if (detectionResult == null)
            {
                return;
            }
        }

        faceMesh.Invoke(texture, detectionResult);
        croppedView.texture = faceMesh.inputTex;
        meshResult = faceMesh.GetResult();

        if (meshResult.score < 0.5f)
        {
            detectionResult = null;
            return;
        }

        if (useLandmarkToDetection)
        {
            detectionResult = faceMesh.LandmarkToDetection(meshResult);
        }
    }

    private void DrawResults(FaceDetect.Result detection, FaceMesh.Result face)
    {
        cameraView.rectTransform.GetWorldCorners(rtCorners);
        Vector3 min = rtCorners[0];
        Vector3 max = rtCorners[2];

        // Draw Face Detection
        if (detection != null)
        {
            draw.color = Color.blue;
            Rect rect = MathTF.Lerp(min, max, detection.rect, true);
            draw.Rect(rect, 0.05f);
            foreach (Vector2 p in detection.keypoints)
            {
                draw.Point(MathTF.Lerp(min, max, new Vector3(p.x, 1f - p.y, 0)), 0.1f);
            }
            draw.Apply();
        }

        if (face != null)
        {
            moveMouth = false;
            // Draw face
            float zScale = (max.x - min.x) / 2;
            draw.color = Color.green;
            for (int i = 0; i < face.keypoints.Length; i++)
            {
                Vector3 kp = face.keypoints[i];
                kp.y = 1f - kp.y;

                Vector3 p = MathTF.Lerp(min, max, kp);
                p.z = face.keypoints[i].z * zScale;

                faceKeypoints[i] = p;

                draw.Point(p, 0.05f);
                //StartCoroutine(DrawSphereForDebug(p, i));
            }
            //Debug.LogFormat("== Key145 distance to Key159 = {0}", Vector3.Distance(face.keypoints[159], face.keypoints[145]));
            draw.Apply();

            // Check blendshape
            // mouth
            distanceLips = Vector3.Distance(face.keypoints[0], face.keypoints[17]);
            distanceLips = RemapValue(distanceLips, lipsLowerMinDistacnce, lipsUpperMaxDistacnce, 0, 100);
            moveMouth = true;

            // Update Mesh
            FaceMeshBuilder.UpdateMesh(faceMeshFilter.sharedMesh, faceKeypoints);
            faceMeshFilter.transform.LookAt(faceMeshFilter.transform.position - GameObject.Find("Main Camera").transform.position);

        }
    }

    float distanceLips = 0, distanceEyes = 0, delay = 5, delayCurrentMount = 0, delayCurrenteyes = 0;
    bool moveMouth = false, moveEyes = false;

    void LateUpdate()
    {
        // mouth
        if (delayCurrentMount < delay && moveMouth)
        {
            mouthBlendShape.SetBlendShapeWeight(0, distanceLips);
            delayCurrentMount += 1;
        }
        if (delayCurrentMount >= delay)
        {
            delayCurrentMount = 0;
        }
    }

    IEnumerator DrawSphereForDebug(Vector3 point, int index)
    {
        /* 
            lipsUpperOuter: [61, 185, 40, 39, 37, 0, 267, 269, 270, 409, 291]
            lipsLowerOuter: [146, 91, 181, 84, 17, 314, 405, 321, 375, 291]
            lipsUpperInner: [78, 191, 80, 81, 82, 13, 312, 311, 310, 415, 308]
            lipsLowerInner: [78, 95, 88, 178, 87, 14, 317, 402, 318, 324, 308]
            
            rightEyeUpper: [246, 161, 160, 159, 158, 157, 173]
            rightEyeLower: [33, 7, 163, 144, 145, 153, 154, 155, 133]

            leftEyeUpper: [466, 388, 387, 386, 385, 384, 398]
            leftEyeLower: [263, 249, 390, 373, 374, 380, 381, 382, 362]
        */

        //point = new Vector3(point.x, point.y, 0);

        if (index == 145 || index == 159 || index == 374 || index == 386)
        {
            int[] lipsUpperOuter = new int[] { 61, 185, 40, 39, 37, 0, 267, 269, 270, 409, 291 };
            int[] lipsLowerOuter = new int[] { 146, 91, 181, 84, 17, 314, 405, 321, 375, 291 };

            int[] rightEyeUpper = new int[] { 246, 161, 160, 159, 158, 157, 173 };
            int[] rightEyeLower = new int[] { 33, 7, 163, 144, 145, 153, 154, 155, 133 };

            int[] leftEyeUpper = new int[] { 466, 388, 387, 386, 385, 384, 398 };
            int[] leftEyeLower = new int[] { 263, 249, 390, 373, 374, 380, 381, 382, 362 };

            float scale = 0.1f;
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.parent = pointParent.transform;
            sphere.transform.name = "circulo: " + index;
            sphere.transform.position = point;
            sphere.transform.localScale = new Vector3(scale, scale, scale);

            // lips
            if (rightEyeUpper.Contains(index) || rightEyeLower.Contains(index) || leftEyeUpper.Contains(index) || leftEyeLower.Contains(index))
                sphere.GetComponent<MeshRenderer>().material.color = Color.red;


            yield return new WaitForSeconds(0.1f);

            GameObject.Destroy(sphere);
        }

        yield return null;

    }

    private float RemapValue(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

}
