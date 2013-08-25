Imports Microsoft.VisualBasic
Imports RQLib.RQLD
Imports System.Xml
Imports System.Runtime.Serialization


Namespace RQQueryResult.RQDescriptionElements

    <DataContract()> _
    Public MustInherit Class RQDescriptionComponent

#Region "Private Members"

        Private _isComplete As Boolean = False

#End Region


#Region "Protected Members"

        Protected _intID As Integer = -1                          'Internal component ID
        Protected _strID As String = ""                           'Semantic Web component ID
        Protected _code As String = ""                            'Coded identifier (f. e. classification notation or PND-Id)
        Protected _friendlyName As String = ""                    'Friendly component name
        Protected _localName As String = ""                       'Local name space + ':' + Code
        Protected _localNameSpace As String = ""
        Protected _intNrOfDocuments As Integer = -1
        Protected _intNrOfRefLinks As Integer = -1
        Protected _dataClient As ComponentDataClient = Nothing

        Protected Enum ValidEnum
            invalid
            valid
            undefined
        End Enum

        Protected _isValid As ValidEnum = ValidEnum.undefined

#End Region


#Region "Public Properties"

        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public Overridable Property LocalName() As String
            Get
                Return Me._localName
            End Get
            Set(ByVal value As String)
                Me._localName = value
            End Set
        End Property


        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public Overridable Property LocalNameSpace() As String
            Get
                Return Me._localNameSpace
            End Get
            Set(ByVal value As String)
                Me._localNameSpace = value
            End Set
        End Property


        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public Overridable Property NrOfClassDocs() As Integer
            Get
                Return Me._intNrOfDocuments
            End Get
            Set(ByVal value As Integer)
                _intNrOfDocuments = value
                Me._isValid = ValidEnum.undefined
            End Set
        End Property


        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public Overridable Property NrOfRefLinks() As Integer
            Get
                Return Me._intNrOfRefLinks
            End Get
            Set(ByVal value As Integer)
                _intNrOfRefLinks = value
                Me._isValid = ValidEnum.undefined
            End Set
        End Property


        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public Overridable Property DataClient() As ComponentDataClient
            Get
                Return Me._dataClient
            End Get
            Set(ByVal value As ComponentDataClient)
                Me._dataClient = value
            End Set
        End Property


        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public ReadOnly Property RDFGraph() As RQClassificationGraph
            Get
                Try
                    If Not IsNothing(Me.DataClient.LDGraph) Then
                        Return Me.DataClient.LDGraph
                    Else
                        Return New RQClassificationGraph(Me)
                    End If
                Catch ex As ArgumentNullException
                    Return New RQClassificationGraph(Me)
                End Try
            End Get
        End Property


        <IgnoreDataMember()> _
        <Xml.Serialization.XmlIgnore()> _
        Public ReadOnly Property IsComplete() As Boolean
            Get
                Return Me._isComplete
            End Get
        End Property

#End Region


#Region "Protected Properties"



#End Region


#Region "Protected Methods"

        Protected MustOverride Function RetrieveId() As String


        Protected MustOverride Sub Read()


        Protected MustOverride Function Write() As Integer


        Protected Friend Sub SetComplete()
            Me._isComplete = True
        End Sub

#End Region


#Region "Constructors"

        Public Sub New()
            Me._isValid = ValidEnum.undefined
        End Sub


        Public Sub New(ByVal localName As String, ByVal isName As Boolean)
            Me.New()
            Me.LocalName = localName
        End Sub

#End Region


#Region "Public Methods"

        Public Sub Load()
            If Me._intID <> 0 Or Me._strID <> "" Then
                Me.Read()
            End If
        End Sub


        Public Function Save() As Integer
            If Me._intID <> 0 Then
                Return Me.Write()
            End If
            Return 1
        End Function


        Public Sub EnableLinkedData()
            If Not IsNothing(Me.DataClient) Then Me.DataClient.IsLinkedDataEnabled = True
        End Sub


        Public Sub DisableLinkedData()
            If Not IsNothing(Me.DataClient) Then Me.DataClient.IsLinkedDataEnabled = False
        End Sub

#End Region

    End Class

End Namespace
