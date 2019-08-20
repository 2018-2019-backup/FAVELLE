
Imports System
Imports System.Collections.Generic
Imports System.Runtime.Serialization

Imports System.Text

Imports Autodesk.Connectivity.WebServices




''' <summary>
''' A container for an LfCycDef object which can be displayed in a list box
''' </summary>
Public Class LfCycDefListItem
    Public LfCycDef As LfCycDef

    Public Sub New(ByVal lfCycDef As LfCycDef)
        Me.LfCycDef = lfCycDef
    End Sub

    Public Overrides Function ToString() As String
        Return LfCycDef.DispName
    End Function
End Class

''' <summary>
''' A container for an LfCycState object which can be displayed in a list box
''' </summary>
Public Class LfCycStateListItem
    Public LfCycState As LfCycState

    Public Sub New(ByVal lfCycState As LfCycState)
        Me.LfCycState = lfCycState
    End Sub

    Public Overrides Function ToString() As String
        Return LfCycState.DispName
    End Function
End Class

''' <summary>
''' A container for an LfCycTrans object which can be displayed in a list box
''' </summary>
Public Class LfCycTransListItem
    Public DispName As String
    Public LfCycTrans As LfCycTrans

    Public Sub New(ByVal lfCycTrans As LfCycTrans)
        Me.LfCycTrans = lfCycTrans
    End Sub

    Public Overrides Function ToString() As String
        Return Me.DispName
    End Function
End Class


''' <summary>
''' A container for an Workflow object which can be displayed in a list box
''' </summary>
Public Class WorkflowListItem
    Public Workflow As Workflow

    Public Sub New(ByVal workflowDef As Workflow)
        Me.Workflow = workflowDef
    End Sub

    Public Overrides Function ToString() As String
        Return Workflow.DispName
    End Function

End Class
''' <summary>
''' A container for an WorkflowState object which can be displayed in a list box
''' </summary>
Public Class WorkflowStateListItem
    Public WorkflowState As WorkflowState

    Public Sub New(ByVal workflowState As WorkflowState)
        Me.WorkflowState = workflowState
    End Sub

    Public Overrides Function ToString() As String
        Return WorkflowState.DispName
    End Function
End Class
''' <summary>
''' A container for an Activity object which can be displayed in a list box
''' </summary>
Public Class WorkflowActivityListItem
    Public DispName As String
    Public Activity As Activity

    Public Sub New(ByVal activity As Activity)
        Me.Activity = activity
    End Sub

    Public Overrides Function ToString() As String
        Return Me.DispName
    End Function
End Class


