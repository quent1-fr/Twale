Imports System.Threading
Imports System.Net
Imports System.Net.Sockets
Imports System.IO
Imports System.Text

Public Class fenetre_principale
    ' DLL permettant de faire clignoter la fenêtre dans la barre des tâches
    Private Declare Function FlashWindow Lib "user32" (ByVal hwnd As Long, ByVal bInvert As Long) As Long

    ' Définition des variables
    Public pseudonyme_client As String
    Public port_communication As Int32 = 2081
    Public port_udp_hello As Int32 = 2083
    Public thread_ecoute As Thread
    Public thread_ecoute_hello As Thread
    Public thread_decouverte_paires As Thread
    Public thread_envoi_messages As Thread
    Public MessageATransmettre As String
    Public ip_ecoute = String.Empty
    Public ip_broadcast = connexion.IpBroadcast.Text ' Adresse IP de broadcast
    Dim unicode_converter As New System.Text.UnicodeEncoding
    Public temps_avant_hello As Integer = 3 ' En secondes
    Public temps_avant_nouvel_hello As Integer = 3 ' En minutes
    Public mauvais_paquets_avant_kick As Integer = 3
    Public cle_chiffrement As String = "sPpE8ïöfÒkrwxñ{<$áýOrÂ:âÓoÁyu¸Sú­¿Íe;ÝvØwúÝ_¶Z´¹OÍIu8ËuúA¿ÖP_CD§`y7Áy±+.thÈ6­°ç\a`ozgw+HÞòz?áÍ¬CÃâFbGfÿdk~^çi`â¯pâÞ0`Å¸ÓEfrÜJê`eY.Çìjv|S×?Ò²bÔzssx½'©ñ" ' Clé de chiffrement pour les messages - http://www.pc-optimise.com/reseaux/wifikeygen.php - La changer serait une bonne idée...

    ' True -> activation de l'affichage des erreurs (si la checkbox2 est checkée)
    Public mode_debug As Boolean = connexion.CheckBox2.Checked

    ' "Base de donnée" des clients connectés
    Public nombre_maximal_clients = 500
    Public liste_paires_ip(nombre_maximal_clients) As String
    Public liste_paires_pseudo(nombre_maximal_clients) As String
    Public liste_perte_paquets(nombre_maximal_clients) As Integer ' Permet de kicker une personne si trois paquets consécutifs sont perdus

    ' Fonctions de délégation
    Public maj_liste_pseudo As New delegate_maj_liste_connectes(AddressOf maj_liste_connectes)
    Public maj_rtb_tchat As New delegate_maj_contenu_tchat(AddressOf maj_contenu_tchat)
    Public maj_statut_tchat As New delegate_afficher_statut_tchat(AddressOf afficher_statut_tchat)
    Public varreactiver_bouton_envoyer As New delegate_reactiver_bouton_envoyer(AddressOf reactiver_bouton_envoyer)

    ' Sons de notification par défaut
    Public son_nouveau_message As System.Media.SoundPlayer = New System.Media.SoundPlayer(My.Resources.nouveau_message)

    ' Définition des fonctions

    ' Chiffrement AES (code largement inspiré de https://stackoverflow.com/questions/5987186/aes-encrypt-string-in-vb-net)
    Public Function chiffrement_AES(ByVal texte As String, ByVal cle As String) As String
        Dim AES As New System.Security.Cryptography.RijndaelManaged
        Dim Hash_AES As New System.Security.Cryptography.MD5CryptoServiceProvider
        Dim chiffre As String = ""

        Try
            Dim hash(31) As Byte
            Dim temp As Byte() = Hash_AES.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(cle))

            Array.Copy(temp, 0, hash, 0, 16)
            Array.Copy(temp, 0, hash, 15, 16)

            AES.Key = hash
            AES.Mode = System.Security.Cryptography.CipherMode.ECB

            Dim DESEncrypter As System.Security.Cryptography.ICryptoTransform = AES.CreateEncryptor
            Dim Buffer As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes(texte)

            chiffre = Convert.ToBase64String(DESEncrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))

            Return chiffre
        Catch ex As Exception
            Return False
        End Try
    End Function

    ' Déchiffrement AES (code largement inspiré de https://stackoverflow.com/questions/5987186/aes-encrypt-string-in-vb-net)
    Public Function dechiffrement_AES(ByVal texte As String, ByVal cle As String) As String
        Dim AES As New System.Security.Cryptography.RijndaelManaged
        Dim Hash_AES As New System.Security.Cryptography.MD5CryptoServiceProvider
        Dim dechiffre As String = ""

        Try
            Dim hash(31) As Byte
            Dim temp As Byte() = Hash_AES.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(cle))

            Array.Copy(temp, 0, hash, 0, 16)
            Array.Copy(temp, 0, hash, 15, 16)

            AES.Key = hash
            AES.Mode = System.Security.Cryptography.CipherMode.ECB

            Dim DESDecrypter As System.Security.Cryptography.ICryptoTransform = AES.CreateDecryptor
            Dim Buffer As Byte() = Convert.FromBase64String(texte)

            dechiffre = System.Text.ASCIIEncoding.ASCII.GetString(DESDecrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))

            Return dechiffre
        Catch ex As Exception
            Return False
        End Try
    End Function

    ' Fonction permettant de quitter uniquement si l'utilisateur le souhaite (en cas d'erreur)
    Function quitter_si_pb()
        If MsgBox("Une erreur fatale a été rencontrée. Souhaitez-vous fermer Twale?", "Oups...", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
            ' À la fermeture, on envoie un message "bye" généralisé
            envoyer_paquet_UDP(ip_broadcast, port_udp_hello, "bye")

            ' On supprime l'icône dans le tray
            IconeNotification.Visible = False

            ' Et on tue la fenêtre de connexion (qui, sinon, empêcherait la fermeture complète du programme)
            connexion.Close()

            Application.Exit()
        End If

        Return True
    End Function

    ' Fonction permettant de parcourir les tableaux liste_paires_ip et liste_paires_pseudo et de trouver le premier id dispo
    Function premier_id_dispo()
        For i As Integer = 0 To nombre_maximal_clients Step 1
            If liste_paires_ip(i) = "" And liste_paires_pseudo(i) = "" Then
                Return i
            End If
        Next

        Return False
    End Function

    ' Fonction permettant d'obtenir la clé liée à un utilisateur dans les tableaux liste_paires_ip et liste_paires_pseudo
    Function recuperer_id(ByVal valeur As String, ByVal est_pseudo As Boolean)
        If est_pseudo = True Then
            For i As Integer = 0 To premier_id_dispo() Step 1
                If liste_paires_pseudo(i) = valeur Then
                    Return i
                End If
            Next

        Else
            For i As Integer = 0 To premier_id_dispo() Step 1
                If liste_paires_ip(i) = valeur Then
                    Return i
                End If
            Next
        End If

        Return False
    End Function

    ' Fonction permettant de traiter les paquets reçus
    Function traiter_requete(ByVal requete As String, ByVal ip_source As String)

        ' Debug: on affiche le message
        If mode_debug = True Then
            MsgBox("Message de " & ip_source & vbNewLine & requete, MsgBoxStyle.Information, "Mode debug")
        End If

        ' Déhiffrement
        requete = dechiffrement_AES(requete, cle_chiffrement)

        ' On isole la commande du contenu
        Dim commande As String = ""
        Dim contenu As String = ""
        For Each partie In requete.Split(" ")

            If commande = "" Then
                commande = partie
            Else
                contenu &= partie & " "
            End If

        Next

        ' Et on traite suivant les cas
        Select Case commande

            Case "hello" ' Connexion d'un nouveau client
                ' Tout d'abord, on vérifie que l'utilisateur n'est pas déjà connecté et que nous n'avons pas reçu notre propre hello
                If recuperer_id(ip_source, False).GetType() = GetType(Int32) Or ip_source = ip_ecoute Then
                    Exit Select
                End If

                ' On prend le premier id disponible
                Dim id As Integer = premier_id_dispo()

                ' Le contenu est le pseudonyme de la personne. On l'ajoute à la liste
                liste_paires_ip(id) = ip_source

                ' Tout comme son pseudo
                liste_paires_pseudo(id) = contenu

                ' Et le nombre de paquets perdus
                liste_perte_paquets(id) = 0

                ' Invocation de la fonction de mise à jour de la liste des pseudonymes
                Me.Invoke(maj_liste_pseudo, New Object() {})

                ' On l'affiche dans le t'chat
                Me.Invoke(maj_statut_tchat, New Object() {contenu & "vient de se connecter", False})

                ' Et on envoie un helloback
                envoyer_paquet_TCP(ip_source, port_communication, "helloback " & pseudonyme_client)

                Exit Select


            Case "helloback" ' Réponse à notre hello
                ' Tout d'abord, on vérifie que l'utilisateur n'est pas déjà connecté
                If recuperer_id(ip_source, False).GetType() = GetType(Int32) Then
                    Exit Select
                End If

                ' On prend le premier id disponible
                Dim id As Integer = premier_id_dispo()

                ' Le contenu est le pseudonyme de la personne. On l'ajoute à la liste
                liste_paires_ip(id) = ip_source

                ' Tout comme son pseudo
                liste_paires_pseudo(id) = contenu

                ' Et le nombre de paquets perdus
                liste_perte_paquets(id) = 0

                ' Invocation de la fonction de mise à jour de la liste des pseudonymes
                Me.Invoke(maj_liste_pseudo, New Object() {})

                ' On l'affiche dans le t'chat
                Me.Invoke(maj_statut_tchat, New Object() {contenu & "vient de se connecter", False})

                Exit Select

            Case "listdl" ' Permet d'envoyer (ou de ré-envoyer) la liste des téléchargements disponibles
                ' Tout d'abord, on vérifie que l'utlisateur est connecté + que ce n'est pas nons
                If recuperer_id(ip_source, False).GetType() <> GetType(Int32) And ip_source <> ip_ecoute Then
                    Exit Select
                End If

            Case "msg" ' Nouveau message sur le t'chat
                ' Tout d'abord, on vérifie que l'utlisateur est connecté + que ce n'est pas nous
                If recuperer_id(ip_source, False).GetType() <> GetType(Int32) And ip_source <> ip_ecoute Then
                    Exit Select
                End If

                ' Invocation de la fonction de mise à jour du t'chat (si le pseudo est connu, ou si c'est nous)
                If recuperer_id(ip_source, False).GetType() = GetType(Int32) Then
                    liste_perte_paquets(recuperer_id(ip_source, False)) = 0 ' Reset nombre de paquets perdus
                    Me.Invoke(maj_rtb_tchat, New Object() {liste_paires_pseudo(recuperer_id(ip_source, False)), contenu})
                ElseIf ip_source = ip_ecoute Then
                    Me.Invoke(maj_rtb_tchat, New Object() {pseudonyme_client, contenu})
                End If

                Exit Select

            Case "bye" ' Déconnexion d'un client
                ' On récupère l'ID associé à l'IP du client souhaitant se déconnecter
                Dim id_client = recuperer_id(ip_source, False)

                ' Si il existe, on le supprime
                If id_client.GetType() = GetType(Int32) Then
                    ' On l'affiche dans le t'chat
                    Me.Invoke(maj_statut_tchat, New Object() {liste_paires_pseudo(id_client) & "vient de se déconnecter", False})

                    ' Invocation de la fonction de mise à jour de la liste des pseudonymes
                    Me.Invoke(maj_liste_pseudo, New Object() {})

                    ' Et on vide les cases des trois tableaux
                    liste_paires_ip(id_client) = ""
                    liste_paires_pseudo(id_client) = ""
                    liste_perte_paquets(id_client) = 0
                End If

                ' Pour finir, on rafraichit la liste des personnes connectées
                Me.Invoke(maj_liste_pseudo, New Object() {})

                Exit Select

        End Select

        Return True
    End Function

    ' Fonction de mise à jour de la liste des personnes connectées
    Delegate Sub delegate_maj_liste_connectes()
    Function maj_liste_connectes()
        ' On vide la listbox
        ListeConnectes.Items.Clear()

        ' On ajoute les pseudonymes connectés un par un
        For Each pseudonyme In liste_paires_pseudo
            If pseudonyme <> "" Then
                ListeConnectes.Items.Add(pseudonyme)
            End If
        Next

        'Puis on ajoute le nôtre
        ListeConnectes.Items.Add(pseudonyme_client & " (c'est vous)")
        Return True
    End Function

    ' Fonction affichant un statut spécifique dans le t'chat (en vert ou en rouge)
    Delegate Sub delegate_afficher_statut_tchat(ByVal contenu As String, ByVal erreur As Boolean)
    Function afficher_statut_tchat(ByVal contenu As String, Optional ByVal erreur As Boolean = False)
        ' On note la position du curseur
        Dim position_curseur = ContenuTchat.TextLength

        ' On imprime le message
        ContenuTchat.AppendText("*** " & contenu & " ***" & vbNewLine)

        ' On note la nouvelle position du curseur
        Dim nouvelle_position_curseur = ContenuTchat.TextLength

        ' On met le message en vert ou en rouge
        ContenuTchat.Select(position_curseur, nouvelle_position_curseur - position_curseur)

        If erreur = False Then
            ContenuTchat.SelectionColor = Color.Green
        Else
            ContenuTchat.SelectionColor = Color.Red
        End If

        ' On scrolle en bas du t'chat
        ContenuTchat.ScrollToCaret()

        ' On fait clignoter la fenêtre (ne fonctionne que sous Windows)
        Try
            FlashWindow(Me.Handle, 1)
        Catch ex As Exception
            If mode_debug = True Then
                MsgBox("Impossible de faire clignoter le programme dans la barre des tâches: " & vbNewLine & ex.ToString(), MsgBoxStyle.Information, "Mode debug")
            End If
        End Try

        Return True
    End Function

    ' Fonction de mise à jour de la richtextbox du t'chat
    Delegate Sub delegate_maj_contenu_tchat(ByVal pseudonyme As String, ByVal contenu As String)
    Function maj_contenu_tchat(ByVal pseudonyme As String, ByVal contenu As String)

        ' Mise à jour du contenu (au format enrichi)

        ' On note la position du pseudonyme
        Dim debut_pseudo = ContenuTchat.TextLength

        ' On imprime le pseudonyme
        ContenuTchat.AppendText("<" & pseudonyme.Trim() & "> ")

        ' On note la position du contenu
        Dim debut_contenu = ContenuTchat.TextLength

        ' On imprime le contenu
        ContenuTchat.AppendText(contenu & vbNewLine)

        ' On note la nouvelle position du curseur
        Dim fin_contenu = ContenuTchat.TextLength

        ' On met le pseudonyme en gras et en bleu (ou rouge si c'est nous)
        ContenuTchat.Select(debut_pseudo, debut_contenu - debut_pseudo)
        If pseudonyme = pseudonyme_client Then
            ContenuTchat.SelectionColor = Color.Red
        Else
            ContenuTchat.SelectionColor = Color.Blue
        End If

        ' On met le contenu en normal et en noir
        ContenuTchat.Select(debut_contenu, fin_contenu - debut_pseudo)
        ContenuTchat.SelectionColor = Color.Black

        ' On scrolle en bas du t'chat
        ContenuTchat.ScrollToCaret()

        ' Si c'est un message distant
        If pseudonyme <> pseudonyme_client Then
            ' Son de notification (si voulu)
            If connexion.CheckBox3.Checked = False Then
                son_nouveau_message.Play()
            End If

            ' On fait clignoter la fenêtre (ne fonctionne que sous Windows)
            Try
                FlashWindow(Me.Handle, 1)
            Catch ex As Exception
                If mode_debug = True Then
                    MsgBox("Impossible de faire clignoter le programme dans la barre des tâches: " & vbNewLine & ex.ToString(), MsgBoxStyle.Information, "Mode debug")
                End If
            End Try
        End If

        Return True
    End Function

    ' Fonction exécutée lors du lancement du thread d'écoute
    Private Function thread_socket_ecoute()

        ' On défini l'adresse IP d'écoute (même si écoute sur toutes les interfaces)
        Dim strHostName As String = System.Net.Dns.GetHostName()
        Dim iphe As System.Net.IPHostEntry = System.Net.Dns.GetHostEntry(strHostName)

        For Each ipheal As System.Net.IPAddress In iphe.AddressList
            If ipheal.AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork Then
                ip_ecoute = ipheal.ToString()
            End If
        Next

        ' Définition du socket et de l'endpoint
        Dim socket_ecoute As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        Dim endpoint_ecoute As New IPEndPoint(IPAddress.Any, port_communication)

        Try
            ' Bind du socket
            socket_ecoute.Bind(endpoint_ecoute)

            ' Ecoute
            socket_ecoute.Listen(5000)

        Catch ex As Exception
            MsgBox("Impossible d'écouter les connexions sur 0.0.0.0:" & port_communication & vbNewLine & "Auriez-vous un logiciel utilisant déjà le port " & port_communication & "?", vbCritical, "Erreur fatale")
            quitter_si_pb()
        End Try

        ' Tableau de bytes pouvant contenir un message entre 0 et 1000 caractères
        Dim listen_bytes(10000) As Byte

        ' Variable contenant le message "reconstitué"
        Dim message As String = ""

        ' Jusqu'à la fin du thread, faire...
        While True
            ' Empêche les erreurs fatales
            Try
                ' On accepte la connexion entrante
                Dim hote_emmeteur = socket_ecoute.Accept()

                ' On stocke le message reçu dans le tableau de bytes
                hote_emmeteur.Receive(listen_bytes)

                ' On stocke l'adresse IP (sans le port)
                Dim ip_source As String = hote_emmeteur.RemoteEndPoint().ToString().Split(":")(0)

                ' Pour chaque byte du tableau
                For i = 0 To 10000 Step 1
                    ' Et si il est > 0 (0 = NULL parasite)
                    If listen_bytes(i) > 0 Then
                        ' On le traduit en alphabet latin, et on l'ajoute au message
                        message &= Chr(listen_bytes(i))
                    End If
                Next

                ' On reset le tableau de bytes
                Array.Clear(listen_bytes, listen_bytes.GetLowerBound(0), listen_bytes.Length)

                ' On traite le message
                traiter_requete(message, ip_source)

                ' Et on le remet à zéro
                message = ""

                ' On peut fermer la connexion
                hote_emmeteur.Close()

            Catch ex As Exception
                ' Debug: on affiche l'erreur
                If mode_debug = True Then
                    MsgBox("Erreur dans le socket TCP: " & vbNewLine & ex.ToString(), MsgBoxStyle.Information, "Mode debug")
                End If
            End Try
        End While

        ' On ferme le socket d'écoute
        socket_ecoute.Close()

        Return True
    End Function

    ' Fonction exécutée lors du lancement du thread dédié au hello
    Private Function thread_socket_ecoute_hello()

        ' Définition du socket et de l'endpoint
        Dim endpoint_ecoute_hello As New IPEndPoint(IPAddress.Any, port_udp_hello)
        Dim socket_ecoute_hello As New UdpClient(endpoint_ecoute_hello)

        ' On autorise la réception de broadcast (ça vaut mieux...)
        socket_ecoute_hello.EnableBroadcast = True

        ' On commence l'écoute des paquets
        ' Variable contenant le message "reconstitué"
        Dim message As String = ""

        ' Jusqu'à la fin du thread, faire...
        While True
            ' Empêche les erreurs fatales
            Try
                ' On récupère l'adresse IP de l'emmeteur du paquet
                Dim endpoint_emmeteur As EndPoint
                Dim listen_bytes As [Byte]() = socket_ecoute_hello.Receive(endpoint_emmeteur)

                ' Et on la stocke
                Dim ip_source As String = endpoint_emmeteur.ToString().Split(":")(0)

                ' Pour chaque byte du tableau
                For Each octet In listen_bytes
                    ' Et si il est > 0 (0 = NULL parasite)
                    If octet > 0 Then
                        ' On le traduit en alphabet latin, et on l'ajoute au message
                        message &= Chr(octet)
                    End If
                Next

                ' On reset le tableau de bytes
                Array.Clear(listen_bytes, listen_bytes.GetLowerBound(0), listen_bytes.Length)

                ' On traite le message
                traiter_requete(message, ip_source)

                ' Et on le remet à zéro
                message = ""

            Catch ex As Exception
                ' Debug: on affiche l'erreur
                If mode_debug = True Then
                    MsgBox("Erreur dans le socket UDP: " & vbNewLine & ex.ToString(), MsgBoxStyle.Information, "Mode debug")
                End If
            End Try
        End While

        ' On ferme le socket d'écoute
        socket_ecoute_hello.Close()

        Return True
    End Function

    ' Fonction permettant d'envoyer un paquet TCP
    Function envoyer_paquet_TCP(ByVal ip As String, ByVal port As Int32, ByVal contenu As String)
        ' Définition du socket et de l'endpoint
        Dim socket_emission As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        Dim endpoint_emission As New IPEndPoint(IPAddress.Parse(ip), port)

        Try
            ' Connexion à l'hôte distant
            socket_emission.Connect(endpoint_emission)

            ' Chiffrement
            contenu = chiffrement_AES(contenu, cle_chiffrement)

            ' Envoi du message
            socket_emission.SendTo(unicode_converter.GetBytes(contenu), endpoint_emission)

        Catch ex As Exception
            ' On ajoute une perte
            liste_perte_paquets(recuperer_id(ip, False)) += 1

            ' Debug: on affiche l'erreur
            If mode_debug = True Then
                MsgBox("Impossible d'émettre des paquets sur " & ip & ":" & port & " (" & liste_paires_pseudo(recuperer_id(ip, False)) & "). " & (mauvais_paquets_avant_kick - liste_perte_paquets(recuperer_id(ip, False))) & " tentatives restantes.", vbCritical, "Erreur fatale")
                MsgBox("Erreur dans l'emission d'un paquet TCP: " & vbNewLine & ex.ToString(), MsgBoxStyle.Information, "Mode debug")
            End If

            ' mauvais_paquets_avant_kick paquets perdus = kick
            If liste_perte_paquets(recuperer_id(ip, False)) >= mauvais_paquets_avant_kick Then
                ' On récupère l'ID associé à l'IP du client forcé à se déconnecter
                Dim id_client = recuperer_id(ip, False)

                ' Si il existe, on le supprime
                If id_client.GetType() = GetType(Int32) Then
                    ' On l'affiche dans le t'chat
                    Me.Invoke(maj_statut_tchat, New Object() {liste_paires_pseudo(id_client) & " a été supprimé de la liste des utilisateurs car il n'a pas été possible de le joindre " & mauvais_paquets_avant_kick & " fois de suite", True})

                    ' Invocation de la fonction de mise à jour de la liste des pseudonymes
                    Me.Invoke(maj_liste_pseudo, New Object() {})

                    ' Et on vide les cases des trois tableaux
                    liste_paires_ip(id_client) = ""
                    liste_paires_pseudo(id_client) = ""
                    liste_perte_paquets(id_client) = 0
                End If

                ' Pour finir, on rafraichit la liste des personnes connectées
                Me.Invoke(maj_liste_pseudo, New Object() {})
            End If

            quitter_si_pb()
        End Try

        ' On ferme le socket d'écoute
        socket_emission.Close()

        Return True

    End Function

    ' Fonction permettant d'envoyer un paquet UDP
    Function envoyer_paquet_UDP(ByVal ip As String, ByVal port As Int32, ByVal contenu As String)
        ' Définition du socket et de l'endpoint
        Dim socket_emission As New Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        Dim endpoint_emission As New IPEndPoint(IPAddress.Parse(ip), port)

        ' On autorise le broadcast
        socket_emission.EnableBroadcast = True

        Try
            ' Chiffrement
            contenu = chiffrement_AES(contenu, cle_chiffrement)

            ' Envoi du message
            socket_emission.SendTo(unicode_converter.GetBytes(contenu), endpoint_emission)

        Catch ex As Exception
            ' Debug: on affiche l'erreur
            If mode_debug = True Then
                MsgBox("Erreur dans l'emission d'un paquet UDP: " & vbNewLine & ex.ToString(), MsgBoxStyle.Information, "Mode debug")
                MsgBox("Impossible d'émettre des paquets sur " & ip & ":" & port & ".", vbCritical, "Erreur fatale")
            Else
                Me.Invoke(maj_statut_tchat, New Object() {"Impossible d'émettre des paquets sur " & ip & ":" & port & ".", True})
            End If

            quitter_si_pb()
        End Try

        ' On ferme le socket d'écoute
        socket_emission.Close()

        Return True

    End Function

    ' Fonction pour envoyer un "hello" avec un retard de temps_avant_hello secondes 
    Function decouverte_paires()
        ' On affiche un message pour signaler l'initalisation des connexions
        Me.Invoke(maj_statut_tchat, New Object() {"Initialisation, veuillez patienter", False})

        ' On fait une pause de 5 secondes
        System.Threading.Thread.Sleep(temps_avant_hello * 1000)

        ' On envoie un message hello généralisé
        envoyer_paquet_UDP(ip_broadcast, port_udp_hello, "hello " & pseudonyme_client)

        ' On affiche un message pour signaler que tout s'est bien passé
        Me.Invoke(maj_statut_tchat, New Object() {"Vous êtes à présent connecté au t'chat", False})

        ' Et on réactive les boutons
        Me.Invoke(varreactiver_bouton_envoyer, New Object() {})

        ' Et on lance la fonction de rebonjour
        rebonjour_silencieux()

        Return True
    End Function

    ' Fonction pour envoyer un "hello" toutes les 5 minutes
    Function rebonjour_silencieux()
        ' Pour toujours
        While True
            ' Toutes les temps_avant_nouvel_hello minutes
            System.Threading.Thread.Sleep(temps_avant_nouvel_hello * 60 * 1000)

            ' On renvoie un hello
            envoyer_paquet_UDP(ip_broadcast, port_udp_hello, "hello " & pseudonyme_client)
        End While

        Return True
    End Function

    ' Fonction permettant de ré-activer le bouton envoyer
    Delegate Sub delegate_reactiver_bouton_envoyer()
    Function reactiver_bouton_envoyer()
        BoutonEnvoyer.Enabled = True
        MessageAEnvoyer.Enabled = True
        Me.Cursor = Cursors.Default
        ContenuTchat.Cursor = Cursors.Default

        Return True
    End Function

    ' Fonction permettant d'envoyer un message via un autre thread (non bloquant)
    Function envoi_via_autre_thread()
        Try
            ' Pour chaque personne connectée
            For Each ip In liste_paires_ip
                If ip <> "" Then
                    ' On envoi le message
                    envoyer_paquet_TCP(ip, port_communication, "msg " & MessageATransmettre)
                End If
            Next
        Catch ex As Exception
            ' Debug: on affiche l'erreur
            If mode_debug = True Then
                MsgBox("Impossible d'envoyer le message à toutes les personnes: " & vbNewLine & ex.ToString(), vbCritical, "Mode debug")
            End If
        End Try

        Return True
    End Function

    ' Début du code "principal"

    Private Sub fenetre_principale_Load() Handles MyBase.Load
        ' On récupère le pseudonyme
        pseudonyme_client = connexion.Pseudonyme.Text.Trim()

        ' Et on l'ajoute à la liste des connectés
        ListeConnectes.Items.Add(pseudonyme_client & " (c'est vous!)")

        ' Après, on défini le numéro de version
        Dim numero_version As String = "2.0.2"

        ' Juste après, on l'affiche dans la page "À propos"
        Apropos.Text = Apropos.Text & vbNewLine & vbNewLine & "Version utilisée: " & numero_version

        ' Dans un autre temps (paye tes synonymes), on vérifie que le logiciel est à jour (via le site internet)
        Try
            ' On récupère le fichier contenant le numéro de la version la plus récente
            Dim source As New System.Text.StringBuilder
            Dim numero_version_web As String = ""
            Dim maximum As Integer
            Dim request As HttpWebRequest = WebRequest.Create("http://quent1-fr.github.io/Twale/version")
            Dim response As WebResponse = request.GetResponse()
            Dim reader As StreamReader = New StreamReader(response.GetResponseStream())
            Do
                source.Append(reader.ReadLine())
            Loop Until reader.ReadLine() Is Nothing
            maximum = source.Length - 1
            For i As Integer = 0 To maximum
                numero_version_web = numero_version_web + source(i)
            Next
            reader.Close()
            response.Close()

            ' Si une version plus récente existe, et si elle est cohérente (< 10 caractères)
            If numero_version_web <> numero_version And Len(numero_version_web) < 10 Then

                ' Si l'utilisateur souhaite la télécharger
                If MsgBox("La version " & numero_version_web & " de Twale 2 est disponible au téléchargement. Souhaitez-vous la télécharger maintenant?", MsgBoxStyle.YesNo, "Une mise à jour de Twale 2 est disponible!") = MsgBoxResult.Yes Then
                    Process.Start("http://quent1-fr.github.io/Twale/twale2-setup.exe")
                End If
            End If

        Catch ex As Exception
            ' Debug: on affiche l'erreur
            If mode_debug = True Then
                MsgBox("Impossible de vérifier quelle est la version la plus récente: " & vbNewLine & ex.ToString(), MsgBoxStyle.Information, "Mode debug")
            End If
        End Try

        ' Ensuite, on démarre les thread d'écoute des connexions entrantes
        thread_ecoute = New Thread(AddressOf thread_socket_ecoute)
        thread_ecoute.IsBackground = True
        thread_ecoute.Start()

        ' Pour le hello ou le bye, on utilise de l'UDP
        thread_ecoute_hello = New Thread(AddressOf thread_socket_ecoute_hello)
        thread_ecoute_hello.IsBackground = True
        thread_ecoute_hello.Start()

        ' On désactive temporairement les élements permettant d'envoyer des messages
        MessageAEnvoyer.Enabled = False
        BoutonEnvoyer.Enabled = False
        Me.Cursor = Cursors.WaitCursor
        ContenuTchat.Cursor = Cursors.WaitCursor

        ' Pour finir, on utilise un thread séparé pour envoyer un hello avec un retard de temps_avant_hello secondes (pour éviter que l'IP ne sont pas encore connue)
        thread_decouverte_paires = New Thread(AddressOf decouverte_paires)
        thread_decouverte_paires.IsBackground = True
        thread_decouverte_paires.Start()
    End Sub

    Private Sub fenetre_principale_close(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        ' On vérifie si la demande de fermeture est volontaire
        If MsgBox("Êtes-vous sûr(e) de vouloir quitter Twale 2?", MsgBoxStyle.YesNo, "Voulez-vous vraiment quitter?") = MsgBoxResult.Yes Then
            ' À la fermeture, on envoie un message "bye" généralisé
            envoyer_paquet_UDP(ip_broadcast, port_udp_hello, "bye")

            ' On supprime l'icône dans le tray
            IconeNotification.Visible = False

            ' Et on tue la fenêtre de connexion (qui, sinon, empêcherait la fermeture complète du programme)
            connexion.Close()
        Else
            ' Si l'utilisateur refuse, on annule
            e.Cancel = True
        End If

    End Sub

    ' Lorsque l'utilisateur souhaite envoyer un message
    Private Sub BoutonEnvoyer_Click(sender As Object, e As EventArgs) Handles BoutonEnvoyer.Click
        ' Si le message n'est pas vide, on l'envoie
        If MessageAEnvoyer.Text.Trim() <> "" Then

            ' On défini le message à envoyer
            MessageATransmettre = MessageAEnvoyer.Text.Trim()

            ' Et on lance le thread d'envoi
            thread_envoi_messages = New Thread(AddressOf envoi_via_autre_thread)
            thread_envoi_messages.IsBackground = True
            thread_envoi_messages.Start()

            ' Puis on l'affiche dans le t'chat
            maj_contenu_tchat(pseudonyme_client, MessageAEnvoyer.Text.Trim())

            ' Ensuite, on "nettoie" le t'chat
            MessageAEnvoyer.Text = ""
        End If
    End Sub

    ' Permet de simuler un click sur le bouton envoyer si l'utlisateur appuie sur la touche entrée
    Private Sub MessageAEnvoyer_KeyPressed(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles MessageAEnvoyer.KeyPress
        ' Si la personne appuie sur la touche entrée
        If e.KeyChar = ChrW(Keys.Enter) Then
            ' On confirme au système que l'on a bien géré la touche
            e.Handled = True

            ' Et on simule un click sur le bouton "Envoyer"
            BoutonEnvoyer.PerformClick()
        Else
            ' On dit au système que l'on n'a pas géré la touche
            e.Handled = False
        End If
    End Sub

    ' Si l'utilisateur choisi de fermer Twale 2
    Private Sub FermerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FermerToolStripMenuItem.Click
        ' Alors on initialise la procédure de fermeture
        Me.Close()
    End Sub

    ' Si l'utlisateur cherche à afficher le t'chat
    Private Sub TchatToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TchatToolStripMenuItem.Click
        ' Alors on affiche la fenêtre principale, et on bascule sur le bon onglet
        Me.WindowState = FormWindowState.Normal
        TabControl1.SelectedTab = TabTchat
    End Sub

    ' Lorsque l'utlisateur clique sur l'icône dans la barre de notification
    Private Sub IconeNotification_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles IconeNotification.MouseDoubleClick
        ' On restaure la fenêtre principale
        Me.WindowState = FormWindowState.Normal
    End Sub

    ' Lorsque l'utlisateur clique sur un lien
    Protected Sub Link_Clicked(ByVal sender As Object, ByVal e As System.Windows.Forms.LinkClickedEventArgs) Handles ContenuTchat.LinkClicked
        Process.Start(e.LinkText)
    End Sub

End Class