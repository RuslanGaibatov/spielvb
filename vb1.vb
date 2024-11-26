Public Class Form1
    Dim hahnPositionX As Integer = 100
    Dim hahnPositionY As Integer = 450
    Dim hahnSpeed As Integer = 20 ' Geschwindigkeit des Huhns
    Dim obstacleSpeed As Integer = 5 ' Anfangsgeschwindigkeit der Hindernisse
    Dim score As Integer = 0 ' Punktestand
    Dim obstacles As New List(Of Rectangle) ' Liste der Hindernisse
    Dim targetX As Integer = 1000 ' X-Position des Ziels
    Dim targetY As Integer = 200 ' Y-Position des Ziels
    Dim targetWidth As Integer = 30 ' Breite des Ziels
    Dim targetHeight As Integer = 50 ' Höhe des Ziels
    Dim rand As New Random()

    Dim gameOver As Boolean = False
    Dim gameWon As Boolean = False ' Variable für Gewinnstatus

    Dim obstacleSpawnInterval As Integer = 1500 ' Interval in Millisekunden für Hindernisse
    Dim lastObstacleTime As Integer = 0

    Public Sub New()
        InitializeComponent()
        Me.Size = New Drawing.Size(1109, 629)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.BackColor = Color.LightSkyBlue
        Me.KeyPreview = True
        Me.DoubleBuffered = True

        ' Timer für das Spielintervall
        Dim timer As New Timer()
        timer.Interval = 30
        AddHandler timer.Tick, AddressOf Timer_Tick
        timer.Start()

        AddObstacles() ' Initiales Hindernis hinzufügen
    End Sub

    Private Sub Timer_Tick(sender As Object, e As EventArgs)
        If gameOver OrElse gameWon Then
            Me.Invalidate()
            Return
        End If

        ' Zeit für das nächste Hindernis
        lastObstacleTime += 30
        If lastObstacleTime >= obstacleSpawnInterval Then
            AddObstacles() ' Füge ein neues Hindernis hinzu
            lastObstacleTime = 0
        End If

        ' Überprüfe auf Kollision mit Ziel
        If hahnPositionX + 30 >= targetX AndAlso hahnPositionY + 30 >= targetY AndAlso hahnPositionY <= targetY + targetHeight Then
            score += 10
            gameWon = True ' Setze den Gewinnstatus auf wahr
        End If

        ' Bewege Hindernisse nach links und überprüfe Kollision
        For i As Integer = 0 To obstacles.Count - 1
            Dim obstacle = obstacles(i)
            obstacle.X -= obstacleSpeed
            If obstacle.X < 0 Then
                obstacles(i) = New Rectangle(rand.Next(700, 1100), rand.Next(500), 20, 20)
            Else
                obstacles(i) = obstacle
            End If

            ' Überprüfe auf Kollision mit Hindernis
            If obstacle.IntersectsWith(New Rectangle(hahnPositionX, hahnPositionY, 30, 30)) Then
                score += 10 ' Punkte bei Kollision mit Hindernis
                If score Mod 20 = 0 Then
                    obstacleSpeed += 1 ' Geschwindigkeit der Hindernisse erhöhen
                End If
            End If
        Next

        Me.Invalidate()
    End Sub

    ' Füge Hindernisse hinzu
    Private Sub AddObstacles()
        obstacles.Add(New Rectangle(rand.Next(700, 1100), rand.Next(500), 20, 20))
    End Sub

    ' Steuerung des Huhns mit W, A, S, D
    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        If gameOver OrElse gameWon Then
            Return
        End If

        MyBase.OnKeyDown(e)

        Select Case e.KeyCode
            Case Keys.W
                If hahnPositionY > 0 Then hahnPositionY -= hahnSpeed
            Case Keys.S
                If hahnPositionY < 470 Then hahnPositionY += hahnSpeed
            Case Keys.A
                If hahnPositionX > 0 Then hahnPositionX -= hahnSpeed
            Case Keys.D
                If hahnPositionX < 970 Then hahnPositionX += hahnSpeed
        End Select

        Me.Invalidate()
    End Sub

    ' Malen der Grafik
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        ' Zeichne das Huhn
        e.Graphics.FillEllipse(Brushes.Yellow, hahnPositionX, hahnPositionY, 30, 30)

        ' Zeichne das Ziel
        e.Graphics.FillRectangle(Brushes.Red, targetX, targetY, targetWidth, targetHeight)

        ' Zeichne die Hindernisse
        For Each obstacle As Rectangle In obstacles
            e.Graphics.FillRectangle(Brushes.Black, obstacle)
        Next

        ' Zeichne den Punktestand (nur die Zahl, ohne "Score:")
        e.Graphics.DrawString(score.ToString(), New Font("Arial", 16), Brushes.Black, 10, 10)

        ' Wenn das Spiel vorbei ist
        If gameOver Then
            e.Graphics.DrawString("Игра окончена!", New Font("Arial", 30), Brushes.Red, 400, 250)
            e.Graphics.DrawString("Нажмите R для перезапуска", New Font("Arial", 16), Brushes.Black, 400, 300)
        End If

        ' Wenn der Spieler gewonnen hat
        If gameWon Then
            e.Graphics.DrawString("Вы выиграли!", New Font("Arial", 30), Brushes.Green, 400, 250)
            e.Graphics.DrawString("Нажмите R для перезапуска", New Font("Arial", 16), Brushes.Black, 400, 300)
        End If
    End Sub

    ' Ermögliche Neustart des Spiels nach Verlust oder Gewinn
    Protected Overrides Sub OnKeyPress(e As KeyPressEventArgs)
        If e.KeyChar = "r"c Then
            gameOver = False
            gameWon = False ' Setze den Gewinnstatus zurück
            score = 0
            hahnPositionX = 100
            hahnPositionY = 450
            AddObstacles() ' Füge ein erstes Hindernis hinzu
            Me.Invalidate()
        End If
        MyBase.OnKeyPress(e)
    End Sub
End Class
