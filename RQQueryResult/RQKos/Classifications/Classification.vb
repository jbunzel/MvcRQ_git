Imports Microsoft.VisualBasic

Namespace RQKos.Classifications

    Public Class SubjClass

#Region "Private Members"

        Private _classDataClient As ClassificationDataClient = Nothing
        Private _intClassID As Integer = -1
        Private _intParentClassID As Integer = -1
        Private _strClassCode As String = ""
        Private _strClassShortTitle As String = ""
        Private _strClassLongTitle As String = ""
        Private _strRefRVKSet As String = ""
        Private _intNrOfSubClasses As Integer = -1
        Private _intNrOfDocuments As Integer = -1
        Private _intNrOfRefLinks As Integer = -1


        Private Enum ValidEnum
            invalid
            valid
            undefined
        End Enum


        Private _isValid As ValidEnum = ValidEnum.undefined

#End Region


#Region "Properties"

        Public ReadOnly Property ClassDataClient() As ClassificationDataClient
            Get
                Return Me._classDataClient
            End Get
        End Property


        Public Property ClassID() As String
            Get
                If Me._strClassCode <> "" And Me._intClassID < 0 Then
                    Me._intClassID = Me.RetrieveClassId()
                End If
                Return CStr(Me._intClassID)
            End Get
            Set(ByVal value As String)
                If CInt(value) <> Me._intClassID Then
                    Me._intClassID = CInt(value)
                    Me._isValid = ValidEnum.undefined
                End If
            End Set
        End Property


        Public Property ParentClassID() As String
            Get
                If Me._strClassCode <> "" And Me._intClassID < 0 Then
                    Me._intClassID = Me.RetrieveClassId()
                End If
                If Me.ClassID >= 0 And Me._intParentClassID < 0 Then
                    Me.Read()
                End If
                Return CStr(Me._intParentClassID)
            End Get
            Set(ByVal value As String)
                If CInt(value) <> Me._intParentClassID Then
                    Me._intParentClassID = CInt(value)
                    Me._isValid = ValidEnum.undefined
                End If
            End Set
        End Property


        Public Property ClassCode() As String
            Get
                If Me._intClassID >= 0 And Me._strClassCode = "" Then
                    Me.Read()
                End If
                Return Me._strClassCode
            End Get
            Set(ByVal value As String)
                Me._strClassCode = value
                Me._isValid = ValidEnum.undefined
            End Set
        End Property


        Public Property ClassShortTitle() As String
            Get
                If Me._strClassCode <> "" And Me._intClassID < 0 Then
                    Me._intClassID = Me.RetrieveClassId()
                End If
                If Me.ClassID >= 0 And Me._strClassShortTitle = "" Then
                    Me.Read()
                End If
                Return Me._strClassShortTitle
            End Get
            Set(ByVal value As String)
                Me._strClassShortTitle = value
                Me._isValid = ValidEnum.undefined
            End Set
        End Property


        Public Property ClassLongTitle() As String
            Get
                If Me._strClassCode <> "" And Me._intClassID < 0 Then
                    Me._intClassID = Me.RetrieveClassId()
                End If
                If Me.ClassID >= 0 And Me._strClassLongTitle = "" Then
                    Me.Read()
                End If
                Return Me._strClassLongTitle
            End Get
            Set(ByVal value As String)
                Me._strClassLongTitle = value
                Me._isValid = ValidEnum.undefined
            End Set
        End Property


        Public Property RefRVKSet() As String
            Get
                If Me._strClassCode <> "" And Me._intClassID < 0 Then
                    Me._intClassID = Me.RetrieveClassId()
                End If
                If Me.ClassID >= 0 And Me._strRefRVKSet = "" Then
                    Me.Read()
                End If
                Return Me._strRefRVKSet
            End Get
            Set(ByVal value As String)
                Me._strRefRVKSet = value
                Me._isValid = ValidEnum.undefined
            End Set
        End Property


        Public Property RefRVKClass() As Utilities.LexicalClass
            Get
                Return New Utilities.LexicalClass(RefRVKSet)
            End Get
            Set(ByVal value As Utilities.LexicalClass)
                Me._strRefRVKSet = value.Expand()
                Me._isValid = ValidEnum.undefined
            End Set
        End Property


        'Public ReadOnly Property OrigRefRVKClass() As String
        '    Get
        '       Return Me._strOrigRefRVKSet
        '    End Get
        'End Property


        Public Property NrOfSubClasses() As Integer
            Get
                If Me._strClassCode <> "" And Me._intClassID < 0 Then
                    Me._intClassID = Me.RetrieveClassId()
                End If
                If Me.ClassID >= 0 And Me._intNrOfSubClasses < 0 Then
                    Me.Read()
                End If
                Return Me._intNrOfSubClasses
            End Get
            Set(ByVal value As Integer)
                _intNrOfSubClasses = value
                Me._isValid = ValidEnum.undefined
            End Set
        End Property


        Public Property NrOfClassDocs() As Integer
            Get
                If Me._strClassCode <> "" And Me._intClassID < 0 Then
                    Me._intClassID = Me.RetrieveClassId()
                End If
                If Me.ClassID >= 0 And Me._intNrOfDocuments < 0 Then
                    Me.Read()
                End If
                Return Me._intNrOfDocuments
            End Get
            Set(ByVal value As Integer)
                _intNrOfDocuments = value
                Me._isValid = ValidEnum.undefined
            End Set
        End Property


        Public Property NrOfRefLinks() As Integer
            Get
                If Me._strClassCode <> "" And Me._intClassID < 0 Then
                    Me._intClassID = Me.RetrieveClassId()
                End If
                If Me.ClassID >= 0 And Me._intNrOfRefLinks < 0 Then
                    Me.Read()
                End If
                Return Me._intNrOfRefLinks
            End Get
            Set(ByVal value As Integer)
                _intNrOfRefLinks = value
                Me._isValid = ValidEnum.undefined
            End Set
        End Property

