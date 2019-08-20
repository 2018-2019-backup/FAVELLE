Imports System.Collections.Generic
Imports System.IO
Module ReadINI
    Public g_IniFile As IniFile.IniFile
    Public g_IniKeys As Hashtable

    Public Sub ReadINIFile(ByVal filename As String)
        g_IniKeys = New Hashtable
        g_IniFile = New IniFile.IniFile(filename, False)
        Dim Sections As ArrayList = g_IniFile.GetSections()
        Dim myEnumerator As System.Collections.IEnumerator = Sections.GetEnumerator()
        While myEnumerator.MoveNext()
            Dim Keys As ArrayList = g_IniFile.GetKeys(myEnumerator.Current.Name)
            Dim mySubEnumerator As System.Collections.IEnumerator = Keys.GetEnumerator()
            While mySubEnumerator.MoveNext()
                g_IniKeys.Add(Trim(mySubEnumerator.Current.Name), Trim(mySubEnumerator.Current.value))
            End While
        End While
    End Sub

End Module
