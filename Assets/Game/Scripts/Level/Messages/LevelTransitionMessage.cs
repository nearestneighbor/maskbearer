public class LevelTransitionMessage
{
    public const string Name = "On" + nameof(LevelTransitionMessage);

    public string LevelName { get; }
    public string TransitionName { get; }
    public Direction TransitionDirection { get; }

    public LevelTransitionMessage(string levelName, string transitionName, Direction transitionDirection)
    {
        LevelName = levelName;
        TransitionName = transitionName;
        TransitionDirection = transitionDirection;
    }

    public enum Direction
    {
        // Walk throught
        Left,
        Right,
        // Jump to a hole
        Down,
        // Jump out from a hole
        UpRight,
        UpLeft
    }
}