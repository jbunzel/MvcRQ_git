Imports System.Math
Imports System.Text.RegularExpressions
Imports RQLib.RQDAL

Namespace Utilities

    Public Class ClassSubRange

#Region "Private Members"

        Private _Min As String
        Private _Max As String
        Private Const LexMinChar As Char = " "c
        Private Const LexMaxChar As Char = "Z"c

#End Region


#Region "Public Properties"

        Public Property LowerBound() As String
            Get
                Return _Min
            End Get
            Set(ByVal Value As String)
                _Min = Value
            End Set
        End Property

        Public Property UpperBound() As String
            Get
                Return _Max
            End Get
            Set(ByVal Value As String)
                _Max = Value
            End Set
        End Property

#End Region


#Region "Constructors"

        Public Sub New(ByVal DefClassSubRange As String)
            Dim ClassRange As String() = DefClassSubRange.Split("-"c)

            If ClassRange.Length > 0 Then
                If Not IsNothing(ClassRange(0)) And ClassRange(0).Trim.Length > 0 Then
                    LowerBound = ClassRange(0).Trim()
                Else
                    LowerBound = "INVALID"
                End If
            End If
            If ClassRange.Length > 1 Then
                If Not IsNothing(ClassRange(1)) And ClassRange(1).Trim.Length > 0 Then
                    UpperBound = ClassRange(1).Trim()
                Else
                    UpperBound = "INVALID"
                End If
            Else
                UpperBound = ClassRange(0).Trim()
            End If
        End Sub

#End Region


#Region "Public Methods"

        Public Function Contains(ByVal TestClassSubRange As ClassSubRange) As Boolean
            Dim b As Boolean = True

            b = b And LexicalGreaterOrEqual(TestClassSubRange.LowerBound, LowerBound) And LexicalLowerOrEqual(TestClassSubRange.LowerBound, UpperBound)
            b = b And LexicalGreaterOrEqual(TestClassSubRange.UpperBound, LowerBound) And LexicalLowerOrEqual(TestClassSubRange.UpperBound, UpperBound)
            Return b
        End Function


        Public Function IsInRange(ByVal TestClassSubRange As String) As Boolean
            Dim ClassRange As String() = TestClassSubRange.Split("-"c)
            Dim b As Boolean = True

            If ClassRange.Length >= 1 Then
                If ClassRange(0).Length > 0 Then
                    b = b And LexicalGreaterOrEqual(ClassRange(0), LowerBound) And LexicalLowerOrEqual(ClassRange(0), UpperBound)
                End If
            End If
            If ClassRange.Length >= 2 Then
                If ClassRange(1).Length > 0 Then
                    b = b And LexicalGreaterOrEqual(ClassRange(1), LowerBound) And LexicalLowerOrEqual(ClassRange(1), UpperBound)
                End If
            End If
            Return b
        End Function


        Public Function LexicalGreaterOrEqual(ByVal str1 As String, ByVal str2 As String) As Boolean
            Dim b As Boolean = True

            str1 = str1.Trim
            str2 = str2.Trim
            If str1.Length > 0 And str2.Length > 0 Then
                Dim i As Integer

                If str1.Length > str2.Length Then
                    str2 = str2.PadRight(str1.Length, LexMinChar)
                End If
                If str2.Length > str1.Length Then
                    str1 = str1.PadRight(str2.Length, LexMinChar)
                End If
                For i = 0 To str1.Length - 1
                    b = b And (str1.Chars(i) >= str2.Chars(i))
                    If b And (str1.Chars(i) > str2.Chars(i)) Then
                        Exit For
                    End If
                Next
                Return b
            Else
                Return False
            End If
        End Function


        Public Function LexicalLowerOrEqual(ByVal str1 As String, ByVal str2 As String) As Boolean
            Dim b As Boolean = True

            str1 = str1.Trim
            str2 = str2.Trim
            If str1.Length > 0 And str2.Length > 0 Then
                Dim i As Integer

                If str1.Length > str2.Length Then
                    str2 = str2.PadRight(str1.Length, LexMaxChar)
                End If
                If str2.Length > str1.Length Then
                    str1 = str1.PadRight(str2.Length, LexMaxChar)
                End If
                For i = 0 To str1.Length - 1
                    b = b And (str1.Chars(i) <= str2.Chars(i))
                    If b And (str1.Chars(i) < str2.Chars(i)) Then
                        Exit For
                    End If
                Next
                Return b
            Else
                Return False
            End If
        End Function


        Public Function Expand(Optional ByVal Junktor As String = ";", Optional ByVal Prefix As String = "", Optional ByVal Postfix As String = "") As String
            Dim strExpand As String = _Min + Postfix
            Dim strAdd As String = _Min
            Dim i As Integer = 0
            Dim j As Integer = strAdd.Length - 1
            Dim z As Integer

            If _Min <> _Max Then
                While strAdd.Chars(i) = _Max.Chars(i)
                    If i < Min(strAdd.Length - 1, _Max.Length - 1) Then i = i + 1 Else Exit While
                End While
                While j >= i + 1
                    If Char.ToUpper(strAdd.Chars(j)) < "Z"c Or strAdd.Chars(j) < "9"c Then
                        Dim k As Integer

                        z = Asc("9"c)
                        If Char.IsLetter(strAdd.Chars(j)) Then z = Asc("Z"c)
                        For k = Asc(strAdd.Chars(j)) + 1 To z
                            strAdd = strAdd.Substring(0, j) + Chr(k)
                            strExpand += Junktor + Prefix + strAdd + Postfix
                        Next
                    End If
                    j = j - 1
                End While
                strAdd = strAdd.Substring(0, j)
                z = Asc(_Min.Chars(i)) + 1
                While z <= Asc(_Max.Chars(i))   'has been <, which did not work for number ranges; now, the max of alpha ranges is doubled in expand!
                    strAdd = strAdd.Substring(0, i) + Chr(z)
                    strExpand += Junktor + Prefix + strAdd + Postfix
                    z = z + 1
                End While
                j = i + 1
                If j <= _Max.Length - 1 Then
                    While (j <= _Max.Length - 1)
                        If (Char.ToUpper(_Max.Chars(j)) <> "Z"c) And (_Max.Chars(j) <> "9"c) Then
                            Dim k As Integer

                            z = Asc("0"c)
                            If Char.IsLetter(_Max.Chars(j)) Then z = Asc("A"c)
                            For k = z To Asc(_Max.Chars(j))
                                strAdd = _Max.Substring(0, j) + Chr(k)
                                strExpand += Junktor + Prefix + strAdd + Postfix
                            Next
                        End If
                        j = j + 1
                    End While
                Else
                    strExpand += Junktor + Prefix + _Max + Postfix
                End If
                i = _Max.Length - 1
            End If
            Return Prefix + strExpand
        End Function


        Public Function Contract() As String
            Return ""
        End Function

