Imports Microsoft.VisualBasic


Namespace RQStateManager.RQSessionState

    Public Class RQEditState
        Inherits sm.es

        Public Enum EditActionTypes
            undefined
            import
            copy
            edit
            add
        End Enum


        Public Property EditHitCount() As Integer
            Get
                Return Me._ehc
            End Get
            Set(ByVal value As Integer)
                Me._ehc = value
            End Set
        End Property


        Public Property EditItemSelect() As Integer
            Get
                Return Me._eis
            End Get
            Set(ByVal value As Integer)
                Me._eis = value
            End Set
        End Property


        Public Property EditActionType() As EditActionTypes
            Get
                Select Case Me._eat
                    Case 0
                        Return EditActionTypes.import
                    Case 1
                        Return EditActionTypes.copy
                    Case 2
                        Return EditActionTypes.edit
                    Case 3
                        Return EditActionTypes.add
                    Case Else
                        Return EditActionTypes.undefined
                End Select
            End Get
            Set(ByVal value As EditActionTypes)
                Select Case value
                    Case EditActionTypes.import
                        Me._eat = 0
                    Case EditActionTypes.copy
                        Me._eat = 1
                    Case EditActionTypes.edit
                        Me._eat = 2
                    Case EditActionTypes.add
                        Me._eat = 3
                    Case Else
                        Me._eat = -1
                End Select
            End Set
        End Property


        Public Property ActiveEditObject() As Object
            Get
                Return Me._aeo
            End Get
            Set(ByVal value As Object)
                Me._aeo = value
            End Set
        End Property


        Public Property ReturnAdress() As String
            Get
                Return Me._rta
            End Get
            Set(ByVal value As String)
                Me._rta = value
            End Set
        End Property


        Public Sub New(ByVal ID As Object)
            MyBase.New(ID)
        End Sub

    End Class

End Namespace