#End Region


#Region "Private Methods"

        Private Function RetrieveClassId() As Integer
            Try
                RetrieveClassId = CInt(Me._classDataClient.GetClassId(ClassificationCode.ClassificationSystems.rq, Me.ClassCode))
            Catch ex As Exception
                RetrieveClassId = -1
            End Try
        End Function


        Private Sub Read()
            If Me._intClassID > 0 Then
                'no class of RQClassificationSystem has ID=0. ClassID=0 is used to retriev the outermost subjClassBranch
                Me._classDataClient.GetClassData(ClassificationCode.ClassificationSystems.rq, Me)
            End If
        End Sub


        Private Function Write() As Integer
            If Me._intClassID > 0 Then
                'no class of RQClassificationSystem has ID=0. ClassID=0 is used to retriev the outermost subjClassBranch
                Me._classDataClient.PutClassData(ClassificationCode.ClassificationSystems.rq, Me)
            End If
            Return 1
        End Function

#End Region


#Region "Constructors"

        Public Sub New()
            Me._classDataClient = New ClassificationDataClient()
        End Sub


        Public Sub New(ByRef classID As String)
            Me.New()
            Me.ClassID = classID
        End Sub

#End Region


#Region "Public Methods"

        Public Sub Load()
            If Me._intClassID <> 0 Then
                Me.Read()
            End If
        End Sub


        Public Function Save() As Integer
            If Me._intClassID <> 0 Then
                Return Me.Write()
            End If
            Return 1
        End Function


        Public Function IsValid(ByRef MajClass As SubjClass) As Boolean
            If _isValid = ValidEnum.undefined Then
                If Not MajClass.Contains(Me) Then
                    EditGlobals.Message += "<p class='smalltext'>ERROR ON SUBCLASS " + Me.ClassID + "</p><p class='smalltext'>Subclass is not contained in class or class code is invalid.</p>"
                    _isValid = ValidEnum.invalid
                ElseIf (Me.NrOfSubClasses > 0) And ((Me.ClassCode = "") Or (Me.RefRVKSet = "")) Then
                    EditGlobals.Message += "<p class='smalltext'>ERROR ON SUBCLASS " + Me.ClassID + "</p><p class='smalltext'>Class containing subclasses must not be deleted.</p>"
                    _isValid = ValidEnum.invalid
                ElseIf Not Me.RefRVKClass.IsValid() Then
                    EditGlobals.Message += "<p class='smalltext'>ERROR ON SUBCLASS " + Me.ClassID + "</p><p class='smalltext'>Invalid RVK class codes.</p>"
                    _isValid = ValidEnum.invalid
                Else
                    _isValid = ValidEnum.valid
                End If
            End If
            Return _isValid = ValidEnum.valid
        End Function


        Public Function Intersects(ByVal thisClass As SubjClass) As Boolean
            Dim lexClass As Utilities.LexicalClass = Me.RefRVKClass

            Return lexClass.Intersects(thisClass.RefRVKClass)
        End Function


        Public Function Contains(ByVal thisClass As SubjClass) As Boolean
            If (thisClass.ClassCode <> "") And (New Utilities.ClassSubRange("").LexicalLowerOrEqual(thisClass.ClassCode, Me.ClassCode) = False) Then
                Return False
            Else
                Dim lexClass As Utilities.LexicalClass = Me.RefRVKClass

                Return lexClass.Contains(thisClass.RefRVKClass)
            End If
        End Function

