Imports System.Web
Imports NTR.API.Imaging

Namespace Handlers
    Public Class GifHandler : Inherits NTR.API.HttpHandlers.ImageHttpHandler

        Public Overrides Sub ProcessRequest(ByVal context As HttpContext)

            context.SkipAuthorization = True

            Dim colorIntput As String = context.Request("color")
            Dim image As String = context.Request("image")

            Dim cacheKey As String = String.Format("{0}-{1}", colorIntput, image)
            Dim cacheKeyC As String = cacheKey + "-c"

            If True Then ' IsNumeric(lid) OrElse IsNumeric(id) Then

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

                    Dim img As New ByteImage()
                    Dim contentType As String = "image/gif"

                    ' Apply transforms
                    'img.ApplyTransformations()

                    ' Set client cache infos
                    SetCacheInfos(context)

                    ' Send image
                    SendImage(context, img.GetBuffer, "image/gif")

                    ' Add to Server cache
                    Dim st As TimeSpan = TimeSpan.FromHours(1)
                    context.Cache.Insert(cacheKey, img.GetBuffer, Nothing, Date.MaxValue, st)
                    context.Cache.Insert(cacheKeyC, contentType, Nothing, Date.MaxValue, st)

                    ' Clear pointer
                    img = Nothing


                    ' Stop all processing here: we're done
                    Exit Sub

                Catch ex As Exception
                    ' - rien faire
                    Throw ex ' Debug purpose
                End Try
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
