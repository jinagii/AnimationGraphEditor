using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class InGameUtil 
{
    public static Camera GetCameraMain()
    {
        return Camera.main;
    }

    public static void Swap<T>(ref T a, ref T b)
    {
        T temp = a;
        a = b; b = temp;
    }

    public static bool IsRendererVisible(Renderer renderer)
    {
        return true;
    }

    public static Vector2 GetGameViewScale()
    {
        return new Vector2(1.0f, 1.0f);
    }

    static public void CalculateFrustumPlanes(Camera camera, ref Plane[] planes)
    {
        // GeometryUtility.CalculateFrustumPlanes() 함수 대체
        // GeometryUtility.CalculateFrustumPlanes() 함수가 호출시 Heap 할당을 함.

        // Left, Right, Down, Up, Near, Far
        float near = camera.nearClipPlane;
        float far = camera.farClipPlane;

        Transform camTr = camera.transform;

        Vector3 forward = camTr.forward;
        Vector3 pos = camTr.position;

        Vector3 nearPos = pos + forward * near;

        // near
        planes[4] = new Plane(forward, nearPos);

        // Far
        planes[5] = new Plane(-forward, pos + forward * far);

        float verticalFOV = camera.fieldOfView;
        float horizontalFOV = Mathf.Atan(Mathf.Tan(verticalFOV * Mathf.Deg2Rad) * camera.aspect) * Mathf.Rad2Deg;

        Vector3 right = camTr.right;
        Vector3 left = -right;
        Vector3 up = camTr.up;
        Vector3 down = -up;

        // Down
        Quaternion rot = Quaternion.AngleAxis(0.5f * verticalFOV, right);
        planes[2] = new Plane(rot * up, pos);

        // Up
        rot = Quaternion.AngleAxis(-0.5f * verticalFOV, right);
        planes[3] = new Plane(rot * down, pos);

        // Left
        rot = Quaternion.AngleAxis(-0.5f * horizontalFOV, up);
        planes[0] = new Plane(rot * right, pos);

        // Right
        rot = Quaternion.AngleAxis(0.5f * horizontalFOV, up);
        planes[1] = new Plane(rot * left, pos);

    }

    public static bool IsValidCamera(Camera camera)
    {
        if (camera.nearClipPlane <= 0.0f)
            return false;

        if (camera.nearClipPlane >= camera.farClipPlane)
            return false;

        if (camera.fieldOfView <= 0.0f)
            return false;

        return true;
    }

#if UNITY_EDITOR
    public static void InitGameViewScale()
    {
        // 매프레임 호출하면,
        // Editor의 focus 없는 상태에서는 task bar가 깜박거리는 문제가 있음.

        System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
        System.Type type = assembly.GetType("UnityEditor.GameView");
        UnityEditor.EditorWindow v = UnityEditor.EditorWindow.GetWindow(type);

        var defScaleField = type.GetField("m_defaultScale", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        float defaultScale = (float)defScaleField.GetValue(v);

        var areaField = type.GetField("m_ZoomArea", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        var areaObj = areaField.GetValue(v);

        var scaleField = areaObj.GetType().GetField("m_Scale", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        //_gameViewScaleValue = (Vector2)scaleField.GetValue(areaObj);
    }
#endif

    public static Transform FindChildByName(string ThisName, Transform ThisGObj)
    {
        Transform ReturnObj;

        // If the name match, we're return it
        if (ThisGObj.name == ThisName)
            return ThisGObj.transform;

        // Else, we go continue the search horizontaly and verticaly
        for (int i = 0; i < ThisGObj.childCount; i++)
        {
            Transform child = ThisGObj.GetChild(i);
            ReturnObj = FindChildByName(ThisName, child);
            if (ReturnObj != null)
                return ReturnObj;
        }

        return null;
    }

#if UNITY_EDITOR
    public static bool IsSceneViewLightingMode(Camera camera)
    {
        var sceneList = SceneView.sceneViews;
        for (int i = 0; i < sceneList.Count; ++i)
        {
            SceneView sceneView = (SceneView)sceneList[i];
            if (sceneView.camera == camera)
            {
                return sceneView.sceneLighting;
            }
        }
        return false;
    }
#endif

    public static float GetRenderScale()
    {
        return 1.0f;
    }
}

public struct ObjectPool<T> where T : class, new()
{
    T[] _buffer;

    public ObjectPool(int bufferCount, Action<T, int> initFunc)
    {
        _buffer = new T[bufferCount];
        for (int i = 0; i < bufferCount; ++i)
        {
            _buffer[i] = new T();
            initFunc(_buffer[i], i);
        }
    }

    public T PopObject()
    {
        T obj = null;
        for (int i = 0; i < _buffer.Length; ++i)
        {
            if (_buffer[i] != null)
            {
                obj = _buffer[i];
                _buffer[i] = null;
                break;
            }
        }

#if SHOW_ASSERT
        Debug.Assert(obj != null, $"no more item to pop");
#endif
        return obj;
    }

    public void PushObject(T obj)
    {
#if SHOW_ASSERT
        bool found = false;
#endif
        for (int i = 0; i < _buffer.Length; ++i)
        {
            if (_buffer[i] == null || _buffer[i].Equals(obj))
            {
                _buffer[i] = obj;
#if SHOW_ASSERT
                found = true;
#endif
                break;
            }
        }

#if SHOW_ASSERT
        Debug.Assert(found, $"No more Space");
#endif
    }

    public void ResetObject(T obj)
    {
        // 중복 허용
#if SHOW_ASSERT
        bool found = false;
#endif
        for (int i = 0; i < _buffer.Length; ++i)
        {
            if (_buffer[i] == null || _buffer[i] == obj)
            {
                _buffer[i] = obj;
#if SHOW_ASSERT
                found = true;
#endif
                break;
            }
        }

#if SHOW_ASSERT
        Debug.Assert(found, $"No more Space");
#endif
    }

    public int GetUsedCount()
    {
        int count = 0;
        for (int i = 0; i < _buffer.Length; ++i)
        {
            if (_buffer[i] == null)
                count++;
        }

        return count;
    }
}



#if UNITY_EDITOR
public static class EditorUIUtil
{
    public static Transform _rootNode = null;

    public static void DrawCircle(ref Vector2 center, float radius, int divCount = 16)
    {
        GL.Begin(GL.TRIANGLES);
        GL.Color(Color.red);

        float cX = center.x;
        float cY = center.y;

        //float circleSize = 20.0f;
        for (int i = 0; i < divCount; ++i)
        {
            float angle = 2.0f * Mathf.PI / (float)divCount * (float)i;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            float angle1 = 2.0f * Mathf.PI / (float)divCount * (float)(i + 1);
            float x1 = Mathf.Cos(angle1) * radius;
            float y1 = Mathf.Sin(angle1) * radius;

            GL.Vertex3(cX, cY, 0);
            GL.Vertex3(x + cX, y + cY, 0);
            GL.Vertex3(x1 + cX, y1 + cY, 0);
        }
        GL.End();
    }

    public static void DrawRect(float xMin, float xMax, float yMin, float yMax)
    {
        GL.Begin(GL.LINES);
        GL.Color(Color.white);
        GL.Vertex3(xMin, yMin, 0);
        GL.Vertex3(xMax, yMin, 0);

        GL.Vertex3(xMin, yMax, 0);
        GL.Vertex3(xMax, yMax, 0);

        GL.Vertex3(xMin, yMin, 0);
        GL.Vertex3(xMin, yMax, 0);

        GL.Vertex3(xMax, yMin, 0);
        GL.Vertex3(xMax, yMax, 0);

        GL.End();
    }

    public static GameObject GetNodeGameObject(in string parentName, in string name, PrimitiveType primitiveType)
    {
        if (_rootNode == null)
        {
            GameObject rootGo = new GameObject();
            rootGo.name = "EditorUtilRoot";
            _rootNode = rootGo.transform;
        }

        string parentNodeName = parentName != "" ? parentName + "_Sphere" : "";

        GameObject parentGo = null;
        if (_rootNode != null && parentNodeName != "")
        {
            Transform[] trs = _rootNode.GetComponentsInChildren<Transform>(true);
            foreach (var t in trs)
            {
                if (t.name == parentNodeName)
                {
                    parentGo = t.gameObject;
                }
            }

            if (null == parentGo)
            {
                parentGo = new GameObject();
                parentGo.name = parentNodeName;
                parentGo.transform.parent = _rootNode;
            }
        }

        GameObject go = null;
        if (parentGo != null)
            go = parentGo.FindChildObject(name);
        else
        {
            Transform[] trs = _rootNode.GetComponentsInChildren<Transform>(true);
            foreach (var t in trs)
            {
                if (t.name == name)
                {
                    go = t.gameObject;
                }
            }
        }

        if (go == null)
        {
            go = GameObject.CreatePrimitive(primitiveType);
            go.name = name;
            if (parentGo != null)
                go.transform.parent = parentGo.transform;
        }

        return go;
    }

    public static void SetDebugSphere(in string parentName, in string name, in Vector3 pos, in float scale, in Color color)
    {
        GameObject go = GetNodeGameObject(parentName, name, PrimitiveType.Sphere);

        go.transform.position = pos;
        go.transform.localScale = new Vector3(scale, scale, scale);

        MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            Material sphereColor = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Shaders/Resources/Debug/SphereColor.mat");
            Material matInstance = new Material(sphereColor);
            matInstance.SetColor("_Color", color);
            meshRenderer.material = matInstance;
        }
    }

    public static void SetActiveDebugSphere(in string parentName, bool active)
    {
        if (_rootNode == null)
            return;

        string parentNodeName = parentName != "" ? parentName + "_Sphere" : "";

        GameObject parentGo = null;
        if (parentNodeName != "")
        {
            Transform parentTr = _rootNode.FindChildObject(parentNodeName);
            parentGo = (parentTr != null) ? parentTr.gameObject : null;
            if (null != parentGo)
            {
                if (parentGo.activeSelf != active)
                    parentGo.SetActive(active);
            }
        }
    }

    public static bool IsActiveDebugSphere(in string parentName)
    {
        if (_rootNode == null)
            return false;

        string parentNodeName = parentName != "" ? parentName + "_Sphere" : "";

        GameObject parentGo = null;
        if (parentNodeName != "")
        {
            Transform parentTr = _rootNode.FindChildObject(parentNodeName);
            parentGo = (parentTr != null) ? parentTr.gameObject : null;

            if (parentGo != null && parentGo.activeSelf)
                return true;
        }

        return false;
    }

    public static void DestoryDebugSphere(in string parentName)
    {
        if (_rootNode == null)
            return;

        string parentNodeName = parentName != "" ? parentName + "_Sphere" : "";

        GameObject parentGo = null;
        if (parentNodeName != "")
        {
            Transform parentTr = _rootNode.FindChildObject(parentNodeName);
            parentGo = (parentTr != null) ? parentTr.gameObject : null;
            if (null != parentGo)
            {
                UnityEngine.Object.Destroy(parentGo);
            }
        }
    }

    public static void SetDebugLine(in string parentName, in string name, in Vector3 p0, in Vector3 p1, float scale, in Color color, float offset = 0.0f)
    {
        GameObject go = GetNodeGameObject(parentName, name, PrimitiveType.Plane);

        go.transform.position = 0.5f * (p0 + p1);
        if (offset != 0.0f)
            go.transform.position += new Vector3(0.0f, offset, 0.0f);
        float dist = (p1 - p0).magnitude;
        go.transform.localScale = new Vector3(scale / 10, 1.0f, dist / 10);

        Quaternion rot = Quaternion.identity;

        Vector3 diff = p1 - p0;
        if (diff.y != 0)
            rot = Quaternion.LookRotation(p1 - p0);
        else
        {
            rot = Quaternion.LookRotation(p1 - p0, Vector3.up);
        }

        go.transform.rotation = rot;

        MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            Material sphereColor = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Shaders/Resources/Debug/SphereColor.mat");
            Material matInstance = new Material(sphereColor);
            matInstance.SetColor("_Color", color);
            meshRenderer.material = matInstance;
        }
    }

    public static void SetDebugLineOffset(in string parentName, in string name, in Vector3 p0, in Vector3 offsetFromP0, float scale, in Color color, float offset = 0.0f)
    {
        SetDebugLine(parentName, name, p0, p0 + offsetFromP0, scale, color, offset);
    }

    public static void SetDebugLineCube(in string parentName, in string name, Vector3[] pos, int start, float scale, in Color color)
    {
        // 0 ~ 3 : top
        // 4 ~ 7 : bottom

        SetDebugLine(parentName, name + "_0", pos[start + 0], pos[start + 1], scale, color);
        SetDebugLine(parentName, name + "_1", pos[start + 1], pos[start + 3], scale, color);
        SetDebugLine(parentName, name + "_2", pos[start + 3], pos[start + 2], scale, color);
        SetDebugLine(parentName, name + "_3", pos[start + 2], pos[start + 0], scale, color);

        SetDebugLine(parentName, name + "_4", pos[start + 0 + 4], pos[start + 1 + 4], scale, color);
        SetDebugLine(parentName, name + "_5", pos[start + 1 + 4], pos[start + 3 + 4], scale, color);
        SetDebugLine(parentName, name + "_6", pos[start + 3 + 4], pos[start + 2 + 4], scale, color);
        SetDebugLine(parentName, name + "_7", pos[start + 2 + 4], pos[start + 0 + 4], scale, color);

        SetDebugLine(parentName, name + "_8", pos[start + 0], pos[start + 0 + 4], scale, color);
        SetDebugLine(parentName, name + "_9", pos[start + 1], pos[start + 1 + 4], scale, color);
        SetDebugLine(parentName, name + "_a", pos[start + 2], pos[start + 2 + 4], scale, color);
        SetDebugLine(parentName, name + "_b", pos[start + 3], pos[start + 3 + 4], scale, color);
    }
    public static GameObject SetDebugCube(in string name, in Vector3 pos, in Vector3 size)
    {
        GameObject go = GameObject.Find(name);
        if (go == null)
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
        }

        go.transform.position = pos;
        go.transform.localScale = new Vector3(size.x, size.y, size.z);
        return go;
    }

    public static void LogCompareVector3(in Vector3 v1, in Vector3 v2, in string tag = "")
    {
        Debug.Log(tag + $" ({v1.x}, {v1.y}, {v1.z}) - ({v2.x}, {v2.y}, {v2.z})");
    }

    public static void LogCompareQuaternion(in Quaternion v1, in Quaternion v2, in string tag = "")
    {
        Debug.Log(tag + $" ({v1.x}, {v1.y}, {v1.z}, {v1.w}) - ({v2.x}, {v2.y}, {v2.z}, {v2.w})");
    }
}
#endif

public static class ExtensionMethods
{
    public static Vector3 XZOnly(this Vector3 a)
    {
        return new Vector3(a.x, 0f, a.z);
    }

    public static GameObject FindChildObject(this GameObject parent, string name)
    {
        Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }

    public static Transform FindChildObject(this Transform parent, string name)
    {
        Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            if (t.gameObject.name == name)
            {
                return t;
            }
        }
        return null;
    }
}

