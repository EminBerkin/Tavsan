namespace Tavsan.Core.Domain;

public enum Command
{
    Forward,
    Back,
    Right,
    Left,
    Jump,
    Crouch
}

public static class CommandExtensions
{
    public static Direction ToDirection(this Command command, Direction facing) => command switch
    {
        Command.Forward or Command.Jump or Command.Crouch => facing,
        Command.Back => facing.Opposite(),
        Command.Left => facing.TurnLeft(),
        Command.Right => facing.TurnRight(),
        _ => throw new ArgumentOutOfRangeException(nameof(command))
    };
}
