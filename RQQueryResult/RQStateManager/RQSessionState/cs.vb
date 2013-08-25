Imports Microsoft.VisualBasic
Imports RQState.Components.Storage
Imports RQState.Components

Namespace sm

    Public Class cs
        Inherits StateObject(Of cs, SessionStorage(Of cs))

        Protected Friend _cid As String = ""    'ClassID (ID of selected classification cateogory)


        Public Sub New(ByVal Id As Object)
            MyBase.New()
            Me.ID = Id
        End Sub


        Public Sub Copy(ByVal from As cs)
            Me._cid = from._cid
        End Sub

    End Class

End Namespace
