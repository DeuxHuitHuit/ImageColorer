Imports System.Web
Imports System.Drawing
Imports NTR.API.Imaging

Namespace Handlers
    Public Class GifHandler : Inherits NTR.API.HttpHandlers.ImageHttpHandler

        Public Overrides Sub ProcessRequest(ByVal context As HttpContext)

            context.SkipAuthorization = True

            Dim colorInput As String = context.Request("color")
            Dim victimInput As String = context.Request("victim")
            Dim imageInput As String = context.Request("image")

            Dim cacheKey As String = String.Format("{0}-{1}", colorInput, imageInput)
            Dim cacheKeyC As String = cacheKey + "-c"

            If Not String.IsNullOrWhiteSpace(colorInput) AndAlso Not String.IsNullOrWhiteSpace(imageInput) AndAlso _
                Not imageInput.Contains("\") AndAlso Not imageInput.Contains("/") Then ' prevent path traversal

                Dim fullPath As String = context.Server.MapPath("~/refs/" & imageInput & ".gif")
                Dim victimColor As Color = Core.GifImage.ParseColor(victimInput)
                Dim newColor As Color = Core.GifImage.ParseColor(colorInput)

                If newColor <> Nothing AndAlso FileIO.FileSystem.FileExists(fullPath) Then

                    If victimColor = Nothing Then
                        victimColor = Color.Black
                    End If

                    ' Try cache
                    'Dim o As Object = context.Cache(cacheKey)
                    'Dim oc As Object = context.Cache(cacheKeyC)
                    'If o IsNot Nothing AndAlso oc IsNot Nothing Then

                    '    Dim b As Byte() = TryCast(o, Byte())
                    '    Dim c As String = TryCast(oc, String)

                    '    SetCacheInfos(context)
                    '    SendImage(context, b, c)

                    '    Exit Sub
                    'End If

                    Try
                        Dim imgObj As Image = Image.FromFile(fullPath)

                        ' Replace victim colors
                        Core.GifImage.ConverToGifImageWithNewColor(imgObj, imgObj.Palette, victimColor, newColor)

                        ' Create ByteImage
                        Dim img As ByteImage = ByteImage.FromImage(imgObj, Imaging.ImageFormat.Gif)
                        Dim contentType As String = "gif"

                        ' Set client cache infos
                        SetCacheInfos(context)

                        ' Send image
                        SendImage(context, img.GetBuffer, contentType)

                        ' Add to Server cache
                        'Dim st As TimeSpan = TimeSpan.FromHours(1)
                        'context.Cache.Insert(cacheKey, img.GetBuffer, Nothing, Date.MaxValue, st)
                        'context.Cache.Insert(cacheKeyC, contentType, Nothing, Date.MaxValue, st)

                        ' Clear pointer
                        imgObj.Dispose()
                        imgObj = Nothing
                        img = Nothing

                        ' Stop all processing here: we're done
                        Exit Sub

                    Catch ex As Exception
                        ' - rien faire
                        Throw ' Debug purpose
                    End Try
                End If
            End If

            ' Image non trouvée
            context.Response.StatusCode = 404
            context.Response.Flush()
        End Sub

        Private Shared Sub SetCacheInfos(ByVal context As HttpContext)
            context.Response.Cache.SetCacheability(HttpCacheability.Public)
            context.Response.Cache.SetAllowResponseInBrowserHistory(True)
            context.Response.Cache.SetExpires(Now.AddYears(1))
        End Sub
    End Class
End Namespace