#End Region

    End Class


    Public Class SubjClassBranch
        Implements IEnumerable

#Region "Private Member"

        Private _arSubjClass() As SubjClass

#End Region


#Region "Properties"

        ReadOnly Property MajorClassID() As String
            Get
                Return _arSubjClass(0).ClassID
            End Get
        End Property


        ReadOnly Property MajorClass() As SubjClass
            Get
                Return _arSubjClass(0)
            End Get
        End Property


        ReadOnly Property count() As Integer
            Get
                For i = 0 To Me._arSubjClass.Length() - 1
                    If IsNothing(Me._arSubjClass(i)) Then
                        Return i
                    End If
                Next
                Return Me._arSubjClass.Length()
            End Get
        End Property


        Property Item(ByVal i As Integer) As SubjClass
            Get
                Return _arSubjClass(i)
            End Get
            Set(ByVal value As SubjClass)
                _arSubjClass(i) = value
            End Set
        End Property

#End Region


#Region "Constructor"

        Public Sub New()
            _arSubjClass = New SubjClass(10) {}
        End Sub


        Public Sub New(ByRef majClassID As String)
            Dim _cl As New SubjClass(majClassID)

            _arSubjClass = New SubjClass(10) {}
            _arSubjClass(0) = _cl
        End Sub


        Public Sub New(ByVal classArray() As SubjClass)
            _arSubjClass = New SubjClass(classArray.Length - 1) {}

            Dim i As Integer
            For i = 0 To classArray.Length - 1
                _arSubjClass(i) = classArray(i)
            Next i
        End Sub


        Public Sub New(ByVal majClass As SubjClass, ByVal minClassBranch As RQDataSet.SystematikDataTable)
            Dim i As Integer

            _arSubjClass = New SubjClass(minClassBranch.Rows.Count + 1) {}
            _arSubjClass(0) = majClass
            For i = 0 To (minClassBranch.Rows.Count - 1)
                Dim cb As New SubjClass()

                cb.ClassID = CType(minClassBranch.Rows(i), RQDataSet.SystematikRow).ID
                cb.ParentClassID = CType(minClassBranch.Rows(i), RQDataSet.SystematikRow).ParentID
                cb.ClassCode = CType(minClassBranch.Rows(i), RQDataSet.SystematikRow).DDCNumber
                cb.ClassShortTitle = CType(minClassBranch.Rows(i), RQDataSet.SystematikRow).Description
                cb.ClassLongTitle = CType(minClassBranch.Rows(i), RQDataSet.SystematikRow).RegensburgDesc
                cb.RefRVKSet = CType(minClassBranch.Rows(i), RQDataSet.SystematikRow).RegensburgSign
                cb.NrOfSubClasses = CType(minClassBranch.Rows(i), RQDataSet.SystematikRow).SubClassCount
                cb.NrOfClassDocs = CType(minClassBranch.Rows(i), RQDataSet.SystematikRow).DocRefCount
                cb.NrOfRefLinks = CInt(CType(minClassBranch.Rows(i), RQDataSet.SystematikRow).DirRefCount)
                _arSubjClass(i + 1) = cb
            Next
        End Sub

