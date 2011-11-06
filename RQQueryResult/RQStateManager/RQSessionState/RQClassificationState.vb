Imports Microsoft.VisualBasic


Namespace RQStateManager.RQSessionState

    Public Class RQClassificationState
        Inherits sm.cs

        Public Property ClassID() As Integer
            Get
                If Me._cid <> "" Then
                    Return CInt(Me._cid)
                Else
                    Return 1
                End If
            End Get
            Set(ByVal value As Integer)
                Me._cid = CStr(value)
            End Set
        End Property


        Public Sub New(ByVal ID As Object)
            MyBase.New(ID)
        End Sub

    End Class

End Namespace
