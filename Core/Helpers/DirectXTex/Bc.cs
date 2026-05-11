
// C# port of BC.cpp from DirectXTex repository
// for the purpose of block-compression (BC) functionality (DXT1, DXT3 and DXT5)
// https://github.com/microsoft/DirectXTex/blob/a6c5c5939a9d3a3ca3d88a1b59a7c319d37fcf78/DirectXTex/BC.cpp
//
// Original code licensed under MIT License, Copyright (c) Microsoft Corporation.
//
// Certain things have been simplified under assumption of
// no flags nor experimental compile-time flags being used.

using System.Numerics;

namespace DirectX
{
    internal static class Bc
    {
        private const int NUM_PIXELS_PER_BLOCK = 16;
        private const float FLT_MIN = 1.17549435E-38f;
        
        private static readonly HDRColorA g_Luminance = new(0.2125f / 0.7154f, 1.0f, 0.0721f / 0.7154f, 1.0f);
        private static readonly HDRColorA g_LuminanceInv = new(0.7154f / 0.2125f, 1.0f, 0.7154f / 0.0721f, 1.0f);
        
        private struct HDRColorA(float r, float g, float b, float a)
        {
            public float r = r;
            public float g = g;
            public float b = b;
            public float a = a;
        }
        
        public struct D3DX_BC1
        {
            public ushort[] rgb;
            public uint bitmap;

            public D3DX_BC1()
            {
                rgb = new ushort[2];
                bitmap = 0;
            }
        };
        
        public struct D3DX_BC2
        {
            public uint[] bitmap;
            public D3DX_BC1 bc1;

            public D3DX_BC2()
            {
                bitmap = new uint[2];
                bc1 = new D3DX_BC1();
            }
        };

        public struct D3DX_BC3
        {
            public byte[] alpha;
            public byte[] bitmap;
            public D3DX_BC1 bc1;

            public D3DX_BC3()
            {
                alpha = new byte[2];
                bitmap = new byte[6];
                bc1 = new D3DX_BC1();
            }
        };
        
        private struct XMU565
        {
            public ushort Packed;

            public XMU565(ushort packed) => Packed = packed;

            public byte R => (byte)((Packed >> 0) & 0x1F);
            public byte G => (byte)((Packed >> 5) & 0x3F);
            public byte B => (byte)((Packed >> 11) & 0x1F);
        }
        
        private static void HDRColorALerp(ref HDRColorA pOut, HDRColorA pC1, HDRColorA pC2, float s)
        {
            pOut.r = pC1.r + s * (pC2.r - pC1.r);
            pOut.g = pC1.g + s * (pC2.g - pC1.g);
            pOut.b = pC1.b + s * (pC2.b - pC1.b);
            pOut.a = pC1.a + s * (pC2.a - pC1.a);
        }

        private static Vector4 XMLoadU565(XMU565 source)
        {
            return new Vector4(source.R, source.G, source.B, 0.0f);
        }

        private static Vector4 XMVectorSwizzle(Vector4 V, uint E0, uint E1, uint E2, uint E3)
        {
            float[] f = { V.X, V.Y, V.Z, V.W };
            return new Vector4(f[E0], f[E1], f[E2], f[E3]);
        }

