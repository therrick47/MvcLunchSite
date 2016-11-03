using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Globalization;
using System.IO;

namespace LunchSite.Models
{
    public class AnswerItem
    {
        public string Answer { get; set; }
        public int Votes { get; set; }

        public AnswerItem(string answer, int votes)
        {
            Answer = answer;
            Votes = votes;
        }
    }

    public class IpAddressItem
    {
        public string Address { get; set; }
        public DateTime DateEntered { get; set; }

        public IpAddressItem(string address, DateTime dateEntered)
        {
            Address = address;
            DateEntered = dateEntered;
        }
    }

    public class VoteDataModel
    {
        private static object _syncLock = new object();
        private static int _totalVotesAllInstances = 0;     //votes from all users
        private int _totalVotesOnLoad;                      //total vote count when file was loaded
        private string _path;
        private bool _isDirty;  //true if file needs updating (because local model has changed)

        public string UniqueName { get; set; }
        public string Question { get; set; }
        public List<AnswerItem> Answers { get; set; }
        public List<IpAddressItem> IpAddresses { get; set; }
        public int ControlWidth { get; set; }

        public string ViewDataName { get { return GetViewDataName(UniqueName); } }

        public static string GetViewDataName(string uniqueName)
        {
            return uniqueName + "PollData";
        }

        public static string GetFilename(string basePath, string uniqueName)
        {
            return basePath + uniqueName + "Poll.xml";
        }

        public VoteDataModel(string name)
        {
            UniqueName = name;
            Answers = new List<AnswerItem>();
            IpAddresses = new List<IpAddressItem>();
        }

        public VoteDataModel(string name, string path) : this(name)
        {
            _path = path;
            _isDirty = false;
        }

        public bool Open()
        {
            bool retval;
            try
            {
                XDocument pollXml;
                lock (_syncLock)
                {
                    pollXml = XDocument.Load(GetFilename(_path, UniqueName));
                    _totalVotesOnLoad = _totalVotesAllInstances;
                }
                ParseXML(pollXml);
                retval = true;
            }
            catch (FileNotFoundException)
            {
                retval = false;
            }
            return retval;
        }

        public bool DoVote(int voteId, string ipAddr)
        {
            bool alreadyVoted = alreadyVoted = CheckIfAlreadyVotedToday(ipAddr);
            if (!alreadyVoted)
            {
                Answers[voteId].Votes++;
                IpAddresses.Add(new IpAddressItem(ipAddr, DateTime.Now));
                Save();
            }
            else
            {
                SaveIfModified();
            }
            return (!alreadyVoted);
        }

        public int GetPercentage(int index)
        {
            int totalVotes = 0;
            foreach (var item in Answers)
                totalVotes += item.Votes;
            if (totalVotes > 0)
                return Answers[index].Votes * 100 / totalVotes;
            else
                return 0;
        }

        public int GetBarLength(int index, int controlWidth)
        {
            int percentage = GetPercentage(index);
            return percentage * controlWidth / 100;
        }

        private void ParseXML(XDocument pollXml)
        {
            Answers.Clear();
            IpAddresses.Clear();

            Question = pollXml.Element("poll").Element("question").Attribute("text").Value;
            foreach (var answer in pollXml.Element("poll").Elements("answer"))
                Answers.Add(new AnswerItem(answer.Attribute("text").Value, int.Parse(answer.Value)));
            foreach (var ipaddr in pollXml.Element("poll").Elements("ipaddr"))
                IpAddresses.Add(new IpAddressItem(ipaddr.Value,
                                                  DateTime.ParseExact(ipaddr.Attribute("dateEntered").Value,
                                                                      "yyyy/MM/dd HH:mm",
                                                                      CultureInfo.InvariantCulture)));
        }

        private bool CheckIfAlreadyVotedToday(string ipAddr)
        {
            bool found = false;
            for (int i = IpAddresses.Count - 1; i >= 0; i--)
            {
                if (DateTime.Now - IpAddresses[i].DateEntered > new TimeSpan(24, 0, 0))
                {
                    IpAddresses.RemoveAt(i);        //Remove expired entries (entries older than 1 day)
                    _isDirty = true;
                }
                else
                {
                    if (IpAddresses[i].Address == ipAddr)
                    {
                        found = true;
                        break;
                    }
                }
            }
            return found;
        }

        private void SaveIfModified()
        {
            if (_isDirty)
            {
                Save();
                _isDirty = false;
            }
        }

        private void Save()
        {
            lock (_syncLock)
            {
                XDocument pollXml;
                if (_totalVotesOnLoad != _totalVotesAllInstances)
                {
                    //Someone else has voted - so we need to reload the file
                    pollXml = XDocument.Load(GetFilename(_path, UniqueName));
                    ParseXML(pollXml);
                }
                XElement root = new XElement("poll");
                XElement questXml = new XElement("question");
                questXml.SetAttributeValue("text", Question);
                root.Add(questXml);
                foreach (var answer in Answers)
                {
                    XElement ansXml = new XElement("answer", answer.Votes);
                    ansXml.SetAttributeValue("text", answer.Answer);
                    root.Add(ansXml);
                }
                foreach (var ipAddr in IpAddresses)
                {
                    XElement ipAddrXml = new XElement("ipaddr", ipAddr.Address);
                    ipAddrXml.SetAttributeValue("dateEntered", ipAddr.DateEntered.ToString("yyyy/MM/dd HH:mm"));
                    root.Add(ipAddrXml);
                }
                pollXml = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"));
                pollXml.Add(root);
                pollXml.Save(GetFilename(_path, UniqueName));
                _totalVotesAllInstances++;
            }
        }
    }
}
