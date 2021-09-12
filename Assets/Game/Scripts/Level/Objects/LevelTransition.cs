using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelTransition : MonoBehaviour
{
    #region Inspector

    [Label("Level")]
    [Dropdown("GetSceneNames")]
    [SerializeField] private string _levelName;

    [ShowIf("HasSceneTrue")]
    [Label("Transition")]
    [Dropdown("GetSceneTransitions")]
    [SerializeField] private string _transitionName;

    [Button]
    [ShowIf("HasSceneFalse")]
    private void Load()
    {
        #if UNITY_EDITOR
            var scenePath = GetScenePath(_levelName);
            var sceneMode = UnityEditor.SceneManagement.OpenSceneMode.Additive;

            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath, sceneMode);
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
        #endif
    }

    [Button]
    [ShowIf("HasSceneTrueAndSelf")]
    private void Unload()
    {
        #if UNITY_EDITOR
            var scene = SceneManager.GetSceneByName(_levelName);

            UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
        #endif
    }

    private bool HasSceneTrueAndSelf() => HasSceneTrue() && gameObject.scene.name != _levelName;
    private bool HasSceneFalse() => !HasSceneTrue();
    private bool HasSceneTrue()
    {
        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene.name == _levelName)
                return true;
        }

        return false;
    }

    private List<string> GetSceneTransitions()
    {
        var transitions = 
            FindObjectsOfType<LevelTransition>()
            .Where(x => x.gameObject.scene.name == _levelName)
            .Where(x => x != this)
            .Select(x => x.name);

        return transitions.ToList();
    }

    private List<string> GetSceneNames()
    {
        var result = new List<string>();
        
        // Start from 2 because we skip Launcher & Template scenes
        for (var i = 2; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var path = SceneUtility.GetScenePathByBuildIndex(i);
            var name = Path.GetFileNameWithoutExtension(path);
            result.Add(name);
        }

        return result;
    }

    private string GetScenePath(string sceneName)
    {
        for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var path = SceneUtility.GetScenePathByBuildIndex(i);
            var name = Path.GetFileNameWithoutExtension(path);
            if (name == sceneName)
                return path;
        }

        return null;
    }

    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        if (UnityEditor.Selection.activeGameObject != this.gameObject)
            return;
        #endif

         var target = FindObjectsOfType<LevelTransition>().FirstOrDefault(x
                => x.name == _transitionName
                && x.gameObject.scene.name == _levelName
                && x.gameObject.activeInHierarchy
        );

        if (target != null)
        {
            var from = transform.position;
            var to = target.transform.position;

            var colorStart = Color.green;
            var colorStop = Color.gray;
            var steps = 10;

            for (var i = 0; i < steps; i++)
            {
                var p1 = from + (to - from) * ((float)(i + 0) / steps);
                var p2 = from + (to - from) * ((float)(i + 1) / steps);

                Gizmos.color = Color.Lerp(colorStart, colorStop, (float)i / steps);
                Gizmos.DrawLine(p1, p2);
            }
        }
    }

    #endregion

    public void Place(Player player, LevelTransitionMessage.Direction direction)
    {
        var offset = Vector2.zero;

        switch (direction)
        {
            case LevelTransitionMessage.Direction.Left:
                offset = Vector2.right;
                break;
            
            case LevelTransitionMessage.Direction.Right:
                offset = Vector2.left;
                break;

            case LevelTransitionMessage.Direction.Down:
                offset = Vector2.down;
                break;
            
            case LevelTransitionMessage.Direction.UpLeft:
                offset = new Vector2(-2, +3);
                break;
            
            case LevelTransitionMessage.Direction.UpRight:
                offset = new Vector2(+2, +3);
                break;
            
            default:
                throw new NotImplementedException();
        }

        player.transform.position = transform.position + (Vector3)offset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SendMessageUpwards(
            LevelTransitionMessage.Name,
            new LevelTransitionMessage(
                _levelName,
                _transitionName,
                GetEnterDirection(other)
            )
        );
    }

    private LevelTransitionMessage.Direction GetEnterDirection(Collider2D other)
    {
        // TODO: Change method of defining orientation
        var horizontal = transform.localScale.x > transform.localScale.y;
        if (horizontal)
        {
            if (other.transform.position.y > transform.position.y)
                return LevelTransitionMessage.Direction.Down;
            
            if (other.transform.position.x < transform.position.x)
                return LevelTransitionMessage.Direction.UpLeft;

            return LevelTransitionMessage.Direction.UpRight;
        }
        else
        {
            if (other.transform.position.x < transform.position.x)
                return LevelTransitionMessage.Direction.Left;

            return LevelTransitionMessage.Direction.Right;
        }
    }
}