#End Region


#Region "Public Methods"

        Public Sub Add(ByRef aSubjClass As SubjClass)
            Dim i As Integer

            For i = 0 To Me._arSubjClass.Length - 1
                If IsNothing(Me._arSubjClass(i)) Then
                    Me._arSubjClass(i) = aSubjClass
                    Exit Sub
                End If
            Next
            System.Array.Resize(Me._arSubjClass, i + 11)
            Me._arSubjClass(i) = aSubjClass
        End Sub


        Public Sub Clear()
            System.Array.Clear(Me._arSubjClass, 0, Me._arSubjClass.Length)
        End Sub


        Public Sub Load()
            Me._arSubjClass(0).Load()
            Me._arSubjClass(0).ClassDataClient.GetNarrowerClassData(ClassificationCode.ClassificationSystems.rq, Me._arSubjClass(0).ClassID, Me)
        End Sub


        Public Sub Update()
            'Should be transferred to ClassificationDataClient
            Dim NewSubClassCount As Integer = 0
            Dim mqQuery As New RQDAL.RQCatalogDAL
            Dim drTable As RQDataSet.SystematikDataTable
            Dim bErr As Boolean = True
            Dim iSuperClassDocCount As Integer = 0
            Dim i As Integer = 0

            If Me.UpdateDocRefs(iSuperClassDocCount) = True Then
                drTable = CType(mqQuery.GetRecordByParentID(Me.MajorClassID, "RQDataSet", "Systematik", True), RQDataSet.SystematikDataTable)
                If Not drTable Is Nothing Then
                    For i = 1 To Me._arSubjClass.Length - 1
                        If Not IsNothing(Me._arSubjClass(i)) Then
                            If i > drTable.Rows.Count Then
                                Dim drRow As RQDataSet.SystematikRow = CType(mqQuery.NewRow("RQDataSet", "Systematik"), RQDataSet.SystematikRow)

                                drTable.Rows.Add(drRow)
                                drRow.ParentID = Me.MajorClassID
                                drRow.DocRefCount = Me._arSubjClass(i).NrOfClassDocs
                                drRow.DirRefCount = Me._arSubjClass(i).NrOfRefLinks
                                drRow.SubClassCount = 0
                            End If
                            If Me._arSubjClass(i).ClassCode = "" Or Me._arSubjClass(i).RefRVKSet = "" Then
                                drTable.Item(i - 1).Delete()
                            Else
                                CType(drTable.Rows(i - 1), RQDataSet.SystematikRow).DDCNumber = Me._arSubjClass(i).ClassCode
                                CType(drTable.Rows(i - 1), RQDataSet.SystematikRow).Description = Me._arSubjClass(i).ClassShortTitle
                                CType(drTable.Rows(i - 1), RQDataSet.SystematikRow).RegensburgDesc = Me._arSubjClass(i).ClassLongTitle
                                CType(drTable.Rows(i - 1), RQDataSet.SystematikRow).RegensburgSign = Me._arSubjClass(i).RefRVKSet
                                CType(drTable.Rows(i - 1), RQDataSet.SystematikRow).DocRefCount = Me._arSubjClass(i).NrOfClassDocs
                                CType(drTable.Rows(i - 1), RQDataSet.SystematikRow).DirRefCount = Me._arSubjClass(i).NrOfRefLinks
                            End If
                        End If
                    Next
                    If mqQuery.UpdateSystematik() <> 0 Then
                        EditGlobals.Message += "Error occured on subclass update.</br>"
                    Else
                        bErr = False
                        NewSubClassCount = drTable.Count
                    End If
                End If
                If bErr = False Then
                    'Update SubClassCount parameter of base class 
                    Me.MajorClass.NrOfSubClasses = CShort(NewSubClassCount)
                    Me.MajorClass.NrOfClassDocs = CShort(iSuperClassDocCount)
                    If Me.MajorClass.Save <> 0 Then
                        EditGlobals.Message += "Error occured on superclass update.</br>"
                    End If
                    EditGlobals.Message += "Classification codes have been updated.</br>"
                Else
                    EditGlobals.Message += "Error occured on update of classification codes.</br>"
                End If
                For i = 1 To Me._arSubjClass.Length - 1
                    If Not IsNothing(Me._arSubjClass(i)) Then
                        If Me._arSubjClass(i).NrOfSubClasses <> 0 Then
                            Dim SubClassBranch As New SubjClassBranch(Me._arSubjClass(i).ClassID)

                            SubClassBranch.Load()
                            SubClassBranch.Update()
                        End If
                    End If
                Next
            End If
        End Sub


        Public Function UpdateDocRefs(ByRef iSuperClassDocCount As Integer) As Boolean
            Return Me.MajorClass.ClassDataClient.UpdateDocRefs(ClassificationCode.ClassificationSystems.rq, Me, iSuperClassDocCount)
            'Dim ResultSet As New RQQueryResult.RQResultSet
            'Dim SuperClass As Utilities.LexicalClass = Me._arSubjClass(0).RefRVKClass
            'Dim item As RQQueryResult.RQResultItem
            'Dim bErr As Boolean = True
            'Dim i As Integer

            ''Zero DocRefCount for subclasses being updated
            'For i = 1 To Me._arSubjClass.Length - 1
            '    If Not IsNothing(Me._arSubjClass(i)) Then
            '        Me._arSubjClass(i).NrOfClassDocs = 0
            '        Me._arSubjClass(i).NrOfRefLinks = 0
            '    End If
            'Next
            'EditGlobals.Message += "<p class='smalltext'>"
            'ResultSet.Find(Me._arSubjClass(0).RefRVKSet)
            'For Each item In ResultSet
            '    Try
            '        Dim clClassString As Utilities.ClassString = New Utilities.ClassString(item.ItemDescription.Classification.Content)
            '        Dim bCLComplete As Boolean = False
            '        Dim arIsInSubClass As New Collections.BitArray(clClassString.Count, False)
            '        Dim arIsInSuperClass As New Collections.BitArray(clClassString.Count, False)
            '        Dim arContainsClassString As New Collections.BitArray(Me._arSubjClass.Length, False)

            '        For i = 0 To clClassString.Count - 1
            '            If (clClassString.Item(i).StartsWith(Globals.ClassCodePrefix)) Then
            '                If MajorClass.RefRVKClass.IsInRange(clClassString.Item(i).Remove(0, 8)) Then
            '                    Dim j As Integer

            '                    For j = 1 To Me._arSubjClass.Length - 1
            '                        If Not IsNothing(Me._arSubjClass(j)) Then
            '                            Dim SubClass As Utilities.LexicalClass = Me._arSubjClass(j).RefRVKClass

            '                            If SubClass.IsInRange(clClassString.Item(i).Remove(0, 8)) Then
            '                                arIsInSubClass(i) = True
            '                                If Not arContainsClassString(j) = True Then
            '                                    Me._arSubjClass(j).NrOfClassDocs += CType(1, Short)
            '                                    If item.RQResultItemType = RQQueryResult.RQResultItem.RQItemType.bookmark Then Me._arSubjClass(j).NrOfRefLinks += CType(1, Short)
            '                                    arContainsClassString(j) = True
            '                                End If
            '                                item.ItemDescription.Classification.Content = New Utilities.ClassString(item.ItemDescription.Classification.Content).CompleteClassString(clClassString.Item(i), Me._arSubjClass(j).ClassCode)
            '                            End If
            '                        End If
            '                    Next
            '                    If Not arIsInSubClass(i) Then
            '                        arIsInSuperClass(i) = True
            '                    End If
            '                End If
            '            End If
            '        Next
            '        For i = 0 To clClassString.Count - 1
            '            If arIsInSuperClass(i) And Not arIsInSubClass(i) Then
            '                iSuperClassDocCount += 1
            '                Exit For
            '            End If
            '        Next
            '    Catch ex As Exception
            '        Dim test As String
            '        test = ex.Message
            '    End Try
            'Next
            'If ResultSet.Update() <> 0 Then
            '    EditGlobals.Message += "Error occured on DocRefCount update for class " & Me._arSubjClass(0).ClassID & ".</br>"
            '    bErr = False
            'Else
            '    EditGlobals.Message += "DocRefCounts have been updated for class " & Me._arSubjClass(0).ClassID & ".</br>"
            'End If
            'Return bErr
        End Function


        Public Function GetSubClassTable() As RQDataSet.SystematikDataTable
            Dim drTable As RQDataSet.SystematikDataTable
            Dim i As Integer

            If (Me._arSubjClass.Length >= 2) And Not IsNothing(Me._arSubjClass(1)) Then
                drTable = New RQDataSet.SystematikDataTable
                For i = 1 To Me._arSubjClass.Length - 1
                    If Not IsNothing(Me._arSubjClass(i)) Then
                        drTable.AddSystematikRow(Me._arSubjClass(i).ParentClassID, _
                                                 Me._arSubjClass(i).ClassCode, _
                                                 Me._arSubjClass(i).ClassShortTitle, _
                                                 Me._arSubjClass(i).RefRVKSet, _
                                                 Me._arSubjClass(i).ClassLongTitle, _
                                                 Me._arSubjClass(i).NrOfSubClasses, _
                                                 Me._arSubjClass(i).NrOfClassDocs, _
                                                 Me._arSubjClass(i).NrOfRefLinks)
                        If CInt(Me._arSubjClass(i).ClassID) > 0 Then
                            CType(drTable.Rows(i - 1), RQDataSet.SystematikRow).ID = CInt(Me._arSubjClass(i).ClassID)
                        End If
                    End If
                Next
            Else
                Dim mqQuery As New RQDAL.RQCatalogDAL

                drTable = CType(mqQuery.GetRecordByParentID(Me._arSubjClass(0).ClassID, "RQDataSet", "Systematik", True), RQDataSet.SystematikDataTable)
            End If
            Return drTable
        End Function


        Public Function IsValid() As Boolean
            Dim MajClass As SubjClass = Me._arSubjClass(0)
            Dim RetValue As Boolean = True
            Dim i As Integer

            For i = 1 To Me._arSubjClass.Length - 1
                If Not IsNothing(Me._arSubjClass(i)) Then
                    Dim MinClass As SubjClass = Me._arSubjClass(i)

                    RetValue = RetValue And MinClass.IsValid(MajClass)
                End If
            Next
            If RetValue = True Then
                For i = 1 To Me._arSubjClass.Length - 1
                    If Not IsNothing(Me._arSubjClass(i)) Then
                        Dim j As Integer

                        For j = i + 1 To Me._arSubjClass.Length - 1
                            If Not IsNothing(Me._arSubjClass(j)) Then
                                Dim FirstMinClass As SubjClass = Me._arSubjClass(i)
                                Dim SecndMinClass As SubjClass = Me._arSubjClass(j)

                                If FirstMinClass.Intersects(SecndMinClass) Then
                                    EditGlobals.Message += "<p class='smalltext'>SUBCLASS " + FirstMinClass.ClassCode + " INTERSECTS SUBCLASS " + SecndMinClass.ClassCode + "</p>"
                                    RetValue = False
                                End If
                            End If
                        Next
                    End If
                Next
                If RetValue = True Then
                    EditGlobals.Message += "<p class='comment'>Class branch is consistent.</p>"
                End If
            End If
            Return RetValue
        End Function


        Public Function IsFeasableWith(ByRef newMajClass As SubjClass) As Boolean
            Dim i As Integer

            If Me._arSubjClass.Length > 1 Then
                For i = 1 To Me._arSubjClass.Length - 1
                    Dim ocl As SubjClass = Me._arSubjClass(i)

                    If Not IsNothing(ocl) Then
                        If Not newMajClass.Contains(ocl) Then
                            EditGlobals.Message += "<p class='comment'>ERROR on SubClass " & newMajClass.ClassID & " <p/>"
                            EditGlobals.Message += "<p class='comment'>New definition range is not consistent with definition range of existing subclasses.<p/>"
                            Return False
                        End If
                    End If
                Next
            Else
                Return False
            End If
            Return True
        End Function


        Public Function GetEnumerator() As IEnumerator _
          Implements IEnumerable.GetEnumerator

            Return New SubjClassEnum(_arSubjClass)
        End Function