        private static void OptimizeAlpha(bool bRange, ref float pX, ref float pY, ReadOnlySpan<float> pPoints, uint cSteps)
        {
            float[] pC6 = [ 5.0f/5.0f, 4.0f/5.0f, 3.0f/5.0f, 2.0f/5.0f, 1.0f/5.0f, 0.0f/5.0f ];
            float[] pD6 = [ 0.0f/5.0f, 1.0f/5.0f, 2.0f/5.0f, 3.0f/5.0f, 4.0f/5.0f, 5.0f/5.0f ];
            float[] pC8 = [ 7.0f/7.0f, 6.0f/7.0f, 5.0f/7.0f, 4.0f/7.0f, 3.0f/7.0f, 2.0f/7.0f, 1.0f/7.0f, 0.0f/7.0f ];
            float[] pD8 = [ 0.0f/7.0f, 1.0f/7.0f, 2.0f/7.0f, 3.0f/7.0f, 4.0f/7.0f, 5.0f/7.0f, 6.0f/7.0f, 7.0f/7.0f ];

            float[] pC = (6 == cSteps) ? pC6 : pC8;
            float[] pD = (6 == cSteps) ? pD6 : pD8;

            float MAX_VALUE = 1.0f;
            float MIN_VALUE;
            if (bRange)
            {
                MIN_VALUE = -1.0f;
            }
            else
            {
                MIN_VALUE = 0.0f;
            }

            float fX = MAX_VALUE;
            float fY = MIN_VALUE;

            if(8 == cSteps)
            {
                for(int iPoint = 0; iPoint < NUM_PIXELS_PER_BLOCK; iPoint++)
                {
                    if(pPoints[iPoint] < fX)
                        fX = pPoints[iPoint];
            
                    if(pPoints[iPoint] > fY)
                        fY = pPoints[iPoint];
                }
            }
            else
            {
                for(int iPoint = 0; iPoint < NUM_PIXELS_PER_BLOCK; iPoint++)
                {
                    if(pPoints[iPoint] < fX && pPoints[iPoint] > MIN_VALUE)
                        fX = pPoints[iPoint];
            
                    if(pPoints[iPoint] > fY && pPoints[iPoint] < MAX_VALUE)
                        fY = pPoints[iPoint];
                }

                if (fX == fY)
                {
                    fY = MAX_VALUE;
                }
            }

            float fSteps = (float) (cSteps - 1);

            for(int iIteration = 0; iIteration < 8; iIteration++)
            {
                float fScale;

                if((fY - fX) < (1.0f / 256.0f))
                    break;
                
                fScale = fSteps / (fY - fX);

                float[] pSteps = new float[8];

                for(int iStep = 0; iStep < cSteps; iStep++)
                    pSteps[iStep] = pC[iStep] * fX + pD[iStep] * fY;

                if(6 == cSteps)
                {
                    pSteps[6] = MIN_VALUE;
                    pSteps[7] = MAX_VALUE;
                }

                float dX  = 0.0f;
                float dY  = 0.0f;
                float d2X = 0.0f;
                float d2Y = 0.0f;

                for(int iPoint = 0; iPoint < NUM_PIXELS_PER_BLOCK; iPoint++)
                {
                    float fDot = (pPoints[iPoint] - fX) * fScale;

                    uint iStep;

                    if(fDot <= 0.0f)
                        iStep = (uint)(((6 == cSteps) && (pPoints[iPoint] <= fX * 0.5f)) ? 6 : 0);
                    else if(fDot >= fSteps)
                        iStep = ((6 == cSteps) && (pPoints[iPoint] >= (fY + 1.0f) * 0.5f)) ? 7 : (cSteps - 1);
                    else
                        iStep = (uint)(fDot + 0.5f);


                    if(iStep < cSteps)
                    {
                        float fDiff = pSteps[iStep] - pPoints[iPoint];

                        dX  += pC[iStep] * fDiff;
                        d2X += pC[iStep] * pC[iStep];

                        dY  += pD[iStep] * fDiff; 
                        d2Y += pD[iStep] * pD[iStep];
                    }
                }

                if(d2X > 0.0f)
                    fX -= dX / d2X;

                if(d2Y > 0.0f)
                    fY -= dY / d2Y;

                if(fX > fY)
                {
                    float f = fX; fX = fY; fY = f;
                }

                if((dX * dX < (1.0f / 64.0f)) && (dY * dY < (1.0f / 64.0f)))
                    break;
            }

            pX = (fX < MIN_VALUE) ? MIN_VALUE : (fX > MAX_VALUE) ? MAX_VALUE : fX;
            pY = (fY < MIN_VALUE) ? MIN_VALUE : (fY > MAX_VALUE) ? MAX_VALUE : fY;
        }
        
        private static void assert(bool condition)
        {
            if (!condition)
            {
                throw new Exception();
            }
        }

        private static void Decode565(HDRColorA pColor, ushort w565)
        {
            pColor.r = (float)((w565 >> 11) & 31) * (1.0f / 31.0f);
            pColor.g = (float)((w565 >> 5) & 63) * (1.0f / 63.0f);
            pColor.b = (float)((w565 >> 0) & 31) * (1.0f / 31.0f);
            pColor.a = 1.0f;
        }

        private static ushort Encode565(HDRColorA pColor)
        {
            HDRColorA Color;

            Color.r = (pColor.r < 0.0f) ? 0.0f : (pColor.r > 1.0f) ? 1.0f : pColor.r;
            Color.g = (pColor.g < 0.0f) ? 0.0f : (pColor.g > 1.0f) ? 1.0f : pColor.g;
            Color.b = (pColor.b < 0.0f) ? 0.0f : (pColor.b > 1.0f) ? 1.0f : pColor.b;

            ushort w;

            w = (ushort)(((int)(Color.r * 31.0f + 0.5f) << 11) |
                           ((int)(Color.g * 63.0f + 0.5f) << 5) |
                           ((int)(Color.b * 31.0f + 0.5f) << 0));

            return w;
        }
            
