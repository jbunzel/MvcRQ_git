Imports RQLib.RQLD

Namespace RQKos.Persons

    Public MustInherit Class PersonDataClient
        Inherits RQQueryResult.RQDescriptionElements.ComponentDataClient

        Protected _personDataSystem As Person.PersonDataSystems = Person.PersonDataSystems.unknown


        Public Property PersonDataSystem() As Person.PersonDataSystems
            Get
                Return Me._personDataSystem
            End Get
            Set(ByVal value As Person.PersonDataSystems)
                Me._personDataSystem = value
            End Set
        End Property


        Public MustOverride Function GetPersonId(ByVal personCode As String) As String


        Public MustOverride Sub GetPersonData(ByRef thePerson As Person)


        Public MustOverride Sub PutPersonData(ByRef thePerson As Person)


        Public MustOverride Function UpdateDocRefs(ByRef thePerson As Person) As Boolean

    End Class

End Namespace
