Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace FavelleVaultTools


    Public Class VaultPath
        Private Shared SEPERATOR As String = "/"

        ''' <summary>
        ''' Gets the path of the parent folder for the inputed path.
        ''' </summary>
        ''' <param name="path">A full vault path.</param>
        ''' <returns>The parent folder path.</returns>
        Public Shared Function GetParentPath(ByVal path As String) As String
            Dim retVal As String = path.Trim()

            If retVal.EndsWith(SEPERATOR) Then
                retVal = retVal.Remove(retVal.Length - 1)
            End If

            Dim index As Integer = retVal.LastIndexOf(SEPERATOR)
            If index < 0 Then
                Return path
            Else
                Return retVal.Substring(0, index)
            End If
        End Function

        ''' <summary>
        ''' Combine 2 Vault path elements into a single path.
        ''' </summary>
        ''' <param name="path1">First part of the path.</param>
        ''' <param name="path2">Second part of the path.</param>
        ''' <returns>The combined path.</returns>
        Public Shared Function Combine(ByVal path1 As String, ByVal path2 As String) As String
            path1 = path1.Trim()
            path2 = path2.Trim()

            If path1.EndsWith(SEPERATOR) Then
                path1 = path1.Remove(path1.Length - 1)
            End If
            If path2.StartsWith(SEPERATOR) Then
                path2 = path2.Substring(1)
            End If

            Return path1 + SEPERATOR + path2
        End Function

        ''' <summary>
        ''' Determines the depth of the path.
        ''' </summary>
        ''' <param name="path">A full Vault path.</param>
        ''' <returns>The number of levels deep that the folder is.  
        ''' "$" will have a depth of 0.</returns>
        Public Shared Function Depth(ByVal path As String) As Integer
            path = path.Trim()
            If Not path.StartsWith("$") Then
                Throw New Exception("Intput is not a full path")
            End If

            Dim _depth As Integer = path.ToCharArray().Count(Function(n) n = "/"c)

            If path.EndsWith("/") Then
                _depth -= 1
            End If

            Return _depth
        End Function
    End Class
End Namespace