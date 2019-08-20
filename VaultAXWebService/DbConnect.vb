Imports System.Data.SqlClient

Module DbConnect
    Public conStr As String = ConfigurationManager.ConnectionStrings("dbCredential").ConnectionString
    Public dbCon As New SqlConnection(conStr)
    Dim cmd As SqlCommand
    Dim da As New SqlDataAdapter


    Public Function OpenDb() As Boolean
        Dim retValue As Boolean = False
        Try
            If dbCon.State = ConnectionState.Closed Then
                dbCon.Open()
            End If
            retValue = True
        Catch EX As Exception
        End Try
        Return retValue
    End Function

    Public Function RetrieveSingleData(ByVal sql As String)
        Dim val = 0
        If OpenDb() Then
            cmd = New SqlCommand(sql, dbCon)
            Try
                val = cmd.ExecuteScalar
            Catch ex As Exception
                val = 0
            End Try
        End If
        Return val
    End Function

    Public Function NonQuery(ByVal sql As String) As Boolean
        Dim retValue As Boolean = False
        Try
            If OpenDb() Then
                cmd = New SqlCommand(sql, dbCon)
                cmd.ExecuteNonQuery()
                retValue = True
            End If
        Catch ex As Exception

        End Try
        Return retValue
    End Function

    Public Function UpdateItemExportStatus(ByVal dt As DataTable) As Boolean
        Dim retValue As Boolean = False
        Try
            Dim qry As String
            If OpenDb() Then
                For Each dr As DataRow In dt.Select("V2AX_IntegrationStatus <> 'OPEN'")
                    qry = String.Format("UPDATE [V2AXItemExport] SET [V2AX_IntegrationStatus] = '{0}' WHERE [V2AX_ItemId] = '{1}'", dr.Item("V2AX_IntegrationStatus").ToString(), dr.Item("V2AX_ItemId").ToString())
                    cmd = New SqlCommand(qry, dbCon)
                    cmd.ExecuteNonQuery()
                Next
                retValue = True
            End If
        Catch ex As Exception

        End Try
        Return retValue
    End Function

    Public Sub SelectDT(ByVal sql As String, ByRef dt As DataTable)
        If OpenDb() Then
            dt = New DataTable("NewTable")
            da = New SqlDataAdapter(sql, dbCon)
            da.Fill(dt)
        End If
    End Sub

    Public Function BulkInsert(ByVal dt As DataTable, ByVal tableName As String) As Boolean
        Dim retValue As Boolean = False
        Try
            If OpenDb() Then
                Using copy As New SqlBulkCopy(dbCon)
                    For Each col As DataColumn In dt.Columns
                        copy.ColumnMappings.Add(col.ColumnName, col.ColumnName)
                    Next
                    copy.DestinationTableName = tableName
                    copy.WriteToServer(dt)
                    retValue = True
                End Using
            End If
        Catch ex As Exception

        End Try
        Return retValue
    End Function


End Module
