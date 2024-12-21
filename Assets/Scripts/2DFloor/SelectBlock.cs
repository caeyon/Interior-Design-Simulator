using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SelectBlock : MonoBehaviour
{
    RaycastHit hit;
    Transform selectedTarget;

    Material outline;
    Renderer renderers;
    List<Material> materialList = new List<Material>();
    static SelectBlock instance = null;

    public List<string> priorityTags = new List<string> { "DOOR", "WINDOW", "WALL" };

    public static SelectBlock Instance
    {
        get
        {
            if (null == instance) instance = FindObjectOfType<SelectBlock>();
            return instance;
        }
    }

    void Awake()
    {
        if (null == instance) instance = this;
    }

    void Start()
    {
        outline = new Material(Shader.Find("DrawOutline"));
    }

    void addOutline(Transform obj)
    {
        if (obj == null) return;

        renderers = obj.GetComponent<Renderer>();

        materialList.Clear();
        materialList.AddRange(renderers.sharedMaterials);
        materialList.Add(outline);

        renderers.materials = materialList.ToArray();
    }

    void removeOutline(Renderer renderer)
    {
        if (renderer != null)
        {
            materialList.Clear();
            materialList.AddRange(renderer.sharedMaterials);
            materialList.Remove(outline);

            renderer.materials = materialList.ToArray();
        }
    }

    void clearTarget()
    {
        if (selectedTarget == null) return;

        selectedTarget = null;
        removeOutline(renderers);
    }

    void selectTarget(Transform obj)
    {
        if (obj == null) return;

        if (obj == selectedTarget)
        {
            clearTarget();
            return;
        }
        clearTarget();
        selectedTarget = obj;
        addOutline(obj);

        // �� �巡�� ����
        if (selectedTarget.CompareTag("DOOR"))
        {
            selectedTarget.transform.GetComponent<DMove>().StartDragging(Input.mousePosition);
            selectedTarget.transform.GetComponent<DMove>().Select();
        }
        else if (selectedTarget.CompareTag("WINDOW"))
        {
            selectedTarget.transform.GetComponent<WinMove>().StartDragging(Input.mousePosition);
            selectedTarget.transform.GetComponent<WinMove>().Select();
        }
        else if (selectedTarget.CompareTag("WALL"))
        {
            selectedTarget.transform.GetComponent<WMoveScale>().StartDragging(Input.mousePosition);
            selectedTarget.transform.GetComponent<WMoveScale>().Select();
        }
    }

    Transform GetHighestPriorityObject(List<RaycastHit> hits)
    {
        foreach (string tag in priorityTags)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.CompareTag(tag))
                {
                    return hit.transform;
                }
            }
        }

        return null;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // ���콺 ��Ŭ��
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.red, 2.0f);

            int layer = 1 << LayerMask.NameToLayer("Block");  // Block ���̾ ����
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layer);
            
            if (hits.Length > 0)
            {

                List<RaycastHit> hitList = new List<RaycastHit>(hits);

                Transform obj = GetHighestPriorityObject(hitList); // �켱������ ���� ��ü ����
                selectTarget(obj);
            }
            else  // Block�� �������� ���� ���
            {
                clearTarget();
            }
        }

        // �̵�
        if (Input.GetMouseButton(0) && selectedTarget != null)
        {
            if (selectedTarget.CompareTag("WALL"))
                selectedTarget.transform.GetComponent<WMoveScale>().MoveObject(Input.mousePosition);
            if (selectedTarget.CompareTag("DOOR"))
                selectedTarget.transform.GetComponent<DMove>().MoveObject(Input.mousePosition);
            if (selectedTarget.CompareTag("WINDOW"))
                selectedTarget.transform.GetComponent<WinMove>().MoveObject(Input.mousePosition);
        }

        // �� ũ�� ����
        if (Input.GetMouseButton(1) && selectedTarget != null && selectedTarget.CompareTag("WALL"))
        {
            selectedTarget.transform.GetComponent<WMoveScale>().ResizeObject(Input.mousePosition);
        }

        // �ð���� ȸ��
        if (Input.GetKeyUp(KeyCode.Q) && selectedTarget != null && selectedTarget.CompareTag("WALL"))
        {
            selectedTarget.transform.GetComponent<WMoveScale>().Rotate();
        }

        //����
        if (Input.GetKeyUp(KeyCode.Delete) && selectedTarget != null)
        {
            if (selectedTarget.CompareTag("WALL"))
            {
                selectedTarget.transform.GetComponent<WMoveScale>().StopDragging();
                selectedTarget.transform.GetComponent<WMoveScale>().Del();
            }
            if (selectedTarget.CompareTag("DOOR"))
            {
                selectedTarget.transform.GetComponent<DMove>().StopDragging();
                selectedTarget.transform.GetComponent<DMove>().Del();
            }
            if (selectedTarget.CompareTag("WINDOW"))
            {
                selectedTarget.transform.GetComponent<WinMove>().StopDragging();
                selectedTarget.transform.GetComponent<WinMove>().Del();
            }
            clearTarget();
        }

        // �巡�� ����
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            if (selectedTarget != null)
            {
                if (selectedTarget.CompareTag("WALL"))
                    selectedTarget.transform.GetComponent<WMoveScale>().StopDragging();
                if (selectedTarget.CompareTag("DOOR"))
                    selectedTarget.transform.GetComponent<DMove>().StopDragging();
                if (selectedTarget.CompareTag("WINDOW"))
                    selectedTarget.transform.GetComponent<WinMove>().StopDragging();
            }
        }
    }
}