#End Region

    End Class


    Public Class SubjClassEnum
        Implements IEnumerator


#Region "Private Members"

        Private _SubjClass() As SubjClass

        ' Enumerators are positioned before the first element
        ' until the first MoveNext() call.
        Dim position As Integer = -1

#End Region


#Region "Constructors"

        Public Sub New(ByVal list() As SubjClass)
            _SubjClass = list
        End Sub

#End Region


#Region "Public Methods"

        Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
            position = position + 1
            Return (position < _SubjClass.Length)
        End Function


        Public Sub Reset() Implements IEnumerator.Reset
            position = -1
        End Sub


        Public ReadOnly Property Current() As Object Implements IEnumerator.Current
            Get
                Try
                    Return _SubjClass(position)
                Catch ex As IndexOutOfRangeException
                    Throw New InvalidOperationException()
                End Try
            End Get
        End Property

#End Region

    End Class


    Public Class SubjClassManager

#Region "Public Members"
        Public Enum Classification
            Shelf
            RVK
            JEL
            undefined
        End Enum

#End Region


#Region "Private Members"

        Private Shared _mClassBranches As New Collections.Generic.List(Of SubjClassBranch)
        Private Shared _bPersist As Boolean = True
        Private Shared _eClassification As Classification = Classification.undefined