        static void OptimizeRGB(ref HDRColorA pX, ref HDRColorA pY, HDRColorA[] pPoints, int cSteps, uint flags)
        {
            const float fEpsilon = (0.25f / 64.0f) * (0.25f / 64.0f);

            float[] pC3 = [2.0f / 2.0f, 1.0f / 2.0f, 0.0f / 2.0f];
            float[] pD3 = [0.0f / 2.0f, 1.0f / 2.0f, 2.0f / 2.0f];
            float[] pC4 = [3.0f / 3.0f, 2.0f / 3.0f, 1.0f / 3.0f, 0.0f / 3.0f];
            float[] pD4 = [0.0f / 3.0f, 1.0f / 3.0f, 2.0f / 3.0f, 3.0f / 3.0f];

            float[] pC = (3 == cSteps) ? pC3 : pC4;
            float[] pD = (3 == cSteps) ? pD3 : pD4;

            HDRColorA X = g_Luminance;
            HDRColorA Y = new(0.0f, 0.0f, 0.0f, 1.0f);

            for (int iPoint = 0; iPoint < NUM_PIXELS_PER_BLOCK; iPoint++)
            {
                if (pPoints[iPoint].r < X.r)
                    X.r = pPoints[iPoint].r;

                if (pPoints[iPoint].g < X.g)
                    X.g = pPoints[iPoint].g;

                if (pPoints[iPoint].b < X.b)
                    X.b = pPoints[iPoint].b;

                if (pPoints[iPoint].r > Y.r)
                    Y.r = pPoints[iPoint].r;

                if (pPoints[iPoint].g > Y.g)
                    Y.g = pPoints[iPoint].g;

                if (pPoints[iPoint].b > Y.b)
                    Y.b = pPoints[iPoint].b;
            }

            HDRColorA AB;

            AB.r = Y.r - X.r;
            AB.g = Y.g - X.g;
            AB.b = Y.b - X.b;

            float fAB = AB.r * AB.r + AB.g * AB.g + AB.b * AB.b;

            if (fAB < FLT_MIN)
            {
                pX.r = X.r;
                pX.g = X.g;
                pX.b = X.b;
                pY.r = Y.r;
                pY.g = Y.g;
                pY.b = Y.b;
                return;
            }

            float fABInv = 1.0f / fAB;

            HDRColorA Dir;
            Dir.r = AB.r * fABInv;
            Dir.g = AB.g * fABInv;
            Dir.b = AB.b * fABInv;

            HDRColorA Mid;
            Mid.r = (X.r + Y.r) * 0.5f;
            Mid.g = (X.g + Y.g) * 0.5f;
            Mid.b = (X.b + Y.b) * 0.5f;

            float[] fDir = new float[4];
            fDir[0] = fDir[1] = fDir[2] = fDir[3] = 0.0f;


            for (int iPoint = 0; iPoint < NUM_PIXELS_PER_BLOCK; iPoint++)
            {
                HDRColorA Pt;
                Pt.r = (pPoints[iPoint].r - Mid.r) * Dir.r;
                Pt.g = (pPoints[iPoint].g - Mid.g) * Dir.g;
                Pt.b = (pPoints[iPoint].b - Mid.b) * Dir.b;

                float f;

                f = Pt.r + Pt.g + Pt.b;
                fDir[0] += f * f;

                f = Pt.r + Pt.g - Pt.b;
                fDir[1] += f * f;

                f = Pt.r - Pt.g + Pt.b;
                fDir[2] += f * f;

                f = Pt.r - Pt.g - Pt.b;
                fDir[3] += f * f;
            }

            float fDirMax = fDir[0];
            int iDirMax = 0;

            for (int iDir = 1; iDir < 4; iDir++)
            {
                if (fDir[iDir] > fDirMax)
                {
                    fDirMax = fDir[iDir];
                    iDirMax = iDir;
                }
            }

            if ((iDirMax & 2) > 0)
            {
                (X.g, Y.g) = (Y.g, X.g);
            }

            if ((iDirMax & 1) > 0)
            {
                (X.b, Y.b) = (Y.b, X.b);
            }
            
            if (fAB < 1.0f / 4096.0f)
            {
                pX.r = X.r;
                pX.g = X.g;
                pX.b = X.b;
                pY.r = Y.r;
                pY.g = Y.g;
                pY.b = Y.b;
                return;
            }
            
            float fSteps = (float)(cSteps - 1);
            for (int iIteration = 0; iIteration < 8; iIteration++)
            {
                HDRColorA[] pSteps = new HDRColorA[4];

                for (int iStep = 0; iStep < cSteps; iStep++)
                {
                    pSteps[iStep].r = X.r * pC[iStep] + Y.r * pD[iStep];
                    pSteps[iStep].g = X.g * pC[iStep] + Y.g * pD[iStep];
                    pSteps[iStep].b = X.b * pC[iStep] + Y.b * pD[iStep];
                }

                Dir.r = Y.r - X.r;
                Dir.g = Y.g - X.g;
                Dir.b = Y.b - X.b;

                float fLen = (Dir.r * Dir.r + Dir.g * Dir.g + Dir.b * Dir.b);

                if (fLen < (1.0f / 4096.0f))
                    break;

                float fScale = fSteps / fLen;

                Dir.r *= fScale;
                Dir.g *= fScale;
                Dir.b *= fScale;
                
                float d2X, d2Y;
                HDRColorA dX, dY;
                d2X = d2Y = dX.r = dX.g = dX.b = dY.r = dY.g = dY.b = 0.0f;

                for (int iPoint = 0; iPoint < NUM_PIXELS_PER_BLOCK; iPoint++)
                {
                    float fDot = (pPoints[iPoint].r - X.r) * Dir.r +
                                 (pPoints[iPoint].g - X.g) * Dir.g +
                                 (pPoints[iPoint].b - X.b) * Dir.b;

                    int iStep;
                    if (fDot <= 0.0f)
                        iStep = 0;
                    if (fDot >= fSteps)
                        iStep = cSteps - 1;
                    else
                        iStep = (int)(fDot + 0.5f);


                    HDRColorA Diff;
                    Diff.r = pSteps[iStep].r - pPoints[iPoint].r;
                    Diff.g = pSteps[iStep].g - pPoints[iPoint].g;
                    Diff.b = pSteps[iStep].b - pPoints[iPoint].b;

                    float fC = pC[iStep] * (1.0f / 8.0f);
                    float fD = pD[iStep] * (1.0f / 8.0f);

                    d2X += fC * pC[iStep];
                    dX.r += fC * Diff.r;
                    dX.g += fC * Diff.g;
                    dX.b += fC * Diff.b;

                    d2Y += fD * pD[iStep];
                    dY.r += fD * Diff.r;
                    dY.g += fD * Diff.g;
                    dY.b += fD * Diff.b;
                }
                
                if (d2X > 0.0f)
                {
                    float f = -1.0f / d2X;

                    X.r += dX.r * f;
                    X.g += dX.g * f;
                    X.b += dX.b * f;
                }

                if (d2Y > 0.0f)
                {
                    float f = -1.0f / d2Y;

                    Y.r += dY.r * f;
                    Y.g += dY.g * f;
                    Y.b += dY.b * f;
                }

                if ((dX.r * dX.r < fEpsilon) && (dX.g * dX.g < fEpsilon) && (dX.b * dX.b < fEpsilon) &&
                    (dY.r * dY.r < fEpsilon) && (dY.g * dY.g < fEpsilon) && (dY.b * dY.b < fEpsilon))
                {
                    break;
                }
            }

            pX.r = X.r;
            pX.g = X.g;
            pX.b = X.b;
            pY.r = Y.r;
            pY.g = Y.g;
            pY.b = Y.b;
        }


