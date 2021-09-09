using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public static class UIManagerExtensions
{
    public static IEnumerator ShowAndWait(this UIManager.UIBehaviour panel) { panel.Show(); while (!panel.IsShow || panel.IsTransit) yield return null; }
    public static IEnumerator HideAndWait(this UIManager.UIBehaviour panel) { panel.Hide(); while (panel.IsShow || panel.IsTransit) yield return null; }

    public static IEnumerator PlayAndAwait(this UIManager.UIBehaviour panel, string state)
    {
        var animator = panel.GetComponent<Animator>();
        if (animator != null && animator.enabled)
        {
            animator.Play(state);
            yield return null;

            var layer = animator.GetCurrentAnimatorStateInfo(0);
            var layerTime = layer.length;
            yield return new WaitForSeconds(layerTime);
        }
    }

    public  static IEnumerator PlayAndAwait(this UIManager.UIBehaviour panel, AnimationClip clip)
    {
        if (clip != null)
        {
            var animator = panel.GetComponent<Animator>();
            if (animator == null)
                animator = panel.gameObject.AddComponent<Animator>();

            if (animator.enabled)
            {
                AnimationPlayableUtilities.PlayClip(animator, clip, out PlayableGraph graph);
                yield return new WaitForSeconds(clip.length);
                graph.Destroy();
            }
        }
        else
        {
            Debug.LogWarning("AnimationClip can't be null");
        }
    }
}