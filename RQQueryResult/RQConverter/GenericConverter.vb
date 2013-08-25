Imports Microsoft.VisualBasic
Imports System.Xml
Imports RQLib.RQQueryResult


Namespace RQConverter

    Public Enum BibliographicFormats
        mods
        oai_dc
        srw_dc
        info_ofi
        pubmed
        rq
        unknown
    End Enum


    Public Enum ServiceType
        UNAPI
        SRU
    End Enum


    Public Class formatDescriptor

        Private _URN As String
        Private _Name As String
        Private _Type As String
        Private _Priority As Integer


        Public Property URN() As String
            Get
                Return _URN
            End Get
            Set(ByVal value As String)
                _URN = value
            End Set
        End Property


        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property


        Public Property Type() As String
            Get
                Return _Type
            End Get
            Set(ByVal value As String)
                _Type = value
            End Set
        End Property


        Public Property Priority() As Integer
            Get
                Return _Priority
            End Get
            Set(ByVal value As Integer)
                _Priority = value
            End Set
        End Property


        Public Sub New()

        End Sub


        'Public Sub New(ByVal format As RQWebAccess.RQWebClients.formatDescriptor)
        '    Me._Name = format.Name
        '    Me._Priority = format.Priority
        '    Me._Type = format.Type
        '    Me._URN = format.URN
        'End Sub


        'Public Function ConvertTo() As RQWebAccess.RQWebClients.formatDescriptor
        '    Dim CT As New RQWebAccess.RQWebClients.formatDescriptor()

        '    CT.Name = Me._Name
        '    CT.Priority = Me._Priority
        '    CT.Type = Me._Type
        '    CT.URN = Me._URN
        '    Return CT
        'End Function


        'Public Sub ConvertTo(ByRef format As RQWebAccess.RQWebClients.formatDescriptor)
        '    format.Name = Me._Name
        '    format.Priority = Me._Priority
        '    format.Type = Me._Type
        '    format.URN = Me._URN
        'End Sub

    End Class


    Public Class GenericConverter
        Inherits XmlTextWriter

        'Public Enum ServiceType
        '    UNAPI
        '    SRU
        'End Enum


        Public Sub New()
            MyBase.New(New System.IO.MemoryStream, System.Text.Encoding.UTF8)

        End Sub


        Public Shared Function Convert(ByVal from As XmlDocument) As XmlDocument
            Return Nothing
        End Function


        'Public Shared Function Convert(ByVal resultItem As RQResultItem, ByVal format As String, Optional ByVal service As ServiceType = ServiceType.UNAPI) As XmlTextReader
        '    Return New RQ2DC(resultItem).GetReader()
        'End Function


        Public Function GetReader() As XmlReader
            Me.BaseStream.Seek(0, IO.SeekOrigin.Begin)
            GetReader = New XmlTextReader(Me.BaseStream)
        End Function

    End Class

End Namespace
