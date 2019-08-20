Public Class VaultCredentials
    Public vServer As String = ConfigurationManager.AppSettings("vServer")
    Public vUser As String = ConfigurationManager.AppSettings("vUser")
    Public vPwd As String = ConfigurationManager.AppSettings("vPwd")
    Public lstVaults As List(Of String) = New List(Of String)
    Public Sub New()
        Dim vaults = ConfigurationManager.AppSettings("lstVaults").Split(",")
        lstVaults = (From v In vaults
                   Select v.Trim()).ToList()
    End Sub

End Class
