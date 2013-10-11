using CastleMaster.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace CastleMaster.Guis
{
    public class GuiManager
    {
        private List<Gui> registeredGuis;
        private Gui importantGui = null;
        private bool hasImportantGui = false;

        public GuiManager()
        {
            registeredGuis = new List<Gui>();
        }

        public void AddGui(Gui gui, bool activate, bool important = false)
        {
            if (important)
            {
                importantGui = gui;
                hasImportantGui = true;
            }
            else
                registeredGuis.Add(gui);
            gui.IsActive = activate;
            registeredGuis = registeredGuis.OrderBy(g => g.Importance).ToList();
        }

        public Gui ImportantGui { get { return importantGui; } }

        public void RemoveGui(Gui gui)
        {
            gui.IsActive = false;
            if (importantGui == gui)
            {
                importantGui = null;
                hasImportantGui = false;
            }
            else
                registeredGuis.Remove(gui);
        }

        public void Update()
        {
            if (hasImportantGui)
            {
                importantGui.Update();
                return;
            }

            for (int i = 0; i < registeredGuis.Count; i++)
            {
                Gui g = registeredGuis[i];
                if (g.IsActive) g.Update();
            }
        }

        public void Render(RenderHelper renderer)
        {
            if (registeredGuis.Count > 0)
            {
                foreach (Gui g in registeredGuis)
                    if (g.IsActive) g.Render(renderer);
            }

            if (hasImportantGui)
                importantGui.Render(renderer);
        }
    }
}
