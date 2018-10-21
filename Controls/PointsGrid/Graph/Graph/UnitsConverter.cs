using System;
using System.Collections.Generic;
using System.Text;

namespace Najm.Controls
{
	class UnitsConverter
	{
		public static Location Pixel2MM(Location pl)
		{
			return new Location(pl.X * _pl2mmX, pl.Y * _pl2mmY);
		}

        public static float Pixel2MM(int val)
        {
            return (float)(val * _pl2mmX);
        }

		public static Location MM2Pixel(Location mm)
		{
			return new Location(mm.X * _mm2plX, mm.Y * _mm2plY);
		}

        public static int MM2Pixel(float val)
        {
            return (int)(val * _mm2plX);
        }

		public static void Calibrate(Location pl, Location mm)
		{
			_pl2mmX = mm.X / pl.X;
			_pl2mmY = mm.Y / pl.Y;
			_mm2plX = pl.X / mm.X;
			_mm2plY = pl.Y / mm.Y;
		}

        private static float _pl2mmX;
        private static float _pl2mmY;
        private static float _mm2plX;
        private static float _mm2plY;
	}
}
