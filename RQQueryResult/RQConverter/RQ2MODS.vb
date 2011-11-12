Imports Microsoft.VisualBasic
Imports System.Xml
Imports System.Xml.Xsl
Imports Modules.RQQueryResult

Namespace RQConverter

    Public Class RQ2MODS
        Inherits GenericConverter

        Public Enum ServiceType
            UNAPI
            SRU
        End Enum


        Public Sub New(ByRef Item As RQResultItem)
            MyBase.New()

            Try
                Dim transf As New XslCompiledTransform(True)
                Dim writer As New XmlTextWriter(New System.IO.MemoryStream, Encoding.UTF8)

                transf.Load(HttpContext.Current.Server.MapPath("~/App_Code/VBCode/RQConverter/rq2dc.xslt"))
                transf.Transform(Item.ConvertTo(""), writer)
                transf.Load(HttpContext.Current.Server.MapPath("~/App_Code/VBCode/RQConverter/DC2MODS/simpleDC2MODS.xsl"))
                writer.BaseStream.Seek(0, IO.SeekOrigin.Begin)
                transf.Transform(New XmlTextReader(writer.BaseStream), Me)
            Catch ex As Exception
                'leave empty
            End Try
            Me.Flush()
            Me.BaseStream.Seek(0, IO.SeekOrigin.Begin)
        End Sub


        Public Sub New(ByRef resultSet As RQResultSet, Optional ByVal service As ServiceType = ServiceType.UNAPI, Optional ByVal fromRecord As Integer = 1, Optional ByVal maxRecords As Integer = 0)
            MyBase.New()

            Try
                Dim transf As New XslCompiledTransform(True)
                Dim writer As New XmlTextWriter(New System.IO.MemoryStream, Encoding.UTF8)
                Dim args As New XsltArgumentList()

                Select Case service
                    Case ServiceType.SRU
                        args.AddParam("interface", "", "SRU")
                    Case Else
                        args.AddParam("interface", "", "UNAPI")
                End Select
                transf.Load(HttpContext.Current.Server.MapPath("~/App_Code/VBCode/RQConverter/rq2dc.xslt"))
                transf.Transform(resultSet.ConvertTo("", fromRecord, maxRecords), writer)
                transf.Load(HttpContext.Current.Server.MapPath("~/App_Code/VBCode/RQConverter/DC2MODS/simpleDC2MODS.xsl"))
                writer.BaseStream.Seek(0, IO.SeekOrigin.Begin)
                transf.Transform(New XmlTextReader(writer.BaseStream), args, Me)
            Catch ex As Exception
                'leave empty
            End Try
            Me.Flush()
            Me.BaseStream.Seek(0, IO.SeekOrigin.Begin)
        End Sub


        Public Overloads Shared Function Convert(ByRef resultSet As RQResultSet, Optional ByVal service As ServiceType = ServiceType.UNAPI, Optional ByVal fromRecord As Integer = 1, Optional ByVal maxRecords As Integer = 0) As XmlDocument
            Dim writer As New XmlTextWriter(New System.IO.MemoryStream, Encoding.UTF8)

            Try
                Dim retVal As New XmlDocument()
                Dim transf As New XslCompiledTransform(True)
                Dim args As New XsltArgumentList()

                Select Case service
                    Case ServiceType.SRU
                        args.AddParam("interface", "", "SRU")
                    Case Else
                        args.AddParam("interface", "", "UNAPI")
                End Select
                transf.Load(HttpContext.Current.Server.MapPath("~/App_Code/VBCode/RQConverter/rq2dc.xslt"))
                transf.Transform(resultSet.ConvertTo("", fromRecord, maxRecords), writer)
                transf.Load(HttpContext.Current.Server.MapPath("~/App_Code/VBCode/RQConverter/DC2MODS/simpleDC2MODS.xsl"))
                writer.BaseStream.Seek(0, IO.SeekOrigin.Begin)
                transf.Transform(New XmlTextReader(writer.BaseStream), args, writer)
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
