Imports Autodesk.Connectivity.WebServices
Imports VDF = Autodesk.DataManagement.Client.Framework
Imports Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections
Imports System.IO
Imports System.Data
Imports System.Xml
Imports Autodesk.Connectivity.Extensibility.Framework

<Assembly: Autodesk.Connectivity.Extensibility.Framework.ExtensionId("9a2be05c-54b1-416a-801d-9f5459b7a2ef")> 

' This number gets incremented for each Vault release.
<Assembly: Autodesk.Connectivity.Extensibility.Framework.ApiVersion("9.0")> 

Public Class WsInterface
    Implements IWebServiceExtension
    Dim vConnection As Connection

    Public Sub OnLoad() Implements IWebServiceExtension.OnLoad
        AddHandler ItemService.UpdateItemLifecycleStateEvents.GetRestrictions, AddressOf UpdateItemLifecycleStateEvents_GetRestrictions
    End Sub

    Private Sub UpdateItemLifecycleStateEvents_GetRestrictions(sender As Object, e As Object)
        Dim itemMasterId As Long = DirectCast(e, Autodesk.Connectivity.WebServices.UpdateItemLifeCycleStateCommandEventArgs).ItemMasterIds(0)

        Dim CurrentCredentials As New Autodesk.Connectivity.WebServicesTools.WebServiceCredentials(DirectCast(sender, IWebService))
        Dim ServiceManager As New Autodesk.Connectivity.WebServicesTools.WebServiceManager(CurrentCredentials)

        Dim userId = CurrentCredentials.SecurityHeader.UserId
        vConnection = New Connection(ServiceManager, vaultName, userId, vaultServer, VDF.Vault.Currency.Connections.AuthenticationFlags.Standard)

        Dim _item As Autodesk.Connectivity.WebServices.Item = 
        Dim itemId As Long = _item.Id
        Dim destState As String = String.Empty
        Dim curLyfCyc() As Autodesk.Connectivity.WebServices.LfCycDef = ServiceManager.DocumentServiceExtensions.GetLifeCycleDefinitionsByIds(New Long() {_file.FileLfCyc.LfCycDefId})
        If Not curLyfCyc(0).DispName.Equals(LifecycleName, StringComparison.InvariantCultureIgnoreCase) Then
            Exit Sub
        End If
        For Each _state As LfCycState In curLyfCyc(0).StateArray
            If _state.Id.Equals(DirectCast(e, UpdateFileLifeCycleStateCommandEventArgs).ToStateIds(0)) Then
                destState = _state.DispName
                Exit For
            End If
        Next
    End Sub

End Class
