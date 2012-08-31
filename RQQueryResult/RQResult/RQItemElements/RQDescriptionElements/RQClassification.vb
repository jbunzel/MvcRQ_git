Imports Microsoft.VisualBasic
Imports RQLib.RQKos.Classifications
Imports System.Runtime.Serialization


Namespace RQQueryResult.RQDescriptionElements

    <DataContract()>
    <KnownType(GetType(SubjClass))> _
    Public Class RQClassification
        Inherits RQDescriptionElement
        Implements Collections.IEnumerable

#Region "Private Members"

        Private _classCodes As New List(Of SubjClass)(1)

#End Region


#Region "Private Methods"

        Private Sub BuildContentString()
            If Not IsNothing(Me._classCodes) Then
                Dim i As Integer = 0

                _content = ""
                For i = 0 To Me._classCodes.Count - 1
                    If Not IsNothing(Me._classCodes(i)) Then _content += Me._classCodes(i).LocalName + "; "
                Next
            End If
        End Sub


        Private Sub BuildClassCodeArray()
            Dim splits() As String = {"; ", ";"}
            Dim splitContent As String() = _content.Split(splits, StringSplitOptions.RemoveEmptyEntries)
            Dim i As Integer = 0

            Me._classCodes.Clear()
            Me._classCodes.TrimExcess()
            For i = 0 To splitContent.Count - 1
                Me._classCodes.Add(New SubjClass(splitContent.ElementAt(i), True))
                If Me.IsLinkedDataEnabled() Then
                    Me._classCodes(i).EnableLinkedData()
                End If
                Me._classCodes.Capacity = i + 1
            Next
        End Sub


        'Private Sub BuildClassCodeArray()
        '    Dim splits() As String = {"; ", ";"}
        '    Dim splitContent As String() = _content.Split(splits, StringSplitOptions.RemoveEmptyEntries)
        '    Dim i As Integer = 0
        '    Dim cn, co As String

        '    For i = 0 To Math.Max(Me._classCodes.Count - 1, splitContent.Count - 1)
        '        Try
        '            cn = splitContent.ElementAt(i)
        '            Try
        '                co = Me._classCodes(i).LocalName
        '            Catch ex As IndexOutOfRangeException
        '                'Array.Resize(Me._classCodes, i + 1)
        '                Me._classCodes.Capacity = i + 1
        '                co = ""
        '            End Try
        '            If cn <> co Then
        '                'Me._classCodes(i) = New SubjClass(cn, True) 'True indicates c is LocalName
        '                Me._classCodes.Add(New SubjClass(cn, True))
        '                If Me.IsLinkedDataEnabled() Then
        '                    Me._classCodes(i).EnableLinkedData()
        '                End If
        '            End If
        '        Catch ex As ArgumentOutOfRangeException
        '            'Array.Resize(Me._classCodes, i)
        '            Me._classCodes.Capacity = i + 1
        '        End Try
        '    Next
        'End Sub

#End Region


#Region "Public Properties"

        Public ReadOnly Property count() As Integer
            Get
                If Not IsNothing(Me._classCodes) Then
                    Return Me._classCodes.Count
                Else
                    Return 0
                End If
            End Get
        End Property


        <DataMember()> _
        <Xml.Serialization.XmlElement()> _
        Public Property Dummy As String
            Get
                Return "DUMMY"
            End Get
            Set(ByVal value As String)

            End Set
        End Property


        <DataMember()> _
        <Xml.Serialization.XmlElement()> _
        Public Property items As List(Of SubjClass)
            Get
                Return Me._classCodes
            End Get
            Set(ByVal value As List(Of SubjClass))
                Me._classCodes = value
            End Set
        End Property


        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public Overrides Property Content() As String
            Get
                Me.BuildContentString()
                Return _content
            End Get
            Set(ByVal value As String)
                Dim classString As New Utilities.ClassString(value)

                value = classString.CompleteClassString()
                If (_content <> value) Then
                    _content = value
                    Me.BuildClassCodeArray()
                End If
            End Set
        End Property

#End Region


#Region "Public Constructors"

        Public Sub New()
            MyBase.New()
        End Sub


        Public Sub New(ByVal Content As String, Optional ByVal EnableLinkedData As Boolean = False)
            MyBase.New(Content, EnableLinkedData)

            If Content <> "" Then
                Dim splits() As String = {"; ", ";"}
                Dim i As Integer = 0

                For Each c As String In _content.Split(splits, StringSplitOptions.RemoveEmptyEntries)
                    'Array.Resize(Me._classCodes, i + 1)
                    Me._classCodes.Capacity = i + 1
                    'Me._classCodes(i) = New SubjClass(c, True) 'True indicates c is LocalName
                    Me._classCodes.Add(New SubjClass(c, True))
                    If Me.IsLinkedDataEnabled() Then
                        Me._classCodes(i).EnableLinkedData()
                    End If
                    i = i + 1
                Next
            End If
        End Sub

#End Region


#Region "Public Methods"

        Public Overrides Function SyntaxCheck(ByRef message As String) As Boolean
            Return True
        End Function


        Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return New RQClassificationEnum(Me)
        End Function


        Public Function GetItem(ByVal i As Integer) As SubjClass
            If i < count Then
                If Not IsNothing(Me._classCodes(i)) Then
                    Return Me._classCodes(i)
                Else
                    Return Nothing
                End If
            End If
            Return Nothing
        End Function


        Public Sub Add(ByVal obj As System.Object)

        End Sub


        'Public Overrides Sub WriteXml(ByVal writer As Xml.XmlWriter)
        '    Dim cc As SubjClass

        '    For Each cc In Me._classCodes
        '        cc.WriteXml(writer)
        '    Next
        'End Sub


        'Public Overrides Sub ReadXml(ByVal reader As Xml.XmlReader)

        'End Sub


        'Public Overrides Function GetSchema() As Xml.Schema.XmlSchema
        '    Return Nothing
        'End Function

#End Region

    End Class


    Public Class RQClassificationEnum
        Implements IEnumerator


#Region "Private Members"

        Private _classification As RQClassification

        ' Enumerators are positioned before the first element
        '        ' until the first MoveNext() call.
        Dim position As Integer = -1

#End Region


#Region "Constructors"

        Public Sub New(ByRef theClass As RQClassification)
            Me._classification = theClass
        End Sub

#End Region


#Region "Public Methods"

        Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
            position = position + 1
            Return (position < Me._classification.count)
        End Function


        Public Sub Reset() Implements IEnumerator.Reset
            position = -1
        End Sub


        Public ReadOnly Property Current() As Object Implements IEnumerator.Current
            Get
                Try
                    Return Me._classification.GetItem(position)
                Catch ex As IndexOutOfRangeException
                    Throw New InvalidOperationException()
                End Try
            End Get
        End Property

#End Region

    End Class

End Namespace
