Imports Microsoft.VisualBasic
Imports RQLib.RQLD
Imports System.Xml
Imports System.Runtime.Serialization
Imports System.Text.RegularExpressions


Namespace RQKos.Persons

    <DataContract()> _
    Public Class Person
        Inherits RQLib.RQQueryResult.RQDescriptionElements.RQDescriptionComponent

        Public Class PersonName
            Private _surname As String = ""
            Private _firstname As String = ""
            Private _nickname As String = ""
            Private _attribute As String = ""


            Public Property SurName() As String
                Get
                    Return Me._surname
                End Get
                Set(value As String)
                    Me._surname = value
                End Set
            End Property


            Public Property FirstName() As String
                Get
                    Return Me._firstname
                End Get
                Set(value As String)
                    Me._firstname = value
                End Set
            End Property


            Public Property NickName() As String
                Get
                    Return Me._nickname
                End Get
                Set(value As String)
                    Me._nickname = value
                End Set
            End Property


            Public Property Attribute() As String
                Get
                    Return Me._attribute
                End Get
                Set(value As String)
                    Me._attribute = value
                End Set
            End Property


            Public ReadOnly Property FriendlyName() As String
                Get
                    FriendlyName = Me.SurName
                    If (Not String.IsNullOrEmpty(Me.FirstName)) Then FriendlyName += ", " + Me.FirstName
                    If (Not String.IsNullOrEmpty(Me.NickName)) Then FriendlyName += " """ + Me.NickName + """"
                    If (Not String.IsNullOrEmpty(Me.Attribute)) Then FriendlyName += " <" + Me.Attribute + ">"
                End Get
            End Property


            Sub New()

            End Sub


            Sub New(ByVal surName As String, ByVal firstName As String, ByVal attribute As String)
                Me._surname = surName
                Me._firstname = firstName
                Me._attribute = attribute
            End Sub

        End Class


        Public Class PersonReference

            Public Enum PersonReferenceType
                alternative
                authority
                pseudonym
                realname
                role
                unknown
            End Enum

            Private _type As PersonReferenceType = PersonReferenceType.unknown
            Private _value As PersonName


            Public Property ReferenceType() As PersonReferenceType
                Get
                    Return Me._type
                End Get
                Set(value As PersonReferenceType)
                    Me._type = value
                End Set
            End Property


            Public ReadOnly Property ReferenceTypeText() As String
                Get
                    Select Case Me._type
                        Case PersonReferenceType.alternative
                            Return "Verw."
                        Case PersonReferenceType.authority
                            Return "Ans."
                        Case PersonReferenceType.pseudonym
                            Return "Pseud."
                        Case PersonReferenceType.role
                            Return "Rolle"
                        Case PersonReferenceType.realname
                            Return "wirkl. Name"
                        Case Else
                            Return ""
                    End Select
                    Return Me._type
                End Get
            End Property


            Public Property ReferenceValue() As PersonName
                Get
                    Return Me._value
                End Get
                Set(value As PersonName)
                    Me._value = value
                End Set
            End Property


            Sub New(ByVal reference As PersonName, ByVal referenceType As PersonReferenceType)
                Me._value = reference
                Me._type = referenceType
            End Sub

        End Class

#Region "Public Enumerations"

        Public Enum PersonDataSystems
            gnd
            rq
            unknown
        End Enum

#End Region


#Region "Private Members"

        Private _personDataSystem As PersonDataSystems = PersonDataSystems.unknown

        Private _name As New PersonName()

        Private _roles As New List(Of PersonReference)()

        Private _lifedata As String = ""

        Private _function As String = ""

#End Region


#Region "Public Properties"

        <DataMember()> _
        <Xml.Serialization.XmlElement()> _
        Public Property PersonID() As String
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
        Public Property PersonDataSystem() As PersonDataSystems
            Get
                Return Me._personDataSystem
            End Get
            Set(ByVal value As PersonDataSystems)
                Me._personDataSystem = value
                Select Case value
                    Case PersonDataSystems.gnd
                        Me._localNameSpace = "§§GND§§"
                        If IsNothing(Me.PersonDataClient) Then Me.PersonDataClient = New LDPersonDataClient(value)
                    Case PersonDataSystems.rq
                        Me._localNameSpace = ""
                        If IsNothing(Me.PersonDataClient) Then Me.PersonDataClient = New RQPersonDataClient()
                    Case Else
                        If IsNothing(Me.PersonDataClient) Then Me.PersonDataClient = New RQPersonDataClient()
                End Select
            End Set
        End Property


        <DataMember()> _
        <Xml.Serialization.XmlElement()> _
        Public Property PersonCode() As String
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
        Public Property PersonalName() As String
            Get
                If String.IsNullOrEmpty(Me._friendlyName) Then
                    If Me._code <> "" And Me._intID < 0 Then Me.Read()
                    Me._friendlyName = Me._name.FriendlyName
                    If (Not String.IsNullOrEmpty(Me._lifedata)) Then Me._friendlyName += " (" + Me._lifedata + ")"
                    If (Not String.IsNullOrEmpty(Me._function)) Then Me._friendlyName += " {" + Me._function + "}"
                    If (Not IsNothing(Me._roles)) Then
                        Dim i As Integer = 0

                        For i = 0 To Me._roles.Count - 1
                            Me._friendlyName += "; " + Me._roles.ElementAt(i).ReferenceValue.FriendlyName + " (=> {" + Me._roles.ElementAt(i).ReferenceTypeText + "})"
                        Next
                    End If
                End If
                Return Me._friendlyName
            End Get
            Set(ByVal value As String)
                Me._friendlyName = value
            End Set
        End Property


        '<DataMember()> _
        '<Xml.Serialization.XmlElement()> _
        'Public Property ClassLongTitle() As String
        '    Get
        '        If Me._code <> "" And Me._intID < 0 Then Me.Read()
        '        Return Me._strClassLongTitle
        '    End Get
        '    Set(ByVal value As String)
        '        Me._strClassLongTitle = value
        '    End Set
        'End Property


        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public Overrides Property LocalName() As String
            Get
                If Me._localName = "" Then
                    Me._localName = Me.PersonalName
                    'If (Not String.IsNullOrEmpty(Me._LifeData)) Then Me._localName += " (" + Me._LifeData + ")"
                    'If (Not String.IsNullOrEmpty(Me._function)) Then Me._localName += " {" + Me._function + "}"
                    'If (Not IsNothing(Me._roles)) Then
                    '    Dim i As Integer = 0

                    '    For i = 0 To Me._roles.Count - 1
                    '        Me._localName += "; " + Me._roles.ElementAt(i).ReferenceValue.FriendlyName + " (=> {" + Me._roles.ElementAt(i).ReferenceTypeText + "})"
                    '    Next
                    'End If
                    If (Not String.IsNullOrEmpty(Me.PersonCode)) Then Me._localName += "; " + Me.LocalNameSpace + ":" + Me.PersonCode + " (=> {PID})"
                End If
                Return Me._localName
            End Get
            Set(ByVal value As String)
                Me._localName = value
            End Set
        End Property


        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public Overrides Property LocalNameSpace() As String
            Get
                If Me._localNameSpace = "" Then
                    Select Case Me.PersonDataSystem
                        Case PersonDataSystems.gnd
                            Me._localNameSpace = "§§GND§§"
                        Case Else

                    End Select
                End If
                Return Me._localNameSpace
            End Get
            Set(ByVal value As String)
                Me._localNameSpace = value
                Select Case value.ToUpper()
                    Case "§§GND§§"
                        Me.PersonDataSystem = PersonDataSystems.gnd
                    Case ""
                        Me.PersonDataSystem = PersonDataSystems.rq
                    Case Else
                        Me.PersonDataSystem = PersonDataSystems.unknown
                End Select
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


        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public Property PersonDataClient() As PersonDataClient
            Get
                Return Me.DataClient
            End Get
            Set(ByVal value As PersonDataClient)
                Me.DataClient = value
            End Set
        End Property

#End Region


#Region "Private Methods"
        ''' <summary>
        ''' Parses string matching regular expression
        ''' </summary>
        ''' <param name="_string">String to be parsed</param>
        ''' <param name="_regularExpression">Regular expresion to match</param>
        ''' <returns>String array matching regular expression</returns>
        ''' <remarks></remarks>
        Public Function ParseByRegex(ByVal _string As String, ByVal _regularExpression As String) As Match
            Dim regex As New Regex(_regularExpression, RegexOptions.IgnoreCase Or RegexOptions.Singleline)
            Dim matches As Match = regex.Match(_string)

            If (matches.Success) Then
                'Dim stringArray(matches.Groups.Count - 2) As String
                ''Skips 0 because regex.Match(0) is original string
                'For i = 1 To matches.Groups.Count - 1
                '    stringArray(i - 1) = matches.Groups(i).ToString
                'Next
                'Return stringArray
                Return matches
            End If
            Return Nothing
        End Function


        Private Sub ParseLocalNameInput(localNameInput As String)
            Dim splits() As String = {"; ", ";"}
            Dim splitContent As String() = localNameInput.Split(splits, StringSplitOptions.RemoveEmptyEntries)
            Dim regexStr As String = "(?<surname>(\w+-*\s*)+)(,\s(?<firstname>(""?\w+""?\s*)+))?(\s*<(?<attribute>(.*))>)?(\s*\((?<lifedata>(.*))\))?(\s*{(?<function>(.*))})?"
            Dim result As Match = ParseByRegex(splitContent(0), regexStr)

            If Not IsNothing(result) Then
                Me._name.SurName = result.Groups("surname").ToString().Trim()
                Me._name.FirstName = result.Groups("firstname").ToString().Trim()
                Me._name.Attribute = result.Groups("attribute").ToString().Trim()
                Me._lifedata = result.Groups("lifedata").ToString().Trim()
                Me._function = result.Groups("function").ToString().Trim()
            End If

            If splitContent.Count > 1 Then
                Dim i As Integer

                For i = 1 To splitContent.Count - 1
                    If splitContent(i).Contains("{PID}") Then
                        result = ParseByRegex(splitContent(i), "(?<namespace>(.*)):(?<code>(.*))\s+.*")

                        If Not IsNothing(result) Then
                            Me.LocalNameSpace = result.Groups("namespace").ToString().Trim()
                            Me.PersonCode = result.Groups("code").ToString().Trim()
                        End If
                    Else
                        'result = ParseByRegex(splitContent(1), "(?<reference>(\w+))(\s<(?<attribute>(.*))>)?(\s+{(?<type>(.*))})?")
                        result = ParseByRegex(splitContent(i), regexStr)

                        If Not IsNothing(result) Then
                            If ("Role Rolle Interpr. Interpret Komponist Photogr.").Contains(result.Groups("function").ToString()) Then
                                Dim reference As New PersonName(result.Groups("surname").ToString().Trim(), "", result.Groups("attribute").ToString().Trim())

                                Me._roles.Add(New PersonReference(reference, PersonReference.PersonReferenceType.role))
                            Else
                                Dim reference As New PersonName(result.Groups("surname").ToString().Trim(),
                                                                result.Groups("firstname").ToString().Trim(),
                                                                result.Groups("attribute").ToString().Trim())

                                If ("Wirkl.Name Wirkl. Name urspr. Name").Contains(result.Groups("function").ToString()) Then
                                    Me._roles.Add(New PersonReference(reference, PersonReference.PersonReferenceType.realname))
                                End If
                                If ("Ans.").Contains(result.Groups("function").ToString()) Then
                                    Me._roles.Add(New PersonReference(reference, PersonReference.PersonReferenceType.authority))
                                End If
                                If ("Verw.").Contains(result.Groups("function").ToString()) Then
                                    Me._roles.Add(New PersonReference(reference, PersonReference.PersonReferenceType.alternative))
                                End If
                                If ("Pseud.").Contains(result.Groups("function").ToString()) Then
                                    Me._roles.Add(New PersonReference(reference, PersonReference.PersonReferenceType.pseudonym))
                                End If
                            End If
                        End If
                    End If

                Next
            End If
        End Sub

#End Region


#Region "Protected Methods"

        Protected Overrides Function RetrieveId() As String
            Try
                RetrieveId = CType(Me.DataClient, PersonDataClient).GetPersonId(Me.PersonCode)
            Catch ex As Exception
                RetrieveId = ""
            End Try
        End Function


        Protected Overrides Sub Read()
            If Me._intID < 0 And Me._strID = "" Then
                Me.PersonID = RetrieveId()
            End If
            If Me._intID > 0 Or Me._strID <> "" Then
                'no class of RQClassificationSystem has ID=0. ClassID=0 is used to retrieve the outermost subjClassBranch
                If Not IsNothing(Me.DataClient) Then CType(Me.DataClient, PersonDataClient).GetPersonData(Me)
            End If
        End Sub


        Protected Overrides Function Write() As Integer
            If Me._intID > 0 Then
                'no class of RQClassificationSystem has ID=0. ClassID=0 is used to retriev the outermost subjClassBranch
                If Not IsNothing(Me.DataClient) Then CType(Me.DataClient, PersonDataClient).PutPersonData(Me)
            End If
            Return 1
        End Function

#End Region


#Region "Constructors"

        Public Sub New()
            Me._isValid = ValidEnum.undefined
        End Sub


        Public Sub New(ByRef personID As String)
            Me.New(personID, DirectCast(Nothing, PersonDataClient))
        End Sub


        Public Sub New(ByVal personID As String, ByVal dataClient As PersonDataClient)
            Me.New()
            Me.DataClient = dataClient
            If personID.StartsWith("http://") Then
                Me.PersonID = -1
                Me._strID = personID
                Me.PersonDataSystem = LDPersonDataClient.GetPersonDataSystem(personID)
            Else
                Me.PersonID = personID
                Me.PersonDataSystem = PersonDataSystems.rq
            End If
        End Sub


        Public Sub New(ByVal personCode As String, ByVal personDataSystem As Person.PersonDataSystems)
            Me.New()
            Me.PersonCode = personCode
            Me.PersonDataSystem = personDataSystem
            Me.PersonID = CType(Me.DataClient, PersonDataClient).GetPersonId(Me.PersonCode)
        End Sub


        Public Sub New(ByVal localName As String, ByVal isName As Boolean)
            Me.New()
            ParseLocalNameInput(localName)
        End Sub

#End Region


#Region "Public Methods"

        'Public Function IsValid(ByRef MajClass As SubjClass) As Boolean
        '    If Me._isValid = ValidEnum.undefined Then
        '        If Not MajClass.Contains(Me) Then
        '            EditGlobals.Message += "<p class='smalltext'>ERROR ON SUBCLASS " + Me.ClassID + "</p><p class='smalltext'>Subclass is not contained in class or class code is invalid.</p>"
        '            Me._isValid = ValidEnum.invalid
        '        ElseIf (Me.NrOfSubClasses > 0) And ((Me.ClassCode = "") Or (Me.RefRVKSet = "")) Then
        '            EditGlobals.Message += "<p class='smalltext'>ERROR ON SUBCLASS " + Me.ClassID + "</p><p class='smalltext'>Class containing subclasses must not be deleted.</p>"
        '            Me._isValid = ValidEnum.invalid
        '        ElseIf Not Me.RefRVKClass.IsValid() Then
        '            EditGlobals.Message += "<p class='smalltext'>ERROR ON SUBCLASS " + Me.ClassID + "</p><p class='smalltext'>Invalid RVK class codes.</p>"
        '            Me._isValid = ValidEnum.invalid
        '        Else
        '            Me._isValid = ValidEnum.valid
        '        End If
        '    End If
        '    Return Me._isValid = ValidEnum.valid
        'End Function

#End Region

    End Class


    Public Class PersonEnum
        Implements IEnumerator


#Region "Private Members"

        Private _person() As Person

        ' Enumerators are positioned before the first element
        ' until the first MoveNext() call.
        Dim position As Integer = -1

#End Region


#Region "Constructors"

        Public Sub New(ByVal list() As Person)
            _person = list
        End Sub

#End Region


#Region "Public Methods"

        Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
            position = position + 1
            Return (position < _person.Length)
        End Function


        Public Sub Reset() Implements IEnumerator.Reset
            position = -1
        End Sub


        Public ReadOnly Property Current() As Object Implements IEnumerator.Current
            Get
                Try
                    Return _person(position)
                Catch ex As IndexOutOfRangeException
                    Throw New InvalidOperationException()
                End Try
            End Get
        End Property

#End Region

    End Class

End Namespace
