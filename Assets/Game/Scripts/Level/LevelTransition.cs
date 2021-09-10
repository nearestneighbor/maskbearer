using System.Collections;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class LevelTransition : MonoBehaviour
{
    [Scene]
    [SerializeField] private string _levelName;
    [SerializeField] private string _transitionName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Main.Instance.StartCoroutine(TransitCoroutine());
    }

    private IEnumerator TransitCoroutine()
    {
        yield return Main.UI.Get<UICurtain>().ShowAndWait();
        yield return Main.Level.Load(_levelName);
        yield return Main.UI.Get<UICurtain>().HideAndWait();
    } 

    private void OnDrawGizmos()
    {
        var transition = FindObjectsOfType<LevelTransition>().FirstOrDefault(x
                => x.name == _transitionName
                && x.gameObject.scene.name == _levelName
        );

        if (transition != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transition.transform.position);
        }
    }
}
