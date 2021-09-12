using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelTransition : MonoBehaviour
{
    [Scene]
    [SerializeField] private string _levelName;
    // [Dropdown("GetSceneTransitions")]
    [SerializeField] private string _transitionName;

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

    private List<string> GetSceneTransitions()
    {
        var transitions = 
            FindObjectsOfType<LevelTransition>()
            .Where(x => x.gameObject.scene.name == _levelName)
            .Select(x => x.name);

        return transitions.ToList();
    }

    private void OnDrawGizmos()
    {
        var transition = FindObjectsOfType<LevelTransition>().FirstOrDefault(x
                => x.name == _transitionName
                && x.gameObject.scene.name == _levelName
                && x.gameObject.activeInHierarchy
        );

        if (transition != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transition.transform.position);
        }
    }
}
