using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[ExecuteAlways]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class LevelDoor : MonoBehaviour
{
    [HideInInspector] [SerializeField] private Sprite _sprite;

    private void Awake()
    {
        if (_levelName == null || _levelName == string.Empty)
            _levelName = gameObject.scene.name;

        if (_doorName == null || _doorName == string.Empty)
            _doorName = gameObject.name;
    }

    private void Start()
    {
        var collider = GetComponent<BoxCollider2D>();
        collider.hideFlags = HideFlags.HideInInspector;
        collider.isTrigger = true;

        var sprite = GetComponent<SpriteRenderer>();
        sprite.hideFlags = HideFlags.HideInInspector;
        sprite.color = Color.green;
        sprite.sprite = _sprite;
    }

    #region Position

    private enum Edge { Top, Right, Bottom, Left }

    [SerializeField] private Edge _edge;
    [SerializeField] private float _size = 2;
    [SerializeField] private float _position;

    private void OnValidate()
    {
        var bounds = GetComponentInParent<LevelBounds>();
        if (bounds == null)
            return;

        var hor = _edge == Edge.Top || _edge == Edge.Bottom;

        var hhalf = (bounds.Size.x + 1) / 2f;
        var hsign = _edge == Edge.Right ? +1 : -1;

        var vhalf = (bounds.Size.y + 1) / 2f;
        var vsign = _edge == Edge.Top ? +1 : -1;

        _size = Mathf.RoundToInt(_size * 2) / 2f;
        _size = Mathf.Clamp(_size, 1, hor ? bounds.Size.x : bounds.Size.y);

        var limit = hhalf - _size/2f - 1/2f;
        _position = Mathf.RoundToInt(_position * 4) / 4f;
        _position = Mathf.Clamp(_position, -limit, +limit);

        transform.localPosition = new Vector3(hor ? _position : hsign * hhalf, hor ? vsign * vhalf : _position, -1);
        transform.localScale = new Vector3(hor ? _size : 1, hor ? 1 : _size, 1);
    }

    private void Update()
    {
        if (Application.isPlaying)
            return;

        OnValidate();
    }

    #endregion

    #region Transition

    [Label("To Level")]
    [Dropdown("GetSceneNames")]
    [SerializeField] private string _levelName;

    [FormerlySerializedAs("_transitionName")]
    [ShowIf("HasSceneTrueAndHasTransitions")]
    [Label("To Door")]
    [Dropdown("GetSceneTransitions")]
    [SerializeField] private string _doorName;

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

    private bool HasSceneTrueAndHasTransitions() => HasSceneTrue() && GetSceneTransitions().Count > 0;
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
            FindObjectsOfType<LevelDoor>()
            .Where(x => x.gameObject.scene.name == _levelName)
            .Where(x => x != this)
            .Select(x => x.name);

        return transitions.ToList();
    }

    private List<string> GetSceneNames()
    {
        var result = new List<string>();
        
        for (var i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var path = SceneUtility.GetScenePathByBuildIndex(i);
            var name = Path.GetFileNameWithoutExtension(path);
            if (i > 2 || name == gameObject.scene.name)
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

         var target = FindObjectsOfType<LevelDoor>().FirstOrDefault(x
                => x.name == _doorName
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

        switch (_edge)
        {
            case Edge.Left: offset = Vector2.right; break;
            case Edge.Right: offset = Vector2.left; break;
            case Edge.Top: offset = Vector2.down; break;
            case Edge.Bottom: offset = new Vector2(direction == LevelTransitionMessage.Direction.UpLeft ? -2 : +2, +3); break;
        }

        player.transform.position = transform.position + (Vector3)offset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SendMessageUpwards(
            LevelTransitionMessage.Name,
            new LevelTransitionMessage(
                _levelName,
                _doorName,
                GetEnterDirection(other)
            )
        );
    }

    private LevelTransitionMessage.Direction GetEnterDirection(Collider2D other)
    {
        var vertical = _edge == Edge.Top || _edge == Edge.Bottom;
        if (vertical)
        {
            if (other.transform.position.y > transform.position.y)
                return LevelTransitionMessage.Direction.Down;
            
            if (other.transform.position.x < transform.position.x)
                return LevelTransitionMessage.Direction.UpLeft;

            return LevelTransitionMessage.Direction.UpRight;
        }
        else
        {
            if (_edge == Edge.Right)
                return LevelTransitionMessage.Direction.Left;

            return LevelTransitionMessage.Direction.Right;
        }
    }
}