#End Region

    End Class


    Public Class LexicalClass

#Region "Private Members"

        Private ClassRange(10) As ClassSubRange

#End Region


#Region "Constructors"

        Sub New(ByVal DefClassRange As String)
            Dim DefClassSubRange As String() = DefClassRange.Split(";"c)

            If DefClassSubRange.Length > 0 Then
                Dim i As Integer = 0

                If DefClassSubRange.Length > ClassRange.Length Then
                    ReDim ClassRange(DefClassSubRange.Length)
                End If
                For i = 0 To DefClassSubRange.Length - 1
                    If Not IsNothing(DefClassSubRange(i)) And DefClassSubRange(i).Length > 0 Then
                        ClassRange(i) = New ClassSubRange(DefClassSubRange(i))
                    End If
                Next
            End If
        End Sub

#End Region


#Region "Public Methods"

        Public Function Expand(Optional ByVal Junktor As String = ";", Optional ByVal Prefix As String = "", Optional ByVal Postfix As String = "") As String
            Dim SubRange As ClassSubRange
            Dim strExpand As String = ""
            Dim strPrefix As String = ""

            For Each SubRange In ClassRange
                If Not IsNothing(SubRange) Then
                    strExpand += strPrefix + SubRange.Expand(Junktor, Prefix, Postfix)
                    strPrefix = Junktor
                End If
            Next
            Return strExpand + ";"
        End Function


        Public Function IsValid() As Boolean
            Dim SubRange As ClassSubRange
            Dim b As Boolean = True

            For Each SubRange In ClassRange
                If Not IsNothing(SubRange) Then
                    b = b And SubRange.LexicalGreaterOrEqual(SubRange.UpperBound, SubRange.LowerBound) And SubRange.LexicalLowerOrEqual(SubRange.LowerBound, SubRange.UpperBound)
                    If Not b Then
                        Exit For
                    End If
                End If
            Next
            Return b
        End Function


        Public Function IsInRange(ByVal TestClassRange As String) As Boolean
            Dim DefClassSubRange As String() = TestClassRange.Split(";"c)

            If DefClassSubRange.Length > 0 Then
                Dim b0 As Boolean = True
                Dim i As Integer

                For i = 0 To DefClassSubRange.Length - 1
                    If DefClassSubRange(i) <> "" Then
                        Dim b1 As Boolean = False
                        Dim j As Integer

                        For j = 0 To ClassRange.Length - 1
                            If Not IsNothing(ClassRange(j)) Then
                                b1 = b1 Or ClassRange(j).IsInRange(DefClassSubRange(i))
                            End If
                        Next
                        b0 = b0 And b1
                    End If
                Next
                Return b0
            Else
                Return False
            End If
        End Function


        Public Function Intersects(ByVal TestClass As LexicalClass) As Boolean
            Dim SubRange As ClassSubRange
            Dim b As Boolean = False

            For Each SubRange In ClassRange
                If Not IsNothing(SubRange) Then

                    Dim OtherSubRange As ClassSubRange

                    For Each OtherSubRange In TestClass.ClassRange
                        If Not IsNothing(OtherSubRange) Then
                            b = b Or (SubRange.LexicalGreaterOrEqual(SubRange.LowerBound, OtherSubRange.LowerBound) And SubRange.LexicalLowerOrEqual(SubRange.LowerBound, OtherSubRange.UpperBound))
                            b = b Or (OtherSubRange.LexicalGreaterOrEqual(OtherSubRange.LowerBound, SubRange.LowerBound) And OtherSubRange.LexicalLowerOrEqual(OtherSubRange.LowerBound, SubRange.UpperBound))
                            If b Then
                                Exit For
                            End If
                        End If
                    Next
                    If b Then
                        Exit For
                    End If
                End If
            Next
            Return b
        End Function


        Public Function Contains(ByVal TestClass As LexicalClass) As Boolean
            Dim SubRange, TestSubRange As ClassSubRange
            Dim b As Boolean

            For Each TestSubRange In TestClass.ClassRange
                If Not IsNothing(TestSubRange) Then
                    b = False
                    For Each SubRange In Me.ClassRange
                        If Not IsNothing(SubRange) Then
                            If SubRange.Contains(TestSubRange) Then
                                b = True
                                Exit For
                            End If
                        End If
                    Next
                    If Not b Then Exit For
                End If
            Next
            Return b
        End Function

