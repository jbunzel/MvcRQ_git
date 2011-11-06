Imports Microsoft.VisualBasic

Namespace RQQueryResult.RQDescriptionElements

    Public Class RQDescriptionElement

#Region "Protected Members"

        Protected _changed As Boolean = False
        Protected _content As String = ""

#End Region


#Region "Public Properties"

        Public Property Changed() As Boolean
            Get
                Return _changed
            End Get
            Set(ByVal value As Boolean)
                _changed = value
            End Set
        End Property


        Public Property Content() As String
            Get
                Return _content
            End Get
            Set(ByVal value As String)
                _content = value
            End Set
        End Property

#End Region


#Region "Public Constructors"

        Public Sub New()
        End Sub


        Public Sub New(ByVal Content As String)
            _content = Content
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
