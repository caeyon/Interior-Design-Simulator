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

    //키 입력 들어올 경우 카메라 높이 변경
    void AdjustCameraHeight(string input)
    {
        float height;

        if (float.TryParse(input, out height)) //입력이 숫자로 유효한지 확인
        {
            if (height >= 30 && height < 230)
            {
                float heightInMeters = (height - 10f) / 100f; //cm->m 단위로 변환
                Vector3 cameraPosition = transform.localPosition;
                cameraPosition.y = heightInMeters; // 눈높이에 맞춰 조정
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
