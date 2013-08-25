Imports Microsoft.VisualBasic
Imports System.IO
Imports System.Xml

Namespace RQLucene


    Public Class StrandDocLoader

        Public Shared Function Load(ByVal file As FileInfo) As StrandDoc
            Dim strTemp, strExt As String

            strTemp = file.Name
            strExt = LCase(Right(strTemp, Len(strTemp) - InStrRev(strTemp, ".", -1, CompareMethod.Text)))
            If strExt = "xml" Then
                Try
                    Dim tstDoc As New XmlDocument()

                    tstDoc.Load(file.FullName)
                    If tstDoc.DocumentType.SystemId.Contains("jbarticle.dtd") Then
                        Return New StrandDoc(file)
                    End If
                Catch ex As Exception
                End Try
            End If
            Return New StrandDoc()
        End Function
    End Class


    Public Class StrandSection

#Region "Private Members"

        Private _title As String = String.Empty
        Private _content As String = String.Empty
        Private _uri As String = String.Empty

#End Region


#Region "Public Properties"

        Protected Friend Property Title() As String
            Get
                Return _title
            End Get
            Set(ByVal value As String)
                _title = value
            End Set
        End Property


        Protected Friend Property Content() As String
            Get
                Return _content
            End Get
            Set(ByVal value As String)
                _content = value
            End Set
        End Property


        Protected Friend Property DocNo() As String
            Get
                Return _uri
            End Get
            Set(ByVal value As String)
                _uri = value
            End Set
        End Property

#End Region


    End Class


    Public Class StrandDoc

#Region "Private Members"

        Private _title As String = String.Empty
        Private _content As String = String.Empty
        Private _uri As String = String.Empty
        Private _sectlist() As StrandSection = Nothing

#End Region


#Region "Public Properties"

        Public ReadOnly Property Count() As Integer
            Get
                If Not IsNothing(_sectlist) Then
                    Return _sectlist.Length
                Else
                    Return 0
                End If
            End Get
        End Property


        Public ReadOnly Property Items() As StrandSection()
            Get
                Return _sectlist
            End Get
        End Property


        Public ReadOnly Property Items(ByVal i As Integer) As StrandSection
            Get
                If _sectlist.Length > i Then
                    Return _sectlist(i)
                Else
                    Return Nothing
                End If
            End Get
        End Property


        Public ReadOnly Property Title() As String
            Get
                Return _title
            End Get
        End Property


        Public ReadOnly Property Content() As String
            Get
                Return _content
            End Get
        End Property


        Public ReadOnly Property Uri() As String
            Get
                Return _uri
            End Get
        End Property

#End Region


#Region "Private Methods"

        Private Function TraverseXML(ByVal DocItr As XPath.XPathNodeIterator, Optional ByVal Sect1Count As Integer = 0, Optional ByRef ParentContent As String = "") As Boolean
            Dim bRetVal As Boolean = False

            While (DocItr.MoveNext)
                Dim TagName As String = DocItr.Current.Name
                Dim bSkip As Boolean = False

                Select Case TagName
                    Case "Sect1"
                        Sect1Count += 1
                        _sectlist(Sect1Count - 1) = New StrandSection()
                        _sectlist(Sect1Count - 1).DocNo = Me.Uri + "&amp;S=" + CStr(Sect1Count)
                    Case "Title"
                        If DocItr.Current.Matches("Sect1/Title") Then
                            _sectlist(Sect1Count - 1).Title = DocItr.Current.Value.Replace(ControlChars.NewLine, " ")
                        End If
                        bSkip = True
                    Case Else
                        If DocItr.Current.GetAttribute("Language", "").ToLower = "english" Then
                            bSkip = True
                        Else
                            If DocItr.Current.NodeType = XPath.XPathNodeType.Text Then
                                Dim strCont As String = DocItr.Current.Value.Replace(ControlChars.NewLine, " ")

                                strCont = strCont.Replace(ControlChars.Tab, "")
                                strCont = strCont.Replace("  ", " ")
                                _sectlist(Sect1Count - 1).Content += " " + strCont
                            End If
                        End If
                End Select
                If Not bSkip Then
                    If Not TraverseXML(DocItr.Current.SelectChildren(XPath.XPathNodeType.All), Sect1Count, "") Then
                        bRetVal = True
                    End If
                End If
            End While
            Return bRetVal
        End Function


        Private Sub Parse(ByVal doc As XPath.XPathDocument)
            Dim xmlDocNav As XPath.XPathNavigator = doc.CreateNavigator()
            Dim xmlDocItr As XPath.XPathNodeIterator = xmlDocNav.Select("Article/Sect1")

            Me._title = xmlDocNav.SelectSingleNode("Article/Title").Value
            Me._content = xmlDocItr.Current.Value
            _sectlist = New StrandSection(xmlDocItr.Count - 1) {}
            TraverseXML(xmlDocItr)
        End Sub

#End Region


#Region "Public Constructors"

        Public Sub New()
        End Sub


        Public Sub New(ByVal file As FileInfo)
            Me._uri = file.Directory.Name + "/" + file.Name
            Parse(New XPath.XPathDocument(file.FullName, XmlSpace.Default))
        End Sub

#End Region


#Region "Public Functions"

        Public Function HasSubsect() As Boolean
            Return Me._sectlist.Length() > 0
        End Function


        Public Function HasContent() As Boolean
            Return Me._content <> String.Empty
        End Function

#End Region

    End Class


    Public Class StrandDocEnum
        Implements IEnumerator


#Region "Private Members"

        Private _stranddoc As StrandDoc

        ' Enumerators are positioned before the first element
        ' until the first MoveNext() call.
        Dim position As Integer = -1

#End Region


#Region "Constructors"

        Public Sub New(ByRef doc As StrandDoc)
            _stranddoc = doc
        End Sub

#End Region


#Region "Public Methods"

        Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
            position = position + 1
            Return (position < _stranddoc.Count)
        End Function


        Public Sub Reset() Implements IEnumerator.Reset
            position = -1
        End Sub


        Public ReadOnly Property Current() As Object Implements IEnumerator.Current
            Get
                Try
                    'Return _rqResultItems(position)
                    Return _stranddoc.Items(position)
                Catch ex As IndexOutOfRangeException
                    Throw New InvalidOperationException()
                End Try
            End Get
        End Property

#End Region

    End Class


End Namespace
