Imports RQLib.RQKos.Classifications
Imports RQLinkedData.LDCloud
Imports RQLinkedData.LDCloud.KnowledgeOrganization.Classifications
Imports RQLib.RQQueryResult.RQDescriptionElements

Namespace RQLD

    Public Class RQLDGraph
        Implements System.Xml.Serialization.IXmlSerializable

        Protected _ldClient As LinkedDataClient = Nothing


        Public Sub New()
            'Needed because class is serializable. Only used for deserialization.
        End Sub


        Sub WriteXml(ByVal writer As Xml.XmlWriter) Implements Xml.Serialization.IXmlSerializable.WriteXml
            Me._ldClient.Write(writer)
        End Sub


        Sub ReadXml(ByVal reader As Xml.XmlReader) Implements Xml.Serialization.IXmlSerializable.ReadXml

        End Sub


        Function GetSchema() As Xml.Schema.XmlSchema Implements Xml.Serialization.IXmlSerializable.GetSchema
            Return Nothing
        End Function

    End Class

End Namespace