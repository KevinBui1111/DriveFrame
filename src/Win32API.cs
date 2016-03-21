using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

class Win32API
{
    public const uint SWP_NOSIZE = 1;
    public const uint SWP_NOMOVE = 2;
    public const uint SWP_NOACTIVATE = 0x0010;
    public const int HWND_TOP = 0;

    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    public static extern bool SetWindowPos(int hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    public const int GWL_EXSTYLE = -20;
    public const int WS_EX_TRANSPARENT = 0x20;
    public const int WS_EX_LAYERED = 0x80000;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BLENDFUNCTION
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }

    public const Int32 ULW_COLORKEY = 0x00000001;
    public const Int32 ULW_ALPHA = 0x00000002;
    public const Int32 ULW_OPAQUE = 0x00000004;

    public const byte AC_SRC_OVER = 0x00;
    public const byte AC_SRC_ALPHA = 0x01;


    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    private static extern int SaveDC(IntPtr hdc);

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        
    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern bool DeleteDC(IntPtr hdc);

    [DllImport("gdi32.dll", ExactSpelling = true)]
    public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern bool DeleteObject(IntPtr hObject);

    [DllImport("UxTheme.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern int DrawThemeTextEx(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, string text, int iCharCount, int dwFlags, ref RECT pRect, ref DTTOPTS pOptions);
    [DllImport("UxTheme.dll", ExactSpelling = true, SetLastError = true)]
    private static extern int DrawThemeText(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, string text, int iCharCount, int dwFlags1, int dwFlags2, ref RECT pRect);
    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    private static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO pbmi, uint iUsage, int ppvBits, IntPtr hSection, uint dwOffset);
    [DllImport("gdi32.dll")]
    private static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

    #region P/Invoke

    public const int WM_NCLBUTTONDOWN = 0xA1;
    public const int HT_CAPTION = 0x2;

    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    [DllImport("user32.dll")]
    public static extern bool ReleaseCapture();

    #endregion

    //Text format consts
    private const int DT_SINGLELINE = 0x00000020;
    private const int DT_CENTER = 0x00000001;
    private const int DT_VCENTER = 0x00000004;
    private const int DT_NOPREFIX = 0x00000800;

    private struct RGBQUAD
    {
        public byte rgbBlue;
        public byte rgbGreen;
        public byte rgbRed;
        public byte rgbReserved;
    };

    private struct BITMAPINFO
    {
        public BITMAPINFOHEADER bmiHeader;
        public RGBQUAD bmiColors;
    };
    private struct BITMAPINFOHEADER
    {
        public int biSize;
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public int biCompression;
        public int biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;
    };

    private struct POINTAPI
    {
        public int x;
        public int y;
    };

    //Consts for CreateDIBSection
    private const int BI_RGB = 0;
    private const int DIB_RGB_COLORS = 0;//color table in RGBs

    private struct DTTOPTS
    {
        public uint dwSize;
        public uint dwFlags;
        public uint crText;
        public uint crBorder;
        public uint crShadow;
        public int iTextShadowType;
        public POINTAPI ptShadowOffset;
        public int iBorderSize;
        public int iFontPropId;
        public int iColorPropId;
        public int iStateId;
        public int fApplyOverlay;
        public int iGlowSize;
        public IntPtr pfnDrawTextCallback;
        public int lParam;
    };

    private const int DTT_COMPOSITED = (int)(1UL << 13);
    private const int DTT_GLOWSIZE = (int)(1UL << 11);

    //Const for BitBlt
    private const int SRCCOPY = 0x00CC0020;

    private struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;


    };

