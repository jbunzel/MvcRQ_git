Imports Microsoft.VisualBasic
Imports RQLib.RQKos.Persons
Imports System.Runtime.Serialization


Namespace RQQueryResult.RQDescriptionElements

    <DataContract()>
    <KnownType(GetType(Person))> _
    Public Class RQAuthors
        Inherits RQArrayDescriptionElement
        Implements Collections.IEnumerable

#Region "Public Properties"

        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public Overrides Property Content() As String
            Get
                Me.BuildContentString()
                Return _content
            End Get
            Set(ByVal value As String)
                If (_content <> value) Then
                    _content = value
                    Me.BuildClassCodeArray()
                End If
            End Set
        End Property

#End Region


#Region "Protected Methods"

        Protected Overrides Function CreateNewComponent(ByVal contentComponent As String) As RQDescriptionComponent
            Return New Person(contentComponent, True)
        End Function

#End Region


#Region "Public Constructors"

        Public Sub New()
            MyBase.New()
        End Sub


        Public Sub New(ByVal Content As String, Optional ByVal EnableLinkedData As Boolean = False)
            MyBase.New(Content, EnableLinkedData)
        End Sub

#End Region


#Region "Public Methods"

        Public Overrides Function SyntaxCheck(ByRef message As String) As Boolean
            Return True
        End Function

#End Region

    End Class

End Namespace
