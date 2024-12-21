using TMPro;
using UnityEngine;

public class CameraHeightAdjuster : MonoBehaviour
{
    public TMP_InputField heightInputField;

    void Start()
    {
        if (heightInputField != null)
        {
            heightInputField.onEndEdit.AddListener(AdjustCameraHeight);
        }
    }

    //Ű �Է� ���� ��� ī�޶� ���� ����
    void AdjustCameraHeight(string input)
    {
        float height;

        if (float.TryParse(input, out height)) //�Է��� ���ڷ� ��ȿ���� Ȯ��
        {
            if (height >= 30 && height < 230)
            {
                float heightInMeters = (height - 10f) / 100f; //cm->m ������ ��ȯ
                Vector3 cameraPosition = transform.localPosition;
                cameraPosition.y = heightInMeters; // �����̿� ���� ����
                transform.localPosition = cameraPosition;
            }
        }
    }

    private void OnDestroy()
    {
        if (heightInputField != null)
        {
            heightInputField.onEndEdit.RemoveListener(AdjustCameraHeight);
        }
    }
}
