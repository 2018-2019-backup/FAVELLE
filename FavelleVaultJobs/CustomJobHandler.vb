'=====================================================================
'  
'  This file is part of the Autodesk Vault API Code Samples.
'
'  Copyright (C) Autodesk Inc.  All rights reserved.
'
'THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
'KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
'IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
'PARTICULAR PURPOSE.
'=====================================================================


Imports System.Linq
Imports System.IO
Imports Autodesk.DataManagement.Client.Framework
Imports Autodesk.Connectivity.JobProcessor.Extensibility
Imports Autodesk.DataManagement.Client.Framework.Vault.Currency.Entities
Imports System.Reflection
Imports Autodesk.Connectivity.Extensibility.Framework
Imports Autodesk.DataManagement.Client.Framework.Currency
Imports Autodesk.Connectivity.WebServices
Imports Autodesk.DataManagement.Client.Framework.Vault.Settings
Imports Autodesk.DataManagement.Client.Framework.Vault.Results
Imports System.Xml
Imports System.Threading
Imports System.Data
Imports axFavelleBOM
Imports Autodesk.DataManagement.Client.Framework.Vault.Currency.Properties


<Assembly: AssemblyProduct("FavelleVaultJobs")> 
<Assembly: ApiVersion("9.0")> 
<Assembly: ExtensionId("50A18DD5-892A-477e-911E-1D6E3C647036")> 


