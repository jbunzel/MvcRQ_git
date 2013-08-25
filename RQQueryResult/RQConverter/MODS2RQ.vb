Imports Microsoft.VisualBasic
Imports System.Xml
Imports System.Xml.Xsl


Namespace RQConverter

    Public Class MODS2RQ
        Inherits GenericConverter


        Public Sub New(ByRef xmlDoc As XmlDocument)
            MyBase.New()

            Try
                Dim transf As New XslCompiledTransform(True)

                transf.Load(HttpContext.Current.Server.MapPath("~/App_Code/VBCode/RQConverter/MODS2RQ/Mods2RQIntern.xslt"))
                transf.Transform(xmlDoc, Me)
            Catch ex As Exception
                'leave empty
            End Try
            Me.Flush()
            Me.BaseStream.Seek(0, IO.SeekOrigin.Begin)

        End Sub


        Public Overloads Shared Function Convert(ByVal from As XmlDocument) As XmlDocument
            Dim writer As New XmlTextWriter(New System.IO.MemoryStream, Encoding.UTF8)

            Try
                Dim retVal As New XmlDocument()
                Dim transf As New XslCompiledTransform(True)
 
                transf.Load(HttpContext.Current.Server.MapPath("~/App_Code/VBCode/RQConverter/MODS2RQ/Mods2RQIntern.xslt"))
                transf.Transform(from, writer)
                writer.Flush()
                writer.BaseStream.Seek(0, IO.SeekOrigin.Begin)
                retVal.Load(writer.BaseStream)
                Return retVal
            Catch ex As Exception
                'leave empty
                Return Nothing
            End Try
        End Function

    End Class

End Namespace
