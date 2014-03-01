Imports RQLib.RQLD
Imports RQLib.RQQueryResult.RQDescriptionElements

Namespace RQKos.Classifications

    Public Class LDClassificationDataClient
        Inherits ClassificationDataClient

        Public Shared Function GetClassificationSystem(ByVal classId As String) As SubjClass.ClassificationSystems
            Return SubjClass.ClassificationSystems.rvk
        End Function


        Public Sub New(ByVal classSystem As SubjClass.ClassificationSystems)
            Me.ClassSystem = classSystem
            Me.LDGraph = New RQClassificationGraph(Me.ClassSystem)
        End Sub


        Public Overrides Function GetClassId(ByVal classNotation As String) As String
            Return CType(Me.LDGraph, RQClassificationGraph).GetUri(Me._classSystem, classNotation)
        End Function


        Public Overrides Sub GetClassData(ByRef theClass As SubjClass) 'SubjClass)
            Dim uri As String = theClass.ClassID

            If Me.IsLinkedDataEnabled() Then
                CType(Me.LDGraph, RQClassificationGraph).Load(uri)
                theClass.ClassShortTitle = CType(Me.LDGraph, RQClassificationGraph).GetPrefLabel(theClass)
                theClass.SetComplete()
            End If
        End Sub


        Public Overrides Function PutClassData(ByRef theClass As SubjClass) As Boolean
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
            Return False
        End Function


        Public Overrides Sub GetNarrowerClassData(ByVal majClassId As Integer, ByRef classBranch As SubjClassBranch)

        End Sub


        Public Overrides Sub GetNarrowerClassData(ByVal majClassId As String, ByRef classBranch As SubjClassBranch)

        End Sub


        Public Overrides Function UpdateDocRefs(ByRef classBranch As SubjClassBranch, ByRef iSuperClassDocCount As Integer, ByRef iSuperClassRefCount As Integer) As Boolean
            Return False
        End Function


        Public Overrides Function Update(ByRef classBranch As SubjClassBranch) As Boolean
            Return False
        End Function


        'Public Shared Function WriteRDFGraph(ByVal theClassification As SubjClass) As RQSkosGraph
        '    Return New RQSkosGraph(theClassification)
        'End Function


        'Public Shared Sub ReadRDFGraph(ByVal theGraph As RQSkosGraph, ByRef theClassification As SubjClass)

        'End Sub


        'Public Function GetAltLabel(ByVal classSystem As SubjClass.ClassificationSystems, ByVal classNotation As String) As String
        '    'string uri = ClassificationSystemClient.GetURI(classSystem);

        '    'If (classNotation.StartsWith(uri)) Then
        '    'return this._ldbase.ObjectOf(classNotation, ClassificationSystemClient.GetPredicate(classSystem, ClassificationSystemClient.ClassificationPredicates.alternative_label))[0];
        '    'Else
        '    'return this._ldbase.ObjectOf(uri + "/" + ClassificationSystemClient.AdaptClassNotation(classSystem, classNotation), ClassificationSystemClient.GetPredicate(classSystem, ClassificationSystemClient.ClassificationPredicates.alternative_label))[0];
        '    Return Nothing
        'End Function


        'Public Function GetPrefLabel(ByVal classSystem As SubjClass.ClassificationSystems, ByVal classNotation As String) As String
        '    Dim uri As String = ClassificationSystemClient.GetURI(classSystem)

        '    If (classNotation.StartsWith(uri)) Then
        '        'Return Me._ldbase.ObjectOf(classNotation, ClassificationSystemClient.GetPredicate(classSystem, ClassificationSystemClient.ClassificationPredicates.preferred_label))(0)
        '    Else
        '        'Return Me._ldbase.ObjectOf(uri + "/" + ClassificationSystemClient.AdaptClassNotation(classSystem, classNotation), ClassificationSystemClient.GetLabelPredicate(classSystem))(0)
        '    End If
        '    Return Nothing
        'End Function


        'Public Function GetBroaderClassNotation(ByVal classSystem As SubjClass.ClassificationSystems, ByVal classNotation As String) As String
        '    'string uri = ClassificationSystemClient.GetURI(classSystem);

        '    'If (classNotation.StartsWith(Uri)) Then
        '    '    return this._ldbase.ObjectOf(classNotation, ClassificationSystemClient.GetPredicate(classSystem, ClassificationSystemClient.ClassificationPredicates.broader_term))[0];
        '    'Else
        '    '    return this._ldbase.ObjectOf(uri + "/" + ClassificationSystemClient.AdaptClassNotation(classSystem, classNotation), ClassificationSystemClient.GetPredicate(classSystem,ClassificationSystemClient.ClassificationPredicates.broader_term))[0];
        '    Return Nothing
        'End Function


    End Class

End Namespace
