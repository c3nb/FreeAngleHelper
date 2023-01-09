using UnityEngine;
using UnityModManagerNet;

namespace FreeAngleHelper
{
    //sadfhasdfjkasdfjasdfjkl;asdfjlasjdfjkasdfjkasdf
    
    public class ShortCut
    {
        [Horizontal]
        [Draw("")] public KeyBindSetting SC = new KeyBindSetting(KeyCode.BackQuote);
    }
    public class Generate
    {
        [Horizontal]
        [Draw("")] public KeyBindSetting G = new KeyBindSetting(KeyCode.G);
    }
    [DrawFields(DrawFieldMask.Public)]
    public class KeyBindSetting
    {
        public KeyBindSetting()
        {
            Key = new KeyBinding();
        }
        
        public KeyBindSetting(KeyCode keyCode)
        {
            Key = new KeyBinding {keyCode = keyCode};
        }
        
        public bool Enabled => Key.keyCode != KeyCode.None;

        public bool Down => Key.Down();

        [Draw("키 설정")] public KeyBinding Key;
    }
    
    [DrawFields(DrawFieldMask.Public)]
    public class KeyBindSettings
    {
        [Header("단축키")]
        [Draw("GUI 단축키")] public ShortCut sc = new ShortCut();
        [Draw("생성 asdfjkl;awfjikpasfdkjlasfdjasfdasdfasfdasdfasfdasdf")] public Generate g = new Generate();
    }
}
