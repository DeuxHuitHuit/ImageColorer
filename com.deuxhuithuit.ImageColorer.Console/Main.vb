Imports System.Console
Imports System.Drawing.Imaging
Imports System.Drawing
Imports System.Runtime.InteropServices

Module Main

    Private Const HEX_COLOR_FORMAT_32 As String = "{0}{1:X2}{2:X2}{3:X2}.{4}" ' color is 16 bits
    Private Const HEX_COLOR_FORMAT_16 As String = "{0}{1:X1}{2:X1}{3:X1}.{4}" ' color is 8 bits
    Private Const RGB_TEXT_COLOR_FORMAT As String = "{0}rgb({1},{2},{3}).{4}" ' color is always 16 bits
    Private Const RGB_FIXED_COLOR_FORMAT As String = "{0}{1:000}{2:000}{3:000}).{4}" ' 16 bits here too

    Private Const COLOR_FORMAT As Integer = 16 ' 16 (X10) | 256 (X100)
    'Private Const COLOR_DEPTH As Byte = 16 ' 8, 16, 24 beware! 1111 1111 / 1111 1111 / 1111 1111

    Private outputFolder As String = "../../output/"
    Private file As String = "../../test.gif"
    Private victim As Color
    Private colorFormat As String = HEX_COLOR_FORMAT_16
    Private stepper As Integer = 256 \ COLOR_FORMAT

    Sub Main()
        parseArgs()

        WriteLine("Welcome in Deux Huit Huit's ImageColorer")
        WriteLine()
        WriteLine("File: {0}", file)
        WriteLine("Output: {0}", outputFolder)
        WriteLine("Filename format {0}", colorFormat)
        WriteLine()
        WriteLine("Color format: {0} bits", COLOR_FORMAT)
        WriteLine("Victim {0}", victim)
        WriteLine()
        Threading.Thread.Sleep(1000)
        Write(" -> 3 -> ")
        Threading.Thread.Sleep(1000)
        Write("2 -> ")
        Threading.Thread.Sleep(1000)
        Write("1 -> ")
        Threading.Thread.Sleep(1000)
        WriteLine(" GO!")
        WriteLine()

        Dim start As Date = Now

        Dim fileInfo As IO.FileInfo = FileIO.FileSystem.GetFileInfo(file)

        If fileInfo IsNot Nothing AndAlso fileInfo.Exists Then
            ProcessFile(fileInfo)
        Else
            WriteLine("ERROR: File '{0}' does not exists. Can not continue.", fileInfo.FullName)
        End If
        WriteLine()
        WriteLine("Took {0:0.000} minutes to create {1} images", (Now - start).TotalMinutes, (COLOR_FORMAT ^ 3))
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
                    ElseIf s.StartsWith("-c:") Then
                        victim = parseVictimColor(s.Remove(0, 3))
                    Else
                        WriteLine("Argument '{0}' not valid.", s)
                    End If
            End Select
        Next
    End Sub

    Private Function parseVictimColor(ByVal s As String) As Color
        Dim r As Integer = 255
        Dim g As Integer = 255
        Dim b As Integer = 255
        Dim splitted() As String = s.Split(","c)
        If splitted.Length <> 3 Then
            If s.Length = 6 Then
                Integer.TryParse(s.Substring(0, 2), r)
                Integer.TryParse(s.Substring(2, 4), g)
                Integer.TryParse(s.Substring(4, 6), b)
            End If
        Else
            Integer.TryParse(splitted(0), r)
            Integer.TryParse(splitted(1), g)
            Integer.TryParse(splitted(2), b)
        End If
        Return Color.FromArgb(255, r, g, b)
    End Function

    Private Sub ProcessFile(ByVal fileInfo As IO.FileInfo)
        'Dim image As Imaging.ByteImage = Imaging.ByteImage.FromPath(fileInfo.FullName)


        Dim img As Drawing.Image = Drawing.Bitmap.FromFile(fileInfo.FullName)

        'getGifImage(CType(img, Bitmap))
        'DirectCast(img, Drawing.Bitmap).MakeTransparent()

        For r As Integer = 0 To 255 Step stepper
            For g As Integer = 0 To 255 Step stepper
                For b As Integer = 0 To 255 Step stepper
                    CreateNewImage(img, r, g, b)
                Next b
            Next g
        Next r

        img.Dispose()
        img = Nothing
    End Sub

    Private Sub CreateNewImage(ByRef refImage As Drawing.Image, ByVal r As Integer, ByVal g As Integer, ByVal b As Integer)
        'Dim newImage As New Drawing.Bitmap(refImage.Size.Width, refImage.Size.Height, PixelFormat.Format24bppRgb)
        'newImage.MakeTransparent()
        'Dim rect As New Drawing.Rectangle(0, 0, refImage.Width, refImage.Height)

        '' http://msdn.microsoft.com/en-us/library/5y289054.aspx
        'Dim graph As Drawing.Graphics = Drawing.Graphics.FromImage(newImage)
        'graph.InterpolationMode = Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
        'graph.SmoothingMode = Drawing.Drawing2D.SmoothingMode.HighQuality
        'graph.CompositingMode = Drawing.Drawing2D.CompositingMode.SourceCopy
        'graph.CompositingQuality = Drawing.Drawing2D.CompositingQuality.HighQuality

        '' http://www.sellsbrothers.com/writing/dotnetimagerecoloring.htm
        'Dim colorMap(0) As Drawing.Imaging.ColorMap
        'colorMap(0) = New Drawing.Imaging.ColorMap
        'colorMap(0).OldColor = Drawing.Color.FromArgb(255, 153, 153, 153)
        'colorMap(0).NewColor = Drawing.Color.FromArgb(255, r, g, b)
        'Dim attr As New Drawing.Imaging.ImageAttributes
        'attr.SetRemapTable(colorMap)

        '' graph.Clear(Drawing.Color.Transparent)
        'graph.DrawImage(refImage, rect, 0, 0, refImage.Width, refImage.Height, Drawing.GraphicsUnit.Pixel, attr)

        'graph.Flush(Drawing.Drawing2D.FlushIntention.Flush)

        'newImage.MakeTransparent()

        Dim newImage As Bitmap = CType(refImage.Clone, Bitmap)

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

        createGifImage(newImage, refImage.Palette, r, g, b)

        SaveNewImage(newImage, r, g, b)

        'graph.Dispose()
        'graph = Nothing
        newImage.Dispose()
        newImage = Nothing
    End Sub

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

    Private Function sd(ByVal n As Integer) As Integer
        If n = 0 Then
            Return 0
        End If
        Return n \ stepper
    End Function

    Private Sub SaveNewImage(ByRef newImage As Drawing.Bitmap, ByVal r As Integer, ByVal g As Integer, ByVal b As Integer)
        If Not FileIO.FileSystem.GetDirectoryInfo(outputFolder).Exists Then
            FileIO.FileSystem.CreateDirectory(outputFolder)
        End If
        Dim fileInfo As IO.FileInfo = FileIO.FileSystem.GetFileInfo(String.Format(colorFormat, outputFolder, sd(r), sd(g), sd(b), "gif"))

        If fileInfo.Exists Then
            fileInfo.Delete()
        End If

        newImage.Save(fileInfo.FullName, Drawing.Imaging.ImageFormat.Gif) ' newImage.RawFormat)

        WriteLine(" - File {0} as been created!", fileInfo.Name)
    End Sub
End Module
