Imports System.Xml
Imports FavelleVaultAXws.axItemService
Imports FavelleVaultAXws.axBOMService
Imports System.ServiceModel
Imports Autodesk.Connectivity.WebServicesTools
Imports Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections
Imports System.Data

Module CommonFunctions
    'Dim settingsFile = "C:\ProgramData\Autodesk\Vault 2016\Extensions\FavelleVaultJobs\FavelleVaultJobs.dll.config"
    Public _dirSep As String = System.IO.Path.DirectorySeparatorChar.ToString()
    Public appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + _dirSep + "Vault2AX" + _dirSep
    Public applicationSettings = appDataPath + "Settings.xml"
    Public dtVaultOrg, dtItemColumnMapping, dtBOMColumnMapping, dtUserEmpIdMapping As DataTable
    'Dim wsObj As wsMethodsSoapClient
    Dim endPointUrl, endPointUrlBom As String
    Dim client As V2AX_ItemListServicesClient
    Dim clientBOM As V2AX_BOMListServicesClient
    Dim context As axItemService.CallContext
    Dim contextBOM As axBOMService.CallContext
    'Dim remoteAddress As EndpointAddress
    Public propToUpdate, vUserName, vPwd, vServer, bomPropToUpdate As String
    Dim adminCredentials As UserPasswordCredentials
    ' Public adminVConnection As Connection
    Dim adminServiceManager As WebServiceManager

    Public Function SetClient(ax As String) As Boolean
        Dim retValue As Boolean = False
        Try
            client = New V2AX_ItemListServicesClient("NetTcpBinding_V2AX_ItemListServices", endPointUrl)
            client.ClientCredentials.Windows.ClientCredential.Domain = "FFCM"
            client.ClientCredentials.Windows.ClientCredential.UserName = "vault1"
            client.ClientCredentials.Windows.ClientCredential.Password = "VA$123"
            client.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation
            context = New axItemService.CallContext()

            DirectCast(DirectCast(DirectCast(client, System.ServiceModel.ClientBase(Of V2AX_ItemListServices)).ChannelFactory, System.ServiceModel.ChannelFactory).Endpoint.Binding, System.ServiceModel.NetTcpBinding).MaxBufferSize = 999999999

            DirectCast(DirectCast(DirectCast(client, System.ServiceModel.ClientBase(Of V2AX_ItemListServices)).ChannelFactory, System.ServiceModel.ChannelFactory).Endpoint.Binding, System.ServiceModel.NetTcpBinding).MaxReceivedMessageSize = 999999999

            DirectCast(DirectCast(DirectCast(client, System.ServiceModel.ClientBase(Of V2AX_ItemListServices)).ChannelFactory, System.ServiceModel.ChannelFactory).Endpoint.Binding, System.ServiceModel.NetTcpBinding).ReceiveTimeout = New TimeSpan(0, 10, 0)

            DirectCast(DirectCast(DirectCast(client, System.ServiceModel.ClientBase(Of V2AX_ItemListServices)).ChannelFactory, System.ServiceModel.ChannelFactory).Endpoint.Binding, System.ServiceModel.NetTcpBinding).SendTimeout = New TimeSpan(0, 10, 0)

            DirectCast(DirectCast(DirectCast(client, System.ServiceModel.ClientBase(Of V2AX_ItemListServices)).ChannelFactory, System.ServiceModel.ChannelFactory).Endpoint.Binding, System.ServiceModel.NetTcpBinding).OpenTimeout = New TimeSpan(0, 10, 0)

            DirectCast(DirectCast(DirectCast(client, System.ServiceModel.ClientBase(Of V2AX_ItemListServices)).ChannelFactory, System.ServiceModel.ChannelFactory).Endpoint.Binding, System.ServiceModel.NetTcpBinding).CloseTimeout = New TimeSpan(0, 10, 0)
            context.Company = ax
            retValue = True
        Catch ex As Exception
            WriteLog(Now.ToString() + ": " + ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
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
            contextBOM = New axBOMService.CallContext()

            'DirectCast(DirectCast(DirectCast(clientBOM, System.ServiceModel.ClientBase(Of V2AX_BOMListServicesClient)).ChannelFactory, System.ServiceModel.ChannelFactory).Endpoint.Binding, System.ServiceModel.NetTcpBinding).MaxBufferSize = 999999999

            'DirectCast(DirectCast(DirectCast(clientBOM, System.ServiceModel.ClientBase(Of V2AX_BOMListServicesClient)).ChannelFactory, System.ServiceModel.ChannelFactory).Endpoint.Binding, System.ServiceModel.NetTcpBinding).MaxReceivedMessageSize = 999999999

            'DirectCast(DirectCast(DirectCast(clientBOM, System.ServiceModel.ClientBase(Of V2AX_BOMListServicesClient)).ChannelFactory, System.ServiceModel.ChannelFactory).Endpoint.Binding, System.ServiceModel.NetTcpBinding).ReceiveTimeout = New TimeSpan(0, 10, 0)

            'DirectCast(DirectCast(DirectCast(clientBOM, System.ServiceModel.ClientBase(Of V2AX_BOMListServicesClient)).ChannelFactory, System.ServiceModel.ChannelFactory).Endpoint.Binding, System.ServiceModel.NetTcpBinding).SendTimeout = New TimeSpan(0, 10, 0)

            'DirectCast(DirectCast(DirectCast(clientBOM, System.ServiceModel.ClientBase(Of V2AX_BOMListServicesClient)).ChannelFactory, System.ServiceModel.ChannelFactory).Endpoint.Binding, System.ServiceModel.NetTcpBinding).OpenTimeout = New TimeSpan(0, 10, 0)

            'DirectCast(DirectCast(DirectCast(clientBOM, System.ServiceModel.ClientBase(Of V2AX_BOMListServicesClient)).ChannelFactory, System.ServiceModel.ChannelFactory).Endpoint.Binding, System.ServiceModel.NetTcpBinding).CloseTimeout = New TimeSpan(0, 10, 0)
            contextBOM.Company = ax
            retValue = True
        Catch ex As Exception
            WriteLog(Now.ToString() + ": " + ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
        Return retValue
    End Function

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
            WriteLog(Now.ToString() + ": " + ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
    End Sub

    

    Public Function ExportItemToAX(ByVal itemCode As String, ByVal chargeCode As String, ByVal desc As String, ByVal pName As String, ByVal remark As String, ByVal srchName As String, ByVal spec As String, ByVal pcControl As Boolean, ByVal ax As String) As Boolean
        Dim retValue As Boolean = False
        Try
            If SetClient(ax) Then
                Dim param As axItemService.V2AX_ItemListDataContract = New axItemService.V2AX_ItemListDataContract
                param.ChargesCode = chargeCode
                param.Description = desc
                param.PcsControlled = pcControl
                param.ProductName = pName
                param.ProductNo = itemCode
                param.Remarks = remark
                param.Specification = spec
                param.SearchName = srchName
                If client.itemExists(context, itemCode) Then
                    retValue = client.modifyItem(context, param)
                    WriteLog("Modify Item " & param.ProductNo & " " & param.ChargesCode + " " & param.PcsControlled.ToString() & " " & param.ProductName & " " + param.Remarks & " " + param.Specification & " " & param.SearchName & " " & retValue.ToString())
                Else
                    retValue = client.CreateItem(context, param)
                    WriteLog("Create Item " & param.ProductNo & " " & param.ChargesCode + " " & param.PcsControlled.ToString() & " " & param.ProductName & " " + param.Remarks & " " + param.Specification & " " & param.SearchName & " " & retValue.ToString())
                End If
                retValue = True
            End If
        Catch ex As Exception
            WriteLog(Now.ToString() + ": " + ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
        Return retValue
    End Function

    Public Function ExportBOMToAX(ByVal bomNo As String, ByVal bomName As String, ByVal itemNo As String, ByVal appBy As String, ByVal chkBy As String, ByVal revNo As String, ByVal sn As String, ByVal revDesc As String, ByVal qty As Decimal, ByVal ax As String, ByVal ChildIds() As V2AX_BOMLineDataContract) As Boolean
        Dim retValue As Boolean = False
        Try
            If SetClientBOM(ax) Then
                Dim param As V2AX_BOMHeaderDataContract = New V2AX_BOMHeaderDataContract()
                param.BOMNo = bomNo
                param.BOMDate = Now.Date
                param.BOMName = bomName
                param.ItemNo = itemNo
                Dim approvedBy = (From row As DataRow In dtUserEmpIdMapping.Rows
                                   Where row(0).ToString.Equals(appBy, StringComparison.InvariantCultureIgnoreCase)
                                   Select row(1)).FirstOrDefault()
                If approvedBy IsNot Nothing Then
                    param.ApprovedBy = approvedBy.ToString()
                Else
                    param.ApprovedBy = String.Empty
                End If
                Dim checkedBy = String.Empty
                If chkBy IsNot Nothing Then
                    checkedBy = (From row As DataRow In dtUserEmpIdMapping.Rows
                    Where row(0).ToString.Equals(chkBy, StringComparison.InvariantCultureIgnoreCase)
                                       Select row(1)).FirstOrDefault().ToString()
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
                retValue = clientBOM.CreateBOMHeader(contextBOM, param)
            End If
        Catch ex As Exception
            WriteLog(Now.ToString() + ": " + ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
        Return retValue
    End Function

    Public Function GetImportItems(ByVal ax As String) As DataTable
        Dim retValue As DataTable = Nothing
        Try
            If SetClient(ax) Then
                Dim itemContract() As axItemService.V2AX_ItemListDataContract = client.getItemList(context)
                If itemContract.Length() > 0 Then
                    retValue = New DataTable()
                    Dim fields() = itemContract.First.GetType.GetProperties
                    For Each field In fields
                        retValue.Columns.Add(field.Name, field.PropertyType)
                    Next
                    For Each item In itemContract
                        Dim row As DataRow = retValue.NewRow()
                        For Each field In fields
                            Dim p = item.GetType.GetProperty(field.Name)
                            row(field.Name) = p.GetValue(item, Nothing)
                        Next
                        retValue.Rows.Add(row)
                    Next
                End If
            End If

        Catch ex As Exception
            WriteLog(Now.ToString() + ": " + ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
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
            WriteLog(Now.ToString() + ": " + ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
        Return retValue
    End Function

#Region "Application Configurations"

    Public Function LoadDataFromSettingsFile() As Boolean
        Dim retValue As Boolean = False
        Try
            If System.IO.File.Exists(applicationSettings) Then
                Dim settingsXml As New XmlDocument
                Dim rootNode, xnBasicSettings, xnPropMapping, xnVaultJobs, xnBOMPropMapping, xnUserMapping As XmlNode
                settingsXml.Load(applicationSettings)
                rootNode = settingsXml.SelectSingleNode("Settings")
                xnBasicSettings = rootNode.SelectSingleNode("BasicSettings")
                xnPropMapping = rootNode.SelectSingleNode("ItemPropertyMapping")
                xnVaultJobs = rootNode.SelectSingleNode("VaultJobs")
                xnBOMPropMapping = rootNode.SelectSingleNode("BomPropertyMapping")
                xnUserMapping = rootNode.SelectSingleNode("VaultUsers")
                LoadBasicSettings(xnBasicSettings)
                LoadPropMapping(xnPropMapping)
                LoadVaultJobs(xnVaultJobs)
                LoadBOMPropMapping(xnBOMPropMapping)
                LoadUserMapping(xnUserMapping)
                retValue = True
            Else
                WriteLog(Now.ToString() & ": " & "Settings file does not exists." & vbNewLine)
            End If
        Catch ex As Exception
            WriteLog(Now.ToString() & ": " & ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
        Return retValue
    End Function

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
            WriteLog(Now.ToString() + ": " + ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
    End Sub

    Private Sub LoadVaultJobs(ByVal xn As XmlNode)
        Try
            If xn IsNot Nothing Then
                propToUpdate = xn.SelectSingleNode("ItemState").InnerText
                bomPropToUpdate = xn.SelectSingleNode("BOMProp").InnerText
            End If
        Catch ex As Exception
            WriteLog(Now.ToString() + ": " + ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
    End Sub

    Public Sub LoadUserMapping(ByVal xn As XmlNode)
        Try
            dtUserEmpIdMapping = New DataTable()
            dtUserEmpIdMapping.Columns.Clear()
            dtUserEmpIdMapping.Columns.Add("Vault_Users")
            dtUserEmpIdMapping.Columns.Add("Employee_Code")
            'For Each childNode As XmlNode In xn.ChildNodes(0)
            '    For Each node As XmlNode In childNode.ChildNodes
            '        dtUserEmpIdMapping.Columns.Add(node.Name)
            '    Next
            'Next
            For Each childNode As XmlNode In xn.ChildNodes
                Dim dr As DataRow = dtUserEmpIdMapping.NewRow
                For Each node As XmlNode In childNode.ChildNodes
                    dr(node.Name) = node.InnerText.Trim()
                Next
                dtUserEmpIdMapping.Rows.Add(dr)
            Next
        Catch ex As Exception
            WriteLog(Now.ToString() + ": " + ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
    End Sub

    Private Sub LoadBOMPropMapping(ByVal xn As XmlNode)
        Try
            dtBOMColumnMapping = New DataTable
            If dtVaultOrg.Rows.Count > 0 And xn IsNot Nothing Then
                dtBOMColumnMapping.Columns.Add("AX_Field")
                For Each row In dtVaultOrg.Rows
                    dtBOMColumnMapping.Columns.Add(row(0).ToString().Trim())
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
            WriteLog(Now.ToString() + ": " + ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
    End Sub


    Private Sub LoadPropMapping(ByVal xn As XmlNode)
        Try
            dtItemColumnMapping = New DataTable
            If dtVaultOrg.Rows.Count > 0 And xn IsNot Nothing Then
                dtItemColumnMapping.Columns.Add("AX_Field")
                For Each row In dtVaultOrg.Rows
                    dtItemColumnMapping.Columns.Add(row(0).ToString().Trim())
                Next
                For Each childNode As XmlNode In xn.ChildNodes
                    Dim dr As DataRow = dtItemColumnMapping.NewRow
                    For Each node As XmlNode In childNode.ChildNodes
                        dr(node.Name) = node.InnerText.Trim()
                    Next
                    dtItemColumnMapping.Rows.Add(dr)
                Next
            End If
        Catch ex As Exception
            WriteLog(Now.ToString() + ": " + ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
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
            WriteLog(Now.ToString() + ": " + ex.Message & vbNewLine & ex.StackTrace.ToString() & vbNewLine)
        End Try
    End Sub

    Private Shared Function QuoteValue(ByVal value As String) As String
        Return String.Concat("""", value.Replace("""", """"""), """")
    End Function
End Class