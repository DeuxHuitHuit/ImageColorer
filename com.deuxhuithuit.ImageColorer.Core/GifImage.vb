Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Public Class GifImage

    Public Shared Sub CreateGifImage(ByRef refImage As Image, ByRef destImage As Image)
        ' Copy the palette to assure colors follow
        destImage.Palette = refImage.Palette

        'now to copy the actual bitmap data
        'lock the source and destination bits
        Dim src As BitmapData = CType(refImage, Bitmap).LockBits(New Rectangle(0, 0, refImage.Width, refImage.Height), ImageLockMode.ReadOnly, refImage.PixelFormat)
        Dim dst As BitmapData = CType(destImage, Bitmap).LockBits(New Rectangle(0, 0, destImage.Width, destImage.Height), ImageLockMode.WriteOnly, destImage.PixelFormat)

        'steps through each pixel
        Dim y As Integer
        For y = 0 To refImage.Height - 1
            Dim x As Integer
            For x = 0 To refImage.Width - 1
                'transferring the bytes
                Marshal.WriteByte(dst.Scan0, dst.Stride * y + x, Marshal.ReadByte(src.Scan0, src.Stride * y + x))
            Next x
        Next y

        'all done, unlock the bitmaps
        CType(refImage, Bitmap).UnlockBits(src)
        CType(destImage, Bitmap).UnlockBits(dst)
    End Sub

    Public Shared Function CreateGifImage(ByRef refImage As Image) As Image
        'Create a new 8 bit per pixel image
        Dim bm As New Bitmap(refImage.Width, refImage.Height, PixelFormat.Format8bppIndexed)

        CreateGifImage(refImage, bm)

        Return bm
    End Function

    Public Shared Sub ReplaceColorInPalette(ByRef refImage As Image, ByVal refPalette As ColorPalette, ByVal victimColor As Color, ByVal newColor As Color)
        'get it's palette
        Dim ncp As ColorPalette = refPalette

        ' Start with the refPalette
        Dim palette As Drawing.Imaging.ColorPalette = refPalette
        For x As Integer = 0 To palette.Entries.Length - 1
            Dim color As Drawing.Color = palette.Entries(x)
            Dim alpha As Integer = 255
            ' if we found our victim
            If color.R = victimColor.R AndAlso color.B = victimColor.B AndAlso color.G = victimColor.G Then
                ' replace it in the palette
                ncp.Entries(x) = Drawing.Color.FromArgb(victimColor.A, newColor.R, newColor.G, newColor.B)
            Else
                ncp.Entries(x) = Drawing.Color.FromArgb(color.A, color.R, color.G, color.B)
            End If
        Next
        're-insert the palette
        refImage.Palette = ncp
    End Sub

    Public Shared Sub ConverToGifImageWithNewColor(ByRef refImage As Image, ByVal refPalette As ColorPalette, ByVal victimColor As Color, ByVal newColor As Color)
        ReplaceColorInPalette(refImage, refPalette, victimColor, newColor)

        ' Rewrite the bitmap data in a new image
        Dim gifImage As Image = Core.GifImage.CreateGifImage(refImage)

        refImage.Dispose()

        refImage = gifImage
    End Sub

    Public Shared Sub ReplaceTransparencyColor()
        'copy all the entries from the old palette removing any transparency
        'Dim n As Integer = 0
        'Dim c As Color
        'For Each c In bm.Palette.Entries
        '    ncp.Entries(n) = Color.FromArgb(255, c)
        '    n += 1
        'Next c
        'Set the newly selected transparency
        'ncp.Entries(0) = Color.FromArgb(0, bm.Palette.Entries(0))
        're-insert the palette
        'refImage.Palette = ncp
    End Sub

    Public Shared Function ParseColor(ByVal s As String) As Color
        Dim r As Integer = 255
        Dim g As Integer = 255
        Dim b As Integer = 255
        Dim splitted() As String = s.Split(","c)
        If splitted.Length <> 3 Then
            If s.Length = 6 Then
                Integer.TryParse(s.Substring(0, 2), Globalization.NumberStyles.HexNumber, Globalization.CultureInfo.InvariantCulture, r)
                Integer.TryParse(s.Substring(2, 2), Globalization.NumberStyles.HexNumber, Globalization.CultureInfo.InvariantCulture, g)
                Integer.TryParse(s.Substring(4, 2), Globalization.NumberStyles.HexNumber, Globalization.CultureInfo.InvariantCulture, b)
            End If
        Else
            Integer.TryParse(splitted(0), r)
            Integer.TryParse(splitted(1), g)
            Integer.TryParse(splitted(2), b)
        End If
        Return Color.FromArgb(255, r, g, b)
    End Function

End Class
