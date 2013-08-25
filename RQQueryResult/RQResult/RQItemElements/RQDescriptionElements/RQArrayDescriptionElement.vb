Imports Microsoft.VisualBasic
Imports RQLib.RQKos.Classifications
Imports System.Runtime.Serialization


Namespace RQQueryResult.RQDescriptionElements

    <DataContract()>
    <KnownType(GetType(SubjClass))> _
    Public MustInherit Class RQArrayDescriptionElement
        Inherits RQDescriptionElement
        Implements Collections.IEnumerable

#Region "Private Members"

        Private _array As New List(Of RQDescriptionComponent)(1)

#End Region


#Region "Protected Methods"

        Protected Sub BuildContentString()
            If Not IsNothing(Me._array) Then
                Dim i As Integer = 0

                MyBase._content = ""
                For i = 0 To Me._array.Count - 1
                    If Not IsNothing(Me._array(i)) Then MyBase._content += Me._array(i).LocalName + "; "
                Next
            End If
        End Sub


        Protected Sub BuildClassCodeArray()
            Dim splits() As String = {"; ", ";"}
            Dim splitContent As String() = _content.Split(splits, StringSplitOptions.RemoveEmptyEntries)
            Dim i As Integer = 0

            Me._array.Clear()
            Me._array.TrimExcess()
            For i = 0 To splitContent.Count - 1
                Dim contentComponent As String = splitContent.ElementAt(i).Trim()

                If (Not String.IsNullOrEmpty(contentComponent)) And (i < splitContent.Count - 1) Then
                    'Hier besser Regex einsetzen, um typische Eingabefehler abzufangen (z. B. [, ], ( => etc.)
                    While (splitContent.ElementAt(i + 1).Contains("(=>"))
                        Try
                            Dim str As String = splitContent.ElementAt(i + 1)
                            Dim str1 As String = str.Substring(0, str.IndexOf("("))
                            Dim str2 As String = str.Substring(str.IndexOf("{"), str.IndexOf("}") - str.IndexOf("{") + 1)
                            contentComponent += "; " + str1 + " " + str2
                        Catch ex As ArgumentException

                        End Try
                        i = i + 1
                        If i = splitContent.Count - 1 Then Exit While
                    End While
                End If
                Me._array.Add(CreateNewComponent(contentComponent))
                If Me.IsLinkedDataEnabled() Then
                    Me._array(Me._array.Count - 1).EnableLinkedData()
                End If
                Me._array.Capacity += 1
            Next
        End Sub

#End Region


#Region "Public Properties"

        Public ReadOnly Property count() As Integer
            Get
                If Not IsNothing(Me._array) Then
                    Return Me._array.Count
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
        Public Property items As List(Of RQDescriptionComponent)
            Get
                Return Me._array
            End Get
            Set(ByVal value As List(Of RQDescriptionComponent))
                Me._array = value
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
                If (_content <> value) Then
                    _content = value
                    Me.BuildClassCodeArray()
                End If
            End Set
        End Property

#End Region


#Region "Protected Methods"

        Protected MustOverride Function CreateNewComponent(ByVal contentComponent As String) As RQDescriptionComponent

#End Region


#Region "Public Constructors"

        Public Sub New()
            MyBase.New()
        End Sub


        Public Sub New(ByVal Content As String, Optional ByVal EnableLinkedData As Boolean = False)
            MyBase.New(Content, EnableLinkedData)

            If Content <> "" Then
                BuildClassCodeArray()
            End If
        End Sub

#End Region


#Region "Public Methods"

        Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return New RQArrayDescriptionElementEnum(Me)
        End Function


        Public Function GetItem(ByVal i As Integer) As SubjClass
            If i < count Then
                If Not IsNothing(Me._array(i)) Then
                    Return Me._array(i)
                Else
                    Return Nothing
                End If
            End If
            Return Nothing
        End Function


        Public Sub Add(ByVal obj As System.Object)

        End Sub

#End Region

    End Class


    Public Class RQArrayDescriptionElementEnum
        Implements IEnumerator


#Region "Private Members"

        Private _array As RQArrayDescriptionElement

        ' Enumerators are positioned before the first element
        '        ' until the first MoveNext() call.
        Dim position As Integer = -1

#End Region


#Region "Constructors"

        Public Sub New(ByRef theElement As RQArrayDescriptionElement)
            Me._array = theElement
        End Sub

#End Region


#Region "Public Methods"

        Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
            position = position + 1
            Return (position < Me._array.count)
        End Function


        Public Sub Reset() Implements IEnumerator.Reset
            position = -1
        End Sub


        Public ReadOnly Property Current() As Object Implements IEnumerator.Current
            Get
                Try
                    Return Me._array.GetItem(position)
                Catch ex As IndexOutOfRangeException
                    Throw New InvalidOperationException()
                End Try
            End Get
        End Property

#End Region

    End Class

End Namespace
