Imports Microsoft.VisualBasic


Namespace RQStateManager.RQSessionState

    Public Class RQNavigationState
        Inherits sm.ns

        Public Enum PageState
            undefined
            display
            edit
        End Enum


        Public Property PageStartIndex() As Integer
            Get
                Return Me._psi
            End Get
            Set(ByVal value As Integer)
                Me._psi = value
            End Set
        End Property


        Public Property HitCount() As Integer
            Get
                Return Me._hc
            End Get
            Set(ByVal value As Integer)
                Me._hc = value
            End Set
        End Property


        Public Property PageSize() As Integer
            Get
                Return Me._ps
            End Get
            Set(ByVal value As Integer)
                Me._ps = value
            End Set
        End Property


        Public Property ItemSelect() As Integer
            Get
                Return Me._is
            End Get
            Set(ByVal value As Integer)
                Me._is = value
            End Set
        End Property


        Public Property DocId() As String
            Get
                Return Me._did
            End Get
            Set(ByVal value As String)
                Me._did = value
            End Set
        End Property


        Public Property ClassName() As String
            Get
                Return Me._cln
            End Get
            Set(ByVal value As String)
                Me._cln = value
            End Set
        End Property


        Public Property PagesState() As PageState
            Get
                Select Case Me._pst
                    Case -1
                        Return PageState.undefined
                    Case 0
                        Return PageState.display
                    Case 1
                        Return PageState.edit
                    Case Else
                        Return PageState.undefined
                End Select
            End Get
            Set(ByVal value As PageState)
                Select Case value
                    Case PageState.undefined
                        Me._pst = -1
                    Case PageState.display
                        Me._pst = 0
                    Case PageState.edit
                        Me._pst = 1
                    Case Else
                        Me._pst = -1
                End Select
            End Set
        End Property


        Public Sub New(ByVal ID As String)
            MyBase.New(ID)
        End Sub

    End Class

End Namespace
