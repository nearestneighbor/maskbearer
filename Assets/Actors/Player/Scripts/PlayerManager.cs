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
    }
}
