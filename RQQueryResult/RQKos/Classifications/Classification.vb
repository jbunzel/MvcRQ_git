Imports Microsoft.VisualBasic
Imports RQLib.RQLD
Imports System.Xml
Imports System.Runtime.Serialization


Namespace RQKos.Classifications

    <DataContract()> _
    Public Class SubjClass
        Inherits RQLib.RQQueryResult.RQDescriptionElements.RQDescriptionComponent

#Region "Public Enumerations"

        Public Enum ClassificationSystems
            ddc
            rvk
            jel
            rq
            oldrq
            unknown
        End Enum

#End Region


#Region "Private Members"

        Private _classSystem As ClassificationSystems = ClassificationSystems.unknown
        Private _strClassLongTitle As String = ""
        Private _intNrOfSubClasses As Integer = -1
        Private _classPath As String = ""
        Private _intParentClassID As Integer = -1
        Private _broaderClass As SubjClass = Nothing
        Private _narrowerClasses() As SubjClass
        Private _strRefRVKSet As String = ""


        'Private Enum ValidEnum
        '    invalid
        '    valid
        '    undefined
        'End Enum


        'Private _isValid As ValidEnum = ValidEnum.undefined

#End Region


#Region "Public Properties"

        <DataMember()> _
        <Xml.Serialization.XmlElement()> _
        Public Property ClassID() As String
            Get
                If Me._code <> "" And Me._intID < 0 And Me._strID = "" Then Me.Read()
                If Me._intID < 0 Then
                    Return Me._strID
                Else
                    Return CStr(Me._intID)
                End If
            End Get
            Set(ByVal value As String)
                Try
                    If CInt(value) <> Me._intID Then
                        Me._intID = CInt(value)
                        Me._isValid = ValidEnum.undefined
                    End If
                Catch ex As InvalidCastException
                    If value <> Me._strID Then
                        Me._strID = value
                        Me._isValid = ValidEnum.undefined
                    End If
                End Try
            End Set
        End Property


        <DataMember()> _
        <Xml.Serialization.XmlElement()> _
        Public Property ClassificationSystem() As ClassificationSystems
            Get
                Return Me._classSystem
            End Get
            Set(ByVal value As ClassificationSystems)
                Me._classSystem = value
                Select Case value
                    Case SubjClass.ClassificationSystems.rvk
                        Me._localNameSpace = "§§RVK§§"
                        If IsNothing(Me.ClassDataClient) Then Me.ClassDataClient = New LDClassificationDataClient(value)
                    Case SubjClass.ClassificationSystems.jel
                        Me._localNameSpace = "§§JEL§§"
                        If IsNothing(Me.ClassDataClient) Then Me.ClassDataClient = New LDClassificationDataClient(value)
                    Case SubjClass.ClassificationSystems.ddc
                        Me._localNameSpace = "§§DDC§§"
                        If IsNothing(Me.ClassDataClient) Then Me.ClassDataClient = New LDClassificationDataClient(value)
                    Case SubjClass.ClassificationSystems.oldrq
                        Me._localNameSpace = ""
                        If IsNothing(Me.ClassDataClient) Then Me.ClassDataClient = New RQClassificationDataClient()
                    Case SubjClass.ClassificationSystems.rq
                        Me._localNameSpace = ""
                        If IsNothing(Me.ClassDataClient) Then Me.ClassDataClient = New RQClassificationDataClient()
                    Case Else
                        If IsNothing(Me.ClassDataClient) Then Me.ClassDataClient = New RQClassificationDataClient()
                End Select
            End Set
        End Property


        <DataMember()> _
        <Xml.Serialization.XmlElement()> _
        Public Property ClassCode() As String
            Get
                If Me._intID >= 0 And Me._code = "" Then Me.Read()
                Return Me._code
            End Get
            Set(ByVal value As String)
                Me._code = value
                Me._isValid = ValidEnum.undefined
            End Set
        End Property


        <DataMember()> _
        <Xml.Serialization.XmlElement()> _
        Public Property ClassShortTitle() As String
            Get
                If Me._code <> "" And Me._intID < 0 Then Me.Read()
                Return Me._friendlyName
            End Get
            Set(ByVal value As String)
                Me._friendlyName = value
            End Set
        End Property


        <DataMember()> _
        <Xml.Serialization.XmlElement()> _
        Public Property ClassLongTitle() As String
            Get
                If Me._code <> "" And Me._intID < 0 Then Me.Read()
                Return Me._strClassLongTitle
            End Get
            Set(ByVal value As String)
                Me._strClassLongTitle = value
            End Set
        End Property


        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public Overrides Property LocalName() As String
            Get
                If Me._localName = "" Then
                    Me._localName = Me._localNameSpace
                    If Me._localNameSpace.Length > 2 Then
                        Me._localName += ":" + Me._code
                    Else
                        Me._localName += Me._code
                    End If
                End If
                Return Me._localName
            End Get
            Set(ByVal value As String)
                Me._localName = value
                If value.IndexOf("§§") >= 0 And value.IndexOf(":") < 0 Then
                    ' Old RQ Classification of type "§§4"
                    LocalNameSpace = "§§"
                    If Me.ClassCode = "" Then
                        Me.ClassCode = value.Remove(0, 2).Trim()
                    End If
                Else
                    LocalNameSpace = value.Substring(0, value.IndexOf(":") - value.IndexOf("§§"))
                    If Me.ClassCode = "" Then
                        If (value.IndexOf(":") - value.IndexOf("§§")) > 0 Then
                            Me.ClassCode = value.Remove(0, value.IndexOf(":") - value.IndexOf("§§") + 1)
                        Else
                            Me.ClassCode = Me._localName
                        End If
                    End If
                End If
            End Set
        End Property


        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public Overrides Property LocalNameSpace() As String
            Get
                If Me._localNameSpace = "" Then
                    Select Case Me.ClassificationSystem
                        Case ClassificationSystems.rvk
                            Me._localNameSpace = "§§RVK§§"
                        Case ClassificationSystems.jel
                            Me._localNameSpace = "§§JEL§§"
                        Case ClassificationSystems.ddc
                            Me._localNameSpace = "§§DDC§§"
                        Case ClassificationSystems.oldrq
                            Me._localNameSpace = "§§"
                    End Select
                End If
                Return Me._localNameSpace
            End Get
            Set(ByVal value As String)
                Me._localNameSpace = value
                Select Case value.ToUpper()
                    Case "§§RVK§§"
                        Me.ClassificationSystem = ClassificationSystems.rvk
                    Case "§§JEL§§"
                        Me.ClassificationSystem = ClassificationSystems.jel
                    Case "§§DDC§§"
                        Me.ClassificationSystem = ClassificationSystems.ddc
                    Case "§§"
                        Me.ClassificationSystem = ClassificationSystems.oldrq
                    Case ""
                        Me.ClassificationSystem = ClassificationSystems.rq
                    Case Else
                        Me.ClassificationSystem = ClassificationSystems.unknown
                End Select
            End Set
        End Property


        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public Property NrOfSubClasses() As Integer
            Get
                If Me._code <> "" And Me._intID < 0 Then Me.Read()
                Return Me._intNrOfSubClasses
            End Get
            Set(ByVal value As Integer)
                _intNrOfSubClasses = value
                Me._isValid = ValidEnum.undefined
            End Set
        End Property


        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public Overrides Property NrOfClassDocs() As Integer
            Get
                If Me._code <> "" And Me._intID < 0 Then Me.Read()
                Return Me._intNrOfDocuments
            End Get
            Set(ByVal value As Integer)
                _intNrOfDocuments = value
                Me._isValid = ValidEnum.undefined
            End Set
        End Property


        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public Overrides Property NrOfRefLinks() As Integer
            Get
                If Me._code <> "" And Me._intID < 0 Then Me.Read()
                Return Me._intNrOfRefLinks
            End Get
            Set(ByVal value As Integer)
                _intNrOfRefLinks = value
                Me._isValid = ValidEnum.undefined
            End Set
        End Property


        <DataMember()> _
        <Xml.Serialization.XmlElement()> _
        Public Property ClassPath() As String
            Get
                If (Me._classPath = "") Then
                    Dim cn As SubjClass = Me

                    Do
                        Me._classPath = Me._classPath.Insert(0, "/" + cn.ClassID + "$" + cn.ClassCode)
                        cn = cn.GetBroaderClass()
                    Loop Until (IsNothing(cn))
                End If
                Return Me._classPath
            End Get
            Set(ByVal value As String)
                Me._classPath = value
            End Set
        End Property


        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public Property ParentClassID() As String
            Get
                If Me._code <> "" And Me._intID < 0 Then Me.Read()
                Return CStr(Me._intParentClassID)
            End Get
            Set(ByVal value As String)
                If CInt(value) <> Me._intParentClassID Then
                    Me._intParentClassID = CInt(value)
                    Me._isValid = ValidEnum.undefined
                End If
            End Set
        End Property


        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public Property RefRVKSet() As String
            Get
                If Me._code <> "" And Me._intID < 0 Then Me.Read()
                Return Me._strRefRVKSet
            End Get
            Set(ByVal value As String)
                Me._strRefRVKSet = value
                Me._isValid = ValidEnum.undefined
            End Set
        End Property


        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public Property RefRVKClass() As Utilities.LexicalClass
            Get
                Return New Utilities.LexicalClass(RefRVKSet)
            End Get
            Set(ByVal value As Utilities.LexicalClass)
                Me._strRefRVKSet = value.Expand()
                Me._isValid = ValidEnum.undefined
            End Set
        End Property


        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public Property ClassDataClient() As ClassificationDataClient
            Get
                Return Me.DataClient
            End Get
            Set(ByVal value As ClassificationDataClient)
                Me.DataClient = value
            End Set
        End Property