Namespace FavelleVaultJobs
    Public Class CustomJobHandler
        Implements IJobHandler

        Public Shared _dirSep As String = System.IO.Path.DirectorySeparatorChar.ToString()
        Public Shared _dllFolder As String = "C:\ProgramData\Autodesk\Vault 2016\Extensions\FavelleVaultJobs"
        Public Shared _workingFolder As String = _dllFolder & _dirSep & "KKMTools" & _dirSep
        Public _SourceFolder As String
        Public _TargetFolder As String

        Dim dtVaultProps, dtItemProp, dtDBItems As DataTable

        Public Sub New()

        End Sub


        Public Shared Sub DeleteFile(ByVal file As String)
            Try
                If IO.File.Exists(file) Then
                    Dim oFileInfo As New IO.FileInfo(file)
                    If (oFileInfo.Attributes And IO.FileAttributes.ReadOnly) > 0 Then
                        oFileInfo.Attributes = oFileInfo.Attributes Xor IO.FileAttributes.ReadOnly
                    End If
                    System.IO.File.Delete(file)
                End If
            Catch ex As Exception
                WriteLog("Deleting " + file + "failed!" + ex.ToString())
            End Try
        End Sub


        Public Shared Sub DeleteDirectory(ByVal dir As String)
            Try
                If IO.Directory.Exists(dir) Then
                    For Each _file In IO.Directory.GetFiles(dir)
                        DeleteFile(_file)
                    Next
                    Dim odirInfo As New IO.DirectoryInfo(dir)
                    If (odirInfo.Attributes And IO.FileAttributes.ReadOnly) > 0 Then
                        odirInfo.Attributes = odirInfo.Attributes Xor IO.FileAttributes.ReadOnly
                    End If
                    System.IO.Directory.Delete(dir, True)
                End If
            Catch ex As Exception
                WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
            End Try
        End Sub


        Public Function CanProcess(ByVal jobType As String) As Boolean Implements IJobHandler.CanProcess
            If jobType.Equals("FavelleVaultJobs.ExportItem.LifeCycleStateChange", StringComparison.InvariantCultureIgnoreCase) Or
                jobType.Equals("FavelleVaultJobs.ExportBOM.LifeCycleStateChange", StringComparison.InvariantCultureIgnoreCase) Or
                 jobType.Equals("FavelleVaultJobs.ImportItem.AutoSync", StringComparison.InvariantCultureIgnoreCase) Or
 jobType.Equals("FavelleVaultJobs.ImportItem.ManualSync", StringComparison.InvariantCultureIgnoreCase) Or
      jobType.Equals("FavelleVaultJobs.ExportItem.ManualSync", StringComparison.InvariantCultureIgnoreCase) Or
       jobType.Equals("FavelleVaultJobs.ExportBOM.ManualSync", StringComparison.InvariantCultureIgnoreCase) Then
                Return True
            End If
            Return False
        End Function

        Public Function Execute(ByVal context As IJobProcessorServices, ByVal job As IJob) As JobOutcome Implements IJobHandler.Execute
            Try
                vConnection = context.Connection
                errorMsg = String.Empty
                If LoadDataFromSettingsFile() Then
                    If job.JobType.Equals("FavelleVaultJobs.ExportItem.LifeCycleStateChange", StringComparison.InvariantCultureIgnoreCase) Then
                        Dim vaultName As String = job.VaultName
                        Dim entityId As Long = Convert.ToInt64(job.Params("EntityId"))
                        'Dim transitionId As Long = Convert.ToInt64(job.Params("LifeCycleTransitionId"))
                        If ExportItems(entityId, vaultName, Nothing, True) Then
                            Return JobOutcome.Success
                        Else
                            If errorMsg <> String.Empty Then
                                context.Log(errorMsg, MessageType.eInformation)
                            Else
                                context.Log("Unable to Export item to AX. Kindly check the server log for more info.", MessageType.eInformation)
                            End If
                            Return JobOutcome.Failure
                        End If
                    ElseIf job.JobType.Equals("FavelleVaultJobs.ExportItem.ManualSync", StringComparison.InvariantCultureIgnoreCase) Then
                        Dim vaultName As String = job.VaultName

                        Dim axOrgs As List(Of String) = Nothing
                        Try
                            axOrgs = job.Params("AxOrgs").ToString().Split(",").ToList()
                        Catch ex As Exception
                            WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
                        End Try
                        If axOrgs Is Nothing Or axOrgs.Count.Equals(0) Or axOrgs(0).Equals(String.Empty) Then
                            axOrgs = Nothing
                        End If
                        Dim itemIds As List(Of String) = job.Params("ItemIds").ToString().Split(",").ToList()
                        Dim subUser = job.Params("vUser").ToString()
                        Dim allow = (From user As DataRow In dtUserEmpIdMapping.Rows
                                   Where user(0).ToString().Equals(subUser, StringComparison.InvariantCultureIgnoreCase)
                                   Select user(3)).FirstOrDefault()
                        If allow Is Nothing Or allow.ToString().Equals("False", StringComparison.InvariantCultureIgnoreCase) Then
                            context.Log(vUserName + " does not have permission to export items to AX", MessageType.eInformation)
                            Return JobOutcome.Failure
                        Else
                            For Each id In itemIds
                                If ExportItems(Convert.ToInt64(id), vaultName, axOrgs).Equals(False) Then
                                    context.Log(errorMsg, MessageType.eInformation)
                                    Return JobOutcome.Failure
                                End If
                            Next
                            Return JobOutcome.Success
                        End If
                    ElseIf job.JobType.Equals("FavelleVaultJobs.ExportBOM.LifeCycleStateChange", StringComparison.InvariantCultureIgnoreCase) Then
                        Dim vaultName As String = job.VaultName
                        Dim entityId As Long = Convert.ToInt64(job.Params("EntityId"))
                        Dim transitionId As Long = Convert.ToInt64(job.Params("LifeCycleTransitionId"))
                        Dim axOrgs As List(Of String) = Nothing
                        Try
                            axOrgs = job.Params("AxOrgs").ToString().Split(",").ToList()
                        Catch ex As Exception
                            'WriteLog( ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
                        End Try

                        If ExportBOM(entityId, vaultName, axOrgs, True) Then
                            Return JobOutcome.Success
                        Else
                            If errorMsg <> String.Empty Then
                                context.Log(errorMsg, MessageType.eInformation)
                            Else
                                context.Log("Unable to Export BOM to AX. Kindly check the server log for more info.", MessageType.eInformation)
                            End If
                            Return JobOutcome.Failure
                        End If
                    ElseIf job.JobType.Equals("FavelleVaultJobs.ExportBOM.ManualSync", StringComparison.InvariantCultureIgnoreCase) Then
                        Dim vaultName As String = job.VaultName
                        Dim axOrgs As List(Of String) = Nothing
                        Try
                            axOrgs = job.Params("AxOrgs").ToString().Split(",").ToList()
                        Catch ex As Exception
                            WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
                        End Try
                        If axOrgs Is Nothing Or axOrgs.Count.Equals(0) Or axOrgs(0).Equals(String.Empty) Then
                            axOrgs = Nothing
                        End If
                        Dim itemIds As List(Of String) = job.Params("ItemIds").ToString().Split(",").ToList()
                        Dim subUser = job.Params("vUser").ToString()
                        Dim allow = (From user As DataRow In dtUserEmpIdMapping.Rows
                                   Where user(0).ToString().Equals(subUser, StringComparison.InvariantCultureIgnoreCase)
                                   Select user(4)).FirstOrDefault()
                        If allow Is Nothing Or allow.ToString().Equals("False", StringComparison.InvariantCultureIgnoreCase) Then
                            context.Log(vUserName + " does not have permission to export BOM to AX", MessageType.eInformation)
                            Return JobOutcome.Failure
                        Else
                            For Each id In itemIds
                                If ExportBOM(id, vaultName, axOrgs).Equals(False) Then
                                    context.Log(errorMsg, MessageType.eInformation)
                                    Return JobOutcome.Failure
                                End If
                            Next
                            Return JobOutcome.Success
                        End If
                    ElseIf job.JobType.Equals("FavelleVaultJobs.ImportItem.AutoSync", StringComparison.InvariantCultureIgnoreCase) Or job.JobType.Equals("FavelleVaultJobs.ImportItem.ManualSync", StringComparison.InvariantCultureIgnoreCase) Then
                        Dim axOrgs As List(Of String) = Nothing
                        Try
                            axOrgs = job.Params("AxOrgs").ToString().Split(",").ToList()
                        Catch ex As Exception
                            WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
                        End Try
                        Dim subUser = job.Params("vUser").ToString()
                        Dim allow = (From user As DataRow In dtUserEmpIdMapping.Rows
                                       Where user(0).ToString().Equals(subUser, StringComparison.InvariantCultureIgnoreCase)
                                       Select user(2)).FirstOrDefault()
                        If allow Is Nothing Or allow.ToString().Equals("False", StringComparison.InvariantCultureIgnoreCase) Then
                            context.Log(vUserName + " does not have permission to import items from AX", MessageType.eInformation)
                            Return JobOutcome.Failure
                        Else
                            For Each ax In axOrgs
                                Dim vaultNames As List(Of String) = (From row As DataRow In dtVaultOrg.Rows
                                                          Where row(1).ToString().Equals(ax, StringComparison.InvariantCultureIgnoreCase)
                                                          Select Convert.ToString(row(0))).ToList()
                                errorMsg = String.Empty
                                dtDBItems = GetImportItems(ax)
                                If dtDBItems IsNot Nothing Then
                                    For Each dr As DataRow In dtDBItems.Rows
                                        dr("Description") = dr("ProductName")
                                    Next
                                    Dim dtTemp = dtDBItems
                                    For Each vaultName In vaultNames
                                        dtDBItems = dtTemp
                                        SetItemProperties(vaultName)
                                    Next
                                End If
                                If errorMsg.Equals(String.Empty) Then
                                    Return JobOutcome.Success
                                Else
                                    context.Log(errorMsg, MessageType.eInformation)
                                    Return JobOutcome.Failure
                                End If
                            Next
                        End If
                    End If
                Else
                    context.Log("Unable to find settings file. Kindly contact administrator to configure settings.", MessageType.eInformation)
                End If

            Catch ex As Exception
                context.Log("Unexpected Error!" + Environment.NewLine + ex.Message, MessageType.eInformation)
                WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
                Return JobOutcome.Failure
            End Try
        End Function
        Public Function IsItemReleased(ByVal itemId As Long, ByRef lastModUser As String) As Boolean
            Dim retValue As Boolean = False
            lastModUser = String.Empty
            errorMsg = String.Empty
            Try
                Dim tItem As Item = Nothing
                Try
                    tItem = vConnection.WebServiceManager.ItemService.GetItemsByIds(New Long() {itemId})(0)
                Catch ex As Exception
                    Try
                        tItem = vConnection.WebServiceManager.ItemService.GetLatestItemByItemMasterId(itemId)
                    Catch ex1 As Exception

                    End Try
                End Try
                If tItem IsNot Nothing Then

                    lastModUser = tItem.LastModUserName
                    Dim stateIds = vConnection.WebServiceManager.DocumentServiceExtensions.GetLifeCycleDefinitionsByIds(New Long() {tItem.LfCyc.LfCycDefId})(0).StateArray
                    retValue = (From state As LfCycState In stateIds
                                Where state.Id.Equals(tItem.LfCycStateId)
                                Select state.ReleasedState).FirstOrDefault()
                Else
                    errorMsg = "Unable to fetch item details. Kinldy try again!"
                End If
            Catch ex As Exception
                WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
            End Try
            Return retValue
        End Function

        Public Function ExportBOM(ByVal entId As Long, ByVal vName As String, ByVal axOrgs As List(Of String), Optional ByVal checkUser As Boolean = False) As Boolean
            Dim retValue As Boolean = False
            Try
                Dim lastModUser = String.Empty
                If IsItemReleased(entId, lastModUser) Then
                    If checkUser Then
                        Dim allow = (From user As DataRow In dtUserEmpIdMapping.Rows
                                   Where user(0).ToString().Equals(lastModUser, StringComparison.InvariantCultureIgnoreCase)
                                   Select user(4)).FirstOrDefault()
                        If allow Is Nothing Or allow.ToString().Equals("False", StringComparison.InvariantCultureIgnoreCase) Then
                            errorMsg = vUserName + " does not have permission to export BOM to AX"
                            Return False
                        End If
                    End If

                    SetdtVaultProps(vName, True)
                    dtBOMColumnMapping.Columns.Add("PropId", System.Type.GetType("System.Int64"))
                    dtBOMColumnMapping.Columns.Add("PropValue")

                    Dim lstProps As List(Of String) = (From dr As DataRow In dtVaultProps.Rows
                                   Where dr.Item("VaultName").ToString().Equals(vName, StringComparison.InvariantCultureIgnoreCase)
                                   Select DirectCast(dr.Item("VaultProp"), String)).ToList()
                    Dim props() As PropDef = vConnection.WebServiceManager.PropertyService.GetPropertyDefinitionsByEntityClassId("ITEM")

                    Dim vaultProp() As PropDef = (From prop In props
                                          Where lstProps.Contains(prop.DispName, StringComparer.InvariantCultureIgnoreCase).Equals(True)
                                            Select prop).ToArray() ' prop.DispName.Equals("Revision", StringComparison.InvariantCultureIgnoreCase).Equals(True) Or
                    For Each dr As DataRow In dtBOMColumnMapping.Rows
                        dr.Item("PropId") = (From prps In vaultProp
                                                  Where prps.DispName.Equals(dr.Item(vName).ToString(), StringComparison.InvariantCultureIgnoreCase)
                                            Select prps.Id).FirstOrDefault()
                    Next

                    Dim itemBom() As ItemAssoc = vConnection.WebServiceManager.ItemService.GetItemBOMAssociationsByItemIds(New Long() {entId}, True)
                    Dim ParIds As List(Of Long) = New List(Of Long)
                    Dim ChildIds As List(Of Long) = New List(Of Long)
                    Dim dtItemAssoc As DataTable = New DataTable()
                    dtItemAssoc.Columns.Add("Parent", System.Type.GetType("System.Int64"))
                    dtItemAssoc.Columns.Add("Child", System.Type.GetType("System.Int64"))
                    dtItemAssoc.Columns.Add("Position", System.Type.GetType("System.String"))
                    dtItemAssoc.Columns.Add("Qty", System.Type.GetType("System.Double"))
                    dtItemAssoc.Columns.Add("AssocId", System.Type.GetType("System.Int64"))
                    Dim newRow As DataRow
                    If itemBom IsNot Nothing Then
                        For Each itemAss As ItemAssoc In itemBom
                            If itemAss.IsIncluded Then
                                newRow = dtItemAssoc.NewRow()
                                newRow("Parent") = itemAss.ParItemID
                                newRow("Position") = itemAss.PositionNum
                                newRow("Child") = itemAss.CldItemMasterID
                                newRow("Qty") = itemAss.Quant
                                newRow("AssocId") = itemAss.Id
                                dtItemAssoc.Rows.Add(newRow)
                            End If
                        Next
                        Dim i = 0
                        If axOrgs Is Nothing Then
                            axOrgs = (From row In dtVaultOrg.Rows
                                     Where row(0).ToString().Equals(vName, StringComparison.InvariantCultureIgnoreCase)
                                     Select DirectCast(row(1), String)).ToList()
                        End If
                        If UpdateBOMHeader(entId, vName, dtItemAssoc, axOrgs) Then
                            Dim ids As List(Of Long) = (From rows As DataRow In dtItemAssoc.Rows
                                      Where rows("Parent") <> entId
                                      Select DirectCast(rows("Parent"), Long)).Distinct().ToList()
                            For Each itemId In ids
                                If UpdateBOMHeader(itemId, vName, dtItemAssoc, axOrgs).Equals(False) Then
                                    Return False
                                End If
                            Next
                            retValue = True
                        End If
                        'For Each itemId In ChildIds.Distinct()
                        '    Dim itmPropInst As PropInst() = vConnection.WebServiceManager.PropertyService.GetPropertiesByEntityIds("ITEM", New Long() {itemId})
                        'Next
                    Else
                        retValue = True
                    End If
                Else
                    If errorMsg.Equals(String.Empty) Then
                        errorMsg = "Item should be in Released state for exporting BOM to AX."
                    End If
                    retValue = False
                End If
            Catch ex As Exception
                WriteLog(ex)
            End Try
            Return retValue
        End Function

        Private Sub GetBOMProp(ByVal itemId As Long, ByRef itemRevision As String, Optional ByVal itemAssocId As Long = -1, Optional ByVal isMasterId As Boolean = False, Optional ByRef getBOMLineProp As Boolean = False)
            Try
                Dim selItem As Item
                If isMasterId Then
                    Try
                        selItem = vConnection.WebServiceManager.ItemService.GetLatestItemByItemMasterId(itemId)
                        itemId = selItem.Id
                    Catch ex As Exception

                    End Try
                End If
                Try
                    itemId = vConnection.WebServiceManager.ItemService.GetItemsByIds(New Long() {itemId}).FirstOrDefault().MasterId
                    selItem = vConnection.WebServiceManager.ItemService.GetLatestItemByItemMasterId(itemId)
                    itemId = selItem.Id
                    itemRevision = selItem.RevNum
                Catch ex As Exception
                    '   WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
                End Try
                Dim itmPropInst As PropInst() = vConnection.WebServiceManager.PropertyService.GetPropertiesByEntityIds("ITEM", New Long() {itemId})
                If itmPropInst IsNot Nothing And dtBOMColumnMapping IsNot Nothing Then
                    For Each dr As DataRow In dtBOMColumnMapping.Rows
                        If dr.Item("PropId") IsNot Nothing Then
                            Dim value As String = String.Empty
                            Try
                                value = (From propInst In itmPropInst
                            Where propInst.PropDefId.Equals(dr.Item("PropId"))
                                           Select propInst.Val).FirstOrDefault()
                                If value Is Nothing Then
                                    value = String.Empty
                                End If
                            Catch ex As Exception
                                WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
                            End Try
                            dr.Item("PropValue") = value
                        End If
                    Next
                    If getBOMLineProp Then
                        Dim definitions() As AssocPropDef = vConnection.WebServiceManager.PropertyService.
                       GetAssociationPropertyDefinitionsByType(AssocPropTyp.ItemBOMAssoc)
                        Dim bomPrpsId As List(Of Long) = New List(Of Long)
                        Dim bomProps As Dictionary(Of String, Long) = New Dictionary(Of String, Long)
                        For Each _def As AssocPropDef In definitions
                            For Each dr As DataRow In dtBOMColumnMapping.Select("AX_Field = '" + _def.DispName + "'")
                                dr("PropId") = _def.Id
                                bomProps.Add(_def.DispName, _def.Id)
                                bomPrpsId.Add(_def.Id)
                            Next
                        Next
                        If bomPrpsId.Count > 0 Then
                            Dim itemAssocProps() As ItemAssocProp = vConnection.WebServiceManager.ItemService.GetItemBOMAssociationProperties(New Long() {itemAssocId}, bomPrpsId.ToArray())
                            If itemAssocProps IsNot Nothing Then
                                For Each _props As ItemAssocProp In itemAssocProps
                                    For Each dr As DataRow In dtBOMColumnMapping.Select("PropId = '" + _props.PropDefId.ToString() + "'")
                                        dr("PropValue") = _props.Val.ToString()
                                    Next
                                Next
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
            End Try
        End Sub

        Private Function UpdateBOMHeader(ByVal id As Long, ByVal vName As String, ByVal dt As DataTable, Optional ByVal axOrgs As List(Of String) = Nothing) As Boolean
            Dim retValue As Boolean = False
            Try
                Dim headerRev As String = String.Empty
                GetBOMProp(id, headerRev)
                Dim itemCode As String = (From dr As DataRow In dtBOMColumnMapping.Rows
                                  Where dr.Item("AX_Field").ToString().Equals("ItemNo", StringComparison.InvariantCultureIgnoreCase)
                                  Select dr.Item("PropValue")).FirstOrDefault()

                Dim bomNo, bomName, appBy, chkBy, revNo, sn, revDesc, modifiedDate, hUOM, hWeightStr As String
                Dim hWeight As Decimal
                modifiedDate = Now.ToString(dtFormat)
                Dim ids = (From row As DataRow In dt.Rows
                                     Where row("Parent") = id
                                     Select row)
                Dim cnt As Integer = ids.Count()
                Dim ChildIds As List(Of V2AX_BOMLineDataContract) = New List(Of V2AX_BOMLineDataContract)
                Dim qty As Decimal

                bomName = (From dr As DataRow In dtBOMColumnMapping.Rows
                            Where dr.Item("AX_Field").ToString().Equals("BOMName", StringComparison.InvariantCultureIgnoreCase)
                            Select dr.Item("PropValue")).FirstOrDefault()

                appBy = (From dr As DataRow In dtBOMColumnMapping.Rows
                            Where dr.Item("AX_Field").ToString().Equals("ApprovedBy", StringComparison.InvariantCultureIgnoreCase)
                            Select dr.Item("PropValue")).FirstOrDefault()

                chkBy = (From dr As DataRow In dtBOMColumnMapping.Rows
                           Where dr.Item("AX_Field").ToString().Equals("CheckedBy", StringComparison.InvariantCultureIgnoreCase)
                           Select dr.Item("PropValue")).FirstOrDefault()

                revNo = (From dr As DataRow In dtBOMColumnMapping.Rows
                           Where dr.Item("AX_Field").ToString().Equals("BOM CHG REV", StringComparison.InvariantCultureIgnoreCase)
                           Select dr.Item("PropValue")).FirstOrDefault()

                sn = (From dr As DataRow In dtBOMColumnMapping.Rows
                           Where dr.Item("AX_Field").ToString().Equals("SN", StringComparison.InvariantCultureIgnoreCase)
                           Select dr.Item("PropValue")).FirstOrDefault()

                revDesc = (From dr As DataRow In dtBOMColumnMapping.Rows
                           Where dr.Item("AX_Field").ToString().Equals("RevDescChanges", StringComparison.InvariantCultureIgnoreCase)
                           Select dr.Item("PropValue")).FirstOrDefault()

                hWeightStr = (From dr As DataRow In dtBOMColumnMapping.Rows
                 Where dr.Item("AX_Field").ToString().Equals("Weight", StringComparison.InvariantCultureIgnoreCase)
                 Select dr.Item("PropValue")).FirstOrDefault()
                Double.TryParse(hWeightStr, hWeight)

                hUOM = (From dr As DataRow In dtBOMColumnMapping.Rows
                Where dr.Item("AX_Field").ToString().Equals("UOM", StringComparison.InvariantCultureIgnoreCase)
                Select dr.Item("PropValue")).FirstOrDefault()
                If headerRev <> String.Empty Then
                    bomNo = itemCode.Replace("-", "") + headerRev
                    revNo = headerRev
                Else
                    bomNo = itemCode.Replace("-", "") + revNo
                End If

                Dim pcControl As Boolean
                Dim strPcControl As String
                strPcControl = (From dr As DataRow In dtBOMColumnMapping.Rows
                Where dr.Item("AX_Field").ToString().Equals("PcsControlled", StringComparison.InvariantCultureIgnoreCase)
                            Select dr.Item("PropValue")).FirstOrDefault()
                Boolean.TryParse(strPcControl, pcControl)
                If (strPcControl.Trim().Equals("1")) Then
                    pcControl = True
                End If
                If pcControl And hUOM.Equals("kg", StringComparison.InvariantCultureIgnoreCase) Then
                    qty = hWeight
                Else
                    qty = 1
                End If


                If axOrgs Is Nothing Then
                    axOrgs = (From row In dtVaultOrg.Rows
                             Where row(0).ToString().Equals(vName, StringComparison.InvariantCultureIgnoreCase)
                             Select DirectCast(row(1), String)).ToList()
                End If
                Dim i = 1
                Dim tempRev As String = String.Empty
                For Each row In ids
                    GetBOMProp(row("Child"), tempRev, row("AssocId"), True, True)
                    Dim cItemNo, cLength, cCat, cRemarks, cPcs, cRevNo, cWidth, cPosition, cWeight, cUom As String
                    cItemNo = (From dr As DataRow In dtBOMColumnMapping.Rows
                       Where dr.Item("AX_Field").ToString().Equals("ItemNo", StringComparison.InvariantCultureIgnoreCase)
                       Select dr.Item("PropValue")).FirstOrDefault()

                    cRevNo = (From dr As DataRow In dtBOMColumnMapping.Rows
                         Where dr.Item("AX_Field").ToString().Equals("BOM CHG REV", StringComparison.InvariantCultureIgnoreCase)
                         Select dr.Item("PropValue")).FirstOrDefault()

                    cLength = (From dr As DataRow In dtBOMColumnMapping.Rows
                         Where dr.Item("AX_Field").ToString().Equals("BOM Length", StringComparison.InvariantCultureIgnoreCase)
                         Select dr.Item("PropValue")).FirstOrDefault()

                    cWidth = (From dr As DataRow In dtBOMColumnMapping.Rows
                    Where dr.Item("AX_Field").ToString().Equals("BOM Width", StringComparison.InvariantCultureIgnoreCase)
                    Select dr.Item("PropValue")).FirstOrDefault()

                    cWeight = (From dr As DataRow In dtBOMColumnMapping.Rows
                  Where dr.Item("AX_Field").ToString().Equals("BOM Weight", StringComparison.InvariantCultureIgnoreCase)
                  Select dr.Item("PropValue")).FirstOrDefault()

                    cCat = (From dr As DataRow In dtBOMColumnMapping.Rows
                 Where dr.Item("AX_Field").ToString().Equals("CAT", StringComparison.InvariantCultureIgnoreCase)
                 Select dr.Item("PropValue")).FirstOrDefault()

                    cRemarks = (From dr As DataRow In dtBOMColumnMapping.Rows
                 Where dr.Item("AX_Field").ToString().Equals("BOMRemark", StringComparison.InvariantCultureIgnoreCase)
                 Select dr.Item("PropValue")).FirstOrDefault()

                    cUom = (From dr As DataRow In dtBOMColumnMapping.Rows
                  Where dr.Item("AX_Field").ToString().Equals("UOM", StringComparison.InvariantCultureIgnoreCase)
                  Select dr.Item("PropValue")).FirstOrDefault()

                    'cRevNo = (From dr As DataRow In dtBOMColumnMapping.Rows
                    '   Where dr.Item("AX_Field").ToString().Equals("RevNo", StringComparison.InvariantCultureIgnoreCase)
                    '   Select dr.Item("PropValue")).FirstOrDefault()

                    cPcs = (From dr As DataRow In dtBOMColumnMapping.Rows
                         Where dr.Item("AX_Field").ToString().Equals("BOM PCS", StringComparison.InvariantCultureIgnoreCase)
                         Select dr.Item("PropValue")).FirstOrDefault()

                    modifiedDate = (From dr As DataRow In dtBOMColumnMapping.Rows
                        Where dr.Item("AX_Field").ToString().Equals("BOMDate", StringComparison.InvariantCultureIgnoreCase)
                        Select dr.Item("PropValue")).FirstOrDefault()
                    If (modifiedDate IsNot Nothing And modifiedDate <> String.Empty) Then
                        Try
                            modifiedDate = Convert.ToDateTime(modifiedDate).ToString(dtFormat)
                        Catch ex As Exception
                        End Try
                    End If
                    If IsDate(modifiedDate).Equals(False) Then
                        modifiedDate = Now.ToString(dtFormat)
                    End If
                    strPcControl = (From dr As DataRow In dtBOMColumnMapping.Rows
                    Where dr.Item("AX_Field").ToString().Equals("PcsControlled", StringComparison.InvariantCultureIgnoreCase)
                                Select dr.Item("PropValue")).FirstOrDefault()
                    Boolean.TryParse(strPcControl, pcControl)
                    If (strPcControl.Trim().Equals("1")) Then
                        pcControl = True
                    End If

                    cPosition = row("Position").ToString()

                    Dim cId As V2AX_BOMLineDataContract = New V2AX_BOMLineDataContract()
                    cId.BOMNo = bomNo
                    If pcControl And cUom.Equals("kg", StringComparison.InvariantCultureIgnoreCase) Then
                        Decimal.TryParse(cWeight, cId.BomQty)
                    Else
                        Decimal.TryParse(row("Qty"), cId.BomQty)
                    End If
                    Dim stIndex = cCat.IndexOf("(")
                    Dim endIndex = cCat.IndexOf(")")
                    If stIndex <> -1 And endIndex <> -1 Then
                        Integer.TryParse(cCat.Substring(stIndex + 1, endIndex - (stIndex + 1)).Trim, cId.CAT)
                    Else
                        cId.CAT = 0
                    End If
                    cId.Remarks = cRemarks
                    Decimal.TryParse(cWeight, cId.Weight)
                    cId.ItemNo = cItemNo
                    Decimal.TryParse(cLength, cId.Length)
                    'cId.LineNo = ""
                    Decimal.TryParse(cPcs, cId.PCS)
                    cId.Position = cPosition
                    cId.RevNo = cRevNo
                    Decimal.TryParse(cWidth, cId.Width)
                    ChildIds.Add(cId)
                    i = i + 1
                Next

                For Each org In axOrgs
                    If (ExportBOMToAX(bomNo, bomName, itemCode, appBy, chkBy, revNo, sn, revDesc, qty, org, modifiedDate, ChildIds.ToArray()).Equals(False)) Then
                        Return False
                    End If
                Next
                UpdateBOMStatus(id, vName)
                For Each row In ids
                    UpdateBOMStatus(row("Child"), vName)
                Next
                retValue = True
            Catch ex As Exception
                errorMsg = "Unexpected Error! " + ex.Message
                WriteLog(ex)
            End Try
            Return retValue
        End Function

        Private Sub SetdtVaultProps(ByVal vName As String, Optional ByVal isBOM As Boolean = False)
            dtVaultProps = New DataTable()
            dtVaultProps.Columns.Add("VaultProp")
            dtVaultProps.Columns.Add("VaultName")
            Dim newRow As DataRow
            If isBOM Then
                For Each row As DataRow In dtBOMColumnMapping.Rows
                    If row(vName).ToString() <> "-" Then
                        newRow = dtVaultProps.NewRow()
                        newRow("VaultName") = vName
                        newRow("VaultProp") = row(vName)
                        dtVaultProps.Rows.Add(newRow)
                    End If
                Next
            Else
                For Each row As DataRow In dtItemColumnMapping.Rows
                    If row(vName).ToString() <> "-" Then
                        newRow = dtVaultProps.NewRow()
                        newRow("VaultName") = vName
                        newRow("VaultProp") = row(vName)
                        dtVaultProps.Rows.Add(newRow)
                    End If
                Next
            End If
        End Sub


        Public Function ExportItems(ByVal entId As Long, ByVal vName As String, ByVal axOrgs As List(Of String), Optional ByVal checkUser As Boolean = False) As Boolean
            Dim retValue As Boolean = False
            Try
                Dim lastModUser = String.Empty
                If IsItemReleased(entId, lastModUser) Then
                    ' GetAdminCredentials(vName)
                    If checkUser Then
                        Dim allow = (From user As DataRow In dtUserEmpIdMapping.Rows
                                   Where user(0).ToString().Equals(lastModUser, StringComparison.InvariantCultureIgnoreCase)
                                   Select user(3)).FirstOrDefault()
                        If allow Is Nothing Or allow.ToString().Equals("False", StringComparison.InvariantCultureIgnoreCase) Then
                            errorMsg = vUserName + " does not have permission to export items to AX"
                            Return False
                        End If
                    End If
                    SetdtVaultProps(vName)
                    dtItemColumnMapping.Columns.Add("PropId", System.Type.GetType("System.Int64"))
                    dtItemColumnMapping.Columns.Add("PropValue")

                    Dim lstProps As List(Of String) = (From dr As DataRow In dtVaultProps.Rows
                                   Where dr.Item("VaultName").ToString().Equals(vName, StringComparison.InvariantCultureIgnoreCase)
                                   Select DirectCast(dr.Item("VaultProp"), String)).ToList()
                    Dim statusProp As String = "Imported from AX"
                    'lstProps.Add(statusProp)
                    Dim props() As PropDef = vConnection.WebServiceManager.PropertyService.GetPropertyDefinitionsByEntityClassId("ITEM")

                    Dim vaultProp() As PropDef = (From prop In props
                                          Where lstProps.Contains(prop.DispName, StringComparer.InvariantCultureIgnoreCase).Equals(True)
                                            Select prop).ToArray() ' prop.DispName.Equals("Revision", StringComparison.InvariantCultureIgnoreCase).Equals(True) Or
                    For Each dr As DataRow In dtItemColumnMapping.Rows
                        dr.Item("PropId") = (From prps In vaultProp
                                                  Where prps.DispName.Equals(dr.Item(vName).ToString(), StringComparison.InvariantCultureIgnoreCase)
                                            Select prps.Id).FirstOrDefault()
                    Next

                    Dim itmPropInst As PropInst() = vConnection.WebServiceManager.PropertyService.GetPropertiesByEntityIds("ITEM", New Long() {entId})
                    For Each dr As DataRow In dtItemColumnMapping.Rows
                        If dr.Item("PropId") IsNot Nothing Then
                            Dim value As String = String.Empty
                            Try
                                value = (From propInst In itmPropInst
                            Where propInst.PropDefId.Equals(dr.Item("PropId"))
                                           Select propInst.Val).FirstOrDefault().ToString()
                            Catch ex As Exception

                            End Try
                            dr.Item("PropValue") = value
                        End If
                    Next

                    ''Test 
                    'For Each dr As DataRow In dtItemColumnMapping.Rows
                    '    Dim str As String = String.Empty
                    '    For Each col As DataColumn In dtItemColumnMapping.Columns
                    '        str += dr(col.ColumnName).ToString() + "   "
                    '    Next
                    '    WriteLog(str)
                    'Next
                    ''
                    Dim itemCode As String = (From dr As DataRow In dtItemColumnMapping.Rows
                                      Where dr.Item("AX_Field").ToString().Equals("ProductNo", StringComparison.InvariantCultureIgnoreCase)
                                      Select dr.Item("PropValue")).FirstOrDefault()
                    If itemCode IsNot Nothing And itemCode <> String.Empty Then
                        '  Dim qry As String
                        ' qry = String.Format("select Count(*) from [V2AX_ItemImport] where V2AX_ItemId = '{0}'", itemCode)
                        Dim cat, chargeCode, desc, iGrp, pName, remark, srchName, spec, prjCat, bomUnit, importFromAx As String
                        'cat = (From dr As DataRow In dtItemColumnMapping.Rows
                        '             Where dr.Item("DBColumn").ToString().Equals("V2AX_CAT", StringComparison.InvariantCultureIgnoreCase)
                        '             Select dr.Item("PropValue")).FirstOrDefault()
                        Try
                            importFromAx = (From dr As DataRow In dtItemColumnMapping.Rows
                                  Where dr.Item("AX_Field").ToString().Equals(statusProp, StringComparison.InvariantCultureIgnoreCase)
                                  Select dr.Item("PropValue")).FirstOrDefault()
                        Catch ex As Exception
                            importFromAx = String.Empty
                        End Try
                      

                        If importFromAx IsNot Nothing And importFromAx.Trim.Equals("Yes", StringComparison.InvariantCultureIgnoreCase) Then
                            retValue = True
                        Else
                            chargeCode = (From dr As DataRow In dtItemColumnMapping.Rows
                                        Where dr.Item("AX_Field").ToString().Equals("ChargesCode", StringComparison.InvariantCultureIgnoreCase)
                                        Select dr.Item("PropValue")).FirstOrDefault()

                            'compId = (From dr As DataRow In dtItemColumnMapping.Rows
                            '            Where dr.Item("AX_Field").ToString().Equals("V2AX_CompID", StringComparison.InvariantCultureIgnoreCase)
                            '            Select dr.Item("PropValue")).FirstOrDefault()


                            desc = (From dr As DataRow In dtItemColumnMapping.Rows
                                        Where dr.Item("AX_Field").ToString().Equals("Description", StringComparison.InvariantCultureIgnoreCase)
                                        Select dr.Item("PropValue")).FirstOrDefault()

                            iGrp = (From dr As DataRow In dtItemColumnMapping.Rows
                                        Where dr.Item("AX_Field").ToString().Equals("ItemGroup", StringComparison.InvariantCultureIgnoreCase)
                                        Select dr.Item("PropValue")).FirstOrDefault()

                            pName = (From dr As DataRow In dtItemColumnMapping.Rows
                                        Where dr.Item("AX_Field").ToString().Equals("ProductName", StringComparison.InvariantCultureIgnoreCase)
                                        Select dr.Item("PropValue")).FirstOrDefault()

                            remark = (From dr As DataRow In dtItemColumnMapping.Rows
                                        Where dr.Item("AX_Field").ToString().Equals("Remarks", StringComparison.InvariantCultureIgnoreCase)
                                        Select dr.Item("PropValue")).FirstOrDefault()

                            srchName = (From dr As DataRow In dtItemColumnMapping.Rows
                                        Where dr.Item("AX_Field").ToString().Equals("SearchName", StringComparison.InvariantCultureIgnoreCase)
                                        Select dr.Item("PropValue")).FirstOrDefault()

                            spec = (From dr As DataRow In dtItemColumnMapping.Rows
                                        Where dr.Item("AX_Field").ToString().Equals("Specification", StringComparison.InvariantCultureIgnoreCase)
                                        Select dr.Item("PropValue")).FirstOrDefault()

                            bomUnit = (From dr As DataRow In dtItemColumnMapping.Rows
                                        Where dr.Item("AX_Field").ToString().Equals("BOMUnit", StringComparison.InvariantCultureIgnoreCase)
                                        Select dr.Item("PropValue")).FirstOrDefault()

                            cat = (From dr As DataRow In dtItemColumnMapping.Rows
                                        Where dr.Item("AX_Field").ToString().Equals("CAT", StringComparison.InvariantCultureIgnoreCase)
                                        Select dr.Item("PropValue")).FirstOrDefault()

                            prjCat = (From dr As DataRow In dtItemColumnMapping.Rows
                                        Where dr.Item("AX_Field").ToString().Equals("ProjCategory", StringComparison.InvariantCultureIgnoreCase)
                                        Select dr.Item("PropValue")).FirstOrDefault()

                            Dim pcControl As Boolean
                            Dim strPcControl As String
                            strPcControl = (From dr As DataRow In dtItemColumnMapping.Rows
                            Where dr.Item("AX_Field").ToString().Equals("PcsControlled", StringComparison.InvariantCultureIgnoreCase)
                                        Select dr.Item("PropValue")).FirstOrDefault()
                            Boolean.TryParse(strPcControl, pcControl)
                            If (strPcControl.Trim().Equals("1")) Then
                                pcControl = True
                            End If
                            If axOrgs Is Nothing Then
                                axOrgs = (From row In dtVaultOrg.Rows
                                         Where row(0).ToString().Equals(vName, StringComparison.InvariantCultureIgnoreCase)
                                         Select DirectCast(row(1), String)).ToList()
                            End If

                            For Each org In axOrgs
                                retValue = ExportItemToAX(itemCode, chargeCode, desc, pName, remark, srchName, spec, pcControl, org, iGrp, bomUnit, cat, prjCat)
                                If retValue.Equals(False) Then
                                    Exit For
                                End If
                            Next
                            If retValue.Equals(True) Then
                                UpdateItemProperties(itemCode, vName)

                            End If
                        End If
                    Else
                        errorMsg = "Item Code is not valid!"
                        WriteLog("Item Code is not valid!")
                    End If
                Else
                    If errorMsg.Equals(String.Empty) Then
                        errorMsg = "Item should be in Released state for exporting to AX."
                    End If
                    retValue = False
                End If
            Catch ex As Exception
                errorMsg = "Unable to export item to AX!" + Environment.NewLine + ex.Message
                WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
            End Try
            Return retValue
        End Function

        Private Function UpdateBOMStatus(ByVal item As Long, ByVal vName As String) As Boolean
            Try
                '  GetAdminCredentials(vName)
                Dim props() As PropDef = vConnection.WebServiceManager.PropertyService.GetPropertyDefinitionsByEntityClassId("ITEM")

                Dim vaultProp() As PropDef = (From prop In props
                                           Where prop.DispName.Equals(bomPropToUpdate, StringComparison.InvariantCultureIgnoreCase)
                                             Select prop).ToArray() ' prop.DispName.Equals("Revision", StringComparison.InvariantCultureIgnoreCase).Equals(True) Or
                Dim vPropIds() As Long = (From prps In vaultProp
                                         Select prps.Id).ToArray()
                '   Dim prpParmArray(tobeModifiedCount) As PropInstParamArray
                '  Dim index As Integer
                Dim itemIndex = 0
                Dim itemIds As New List(Of Long)
                Dim selItem As Item
                Try
                    selItem = vConnection.WebServiceManager.ItemService.GetItemsByIds(New Long() {item})(0)
                Catch ex As Exception
                    selItem = vConnection.WebServiceManager.ItemService.GetLatestItemByItemMasterId(item)
                End Try
                'Dim selItem As Item = vConnection.WebServiceManager.ItemService.GetItemsByIds(New Long() {item})(0)
                itemIds.Add(selItem.RevId)
                Dim propInstParams As New List(Of PropInstParam)
                Dim propInstParamsArrays(1) As PropInstParamArray
                Dim newpropInstParamsArrays As New List(Of PropInstParamArray)


                propInstParams = New List(Of PropInstParam)


                Dim myItem As Item = vConnection.WebServiceManager.ItemService.GetLatestItemByItemMasterId(selItem.MasterId)
                For Each vProp In vaultProp
                    For Each prp As PropDef In props.Where(Function(T) T.Id.Equals(vProp.Id))
                        Dim propInst As New PropInstParam()

                        propInst.PropDefId = prp.Id
                        'prp.DispName.ToString()
                        propInst.Val = "Yes"
                        If (propInst.Val Is Nothing) Then
                            propInst.Val = String.Empty
                        End If
                        propInstParams.Add(propInst)
                    Next
                Next
                Dim propInstParamsArray As New PropInstParamArray()
                propInstParamsArray.Items = propInstParams.ToArray()
                propInstParamsArrays(itemIndex) = propInstParamsArray
                newpropInstParamsArrays.Add(propInstParamsArray)
                itemIndex = itemIndex + 1

                Try
                    If itemIds.Count <> 0 Then
                        vConnection.WebServiceManager.ItemService.DeleteUncommittedItems(True)
                        Dim newItemIds = vConnection.WebServiceManager.ItemService.EditItems(itemIds.ToArray()).Select(Function(t) t.RevId).ToArray()
                        Dim comItems() = vConnection.WebServiceManager.ItemService.UpdateItemProperties(newItemIds, newpropInstParamsArrays.ToArray())
                        vConnection.WebServiceManager.ItemService.UpdateAndCommitItems(comItems)
                        Return True
                    End If
                Catch ex As Exception
                    WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
                End Try
            Catch ex As Exception
                WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
            End Try
            Return False
        End Function

        Private Function UpdateItemProperties(ByVal item As String, ByVal vName As String) As Boolean
            Try
                '  GetAdminCredentials(vName)
                Dim props() As PropDef = vConnection.WebServiceManager.PropertyService.GetPropertyDefinitionsByEntityClassId("ITEM")

                Dim vaultProp() As PropDef = (From prop In props
                                           Where prop.DispName.Equals(propToUpdate, StringComparison.InvariantCultureIgnoreCase)
                                             Select prop).ToArray() ' prop.DispName.Equals("Revision", StringComparison.InvariantCultureIgnoreCase).Equals(True) Or
                Dim vPropIds() As Long = (From prps In vaultProp
                                         Select prps.Id).ToArray()
                '   Dim prpParmArray(tobeModifiedCount) As PropInstParamArray
                '  Dim index As Integer
                Dim itemIndex = 0
                Dim itemIds As New List(Of Long)
                Dim selItem As Item = vConnection.WebServiceManager.ItemService.GetLatestItemByItemNumber(item)
                itemIds.Add(selItem.RevId)
                Dim propInstParams As New List(Of PropInstParam)
                Dim propInstParamsArrays(1) As PropInstParamArray
                Dim newpropInstParamsArrays As New List(Of PropInstParamArray)


                propInstParams = New List(Of PropInstParam)


                Dim myItem As Item = vConnection.WebServiceManager.ItemService.GetLatestItemByItemNumber(item)
                For Each vProp In vaultProp
                    For Each prp As PropDef In props.Where(Function(T) T.Id.Equals(vProp.Id))
                        Dim propInst As New PropInstParam()

                        propInst.PropDefId = prp.Id
                        'prp.DispName.ToString()
                        propInst.Val = "Yes"
                        If (propInst.Val Is Nothing) Then
                            propInst.Val = String.Empty
                        End If
                        propInstParams.Add(propInst)
                    Next
                Next
                Dim propInstParamsArray As New PropInstParamArray()
                propInstParamsArray.Items = propInstParams.ToArray()
                propInstParamsArrays(itemIndex) = propInstParamsArray
                newpropInstParamsArrays.Add(propInstParamsArray)
                itemIndex = itemIndex + 1

                Try
                    If itemIds.Count <> 0 Then
                        vConnection.WebServiceManager.ItemService.DeleteUncommittedItems(True)
                        Dim newItemIds = vConnection.WebServiceManager.ItemService.EditItems(itemIds.ToArray()).Select(Function(t) t.RevId).ToArray()
                        Dim comItems() = vConnection.WebServiceManager.ItemService.UpdateItemProperties(newItemIds, newpropInstParamsArrays.ToArray())
                        vConnection.WebServiceManager.ItemService.UpdateAndCommitItems(comItems)
                        Return True
                    End If
                Catch ex As Exception
                    WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
                End Try
            Catch ex As Exception
                WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
            End Try
            Return False
        End Function

        Public Sub OnJobProcessorShutdown(ByVal context As IJobProcessorServices) Implements IJobHandler.OnJobProcessorShutdown

        End Sub

        Public Sub OnJobProcessorSleep(ByVal context As IJobProcessorServices) Implements IJobHandler.OnJobProcessorSleep
        End Sub

        Public Sub OnJobProcessorStartup(ByVal context As IJobProcessorServices) Implements IJobHandler.OnJobProcessorStartup

        End Sub

        Public Sub OnJobProcessorWake(ByVal context As IJobProcessorServices) Implements IJobHandler.OnJobProcessorWake

        End Sub

        Public Function getFileProperties(ByVal itemId As Long) As Dictionary(Of String, String)
            Try
                Dim props() As PropDef = vConnection.WebServiceManager.PropertyService.GetPropertyDefinitionsByEntityClassId("ITEM")
                Dim propDefIds() As Long
                propDefIds = (From prop In props
                              Select prop.Id).ToArray()
                Dim fileProp() As Autodesk.Connectivity.WebServices.PropInst = vConnection.WebServiceManager.PropertyService.GetProperties("ITEM", New Long() {itemId}, propDefIds.ToArray())

                Dim ret As Dictionary(Of String, String) = New Dictionary(Of String, String)
                For Each prop In props
                    Dim singlePropValue As String = String.Empty
                    Try
                        singlePropValue = (From _fp In fileProp
                                           Where _fp.PropDefId.Equals(prop.Id)
                                           Select _fp.Val).FirstOrDefault().ToString()
                    Catch ex As Exception
                        WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
                    End Try
                    If singlePropValue Is Nothing Then
                        singlePropValue = String.Empty
                    End If
                    ret.Add(prop.DispName, singlePropValue)
                Next
                Return ret
            Catch ex As Exception
                WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
            End Try
            Return Nothing
        End Function