        static void DecodeBC1(ref Span<Vector4> pColor, D3DX_BC1 pBC )
        {
            Vector4 s_Scale = new
            (
                1.0f / 31.0f, 1.0f / 63.0f, 1.0f / 31.0f, 1.0f
            );

            Vector4 clr0 = XMLoadU565(new XMU565(pBC.rgb[0]));
            Vector4 clr1 = XMLoadU565(new XMU565(pBC.rgb[1]));

            clr0 *= s_Scale;
            clr1 *= s_Scale;

            clr0 = XMVectorSwizzle(clr0, 2, 1, 0, 3);
            clr1 = XMVectorSwizzle(clr1, 2, 1, 0, 3);

            clr0.W = 1.0f; // clr0 = XMVectorSelect(g_XMIdentityR3, clr0, g_XMSelect1110);
            clr1.W = 1.0f; // clr1 = XMVectorSelect(g_XMIdentityR3, clr1, g_XMSelect1110);

            Vector4 clr2, clr3;
            if (pBC.rgb[0] <= pBC.rgb[1])
            {
                clr2 = Vector4.Lerp(clr0, clr1, 0.5f);
                clr3 = Vector4.Zero;
            }
            else
            {
                clr2 = Vector4.Lerp(clr0, clr1, 1.0f / 3.0f);
                clr3 = Vector4.Lerp(clr0, clr1, 2.0f / 3.0f);
            }

            uint dw = pBC.bitmap;

            for (int i = 0; i < NUM_PIXELS_PER_BLOCK; ++i, dw >>= 2)
            {
                switch (dw & 3)
                {
                    case 0: pColor[i] = clr0; break;
                    case 1: pColor[i] = clr1; break;
                    case 2: pColor[i] = clr2; break;

                    default: pColor[i] = clr3; break;
                }
            }
        }
        
