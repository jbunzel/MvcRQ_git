Imports RQLib.RQLD
Imports RQLib.RQQueryResult.RQDescriptionElements

Namespace RQKos.Persons

    Public Class LDPersonDataClient
        Inherits PersonDataClient

        Public Shared Function GetPersonDataSystem(ByVal classId As String) As Person.PersonDataSystems
            Return Person.PersonDataSystems.gnd
        End Function


        Public Sub New(ByVal personDataSystem As Person.PersonDataSystems)
            Me.PersonDataSystem = personDataSystem
            Me.LDGraph = New RQPersonGraph(Me.PersonDataSystem)
        End Sub


        Public Overrides Function GetPersonId(ByVal personCode As String) As String
            Return CType(Me.LDGraph, RQPersonGraph).GetUri(Me._personDataSystem, personCode)
        End Function


        Public Overrides Sub GetPersonData(ByRef thePerson As Person)
            Dim uri As String = thePerson.PersonID

            If Me.IsLinkedDataEnabled() Then
                CType(Me.LDGraph, RQPersonGraph).Load(uri)
                'thePerson.PersonName = CType(Me.LDGraph, RQPersonGraph).GetPrefName(thePerson)
            End If
        End Sub


        Public Overrides Sub PutPersonData(ByRef thePerson As Person)
            'Dim mqQuery As New RQDAL.RQCatalogDAL
            'Dim drRow As RQDataSet.SystematikRow

            'drRow = CType(mqQuery.GetRecordByID(CStr(theClass.ClassID), "RQDataSet", "Systematik", True), RQDataSet.SystematikRow)
            'drRow.ParentID = theClass.ParentClassID
            'drRow.DDCNumber = theClass.ClassCode
            'drRow.Description = theClass.ClassShortTitle
            'drRow.RegensburgDesc = theClass.ClassLongTitle
            'drRow.RegensburgSign = theClass.RefRVKSet
            'drRow.DocRefCount = theClass.NrOfClassDocs
            'drRow.SubClassCount = theClass.NrOfSubClasses
            'drRow.DirRefCount = theClass.NrOfRefLinks
        End Sub


        Public Overrides Function UpdateDocRefs(ByRef thePerson As Person) As Boolean
            Return False
        End Function

    End Class

End Namespace
