using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private void Update()
    {
        UpdateHealth();
    }

    private void UpdateHealth()
    {
        PlayerData.Instance.HealthSegments.ToList().ForEach(segment => segment.UpdateSegment(Time.deltaTime));
        string message = "Health: ";
        PlayerData.Instance.HealthSegments.ToList().ForEach(segment => message += segment.CurrentSegmentSize.ToString() + " ");
        Debug.Log(message);
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        // TODO: Update health UI
    }
}