        static void EncodeBC1(ref D3DX_BC1 pBC, HDRColorA[] pColor, bool bColorKey, float alphaRef, uint flags)
        {
            int uSteps;

            if (bColorKey)
            {
                int uColorKey = 0;

                for (int i = 0; i < NUM_PIXELS_PER_BLOCK; ++i)
                {
                    if (pColor[i].a < alphaRef)
                        uColorKey++;
                }

                if (NUM_PIXELS_PER_BLOCK == uColorKey)
                {
                    pBC.rgb[0] = 0x0000;
                    pBC.rgb[1] = 0xffff;
                    pBC.bitmap = 0xffffffff;
                    return;
                }

                uSteps = (uColorKey > 0) ? 3 : 4;
            }
            else
            {
                uSteps = 4;
            }

            HDRColorA[] Color = new HDRColorA[NUM_PIXELS_PER_BLOCK];

            for (int i = 0; i < NUM_PIXELS_PER_BLOCK; ++i)
            {
                HDRColorA Clr;
                Clr.r = pColor[i].r;
                Clr.g = pColor[i].g;
                Clr.b = pColor[i].b;

                Color[i].r = (float)(int)(Clr.r * 31.0f + 0.5f) * (1.0f / 31.0f);
                Color[i].g = (float)(int)(Clr.g * 63.0f + 0.5f) * (1.0f / 63.0f);
                Color[i].b = (float)(int)(Clr.b * 31.0f + 0.5f) * (1.0f / 31.0f);

                Color[i].a = 1.0f;
                
                Color[i].r *= g_Luminance.r;
                Color[i].g *= g_Luminance.g;
                Color[i].b *= g_Luminance.b;
            }

            HDRColorA ColorA = new(0, 0, 0, 0);
            HDRColorA ColorB = new(0, 0, 0, 0);
            HDRColorA ColorC = new(0, 0, 0, 0);
            HDRColorA ColorD = new(0, 0, 0, 0);

            OptimizeRGB(ref ColorA, ref ColorB, Color, uSteps, flags);
            
            ColorC.r = ColorA.r * g_LuminanceInv.r;
            ColorC.g = ColorA.g * g_LuminanceInv.g;
            ColorC.b = ColorA.b * g_LuminanceInv.b;

            ColorD.r = ColorB.r * g_LuminanceInv.r;
            ColorD.g = ColorB.g * g_LuminanceInv.g;
            ColorD.b = ColorB.b * g_LuminanceInv.b;

            ushort wColorA = Encode565(ColorC);
            ushort wColorB = Encode565(ColorD);

            if ((uSteps == 4) && (wColorA == wColorB))
            {
                pBC.rgb[0] = wColorA;
                pBC.rgb[1] = wColorB;
                pBC.bitmap = 0x00000000;
                return;
            }

            Decode565(ColorC, wColorA);
            Decode565(ColorD, wColorB);

            ColorA.r = ColorC.r * g_Luminance.r;
            ColorA.g = ColorC.g * g_Luminance.g;
            ColorA.b = ColorC.b * g_Luminance.b;

            ColorB.r = ColorD.r * g_Luminance.r;
            ColorB.g = ColorD.g * g_Luminance.g;
            ColorB.b = ColorD.b * g_Luminance.b;

            HDRColorA[] Step = new HDRColorA[4];

            if ((3 == uSteps) == (wColorA <= wColorB))
            {
                pBC.rgb[0] = wColorA;
                pBC.rgb[1] = wColorB;

                Step[0] = ColorA;
                Step[1] = ColorB;
            }
            else
            {
                pBC.rgb[0] = wColorB;
                pBC.rgb[1] = wColorA;

                Step[0] = ColorB;
                Step[1] = ColorA;
            }

            int[] pSteps3 = [0, 2, 1];
            int[] pSteps4 = [0, 2, 3, 1];
            int[] pSteps;

            if (3 == uSteps)
            {
                pSteps = pSteps3;

                HDRColorALerp(ref Step[2], Step[0], Step[1], 0.5f);
            }
            else
            {
                pSteps = pSteps4;

                HDRColorALerp(ref Step[2], Step[0], Step[1], 1.0f / 3.0f);
                HDRColorALerp(ref Step[3], Step[0], Step[1], 2.0f / 3.0f);
            }

            HDRColorA Dir;
            Dir.r = Step[1].r - Step[0].r;
            Dir.g = Step[1].g - Step[0].g;
            Dir.b = Step[1].b - Step[0].b;

            float fSteps = (float)(uSteps - 1);
            float fScale = (wColorA != wColorB) ? (fSteps / (Dir.r * Dir.r + Dir.g * Dir.g + Dir.b * Dir.b)) : 0.0f;

            Dir.r *= fScale;
            Dir.g *= fScale;
            Dir.b *= fScale;

            uint dw = 0;

            for (int i = 0; i < NUM_PIXELS_PER_BLOCK; ++i)
            {
                if ((3 == uSteps) && (pColor[i].a < alphaRef))
                {
                    dw = (uint)((3 << 30) | (dw >> 2));
                }
                else
                {
                    HDRColorA Clr = new (0,0,0,0);
                    
                    Clr.r = pColor[i].r * g_Luminance.r;
                    Clr.g = pColor[i].g * g_Luminance.g;
                    Clr.b = pColor[i].b * g_Luminance.b;

                    float fDot = (Clr.r - Step[0].r) * Dir.r + (Clr.g - Step[0].g) * Dir.g +
                                 (Clr.b - Step[0].b) * Dir.b;
                    uint iStep;

                    if (fDot <= 0.0f)
                        iStep = 0;
                    else if (fDot >= fSteps)
                        iStep = 1;
                    else
                        iStep = (uint)(pSteps[(int)(fDot + 0.5f)]);

                    dw = (iStep << 30) | (dw >> 2);
                }
            }

            pBC.bitmap = dw;
        }
        
