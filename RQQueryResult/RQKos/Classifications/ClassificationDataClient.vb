'Imports RQLinkedData
'Imports RQWebAccess.RQLinkedDataClients.ClassificationData

Namespace RQKos.Classifications

    Public Class ClassificationDataClient

        'private LDBase _ldbase;


        Public Sub New()

        End Sub


        Public Function GetClassId(ByVal classSystem As ClassificationCode.ClassificationSystems, ByVal classNotation As String) As String
            If classSystem = ClassificationCode.ClassificationSystems.rq Then
                Dim mqQuery As New RQDAL.RQCatalogDAL

                Return mqQuery.GetClassID(classNotation)
            Else
                Return ""
            End If
        End Function


        Public Sub GetClassData(ByVal classSystem As ClassificationCode.ClassificationSystems, ByRef theClass As SubjClass)
            If classSystem = ClassificationCode.ClassificationSystems.rq Then
                Dim mqQuery As New RQDAL.RQCatalogDAL
                Dim drRow As RQDataSet.SystematikRow

                drRow = CType(mqQuery.GetRecordByID(CStr(theClass.ClassID), "RQDataSet", "Systematik", True), RQDataSet.SystematikRow)
                theClass.ParentClassID = drRow.ParentID
                theClass.ClassCode = drRow.DDCNumber
                theClass.ClassShortTitle = drRow.Description
                theClass.ClassLongTitle = drRow.RegensburgDesc
                theClass.RefRVKSet = drRow.RegensburgSign
                theClass.NrOfClassDocs = drRow.DocRefCount
                theClass.NrOfSubClasses = drRow.SubClassCount
                theClass.NrOfRefLinks = drRow.DirRefCount
            End If
        End Sub


        Public Sub PutClassData(ByVal classSystem As ClassificationCode.ClassificationSystems, ByRef theClass As SubjClass)
            If classSystem = ClassificationCode.ClassificationSystems.rq Then
                Dim mqQuery As New RQDAL.RQCatalogDAL
                Dim drRow As RQDataSet.SystematikRow

                drRow = CType(mqQuery.GetRecordByID(CStr(theClass.ClassID), "RQDataSet", "Systematik", True), RQDataSet.SystematikRow)
                drRow.ParentID = theClass.ParentClassID
                drRow.DDCNumber = theClass.ClassCode
                drRow.Description = theClass.ClassShortTitle
                drRow.RegensburgDesc = theClass.ClassLongTitle
                drRow.RegensburgSign = theClass.RefRVKSet
                drRow.DocRefCount = theClass.NrOfClassDocs
                drRow.SubClassCount = theClass.NrOfSubClasses
                drRow.DirRefCount = theClass.NrOfRefLinks
            End If
        End Sub


        Public Function GetAltLabel(ByVal classSystem As ClassificationCode.ClassificationSystems, ByVal classNotation As String) As String
            'string uri = ClassificationSystemClient.GetURI(classSystem);

            'If (classNotation.StartsWith(uri)) Then
            'return this._ldbase.ObjectOf(classNotation, ClassificationSystemClient.GetPredicate(classSystem, ClassificationSystemClient.ClassificationPredicates.alternative_label))[0];
            'Else
            'return this._ldbase.ObjectOf(uri + "/" + ClassificationSystemClient.AdaptClassNotation(classSystem, classNotation), ClassificationSystemClient.GetPredicate(classSystem, ClassificationSystemClient.ClassificationPredicates.alternative_label))[0];
            Return Nothing
        End Function


        Public Function GetPrefLabel(ByVal classSystem As ClassificationCode.ClassificationSystems, ByVal classNotation As String) As String
            'string uri = ClassificationSystemClient.GetURI(classSystem);

            'If (classNotation.StartsWith(Uri)) Then
            '    return this._ldbase.ObjectOf(classNotation, ClassificationSystemClient.GetPredicate(classSystem, ClassificationSystemClient.ClassificationPredicates.preferred_label))[0];
            'Else
            '    return this._ldbase.ObjectOf(uri + "/" + ClassificationSystemClient.AdaptClassNotation(classSystem, classNotation), ClassificationSystemClient.GetLabelPredicate(classSystem))[0];
            Return Nothing
        End Function


        Public Function GetBroaderClassNotation(ByVal classSystem As ClassificationCode.ClassificationSystems, ByVal classNotation As String) As String
            'string uri = ClassificationSystemClient.GetURI(classSystem);

            'If (classNotation.StartsWith(Uri)) Then
            '    return this._ldbase.ObjectOf(classNotation, ClassificationSystemClient.GetPredicate(classSystem, ClassificationSystemClient.ClassificationPredicates.broader_term))[0];
            'Else
            '    return this._ldbase.ObjectOf(uri + "/" + ClassificationSystemClient.AdaptClassNotation(classSystem, classNotation), ClassificationSystemClient.GetPredicate(classSystem,ClassificationSystemClient.ClassificationPredicates.broader_term))[0];
            Return Nothing
        End Function


        Public Sub GetNarrowerClassData(ByVal classSystem As ClassificationCode.ClassificationSystems, ByVal majClassId As Integer, ByRef classBranch As SubjClassBranch)
            Dim mqQuery As New RQDAL.RQCatalogDAL
            Dim drTable As RQDataSet.SystematikDataTable = CType(mqQuery.GetRecordByParentID(majClassId, "RQDataSet", "Systematik", True), RQDataSet.SystematikDataTable)
            Dim drRow As RQDataSet.SystematikRow

            For Each drRow In drTable
                Dim _cl As SubjClass = New SubjClass(drRow.ID)

                _cl.ParentClassID = drRow.ParentID
                _cl.ClassCode = drRow.DDCNumber
                _cl.ClassShortTitle = drRow.Description
                _cl.ClassLongTitle = drRow.RegensburgDesc
                _cl.RefRVKSet = drRow.RegensburgSign
                _cl.NrOfClassDocs = drRow.DocRefCount
                _cl.NrOfSubClasses = drRow.SubClassCount
                _cl.NrOfRefLinks = drRow.DirRefCount
                classBranch.Add(_cl)
            Next
        End Sub


        Public Function UpdateDocRefs(ByVal classSystem As ClassificationCode.ClassificationSystems, ByRef classBranch As SubjClassBranch, Optional ByVal iSuperClassDocCount As Integer = 0) As Boolean
            Dim ResultSet As New RQQueryResult.RQResultSet
            Dim SuperClass As Utilities.LexicalClass = classBranch.MajorClass.RefRVKClass 'Me._arSubjClass(0).RefRVKClass
            Dim item As RQQueryResult.RQResultItem
            Dim bErr As Boolean = True
            Dim i As Integer

            'Zero DocRefCount for subclasses being updated
            For i = 1 To classBranch.count - 1 'Me._arSubjClass.Length - 1
                If Not IsNothing(classBranch.Item(i)) Then 'Me._arSubjClass(i)) Then
                    classBranch.Item(i).NrOfClassDocs = 0
                    classBranch.Item(i).NrOfRefLinks = 0
                End If
            Next
            EditGlobals.Message += "<p class='smalltext'>"
            ResultSet.Find(classBranch.MajorClass.RefRVKSet) 'Me._arSubjClass(0).RefRVKSet)
            For Each item In ResultSet
                Try
                    Dim clClassString As Utilities.ClassString = New Utilities.ClassString(item.ItemDescription.Classification.Content)
                    Dim bCLComplete As Boolean = False
                    Dim arIsInSubClass As New Collections.BitArray(clClassString.Count, False)
                    Dim arIsInSuperClass As New Collections.BitArray(clClassString.Count, False)
                    Dim arContainsClassString As New Collections.BitArray(classBranch.count, False) 'Me._arSubjClass.Length, False)

                    For i = 0 To clClassString.Count - 1
                        If (clClassString.Item(i).StartsWith(Globals.ClassCodePrefix)) Then
                            If classBranch.MajorClass.RefRVKClass.IsInRange(clClassString.Item(i).Remove(0, 8)) Then
                                Dim j As Integer

                                For j = 1 To classBranch.count - 1 'Me._arSubjClass.Length - 1
                                    If Not IsNothing(classBranch.Item(j)) Then 'Me._arSubjClass(j)) Then
                                        Dim SubClass As Utilities.LexicalClass = classBranch.Item(j).RefRVKClass 'Me._arSubjClass(j).RefRVKClass

                                        If SubClass.IsInRange(clClassString.Item(i).Remove(0, 8)) Then
                                            arIsInSubClass(i) = True
                                            If Not arContainsClassString(j) = True Then
                                                classBranch.Item(j).NrOfClassDocs += CType(1, Short) 'Me._arSubjClass(j).NrOfClassDocs += CType(1, Short)
                                                If item.RQResultItemType = RQQueryResult.RQResultItem.RQItemType.bookmark Then classBranch.Item(j).NrOfRefLinks += CType(1, Short)
                                                arContainsClassString(j) = True
                                            End If
                                            item.ItemDescription.Classification.Content = New Utilities.ClassString(item.ItemDescription.Classification.Content).CompleteClassString(clClassString.Item(i), classBranch.Item(j).ClassCode)
                                        End If
                                    End If
                                Next
                                If Not arIsInSubClass(i) Then
                                    arIsInSuperClass(i) = True
                                End If
                            End If
                        End If
                    Next
                    For i = 0 To clClassString.Count - 1
                        If arIsInSuperClass(i) And Not arIsInSubClass(i) Then
                            iSuperClassDocCount += 1
                            Exit For
                        End If
                    Next
                Catch ex As Exception
                    Dim test As String
                    test = ex.Message
                End Try
            Next
            If ResultSet.Update() <> 0 Then
                EditGlobals.Message += "Error occured on DocRefCount update for class " & classBranch.MajorClass.ClassID & ".</br>" 'Me._arSubjClass(0).ClassID & ".</br>"
                bErr = False
            Else
                EditGlobals.Message += "DocRefCounts have been updated for class " & classBranch.MajorClass.ClassID & ".</br>" 'Me._arSubjClass(0).ClassID & ".</br>"
            End If
            Return bErr
        End Function

    End Class

End Namespace
