function VoteControlShowResults(uniqueName, asPercent)
{
    var question1Ctrl = document.getElementById(uniqueName + "Questions");
    var answer1aCtrl = document.getElementById(uniqueName + "AnswersWithPercent");
    var answer1bCtrl = document.getElementById(uniqueName + "AnswersWithCount");

    question1Ctrl.style.display = "none";

    if (asPercent == true)
    {
        answer1aCtrl.style.display = "";
        answer1bCtrl.style.display = "none";
    }
    else
    {
        answer1aCtrl.style.display = "none";
        answer1bCtrl.style.display = "";
    }
}

function VoteControlShowQuestion(uniqueName)
{
    var question1Ctrl = document.getElementById(uniqueName + "Questions");
    var answer1aCtrl = document.getElementById(uniqueName + "AnswersWithPercent");
    var answer1bCtrl = document.getElementById(uniqueName + "AnswersWithCount");

    question1Ctrl.style.display = "";
    answer1aCtrl.style.display = "none";
    answer1bCtrl.style.display = "none";
}

function VoteControlDoVote(basePath, uniqueName)
{
    var xmlhttp;
    if (window.XMLHttpRequest)
    {  // code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp=new XMLHttpRequest();
    }
    else
    {  // code for IE6, IE5
        xmlhttp=new ActiveXObject("Microsoft.XMLHTTP");
    }
    xmlhttp.onreadystatechange = function()
    {
        if (xmlhttp.readyState == 4 && xmlhttp.status == 200)
        {
            if (xmlhttp.responseText.substring(0, 1) == "*")
                alert(xmlhttp.responseText.substring(1));
            else
            {
                document.getElementById(uniqueName + "VoteControl").innerHTML = xmlhttp.responseText;
                VoteControlShowResults(uniqueName, true);
            }
        }
    }

    var voteIndex = getVoteIndex(uniqueName + "QuestionList");

    if (voteIndex >= 0)
    {
        xmlhttp.open("POST", basePath + "DoVote/" + uniqueName + '/' + voteIndex, true);
        xmlhttp.send();
    }
    else
        alert("Please select an item to vote for");
}

function getVoteIndex(controlName)
{
    var voteList = document.getElementById(controlName);
    var radioCount = 0;
    var voteIndex = -1;

    for (x = 0; x < voteList.childNodes.length; x++)
    {
        if (voteList.childNodes[x].childNodes.length > 0)
        {
            var radioBtn = voteList.childNodes[x].getElementsByTagName("input");
            if (radioBtn != null)
            {
                if (radioBtn[0].checked)
                {
                    voteIndex = radioCount;
                    break;
                }
                radioCount++;
            }
        }
    }
    return voteIndex;
}