        //=====================================================================================
        // Entry points
        //=====================================================================================

        public static void D3DXDecodeBC1(Span<Vector4> pColor, D3DX_BC1 pBC)
        {
            DecodeBC1(ref pColor, pBC);
        }

        public static void D3DXEncodeBC1(ref D3DX_BC1 pBC, Span<Vector4> pColor, float alphaRef, uint flags)
        {
            HDRColorA[] Color = new HDRColorA[NUM_PIXELS_PER_BLOCK];

            for (int i = 0; i < NUM_PIXELS_PER_BLOCK; ++i)
            {
                Color[i] = new(pColor[i].X, pColor[i].Y, pColor[i].Z, pColor[i].W);
            }

            EncodeBC1(ref pBC, Color, true, alphaRef, flags);
        }

        
        public static void D3DXDecodeBC2(Span<Vector4> pColor, D3DX_BC2 pBC2)
        {
            DecodeBC1(ref pColor, pBC2.bc1);

            uint dw = pBC2.bitmap[0];

            for (int i = 0; i < 8; ++i, dw >>= 4)
                pColor[i].W = (float)(dw & 0xf) * (1.0f / 15.0f);

            dw = pBC2.bitmap[1];

            for (int i = 8; i < NUM_PIXELS_PER_BLOCK; ++i, dw >>= 4)
                pColor[i].W = (float)(dw & 0xf) * (1.0f / 15.0f);
        }

