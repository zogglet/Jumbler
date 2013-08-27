Imports System.IO
Imports PuzzlePieceButton
Imports System.Data
Imports System.Data.SqlClient

Partial Class _Default
    Inherits System.Web.UI.Page

    Dim boardDir As String = ""

    Dim pieces As ArrayList
    Dim jumbledPieces As ArrayList

    Dim oConn As New SqlConnection(ConfigurationManager.ConnectionStrings.Item("HighScoresConnectionString").ConnectionString)
    Dim oCmd As New SqlCommand
    Dim oDA As New SqlDataAdapter
    Dim odTbl As New DataTable
    Dim strSQL As String = ""
    Dim oParam As New SqlParameter


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If IsPostBack Then

            If Session("FromBoard") = True Then
                Dim pieces_tbl As Table = CType(Session("BoardTable"), Table)
                pieces_tbl.Rows.Clear()

                board_pnl.Controls.Add(pieces_tbl)

                Dim pieces As ArrayList = CType(Session("OrderedPieces"), ArrayList)
                Dim numRows As Integer = Math.Sqrt(pieces.Count)

                Dim row As TableRow = Nothing
                Dim cell As TableCell = Nothing

                Dim i As Integer = 0

                While i < pieces.Count

                    For j As Integer = 0 To numRows - 1

                        row = New TableRow()

                        For k As Integer = 0 To numRows - 1
                            cell = New TableCell()
                            cell.Width = 612 / numRows

                            cell.Controls.Add(CType(pieces(i), PuzzlePieceButton))

                            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(pieces(i))
                            AddHandler CType(pieces(i), PuzzlePieceButton).Click, AddressOf pieceClick

                            row.Cells.Add(cell)
                            i += 1
                        Next

                        pieces_tbl.Rows.Add(row)
                    Next
                End While

            End If
        Else
            Session("FromBoard") = Nothing

        End If
    End Sub

    Protected Sub generateBoard()

        Dim pieces_tbl As Table = New Table()
        pieces_tbl.ID = "pieces_tbl"
        pieces_tbl.Width = 612

        board_pnl.Enabled = True
        board_pnl.Controls.Add(pieces_tbl)
        Session("BoardTable") = pieces_tbl

        'Hey, I want to use the DDL to easily access the number of table cells needed. Sorry for the inconsistency in 
        'by what property of the DDL I'm retrieving data.
        boardDir = Page.MapPath(".") & "\" & puzzle_ddl.SelectedValue & "\" & difficulty_ddl.SelectedItem.Text

        'To accommodate for puzzles with a different number of pieces
        Dim numPieces = Directory.GetFiles(boardDir, "*").Length - 1

        Dim numRows As Integer = Math.Sqrt(numPieces)

        Dim row As TableRow = Nothing
        Dim cell As TableCell = Nothing

        pieces = New ArrayList(numPieces)
        jumbledPieces = New ArrayList()

        Dim piece As PuzzlePieceButton = Nothing
        Dim i As Integer = 0

        While i < numPieces

            row = New TableRow()

            For j As Integer = 0 To numRows - 1

                cell = New TableCell()
                cell.Width = 612 / numRows

                piece = New PuzzlePieceButton()

                With piece
                    .ID = "piece_" & i + 1
                    .Width = (612 / numRows) - 2
                    .Height = (612 / numRows) - 2
                    .ImageUrl = puzzle_ddl.SelectedValue & "/" & difficulty_ddl.SelectedItem.Text & "/piece_" & i + 1 & ".png"
                    .Order = i + 1
                End With

                cell.Controls.Add(piece)
                pieces.Add(piece)

                ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(piece)
                AddHandler piece.Click, AddressOf pieceClick

                row.Cells.Add(cell)
                i += 1

            Next

            pieces_tbl.Rows.Add(row)
        End While

        jumblePieces()

        Session("Board") = puzzle_ddl.SelectedItem.Text
        Session("SelectedCounter") = 0
        Session("MovesMade") = 0
        Session("CorrectMoves") = 0
        Session("FromBoard") = True
        Session("PiecesLeft") = jumbledPieces.Count
        Session("OrderedPieces") = pieces
        Session("JumbledPieces") = jumbledPieces
        Session("Score") = 10 * (CType(Session("OrderedPieces"), ArrayList).Count - Session("PiecesLeft"))

        Session("Timed") = timed_cbx.Checked
        Session("SecondCounter") = 60

    End Sub

    Protected Sub createBoard_btn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles createBoard_btn.Click

        Session("FromBoard") = Nothing

        options_pnl.Visible = False
        status_pnl.Visible = True

        generateBoard()

        board_lit.Text = "Board: <b>" & Session("Board") & "</b><br />Score: <b>" & Session("Score") & "</b>"
        status_lit.Text = "<i>Select two pieces to switch their positions.</i><br />Pieces left: <b>" & Session("Piecesleft") & "</b>"

        If Session("Timed") = True Then
            countdown_lit.Text = "<span class='CountdownStyle'>:" & Session("SecondCounter") & "</span>"

            puzzle_timer.Enabled = True
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "puzzle_timer", "startTimer();", True)
        End If


    End Sub

    Protected Sub jumblePieces()

        Dim rand As New Random()
        Dim i As Integer = pieces.Count - 1
        Dim newPos As Integer = 0
        Dim tempImgUrl As String = ""
        Dim tempOrder As Integer = 0

        While i >= 0
            newPos = rand.Next(pieces.Count - 1)
            tempImgUrl = CType(pieces(i), PuzzlePieceButton).ImageUrl
            tempOrder = CType(pieces(i), PuzzlePieceButton).Order

            CType(pieces(i), PuzzlePieceButton).ImageUrl = CType(pieces(newPos), PuzzlePieceButton).ImageUrl
            CType(pieces(i), PuzzlePieceButton).Order = CType(pieces(newPos), PuzzlePieceButton).Order

            CType(pieces(newPos), PuzzlePieceButton).ImageUrl = tempImgUrl
            CType(pieces(newPos), PuzzlePieceButton).Order = tempOrder

            i -= 1

        End While

        For j As Integer = 0 To pieces.Count - 1
            If CType(pieces(j), PuzzlePieceButton).Order = j + 1 Then
                CType(pieces(j), PuzzlePieceButton).Correct = True
            Else
                jumbledPieces.Add(CType(pieces(j), PuzzlePieceButton))
            End If

            'Showing the order just for testing
            'CType(pieces(j), PuzzlePieceButton).Text = CType(pieces(j), PuzzlePieceButton).Order
        Next

        board_updatePnl.Update()

    End Sub

    Protected Sub resetPuzzle()

        If Session("Timed") = True Then
            puzzle_timer.Enabled = False
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "puzzle_timer", "stopTimer();", True)
        End If


        Session.Clear()

        board_pnl.Controls.Clear()

        result_mpExt.Hide()

        status_pnl.Visible = False
        options_pnl.Visible = True

        board_updatePnl.Update()
        optionsStatus_updatePnl.Update()
    End Sub


    Protected Sub pieceClick(ByVal sender As Object, ByVal e As EventArgs)

        Dim clickedPiece As PuzzlePieceButton = CType(sender, PuzzlePieceButton)
        Dim innerStr As String = ""
        Dim baseScore As Integer
        Dim percentCorrectMoves As Integer
        Dim scoreStr As String = ""
        Dim moveExisted As Boolean

        Session("SelectedCounter") += 1

        If Session("SelectedCounter") = 2 Then

            Session("SelectedCounter") = 0

            moveExisted = didMoveExist(CType(Session("FirstSelected"), PuzzlePieceButton).Order, Session("Selected2Order"))

            If clickedPiece.Order <> CType(Session("FirstSelected"), PuzzlePieceButton).Order And moveExisted = False Then
                Session("MovesMade") += 1
            End If

            'Save these values to swap with first selected values
            Session("Selected2Order") = clickedPiece.Order
            Session("Selected2ImageUrl") = clickedPiece.ImageUrl

            'Swap order with first selected piece
            clickedPiece.Order = CType(Session("FirstSelected"), PuzzlePieceButton).Order
            clickedPiece.ImageUrl = CType(Session("FirstSelected"), PuzzlePieceButton).ImageUrl

            'Swap first selected values with second
            CType(Session("FirstSelected"), PuzzlePieceButton).Order = Session("Selected2Order")
            CType(Session("FirstSelected"), PuzzlePieceButton).ImageUrl = Session("Selected2ImageUrl")
            CType(Session("FirstSelected"), PuzzlePieceButton).Selected = False
            clickedPiece.Selected = False

            'Check for correct pieces, and configure JumbledPieces and OrderedPieces arrays acccordingly
            For j As Integer = 0 To CType(Session("OrderedPieces"), ArrayList).Count - 1
                If CType(CType(Session("OrderedPieces"), ArrayList)(j), PuzzlePieceButton).Order = j + 1 Then
                    CType(CType(Session("OrderedPieces"), ArrayList)(j), PuzzlePieceButton).Correct = True
                    CType(Session("JumbledPieces"), ArrayList).Remove(CType(Session("OrderedPieces"), ArrayList)(j))
                Else
                    If Not CType(Session("JumbledPieces"), ArrayList).Contains(CType(Session("OrderedPieces"), ArrayList)(j)) Then
                        CType(Session("JumbledPieces"), ArrayList).Add(CType(Session("OrderedPieces"), ArrayList)(j))
                    End If
                    CType(CType(Session("OrderedPieces"), ArrayList)(j), PuzzlePieceButton).Correct = False
                End If
            Next

            Session("PiecesLeft") = CType(Session("JumbledPieces"), ArrayList).Count
            Session("Score") = 10 * (CType(Session("OrderedPieces"), ArrayList).Count - Session("PiecesLeft"))

            ' Swapping a piece with itself doesn't count as a correct move
            If (CType(Session("FirstSelected"), PuzzlePieceButton).Correct = True Or clickedPiece.Correct = True) And (clickedPiece.Order <> CType(Session("FirstSelected"), PuzzlePieceButton).Order) And moveExisted = False Then
                Session("CorrectMoves") += 1
            End If


            '''''''''''''''''''''''''''''

            'For testing
            test_lit.Text = "Moves: " & Session("MovesMade") & "<br />Correct moves: " & Session("CorrectMoves")


            If Session("PiecesLeft") = 0 Then

                Dim scores As DataTable = currentHighScores()

                If Session("Timed") = True Then
                    puzzle_timer.Enabled = False
                    Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "puzzle_timer", "stopTimer();", True)
                End If

                board_pnl.Enabled = False

                'Score and result configuration
                baseScore = IIf(Session("Timed") = True, Session("Score") + (Session("SecondCounter") * 10), Session("Score"))
                percentCorrectMoves = (Session("CorrectMoves") / Session("MovesMade")) * 100

                Session("FinalScore") = baseScore * percentCorrectMoves

                scoreStr = "Score: " & IIf(Session("Timed") = True, "<span class='SuccessStyle'>" & Session("Score") & "</span><br />x <span class='SuccessStyle'>" & Session("SecondCounter") & "</span> (time remaining multiplier)<br />x <span class='SuccessStyle'>" & percentCorrectMoves & "</span> (percentage of total moves made correct)<br /><span class='ScoreStyle'>= <span class='SuccessStyle'>" & baseScore * percentCorrectMoves & "</span></span>", "<span class='SuccessStyle'>" & Session("Score") & "</span><br />x <span class='SuccessStyle'>" & percentCorrectMoves & "</span> (percentage of total moves correct)<br />= <span class='SuccessStyle'>" & baseScore * percentCorrectMoves & "</span>")
                innerStr = "<span class='SuccessStyle'><i>Congratulations! You solved the puzzle " & IIf(Session("Timed") = True, "in time", "") & "!</i></span>"

                result_lit.Text = "<span class='GameOverStyle'>GAME OVER</span><br /><br /><i>Congratulations! You solved the puzzle" & IIf(Session("Timed") = True, " in " & 60 - Session("SecondCounter") & " seconds", "") & "!</i><br />" & scoreStr

                If Session("FinalScore") > CInt(scores.Rows(scores.Rows.Count - 1)("Score")) Then
                    highScores_pnl.Visible = True
                End If

                'I'm not sure why, but in order for the modal popup to show its content in this handler, I need to manually update the update panel. I don't have to in the tick handler...
                result_updatePnl.Update()
                result_mpExt.Show()

            Else

                innerStr = IIf(CType(Session("FirstSelected"), PuzzlePieceButton).Correct = False And clickedPiece.Correct = False, IIf(moveExisted = False, "<i>Keep swapping!</i>", "<span class='FailStyle'><i>You've made that move already</i></span>"), "<span class='SuccessStyle'><i>Correct selection!</i></span>")

                board_lit.Text = "Board: <b>" & Session("Board") & "</b>"
                status_lit.Text = innerStr & "<br />Pieces left: <b>" & Session("PiecesLeft") & "</b>"

            End If

        Else
            clickedPiece.Selected = True
            innerStr = "<i>Select another piece to swap.</i>"

            Session("FirstSelected") = clickedPiece
        End If

        board_lit.Text = "Board: <b>" & Session("Board") & "</b>" & IIf(Session("PiecesLeft") = 0, "<br />" & scoreStr, "")
        status_lit.Text = innerStr & IIf(Session("PiecesLeft") <> 0, "<br />Pieces left: <b>" & Session("PiecesLeft") & "</b>", "")

        optionsStatus_updatePnl.Update()

        Session("FromBoard") = True

    End Sub


    Protected Sub puzzle_timer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles puzzle_timer.Tick

        Session("SecondCounter") -= 1

        If Session("SecondCounter") = 0 Then

            Dim scores As DataTable = currentHighScores()

            puzzle_timer.Enabled = False
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "puzzle_timer", "stopTimer();", True)

            board_pnl.Enabled = False

            'Score and result configuration
            Dim percentSolved As Integer = ((CType(Session("OrderedPieces"), ArrayList).Count - Session("PiecesLeft")) / (CType(Session("OrderedPieces"), ArrayList).Count)) * 100
            Dim percentCorrectMoves = Math.Floor((Session("CorrectMoves") / Session("MovesMade")) * 100)

            Session("FinalScore") = Session("Score") * percentCorrectMoves

            board_lit.Text = "Board: <b>" & Session("Board") & "</b><br />Score: <span class='SuccessStyle'>" & Session("Score") & "</span><br />x <span class='SuccessStyle'>" & percentCorrectMoves & "</span> (percent of total moves correct)<br />= <span class='SuccessStyle'>" & Session("FinalScore") & "</span>"
            status_lit.Text = "<b>GAME OVER</b><br />" & IIf(Session("PiecesLeft") = 0, "<span class='SuccessStyle'>Congratulations! You solved the puzzle just in time!</span>", "Pieces Missed: <span class='FailStyle'>" & Session("PiecesLeft") & "</span>")

            result_lit.Text = "<span class='GameOverStyle'>GAME OVER</span><br /><br />" & IIf(Session("PiecesLeft") = 0, "Congratulations! You solved the puzzle just in time!", "Pieces Missed: <span class='FailStyle'>" & Session("PiecesLeft") & "</span><br /><span class='SuccessStyle'>" & percentSolved & "%</span> solved </span>") & "<br />Score: <span class='SuccessStyle'>" & Session("Score") & "</span><br />x <span class='SuccessStyle'>" & percentCorrectMoves & "</span> (percent of total moves correct)<br />= <span class='SuccessStyle'>" & Session("FinalScore") & "</span>"

            If Session("FinalScore") > CInt(scores.Rows(scores.Rows.Count - 1)("Score")) Then
                highScores_pnl.Visible = True
            End If

            result_mpExt.Show()

        Else
            countdown_lit.Text = "<span class='CountdownStyle'>:" & IIf(Session("SecondCounter") <= 9, "0", "") & Session("SecondCounter") & "</span>"

        End If

    End Sub

    Protected Sub hint_btn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles hint_btn.Click

        hint_lit.Text = "<h4>" & Session("Board") & "</h4>" & "<img src='" & puzzle_ddl.SelectedValue & "_hint.png' />"
        hint_mpExt.Show()

        If Session("Timed") = True Then
            puzzle_timer.Enabled = False
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "puzzle_timer", "stopTimer();", True)

            countdown_lit.Text = "<span class='CountdownStyle'>[PAUSED]</span>"
        End If


    End Sub

    Protected Sub close_lBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles close_lBtn.Click

        hint_mpExt.Hide()

        If Session("Timed") = True Then
            puzzle_timer.Enabled = True
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "puzzle_timer", "startTimer();", True)
        End If


    End Sub

    Protected Sub timed_cbx_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles timed_cbx.CheckedChanged
        countdown_lit.Visible = timed_cbx.Checked
    End Sub

    Protected Sub instructions_btn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles instructions_btn.Click
        Dim instructionsStr As String = "<h4>Welcome to Jumbler</h4>" & _
                                        "<p>In this game, you must reorder the pieces of a jumbled Halo-related image. This is done by selecting two pieces, whose positions will then swap." & _
                                        "<p>You can choose to play the game timed or untimed. If you choose to play it timed, you will have one minute to solve the puzzle. If you solve the puzzle " & _
                                        "within the time limit, you win the game, and your score will reflect the time in which you completed the puzzle. If the puzzle isn't solved in time, you lose.</p>" & _
                                        "<p><i>I created this game as a study in dynamic control generation.</i></p>"

        instructions_lit.Text = instructionsStr

        instructions_mpExt.Show()
    End Sub

    Protected Sub close_lBtn3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles close_lBtn3.Click
        instructions_mpExt.Hide()
    End Sub

    Protected Function didMoveExist(ByVal order1 As Integer, ByVal order2 As Integer) As Boolean

        If order1 = Nothing Or order2 = Nothing Then
            Return False
        End If
        Return IIf(order1 = order2 Or order2 = order1, True, False)

    End Function

    Protected Function currentHighScores() As DataTable

        Try
            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            odTbl.Clear()

            strSQL = "SELECT *, DifficultyLevel, BoardName FROM HighScores INNER JOIN Difficulties ON HighScores.DifficultyID = Difficulties.ID INNER JOIN PuzzleBoards ON HighScores.PuzzleBoardID = PuzzleBoards.ID ORDER BY Score DESC"

            oCmd.CommandText = strSQL
            oDA.SelectCommand = oCmd

            oDA.Fill(odTbl)

            Return odTbl

        Catch ex As Exception
            Throw ex
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If

            oCmd.Dispose()
            oDA.Dispose()
        End Try

    End Function

    Protected Sub submitScore_btn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles submitScore_btn.Click

        result_mpExt.Hide()

        Dim scores As DataTable = currentHighScores()

        If scores.Rows.Count < 10 Then
            addNewHighScore(False)
        ElseIf Session("FinalScore") >= CInt(scores.Rows(scores.Rows.Count - 1)("Score")) Then
            addNewHighScore(True)
        End If

        scores_gv.DataSource = currentHighScores()
        scores_gv.DataBind()

        viewScores_UpdatePnl.Update()

        highScores_mpExt.Show()

    End Sub

    Protected Sub addNewHighScore(ByVal replace As Boolean)

        Try
            oCmd.Connection = oConn
            oCmd.CommandType = CommandType.Text

            oCmd.Parameters.Clear()
            odTbl.Clear()

            If replace Then

                strSQL = "DELETE FROM HighScores WHERE Score = (SELECT MIN(Score) FROM HighScores)"

                oCmd.CommandText = strSQL

                oCmd.Connection.Open()
                oCmd.ExecuteScalar()
                oCmd.Connection.Close()

            End If

            strSQL = "INSERT INTO HighScores (Name, Score, DifficultyID, PuzzleBoardID, TheDate) VALUES (@Name, @Score, @Difficulty, @PuzzleBoard, @TheDate)"

            oCmd.CommandText = strSQL

            oParam = New SqlParameter()
            oParam.ParameterName = "Name"
            oParam.SqlDbType = SqlDbType.VarChar
            oParam.Value = IIf(score_txt.Text.Trim.Length > 0, score_txt.Text.Trim, "Anonymous")
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter()
            oParam.ParameterName = "Score"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = Session("FinalScore")
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter()
            oParam.ParameterName = "Difficulty"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = difficulty_ddl.SelectedIndex
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter()
            oParam.ParameterName = "PuzzleBoard"
            oParam.SqlDbType = SqlDbType.Int
            oParam.Value = puzzle_ddl.SelectedIndex
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter()
            oParam.ParameterName = "TheDate"
            oParam.SqlDbType = SqlDbType.DateTime
            oParam.Value = DateTime.Now
            oCmd.Parameters.Add(oParam)

            oCmd.Connection.Open()
            oCmd.ExecuteScalar()

        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    Protected Sub close_lbtn4_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles close_lbtn4.Click
        highScores_mpExt.Hide()
    End Sub

    Public Function formatDateText(ByVal theDate As DateTime) As String
        Return (theDate.ToShortDateString & ", " & theDate.ToShortTimeString)
    End Function

    Protected Sub scores_gv_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles scores_gv.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.RowIndex = 0 Then
                e.Row.Cells(0).ForeColor = Drawing.ColorTranslator.FromHtml("#a8e03d")
                For i As Integer = 1 To e.Row.Cells.Count - 1
                    e.Row.Cells(i).Font.Bold = True
                Next
            End If
        End If
    End Sub

    Protected Sub viewScores_btn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles viewScores_btn.Click

        scores_gv.DataSource = currentHighScores()
        scores_gv.DataBind()

        highScores_mpExt.Show()

    End Sub
End Class
