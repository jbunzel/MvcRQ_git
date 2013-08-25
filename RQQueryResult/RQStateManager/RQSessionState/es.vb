Imports Microsoft.VisualBasic
Imports RQState.Components.Storage
Imports RQState.Components

Namespace sm

    Public Class es
        Inherits StateObject(Of es, SessionStorage(Of es))

        Protected Friend _eis As Integer = -1       'EditItemSelect
        Protected Friend _ehc As Integer = 0        'EditHitCount
        Protected Friend _eat As Integer = -1       'EditActiontype
        Protected Friend _rta As String = ""        'ReturnAdress
        Protected Friend _aeo As Object = Nothing   'ActiveEditObject


        Public Sub New(ByVal Id As Object)
            MyBase.New()
            Me.ID = Id
        End Sub


        Public Sub Copy(ByVal from As es)
            Me._eis = from._eis
            Me._ehc = from._ehc
        End Sub

    End Class

End Namespace
