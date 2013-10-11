using CastleMaster.Graphics;

namespace CastleMaster.Guis
{
    public class Gui
    {
        public const int IMPORTANCE_LOW = 0;
        public const int IMPORTANCE_MEDIUM = 1;
        public const int IMPORTANCE_HIGH = 2;

        protected GuiManager manager;

        public Gui(GuiManager manager)
        {
            this.manager = manager;
            IsActive = false;
            Removed = false;
        }

        public virtual int Importance { get { return IMPORTANCE_LOW; } }

        public bool IsActive { get; set; }

        public bool Removed { get; set; }

        public virtual void Remove()
        {
            Removed = true;
            manager.RemoveGui(this);
        }

        public virtual void Update() { }

        public virtual void Render(RenderHelper renderer) { }
    }
}
