Imports Microsoft.VisualBasic
Imports RQState.Components.Storage
Imports RQState.Components

Namespace sm

    <Xml.Serialization.XmlInclude(GetType(RQLib.RQStateManager.RQSessionState.RQQueryState))> _
    Public Class qs
        Inherits StateObject(Of qs, ConfigurableStorage(Of qs))

        Protected Friend _qry As RQQueryForm.RQquery = Nothing    'Query
        Protected Friend _dqf As Boolean = True                         'Default query field selection
        Protected Friend _qfb As Boolean = False                        'Query Field dropdownbox visible
        Protected Friend _ecb As Integer = -1                           'External catalog box visible


        Public Sub New()

        End Sub


        Public Sub New(ByVal Id As Object)
            MyBase.New()
            Me.ID = Id
        End Sub


        Public Sub Copy(ByVal from As qs)
            Me._qry = from._qry
            Me._dqf = from._dqf
        End Sub

    End Class

End Namespace
