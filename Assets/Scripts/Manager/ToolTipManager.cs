using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ToolTipManager : MonoBehaviour
{
    public GameObject tooltipPanel; //툴팁 패널
    private GameObject previousObject;
    public TMP_Text nameText; 
    public TMP_Text sizeText;
    public Canvas canvas;  
    public Vector2 tooltipOffset = new Vector2(30, 30); //마우스와 툴팁 사이의 오프셋

    private RectTransform canvasRectTransform; //Canvas의 RectTransform
    private bool isTooltipVisible = false; //툴팁이 현재 보이는지 여부

    void Start()
    {
        canvasRectTransform = canvas.GetComponent<RectTransform>();

        //초기에는 툴팁을 숨김
        tooltipPanel.SetActive(false);
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            //UI 위에 있을 경우 툴팁 숨김
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
        Vector2 localPosition; //Canvas 내부에서 변환된 좌표
        Vector2 mousePosition = Input.mousePosition; //현재 마우스 위치

        //RectTransform을 가져옴
        RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();

        //마우스 좌표를 Canvas 내부 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPosition
        );

        //툴팁의 가로 길이를 고려하여 마우스의 위치가 화면의 오른쪽 끝으로 가면 툴팁을 왼쪽으로 이동
        if (mousePosition.x > Screen.width * 0.75f)
        {
            localPosition.x -= tooltipRect.sizeDelta.x * 0.5f; //텍스트 UI 가로 길이를 고려
        }

        //마우스 아래로 위치하도록 Y값을 조정
        localPosition.y += tooltipOffset.y;

        //오프셋 추가: 툴팁이 마우스를 따라가도록 함
        localPosition.x += tooltipOffset.x;

        //툴팁 위치 설정
        tooltipRect.anchoredPosition = localPosition;
    }
}

