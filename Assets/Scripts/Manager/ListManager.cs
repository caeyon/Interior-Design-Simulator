using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class ListManager : MonoBehaviour
{
    public Button showListB;
    public Button returnB;
    public Image backGround;
    CameraController mainCamera;
    public GameObject UIPrefab;
    public Transform content;
    public TMP_Text totalPriceText;

    public RawImage adRawImage;
    public Button adButton;

    private int totalPrice = 0;

    void Start()
    {
        mainCamera = Camera.main.GetComponent<CameraController>();
        showListB.onClick.AddListener(showList);
        returnB.onClick.AddListener(returnEdit);
    }

    void showList()
    {
        backGround.gameObject.SetActive(true);
        //다른 캔버스들은 끄기 추가
        mainCamera.enabled = false;

        //가구 리스트 정보 가져오기
        getfurnitureList();

        getadv();
    }

    private void getadv()
    {
        //광고(구매링크, 이미지 URL 가져오기)
        int randomInt = Random.Range(1, 41); //1~40
        string randomString = randomInt.ToString();

        FurnitureDatabase.getAD(randomString, (ad) =>
        {
            if (ad != null)
            {
                //이미지 표시
                StartCoroutine(LoadImageFromURL(ad.adImage, adRawImage));

                //버튼 링크
                adButton.onClick.AddListener(() =>
                {
                    Application.OpenURL(ad.adUrl);
                });
            }
        });
    }

    private void getfurnitureList()
    {
        totalPrice = 0;
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        //비동기 작업 수를 추적하기 위한 변수
        int pendingTasks = FCategoryManager.furnitureDataList.Count;

        foreach (FurnitureData furniture in FCategoryManager.furnitureDataList)
        {
            FurnitureDatabase.getFurniture(furniture.originalName, (furniture) =>
            {
                if (furniture != null)
                {
                    if (furniture.name == "ikea 주방")
                    {
                        //비동기 작업 완료 처리
                        pendingTasks--;
                        if (pendingTasks == 0)
                        {
                            UpdateTotalPriceText();
                        }
                        return; //해당 아이템을 처리하지 않고 건너뜀
                    }

                    Transform existingFurniture = FindExistingFurnitureUI($"가구명: {furniture.name}");

                    if (existingFurniture != null)
                    {
                        //이미 존재하면 수량 증가
                        TMP_Text quantityText = existingFurniture.Find("quantity").GetComponent<TMP_Text>();
                        if (quantityText != null)
                        {
                            if (int.TryParse(quantityText.text, out int quantity))
                            {
                                quantity++;
                                quantityText.text = quantity.ToString();
                            }
                        }

                        //총 가격 추가
                        totalPrice += furniture.price;
                    }
                    else
                    {
                        GameObject newFurniture = Instantiate(UIPrefab, content); //프리팹을 Content에 생성

                        //가구 정보 표시
                        TMP_Text nameText = newFurniture.transform.Find("name").GetComponent<TMP_Text>();
                        if (nameText != null)
                        {
                            nameText.text = $"가구명: {furniture.name}";
                        }

                        TMP_Text priceText = newFurniture.transform.Find("price").GetComponent<TMP_Text>();
                        if (priceText != null)
                        {
                            priceText.text = $"가격: {furniture.price}";
                            totalPrice += furniture.price;
                        }

                        TMP_Text sizeText = newFurniture.transform.Find("size").GetComponent<TMP_Text>();
                        if (sizeText != null)
                        {
                            sizeText.text = $"사이즈: {furniture.size}";
                        }

                        TMP_Text quantityText = newFurniture.transform.Find("quantity").GetComponent<TMP_Text>();
                        if (quantityText != null)
                        {
                            quantityText.text = string.Empty;
                            quantityText.text = "1";
                        }

                        Transform imageTransform = newFurniture.transform.Find("furnitureImage");
                        if (imageTransform != null)
                        {
                            RawImage rawImage = imageTransform.GetComponent<RawImage>();
                            if (rawImage != null)
                            {
                                StartCoroutine(LoadImageFromURL(furniture.imageUrl, rawImage));
                            }
                        }

                        Transform buttonTransform = newFurniture.transform.Find("link");
                        if (buttonTransform != null)
                        {
                            Button button = buttonTransform.GetComponent<Button>();
                            if (button != null)
                            {
                                button.onClick.AddListener(() =>
                                {
                                    Application.OpenURL(furniture.link);
                                });
                            }
                        }
                    }
                }

                //비동기 작업 완료 처리
                pendingTasks--;
                if (pendingTasks == 0)
                {
                    UpdateTotalPriceText();
                }
            });
        }
    }

    //총 가격 업데이트 함수
    private void UpdateTotalPriceText()
    {
        totalPriceText.text = totalPrice.ToString();
    }

    //동일한 가구 UI 찾는 함수
    private Transform FindExistingFurnitureUI(string furnitureName)
    {
        foreach (Transform child in content)
        {
            TMP_Text nameText = child.Find("name").GetComponent<TMP_Text>();
            if (nameText != null && nameText.text == furnitureName)
            {
                return child;
            }
        }
        return null;
    }

    private IEnumerator LoadImageFromURL(string url, RawImage targetImage)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                targetImage.texture = texture; //RawImage에 텍스처 적용
            }
            else
            {
                UnityEngine.Debug.LogError($"이미지 로드 실패: {request.error}");
            }
        }
    }

    void returnEdit()
    {
        backGround.gameObject.SetActive(false);
        mainCamera.enabled = true;
    }
}