    public static void DrawTextOnGlass(IntPtr hwnd, String text, Font font, Rectangle ctlrct, int iglowSize)
    {
        //Rectangle rc = new Rectangle(ctlrct.Left, ctlrct.Top, ctlrct.Width + 2 * iglowSize, ctlrct.Height + 2 * iglowSize);
        Rectangle rc = new Rectangle(ctlrct.Left, ctlrct.Top, ctlrct.Width, ctlrct.Height);
        //Rectangle rc2 = new Rectangle(0,0,rc.Width, rc.Height);
        RECT rc2 = new RECT { right = rc.Width, bottom = rc.Height };

        IntPtr destdc = GetDC(hwnd);    //hwnd must be the handle of form,not control
        IntPtr Memdc = CreateCompatibleDC(destdc);   // Set up a memory DC where we'll draw the text.
        IntPtr bitmap;
        IntPtr bitmapOld = IntPtr.Zero;
        IntPtr logfnotOld;

            int uFormat = DT_SINGLELINE | DT_CENTER | DT_VCENTER | DT_NOPREFIX;   //text format

            BITMAPINFO dib = new BITMAPINFO();
            dib.bmiHeader.biHeight = -rc.Height;         // negative because DrawThemeTextEx() uses a top-down DIB
            dib.bmiHeader.biWidth = rc.Width;
            dib.bmiHeader.biPlanes = 1;
            dib.bmiHeader.biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
            dib.bmiHeader.biBitCount = 32;
            dib.bmiHeader.biCompression = BI_RGB;
            if (!(SaveDC(Memdc) == 0))
            {
                bitmap = CreateDIBSection(Memdc, ref dib, DIB_RGB_COLORS, 0, IntPtr.Zero, 0);   // Create a 32-bit bmp for use in offscreen drawing when glass is on
                if (!(bitmap == IntPtr.Zero))
                {
                    bitmapOld = SelectObject(Memdc, bitmap);
                    IntPtr hFont = font.ToHfont();
                    logfnotOld = SelectObject(Memdc, hFont);
                    try
                    {

                        System.Windows.Forms.VisualStyles.VisualStyleRenderer renderer = new System.Windows.Forms.VisualStyles.VisualStyleRenderer(System.Windows.Forms.VisualStyles.VisualStyleElement.Window.Caption.Active);

                        DTTOPTS dttOpts = new DTTOPTS();

                        dttOpts.dwSize = (uint)Marshal.SizeOf(typeof(DTTOPTS));

                        dttOpts.dwFlags = DTT_COMPOSITED | DTT_GLOWSIZE;


                        dttOpts.iGlowSize = iglowSize;

                        DrawThemeTextEx(renderer.Handle, Memdc, 0, 0, text, -1, uFormat, ref rc2, ref dttOpts);




                        BitBlt(destdc, rc.Left, rc.Top, rc.Width, rc.Height, Memdc, 0, 0, SRCCOPY);



                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message);
                    }

                    //Remember to clean up
                    SelectObject(Memdc, bitmapOld);
                    SelectObject(Memdc, logfnotOld);
                    DeleteObject(bitmap);
                    DeleteObject(hFont);

                    ReleaseDC(Memdc, destdc);
                    DeleteDC(Memdc);
                }
            }
    }

    /// <para>Changes the current bitmap with a custom opacity level.  Here is where all happens!</para>
    internal static void SetBitmap(Form frm, Bitmap bitmap, byte opacity)
    {
        //if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
        //    throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");

        // The ideia of this is very simple,
        // 1. Create a compatible DC with screen;
        // 2. Select the bitmap with 32bpp with alpha-channel in the compatible DC;
        // 3. Call the UpdateLayeredWindow.

        IntPtr screenDc = Win32API.GetDC(IntPtr.Zero);
        IntPtr memDc = Win32API.CreateCompatibleDC(screenDc);
        IntPtr hBitmap = IntPtr.Zero;
        IntPtr oldBitmap = IntPtr.Zero;

        try
        {
            hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));  // grab a GDI handle from this GDI+ bitmap
            oldBitmap = Win32API.SelectObject(memDc, hBitmap);

            Size size = bitmap.Size;
            Point pointSource = new Point(0, 0);
            Point topPos = new Point(frm.Left, frm.Top);
            Win32API.BLENDFUNCTION blend = new Win32API.BLENDFUNCTION();
            blend.BlendOp = Win32API.AC_SRC_OVER;
            blend.BlendFlags = 0;
            blend.SourceConstantAlpha = opacity;
            blend.AlphaFormat = Win32API.AC_SRC_ALPHA;

            bool res = Win32API.UpdateLayeredWindow(frm.Handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend, Win32API.ULW_ALPHA);
        }
        finally
        {
            Win32API.ReleaseDC(IntPtr.Zero, screenDc);
            if (hBitmap != IntPtr.Zero)
            {
                Win32API.SelectObject(memDc, oldBitmap);
                Win32API.DeleteObject(hBitmap);
            }
            Win32API.DeleteDC(memDc);
        }
    }

}

public static class DwmAPI
    {
        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMargins);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }

    }
