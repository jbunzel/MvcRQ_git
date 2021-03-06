﻿Namespace RQKos.Classifications

    Public Class RQClassificationDataClient
        Inherits ClassificationDataClient

#Region "Public Constructors"

        Public Sub New()
        End Sub

#End Region


#Region "Private Methods"
        Private Function DeleteSubClass(ByRef classBranch As SubjClassBranch, Index As Integer) As Boolean
            Dim retVal = False

            If Not IsNothing(classBranch.Item(Index)) Then
                Dim mqQuery As New RQDAL.RQCatalogDAL
                Dim drTable As RQDataSet.SystematikDataTable = CType(mqQuery.GetRecordByParentID(classBranch.MajorClass.ParentClassID, "RQDataSet", "Systematik", True), RQDataSet.SystematikDataTable)

                For i = 0 To drTable.Rows.Count - 1
                    If drTable.Item(i).ID = classBranch.Item(Index).ClassID Then
                        drTable.Item(i).Delete()
                        Exit For
                    End If
                Next
                retVal = (mqQuery.UpdateSystematik() = 0)
            End If
            Return retVal
        End Function


        Private Function DeleteSubBranches(ByRef classBranch As SubjClassBranch) As Boolean
            Dim i As Integer = 0
            Dim retVal As Boolean = False
            Dim mqQuery As New RQDAL.RQCatalogDAL
            Dim drTable As RQDataSet.SystematikDataTable = CType(mqQuery.GetRecordByParentID(classBranch.MajorClassID, "RQDataSet", "Systematik", True), RQDataSet.SystematikDataTable)

            If Not drTable Is Nothing Then
                For i = 1 To classBranch.count - 1
                    If Not IsNothing(classBranch.Item(i)) Then
                        If classBranch.Item(i).NrOfSubClasses > 0 Then
                            Dim subClassBranch As New Classifications.SubjClassBranch(classBranch.Item(i).ClassID)

                            DeleteSubBranches(subClassBranch)
                        End If
                        drTable.Item(i - 1).Delete()
                        classBranch.Item(i) = Nothing
                    End If
                Next
                retVal = (mqQuery.UpdateSystematik() = 0)
            End If
            Return retVal
        End Function

        ''' <summary>
        ''' Filters bookmark folders which are classified with would not show up  
        ''' </summary>
        ''' <param name="item"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function FilterBookmarkFolders(ByRef item As RQQueryResult.RQResultItem, ByRef classname As String) As Boolean
            If item.ItemDescription.Feld30.Contains("ZZ999") Then
                Return False
            End If
            Return item.ItemDescription.Classification.Content.Contains(classname) Or (Not item.ItemDescription.Feld30.Contains("ZZ9999"))
        End Function

#End Region


