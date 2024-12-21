using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ToolTipManager : MonoBehaviour
{
    public GameObject tooltipPanel; //���� �г�
    private GameObject previousObject;
    public TMP_Text nameText; 
    public TMP_Text sizeText;
    public Canvas canvas;  
    public Vector2 tooltipOffset = new Vector2(30, 30); //���콺�� ���� ������ ������

    private RectTransform canvasRectTransform; //Canvas�� RectTransform
    private bool isTooltipVisible = false; //������ ���� ���̴��� ����

    void Start()
    {
        canvasRectTransform = canvas.GetComponent<RectTransform>();

        //�ʱ⿡�� ������ ����
        tooltipPanel.SetActive(false);
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            //UI ���� ���� ��� ���� ����
            HideTooltip();
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject hoveredObject = hit.transform.gameObject;

            if (hoveredObject.CompareTag("FURNITURE"))
            {
                if (previousObject != hoveredObject)
                {
                    previousObject = hoveredObject;
                }

                FurnitureData furniture = FCategoryManager.furnitureDataList.Find(data => data.furnitureName == hoveredObject.name);

                if(furniture.siteName != null)
                {
                    ShowTooltip($"{furniture.siteName}", $"{furniture.size}"); 
                    UpdateTooltipPosition();
                    return;
                }
            }
            else
            {
                HideTooltip();
            }
        }
    }

    void ShowTooltip(string name, string size)
    {
        tooltipPanel.SetActive(true); 
        nameText.text = name;
        sizeText.text = size;
        isTooltipVisible = true;
    }

    void HideTooltip()
    {
        tooltipPanel.SetActive(false); 
        isTooltipVisible = false;      
    }

    void UpdateTooltipPosition()
    {
        Vector2 localPosition; //Canvas ���ο��� ��ȯ�� ��ǥ
        Vector2 mousePosition = Input.mousePosition; //���� ���콺 ��ġ

        //RectTransform�� ������
        RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();

        //���콺 ��ǥ�� Canvas ���� ��ǥ�� ��ȯ
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPosition
        );

        //������ ���� ���̸� ����Ͽ� ���콺�� ��ġ�� ȭ���� ������ ������ ���� ������ �������� �̵�
        if (mousePosition.x > Screen.width * 0.75f)
        {
            localPosition.x -= tooltipRect.sizeDelta.x * 0.5f; //�ؽ�Ʈ UI ���� ���̸� ���
        }

        //���콺 �Ʒ��� ��ġ�ϵ��� Y���� ����
        localPosition.y += tooltipOffset.y;

        //������ �߰�: ������ ���콺�� ���󰡵��� ��
        localPosition.x += tooltipOffset.x;

        //���� ��ġ ����
        tooltipRect.anchoredPosition = localPosition;
    }
}

