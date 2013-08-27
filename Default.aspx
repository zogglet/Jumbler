<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Jumbler: A Game by Maggy Maffia</title>
    <link href="style.css" rel="stylesheet" type="text/css" />
    
    <script type="text/javascript" language="javascript">

        function startTimer() {
            var timer = $get('<%# puzzle_timer.ClientID %>');
            timer._startTimer();
        }

        function stopTimer() {
            var timer = $get('<%# puzzle_timer.ClientID %>');
            timer._stopTimer();
        }

        //Prevents Timer from snapping to the top of the page on each tick
        function scrollTo() {
            return;
        }
    
    </script>
    
</head>
<body>
    <form id="form1" runat="server">
    
        <%--Required for use of AJAX Control Toolkit --%>
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" />
        
        <h1>Jumbler</h1>
        <h2>A Game by Maggy Maffia</h2>
        
        <div id="outer_div">
        
            <table id="outer_table">
                <tr>
                    <td>
                        Welcome to <b>Jumbler</b>, a Halo-themed tile puzzle!
                        <span class="Divider"><br /></span>
                        
                        <br />
                        <asp:UpdatePanel ID="optionsStatus_updatePnl" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="options_pnl" runat="server">
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <asp:DropDownList ID="puzzle_ddl" runat="server" CssClass="InputStyle" DataSourceID="puzzle_sds" DataTextField="BoardName" DataValueField="Directory" />
                                                
                                                <asp:SqlDataSource ID="puzzle_sds" runat="server" ConnectionString="<%$ ConnectionStrings:HighScoresConnectionString %>" 
                                                    SelectCommand="SELECT Directory, BoardName FROM PuzzleBoards UNION SELECT '-1' AS Directory, '&laquo; Select Puzzle &raquo;' AS BoardName ORDER BY Directory" />
                                                    
                                                <asp:CompareValidator ID="puzzle_cVal" runat="server" ControlToValidate="puzzle_ddl" Operator="NotEqual" 
                                                    ValueToCompare="-1" ErrorMessage="Please select a puzzle first." Display="None" />
                                                <asp:ValidatorCalloutExtender ID="puzzle_vcExt" runat="server" TargetControlID="puzzle_cVal" WarningIconImageUrl="warningIcon.png" 
                                                    CloseImageUrl="closeIcon.png" CssClass="ValidatorCalloutStyle" Width="200px" />
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="timed_cbx" runat="server" Text="Timed" TextAlign="Right" AutoPostBack="true" />
                                            </td>
                                        
                                            <td align="right">
                                                <asp:Button ID="viewScores_btn" runat="server" Text="High Scores" CssClass="ButtonStyle" CausesValidation="false" />
                                                &nbsp;<asp:Button ID="instructions_btn" runat="server" Text="Instructions" CssClass="ButtonStyle" CausesValidation="false" />
                                                &nbsp;<asp:Button ID="createBoard_btn" runat="server" Text="Start Puzzle &raquo;" CssClass="ButtonStyle" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:DropdownList ID="difficulty_ddl" runat="server" CssClass="InputStyle" DataSourceID="difficulty_sds" DataTextField="DifficultyLevel" DataValueField="ButtonCount" />
                                                
                                                <asp:SqlDataSource ID="difficulty_sds" runat="server" ConnectionString="<%$ ConnectionStrings:HighScoresConnectionString %>" 
                                                    SelectCommand="SELECT ButtonCount, DifficultyLevel FROM Difficulties UNION SELECT -1 AS ButtonCount, '&laquo; Select Difficulty &raquo;' AS DifficultyLevel ORDER BY ButtonCount" />
                                               
                                                <asp:CompareValidator ID="difficulty_cVal" runat="server" ControlToValidate="difficulty_ddl" Operator="NotEqual" 
                                                    ValueToCompare="-1" ErrorMessage="Please select a difficulty first." Display="None" />
                                                <asp:ValidatorCalloutExtender ID="difficulty_vcExt" runat="server" TargetControlID="difficulty_cVal" WarningIconImageUrl="warningIcon.png" 
                                                    CloseImageUrl="closeIcon.png" CssClass="ValidatorCalloutStyle" Width="200px" />

                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                
                                <asp:Panel ID="status_pnl" runat="server" Visible="false">
                                    <table width="100%">
                                        <tr>
                                            <td valign="top">
                                                <asp:Literal ID="countdown_lit" runat="server" />
                                            </td>
                                            <td valign="top">
                                                <asp:literal ID="board_lit" runat="server" />
                                            </td>
                                            <td valign="top">
                                                <asp:Literal ID="status_lit" runat="server" />
                                                <br /><asp:Literal ID="test_lit" runat="server" />
                                            </td>
                                            <td align="right">
                                                <asp:Button ID="hint_btn" runat="server" Text="Show Hint" CssClass="ButtonStyle" CausesValidation="false" />
                                                &nbsp;<asp:Button ID="reset_btn" runat="server" Text="Restart Puzzle" CssClass="ButtonStyle" OnClick="resetPuzzle" CausesValidation="false" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </ContentTemplate>
                            
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="createBoard_btn" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="reset_btn" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="puzzle_timer" EventName="Tick" />
                                <asp:AsyncPostBackTrigger ControlID="hint_btn" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="restart_lBtn" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="timed_cbx" EventName="CheckedChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <asp:DropShadowExtender ID="instructions_dsExt" runat="server" TargetControlID="instructions_pnl" Opacity=".15" Width="4" TrackPosition="true" />
                        
                        <%--Instructions Modal Popup--%>
                        <asp:ModalPopupExtender ID="instructions_mpExt" runat="server" TargetControlID="dummy3" PopupControlID="instructions_pnl" />
                        <input type="button" id="dummy3" runat="server" style="display: none;" />
                        
                        <asp:Panel ID="instructions_pnl" runat="server" CssClass="ModalStyle" Width="350px">
                            <asp:UpdatePanel ID="instructions_updatePnl" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Literal ID="instructions_lit" runat="server" />
                                </ContentTemplate>
                                
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="instructions_btn" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="close_lbtn3" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                            
                            <span style="text-align: center; width:100%; display: block;"><asp:LinkButton ID="close_lBtn3" runat="server" Text="[Close]" CausesValidation="false" /></span>
                            
                        </asp:Panel>
                        
                        <asp:DropShadowExtender ID="hint_dsExt" runat="server" TargetControlID="hint_pnl" Opacity=".25" Width="4" TrackPosition="true" />

                        <%--Hint Modal Popup--%>
                        <asp:ModalPopupExtender ID="hint_mpExt" runat="server" TargetControlID="dummy" PopupControlID="hint_pnl" />
                        <input type="button" id="dummy" runat="server" style="display: none;" />
                        
                        <asp:Panel ID="hint_pnl" runat="server" CssClass="ModalStyle">
                            <asp:UpdatePanel ID="hint_updatePnl" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Literal ID="hint_lit" runat="server" />
                                </ContentTemplate>
                                
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="hint_btn" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="close_lBtn" EventName="Click" />
                                </Triggers>
                                
                            </asp:UpdatePanel>
                            <br />
                            <span style="text-align:center;display:block;width:100%;"><asp:LinkButton ID="close_lBtn" runat="server" Text="[Close]" CausesValidation="false" /></span>
                        </asp:Panel>
                        
                        
                        <asp:DropShadowExtender ID="result_dsExt" runat="server" TargetControlID="result_pnl" Opacity=".15" Width="4" TrackPosition="true" />
                        
                        <%--Result Modal Popup--%>
                        <asp:ModalPopupExtender ID="result_mpExt" runat="server" TargetControlID="dummy2" PopupControlID="result_pnl" />
                        <input type="button" id="dummy2" runat="server" style="display: none;" />
                        
                        <asp:Panel ID="result_pnl" runat="server" CssClass="ResultStyle">
                            <asp:UpdatePanel ID="result_updatePnl" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Literal ID="result_lit" runat="server" />
                                    
                                    <%--High Score input panel--%>
                                    <asp:Panel ID="highScores_pnl" runat="server" CssClass="inner" Visible="false">
                                        <span class="ModalDivider">&nbsp;</span>
                                        <br /><span class="GameOverStyle">New High Score!</span>
                                        <p>Enter your name to submit your score: <asp:TextBox ID="score_txt" runat="server" /></p>
                                        <asp:Button ID="submitScore_btn" runat="server" CssClass="ButtonStyle" Text="Submit &raquo;" />
                                        <br />
                                    </asp:Panel>
                                    
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="puzzle_timer" EventName="Tick" />
                                    <asp:AsyncPostBackTrigger ControlID="restart_lBtn" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="submitScore_btn" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <br />
                            <span class="inner"><asp:LinkButton ID="restart_lBtn" runat="server" Text="Play Again" OnClick="resetPuzzle" CausesValidation="false" /></span>
                            
                        </asp:Panel>
                        
                        
                        <asp:DropShadowExtender ID="highScores_dsExt" runat="server" TargetControlID="viewScores_pnl" Opacity=".15" Width="4" TrackPosition="true" />
                        
                        <%--High Scores Modal Popup--%>
                        <asp:ModalPopupExtender ID="highScores_mpExt" runat="server" TargetControlID="dummy4" PopupControlID="viewScores_pnl" />
                        <input type="button" id="dummy4" runat="server" style="display: none;" />
                        
                        
                        <%--High score display panel--%>
                        <asp:Panel ID="viewScores_pnl" runat="server" CssClass="ModalStyle">
                            
                            <asp:UpdatePanel ID="viewScores_UpdatePnl" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table width="100%">
                                        <tr>
                                            <td align="center">
                                                <span class="GameOverStyle">High Scores</span>
                                                <br /><br />
                                                <asp:Gridview ID="scores_gv" runat="server" DataKeyNames="ID" AutoGenerateColumns="false" GridLines="Horizontal" AllowSorting="false" Width="510px">
                                                    <Columns>
                                                        <asp:BoundField DataField="Score" HeaderText="Score" ItemStyle-CssClass="GVScoreStyle" />
                                                        <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-CssClass="GVItemStyle" />
                                                        <asp:BoundField DataField="DifficultyLevel" HeaderText="Difficulty" ItemStyle-CssClass="GVItemStyle" />
                                                        <asp:BoundField DataField="BoardName" HeaderText="Puzzle" ItemStyle-CssClass="GVItemStyle" />
                                                        <asp:TemplateField HeaderText="Date" ItemStyle-CssClass="GVItemStyle">
                                                            <ItemTemplate>
                                                                <asp:Literal ID="date_lit" runat="server" Text='<%#formatDateText(eval("TheDate")) %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        
                                                    </Columns>
                                                    <HeaderStyle CssClass="GVHeaderStyle" />
                                                </asp:Gridview>
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                                
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="viewScores_btn" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="close_lbtn4" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="submitScore_btn" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                            
                            <span style="text-align: center; width:100%; display: block;"><asp:LinkButton ID="close_lbtn4" runat="server" Text="[Close]" CausesValidation="false" /></span>
                            
                            </asp:Panel>


                     </td>
                </tr>
                
                <tr>
                    <td colspan="2">
                        <span class="Divider"></span>
                    </td>
                </tr>
                
                <tr>
                    <td colspan="2" align="center">
                        <asp:UpdatePanel ID="board_updatePnl" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="board_pnl" runat="server"></asp:Panel>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="createBoard_btn" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="reset_btn" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="puzzle_timer" EventName="Tick" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <asp:Timer ID="puzzle_timer" runat="server" Interval="1000" Enabled="false" />
                    </td>
                </tr>

            </table>
        
        </div>
        
        <div class="Footer">
            Copyright &copy; 2011, <a href="mailto:maggy@zogglet.com?subject=About your awesome Jumbler game">Maggy Maffia</a>
        </div>
        
        
    </form>
</body>
</html>
