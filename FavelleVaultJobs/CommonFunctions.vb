Imports System.Xml
Imports axFavelle
Imports axFavelleBOM
Imports System.ServiceModel
Imports Autodesk.Connectivity.WebServicesTools
Imports Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections
Imports System.Data
Imports Autodesk.DataManagement.Client.Framework

Module CommonFunctions
    Dim settingsFile = "C:\ProgramData\Autodesk\Vault 2016\Extensions\FavelleVaultJobs\FavelleVaultJobs.dll.config"
    Public _dirSep As String = System.IO.Path.DirectorySeparatorChar.ToString()
    Public appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + _dirSep + "Vault2AX" + _dirSep
    Public applicationSettings = appDataPath + "Settings.xml"
    Public dtVaultOrg, dtItemColumnMapping, dtBOMColumnMapping, dtUserEmpIdMapping As DataTable
    Public catValues As Dictionary(Of String, String) = New Dictionary(Of String, String)
    'Dim wsObj As wsMethodsSoapClient
    Dim endPointUrl, endPointUrlBom As String
    Dim client As V2AX_ItemListServicesClient
    Dim clientBOM As V2AX_BOMListServicesClient
    Dim context As axFavelle.CallContext
    Dim contextBOM As axFavelleBOM.CallContext
    'Dim remoteAddress As EndpointAddress
    Public propToUpdate, vUserName, vPwd, vServer, bomPropToUpdate As String
    Dim adminCredentials As UserPasswordCredentials
    ' Public adminVConnection As Connection
    Dim adminServiceManager As WebServiceManager
    Public errorMsg = String.Empty
    Public vConnection As Vault.Currency.Connections.Connection
    Public dtFormat = "MM/dd/yyyy"
    Public Function SetClient(ax As String) As Boolean
        Dim retValue As Boolean = False
        Try
            client = New V2AX_ItemListServicesClient("NetTcpBinding_V2AX_ItemListServices", endPointUrl)
            client.ClientCredentials.Windows.ClientCredential.Domain = "FFCM"
            client.ClientCredentials.Windows.ClientCredential.UserName = "vault1"
            client.ClientCredentials.Windows.ClientCredential.Password = "VA$123"
            client.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation
            context = New axFavelle.CallContext()

            DirectCast(DirectCast(DirectCast(client, System.ServiceModel.ClientBase(Of V2AX_ItemListServices)).ChannelFactory, System.ServiceModel.ChannelFactory).Endpoint.Binding, System.ServiceModel.NetTcpBinding).MaxBufferSize = 999999999

            DirectCast(DirectCast(DirectCast(client, System.ServiceModel.ClientBase(Of V2AX_ItemListServices)).ChannelFactory, System.ServiceModel.ChannelFactory).Endpoint.Binding, System.ServiceModel.NetTcpBinding).MaxReceivedMessageSize = 999999999

            DirectCast(DirectCast(DirectCast(client, System.ServiceModel.ClientBase(Of V2AX_ItemListServices)).ChannelFactory, System.ServiceModel.ChannelFactory).Endpoint.Binding, System.ServiceModel.NetTcpBinding).ReceiveTimeout = New TimeSpan(0, 10, 0)

            DirectCast(DirectCast(DirectCast(client, System.ServiceModel.ClientBase(Of V2AX_ItemListServices)).ChannelFactory, System.ServiceModel.ChannelFactory).Endpoint.Binding, System.ServiceModel.NetTcpBinding).SendTimeout = New TimeSpan(0, 10, 0)

            DirectCast(DirectCast(DirectCast(client, System.ServiceModel.ClientBase(Of V2AX_ItemListServices)).ChannelFactory, System.ServiceModel.ChannelFactory).Endpoint.Binding, System.ServiceModel.NetTcpBinding).OpenTimeout = New TimeSpan(0, 10, 0)

            DirectCast(DirectCast(DirectCast(client, System.ServiceModel.ClientBase(Of V2AX_ItemListServices)).ChannelFactory, System.ServiceModel.ChannelFactory).Endpoint.Binding, System.ServiceModel.NetTcpBinding).CloseTimeout = New TimeSpan(0, 10, 0)
            context.Company = ax
            retValue = True
        Catch ex As Exception
            errorMsg = "Unable to connect to AX Item Service."
            WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
        Return retValue
    End Function

    Public Function SetClientBOM(ax As String) As Boolean
        Dim retValue As Boolean = False
        Try
            clientBOM = New V2AX_BOMListServicesClient("NetTcpBinding_V2AX_BOMListServices", endPointUrlBom)
            clientBOM.ClientCredentials.Windows.ClientCredential.Domain = "FFCM"
            clientBOM.ClientCredentials.Windows.ClientCredential.UserName = "vault1"
            clientBOM.ClientCredentials.Windows.ClientCredential.Password = "VA$123"
            clientBOM.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation
            contextBOM = New axFavelleBOM.CallContext()

            'DirectCast(DirectCast(DirectCast(clientBOM, System.ServiceModel.ClientBase(Of V2AX_BOMListServicesClient)).ChannelFactory, System.ServiceModel.ChannelFactory).Endpoint.Binding, System.ServiceModel.NetTcpBinding).MaxBufferSize = 999999999

            'DirectCast(DirectCast(DirectCast(clientBOM, System.ServiceModel.ClientBase(Of V2AX_BOMListServicesClient)).ChannelFactory, System.ServiceModel.ChannelFactory).Endpoint.Binding, System.ServiceModel.NetTcpBinding).MaxReceivedMessageSize = 999999999

            'DirectCast(DirectCast(DirectCast(clientBOM, System.ServiceModel.ClientBase(Of V2AX_BOMListServicesClient)).ChannelFactory, System.ServiceModel.ChannelFactory).Endpoint.Binding, System.ServiceModel.NetTcpBinding).ReceiveTimeout = New TimeSpan(0, 10, 0)

            'DirectCast(DirectCast(DirectCast(clientBOM, System.ServiceModel.ClientBase(Of V2AX_BOMListServicesClient)).ChannelFactory, System.ServiceModel.ChannelFactory).Endpoint.Binding, System.ServiceModel.NetTcpBinding).SendTimeout = New TimeSpan(0, 10, 0)

            'DirectCast(DirectCast(DirectCast(clientBOM, System.ServiceModel.ClientBase(Of V2AX_BOMListServicesClient)).ChannelFactory, System.ServiceModel.ChannelFactory).Endpoint.Binding, System.ServiceModel.NetTcpBinding).OpenTimeout = New TimeSpan(0, 10, 0)

            'DirectCast(DirectCast(DirectCast(clientBOM, System.ServiceModel.ClientBase(Of V2AX_BOMListServicesClient)).ChannelFactory, System.ServiceModel.ChannelFactory).Endpoint.Binding, System.ServiceModel.NetTcpBinding).CloseTimeout = New TimeSpan(0, 10, 0)
            contextBOM.Company = ax
            retValue = True
        Catch ex As Exception
            errorMsg = "Unable to connect to AX BOM Service"
            WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
        Return retValue
    End Function
    Public Sub WriteLog(ByVal ex As Exception)
        Try
            Dim str = String.Empty
            Dim log As IO.StreamWriter
            Dim logfile As String = appDataPath & "log.txt"
            str = ex.Message
            If ex.StackTrace IsNot Nothing Then
                str = str + vbNewLine + ex.StackTrace.ToString()
            End If
            If ex.InnerException IsNot Nothing Then
                str = str + vbNewLine + ex.InnerException.ToString()
            End If
            If Not System.IO.File.Exists(logfile) Then
                log = New IO.StreamWriter(logfile)
            Else
                log = System.IO.File.AppendText(logfile)
            End If
            log.WriteLine()
            log.WriteLine(Now.ToString() + ": " + str)
            log.Close()
        Catch

        End Try
    End Sub
    Public Sub WriteLog(ByVal str As String)
        Try
            Dim log As IO.StreamWriter
            Dim logfile As String = appDataPath & "log.txt"
            If Not System.IO.File.Exists(logfile) Then
                log = New IO.StreamWriter(logfile)
            Else
                log = System.IO.File.AppendText(logfile)
            End If
            log.WriteLine()
            log.WriteLine(Now.ToString() + ": " + str)
            log.Close()
        Catch ex As Exception
            ' WriteLog( ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
    End Sub

    Public Function GetRecCount(ByVal qry As String) As Integer
        Dim retValue As Integer = 0
        Try
            'If getEndPoint() Then
            '    Using wsObj = New wsMethodsSoapClient(New System.ServiceModel.BasicHttpBinding(), remoteAddress)
            '        wsObj.Endpoint.Binding.SendTimeout = New TimeSpan(0, 0, 0, 3)
            '        retValue = wsObj.IfItemExistsItemImport(qry)
            '    End Using
            'End If
        Catch ex As Exception
            WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
        Return retValue
    End Function

    Public Function ExportItemToAX(ByVal itemCode As String, ByVal chargeCode As String, ByVal desc As String, ByVal pName As String, ByVal remark As String, ByVal srchName As String, ByVal spec As String, ByVal pcControl As Boolean, ByVal ax As String, ByVal itemGrp As String, ByVal bomUnit As String, ByVal cat As String, ByVal prjCat As String) As Boolean
        Dim retValue As Boolean = False
        Try
            If SetClient(ax) Then
                Dim param As axFavelle.V2AX_ItemListDataContract = New axFavelle.V2AX_ItemListDataContract
                param.Company = ax
                param.ChargesCode = chargeCode
                param.Description = desc
                param.PcsControlled = pcControl
                param.ProductName = pName
                param.ProductNo = itemCode
                param.Remarks = remark
                param.Specification = spec
                param.SearchName = srchName
                param.ItemGroup = itemGrp
                'added as per CR on 09 May, 2017
                param.BOMUnit = bomUnit
                Dim stIndex = cat.IndexOf("(")
                Dim endIndex = cat.IndexOf(")")
                If stIndex <> -1 And endIndex <> -1 Then
                    Integer.TryParse(cat.Substring(stIndex + 1, endIndex - (stIndex + 1)).Trim, param.CAT)
                Else
                    param.CAT = 0
                End If

                param.ProjCategory = prjCat

                If ValidateItemValues(param) Then
                    Dim status = String.Empty
                    If client.itemExists(context, itemCode) Then
                        Try
                            retValue = client.modifyItem(context, param)
                            status = "Item added to Item Modification table."
                        Catch ex As Exception
                            errorMsg = "Unable to modify the item in AX." + Environment.NewLine + ex.Message
                            status = ex.Message
                        End Try
                    Else
                        Try
                            retValue = client.CreateItem(context, param)
                            status = "Item added to Item Export table."
                        Catch ex As Exception
                            errorMsg = "Unable to add item to AX." + Environment.NewLine + ex.Message
                            WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
                            status = ex.Message
                        End Try
                    End If
                    Rfc4180Writer.WriteLogItemExport(param, status)
                    retValue = True
                Else
                    errorMsg = "Kindly fill all the fields required for Item Export. Check Vault2AXItemExport log in server for more info. " + Environment.NewLine + validationMsg
                    Rfc4180Writer.WriteLogItemExport(param, "Validation Error! All the fields should contain value.")
                End If
            End If
        Catch ex As Exception
            WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
        Return retValue
    End Function
    Public Function ValidateBOMValues(ByVal param As axFavelleBOM.V2AX_BOMHeaderDataContract) As Boolean
        Dim retValue As Boolean = True
        validationMsg = String.Empty
        Dim tempLst As List(Of String) = New List(Of String)
        Dim tempLst1 As List(Of String) = New List(Of String)
        Try
            If param.BOMNo.Equals(String.Empty) Then
                tempLst.Add("BOM No (<ItemCode>-<Revision>)")
                retValue = False
            End If
            If param.BOMName.Equals(String.Empty) Then
                tempLst.Add("Item Code")
                retValue = False
            End If
            If param.CheckedBy.Equals(String.Empty) Then
                tempLst.Add("Engg Checked By")
                retValue = False
            End If
            If param.ApprovedBy.Equals(String.Empty) Then
                tempLst.Add("Approved By")
                retValue = False
            End If
            'If param.BomQty Is Nothing Then
            '    Return False
            'End If
            If param.RevDescChanges.Equals(String.Empty) Then
                tempLst.Add("Description Rev")
                retValue = False
            End If
            If param.RevNo.Equals(String.Empty) Then
                tempLst.Add("Revision")
                retValue = False
            End If
            If param.SN.Equals(String.Empty) Then
                tempLst.Add("First Use SN")
                retValue = False
            End If
            If param.ItemNo.Equals(String.Empty) Then
                tempLst.Add("Item Code")
                retValue = False
            End If
            If (tempLst.Count > 0) Then
                validationMsg += " Kindly verify " + String.Join(",", tempLst) + " in " + param.ItemNo + ". " + Environment.NewLine
            End If
            For Each p In param.BomLine
                tempLst1.Clear()
                If p.ItemNo.Equals(String.Empty) Then
                    tempLst1.Add("Item Code")
                    retValue = False
                End If
                If p.RevNo.Equals(String.Empty) Then
                    tempLst1.Add("BOM CHG REV")
                    retValue = False
                End If
                If p.CAT.ToString().Equals(String.Empty) Then
                    tempLst1.Add("CAT")
                    retValue = False
                End If
                If p.BomQty.ToString().Equals(String.Empty) Then
                    tempLst1.Add("QTY")
                    retValue = False
                End If
                If p.Length.ToString().Equals(String.Empty) Then
                    tempLst1.Add("BOM Length")
                    retValue = False
                End If
                If p.Weight.ToString().Equals(String.Empty) Then
                    tempLst1.Add("BOM Weight")
                    retValue = False
                End If
                If p.Width.ToString().Equals(String.Empty) Then
                    tempLst1.Add("BOM Width")
                    retValue = False
                End If
                If p.Remarks.ToString().Equals(String.Empty) Then
                    tempLst1.Add("BOMRemarks")
                    retValue = False
                End If
                If p.Position.ToString().Equals(String.Empty) Then
                    tempLst1.Add("Position")
                    retValue = False
                End If
                If p.PCS.ToString().Equals(String.Empty) Then
                    tempLst1.Add("BOM PCS")
                    retValue = False
                End If
                If (tempLst1.Count > 0) Then
                    validationMsg += " Kindly verify " + String.Join(",", tempLst1) + " in " + p.ItemNo + Environment.NewLine
                End If
            Next

        Catch ex As Exception
            retValue = False
        End Try
        Return retValue
    End Function
    Public validationMsg As String
    Public Function ValidateItemValues(ByVal param As axFavelle.V2AX_ItemListDataContract) As Boolean
        Dim retValue As Boolean = True
        validationMsg = String.Empty
        Dim tempLst As List(Of String) = New List(Of String)
        Try
            If param.ProductNo.Equals(String.Empty) Then
                tempLst.Add("Item Code")
                retValue = False
            End If
            If param.ChargesCode.Equals(String.Empty) Then
                tempLst.Add("Charge Code")
                retValue = False
            End If
            If param.ItemGroup.Equals(String.Empty) Then
                tempLst.Add("Item Group")
                retValue = False
            End If
            If param.Description.Equals(String.Empty) Then
                tempLst.Add("Item Code Description")
                retValue = False
            End If
            If param.PcsControlled.Equals(String.Empty) Then
                tempLst.Add("Pcs Controlled")
                retValue = False
            End If
            If param.ProductName.Equals(String.Empty) Then
                tempLst.Add("Item Code Description")
                retValue = False
            End If
            If param.Remarks.Equals(String.Empty) Then
                tempLst.Add("Remark")
                retValue = False
            End If
            If param.Specification.Equals(String.Empty) Then
                tempLst.Add("Specification")
                retValue = False
            End If
            If param.SearchName.Equals(String.Empty) Then
                tempLst.Add("Search Name")
                retValue = False
            End If
            If param.CAT.ToString().Equals(String.Empty) Then
                tempLst.Add("CAT")
                retValue = False
            End If
            If param.BOMUnit.ToString().Equals(String.Empty) Then
                tempLst.Add("Inventory UOM")
                retValue = False
            End If
            If param.ProjCategory.ToString().Equals(String.Empty) Then
                tempLst.Add("ProjectCategory")
                retValue = False
            End If
            If tempLst.Count > 0 Then
                validationMsg = String.Join(",", tempLst)
            End If
        Catch ex As Exception
            retValue = False
        End Try
        Return retValue
    End Function
    Public Function ExportBOMToAX(ByVal bomNo As String, ByVal bomName As String, ByVal itemNo As String, ByVal appBy As String, ByVal chkBy As String, ByVal revNo As String, ByVal sn As String, ByVal revDesc As String, ByVal qty As Decimal, ByVal ax As String, ByVal modifiedDate As String, ByVal ChildIds() As V2AX_BOMLineDataContract) As Boolean
        Dim retValue As Boolean = False
        Try
            If SetClientBOM(ax) Then
                Dim param As V2AX_BOMHeaderDataContract = New V2AX_BOMHeaderDataContract()
                param.BOMNo = bomNo
                If modifiedDate IsNot Nothing And modifiedDate <> String.Empty Then
                    param.BOMDate = modifiedDate
                Else
                    param.BOMDate = Now.ToString(dtFormat)
                End If
                param.BOMName = bomName
                param.ItemNo = itemNo
                Dim approvedBy = (From row As DataRow In dtUserEmpIdMapping.Rows
                                   Where row(0) IsNot Nothing And row(0).ToString.Equals(appBy, StringComparison.InvariantCultureIgnoreCase)
                                   Select row(1)).FirstOrDefault()
                If approvedBy IsNot Nothing Then
                    param.ApprovedBy = approvedBy.ToString()
                Else
                    param.ApprovedBy = String.Empty
                End If
                Dim checkedBy = String.Empty
                If chkBy IsNot Nothing Then
                    checkedBy = (From row As DataRow In dtUserEmpIdMapping.Rows
                    Where row(0) IsNot Nothing And row(0).ToString.Equals(chkBy, StringComparison.InvariantCultureIgnoreCase)
                                       Select row(1)).FirstOrDefault()
                End If
                If checkedBy IsNot Nothing Then

                    param.CheckedBy = checkedBy.ToString()
                Else
                    param.CheckedBy = String.Empty
                End If
                param.RevNo = revNo
                param.BomQty = qty
                param.SN = sn
                param.RevDescChanges = revDesc
                param.BomLine = ChildIds
                param.BOMDate = modifiedDate
                Dim status As String = String.Empty
                If ValidateBOMValues(param) Then
                    Try
                        retValue = clientBOM.CreateBOMHeader(contextBOM, param)
                        If retValue.Equals(True) Then
                            status = "BOM Added to AX."
                        Else
                            status = "BOM not accepted by AX. Kindly remove the existing BOM in AX and try again."
                            errorMsg = status
                        End If
                    Catch ex As Exception
                        errorMsg = "Unable to send BOM to AX." + Environment.NewLine + ex.Message
                        WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
                        status = ex.Message
                    End Try
                Else
                    errorMsg = "Kindly fill all the fields required for BOM Export. Check Vault2AXBOM_HeaderExport and Vault2AXBOM_LineExport log in server for more info." + Environment.NewLine + validationMsg
                    status = "Validation Error! All the fields in BOM Header and Line Item requies value."
                End If
                Rfc4180Writer.WriteLogBOMExport(param, status)
            End If
        Catch ex As Exception
            errorMsg = "Unable to send BOM to AX." + "1" + Environment.NewLine + ex.Message
            WriteLog(ex)
        End Try
        Return retValue
    End Function

    Public Function GetImportItems(ByVal ax As String) As DataTable
        Dim retValue As DataTable = Nothing
        Try
            If SetClient(ax) Then
                Dim itemContract() As axFavelle.V2AX_ItemListDataContract = client.getItemList(context)
                If itemContract.Length() > 0 Then
                    retValue = New DataTable()
                    Dim fields() = itemContract.First.GetType.GetProperties
                    For Each field In fields
                        retValue.Columns.Add(field.Name) ', field.PropertyType
                    Next
                    For Each item In itemContract
                        Dim row As DataRow = retValue.NewRow()
                        For Each field In fields
                            Dim p = item.GetType.GetProperty(field.Name)
                            If field.Name.Equals("CAT", StringComparison.InvariantCultureIgnoreCase) Then
                                row(field.Name) = catValues(p.GetValue(item, Nothing))
                            Else
                                row(field.Name) = p.GetValue(item, Nothing)
                            End If
                        Next
                        retValue.Rows.Add(row)
                    Next
                End If
            End If
        Catch ex As Exception
            errorMsg = "Unable to import items from " + ax + "(AX)." + Environment.NewLine + ex.Message
            WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
        Return retValue
    End Function

    Public Function UpdateImportStatus(ByVal dt As DataTable) As Boolean
        Dim retValue As Boolean = False
        Try
            Dim csvFile As String = appDataPath + "AX2VaultItemImport-" + Now.ToString("ddMMyy hhmmss") + ".csv"
            Using writer As IO.StreamWriter = New IO.StreamWriter(csvFile)
                Rfc4180Writer.WriteDataTable(dt, writer, True)
            End Using

            Dim status As Integer = -1
            For Each dr As DataRow In dt.Rows
                If dr("V2AX_IntegrationStatus").Equals("SUCCESS") Then
                    status = 1
                Else
                    status = 2
                End If
                Dim value As String = client.updateItemExportStatus(context, dr("ProductNo").ToString(), dr("IntegrationDate").ToString(), status)
            Next
        Catch ex As Exception
            WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
        Return retValue
    End Function

    Public Function GetAdminCredentials(ByVal vaultName As String) As Boolean
        Try
            adminCredentials = New UserPasswordCredentials(vServer, vaultName, vUserName, vPwd)
            adminServiceManager = New WebServiceManager(adminCredentials)
            Dim userId As Long = adminCredentials.SecurityHeader.UserId
            vConnection = New Connection(adminServiceManager, vaultName, userId, vServer, AuthenticationFlags.Standard)
            Return True
        Catch ex As Exception
            WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
            Return False
        End Try
    End Function

