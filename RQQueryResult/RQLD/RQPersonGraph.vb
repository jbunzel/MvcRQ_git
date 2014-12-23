Imports System.Collections.Generic
Imports RQLib.RQKos.Persons
Imports RQLinkedData.LDCloud.KnowledgeOrganization.Persons

Namespace RQLD

    Public Class RQPersonGraph
        Inherits RQLDGraph
        Implements System.Xml.Serialization.IXmlSerializable

#Region "Public constructors"

        Public Sub New()
            'Needed because class is serializable. Only used for deserialization.
        End Sub


        Public Sub New(ByVal personDataSystem As Person.PersonDataSystems)
            Select Case (personDataSystem)
                Case Person.PersonDataSystems.gnd
                    Me._ldClient = New GndPersonDataSystemClient()
                Case Person.PersonDataSystems.rq
                    Me._ldClient = New RqPersonDataSystemClient()
                Case Else
                    Me._ldClient = New PersonDataSystemClient()
            End Select
        End Sub


        Public Sub New(ByVal thePerson As Person)
            Me.New(thePerson.PersonDataSystem)
            Me.Load(thePerson)
        End Sub

#End Region


#Region "Public Methods"

        Public Sub Load(ByVal thePerson As Person)
            Dim dict As New System.Collections.Specialized.StringDictionary()

            'dict.Add("ClassCode", theClass.ClassCode)
            'dict.Add("ClassLongTitle", theClass.ClassLongTitle)
            'dict.Add("ClassShortTitle", theClass.ClassShortTitle)
            'If Not IsNothing(theClass.GetBroaderClass) Then
            '    dict.Add("Broader", theClass.GetBroaderClass.ClassCode)
            'End If
            Me._ldClient.Load(dict)
        End Sub


        Public Sub Load(ByVal uri As String)
            Me._ldClient.Load(uri)
        End Sub


        Public Function GetUri(ByVal personDataSystem As Person.PersonDataSystems, ByVal personCode As String) As String
            Return PersonDataSystemClient.GetURI(personDataSystem, personCode)
        End Function


        Public Function GetPrefName(ByVal thePerson As Person) As String
            Return CType(Me._ldClient, GndPersonDataSystemClient).GetPreferredLabel(thePerson.PersonID)
        End Function


        Public Function GetPredicates(ByVal personID As String) As String()
            Return CType(Me._ldClient, GndPersonDataSystemClient).GetPredicates(personID)
        End Function


        Public Function GetPredicateObjects(ByVal personID As String) As Dictionary(Of String, String)
            Return CType(Me._ldClient, GndPersonDataSystemClient).GetPredicateObjects(personID)
        End Function


#End Region

    End Class

End Namespace