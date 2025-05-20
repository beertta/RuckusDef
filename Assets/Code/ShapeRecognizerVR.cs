using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Para usar OpenCVForUnity, debes tener la importación y paquete instalado en el proyecto.
// Comentario para que no sea error si no lo tienen.
#if OPENCV_FOR_UNITY
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
#endif

public class ShapeRecognizerVR : MonoBehaviour
{
    [Header("Configuración")]
    public RenderTexture drawingRenderTexture; // RenderTexture de la libreta donde se dibuja.
    public Transform rightControllerTransform; // Transform del Right Controller.
    
    [Header("Prefabs para instanciar")]
    public GameObject pyramidPrefab; // Para triángulo
    public GameObject cubePrefab;    // Para rectángulo
    public GameObject spherePrefab;  // Para círculo

#if OPENCV_FOR_UNITY
    // Tamaño máximo para escalar la imagen para procesar (para mejor performance)
    private const int maxProcessSize = 256;
#endif

    // Método público para activar la detección y la instanciación
    // Lo puedes llamar desde un evento input del XR Device Simulator o XR Interaction Toolkit
    public void DetectAndInstantiateShape()
    {
#if OPENCV_FOR_UNITY
        if (drawingRenderTexture == null || rightControllerTransform == null ||
            (pyramidPrefab == null && cubePrefab == null && spherePrefab == null))
        {
            Debug.LogWarning("Faltan referencias asignadas en ShapeRecognizerVR");
            return;
        }

        // Paso 1: Convertir RenderTexture a Texture2D
        Texture2D tex2D = RenderTextureToTexture2D(drawingRenderTexture);

        // Paso 2: Procesar la imagen con OpenCV para encontrar formas
        ShapeType shape = DetectShapeOpenCV(tex2D);

        // Paso 3: Instanciar el objeto según forma detectada
        InstantiateShape(shape);

        // Limpiar textura para no hacer memory leak
        Destroy(tex2D);
#else
        Debug.LogError("Para detectar las formas necesitas importar OpenCVForUnity y definir OPENCV_FOR_UNITY en Scripting Define Symbols");
#endif
    }

#if OPENCV_FOR_UNITY
    private Texture2D RenderTextureToTexture2D(RenderTexture rt)
    {
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        return tex;
    }

    private enum ShapeType
    {
        None,
        Triangle,
        Rectangle,
        Circle
    }

    private ShapeType DetectShapeOpenCV(Texture2D inputTex)
    {
        Mat mat = new Mat(inputTex.height, inputTex.width, CvType.CV_8UC4);
        Utils.texture2DToMat(inputTex, mat);
        Imgproc.cvtColor(mat, mat, Imgproc.COLOR_RGBA2GRAY);

        // Blur para reducir ruido
        Imgproc.GaussianBlur(mat, mat, new Size(5, 5), 0);

        // Umbral para binarizar imagen
        Imgproc.threshold(mat, mat, 100, 255, Imgproc.THRESH_BINARY_INV);

        // Encontrar contornos
        List<MatOfPoint> contours = new List<MatOfPoint>();
        Mat hierarchy = new Mat();
        Imgproc.findContours(mat, contours, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);

        if (contours.Count == 0)
        {
            mat.release();
            hierarchy.release();
            return ShapeType.None;
        }

        // Encontrar contorno más grande (suponemos que es la forma)
        double maxArea = 0;
        int maxContourIndex = 0;
        for (int i = 0; i < contours.Count; i++)
        {
            double area = Imgproc.contourArea(contours[i]);
            if (area > maxArea)
            {
                maxArea = area;
                maxContourIndex = i;
            }
        }

        MatOfPoint largestContour = contours[maxContourIndex];
        double peri = Imgproc.arcLength(new MatOfPoint2f(largestContour.toArray()), true);
        MatOfPoint2f approxCurve = new MatOfPoint2f();
        Imgproc.approxPolyDP(new MatOfPoint2f(largestContour.toArray()), approxCurve, 0.04 * peri, true);

        int vertices = approxCurve.toArray().Length;

        mat.release();
        hierarchy.release();

        // Clasificar la forma basada en número de vértices
        if (vertices == 3)
            return ShapeType.Triangle;
        else if (vertices == 4)
        {
            // Opcionalmente podrías verificar ángulo para asegurar rectángulo
            return ShapeType.Rectangle;
        }
        else
        {
            // Para más de 4 vértices consideramos círculo
            return ShapeType.Circle;
        }
    }
#endif

    private void InstantiateShape(
#if OPENCV_FOR_UNITY
        ShapeType shape
#else
        // Cuando no tienes OpenCV, no instancio nada.
        int dummy = 0
#endif
    )
    {
#if OPENCV_FOR_UNITY
        GameObject prefabToInstantiate = null;

        switch (shape)
        {
            case ShapeType.Triangle:
                prefabToInstantiate = pyramidPrefab;
                break;
            case ShapeType.Rectangle:
                prefabToInstantiate = cubePrefab;
                break;
            case ShapeType.Circle:
                prefabToInstantiate = spherePrefab;
                break;
            default:
                Debug.Log("No se detectó una forma válida.");
                return;
        }

        if(prefabToInstantiate != null)
        {
            Instantiate(prefabToInstantiate, rightControllerTransform.position, rightControllerTransform.rotation);
            Debug.Log($"Instanciado objeto: {shape}");
        }
#else
        Debug.LogWarning("No hay detección sin OpenCV. Instanciación desactivada.");
#endif
    }
}

