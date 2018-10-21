using System;
using System.Collections.Generic;
using System.Text;

namespace Najm.ImagingCore.ColorMaps
{
    // a silly class just to avoid repeating this RGB lookup for all color map that share the same implementation!

    abstract class RGBColorMap : BaseColorMap
    {
        public abstract override void Initialize();
        public override void Lookup(int colorIndex, out RGB rgbColor)
        {
            if (_table.Length != 256)   // equation below is generic enough but I want to avoid div/mul for no reason.
            {
                colorIndex = (int)((double)colorIndex * (double)_table.Length / 256.0);
            }
            rgbColor.R = _table[colorIndex].R;
            rgbColor.G = _table[colorIndex].G;
            rgbColor.B = _table[colorIndex].B;
        }

        protected void BuildMap(RGB[] map, int tableLength)
        {
            _table = new RGB[tableLength];
            for (int i = 0; i < tableLength; i++)
            {
                int j = (int)(i * map.Length / tableLength + 0.5);
                _table[i].R = map[j].R;
                _table[i].G = map[j].G;
                _table[i].B = map[j].B;
            }
        }
    }
}