#Region "Import Items AutoSync"
        Private Function UpdateItemProperties(ByVal vltName As String, ByVal lstProps As List(Of String), ByVal dr As DataRow) As Boolean
            Try
                Dim statusProp As String = "Imported from AX"
                Dim props() As PropDef = vConnection.WebServiceManager.PropertyService.GetPropertyDefinitionsByEntityClassId("ITEM")
                If Not lstProps.Contains(statusProp) Then
                    lstProps.Add(statusProp)
                End If
                Dim vaultProp() As PropDef = (From prop In props
                                           Where lstProps.Contains(prop.DispName, StringComparer.InvariantCultureIgnoreCase).Equals(True)
                                             Select prop).ToArray() ' prop.DispName.Equals("Revision", StringComparison.InvariantCultureIgnoreCase).Equals(True) Or
                Dim vPropIds() As Long = (From prps In vaultProp
                                         Select prps.Id).ToArray()

                Dim tobeModifiedCount = dtItemProp.Select("ToBeAdded = " & False).Count()
                '   Dim prpParmArray(tobeModifiedCount) As PropInstParamArray
                '  Dim index As Integer
                Dim itemIndex = 0
                Dim itemIds As New List(Of Long)
                Dim propInstParams As New List(Of PropInstParam)
                Dim propInstParamsArrays(tobeModifiedCount - 1) As PropInstParamArray
                Dim newpropInstParamsArrays As New List(Of PropInstParamArray)

                'For Each dr As DataRow In dtDBItems.Rows 'Select("dataAreaId = '" & vltName & "'")
                If dr.Item("ProductNo").ToString() <> String.Empty Then
                    propInstParams = New List(Of PropInstParam)
                    For Each tempRow As DataRow In dtItemProp.Select("ProductNo = '" + dr("ProductNo").ToString() + "'")
                        If tempRow("ToBeAdded").ToString().Equals("False", StringComparison.InvariantCultureIgnoreCase) Then
                            Dim myItem As Item = vConnection.WebServiceManager.ItemService.GetLatestItemByItemNumber(tempRow("ProductNo"))
                            itemIds.Add(Convert.ToDouble(tempRow("RevId").ToString()))
                            For Each vProp In vaultProp
                                For Each prp As PropDef In props.Where(Function(T) T.Id.Equals(vProp.Id))
                                    Dim propInst As New PropInstParam()

                                    propInst.PropDefId = prp.Id
                                    'prp.DispName.ToString()
                                    If prp.DispName.Equals(statusProp, StringComparison.InvariantCultureIgnoreCase) Then
                                        propInst.Val = "Yes"
                                    Else
                                        Dim colName As String = (From row As DataRow In dtItemColumnMapping.Rows Where row.Item(vltName).ToString().Equals(prp.DispName.ToString())
                                 Select DirectCast(row.Item("AX_Field"), String)).FirstOrDefault()

                                        propInst.Val = dr.Item(colName).ToString().ToUpper()
                                    End If

                                    If (propInst.Val Is Nothing) Then
                                        propInst.Val = String.Empty
                                    End If

                                    propInstParams.Add(propInst)
                                Next
                            Next
                            Dim propInstParamsArray As New PropInstParamArray()
                            propInstParamsArray.Items = propInstParams.ToArray()
                            propInstParamsArrays(itemIndex) = propInstParamsArray
                            newpropInstParamsArrays.Add(propInstParamsArray)
                            itemIndex = itemIndex + 1
                        End If
                    Next
                End If
                ' Next
                Try
                    If itemIds.Count <> 0 Then
                        vConnection.WebServiceManager.ItemService.DeleteUncommittedItems(True)
                        Dim newItemIds = vConnection.WebServiceManager.ItemService.EditItems(itemIds.ToArray()).Select(Function(t) t.RevId).ToArray()
                        Try
                            Dim comItems() = vConnection.WebServiceManager.ItemService.UpdateItemProperties(newItemIds, newpropInstParamsArrays.ToArray())
                            vConnection.WebServiceManager.ItemService.UpdateAndCommitItems(comItems)
                        Catch ex As Exception
                            WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
                            vConnection.WebServiceManager.ItemService.UndoEditItems(newItemIds)
                        End Try
                        Return True
                    End If
                Catch ex As Exception
                    WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
                End Try
            Catch ex As Exception
                WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
            End Try
            Return False
            '    MessageBox.Show("Unable to Update Properties! Kindly Try Again", "Update Items From CSV", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

        End Function

        Private Sub SetItemProperties(ByVal vlt As String)
            Try
                Dim lstProps = New List(Of String)
                dtItemProp = New DataTable()
                dtItemProp = dtDBItems.Clone()
                dtItemProp.Columns.Add("ToBeAdded")
                dtItemProp.Columns.Add("Id")
                dtItemProp.Columns.Add("RevId")
                ' dtItemProp.Columns.Add("V2AX_IntegrationStatus")
                If Not dtDBItems.Columns.Contains("V2AX_IntegrationStatus") Then
                    dtDBItems.Columns.Add("V2AX_IntegrationStatus")
                End If
                Dim itm As Item = Nothing
                GetAdminCredentials(vlt)
                SetdtVaultProps(vlt)
                lstProps = (From dr As DataRow In dtVaultProps.Rows
                               Where dr.Item("VaultName").ToString().Equals(vlt, StringComparison.InvariantCultureIgnoreCase)
                               Select DirectCast(dr.Item("VaultProp"), String)).ToList()
                Try
                    For Each dr As DataRow In dtDBItems.Rows 'Select("V2AX_IntegrationStatus <> 'SUCCESS'")
                        If dr.Item("ProductNo").ToString() <> String.Empty Then
                            Dim newRow As DataRow = dtItemProp.NewRow()
                            For ind As Integer = 0 To dtDBItems.Columns.Count - 1
                                newRow(ind) = dr(ind)
                            Next
                            Try
                                itm = vConnection.WebServiceManager.ItemService.GetLatestItemByItemNumber(dr.Item("ProductNo"))
                                newRow("Id") = itm.Id
                                newRow("RevId") = itm.RevId
                                newRow("ToBeAdded") = False
                                dr.Item("V2AX_IntegrationStatus") = "SUCCESS"
                            Catch ex As Exception
                                If AddItem(dr.Item("ProductNo").ToString()) Then
                                    itm = vConnection.WebServiceManager.ItemService.GetLatestItemByItemNumber(dr.Item("ProductNo"))
                                    newRow("ToBeAdded") = False
                                    newRow("Id") = itm.Id
                                    newRow("RevId") = itm.RevId
                                    dr.Item("V2AX_IntegrationStatus") = "SUCCESS"
                                Else
                                    newRow("ToBeAdded") = True
                                    newRow("Id") = -1
                                    newRow("RevId") = -1
                                    dr.Item("V2AX_IntegrationStatus") = "ERROR"
                                End If
                            End Try
                            dtItemProp.Rows.Add(newRow)
                        End If
                        Try
                            If UpdateItemProperties(vlt, lstProps, dr).Equals(False) Then
                                '  For Each dr As DataRow In dtDBItems.Select("V2AX_IntegrationStatus = 'SUCCESS'")
                                dr.Item("V2AX_IntegrationStatus") = "ERROR"
                                ' Next
                            ElseIf itm IsNot Nothing Then
                                MoveToRelease(itm)
                            End If
                        Catch ex As Exception
                            WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
                            ' For Each dr As DataRow In dtDBItems.Select("V2AX_IntegrationStatus = 'SUCCESS'")
                            dr.Item("V2AX_IntegrationStatus") = "ERROR"
                            '  Next
                        End Try
                    Next
                Catch ex As Exception
                    WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
                End Try
                UpdateImportStatus(dtDBItems)
            Catch ex As Exception
                WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
            End Try
        End Sub
        Private Sub MoveToRelease(ByVal itm As Item)
            Try
                Dim curLifecycle As LfCycDef = vConnection.WebServiceManager.LifeCycleService.GetLifeCycleDefinitionsByIds(New Long() {itm.LfCyc.LfCycDefId}).FirstOrDefault()
                Dim releaseId = (From st In curLifecycle.StateArray
                                Where st.ReleasedState.Equals(True)
                                Select st.Id).FirstOrDefault()
                If curLifecycle IsNot Nothing And itm.LfCycStateId <> releaseId Then
                    vConnection.WebServiceManager.ItemService.UpdateItemLifeCycleStates(New Long() {itm.MasterId}, New Long() {releaseId}, "")
                End If
            Catch ex As Exception

            End Try
        End Sub
        Private Function AddItem(ByVal itemNumber As String) As Boolean
            Dim retValue As Boolean = True
            Try


                Dim itemSvc As ItemService = vConnection.WebServiceManager.ItemService

                Dim categories As Cat() = vConnection.WebServiceManager.CategoryService.GetCategoriesByEntityClassId("ITEM", True)

                Dim catId As Long = -1
                For Each category As Cat In categories
                    If (category.Name = "Part") Then
                        catId = category.Id
                    End If
                Next

                Dim editableItem As Item = vConnection.WebServiceManager.ItemService.AddItemRevision(catId)

                Dim numSchms() As NumSchm

                numSchms = itemSvc.GetNumberingSchemesByType(NumSchmType.Activated)



                Dim numArr As ArrayList

                numArr = New ArrayList

                Dim _numSchm As NumSchm



                For Each _numSchm In numSchms

                    If (_numSchm.Name = "Mapped") Then numArr.Add(_numSchm.SchmID)

                Next



                Dim numSchmIds() As Long = CType(numArr.ToArray(GetType(System.Int64)), Long())



                Dim masterIds(0) As Long

                masterIds(0) = editableItem.MasterId


                Dim newItem() As String

                newItem = New String() {itemNumber}

                Dim fieldInputs(0) As StringArray

                Dim tempArr As New StringArray

                tempArr.Items = newItem

                fieldInputs(0) = tempArr

                Dim restric() As ProductRestric = Nothing

                Dim numbers As ItemNum() = itemSvc.AddItemNumbers(masterIds, numSchmIds, fieldInputs, restric)

                editableItem.ItemNum = numbers(0).ItemNum1


                ' commit the item, which finalizes the object

                Dim items(0) As Item

                items(0) = editableItem

                itemSvc.UpdateAndCommitItems(items)

            Catch ex As Exception
                retValue = False
                WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
            End Try
            Return retValue
        End Function
#End Region



    End Class



End Namespace

