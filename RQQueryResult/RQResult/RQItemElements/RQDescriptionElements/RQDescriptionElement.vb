Imports Microsoft.VisualBasic
Imports System.Xml
Imports System.Runtime.Serialization
Imports RQLib.RQKos.Classifications

Namespace RQQueryResult.RQDescriptionElements

    <DataContract()> _
    <KnownType(GetType(RQClassification))> _
    Public Class RQDescriptionElement

#Region "Private Members"

        Protected _changed As Boolean = False
        Protected _content As String = ""
        Protected _ldEnabled As Boolean = False

#End Region


#Region "Public Properties"

        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public Property Changed() As Boolean
            Get
                Return _changed
            End Get
            Set(ByVal value As Boolean)
                _changed = value
            End Set
        End Property


        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public Overridable Property Content() As String
            Get
                Return _content
            End Get
            Set(ByVal value As String)
                _content = value
            End Set
        End Property


        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public Property IsLinkedDataEnabled As Boolean
            Get
                Return Me._ldEnabled
            End Get
            Set(ByVal value As Boolean)
                Me._ldEnabled = value
            End Set
        End Property

#End Region


#Region "Public Constructors"

        Public Sub New()
        End Sub


        Public Sub New(ByVal Content As String, Optional ByVal EnableLinkedData As Boolean = False)
            If (Not IsNothing(Content)) Then Me._content = Content.Trim()
            Me._ldEnabled = EnableLinkedData
        End Sub

#End Region


#Region "Public Methods"

        Public Overridable Function SyntaxCheck(ByRef message As String) As Boolean
            Return True
        End Function


        Public Overrides Function ToString() As String
            Return Me.Content
        End Function

#End Region

    End Class

End Namespace
