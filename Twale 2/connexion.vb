Public Class connexion

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Si le pseudonyme est rempli, on continue
        If Pseudonyme.Text.Trim() <> "" And IpBroadcast.Text.Trim() <> "" Then

            ' Si la personne souhaite que le programme retienne son pseudonyme...
            If CheckBox1.Checked = True Then
                Dim writer As System.IO.StreamWriter = New System.IO.StreamWriter(My.Application.GetEnvironmentVariable("appdata") & "\pseudo.twale2", False)
                writer.Write(Pseudonyme.Text)
                writer.Flush()
                writer.Close()

                ' Sinon, on tente de supprimer le fichier
            ElseIf System.IO.File.Exists(My.Application.GetEnvironmentVariable("appdata") & "\pseudo.twale2") = True Then
                System.IO.File.Delete(My.Application.GetEnvironmentVariable("appdata") & "\pseudo.twale2")
            End If

            ' On enregistre l'IP de broadcast
            Dim writer2 As System.IO.StreamWriter = New System.IO.StreamWriter(My.Application.GetEnvironmentVariable("appdata") & "\ipbroadcast.twale2", False)
            writer2.Write(IpBroadcast.Text)
            writer2.Flush()
            writer2.Close()

            ' On cache la fenêtre de connexion
            Me.Hide()

            ' Et on affiche la fenêtre principale
            fenetre_principale.Show()

            ' Sinon, on affiche une fenêtre d'erreur
        Else
            If Pseudonyme.Text.Trim() = "" Then
                MsgBox("Vous n'avez pas choisi de pseudonyme!", MsgBoxStyle.Critical, "Pas si vite!")
            End If

            If IpBroadcast.Text.Trim() = "" Then
                MsgBox("Vous n'avez pas choisi d'adresse IP de broadcast!", MsgBoxStyle.Critical, "Pas si vite!")
            End If
        End If
    End Sub

    Private Sub connexion_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' On commence par vérifier si la personne a déjà enregistré son pseudonyme/l'IP de broadcast
        Try
            ' On tente de charger le pseudonyme
            Dim reader As System.IO.StreamReader
            reader = New System.IO.StreamReader(My.Application.GetEnvironmentVariable("appdata") & "\pseudo.twale2")
            Pseudonyme.Text = reader.ReadLine()
            reader.Close()

            ' Si ça a marché, alors on re-coche la checkbox
            CheckBox1.Checked = True

            ' On tente de charger l'IP de broadcast
            Dim reader2 As System.IO.StreamReader
            reader2 = New System.IO.StreamReader(My.Application.GetEnvironmentVariable("appdata") & "\ipbroadcast.twale2")
            IpBroadcast.Text = reader2.ReadLine()
            reader2.Close()
        Catch ex As Exception

        End Try
    End Sub

End Class