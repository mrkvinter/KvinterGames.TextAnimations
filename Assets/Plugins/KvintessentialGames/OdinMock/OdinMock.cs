#if !ODIN_INSPECTOR
namespace KvintessentialGames.OdinMock
{
    public class ShowInInspectorAttribute : System.Attribute
    {
    }

    public class HideInInspectorAttribute : System.Attribute
    {
    }

    public class InlineEditorAttribute : System.Attribute
    {
    }

    public class PropertyOrderAttribute : System.Attribute
    {
        public PropertyOrderAttribute(int order)
        {
        }
    }

    public class TitleAttribute : System.Attribute
    {
        public TitleAttribute(string title)
        {
        }
    }

    public class LabelWidthAttribute : System.Attribute
    {
        public LabelWidthAttribute(int width)
        {
        }
    }

    public class ShowIfAttribute : System.Attribute
    {
        public ShowIfAttribute(string condition)
        {
        }
    }

    public class ValueDropdownAttribute : System.Attribute
    {
        public ValueDropdownAttribute(string methodName)
        {
        }

        public bool IsUniqueList { get; set; }
    }
}
#endif