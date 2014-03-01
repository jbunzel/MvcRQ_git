Imports Microsoft.VisualBasic
Imports RQLib.RQKos.Classifications
Imports System.Runtime.Serialization


Namespace RQQueryResult.RQDescriptionElements

    <DataContract()>
    <KnownType(GetType(SubjClass))> _
    Public Class RQClassification
        Inherits RQArrayDescriptionElement
        Implements Collections.IEnumerable

#Region "Private Members"

        Private _classEditMode As Boolean = False

#End Region


#Region "Public Properties"

        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public Overrides Property Content() As String
            Get
                Me.BuildContentString()
                Return _content
            End Get
            Set(ByVal value As String)
                Dim classString As New Utilities.ClassString(value)

                If Not Me._classEditMode Then value = classString.CompleteClassString()
                If (_content <> value) Then
                    _content = value
                    Me.BuildClassCodeArray()
                End If
            End Set
        End Property

#End Region


#Region "Protected Methods"

        Protected Overrides Function CreateNewComponent(ByVal contentComponent As String) As RQDescriptionComponent
            Return New SubjClass(contentComponent, True)
        End Function

#End Region


#Region "Public Constructors"

        Public Sub New()
            MyBase.New()
        End Sub

        'This constructor should only be used, if content is a completed (i. e. rqc class codes included) ClassificationFieldContent string.
        'Otherwise use New() contructor and set RQClassification.Content = Content.
        Public Sub New(ByVal Content As String, Optional ByVal EnableLinkedData As Boolean = False)
            MyBase.New(Content, EnableLinkedData)
        End Sub

#End Region


#Region "Public Override Methods"

        Public Overrides Function SyntaxCheck(ByRef message As String) As Boolean
            Return True
        End Function

#End Region

#Region "Public Methods"

        Public Sub ClassEditMode(ByVal switch As Boolean)
            Me._classEditMode = switch
        End Sub

#End Region

    End Class

End Namespace
