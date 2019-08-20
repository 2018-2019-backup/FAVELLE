Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class wsMethods
    Inherits System.Web.Services.WebService
    Dim qryStr As String
    <WebMethod()> _
    Public Function GetItemsforVaults() As DataTable
        Dim dt As DataTable = Nothing
        Try
            qryStr = "SELECT * FROM [V2AXItemExport] WHERE [V2AX_IntegrationStatus] = 'OPEN' OR [V2AX_IntegrationStatus] = 'ERROR'"
            DbConnect.SelectDT(qryStr, dt)
        Catch ex As Exception

        End Try
        Return dt
    End Function

    'UpdateV2AXItemExportStatus
    <WebMethod()> _
    Public Function UpdateV2AXItemExportStatus(ByVal dt As DataTable) As Boolean
        Dim retValue As Boolean = False
        Try
            retValue = UpdateItemExportStatus(dt)
        Catch ex As Exception

        End Try
        Return retValue
    End Function

    <WebMethod()> _
    Public Function GetItemsforVault(ByVal vaultName As String) As DataTable
        Dim dt As DataTable = Nothing
        Try
            qryStr = String.Format("SELECT * FROM [V2AXItemExport] where [dataAreaId] = '{0}' AND ([V2AX_IntegrationStatus] = 'OPEN' OR [V2AX_IntegrationStatus] = 'ERROR')", vaultName)
            DbConnect.SelectDT(qryStr, dt)
        Catch ex As Exception

        End Try
        Return dt
    End Function
    <WebMethod()> _
    Public Function InsertBulk(ByVal dt As DataTable, ByVal tblName As String) As Boolean
        Dim retVal As Boolean = False
        Try
            retVal = DbConnect.BulkInsert(dt, tblName)
        Catch ex As Exception

        End Try
        Return retVal
    End Function
    <WebMethod()> _
    Public Function InsertIntoAX(ByVal str As String) As Boolean
        Return NonQuery(str)
    End Function

    <WebMethod()> _
    Public Function GetPropertyColumnMapping() As DataTable
        Dim retVal As DataTable = New DataTable("DBPropMapping")
        Dim cols As List(Of String) = ConfigurationManager.AppSettings("dbColumns").Split(",").Select(Function(x) x.Trim()).ToList()
        Dim props As List(Of String) = ConfigurationManager.AppSettings("vaultProps").Split(",").Select(Function(x) x.Trim()).ToList()
        retVal.Columns.Add("DBColumn")
        retVal.Columns.Add("VaultProp")
        For i As Integer = 0 To cols.Count - 1
            retVal.Rows.Add({cols(i), props(i)})
        Next
        'retVal.Rows.Add({"V2AX_CAT", "CAT"})
        'retVal.Rows.Add({"V2AX_ChargesCode", "Charge Code"})
        'retVal.Rows.Add({"V2AX_CompID", "AX Company"})
        'retVal.Rows.Add({"V2AX_Description", "Item Code Description"})
        'retVal.Rows.Add({"V2AX_ItemGroup", "Item Group"})
        'retVal.Rows.Add({"V2AX_ItemId", "Item Code"})
        'retVal.Rows.Add({"V2AX_PcsControlled", "Pcs Controlled"})
        'retVal.Rows.Add({"V2AX_ProductName", "Production Pool"})
        'retVal.Rows.Add({"V2AX_Remarks", "Remark"})
        'retVal.Rows.Add({"V2AX_SearchName", "Search Name"})
        'retVal.Rows.Add({"V2AX_Specification", "Specification"})
        Return retVal
    End Function

    <WebMethod()> _
    Public Function GetVaultProps() As DataTable
        Dim retVal As DataTable = New DataTable("VaultProps")
        Dim cols As List(Of String) = ConfigurationManager.AppSettings("lstVaults").Split(",").Select(Function(x) x.Trim()).ToList()
        retVal.Columns.Add("VaultName")
        retVal.Columns.Add("VaultProp")
        For Each vlt In cols
            Dim props As List(Of String) = ConfigurationManager.AppSettings(vlt).Split(",").Select(Function(x) x.Trim()).ToList()
            For i As Integer = 0 To props.Count - 1
                retVal.Rows.Add({vlt, props(i)})
            Next
        Next
        'retVal.Rows.Add({"V2AX_CAT", "CAT"})
        'retVal.Rows.Add({"V2AX_ChargesCode", "Charge Code"})
        'retVal.Rows.Add({"V2AX_CompID", "AX Company"})
        'retVal.Rows.Add({"V2AX_Description", "Item Code Description"})
        'retVal.Rows.Add({"V2AX_ItemGroup", "Item Group"})
        'retVal.Rows.Add({"V2AX_ItemId", "Item Code"})
        'retVal.Rows.Add({"V2AX_PcsControlled", "Pcs Controlled"})
        'retVal.Rows.Add({"V2AX_ProductName", "Production Pool"})
        'retVal.Rows.Add({"V2AX_Remarks", "Remark"})
        'retVal.Rows.Add({"V2AX_SearchName", "Search Name"})
        'retVal.Rows.Add({"V2AX_Specification", "Specification"})
        Return retVal
    End Function

    <WebMethod()> _
    Public Function GetVaultDetails() As VaultCredentials
        Return New VaultCredentials()
    End Function

    <WebMethod()> _
    Public Function IfItemExistsItemImport(ByVal qry As String) As Integer
        Return RetrieveSingleData(qry)
    End Function

    

End Class