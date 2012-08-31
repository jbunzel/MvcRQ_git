Imports RQLib.RQLD

Namespace RQKos.Classifications

    Public MustInherit Class ClassificationDataClient

        Protected _classSystem As SubjClass.ClassificationSystems = SubjClass.ClassificationSystems.unkown
        Protected _skosGraph As RQSkosGraph = Nothing
        Private _LDEnabled As Boolean = False


        Public Property SkosGraph() As RQSkosGraph
            Get
                Return Me._skosGraph
            End Get
            Set(ByVal value As RQSkosGraph)
                Me._skosGraph = value
            End Set
        End Property


        Public Property ClassSystem() As SubjClass.ClassificationSystems
            Get
                Return Me._classSystem
            End Get
            Set(ByVal value As SubjClass.ClassificationSystems)
                Me._classSystem = value
            End Set
        End Property


        Public Property IsLinkedDataEnabled() As Boolean
            Get
                Return Me._LDEnabled
            End Get
            Set(ByVal value As Boolean)
                Me._LDEnabled = value
            End Set
        End Property


        Public MustOverride Function GetClassId(ByVal classNotation As String) As String


        Public MustOverride Sub GetClassData(ByRef theClass As SubjClass)


        Public MustOverride Sub PutClassData(ByRef theClass As SubjClass)


        Public MustOverride Sub GetNarrowerClassData(ByVal majClassId As Integer, ByRef classBranch As SubjClassBranch)


        Public MustOverride Sub GetNarrowerClassData(ByVal majClassId As String, ByRef classBranch As SubjClassBranch)


        Public MustOverride Function UpdateDocRefs(ByRef classBranch As SubjClassBranch, Optional ByVal iSuperClassDocCount As Integer = 0) As Boolean

    End Class

End Namespace
