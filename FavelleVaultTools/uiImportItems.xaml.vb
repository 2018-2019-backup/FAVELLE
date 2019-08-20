
Imports System.ServiceModel
Imports System.Xml
Imports FavelleVaultTools.wsFavelle
Imports System.Data
Imports Autodesk.Connectivity.WebServices
Imports Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections
Imports Autodesk.Connectivity.WebServicesTools

Public Class uiImportItems
    Dim dtDBItems As DataTable
    Dim dtPropertyMapping As DataTable
    Dim dtVaultProps As DataTable
    Private dtItemProp As DataTable
    Private Sub uiImportItems_Loaded(sender As Object, e As RoutedEventArgs) Handles MyBase.Loaded
        Try
            If wsGetVaultDetails() Then
                dtDBItems = GetImportItems()
                dtPropertyMapping = GetDBPropertyMapping()
                dtVaultProps = GetVaultsProps()
                ' dtPropertyMapping = Get
                If dtDBItems IsNot Nothing Then
                    dgItems.ItemsSource = dtDBItems.DefaultView()
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Function UpdateItemProperties(ByVal vltName As String, ByVal lstProps As List(Of String)) As Boolean
        Try
            Dim props() As PropDef = adminVConnection.WebServiceManager.PropertyService.GetPropertyDefinitionsByEntityClassId("ITEM")

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

            For Each dr As DataRow In dtDBItems.Rows 'Select("dataAreaId = '" & vltName & "'")
                If dr.Item("V2AX_ItemId").ToString() <> String.Empty Then
                    propInstParams = New List(Of PropInstParam)
                    For Each tempRow As DataRow In dtItemProp.Select("V2AX_ItemId = '" + dr("V2AX_ItemId").ToString() + "'")
                        If tempRow("ToBeAdded").ToString().Equals("False", StringComparison.InvariantCultureIgnoreCase) Then
                            Dim myItem As Item = adminVConnection.WebServiceManager.ItemService.GetLatestItemByItemNumber(tempRow("V2AX_ItemId"))
                            itemIds.Add(Convert.ToDouble(tempRow("RevId").ToString()))
                            For Each vProp In vaultProp
                                For Each prp As PropDef In props.Where(Function(T) T.Id.Equals(vProp.Id))
                                    Dim propInst As New PropInstParam()

                                    propInst.PropDefId = prp.Id
                                    'prp.DispName.ToString()
                                    propInst.Val = dr.Item((From row As DataRow In dtPropertyMapping Where row.Item("VaultProp").ToString().Equals(prp.DispName.ToString())
                                        Select DirectCast(row.Item("DBColumn"), String)).FirstOrDefault()).ToString()
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
            Next
            Try
                If itemIds.Count <> 0 Then
                    adminVConnection.WebServiceManager.ItemService.DeleteUncommittedItems(True)
                    Dim newItemIds = adminVConnection.WebServiceManager.ItemService.EditItems(itemIds.ToArray()).Select(Function(t) t.RevId).ToArray()
                    Dim comItems() = adminVConnection.WebServiceManager.ItemService.UpdateItemProperties(newItemIds, newpropInstParamsArrays.ToArray())
                    adminVConnection.WebServiceManager.ItemService.UpdateAndCommitItems(comItems)
                    Return True
                End If
            Catch ex As Exception
                '  vCon.WebServiceManager.ItemService.UndoEditItems(itemIds.ToArray)
                ' CreateNotification("Unable to Set Properties for Items." & ex.Message)
                ' WriteLog(ex.Message.ToString)
            End Try
        Catch ex As Exception
            ' CreateNotification("Unable to Set Properties for Items." & ex.Message)
            ' WriteLog(ex.Message.ToString)
        End Try
        Return False
        '    MessageBox.Show("Unable to Update Properties! Kindly Try Again", "Update Items From CSV", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

    End Function

    Private Sub SetItemProperties(ByVal lstVault As List(Of String))
        Try
            Dim lstProps = New List(Of String)
            dtItemProp = New DataTable()
            dtItemProp = dtDBItems.Clone()
            dtItemProp.Columns.Add("ToBeAdded")
            dtItemProp.Columns.Add("Id")
            dtItemProp.Columns.Add("RevId")
            For Each vlt In lstVault
                If GetAdminCredentials(vlt) Then
                    lstProps = (From dr As DataRow In dtVaultProps
                               Where dr.Item("VaultName").ToString().Equals(vlt, StringComparison.InvariantCultureIgnoreCase)
                               Select DirectCast(dr.Item("VaultProp"), String)).ToList()
                    Try
                        For Each dr As DataRow In dtDBItems.Rows 'Select("dataAreaId = '" & vlt & "'")
                            If dr.Item("V2AX_ItemId").ToString() <> String.Empty Then
                                Dim newRow As DataRow = dtItemProp.NewRow()
                                For ind As Integer = 0 To dtDBItems.Columns.Count - 1
                                    newRow(ind) = dr(ind)
                                Next
                                Try
                                    Dim itm As Item = adminVConnection.WebServiceManager.ItemService.GetLatestItemByItemNumber(dr.Item("V2AX_ItemId"))
                                    newRow("Id") = itm.Id
                                    newRow("RevId") = itm.RevId
                                    newRow("ToBeAdded") = False
                                    dr.Item("V2AX_IntegrationStatus") = "SUCCESS"
                                Catch ex As Exception
                                    If AddItem(dr.Item("V2AX_ItemId").ToString()) Then
                                        Dim itm As Item = adminVConnection.WebServiceManager.ItemService.GetLatestItemByItemNumber(dr.Item("V2AX_ItemId"))
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
                        Next
                    Catch ex As Exception
                        ' WriteLog(ex.Message.ToString)
                    End Try
                    Try
                        If UpdateItemProperties(vlt, lstProps).Equals(False) Then
                            For Each dr As DataRow In dtDBItems.Select("dataAreaId = '" & vlt & "' AND V2AX_IntegrationStatus = 'SUCCESS'")
                                dr.Item("V2AX_IntegrationStatus") = "ERROR"
                            Next
                        End If
                    Catch ex As Exception
                        For Each dr As DataRow In dtDBItems.Select("dataAreaId = '" & vlt & "' AND V2AX_IntegrationStatus = 'SUCCESS'")
                            dr.Item("V2AX_IntegrationStatus") = "ERROR"
                        Next
                    End Try
                End If
            Next
            If UpdateImportStatus(dtDBItems) Then
                MessageBox.Show("Items are imported/updated successfully! Kindly check the status column for individual item status", "Favelle Vault Tools", MessageBoxButton.OK, MessageBoxImage.Information)
            Else
                MessageBox.Show("Unable to update the status in stagging table. Kindly try again!", "Favelle Vault Tools", MessageBoxButton.OK, MessageBoxImage.Information)
            End If
        Catch ex As Exception
            MessageBox.Show("Unexpected error occured. Kindly try again!", "Favelle Vault Tools", MessageBoxButton.OK, MessageBoxImage.Information)
        End Try
    End Sub

    Private Function AddItem(ByVal itemNumber As String) As Boolean
        Dim retValue As Boolean = True
        Try


            Dim itemSvc As ItemService = adminVConnection.WebServiceManager.ItemService

            Dim categories As Cat() = adminVConnection.WebServiceManager.CategoryService.GetCategoriesByEntityClassId("ITEM", True)

            Dim catId As Long = -1
            For Each category As Cat In categories
                If (category.Name = "Part") Then
                    catId = category.Id
                End If
            Next

            Dim editableItem As Item = adminVConnection.WebServiceManager.ItemService.AddItemRevision(catId)

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
            ' WriteLog(ex.Message.ToString)
            ' CreateNotification("Unable to Add Item " & itemNumber & ". " & ex.Message.ToString())
        End Try
        Return retValue
    End Function

    Private Sub btnImport_Click_1(sender As Object, e As RoutedEventArgs)
        Try
            Dim lstVault As List(Of String) = New List(Of String)
            'If rdbtnAllVault.IsChecked Then
            '  SetItemProperties(adminObj.lstVaults.Select(Function(x) x).ToList())
            'Else
            'SetItemProperties(New List(Of String)({curVault}))
            'End If
        Catch ex As Exception
            MessageBox.Show("Unexpected error occured. Kindly try again!", "Favelle Vault Tools", MessageBoxButton.OK, MessageBoxImage.Information)
        Finally

        End Try
    End Sub
End Class
