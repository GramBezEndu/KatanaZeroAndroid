using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine.SpecialEffects
{
    public class RainbowEffect : SpecialEffect
    {
        private double currentHue = 0f;
        public override void Update(GameTime gameTime)
        {
            if (Enabled)
            {
                foreach (var target in targets)
                    target.Color = HslToRgb(currentHue, 0.5, 0.5);
                currentHue += 0.015f;
                if (currentHue >= 1f)
                    currentHue = 0f;
            }
        }

        // Method source modified from: https://geekymonkey.com/Programming/CSharp/RGB2HSL_HSL2RGB.htm
        // Given H,S,L in range of 0-1
        // Returns a Color (RGB struct) in range of 0-255
        public static Color HslToRgb(double h, double sl, double l)
        {
            double v;
            double r, g, b;

            // default to gray
            r = l;
            g = l;
            b = l;
            v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);

            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }
            Color rgb = new Color(Convert.ToByte(r * 255.0f), Convert.ToByte(g * 255.0f), Convert.ToByte(b * 255.0f));
            return rgb;
        }
    }
}