#End Region


#Region "Private Methods"

        Protected Overrides Function RetrieveId() As String
            Try
                RetrieveId = CType(Me.DataClient, ClassificationDataClient).GetClassId(Me.ClassCode)
            Catch ex As Exception
                RetrieveId = ""
            End Try
        End Function


        Protected Overrides Sub Read()
            If Me._intID < 0 And Me._strID = "" Then
                Me.ClassID = RetrieveId()
            End If
            If Me._intID > 0 Or Me._strID <> "" Then
                'no class of RQClassificationSystem has ID=0. ClassID=0 is used to retrieve the outermost subjClassBranch
                If Not IsNothing(Me.DataClient) Then CType(Me.DataClient, ClassificationDataClient).GetClassData(Me)
            End If
        End Sub


        Protected Overrides Function Write() As Integer
            If Me._intID > 0 Then
                'no class of RQClassificationSystem has ID=0. ClassID=0 is used to retriev the outermost subjClassBranch
                If Not IsNothing(Me.DataClient) Then CType(Me.DataClient, ClassificationDataClient).PutClassData(Me)
            End If
            Return 1
        End Function

#End Region


#Region "Constructors"

        Public Sub New()
            Me._isValid = ValidEnum.undefined
        End Sub


        Public Sub New(ByRef classID As String)
            Me.New(classID, DirectCast(Nothing, ClassificationDataClient))
        End Sub


        Public Sub New(ByVal classID As String, ByVal dataClient As ClassificationDataClient)
            Me.New()
            Me.DataClient = dataClient
            If classID.StartsWith("http://") Then
                Me.ClassID = -1
                Me._strID = classID
                Me.ClassificationSystem = LDClassificationDataClient.GetClassificationSystem(classID)
            Else
                Me.ClassID = classID
                Me.ClassificationSystem = ClassificationSystems.rq
            End If
        End Sub


        Public Sub New(ByVal classCode As String, ByVal classSystem As SubjClass.ClassificationSystems)
            Me.New()
            Me.ClassCode = classCode
            Me.ClassificationSystem = classSystem
            Me.ClassID = CType(Me.DataClient, ClassificationDataClient).GetClassId(Me.ClassCode)
        End Sub


        Public Sub New(ByVal localName As String, ByVal isName As Boolean)
            Me.New()
            Me.LocalName = localName
        End Sub

