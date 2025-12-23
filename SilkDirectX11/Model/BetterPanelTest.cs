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
            this.DoubleBuffered = false;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            
           
        }
    }
}
