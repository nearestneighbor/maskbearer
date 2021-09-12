using UnityEngine;
using UnityEngine.UI;

public class HUD : UIManager.UIBehaviour
{
    public GameObject healthSegmentPrefab;
    private Transform _segmentsParent;

    private void Awake()
    {
        _segmentsParent = transform.Find("Segments Bar/Segments");
        ModifyHUD();
        _segmentsParent.GetChild(0).Find("Left Cap").gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateHUD();
    }

    public void ModifyHUD()
    {
        if (healthSegmentPrefab == null) return;

        foreach (Transform segment in _segmentsParent)
            Destroy(segment.gameObject);
        
        for (int i = 0; i < PlayerData.Instance.NumMaxHealthSegments; i++)
        {
            var segment = Instantiate(healthSegmentPrefab, _segmentsParent);
            var mid = segment.transform.Find("Mid");
            mid.Find("Bar").GetComponent<Image>().material = new Material(Shader.Find("Custom/Progress Wipe"));
            var midTransform = mid.GetComponent<RectTransform>();
            midTransform.sizeDelta = new Vector2(PlayerData.Instance.MaxHealthSegmentSize, midTransform.sizeDelta.y);

        }

        _segmentsParent.GetChild(0).Find("Left Cap").gameObject.SetActive(false);
        _segmentsParent.GetChild(_segmentsParent.childCount - 1).Find("Right Cap").gameObject.SetActive(false);
    }

    private void UpdateHUD()
    {
        for (int i = 0; i < PlayerData.Instance.NumMaxHealthSegments; i++)
        {
            var segment = PlayerData.Instance.HealthSegments[i];
            var segmentObj = _segmentsParent.GetChild(i);
            var barMaterial = segmentObj.Find("Mid/Bar").GetComponent<Image>().material;
            barMaterial.SetFloat("_Progress", segment.CurrentSegmentSize / segment.maxSegmentSize);
        }
    }
}
