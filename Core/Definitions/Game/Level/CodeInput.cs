namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.Input.CodeInput")]
    [Flags]
    public enum CodeInput
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8,
        SpinLeft = 16,
        SpinRight = 32,
        Jump = 64,
    }
}