#Region "Public Method Overrides"

        Public Overrides Function GetClassId(ByVal classNotation As String) As String
            Dim _mqQuery = New RQDAL.RQCatalogDAL

            Return _mqQuery.GetClassID(classNotation)
        End Function


        Public Overrides Sub GetClassData(ByRef theClass As SubjClass)
            Dim _mqQuery = New RQDAL.RQCatalogDAL
            Dim drRow As RQDataSet.SystematikRow

            drRow = CType(_mqQuery.GetRecordByID(CStr(theClass.ClassID), "RQDataSet", "Systematik", True), RQDataSet.SystematikRow)
            theClass.ParentClassID = drRow.ParentID
            theClass.ClassCode = drRow.DDCNumber
            theClass.ClassShortTitle = drRow.Description
            theClass.ClassLongTitle = drRow.RegensburgDesc
            theClass.RefRVKSet = drRow.RegensburgSign
            theClass.NrOfClassDocs = drRow.DocRefCount
            theClass.NrOfSubClasses = drRow.SubClassCount
            'STRANGE ERROR: If drRow.DirRefCount is DBNull or Nothing (depending of RQDataSet-Properties) if taken from a stored value in _mqQuery.GetRecordByID
            theClass.NrOfRefLinks = IIf(String.IsNullOrEmpty(drRow.DirRefCount), 0, drRow.DirRefCount)
            theClass.SetComplete()
        End Sub


        Public Overrides Function PutClassData(ByRef theClass As SubjClass) As Boolean
            Dim _mqQuery = New RQDAL.RQCatalogDAL
            Dim drRow As RQDataSet.SystematikRow

            drRow = CType(_mqQuery.GetRecordByID(CStr(theClass.ClassID), "RQDataSet", "Systematik", True), RQDataSet.SystematikRow)
            drRow.ParentID = theClass.ParentClassID
            drRow.DDCNumber = theClass.ClassCode
            drRow.Description = theClass.ClassShortTitle
            drRow.RegensburgDesc = IIf(IsNothing(theClass.ClassLongTitle) Or theClass.ClassLongTitle = "", "-", theClass.ClassLongTitle)
            drRow.RegensburgSign = theClass.RefRVKSet
            drRow.SubClassCount = theClass.NrOfSubClasses
            drRow.DocRefCount = theClass.NrOfClassDocs
            drRow.DirRefCount = theClass.NrOfRefLinks
            Return Not _mqQuery.UpdateSystematik()
        End Function


        Public Overrides Sub GetNarrowerClassData(ByVal majClassId As Integer, ByRef classBranch As SubjClassBranch)
            Dim _mqQuery = New RQDAL.RQCatalogDAL
            Dim drTable As RQDataSet.SystematikDataTable = CType(_mqQuery.GetRecordByParentID(majClassId, "RQDataSet", "Systematik", True), RQDataSet.SystematikDataTable)
            Dim drRow As RQDataSet.SystematikRow

            For Each drRow In drTable
                Dim _cl As SubjClass = New SubjClass(drRow.ID, Me)

                _cl.ClassificationSystem = SubjClass.ClassificationSystems.rq
                _cl.ParentClassID = drRow.ParentID
                _cl.ClassCode = drRow.DDCNumber
                _cl.ClassShortTitle = drRow.Description
                _cl.ClassLongTitle = drRow.RegensburgDesc
                _cl.RefRVKSet = drRow.RegensburgSign
                _cl.NrOfClassDocs = drRow.DocRefCount
                _cl.NrOfSubClasses = drRow.SubClassCount
                'STRANGE ERROR: If drRow.DirRefCount is DBNull or Nothing (depending of RQDataSet-Properties) if taken from a stored value in _mqQuery.GetRecordByID
                _cl.NrOfRefLinks = IIf(String.IsNullOrEmpty(drRow.DirRefCount), 0, drRow.DirRefCount)
                '_cl.NrOfRefLinks = drRow.DirRefCount
                classBranch.Add(_cl)
            Next
        End Sub


        Public Overrides Sub GetNarrowerClassData(ByVal majClassId As String, ByRef classBranch As SubjClassBranch)

        End Sub


        Public Overrides Function UpdateDocRefs(ByRef classBranch As SubjClassBranch, ByRef iMajorClassDocCount As Integer, ByRef iMajorClassRefCount As Integer) As Boolean
            Dim ResultSet As New RQQueryResult.RQResultSet(True)
            Dim item As RQQueryResult.RQResultItem
            Dim bErr As Boolean = True
            Dim i As Integer

            'Zero DocRefCount for subclasses being updated
            For i = 1 To classBranch.count - 1
                If Not IsNothing(classBranch.Item(i)) Then
                    classBranch.Item(i).NrOfClassDocs = 0
                    classBranch.Item(i).NrOfRefLinks = 0
                End If
            Next
            EditGlobals.Message += "<p class='smalltext'>"
            ResultSet.Find(classBranch.MajorClass.RefRVKSet)
            For Each item In ResultSet
                Dim bChanged As Boolean = False
                Dim strWorkClassString As String = item.ItemDescription.Classification.Content
                Dim clClassString As Utilities.ClassString = New Utilities.ClassString(strWorkClassString)
                Dim bHasNoMinorClassCodes As Boolean = True
                Dim bHasMajorClassCodes As Boolean = False
                Dim arHasAClassStringIn As New Collections.BitArray(classBranch.count, False)
                Dim arIsMajorClassCode As New Collections.BitArray(clClassString.Count, False)

                For i = 0 To clClassString.Count - 1
                    Dim arHasThisClassStringIn As New Collections.BitArray(classBranch.count, False)

                    If (clClassString.Item(i).StartsWith(Globals.ClassCodePrefix)) Then
                        If (clClassString.Item(i).Contains("QP")) Then
                            Dim test As String = "TEST"
                        End If
                        If classBranch.MajorClass.RefRVKClass.IsInRange(clClassString.Item(i).Remove(0, 8)) Then
                            Dim j As Integer

                            For j = 1 To classBranch.count - 1
                                If Not IsNothing(classBranch.Item(j)) Then
                                    Dim SubClass As Utilities.LexicalClass = classBranch.Item(j).RefRVKClass

                                    If SubClass.IsInRange(clClassString.Item(i).Remove(0, 8)) Then
                                        If Not arHasAClassStringIn(j) = True Then
                                            If item.RQResultItemType = RQQueryResult.RQResultItem.RQItemType.bookmark Then
                                                ''If Not Me.FilterBookmarkFolders(item, clClassString.Item(i).Remove(0, 8)) Then Exit For
                                                classBranch.Item(j).NrOfRefLinks += CType(1, Short)
                                            Else
                                                classBranch.Item(j).NrOfClassDocs += CType(1, Short)
                                            End If
                                            arHasAClassStringIn(j) = True
                                            bHasNoMinorClassCodes = False
                                        End If
                                        arHasThisClassStringIn(j) = True
                                        strWorkClassString = New Utilities.ClassString(strWorkClassString).CompleteClassString(clClassString.Item(i), classBranch.Item(j).ClassCode)
                                        bChanged = True
                                    Else
                                        arIsMajorClassCode(i) = True 'clClassString.Item(i) gehört entweder zu einer anderen Minorklasse oder zur Majorklasse. Letzteres wird angenommen 
                                    End If
                                End If
                            Next
                        End If
                    End If
                    If arIsMajorClassCode(i) Then
                        For j = 1 To classBranch.count - 1 'Teste, ob clClassString.Item(i) zu einer anderen Minorklasse gehört. Wenn ja, gehört es nicht zur Majorklasse
                            arIsMajorClassCode(i) = arIsMajorClassCode(i) And (Not arHasThisClassStringIn(j))
                        Next
                        If arIsMajorClassCode(i) Then 'Vervollständige den clClassString mit dem ClassCode der Majorklasse 
                            Dim bToDo = True

                            If Not IsNothing(clClassString.Item(i - 1)) Then bToDo = clClassString.Item(i - 1) <> classBranch.MajorClass.ClassCode
                            If bToDo Then
                                strWorkClassString = New Utilities.ClassString(strWorkClassString).CompleteClassString(clClassString.Item(i), classBranch.MajorClass.ClassCode)
                                bChanged = True
                            End If
                        End If
                    End If
                Next
                If bChanged Then
                    Dim clWorkItemCopy As New RQQueryResult.RQResultItem(item)

                    clWorkItemCopy.ItemDescription.Classification = New RQLib.RQQueryResult.RQDescriptionElements.RQClassification(strWorkClassString)
                    item.ItemDescription.Classification.ClassEditMode(True) 'set to prevent class completion with old classification data when item is changed
                    item.Change(clWorkItemCopy)
                    item.ItemDescription.Classification.ClassEditMode(False)
                End If
                For i = 0 To clClassString.Count - 1
                    bHasMajorClassCodes = bHasMajorClassCodes Or arIsMajorClassCode(i)
                Next
                If bHasNoMinorClassCodes Or bHasMajorClassCodes Then
                    If item.RQResultItemType = RQQueryResult.RQResultItem.RQItemType.bookmark Then
                        ''If Not Me.FilterBookmarkFolders(item, clClassString.Item(i).Remove(0, 8)) Then Exit For
                        iMajorClassRefCount += 1
                    Else
                        iMajorClassDocCount += 1
                    End If
                End If
            Next
            If ResultSet.Update() <> 0 Then
                EditGlobals.Message += "Error occured on DocRefCount update for class " & classBranch.MajorClass.ClassID & ".</br>"
                bErr = False
            Else
                EditGlobals.Message += "DocRefCounts have been updated for class " & classBranch.MajorClass.ClassID & ".</br>"
            End If
            Return bErr
        End Function

        ''' <summary>
        ''' Updates a given classification branch and all document and bookmark references pointing to it or its sub classifications. 
        ''' </summary>
        ''' <param name="classBranch">
        ''' A valid and feasible (cf. class SubjClassBranch) classification branch to update. 
        ''' </param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Update(ByRef classBranch As SubjClassBranch) As Boolean
            Dim retVal As Boolean = False
            Dim iSuperClassDocCount As Integer = 0
            Dim iSuperClassRefCount As Integer = 0

            If Me.UpdateDocRefs(classBranch, iSuperClassDocCount, iSuperClassRefCount) = True Then
                Dim i As Integer = 0
                Dim mqQuery As New RQDAL.RQCatalogDAL
                Dim drTable As RQDataSet.SystematikDataTable = CType(mqQuery.GetRecordByParentID(classBranch.MajorClassID, "RQDataSet", "Systematik", True), RQDataSet.SystematikDataTable)

                EditGlobals.AddHint("OK    ", "Dokumente und Bookmarks von " + classBranch.Item(0).ClassShortTitle + " wurden neu zugeordnet.")
                If Not drTable Is Nothing Then
                    For i = 1 To classBranch.count - 1
                        If Not IsNothing(classBranch.Item(i)) Then
                            If i > drTable.Rows.Count Then drTable.Rows.Add(CType(mqQuery.NewRow("RQDataSet", "Systematik"), RQDataSet.SystematikRow))
                            CType(drTable.Rows(i - 1), RQDataSet.SystematikRow).DDCNumber = classBranch.Item(i).ClassCode
                            CType(drTable.Rows(i - 1), RQDataSet.SystematikRow).Description = classBranch.Item(i).ClassShortTitle
                            CType(drTable.Rows(i - 1), RQDataSet.SystematikRow).DirRefCount = classBranch.Item(i).NrOfRefLinks
                            CType(drTable.Rows(i - 1), RQDataSet.SystematikRow).DocRefCount = classBranch.Item(i).NrOfClassDocs
                            CType(drTable.Rows(i - 1), RQDataSet.SystematikRow).ParentID = classBranch.MajorClassID
                            CType(drTable.Rows(i - 1), RQDataSet.SystematikRow).RegensburgDesc = IIf(IsNothing(classBranch.Item(i).ClassLongTitle) Or classBranch.Item(i).ClassLongTitle = "", "-", classBranch.Item(i).ClassLongTitle)
                            CType(drTable.Rows(i - 1), RQDataSet.SystematikRow).RegensburgSign = classBranch.Item(i).RefRVKSet
                            CType(drTable.Rows(i - 1), RQDataSet.SystematikRow).SubClassCount = classBranch.Item(i).NrOfSubClasses
                        End If
                    Next
                    classBranch.MajorClass.NrOfSubClasses = CShort(drTable.Count)
                    classBranch.MajorClass.NrOfClassDocs = CShort(iSuperClassDocCount)
                    classBranch.MajorClass.NrOfRefLinks = CShort(iSuperClassRefCount)
                    retVal = Not mqQuery.UpdateSystematik()
                    If classBranch.MajorClass.ClassCode <> "NULL" Then retVal = retVal And Me.PutClassData(classBranch.MajorClass) 'Update SubClassCount parameter of base class if not root class
                    If retVal = True Then
                        EditGlobals.AddHint("OK    ", "Der Klassifikationsstrang '" + classBranch.Item(0).ClassShortTitle + "' wurde aktualisiert.")

                        For i = 1 To classBranch.count - 1
                            If Not IsNothing(classBranch.Item(i)) Then
                                If classBranch.Item(i).NrOfSubClasses > 0 Then
                                    Dim subClassBranch = New SubjClassBranch(classBranch.Item(i), CType(New RQDAL.RQCatalogDAL().GetRecordByParentID(classBranch.Item(i).ClassID, "RQDataSet", "Systematik", True), RQDataSet.SystematikDataTable))

                                    subClassBranch.ChangeSubClassCodes()
                                    Update(subClassBranch)
                                End If
                            End If
                        Next
                    Else
                        EditGlobals.AddHint("FEHLER", "Der Klassifikationsstrang '" + classBranch.Item(0).ClassShortTitle + "' konnte nicht aktualisiert werden.")
                    End If
                End If
            Else
                EditGlobals.AddHint("FEHLER", "Dokumente und Bookmarks von " + classBranch.Item(0).ClassShortTitle + " konnten nicht neu zugeordnet werden.")
            End If
            Return retVal
        End Function


        ''' <summary>
        ''' Deletes a given classification branch with major and all sub classification classes
        ''' </summary>
        ''' <param name="classBranch">
        ''' Classification branch to delete
        ''' </param>
        ''' <returns>
        ''' True if function was executed without error.
        ''' </returns>
        ''' <remarks>
        ''' If errors occured hints are written to the Hints structore of RQLIb module EditGlobals.
        '''</remarks>
        Public Overrides Function Delete(ByRef classBranch As SubjClassBranch) As Boolean
            Dim retVal As Boolean = False
            Dim iSuperClassDocCount As Integer = 0
            Dim iSuperClassRefCount As Integer = 0
            Dim clSuperClassBranch As New SubjClassBranch(classBranch.Item(0).ParentClassID)

            retVal = Me.DeleteSubBranches(classBranch)
            If (retVal) Then
                EditGlobals.AddHint("OK    ", "Unterklassen von " + classBranch.Item(0).ClassShortTitle + " wurden gelöscht.")
                retVal = Me.DeleteSubClass(classBranch, 0)
                If (retVal) Then
                    EditGlobals.AddHint("OK    ", "Klasse " + classBranch.Item(0).ClassShortTitle + " wurde gelöscht.")
                    classBranch = clSuperClassBranch
                    classBranch.Load()
                    classBranch.MajorClass.NrOfSubClasses += -1
                    If (Me.UpdateDocRefs(classBranch, iSuperClassDocCount, iSuperClassRefCount)) Then
                        EditGlobals.AddHint("OK    ", "Dokumente und Bookmarks wurden neu zugeordnet.")
                        classBranch.MajorClass.NrOfClassDocs = CShort(iSuperClassDocCount)
                        classBranch.MajorClass.NrOfRefLinks = CShort(iSuperClassRefCount)
                        If (Me.PutClassData(classBranch.MajorClass)) Then
                            EditGlobals.AddHint("OK    ", "Daten der Klasse " + classBranch.Item(0).ClassShortTitle + " wurden aktualisiert.")
                        Else
                            retVal = False
                            EditGlobals.AddHint("FEHLER", "Daten der Klasse " + classBranch.Item(0).ClassShortTitle + " konnten nicht aktualisiert werden.")
                        End If
                    Else
                        retVal = False
                        EditGlobals.AddHint("FEHLER", "Dokumente und Bookmarks konnten nicht neu zugeordnet werden.")
                    End If
                Else
                    EditGlobals.AddHint("FEHLER", "Klasse " + classBranch.Item(0).ClassShortTitle + " konnte nicht gelöscht werden.")
                End If
            Else
                EditGlobals.AddHint("FEHLER", "Unterklassen von " + classBranch.Item(0).ClassShortTitle + " konnten nicht gelöscht werden.")
            End If
            Return retVal
        End Function


