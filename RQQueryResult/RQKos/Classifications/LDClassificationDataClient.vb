'Imports RQLinkedData
'Imports RQWebAccess.RQLinkedDataClients.ClassificationData

Namespace RQQueryResult.DescriptionObjects

    Public Class LDClassificationDataClient

        'private LDBase _ldbase;


        Public Sub New()

        End Sub


        Public Function GetAltLabel(ByVal classSystem As ClassificationCode.ClassificationSystems, ByVal classNotation As String) As String
            'string uri = ClassificationSystemClient.GetURI(classSystem);

            'If (classNotation.StartsWith(uri)) Then
            'return this._ldbase.ObjectOf(classNotation, ClassificationSystemClient.GetPredicate(classSystem, ClassificationSystemClient.ClassificationPredicates.alternative_label))[0];
            'Else
            'return this._ldbase.ObjectOf(uri + "/" + ClassificationSystemClient.AdaptClassNotation(classSystem, classNotation), ClassificationSystemClient.GetPredicate(classSystem, ClassificationSystemClient.ClassificationPredicates.alternative_label))[0];
            Return Nothing
        End Function


        Public Function GetPrefLabel(ByVal classSystem As ClassificationCode.ClassificationSystems, ByVal classNotation As String) As String
            'string uri = ClassificationSystemClient.GetURI(classSystem);

            'If (classNotation.StartsWith(Uri)) Then
            '    return this._ldbase.ObjectOf(classNotation, ClassificationSystemClient.GetPredicate(classSystem, ClassificationSystemClient.ClassificationPredicates.preferred_label))[0];
            'Else
            '    return this._ldbase.ObjectOf(uri + "/" + ClassificationSystemClient.AdaptClassNotation(classSystem, classNotation), ClassificationSystemClient.GetLabelPredicate(classSystem))[0];
            Return Nothing
        End Function


        Public Function GetBroaderClassNotation(ByVal classSystem As ClassificationCode.ClassificationSystems, ByVal classNotation As String) As String
            'string uri = ClassificationSystemClient.GetURI(classSystem);

            'If (classNotation.StartsWith(Uri)) Then
            '    return this._ldbase.ObjectOf(classNotation, ClassificationSystemClient.GetPredicate(classSystem, ClassificationSystemClient.ClassificationPredicates.broader_term))[0];
            'Else
            '    return this._ldbase.ObjectOf(uri + "/" + ClassificationSystemClient.AdaptClassNotation(classSystem, classNotation), ClassificationSystemClient.GetPredicate(classSystem,ClassificationSystemClient.ClassificationPredicates.broader_term))[0];
            Return Nothing
        End Function

    End Class

End Namespace
