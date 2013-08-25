Imports RQLinkedData

Namespace RQLD

    Public Class RQLDBase
        Implements System.Xml.Serialization.IXmlSerializable

        Private _ldb As LDBase


        Public Sub New()
            MyBase.New()

            Me._ldb = New LDBase()
        End Sub


        Sub SetBaseUriAndNamespace(ByVal baseUriStr As String, ByVal baseNamespace As String)
            Me._ldb.BaseUri = New Uri(baseUriStr)
            Me._ldb.NamespaceMap.AddNamespace(baseNamespace, New Uri(baseUriStr + "/" + baseNamespace))
        End Sub


        Sub SetNamespace(ByVal name As String, ByVal uri As Uri)
            Me._ldb.NamespaceMap.AddNamespace(name, uri)
        End Sub


        Sub CreateRDFTriple(ByVal subj As String, ByVal pred As String, ByVal obj As String)

            Me._ldb.CreateTriple(subj, pred, obj)
        End Sub


        Sub WriteXml(ByVal writer As Xml.XmlWriter) Implements Xml.Serialization.IXmlSerializable.WriteXml
            Me._ldb.Write(writer)
        End Sub


        Sub ReadXml(ByVal reader As Xml.XmlReader) Implements Xml.Serialization.IXmlSerializable.ReadXml

        End Sub


        Function GetSchema() As Xml.Schema.XmlSchema Implements Xml.Serialization.IXmlSerializable.GetSchema
            Return Nothing
        End Function

    End Class

End Namespace
