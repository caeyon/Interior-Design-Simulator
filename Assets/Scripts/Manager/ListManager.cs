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
        //�ٸ� ĵ�������� ���� �߰�
        mainCamera.enabled = false;

        //���� ����Ʈ ���� ��������
        getfurnitureList();

        getadv();
    }

    private void getadv()
    {
        //����(���Ÿ�ũ, �̹��� URL ��������)
        int randomInt = Random.Range(1, 41); //1~40
        string randomString = randomInt.ToString();

        FurnitureDatabase.getAD(randomString, (ad) =>
        {
            if (ad != null)
            {
                //�̹��� ǥ��
                StartCoroutine(LoadImageFromURL(ad.adImage, adRawImage));

                //��ư ��ũ
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

        //�񵿱� �۾� ���� �����ϱ� ���� ����
        int pendingTasks = FCategoryManager.furnitureDataList.Count;

        foreach (FurnitureData furniture in FCategoryManager.furnitureDataList)
        {
            FurnitureDatabase.getFurniture(furniture.originalName, (furniture) =>
            {
                if (furniture != null)
                {
                    if (furniture.name == "ikea �ֹ�")
                    {
                        //�񵿱� �۾� �Ϸ� ó��
                        pendingTasks--;
                        if (pendingTasks == 0)
                        {
                            UpdateTotalPriceText();
                        }
                        return; //�ش� �������� ó������ �ʰ� �ǳʶ�
                    }

                    Transform existingFurniture = FindExistingFurnitureUI($"������: {furniture.name}");

                    if (existingFurniture != null)
                    {
                        //�̹� �����ϸ� ���� ����
                        TMP_Text quantityText = existingFurniture.Find("quantity").GetComponent<TMP_Text>();
                        if (quantityText != null)
                        {
                            if (int.TryParse(quantityText.text, out int quantity))
                            {
                                quantity++;
                                quantityText.text = quantity.ToString();
                            }
                        }

                        //�� ���� �߰�
                        totalPrice += furniture.price;
                    }
                    else
                    {
                        GameObject newFurniture = Instantiate(UIPrefab, content); //�������� Content�� ����

                        //���� ���� ǥ��
                        TMP_Text nameText = newFurniture.transform.Find("name").GetComponent<TMP_Text>();
                        if (nameText != null)
                        {
                            nameText.text = $"������: {furniture.name}";
                        }

                        TMP_Text priceText = newFurniture.transform.Find("price").GetComponent<TMP_Text>();
                        if (priceText != null)
                        {
                            priceText.text = $"����: {furniture.price}";
                            totalPrice += furniture.price;
                        }

                        TMP_Text sizeText = newFurniture.transform.Find("size").GetComponent<TMP_Text>();
                        if (sizeText != null)
                        {
                            sizeText.text = $"������: {furniture.size}";
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

                //�񵿱� �۾� �Ϸ� ó��
                pendingTasks--;
                if (pendingTasks == 0)
                {
                    UpdateTotalPriceText();
                }
            });
        }
    }

    //�� ���� ������Ʈ �Լ�
    private void UpdateTotalPriceText()
    {
        totalPriceText.text = totalPrice.ToString();
    }

    //������ ���� UI ã�� �Լ�
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
                targetImage.texture = texture; //RawImage�� �ؽ�ó ����
            }
            else
            {
                UnityEngine.Debug.LogError($"�̹��� �ε� ����: {request.error}");
            }
        }
    }

    void returnEdit()
    {
        backGround.gameObject.SetActive(false);
        mainCamera.enabled = true;
    }
}