#End Region


#Region "Public Methods"

        Public Function IsValid(ByRef MajClass As SubjClass) As Boolean
            If Me._isValid = ValidEnum.undefined Then
                If Not MajClass.Contains(Me) Then
                    EditGlobals.Message += "<p class='smalltext'>ERROR ON SUBCLASS " + Me.ClassID + "</p><p class='smalltext'>Subclass is not contained in class or class code is invalid.</p>"
                    Me._isValid = ValidEnum.invalid
                ElseIf (Me.NrOfSubClasses > 0) And ((Me.ClassCode = "") Or (Me.RefRVKSet = "")) Then
                    EditGlobals.Message += "<p class='smalltext'>ERROR ON SUBCLASS " + Me.ClassID + "</p><p class='smalltext'>Class containing subclasses must not be deleted.</p>"
                    Me._isValid = ValidEnum.invalid
                ElseIf Not Me.RefRVKClass.IsValid() Then
                    EditGlobals.Message += "<p class='smalltext'>ERROR ON SUBCLASS " + Me.ClassID + "</p><p class='smalltext'>Invalid RVK class codes.</p>"
                    Me._isValid = ValidEnum.invalid
                Else
                    Me._isValid = ValidEnum.valid
                End If
            End If
            Return Me._isValid = ValidEnum.valid
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


        Public Function GetBroaderClass() As SubjClass
            If IsNothing(Me._broaderClass) Then
                If Me.ParentClassID <> "-1" And Me.ParentClassID <> "0" Then
                    Me._broaderClass = New SubjClass(Me.ParentClassID, Me.ClassDataClient)
                Else
                    Return Nothing
                End If
            End If
            Return Me._broaderClass
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


        Public Sub New(ByRef majClassCode As String, ByVal classSystem As SubjClass.ClassificationSystems)
            Dim _cl As New SubjClass(majClassCode, classSystem)

            _arSubjClass = New SubjClass(10) {}
            _arSubjClass(0) = _cl
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

        Public Sub Add(ByVal aSubjClass As SubjClass)
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
            Dim id As String

            Me._arSubjClass(0).Load()
            id = Me._arSubjClass(0).ClassID
            Try
                Me._arSubjClass(0).ClassDataClient.GetNarrowerClassData(CInt(id), Me)
            Catch ex As InvalidCastException
                Me._arSubjClass(0).ClassDataClient.GetNarrowerClassData(id, Me)
            End Try

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
            Return Me.MajorClass.ClassDataClient.UpdateDocRefs(Me, iSuperClassDocCount)
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