#End Region


#Region "Public Methods"

        'Public Function GetAltLabel(ByVal classSystem As ClassificationCode.ClassificationSystems, ByVal classNotation As String) As String
        '    'string uri = ClassificationSystemClient.GetURI(classSystem);

        '    'If (classNotation.StartsWith(uri)) Then
        '    'return this._ldbase.ObjectOf(classNotation, ClassificationSystemClient.GetPredicate(classSystem, ClassificationSystemClient.ClassificationPredicates.alternative_label))[0];
        '    'Else
        '    'return this._ldbase.ObjectOf(uri + "/" + ClassificationSystemClient.AdaptClassNotation(classSystem, classNotation), ClassificationSystemClient.GetPredicate(classSystem, ClassificationSystemClient.ClassificationPredicates.alternative_label))[0];
        '    Return Nothing
        'End Function


        'Public Function GetPrefLabel(ByVal classSystem As ClassificationCode.ClassificationSystems, ByVal classNotation As String) As String
        '    'string uri = ClassificationSystemClient.GetURI(classSystem);

        '    'If (classNotation.StartsWith(Uri)) Then
        '    '    return this._ldbase.ObjectOf(classNotation, ClassificationSystemClient.GetPredicate(classSystem, ClassificationSystemClient.ClassificationPredicates.preferred_label))[0];
        '    'Else
        '    '    return this._ldbase.ObjectOf(uri + "/" + ClassificationSystemClient.AdaptClassNotation(classSystem, classNotation), ClassificationSystemClient.GetLabelPredicate(classSystem))[0];
        '    Return Nothing
        'End Function


        'Public Function GetBroaderClassNotation(ByVal classSystem As ClassificationCode.ClassificationSystems, ByVal classNotation As String) As String
        '    'string uri = ClassificationSystemClient.GetURI(classSystem);

        '    'If (classNotation.StartsWith(Uri)) Then
        '    '    return this._ldbase.ObjectOf(classNotation, ClassificationSystemClient.GetPredicate(classSystem, ClassificationSystemClient.ClassificationPredicates.broader_term))[0];
        '    'Else
        '    '    return this._ldbase.ObjectOf(uri + "/" + ClassificationSystemClient.AdaptClassNotation(classSystem, classNotation), ClassificationSystemClient.GetPredicate(classSystem,ClassificationSystemClient.ClassificationPredicates.broader_term))[0];
        '    Return Nothing
        'End Function

#End Region

    End Class

End Namespace
