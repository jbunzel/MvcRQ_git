Imports RQLib.RQKos.Classifications
Imports RQLinkedData.LDCloud.KnowledgeOrganization.Classifications

Namespace RQLD

    Public Class RQClassificationGraph
        Inherits RQLDGraph
        Implements System.Xml.Serialization.IXmlSerializable

        Public Sub New()
            'Needed because class is serializable. Only used for deserialization.
        End Sub


        Public Sub New(ByVal classSystem As SubjClass.ClassificationSystems)
            Select Case (classSystem)
                Case SubjClass.ClassificationSystems.rq
                    Me._ldClient = New RqClassificationSystemClient()
                Case SubjClass.ClassificationSystems.rvk
                    Me._ldClient = New RvkClassificationSystemClient()
                Case SubjClass.ClassificationSystems.jel
                    Me._ldClient = New JelClassificationSystemClient()
                Case SubjClass.ClassificationSystems.ddc
                    Me._ldClient = New DdcClassificationSystemClient()
                Case Else
                    Me._ldClient = New ClassificationSystemClient()
            End Select
        End Sub


        Public Sub New(ByVal theClass As SubjClass)
            Me.New(theClass.ClassificationSystem)
            Me.Load(theClass)
        End Sub


        Public Sub Load(ByVal theClass As SubjClass)
            Dim dict As New System.Collections.Specialized.StringDictionary()

            dict.Add("ClassCode", theClass.ClassCode)
            dict.Add("ClassLongTitle", theClass.ClassLongTitle)
            dict.Add("ClassShortTitle", theClass.ClassShortTitle)
            If Not IsNothing(theClass.GetBroaderClass) Then
                dict.Add("Broader", theClass.GetBroaderClass.ClassCode)
            End If
            Me._ldClient.Load(dict)
        End Sub


        Public Sub Load(ByVal uri As String)
            Me._ldClient.Load(uri)
        End Sub


        Public Function GetUri(ByVal classSystem As SubjClass.ClassificationSystems, ByVal classNotation As String) As String
            Return ClassificationSystemClient.GetURI(classSystem, classNotation)
        End Function


        Public Function GetPrefLabel(ByVal theClass As SubjClass) As String
            Return CType(Me._ldClient, ClassificationSystemClient).GetPreferredLabel(theClass.ClassCode)
        End Function

    End Class

End Namespace