        public static void D3DXEncodeBC2(ref D3DX_BC2 pBC, Span<Vector4> pColor, uint flags)
        {
            HDRColorA[] Color = new HDRColorA[NUM_PIXELS_PER_BLOCK];
            for (int i = 0; i < NUM_PIXELS_PER_BLOCK; ++i)
            {
                Color[i] = new(pColor[i].X, pColor[i].Y, pColor[i].Z, pColor[i].W);
            }
            
            pBC.bitmap[0] = 0;
            pBC.bitmap[1] = 0;

            float[] fError = new float[NUM_PIXELS_PER_BLOCK];

            for (int i = 0; i < NUM_PIXELS_PER_BLOCK; ++i)
            {
                float fAlph = Color[i].a;

                uint u = (uint)(int)(fAlph * 15.0f + 0.5f);

                pBC.bitmap[i >> 3] >>= 4;
                pBC.bitmap[i >> 3] |= (u << 28);
            }

            EncodeBC1(ref pBC.bc1, Color, false, 0.0f, flags);
        }
        
        public static void D3DXDecodeBC3(Span<Vector4> pColor, D3DX_BC3 pBC3)
        {
            DecodeBC1(ref pColor, pBC3.bc1);

            float[] fAlpha = new float[8];

            fAlpha[0] = ((float)pBC3.alpha[0]) * (1.0f / 255.0f);
            fAlpha[1] = ((float)pBC3.alpha[1]) * (1.0f / 255.0f);

            if (pBC3.alpha[0] > pBC3.alpha[1])
            {
                for (int i = 1; i < 7; ++i)
                    fAlpha[i + 1] = (fAlpha[0] * (7 - i) + fAlpha[1] * i) * (1.0f / 7.0f);
            }
            else
            {
                for (int i = 1; i < 5; ++i)
                    fAlpha[i + 1] = (fAlpha[0] * (5 - i) + fAlpha[1] * i) * (1.0f / 5.0f);

                fAlpha[6] = 0.0f;
                fAlpha[7] = 1.0f;
            }

            uint dw = pBC3.bitmap[0] | (uint)(pBC3.bitmap[1] << 8) | (uint)(pBC3.bitmap[2] << 16);

            for (int i = 0; i < 8; ++i, dw >>= 3)
                pColor[i].W = fAlpha[dw & 0x7];

            dw = pBC3.bitmap[3] | (uint)(pBC3.bitmap[4] << 8) | (uint)(pBC3.bitmap[5] << 16);

            for (int i = 8; i < NUM_PIXELS_PER_BLOCK; ++i, dw >>= 3)
                pColor[i].W = fAlpha[dw & 0x7];
        }

