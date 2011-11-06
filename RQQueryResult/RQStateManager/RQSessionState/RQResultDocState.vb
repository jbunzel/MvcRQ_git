Imports Microsoft.VisualBasic


Namespace RQStateManager.RQSessionState

    Public Class RQResultDocState
        Inherits sm.rd

        Public Property ResultDoc() As System.Xml.XmlDocument
            Get
                Return Me._rd
            End Get
            Set(ByVal value As System.Xml.XmlDocument)
                Me._rd = value
            End Set
        End Property


        Public Property ResultRawDoc() As System.Xml.XmlDocument
            Get
                Return Me._rwd
            End Get
            Set(ByVal value As System.Xml.XmlDocument)
                Me._rwd = value
            End Set
        End Property


        Public Sub New(ByVal ID As Object)
            MyBase.New(ID)
        End Sub

    End Class

End Namespace