#End Region


#Region "Properties"

        Public Shared Property Persist() As Boolean
            Get
                Return _bPersist
            End Get
            Set(ByVal value As Boolean)
                _bPersist = value
            End Set
        End Property


        Public Shared ReadOnly Property ClassificationName() As Classification
            Get
                Return _eClassification
            End Get
        End Property

#End Region


#Region "Private Methods"

        'Finds the complete subject class branch of the major subject class with ClassId=majClassID
        Private Shared Function Find(ByRef majClassID As String) As SubjClassBranch
            Dim i As Integer = 0

            For i = 0 To _mClassBranches.Count - 1
                If Not IsNothing(_mClassBranches(i)) Then
                    If _mClassBranches(i).MajorClassID = majClassID Then
                        Return _mClassBranches(i)
                    End If
                End If
            Next

            Dim _cb As SubjClassBranch

            _cb = New SubjClassBranch(CStr(majClassID))
            _cb.Load()
            For i = 0 To _mClassBranches.Count - 1
                If IsNothing(_mClassBranches(i)) Then
                    _mClassBranches(i) = _cb
                    Return _mClassBranches(i)
                End If
            Next
            _mClassBranches.Add(_cb)
            Return _mClassBranches(_mClassBranches.Count - 1)
        End Function

#End Region


