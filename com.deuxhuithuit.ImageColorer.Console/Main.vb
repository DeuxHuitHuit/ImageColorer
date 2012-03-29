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



    Private Sub ProcessFile(ByVal fileInfo As IO.FileInfo)
        Dim img As Drawing.Image = Drawing.Bitmap.FromFile(fileInfo.FullName)

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
        Dim newImage As Bitmap = CType(refImage.Clone, Bitmap)


        createGifImage(newImage, refImage.Palette, r, g, b)

        SaveNewImage(newImage, r, g, b)

        newImage.Dispose()
        newImage = Nothing
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