#End Region

    End Class


    Public Class ClassString

#Region "Private Members"
        Private _ClassCodes As ArrayList = New ArrayList
        'Variable had to be defined, because of problems with '§' in code inline strings
        Private ClassCodePrefix As String = CStr(Chr(167)) + CStr(Chr(167)) + "RVK" + CStr(Chr(167)) + CStr(Chr(167)) + ":"
#End Region


#Region "Public Properties"

        Public Property Value() As String
            Get
                Dim strCl As String
                Dim strRet As String = ""

                For Each strCl In _ClassCodes
                    If Not IsNothing(strCl) And strCl.Trim.Length > 0 Then
                        strRet += strCl + "; "
                    End If
                Next
                Return strRet.TrimEnd(" "c)
            End Get
            Set(ByVal Value As String)
                Dim ClassElements As String() = CType(Value, String).Trim.Split(";"c)
                Dim i As Integer

                For i = 0 To CType((ClassElements.Length > 0), Integer)
                    If Not IsNothing(ClassElements(0)) And ClassElements(0).Trim.Length > 0 Then
                        _ClassCodes.Add(ClassElements(0).Trim())
                    End If
                Next
            End Set
        End Property


        Public Property Item(ByVal Index As Integer) As String
            Get
                Return CStr(_ClassCodes.Item(Index))
            End Get
            Set(ByVal Value As String)
                _ClassCodes.Insert(Index, Value)
            End Set
        End Property


        Public ReadOnly Property Count() As Integer
            Get
                Return _ClassCodes.Count
            End Get
        End Property

#End Region


#Region "Constructors"

        Public Sub New(ByVal strClassString As String)
            Dim ClassElements As String() = strClassString.Trim.Split(";"c)
            Dim i As Integer

            For i = 0 To ClassElements.Length - 1
                If Not IsNothing(ClassElements(i)) And ClassElements(i).Trim.Length > 0 Then
                    _ClassCodes.Add(ClassElements(i).Trim())
                End If
            Next
        End Sub