        public static void D3DXEncodeBC3(ref D3DX_BC3 pBC, Span<Vector4> pColor, uint flags)
        {
            HDRColorA[] Color = new HDRColorA[NUM_PIXELS_PER_BLOCK];
            for (int i = 0; i < NUM_PIXELS_PER_BLOCK; ++i)
            {
                Color[i] = new(pColor[i].X, pColor[i].Y, pColor[i].Z, pColor[i].W);
            }


            float[] fAlpha = new float[NUM_PIXELS_PER_BLOCK];
            float[] fError = new float[NUM_PIXELS_PER_BLOCK];

            float fMinAlpha = Color[0].a;
            float fMaxAlpha = Color[0].a;

            for (int i = 0; i < NUM_PIXELS_PER_BLOCK; ++i)
            {
                float fAlph = Color[i].a;

                fAlpha[i] = (int)(fAlph * 255.0f + 0.5f) * (1.0f / 255.0f);

                if (fAlpha[i] < fMinAlpha)
                    fMinAlpha = fAlpha[i];
                else if (fAlpha[i] > fMaxAlpha)
                    fMaxAlpha = fAlpha[i];
            }

            EncodeBC1(ref pBC.bc1, Color, false, 0.0f, flags);

            if (1.0f == fMinAlpha)
            {
                pBC.alpha[0] = 0xff;
                pBC.alpha[1] = 0xff;
                Array.Clear(pBC.bitmap, 0, 6);
                return;
            }

            uint uSteps = (uint)(((0.0f == fMinAlpha) || (1.0f == fMaxAlpha)) ? 6 : 8);

            float fAlphaA = 0.0f, fAlphaB = 0.0f;
            OptimizeAlpha(false, ref fAlphaA, ref fAlphaB, fAlpha, uSteps);

            byte bAlphaA = (byte)(int)(fAlphaA * 255.0f + 0.5f);
            byte bAlphaB = (byte)(int)(fAlphaB * 255.0f + 0.5f);

            fAlphaA = (float)bAlphaA * (1.0f / 255.0f);
            fAlphaB = (float)bAlphaB * (1.0f / 255.0f);

            if ((8 == uSteps) && (bAlphaA == bAlphaB))
            {
                pBC.alpha[0] = bAlphaA;
                pBC.alpha[1] = bAlphaB;
                Array.Clear(pBC.bitmap, 0, 6);
                return;
            }

            int[] pSteps6 = [0, 2, 3, 4, 5, 1];
            int[] pSteps8 = [0, 2, 3, 4, 5, 6, 7, 1];

            int[] pSteps;
            float[] fStep = new float[8];

            if (6 == uSteps)
            {
                pBC.alpha[0] = bAlphaA;
                pBC.alpha[1] = bAlphaB;

                fStep[0] = fAlphaA;
                fStep[1] = fAlphaB;

                for (int i = 1; i < 5; ++i)
                    fStep[i + 1] = (fStep[0] * (5 - i) + fStep[1] * i) * (1.0f / 5.0f);

                fStep[6] = 0.0f;
                fStep[7] = 1.0f;

                pSteps = pSteps6;
            }
            else
            {
                pBC.alpha[0] = bAlphaB;
                pBC.alpha[1] = bAlphaA;

                fStep[0] = fAlphaB;
                fStep[1] = fAlphaA;

                for (int i = 1; i < 7; ++i)
                    fStep[i + 1] = (fStep[0] * (7 - i) + fStep[1] * i) * (1.0f / 7.0f);

                pSteps = pSteps8;
            }

            float fSteps = (float)(uSteps - 1);
            float fScale = (fStep[0] != fStep[1]) ? (fSteps / (fStep[1] - fStep[0])) : 0.0f;

            for (int iSet = 0; iSet < 2; iSet++)
            {
                uint dw = 0;

                int iMin = iSet * 8;
                int iLim = iMin + 8;

                for (int i = iMin; i < iLim; ++i)
                {
                    float fAlph = Color[i].a;
                    float fDot = (fAlph - fStep[0]) * fScale;

                    uint iStep;
                    if (fDot <= 0.0f)
                        iStep = (uint)(((6 == uSteps) && (fAlph <= fStep[0] * 0.5f)) ? 6 : 0);
                    else if (fDot >= fSteps)
                        iStep = (uint)(((6 == uSteps) && (fAlph >= (fStep[1] + 1.0f) * 0.5f)) ? 7 : 1);
                    else
                        iStep = (uint)(pSteps[(int)(fDot + 0.5f)]);

                    dw = (iStep << 21) | (dw >> 3);
                }

                pBC.bitmap[0 + iSet * 3] = (byte)(dw & 0xFF);
                pBC.bitmap[1 + iSet * 3] = (byte)((dw >> 8) & 0xFF);
                pBC.bitmap[2 + iSet * 3] = (byte)((dw >> 16) & 0xFF);
            }
        }

        //=====================================================================================
        // custom BC block readers and writers for convenience
        //=====================================================================================
        
        public static D3DX_BC1 ReadBC1(BinaryReader reader)
        {
            return new D3DX_BC1()
            {
                rgb = [reader.ReadUInt16(), reader.ReadUInt16()],
                bitmap = reader.ReadUInt32()
            };
        }

        public static D3DX_BC2 ReadBC2(BinaryReader reader)
        {
            return new D3DX_BC2()
            {
                bitmap = [reader.ReadUInt32(), reader.ReadUInt32()],
                bc1 = ReadBC1(reader)
            };
        }

        public static D3DX_BC3 ReadBC3(BinaryReader reader)
        {
            return new D3DX_BC3()
            {
                alpha = reader.ReadBytes(2),
                bitmap = reader.ReadBytes(6),
                bc1 = ReadBC1(reader)
            };
        }

        public static void WriteBC1(BinaryWriter writer, D3DX_BC1 value)
        {
            writer.Write(value.rgb[0]);
            writer.Write(value.rgb[1]);
            writer.Write(value.bitmap);
        }

        public static void WriteBC2(BinaryWriter writer, D3DX_BC2 value)
        {
            writer.Write(value.bitmap[0]);
            writer.Write(value.bitmap[1]);
            WriteBC1(writer, value.bc1);
        }
        
        public static void WriteBC3(BinaryWriter writer, D3DX_BC3 value)
        {
            writer.Write(value.alpha[0]);
            writer.Write(value.alpha[1]);
            writer.Write(value.bitmap[0]);
            writer.Write(value.bitmap[1]);
            writer.Write(value.bitmap[2]);
            writer.Write(value.bitmap[3]);
            writer.Write(value.bitmap[4]);
            writer.Write(value.bitmap[5]);
            WriteBC1(writer, value.bc1);
        }
    }
}