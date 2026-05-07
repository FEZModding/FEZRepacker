namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.Input.CodeInput, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
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
