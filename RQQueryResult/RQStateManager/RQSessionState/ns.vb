Imports Microsoft.VisualBasic
Imports RQState.Components.Storage
Imports RQState.Components

Namespace sm

    Public Class ns
        Inherits StateObject(Of ns, SessionStorage(Of ns))

        Protected Friend _psi As Integer = -1   'PageStartIndex (offset of first document at displayed page)
        Protected Friend _hc As Integer = 0     'HitCount
        Protected Friend _ps As Integer = -1    'pageSize
        Protected Friend _is As Integer = -1    'ItemSelect (index of displayed item)
        Protected Friend _did As String = ""    'DocId (document id of displayed item)
        Protected Friend _cln As String = ""    'ClsName (class name of displayed section)
        Protected Friend _pst As Integer = -1   'Page Status: 0=Display 1=Edit -1=undefined


        Public Sub New(ByVal Id As Object)
            MyBase.New()
            Me.ID = Id
        End Sub


        Public Sub Copy(ByVal from As ns)
            Me._psi = from._psi
            Me._hc = from._hc
            Me._ps = from._ps
            Me._is = from._is
            Me._did = from._did
            Me._cln = from._cln
        End Sub

    End Class

End Namespace