#Region "Constructors"

        Public Sub New()
            _mClassBranches = New Collections.Generic.List(Of SubjClassBranch)
        End Sub


        Public Sub New(ByVal ClassificationName As Classification)
            Me.New()
            _eClassification = ClassificationName
        End Sub

#End Region


#Region "Public Methods"

        Public Shared Sub Init(ByRef majClassID As String)
            Clear()
            Find(majClassID)
        End Sub


        Public Shared Sub Clear()
            Dim i As Integer

            For i = 0 To (_mClassBranches.Count - 1)
                If Not IsNothing(_mClassBranches(i)) Then
                    _mClassBranches(i).Clear()
                    _mClassBranches(i) = Nothing
                End If
            Next
        End Sub


        'Gets the minor subject classes as SystematikDataTable of the major subject class with classId=majClassId 
        Public Shared Function GetMinorClasses(ByRef majClassID As String) As RQDataSet.SystematikDataTable
            Return Find(majClassID).GetSubClassTable
        End Function


        'Gets the major class with classId=majClassID
        Public Shared Function GetMajorClass(ByRef majClassID As String) As SubjClass
            Return Find(majClassID).MajorClass
        End Function


        'Replaces the branch of major subject class with classID=majClassId by an array of minor subject classes given as SystematikDataTable 
        Public Shared Function Replace(ByRef majClassId As String, ByVal minClassBranch As RQDataSet.SystematikDataTable) As Boolean
            Dim clb As SubjClassBranch = Find(majClassId)
            Dim RetValue As Boolean = True
            Dim i As Integer

            For i = 0 To (minClassBranch.Rows.Count - 1)
                Dim cb As New SubjClass()

                cb.ClassID = CType(minClassBranch.Rows(i), RQDataSet.SystematikRow).ID
                cb.ParentClassID = CType(minClassBranch.Rows(i), RQDataSet.SystematikRow).ParentID
                cb.ClassCode = CType(minClassBranch.Rows(i), RQDataSet.SystematikRow).DDCNumber
                cb.ClassShortTitle = CType(minClassBranch.Rows(i), RQDataSet.SystematikRow).Description
                cb.ClassLongTitle = CType(minClassBranch.Rows(i), RQDataSet.SystematikRow).RegensburgDesc
                cb.RefRVKSet = CType(minClassBranch.Rows(i), RQDataSet.SystematikRow).RegensburgSign
                If CType(minClassBranch.Rows(i), RQDataSet.SystematikRow).SubClassCount > 0 _
                    And (clb.Item(i + 1).RefRVKSet <> cb.RefRVKSet) Then
                    If Not Find(cb.ClassID).IsFeasableWith(cb) Then
                        cb.RefRVKSet = clb.Item(i + 1).RefRVKSet
                        RetValue = False
                    End If
                End If
                cb.NrOfSubClasses = CType(minClassBranch.Rows(i), RQDataSet.SystematikRow).SubClassCount
                cb.NrOfClassDocs = CType(minClassBranch.Rows(i), RQDataSet.SystematikRow).DocRefCount
                cb.NrOfRefLinks = CInt(CType(minClassBranch.Rows(i), RQDataSet.SystematikRow).DirRefCount)
                If Not IsNothing(clb.Item(i + 1)) Then
                    clb.Item(i + 1) = cb
                Else
                    clb.Add(cb)
                End If
            Next
            Return RetValue
        End Function


        'Updates an array of minor subject classes given as SystematikDataTable of the major subject class with classID=majClassId 
        Public Shared Sub UpdateMinorClasses(ByRef majClassID As String)
            If _bPersist Then
                Find(majClassID).Update()
            End If
        End Sub


        'Deletes the minor subject class with classId=minClassId from the branch of the major subject class with classID=majClassId 
        Public Shared Sub DeleteMinorClass(ByRef majClassID As String, ByRef minClassID As String)
        End Sub


        'Appends an empty minor subject class with optional classId=minClassId to the branch of the major subject class with classID=majClassId 
        Public Shared Sub InsertMinorClass(ByRef majClassID As String, Optional ByRef minClassID As String = "")
            Dim cb As SubjClassBranch

            For Each cb In _mClassBranches
                If Not IsNothing(cb) Then
                    If cb.MajorClassID = majClassID Then
                        cb.Add(New SubjClass())
                        Exit For
                    End If
                End If
            Next
        End Sub


        Public Shared Function GetParentClassID(ByRef majClassID As String) As String
            Dim cb As SubjClassBranch

            For Each cb In _mClassBranches
                If Not IsNothing(cb) Then
                    If cb.MajorClassID = majClassID Then
                        Return cb.Item(0).ParentClassID
                    End If
                End If
            Next
            Return ""
        End Function


        Public Shared Function IsValid(ByRef majClassID As String) As Boolean
            Dim cb As SubjClassBranch

            For Each cb In _mClassBranches
                If Not IsNothing(cb) Then
                    If cb.MajorClassID = majClassID Then
                        Return cb.IsValid
                    End If
                End If
            Next
            Return False
        End Function


        Public Shared Function IsValid(ByRef majClassID As String, ByVal minClassBranch As RQDataSet.SystematikDataTable) As Boolean
            Dim oldClb As SubjClassBranch = Find(majClassID)
            Dim newClb As New SubjClassBranch(oldClb.Item(0), minClassBranch)

            If newClb.IsValid And Replace(majClassID, minClassBranch) Then
                Return True
            End If
            Return False
        End Function

#End Region

    End Class

End Namespace
