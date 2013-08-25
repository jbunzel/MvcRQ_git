Imports Microsoft.VisualBasic
Imports System.Xml
Imports System.Xml.Xsl
Imports System.Web
Imports RQLib.RQQueryResult

Namespace RQConverter

    Public Class RQ2DC
        Inherits GenericConverter

        Public Sub New(ByRef Item As RQResultItem)
            MyBase.New()
            Try
                Dim transf As New XslCompiledTransform(True)

                transf.Load(HttpContext.Current.Server.MapPath("~/xslt/rq2dc.xslt"))
                transf.Transform(Item.Serialize(False, False, True, False), Me)
            Catch ex As Exception
                'leave empty
            End Try
            Me.Flush()
            Me.BaseStream.Seek(0, IO.SeekOrigin.Begin)
        End Sub


        Public Sub New(ByRef resultSet As RQResultSet, Optional ByVal service As ServiceType = ServiceType.UNAPI, Optional ByVal fromRecord As Integer = 1, Optional ByVal maxRecords As Integer = 0)
            Try
                Dim transf As New XslCompiledTransform(True)
                Dim args As New XsltArgumentList()

                Select Case service
                    Case ServiceType.SRU
                        args.AddParam("interface", "", "SRU")
                    Case Else
                        args.AddParam("interface", "", "UNAPI")
                End Select
                transf.Load(HttpContext.Current.Server.MapPath("~/xslt/rq2dc.xslt"))
                transf.Transform(resultSet.Serialize(fromRecord, maxRecords, False, False, True, False), args, Me)
            Catch ex As Exception
                'leave empty
            End Try
            Me.Flush()
            Me.BaseStream.Seek(0, IO.SeekOrigin.Begin)
        End Sub


        Public Overloads Shared Function Convert(ByRef resultSet As RQResultSet, Optional ByVal service As ServiceType = ServiceType.UNAPI, Optional ByVal fromRecord As Integer = 1, Optional ByVal maxRecords As Integer = 0) As XmlDocument
            Dim writer As New XmlTextWriter(New System.IO.MemoryStream, System.Text.Encoding.UTF8)

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
                transf.Load(HttpContext.Current.Server.MapPath("~/xslt/rq2dc.xslt"))
                transf.Transform(resultSet.ConvertTo("", fromRecord, maxRecords), args, writer)
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
