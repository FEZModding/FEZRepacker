﻿using FEZRepacker.XNB.Attributes;

namespace FezEngine.Structure
{
    [XNBType("FezEngine.Readers.DotDialogueLineReader")]
    class DotDialogueLine
    {
        [XNBProperty(UseConverter = true)]
        public string ResourceText { get; set; }

        [XNBProperty]
        public bool Grouped { get; set; }


        public DotDialogueLine()
        {
            ResourceText = "";
        }
    }
}