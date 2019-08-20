Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace FavelleVaultTools
    Public Interface ILifeCycleEvent
        Sub CommitChanges()
        Sub AddJobEvent()
        Sub DeleteJobEvent()
        Sub PopulateUI()

        ReadOnly Property HasPendingChanges() As Boolean
        ReadOnly Property CanAddJobEvent() As Boolean
        ReadOnly Property CanDeleteJobEvent() As Boolean
        ReadOnly Property IsRunning() As Boolean
    End Interface
End Namespace


