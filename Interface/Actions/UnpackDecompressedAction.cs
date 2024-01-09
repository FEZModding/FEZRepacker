namespace FEZRepacker.Interface.Actions
{
    internal class UnpackDecompressedAction : UnpackAction
    {
        protected override UnpackingMode Mode => UnpackingMode.DecompressedXNB;
        public override string Name => "--unpack-decompressed";
        public override string[] Aliases => new string[] {};
        public override string Description =>
            "Unpacks entire .PAK package into specified directory (creates one if doesn't exist)." +
            "and attempts to decompress all XNB assets, but does not convert them.";
    }
}
