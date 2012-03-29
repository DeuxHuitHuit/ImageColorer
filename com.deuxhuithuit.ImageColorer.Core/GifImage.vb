Imports System.Drawing
Imports System.Drawing.Imaging

Public Class GifImage

    Private Sub createGifImage(ByRef _gifImage As Drawing.Bitmap, ByVal refPalette As ColorPalette, _
                               ByVal r As Integer, ByVal g As Integer, ByVal b As Integer)
        'Create a new 8 bit per pixel image
        Dim bm As New Bitmap(_gifImage.Width, _gifImage.Height, PixelFormat.Format8bppIndexed)
        'get it's palette
        Dim ncp As ColorPalette = bm.Palette

        'copy all the entries from the old palette removing any transparency
        'Dim n As Integer = 0
        'Dim c As Color
        'For Each c In bm.Palette.Entries
        '    ncp.Entries(n) = Color.FromArgb(255, c)
        '    n += 1
        'Next c
        Dim palette As Drawing.Imaging.ColorPalette = refPalette '  _gifImage.Palette
        For x As Integer = 0 To palette.Entries.Length - 1
            Dim color As Drawing.Color = palette.Entries(x)
            Dim alpha As Integer = 255
            ' if we found our victim
            If color.R = victim.R AndAlso color.B = victim.B AndAlso color.G = victim.G Then
                ' replace it in the palette
                ncp.Entries(x) = Drawing.Color.FromArgb(victim.A, r, g, b)
            Else
                ncp.Entries(x) = Drawing.Color.FromArgb(color.A, color.R, color.G, color.B)
            End If
        Next
        'Set the newly selected transparency
        'ncp.Entries(0) = Color.FromArgb(0, bm.Palette.Entries(0))
        're-insert the palette
        bm.Palette = ncp

        'now to copy the actual bitmap data
        'lock the source and destination bits
        Dim src As BitmapData = CType(_gifImage, Bitmap).LockBits(New Rectangle(0, 0, _gifImage.Width, _gifImage.Height), ImageLockMode.ReadOnly, _gifImage.PixelFormat)
        Dim dst As BitmapData = bm.LockBits(New Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.WriteOnly, bm.PixelFormat)

        If (True) Then
            'steps through each pixel
            Dim y As Integer
            For y = 0 To _gifImage.Height - 1
                Dim x As Integer
                For x = 0 To _gifImage.Width - 1
                    'transferring the bytes
                    Marshal.WriteByte(dst.Scan0, dst.Stride * y + x, Marshal.ReadByte(src.Scan0, src.Stride * y + x))
                Next x
            Next y
        End If
        'all done, unlock the bitmaps
        CType(_gifImage, Bitmap).UnlockBits(src)
        bm.UnlockBits(dst)


        _gifImage.Dispose()
        'set the new image in place
        _gifImage = bm
    End Sub



    Public Shared Function ParseColor(ByVal s As String) As Color
        Dim r As Integer = 255
        Dim g As Integer = 255
        Dim b As Integer = 255
        Dim splitted() As String = s.Split(","c)
        If splitted.Length <> 3 Then
            If s.Length = 6 Then
                Integer.TryParse(s.Substring(0, 2), Globalization.NumberStyles.HexNumber, Globalization.CultureInfo.InvariantCulture, r)
                Integer.TryParse(s.Substring(2, 4), Globalization.NumberStyles.HexNumber, Globalization.CultureInfo.InvariantCulture, g)
                Integer.TryParse(s.Substring(4, 6), Globalization.NumberStyles.HexNumber, Globalization.CultureInfo.InvariantCulture, b)
            End If
        Else
            Integer.TryParse(splitted(0), r)
            Integer.TryParse(splitted(1), g)
            Integer.TryParse(splitted(2), b)
        End If
        Return Color.FromArgb(255, r, g, b)
    End Function

End Class
