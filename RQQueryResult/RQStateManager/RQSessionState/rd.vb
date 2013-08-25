Imports Microsoft.VisualBasic
Imports RQState.Components.Storage
Imports RQState.Components

Namespace sm

    Public Class rd
        Inherits StateObject(Of rd, SessionStorage(Of rd))

        Protected Friend _rd As System.Xml.XmlDocument = Nothing  'xmlResultDoc (transformed result document of last query)
        Protected Friend _rwd As System.Xml.XmlDocument = Nothing 'xmlRawDoc (untransfromed result document of last query)


        Public Sub New(ByVal Id As Object)
            MyBase.New()
            Me.ID = Id
        End Sub


        Public Sub Copy(ByVal from As rd)
            Me._rd = from._rd
            Me._rwd = from._rwd
        End Sub

    End Class

End Namespace