#Region "Application Configurations"

    Public Function LoadDataFromSettingsFile() As Boolean
        Dim retValue As Boolean = False
        Try
            If System.IO.File.Exists(applicationSettings) Then
                Dim settingsXml As New XmlDocument
                Dim rootNode, xnBasicSettings, xnPropMapping, xnVaultJobs, xnBOMPropMapping, xnUserMapping, xnCATValues As XmlNode
                settingsXml.Load(applicationSettings)
                rootNode = settingsXml.SelectSingleNode("Settings")
                xnBasicSettings = rootNode.SelectSingleNode("BasicSettings")
                xnPropMapping = rootNode.SelectSingleNode("ItemPropertyMapping")
                xnVaultJobs = rootNode.SelectSingleNode("VaultJobs")
                xnBOMPropMapping = rootNode.SelectSingleNode("BomPropertyMapping")
                xnUserMapping = rootNode.SelectSingleNode("VaultUsers")
                xnCATValues = rootNode.SelectSingleNode("CATValueMapping")
                LoadBasicSettings(xnBasicSettings)
                LoadPropMapping(xnPropMapping)
                LoadVaultJobs(xnVaultJobs)
                LoadBOMPropMapping(xnBOMPropMapping)
                LoadUserMapping(xnUserMapping)
                LoadCATValueMapping(xnCATValues)
                retValue = True
            Else
                WriteLog(Now.ToString() & ": " & "Settings file does not exists." & vbNewLine)
            End If
        Catch ex As Exception
            WriteLog(Now.ToString() & ": " & ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
        Return retValue
    End Function

    Private Sub LoadCATValueMapping(ByVal xn As XmlNode)
        Try
            catValues.Clear()
            For Each node As XmlNode In xn.SelectNodes("CAT")
                catValues.Add(node.SelectSingleNode("dcCATAx").InnerText, node.SelectSingleNode("dcCATVault").InnerText)
            Next
        Catch ex As Exception

        End Try
    End Sub
    Private Sub LoadBasicSettings(xn As XmlNode)
        Try
            vUserName = xn.SelectSingleNode("vUSername").InnerText
            vPwd = xn.SelectSingleNode("vPassword").InnerText
            vServer = xn.SelectSingleNode("vServer").InnerText
            endPointUrl = xn.SelectSingleNode("AXService").InnerText
            endPointUrlBom = xn.SelectSingleNode("AXBOMService").InnerText
            dtVaultOrg = New DataTable()
            dtVaultOrg.Columns.Add("Vault")
            dtVaultOrg.Columns.Add("Organisation")
            For Each node As XmlNode In xn.SelectNodes("VO")
                dtVaultOrg.Rows.Add(node.SelectSingleNode("Vault").InnerText, node.SelectSingleNode("Organisation").InnerText)
            Next
        Catch ex As Exception
            WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
    End Sub

    Private Sub LoadVaultJobs(ByVal xn As XmlNode)
        Try
            If xn IsNot Nothing Then
                propToUpdate = xn.SelectSingleNode("ItemState").InnerText
                bomPropToUpdate = xn.SelectSingleNode("BOMProp").InnerText
            End If
        Catch ex As Exception
            WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
    End Sub

    Public Sub LoadUserMapping(ByVal xn As XmlNode)
        Try
            dtUserEmpIdMapping = New DataTable()
            dtUserEmpIdMapping.Columns.Clear()
            'dtUserEmpIdMapping.Columns.Add("Vault_Users")
            'dtUserEmpIdMapping.Columns.Add("Employee_Code")
            For Each childNode As XmlNode In xn.ChildNodes(0).ChildNodes
                dtUserEmpIdMapping.Columns.Add(childNode.Name)
            Next
            For Each childNode As XmlNode In xn.ChildNodes
                Dim dr As DataRow = dtUserEmpIdMapping.NewRow
                For Each node As XmlNode In childNode.ChildNodes
                    dr(node.Name) = node.InnerText.Trim()
                Next
                dtUserEmpIdMapping.Rows.Add(dr)
            Next
        Catch ex As Exception
            WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
    End Sub

    Private Sub LoadBOMPropMapping(ByVal xn As XmlNode)
        Try
            dtBOMColumnMapping = New DataTable
            If dtVaultOrg.Rows.Count > 0 And xn IsNot Nothing Then
                dtBOMColumnMapping.Columns.Add("AX_Field")
                For Each row In dtVaultOrg.Rows
                    If Not dtBOMColumnMapping.Columns.Contains(row(0).ToString().Trim()) Then
                        dtBOMColumnMapping.Columns.Add(row(0).ToString().Trim())
                    End If
                Next
                For Each childNode As XmlNode In xn.ChildNodes
                    Dim dr As DataRow = dtBOMColumnMapping.NewRow
                    For Each node As XmlNode In childNode.ChildNodes
                        dr(node.Name) = node.InnerText.Trim()
                    Next
                    dtBOMColumnMapping.Rows.Add(dr)
                Next
            End If
        Catch ex As Exception
            WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
    End Sub


    Private Sub LoadPropMapping(ByVal xn As XmlNode)
        Try
            dtItemColumnMapping = New DataTable
            Dim dr As DataRow
            If dtVaultOrg.Rows.Count > 0 And xn IsNot Nothing Then
                dtItemColumnMapping.Columns.Add("AX_Field")
                For Each row In dtVaultOrg.Rows
                    If Not dtItemColumnMapping.Columns.Contains(row(0).ToString().Trim()) Then
                        dtItemColumnMapping.Columns.Add(row(0).ToString().Trim())
                    End If
                Next
                For Each childNode As XmlNode In xn.ChildNodes
                    dr = dtItemColumnMapping.NewRow
                    For Each node As XmlNode In childNode.ChildNodes
                        dr(node.Name) = node.InnerText.Trim()
                    Next
                    dtItemColumnMapping.Rows.Add(dr)
                Next
                Dim statusProp As String = "Imported from AX"
                dr = dtItemColumnMapping.NewRow
                For i As Integer = 0 To dtItemColumnMapping.Columns.Count - 1
                    dr(i) = statusProp
                Next
                dtItemColumnMapping.Rows.Add(dr)
            End If
        Catch ex As Exception
            WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
    End Sub

#End Region
End Module

Public Class Rfc4180Writer
    Public Shared Sub WriteDataTable(ByVal sourceTable As DataTable, ByVal writer As IO.TextWriter, ByVal includeHeaders As Boolean)
        Try

            If (includeHeaders) Then
                Dim headerValues As IEnumerable(Of String) = sourceTable.Columns.OfType(Of DataColumn)().Select(Function(column) QuoteValue(column.ColumnName))

                writer.WriteLine(String.Join(",", headerValues))
            End If

            Dim items As IEnumerable(Of String) = Nothing
            For Each row As DataRow In sourceTable.Rows
                items = row.ItemArray.Select(Function(obj) QuoteValue(obj.ToString()))
                writer.WriteLine(String.Join(",", items))
            Next
            writer.Flush()
        Catch ex As Exception
            WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
    End Sub
    Public Shared Sub WriteLogItemExport(ByVal param As axFavelle.V2AX_ItemListDataContract, ByVal status As String)
        Try
            Dim csvFile As String = appDataPath + "Vault2AXItemExport.csv"
            Dim csvContent As Text.StringBuilder = New Text.StringBuilder()
            If Not System.IO.File.Exists(csvFile) Then
                ' Using writer As IO.StreamWriter = New IO.StreamWriter(csvFile)
                csvContent.AppendLine("ProductNo" + "," + "ProductName" + "," + "ChargesCode" + "," + "ItemGroup" + "," + "Description" + "," + "PcsControlled" + "," + "Remarks" + "," + "SearchName" + "," + "Specification" + ", UOM, CAT, Project Category, IntegrationDate,Status Description, ExportedDate")
                ' End Using
            End If

            csvContent.AppendLine(QuoteValue(param.ProductNo) + "," + QuoteValue(param.ProductName) + "," + QuoteValue(param.ChargesCode) + "," + QuoteValue(param.ItemGroup) + "," + QuoteValue(param.Description) + "," + param.PcsControlled.ToString() + "," + QuoteValue(param.Remarks) + "," + QuoteValue(param.SearchName) + "," + QuoteValue(param.Specification) + "," + param.BOMUnit + "," + param.CAT.ToString() + "," + param.ProjCategory + "," + param.IntegrationDate.ToString() + "," + status + "," + Now.ToString())
            IO.File.AppendAllText(csvFile, csvContent.ToString())
        Catch ex As Exception
            WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
    End Sub
    Public Shared Sub WriteLogBOMExport(ByVal param As axFavelleBOM.V2AX_BOMHeaderDataContract, ByVal status As String)
        Try
            Dim csvFile As String = appDataPath + "Vault2AXBOM_HeaderExport.csv"
            Dim csvContent As Text.StringBuilder = New Text.StringBuilder()
            If Not System.IO.File.Exists(csvFile) Then
                ' Using writer As IO.StreamWriter = New IO.StreamWriter(csvFile)
                csvContent.AppendLine("BOMNo" + "," + "BOMName" + "," + "CheckedBy" + "," + "ApprovedBy" + "," + "BomQty" + "," + "RevDescChanges" + "," + "Revision" + "," + "SN" + "," + "ItemNo" + ",BOMDate" + "," + "Status Description" + ",ExportDate")
                ' End Using
            End If
            csvContent.AppendLine(QuoteValue(param.BOMNo) + "," + QuoteValue(param.BOMName) + "," + QuoteValue(param.CheckedBy) + "," + QuoteValue(param.ApprovedBy) + "," + param.BomQty.ToString() + "," + QuoteValue(param.RevDescChanges) + "," + QuoteValue(param.RevNo) + "," + QuoteValue(param.SN) + "," + QuoteValue(param.ItemNo) + "," + QuoteValue(param.BOMDate.ToString()) + "," + status + "," + Now.ToString())
            IO.File.AppendAllText(csvFile, csvContent.ToString())

        Catch ex As Exception
            WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
        WriteLogBOMLineExport(param.BomLine, status)
    End Sub
    Public Shared Sub WriteLogBOMLineExport(param() As axFavelleBOM.V2AX_BOMLineDataContract, ByVal status As String)
        Try
            Dim csvFile As String = appDataPath + "Vault2AXBOM_LineExport.csv"
            Dim csvContent As Text.StringBuilder = New Text.StringBuilder()
            If Not System.IO.File.Exists(csvFile) Then
                ' Using writer As IO.StreamWriter = New IO.StreamWriter(csvFile)
                csvContent.AppendLine("BOMNo" + "," + "BomQty" + "," + "ItemNo" + "," + "Length" + "," + "PCS" + "," + "Position" + "," + "BOM CHG REV" + "," + "Width" + ",Weight, CAT, BOM Remarks," + "Status Description,Export Date")
                ' End Using
            End If
            For Each p In param
                csvContent.AppendLine(QuoteValue(p.BOMNo) + "," + p.BomQty.ToString() + "," + QuoteValue(p.ItemNo) + "," + p.Length.ToString() + "," + p.PCS.ToString() + "," + p.Position + "," + QuoteValue(p.RevNo) + "," + p.Width.ToString() + "," + p.Weight.ToString() + "," + p.CAT.ToString() + "," + QuoteValue(p.Remarks) + "," + status + "," + Now.ToString())
            Next
            IO.File.AppendAllText(csvFile, csvContent.ToString())
        Catch ex As Exception
            WriteLog(ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
    End Sub
    Private Shared Function QuoteValue(ByVal value As Object) As String
        If value Is Nothing Then
            Return String.Empty
        End If
        Return String.Concat("""", value.Replace("""", """"""), """")
    End Function

End Class