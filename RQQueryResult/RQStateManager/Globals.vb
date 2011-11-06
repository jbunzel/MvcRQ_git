Imports Microsoft.VisualBasic
Imports RQLib.RQStateManager.RQSessionState


Namespace RQStateManager

    Public Module Globals

        'User Preferences (to be re-implemented as user state variables)
        Public bLinkViewContexts As Boolean = True          'the context of the present view is transferred to the new view

        'Session States
        Public NavigationState As RQNavigationState
        Public QueryState As RQQueryState
        Public ResultDocState As RQResultDocState
        Public ClassificationState As RQClassificationState
        Public EditState As RQEditState
    End Module

End Namespace
