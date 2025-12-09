using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkDirectX11.Model
{
    public class BetterPanelTest : Panel
    {
        public BetterPanelTest()
        {
            this.DoubleBuffered = true;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Do not clear background → no white flash
            // base.OnPaintBackground(e);
        }
    }
}
