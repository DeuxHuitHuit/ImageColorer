Imports System.Console
Imports System.Drawing.Imaging
Imports System.Drawing
Imports System.Runtime.InteropServices

Module Main

    Private outputFolder As String = "../../output/"
    Private file As String = "../../test.gif"

    Sub Main()
        parseArgs()

        WriteLine("Welcome in Deux Huit Huit's ImageColorer")
        WriteLine()
        WriteLine("File: {0}", file)
        WriteLine("Output: {0}", outputFolder)
        WriteLine()

        Dim fileInfo As IO.FileInfo = FileIO.FileSystem.GetFileInfo(file)

        If fileInfo IsNot Nothing AndAlso fileInfo.Exists Then
            ProcessFile(fileInfo)
        Else
            WriteLine("ERROR: File '{0}' does not exists. Can not continue.", fileInfo.FullName)
        End If
        WriteLine()
        WriteLine("Hit <Enter> to exit...")
        ReadLine()
    End Sub

    Private Sub parseArgs()
        For Each s As String In My.Application.CommandLineArgs
            Select Case s

                Case "-v"

                Case Else
                    If s.StartsWith("-f:") Then
                        file = s.Remove(0, 3)
                    ElseIf s.StartsWith("-o:") Then
                        outputFolder = s.Remove(0, 3)
                    Else
                        WriteLine("Argument '{0}' not valid.", s)
                    End If
            End Select
        Next
    End Sub

    Private Sub ProcessFile(ByVal fileInfo As IO.FileInfo)
        'Dim image As Imaging.ByteImage = Imaging.ByteImage.FromPath(fileInfo.FullName)


        Dim img As Drawing.Image = Drawing.Bitmap.FromFile(fileInfo.FullName)

        'getGifImage(CType(img, Bitmap))
        'DirectCast(img, Drawing.Bitmap).MakeTransparent()

        For r As Integer = 0 To 255
            For g As Integer = 0 To 0
                For b As Integer = 0 To 0
                    CreateNewImage(img, r, g, b)
                Next
            Next
        Next

        img.Dispose()
        img = Nothing
    End Sub

    Private Sub CreateNewImage(ByRef refImage As Drawing.Image, ByVal r As Integer, ByVal g As Integer, ByVal b As Integer)
        Dim newImage As New Drawing.Bitmap(refImage.Size.Width, refImage.Size.Height, PixelFormat.Format24bppRgb)
        newImage.MakeTransparent()
        Dim rect As New Drawing.Rectangle(0, 0, refImage.Width, refImage.Height)

        ' http://msdn.microsoft.com/en-us/library/5y289054.aspx
        Dim graph As Drawing.Graphics = Drawing.Graphics.FromImage(newImage)
        graph.InterpolationMode = Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
        graph.SmoothingMode = Drawing.Drawing2D.SmoothingMode.HighQuality
        graph.CompositingMode = Drawing.Drawing2D.CompositingMode.SourceCopy
        graph.CompositingQuality = Drawing.Drawing2D.CompositingQuality.HighQuality

        ' http://www.sellsbrothers.com/writing/dotnetimagerecoloring.htm
        Dim colorMap(0) As Drawing.Imaging.ColorMap
        colorMap(0) = New Drawing.Imaging.ColorMap
        colorMap(0).OldColor = Drawing.Color.FromArgb(255, 153, 153, 153)
        colorMap(0).NewColor = Drawing.Color.FromArgb(255, r, g, b)
        Dim attr As New Drawing.Imaging.ImageAttributes
        attr.SetRemapTable(colorMap)

        ' graph.Clear(Drawing.Color.Transparent)
        graph.DrawImage(refImage, rect, 0, 0, refImage.Width, refImage.Height, Drawing.GraphicsUnit.Pixel, attr)

        graph.Flush(Drawing.Drawing2D.FlushIntention.Flush)

        'newImage.MakeTransparent()

        'Dim newImage As Bitmap = CType(refImage.Clone, Bitmap)

        'Dim transparent As Color = DirectCast(newImage, Bitmap).GetPixel(0, 0)

        'createGifImage(newImage, refImage.Palette, transparent)

        ' change the palette
        'Dim palette As Drawing.Imaging.ColorPalette = refImage.Palette
        'Dim transparent As Color = DirectCast(refImage, Bitmap).GetPixel(0, 0)
        'For x As Integer = 0 To palette.Entries.Length - 1
        '    Dim color As Drawing.Color = palette.Entries(x)
        '    Dim alpha As Integer = 255
        '    If color = Drawing.Color.Transparent Then
        '        alpha = 0
        '    End If
        '    palette.Entries(x) = Drawing.Color.FromArgb(alpha, color.R, color.G, color.B)
        'Next
        'newImage.Palette = palette

        SaveNewImage(newImage, r, g, b)

        'graph.Dispose()
        'graph = Nothing
        newImage.Dispose()
        newImage = Nothing
    End Sub

    Private Sub createGifImage(ByRef _gifImage As Drawing.Bitmap, ByVal refPalette As ColorPalette, ByVal transparent As Color)
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
            If color.R = transparent.R AndAlso color.B = transparent.B AndAlso color.G = transparent.G Then
                alpha = 0
            End If
            ncp.Entries(x) = Drawing.Color.FromArgb(alpha, color.R, color.G, color.B)
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

    Private Sub SaveNewImage(ByRef newImage As Drawing.Bitmap, ByVal r As Integer, ByVal g As Integer, ByVal b As Integer)
        If Not FileIO.FileSystem.GetDirectoryInfo(outputFolder).Exists Then
            FileIO.FileSystem.CreateDirectory(outputFolder)
        End If
        Dim fileInfo As IO.FileInfo = FileIO.FileSystem.GetFileInfo(String.Format("{0}{1:000}-{2:000}-{3:000}.gif", outputFolder, r, g, b))

        If fileInfo.Exists Then
            fileInfo.Delete()
        End If

        newImage.Save(fileInfo.FullName, Drawing.Imaging.ImageFormat.Gif) ' newImage.RawFormat)

        WriteLine(" - File {0} as been created!", fileInfo.Name)
    End Sub
End Module
