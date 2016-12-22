<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LunchSite.Models.VoteDataModel>" %>

<% if (false)
   { //Intellisense Includes (doesn't affect rendered page) %>
    <link href="../../Content/VoteControl.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/VoteControl.js" rel="script" type="text/javascript" />
<% } %>

<div id="<%= Model.UniqueName %>VoteControl" class="VoteControl" style="width:<%= Model.ControlWidth %>px;">
    <div class="VoteControlQuestion"><%= Model.Question %></div>
    <div class="VoteControlInnerBox">
        <div id="<%= Model.UniqueName %>Questions">
            <ul id="<%= Model.UniqueName %>QuestionList">
                <%  //This is the questions, which are radio buttons with labels
                    for (int rowIndex = 0; rowIndex < Model.Answers.Count; rowIndex++)
                    {
                %>
                <li class='<%= (rowIndex %2 == 0) ? "VoteControlNormalRow" : "VoteControlAlternateRow" %>'>
                    <input id="<%= Model.UniqueName %>Radio<%= rowIndex+1 %>" type="radio"
                           name="<%= Model.UniqueName %>Questions" class="VoteControlRadioButton" />
                    <label for="<%= Model.UniqueName %>Radio<%= rowIndex+1 %>">
                        <%= Model.Answers[rowIndex].Answer%>
                    </label>
                    <div class="VoteControlPollBarSpacer">&nbsp;</div>
                    <div style="clear: both"></div>
                </li>
                <%  }
                %>
            </ul>
            <div style="margin-top: 10px">
                <input type="button" value="Vote"
                       onclick='VoteControlDoVote("<%= Url.Content("~/") %>", "<%= Model.UniqueName %>");' />
                <a href="#" onclick="VoteControlShowResults('<%= Model.UniqueName %>', true);return false;"
                   style="float: right" class="VoteControlLinks">Skip to Results</a>
                <div style="clear: both"></div>
            </div>
        </div>
        <div id="<%= Model.UniqueName %>AnswersWithPercent" style="display:none">
            <ul>
                <%  //This is the answers with percentages, including a bar underneath with a length proportional to the percentage
                    for (int rowIndex = 0; rowIndex < Model.Answers.Count; rowIndex++)
                    {
                %>
                <li class='<%= (rowIndex %2 == 0) ? "VoteControlNormalRow" : "VoteControlAlternateRow" %>'>
                    <div class="VoteControlPollPercent"><%= Model.GetPercentage(rowIndex) %>%</div>
                    <label class="VoteControlPollLabel"><%= Model.Answers[rowIndex].Answer%></label>
                    <div style="width: <%= Model.GetBarLength(rowIndex,200) %>px" class="VoteControlPollBar">
                       &nbsp;</div>
                </li>
                <% } %>
            </ul>
            <div style="margin-top: 10px">
                <a href="#" onclick="VoteControlShowResults('<%= Model.UniqueName %>', false);return false;"
                   style="float: left" class="VoteControlLinks">Show Counts</a>
                <a href="#" onclick="VoteControlShowQuestion('<%= Model.UniqueName %>');return false;"
                   style="float: right" class="VoteControlLinks">Back to Voting</a>
                <div style="clear: both"></div>
            </div>
        </div>
        <div id="<%= Model.UniqueName %>AnswersWithCount" style="display:none">
            <ul>
                <%  //This is the answers with votes, including a bar underneath with a length proportional to the percentage
                    for (int rowIndex = 0; rowIndex < Model.Answers.Count; rowIndex++)
                    {
                %>
                <li class='<%= (rowIndex %2 == 0) ? "VoteControlNormalRow" : "VoteControlAlternateRow" %>'>
                    <div class="VoteControlPollCount">
                        <%= Model.Answers[rowIndex].Votes%><span style="font-size: xx-small"> votes</span>
                    </div>
                    <label class="VoteControlPollLabel"><%= Model.Answers[rowIndex].Answer%></label>
                    <div style="width: <%= Model.GetBarLength(rowIndex,200) %>px" class="VoteControlPollBar">
                       &nbsp;</div>
                </li>
                <% } %>
            </ul>
            <div style="margin-top: 10px">
                <a href="#" onclick="VoteControlShowResults('<%= Model.UniqueName %>', true);return false;"
                   style="float: left" class="VoteControlLinks">Percentages</a>
                <a href="#" onclick="VoteControlShowQuestion('<%= Model.UniqueName %>');return false;"
                   style="float: right" class="VoteControlLinks">Back to Voting</a>
                <div style="clear: both"></div>
            </div>
        </div>
    </div>
</div>
