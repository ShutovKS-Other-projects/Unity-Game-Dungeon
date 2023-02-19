using Item;
using Other;
namespace Player
{
    [System.Serializable]
    public class Attribute
    {
        [System.NonSerialized] public PlayerInventory parent;
        public Attributes type;
        public ModifiableInt value;

        public void SetParent(PlayerInventory _parent)
        {
            parent = _parent;
            value = new ModifiableInt(AttributeModified);
        }

        public void AttributeModified()
        {
            parent.AttributeModified(this);
        }
    }
}