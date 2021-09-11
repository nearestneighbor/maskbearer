using System.Linq;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    private void Update()
    {
        PlayerData.Instance.HealthSegments.ToList().ForEach(segment => segment.UpdateSegment(Time.deltaTime));
        string message = "Health: ";
        PlayerData.Instance.HealthSegments.ToList().ForEach(segment => message += segment.CurrentSegmentSize.ToString() + " ");
        Debug.Log(message);
    }
}