#End Region


#Region "Public Methods"

        Public Function UpdateClassStringFor(ByVal NewClassName As String, ByVal ClassMapCode As String) As String
            Dim i As Integer

            For i = 0 To _ClassCodes.Count - 1
                Dim strCl As String = CStr(_ClassCodes(i))

                If ClassMapCode = strCl Then
                    If i > 0 Then
                        If NewClassName.StartsWith(CType(_ClassCodes(i - 1), String)) Then
                            _ClassCodes(i - 1) = NewClassName
                        Else
                            _ClassCodes.Insert(i, NewClassName)
                        End If
                        Exit For
                    End If
                End If
            Next
            Return Value
        End Function


        Public Function UpdateClassString(ByVal NewClassName As String, ByVal ClassMap As LexicalClass) As String
            Dim i As Integer

            For i = 0 To _ClassCodes.Count - 1
                Dim strCl As String = CStr(_ClassCodes(i))

                If strCl.StartsWith(ClassCodePrefix) Then
                    strCl = strCl.Remove(0, 8)
                    If ClassMap.IsInRange(strCl) Then
                        If i > 0 Then
                            If NewClassName.StartsWith(CType(_ClassCodes(i - 1), String)) Then
                                _ClassCodes(i - 1) = NewClassName
                            Else
                                _ClassCodes.Insert(i, NewClassName)
                            End If
                            Exit For
                        End If
                    End If
                End If
            Next
            Return Value
        End Function


        Public Function CompleteClassString(ByVal ClassCode As String, ByVal ShelfCode As String) As String
            'if the class string contains (RVK) classification code ClassCode
            'then ShelfCode is inserted in front resp. substituted for existing shelf code in front
            Dim i As Integer = 0

            For i = 0 To Me._ClassCodes.Count - 1
                If CStr(Me._ClassCodes.Item(i)) = ClassCode Then
                    If i = 0 Then
                        Me._ClassCodes.Insert(0, ShelfCode)
                    ElseIf Me._ClassCodes.Item(i - 1) <> ShelfCode Then
                        If CStr(Me._ClassCodes.Item(i - 1)).Contains(ClassCodePrefix) Then
                            Me._ClassCodes.Insert(i, ShelfCode)
                        Else
                            Me._ClassCodes.Item(i - 1) = ShelfCode
                        End If
                    End If
                    Exit For
                End If
            Next
            Return Me.Value
        End Function


        Public Function CompleteClassString() As String
            'if the class string contains RVK classification codes
            'the corresponding MyClassification codes are inserted

            If Value.IndexOf(ClassCodePrefix, StringComparison.InvariantCulture) >= 0 Then
                Dim i As Integer = _ClassCodes.Count - 1

                'Testfunktion, um Behandlung der §§ Zeichen zu überprüfen
                Dim b1 As Char() = CType(Value, Char())
                Dim b2 As Char() = CType(ClassCodePrefix, Char())

                'Remove any shelf (MyClassification) codes
                While i >= 0
                    Dim strCl As String = CStr(_ClassCodes(i))

                    If Not Regex.IsMatch(strCl, "§§*§§*") Then 'If Not strCl.StartsWith(CStr(ClassCodePrefix)) Then
                        _ClassCodes.RemoveAt(i)
                    End If
                    i += -1
                End While

                'Lookup Systematik entries for RVK classification code
                If _ClassCodes.Count > 0 Then
                    Dim SysQuery As New RQCatalogDAL
                    Dim j As Integer = _ClassCodes.Count

                    i = 0
                    While j > 0
                        Dim strCl As String = CStr(_ClassCodes(i))

                        If strCl.StartsWith(ClassCodePrefix) Then
                            Dim strClassName As String

                            strCl = strCl.Remove(0, 8)
                            strClassName = SysQuery.GetMyClassNameFromRVKCode(strCl, "RQDataSet", True)
                            If strClassName <> "" Then
                                If strClassName.StartsWith("ERROR") Then
                                    Return strClassName
                                Else
                                    _ClassCodes.Insert(i, strClassName)
                                    i += 1
                                End If
                            End If
                        End If
                        j += -1
                        i += 1
                    End While
                End If
            End If
            Return Value
        End Function

#End Region

    End Class

End Namespace